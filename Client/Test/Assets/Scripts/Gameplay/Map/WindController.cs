using UnityEngine;
using UnityEditor;

[ExecuteAlways] // 编辑器模式下也生效
public class WindController : MonoBehaviour
{
    public Material mat;      // 需要控制的材质
    public bool useWindTex;   // Inspector 上开关

    private bool lastUseWindTex; // 上一次的状态
    void Update()
    {
        if (mat == null) return;
        if(lastUseWindTex == useWindTex)return; // 状态未改变，跳过

        lastUseWindTex = useWindTex; // 更新状态
        mat.SetFloat("_UseWindTex", useWindTex ? 1 : 0);
    }
}