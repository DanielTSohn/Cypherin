using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CipherSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField textInput;
    [SerializeField] private TMP_InputField keyInput;
    [SerializeField] private BaseCipher cipher;
    [SerializeField] private TMP_Dropdown cipherSelect;
    [SerializeField] private TextMeshProUGUI outputText;
    public enum CipherType { Caesar, Vigenere }
    public CipherType cipherType = CipherType.Caesar;

    public void UpdateCipherSelection(int value)
    {
        keyInput.text = string.Empty;
        cipherType = (CipherType)value;
        cipher.cipherType = cipherType;
        switch (cipherType)
        {
            case CipherType.Caesar:
                keyInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
                break;
            case CipherType.Vigenere:
                keyInput.characterValidation = TMP_InputField.CharacterValidation.Name;
                break;
            default:
                break;
        }
    }

    public void UpdateText(char character)
    {
        outputText.text += character;
        GUIUtility.systemCopyBuffer = outputText.text;
    }

    public void EncodeText()
    {
        if(!string.IsNullOrEmpty(keyInput.text))
        {
            outputText.text = string.Empty;
            switch (cipherType)
            {
                case CipherType.Caesar:
                    cipher.Encrypt(textInput.text.ToLower(), int.Parse(keyInput.text));
                    break;
                case CipherType.Vigenere:
                    cipher.Encrypt(textInput.text.ToLower(), keyInput.text.ToLower());
                    break;
                default:
                    break;
            }
        }
        else
        {
            EmptyKeyNote();
        }
    }

    public void DecodeText()
    {
        if (!string.IsNullOrEmpty(keyInput.text))
        {
            outputText.text = string.Empty;
            switch (cipherType)
            {
                case CipherType.Caesar:
                    cipher.Decrypt(textInput.text.ToLower(), int.Parse(keyInput.text));
                    break;
                case CipherType.Vigenere:
                    cipher.Decrypt(textInput.text.ToLower(), keyInput.text.ToLower());
                    break;
                default:
                    break;
            }
        }
        else
        {
            EmptyKeyNote();
        }
    }

    private void EmptyKeyNote()
    {
        switch (cipherType)
        {
            case CipherType.Caesar:
                outputText.text = "Input a shift value for the key first!";
                break;
            case CipherType.Vigenere:
                outputText.text = "Input a word for the key first!";
                break;
            default:
                break;
        }
    }
}
