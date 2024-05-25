using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static T[] ConcatenateArrays<T>(T[] array1, T[] array2)
    {
        int length1 = array1.Length;
        int length2 = array2.Length;
        T[] result = new T[length1 + length2];

        // Copy the first array into the result array
        Array.Copy(array1, 0, result, 0, length1);

        // Copy the second array into the result array
        Array.Copy(array2, 0, result, length1, length2);

        return result;
    }
}
