using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class Beam : MonoBehaviour
{
    public int energyCost;
    public GameObject beamMain;
    public GameObject beamBrust;
    public GameObject beamIncantation;
    public AudioSource audioSource;
    public AudioClip accumulatingAudio;
    public AudioClip releaseAudio;
    public AudioClip denyAudio;
    public SwingDetection swingDetection;

    private HapticImpulsePlayer hapticPlayer => swingDetection.HapticPlayer;
    private EnergyManager energyManager;
    private ParticleSystem[] _beamMainParticles;
    private ParticleSystem[] _beamBurstParticles;
    private ParticleSystem[] _beamIncantationParticles;

    private float _time_stage1 = 1f;
    private float _time_stage2 = 3f;

    private void Awake()
    {
        _beamMainParticles = beamMain.GetComponentsInChildren<ParticleSystem>();
        _beamBurstParticles = beamBrust.GetComponentsInChildren<ParticleSystem>();
        _beamIncantationParticles = beamIncantation.GetComponentsInChildren<ParticleSystem>();
        energyManager = FindFirstObjectByType<EnergyManager>();
    }

    public void ActivateBeam()
    {
        if (energyManager.Energy < energyCost)
        {
            audioSource.PlayOneShot(denyAudio, 0.7f);
            return;
        }
        energyManager.Energy -= energyCost;
        StartCoroutine(PlayBeam());
    }

    IEnumerator PlayBeam()
    {
        foreach (var ps in _beamIncantationParticles)
        {
            ps.Play();
        }
        audioSource.PlayOneShot(accumulatingAudio);
        hapticPlayer.SendHapticImpulse(0.5f, _time_stage1 / 2);
        yield return new WaitForSeconds(_time_stage1);
        foreach (var ps in _beamMainParticles)
        {
            ps.Play();
        }
        foreach (var ps in _beamBurstParticles)
        {
            ps.Play();
        }
        audioSource.PlayOneShot(releaseAudio);
        hapticPlayer.SendHapticImpulse(0.5f, _time_stage2);
    }
}
