using JKFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITipsItem : MonoBehaviour
{
    [SerializeField] Image bg;
    [SerializeField] Text infoText;

    public void Init(string info)
    {
        infoText.text = info;
        // 显现出来
        StartCoroutine(Show());
    }

    IEnumerator Show()
    {
        Color bgColor = bg.color;
        bgColor.a = 0;
        Color textColor = infoText.color;
        textColor.a = 0;

        bg.color = bgColor;
        infoText.color = textColor;
        while (bgColor.a < 1)
        {
            yield return null;
            bgColor.a += Time.deltaTime;
            textColor.a += Time.deltaTime;
            bg.color = bgColor;
            infoText.color = textColor;
        }
        yield return CoroutineTool.WaitForSeconds(1.5f);
        while (bgColor.a > 0)
        {
            yield return null;
            bgColor.a -= Time.deltaTime;
            textColor.a -= Time.deltaTime;
            bg.color = bgColor;
            infoText.color = textColor;
        }
        this.GameObjectPushPool();
    }
}
