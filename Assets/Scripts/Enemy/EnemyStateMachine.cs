using HacknSlash.Core;

namespace HacknSlash.Enemy
{
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Stunned,
        Airborne,
        Dead
    }

    public class EnemyStateMachine : StateMachine<EnemyState>
    {
    }
}
