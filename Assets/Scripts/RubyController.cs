using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class RubyController : MonoBehaviour
{
    //Player defualt
    public GameObject Ruby;
    public float speed = 3.0f;
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;
    
    //audio
    AudioSource audioSource;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip healthSound;
    public AudioClip jambiSound;
    public AudioClip BackGroundMusic;
    public AudioClip GameOver;
    public AudioClip WinClip;

    // Speed
    public float boostingSpeed = 60.0f;
    private float boostTimer;
    private bool boosting;
    private bool moving;

    //timer
    public float timeValue = 90;
    public TextMeshProUGUI timerText;
    
    //texts
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI score;
    private int scoreValue = 0;
    public GameObject winTextObjects;
    public GameObject LoseTextObject;
    bool restart;
    
    public int health { get { return currentHealth; }}
    int currentHealth;

    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal; 
    float vertical;

    public GameObject healthIncrease;
    public GameObject healthDecrease;
    public GameObject projectilePrefab;
    public GameObject ammoTextObject;
    public int ammo { get { return cogs; }}
    public int cogs = 6;
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    public static int level;

    // Start is called before the first frame update
    void Start()
    {

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

        audioSource= GetComponent<AudioSource>();
        audioSource.clip = BackGroundMusic;
        audioSource.Play();
        audioSource.loop = true;

        score.text = "Robots Fixed: " + scoreValue.ToString() + "/6";
        winTextObjects.SetActive(false);
        LoseTextObject.SetActive(false);
        restart = false;

        // Speed
        speed = 3;
        boostTimer = 0;
        boosting = false;

    }

    // Update is called once per frame
    void Update()
    {
        //timer
        if (timeValue > 0)
        {
            timeValue -= Time.deltaTime;
            
             if ( timeValue < 0 )
            {
               LoseTextObject.SetActive(true);
               restart = true;
               audioSource= GetComponent<AudioSource>();
               audioSource.clip = GameOver;
               audioSource.Play();
               Destroy(gameObject.GetComponent<SpriteRenderer>());
            }
        }

        else
        {
            timeValue = 0;
        }
        DisplayTime(timeValue);

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
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

        //Speed
       // if (boosting == true;)
        
            //boostTimer += Time.deltaTime;
            //if(boostTimer >= 3)
            //  {
            //   speed = boostingSpeed;
            //  boostTimer = 0;
            //   boosting = false;
            // }

      //      speed = boostingSpeed;
       

       // if (boosting == false)
      //  {
     //       speed = 3.0f;
     //   }

        // Debug Key for boosting

        if(Input.GetKeyDown(KeyCode.B))
        {
            speed = boostingSpeed;
            boosting = true;
            Debug.Log("Boosting Active");
            Debug.Log("Currentspeed" + speed);
        }

        if (Input.GetKeyDown(KeyCode.L)) 
        {
            speed = 3.0f;
            boosting = false;
            Debug.Log("Bossting Deactived");
            Debug.Log("Current Speed is   " + speed);
        }

        if (moving)
        {
            this.transform.Translate(new Vector3(Time.deltaTime * speed, 0, 0));
        }

    
        if(Input.GetKeyDown(KeyCode.C))
        {
            if (cogs > 0)
            {
                Launch();
                AmmoCount(-1);
                AmmoText();
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
                    PlaySound(jambiSound);
                }
            }

             if (scoreValue >= 6)
            {
                SceneManager.LoadScene("Level2");
                
            }
                
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            {
                if (restart == true)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            SceneManager.LoadScene("Main Scene 1");
        }


    }


    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + 3.0f * horizontal * Time.deltaTime;
        position.y = position.y + 3.0f * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "SpeedBoost")
        {
            boosting = true;
            speed = 10;
            Destroy(other.gameObject);
        }
    }

    //timer
    void DisplayTime(float timeToDisplay)
    {
        if(timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }
        else if (timeToDisplay > 0)
        {
          timeToDisplay += 1; 
        }
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
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
            Instantiate(healthDecrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitSound);
        }

        if (amount > 0)
        {
            Instantiate(healthIncrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(healthSound);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if (currentHealth <= 0)
        {
            LoseTextObject.SetActive(true);
            transform.position = new Vector3(-5f, 0f, -100f);
            speed = 0;
            Destroy(gameObject.GetComponent<SpriteRenderer>());
            restart = true;


            audioSource.clip = BackGroundMusic;
            audioSource.Stop();
            audioSource.clip = GameOver;
            audioSource.Play();
            audioSource.loop = false;
        }
         
    }

    
    public void ChangeScore(int Scores)
    { 
        scoreValue += Scores;
        score.text = "Robots Fixed: " + scoreValue.ToString()  + "/6";

        if(scoreValue >= 6)
        {
            winTextObjects.SetActive(true);

            audioSource.clip = WinClip;
            audioSource.Play();
        
        }
       
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void AmmoText()
    {
        ammoText.text = "Ammo: " + cogs.ToString();
    }

    public void AmmoCount(int amount)
    {
        cogs = Mathf.Abs(cogs + amount);
        
    }
}

