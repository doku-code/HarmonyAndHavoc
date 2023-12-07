using JFM;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace JFM
{
    public class PlayerStateMachine
    {
        public PlayerState currentState;
        public Dictionary<PlayerState.STATE, PlayerState> states = new Dictionary<PlayerState.STATE, PlayerState>();

        public PlayerStateMachine(PlayerState[] states, PlayerController player)
        {
            for (int i = 0; i < states.Length; i++)
            {
                PlayerState state = states[i];
                state.Initialize(player);
                this.states.Add(state.name, state);
            }

            currentState = this.states[PlayerState.STATE.IDLE];
        }

        public void Update()
        {
            currentState = currentState.Process();
        }

        public void ChangeState(PlayerState nextState)
        {
            currentState.SetNextState(nextState);
        }

        public PlayerState GetNextState()
        {
            return currentState.GetNextState();
        }
    }
}
