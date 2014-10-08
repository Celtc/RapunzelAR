using UnityEngine;
using System.Collections;

[System.Serializable]
public class AnimationCustomCurves
{
    public AnimationCustomCurves(string name, AnimationCurve deltaX, AnimationCurve deltaY, AnimationCurve deltaZ)
    {
        this.name = name;
        this.nameHash = Animator.StringToHash(name);
        this.deltaX = deltaX;
        this.deltaY = deltaY;
        this.deltaZ = deltaZ;
    }

    public AnimationCustomCurves(string name, int nameHash, AnimationCurve deltaX, AnimationCurve deltaY, AnimationCurve deltaZ)
    {
        this.name = name;
        this.nameHash = nameHash;
        this.deltaX = deltaX;
        this.deltaY = deltaY;
        this.deltaZ = deltaZ;
    }

    public string name;
    public int nameHash;
    public AnimationCurve deltaX;
    public AnimationCurve deltaY;
    public AnimationCurve deltaZ;
}
