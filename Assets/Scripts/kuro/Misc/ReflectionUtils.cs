using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace kuro.Core
{
    public enum ParameterPrefixType
    {
        None,
        In,
        Out,
        Ref,
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class WildCardParameterAttribute : Attribute
    {
    }

    public static class ReflectionUtils
    {
        public static ConstructorInfo QC(this Type type) => QC(type, Array.Empty<Type>());

        public static ConstructorInfo QC(this Type self, params Type[] parameterTypes)
        {
            int paramCount = parameterTypes?.Length ?? 0;
            foreach (var constructor in self.GetConstructors((BindingFlags)(-1)))
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length != paramCount)
                    continue;

                if (parameters.IsEquals(parameterTypes,
                        (x, y) => x.GetCustomAttribute<WildCardParameterAttribute>() != null ||
                                  (x.ParameterType.IsByRef ? x.ParameterType.GetElementType() : x.ParameterType) == y))
                    return constructor;
            }

            return null;
        }

        public static ConstructorInfo QC(this Type self, params ParameterInfo[] parameterTypes)
        {
            int paramCount = parameterTypes?.Length ?? 0;
            foreach (var constructor in self.GetConstructors((BindingFlags)(-1)))
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length != paramCount)
                    continue;

                if (parameters.IsEquals(parameterTypes,
                        (x, y) => x.GetCustomAttribute<WildCardParameterAttribute>() != null ||
                                  y.GetCustomAttribute<WildCardParameterAttribute>() != null ||
                                  (x.ParameterType.IsByRef ? x.ParameterType.GetElementType() : x.ParameterType) == y.ParameterType))
                    return constructor;
            }

            return null;
        }

        public static MethodInfo QM(this Type type, string name) => QM(type, name, Array.Empty<string>());

        public static MethodInfo QM(this Type type, string name, params string[] parameterTypes)
        {
            if (type == null)
                return null;
            int paramCount = parameterTypes?.Length ?? 0;
            foreach (var method in type.GetMethods((BindingFlags)(-1)))
            {
                if (method.Name != name)
                    continue;
                var parameters = method.GetParameters();
                if (parameters.Length != paramCount)
                    continue;

                if (parameters.IsEquals(parameterTypes,
                        (x, y) => x.GetCustomAttribute<WildCardParameterAttribute>() != null ||
                                  x.ParameterType.FullName == y))
                    return method;
            }

            return type.BaseType?.QM(name, parameterTypes);
        }

        public static MethodInfo QM(this Type self, string name, params Type[] parameterTypes)
        {
            if (self == null)
                return null;
            int paramCount = parameterTypes?.Length ?? 0;
            foreach (var method in self.GetMethods((BindingFlags)(-1)))
            {
                if (method.Name != name)
                    continue;
                var parameters = method.GetParameters();
                if (parameters.Length != paramCount)
                    continue;

                if (parameters.IsEquals(parameterTypes,
                        (x, y) => x.GetCustomAttribute<WildCardParameterAttribute>() != null ||
                                  (x.ParameterType.IsByRef ? x.ParameterType.GetElementType() : x.ParameterType) == y))
                    return method;
            }

            return self.BaseType?.QM(name, parameterTypes);
        }

        public static MethodInfo QM(this Type self, string name, params ParameterInfo[] parameterTypes)
        {
            int paramCount = parameterTypes?.Length ?? 0;
            foreach (var method in self.GetMethods((BindingFlags)(-1)))
            {
                if (method.Name != name)
                    continue;
                var parameters = method.GetParameters();
                if (parameters.Length != paramCount)
                    continue;

                if (parameters.IsEquals(parameterTypes,
                        (x, y) => x.GetCustomAttribute<WildCardParameterAttribute>() != null ||
                                  y.GetCustomAttribute<WildCardParameterAttribute>() != null ||
                                  (x.ParameterType.IsByRef ? x.ParameterType.GetElementType() : x.ParameterType) ==
                                  (y.ParameterType.IsByRef ? y.ParameterType.GetElementType() : y.ParameterType)))
                    return method;
            }

            return self.BaseType?.QM(name, parameterTypes);
        }

        public static FieldInfo QF(this Type type, string name)
        {
            for (var t = type; t != null; t = t.BaseType)
            {
                var r = t.GetField(name, (BindingFlags)(-1));
                if (r != null)
                    return r;
            }

            return null;
        }

        public static PropertyInfo QP(this Type type, string name)
        {
            for (var t = type; t != null; t = t.BaseType)
            {
                var r = t.GetProperty(name, (BindingFlags)(-1));
                if (r != null)
                    return r;
            }

            return null;
        }

        public static string GetFlatName(this MemberInfo self) => Regex.Replace(self.Name, "[<>.]", "_");

        private static string GetTypeFullName(this Type type)
        {
            string fullName = type.Name;
            var it = type;
            while (it.IsNested)
            {
                it = it.DeclaringType;
                fullName = $"{it.Name}.{fullName}";
            }

            if (!string.IsNullOrEmpty(type.Namespace))
                fullName = $"{type.Namespace}.{fullName}";
            return fullName;
        }

        private static readonly Dictionary<Type, string> s_internalTypeNameMap = new()
        {
            { typeof(object), "object" },
            { typeof(string), "string" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(short), "short" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(ushort), "ushort" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(bool), "bool" },
            { typeof(void), "void" },
            { typeof(void*), "void*" },
        };

        public static string GetTypeDisplayName(this Type type, bool flat = false)
        {
            if (type == null)
                return "";

            if (s_internalTypeNameMap.TryGetValue(type, out var internalName))
                return internalName;

            string fullName = "";
            if (type.IsArray)
            {
                var elementTypeName = type.GetElementType().GetTypeDisplayName(flat);
                var rank = type.GetArrayRank();
                if (flat)
                    fullName = $"Array{(rank > 1 ? $"_{rank}" : "")}_{elementTypeName}";
                else
                    fullName = $"{elementTypeName}[{string.Join(',', Enumerable.Repeat("", rank - 1))}]";
            }
            else if (type.IsByRef)
            {
                fullName = type.GetElementType().GetTypeDisplayName(flat);
            }
            else if (type.IsGenericParameter)
            {
                fullName = type.Name;
            }
            else
            {
                fullName = type.GetTypeFullName();

                var parts = fullName.Split('+');

                var plusIdx = fullName.IndexOf('+');
                if (plusIdx != -1)
                {
                }

                if (type.IsGenericType)
                {
                    var idx = fullName.IndexOf('`');
                    if (idx != -1)
                        fullName = fullName.Substring(0, idx);
                    if (type.IsNested)
                        fullName = fullName.Replace('+', '.');

                    if (!flat)
                    {
                        fullName += "<";
                        fullName += string.Join(", ", type.GetGenericArguments().Select(x => x.GetTypeDisplayName(flat)));
                        fullName += ">";
                    }
                    else
                    {
                        fullName += "_";
                        fullName += string.Join("_", type.GetGenericArguments().Select(x => x.GetTypeDisplayName(flat)));
                    }
                }
            }

            if (flat)
                fullName = fullName.Replace('.', '_');
            return fullName;
        }


        public static ParameterPrefixType GetParameterPrefix(this ParameterInfo self)
        {
            if (self.IsIn)
                return self.IsOut ? ParameterPrefixType.Ref : ParameterPrefixType.In;
            if (self.IsOut)
                return ParameterPrefixType.Out;
            if (self.ParameterType.IsByRef)
                return ParameterPrefixType.Ref;
            return ParameterPrefixType.None;
        }

        public static string GetParameterPrefixString(this ParameterInfo self, bool space = true)
        {
            switch (self.GetParameterPrefix())
            {
                case ParameterPrefixType.In:
                    return space ? "in " : "in";
                case ParameterPrefixType.Out:
                    return space ? "out " : "out";
                case ParameterPrefixType.Ref:
                    return space ? "ref " : "ref";
                default:
                    return "";
            }
        }

        public static bool Is<T>(this Type type) => type.Is(typeof(T));

        public static bool Is(this Type type, Type baseType)
        {
            if (type == null)
                return false;

            return baseType.IsAssignableFrom(type);
        }

        public static T Cast<T>(this Delegate self) where T : Delegate => (T)self.Cast(typeof(T));

        public static Delegate Cast(this Delegate self, Type type) => self != null && type != null ? Delegate.CreateDelegate(type, self.Target, self.Method) : null;

        public static bool IfObsolete(this MemberInfo self) => self.GetCustomAttribute<ObsoleteAttribute>() != null;

        public static bool HasPointer(this Type self)
        {
            if (self == null)
                return false;
            if (self.IsPointer)
                return true;
            if (self.HasElementType && HasPointer(self.GetElementType()))
                return true;
            if (self.IsGenericType)
            {
                foreach (var ga in self.GetGenericArguments())
                {
                    if (HasPointer(ga))
                        return true;
                }
            }

            return false;
        }

        public static bool GetArrayOrListElementType(this Type type, out Type elementType)
        {
            elementType = null;
            if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }

            if (type.IsGenericType && typeof(IList).IsAssignableFrom(type))
            {
                elementType = type.GetGenericArguments().ElementAtOrDefault(0);
                return true;
            }

            return false;
        }

        public static Type GetArrayOrListElementType(this Type type) => type.GetArrayOrListElementType(out var elementType) ? elementType : type;

        public static T CreateDelegate<T>(this MethodInfo self) where T : Delegate
        {
            return (T)self.CreateDelegate(typeof(T));
        }


        public class AssemblyCache
        {
            private readonly Assembly _assembly;
            private readonly Lazy<Type[]> _types;
            public Type[] Types => _types?.Value;
            private readonly Lazy<Dictionary<string, Type>> _typeDictByDisplayName;
            public Type GetTypeByDisplayName(string displayName) => _typeDictByDisplayName.Value.GetValueOrDefault(displayName);

            public Assembly Assembly => _assembly;

            private string _fullName;
            public string FullName => _fullName ??= _assembly?.FullName ?? "";

            private string _shortName;
            public string ShortName => _shortName ??= _assembly?.GetName()?.Name ?? "";

            public AssemblyCache(Assembly assembly)
            {
                _assembly = assembly;
                _typeDictByDisplayName = new(() =>
                {
                    var dict = new Dictionary<string, Type>();
                    foreach (var type in Types)
                    {
                        if (type == null)
                            continue;
                        var displayName = type.GetTypeDisplayName();
                        dict[displayName] = type;
                    }

                    return dict;
                });
                _types = new(() => _assembly.GetTypes());
            }
        }

        public static readonly Lazy<AssemblyCache[]> EditorAssemblies = new(() =>
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var r = new AssemblyCache[assemblies.Length];
            for (var i = 0; i < assemblies.Length; ++i)
                r[i] = new(assemblies[i]);
            return r;
        });

        public static readonly Lazy<Dictionary<string, AssemblyCache>> EditorAssemblyByShortName = new(() =>
        {
            var r = new Dictionary<string, AssemblyCache>();
            foreach (var a in EditorAssemblies.Value)
                r[a.ShortName] = a;
            return r;
        });

        public static AssemblyCache GetEditorAssemblyByShortName(string assemblyShortName) =>
            string.IsNullOrEmpty(assemblyShortName) ? null : EditorAssemblyByShortName.Value.GetValueOrDefault(assemblyShortName);

        public static Type GetTypeByFullName(string assemblyShortName, string typeFullName)
        {
            if (string.IsNullOrEmpty(assemblyShortName) || string.IsNullOrEmpty(typeFullName))
                return null;
            return GetEditorAssemblyByShortName(assemblyShortName)?.Assembly?.GetType(typeFullName, false, false);
        }

        static readonly ConcurrentDictionary<Type, bool> s_typePublicMap = new();

        private static bool IfPublicImpl(this Type self)
        {
            if (!s_typePublicMap.TryGetValue(self, out var value))
            {
                value = true;
                var it = self;
                while (it.IsNested)
                {
                    if (!it.IsNestedPublic)
                    {
                        value = false;
                        break;
                    }

                    it = it.DeclaringType;
                }

                if (value && !it.IsPublic && !it.IsGenericParameter)
                    value = false;

                if (value && self.IsGenericType)
                {
                    foreach (var gt in self.GetGenericArguments())
                    {
                        if (!gt.IsGenericTypeParameter && !gt.IfPublicImpl())
                        {
                            value = false;
                            break;
                        }
                    }
                }

                if (value && self.HasElementType)
                {
                    if (!self.GetElementType().IfPublicImpl())
                        value = false;
                }

                s_typePublicMap.TryAdd(self, value);
            }

            return value;
        }

        public static bool IfGetPublic(this PropertyInfo self)
        {
            if (self.CanRead && !self.GetMethod.IsPublic)
                return false;
            return self.DeclaringType.IfPublic();
        }

        public static bool IfSetPublic(this PropertyInfo self)
        {
            if (self.CanWrite && !self.SetMethod.IsPublic)
                return false;
            return self.DeclaringType.IfPublic();
        }

        public static bool IfPublic<T>(this T self) where T : MemberInfo => MemberInfoTraits<T>.IfPublic(self);
        public static bool IfConst<T>(this T self) where T : MemberInfo => MemberInfoTraits<T>.IfConst(self);
        public static bool IfStatic<T>(this T self) where T : MemberInfo => MemberInfoTraits<T>.IfStatic(self);
        public static bool IfCanRead<T>(this T self) where T : MemberInfo => MemberInfoTraits<T>.IfCanRead(self);
        public static bool IfCanWrite<T>(this T self) where T : MemberInfo => MemberInfoTraits<T>.IfCanWrite(self);
        public static Type GetMemberType<T>(this T self) where T : MemberInfo => MemberInfoTraits<T>.GetMemberType(self);
        public static IEnumerable<Type> ForEachMemberType<T>(this T self) where T : MemberInfo => MemberInfoTraits<T>.ForEachMemberType(self);


        private static bool s_memberInfoTraitsInited = false;

        [InitializeOnLoadMethod]
        private static void MemberInfoTraitsInit()
        {
            if (s_memberInfoTraitsInited)
                return;
            s_memberInfoTraitsInited = true;

            MemberInfoTraits<FieldInfo>.IfPublic = x => x.IsPublic && x.DeclaringType.IfPublic();
            MemberInfoTraits<PropertyInfo>.IfPublic = x =>
            {
                if (x.CanRead && !x.GetMethod.IsPublic)
                    return false;
                if (x.CanWrite && !x.SetMethod.IsPublic)
                    return false;
                return x.DeclaringType.IfPublic();
            };
            MemberInfoTraits<MethodInfo>.IfPublic = x => x.IsPublic && x.DeclaringType.IfPublic();
            MemberInfoTraits<ConstructorInfo>.IfPublic = x => x.IsPublic && x.DeclaringType.IfPublic();
            MemberInfoTraits<EventInfo>.IfPublic = x =>
            {
                if (x.AddMethod?.IsPublic == false)
                    return false;
                if (x.RemoveMethod?.IsPublic == false)
                    return false;
                if (x.RaiseMethod?.IsPublic == false)
                    return false;
                return x.DeclaringType.IfPublic();
            };
            MemberInfoTraits<Type>.IfPublic = IfPublicImpl;
            MemberInfoTraits<MemberInfo>.IfPublic = x => x switch
            {
                FieldInfo fi => fi.IfPublic(),
                PropertyInfo pi => pi.IfPublic(),
                MethodInfo mi => mi.IfPublic(),
                ConstructorInfo ci => ci.IfPublic(),
                EventInfo ei => ei.IfPublic(),
                Type t => t.IfPublic(),
                _ => false
            };

            MemberInfoTraits<FieldInfo>.IfConst = x => x.IsLiteral && !x.IsInitOnly;
            MemberInfoTraits<PropertyInfo>.IfConst = x => false;
            MemberInfoTraits<MethodInfo>.IfConst = x => false;
            MemberInfoTraits<ConstructorInfo>.IfConst = x => false;
            MemberInfoTraits<EventInfo>.IfConst = x => false;
            MemberInfoTraits<Type>.IfConst = x => false;
            MemberInfoTraits<MemberInfo>.IfConst = x => x switch
            {
                FieldInfo fi => fi.IfConst(),
                PropertyInfo pi => pi.IfConst(),
                MethodInfo mi => mi.IfConst(),
                ConstructorInfo ci => ci.IfConst(),
                EventInfo ei => ei.IfConst(),
                Type t => t.IfConst(),
                _ => false
            };

            MemberInfoTraits<FieldInfo>.IfStatic = x => x.IsStatic;
            MemberInfoTraits<PropertyInfo>.IfStatic = x =>
            {
                if (x.CanRead && !x.GetMethod.IsStatic)
                    return false;
                if (x.CanWrite && !x.SetMethod.IsStatic)
                    return false;
                return true;
            };
            MemberInfoTraits<MethodInfo>.IfStatic = x => x.IsStatic;
            MemberInfoTraits<ConstructorInfo>.IfStatic = x => x.IsStatic;
            MemberInfoTraits<EventInfo>.IfStatic = x =>
            {
                if (x.AddMethod?.IsStatic == false)
                    return false;
                if (x.RemoveMethod?.IsStatic == false)
                    return false;
                if (x.RaiseMethod?.IsStatic == false)
                    return false;
                return true;
            };
            MemberInfoTraits<Type>.IfStatic = x => x.IsAbstract && x.IsSealed;
            MemberInfoTraits<MemberInfo>.IfStatic = x => x switch
            {
                FieldInfo fi => fi.IfStatic(),
                PropertyInfo pi => pi.IfStatic(),
                MethodInfo mi => mi.IfStatic(),
                ConstructorInfo ci => ci.IfStatic(),
                EventInfo ei => ei.IfStatic(),
                Type t => t.IfStatic(),
                _ => false
            };

            MemberInfoTraits<FieldInfo>.GetMemberType = x => x.FieldType;
            MemberInfoTraits<PropertyInfo>.GetMemberType = x => x.PropertyType;
            MemberInfoTraits<MethodInfo>.GetMemberType = x => x.ReturnType;
            MemberInfoTraits<ConstructorInfo>.GetMemberType = x => x.DeclaringType;
            MemberInfoTraits<EventInfo>.GetMemberType = x => x.EventHandlerType;
            MemberInfoTraits<Type>.GetMemberType = x => x;
            MemberInfoTraits<MemberInfo>.GetMemberType = x => x switch
            {
                FieldInfo fi => fi.GetMemberType(),
                PropertyInfo pi => pi.GetMemberType(),
                MethodInfo mi => mi.GetMemberType(),
                ConstructorInfo ci => ci.GetMemberType(),
                EventInfo ei => ei.GetMemberType(),
                Type t => t.GetMemberType(),
                _ => null
            };

            MemberInfoTraits<FieldInfo>.IfCanRead = x => true;
            MemberInfoTraits<PropertyInfo>.IfCanRead = x => x.CanRead;
            MemberInfoTraits<MethodInfo>.IfCanRead = x => false;
            MemberInfoTraits<ConstructorInfo>.IfCanRead = x => false;
            MemberInfoTraits<EventInfo>.IfCanRead = x => false;
            MemberInfoTraits<Type>.IfCanRead = x => false;
            MemberInfoTraits<MemberInfo>.IfCanRead = x => x switch
            {
                FieldInfo fi => true,
                PropertyInfo pi => pi.CanRead,
                _ => false
            };

            MemberInfoTraits<FieldInfo>.IfCanWrite = x => !(x.DeclaringType?.IsEnum ?? false);
            MemberInfoTraits<PropertyInfo>.IfCanWrite = x => x.CanWrite;
            MemberInfoTraits<MethodInfo>.IfCanWrite = x => false;
            MemberInfoTraits<ConstructorInfo>.IfCanWrite = x => false;
            MemberInfoTraits<EventInfo>.IfCanWrite = x => false;
            MemberInfoTraits<Type>.IfCanWrite = x => false;
            MemberInfoTraits<MemberInfo>.IfCanWrite = x => x switch
            {
                FieldInfo fi => !(x.DeclaringType?.IsEnum ?? false),
                PropertyInfo pi => pi.CanWrite,
                _ => false
            };

            MemberInfoTraits<FieldInfo>.ForEachMemberType = x => ForeachType(x.FieldType);
            MemberInfoTraits<PropertyInfo>.ForEachMemberType = x => ForeachType(x.PropertyType);
            MemberInfoTraits<MethodInfo>.ForEachMemberType = ForeachMethodType;
            MemberInfoTraits<ConstructorInfo>.ForEachMemberType = x => ForeachType(x.DeclaringType);
            MemberInfoTraits<EventInfo>.ForEachMemberType = x => ForeachType(x.EventHandlerType);
            MemberInfoTraits<Type>.ForEachMemberType = ForeachType;
            MemberInfoTraits<MemberInfo>.ForEachMemberType = x => x switch
            {
                FieldInfo fi => fi.ForEachMemberType(),
                PropertyInfo pi => pi.ForEachMemberType(),
                MethodInfo mi => mi.ForEachMemberType(),
                ConstructorInfo ci => ci.ForEachMemberType(),
                EventInfo ei => ei.ForEachMemberType(),
                Type t => t.ForEachMemberType(),
                _ => null
            };

            static IEnumerable<Type> ForeachType(Type x)
            {
                yield return x;
            }

            static IEnumerable<Type> ForeachMethodType(MethodInfo x)
            {
                if (x.ReturnType != typeof(void))
                    yield return x.ReturnType;
                foreach (var p in x.GetParameters())
                    yield return p.ParameterType;
            }
        }

        private class MemberInfoTraits<T> where T : MemberInfo
        {
            private static Func<T, bool> s_ifPublic;
            private static Func<T, bool> s_ifConst;
            private static Func<T, bool> s_ifStatic;
            private static Func<T, bool> s_ifCanRead;
            private static Func<T, bool> s_ifCanWrite;
            private static Func<T, Type> s_getRealMemberType;
            private static Func<T, IEnumerable<Type>> s_forEachMemberType;
            private static Type s_type = typeof(T);

            public static Func<T, bool> IfPublic
            {
                get
                {
                    MemberInfoTraitsInit();
                    return s_ifPublic;
                }
                set => s_ifPublic = value;
            }

            public static Func<T, bool> IfConst
            {
                get
                {
                    MemberInfoTraitsInit();
                    return s_ifConst;
                }
                set => s_ifConst = value;
            }

            public static Func<T, bool> IfStatic
            {
                get
                {
                    MemberInfoTraitsInit();
                    return s_ifStatic;
                }
                set => s_ifStatic = value;
            }

            public static Func<T, bool> IfCanRead
            {
                get
                {
                    MemberInfoTraitsInit();
                    return s_ifCanRead;
                }
                set => s_ifCanRead = value;
            }

            public static Func<T, bool> IfCanWrite
            {
                get
                {
                    MemberInfoTraitsInit();
                    return s_ifCanWrite;
                }
                set => s_ifCanWrite = value;
            }

            public static Func<T, Type> GetMemberType
            {
                get
                {
                    MemberInfoTraitsInit();
                    return s_getRealMemberType;
                }
                set => s_getRealMemberType = value;
            }

            public static Func<T, IEnumerable<Type>> ForEachMemberType
            {
                get
                {
                    MemberInfoTraitsInit();
                    return s_forEachMemberType;
                }
                set => s_forEachMemberType = value;
            }
        }
    }
}