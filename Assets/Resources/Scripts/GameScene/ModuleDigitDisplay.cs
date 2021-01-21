using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class ModuleDigitDisplay : MonoBehaviour
{
    public TextMeshPro digit_template;
    public Vector3 move_offset = new Vector3(0.0f, 1.0f, 0.0f);
    public float time_duration = 2.0f;
    List<TextMeshPro> nodes = new List<TextMeshPro>();
    List<TextMeshPro> pools = new List<TextMeshPro>();
    void Start()
    {

    }
    public void AddDigit(string msg)
    {
        TextMeshPro node = null;
        if (0 < pools.Count)
        {
            node = pools[pools.Count - 1];
            pools.RemoveAt(pools.Count - 1);
            node.gameObject.SetActive(true);
        }
        else {
            node = Instantiate(digit_template, transform);
        }
        node.transform.position = transform.position;
        node.text = msg;
        var sequence = DOTween.Sequence();
        sequence.Insert(0.0f, node.transform.DOMove(node.transform.position + move_offset, time_duration).SetEase(Ease.Linear));
        //sequence.Insert(0.0f, node.GetComponent<MeshRenderer>().material.DOFade(0.0f, time_duration).SetEase(Ease.Linear));
        sequence.onComplete = () => { node.gameObject.SetActive(false); pools.Add(node); };
        sequence.SetAutoKill(true);
    }
}
