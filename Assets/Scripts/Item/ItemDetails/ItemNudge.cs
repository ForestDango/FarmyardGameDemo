using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ڵ��������ָ�����͵���Ʒʱ����Ʒ�ᷢ�����һζ��Ľ���
/// </summary>
public class ItemNudge : MonoBehaviour
{
    [SerializeField] private float pauseTime = 0.04f;
    private WaitForSeconds pause;
    private bool isAnimating = false;

    private void Awake()
    {
        pause = new WaitForSeconds(pauseTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAnimating)
        {
            if(gameObject.transform.position.x < collision.transform.position.x) //��������
            {
                StartCoroutine(nameof(RotateAndClock));
            }
            else
            {
                StartCoroutine(nameof(RotateClock));
            }
        }

        if(collision.gameObject.tag == "Player")
        {
            AudioManager.Instance.PlaySound(SoundName.effectRustle);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isAnimating)
        {
            if (gameObject.transform.position.x > collision.transform.position.x) //��������
            {
                StartCoroutine(nameof(RotateAndClock));
            }
            else
            {
                StartCoroutine(nameof(RotateClock));
            }
        }

        if (collision.gameObject.tag == "Player")
        {
            AudioManager.Instance.PlaySound(SoundName.effectRustle);
        }
    }

    private IEnumerator RotateAndClock()
    {
        isAnimating = true;

        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);
            yield return pause;
        }

        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
            yield return pause;
        }

        gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);

        yield return pause;

        isAnimating = false;
    }

    private IEnumerator RotateClock()
    {
        isAnimating = true;

        for (int i = 0; i < 4; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
            yield return pause;
        }

        for (int i = 0; i < 5; i++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);
            yield return pause;
        }

        gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);

        yield return pause;

        isAnimating = false;
    }
}
