using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CharacterSystems {
    using GameManagement;

    public class NPCController : MonoBehaviour
    {
        public NPC npc;

        private GameManager gm;
        private Animator anim;
        private SpriteRenderer spriteRen;
        private MoveController move;
        private Grid grid;
        private Tilemap map;

        [Header("Pathing")]
        //Patroling
        [SerializeField]
        private bool walkPointSet;
        [SerializeField]
        private Vector3Int walkPoint;


        // Searching for a seat
        public float sittingTime = 10;
        [SerializeField]
        private float sittingTimer;
        [SerializeField]
        private bool foundSeat = false;
        [SerializeField]
        private bool isSitting = false;
        private SeatEntity seat;

        private bool isLeaving = false;

        [Header("Idling")]
        //Idling
        public float minIdleTime, maxIdleTime;
        private float idleTimer;
        private bool idling;



        #region Unity Functions

        private void Start() {
            gm = GameManager.singleton;
            grid = GameManager.grid;
            map = GameManager.map;
            move = GetComponent<MoveController>();
            spriteRen = GetComponentInChildren<SpriteRenderer>();

            InitNPC();
        }

        private void Update() {
            /*
            Psudocode:
            Check if there is any seats available
                If there isn't any seats, wander around the restaurant idling
                If there is, mark that seat as taken and path to it
                    Sit in the seat and start a timer
                    once the timer <= 0 get up from the seat and leave
            */

            if (!isLeaving) {
                if (!foundSeat) {
                    // Search for a seat, returns false if there are no available seats.
                    if (!SearchForSeat()) {
                        // If there are no seats the NPC will patrol/Idle around the restaurant
                        if (!idling)        // If the NPC isnt idling then patrol
                            Patroling();
                        else {              // Otherwise idle (which just counts down the idle timer)
                            Idle();
                        }
                    }
                } else {
                    if (!isSitting) {
                        // the NPC should now be pathing towards the available seats access point.
                        // Once at the seat, sit in it
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
            if (gm.seatList.Count > 0) {
                foreach (SeatEntity _seat in gm.seatList) {
                    if (!_seat.occupied) {
                        // Set necessary bools
                        foundSeat = true;
                        _seat.SetOccupied(true);
                        if (idling) idling = false;

                        // Path to the seats access pos
                        move.AutoPath(_seat.accessPoint.position);

                        seat = _seat;

                        return true;
                    }
                }
            }
            return false;
        }
        private void SitInSeat() {
            isSitting = true;
            sittingTimer = sittingTime;

            // Makes sure the NPC sitting in the chair appears in front of the chair.
            //spriteRen.sortingOrder = 1;   // Currently doing this in the [SeatEntity] script, but its making the seat behind everything else.

            // Moves the NPC onto the seat
            transform.position = grid.GetCellCenterWorld(grid.WorldToCell(seat.transform.position));
        }
        private void LeaveSeat() {
            foundSeat = false;
            isSitting = false;
            seat.SetOccupied(false);

            transform.position = grid.GetCellCenterWorld(grid.WorldToCell(seat.accessPoint.position));
            seat = null;

            // Sets the NPCs sorting order back to normal.
            //spriteRen.sortingOrder = 0;
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
