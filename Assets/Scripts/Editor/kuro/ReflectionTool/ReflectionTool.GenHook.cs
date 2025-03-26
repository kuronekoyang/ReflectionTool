using System.Linq;
using kuro.Core;

namespace kuro.ReflectionTool
{
    public partial class ReflectionTool
    {
        public class GenHookTool : GenToolBase
        {
            protected override GenInfoBase[] GenInfoList => HookInfoList;

            public class GenHookJob : GenJobBase
            {
                public GenHookJob(GenToolBase genTool, MemberCollection collection) : base(genTool, collection)
                {
                    this.FileName = $"Hook_{this.TypeFlatName}.cs";
                }

                void DoMethodImpl(Member m, MonoHookType hookType, bool isStatic, TypeName returnTypeName, ParameterTypeName[] parameterTypeNames)
                {
                    var strAccess = isStatic ? "static " : "";
                    Builder.AppendLine();
                    Builder.AppendLine($"[{nameof(MonoHookAttribute)}({nameof(MonoHookType)}.{hookType}, {(m.WaitForGUISkin ? "true" : "false")})]");
                    Builder.AppendLine($"public class {m.UniqueName}");
                    Builder.AppendLine("{");
                    using (Builder.Indent())
                    {
                        var parameterDecl = string.Join(", ", parameterTypeNames.Select(x => $"{x.ParameterModifier}{x.TypeName.Name} {x.Parameter.Name}"));
                        var originalParameterDecl = string.Join(", ", parameterTypeNames.Select(x =>
                        {
                            if (x.TypeName.Mode == TypeNameMode.Public || x.TypeName.Mode == TypeNameMode.Void)
                                return $"{x.ParameterModifier}{x.TypeName.Name} {x.Parameter.Name}";
                            return $"[{nameof(WildCardParameterAttribute)}] {x.ParameterModifier}object {x.Parameter.Name}";
                        }));

                        var returnDecl = returnTypeName.Name;
                        var originalReturnDecl = (returnTypeName.Mode == TypeNameMode.Public || returnTypeName.Mode == TypeNameMode.Void) ? returnTypeName.Name : "object";

                        Builder.AppendLine($"public static string {KAssembly} = \"{AssemblyShortName}\";");
                        Builder.Comment($"<see cref=\"{TypeFullName}\"/>");
                        Builder.AppendLine($"public static string {KType} = \"{TypeFullName}\";");
                        Builder.AppendLine($"public delegate {returnDecl} {KDelegate}({parameterDecl});");

                        Builder.AppendLine();
                        Builder.AppendLine($"public struct Param");
                        Builder.AppendLine("{");
                        using (Builder.Indent())
                        {
                            foreach (var p in parameterTypeNames)
                            {
                                Builder.AppendLine($"public {p.TypeName.Name} {p.Parameter.Name};");
                            }

                            Builder.AppendLine();
                            if (!isStatic)
                            {
                                var typeName = GetTypeName(Type);
                                if (typeName.Mode == TypeNameMode.Export && HasWrapExporter(Type))
                                    Builder.AppendLine($"public {typeName.Name} {KThis};");
                                else if (!Type.IsValueType && Type.IfPublic())
                                    Builder.AppendLine($"public {typeName.Name} {KThis};");
                                else
                                    Builder.AppendLine($"public object {KThis};");
                            }

                            if (returnTypeName.Mode != TypeNameMode.Void)
                                Builder.AppendLine($"public {returnDecl} {KResult};");
                            Builder.AppendLine($"public {KDelegate} {KOriginal};");
                            Builder.AppendLine($"public object {KUserdata};");
                        }

                        Builder.AppendLine("}");

                        Builder.AppendLine();
                        Builder.AppendLine($"public delegate bool BeginDelegate(ref Param param);");
                        Builder.AppendLine($"public delegate void EndDelegate(ref Param param);");
                        Builder.AppendLine($"public static BeginDelegate OnBegin = null;");
                        Builder.AppendLine($"public static EndDelegate OnEnd = null;");

                        Builder.AppendLine();
                        Builder.AppendLine($"[MethodImpl(MethodImplOptions.NoOptimization)]");
                        Builder.AppendLine($"public {strAccess}{originalReturnDecl} {KReplace}({originalParameterDecl})");
                        Builder.AppendLine("{");
                        using (Builder.Indent())
                        {
                            foreach (var p in parameterTypeNames)
                            {
                                if (p.RefMode == RefMode.Out)
                                    Builder.AppendLine($"{p.Parameter.Name} = default;");
                            }

                            Builder.AppendLine($"Param param = new();");

                            foreach (var p in parameterTypeNames)
                            {
                                if (p.TypeName.Mode == TypeNameMode.Export)
                                    Builder.AppendLine($"param.{p.Parameter.Name} = new({p.Parameter.Name});");
                                else
                                    Builder.AppendLine($"param.{p.Parameter.Name} = {p.Parameter.Name};");
                            }

                            if (!isStatic)
                            {
                                var typeName = GetTypeName(Type);
                                if (typeName.Mode == TypeNameMode.Export && HasWrapExporter(Type))
                                    Builder.AppendLine($"param.{KThis} = new(this);");
                                else if (!Type.IsValueType && Type.IfPublic())
                                    Builder.AppendLine($"param.{KThis} = ((object)this) as {typeName.Name};");
                                else
                                    Builder.AppendLine($"param.{KThis} = this;");
                            }

                            if (returnTypeName.Mode != TypeNameMode.Void)
                                Builder.AppendLine($"param.{KResult} = default;");

                            Builder.AppendLine($"param.{KOriginal} = {KOriginalWrap};");

                            Builder.AppendLine();
                            Builder.AppendLine($"if (OnBegin?.Invoke(ref param) == true)");
                            Builder.AppendLine("{");
                            using (Builder.Indent())
                            {
                                foreach (var p in parameterTypeNames)
                                {
                                    if (p.RefMode == RefMode.Out || p.RefMode == RefMode.Ref)
                                        Builder.AppendLine($"{p.Parameter.Name} = param.{p.Parameter.Name};");
                                }

                                if (returnTypeName.Mode != TypeNameMode.Void)
                                    Builder.AppendLine($"return param.{KResult};");
                                else
                                    Builder.AppendLine($"return;");
                            }

                            Builder.AppendLine("}");

                            Builder.AppendLine();
                            Builder.AppendLine(
                                $"{(returnTypeName.Mode != TypeNameMode.Void ? $"param.{KResult} = " : "")}{KOriginalWrap}({string.Join(", ", parameterTypeNames.Select(x => $"{x.ParameterModifier}param.{x.Parameter.Name}"))});");
                            Builder.AppendLine();
                            Builder.AppendLine($"OnEnd?.Invoke(ref param);");

                            foreach (var p in parameterTypeNames)
                            {
                                if (p.RefMode == RefMode.Out || p.RefMode == RefMode.Ref)
                                    Builder.AppendLine($"{p.Parameter.Name} = param.{p.Parameter.Name};");
                            }

                            if (returnTypeName.Mode != TypeNameMode.Void)
                                Builder.AppendLine($"return param.{KResult};");
                        }

                        Builder.AppendLine("}");

                        Builder.AppendLine();
                        Builder.AppendLine($"[MethodImpl(MethodImplOptions.NoOptimization)]");
                        Builder.AppendLine($"public {strAccess}{returnDecl} {KOriginal}({originalParameterDecl})");
                        Builder.AppendLine("{");
                        using (Builder.Indent())
                        {
                            foreach (var p in parameterTypeNames)
                            {
                                if (p.RefMode == RefMode.Out)
                                    Builder.AppendLine($"{p.Parameter.Name} = default;");
                            }

                            if (returnTypeName.Mode != TypeNameMode.Void)
                                Builder.AppendLine($"return default;");
                        }

                        Builder.AppendLine("}");


                        Builder.AppendLine();
                        Builder.AppendLine($"[MethodImpl(MethodImplOptions.NoOptimization)]");
                        Builder.AppendLine($"public {strAccess}{returnDecl} {KOriginalWrap}({parameterDecl})");
                        Builder.AppendLine("{");
                        using (Builder.Indent())
                        {
                            Builder.BeginLine("");
                            if (returnTypeName.Mode != TypeNameMode.Void)
                                Builder.Append($"return ");

                            if (returnTypeName.Mode == TypeNameMode.Export)
                                Builder.Append("new(");
                            var args = string.Join(", ", parameterTypeNames.Select(x =>
                            {
                                if (x.TypeName.Mode == TypeNameMode.Export)
                                    return $"{x.ParameterModifier}{x.Parameter.Name}.{KSelf}";
                                return $"{x.ParameterModifier}{x.Parameter.Name}";
                            }));
                            Builder.Append($"{KOriginal}({args})");
                            if (returnTypeName.Mode == TypeNameMode.Export)
                                Builder.Append(")");
                            Builder.EndLine(";");
                        }

                        Builder.AppendLine("}");
                    }

                    Builder.AppendLine("}");
                }

                void DoMethod()
                {
                    foreach (var m in Collection.Methods)
                    {
                        var methodInfo = m.MethodInfo;
                        var returnTypeName = GetTypeName(methodInfo.ReturnType);
                        var parameterTypeNames = methodInfo.GetParameters().Select(GetTypeName).ToArray();
                        DoMethodImpl(m, MonoHookType.Method, methodInfo.IsStatic, returnTypeName, parameterTypeNames);
                    }
                }

                void DoConstructor()
                {
                    foreach (var m in Collection.Constructors)
                    {
                        var constructorInfo = m.ConstructorInfo;
                        var returnTypeName = GetTypeName(typeof(void));
                        var parameterTypeNames = constructorInfo.GetParameters().Select(GetTypeName).ToArray();
                        DoMethodImpl(m, MonoHookType.Constructor, false, returnTypeName, parameterTypeNames);
                    }
                }

                public override void Execute()
                {
                    Builder.AppendLine("// This file was automatically generated by kuroneko.");
                    Builder.AppendLine("// ReSharper disable InconsistentNaming");
                    Builder.AppendLine("using System;");
                    Builder.AppendLine("using System.Reflection;");
                    Builder.AppendLine("using System.Runtime.CompilerServices;");
                    Builder.AppendLine($"using {typeof(MonoHook.MethodHook).Namespace};");
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
                            Builder.AppendLine($"public static class {KHook}");
                            Builder.AppendLine("{");
                            using (Builder.Indent())
                            {
                                DoMethod();
                                DoConstructor();
                            }

                            Builder.AppendLine("}");
                        }

                        Builder.AppendLine("}");
                    }

                    Builder.AppendLine("}");

                    Save();
                }
            }

            protected override GenJobBase CreateJob(MemberCollection collection) => new GenHookJob(this, collection);
        }
    }
}