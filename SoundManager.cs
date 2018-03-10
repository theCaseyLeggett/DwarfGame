using UnityEngine;
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0,1)]
    public float volume = 0.5f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;
    [Range(0, 0.5f)]
    public float volumeVariance = 0.1f;
    [Range(0, 0.5f)]
    public float pitchVariance = 0.1f;

    private AudioSource source;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
    }

    public void Play()
    {
        if (source == null)
        {
            Debug.Log("Audiosource not found!");
            return;
        }
        source.volume = volume * (1 + Random.Range(-volumeVariance / 2f, volumeVariance / 2f));
        source.pitch = pitch * (1 + Random.Range(-pitchVariance / 2f, pitchVariance / 2f));
        source.Play();
    }
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField]
    private Sound[] soundArray;

    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundManager>();
            }
            return instance;
        }
    }

    private void Start()
    {
        for (int i = 0; i < soundArray.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + soundArray[i].name);
            _go.transform.SetParent(this.transform);
            soundArray[i].SetSource(_go.AddComponent<AudioSource>());
        }
    }

    public void PlaySound(string _name)
    {
        for (int i = 0; i < soundArray.Length; i++)
        {
            if (soundArray[i].name == _name)
            {
                soundArray[i].Play();
                return;
            }
        }

        // if no sound with name we get this far, ergo
        Debug.Log("Sound name (" + _name + ") not found in soundArray!");
    }
}
