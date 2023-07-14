using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

/// <summary>
/// Simple way to access alphabets as a dictionary with key being the char and value the relative position from 0
/// </summary>
public static class Alphabet
{
    /// <summary>
    /// The english alphabet ordered by char with values from 0-25 with space included with value -1
    /// </summary>
    public static readonly Dictionary<char, int> English = new()
        {
            { 'a', 0 },
            { 'b', 1 },
            { 'c', 2 },
            { 'd', 3 },
            { 'e', 4 },
            { 'f', 5 },
            { 'g', 6 },
            { 'h', 7 },
            { 'i', 8 },
            { 'j', 9 },
            { 'k', 10 },
            { 'l', 11 },
            { 'm', 12 },
            { 'n', 13 },
            { 'o', 14 },
            { 'p', 15 },
            { 'q', 16 },
            { 'r', 17 },
            { 's', 18 },
            { 't', 19 },
            { 'u', 20 },
            { 'v', 21 },
            { 'w', 22 },
            { 'x', 23 },
            { 'y', 24 },
            { 'z', 25 },
            { ' ', -1 }
        };
}

public class BaseCipher : MonoBehaviour
{
    [HideInInspector] public CipherSelector.CipherType cipherType = CipherSelector.CipherType.Caesar;

    [Tooltip("Called when encryption has finished")]
    [SerializeField] private UnityEvent OnEncrypted;

    [Tooltip("Called when decryption has finished")]
    [SerializeField] private UnityEvent OnDecrypted;

    /// <summary>
    /// Encrypts the inputed plain text dynamically 
    /// </summary>
    /// <typeparam name="KeyType">The dynamic type of the key due to different needs of ciphers</typeparam>
    /// <param name="plainText">The normal untranslated text to be encrypted</param>
    /// <param name="translationBuffer">The translation buffer to store encrypted chars into, passed by reference</param>
    /// <param name="key">The key used to encrypt the plain text, function handles logic of dynamic type</param>
    /// <returns></returns>
    public bool Encrypt<KeyType>(in LinkedList<(char, int)> plainText, out char[] translationBuffer, KeyType key)
    {
        /// Set size of buffer and start position at beginning of buffer
        translationBuffer = new char[plainText.Count];
        int position = 0;
        foreach (var charKey in plainText)
        {
            /// Default space value, will always adjust current position in buffer and iterate to next
            char character = ' ';
            /// If the charKey has a positional value to calculate from
            if (charKey.Item2 >= 0)
            {
                switch (cipherType)
                {
                    case CipherSelector.CipherType.Caesar:
                        switch (key)
                        {
                            case int shift:
                                character = (char)((Alphabet.English[charKey.Item1] + shift) % 26);
                                break;
                            default:
                                Debug.LogError("Incorrect key type");
                                break;
                        }
                        break;
                    case CipherSelector.CipherType.Vigenere:
                        switch (key)
                        {
                            case string word:
                                character = (char)((Alphabet.English[charKey.Item1] + Alphabet.English[word[position % word.Length]]) % 26);
                                break;
                            default:
                                Debug.LogError("Incorrect key type, expected type string, got type : " + key.GetType());
                                break;
                        }
                        break;
                    default:
                        return false;
                }
                /// Put the character value in range of the English alphabet
                character += 'a';
            }
            translationBuffer[position] = character;
            position++;
        }

        OnEncrypted.Invoke();
        return true;
    }

    /// <summary>
    /// Decrypts the inputed cipher text dynamically 
    /// </summary>
    /// <typeparam name="KeyType">The dynamic type of the key due to different needs of ciphers</typeparam>
    /// <param name="cipherText">The encrypted cipher text to be decrypted</param>
    /// <param name="translationBuffer">The translation buffer to store decrypted chars into, passed by reference</param>
    /// <param name="key">The key used to decrypt the cipher text, function handles logic of dynamic type</param>
    /// <returns></returns>
    public bool Decrypt<KeyType>(in LinkedList<(char, int)> cipherText, out char[] translationBuffer, KeyType key)
    {
        /// Set size of buffer and start position at beginning of buffer
        translationBuffer = new char[cipherText.Count];
        int position = 0;
        foreach (var charKey in cipherText)
        {
            /// Default space value, will always adjust current position in buffer and iterate to next
            char character = ' ';
            /// If the charKey has a positional value to calculate from
            if (charKey.Item2 >= 0)
            {
                switch (cipherType)
                {
                    case CipherSelector.CipherType.Caesar:
                        switch (key)
                        {
                            case int shift:
                                character = (char)((Alphabet.English[charKey.Item1] + 26 - shift) % 26);
                                break;
                            default:
                                Debug.LogError("Incorrect key type");
                                break;
                        }
                        break;
                    case CipherSelector.CipherType.Vigenere:
                        switch (key)
                        {
                            case string word:
                                character = (char)((Alphabet.English[charKey.Item1] + 26 - Alphabet.English[word[position % word.Length]]) % 26);
                                break;
                            default:
                                Debug.LogError("Incorrect key type");
                                break;
                        }
                        break;
                    default:
                        break;
                }
                /// Put the character value in range of the English alphabet
                character += 'a';
            }
            translationBuffer[position] = character;
            position++;
        }

        OnEncrypted.Invoke();
        return true;
    }
}
