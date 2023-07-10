using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseCipher : MonoBehaviour
{
    [Tooltip("The time curve to shift between letters")]
    [SerializeField] private AnimationCurve letterShiftTimeCurve;
    [SerializeField] private float timeCurveMultiplier = 1.0f;
    [HideInInspector] public CipherSelector.CipherType cipherType = CipherSelector.CipherType.Caesar;
    public UnityEvent<char> OnCharTranslate;

    public void Encrypt<KeyType>(in string plainText, KeyType key)
    {
        StopAllCoroutines();
        StartCoroutine(EnumerateEncryption(plainText.ToCharArray(), key));
    }

    protected IEnumerator EnumerateEncryption<KeyType>(char[] translationBuffer, KeyType key)
    {
        float time = 0;
        for (int i = 0; i < translationBuffer.Length; i++)
        {
            if (translationBuffer[i] == ' ') { OnCharTranslate.Invoke(' '); continue; }
            Encode(ref translationBuffer[i], key);
            yield return new WaitForSeconds(letterShiftTimeCurve.Evaluate(time) * timeCurveMultiplier);
            time += Time.fixedDeltaTime;
        }
    }

    protected void Encode<KeyType>(ref char character, KeyType key)
    {
        switch(cipherType)
        {
            case CipherSelector.CipherType.Caesar:
                switch (key)
                {
                    case int shift:
                        character = (char)(((character + shift - 'a') % 26) + 'a');
                        break;
                    default:
                        Debug.LogError("Incorrect key type");
                        break;
                }
                break;
            case CipherSelector.CipherType.Vigenere:
                switch (key)
                {
                    case string:

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

    protected void Decode<KeyType>(ref char character, KeyType key)
    {
        switch (cipherType)
        {
            case CipherSelector.CipherType.Caesar:
                switch (key)
                {
                    case int shift:
                        character = (char)(((character + (26 - shift) - 'a') % 26) + 'a');
                        break;
                    default:
                        Debug.LogError("Incorrect key type");
                        break;
                }
                break;
            case CipherSelector.CipherType.Vigenere:
                switch (key)
                {
                    case string:

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

    protected IEnumerator EnumerateDecryption<KeyType>(char[] translationBuffer, KeyType key)
    {
        float time = 0;
        for (int i = 0; i < translationBuffer.Length; i++)
        {
            if (translationBuffer[i] == ' ') { OnCharTranslate.Invoke(' '); continue; }
            Decode(ref translationBuffer[i], key);
            yield return new WaitForSeconds(letterShiftTimeCurve.Evaluate(time) * timeCurveMultiplier);
            time += Time.fixedDeltaTime;
        }
    }
}
