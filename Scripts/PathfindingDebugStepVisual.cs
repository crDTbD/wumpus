﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeMonkey.Utils;

public class PathfindingDebugStepVisual : MonoBehaviour {

    public static PathfindingDebugStepVisual Instance { get; private set; }

    [SerializeField] private Transform pfPathfindingDebugStepVisualNode;
    private List<Transform> visualNodeList;
    private List<GridSnapshotAction> gridSnapshotActionList;
    private bool autoShowSnapshots;
    private float autoShowSnapshotsTimer;
    private Transform[,] visualNodeArray; 

    private void Awake() {
        Instance = this;
        visualNodeList = new List<Transform>();
        gridSnapshotActionList = new List<GridSnapshotAction>();
    }

    public void Setup(Grilla<CasillaNodo> grid) {
        visualNodeArray = new Transform[grid.GetWidth(), grid.GetHeight()];

        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                Vector3 gridPosition = new Vector3(x, y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f;
                Transform visualNode = CreateVisualNode(gridPosition);
                visualNodeArray[x, y] = visualNode;
                visualNodeList.Add(visualNode);
            }
        }
        HideNodeVisuals();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ShowNextSnapshot();
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            autoShowSnapshots = true;
        }

        if (autoShowSnapshots) {
            float autoShowSnapshotsTimerMax = .05f;
            autoShowSnapshotsTimer -= Time.deltaTime;
            if (autoShowSnapshotsTimer <= 0f) {
                autoShowSnapshotsTimer += autoShowSnapshotsTimerMax;
                ShowNextSnapshot();
                if (gridSnapshotActionList.Count == 0) {
                    autoShowSnapshots = false;
                }
            }
        }
    }

    private void ShowNextSnapshot() {
        if (gridSnapshotActionList.Count > 0) {
            GridSnapshotAction gridSnapshotAction = gridSnapshotActionList[0];
            gridSnapshotActionList.RemoveAt(0);
            gridSnapshotAction.TriggerAction();
        }
    }

    public void ClearSnapshots() {
        gridSnapshotActionList.Clear();
    }

    public void TakeSnapshot(Grilla<CasillaNodo> grid, CasillaNodo current, List<CasillaNodo> openList, List<CasillaNodo> closedList) {
        GridSnapshotAction gridSnapshotAction = new GridSnapshotAction();
        gridSnapshotAction.AddAction(HideNodeVisuals);
        
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                CasillaNodo CasillaNodo = grid.GetGridObject(x, y);

              
                Vector3 gridPosition = new Vector3(CasillaNodo.x, CasillaNodo.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f;
                bool isCurrent = CasillaNodo == current;
                bool isInOpenList = openList.Contains(CasillaNodo);
                bool isInClosedList = closedList.Contains(CasillaNodo);
                int tmpX = x;
                int tmpY = y;

                gridSnapshotAction.AddAction(() => {
                    Transform visualNode = visualNodeArray[tmpX, tmpY];

                    Color backgroundColor = UtilsClass.GetColorFromString("636363");

                    if (isInClosedList) {
                        backgroundColor = new Color(1, 0, 0);
                    }
                    if (isInOpenList) {
                        backgroundColor = UtilsClass.GetColorFromString("009AFF");
                    }
                    if (isCurrent) {
                        backgroundColor = new Color(0, 1, 0);
                    }

                    visualNode.Find("sprite").GetComponent<SpriteRenderer>().color = backgroundColor;
                });
            }
        }

        gridSnapshotActionList.Add(gridSnapshotAction);
    }

    public void TakeSnapshotFinalPath(Grilla<CasillaNodo> grid, List<CasillaNodo> path) {
        GridSnapshotAction gridSnapshotAction = new GridSnapshotAction();
        gridSnapshotAction.AddAction(HideNodeVisuals);
        
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                CasillaNodo CasillaNodo = grid.GetGridObject(x, y);

           
                Vector3 gridPosition = new Vector3(CasillaNodo.x, CasillaNodo.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f;
                bool isInPath = path.Contains(CasillaNodo);
                int tmpX = x;
                int tmpY = y;

                gridSnapshotAction.AddAction(() => { 
                    Transform visualNode = visualNodeArray[tmpX, tmpY];


                    Color backgroundColor;

                    if (isInPath) {
                        backgroundColor = new Color(0, 1, 0);
                    } else {
                        backgroundColor = UtilsClass.GetColorFromString("636363");
                    }

                    visualNode.Find("sprite").GetComponent<SpriteRenderer>().color = backgroundColor;
                });
            }
        }

        gridSnapshotActionList.Add(gridSnapshotAction);
    }

    private void HideNodeVisuals() {
        foreach (Transform visualNodeTransform in visualNodeList) {
            SetupVisualNode(visualNodeTransform, 9999, 9999, 9999);
        }
    }

    private Transform CreateVisualNode(Vector3 position) {
        Transform visualNodeTransform = Instantiate(pfPathfindingDebugStepVisualNode, position, Quaternion.identity);
        return visualNodeTransform;
    }

    private void SetupVisualNode(Transform visualNodeTransform, int CostoG, int CostoH, int CostoF) {
        if (CostoF < 1000) {
            visualNodeTransform.Find("gCostText").GetComponent<TextMeshPro>().SetText(CostoG.ToString());
            visualNodeTransform.Find("hCostText").GetComponent<TextMeshPro>().SetText(CostoH.ToString());
            visualNodeTransform.Find("fCostText").GetComponent<TextMeshPro>().SetText(CostoF.ToString());
        } else {
            visualNodeTransform.Find("gCostText").GetComponent<TextMeshPro>().SetText("");
            visualNodeTransform.Find("hCostText").GetComponent<TextMeshPro>().SetText("");
            visualNodeTransform.Find("fCostText").GetComponent<TextMeshPro>().SetText("");
        }
    }

    private class GridSnapshotAction {

        private Action action;

        public GridSnapshotAction() {
            action = () => { };
        }

        public void AddAction(Action action) {
            this.action += action;
        }

        public void TriggerAction() {
            action();
        }

    }

}

