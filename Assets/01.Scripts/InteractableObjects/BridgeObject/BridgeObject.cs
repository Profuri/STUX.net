using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BridgeObject : MonoBehaviour
{
    public void Move(Vector3 endValue, float duration, bool remove = false)
    {
        transform.DOMove(endValue, duration).SetEase(Ease.InOutFlash)
            .OnComplete(() =>
            {
                if(remove)
                    Destroy(this.gameObject);
            });
        
    }
}