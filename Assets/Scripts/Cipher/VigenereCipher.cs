using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class VigenereCipher : BaseCipher<string>
{
    private int position = 0;

    public void Awake()
    {
        cipherType = CipherType.Vigenere;
    }

    public override bool Encrypt(string key)
    {
        position = 0;
        return base.Encrypt(key);
    }

    protected override char Encode(in LinkedListNode<(char, int)> node, string key)
    {
        return (char)((node.Value.Item2 + Alphabet.English[key[position++ % key.Length]]) % 26);
    }

    public override bool Decrypt(string key)
    {
        position = 0;
        return base.Decrypt(key);
    }

    protected override char Decode(in LinkedListNode<(char, int)> node, string key)
    {
        return (char)((Alphabet.English[node.Value.Item1] + 26 - Alphabet.English[key[position++ % key.Length]]) % 26);
    }
}