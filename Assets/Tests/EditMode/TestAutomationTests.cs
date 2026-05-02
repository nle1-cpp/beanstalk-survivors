using System.IO;
using NUnit.Framework;
using UnityEditor;

public class TestAutomationTests
{
    [Test]
    public void ProjectRootPointsAtRepoRoot()
    {
        string root = TestAutomation.GetProjectRoot();
        Assert.That(Directory.Exists(Path.Combine(root, "Assets")), Is.True);
        Assert.That(Directory.Exists(Path.Combine(root, "ProjectSettings")), Is.True);
    }

    [Test]
    public void EnabledSceneListIncludesSampleScene()
    {
        string[] scenes = TestAutomation.GetEnabledScenePaths();
        Assert.That(scenes, Does.Contain("Assets/Scenes/SampleScene.unity"));
    }

    [TestCase(BuildTarget.StandaloneLinux64)]
    [TestCase(BuildTarget.StandaloneWindows64)]
    [TestCase(BuildTarget.StandaloneOSX)]
    public void BuildRootMapsByTarget(BuildTarget target)
    {
        string expected = Path.Combine("Builds", "Playtest", target.ToString());
        string relative = Path.GetRelativePath(TestAutomation.GetProjectRoot(), TestAutomation.GetBuildRoot(target));
        Assert.That(relative, Is.EqualTo(expected));
    }
}
