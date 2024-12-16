using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JournalScript : MonoBehaviour, IPointerDownHandler
{
    public float moveSpeed = 100f; // Speed at which the children move upward
    public float duration = 3f;

    public Boolean isUp = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        float incrementY = 60f;

        if (isUp)
        {
            foreach (Transform child in transform)
            {
                Vector3 childLocalPosition = child.localPosition;
                child.localPosition = new Vector3(childLocalPosition.x, childLocalPosition.y - incrementY, childLocalPosition.z);
            }
            
        }
        else
        {
            foreach (Transform child in transform)
            {
                Vector3 childLocalPosition = child.localPosition;
                child.localPosition = new Vector3(childLocalPosition.x, childLocalPosition.y + incrementY, childLocalPosition.z);
            }
        }
        isUp = !isUp;
    }
}

