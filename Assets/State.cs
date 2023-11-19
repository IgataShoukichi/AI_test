using UnityEngine;
using UnityEngine.AI;

public class State : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] GameObject target;�@//Player�G
    [SerializeField] GameObject[] anyObject;//�����|�C���g
    enum EnemyState
    {
        WAIT,
        MOVE,
        CHISE,
        ATTACK
    }

    //wait�̏�Ԃ��Ǘ�
    float waitDesire = 0;�@//�҂��A�O�`�P�G
    public float waitTime;�@//�ҋ@���ԁG
    float searchDesire = 0;
    public float searchTime;//�T������

    //move�̏�Ԃ��Ǘ�
    float moveDesire = 0;�@//�ړ��A�O�`�P�G
    public float moveTime;�@//�ړ����ԁG
    bool moveing;

    float attackDesire = 0; //�U���A�O�`�P�G
    public float attackTime; //�U���̊Ԋu�G

    public float enemyHP;//�G��HP
    float enemyHPhalf;

    //������ԁG
    EnemyState CurrentState = EnemyState.WAIT;
    bool StateEnter = true;

    void ChangeState(EnemyState newState)
    {
        //��Ԃ�؂�ւ���
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
                case EnemyState.WAIT: //wait���̏����G
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = false;
                        StateEnter = false;
                        Debug.Log("�ҋ@���");
                        moveDesire = 0;
                    }
                    //�ҋ@���[�V�����������ɒǉ�

                    moveDesire += Time.deltaTime / waitTime;
                    searchDesire += Time.deltaTime / searchTime;

                    if (searchDesire >= 1)
                    {
                        agent.isStopped = true;
                        Debug.Log("�T����");

                        //�T�����[�V�����������ɒǉ�
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

                case EnemyState.MOVE: //move���̏����G
                    #region
                    if (StateEnter)
                    {
                        StateEnter = false;
                        Debug.Log("�ړ����");
                        waitDesire = 0;
                        agent.isStopped = false;
                    }

                    //�ړ����[�V�����������ɒǉ�

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

                case EnemyState.CHISE: //chise���̏����G
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = false;
                        StateEnter = false;
                        Debug.Log("�ǐՏ��");
                    }
                    agent.destination = target.transform.position;

                    //�ǐՃ��[�V�����������ɒǉ�

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

                case EnemyState.ATTACK: //attack���̏����G
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = true;
                        StateEnter = false;
                        Debug.Log("�U�����");
                        //�U���O���[�V�����������ɒǉ�
                    }

                    attackDesire += Time.deltaTime / attackTime;

                    var dir = (target.transform.position - transform.position).normalized;
                    dir.y = 0;
                    Quaternion setRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, setRotation, agent.angularSpeed * 0.1f * Time.deltaTime);


                    if (attackDesire >= 1)
                    {
                        Debug.Log("�U��");

                        //�U�����[�V�����������ɒǉ�

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
        }//HP50%�ȏ�̏��

        if (enemyHP < enemyHPhalf)
        {
            switch (CurrentState)
            {
                case EnemyState.WAIT: //wait���̏����G
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = false;
                        StateEnter = false;
                        Debug.Log("�ҋ@���");
                        moveDesire = 0;
                    }

                    //�ҋ@���[�V�����������ɒǉ�

                    moveDesire += Time.deltaTime / waitTime;
                    searchDesire += Time.deltaTime / searchTime;

                    if (searchDesire >= 1)
                    {
                        agent.isStopped = true;
                        Debug.Log("�T����");

                        //�T�����[�V�����������ɒǉ�

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

                case EnemyState.MOVE: //move���̏����G
                    #region
                    if (StateEnter)
                    {
                        StateEnter = false;
                        Debug.Log("�ړ����");
                        waitDesire = 0;
                        agent.isStopped = false;
                    }

                    //�ړ����[�V�����������ɒǉ�

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

                case EnemyState.CHISE: //chise���̏����G
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = false;
                        StateEnter = false;
                        Debug.Log("�ǐՏ��");
                    }
                    agent.destination = target.transform.position;

                    //�ǐՃ��[�V�����������ɒǉ�

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

                case EnemyState.ATTACK: //attack���̏����G
                    #region
                    if (StateEnter)
                    {
                        agent.isStopped = true;
                        StateEnter = false;
                        Debug.Log("�U�����");
                        //�U���O���[�V�����������ɒǉ�
                    }

                    attackDesire += Time.deltaTime / attackTime;

                    var dir = (target.transform.position - transform.position).normalized;
                    dir.y = 0;
                    Quaternion setRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, setRotation, agent.angularSpeed * 0.1f * Time.deltaTime);


                    if (attackDesire >= 1)
                    {
                        Debug.Log("�U��");

                        //�U�����[�V�����������ɒǉ�

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
        }//HP50%�ȉ��̏��

        if (enemyHP <= 0)
        {
            //�G���|����郂�[�V������ǉ�
            Debug.Log("�s���I��");
        }//HP0%
    }
}
