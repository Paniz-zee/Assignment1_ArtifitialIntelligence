using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public GameObject characterPrefab;
    public GameObject targetPrefab;
    public GameObject enemyPrefab;

    private GameObject character;
    private GameObject target;
    private GameObject enemy;

    public AudioClip seekSound;
    public AudioClip fleeSound;
    public AudioClip arrivalSound;
    public AudioClip avoidanceSound;
    private AudioSource audioSource;

    private float circularRadius = 8f;
    private float circularSpeed = 3.5f;

    private float angle = 0f;


    private void Start()
    {

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.2f;

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) StartSeek();
        if (Input.GetKeyDown(KeyCode.Alpha2)) StartFlee();
        if (Input.GetKeyDown(KeyCode.Alpha3)) StartArrival();
        if (Input.GetKeyDown(KeyCode.Alpha4)) StartAvoidance();
        if (Input.GetKeyDown(KeyCode.Alpha0)) ResetScene();

        if (character != null && Input.GetKey(KeyCode.Alpha1))
        {
            PerformCircularMovement();
        }
    }

    void StartSeek()
    {
        ResetScene();
        SpawnObjects();

        //Vector3 direction = (target.transform.position - character.transform.position).normalized;
        //character.GetComponent<Rigidbody2D>().velocity = direction * 5f;
        PlaySound(seekSound);
        if (character != null && target != null)
        {
            Rigidbody2D rb = character.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }
        }


    }


    void PerformCircularMovement()
    {

        if (character == null || target == null) return;
        angle += circularSpeed * Time.deltaTime;

        float x = Mathf.Cos(angle) * circularRadius;
        float y = Mathf.Sin(angle) * circularRadius;
        Vector3 center = target.transform.position;
        Vector3 newPosition = new Vector3(center.x + x, center.y + y, character.transform.position.z);

        Rigidbody2D rb = character.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector3 direction = (newPosition - character.transform.position).normalized;
            rb.velocity = direction * circularSpeed;
        }
    }

    void StartFlee()
    {
        ResetScene();
        SpawnObjects(true);
        if (character == null || enemy == null)
        {
            Debug.LogError("Character or Enemy is null in StartFlee!");
            return;
        }

        Vector3 direction = (character.transform.position - enemy.transform.position).normalized;
        character.GetComponent<Rigidbody2D>().velocity = direction * 3.5f;
        PlaySound(fleeSound);

    }


    void StartArrival()
    {
        ResetScene();
        SpawnObjects();

        StartCoroutine(MoveCharacterToTarget());
        PlaySound(arrivalSound);

    }

    void StartAvoidance()
    {
        ResetScene();
        SpawnObjects(true);

        StartCoroutine(MoveCharacterAvoidingHazard());
        PlaySound(avoidanceSound);

    }

    void ResetScene()
    {
        StopAllCoroutines();

        if (character != null) Destroy(character);
        if (target != null) Destroy(target);
        if (enemy != null) Destroy(enemy);

        character = null;
        target = null;
        enemy = null;
    }

    void SpawnObjects(bool includeEnemy = false)
    {


        character = Instantiate(characterPrefab, GetRandomPosition(), Quaternion.identity);
        character.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        AudioSource characterAudioSource = character.GetComponent<AudioSource>();
        if (characterAudioSource != null && !characterAudioSource.enabled)
        {
            characterAudioSource.enabled = true;
        }
        target = Instantiate(targetPrefab, GetRandomPosition(), Quaternion.identity);
        target.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);


        if (includeEnemy)
        {
            enemy = Instantiate(enemyPrefab, GetRandomPosition(), Quaternion.identity);
            enemy.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }

    Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-7f, 7f), Random.Range(-4f, 4f), 0);
    }

    IEnumerator MoveCharacterToTarget()
    {
        if (character == null || target == null) yield break;

        Rigidbody2D rb = character.GetComponent<Rigidbody2D>();

        while (character != null && target != null && Vector3.Distance(character.transform.position, target.transform.position) > 0.1f)
        {
            Vector3 direction = (target.transform.position - character.transform.position).normalized;
            rb.velocity = direction * Mathf.Clamp(Vector3.Distance(character.transform.position, target.transform.position), 0, 3.5f);
            yield return null;
        }

        if (rb != null)
            rb.velocity = Vector3.zero;
    }

    IEnumerator MoveCharacterAvoidingHazard()
    {
        if (character == null || target == null || enemy == null) yield break;

        Rigidbody2D rb = character.GetComponent<Rigidbody2D>();

        while (character != null && target != null && Vector3.Distance(character.transform.position, target.transform.position) > 0.1f)
        {
            Vector3 direction = (target.transform.position - character.transform.position).normalized;

            if (enemy != null && Vector3.Distance(character.transform.position, enemy.transform.position) < 2f)
            {
                direction += (character.transform.position - enemy.transform.position).normalized;
            }

            rb.velocity = direction.normalized * 3.5f;
            yield return null;
        }

        if (rb != null)
            rb.velocity = Vector3.zero;
    }
    void PlaySound(AudioClip clip, float volume = 1f)
    {

        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);

        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip is missing!");
        }
    }
}

