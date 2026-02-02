using UnityEngine;

public class BloodWalls : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        // Get the ParticleSystem from the colliding object
        ParticleSystem particleSystem = other.GetComponent<ParticleSystem>();
        ParticleSystemRenderer renderer = other.GetComponent<ParticleSystemRenderer>();

        if (particleSystem != null)
        {
            var mainModule = particleSystem.main;

            if(particleSystem.name.Contains("Blood"))
            {
                mainModule.gravityModifier = 0f; // Turn off gravity for blood
            }
            mainModule.startSpeed = 0f;   // Stop movement

            if (renderer != null)
            {
                renderer.sortingOrder = -4;
            }
        }
    }
}
