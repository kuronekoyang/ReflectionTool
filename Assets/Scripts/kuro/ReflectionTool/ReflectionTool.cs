using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using kuro.Core;
using UnityEngine;

namespace kuro.ReflectionTool
{
    public partial class ReflectionTool
    {
        private static bool HasWrapExporter(Type t) => t.FullName is { } name && WrapInfoList.Any(x => x.TypeFullName == name);
        private static bool HasHookExporter(Type t) => t.FullName is { } name && HookInfoList.Any(x => x.TypeFullName == name);

        public abstract class GenInfoBase
        {
            public string AssemblyShortName;
            public string TypeFullName;
            public string GeneratePath;
        }

        public class GenWrapInfo : GenInfoBase
        {
            public GenWrapInfo(string assemblyShortName, string typeFullName, string generatePath = null)
            {
                AssemblyShortName = assemblyShortName;
                TypeFullName = typeFullName;
                GeneratePath = generatePath;
            }
        }

        public class GenHookInfo : GenInfoBase
        {
            public readonly string MethodName;
            public readonly Type[] ParamTypes;
            public readonly bool WaitForGUISkin;

            public GenHookInfo(string assemblyShortName, string typeFullName, string methodName, Type[] paramTypes, string generatePath = null)
            {
                AssemblyShortName = assemblyShortName;
                TypeFullName = typeFullName;
                MethodName = methodName;
                ParamTypes = paramTypes;
                GeneratePath = generatePath;
                WaitForGUISkin = false;
            }

            public GenHookInfo(string assemblyShortName, string typeFullName, string methodName, bool waitForGUISkin, Type[] paramTypes, string generatePath = null)
            {
                AssemblyShortName = assemblyShortName;
                TypeFullName = typeFullName;
                MethodName = methodName;
                ParamTypes = paramTypes;
                GeneratePath = generatePath;
                WaitForGUISkin = waitForGUISkin;
            }
        }


        public struct Member
        {
            public MemberInfo MemberInfo;
            public string MemberOriginName;
            public string RawFlatName;
            public string UniqueName;
            public bool WaitForGUISkin;
            public FieldInfo FieldInfo => MemberInfo as FieldInfo;
            public PropertyInfo PropertyInfo => MemberInfo as PropertyInfo;
            public MethodInfo MethodInfo => MemberInfo as MethodInfo;
            public ConstructorInfo ConstructorInfo => MemberInfo as ConstructorInfo;
            public Type DelegateInfo => MemberInfo as Type;
        }

        public class MemberCollection
        {
            public readonly MemberCollectionGroup OwnerGroup;
            public readonly Type Type;
            public readonly List<Member> Fields = new();
            public readonly List<Member> Properties = new();
            public readonly List<Member> Methods = new();
            public readonly List<Member> Constructors = new();
            public readonly List<Member> Delegates = new();
            public readonly List<Member> FieldOrProperties = new();
            public readonly string GeneratePath;

            private Dictionary<string, int> _nameCount = new();

            public MemberCollection(MemberCollectionGroup ownerGroup, Type type, string generatePath)
            {
                OwnerGroup = ownerGroup;
                Type = type;
                GeneratePath = generatePath;
            }

            private void AddMemberImpl(MemberInfo member, bool isSpecial, bool waitForGUISkin)
            {
                if (member == null)
                    return;
                if (member.IfObsolete())
                    return;

                if (member.Name.StartsWith('<'))
                    return;

                Member m = new();
                m.MemberInfo = member;
                m.MemberOriginName = member.Name;
                m.RawFlatName = member.GetFlatName();
                m.WaitForGUISkin = waitForGUISkin;

                if (!_nameCount.TryGetValue(m.RawFlatName, out var count))
                    count = 0;
                count += 1;
                _nameCount[m.RawFlatName] = count;

                if (count > 1)
                    m.UniqueName = $"{m.RawFlatName}__{count}";
                else
                    m.UniqueName = m.RawFlatName;

                switch (member)
                {
                    case FieldInfo fi:
                    {
                        if (!isSpecial && fi.IsSpecialName)
                            return;
                        if (fi.FieldType.IfObsolete())
                            return;
                        Fields.Add(m);
                        FieldOrProperties.Add(m);
                        break;
                    }
                    case PropertyInfo pi:
                    {
                        if (pi.PropertyType.IfObsolete())
                            return;
                        Properties.Add(m);
                        FieldOrProperties.Add(m);
                        break;
                    }
                    case MethodInfo mi:
                    {
                        if (!isSpecial && mi.IsSpecialName)
                            return;
                        if (mi.ReturnType.IfObsolete())
                            return;
                        if (mi.GetParameters().Any(x => x.ParameterType.IfObsolete()))
                            return;
                        Methods.Add(m);
                        break;
                    }
                    case ConstructorInfo ci:
                    {
                        if (ci.GetParameters().Any(x => x.ParameterType.IfObsolete()))
                            return;
                        Constructors.Add(m);
                        break;
                    }
                    case Type ei:
                    {
                        if (ei.IfObsolete())
                            return;
                        if (ei.Is<Delegate>())
                        {
                            var invoke = ei.GetMethod("Invoke");
                            if (invoke != null && invoke.ReturnType.IfPublic() &&
                                !invoke.ReturnType.ContainsGenericParameters &&
                                !invoke.GetGenericArguments().Any(x => x.ContainsGenericParameters || !x.IfPublic()))
                            {
                                Delegates.Add(m);
                                OwnerGroup.DeclaredDelegateTypes.Add(ei);
                            }
                        }

                        break;
                    }
                }
            }

            public void AddMember(GenInfoBase info)
            {
                switch (info)
                {
                    case GenWrapInfo:
                    {
                        foreach (var m in Type.GetMembers((BindingFlags)(-1)))
                        {
                            if (m.IfPublic())
                                continue;
                            AddMemberImpl(m, false, false);
                        }

                        break;
                    }
                    case GenHookInfo hi:
                    {
                        if (hi.ParamTypes != null)
                        {
                            if (hi.MethodName == ".ctor")
                            {
                                var method = Type.QC(hi.ParamTypes);
                                if (method != null)
                                    AddMemberImpl(method, false, hi.WaitForGUISkin);
                            }
                            else
                            {
                                var method = Type.QM(hi.MethodName, hi.ParamTypes);
                                if (method != null)
                                    AddMemberImpl(method, false, hi.WaitForGUISkin);
                            }
                        }
                        else if (hi.MethodName.StartsWith("set."))
                        {
                            var method = Type.QP(hi.MethodName.Substring(4))?.SetMethod;
                            if (method != null)
                                AddMemberImpl(method, true, hi.WaitForGUISkin);
                        }
                        else if (hi.MethodName.StartsWith("get."))
                        {
                            var method = Type.QP(hi.MethodName.Substring(4))?.GetMethod;
                            if (method != null)
                                AddMemberImpl(method, true, hi.WaitForGUISkin);
                        }
                        else
                        {
                            foreach (var method in Type.GetMember(hi.MethodName, (BindingFlags)(-1)).OfType<MethodInfo>())
                                AddMemberImpl(method, false, hi.WaitForGUISkin);
                        }

                        break;
                    }
                }
            }
        }

        public class MemberCollectionGroup
        {
            public readonly List<MemberCollection> List = new();
            public readonly HashSet<Type> DeclaredTypes = new();
            public readonly HashSet<Type> DeclaredDelegateTypes = new();

            private MemberCollection GetMemberCollection(string assemblyShortName, string typeFullName, string generatePath)
            {
                if (string.IsNullOrWhiteSpace(typeFullName))
                    return null;
                var result = List.FirstOrDefault(x => x.Type.FullName == typeFullName);
                if (result != null)
                    return result;

                var type = ReflectionUtils.GetTypeByFullName(assemblyShortName, typeFullName);
                if (type == null)
                    return null;
                // if (type.IsGenericType)
                //     return null;

                result = new(this, type, generatePath);
                List.Add(result);
                DeclaredTypes.Add(type);
                return result;
            }

            public void AddMember(IEnumerable<GenInfoBase> genInfoList)
            {
                foreach (var info in genInfoList)
                {
                    var collection = GetMemberCollection(info.AssemblyShortName, info.TypeFullName, info.GeneratePath);
                    collection?.AddMember(info);
                }
            }
        }

        public abstract class GenToolBase
        {

            public enum TypeNameMode
            {
                Public,
                Export,
                Array,
                List,
                Dictionary,
                Object,
                Void,
                Generic,
                Delegate,
                ExportDelegate,
            }

            public enum RefMode
            {
                None,
                In,
                Out,
                Ref,
            }

            public struct TypeName
            {
                public string Name;
                public TypeNameMode Mode;
                public bool HasPointer;

                public string Cast(string value) => Mode switch
                {
                    TypeNameMode.Export => $"new {Name}({value})",
                    TypeNameMode.ExportDelegate => $"({value} as Delegate)?.{nameof(ReflectionUtils.Cast)}<{Name}>()",
                    TypeNameMode.Void => $"{value}",
                    _ => $"({Name})({value})"
                };

                public string CastTo(string value, string type) => Mode switch
                {
                    TypeNameMode.Export => $"{value}.{KSelf}",
                    TypeNameMode.ExportDelegate => $"{value}?.{nameof(ReflectionUtils.Cast)}({type})",
                    TypeNameMode.Void => "",
                    _ => value
                };
            }


            public struct ParameterTypeName
            {
                public TypeName TypeName;
                public ParameterInfo Parameter;
                public RefMode RefMode;

                public string ParameterModifier => Parameter.GetParameterPrefixString();

                public string Cast(string value) => TypeName.Cast(value);

                public string GetValue(string value) => TypeName.CastTo(value, "null");
            }

            public abstract class GenJobBase
            {
                private GenToolBase _genTool;
                public readonly MemberCollection Collection;

                public readonly Type Type;
                public readonly string AssemblyShortName;
                public readonly string TypeFullName;
                public readonly string TypeFlatName;
                public readonly string TypeGenericSuffix;
                public readonly string TypeDisplayName;
                public readonly bool TypeIsStatic;
                public readonly bool TypeIsUnityObject;
                public readonly string ObjectTypeName;

                public readonly IndentStringBuilder Builder;
                public string FileName;

                protected GenJobBase(GenToolBase genTool, MemberCollection collection)
                {
                    _genTool = genTool;
                    Collection = collection;

                    Type = collection.Type;
                    AssemblyShortName = Type.Assembly.GetName().Name;
                    TypeFullName = Type.FullName;
                    TypeFlatName = Type.GetTypeDisplayName(true);
                    TypeDisplayName = Type.GetTypeDisplayName(false);
                    TypeGenericSuffix = Type.IsGenericTypeDefinition ? $"<{string.Join(", ", Type.GetGenericArguments().Select(x => x.Name))}>" : "";
                    TypeIsStatic = Type.IfStatic();
                    TypeIsUnityObject = Type.Is<UnityEngine.Object>();
                    ObjectTypeName = TypeIsUnityObject ? "UnityEngine.Object" : "object";

                    Builder = new();
                }

                public bool HasExporter(Type type)
                {
                    var assemblyShortName = type.Assembly.GetName().Name;
                    var typeFullName = type.FullName;
                    return WrapInfoList.Any(x => x.AssemblyShortName == assemblyShortName && x.TypeFullName == typeFullName);
                }

                public bool HasExporterDelegate(Type type) => _genTool.Group.DeclaredDelegateTypes.Contains(type);

                public ParameterTypeName GetTypeName(ParameterInfo p)
                {
                    ParameterTypeName result = new();
                    result.TypeName = GetTypeName(p.ParameterType);
                    result.Parameter = p;
                    if (p.IsOut)
                        result.RefMode = p.IsIn ? RefMode.Ref : RefMode.Out;
                    else if (p.IsIn)
                        result.RefMode = RefMode.In;
                    else if (p.ParameterType.IsByRef)
                        result.RefMode = RefMode.Ref;
                    else
                        result.RefMode = RefMode.None;
                    return result;
                }

                public TypeName GetTypeName(Type t)
                {
                    TypeName result = new();
                    result.HasPointer = t.HasPointer();

                    if (t.IsByRef)
                        t = t.GetElementType();

                    if (t == typeof(void))
                    {
                        result.Name = "void";
                        result.Mode = TypeNameMode.Void;
                    }
                    else if (t.IsGenericParameter)
                    {
                        result.Name = t.Name;
                        result.Mode = TypeNameMode.Generic;
                    }
                    else if (t.IfPublic())
                    {
                        result.Name = t.GetTypeDisplayName(false);
                        result.Mode = TypeNameMode.Public;
                    }
                    else if (HasExporter(t))
                    {
                        result.Name = t.GetTypeDisplayName(true);
                        result.Mode = TypeNameMode.Export;
                    }
                    else if (HasExporterDelegate(t))
                    {
                        result.Name = t.DeclaringType != Type ? $"{t.DeclaringType.GetTypeDisplayName(true)}.{t.Name}" : t.Name;
                        result.Mode = TypeNameMode.ExportDelegate;
                    }
                    else if (t.IsArray)
                    {
                        result.Name = "Array";
                        result.Mode = TypeNameMode.Array;
                    }
                    else if (t.Is(typeof(IList)))
                    {
                        result.Name = "System.Collections.IList";
                        result.Mode = TypeNameMode.List;
                    }
                    else if (t.Is(typeof(IDictionary)))
                    {
                        result.Name = "System.Collections.IDictionary";
                        result.Mode = TypeNameMode.Dictionary;
                    }
                    else if (t.Is<Delegate>())
                    {
                        result.Name = "Delegate";
                        result.Mode = TypeNameMode.Delegate;
                    }
                    else
                    {
                        result.Name = "object";
                        result.Mode = TypeNameMode.Object;
                    }

                    return result;
                }

                private Type TestSuperType(Type t)
                {
                    if (t == null)
                        return null;
                    if (t == typeof(object) || t == typeof(UnityEngine.Object))
                        return null;
                    if (t.IfPublic())
                        return t;
                    if (HasExporter(t))
                        return t;
                    return t.BaseType;
                }

                public Type GetSuperType(Type t) => t.IfPublic() ? t : TestSuperType(t.BaseType);

                private Type TestBaseType(Type t)
                {
                    if (t == null)
                        return null;
                    if (t == typeof(object) || t == typeof(UnityEngine.Object))
                        return null;
                    if (t.IfPublic())
                        return t;
                    return t.BaseType;
                }

                public Type GetBaseType(Type t) => t.IfPublic() ? t : TestBaseType(t.BaseType);

                public void Save()
                {
                    var path = Path.Combine(this.Collection.GeneratePath ?? DefaultGeneratePath, FileName);
                    IOUtils.WriteAllText(path, Builder.ToString());
                }

                public abstract void Execute();
            }

            protected abstract GenInfoBase[] GenInfoList { get; }
            protected readonly MemberCollectionGroup Group = new();


            protected abstract GenJobBase CreateJob(MemberCollection collection);

            public void Execute()
            {
                Group.AddMember(GenInfoList);

                List<GenJobBase> resultList = new();
                foreach (var collection in Group.List)
                    resultList.Add(CreateJob(collection));

                Parallel.ForEach(resultList, x => x.Execute());
            }
        }
    }
}