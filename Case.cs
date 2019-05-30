using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Case : MonoBehaviour {

    public float destroyTime;
    private bool opened = false;
    public Animator animator;
    public Transform iconLocation;
    public GameObject[] items;
    public float[] percentage;

    public int OpenCase()
    {
        if (opened)
            return -1;
        opened = true;
        animator.SetBool("CaseOpen", true);
        float rand = Random.Range(0f, 1f);
        int eq = 0;
        if (rand < sum(1))
            eq = 0;
        else if (rand < sum(2))
            eq = 1;
        else if (rand < sum(3))
            eq = 2;
        else if (rand < sum(4))
            eq = 3;
        else if (rand < sum(5))
            eq = 4;
        else
            eq = 5;
        GameObject obj = Instantiate(items[eq], iconLocation.position, iconLocation.rotation);
        Destroy(gameObject, destroyTime);
        Destroy(obj, destroyTime); 
        return eq;
    }

    private float sum(int x)
    {
        float s = 0;
        for(int i = 0; i < x; i++)
        {
            s += percentage[i];
        }
        return s;
    }
}
