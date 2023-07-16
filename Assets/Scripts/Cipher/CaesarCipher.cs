using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CaesarCipher : BaseCipher<int>
{
    public void Awake()
    {
        cipherType = CipherType.Caesar;
    }

    protected override char Encode(in LinkedListNode<(char, int)> node, int key)
    {
        return (char)((node.Value.Item2 + key) % 26);
    }

    protected override char Decode(in LinkedListNode<(char, int)> node, int key)
    {
        return (char)((node.Value.Item2 + 26 - key) % 26);
    }
}