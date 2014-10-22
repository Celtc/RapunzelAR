using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Character))]
public class CharacterStadistics : MonoBehaviour
{
    #region Variables (private)

    [SerializeField]
    private int _moves = 0;

    [SerializeField]
    private float _timer = 0f;

    #endregion

    #region Propiedades (public)

    public int Moves
    {
        get { return _moves; }
    }

    public float Timer
    {
        get { return _timer; }
    }

    #endregion

    #region Funciones de evento de Unity

    /// <summary>
    /// Llamado siempre al inicializar el componente.
    /// </summary>
    void Awake()
    {

    }

    /// <summary>
    /// Llamado al inicializar el componente, si MonoBehaviour esta habilitado.
    /// </summary>
    void Start()
    {
        // Registra los eventos
        var character = GetComponent<Character>();
        character.RegisterPushDel(() => { if (this.enabled) _moves++; });
        character.RegisterPullDel(() => { if (this.enabled) _moves++; });
    }

    /// <summary>
    /// Update es llamado una vez por frame, si MonoBehaviour esta habilitado.
    /// </summary>
    void Update()
    {
        // Incrementa el timer
        _timer += Time.deltaTime;
    }

    #endregion

    #region Metodos

    #endregion
}
