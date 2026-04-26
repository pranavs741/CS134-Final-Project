using UnityEngine;

public class BallEffects : MonoBehaviour
{
    [Header("Particle System")]
    [Tooltip("Particle system to play. Leave empty to auto-find on this object or its children.")]
    [SerializeField] private ParticleSystem particles;

    [Header("Colors")]
    [SerializeField] private Color resetColor = Color.green;
    [SerializeField] private Color backboardColor = Color.blue;
    [SerializeField] private Color rimColor = Color.red;

    void Awake()
    {
        if (particles == null) particles = GetComponent<ParticleSystem>();
        if (particles == null) particles = GetComponentInChildren<ParticleSystem>();
    }

    public void PlayResetEffect() => PlayWithColor(resetColor);
    public void PlayBackboardEffect() => PlayWithColor(backboardColor);
    public void PlayRimEffect() => PlayWithColor(rimColor);

    private void PlayWithColor(Color color)
    {
        if (particles == null) return;

        var main = particles.main;
        main.startColor = new ParticleSystem.MinMaxGradient(color);

        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particles.Play();
    }
}
