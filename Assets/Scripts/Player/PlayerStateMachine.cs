using HacknSlash.Core;

namespace HacknSlash.Player
{
    public enum PlayerState
    {
        Grounded,
        Dashing,
        Jumping,
        Falling,
        Attacking,
        AirAttacking,
        Stunned
    }

    public class PlayerStateMachine : StateMachine<PlayerState>
    {
    }
}
