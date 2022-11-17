using QFSW.MOP2;
using UnityEngine;

public class ParticleReliser : MonoBehaviour
{
    [SerializeField] private ParticlesPoolNames _poolName;

    public void OnParticleSystemStopped()
    {
        MasterObjectPooler.Instance.Release(gameObject, $"{_poolName}");
    }
}
