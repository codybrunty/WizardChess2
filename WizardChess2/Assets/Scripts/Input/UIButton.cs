using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIInputReceiver))]
public class UIButton : Button{

    private InputReceiver inputReceiver;

    protected override void Awake() {
        base.Awake();
        inputReceiver = GetComponent<UIInputReceiver>();
        onClick.AddListener(()=>inputReceiver.OnInputReceived());
    }

}
