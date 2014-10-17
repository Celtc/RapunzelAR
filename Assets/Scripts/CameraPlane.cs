using UnityEngine;
using System.Collections;

public class CameraPlane
{
    //Builder
    public CameraPlane(Camera cam, float distance)
    {
        _lowerLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, distance));
        _lowerRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth - 1, 0, distance));
        _upperLeft = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight - 1, distance));
        _upperRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth - 1, cam.pixelHeight - 1, distance));

        _width = Vector3.Distance(_upperRight, _upperLeft);
        _height = Vector3.Distance(_lowerRight, _upperRight);
    }

    private Vector3 _upperLeft;
    public Vector3 UpperLeft
    {
        get { return _upperLeft; }
    }

    private Vector3 _upperRight;
    public Vector3 UpperRight
    {
        get { return _upperRight; }
    }

    private Vector3 _lowerLeft;
    public Vector3 LowerLeft
    {
        get { return _lowerLeft; }
    }

    private Vector3 _lowerRight;
    public Vector3 LowerRight
    {
        get { return _lowerRight; }
    }

    private float _height;
    public float Height
    {
        get { return _height; }
    }

    private float _width;
    public float Width
    {
        get { return _width; }
    }
}
