using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float speed;
    public float damage;
    public float radius;
    private Rigidbody2D rigidbody;

	void Start () {
        rigidbody = GetComponent<Rigidbody2D>();
    }
	
	void Update () {
        Collider2D[] colliders = new Collider2D[128];
        Physics2D.OverlapCircle(transform.position, radius, new ContactFilter2D(), colliders);
        foreach(Collider2D col in colliders)
        {
            if (col == null)
                break;
            if(col.gameObject.tag == "Untagged" || col.gameObject.tag == "Destroy")
                Destroy(gameObject);
            if(col.gameObject.tag == "Zombie")
            {
                Zombie comp = col.gameObject.GetComponent<Zombie>();
                if (comp.dead)
                    continue;
                float r = Random.Range(0f, 1f);
                float dm = damage;
                if (r < comp.criticalRate)
                    dm *= 2;
                comp.EatDamage(Random.Range(dm - dm / 5, dm + dm / 5));
                Destroy(gameObject);
            }
        }
	}

    void FixedUpdate()
    {
        rigidbody.velocity = new Vector2(speed * Time.deltaTime, 0f);
    }
}
