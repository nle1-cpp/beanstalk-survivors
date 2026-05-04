using System;
using UnityEngine;

public enum SoundType
{
    // Player
    Player_Jump,
    Player_Hurt_Projectile,
    Player_Dash,
    Player_Death,
    Player_Land,
    Player_Footstep,
    Player_Hurt,

    // Weapons
    Weapon_Switch,
    Weapon_Pickup,
    Weapon_Melee,
    Weapon_Rocket_Fire,
    Weapon_Jar_Fire,
    Platform_Spawn,

    // Enemy
    Enemy_Spawn,
    Enemy_Attack,
    Enemy_Shoot,
    Enemy_Death,
    Stomper_Cloud,

    // Boss
    Cat_Ascend,
    Cat_Start,
    Cat_Windup,
    Cat_Charge,
    Enemy_Hurt_Melee,

    // BGM
    BGM_Clear
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;

    private static SoundManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        // Safe singleton setup
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    // =========================
    // GLOBAL SOUND (2D / UI / Player)
    // =========================
    public static void PlaySound(SoundType sound, float volume = 1f)
    {
        if (instance == null) return;
        if (!instance.TryGetSoundList(sound, out SoundList list)) return;

        if (list.Sounds == null || list.Sounds.Length == 0)
        {
            Debug.LogWarning($"No audio clips assigned for: {sound}");
            return;
        }

        AudioClip clip = list.Sounds[UnityEngine.Random.Range(0, list.Sounds.Length)];

        instance.audioSource.PlayOneShot(clip, volume);
    }

    // =========================
    // POSITIONAL SOUND (3D world)
    // =========================
    public static void PlaySoundAtPosition(SoundType sound, Vector3 position, float volume = 1f)
    {
        if (instance == null) return;
        if (!instance.TryGetSoundList(sound, out SoundList list)) return;

        if (list.Sounds == null || list.Sounds.Length == 0)
        {
            Debug.LogWarning($"No audio clips assigned for: {sound}");
            return;
        }

        AudioClip clip = list.Sounds[UnityEngine.Random.Range(0, list.Sounds.Length)];

        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    private bool TryGetSoundList(SoundType sound, out SoundList list)
    {
        int soundIndex = (int)sound;
        if (soundList == null || soundIndex < 0 || soundIndex >= soundList.Length)
        {
            Debug.LogWarning($"Sound list is missing an entry for: {sound}");
            list = default;
            return false;
        }

        list = soundList[soundIndex];
        return true;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        string[] names = Enum.GetNames(typeof(SoundType));

        if (soundList == null)
            soundList = new SoundList[names.Length];

        Array.Resize(ref soundList, names.Length);

        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif
}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds { get => sounds; }

    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}
