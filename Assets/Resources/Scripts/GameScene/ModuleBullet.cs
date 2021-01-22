using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ModuleBullet : MonoBehaviour
{
    public int attack = 0;
    public float speed = 2.0f;
    [HideInInspector]
    public Person target;
    public virtual void Start()
    {
        StartCoroutine(OnMoveToTarget());
    }

    IEnumerator OnMoveToTarget() {
        Tween action = null;
        while (null != target) {
            float distance = (target.transform.position - transform.position).magnitude;
            if (UtilityTool.block_size > distance) {
                break;
            }
            float time = distance / speed;
            if (null != action) {
                action.Kill();
                action = null;
            }
            action = transform.DOMove(target.transform.position, speed);
            yield return new WaitForSeconds(Mathf.Min(time, 2.0f));
        }
        OnAction();
        Destroy(gameObject);
    }

    protected virtual void OnAction() {
        if (null != target) {
            target.hp -= attack;
        }
    }
}
