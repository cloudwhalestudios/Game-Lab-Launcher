using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Only works horizontally for now
public class Parallax : MonoBehaviour
{
    [Serializable]
    public class ParallaxTransform
    {
        public RectTransform transform;
        public float speed;
        [HideInInspector] public float startX;
        [HideInInspector] public bool copied = false;
        [HideInInspector] public bool kill = false;
    }
    public RectTransform canvas;
    public float spawnBuffer = 5f;
    public List<ParallaxTransform> parallaxTransforms = new List<ParallaxTransform>();

    List<ParallaxTransform> activeParallaxTransforms;

    // Start is called before the first frame update
    void Start()
    {
        activeParallaxTransforms = new List<ParallaxTransform>(parallaxTransforms);
        foreach (var pt in activeParallaxTransforms)
        {
            pt.startX = pt.transform.localPosition.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var pt in activeParallaxTransforms.ToArray())
        {
            // move
            var pos = pt.transform.localPosition;
            pos.x += pt.speed * Time.unscaledDeltaTime;
            pt.transform.localPosition = pos;

            // handle localPosition
            if (pt.copied && ParallaxTransformCrossedScreen(pt, false))
            {
                CleanupParallaxTransform(pt);
            }
            else if (!pt.copied && ParallaxTransformCrossedScreen(pt))
            {
                pt.copied = true;
                InstantiateCopy(pt);
            }
        }

        activeParallaxTransforms.RemoveAll(x => x.kill);
    }

    bool ParallaxTransformCrossedScreen(ParallaxTransform pt, bool useScreenWidth = true)
    {
        var canvasSize = (useScreenWidth) ? canvas.sizeDelta.x : 0;
        if (pt.speed < 0)
        {
            return pt.transform.localPosition.x <= pt.startX - pt.transform.sizeDelta.x + canvasSize + spawnBuffer;
        }
        else
        {
            return pt.transform.localPosition.x >= pt.startX + pt.transform.sizeDelta.x - canvasSize - spawnBuffer;
        }
    }

    void InstantiateCopy(ParallaxTransform pt)
    {
        // New Par
        var newParallaxTransform = new ParallaxTransform();
        newParallaxTransform.transform = Instantiate(pt.transform.gameObject, pt.transform.parent).GetComponent<RectTransform>();

        var xOffset = -Mathf.Sign(pt.speed) * (pt.transform.sizeDelta.x + spawnBuffer);
        newParallaxTransform.transform.localPosition = pt.transform.localPosition + new Vector3(xOffset, 0);

        // flip
        newParallaxTransform.transform.localScale = new Vector3(-pt.transform.localScale.x, 1, 1);

        newParallaxTransform.startX = pt.startX;
        newParallaxTransform.speed = pt.speed;
        newParallaxTransform.transform.name = pt.transform.name;

        activeParallaxTransforms.Add(newParallaxTransform);
    }

    void CleanupParallaxTransform(ParallaxTransform pt)
    {
        pt.kill = true;
        DestroyImmediate(pt.transform.gameObject);
    }
}
