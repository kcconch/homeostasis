using System;
using UnityEngine;

public enum CameraDirection
{
    ZPlus,
    ZMinus,
    YPlus,
    YMinus,
}

public class BigScreenCamera : MonoBehaviour
{
    [Range(1, 6)]
    public int NumScreens = 4;

    public int ScreenWidth = 1920;
    public int ScreenHeight = 690;

    public float Scale = 1;

    public float Depth = 1000;
    public CameraDirection Direction = CameraDirection.ZPlus;

    private int TotalWidth;

    private Camera _cam;

    private void Awake()
    {
        TotalWidth = ScreenWidth * NumScreens;
        InitializeCamera();
    }

    // Set up the camera to have the proper size and resolution
    private void InitializeCamera()
    {
        // Get the camera we're going to initialize...
        // First we're going to check if there's a camera attached to the same object as this script...
        _cam = GetComponent<Camera>();

        // If there isn't then check to see if there's a camera tagged as the main camera in the scene...
        if (_cam == null)
        {
            _cam = Camera.main;
        }

        // If we can't find a camera then bail out
        if (_cam == null)
        {
            throw new MissingComponentException("Could not find a camera");
        }

        var invScale = 1.0f / Scale;

        // Otherwise set up the dimensions...
        _cam.rect = new Rect(0, 0, TotalWidth * invScale, ScreenHeight * invScale);
        _cam.orthographic = true;
        _cam.orthographicSize = ScreenHeight * 0.5f * invScale;
        _cam.nearClipPlane = 0;
        _cam.farClipPlane = Depth;

        switch (Direction)
        {
            case CameraDirection.ZPlus:
                _cam.transform.position = new Vector3(0, 0, -Depth * 0.5f);
                break;
            case CameraDirection.ZMinus:
                _cam.transform.position = new Vector3(0, 0, Depth * 0.5f);
                break;
            case CameraDirection.YPlus:
                _cam.transform.position = new Vector3(0, -Depth * 0.5f, 0);
                break;
            case CameraDirection.YMinus:
                _cam.transform.position = new Vector3(0, Depth * 0.5f, 0);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        transform.LookAt(Vector3.zero);

        // And set the resolution
        Screen.SetResolution(TotalWidth, ScreenHeight, false);
    }

    private void OnValidate()
    {
        InitializeCamera();
    }

    private void Reset()
    {
        InitializeCamera();
    }

}