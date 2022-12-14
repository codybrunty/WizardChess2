using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Board))]
public class BoardInputHandler : MonoBehaviour, IInputHandler{

    private Board board;
    private void Awake() {
        board = GetComponent<Board>();
    }

    public void ProccessInput(Vector3 inputPosition, GameObject selectedObject, Action callback) {
        board.OnSquareSelected(inputPosition);
    }



}
