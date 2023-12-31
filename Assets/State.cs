using UnityEngine;
using UnityEngine.AI;

public class State : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] GameObject target;　//Player；
    [SerializeField] GameObject[] anyObject;//見回るポイント
    enum EnemyState
    {
        WAIT,
        MOVE,
        CHISE,
        ATTACK
    }

    //waitの状態を管理
    float waitDesire = 0;　//待ち、０〜１；
    public float waitTime;　//待機時間；
    float searchDesire = 0;
    public float searchTime;//探索時間

    //moveの状態を管理
    float moveDesire = 0;　//移動、０〜１；
    public float moveTime;　//移動時間；
    bool moveing;

    float attackDesire = 0; //攻撃、０〜１；
    public float attackTime; //攻撃の間隔；

    public float enemyHP;//敵のHP
    float enemyHPhalf;

    //初期状態；
    EnemyState CurrentState = EnemyState.WAIT;
    bool StateEnter = true;

    void ChangeState(EnemyState newState)
    {
        //状態を切り替える
        CurrentState = newState;
        StateEnter = true;
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyHPhalf = enemyHP / 2;
    }

    private void Update()
    {
        var forward = transform.TransformDirection(Vector3.forward);
        var targetDirection = target.transform.position - transform.position;
        var angle = Vector3.Angle(forward, targetDirection);
        var distance = Vector3.Distance(target.transform.position, transform.position);

        if (enemyHP >= enemyHPhalf)
        {
            switch (CurrentState)
            {
                case EnemyState.WAIT: //wait時の処理；
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = false;
                        StateEnter = false;
                        Debug.Log("待機状態");
                        moveDesire = 0;
                    }
                    //待機モーションをここに追加

                    moveDesire += Time.deltaTime / waitTime;
                    searchDesire += Time.deltaTime / searchTime;

                    if (searchDesire >= 1)
                    {
                        agent.isStopped = true;
                        Debug.Log("探索中");

                        //探索モーションをここに追加
                        searchDesire = 0;
                    }

                    if (moveDesire >= 1)
                    {

                        ChangeState(EnemyState.MOVE);
                        if (moveing)
                        {
                            moveing = false;
                        }
                        else if (!moveing)
                        {
                            moveing = true;
                        }
                        return;
                    }
                    if (distance <= 8 && angle <= 60)
                    {
                        ChangeState(EnemyState.CHISE);
                        return;
                    }
                    break;
                #endregion

                case EnemyState.MOVE: //move時の処理；
                    #region
                    if (StateEnter)
                    {
                        StateEnter = false;
                        Debug.Log("移動状態");
                        waitDesire = 0;
                        agent.isStopped = false;
                    }

                    //移動モーションをここに追加

                    waitDesire += Time.deltaTime / moveTime;

                    if (moveing)
                    {
                        agent.destination = anyObject[0].transform.position;
                    }
                    if (!moveing)
                    {
                        agent.destination = anyObject[1].transform.position;
                    }

                    if (waitDesire >= 1)
                    {
                        ChangeState(EnemyState.WAIT);
                        return;
                    }
                    if (distance <= 5 && angle <= 45)
                    {
                        ChangeState(EnemyState.CHISE);
                        return;
                    }
                    break;
                #endregion

                case EnemyState.CHISE: //chise時の処理；
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = false;
                        StateEnter = false;
                        Debug.Log("追跡状態");
                    }
                    agent.destination = target.transform.position;

                    //追跡モーションをここに追加

                    if (!(distance <= 8))
                    {
                        ChangeState(EnemyState.WAIT);
                        return;
                    }
                    if (distance <= 2.5)
                    {
                        ChangeState(EnemyState.ATTACK);
                        return;
                    }
                    break;
                #endregion

                case EnemyState.ATTACK: //attack時の処理；
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = true;
                        StateEnter = false;
                        Debug.Log("攻撃状態");
                        //攻撃前モーションをここに追加
                    }

                    attackDesire += Time.deltaTime / attackTime;

                    var dir = (target.transform.position - transform.position).normalized;
                    dir.y = 0;
                    Quaternion setRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, setRotation, agent.angularSpeed * 0.1f * Time.deltaTime);


                    if (attackDesire >= 1)
                    {
                        Debug.Log("攻撃");

                        //攻撃モーションをここに追加

                        attackDesire = 0;

                    }

                    if (distance >= 3)
                    {
                        ChangeState(EnemyState.CHISE);
                        return;
                    }
                    break;
                    #endregion

            }
        }//HP50%以上の状態

        if (enemyHP < enemyHPhalf)
        {
            switch (CurrentState)
            {
                case EnemyState.WAIT: //wait時の処理；
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = false;
                        StateEnter = false;
                        Debug.Log("待機状態");
                        moveDesire = 0;
                    }

                    //待機モーションをここに追加

                    moveDesire += Time.deltaTime / waitTime;
                    searchDesire += Time.deltaTime / searchTime;

                    if (searchDesire >= 1)
                    {
                        agent.isStopped = true;
                        Debug.Log("探索中");

                        //探索モーションをここに追加

                        searchDesire = 0;
                    }

                    if (moveDesire >= 1)
                    {

                        ChangeState(EnemyState.MOVE);
                        if (moveing)
                        {
                            moveing = false;
                        }
                        else if (!moveing)
                        {
                            moveing = true;
                        }
                        return;
                    }
                    if (distance <= 8 && angle <= 60)
                    {
                        ChangeState(EnemyState.CHISE);
                        return;
                    }
                    break;
                #endregion

                case EnemyState.MOVE: //move時の処理；
                    #region
                    if (StateEnter)
                    {
                        StateEnter = false;
                        Debug.Log("移動状態");
                        waitDesire = 0;
                        agent.isStopped = false;
                    }

                    //移動モーションをここに追加

                    waitDesire += Time.deltaTime / moveTime;

                    if (moveing)
                    {
                        agent.destination = anyObject[0].transform.position;
                    }
                    if (!moveing)
                    {
                        agent.destination = anyObject[1].transform.position;
                    }

                    if (waitDesire >= 1)
                    {
                        ChangeState(EnemyState.WAIT);
                        return;
                    }
                    if (distance <= 5 && angle <= 45)
                    {
                        ChangeState(EnemyState.CHISE);
                        return;
                    }
                    break;
                #endregion

                case EnemyState.CHISE: //chise時の処理；
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = false;
                        StateEnter = false;
                        Debug.Log("追跡状態");
                    }
                    agent.destination = target.transform.position;

                    //追跡モーションをここに追加

                    if (!(distance <= 8))
                    {
                        ChangeState(EnemyState.WAIT);
                        return;
                    }
                    if (distance <= 2.5)
                    {
                        ChangeState(EnemyState.ATTACK);
                        return;
                    }
                    break;
                #endregion

                case EnemyState.ATTACK: //attack時の処理；
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = true;
                        StateEnter = false;
                        Debug.Log("攻撃状態");
                        //攻撃前モーションをここに追加
                    }

                    attackDesire += Time.deltaTime / attackTime;

                    var dir = (target.transform.position - transform.position).normalized;
                    dir.y = 0;
                    Quaternion setRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, setRotation, agent.angularSpeed * 0.1f * Time.deltaTime);


                    if (attackDesire >= 1)
                    {
                        Debug.Log("攻撃");

                        //攻撃モーションをここに追加

                        attackDesire = 0;

                    }

                    if (distance >= 3)
                    {
                        ChangeState(EnemyState.CHISE);
                        return;
                    }
                    break;
                    #endregion

            }
        }//HP50%以下の状態

        if (enemyHP <= 0)
        {
            //敵が倒されるモーションを追加
            Debug.Log("行動終了");
        }//HP0%
    }
}
