using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CipherSelector : MonoBehaviour
{
    [Tooltip("The input field that will be translated")]
    [SerializeField] private TMP_InputField textInput;
   
    [Tooltip("The input field that takes the key to encrypt to text input")]
    [SerializeField] private TMP_InputField keyInput;

    [Tooltip("The cipher script used to encode and decode")]  
    [SerializeField] private BaseCipher cipher;

    [Tooltip("The dropdown menu for selecting the type of cipher")]
    [SerializeField] private TMP_Dropdown cipherSelect;

    [Tooltip("The output field that displays the plain or cipher text")]
    [SerializeField] private TextMeshProUGUI outputText;

    public enum CipherType { Caesar, Vigenere }
    [HideInInspector] public CipherType cipherType = CipherType.Caesar;

    /// <summary>
    /// Used to store text values and their position in the English Alphabet
    /// </summary>
    private LinkedList<(char, int)> textValues = new();
    
    /// <summary>
    /// Switches what encryption / decryption algorithm will be used, sets the key input field to try and stop user error
    /// </summary>
    /// <param name="value">The enumeration value to set to</param>
    public void UpdateCipherSelection(int value)
    {
        keyInput.text = string.Empty;
        cipherType = (CipherType)value;
        cipher.cipherType = cipherType;
        switch (cipherType)
        {
            case CipherType.Caesar:
                keyInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
                keyInput.characterLimit = 9;
                break;
            case CipherType.Vigenere:
                keyInput.characterValidation = TMP_InputField.CharacterValidation.Name;
                keyInput.characterLimit = 0;
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// Puts the input field text into a linked list in tuples of form (char, int) 
    /// where char is the lowercase alphabetical character and int is the position in the English alphabet starting from 0
    /// </summary>
    private void RecordText()
    {
        try
        {
            textValues.Clear();
            foreach (char c in textInput.text)
            {
                if (char.IsLetter(c))
                {
                    char lowerC = char.ToLower(c);
                    if (Alphabet.English.TryGetValue(lowerC, out int value))
                    {
                        textValues.AddLast((lowerC, value));
                        continue;
                    }
                }
                textValues.AddLast((c, -1));
            }
        }
        catch
        {
            textValues.Clear();
            Debug.LogWarning("Foreach interrupted");
        }
    }

    /// <summary>
    /// Updates the output text field to whatever output from translating
    /// </summary>
    /// <param name="translationBuffer">The char array buffer to read from, passed in by reference</param>
    public void UpdateText(in char[] translationBuffer)
    {
        outputText.text = translationBuffer.ArrayToString();
        GUIUtility.systemCopyBuffer = outputText.text;
    }

    /// <summary>
    /// Encodes the text, dynamically chooses which cipher to choose
    /// Has error handling and updates the output text
    /// </summary>
    public void EncodeText()
    {
        if(!string.IsNullOrEmpty(keyInput.text))
        {
            RecordText();
            outputText.text = string.Empty;
            switch (cipherType)
            {
                case CipherType.Caesar:
                    if(int.TryParse(keyInput.text, out int value))
                    {
                        if (cipher.Encrypt(textValues, out char[] caesarBuffer, value % 26))
                            UpdateText(caesarBuffer);
                        else
                            Debug.LogWarning("Encryption Failed");
                    }
                    else { outputText.text = "Need an integer for the key"; }
                    break;
                case CipherType.Vigenere:
                    if(cipher.Encrypt(textValues, out char[] vigenereBuffer, keyInput.text.ToLower()))
                        UpdateText(vigenereBuffer);
                    else
                        Debug.LogWarning("Encryption Failed");
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

    /// <summary>
    /// Decodes the text, dynamically chooses which cipher to choose
    /// Has error handling and updates the output text
    /// </summary>
    public void DecodeText()
    {
        if (!string.IsNullOrEmpty(keyInput.text))
        {
            RecordText();
            outputText.text = string.Empty;
            switch (cipherType)
            {
                case CipherType.Caesar:
                    if (int.TryParse(keyInput.text, out int value))
                    {
                        if (cipher.Decrypt(textValues, out char[] caesarBuffer, value % 26))
                            UpdateText(caesarBuffer);
                        else
                            Debug.LogWarning("Encryption Failed");
                    }
                    else { outputText.text = "Need an integer for the key"; }
                    break;
                case CipherType.Vigenere:
                    if (cipher.Decrypt(textValues, out char[] vigenereBuffer, keyInput.text.ToLower()))
                        UpdateText(vigenereBuffer);
                    else
                        Debug.LogWarning("Encryption Failed");
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

    /// <summary>
    /// Dynamically gives error messages to the user via the output text
    /// </summary>
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
