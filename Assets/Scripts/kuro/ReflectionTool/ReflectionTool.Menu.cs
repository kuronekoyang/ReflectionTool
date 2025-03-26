using UnityEditor;

namespace kuro.ReflectionTool
{
    public partial class ReflectionTool
    {
        [MenuItem("Tools/Reflection Tool/Generate Wrap", priority = 211)]
        private static void GenerateWrap()
        {
            try
            {
                AssetDatabase.DisallowAutoRefresh();
                new GenWrapTool().Execute();
            }
            finally
            {
                AssetDatabase.AllowAutoRefresh();
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Reflection Tool/Generate Hook", priority = 212)]
        private static void GenerateHook()
        {
            try
            {
                AssetDatabase.DisallowAutoRefresh();
                new GenHookTool().Execute();
            }
            finally
            {
                AssetDatabase.AllowAutoRefresh();
            }

            AssetDatabase.Refresh();
        }
    }
}