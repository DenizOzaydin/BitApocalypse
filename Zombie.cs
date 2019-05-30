using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour {

    public float speed;
    public float health;
    private float maxHealth;
    private Rigidbody2D rigidbody;
    private Animator animator;
    public float damage;
    public Transform attackPoint;
    public float attackRadius;
    private bool attacking = false;
    private bool attacked = false;
    public float attackTime;
    private float t_attackTime = 0;
    public float timeBetweenTwoAttacks;
    private float t_timeBetweenTwoAttacks;
    private bool injured = false;
    public float injureTime;
    public float criticalRate;
    private float t_injureTime = 0;
    public bool dead = false;
    public float dyingTime;
    public GameObject healthBar;
    public GameObject redBar;
    private float maxScale;
    public AudioSource audioSource;
    private float t_randomaudio = 0f;
    private float t_nextaudio;

	void Start () {
        t_nextaudio = Random.Range(2f, 5f);
        maxHealth = health;
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        maxScale = healthBar.transform.localScale.x;
	}
	
	void Update () {
        if (dead)
            return;
        AttackingEvents();
        InjuringEvents();
        SoundEvents();
        animator.SetBool("Injured", injured);
        animator.SetFloat("Health", health); 
	}

    private void FixedUpdate()
    {
        if (!dead)
            rigidbody.velocity = new Vector2(speed * Time.deltaTime, rigidbody.velocity.y);
        if(injured)
            rigidbody.velocity = new Vector2(speed * Time.deltaTime / 2, rigidbody.velocity.y);
        if (dead)
            rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
    }

    public void EatDamage(float damage)
    {
        health = Mathf.Clamp(health - damage, 0f, maxHealth);
        WaveSpawner.instance.score += (int)maxHealth;
        injured = true;
    }

    private void AttackingEvents()
    {
        Collider2D[] cols = new Collider2D[128];
        Physics2D.OverlapCircle(attackPoint.position, attackRadius, new ContactFilter2D(), cols);
        bool inRange = false;
        foreach(Collider2D player in cols)
        {
            if (player == null)
                 break;
            if (player.tag == "Player")
                inRange = true;
        }
        if (inRange)
            t_timeBetweenTwoAttacks += Time.deltaTime;
        else
            t_timeBetweenTwoAttacks = 0;
        if(t_timeBetweenTwoAttacks > timeBetweenTwoAttacks && !attacking)
        {
            Collider2D[] colliders = new Collider2D[128];
            Physics2D.OverlapCircle(attackPoint.position, attackRadius, new ContactFilter2D(), colliders);
            foreach(Collider2D collider in colliders)
            {
                if (collider == null)
                    return;
                if(collider.tag == "Player")
                {
                    Player player = collider.gameObject.GetComponent<Player>();
                    player.EatDamage(Random.Range(damage - damage / 5, damage + damage / 5));
                    t_timeBetweenTwoAttacks = 0;
                    attacking = true;
                }
            }
        }
        if(attacking)
        {
            t_attackTime += Time.deltaTime;
            if(t_attackTime > attackTime)
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
            t_injureTime = 0;
            injured = false;
        }
        if(health <= 0)
        {
            dead = true;
            redBar.SetActive(false);
            WaveSpawner.instance.score += (int)(maxHealth * damage / timeBetweenTwoAttacks);
            Destroy(gameObject, dyingTime);
            WaveSpawner.instance.zombieCount--;
        }
        healthBar.transform.localScale = new Vector2(health * maxScale / maxHealth, healthBar.transform.localScale.y);
    }

    private void SoundEvents()
    {
        t_randomaudio += Time.deltaTime;
        if(t_randomaudio > t_nextaudio)
        {
            audioSource.Play();
            t_nextaudio = Random.Range(4f, 10f);   
        }
    }
}
