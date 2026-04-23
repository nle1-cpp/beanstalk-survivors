using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayerMovementPlayModeTests
{
    [UnityTest]
    public IEnumerator SampleSceneLoadsPlayerAndCamera()
    {
        yield return SceneManager.LoadSceneAsync("SampleScene");
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
}
