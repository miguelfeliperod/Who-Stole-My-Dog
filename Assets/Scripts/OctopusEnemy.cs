using System.Collections;
using UnityEngine;

public class OctopusEnemy : BaseEnemy
{
    [SerializeField] bool isGood = true;
    [SerializeField] float changeTimer = 0;
    [SerializeField] AudioClip sfx;
    [SerializeField] AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    private void Update()
    {
        changeTimer += Time.deltaTime;

        if (changeTimer >= attackRate) { 
            StartCoroutine(ChangeForm());
            changeTimer = 0;
        }
    }

    IEnumerator ChangeForm()
    {
        isGood = !isGood;
        animator.SetBool("isGood", isGood);
        audioSource.PlayOneShot(sfx);
        yield return new WaitForSeconds(0.8f);
        if (isGood) boxCollider.gameObject.layer = LayerMask.NameToLayer("Default");
        else boxCollider.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }
}
