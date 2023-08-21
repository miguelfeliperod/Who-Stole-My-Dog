using System.Collections;
using UnityEngine;

public class OctopusEnemy : BaseEnemy
{
    [SerializeField] bool isGood = true;
    [SerializeField] float changeTimer = 0;

    private void Update()
    {
        changeTimer += Time.deltaTime;
        if (changeTimer >= attackRate) StartCoroutine(ChangeForm());
    }

    IEnumerator ChangeForm()
    {
        isGood = !isGood;
        animator.SetBool("isGood", isGood);
        changeTimer = 0;
        yield return new WaitForSeconds(0.8f);
        if (isGood) boxCollider.gameObject.layer = LayerMask.NameToLayer("Default");
        else boxCollider.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }
}
