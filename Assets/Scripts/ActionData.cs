using UnityEngine;
using System.Collections;

[System.Serializable]
public class ActionData
{
    #region Variables (private)

    [SerializeField]
    private float _baseDuration;
    [SerializeField]
    private AnimationCurve _deltaX;
    [SerializeField]
    private AnimationCurve _deltaY;
    [SerializeField]
    private AnimationCurve _deltaZ;
    [SerializeField]
    private AnimationCurve _deltaR;

    #endregion

    #region Properties (public)

    public float baseDuration { get { return _baseDuration; } }
    public AnimationCurve deltaX { get { return _deltaX; } }
    public AnimationCurve deltaY { get { return _deltaY; } }
    public AnimationCurve deltaZ { get { return _deltaZ; } }
    public AnimationCurve deltaR { get { return _deltaR; } }
    
    #endregion

    #region Metodos privados

    private AnimationCurve EmptyCurve()
    {
        var curve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
        curve.RemoveKey(1);
        curve.RemoveKey(0);
        return curve;
    }

    #endregion

    #region Metodos publicos

    public ActionData(float baseDuration)
    {
        this._baseDuration = baseDuration;
        this._deltaX = this._deltaY = this._deltaZ = this._deltaR = EmptyCurve();
    }
    
    #endregion
}