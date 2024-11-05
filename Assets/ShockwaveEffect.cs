using UnityEngine;

public class ShockwaveEffect : MonoBehaviour
{
    private ParticleSystem particleSystem;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();

        // Optional: Configure particle system to expand in sync with ShockwaveProjectile
        var mainModule = particleSystem.main;
        mainModule.startSize = 0.5f;
        mainModule.startLifetime = 1f;
        mainModule.startColor = new ParticleSystem.MinMaxGradient(Color.white, new Color(1, 1, 1, 0));
    }
}
