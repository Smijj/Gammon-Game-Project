using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharacterSystems {
    using GameManagement;

    public enum Effect {
        None,
        Poison,
        Slow,
        Stun
    }

    public class CharacterStats : MonoBehaviour {
        [Header("Character Details")]
        public new string name;
        public Sprite charIcon;
        [TextArea(3, 6)]
        public string description;

        [Header("Character Stats")]
        //public HealthBar healthBar;
        public float maxHealth = 100;
        public bool infiniteHealth = false;


        [Header("Debug")]
        //[HideInInspector]
        public float currentHealth;
        //[HideInInspector]
        public float speedModifier = 1;
        
        //[HideInInspector]
        public bool isPoisoned = false;
        //[HideInInspector]
        public bool isSlowed = false;
        //[HideInInspector]
        public bool isStunned = false;

        /*[SerializeField]
        private int poisonStacks = 0;
        [SerializeField]
        private int slowStacks = 0;
        [SerializeField]
        private int stunStacks = 0;*/

        public bool isDashing = false;
        public bool isAttacking = false;
        //[HideInInspector]
        public bool isDead = false;

        private GameManagement.GameManager gm;


        #region Unity Functions

        private void Start() {
            gm = GameManagement.GameManager.singleton;

            if (infiniteHealth)
                maxHealth = 10000; 
        
            currentHealth = maxHealth;
            speedModifier = 1;

            //if (healthBar)
                //healthBar.SetMaxHeath(maxHealth);
        }

        #endregion


        #region Public Funtions

        /*public void AddHealth(float _health) {
            if (currentHealth < maxHealth) {
                if (_health + currentHealth > maxHealth) {
                    if (gameObject.tag == "Player")
                        GameManager.singleton.ChangeNumber(GameManager.healthNumPos, maxHealth - currentHealth);
                    currentHealth = maxHealth;
                } else {
                    if (gameObject.tag == "Player")
                        GameManager.singleton.ChangeNumber(GameManager.healthNumPos, _health);
                    currentHealth += _health;
                }

                if (healthBar)
                    healthBar.SetHealth(currentHealth);
            }
            if (currentHealth > maxHealth) {
                currentHealth = maxHealth;
            }
        }

        public void DealDamage(float _damage) {
            if (!infiniteHealth) {
                if (currentHealth > 0) {
                    if (currentHealth - _damage < 0) {
                        if (gameObject.tag == "Player") {
                            GameManager.singleton.ChangeNumber(GameManager.healthNumPos, -currentHealth);
                        }
                        currentHealth = 0;
                    } else {
                        if (gameObject.tag == "Player") {
                            GameManager.singleton.ChangeNumber(GameManager.healthNumPos, -_damage);
                        }
                        currentHealth -= _damage;
                    }

                    if (healthBar)
                        healthBar.SetHealth(currentHealth);
                }
                if (currentHealth <= 0) {
                    Die();
                }
            }
        }*/

        /*public IEnumerator DealDamageOverTime(float _tickDamage, float _duration) {
            float durationCounter = _duration;
            while (true) {
                yield return new WaitForSeconds(1);
                durationCounter -= 1;
                if (durationCounter < 0) break;

                //if (gameObject.tag == "Enemy") Debug.Log(name + " enemy is hit with " + _tickDamage + " damage.");
                DealDamage(_tickDamage);
            }
        }*/

        /*public IEnumerator AddStatusEffect(Effect _status, float _duration, float _tickDamage = 0, float _speedModifier = 1) {

            if (_status == Effect.Poison) {
                StartCoroutine(DealDamageOverTime(_tickDamage, _duration));
                isPoisoned = true;
                poisonStacks += 1;
            }
            if (_status == Effect.Slow) {
                speedModifier = _speedModifier;
                isSlowed = true;
                slowStacks += 1;
            }
            if (_status == Effect.Stun) {
                //Debug.Log("Stuned " + name);
                isStunned = true;
                stunStacks += 1;
            }
            

            yield return new WaitForSeconds(_duration);

            if (_status == Effect.Poison) {
                if (poisonStacks == 1) isPoisoned = false;
                poisonStacks -= 1;
            }
            if (_status == Effect.Slow) {
                if (slowStacks == 1) {
                    speedModifier = 1;
                    isSlowed = false;
                }
                slowStacks -= 1;
            }
            if (_status == Effect.Stun) {
                if (stunStacks == 1) isStunned = false;
                stunStacks -= 1;
            }
        }*/

        #endregion


        #region Private Functions

        /*private void Die() {
            StopCoroutine("DealDamageOverTime");
            isDead = true;
        }*/

        #endregion

    }


    

}
