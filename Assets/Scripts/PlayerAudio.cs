using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("AudioSource")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopSource;

    [Header("Common")]
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip stunClip;
    [SerializeField] private AudioClip formChangeClip;

    [Header("Normal Form")]
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip jumpTakeoffClip;
    [SerializeField] private AudioClip jumpLandClip;
    [SerializeField] private AudioClip scratchClip;

    public void PlayHurt()
    {
        sfxSource.PlayOneShot(hurtClip);
    }
    public void PlayStun()
    {
        sfxSource.PlayOneShot(stunClip);
    }
    public void PlayWalk()
    {
        sfxSource.PlayOneShot(walkClip);
    }
    public void PlayJumpTakeoff()
    {
        sfxSource.PlayOneShot(jumpTakeoffClip);
    }
    public void PlayJumpLand()
    {
        sfxSource.PlayOneShot(jumpLandClip);
    }
    public void PlayScratch()
    {
        sfxSource.PlayOneShot(scratchClip);
    }
    public void PlayFormChange()
    {
        sfxSource.PlayOneShot(formChangeClip);
    }
    public void StartWalkLoop()
    {
        // 이미 재생 중이면 재시작 안함
        if (loopSource.clip == walkClip && loopSource.isPlaying) return;

        loopSource.clip = walkClip;
        loopSource.loop = true;
        loopSource.Play();
    }
    public void StopWalkLoop()
    {
        if (loopSource.clip == walkClip)
            loopSource.Stop();
    }
}
