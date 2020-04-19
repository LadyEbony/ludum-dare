using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldParent : MonoBehaviour
{
    private static WorldParent _instance;

    public static WorldParent instance
    {
        get
        {
            if (_instance) return _instance;
            _instance = FindObjectOfType<WorldParent>();
            if (_instance) return _instance;
            throw new System.Exception("Network manager not instanced in scene");
        }
        set
        {
            _instance = value;
        }
    }

}
