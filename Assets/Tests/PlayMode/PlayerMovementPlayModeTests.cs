using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayerMovementPlayModeTests
{
    [UnityTest]
    public IEnumerator SampleSceneLoadsPlayerAndCamera()
    {
        LogAssert.Expect(LogType.Exception, new Regex("MissingReferenceException: The variable enemyPrefabs of WaveData doesn't exist anymore\\."));
        yield return SceneManager.LoadSceneAsync("ArenaScene");
        yield return null;

        Assert.That(Object.FindFirstObjectByType<PlayerMovement>(), Is.Not.Null);
        Assert.That(Object.FindFirstObjectByType<PlayerCamera>(), Is.Not.Null);
    }

    [UnityTest]
    public IEnumerator ResetForPlaytestRestoresPositionAndAirJumps()
    {
        GameObject player = new GameObject("TestPlayer");
        player.AddComponent<Rigidbody>();
        player.AddComponent<CapsuleCollider>();

        PlayerMovement movement = player.AddComponent<PlayerMovement>();
        movement.moveSpeed = 6f;
        movement.jumpForce = 5f;
        movement.airMultiplier = 1f;
        movement.playerHeight = 2f;
        movement.groundLayer = ~0;
        movement.orientation = player.transform;
        movement.maxAirJumps = 2;

        yield return null;

        movement.ResetForPlaytest(new Vector3(1f, 2f, 3f));

        Assert.That((player.transform.position - new Vector3(1f, 2f, 3f)).sqrMagnitude, Is.LessThan(0.0001f));
        Assert.That(movement.Velocity.sqrMagnitude, Is.LessThan(0.0001f));
        Assert.That(movement.AvailableAirJumps, Is.EqualTo(movement.MaxAirJumps));

        Object.Destroy(player);
        yield return null;
    }

    [UnityTest]
    public IEnumerator GroundedJumpDoesNotConsumeAirJumps()
    {
        PlayerMovement movement = CreateTestPlayer();
        yield return null;

        movement.isGrounded = true;
        movement.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

        int before = movement.AvailableAirJumps;
        InvokePrivate(movement, "TryPerformJump");
        movement.enabled = false;
        yield return new WaitForFixedUpdate();

        Assert.That(movement.AvailableAirJumps, Is.EqualTo(before));
        Assert.That(movement.Velocity.y, Is.GreaterThan(0f));

        Object.Destroy(movement.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator AirborneJumpConsumesOneAirJump()
    {
        PlayerMovement movement = CreateTestPlayer();
        yield return null;

        movement.isGrounded = false;
        movement.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

        int before = movement.AvailableAirJumps;
        bool jumped = (bool)InvokePrivate(movement, "TryPerformJump");
        movement.enabled = false;
        yield return new WaitForFixedUpdate();

        Assert.That(jumped, Is.True);
        Assert.That(movement.AvailableAirJumps, Is.EqualTo(before - 1));
        Assert.That(movement.Velocity.y, Is.GreaterThan(0f));

        Object.Destroy(movement.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator AirborneJumpBlockedWhenNoAirJumpsRemain()
    {
        PlayerMovement movement = CreateTestPlayer();
        yield return null;

        movement.isGrounded = false;
        SetPrivateField(movement, "availableAirJumps", 0);
        movement.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

        bool jumped = (bool)InvokePrivate(movement, "TryPerformJump");
        movement.enabled = false;
        yield return new WaitForFixedUpdate();

        Assert.That(jumped, Is.False);
        Assert.That(movement.AvailableAirJumps, Is.EqualTo(0));
        Assert.That(movement.Velocity, Is.EqualTo(Vector3.zero));

        Object.Destroy(movement.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator GroundedMoveInputAcceleratesHorizontalVelocity()
    {
        PlayerMovement movement = CreateTestPlayer();
        yield return null;

        movement.isGrounded = true;
        movement.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        SetPrivateField(movement, "moveInput", new Vector2(0f, 1f));

        InvokePrivate(movement, "ApplyHorizontalMovement", false);

        float horizontalSpeed = new Vector3(movement.Velocity.x, 0f, movement.Velocity.z).magnitude;
        Assert.That(horizontalSpeed, Is.GreaterThan(0f));
        Assert.That(horizontalSpeed, Is.LessThan(movement.moveSpeed));
        Assert.That(movement.Velocity.y, Is.EqualTo(0f).Within(0.0001f));

        Object.Destroy(movement.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator GroundedMoveInputEventuallyReachesMoveSpeed()
    {
        PlayerMovement movement = CreateTestPlayer();
        yield return null;

        movement.isGrounded = true;
        movement.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        SetPrivateField(movement, "moveInput", new Vector2(0f, 1f));

        int stepsToMaxSpeed = Mathf.CeilToInt(movement.groundTimeToMaxSpeed / Time.fixedDeltaTime);
        for (int i = 0; i < stepsToMaxSpeed; i++)
        {
            InvokePrivate(movement, "ApplyHorizontalMovement", false);
        }

        float horizontalSpeed = new Vector3(movement.Velocity.x, 0f, movement.Velocity.z).magnitude;
        Assert.That(horizontalSpeed, Is.EqualTo(movement.moveSpeed).Within(0.0001f));
        Assert.That(movement.Velocity.y, Is.EqualTo(0f).Within(0.0001f));

        Object.Destroy(movement.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator GroundedMoveInputPreservesAndBuildsHorizontalMomentum()
    {
        PlayerMovement movement = CreateTestPlayer();
        yield return null;

        movement.isGrounded = true;
        movement.GetComponent<Rigidbody>().linearVelocity = new Vector3(0f, -5f, 3f);
        SetPrivateField(movement, "moveInput", new Vector2(0f, 1f));

        InvokePrivate(movement, "ApplyHorizontalMovement", false);

        float horizontalSpeed = new Vector3(movement.Velocity.x, 0f, movement.Velocity.z).magnitude;
        Assert.That(horizontalSpeed, Is.GreaterThan(3f));
        Assert.That(horizontalSpeed, Is.LessThanOrEqualTo(movement.moveSpeed));
        Assert.That(movement.Velocity.y, Is.EqualTo(-5f).Within(0.0001f));

        Object.Destroy(movement.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator GroundedUpdateDoesNotApplyLinearDamping()
    {
        PlayerMovement movement = CreateTestPlayer();
        movement.GetComponent<Rigidbody>().linearDamping = 0f;
        movement.transform.position = Vector3.up;
        movement.gameObject.layer = 2;
        movement.groundLayer = 1 << 0;
        GameObject ground = CreateGroundUnderPlayer();
        yield return null;

        SetPrivateField(movement, "moveInput", new Vector2(0f, 1f));
        SetPrivateField(movement, "wasGrounded", true);

        InvokePrivate(movement, "Update");

        Assert.That(movement.isGrounded, Is.True);
        Assert.That(movement.GetComponent<Rigidbody>().linearDamping, Is.EqualTo(0f).Within(0.0001f));

        Object.Destroy(ground);
        Object.Destroy(movement.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator GroundedNoInputBrakesHorizontalVelocity()
    {
        PlayerMovement movement = CreateTestPlayer();
        yield return null;

        movement.isGrounded = true;
        movement.GetComponent<Rigidbody>().linearVelocity = new Vector3(0f, 0f, 5f);
        SetPrivateField(movement, "moveInput", Vector2.zero);

        InvokePrivate(movement, "ApplyHorizontalMovement", false);

        Assert.That(new Vector3(movement.Velocity.x, 0f, movement.Velocity.z).magnitude, Is.LessThan(5f));
        Assert.That(movement.Velocity.y, Is.EqualTo(0f).Within(0.0001f));

        Object.Destroy(movement.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator AirJumpsRefillWhileGroundedNotAirborne()
    {
        PlayerMovement movement = CreateTestPlayer();
        movement.airJumpRefillInterval = 0f;
        yield return null;

        SetPrivateField(movement, "availableAirJumps", 0);
        movement.isGrounded = true;

        InvokePrivate(movement, "UpdateAirJumpRefill");
        Assert.That(movement.AvailableAirJumps, Is.EqualTo(1));

        SetPrivateField(movement, "availableAirJumps", 0);
        movement.isGrounded = false;

        InvokePrivate(movement, "UpdateAirJumpRefill");
        Assert.That(movement.AvailableAirJumps, Is.EqualTo(0));

        Object.Destroy(movement.gameObject);
        yield return null;
    }

    private static PlayerMovement CreateTestPlayer()
    {
        GameObject player = new GameObject("TestPlayer");
        player.AddComponent<Rigidbody>();
        player.AddComponent<CapsuleCollider>();

        PlayerMovement movement = player.AddComponent<PlayerMovement>();
        movement.moveSpeed = 6f;
        movement.jumpForce = 5f;
        movement.airMultiplier = 1f;
        movement.playerHeight = 2f;
        movement.groundLayer = ~0;
        movement.orientation = player.transform;
        movement.maxAirJumps = 2;
        movement.airJumpRefillInterval = 0f;

        return movement;
    }

    private static GameObject CreateGroundUnderPlayer()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "TestGround";
        ground.transform.position = new Vector3(0f, -0.1f, 0f);
        ground.transform.localScale = new Vector3(4f, 0.2f, 4f);
        ground.layer = 0;
        return ground;
    }

    private static object InvokePrivate(object target, string methodName, params object[] parameters)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"Missing method {methodName}");
        return method.Invoke(target, parameters);
    }

    private static void SetPrivateField<T>(object target, string fieldName, T value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"Missing field {fieldName}");
        field.SetValue(target, value);
    }
}
