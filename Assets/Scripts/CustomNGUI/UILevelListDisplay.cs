using UnityEngine;
using System.Collections;

public class UILevelListDisplay : MonoBehaviour
{
	#region Variables (private)

    [SerializeField]
    private GameObject _UILevelInfo;

    [SerializeField]
    private UIButton _UIPlayButton;

    [SerializeField]
    private UIScrollView _UIScrollView;

    [SerializeField]
    private UITable _UITable;

    [SerializeField]
    private GameObject _elementPrefab;

	#endregion

	#region Propiedades (public)

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
	void Start ()
    {
        DisablePlayButton();
        PopulateList();
	}

	/// <summary>
    /// Update es llamado una vez por frame, si MonoBehaviour esta habilitado.
	/// </summary>
	void Update () 
	{
	
	}

	#endregion

	#region Metodos

    /// <summary>
    /// Deshabilita el boton de Play
    /// </summary>
    public void DisablePlayButton()
    {
        _UIPlayButton.isEnabled = false;
    }

    /// <summary>
    /// Genera los botones que permitiran seleccionar el nivel
    /// </summary>
    public void PopulateList()
    {
        for (int i = 0; i < Index.Levels.Count; i++)
        {
            var currLevel = Index.Levels[i];

            // Crea y posiciona el boton
            var newElement = Instantiate(_elementPrefab) as GameObject;
            newElement.transform.parent = _UITable.transform;
            newElement.transform.localScale = Vector3.one;
            newElement.transform.localPosition = Vector3.zero;
            newElement.transform.localRotation = Quaternion.identity;

            // Establece el label
            var buttonLabel = newElement.GetComponentInChildren<UILabel>();
            buttonLabel.text = currLevel.name;

            // Le asigna la logica
            var buttonLogic = newElement.GetComponent<UILevelSelectButton>();
            buttonLogic.LevelIndex = i;
            buttonLogic.UILevelInfo = _UILevelInfo;
            buttonLogic.UIPlayButton = _UIPlayButton;

            // Si el level anterior no fue completado este boton queda deshabilitado
            if (i > 0 && !PlayerPrefs.HasKey(Index.Levels[i - 1].GetHashCode().ToString()))
                ;//newElement.GetComponent<UIButton>().isEnabled = false;
            
            // Logica de scroll
            var scrollLogic = newElement.GetComponent<UIDragScrollView>();
            scrollLogic.scrollView = _UIScrollView;
        }

        _UITable.Reposition();
    }

	#endregion
}
