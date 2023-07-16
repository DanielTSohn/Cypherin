using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseCipher<KeyType> : MonoBehaviour, MMEventListener<TranslationRequest<KeyType>>
{
    protected CipherType cipherType = CipherType.Caesar;

    [Tooltip("Called when encryption has finished")]
    [SerializeField] private UnityEvent OnEncrypted;

    [Tooltip("Called when decryption has finished")]
    [SerializeField] private UnityEvent OnDecrypted;

    public void OnEnable()
    {
        this.MMEventStartListening();
    }

    public void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(TranslationRequest<KeyType> request)
    {
        if(request.CipherType == cipherType)
        {
            if(request.Encrypt)
            {
                Encrypt(request.Key);
            }
            else
            {
                Decrypt(request.Key);
            }
        }
    }

    protected abstract char Encode(in LinkedListNode<(char, int)> node, KeyType key);

    /// <summary>
    /// Encrypts the inputed plain text dynamically 
    /// </summary>
    /// <typeparam name="KeyType">The dynamic type of the key due to different needs of ciphers</typeparam>
    /// <param name="plainText">The normal untranslated text to be encrypted</param>
    /// <param name="translationBuffer">The translation buffer to store encrypted chars into, passed by reference</param>
    /// <param name="key">The key used to encrypt the plain text, function handles logic of dynamic type</param>
    /// <returns></returns>
    public virtual bool Encrypt(KeyType key)
    {
        /// Set size of buffer and start position at beginning of buffer
        CipherSelector.Instance.SetBufferSize(CipherSelector.Instance.GetInputTextCount());
        int position = 0;
        for (var node = CipherSelector.Instance.GetTextInputFirstNode(); node != null; node = node.Next)
        {
            /// Default space value, will always adjust current position in buffer and iterate to next
            char character = ' ';
            /// If the charKey has a positional value to calculate from
            if (node.Value.Item2 >= 0)
            {
                try { character = Encode(node, key); }
                catch { MMEventManager.TriggerEvent(false); return false; }
                /// Put the character value in range of the English alphabet
                character += 'a';
            }
            CipherSelector.Instance.SetBufferChar(character, position);
            position++;
        }

        OnEncrypted.Invoke();
        MMEventManager.TriggerEvent(true);
        return true;
    }

    protected abstract char Decode(in LinkedListNode<(char, int)> node, KeyType key);

    /// <summary>
    /// Decrypts the inputed cipher text dynamically 
    /// </summary>
    /// <typeparam name="KeyType">The dynamic type of the key due to different needs of ciphers</typeparam>
    /// <param name="cipherText">The encrypted cipher text to be decrypted</param>
    /// <param name="translationBuffer">The translation buffer to store decrypted chars into, passed by reference</param>
    /// <param name="key">The key used to decrypt the cipher text, function handles logic of dynamic type</param>
    /// <returns></returns>
    public virtual bool Decrypt(KeyType key)
    {
        /// Set size of buffer and start position at beginning of buffer
        CipherSelector.Instance.SetBufferSize(CipherSelector.Instance.GetInputTextCount());
        int position = 0;
        for (var node = CipherSelector.Instance.GetTextInputFirstNode(); node != null; node = node.Next)
        {
            /// Default space value, will always adjust current position in buffer and iterate to next
            char character = ' ';
            /// If the charKey has a positional value to calculate from
            if (node.Value.Item2 >= 0)
            {
                try { character = Decode(node, key); }
                catch { MMEventManager.TriggerEvent(false); return false; }
                /// Put the character value in range of the English alphabet
                character += 'a';
            }
            CipherSelector.Instance.SetBufferChar(character, position);
            position++;
        }

        OnEncrypted.Invoke();
        MMEventManager.TriggerEvent(true);
        return true;
    }
}
