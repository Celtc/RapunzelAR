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

    public void DisablePlayButton()
    {
        _UIPlayButton.isEnabled = false;
    }

    public void PopulateList()
    {
        for (int i = 0; i < Index.Levels.Count; i++)
        {
            var currLevel = Index.Levels[i];

            var newElement = Instantiate(_elementPrefab) as GameObject;
            newElement.transform.parent = _UITable.transform;
            newElement.transform.localScale = Vector3.one;
            newElement.transform.localPosition = Vector3.zero;
            newElement.transform.localRotation = Quaternion.identity;

            var buttonLabel = newElement.GetComponentInChildren<UILabel>();
            buttonLabel.text = currLevel.name;

            var buttonLogic = newElement.GetComponent<UILevelSelectButton>();
            buttonLogic.LevelIndex = i;
            buttonLogic.UILevelInfo = _UILevelInfo;
            buttonLogic.UIPlayButton = _UIPlayButton;
            
            var scrollLogic = newElement.GetComponent<UIDragScrollView>();
            scrollLogic.scrollView = _UIScrollView;
        }

        _UITable.Reposition();
    }

	#endregion
}
