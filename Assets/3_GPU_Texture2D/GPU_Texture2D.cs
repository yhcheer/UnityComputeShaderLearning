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

    Color[] colors;

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

        colors = tex.GetPixels();

        Debug.Log("Get Pixels done");

        SetPixels(colors);

        Debug.Log("Set Pixels done");

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
        //for(int i = 0; i < colors.Length; i++)
        //{
        //    inputDatas[i].color = colors[i];
        //}
        Apply();
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
