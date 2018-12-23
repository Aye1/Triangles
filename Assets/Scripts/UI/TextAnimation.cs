using UnityEngine;
using UnityEditor;
using System;

public class TextAnimation : MonoBehaviour
{
    Animation animation;
    
    private void Start()
    {
        animation = GetComponent<Animation>();
        InternalStart();
    }
    protected virtual void InternalStart()
    {
        throw new NotImplementedException();
    }
    protected void PlayAnimation()
    {
        animation.Play();
    }
}