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
        private const string DefaultGeneratePath = "Assets/Scripts/Generated/";
        //private const string ExternalGeneratePath = "";

        /// <summary>
        /// 配置反射类型
        /// </summary>
        private static GenWrapInfo[] WrapInfoList => new GenWrapInfo[]
        {
            new("Assembly-CSharp", "kuro.Shell"),
            new("Assembly-CSharp", "kuro.Shell+BaseClass"),
            new("Assembly-CSharp", "kuro.Shell+SampleClass"),
            new("Assembly-CSharp", "kuro.Shell+SampleClass+SampleEnum"),

            // new("UnityEngine.IMGUIModule", "UnityEngine.GUISkin"),
            // new("UnityEditor.CoreModule", "UnityEditor.Editor"),
            // new("UnityEditor.CoreModule", "UnityEditor.HandleUtility"),
            // new("UnityEditor.CoreModule", "UnityEditor.EditorGUI"),
            // new("UnityEditor.CoreModule", "UnityEditor.EditorGUIUtility"),
        };

        /// <summary>
        /// 这里是专门配置需要Hook的函数的，目的是为了修改这些函数的原始行为，如果仅仅只是要反射，请往上看
        /// </summary>
        private static GenHookInfo[] HookInfoList => new GenHookInfo[]
        {
            new("Assembly-CSharp", "kuro.Shell+SampleClass", "InvokeDelegate", null),
            new("Assembly-CSharp", "kuro.Shell+SampleClass", ".ctor", new[] { typeof(int) }),
            new("Assembly-CSharp", "kuro.Shell+SampleClass", "set.Value", null),

            // new("UnityEngine.CoreModule", "UnityEngine.GameObject", "SetActive", null),
            // new("UnityEngine.CoreModule", "UnityEngine.Transform", "set.position", null),
        };

        private const string KSelf = "__self__";
        private const string KAssembly = "__assembly__";
        private const string KType = "__type__";
        private const string KBase = "__base__";
        private const string KSuper = "__super__";
        private const string KValid = "__valid__";
        private const string KHook = "__hook__";
        private const string KDelegate = "__delegate__";
        private const string KUserdata = "__userdata__";
        private const string KReplace = "__replace__";
        private const string KOriginal = "__original__";
        private const string KOriginalWrap = "__original_wrap_";
        private const string KThis = "__this__";
        private const string KResult = "__result__";
        private const string KNew = "__new__";
        private const string KPool = "__pool__";
        private const string KParams = "__params__";
        private const string KInt = "__int__";
        private const string KPrefix = "__";
        private const string KDelegatePrefix = "__D";
        private const string KElementPrefix = "__E";
        private const string KGetPrefix = "__Get";
        private const string KSetPrefix = "__Set";
    }
}