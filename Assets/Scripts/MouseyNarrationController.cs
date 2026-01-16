using UnityEngine;

public class MouseyNarrationController : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public Animator animator;

    void Update()
    {
        if (audioSource == null || animator == null)
            return;

        // Animation follows audio state
        animator.SetBool("Talking", audioSource.isPlaying);
    }

    // ▶ PLAY
    public void PlayAudio()
    {
        if (audioSource == null) return;

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    // ⏸ PAUSE
    public void PauseAudio()
    {
        if (audioSource == null) return;

        if (audioSource.isPlaying)
            audioSource.Pause();
    }

    // ⏹ STOP
    public void StopAudio()
    {
        if (audioSource == null) return;

        audioSource.Stop();
        animator.SetBool("Talking", false);
    }

    // 🔁 RESTART
    public void RestartAudio()
    {
        if (audioSource == null) return;

        audioSource.Stop();
        audioSource.Play();
    }

    // 🔊 VOLUME
    public void SetVolume(float volume)
    {
        if (audioSource == null) return;

        audioSource.volume = volume;
    }
}
