using UnityEngine;

public class MetroShooter : MonoBehaviour
{
    [SerializeField] GameObject metroPrefab;
    [SerializeField] float timeBetweenShots;
    [SerializeField] float speed;
    [SerializeField] Color color;
    [SerializeField] bool toRight;

    public void ShootMetro(float speed)
    {
        Metro metro = Instantiate(metroPrefab, transform.position, transform.rotation).GetComponent<Metro>();
        metro.SetInitialState(toRight ? speed : - speed, color);
    }
}
