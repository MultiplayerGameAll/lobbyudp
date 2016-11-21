using UnityEngine;
using System.Collections;
using System;

public class JsonHelper
{
    public static Product[] FromJson(string json)
    {
        WrapperProductDto wrapper = UnityEngine.JsonUtility.FromJson<WrapperProductDto>(json);
        Debug.Log(wrapper);
        return wrapper.productDto;
    }

    public static string ToJson<T>(Product[] array)
    {
        WrapperProductDto wrapper = new WrapperProductDto();
        wrapper.productDto = array;
        return UnityEngine.JsonUtility.ToJson(wrapper);
    }

    [Serializable]
    private class WrapperProductDto
    {
        public Product[] productDto;
    }
}
