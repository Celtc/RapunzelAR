using UnityEngine;
using System;
using System.Collections;
using MMT;

public class MobilePlayVideoBehaviour : MonoBehaviour
{
    public void Start()
    {
        CreatePlane();

        AttachPlaybackComponent();
    }

    private void CreatePlane()
    {
        var cam = Camera.main;
        var planePoints = new CameraPlane(cam, cam.farClipPlane - 1f);
        var rotation = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);

        renderPlane = PrimitiveCreatorUtil.CreatePlane("VideoRenderPlane", planePoints.LowerLeft, rotation, PrimitiveCreatorUtil.Orientation.Vertical, 1, 1,
            planePoints.Width, planePoints.Height, PrimitiveCreatorUtil.PlaneAnchorPoint.BottomLeft, false, movieMaterial);
        renderPlane.transform.parent = cam.transform;
    }

    private void AttachPlaybackComponent()
    {
        var movieText = renderPlane.AddComponent<MobileMovieTexture>();
        movieText.Path = this.moviePath;
        movieText.MovieMaterial = new Material[] { this.movieMaterial };
        movieText.PlayAutomatically = this.playAutomatically;
    }

    public string moviePath;
    public Material movieMaterial;
    public bool playAutomatically = true;
    public bool loop = true;

    private GameObject renderPlane;
}