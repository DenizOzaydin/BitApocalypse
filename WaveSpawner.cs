using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour {

    public static WaveSpawner instance;

    public int wave = 0;
    public float startingTime;
    private float t_startingTime;
    public float waitTime;
    private float t_waitTime = 0;
    public int zombieCount = 0;
    public GameObject[] zombiePrefabs;
    public Transform[] spawnPoints;
    private bool spawnContinueing = false;
    private bool waveContinueing = false;
    public TextMeshProUGUI waveInfo;
    private bool waveStarted = false;
    public Animator waveInfoAnimator;
    public float waveInfoTime;
    private float t_waveInfoTime = 0f;
    public float timeBetweenTwoWaves;
    private float t_timeBetweenTwoWaves = 0f;
    public GameObject casePrefab;
    public Transform[] caseSpawnPoints;
    public bool caseFelt = false;
    public bool caseOpened = true;
    private bool tabancasiVar = false;
    private bool pompalisiVar = false;
    public int score = 0;
    public bool dead = false;
    private float dyingTime = 3f;
    private float t_dyingTime = 0;
    public GameObject panel1;
    public GameObject panel2;
    public Text scoreText;
    public Text gameOverScoreText;

    private void Awake()
    {
        instance = this;
    }

	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        DyingEvents();
        GameStartingEvents();
        if (wave == 0)
            return;
        if (spawnContinueing)
            waveContinueing = true;
        else
            waveContinueing = zombieCount != 0;
        if (!waveContinueing && !caseFelt)
        {
            SpawnCase(Random.Range(0, Random.Range(0, caseSpawnPoints.Length)));
            caseFelt = true;
            caseOpened = false;
        }
        if (!waveContinueing && caseOpened)
        {
            t_timeBetweenTwoWaves += Time.deltaTime;
        }
        if(!waveContinueing && caseOpened && t_timeBetweenTwoWaves > timeBetweenTwoWaves)
        {
            StartCoroutine(SpawnWave());
            t_timeBetweenTwoWaves = 0;
        }
        if(waveStarted)
        {
            t_waveInfoTime += Time.deltaTime;
            if (t_waveInfoTime > waveInfoTime)
            {
                t_waveInfoTime = 0;
                waveStarted = false;
            } 
        }
        scoreText.text = "SCORE: " + score.ToString();
        gameOverScoreText.text = scoreText.text;
        waveInfoAnimator.SetBool("WaveStarted", waveStarted);
    }

    public IEnumerator SpawnWave()
    {
        wave++;
        waveInfo.text = "WAVE " + wave.ToString(); 
        spawnContinueing = true;
        waveStarted = true;
        caseFelt = false;
        
        int top = 5 + wave / 2;
        float z1 = Mathf.Clamp(1.05f - 0.05f * wave, 0f, 1f);
        float z3 = Mathf.Clamp(0.01f * (wave - 1), 0f, 1f);
        float z2 = Mathf.Clamp(1 - z1 - z3, 0f, 1f);
        for (int i = 0; i < top; i++)
        {
            float rz = Random.Range(0f, 1f);
            if (rz < z1)
            {
                SpawnZombie(0, Random.Range(0, spawnPoints.Length));
            }
            else if (rz < z1 + z2)
            {
                SpawnZombie(1, Random.Range(0, spawnPoints.Length));
            }
            else
            {
                SpawnZombie(2, Random.Range(0, spawnPoints.Length));
            }
            yield return new WaitForSeconds(waitTime);
        }
        spawnContinueing = false;
    }

    private void SpawnZombie(int x, int y)
    {
        GameObject zombie = Instantiate(zombiePrefabs[x], spawnPoints[y].position, spawnPoints[y].rotation);
        zombieCount++;
        if(y == 1)
        {
            zombie.transform.localScale = new Vector2(-zombie.transform.localScale.x, zombie.transform.localScale.y);
            Zombie comp = zombie.GetComponent<Zombie>();
            comp.speed = -comp.speed;
        }
    }

    private void SpawnCase(int x)
    {
        Instantiate(casePrefab, caseSpawnPoints[x].position, caseSpawnPoints[x].rotation);
        caseFelt = true;
        caseOpened = false;
    }

    private void GameStartingEvents()
    {
        if (t_startingTime < startingTime && wave == 0)
            t_startingTime += Time.deltaTime;
        if (t_startingTime > startingTime && wave == 0)
            StartCoroutine(SpawnWave());
    }

    private void CaseEvents()
    {
        if(!waveContinueing && !spawnContinueing && !caseFelt)
        {
            SpawnCase(Random.Range(0, caseSpawnPoints.Length));
        }
    }

    public void Restart()
    {
        Application.Quit();
    }

    public void DyingEvents()
    {
        if(dead)
        {
            t_dyingTime += Time.deltaTime;
            if(t_dyingTime > dyingTime)
            {
                panel1.SetActive(false);
                panel2.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }
}
