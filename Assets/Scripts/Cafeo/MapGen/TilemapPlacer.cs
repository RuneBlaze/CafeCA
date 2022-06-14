using UnityEngine;
using UnityEngine.Tilemaps;

namespace Cafeo.MapGen
{
    public class TilemapPlacer : MonoBehaviour
    {
        private Tilemap tilemap;

        private TileBase wall;

        public void Start()
        {
            tilemap = GetComponent<Tilemap>();
            var tiles = tilemap.GetTilesBlock(tilemap.cellBounds);
            // Debug.Log(tilemap.cellBounds);
            foreach (var tile in tiles)
            {
                if (tile == null) continue;
                wall = tile;
                break;
            }

            Debug.Log("Wall: " + wall);
            Demolish();
        }

        public void Demolish()
        {
            tilemap.ClearAllTiles();
        }

        public void Erase(Vector2Int pos)
        {
            tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), null);
        }

        public void EraseHorizontal(Vector2Int origin, int length)
        {
            for (var i = 0; i < length; i++) Erase(origin + new Vector2Int(i, 0));
        }

        public void EraseVertical(Vector2Int origin, int length)
        {
            for (var i = 0; i < length; i++) Erase(origin + new Vector2Int(0, i));
        }

        public void EraseHorizontalCentered(Vector2Int origin, int length)
        {
            EraseHorizontal(origin + new Vector2Int(-length / 2, 0), length);
        }

        public void EraseVerticalCentered(Vector2Int origin, int length)
        {
            EraseVertical(origin + new Vector2Int(0, -length / 2), length);
        }

        public void PlaceHorizontal(Vector2Int origin, int length)
        {
            for (var i = 0; i < length; i++) tilemap.SetTile(new Vector3Int(origin.x + i, origin.y, 0), wall);
        }

        public void PlaceVertical(Vector2Int origin, int length)
        {
            for (var i = 0; i < length; i++) tilemap.SetTile(new Vector3Int(origin.x, origin.y + i, 0), wall);
        }

        public void PlaceBox(RectInt rect)
        {
            // Debug.Log("Placing box: " + rect);
            PlaceHorizontal(rect.min, rect.width);
            PlaceVertical(rect.min, rect.height);
            PlaceHorizontal(rect.min + Vector2Int.up * rect.height, rect.width);
            PlaceVertical(rect.min + Vector2Int.right * rect.width, rect.height);
        }

        public void EraseOpenings(RectInt rect, RandomMap.Edges edges)
        {
            const int openingSize = 3;
            if ((edges & RandomMap.Edges.Left) != 0)
            {
                var leftPoint = rect.min + Vector2Int.up * rect.height / 2;
                EraseVerticalCentered(leftPoint, openingSize);
            }

            if ((edges & RandomMap.Edges.Right) != 0)
            {
                var rightPoint = rect.min + Vector2Int.up * rect.height / 2 + Vector2Int.right * rect.width;
                EraseVerticalCentered(rightPoint, openingSize);
            }

            if ((edges & RandomMap.Edges.Top) != 0)
            {
                var topPoint = rect.min + Vector2Int.up * rect.height + Vector2Int.right * rect.width / 2;
                EraseHorizontalCentered(topPoint, openingSize);
            }

            if ((edges & RandomMap.Edges.Bottom) != 0)
            {
                var bottomPoint = rect.min + Vector2Int.right * rect.width / 2;
                EraseHorizontalCentered(bottomPoint, openingSize);
            }
        }
    }
}