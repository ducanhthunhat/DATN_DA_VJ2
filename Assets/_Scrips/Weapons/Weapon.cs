using System;
using UnityEngine;
using DucAnh.Utilities;

namespace DucAnh.Weapons
{
    
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private int numberOfAtttacks;
        [SerializeField] private float attackCounterResetCooldown;
        public int CurrentAttackCounter
        {
            get => currentAttackCounter;
            private set
            {
                if(value >= numberOfAtttacks)
                {
                    currentAttackCounter = 0;
                }
                else
                {
                    currentAttackCounter = value;
                }
            }
        }

        public event Action OnExit;
        private Animator anim;
        private GameObject baseGameObject;
        private AnimationEventHandler eventHandler;
        private int currentAttackCounter;
        private Timer attackCounterResetTime;

        public void Enter()
        {
            print($"{transform.name} enter");

            attackCounterResetTime.StopTimer();

            anim.SetBool("active", true);
            anim.SetInteger("counter", currentAttackCounter);
        }



        private void Exit()
        {
            anim.SetBool("active", false);

            CurrentAttackCounter++;
            attackCounterResetTime.StartTimer();

            OnExit?.Invoke();
        }

        private void Awake()
        {
            baseGameObject = transform.Find("Base").gameObject;
            anim = baseGameObject.GetComponent<Animator>();

            eventHandler = baseGameObject.GetComponent<AnimationEventHandler>();

            attackCounterResetTime = new Timer(attackCounterResetCooldown);
        }

        private void Update()
        {
            attackCounterResetTime.Tick();
        }

        private void ResetAttackCounter() => CurrentAttackCounter = 0;

        private void OnEnable()
        {
            eventHandler.OnFinish += Exit;
            attackCounterResetTime.OnTimerDone += ResetAttackCounter;
        }

        private void OnDisable()
        {
            eventHandler.OnFinish -= Exit;
            attackCounterResetTime.OnTimerDone -= ResetAttackCounter;
        }
    }

}
