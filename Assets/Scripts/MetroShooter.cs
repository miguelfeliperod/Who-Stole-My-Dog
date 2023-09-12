using System.Threading;
using UnityEditor.Rendering;
using UnityEngine;

public class MetroShooter : MonoBehaviour
{
    [SerializeField] GameObject metroPrefab;
    [SerializeField] float timeBetweenShots;
    [SerializeField] float speed;
    [SerializeField] Color color;
    float timer = 0;
    
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeBetweenShots)
        {
            timer = 0;
            ShootMetro(speed, color);
        }
    }

    void ShootMetro(float speed, Color color)
    {
        Metro metro = Instantiate(metroPrefab, transform.position, transform.rotation).GetComponent<Metro>();
        metro.SetInitialState(speed, color);
    }
}
