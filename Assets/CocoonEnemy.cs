using System.Collections;
using UnityEngine;

public class CocoonEnemy : BaseEnemy
{
    [SerializeField] float timeToBorn;
    [SerializeField] GameObject butterflyPrefab;
    bool spawned = false;
    float timer = 0;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeToBorn && !spawned)
        {
            StartCoroutine(ButterflyBorn());
            spawned = true;
        }
    }

    IEnumerator ButterflyBorn()
    {
        animator.Play("CocoonDie");
        Instantiate(butterflyPrefab, transform.position, transform.rotation);
        yield return null;
        StartCoroutine(Die());
    }
}
