﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KModkit;
using UnityEngine;

public class TapCodeState : RuleStateController {

    // Do not account for K since it never appears in any word in the word list.
    private const string TAP_ALPHABET = "ABCDEFGHIJLMNOPQRSTUVWXYZ";
    private readonly string[][] _words = new string[][] {
        new string[] { "ROE", "LAW", "RAJ", "TEE", "NOT" },
        new string[] { "PHI", "JAY", "ORB", "MOL", "PUT" },
        new string[] { "RUE", "ONE", "LED", "NUN", "PAL" },
        new string[] { "ORE", "JUG", "SEE", "PEA", "LEG" },
        new string[] { "YES", "NOW", "TED", "HEN", "SAC" },
        new string[] { "WEE", "WRY", "MAC", "SON", "NIL" },
        new string[] { "RIB", "YEN", "TRY", "ZOO", "PIT" },
    };

    private string _tappedWord;
    private int _originRegionNumber;
    private int _targetRegionNumber;

    private Coroutine _playingTapCode;

    public override IEnumerator OnStateEnter(Region pressedRegion) {
        yield return null;
        _originRegionNumber = pressedRegion.Position;
        _module.Log($"Region {_originRegionNumber} is playing tap code.");
        _targetRegionNumber = _module.AvailableRegions.PickRandom();

        int forwardDistance = (_targetRegionNumber - pressedRegion.Position + 8) % 8;
        _tappedWord = _words[forwardDistance - 1].PickRandom();

        _module.Log($"The word being transmitted is {_tappedWord}.");
        _module.Log($"The corresponding region is press is {_targetRegionNumber}.");

        transform.position = pressedRegion.transform.position;
        _playingTapCode = StartCoroutine(PlayTapCode());
    }

    public override IEnumerator HandleRegionPress(Region pressedRegion) {
        int pressedPosition = pressedRegion.Position;

        StopPlaying();

        if (pressedPosition != _originRegionNumber) {
            if (pressedPosition == _targetRegionNumber) {
                // ! _module.GetNewState(pressedRegion);
                _module.Log("Correct!");
            }
            else {
                _module.Strike($"Incorrectly pressed region {pressedPosition}. Strike!");
                yield return new WaitForSeconds(0.5f);
                _playingTapCode = StartCoroutine(PlayTapCode());
            }
        }
        else {
            _playingTapCode = StartCoroutine(PlayTapCode());
        }
    }

    private IEnumerator PlayTapCode() {
        foreach (char letter in _tappedWord) {
            int position = TAP_ALPHABET.IndexOf(letter);
            int row = position / 5 + 1;
            int column = position % 5 + 1;

            for (int i = 0; i < row; i++) {
                _module.BombAudio.PlaySoundAtTransform("Tap", transform);
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < column; i++) {
                _module.BombAudio.PlaySoundAtTransform("Tap", transform);
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void StopPlaying() {
        if (_playingTapCode != null) {
            StopCoroutine(_playingTapCode);
        }
    }
}