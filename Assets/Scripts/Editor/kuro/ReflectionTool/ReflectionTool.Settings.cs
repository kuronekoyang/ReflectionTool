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
        private const string DefaultGeneratePath = "Assets/Scripts/Editor/kuro/ReflectionTool/Generated/";
        //private const string ExternalGeneratePath = "";

        /// <summary>
        /// 这里是配置反射的
        /// </summary>
        private static GenWrapInfo[] WrapInfoList => new GenWrapInfo[]
        {
            new("UnityEngine.IMGUIModule", "UnityEngine.GUISkin"),

            new("UnityEditor.CoreModule", "UnityEditor.Editor"),
            new("UnityEditor.CoreModule", "UnityEditor.HandleUtility"),
            new("UnityEditor.CoreModule", "UnityEditor.EditorGUI"),
            new("UnityEditor.CoreModule", "UnityEditor.EditorGUIUtility"),

            // new("UnityEditor.CoreModule", "UnityEditor.SceneView"),
            // new("UnityEditor.CoreModule", "UnityEditor.OutlineDrawMode"),
            // new("UnityEditor.CoreModule", "UnityEditor.PrefColor"),
            // new("UnityEditor.CoreModule", "UnityEditor.Overlays.OverlayUtilities"),
            // new("UnityEditor.CoreModule", "UnityEditor.Overlays.Overlay"),
            // new("UnityEditor.CoreModule", "UnityEditor.Overlays.OverlayCanvas"),
            // new("UnityEditor.CoreModule", "UnityEditor.Overlays.OverlayMenu"),
            // new("UnityEditor.CoreModule", "UnityEditor.Overlays.OverlayMenuItem"),
            // new("UnityEditor.CoreModule", "UnityEditor.TransformToolsOverlayToolBar"),
            // new("UnityEditor.CoreModule", "UnityEditor.Tuple"),
            // new("UnityEditor.CoreModule", "UnityEditor.Handles"),
            // new("UnityEditor.CoreModule", "UnityEditor.Handles+CameraFilterMode"),
            // new("UnityEditor.CoreModule", "UnityEditor.Handles+PositionHandleParam"),
            // new("UnityEditor.CoreModule", "UnityEditor.Handles+PositionHandleParam+Handle"),
            // new("UnityEditor.CoreModule", "UnityEditor.Handles+PositionHandleParam+Orientation"),
            // new("UnityEngine.IMGUIModule", "UnityEngine.GUIClip"),
            // new("UnityEditor.EditorToolbarModule", "UnityEditor.Toolbars.SnapSettings"),
            // new("UnityEditor.CoreModule", "UnityEditor.GridAndSnapToolBar"),
            // new("UnityEditor.CoreModule", "UnityEditor.AnnotationUtility"),
            // new("UnityEditor.CoreModule", "UnityEditor.AI.NavMeshBuilder"),
            // new("UnityEditor.CoreModule", "UnityEditor.Build.BuildPlatforms"),
            // new("UnityEditor.CoreModule", "UnityEditor.Build.BuildPlatform"),
            // new("UnityEditor.CoreModule", "UnityEditor.AudioUtil"),
            // new("UnityEditor.CoreModule", "UnityEditor.LogEntries"),
            // new("UnityEditor.CoreModule", "UnityEditor.LogEntry"),
            // new("UnityEditor.CoreModule", "UnityEditor.ConsoleWindow"),
            // new("UnityEditor.CoreModule", "UnityEditor.ConsoleWindow+Mode"),
            // new("UnityEditor.CoreModule", "UnityEditor.TextureImporterInspector"),
            // new("UnityEditor.CoreModule", "UnityEditor.TextureImporterInspector+Styles"),
            // new("UnityEditor.CoreModule", "UnityEditor.TextureImporterInspector+TextureInspectorTypeGUIProperties"),
            // new("UnityEditor.CoreModule", "UnityEditor.TextureImporterInspector+TextureInspectorGUIElement"),
            // new("UnityEditor.CoreModule", "UnityEditor.IHVImageFormatImporterInspector"),
            // new("UnityEditor.CoreModule", "UnityEditor.IHVImageFormatImporterInspector+Styles"),
            // new("UnityEditor.CoreModule", "UnityEditor.TextureInspector"),
            // new("UnityEditor.CoreModule", "UnityEditor.AudioImporterInspector"),
            // new("UnityEditor.CoreModule", "UnityEditor.ModelImporterEditor"),
            // new("UnityEditor.CoreModule", "UnityEditor.AssetImporterTabbedEditor"),
            // new("UnityEditor.CoreModule", "UnityEditor.ModelImporterModelEditor"),
            // new("UnityEditor.CoreModule", "UnityEditor.ModelImporterRigEditor"),
            // new("UnityEditor.CoreModule", "UnityEditor.ModelImporterRigEditor+Styles"),
            // new("UnityEditor.CoreModule", "UnityEditor.ModelImporterClipEditor"),
            // new("UnityEditor.CoreModule", "UnityEditor.ModelImporterClipEditor+Styles"),
            // new("UnityEditor.CoreModule", "UnityEditor.ModelImporterMaterialEditor"),
            // new("UnityEditor.CoreModule", "UnityEditor.BaseTextureImportPlatformSettings"),
            // new("UnityEditor.CoreModule", "UnityEditor.TextureImportPlatformSettings"),
            // new("UnityEditor.CoreModule", "UnityEditor.TextureImportPlatformSettingsData"),
            // new("UnityEditor.CoreModule", "UnityEditor.EditorGUILayout"),
            // new("UnityEditor.CoreModule", "UnityEditor.EditorStyles"),
            // new("UnityEditor.CoreModule", "UnityEditor.PropertyModification"),
            // new("UnityEditor.CoreModule", "UnityEditor.LightmapEditorSettings"),
            // new("UnityEditor.PresetsUIModule", "UnityEditor.Presets.PresetEditor"),
            // new("UnityEditor.CoreModule", "UnityEditor.SerializedProperty"),
            // new("UnityEditor.CoreModule", "UnityEditor.BuiltinResource"),
            // new("UnityEditor.CoreModule", "UnityEditor.ObjectSelector"),
            // new("UnityEditor.CoreModule", "UnityEditor.ObjectListArea"),
            // new("UnityEditor.CoreModule", "UnityEditor.ObjectListArea+Group"),
            // new("UnityEditor.CoreModule", "UnityEditor.ObjectListArea+LocalGroup"),
            // new("UnityEditor.CoreModule", "UnityEditor.ObjectListArea+LocalGroup+ExtraItem"),
            // new("UnityEditor.CoreModule", "UnityEditor.TextureUtil"),
            // new("UnityEditor.CoreModule", "UnityEditor.EditorGUI+ObjectFieldValidatorOptions"),
            // new("UnityEngine.UIElementsModule", typeof(UnityEngine.UIElements.BaseField<UnityEngine.Object>).FullName),
            // new("UnityEditor.CoreModule", "UnityEditor.AddComponent.AddComponentWindow"),
            // new("UnityEditor.CoreModule", "UnityEditor.AddComponent.ComponentDropdownItem"),
            // new("UnityEditor.CoreModule", "UnityEditor.IMGUI.Controls.AdvancedDropdownItem"),
            // new("UnityEditor.CoreModule", "UnityEditor.AddComponent.AddComponentDataSource+MenuItemData"),
        };

        /// <summary>
        /// 这里是专门配置需要Hook的函数的，目的是为了修改这些函数的原始行为，如果仅仅只是要反射，请往上看
        /// </summary>
        private static GenHookInfo[] HookInfoList => new GenHookInfo[]
        {
            /// 这里是专门配置需要Hook的函数的，目的是为了修改这些函数的原始行为，如果仅仅只是要反射，请往上看

            // new("UnityEditor.CoreModule", "UnityEditor.SceneView", "HandleSelectionAndOnSceneGUI", null),
            // new("UnityEditor.CoreModule", "UnityEditor.Overlays.OverlayMenu", "RebuildMenu", null),
            // new("UnityEditor.EditorToolbarModule", "UnityEditor.Toolbars.SnapSettings", "UpdateGridSnapEnableState", null),
            // new("Unity.Formats.Fbx.Editor", "UnityEditor.Formats.Fbx.Exporter.ExportModelEditorWindow", "Export", null, EditorWindowGeneratePath),
            // new("UnityEditor.CoreModule", "UnityEditor.SceneView", "DrawRenderModeOverlay", null),
            // new("UnityEditor.CoreModule", "UnityEditor.Handles", "DrawOutline", new[] { typeof(int[]), typeof(int[]), typeof(Color), typeof(Color), typeof(float) }),
            // new("UnityEditor.CoreModule", "UnityEditor.Handles", "DrawOutline", new[] { typeof(Renderer[]), typeof(Color), typeof(Color), typeof(float) }),
            // new("UnityEditor.CoreModule", "UnityEditor.Handles", "DrawOutline", new[] { typeof(GameObject[]), typeof(Color), typeof(Color), typeof(float) }),
            // new("UnityEditor.CoreModule", "UnityEditor.Handles", "DrawOutline", new[] { typeof(List<GameObject>), typeof(Color), typeof(Color), typeof(float) }),
            // new("UnityEditor.CoreModule", "UnityEditor.Handles", "DrawOutline", new[] { typeof(Renderer[]), typeof(Color), typeof(float) }),
            // new("UnityEditor.CoreModule", "UnityEditor.Handles", "DrawOutline", new[] { typeof(GameObject[]), typeof(Color), typeof(float) }),
            // new("UnityEditor.CoreModule", "UnityEditor.Handles", "DrawOutline", new[] { typeof(List<GameObject>), typeof(Color), typeof(float) }),
            // new("UnityEditor.CoreModule", "UnityEditor.Handles", "DrawOutline", new[] { typeof(int[]), typeof(Color), typeof(float) }),
            // new("UnityEditor.CoreModule", "UnityEditor.LogEntries", "RowGotDoubleClicked", null),
            // new("UnityEditor.CoreModule", "UnityEditor.ConsoleWindow", "SetActiveEntry", null),
            // new("UnityEditor.CoreModule", "UnityEditor.HandleUtility", "FilterInstanceIDs", null),
            // new("UnityEditor.CoreModule", "UnityEditor.PreferencesProvider", "ShowExternalApplications", true, null),
            // new("UnityEditor.CoreModule", "UnityEditor.AddComponent.AddComponentWindow", "OnItemSelected", null),
            // new("UnityEditor.CoreModule", "UnityEditor.AddComponent.AddComponentDataSource", "GetSortedMenuItems", null),
            // new("UnityEditor.CoreModule", "UnityEditor.AddComponent.ComponentDropdownItem", ".ctor",
            //     new[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(bool) }),
            // new("UnityEditor.CoreModule", "UnityEditor.ObjectListArea+LocalGroup", "RefreshBuiltinResourceList", null),
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