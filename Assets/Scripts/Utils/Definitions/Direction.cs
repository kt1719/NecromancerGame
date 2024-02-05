using UnityEngine;

namespace GameDefinitions {
    public class Direction
    {
        public static implicit operator Vector2Int(Direction direction)
        {
            return direction.direction;
        }

        public static implicit operator Vector2(Direction direction)
        {
            return direction.direction;
        }

        /// <summary>
        /// Overload operators
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>

        public static Direction operator +(Direction a, Vector2Int b)
        {
            return new Direction(a.direction.x + b.x, a.direction.y + b.y);
        }

        public static Direction operator -(Direction a, Vector2Int b)
        {
            return new Direction(a.direction.x - b.x, a.direction.y - b.y);
        }
        
        public static Direction operator *(Direction a, int b)
        {
            return new Direction(a.direction.x * b, a.direction.y * b);
        }

        public static Direction operator /(Direction a, int b)
        {
            return new Direction(a.direction.x / b, a.direction.y / b);
        }

        public static Direction operator *(Direction a, float b)
        {
            return new Direction((int)(a.direction.x * b), (int)(a.direction.y * b));
        }

        public static readonly Direction None = new Direction(0, 0);
        public static readonly Direction Up = new Direction(0, 1);
        public static readonly Direction Down = new Direction(0, -1);
        public static readonly Direction Left = new Direction(-1, 0);
        public static readonly Direction Right = new Direction(1, 0);
        public static readonly Direction UpLeft = new Direction(-1, 1);
        public static readonly Direction UpRight = new Direction(1, 1);
        public static readonly Direction DownLeft = new Direction(-1, -1);
        public static readonly Direction DownRight = new Direction(1, -1);

        private Vector2Int direction = Vector2Int.zero;

        public Direction(int x, int y)
        {
            direction = new Vector2Int(x, y);
        }

        public Direction(float x, float y)
        {
            direction = new Vector2Int((int)x, (int)y);
        }

        public Direction(Vector2Int direction)
        {
            this.direction = direction;
        }
    }
}