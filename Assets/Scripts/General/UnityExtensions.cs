﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

static public class UnityExtensions
{
    /// <summary>
    /// Returns a new Vector3 formed by scaling this Vector3 by another.
    /// </summary>
    static public Vector3 ScaledBy(this Vector3 thisVector, Vector3 otherVector)
    {
        return Vector3.Scale(thisVector, otherVector);
    }

    /// <summary>
    /// Returns the squared area of the vector, i.e., the product of its two
    /// axis values.
    /// </summary>
    static public float Area(this Vector2 vector)
    {
        return vector.x * vector.y;
    }
    /// <summary>
    /// Returns the squared area of the vector, i.e., the product of its two
    /// axis values.
    /// </summary>
    static public float Area(this Vector2Int vector)
    {
        return vector.x * vector.y;
    }

    /// <summary>
    /// Toggles the enabled state of the given keyword to the opposite of its
    /// current state.
    /// </summary>
    static public void ToggleKeyword(this Material material, string keyword)
    {
        bool isEnabled = material.IsKeywordEnabled(keyword);
        material.ToggleKeyword(keyword, !isEnabled);
    }

    /// <summary>
    /// Toggles the enabled state of the given keyword to the given state.
    /// </summary>
    static public void ToggleKeyword(this Material material, string keyword, bool doEnable)
    {
        if (doEnable) {
            material.EnableKeyword(keyword);
        }
        else {
            material.DisableKeyword(keyword);
        }
    }

    /// <summary>
    /// Calls <code>GetComponent</code> to retrieve the component of the
    /// specified type within the GameObject, but throws an error if no
    /// matching object exists.
    /// </summary>
    public static T GetRequiredComponent<T>(this MonoBehaviour obj) where T : MonoBehaviour
    {
        T component = obj.GetComponent<T>();
        if (component == null) {
            Debug.LogErrorFormat(obj, "Expected to find component of type \"{0}\" but found none", typeof(T));
        }
        return component;
    }
}

static public class UnityUtils
{
    static public void Log(string message, params object[] formatArgs)
    {
        Debug.Log(message.Format(formatArgs));
    }
}