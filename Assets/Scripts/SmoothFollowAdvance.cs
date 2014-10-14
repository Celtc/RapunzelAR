using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Smooth Follow Advance")]
public class SmoothFollowAdvance : MonoBehaviour
{
	#region Variables (private)

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private BoolVector3 _translationFollow = new BoolVector3(true, true, true);
    [SerializeField]
    private bool _lookAtPlayer = true;

    [SerializeField]
    private Vector3 _offset = new Vector3(0f, 1.25f, -5f);
    [SerializeField]
    private Vector3 _lookAtOffset = new Vector3(0f, 1f, 0f);
    [SerializeField]
    private Vector3 _angles = new Vector3(30f, -15f, 0f);

    [SerializeField]
    private float _translationDamping = 2.0f;
    [SerializeField]
    private float _rotationDamping = 3.0f;

    private Vector3 _initialRotation;

	#endregion

	#region Propiedades (public)

	#endregion

	#region Funciones de evento de Unity

    /// <summary>
    /// Llamado al inicializar el componente, si MonoBehaviour esta habilitado.
    /// </summary>
    void Start()
    {
        _initialRotation = transform.eulerAngles;
    }

	/// <summary>
    /// LateUpdate es llamado una vez por frame luego del resto de las rutinas, si MonoBehaviour esta habilitado.
	/// </summary>
	void LateUpdate () 
	{
        // Si no hay target sale
        if (!_target)
            return;

        // Si se esta mirando al jugador almacena la rotacion actual
        Quaternion lookAtRotation = Quaternion.identity;
        if (_lookAtPlayer)
        {
            lookAtRotation = transform.rotation;
            transform.rotation = Quaternion.Euler(_angles);
        }

        // Calcula la rotacion en angulos
        var currentRotation = transform.eulerAngles;
        var wantedRotation = new Vector3(
            _angles.x < 0 ? _angles.x + 360f : _angles.x,
            _angles.y < 0 ? _angles.y + 360f : _angles.y,
            _angles.z < 0 ? _angles.z + 360f : _angles.z
        );

        // Calcula la posicion
        var cam2target = transform.position - _target.position;
        var currentOffset = new Vector3(
            Vector3.Dot(cam2target, transform.right),
            Vector3.Dot(cam2target, transform.up),
            Vector3.Dot(cam2target, transform.forward)
        );
        var wantedOffset = new Vector3(
            _translationFollow.x ? _offset.x : currentOffset.x,
            _translationFollow.y ? _offset.y : currentOffset.y,
            _translationFollow.z ? _offset.z : currentOffset.z
        );
        
        // Damp de traslacion
        currentOffset = Vector3.Lerp(currentOffset, wantedOffset, _translationDamping * Time.deltaTime);

        // Damp de rotacion
        currentRotation = Vector3.Lerp(currentRotation, wantedRotation, _rotationDamping * Time.deltaTime);

        // Convierte la rotacion en quaterniones
        var currentQRotation = Quaternion.Euler(currentRotation);
        transform.rotation = currentQRotation;
        
        // Ubica la camara
        transform.position = _target.position;
        transform.position += currentQRotation * currentOffset;

        // Si se esta mirando al jugador modifica la rotacion
        if (_lookAtPlayer)
        {
            transform.LookAt(_target.position + _lookAtOffset);           
            transform.rotation = Quaternion.Lerp(lookAtRotation, transform.rotation, _rotationDamping * Time.deltaTime);
        }
	}

	#endregion

	#region Metodos

    public void SetTarget(Transform target)
    {
        this._target = target;
    }

	#endregion
}
