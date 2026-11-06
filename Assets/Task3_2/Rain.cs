using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{
    public GameObject rainEffect;
    public float moveSpeed = 3f;

    public List<GameObject> humans;

    private int currentIndex = 0;
    private Transform cloud;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isRaining = false;
    private ParticleSystem rainParticles;

    // Start is called before the first frame update
    void Start()
    {
        cloud = GetComponent<Transform>();
        targetPosition = cloud.position;
        if (rainEffect != null)
        {
            rainParticles = rainEffect.GetComponent<ParticleSystem>();

            if (rainParticles != null)
            {
                rainParticles.Stop();
                isRaining = false;
            }
        }
        else
        {
            Debug.LogWarning("Rain effect is not assigned!");
        }
        if (humans == null || humans.Count == 0)
        {
            Debug.LogWarning("Humans list is empty!");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        SmoothMove();
        UpdateRain();
    }
    private void SmoothMove()
    {
        if (cloud == null || !isMoving) return;
        cloud.position = Vector3.Lerp(cloud.position, targetPosition, moveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(cloud.position, targetPosition);
        if (distance < 1f)
        {
            StartRain();
        }
        if (distance < 0.1f)
        {
            cloud.position = targetPosition;
            isMoving = false;
        }
    }

    private void UpdateRain()
    {
        if (rainParticles == null) return;
    }

    private void MoveCloud(int idx)
    {
        if (humans == null || idx < 0 || idx >= humans.Count || humans[idx] == null || cloud == null)
            return;

        StopRain();

        targetPosition = new Vector3(
            humans[idx].transform.position.x,
            cloud.position.y,
            humans[idx].transform.position.z
        );

        isMoving = true;
        isRaining = false;
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            MoveCloud(currentIndex);
            NextHuman();
        }
    }

    private void NextHuman()
    {
        if (humans == null || humans.Count == 0) return;
        currentIndex = (currentIndex + 1) % humans.Count;
    }

    private void StartRain()
    {
        if (rainParticles != null && !isRaining)
        {
            rainParticles.Play();
            isRaining = true;
            Debug.Log("Rain started!");
        }
    }

    private void StopRain()
    {
        if (rainParticles != null && isRaining)
        {
            rainParticles.Stop();
            isRaining = false;
            Debug.Log("Rain stopped!");
        }
    }
}