using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    public class HoleManager
    {
        private readonly MissionManager _missionManager;
        private readonly List<Hole> _holes = new();
        private readonly GameObject _holePrefab;
        private readonly float _holePadding;
        private readonly Vector3 _gameAreaPosition;
        private readonly Vector3 _gameAreaSize;

        
        /********************** CONSTRUCTION **********************/
        
        public HoleManager(MissionManager missionManager, GameObject holePrefab, float holePadding, Vector3 gameAreaPosition, Vector3 gameAreaSize)
        {
            _missionManager = missionManager;
            _holePrefab = holePrefab;
            _holePadding = holePadding;
            _gameAreaPosition = gameAreaPosition;
            _gameAreaSize = gameAreaSize;
        }

        public void SpawnHoles(int count)
        {
            _holes.Clear();
            HoleState holeState = HoleState.Good;

            for (int i = 0; i < count; i++)
            {
                Vector3 position = GetRandomHolePosition(Vector3.zero);
                var holeObj = Object.Instantiate(_holePrefab, position, Quaternion.identity);
                var hole = holeObj.GetComponent<Hole>();
                hole.Initialize(_missionManager, holeState, OnHoleHit);
                holeState = holeState == HoleState.Good ? HoleState.Bad : HoleState.Good;
                _holes.Add(hole);
            }
        }

        public void ClearHoles()
        {
            foreach (var hole in _holes)
                Object.Destroy(hole.gameObject);
            _holes.Clear();
        }
        
        
        /********************** INNER LOGIC **********************/
        private Vector3 GetRandomHolePosition(Vector3 defaultPosition)
        {
            Vector2 boundsX = new(_gameAreaPosition.x - _gameAreaSize.x / 2f, _gameAreaPosition.x + _gameAreaSize.x / 2f);
            Vector2 boundsZ = new(_gameAreaPosition.z - _gameAreaSize.z / 2f, _gameAreaPosition.z + _gameAreaSize.z / 2f);

            Vector3 position = defaultPosition;
            int attempts = 50; // if it is hard to find a new location, then spawn in the current
            do
            {
                float x = Random.Range(boundsX.x, boundsX.y);
                float z = Random.Range(boundsZ.x, boundsZ.y);
                position = new Vector3(x, _gameAreaPosition.y, z);
            } while (_holes.Any(h => Vector3.Distance(h.transform.position, position) < _holePadding) && --attempts > 0);

            return position;
        }

        private void OnHoleHit(Hole hole)
        {
            bool goodHole = hole.State == HoleState.Good;
            _missionManager.AddScore(goodHole);
            hole.Disappear(() =>
            {
                hole.transform.position = GetRandomHolePosition(hole.transform.position);
                hole.Appear();
            });
        }

        
        
    } // end of class
}