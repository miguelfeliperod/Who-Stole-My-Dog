using System.Collections;
using UnityEngine;

public class CocoonEnemy : BaseEnemy
{
    [SerializeField] float timeToBorn;
    [SerializeField] GameObject butterflyPrefab;
    [SerializeField] GameObject sushiPrefab;
    [SerializeField] AudioClip sfx;
    bool spawned = false;
    float timer = 0;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeToBorn && !spawned && !isDead)
        {
            StartCoroutine(ButterflyBorn());
            spawned = true;
        }
    }

    public override IEnumerator Die()
    {
        if(Random.Range(0,2) == 0) 
            Instantiate(sushiPrefab, transform.position + new Vector3(0,1), transform.rotation);
        GameManager.Instance.audioManager.PlaySFX(sfx, pitch: 0.6f);
        return base.Die();
    }

    IEnumerator ButterflyBorn()
    {
        animator.Play("CocoonDie");
        Instantiate(butterflyPrefab, transform.position, transform.rotation);
        yield return null;
        GameManager.Instance.audioManager.PlaySFX(sfx, pitch: 0.6f);
        StartCoroutine(base.Die());
    }
}
