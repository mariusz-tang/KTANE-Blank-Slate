﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrailleDisplay : MonoBehaviour {

    [SerializeField] private BrailleDot[] _brailleDots;

    public void Display(int[] positions, Color colour) {
        foreach (BrailleDot dot in _brailleDots) {
            if (Array.Exists(positions, p => p == dot.Position)) {
                dot.Renderer.enabled = true;
                dot.Renderer.material.SetColor("_OutlineColor", colour);
            }
            else {
                dot.Renderer.enabled = false;
            }
        }
    }

    public void Clear() {
        Array.ForEach(_brailleDots, d => d.Renderer.enabled = false);
    }
}
