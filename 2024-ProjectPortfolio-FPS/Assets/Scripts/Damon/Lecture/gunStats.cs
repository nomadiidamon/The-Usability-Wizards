using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject gunModel;
    //private BoxCollider collider;
    public int shootDamage;
    public float shootRate;
    public int shootDistance;
    public int ammoCur, ammoMax;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootVolume;
    public AudioClip[] pickupSound;
    public float pickupVolume;
    public AudioClip[] switchSound;
    public float switchVolume;

    public bool isCreator;
    public bool isShield;

}
