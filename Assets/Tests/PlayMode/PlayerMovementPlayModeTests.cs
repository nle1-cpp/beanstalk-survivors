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
        movement.groundDrag = 1f;
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
        movement.groundDrag = 1f;
        movement.playerHeight = 2f;
        movement.groundLayer = ~0;
        movement.orientation = player.transform;
        movement.maxAirJumps = 2;
        movement.airJumpRefillInterval = 0f;

        return movement;
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
