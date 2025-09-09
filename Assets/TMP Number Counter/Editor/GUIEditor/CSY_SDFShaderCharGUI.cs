using UnityEditor;

namespace CSY_Editor
{
    public class CSY_SDFShaderCharGUI : CSY_SDFShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            base.OnGUI(materialEditor, properties);

            MaterialProperty _CharNum = FindProperty("_CharNum", properties);
            materialEditor.ShaderProperty(_CharNum, _CharNum.displayName);
        }
    }
}
