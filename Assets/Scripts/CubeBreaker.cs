using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBreaker : MonoBehaviour
{
    [Header("生成小正方体的大小")]
    public float cubeSize = 0.2f;
    [Header("生成cube数量  cubesInRow*cubesInRow*cubesInRow")]
    public int cubesInRow = 3;

    float cubesPivotDistance;
    Vector3 cubesPivot;
    [Header("爆炸力大小")]
    public float explosionForce = 50f;
    [Header("爆炸力半径范围")]
    public float explosionRadius = 4f;
    [Header("爆炸。。。物体下方施加力")]
    public float explosionUpward = 0.4f;
    private GameObject piece;
    void Start()
    {
        cubesPivotDistance = cubeSize * cubesInRow / 2;

        cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Plane")
        {
            explode();
        }

    }

    public void explode()
    {

        gameObject.SetActive(false);


        for (int x = 0; x < cubesInRow; x++)
        {
            for (int y = 0; y < cubesInRow; y++)
            {
                for (int z = 0; z < cubesInRow; z++)
                {
                    createPiece(x, y, z);
                }
            }
        }


        Vector3 explosionPos = transform.position;

        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {

                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
            }
        }

    }

    void createPiece(int x, int y, int z)
    {

        piece = GameObject.Instantiate(Resources.Load("Cube")) as GameObject;
        piece.transform.position = transform.position + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z) - cubesPivot;
        piece.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

        piece.AddComponent<DestorySelf>();
        piece.AddComponent<Rigidbody>();
        piece.GetComponent<Rigidbody>().mass = cubeSize;
    }

}
public class DestorySelf : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 5);
    }
}

