using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CharacterSystems {
    using GameManagement;

    public class NPCController : MonoBehaviour
    {
        public NPC npc;

        private Animator anim;
        private SpriteRenderer spriteRen;
        private MoveController move;
        private Grid grid;
        private Tilemap map;

        [Header("Patrolling")]
        //Patroling
        [SerializeField]
        private bool walkPointSet;
        [SerializeField]
        private Vector3Int walkPoint;

        //Idling
        public float minIdleTime, maxIdleTime;
        private float idleTimer;
        private bool idling;


        #region Unity Functions

        private void Start() {
            grid = GameManager.grid;
            map = GameManager.map;
            move = GetComponent<MoveController>();
            spriteRen = GetComponentInChildren<SpriteRenderer>();

            InitNPC();
        }

        private void Update() {
            // Check if there is any seats available
            // If there is mark that seat as taken and path to it
            // Sit in the seat
            // If there isn't any seats, wander around the restaurant idling

            if (!idling)        // If the NPC isnt idling then patrol
                Patroling();
            else {              // Otherwise idle (which just counts down the idle timer)
                Idle();
            }


        }


        #endregion


        #region Public Functions

        public void InitNPC() {
            spriteRen.sprite = npc.charArt;
        }


        #endregion


        #region Private Functions

        private void Idle() {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
                idling = false;
        }

        private void Patroling() {
            if (!walkPointSet) SearchWalkPoint();

            if (walkPointSet) {
                move.AutoPath(walkPoint);
                move.SetActivePath();
            }

            //Walkpoint reached
            if (move.atDestination) {
                walkPointSet = false;
                if (Random.Range(0, 2) == 0) {    // There is a 50% chance an enemy will idle when they reach their walk destination
                    idling = true;
                    idleTimer = Random.Range(minIdleTime, maxIdleTime);
                }
            }
        }

        private void SearchWalkPoint() {
            //Calculate random point in range
            float randomX = Random.Range(-map.size.x, map.size.x);
            float randomY = Random.Range(-map.size.x, map.size.x);

            walkPoint = grid.WorldToCell(new Vector3(transform.position.x + randomX, transform.position.y + randomY));
            if (map.HasTile(walkPoint) && !move.WalkableTileCheck(walkPoint)) {
                walkPointSet = true;
            }
        }

        #endregion


    }
}
