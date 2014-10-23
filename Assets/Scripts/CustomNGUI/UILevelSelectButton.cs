using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIButton))]
public class UILevelSelectButton : MonoBehaviour
{
	#region Variables (private)

    [SerializeField]
    private int _levelIndex;

    [SerializeField]
    private GameObject _UILevelInfo;

    [SerializeField]
    private UIButton _UIPlayButton;

    private UIButton _UIButton;

	#endregion

	#region Propiedades (public)
    
    public int LevelIndex
    {
        get { return _levelIndex; }
        set { _levelIndex = value; }
    }

    public GameObject UILevelInfo
    {
        get { return _UILevelInfo; }
        set { _UILevelInfo = value; }
    }

    public UIButton UIPlayButton
    {
        get { return _UIPlayButton; }
        set { _UIPlayButton = value; }
    }

	#endregion

	#region Funciones de evento de Unity

    /// <summary>
    /// Llamado siempre al inicializar el componente.
    /// </summary>
    void Awake()
    {
        if (_UIButton == null)
        {
            _UIButton = GetComponent<UIButton>();
        }
    }

    /// <summary>
    /// Llamado al inicializar el componente, si MonoBehaviour esta habilitado.
    /// </summary>
    void Start()
    {

    }

	#endregion

	#region Metodos
    
    public void Selected()
    {
        if (!enabled) return;

        // Establece el nuevo nivel seleccionado
        Level.Instance.Set(_levelIndex);
        var levelInfo = Level.Instance.Info;

        // Muestra el Nombre del nivel
        var UILevelName = _UILevelInfo.transform.FindChild("UILabel - LevelName").GetComponent<UILabel>();
        UILevelName.text = levelInfo.LevelName;

        // Muestra la dificultad del nivel
        var UIDifficulty = _UILevelInfo.transform.FindChild("UILabel - Difficulty").GetComponent<UILabel>();
        UIDifficulty.text = levelInfo.Difficulty == 1 ? "[66FA33]Fácil[-]" : (levelInfo.Difficulty == 2 ? "[FFFF66]Media[-]" : "[FF6333]Difícil[-]");

        // Determina si existe una puntuacion almacenada para el nivel
        var hashID = Level.Instance.HashID.ToString();
        var UIScore = _UILevelInfo.transform.FindChild("UILabel - Score").GetComponent<UILabel>();
        var UIPrize = _UILevelInfo.transform.FindChild("Sprite - Prize").GetComponent<UISprite>();
        if (PlayerPrefs.HasKey(hashID))
        {
            // Si la hay extrae el puntaje almacenado
            var currScore = PlayerPrefs.GetInt(hashID);

            // Muestra el puntaje
            UIScore.text = currScore.ToString();

            // Muestra la medalla acorde
            if (currScore >= levelInfo.GoldScore)
            {
                UIPrize.spriteName = "GoldPrize";
            }
            else if (currScore >= levelInfo.SilverScore)
            {
                UIPrize.spriteName = "SilverPrize";
            }
            else
            {
                UIPrize.spriteName = "BronzePrize";
            }
        }
        else
        {
            // No hay puntaje ni medalla
            UIScore.text = "-";
            UIPrize.spriteName = string.Empty;
            UIPrize.enabled = false;
        }

        // Habilita el button de play
        _UIPlayButton.isEnabled = true;
    }

	#endregion
}
