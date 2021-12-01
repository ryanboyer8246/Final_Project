using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RubyController : MonoBehaviour
{
    public GameObject LoseText;
    public GameObject WinText;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI NewScoreText;
    public GameObject JambiText;
    bool gameOver = true;

    public AudioClip throwCog;
    public AudioClip hitSound;
    public AudioClip WinMusic;
    public AudioClip LoseMusic;
    public AudioClip LoopMusic;
    public AudioClip JambiSound;
    public AudioClip TikuSound;
    public AudioClip CoinSound;

    public int score;
    public int newScore;

    public float speed = 3.0f;

    public int maxHealth = 5;

    public GameObject projectilePrefab;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public ParticleSystem healthEffect;
    public ParticleSystem hitEffect;

    public float timeInvincible = 2.0f;
    public bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;

    Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        renderer = GetComponent<Renderer>();

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = LoopMusic;
        audioSource.Play();

        LoseText.SetActive(false);
        WinText.SetActive(false);
        JambiText.SetActive(false);

        score = 0;
        ScoreText.text = "Robots Fixed: " + score.ToString();

        newScore = 0;
        NewScoreText.text = "";

        gameOver = false;

        currentHealth = maxHealth;
        
        if (SceneManager.GetActiveScene().name == "Level2")
        {
            ScoreText.text = "";
            NewScoreText.text = "Coins: " + newScore;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // Update is called once per frame SceneManager.GetActiveScene().buildIndex
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene("Main");
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                    if (SceneManager.GetActiveScene().name == "Main")
                    {
                        PlaySound(JambiSound);
                    }
                    else if (SceneManager.GetActiveScene().name == "Level2")
                    {
                        PlaySound(TikuSound);
                    }
                }
                if (score == 4)
                {
                    SceneManager.LoadScene("Level2");
                }
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            Instantiate(hitEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            hitEffect.Play();

            PlaySound(hitSound);
        }
        if (amount > 0)
        {
            Instantiate(healthEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            healthEffect.Play();
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if (currentHealth == 0)
        {
            Destroy(renderer);
            speed = 0;
            transform.position = new Vector2(1.49f, -3.72f);
            WinText.SetActive(false);
            LoseText.SetActive(true);
            audioSource.Stop();
            audioSource.clip = LoseMusic;
            audioSource.Play();
            gameOver = true;
        }
    }

    public void ChangeScore(int scoreAmount)
    {
        score = score + scoreAmount;
        ScoreText.text = "Robots Fixed: " + score.ToString();
        if (score == 4)
        {
            JambiText.SetActive(true);
        }
    }

    void OnCollisionEnter2D (Collision2D collision)
    {
        if (collision.collider.tag == "Coin")
        {
            newScore += 1;
            NewScoreText.text = "Coins: " + newScore.ToString();
            Destroy(collision.collider.gameObject);
            PlaySound(CoinSound);
        }
        if (collision.collider.tag =="Coin")
        {
            if (newScore == 8)
            {
                WinText.SetActive(true);
                audioSource.Stop();
                audioSource.clip = WinMusic;
                audioSource.Play();
                gameOver = true;
            }
        }
    }
    
    public void ChangeNewScore(int newScore)
    {
    
    }


    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwCog);
    }
} 