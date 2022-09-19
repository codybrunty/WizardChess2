using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareSelectorCreator : MonoBehaviour{

    [SerializeField] private Material freeSquareMaterial;
    [SerializeField] private Material opponentSquareMaterial;
    [SerializeField] private GameObject selectorPrefab;
    private List<GameObject> instantiateSelectors = new List<GameObject>();

    public void ShowSelection(Dictionary<Vector3,bool> squareData) {
        ClearSelection();
        foreach (var data in squareData) {
            GameObject selector = Instantiate(selectorPrefab, data.Key, Quaternion.identity);
            instantiateSelectors.Add(selector); 
            foreach(var setter in selector.GetComponentsInChildren<MaterialSetter>()) {
                setter.SetSingleMaterial(data.Value?freeSquareMaterial:opponentSquareMaterial);
            }
        }
    }
    public void ClearSelection() {
        foreach (var selector in instantiateSelectors) { 
            Destroy(selector);
        }
        instantiateSelectors.Clear();
    }
}
