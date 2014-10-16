using UnityEngine;
using System;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
	#region Variables (private)

    [SerializeField]
    private float _fadeSpeed = 1.5f;
    [SerializeField]
    private Texture _blackTexture;

    private Action _action;

    private bool _inout = false;
    [SerializeField]
    private float _alphaValue = 0f;
    private bool _screenFaded = false;
    private FadeMode _fadeMode = FadeMode.FadeToBlack;

	#endregion

	#region Propiedades (public)
    
    public float FadeSpeed
    {
        get { return _fadeSpeed; }
        set { _fadeSpeed = value; }
    }

    public Action Action
    {
        get { return _action; }
        set { _action = value; }
    }

	#endregion

	#region Funciones de evento de Unity
    
	/// <summary>
    /// Llamado al inicializar el componente, si MonoBehaviour esta habilitado.
	/// </summary>
	void Start ()	
	{
        this.enabled = false;
	}

	/// <summary>
    /// Update es llamado una vez por frame, si MonoBehaviour esta habilitado.
	/// </summary>
	void OnGUI() 
	{
        Fade();
	}

	#endregion

	#region Metodos
    
    public void FadeInOut()
    {
        this.enabled = true;
        this._inout = true;
        this._fadeMode = FadeMode.FadeToBlack;
    }

    public void FadeToBlack()
    {
        this.enabled = true;
        this._fadeMode = FadeMode.FadeToBlack;
    }

    public void FadeToClear()
    {
        this._fadeMode = FadeMode.FadeToClear;
    }

    private void Fade()
    {
        // Obtiene el nueva alpha
        var deltaAlpha = Time.deltaTime / _fadeSpeed;
        if (_fadeMode == FadeMode.FadeToBlack)
            _alphaValue += deltaAlpha;
        else
            _alphaValue -= deltaAlpha;
        _alphaValue = Mathf.Clamp01(_alphaValue);

        // Cambia el alpha a la textura y la dibuja
        GUI.color = new Color(_alphaValue, _alphaValue, _alphaValue, _alphaValue);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _blackTexture);

        // Determina si la pantalla se encuentra totalmente opaca
        if (_alphaValue == 1f)
        {
            // Ejecuta una unica vez cuando acaba de volverse oscura
            if (_screenFaded == false)
            {
                if (_action != null) _action();
                if (_inout) _fadeMode = FadeMode.FadeToClear;
                _screenFaded = true;
            }
        }
        else
        {
            // La pantalla no esta totalmente oscura
            _screenFaded = false;
        }

        // Si el alpha es 0 se auto desactiva
        if (_alphaValue == 0f)
            this.enabled = false;
    }

	#endregion

    enum FadeMode
    {
        FadeToClear,
        FadeToBlack,
    }
}
