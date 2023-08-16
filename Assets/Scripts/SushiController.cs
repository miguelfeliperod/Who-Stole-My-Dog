using UnityEngine;

public class SushiController : MonoBehaviour
{
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private float timer = 0;
    private AudioManager audioManager;
    [SerializeField] LayerMask playerLayer;

    [SerializeField] private Collider2D coinCollider;

    public AnimationCurve movementCurve;
    public Vector3 verticalVariation = new Vector3(0, 1, 0);
    public float animationDuration;

    void Start()
    {
        audioManager = AudioManager.instance;
        originalPosition = transform.position;
        targetPosition = originalPosition + verticalVariation;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > animationDuration)
        {
            originalPosition = transform.position;
            verticalVariation = - verticalVariation;
            targetPosition = originalPosition + verticalVariation;
            timer = 0;
        }
         transform.position = Vector2.Lerp(originalPosition, targetPosition, movementCurve.Evaluate(timer / animationDuration));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.GetComponent<Collider2D>().gameObject.layer & playerLayer) == 0) return;
        // audioManager.PlayOneShot
        GameManager.Instance.playerController.GetFood(1);
        Destroy(gameObject);
    }
}
