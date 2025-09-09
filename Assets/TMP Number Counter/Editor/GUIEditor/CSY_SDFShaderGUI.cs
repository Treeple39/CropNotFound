using UnityEngine;
using UnityEditor;

namespace CSY_Editor
{
    public class CSY_SDFShaderGUI : TMPro.EditorUtilities.TMP_SDFShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            base.OnGUI(materialEditor, properties);

            GUILayout.Label("Custom Properties", EditorStyles.boldLabel);

            MaterialProperty _WaveStrength = FindProperty("_WaveStrength", properties);
            materialEditor.ShaderProperty(_WaveStrength, _WaveStrength.displayName);

            MaterialProperty _WaveFreq = FindProperty("_WaveFreq", properties);
            materialEditor.ShaderProperty(_WaveFreq, _WaveFreq.displayName);

            MaterialProperty _WaveSpeed = FindProperty("_WaveSpeed", properties);
            materialEditor.ShaderProperty(_WaveSpeed, _WaveSpeed.displayName);
        }
    }
}
