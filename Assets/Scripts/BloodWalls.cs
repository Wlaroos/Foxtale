using UnityEngine;

public class BloodWalls : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        // Get the ParticleSystem from the colliding object
        ParticleSystem particleSystem = other.GetComponent<ParticleSystem>();

        if (particleSystem != null)
        {
            // Access the ParticleSystem's main module to modify gravity
            var mainModule = particleSystem.main;
            mainModule.gravityModifier = 0f; // Turn off gravity
            mainModule.startSpeed = 0f;   // Stop movement
        }
    }
}
