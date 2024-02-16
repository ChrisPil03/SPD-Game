using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void UpdateHealthBar(int playerHealth)
    {
        float totalFramesInAnimation = 17;

        for (int frame = 1; frame <= totalFramesInAnimation; frame++)
        {
            float normalizedTime = frame / totalFramesInAnimation;

            if (frame == 17)
            {
                anim.Play("Health bar", 0, 16/17f);
                return;
            }

            if (1 - (playerHealth / 100f) <= normalizedTime)
            {
                anim.Play("Health bar", 0, normalizedTime);
                return;
            }
        }
    }
}
