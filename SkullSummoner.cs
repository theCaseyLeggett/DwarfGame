using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullSummoner : MonoBehaviour {

    public Transform[] patrolWaypoints;
    public float patrolSpeed = 0.012f;
    public float stayTime;
    public GameObject skeleton;
    public GameObject skullMissiles;
    public Transform spawnPoint;
    public GameObject summonEffect;

    private Animator anim;
    private int currentWaypoint;
    private Vector2 target;
    private bool coroutineRunning;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isAsleep", true);
        summonEffect.SetActive(false);
        coroutineRunning = false;
    }
    
    public void AwakenSkullSummoner()
    {
       GetComponent<NPC>().enabled = true;
       anim.SetBool("isAsleep", false);
       StartCoroutine("Summon");
       coroutineRunning = true;
    }

    private void SummonSkeleton()
    {
        Instantiate(skeleton, spawnPoint.position, spawnPoint.rotation);
    }

    private void SummonSkullMissiles()
    {
        Instantiate(skullMissiles, spawnPoint.position, spawnPoint.rotation);
    }

    public void ResetSkullSummoner()
    {
        anim.SetBool("isAsleep", true);
        GetComponent<NPC>().enabled = false;
        summonEffect.SetActive(false);
        coroutineRunning = false;
        currentWaypoint = 0;
    }

    IEnumerator Summon()
    {
        while (true)
        {
            if (transform.position.x == patrolWaypoints[currentWaypoint].position.x)
            {
                currentWaypoint++;
                anim.SetBool("isSummoning", true);
                summonEffect.SetActive(true);
                summonEffect.GetComponent<AudioSource>().Play();
                if (currentWaypoint == 4)
                {
                    Invoke("SummonSkullMissiles", stayTime / 3);
                    Invoke("ResetBool", stayTime);
                    yield return new WaitForSeconds(stayTime * 2f);
                }
                else
                {
                    Invoke("SummonSkeleton", stayTime / 3);
                    Invoke("ResetBool", stayTime);
                    yield return new WaitForSeconds(stayTime);
                }
            }

            if (currentWaypoint >= patrolWaypoints.Length)
            {
                currentWaypoint = 0;
            }

            if (transform.position.x > patrolWaypoints[currentWaypoint].position.x)
            {
                transform.localScale = new Vector2(-1, 1);
            }
            else if (transform.position.x < patrolWaypoints[currentWaypoint].position.x)
            {
                transform.localScale = new Vector2(1, 1);
            }

            target = new Vector2(patrolWaypoints[currentWaypoint].position.x, patrolWaypoints[currentWaypoint].position.y);

            transform.position = Vector2.MoveTowards(transform.position, target, patrolSpeed);
            
            yield return null;
        }
    }

    private void ResetBool()
    {
        anim.SetBool("isSummoning", false);
        summonEffect.SetActive(false);
    }

    private void OnEnable()
    {
        if(anim != null)
        anim.SetBool("isAsleep", true);
    }

}

