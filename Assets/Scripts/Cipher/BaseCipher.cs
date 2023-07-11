using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseCipher : MonoBehaviour
{
    [Tooltip("The time curve to shift between letters")]
    [SerializeField] private float typeSpeedMultiplier = 1.0f;
    [SerializeField] private AnimationCurve speedupCurve;
    [HideInInspector] public CipherSelector.CipherType cipherType = CipherSelector.CipherType.Caesar;
    public UnityEvent<char> OnCharTranslate;
    public UnityEvent OnEncrypted;
    public UnityEvent OnDecrypted;

    Dictionary<char, int> alphabet = new Dictionary<char, int>()
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
            { 'z', 25 }
        };

    public void Encrypt<KeyType>(in string plainText, KeyType key)
    {
        StopAllCoroutines();
        StartCoroutine(EnumerateEncryption(plainText.ToCharArray(), key));
    }

    protected IEnumerator EnumerateEncryption<KeyType>(char[] translationBuffer, KeyType key)
    {
        float time = 0;
        int position = 0;
        for (int i = 0; i < translationBuffer.Length; i++)
        {
            if (translationBuffer[i] == ' ') { OnCharTranslate.Invoke(' '); continue; }
            Encode(ref translationBuffer[i], position, ref key);
            position++;

            float waitTime = Mathf.PerlinNoise1D(time) * typeSpeedMultiplier * speedupCurve.Evaluate(time);
            if(waitTime >= 0) { yield return new WaitForSeconds(waitTime); }
            time += Time.fixedDeltaTime;
        }

        OnEncrypted.Invoke();
    }

    protected void Encode<KeyType>(ref char character, int position, ref KeyType key)
    {
        switch(cipherType)
        {
            case CipherSelector.CipherType.Caesar:
                switch (key)
                {
                    case int shift:
                        character = (char)((alphabet[character] + shift) % 26);
                        character += 'a';
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
                        character = (char)((alphabet[character] + alphabet[word[position % word.Length]]) % 26);
                        character += 'a';
                        break;
                    default:
                        Debug.LogError("Incorrect key type");
                        break;
                }
                break;
            default:
                break;
        }
        OnCharTranslate.Invoke(character);
    }

    public void Decrypt<KeyType>(in string cipherText, KeyType key)
    {
        StopAllCoroutines();
        StartCoroutine(EnumerateDecryption(cipherText.ToCharArray(), key));
    }

    protected IEnumerator EnumerateDecryption<KeyType>(char[] translationBuffer, KeyType key)
    {
        float time = 1;
        int position = 0;
        for (int i = 0; i < translationBuffer.Length; i++)
        {
            if (translationBuffer[i] == ' ') { OnCharTranslate.Invoke(' '); continue; }
            Decode(ref translationBuffer[i], position, ref key);
            position++;

            float waitTime = Mathf.PerlinNoise1D(time) * typeSpeedMultiplier * speedupCurve.Evaluate(time);
            if (waitTime >= 0) { yield return new WaitForSeconds(waitTime); }
            time += Time.fixedDeltaTime;
        }
        OnDecrypted.Invoke();
    }

    protected void Decode<KeyType>(ref char character, int position, ref KeyType key)
    {
        switch (cipherType)
        {
            case CipherSelector.CipherType.Caesar:
                switch (key)
                {
                    case int shift:
                        character = (char)((alphabet[character] + 26 - shift) % 26);
                        character += 'a';
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
                        character = (char)((alphabet[character] + 26 - alphabet[word[position % word.Length]]) % 26);
                        character += 'a';
                        break;
                    default:
                        Debug.LogError("Incorrect key type");
                        break;
                }
                break;
            default:
                break;
        }
        OnCharTranslate.Invoke(character);
    }
}
