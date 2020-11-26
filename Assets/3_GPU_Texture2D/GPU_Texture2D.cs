using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Compute Shader 重写Texture2D的 getpixels setpixels 和 apply
/// </summary>
public class GPU_Texture2D : MonoBehaviour
{
    int width;
    int height;

    public Texture2D tex;
    public RenderTexture rt;

    public ComputeShader shader;
    public ComputeShader getPixelsShader;
    //public ComputeShader SetPixelsShader;

    // GetPixels获取的数据
    Color[] colors;

    // Apply输入的数据
    struct data
    {
        public Vector4 color;
    }
    data[] inputDatas;

    // Start is called before the first frame update
    void Start()
    {
        width = tex.width;
        height = tex.height;

        inputDatas = new data[width * height];

        // cpu version
        //colors = tex.GetPixels();
        // gpu version
        colors = GetPixels(tex);

        Debug.Log("Get Pixels done");

        // cpu version
        SetPixels(colors);
        // TODO:gpu version

        Debug.Log("Set Pixels done");

        Apply();
    }


    public void SetPixel(int x, int y, Color color)
    {
        int i = x + width * y;
        inputDatas[i].color = color;

    }

    public void SetPixels(Color[] colors)
    {
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                SetPixel(x, y, colors[x + y * width]);
            }
        }
    }

    Color[] GetPixels(Texture2D tex)
    {
        int width = tex.width;
        int height = tex.height;
        colors = new Color[width * height];

        // 申请一个outputBuffer来接取
        ComputeBuffer outputBuffer = new ComputeBuffer(colors.Length, 16);

        int kernel = getPixelsShader.FindKernel("CSMain");
        // cpu => gpu
        getPixelsShader.SetTexture(kernel, "inputTexture", tex);
        getPixelsShader.SetInt("width", width);
        // gpu => cpu 
        getPixelsShader.SetBuffer(kernel, "outputDatas", outputBuffer);

        // Dispatch
        getPixelsShader.Dispatch(kernel, width/8, height/8, 1);

        // 转换成Color[]
        outputBuffer.GetData(colors);
        // release buffer
        outputBuffer.Release();
        return colors;
    }

    public void Apply()
    {
        rt = new RenderTexture(width, height, 24);
        rt.enableRandomWrite = true;
        rt.Create();

        ComputeBuffer inputBuffer = new ComputeBuffer(inputDatas.Length, 16);

        int kernel = shader.FindKernel("CSMain");

        inputBuffer.SetData(inputDatas);

        shader.SetBuffer(kernel, "inputColors", inputBuffer);
        shader.SetInt("width", width);

        shader.SetTexture(kernel, "Result", rt);

        shader.Dispatch(kernel, width / 8, height / 8, 1);

        inputBuffer.Release();
    }
}
