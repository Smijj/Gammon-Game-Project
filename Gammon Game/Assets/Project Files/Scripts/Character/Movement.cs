using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character {
    public class Movement : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 7f;
        public int moveDistance = 1;
        public LayerMask WhatStopsMovement;

        public bool isMoving = false;
        
        private Vector3 targetPoint;
        [HideInInspector]
        public Vector3 lastPoint;

        [Header("Grid Stuff")]
        public Vector3 mousePos;
        public Vector3Int currentCellPos;
        public Vector3 currentCellCenter;

        public Grid grid;
        private Camera cam;


        private void OnEnable() {
            cam = Camera.main;

            targetPoint = transform.position;
            lastPoint = transform.position;
        }

        private void Update() {

            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            currentCellPos = grid.WorldToCell(mousePos);
            currentCellCenter = grid.GetCellCenterWorld(currentCellPos);

        }

        public void IncrementXPosition(float _xinput) {
            if (!Physics2D.OverlapCircle(targetPoint + new Vector3(_xinput * moveDistance, 0, 0), 0.2f, WhatStopsMovement)) {
                lastPoint = transform.position;
                targetPoint += new Vector3(_xinput * moveDistance, 0, 0);
                isMoving = true;
            }
        }
        public void IncrementYPosition(float _yinput) {
            if (!Physics2D.OverlapCircle(targetPoint + new Vector3(0, _yinput * moveDistance, 0), 0.2f, WhatStopsMovement)) {
                lastPoint = transform.position;
                targetPoint += new Vector3(0, _yinput * moveDistance, 0);
                isMoving = true;
            }
        }

        public void SetTargetPosition(Vector3 _targetPoint) {
            targetPoint = _targetPoint;
        } 

        public void Move() {
            if (transform.position != targetPoint) {
                // Moves the gameobject to the target position.
                transform.position = Vector3.MoveTowards(transform.position, targetPoint, moveSpeed * Time.deltaTime);
            } else {
                isMoving = false;
            }
        }
    }
}
