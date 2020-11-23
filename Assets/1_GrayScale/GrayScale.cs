using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 输入贴图 使用ComputeShader转为灰度图
/// </summary>
public class GrayScale : MonoBehaviour
{

    public Texture2D inputTexture;
    public RenderTexture outputRT;

    public ComputeShader shader;

    // Start is called before the first frame update
    void Start()
    {
        RenderTexture rt = new RenderTexture(inputTexture.width, inputTexture.height, 24);
        rt.enableRandomWrite = true;
        rt.Create();

        outputRT = new RenderTexture(inputTexture.width, inputTexture.height, 24);
        outputRT.enableRandomWrite = true;
        outputRT.Create();


        int kernel = shader.FindKernel("CSMain");
        shader.SetTexture(kernel, "inputTexture", inputTexture);
        shader.SetTexture(kernel, "outputTexture", outputRT);
        shader.Dispatch(kernel, inputTexture.width, inputTexture.height, 1);
    }

}
