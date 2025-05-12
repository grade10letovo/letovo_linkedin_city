using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

[RequireComponent(typeof(TMP_InputField))]
public class EnglishOnlyInput : MonoBehaviour
{
    private TMP_InputField inputField;

    // Регулярка для разрешённых символов (только латиница)
    private static readonly Regex allowedCharacters = new Regex(@"[^a-zA-Z0-9!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]", RegexOptions.Compiled);

    // Событие: когда введён запрещённый символ
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
        // Проверяем, есть ли запрещённые символы
        if (allowedCharacters.IsMatch(text))
        {
            // Оповещаем подписчиков о некорректном вводе
            OnInvalidInputDetected?.Invoke(text);

            // Убираем все символы, кроме английских
            string filtered = Regex.Replace(text, @"[^a-zA-Z]", "");
            inputField.text = filtered;
        }
    }
}
