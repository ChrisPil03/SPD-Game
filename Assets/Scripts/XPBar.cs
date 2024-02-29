using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPBar : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void UpdateXPBar(int playerXP)
    {
        float totalFramesInAnimation = 15f;

        for (int frame = 1; frame <= totalFramesInAnimation; frame++)
        {
            float normalizedTime = frame / totalFramesInAnimation;

            if (frame == totalFramesInAnimation)
            {
                anim.Play("XP bar", 0, 14/15f);
                return;
            }

            if ((playerXP / 100f) <= normalizedTime)
            {
                anim.Play("XP bar", 0, normalizedTime);
                return;
            }
        }
    }
}
