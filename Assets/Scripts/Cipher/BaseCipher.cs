using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
