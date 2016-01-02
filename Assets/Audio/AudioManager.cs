using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {

    private static AudioManager instance;
    private AudioSource Source;
    public AudioClip collect, jump, boost, death, gui, background;

    // Use this for initialization
    void Start () {

        instance = this;
        Source = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void PlayCollect()
    {
        instance.Source.PlayOneShot(instance.collect, 1.0f);
    }

    public static void PlayJump()
    {
        instance.Source.PlayOneShot(instance.jump, 1.0f);
    }

    public static void PlayBoostJump()
    {
        instance.Source.PlayOneShot(instance.boost, 1.0f);
    }

    public static void PlayGUI()
    {
        instance.Source.PlayOneShot(instance.gui, 1.0f);
    }

    public static void PlayDeath()
    {
        instance.Source.PlayOneShot(instance.death, 1.0f);
    }

    public static void PlayBM()
    {
        instance.Source.loop = true;
        instance.Source.clip = instance.background;
        instance.Source.Play();
    }

    public static void StopBM()
    {
        instance.Source.Stop();
    }

}
