using System.Collections.Generic;

[System.Serializable]
public class Alumnus
{
    public string name;
    public string surname;
    public string univ;
    public string faculty;
    public int year;
    public string study_form;
    public string spec_sub;
    public string city;
    public string region;
}

[System.Serializable]
public class AlumnusListWrapper
{
    public List<Alumnus> alumni;
}
