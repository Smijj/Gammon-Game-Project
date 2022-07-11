using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CharacterSystems {
    using GameManagement;
    using RestaurantSystems;

    public class NPCController : MonoBehaviour
    {
        public NPC npc;

        private GameManager gm;
        private Animator anim;
        private SpriteRenderer spriteRen;
        private MoveController move;
        private Grid grid;

        [Header("Pathing")]
        //Patroling
        [SerializeField]
        private bool walkPointSet;
        [SerializeField]
        private Vector3Int walkPoint;


        // Searching for a chair
        public float sittingTime = 10;
        [SerializeField]
        private float sittingTimer;
        [SerializeField]
        private bool foundSeat = false;
        [SerializeField]
        private bool isSitting = false;
        private ChairEntity chair;

        private bool isLeaving = false;

        [Header("Idling")]
        //Idling
        public float minIdleTime, maxIdleTime;
        public int idleRange = 10;
        private float idleTimer;
        private bool idling;



        #region Unity Functions

        private void Start() {
            gm = GameManager.singleton;
            grid = GameManager.grid;
            move = GetComponent<MoveController>();
            spriteRen = GetComponentInChildren<SpriteRenderer>();

            InitNPC();
        }

        private void Update() {
            /*
            Psudocode:
            Check if there is any chairs available
                If there isn't any chairs, wander around the restaurant idling
                If there is, mark that chair as taken and path to it
                    Sit in the chair and start a timer
                    once the timer <= 0 get up from the chair and leave
            */

            if (!isLeaving) {
                if (!foundSeat) {
                    // Search for a chair, returns false if there are no available chairs.
                    if (!SearchForSeat()) {
                        // If there are no chairs the NPC will patrol/Idle around the restaurant
                        if (!idling)        // If the NPC isnt idling then patrol
                            Patroling();
                        else {              // Otherwise idle (which just counts down the idle timer)
                            Idle();
                        }
                    }
                } else {
                    if (!isSitting) {
                        // the NPC should now be pathing towards the available chairs access point.
                        // Once at the chair, sit in it
                        if (move.atDestination) {
                            SitInSeat();
                        }
                    } else {
                        sittingTimer -= Time.deltaTime;
                        if (sittingTimer <= 0) {
                            LeaveSeat();
                            // Leave resaurant
                            LeaveResaurant();
                        }
                    }
                }
            } else {
                if (move.atDestination) {
                    // NPC is at the restaurants door, destroy its entity.
                    gm.npcManager.DestroyNPC(gameObject);
                }
            }
        }


        #endregion


        #region Public Functions

        public void InitNPC() {
            spriteRen.sprite = npc.charArt;
        }


        #endregion


        #region Private Functions


        private bool SearchForSeat() {
            if (gm.activeChairList.Count > 0) {
                foreach (ChairEntity _chair in gm.activeChairList) {
                    if (!_chair.occupied) {
                        // Set necessary bools
                        foundSeat = true;
                        _chair.occupied = true;
                        if (idling) idling = false;

                        // Path to the chairs access pos
                        move.AutoPath(_chair.accessPoint.position);

                        chair = _chair;

                        return true;
                    }
                }
            }
            return false;
        }
        private void SitInSeat() {
            isSitting = true;
            sittingTimer = sittingTime;

            // Makes sure the NPC sitting in the chair appears in front of the chair by lowering the sorting order of the chair
            chair.spriteRen.sortingOrder = -1; 

            // Moves the NPC onto the chair
            transform.position = grid.GetCellCenterWorld(grid.WorldToCell(chair.transform.position));
        }
        private void LeaveSeat() {
            foundSeat = false;
            isSitting = false;
            chair.occupied = false;
            
            // Sets the chairs sorting order back to normal.
            chair.spriteRen.sortingOrder = 0;

            transform.position = grid.GetCellCenterWorld(grid.WorldToCell(chair.accessPoint.position));
            chair = null;

        }

        private void LeaveResaurant() {
            isLeaving = true;
            move.AutoPath(gm.npcManager.spawnLocation.position);
        }


        private void Idle() {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
                idling = false;
        }

        private void Patroling() {
            if (!walkPointSet) SearchWalkPoint();

            if (walkPointSet) {
                move.AutoPath(walkPoint);
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
            float randomX = Random.Range(-idleRange, idleRange);
            float randomY = Random.Range(-idleRange, idleRange);

            walkPoint = grid.WorldToCell(new Vector3(transform.position.x + randomX, transform.position.y + randomY));
            if (move.isTileCheck(walkPoint) && !move.WalkableTileCheck(walkPoint)) {
                walkPointSet = true;
            }
        }

        #endregion


    }
}
