using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("各界面音频")]
    public AudioClip mainBGM;
    public AudioClip shopBGM;
    public AudioClip gameOverBGM;
    
    [Header("叠加音频")]
    [Range(0f, 1f)] public float overlayVolume = 1f;
    [SerializeField] private AudioSource overlaySource;
    public AudioClip man;

    [Header("过渡设置")]
    [Range(0.1f, 5f)]
    public float fadeDuration = 1.2f;
    [Range(0, 1f)] 
    public float maxVolume = 1f;

    [SerializeField]private AudioSource sourceA;
    [SerializeField]private AudioSource sourceB;
    private AudioSource _current;
    private AudioSource _next;

    private Coroutine _fadeCoroutine;

    private bool _isShopBGM;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitSource(sourceA);
        InitSource(sourceB);
        
        overlaySource.playOnAwake = false;
        overlaySource.loop = false;

        _current = sourceA;
        _next = sourceB;
    }

    private void InitSource(AudioSource source)
    {
        source.loop = true;
        source.playOnAwake = false;
        source.volume = 0f;
    }

    #region 对外接口

    public void PlayerMan()
    {
        _isShopBGM = false;
        PlayOverlay(man);
    }

    public void PlayMainBGM()
    {
        _isShopBGM = false;
        PlayBGM(mainBGM);
    }

    public void PlayShopBGM()
    {
        _isShopBGM = true;
        PlayBGM(shopBGM);
    }

    public void PlayGameOverBGM()
    {
        _isShopBGM = false;
        PlayBGM(gameOverBGM);
    }

    #endregion
    
    public void PlayOverlay(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        overlaySource.PlayOneShot(clip, volume * overlayVolume);
    }

    private void PlayBGM(AudioClip clip)
    {
        if (_current.clip == clip)
            return;

        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _next.clip = clip;
        _fadeCoroutine = StartCoroutine(CrossFade());
    }

    private IEnumerator CrossFade()
    {
        _next.volume = 0f;
        _next.Play();

        float time = 0f;
        float startVolume = _current.volume;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            _current.volume = Mathf.Lerp(startVolume, 0f, t);
            _next.volume = Mathf.Lerp(0f, maxVolume, t);

            yield return null;
        }

        if (!_isShopBGM)
        {
            _current.Stop();
        }
        else
        {
            _current.Pause();
        }
        
        _current.volume = 0f;
        _next.volume = maxVolume;

        // 交换
        (_current, _next) = (_next, _current);
    }
}
