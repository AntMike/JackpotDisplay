using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class SetValue : MonoBehaviour
{
    public float duration = 1;

    [SerializeField] private int nextValue = 0;
    [SerializeField] private TMP_Text text;

    #region Test params
    [Header("Test field")]
    [SerializeField] private bool setNewValue = false;


    private void Update()
    {
        if (setNewValue)
        {
            setNewValue = false;
            SetValueToText(nextValue);
        }
    }
    #endregion


    /// <summary>
    /// Set num to text component
    /// </summary>
    /// <param name="num"></param>
    public void SetValueToText(int num)
    {
        nextValue = num;
        StartCoroutine(SetValueCor());
    }

    private IEnumerator SetValueCor()
    {
        float t = 0;
        while (t<=duration)
        {
            t += Time.deltaTime;
            text.text = SetDotToNumber(Mathf.Lerp(0f, nextValue, t / duration));
            yield return new WaitForEndOfFrame();
        }
        text.text = SetDotToNumber(nextValue);
    }

    //replace ' ' symbol (space) to dot
    private string SetDotToNumber(float num)
    {
        var endResult = $"{num:#,0}";
        endResult = endResult.Replace(' ', '.');
        return endResult;
    }
}
