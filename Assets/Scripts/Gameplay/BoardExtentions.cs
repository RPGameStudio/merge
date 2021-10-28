using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Gameplay
{
    public static class BoardExtentions
    {
        private static Random _random;
        private static Camera _camera;

        static BoardExtentions()
        {
            _random = new Random((uint)UnityEngine.Random.Range(0, 100010129));
            _camera = Camera.main;
        }

        public static Vector3 GetFreePosition<T>(this IEnumerable<T> positions, float minThereshold, float itemRadius) where T : MonoBehaviour
        {
            var result = new Vector3();

            var xBorderSize = (int)(Screen.width * 0.2f / 2);
            var yBorderSize = (int)(Screen.width * 0.3f);
            const int MAX_TRIES = 20;
            var tries = 0;

            do
            {
                result = _camera.ScreenToWorldPoint(new Vector3(_random.NextInt(xBorderSize, Screen.width - xBorderSize), _random.NextInt(xBorderSize, Screen.height - yBorderSize), _camera.farClipPlane / 2));
            } while (tries++ < MAX_TRIES && positions.Any(x => Vector3.Distance(x.transform.position, result) < minThereshold + itemRadius));

            return result;
        }

        public static Vector3 GetNearestRandomPosition(this Vector3 point, float minThreshold, float itemRadius)
        {
            Vector3 result;
            float range = minThreshold + itemRadius * 2;
            const int MAX_TRIES = 20;
            var tries = 0;

            do
            {
                result = new Vector3(_random.NextFloat(-range, range),
                                    _random.NextFloat(-range, range));
            } while (tries++ < MAX_TRIES && Vector2.Distance(result, point) < minThreshold);

            return point + result;
        }
    }
}