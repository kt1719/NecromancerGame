using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using Player;
using GameDefinitions;
using UnityEngine.Assertions;

namespace Tests{
    public class PlayerTest
    {
        [UnityTest]
        public IEnumerator PlayerMovementUp()
        {
            Rigidbody2D rb;
            float speed = 10f;
            CallPlayerMove(out rb, speed, Direction.Up);
            // Assert that the velocity of the Rigidbody2D component is (0, 10)
            Assert.AreEqual(rb.velocity, new Vector2(0, speed));
            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayerMovementDown()
        {
            Rigidbody2D rb;
            float speed = 10f;
            CallPlayerMove(out rb, speed, Direction.Down);
            // Assert that the velocity of the Rigidbody2D component is (0, -10)
            Assert.AreEqual(rb.velocity, new Vector2(0, -speed));
            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayerMovementLeft()
        {
            Rigidbody2D rb;
            float speed = 10f;
            CallPlayerMove(out rb, speed, Direction.Left);
            // Assert that the velocity of the Rigidbody2D component is (-10, 0)
            Assert.AreEqual(rb.velocity, new Vector2(-speed, 0));
            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayerMovementRight()
        {
            Rigidbody2D rb;
            float speed = 10f;
            CallPlayerMove(out rb, speed, Direction.Right);
            // Assert that the velocity of the Rigidbody2D component is (10, 0)
            Assert.AreEqual(rb.velocity, new Vector2(speed, 0));
            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayerMovementUpLeft()
        {
            Rigidbody2D rb;
            float speed = 10f;
            CallPlayerMove(out rb, speed, Direction.UpLeft);
            // Assert that the velocity of the Rigidbody2D component is (-10, 10)
            Assert.AreEqual(rb.velocity, new Vector2(-speed, speed));
            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayerMovementUpRight()
        {
            Rigidbody2D rb;
            float speed = 10f;
            CallPlayerMove(out rb, speed, Direction.UpRight);
            // Assert that the velocity of the Rigidbody2D component is (10, 10)
            Assert.AreEqual(rb.velocity, new Vector2(speed, speed));
            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayerMovementDownLeft()
        {
            Rigidbody2D rb;
            float speed = 10f;
            CallPlayerMove(out rb, speed, Direction.DownLeft);
            // Assert that the velocity of the Rigidbody2D component is (-10, -10)
            Assert.AreEqual(rb.velocity, new Vector2(-speed, -speed));
            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayerMovementDownRight()
        {
            Rigidbody2D rb;
            float speed = 10f;
            CallPlayerMove(out rb, speed, Direction.DownRight);
            // Assert that the velocity of the Rigidbody2D component is (10, -10)
            Assert.AreEqual(rb.velocity, new Vector2(speed, -speed));
            yield return null;
        }

        private static void CallPlayerMove(out Rigidbody2D rb, float speed, Direction dir)
        {
            // Instantiate a new GameObject
            GameObject gameObject = new GameObject();
            rb = gameObject.AddComponent<Rigidbody2D>();
            // Assert that the velocity of the Rigidbody2D component is (0, 0)
            Assert.AreEqual(rb.velocity, new Vector2(0, 0));
            PlayerMovement playerMovement = gameObject.AddComponent<PlayerMovement>();
            playerMovement.InstantiateScript();
            playerMovement.Move(dir, speed);
        }
    }
}
