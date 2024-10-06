using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.IK;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int level = 0;

    private const int playerMaxHp = 1000;
    private int playerHP = 1000;

    private List<int> enemiesToSpawn = new List<int> {0, 10, 0};

    private const int humanityOriginalCount = 8180221;
    private int humansLeft = 8180221; //thousand
    private const float E = 2.71828f;
    private const int maxSeroVictimCount = 1000000;
    private bool level1 = false;
    private float lvl1timer = 15f;

    private bool level2 = false;
    private float lvl2timer = 10f;

    private bool level3 = false;
    private float lvl3timer = 5f;

    private List<float> spawnCooldowns = new List<float> { 1.5f, 5f, 1f }; // airplanes, jets, rockets

    private int enemyTypeToSpawn = 0;
    private float enemyTypeChangeTimer = 0;
    private float activeEnemySpawnCooldown = 1.5f;
    private float enemySpawnTimer = 0;
   
    
    

    [SerializeField] private GameObject player;
    [HideInInspector] public RectTransform playerRT { get; private set; }
    private RectTransform rt;
    private UserInterface ui;
    [SerializeField] private List<SpawnLine> spawnLines; //up down left right
    [SerializeField] private List<GameplayLine> gameplayLinesT0;
    [SerializeField] private List<GameplayLine> gameplayLinesT1;

    [Header("Prefabs")]
    [SerializeField] private GameObject airplane;
    [SerializeField] private GameObject jet;
    [SerializeField] private GameObject rocket; 

    private void Awake()
    {
        if (instance == null)
        instance = this;
    }

    private void Start()
    {
        playerRT = player.GetComponent<RectTransform>();
        Cursor.visible = false;
        rt = GetComponent<RectTransform>();
        ui = UserInterface.instance;
        ui.UpdateVictims(humanityOriginalCount, humanityOriginalCount, 1f);
        ui.ChangeHP(playerMaxHp, playerMaxHp, true);
    }

    private void Update()
    {
        enemyTypeChangeTimer += Time.deltaTime;
        enemySpawnTimer += Time.deltaTime;

        if (level1 == false)
        {
            if (enemyTypeChangeTimer > lvl1timer) //type change
            {
                enemyTypeToSpawn = GetEnemyToSpawn();
                activeEnemySpawnCooldown = spawnCooldowns[enemyTypeToSpawn];
                enemyTypeChangeTimer = 0;
            }
            if (enemySpawnTimer > activeEnemySpawnCooldown) //enemy spawn
            {
                SpawnEnemy(enemyTypeToSpawn);
                enemySpawnTimer = 0;
            }
            if (GetAllEnemies() <= 0 || GetAliveHumansPercentage() <= .9f) //level end
            {
                level1 = true;
                enemiesToSpawn = new List<int> { 5, 20, 0 };
                enemyTypeChangeTimer = 0;
            }
        }
        else
        if (level2 == false)
        {
            //fighting rockets jets etc
            if (enemyTypeChangeTimer > lvl2timer) //type change
            {
                enemyTypeToSpawn = GetEnemyToSpawn();
                activeEnemySpawnCooldown = spawnCooldowns[enemyTypeToSpawn];
                enemyTypeChangeTimer = 0;
            }
            if (enemySpawnTimer > activeEnemySpawnCooldown) //enemy spawn
            {
                SpawnEnemy(enemyTypeToSpawn);
                enemySpawnTimer = 0;
            }
            if (GetAllEnemies() <= 10 || GetAliveHumansPercentage() <= .5f)
            {
                //level2 = true;
                enemiesToSpawn = new List<int> { Random.Range(0, 30), Random.Range(0, 8), };
                enemySpawnTimer = -15;
                enemyTypeChangeTimer = 0;
            }
        }
        else
        if (level3 == false)
        {
            //fighting laser destoyer
            if (enemyTypeChangeTimer > lvl3timer)
            {
                SpawnEnemy(GetEnemyToSpawn());
                enemyTypeChangeTimer = 0;
            }

            if (GetAliveHumansPercentage()<= 0f)
            {
                //gameEnd
            }
        }
    }


    float GetAliveHumansPercentage()
    {
        return (float)humansLeft / humanityOriginalCount;
    }

    public void TakeDamage(int dmg)
    {
        if (dmg > 0)
        {
            AudioManager.instance.PlayDamage();
            if ((playerHP -= dmg) <= 0)
            {
                Death();
            }
            else
            {

                ui.ChangeHP(playerHP, playerMaxHp, false);
            }
        }
        else
        {
            playerHP -= dmg;
            ui.ChangeHP(playerHP, playerMaxHp, true);
            AudioManager.instance.PlayHeal();
        }
    }

    public Vector2 DirectionToPlayer(Vector2 point)
    {
        return playerRT.anchoredPosition - point;
    }

    public void CollidedWith(GameObject enemy)
    {
        enemy.GetComponent<Enemy>().GetHit();
    }

    private void SpawnEnemy(int type)
    {
        enemiesToSpawn[type]--;
        SpawnLine suitableLine = GetSuitableSpawnLine(airplane.GetComponent<Enemy>().spawnDirections);
        float angle = Mathf.Atan2(suitableLine.GetFacingDirection().y, suitableLine.GetFacingDirection().x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        Vector3 randomPositon = Vector3.zero;
        switch (type)
        {
            case 0:
                RectTransform _airplane = Instantiate(this.airplane, rt).GetComponent<RectTransform>();
                _airplane.localPosition = Vector3.zero;
                randomPositon = rt.InverseTransformPoint(suitableLine.GetRandomWorldPos());
                _airplane.localPosition = randomPositon;


                //fixed weird offset on z axis
                Vector3 temp = _airplane.anchoredPosition;
                temp.z = 0; // =_)))))))
                _airplane.anchoredPosition3D = temp;


                _airplane.GetComponent<Airplane>().SetMovingDirection(playerRT.position - _airplane.position);
                //SetMovingDirection(suitableLine.GetFacingDirection());

                break;
            case 1:
                RectTransform _jet = Instantiate(jet, rt).GetComponent<RectTransform>();
                _jet.localPosition = Vector3.zero;
                randomPositon = rt.InverseTransformPoint(suitableLine.GetRandomWorldPos());
                _jet.localPosition = randomPositon;


                //fixed weird offset on z axis
                temp = _jet.anchoredPosition;
                temp.z = 0; // =_)))))))
                _jet.anchoredPosition3D = temp;
                break;
            case 2:

                break;
            case 3:

                break;
        }
    }

    private SpawnLine GetSuitableSpawnLine(bool[] arg)
    {
        List<SpawnLine> suitableLines = new List<SpawnLine>();
        for(int i=0;i<4;i++)
        {
            if (arg[i])
            {
                suitableLines.Add(spawnLines[i]);
            }
        }
        return suitableLines[Random.Range(0, suitableLines.Count)];
    }

    public void AddVictims(int arg, bool isMajor)
    {
        if ((humansLeft -= arg) <= 0)
        {
            ui.ShowWinScreen(GetGrade());

            player.SetActive(false);
            this.enabled = false;
        }
        else
        {
            if (isMajor) TakeDamage((int)(-arg / 10000f));
            ui.UpdateVictims(humansLeft, humansLeft, GetAliveHumansPercentage());
        }
    }

    private string GetGrade()
    {
        if (playerHP >= 1500)
        {
            return "<palette>S";
        }
        if (playerHP >= 1000)
        {
            return "<color=yellow>A";
        }
        if (playerHP >= 500)
        {
            return "<color=green>B";
        }

        return "<color=white>C";
    }

    public GameplayLine GetGameplayLineT0()
    {
        return gameplayLinesT0[Random.Range(0, gameplayLinesT0.Count)];
    }

    public GameplayLine GetGameplayLineT1()
    {
        return gameplayLinesT1[Random.Range(0, gameplayLinesT1.Count)];
    }

    private int GetEnemyToSpawn()
    {

        int totalCount = GetAllEnemies();

        int randomValue = Random.Range(0, totalCount);

        int cumulativeCount = 0;

        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            cumulativeCount += enemiesToSpawn[i];

            if (randomValue < cumulativeCount)
            {
                return i; 
            }
        }

        return 0;
    }

    private int GetAllEnemies()
    {
        int res = 0;
        enemiesToSpawn.ForEach(enemy => { res += enemy; });

        return res;
    }

    public void CeroFired(float percentage)
    {
        StartCoroutine(CeroFireCor(percentage));
    }

    IEnumerator CeroFireCor(float percentage)
    {
        yield return new WaitForSeconds(3);
        int rand = Random.Range(-100, 100);
        if (percentage>0.66f)
        {
            AudioManager.instance.PlayHexp();
            ui.ShakeScreen(percentage);
            AddVictims((maxSeroVictimCount+rand) * (int)Mathf.Pow(percentage, 2.71828f * 1.5f), true);
        }
        else
        {
            AudioManager.instance.PlayLexp();
            AddVictims((int)(percentage * ((maxSeroVictimCount+rand) / 100f)), false);
            ui.ShakeScreen(percentage / 10f);
        }
    }

    private void Death()
    {
        player.GetComponent<PlayerController>().enabled = false;
        Rigidbody2D rd2d = player.GetComponent<Rigidbody2D>();
        rd2d.bodyType = RigidbodyType2D.Dynamic;
        rd2d.constraints = RigidbodyConstraints2D.None;
        rd2d.AddTorque(66.6f);
        player.GetComponentInChildren<IKManager2D>().enabled = false;

        ui.gameObject.SetActive(false);
        StartCoroutine(DeathCor());
    }

    private IEnumerator DeathCor()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }
}
