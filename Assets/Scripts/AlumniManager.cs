using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
public class AlumniManager : MonoBehaviour
{

    public GameObject headerRowPrefab;
    public GameObject rowPrefab;        // Префаб строки
    public Transform contentParent;     // Контейнер (Content в Scroll View)
    public Button showButton;

    private List<Alumnus> allAlumni;
    private List<Alumnus> filteredAlumni;

    void Start()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("alumnus");
        string wrappedJson = "{\"alumni\":" + jsonAsset.text + "}";
        AlumnusListWrapper wrapper = JsonUtility.FromJson<AlumnusListWrapper>(wrappedJson);
        allAlumni = wrapper.alumni;

        showButton.onClick.AddListener(DisplayList);
    }

    void DisplayList()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Добавим заголовок
        Instantiate(headerRowPrefab, contentParent);

        string currentUniversity = FindObjectOfType<UniversityTag>()?.universityName;
        filteredAlumni = allAlumni.Where(a => a.univ == currentUniversity).ToList();

        foreach (var alumnus in filteredAlumni)
        {
            GameObject row = Instantiate(rowPrefab, contentParent);

            // Поиск текстовых компонентов
            Text nameText = row.transform.Find("NameText").GetComponent<Text>();
            Text facultyText = row.transform.Find("FacultyText").GetComponent<Text>();
            Text yearText = row.transform.Find("YearText").GetComponent<Text>();

            nameText.text = $"{alumnus.name} {alumnus.surname}";
            facultyText.text = string.IsNullOrEmpty(alumnus.faculty) ? "—" : alumnus.faculty;
            yearText.text = alumnus.year.ToString();
        }
    }


}
