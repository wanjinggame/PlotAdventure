using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBreaker : MonoBehaviour
{
    [Header("����С������Ĵ�С")]
    public float cubeSize = 0.2f;
    [Header("����cube����  cubesInRow*cubesInRow*cubesInRow")]
    public int cubesInRow = 3;

    float cubesPivotDistance;
    Vector3 cubesPivot;
    [Header("��ը����С")]
    public float explosionForce = 50f;
    [Header("��ը���뾶��Χ")]
    public float explosionRadius = 4f;
    [Header("��ը�����������·�ʩ����")]
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

