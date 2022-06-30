using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CoinMove : MonoBehaviour
{
    public Transform targetPos;
    public float moveSpeed;

    float curTimer;
    [SerializeField] AudioClip getAudio;

    private void Start()
    {
        curTimer = moveSpeed;

        GetComponent<AudioSource>().PlayOneShot(getAudio);
    }

    void Update()
    {
        curTimer -= Time.deltaTime;
        if (curTimer <= 0)
        {
            Destroy(gameObject);
            return;
        }

        if (targetPos)
        {
            //transform.DOMove(targetPos.position, moveSpeed);
        }

        
    }
}
