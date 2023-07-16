using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum CipherType { Caesar, Vigenere }

public struct TranslationRequest<KeyType>
{
    public TranslationRequest(CipherType cipher, bool encrypt, KeyType key)
    {
        CipherType = cipher;
        Encrypt = encrypt;
        Key = key;
    }

    /// <summary>
    /// The type of cipher to encode / decode with
    /// </summary>
    public CipherType CipherType;

    /// <summary>
    /// True to encrypt, False to Decrypt
    /// </summary>
    public bool Encrypt;

    /// <summary>
    /// The key used to encrypt or Decrypt the message
    /// </summary>
    public KeyType Key;
}

public class CipherSelector : MonoBehaviour, MMEventListener<bool>
{
    [Tooltip("The input field that will be translated")]
    [SerializeField] private TMP_InputField textInput;

    [Tooltip("The input field that takes the key to encrypt to text input")]
    [SerializeField] private TMP_InputField keyInput;

    [Tooltip("The dropdown menu for selecting the type of cipher")]
    [SerializeField] private TMP_Dropdown cipherSelect;

    [Tooltip("The output field that displays the plain or cipher text")]
    [SerializeField] private TextMeshProUGUI outputText;

    [HideInInspector] public CipherType cipherType = CipherType.Caesar;

    private char[] translationBuffer;
    public int GetBufferLength() { return translationBuffer.Length; }
    public void SetBufferSize(int size) { translationBuffer = new char[size]; }
    public bool SetBufferChar(char c, int index)
    {
        try
        {
            translationBuffer[index] = c;
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Singleton pattern
    /// </summary>
    public static CipherSelector Instance { get; private set; }

    /// <summary>
    /// Used to store text values and their position in the English Alphabet
    /// </summary>
    private LinkedList<(char, int)> textValues = new();
    public int GetInputTextCount() { return textValues.Count; }
    public LinkedListNode<(char, int)> GetTextInputFirstNode() { return textValues.First; }

    public void OnEnable()
    {
        this.MMEventStartListening();
    }

    public void OnDisable()
    {
        this.MMEventStopListening();
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Switches what encryption / decryption algorithm will be used, sets the key input field to try and stop user error
    /// </summary>
    /// <param name="value">The enumeration value to set to</param>
    public void UpdateCipherSelection(int value)
    {
        keyInput.text = string.Empty;
        cipherType = (CipherType)value;
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

    public void OnMMEvent(bool translated)
    {
        if(translated)
        {
            UpdateText();
        }
        else
        {
            outputText.text = "Translation Failed";
        }
    }

    /// <summary>
    /// Updates the output text field to whatever output from translating
    /// </summary>
    /// <param name="translationBuffer">The char array buffer to read from, passed in by reference</param>
    public void UpdateText()
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
                        MMEventManager.TriggerEvent<TranslationRequest<int>>(new(cipherType, true, value % 26));
                    }
                    else { outputText.text = "Need an integer for the key"; }
                    break;
                case CipherType.Vigenere:
                    MMEventManager.TriggerEvent<TranslationRequest<string>>(new(cipherType, true, keyInput.text.ToLower()));
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
                        MMEventManager.TriggerEvent<TranslationRequest<int>>(new(cipherType, false, value % 26));
                    }
                    else { outputText.text = "Need an integer for the key"; }
                    break;
                case CipherType.Vigenere:
                    MMEventManager.TriggerEvent<TranslationRequest<string>>(new(cipherType, false, keyInput.text.ToLower()));
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
