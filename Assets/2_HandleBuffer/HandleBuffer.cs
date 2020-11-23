using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 输入数组 使用ComputeShader使数组翻倍
/// </summary>
public class HandleBuffer : MonoBehaviour
{
    public ComputeShader shader;

    struct data
    {
        public float a;
        public float b;
        public float c;
    }

    data[] input;
    data[] output;


    // Start is called before the first frame update
    void Start()
    {
        input = new data[3];
        output = new data[3];

        Debug.Log("输入结果");
        for (int i = 0; i < input.Length; i++)
        {
            input[i].a = i * 3 + 1;
            input[i].b = i * 3 + 2;
            input[i].c = i * 3 + 3;
            Debug.Log(input[i].a + "" + input[i].b + "" + input[i].c);
        }

        ComputeBuffer inputBuffer = new ComputeBuffer(input.Length, 12);// 3个float 共12字节
        ComputeBuffer outputBuffer = new ComputeBuffer(output.Length, 12);

        int kernel = shader.FindKernel("CSMain");

        // data => buffer
        inputBuffer.SetData(input);

        // buffer cpu => gpu
        shader.SetBuffer(kernel, "inputDatas", inputBuffer);
        shader.SetBuffer(kernel, "outputDatas", outputBuffer);

        // Dispatch
        shader.Dispatch(kernel, input.Length, 1, 1);

        // buffer => data
        outputBuffer.GetData(output);

        Debug.Log("输出结果");
        for (int i = 0; i < output.Length; i++)
        {
            Debug.Log(output[i].a + "" + output[i].b + "" + output[i].c);
        }

        // release buffer
        inputBuffer.Release();
        outputBuffer.Release();
    }


}
