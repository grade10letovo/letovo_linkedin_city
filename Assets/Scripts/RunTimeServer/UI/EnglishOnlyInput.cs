using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

[RequireComponent(typeof(TMP_InputField))]
public class EnglishOnlyInput : MonoBehaviour
{
    private TMP_InputField inputField;

    // ��������� ��� ����������� �������� (������ ��������)
    private static readonly Regex allowedCharacters = new Regex(@"[^a-zA-Z0-9!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]", RegexOptions.Compiled);

    // �������: ����� ����� ����������� ������
    public event Action<string> OnInvalidInputDetected;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener(FilterInput);
    }

    private void OnDestroy()
    {
        inputField.onValueChanged.RemoveListener(FilterInput);
    }

    private void FilterInput(string text)
    {
        // ���������, ���� �� ����������� �������
        if (allowedCharacters.IsMatch(text))
        {
            // ��������� ����������� � ������������ �����
            OnInvalidInputDetected?.Invoke(text);

            // ������� ��� �������, ����� ����������
            string filtered = Regex.Replace(text, @"[^a-zA-Z]", "");
            inputField.text = filtered;
        }
    }
}
