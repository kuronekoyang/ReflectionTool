using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using kuro.Core;
using MonoHook;

namespace kuro.ReflectionTool
{
    public enum MonoHookType
    {
        Method,
        Property,
        Constructor,
        PropertyGet,
        PropertySet,
    }

    public class MonoHookAttribute : Attribute
    {
        public MonoHookType HookType;
        public bool WaitGUISkin;
        public Type[] Parameters;

        public MonoHookAttribute()
        {
            this.HookType = MonoHookType.Method;
            this.WaitGUISkin = false;
            this.Parameters = null;
        }

        public MonoHookAttribute(MonoHookType hookType, bool waitGUISkin, params Type[] parameters)
        {
            this.HookType = hookType;
            this.WaitGUISkin = waitGUISkin;
            this.Parameters = parameters;
        }
    }

    public class MonoHookInstaller
    {
        const string __assembly__Str = "__assembly__";
        const string __type__Str = "__type__";
        const string __original__Str = "__original__";
        const string __replace__Str = "__replace__";

        private MethodHook _hook = null;

        private delegate MethodBase GetTargetMethodDelegate(Type originType, Type hookType, MethodInfo __replace__);

        static string GetHookTypeName(Type hookType)
        {
            var name = hookType.Name;
            var idx = name.LastIndexOf("__");
            if (idx == -1)
                return name;
            if (!int.TryParse(name.Substring(idx + 2), out var _))
                return name;
            return name.Substring(0, idx);
        }

        void InitMethod(Type targetType, Type hookType, GetTargetMethodDelegate getter)
        {
            if (_hook != null)
                return;

            // 找到被替换成的新方法
            MethodInfo miReplacement = hookType.GetMethod(__replace__Str, (BindingFlags)(-1));

            // 这个方法是用来调用原始方法的
            MethodInfo miProxy = hookType.GetMethod(__original__Str, (BindingFlags)(-1));

            // 找到需要 Hook 的方法
            MethodBase miTarget = getter(targetType, hookType, miReplacement);

            if (miTarget == null)
            {
                Debug.LogErrorFormat("{0} hook failed, method {1} is invalid", targetType?.FullName ?? "?", GetHookTypeName(hookType) ?? "?");
                return;
            }

            if (miReplacement.IsStatic != miTarget.IsStatic || miReplacement.IsStatic != miProxy.IsStatic)
            {
                Debug.LogError($"{targetType.FullName ?? "?"} hook failed, method static is not match");
                return;
            }

            // 创建一个 Hook 并 Install 就OK啦, 之后无论哪个代码再调用原始方法都会重定向到
            //  我们写的方法ヾ(ﾟ∀ﾟゞ)
            _hook = new MethodHook(miTarget, miReplacement, miProxy);
            _hook.Install();
        }

        private static List<KeyValuePair<Type, MonoHookAttribute>> s_allHookList;
        private static readonly List<MonoHookInstaller> s_monoHookInstallerList = new();

        static MethodBase GetTargetMethod(Type originType, Type hookType, MethodInfo __replace__)
        {
            return originType.QM(GetHookTypeName(hookType), __replace__.GetParameters());
        }

        static MethodBase GetTargetConstructor(Type originType, Type hookType, MethodInfo __replace__)
        {
            return originType.QC(__replace__.GetParameters());
        }

        static MethodBase GetTargetPropertyGet(Type originType, Type hookType, MethodInfo __replace__)
        {
            return originType.GetProperty(GetHookTypeName(hookType), (BindingFlags)(-1))?.GetMethod;
        }

        static MethodBase GetTargetPropertySet(Type originType, Type hookType, MethodInfo __replace__)
        {
            return originType.GetProperty(GetHookTypeName(hookType), (BindingFlags)(-1))?.SetMethod;
        }

        [InitializeOnLoadMethod]
        static void Initalize()
        {
#if UNITY_EDITOR_OSX
            return;
#endif
            EditorPrefs.SetBool("ScriptDebugInfoEnabled", true);
            UnityEditor.Compilation.CompilationPipeline.codeOptimization = UnityEditor.Compilation.CodeOptimization.Debug;

            s_allHookList = TypeCache.GetTypesWithAttribute<MonoHookAttribute>()
                .Select(x => new KeyValuePair<Type, MonoHookAttribute>(x, x.GetCustomAttribute<MonoHookAttribute>()))
                .ToList();
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
            OnEditorUpdate();
        }

        private static FieldInfo s_GUISkin_current = typeof(UnityEngine.GUISkin).QF("current");

        static void OnEditorUpdate()
        {
            for (int i = s_allHookList.Count - 1; i >= 0; --i)
            {
                var info = s_allHookList[i];
                if (info.Value.WaitGUISkin)
                {
                    if (s_GUISkin_current?.GetValue(null) as GUISkin == null)
                        continue;
                }

                try
                {
                    var installer = new MonoHookInstaller();

                    var assemblyShortName = info.Key.GetField(__assembly__Str, (BindingFlags)(-1))?.GetValue(null) as string;
                    var typeFullName = info.Key.GetField(__type__Str, (BindingFlags)(-1))?.GetValue(null) as string;
                    var targetType = ReflectionUtils.GetTypeByFullName(assemblyShortName, typeFullName);
                    if (targetType == null)
                    {
                        s_allHookList.RemoveAt(i);
                        continue;
                    }

                    switch (info.Value.HookType)
                    {
                        case MonoHookType.Method:
                            installer.InitMethod(targetType, info.Key, GetTargetMethod);
                            break;
                        case MonoHookType.Constructor:
                            installer.InitMethod(targetType, info.Key, GetTargetConstructor);
                            break;
                        case MonoHookType.PropertyGet:
                            installer.InitMethod(targetType, info.Key, GetTargetPropertyGet);
                            break;
                        case MonoHookType.PropertySet:
                            installer.InitMethod(targetType, info.Key, GetTargetPropertySet);
                            break;
                    }

                    s_monoHookInstallerList.Add(installer);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message + "\n" + e.StackTrace);
                }

                s_allHookList.RemoveAt(i);
            }

            if (s_allHookList.Count == 0)
            {
                EditorApplication.update -= OnEditorUpdate;
            }
        }
    }
}