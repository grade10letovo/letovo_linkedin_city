using UnityEngine;

[CreateAssetMenu(fileName = "APIConfig", menuName = "Configurations/APIConfig", order = 1)]
public class APIConfigSO : ScriptableObject
{
    public string apiDomen = "https://your-default-api-url.com";
    public string apiUniversityUrl = "/universities";
}
