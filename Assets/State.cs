using UnityEngine;
using UnityEngine.AI;

public class State : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] GameObject target;@//PlayerG
    [SerializeField] GameObject[] anyObject;//Œ©‰ñ‚éƒ|ƒCƒ“ƒg
    enum EnemyState
    {
        WAIT,
        MOVE,
        CHISE,
        ATTACK
    }

    //wait‚Ìó‘Ô‚ğŠÇ—
    float waitDesire = 0;@//‘Ò‚¿A‚O`‚PG
    public float waitTime;@//‘Ò‹@ŠÔG
    float searchDesire = 0;
    public float searchTime;//’TõŠÔ

    //move‚Ìó‘Ô‚ğŠÇ—
    float moveDesire = 0;@//ˆÚ“®A‚O`‚PG
    public float moveTime;@//ˆÚ“®ŠÔG
    bool moveing;

    float attackDesire = 0; //UŒ‚A‚O`‚PG
    public float attackTime; //UŒ‚‚ÌŠÔŠuG

    public float enemyHP;//“G‚ÌHP
    float enemyHPhalf;

    //‰Šúó‘ÔG
    EnemyState CurrentState = EnemyState.WAIT;
    bool StateEnter = true;

    void ChangeState(EnemyState newState)
    {
        //ó‘Ô‚ğØ‚è‘Ö‚¦‚é
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
                case EnemyState.WAIT: //wait‚Ìˆ—G
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = false;
                        StateEnter = false;
                        Debug.Log("‘Ò‹@ó‘Ô");
                        moveDesire = 0;
                    }
                    //‘Ò‹@ƒ‚[ƒVƒ‡ƒ“‚ğ‚±‚±‚É’Ç‰Á

                    moveDesire += Time.deltaTime / waitTime;
                    searchDesire += Time.deltaTime / searchTime;

                    if (searchDesire >= 1)
                    {
                        agent.isStopped = true;
                        Debug.Log("’Tõ’†");

                        //’Tõƒ‚[ƒVƒ‡ƒ“‚ğ‚±‚±‚É’Ç‰Á
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

                case EnemyState.MOVE: //move‚Ìˆ—G
                    #region
                    if (StateEnter)
                    {
                        StateEnter = false;
                        Debug.Log("ˆÚ“®ó‘Ô");
                        waitDesire = 0;
                        agent.isStopped = false;
                    }

                    //ˆÚ“®ƒ‚[ƒVƒ‡ƒ“‚ğ‚±‚±‚É’Ç‰Á

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

                case EnemyState.CHISE: //chise‚Ìˆ—G
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = false;
                        StateEnter = false;
                        Debug.Log("’ÇÕó‘Ô");
                    }
                    agent.destination = target.transform.position;

                    //’ÇÕƒ‚[ƒVƒ‡ƒ“‚ğ‚±‚±‚É’Ç‰Á

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

                case EnemyState.ATTACK: //attack‚Ìˆ—G
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = true;
                        StateEnter = false;
                        Debug.Log("UŒ‚ó‘Ô");
                        //UŒ‚‘Oƒ‚[ƒVƒ‡ƒ“‚ğ‚±‚±‚É’Ç‰Á
                    }

                    attackDesire += Time.deltaTime / attackTime;

                    var dir = (target.transform.position - transform.position).normalized;
                    dir.y = 0;
                    Quaternion setRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, setRotation, agent.angularSpeed * 0.1f * Time.deltaTime);


                    if (attackDesire >= 1)
                    {
                        Debug.Log("UŒ‚");

                        //UŒ‚ƒ‚[ƒVƒ‡ƒ“‚ğ‚±‚±‚É’Ç‰Á

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
        }//HP50%ˆÈã‚Ìó‘Ô

        if (enemyHP < enemyHPhalf)
        {
            switch (CurrentState)
            {
                case EnemyState.WAIT: //wait‚Ìˆ—G
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = false;
                        StateEnter = false;
                        Debug.Log("‘Ò‹@ó‘Ô");
                        moveDesire = 0;
                    }

                    //‘Ò‹@ƒ‚[ƒVƒ‡ƒ“‚ğ‚±‚±‚É’Ç‰Á

                    moveDesire += Time.deltaTime / waitTime;
                    searchDesire += Time.deltaTime / searchTime;

                    if (searchDesire >= 1)
                    {
                        agent.isStopped = true;
                        Debug.Log("’Tõ’†");

                        //’Tõƒ‚[ƒVƒ‡ƒ“‚ğ‚±‚±‚É’Ç‰Á

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

                case EnemyState.MOVE: //move‚Ìˆ—G
                    #region
                    if (StateEnter)
                    {
                        StateEnter = false;
                        Debug.Log("ˆÚ“®ó‘Ô");
                        waitDesire = 0;
                        agent.isStopped = false;
                    }

                    //ˆÚ“®ƒ‚[ƒVƒ‡ƒ“‚ğ‚±‚±‚É’Ç‰Á

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

                case EnemyState.CHISE: //chise‚Ìˆ—G
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = false;
                        StateEnter = false;
                        Debug.Log("’ÇÕó‘Ô");
                    }
                    agent.destination = target.transform.position;

                    //’ÇÕƒ‚[ƒVƒ‡ƒ“‚ğ‚±‚±‚É’Ç‰Á

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

                case EnemyState.ATTACK: //attack‚Ìˆ—G
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = true;
                        StateEnter = false;
                        Debug.Log("UŒ‚ó‘Ô");
                        //UŒ‚‘Oƒ‚[ƒVƒ‡ƒ“‚ğ‚±‚±‚É’Ç‰Á
                    }

                    attackDesire += Time.deltaTime / attackTime;

                    var dir = (target.transform.position - transform.position).normalized;
                    dir.y = 0;
                    Quaternion setRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, setRotation, agent.angularSpeed * 0.1f * Time.deltaTime);


                    if (attackDesire >= 1)
                    {
                        Debug.Log("UŒ‚");

                        //UŒ‚ƒ‚[ƒVƒ‡ƒ“‚ğ‚±‚±‚É’Ç‰Á

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
        }//HP50%ˆÈ‰º‚Ìó‘Ô

        if (enemyHP <= 0)
        {
            //“G‚ª“|‚³‚ê‚éƒ‚[ƒVƒ‡ƒ“‚ğ’Ç‰Á
            Debug.Log("s“®I—¹");
        }//HP0%
    }
}
