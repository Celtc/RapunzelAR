using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterLocomotion))]
public class CustomCharacterController : MonoBehaviour
{
    #region Variables (private)

    private CharacterLocomotion _charLocomotion;

    #endregion

    #region Properties (public)

    #endregion

    #region Unity event functions

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        // Extrae el componente encargado de mover el pj
        _charLocomotion = GetComponent<CharacterLocomotion>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Registra el input
        var input = new CharacterInput(GetInput(), Input.GetAxis("A") != 0, false);

        // Envia el input
        _charLocomotion.SetInput(input);
    }

    #endregion

    #region Methods

    private IntVector3 GetInput()
    {
        // Cachea el input
        var hAxis = (int)Input.GetAxis("Horizontal");
        var vAxis = (int)Input.GetAxis("Vertical");

        // Normaliza el vector de input
        IntVector3 inputVector = null;
        if (hAxis != 0 || vAxis != 0)
        {
            inputVector = new IntVector3(
                (int)((hAxis != 0 ? 1 : 0) * Mathf.Sign(hAxis)), 
                0, 
                (int)((vAxis != 0 ? 1 : 0) * Mathf.Sign(vAxis))
            );
        }

        // Devuelve si hubo input
        return inputVector;
    }
        
    #endregion
}
