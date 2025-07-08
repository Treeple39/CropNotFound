using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;

public class ScoreAdd : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshPro textMesh;

    public void ShowScore(int score, int addMod = 0)
    {
        textMesh.gameObject.SetActive(true);
        textMesh.text = "+" + score.ToString();

        if (textMesh.enableVertexGradient == true) 
        {
            Color _Color1 = Color.white;
            Color _Color2 = Color.grey;

            switch (addMod)
            {
                case 0:
                    ColorUtility.TryParseHtmlString("#FFD800FF", out _Color1);
                    ColorUtility.TryParseHtmlString("#FF3F00FF", out _Color2);
                    break;
                case 1:
                    ColorUtility.TryParseHtmlString("#00FFDEFF", out _Color1);
                    ColorUtility.TryParseHtmlString("#FF00C9FF", out _Color2);
                    break;
                default:
                    break;
            }

            VertexGradient gradient = new VertexGradient(
                _Color1,    // ���Ͻ���ɫ
                _Color1,    // ���Ͻ���ɫ
                _Color2,    // ���½���ɫ
                _Color2     // ���½���ɫ
            );

            textMesh.colorGradient = gradient;
        }

        anim.SetInteger("addMod",addMod);
    }
}
