﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Linq;
using UnityEngine.Networking;
using System.Linq.Expressions;
using System.IO;
using UnityEditor;
using Netherlands3D.Core;
using Netherlands3D.Interface;
using Netherlands3D.Interface.SidePanel;

namespace Netherlands3D.Traffic
{
    public class GenerateRoads : MonoBehaviour
    {
        public GameObject roadObject;

        public List<RoadObject> allLoadedRoads = new List<RoadObject>();
        public List<RoadObject> shuffledRoadsList = new List<RoadObject>();
        public static GenerateRoads Instance = null;

        public const string roadsFileName = "traffic/road_line.geojson";

        private int shuffleFrameCount;
        private string bbox;
        private Vector3WGS bottomLeftWGS;
        private Vector3WGS topRightWGS;

        public GridSelection grid;
        public GameObject stopButton;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void WaitForGridBounds()
        {
            //Make sure you only subscribe once
            grid.onGridSelected.RemoveAllListeners();
            grid.onGridSelected.AddListener(ShowTraffic);
        }

        public void StopWaitingForGridBounds()
        {
            grid.onGridSelected.RemoveListener(ShowTraffic);
        }

        public void ShowTraffic(Bounds bounds)
        {
            bottomLeftWGS = CoordConvert.UnitytoWGS84(bounds.min);
            topRightWGS = CoordConvert.UnitytoWGS84(bounds.max);
            StopWaitingForGridBounds();
            StartSimulation();
        }

        public void StartSimulation()
        {
            gameObject.GetComponent<TrafficSimulator>().StartSimulation(false);
            grid.gameObject.SetActive(false);
            stopButton.SetActive(true);
            allLoadedRoads.Clear();
            gameObject.GetComponent<TrafficSimulator>().StartSimulation(true);
            StartCoroutine(GetRoadsJson());
        }

        private void Update()
        {
            shuffleFrameCount++;
            if (shuffleFrameCount % 60 == 0)
            {
                if (allLoadedRoads.Count > 0)
                {
                    shuffledRoadsList = GenerateRoads.Instance.allLoadedRoads.OrderBy(x => Random.value).ToList();
                }
            }
        }

        /// <summary>
        /// Retrieves the road Json from Open Street Maps
        /// </summary>
        public IEnumerator GetRoadsJson()
        {
            string prefixRequest = "https://overpass-api.de/api/interpreter?data=[out:json];";
            string paramRequest = "way[highway~\"motorway|trunk|primary|secondary|tertiary|motorway_link|trunk_link|primary_link|secondary_link|tertiary_link|unclassified|residential|living_street|track|road\"]";
            bbox = "(" + bottomLeftWGS.lat + "," + bottomLeftWGS.lon + "," + topRightWGS.lat + "," + topRightWGS.lon + ");";
            string suffixRequest = "out geom;";
            string fullRequest = prefixRequest + paramRequest + bbox + suffixRequest;

            // send http request
            var request = UnityWebRequest.Get(fullRequest);
            {
                yield return request.SendWebRequest();

                if (request.isDone && request.result != UnityWebRequest.Result.ProtocolError)
                {
                    // catches the data
                    StartRoadGeneration(request.downloadHandler.text);
                }
            }
        }

        /// <summary>
        /// Retrieves the Json data to start the road generation
        /// </summary>
        /// <param name="jsonData"></param>
        public void StartRoadGeneration(string jsonData)
        {
            string jsonString = jsonData;
            JSONNode N = JSON.Parse(jsonString);
            
            // adds coordinates of the street with the index
            for (int i = 0; i < N["elements"].Count; i++)
            {
                //AddCoordinates(i, N);
                GenerateRoad(N["elements"][i]);
            }
        }

        /// <summary>
        /// Generate the road based on ID
        /// </summary>
        /// <param name="road"></param>
        public void GenerateRoad(JSONNode road)
        {
            GameObject temp = Instantiate(roadObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
            temp.transform.SetParent(this.transform);
            RoadObject tempRoadObject = temp.GetComponent<RoadObject>();
            tempRoadObject.CreateRoad(road);
            allLoadedRoads.Add(tempRoadObject);
        }
    }
}