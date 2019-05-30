using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public float speed;
    public float health;
    private float maxHealth;
    public Image healthBar;
    public Text healthText;
    private int equipment;
    private float movement;
    private Rigidbody2D rigidbody;
    private Animator animator;
    public float startingTime;
    private float t_startingTime = 0;
    public Transform attackPoint;
    public float attackRadius;
    public float swordDamage;
    private bool attacking = false;
    private bool attacking2 = false;
    public float attackTime;
    private float t_attackTime = 0;
    private bool injured = false;
    public float injureTime;
    private float t_injureTime = 0;
    private bool dead;
    public GameObject gunBullet;
    public Transform gunFirePoint;
    public GameObject shotgunBullet;
    public Transform shotgunFirePoint;
    public Text[] texts;
    private int[] counts;
    public bool powered = true;
    public float powerTime;
    private float t_powerTime;
    private float[] gunHealth;
    public Image[] gunUI;
    public Image[] gunBars;
    public AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioSource audioSource3;
    private bool audioStarted = false;

	void Start () {
        gunHealth = new float[2];
        gunHealth[0] = 0;
        gunHealth[1] = 0; 
        equipment = -1;
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        maxHealth = health;
        counts = new int[texts.Length];
        for (int i = 0; i < counts.Length; i++)
            counts[i] = 0;
    }
	
	void Update () {
        if (dead)
            return;
        SoundEvents();
        StartingEvents();
        ChangingEquipment();
        AttackingEvents();
        InjuringEvents();
        CaseOpeningEvents(); 
        EffectEvents();   
        UpdatePanel();
        animator.SetBool("Attacking", attacking);
        animator.SetBool("Walking", movement != 0f);
        animator.SetInteger("Equipment", equipment);
        animator.SetFloat("Health", health);
        animator.SetBool("Injured", injured);
        animator.SetBool("Attacking2", attacking2);
	}

    private void FixedUpdate()
    {
        if (dead)
            return; 
        movement = Input.GetAxis("Horizontal");
        rigidbody.velocity = new Vector2(speed * movement * Time.deltaTime, rigidbody.velocity.y);
        if (movement < 0f)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        if (movement > 0f)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    private void Attack()
    {
        Collider2D[] colliders = new Collider2D[128];
        Physics2D.OverlapCircle(attackPoint.position, attackRadius, new ContactFilter2D(), colliders);
        foreach(Collider2D zm in colliders)
        {
            if (zm == null)
                break;
            if(zm.gameObject.tag == "Zombie")
            {
                Zombie zombie = zm.gameObject.GetComponent<Zombie>();
                float dm = swordDamage;
                if(Random.Range(0f, 1f) < zombie.criticalRate)
                {
                    dm *= 2;
                }
                if (powered)
                    dm *= 2;
                dm = Random.Range(dm - dm / 5f, dm + dm / 5f);
                audioSource3.Play();
                zombie.EatDamage(dm);
            }
        }
    }

    private void ShootGun()
    {
        audioSource2.Play();
        gunHealth[0] = Mathf.Clamp(gunHealth[0] - 0.025f, 0f, 1f);
        GameObject bullet = Instantiate(gunBullet, gunFirePoint.position, gunFirePoint.rotation);
        counts[2]--;
        if(transform.localScale.x < 0f)
        {
            bullet.transform.localScale = new Vector2(-Mathf.Abs(bullet.transform.localScale.x), bullet.transform.localScale.y);
            bullet.GetComponent<Bullet>().speed = -bullet.GetComponent<Bullet>().speed;
            if (powered)
                bullet.GetComponent<Bullet>().damage *= 2f;
        } 
    }

    private void ShootShotgun()
    {
        audioSource2.Play();
        gunHealth[1] = Mathf.Clamp(gunHealth[1] - 0.025f, 0f, 1f);
        GameObject bullet = Instantiate(shotgunBullet, shotgunFirePoint.position, shotgunFirePoint.rotation);
        counts[3]--;
        if (transform.localScale.x < 0f)
        {
            bullet.transform.localScale = new Vector2(-Mathf.Abs(bullet.transform.localScale.x), bullet.transform.localScale.y);
            bullet.GetComponent<Bullet>().speed = -bullet.GetComponent<Bullet>().speed;
            if (powered)
                bullet.GetComponent<Bullet>().damage *= 1.5f;
        }
    }

    public void EatDamage(float damage)
    {
        health = Mathf.Clamp(health - damage, 0f, maxHealth);
        injured = true;
    }

    private void StartingEvents()
    {
        if (t_startingTime < startingTime)
        {
            t_startingTime += Time.deltaTime;
        }
        if (t_startingTime > startingTime && equipment == -1)
        {
            equipment = 0;
        }
    }

    private void EffectEvents()
    {
        if(Input.GetKeyDown("r") && counts[0] > 0)
        {
            health = Mathf.Clamp(health + maxHealth / 4, 0f, maxHealth);
            counts[0]--;
        }
        if(Input.GetKeyDown("t") && !powered && counts[1] > 0)
        {
            powered = true;
            counts[1]--;
        }
        if (powered)
        {
            t_powerTime += Time.deltaTime;
        }
        if(t_powerTime > powerTime)
        {
            powered = false;
            t_powerTime = 0;
        }
    }

    private void AttackingEvents()
    {
        if(equipment == 0)
        {
            if (Input.GetMouseButtonDown(0) && !attacking && !injured)
            {
                attacking = true;
                Attack();
            }
            if (attacking)
            {
                t_attackTime += Time.deltaTime;
            }
            if (t_attackTime > attackTime)
            {
                t_attackTime = 0;
                attacking = false;
            }
        }
        if(equipment == 1)
        {
            if (Input.GetMouseButtonDown(0) && !attacking && !injured && gunHealth[0] > 0f && counts[2] > 0)
            {
                attacking = true;
                ShootGun();
            }
            if (Input.GetMouseButtonDown(1) && !attacking && !injured)
            {
                attacking = true;
                attacking2 = true;
                Attack();
            }
            if (attacking)
            {
                t_attackTime += Time.deltaTime;
            }
            if (t_attackTime > attackTime)
            {
                t_attackTime = 0;
                attacking = false;
                attacking2 = false;
            }
        }
        if(equipment == 2)
        {
            if (Input.GetMouseButtonDown(0) && !attacking && !injured && gunHealth[1] > 0f && counts[3] > 0)
            {
                attacking = true;
                ShootShotgun();
            }
            if (attacking)
            {
                t_attackTime += Time.deltaTime;
            }
            if (t_attackTime > attackTime)
            {
                t_attackTime = 0;
                attacking = false;
            }
        }
    }

    private void InjuringEvents()
    {
        if(injured)
        {
            t_injureTime += Time.deltaTime;
        }
        if(t_injureTime > injureTime)
        {
            injured = false;
            t_injureTime = 0;
        }
        if(health <= 0)
        {
            dead = true;
            WaveSpawner.instance.dead = true;
            equipment = -1;
        }
        if(dead)
        {
            Destroy(gameObject, 1f);
        }
        healthBar.fillAmount = health / maxHealth;
        healthText.text = "%"+ ((int)(health * 100 / maxHealth)).ToString();
    }

    private void CaseOpeningEvents()
    {
        if(Input.GetKeyDown("e"))
        {
            Collider2D[] colliders = new Collider2D[128];
            Physics2D.OverlapCircle(attackPoint.position, attackRadius, new ContactFilter2D(), colliders);
            foreach(Collider2D col in colliders)
            {
                if (col == null)
                    break;
                if(col.tag == "Case")
                {
                    Case obj = col.gameObject.GetComponent<Case>();
                    int i = obj.OpenCase();
                    if (i == 0)
                        counts[0]++;
                    if (i == 1)
                        counts[1]++;
                    if (i == 2)
                        gunHealth[0] = 1f;
                    if (i == 3)
                        gunHealth[1] = 1f;
                    if (i == 4)
                        counts[2] += Random.Range(7, 15);
                    if (i == 5)
                        counts[3] += Random.Range(5, 10);
                    WaveSpawner.instance.caseOpened = true;
                }
            }
        }
    }

    private void ChangingEquipment()
    {
        if (equipment == 1 && gunHealth[0] <= 0f)
            equipment = 0;
        if (equipment == 2 && gunHealth[1] <= 0f)
            equipment = 0;
        if (Input.GetKeyDown("2") && gunHealth[0] >= 0f)
            equipment = 1;
        if (Input.GetKeyDown("3") && gunHealth[1] >= 0f)
            equipment = 2;
        if (Input.GetKeyDown("1"))
            equipment = 0;
    }

    void SoundEvents()
    {
        if (!audioStarted && movement != 0f)
        {
            audioSource.Play();
            audioStarted = true;
        }
            
        if (audioStarted && movement == 0f)
        {
            audioSource.Pause();
            audioStarted = false;
        }
            
    }

    private void UpdatePanel()
    {
        for(int i = 0; i < texts.Length; i++)
            texts[i].text = "x" + counts[i].ToString();
        texts[0].text += " (R)";
        texts[1].text += " (T)";
        gunUI[0].gameObject.SetActive(gunHealth[0] > 0f);
        gunUI[1].gameObject.SetActive(gunHealth[1] > 0f);
        gunBars[0].fillAmount = gunHealth[0];
        gunBars[1].fillAmount = gunHealth[1];
        if (equipment == 0)
        {
            gunUI[0].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            gunUI[1].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
        }
        if (equipment == 1)
        {
            gunUI[0].GetComponent<Image>().color = new Color(1f, 1f, 1f);
            gunUI[1].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
        }
        if (equipment == 2)
        {
            gunUI[0].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            gunUI[1].GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
    }
}
