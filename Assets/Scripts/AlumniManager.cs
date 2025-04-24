using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class AlumniManager : MonoBehaviour
{
    [Header("UI Prefab и контейнер")]
    [Tooltip("Префаб строки с компонентом AlumniRowUI")]
    public GameObject rowPrefab;
    [Tooltip("Content внутри ScrollView (Viewport/Content)")]
    public Transform contentParent;

    [Header("Кнопки и фильтры")]
    [Tooltip("Кнопка, по нажатию которой выводится список")]
    public Button showButton;
    [Tooltip("Кнопка, по нажатию которой скрывается список")]
    public Button closeButton;
    [Tooltip("Dropdown для фильтрации по факультету (TMP_Dropdown)")]
    public TMP_Dropdown facultyDropdown;

    [Header("ScrollView или его контейнер")]
    [Tooltip("Объект, содержащий ScrollView (можно сам ScrollView)")]
    public GameObject scrollViewContainer;

    private List<Alumnus> allAlumni;

    void Start()
    {
        // Загрузка и парсинг JSON
        TextAsset asset = Resources.Load<TextAsset>("alumnus");
        if (asset == null)
        {
            Debug.LogError("Не найден Resources/alumnus.json");
            return;
        }

        string wrapped = "{\"alumni\":" + asset.text + "}";
        AlumnusListWrapper wrapper = JsonUtility.FromJson<AlumnusListWrapper>(wrapped);
        allAlumni = wrapper.alumni;

        // Инициализация Dropdown факультетов
        InitFacultyDropdown();

        // Подписка на кнопки
        if (showButton != null)
            showButton.onClick.AddListener(DisplayList);
        else
            Debug.LogError("Не назначена кнопка showButton в инспекторе!");

        if (closeButton != null)
            closeButton.onClick.AddListener(HideList);
        else
            Debug.LogError("Не назначена кнопка closeButton в инспекторе!");

        // При старте скрываем ScrollView
        if (scrollViewContainer != null)
            scrollViewContainer.SetActive(false);
    }

    /// <summary>
    /// Заполняет TMP_Dropdown уникальными факультетами из allAlumni
    /// </summary>
    private void InitFacultyDropdown()
    {
        if (facultyDropdown == null) return;

        // Собираем список факультетов + опция "Все"
        var faculties = allAlumni
            .Select(a => string.IsNullOrEmpty(a.faculty) ? "—" : a.faculty)
            .Distinct()
            .OrderBy(f => f)
            .ToList();
        faculties.Insert(0, "Все");

        // Очищаем и добавляем
        facultyDropdown.ClearOptions();
        facultyDropdown.AddOptions(faculties);

        // Подписываемся на изменение
        facultyDropdown.onValueChanged.AddListener(index => DisplayList());
    }

    void DisplayList()
    {
        if (rowPrefab == null || contentParent == null)
        {
            Debug.LogError("rowPrefab или contentParent не назначены!");
            return;
        }

        // Показываем ScrollView
        if (scrollViewContainer != null)
            scrollViewContainer.SetActive(true);

        // Очищаем контент
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Определяем текущий вуз
        UniversityTag tag = FindObjectOfType<UniversityTag>();
        if (tag == null || string.IsNullOrEmpty(tag.universityName))
        {
            Debug.LogError("UniversityTag не найден или поле universityName пусто!");
            return;
        }
        string currentUniv = tag.universityName;

        // Фильтрация по вузу
        IEnumerable<Alumnus> filtered = allAlumni.Where(a => a.univ == currentUniv);

        // Фильтрация по факультету из Dropdown
        if (facultyDropdown != null && facultyDropdown.value > 0)
        {
            string selectedFaculty = facultyDropdown.options[facultyDropdown.value].text;
            filtered = filtered.Where(a => (string.IsNullOrEmpty(a.faculty) ? "—" : a.faculty) == selectedFaculty);
        }

        // Создаём UI-строки
        foreach (Alumnus alum in filtered)
        {
            GameObject rowGO = Instantiate(rowPrefab, contentParent);
            AlumniRowUI rowUI = rowGO.GetComponent<AlumniRowUI>();
            if (rowUI != null)
            {
                rowUI.SetData(alum);
            }
            else
            {
                Debug.LogError($"В префабе {rowPrefab.name} отсутствует компонент AlumniRowUI!");
            }
        }
    }

    void HideList()
    {
        if (scrollViewContainer != null)
            scrollViewContainer.SetActive(false);
    }
}
