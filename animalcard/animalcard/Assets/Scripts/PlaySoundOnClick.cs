using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlaySoundOnClick : MonoBehaviour
{
    public AudioClip audioClip;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = FindFirstObjectByType<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // 카드가 새로 인식되어 활성화될 때 한 번 재생
    private void OnEnable()
    {
        if (audioClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    // 터치 처리
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform && audioClip != null)
                {
                    audioSource.PlayOneShot(audioClip);
                }
            }
        }
    }
}