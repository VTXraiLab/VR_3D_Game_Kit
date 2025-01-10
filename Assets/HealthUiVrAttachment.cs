using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUiVrAttachment : MonoBehaviour
{
    [SerializeField]
    GameObject attachPoint;

    [SerializeField]
    GameObject healthUi;

    [SerializeField]
    Vector3 offset;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //healthUi.transform.position = attachPoint.transform.position + offset;
        //PullHeartsCloser();
    }

    void PullHeartsCloser()
    {

        Canvas canvas = healthUi.GetComponentInChildren<Canvas>();
        foreach (RectTransform heart in canvas.gameObject.GetComponentsInChildren<RectTransform>())
        {

            heart.anchoredPosition = Vector3.zero;
        
        }
    }
}