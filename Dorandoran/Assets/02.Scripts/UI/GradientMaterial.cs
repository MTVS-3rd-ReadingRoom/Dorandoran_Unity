using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientMaterial : MonoBehaviour
{
    public Gradient gradient;

    [Range(0, 1)]
    public float t;

    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }
    private void Update()
    {

        image.color = gradient.Evaluate(t);
    }

}
