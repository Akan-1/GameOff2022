using QFSW.MOP2;
using UnityEngine;

public static class ParticleCreator
{
    public static void Create(string _particlesPoolName, Vector3 position, bool mirror = false)
    {
        ParticleSystem _particleSystem = MasterObjectPooler.Instance.GetObjectComponent<ParticleSystem>(_particlesPoolName);
        _particleSystem.transform.position = position;
        
        if (mirror)
        {
            _particleSystem.startSpeed = -_particleSystem.startSpeed;
        }
    }
}
