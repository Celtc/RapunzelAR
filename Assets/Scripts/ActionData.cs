using UnityEngine;
using System.Collections;

[System.Serializable]
public class ActionData
{
    #region Variables (private)

    [SerializeField]
    private string _name;
    [SerializeField]
    private float _duration;
    [SerializeField]
    private AnimationCurve _deltaX;
    [SerializeField]
    private AnimationCurve _deltaY;
    [SerializeField]
    private AnimationCurve _deltaZ;
    [SerializeField]
    private AnimationCurve _deltaR;
    [SerializeField]
    private bool _rootMotion;

    #endregion

    #region Properties (public)

    public string Name { get { return _name; } }
    public float duration { get { return _duration; } }
    public AnimationCurve deltaX { get { return _deltaX; } }
    public AnimationCurve deltaY { get { return _deltaY; } }
    public AnimationCurve deltaZ { get { return _deltaZ; } }
    public AnimationCurve deltaR { get { return _deltaR; } }
    public bool RootMotion { get { return _rootMotion; } }

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
    
    public ActionData(string name, float duration)
    {
        this._name = name;
        this._duration = duration;
        this._deltaX = this._deltaY = this._deltaZ = this._deltaR = EmptyCurve();
        this._rootMotion = false;
    }

    #endregion
}