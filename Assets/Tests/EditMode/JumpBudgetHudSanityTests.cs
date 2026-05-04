using System;
using System.IO;
using NUnit.Framework;

public class JumpBudgetHudSanityTests
{
    private static readonly string ProjectRoot = TestAutomation.GetProjectRoot();

    [Test]
    public void ArenaSceneContainsJumpBudgetHudWiring()
    {
        string scene = ReadAssetText("Assets", "Scenes", "ArenaScene.unity");

        StringAssert.Contains("m_Name: Jump Budget HUD", scene);
        StringAssert.Contains("m_AnchorMin: {x: 1, y: 0}", scene);
        StringAssert.Contains("m_AnchorMax: {x: 1, y: 0}", scene);
        StringAssert.Contains("m_Pivot: {x: 1, y: 0}", scene);
        StringAssert.Contains("m_AnchoredPosition: {x: -40, y: 40}", scene);
        StringAssert.Contains("m_SizeDelta: {x: 96, y: 96}", scene);
        StringAssert.Contains("size: 96", scene);
        StringAssert.Contains("playerMovement: {fileID: 1273729961}", scene);
    }

    [Test]
    public void ArenaSceneDoesNotKeepDuplicateRootPlayerCameraOverride()
    {
        string scene = ReadAssetText("Assets", "Scenes", "ArenaScene.unity");

        Assert.That(scene, Does.Not.Contain("addedObject: {fileID: 1273729967}"));
    }

    [Test]
    public void PlayerPrefabKeepsPlayerCameraChildAndOrientationWiring()
    {
        string prefab = ReadAssetText("Assets", "Prefabs", "Player.prefab");

        StringAssert.Contains("m_Name: PlayerCamera", prefab);
        StringAssert.Contains("playerOrientation: {fileID: 6154599537794840629}", prefab);
        StringAssert.Contains("m_Name: Orientation", prefab);
    }

    [Test]
    public void PlayerMovementConsumesAirJumpsOnlyInAirborneBranch()
    {
        string source = ReadAssetText("Assets", "Scripts", "Player", "PlayerMovement.cs");
        string method = ExtractMethodBlock(source, "private bool TryPerformJump()");

        int groundedJumpIndex = method.IndexOf("PerformJump(false)", StringComparison.Ordinal);
        int decrementIndex = method.IndexOf("availableAirJumps--", StringComparison.Ordinal);
        int airborneJumpIndex = method.IndexOf("PerformJump(true)", StringComparison.Ordinal);

        Assert.That(groundedJumpIndex, Is.GreaterThanOrEqualTo(0));
        Assert.That(decrementIndex, Is.GreaterThanOrEqualTo(0));
        Assert.That(airborneJumpIndex, Is.GreaterThanOrEqualTo(0));
        Assert.That(groundedJumpIndex, Is.LessThan(decrementIndex));
        Assert.That(decrementIndex, Is.LessThan(airborneJumpIndex));
        Assert.That(CountOccurrences(method, "availableAirJumps--"), Is.EqualTo(1));
    }

    private static string ReadAssetText(params string[] relativeSegments)
    {
        string path = ProjectRoot;

        foreach (string segment in relativeSegments)
        {
            path = Path.Combine(path, segment);
        }

        return File.ReadAllText(path);
    }

    private static string ExtractMethodBlock(string source, string signature)
    {
        int signatureIndex = source.IndexOf(signature, StringComparison.Ordinal);
        Assert.That(signatureIndex, Is.GreaterThanOrEqualTo(0), $"Missing method signature: {signature}");

        int openBraceIndex = source.IndexOf('{', signatureIndex);
        Assert.That(openBraceIndex, Is.GreaterThan(signatureIndex), $"Missing method body for: {signature}");

        int depth = 0;
        for (int i = openBraceIndex; i < source.Length; i++)
        {
            if (source[i] == '{')
            {
                depth++;
            }
            else if (source[i] == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return source.Substring(signatureIndex, i - signatureIndex + 1);
                }
            }
        }

        Assert.Fail($"Unbalanced braces while parsing method: {signature}");
        return string.Empty;
    }

    private static int CountOccurrences(string text, string token)
    {
        int count = 0;
        int index = 0;

        while ((index = text.IndexOf(token, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += token.Length;
        }

        return count;
    }
}
