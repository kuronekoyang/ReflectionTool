using System;
using System.Linq;
using System.Reflection;
using kuro.Core;

namespace kuro.ReflectionTool
{
    public partial class ReflectionTool
    {
        public class GenWrapTool : GenToolBase
        {
            protected override GenInfoBase[] GenInfoList => WrapInfoList;

            public class GenWrapJob : GenJobBase
            {
                public GenWrapJob(GenToolBase genTool, MemberCollection collection) : base(genTool, collection)
                {
                    FileName = $"Wrap_{TypeFlatName}.cs";
                }

                private static bool CanUseDelegate(MethodInfo methodInfo)
                {
                    if (methodInfo == null)
                        return false;
                    if (!methodInfo.IsStatic)
                        return false;
                    if (methodInfo.GetParameters().Any(x => !x.ParameterType.IfPublic()))
                        return false;
                    if (methodInfo.ReturnType != typeof(void) && !methodInfo.ReturnType.IfPublic())
                        return false;
                    return true;
                }

                private static bool CanUseEmit(MemberInfo memberInfo)
                {
                    if (memberInfo == null)
                        return false;
                    if (!memberInfo.GetMemberType().IfPublic())
                        return false;
                    if (memberInfo.IfConst())
                        return false;
                    return true;
                }

                void DoType()
                {
                    Builder.AppendLine();
                    Builder.Comment($"<see cref=\"{TypeFullName}\"/>");
                    Builder.AppendLine($"public static Type {KType} {{ get; }} = {nameof(ReflectionUtils)}.{nameof(ReflectionUtils.GetTypeByFullName)}(\"{AssemblyShortName}\", \"{TypeFullName}\");");
                }

                void DoFieldElementTypeReflection(Member m)
                {
                    var memberType = m.MemberInfo.GetMemberType();
                    var memberTypeName = GetTypeName(memberType);

                    switch (memberTypeName.Mode)
                    {
                        case TypeNameMode.Array:
                        case TypeNameMode.List:
                        {
                            if (memberType.GetArrayOrListElementType(out var elementType))
                            {
                                Builder.AppendLine();
                                Builder.AppendLine($"private static Type _{KElementPrefix}{KPrefix}{m.UniqueName};");
                                Builder.BeginLine($"public static Type {KElementPrefix}{KPrefix}{m.UniqueName}");
                                Builder.EndLine(
                                    $" => _{KElementPrefix}{KPrefix}{m.UniqueName} ??= {nameof(ReflectionUtils)}.{nameof(ReflectionUtils.GetTypeByFullName)}(\"{elementType.Assembly.GetName().Name}\", \"{elementType.FullName}\");");
                            }

                            break;
                        }
                    }
                }

                void DoFieldReflection()
                {
                    foreach (var m in Collection.Fields)
                    {
                        Builder.AppendLine();
                        if (m.MemberInfo.DeclaringType?.IsEnum ?? false)
                        {
                            Builder.AppendLine($"private static object _{KPrefix}{m.UniqueName};");
                            Builder.BeginLine($"private static object {KPrefix}{m.UniqueName} => ");
                            Builder.Append($"_{KPrefix}{m.UniqueName} ??= {KType}.{nameof(ReflectionUtils.QF)}(\"{m.MemberOriginName}\").GetValue(null)");
                            Builder.EndLine(";");
                        }
                        else if (CanUseEmit(m.MemberInfo))
                        {
                            var propertyTypeName = GetTypeName(m.MemberInfo.GetMemberType());
                            var strAccess = "";
                            var hasPointer = propertyTypeName.HasPointer;
                            if (hasPointer)
                                strAccess += "unsafe ";

                            {
                                var name = $"{KGetPrefix}{KPrefix}{m.UniqueName}";
                                var delegateTypeName = $"Func<object, {propertyTypeName.Name}>";
                                Builder.AppendLine($"private static {strAccess}{delegateTypeName} _{name};");
                                Builder.BeginLine($"private static {strAccess}{delegateTypeName} {name} => _{name} ??= ");
                                Builder.Append($"({KType}.{nameof(ReflectionUtils.QF)}(\"{m.MemberOriginName}\"))");
                                Builder.Append($".{nameof(ILUtils.ILGet)}<{propertyTypeName.Name}>()");
                                Builder.EndLine(";");
                            }

                            {
                                var name = $"{KSetPrefix}{KPrefix}{m.UniqueName}";
                                var delegateTypeName = $"Action<object, {propertyTypeName.Name}>";
                                Builder.AppendLine($"private static {strAccess}{delegateTypeName} _{name};");
                                Builder.BeginLine($"private static {strAccess}{delegateTypeName} {name} => _{name} ??= ");
                                Builder.Append($"({KType}.{nameof(ReflectionUtils.QF)}(\"{m.MemberOriginName}\"))");
                                Builder.Append($".{nameof(ILUtils.ILSet)}<{propertyTypeName.Name}>()");
                                Builder.EndLine(";");
                            }
                        }
                        else
                        {
                            Builder.AppendLine($"private static FieldInfo _{KPrefix}{m.UniqueName};");
                            Builder.BeginLine($"private static FieldInfo {KPrefix}{m.UniqueName}");
                            Builder.EndLine($" => _{KPrefix}{m.UniqueName} ??= {KType}?.{nameof(ReflectionUtils.QF)}(\"{m.MemberOriginName}\");");
                        }

                        DoFieldElementTypeReflection(m);
                    }

                    foreach (var m in Collection.Properties)
                    {
                        Builder.AppendLine();

                        if (CanUseEmit(m.PropertyInfo))
                        {
                            var propertyTypeName = GetTypeName(m.MemberInfo.GetMemberType());
                            var strAccess = "";
                            var hasPointer = propertyTypeName.HasPointer;
                            if (hasPointer)
                                strAccess += "unsafe ";

                            if (m.MemberInfo.IfCanRead())
                            {
                                var name = $"{KGetPrefix}{KPrefix}{m.UniqueName}";
                                var delegateTypeName = $"Func<object, {propertyTypeName.Name}>";
                                Builder.AppendLine($"private static {strAccess}{delegateTypeName} _{name};");
                                Builder.BeginLine($"private static {strAccess}{delegateTypeName} {name} => _{name} ??= ");
                                Builder.Append($"({KType}.{nameof(ReflectionUtils.QP)}(\"{m.MemberOriginName}\"))");
                                Builder.Append($".{nameof(ILUtils.ILGet)}<{propertyTypeName.Name}>()");
                                Builder.EndLine(";");
                            }

                            if (m.MemberInfo.IfCanWrite())
                            {
                                var name = $"{KSetPrefix}{KPrefix}{m.UniqueName}";
                                var delegateTypeName = $"Action<object, {propertyTypeName.Name}>";
                                Builder.AppendLine($"private static {strAccess}{delegateTypeName} _{name};");
                                Builder.BeginLine($"private static {strAccess}{delegateTypeName} {name} => _{name} ??= ");
                                Builder.Append($"({KType}.{nameof(ReflectionUtils.QP)}(\"{m.MemberOriginName}\"))");
                                Builder.Append($".{nameof(ILUtils.ILSet)}<{propertyTypeName.Name}>()");
                                Builder.EndLine(";");
                            }
                        }
                        else
                        {
                            Builder.AppendLine($"private static PropertyInfo _{KPrefix}{m.UniqueName};");
                            Builder.BeginLine($"private static PropertyInfo {KPrefix}{m.UniqueName}");
                            Builder.EndLine($" => _{KPrefix}{m.UniqueName} ??= {KType}?.{nameof(ReflectionUtils.QP)}(\"{m.MemberOriginName}\");");
                        }

                        DoFieldElementTypeReflection(m);
                    }
                }

                void DoMethodReflection()
                {
                    foreach (var m in Collection.Methods)
                    {
                        var parameters = m.MethodInfo.GetParameters();
                        string getMethodInfoStr = $"{KType}.{nameof(ReflectionUtils.QM)}";
                        if (parameters.Length == 0)
                            getMethodInfoStr += $"(\"{m.RawFlatName}\")";
                        else
                            getMethodInfoStr += $"(\"{m.RawFlatName}\", {string.Join(", ", parameters.Select(p => $"\"{p.ParameterType.FullName}\""))})";

                        Builder.AppendLine();
                        if (CanUseDelegate(m.MethodInfo))
                        {
                            var parameterTypeNames = parameters.Select(GetTypeName).ToArray();
                            var strParamDecl = string.Join(", ", parameterTypeNames.Select(x => $"{x.ParameterModifier}{x.TypeName.Name} {x.Parameter.Name}"));
                            var returnTypeName = GetTypeName(m.MethodInfo.ReturnType);
                            var strAccess = "";
                            var hasPointer = returnTypeName.HasPointer || parameterTypeNames.Any(x => x.TypeName.HasPointer);
                            if (hasPointer)
                                strAccess += "unsafe ";

                            var delegateTypeName = $"{KDelegatePrefix}{KPrefix}{m.UniqueName}";
                            Builder.AppendLine($"private {strAccess}delegate {returnTypeName.Name} {delegateTypeName}({strParamDecl});");
                            Builder.AppendLine($"private static {delegateTypeName} _{KPrefix}{m.UniqueName};");
                            Builder.BeginLine($"private static {delegateTypeName} {KPrefix}{m.UniqueName}");
                            Builder.EndLine($" => _{KPrefix}{m.UniqueName} ??= {getMethodInfoStr}?.{nameof(ReflectionUtils.CreateDelegate)}<{delegateTypeName}>();");
                        }
                        else
                        {
                            Builder.AppendLine($"private static MethodInfo _{KPrefix}{m.UniqueName};");
                            Builder.BeginLine($"private static MethodInfo {KPrefix}{m.UniqueName}");
                            Builder.EndLine($" => _{KPrefix}{m.UniqueName} ??= {getMethodInfoStr};");
                        }
                    }
                }

                void DoCtor()
                {
                    if (TypeIsStatic)
                        return;
                    if (TypeIsUnityObject)
                        return;
                    if (Type.IsAbstract || Type.IsInterface || Type.IsEnum)
                        return;

                    foreach (var m in Collection.Constructors)
                    {
                        var methodInfo = m.ConstructorInfo;
                        if (methodInfo.IfStatic())
                            continue;
                        if (methodInfo.IfPublic())
                            continue;

                        var strGenericDecl = methodInfo.IsGenericMethod ? $"<{string.Join(", ", methodInfo.GetGenericArguments().Select(x => x.Name))}>" : "";
                        var parameters = methodInfo.GetParameters();

                        var parameterTypeNames = parameters.Select(GetTypeName).ToArray();
                        var strParamDecl = string.Join(", ", parameterTypeNames.Select(x => $"{x.ParameterModifier}{x.TypeName.Name} {x.Parameter.Name}"));

                        Builder.AppendLine();
                        Builder.AppendLine($"public static {TypeFlatName} {KNew}{strGenericDecl}({strParamDecl})");
                        Builder.AppendLine("{");
                        using (Builder.Indent())
                        {
                            if (parameterTypeNames.Length > 0)
                            {
                                Builder.AppendLine($"var {KPool} = {nameof(FixedArrayPool)}<object>.{nameof(FixedArrayPool<object>.Shared)}({parameters.Length});");
                                Builder.AppendLine($"var {KParams} = {KPool}.{nameof(FixedArrayPool<object>.Rent)}();");
                                var args = parameterTypeNames.Select(x =>
                                {
                                    if (x.TypeName.Mode == TypeNameMode.Export)
                                        return $"{x.Parameter.Name}.{KSelf}";
                                    else
                                        return x.Parameter.Name;
                                }).ToArray();
                                for (int i = 0; i < args.Length; ++i)
                                    Builder.AppendLine($"{KParams}[{i}] = {args[i]};");
                                Builder.AppendLine($@"var {KResult} = Activator.CreateInstance({KType}, {KParams});");
                                Builder.AppendLine($"{KPool}.Return({KParams});");
                                Builder.AppendLine($"return new({KResult});");
                            }
                            else
                            {
                                Builder.AppendLine($"return new(Activator.CreateInstance({KType}));");
                            }
                        }

                        Builder.AppendLine("}");
                    }
                }

                void DoEnum()
                {
                    if (!Type.IsEnum)
                        return;

                    Builder.AppendLine();
                    Builder.AppendLine($"public int {KInt}() => {KValid} ? Convert.ToInt32({KSelf}) : 0;");
                    Builder.AppendLine($"public static implicit operator int({TypeFlatName} self) => self.{KInt}();");
                    Builder.AppendLine($"public static {TypeFlatName} {KInt}(int value) => new(Enum.ToObject({KType}, value));");
                    Builder.AppendLine($"public static implicit operator {TypeFlatName}(int value) => {KInt}(value);");

                    Builder.AppendLine();
                    Builder.AppendLine($"public static bool operator == ({TypeFlatName} a, {TypeFlatName} b) => a.__int__() == b.__int__();");
                    Builder.AppendLine($"public static bool operator != ({TypeFlatName} a, {TypeFlatName} b) => a.__int__() != b.__int__();");
                    Builder.AppendLine(
                        $"public override bool Equals(object obj) => (obj is {TypeFlatName} other && this == other) || (obj is Enum && obj.GetType() == __type__ && object.Equals(__self__, obj));");
                    Builder.AppendLine($"public override int GetHashCode() => __valid__ ? __int__() : 0;");

                    if (Type.GetCustomAttribute<FlagsAttribute>() != null)
                    {
                        Builder.AppendLine();
                        Builder.AppendLine($"public static {TypeFlatName} operator & ({TypeFlatName} a, {TypeFlatName} b) => __int__(a.__int__() & b.__int__());");
                        Builder.AppendLine($"public static {TypeFlatName} operator | ({TypeFlatName} a, {TypeFlatName} b) => __int__(a.__int__() | b.__int__());");
                        Builder.AppendLine($"public static {TypeFlatName} operator ^ ({TypeFlatName} a, {TypeFlatName} b) => __int__(a.__int__() ^ b.__int__());");
                        Builder.AppendLine($"public static {TypeFlatName} operator ~ ({TypeFlatName} a) => __int__(~a.__int__());");
                        Builder.AppendLine($"public static {TypeFlatName} operator << ({TypeFlatName} a, int b) => __int__(a.__int__() << b);");
                        Builder.AppendLine($"public static {TypeFlatName} operator >> ({TypeFlatName} a, int b) => __int__(a.__int__() >> b);");
                    }
                }

                void DoCtorEx()
                {
                    if (TypeIsStatic)
                        return;

                    Builder.AppendLine();
                    Builder.AppendLine($"public {TypeFlatName}(object {KSelf}) => this.{KSelf} = {KSelf} as {ObjectTypeName};");
                    Builder.AppendLine($"public {ObjectTypeName} {KSelf};");
                    Builder.BeginLine($"public bool {KValid}");
                    Builder.EndLine($" => {KSelf} != null && {KType} != null;");

                    var superType = GetSuperType(Type);
                    if (superType != null)
                    {
                        var superTypeName = GetTypeName(superType);
                        Builder.BeginLine($"public {superTypeName.Name} {KSuper}");
                        Builder.EndLine($" => {superTypeName.Cast(KSelf)};");
                    }

                    var baseType = GetBaseType(Type);
                    if (baseType != null)
                    {
                        var baseTypeName = GetTypeName(baseType);
                        Builder.BeginLine($"public {baseTypeName.Name} {KBase}");
                        Builder.EndLine($" => {baseTypeName.Cast(KSelf)};");
                    }
                }

                void DoField()
                {
                    foreach (var m in Collection.FieldOrProperties)
                    {
                        var memberInfo = m.MemberInfo;
                        var isStatic = memberInfo.IfStatic();
                        var canRead = memberInfo.IfCanRead();
                        var canWrite = memberInfo.IfCanWrite();
                        var memberType = memberInfo.GetMemberType();
                        var memberTypeName = GetTypeName(memberType);

                        var strAccess = isStatic ? " static " : " ";
                        var strType = $"{KPrefix}{m.UniqueName}.{(m.FieldInfo != null ? "FieldType" : "PropertyType")}";
                        var strObj = isStatic ? "null" : KSelf;
                        var strMember = m.RawFlatName;
                        Builder.AppendLine();
                        Builder.BeginLine($"public{strAccess}{memberTypeName.Name} {strMember}");
                        if (canRead && !canWrite)
                        {
                            if (m.MemberInfo.DeclaringType?.IsEnum ?? false)
                                Builder.EndLine($" => {memberTypeName.Cast($"{KPrefix}{m.UniqueName}")};");
                            else if (CanUseEmit(m.MemberInfo))
                                Builder.EndLine($" => {KGetPrefix}{KPrefix}{m.UniqueName}({strObj});");
                            else
                                Builder.EndLine($" => {memberTypeName.Cast($"{KPrefix}{m.UniqueName}.GetValue({strObj})")};");
                        }
                        else
                        {
                            Builder.EndLine();
                            Builder.AppendLine("{");
                            using (Builder.Indent())
                            {
                                if (canRead)
                                {
                                    if (CanUseEmit(m.MemberInfo))
                                        Builder.AppendLine($"get => {KGetPrefix}{KPrefix}{m.UniqueName}({strObj});");
                                    else
                                        Builder.AppendLine($"get => {memberTypeName.Cast($"{KPrefix}{m.UniqueName}.GetValue({strObj})")};");
                                }

                                if (canWrite)
                                {
                                    if (CanUseEmit(m.MemberInfo))
                                        Builder.AppendLine($"set => {KSetPrefix}{KPrefix}{m.UniqueName}({strObj}, value);");
                                    else
                                        Builder.AppendLine($"set => {KPrefix}{m.UniqueName}.SetValue({strObj}, {memberTypeName.CastTo("value", strType)});");
                                }
                            }

                            Builder.AppendLine("}");
                        }

                        if (canRead)
                        {
                            switch (memberTypeName.Mode)
                            {
                                case TypeNameMode.Array:
                                {
                                    var elementType = memberType.GetElementType();
                                    var elementTypeName = GetTypeName(elementType);
                                    Builder.AppendLine();
                                    Builder.AppendLine(
                                        $"public{strAccess}{elementTypeName.Name} {strMember}{KPrefix}GetItem(int i) => {elementTypeName.Cast($"{strMember}?.GetValue(i)")};");
                                    Builder.AppendLine(
                                        $"public{strAccess}void {strMember}{KPrefix}SetItem(int i, {elementTypeName.Name} value) => {strMember}?.SetValue({elementTypeName.CastTo("value", $"{KElementPrefix}{KPrefix}{m.UniqueName}")}, i);");
                                    break;
                                }
                                case TypeNameMode.List:
                                {
                                    if (memberType.GetArrayOrListElementType(out var elementType))
                                    {
                                        var elementTypeName = GetTypeName(elementType);
                                        Builder.AppendLine();
                                        Builder.AppendLine(
                                            $"public{strAccess}{elementTypeName.Name} {strMember}{KPrefix}GetItem(int i) => {elementTypeName.Cast($"{strMember}?[i]")};");
                                        Builder.AppendLine($"public{strAccess}void {strMember}{KPrefix}SetItem(int i, {elementTypeName.Name} value)");
                                        Builder.AppendLine("{");
                                        using (Builder.Indent())
                                        {
                                            Builder.AppendLine($"var __list__ = {strMember};");
                                            Builder.AppendLine($"if (__list__ == null) return;");
                                            Builder.AppendLine($"__list__[i] = {elementTypeName.CastTo("value", $"{KElementPrefix}{KPrefix}{m.UniqueName}")};");
                                        }

                                        Builder.AppendLine("}");
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }

                void DoMethod()
                {
                    foreach (var m in Collection.Methods)
                    {
                        var methodInfo = m.MethodInfo;
                        var isStatic = methodInfo.IfStatic();
                        var strAccess = isStatic ? " static " : " ";
                        var strObj = isStatic ? "null" : KSelf;
                        var strMethod = m.RawFlatName;
                        if (strMethod == "Finalize")
                            strMethod = "__Finalize__";
                        var strGenericDecl = methodInfo.IsGenericMethod ? $"<{string.Join(", ", methodInfo.GetGenericArguments().Select(x => x.Name))}>" : "";
                        var parameters = methodInfo.GetParameters();
                        var parameterTypeNames = parameters.Select(GetTypeName).ToArray();
                        var strParamDecl = string.Join(", ", parameterTypeNames.Select(x => $"{x.ParameterModifier}{x.TypeName.Name} {x.Parameter.Name}"));
                        var returnTypeName = GetTypeName(methodInfo.ReturnType);

                        if (!isStatic && (methodInfo.Name == "Equals" || methodInfo.Name == "GetHashCode" || methodInfo.Name == "ToString"))
                            strAccess += "override ";

                        var hasPointer = returnTypeName.HasPointer || parameterTypeNames.Any(x => x.TypeName.HasPointer);
                        if (hasPointer)
                            strAccess += "unsafe ";

                        Builder.AppendLine();
                        Builder.BeginLine($"public{strAccess}{returnTypeName.Name} {strMethod}{strGenericDecl}({strParamDecl})");
                        if (CanUseDelegate(methodInfo))
                        {
                            Builder.Append(" => ");
                            Builder.Append($"{KPrefix}{m.UniqueName}(");
                            Builder.Append(string.Join(", ", parameterTypeNames.Select(x => $"{x.ParameterModifier}{x.Parameter.Name}")));
                            Builder.EndLine(");");
                        }
                        else
                        {
                            Builder.EndLine();
                            Builder.AppendLine("{");
                            using (Builder.Indent())
                            {
                                string strParam;
                                if (parameterTypeNames.Length > 0)
                                {
                                    strParam = KParams;

                                    Builder.AppendLine($"var {KPool} = {nameof(FixedArrayPool)}<object>.{nameof(FixedArrayPool<object>.Shared)}({parameters.Length});");
                                    Builder.AppendLine($"var {KParams} = {KPool}.{nameof(FixedArrayPool<object>.Rent)}();");
                                    for (var i = 0; i < parameterTypeNames.Length; i++)
                                    {
                                        var p = parameterTypeNames[i];

                                        if (p.Parameter.ParameterType.IsPointer)
                                        {
                                            Builder.AppendLine($"{KParams}[{i}] = new IntPtr({p.GetValue(p.Parameter.Name)});");
                                        }
                                        else if (p.RefMode == RefMode.Out)
                                        {
                                            var flags = methodInfo.MethodImplementationFlags;
                                            if ((flags & MethodImplAttributes.InternalCall) != 0 ||
                                                (flags & MethodImplAttributes.Native) != 0 ||
                                                (flags & MethodImplAttributes.PreserveSig) != 0)
                                            {
                                                if (p.TypeName.Mode == TypeNameMode.Export)
                                                    Builder.AppendLine($"{KParams}[{i}] = Activator.CreateInstance({p.TypeName.Name}.{KType});");
                                                else
                                                    Builder.AppendLine($"{KParams}[{i}] = Activator.CreateInstance(typeof({p.TypeName.Name}));");
                                            }
                                            else
                                            {
                                                Builder.AppendLine($"{KParams}[{i}] = null;");
                                            }
                                        }
                                        else
                                        {
                                            Builder.AppendLine($"{KParams}[{i}] = {p.GetValue(p.Parameter.Name)};");
                                        }
                                    }
                                }
                                else
                                {
                                    strParam = "Array.Empty<object>()";
                                }

                                if (returnTypeName.Mode == TypeNameMode.Void)
                                    Builder.AppendLine($"{KPrefix}{m.UniqueName}?.Invoke({strObj}, {strParam});");
                                else
                                    Builder.AppendLine($"var {KResult} = {KPrefix}{m.UniqueName}?.Invoke({strObj}, {strParam});");

                                if (parameterTypeNames.Length > 0)
                                {
                                    for (var i = 0; i < parameterTypeNames.Length; i++)
                                    {
                                        var p = parameterTypeNames[i];
                                        if (p.RefMode == RefMode.Out || p.RefMode == RefMode.Ref)
                                            Builder.AppendLine($"{p.Parameter.Name} = {p.Cast($"{KParams}[{i}]")};");
                                    }

                                    Builder.AppendLine($"{KPool}.{nameof(FixedArrayPool<object>.Return)}({KParams});");
                                }

                                if (returnTypeName.Mode != TypeNameMode.Void)
                                    Builder.AppendLine($"return {KResult} != null ? {returnTypeName.Cast($"{KResult}")} : default;");
                            }

                            Builder.AppendLine("}");
                        }
                    }
                }

                void DoDelegate()
                {
                    Builder.AppendLine();
                    foreach (var m in Collection.Delegates)
                    {
                        var delegateInfo = m.DelegateInfo;
                        var methodInfo = delegateInfo.GetMethod("Invoke");
                        var returnTypeName = GetTypeName(methodInfo.ReturnType);
                        var parameters = methodInfo.GetParameters();
                        var parameterTypeNames = parameters.Select(GetTypeName).ToArray();

                        Builder.AppendLine(
                            $"public delegate {returnTypeName.Name} {m.RawFlatName}({string.Join(", ", parameterTypeNames.Select(x => $"{x.ParameterModifier}{x.TypeName.Name} {x.Parameter.Name}"))});");
                    }
                }

                public override void Execute()
                {
                    Builder.AppendLine("// This file was automatically generated by kuroneko.");
                    Builder.AppendLine("// ReSharper disable InconsistentNaming");
                    Builder.AppendLine("using System;");
                    Builder.AppendLine("using System.Reflection;");
                    Builder.AppendLine("using System.Runtime.CompilerServices;");
                    Builder.AppendLine($"using {typeof(ReflectionUtils).Namespace};");
                    Builder.AppendLine();
                    Builder.AppendLine($"namespace {typeof(ReflectionTool).Namespace}");
                    Builder.AppendLine("{");
                    using (Builder.Indent())
                    {
                        Builder.AppendLine($"public partial struct {TypeFlatName}{TypeGenericSuffix}");
                        Builder.AppendLine("{");
                        using (Builder.Indent())
                        {
                            DoType();
                            DoDelegate();
                            DoField();
                            DoMethod();
                            DoCtorEx();
                            DoCtor();
                            DoEnum();
                            DoFieldReflection();
                            DoMethodReflection();
                        }

                        Builder.AppendLine("}");

                        if (Type.IfPublic() && !TypeIsStatic)
                        {
                            Builder.AppendLine($"public static class {TypeFlatName}_Extension");
                            Builder.AppendLine("{");
                            using (Builder.Indent())
                            {
                                Builder.AppendLine(
                                    $"public static {TypeFlatName}{TypeGenericSuffix} ReflectionHelper{TypeGenericSuffix}(this {TypeDisplayName} self) => new(self);");
                            }

                            Builder.AppendLine("}");
                        }
                    }

                    Builder.AppendLine("}");

                    Save();
                }
            }

            protected override GenJobBase CreateJob(MemberCollection collection) => new GenWrapJob(this, collection);
        }
    }
}