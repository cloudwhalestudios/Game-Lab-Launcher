using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollImage : MonoBehaviour
{

    public float scrollSpeed;
    public float speed;
    public Vector3 startOffset;
    public float startPoint;
    public float endPoint;

    private void Start()
    {
        transform.position += startOffset;
    }

    void Update()
    {
        if (((RectTransform)transform).pivot.x <= endPoint)
        {
            transform.position = new Vector3(startPoint, transform.position.y, transform.position.z);
        }
        
        else
        {
            speed = scrollSpeed * Time.deltaTime;

            transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);
        }
    }
}
