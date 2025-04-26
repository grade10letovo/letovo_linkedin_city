using UnityEngine;
using TMPro;

public class AlumniRowUI : MonoBehaviour
{
    public TMP_Text FirstNameText;
    public TMP_Text LastNameText;
    public TMP_Text FacultyText;
    public TMP_Text YearText;

    public void SetData(Alumnus alum)
    {
        FirstNameText.text = alum.name;
        LastNameText.text = alum.surname;
        FacultyText.text = string.IsNullOrEmpty(alum.faculty) ? "—" : alum.faculty;
        YearText.text = alum.year.ToString();
    }
}
