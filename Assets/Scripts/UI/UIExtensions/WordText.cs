using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WordText: MonoBehaviour
{
    private Text text;
    public string str;
    public float time = 0.05f;

    private void Start()
    {
        text = transform.GetComponent<Text>();
        str = string.Empty;
        UpdateWordStr("�����澺�Ļ���ý\n����������");
    }

    public void UpdateWordStr(string str)
    {
        text.text = string.Empty;
        this.str = str;
        StartCoroutine("WordForWotdText");
    }

    private void Update()
    {

    }

    IEnumerator WordForWotdText()
    { 
        foreach (var item in str)
        {
            text.text += item;
            yield return new WaitForSeconds(time);
        }
    }


}
