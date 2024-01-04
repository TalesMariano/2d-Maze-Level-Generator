using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//https://forum.unity.com/threads/multiple-bools-in-one-line-in-inspector.181778/
//https://docs.unity3d.com/Manual/editor-CustomEditors.html
// [] https://discussions.unity.com/t/how-to-change-inspector-with-non-monobehaviour-objects/216204/2
public class TestMapNode : MonoBehaviour
{
    public Vector3 lookAtPoint = Vector3.zero;

    public bool up;
    public bool right;
    public bool down;
    public bool left;

    void Update()
    {
        transform.LookAt(lookAtPoint);
    }

    public bool IsPalindrome(string s)
    {
        var chars = s.ToCharArray();


        int pointer_1 = 0;
        int pointer_2 = chars.Length - 1;

        while (pointer_1 < pointer_2)
        {
            if (!char.IsLetterOrDigit(chars[pointer_1]))
            {
                pointer_1++;
            }
            else if (!char.IsLetterOrDigit(chars[pointer_2]))
            {
                pointer_2--;
            }
            else
            {
                if (char.ToLower(chars[pointer_1]) != char.ToLower(chars[pointer_2]))
                    return false;

                pointer_1++;
                pointer_2--;
            }

        }
        return true;
    }

    public int[] TwoSum(int[] numbers, int target)
    {
        int pointer1 = 0;
        int pointer2 = numbers.Length - 1;

        while (pointer1 < pointer2)
        {
            if (numbers[pointer1] + numbers[pointer2] == target)
                return new int[] { pointer1+1, pointer2+1 };


            else if (numbers[pointer1] + numbers[pointer2] > target)
                pointer2--;
            else if (numbers[pointer1] + numbers[pointer2] < target)
                pointer1++;
        }

        return new int[] { pointer1 + 1, pointer2 + 1 };
    }




}
