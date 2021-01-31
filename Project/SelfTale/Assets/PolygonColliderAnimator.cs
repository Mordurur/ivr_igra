using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonColliderAnimator : MonoBehaviour
{
    [SerializeField] PolygonCollider2D[] colliders;
    private int currentColliderIndex;

    public void SetColliderForSprite(int spriteNum)
    {
        colliders[currentColliderIndex].enabled = false;
        currentColliderIndex = spriteNum;
        colliders[currentColliderIndex].enabled = true;
    }
}
