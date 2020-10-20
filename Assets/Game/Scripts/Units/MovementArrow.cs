using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementArrow : MonoBehaviour
{
    public static MovementArrow Create(HexCell origin, HexCell destination)
    {
        var go = Instantiate(Resources.Load("UI/Movement Arrow") as GameObject).GetComponent<MovementArrow>();
        go.transform.parent = origin.transform;
        // Rotate the arrow to face upward and point towards the destination cell.
        go.transform.rotation = Quaternion.LookRotation(destination.transform.position - origin.transform.position);
        // Position the arrow between the orign &destination cells but raised above them(in the Y - axis).
        var newpos = (destination.transform.position - origin.transform.position) / 2;
        newpos.y = Math.Max(destination.transform.position.y, origin.transform.position.y) - go.transform.localScale.y;
        go.transform.localPosition = newpos;
        return go;
    }

    [SerializeField]
    SpriteMask spriteMask;

    void OnEnable()
    {  
        if (spriteMask == null) { throw new System.Exception("Missing required Sprite Mask component."); }
    }

    public void SetCompletion(float completionRatio)
    {
        spriteMask.alphaCutoff = completionRatio;
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}
