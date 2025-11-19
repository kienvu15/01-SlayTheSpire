using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoundData
{
    public string key;
    public AudioClip clip;
    [Range(0f, 1f)] public float defaultVolume = 1f;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Sound FX Object")]
    [SerializeField] private GameObject soundFXObject;
    [SerializeField] private int poolSize = 10;

    [Header("Audio Library")]
    [SerializeField] private List<SoundData> soundList = new List<SoundData>();
    private Dictionary<string, SoundData> soundDict = new Dictionary<string, SoundData>();

    private readonly Queue<AudioSource> pool = new Queue<AudioSource>();

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Convert List → Dictionary
        foreach (var s in soundList)
        {
            if (!soundDict.ContainsKey(s.key))
                soundDict.Add(s.key, s);
        }

        // Create pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(soundFXObject, transform);
            obj.SetActive(false);
            pool.Enqueue(obj.GetComponent<AudioSource>());
        }
    }

    private AudioSource GetFromPool()
    {
        if (pool.Count > 0)
        {
            var src = pool.Dequeue();
            src.gameObject.SetActive(true);
            return src;
        }
        else
        {
            var obj = Instantiate(soundFXObject, transform);
            return obj.GetComponent<AudioSource>();
        }
    }

    private void ReturnToPool(AudioSource src)
    {
        src.Stop();
        src.clip = null; 
        src.gameObject.SetActive(false);
        pool.Enqueue(src);
    }

    public void Play(string key, Transform spawnTransform = null, float? overrideVolume = null)
    {
        if (!soundDict.TryGetValue(key, out var data) || data.clip == null)
        {
            Debug.LogWarning($"Sound '{key}' not found!");
            return;
        }

        var src = GetFromPool();
        src.transform.position = spawnTransform ? spawnTransform.position : Vector3.zero;
        src.clip = data.clip;
        src.volume = overrideVolume ?? data.defaultVolume;
        src.Play();
        StartCoroutine(ReturnAfterPlay(src));
    }

    private IEnumerator ReturnAfterPlay(AudioSource src)
    {
        yield return new WaitForSeconds(src.clip.length);
        ReturnToPool(src);
    }
}
