using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public LineRenderer circleRenderer;

    void Start()
    {
        DrawCircle(100, 0.18f);
    }

    void DrawCircle(int steps, float radius)
    {
        circleRenderer.positionCount = steps;

        for (int currentStep = 0; currentStep < steps; currentStep++)
        {
            float circumferenceProgress = (float)currentStep / steps;

            float currentRadian = circumferenceProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float zScaled = Mathf.Sin(currentRadian);

            float x = xScaled * radius;
            float z = zScaled * radius;

            Vector3 currentPosition = new Vector3(x, 0, z);

            circleRenderer.SetPosition(currentStep, currentPosition);
        }
    }
}
