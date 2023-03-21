using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    bool waitForNextWave = true;
    int waveIndex = 0;

    [SerializeField] List<WaveSO> allWaves = new List<WaveSO>();
    List<float> timers = new List<float>();
    List<float> spawnTimers = new List<float>();

    [SerializeField] Button startWaveButton;

    bool stopSpawning;

    [SerializeField] float timeBetweenWaves;
    private bool checkGameWinState;

    private void Start()
    {
        StartGame();

        GameManager.Instance.OnGameRestart += StartGame;

        GameManager.Instance.OnPauseGame += StopSpawning; 
    }

    private IEnumerator TimeBetweenWaves(float waitTime)
    {
        UiManager.Instance.KillAllDoTweens();
        UiManager.Instance.SetWaveProgressBar(timeBetweenWaves);
        yield return new WaitForSeconds(waitTime);
        StartNewWaveButton(); 
    }

    void StartGame()
    {
        waveIndex = 0; 
        SetNewWave(waveIndex);
        stopSpawning = false;
        waitForNextWave = true;
        startWaveButton.interactable = true;
        checkGameWinState = false;
    }

    void SetNewWave(int wave)
    {
        timers.Clear();
        spawnTimers.Clear();
        foreach (var item in allWaves[wave].subWaves)
        {
            timers.Add(0);
            spawnTimers.Add(0);
        }
        StartCoroutine(TimeBetweenWaves(timeBetweenWaves));
    }


    private void FixedUpdate()
    {
        if (stopSpawning) return;
        if (!waitForNextWave) SpawningWave(waveIndex);
    }
    private void Update()
    {
        if (checkGameWinState) CheckGameFinished(); 
    }

    void SpawningWave(int waveLevel)
    {

        bool allSubwavesFinished = false;
        for (int i = 0; i < allWaves[waveLevel].subWaves.Count; i++)
        {
            SubWaveSO wave = allWaves[waveLevel].subWaves[i];
            if (timers[i] > wave.endTime)
            {
                allSubwavesFinished = true;
                continue;
            }
            else allSubwavesFinished = false;

            timers[i] += Time.deltaTime;
            if (timers[i] >= wave.startTime)
            {
                spawnTimers[i] += Time.deltaTime;

                if (spawnTimers[i] > 1 / wave.frequency)
                {
                    spawnTimers[i] = 0;
                    SpawnNewEnemy((int)wave.enemyId);
                }
            }
        }
        if (allSubwavesFinished) WaveFinished();
    }
    void WaveFinished()
    {
        waitForNextWave = true;
        startWaveButton.interactable = true;
        waveIndex++;
        if (waveIndex < allWaves.Count)
        {
            SetNewWave(waveIndex);
        }
        else
        {
            GameFinished();
            startWaveButton.interactable = false;
            //waitForNextWave = true;
        }
    }

    void GameFinished()
    {
        checkGameWinState = true; 
    }
    void CheckGameFinished()
    {
        if (ObjectPooler.Instance.GetAllActiveEnemies().Count < 1) GameManager.Instance.DisplayGameWon(); 
    }

    public void StartNewWaveButton()
    {
        startWaveButton.interactable = false;
        waitForNextWave = false;

        UiManager.Instance.KillAllDoTweens();
        UiManager.Instance.SetWaveProgressBar(allWaves[waveIndex].subWaves[allWaves[waveIndex].subWaves.Count - 1].endTime, waveIndex + 1, allWaves.Count);

        StopAllCoroutines();
    }

    void SpawnNewEnemy(int type)
    {
        EnemyManager newEnemy = ObjectPooler.Instance.GetPooledEnemy(type);
        newEnemy.ResetEnemy();
        newEnemy.gameObject.SetActive(true);
    }

    void StopSpawning()
    {
        stopSpawning = true;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameRestart -= StartGame;

        GameManager.Instance.OnPauseGame -= StopSpawning;
    }


}
