﻿using UnityEngine;
using System.Collections;

public class RangedEnemy : MonoBehaviour {

 
    [Header("Ranged Enemy")]

    public Rigidbody bulletPrefab;
    public Transform firePoint;
    public float fireDelay;
    [Range(0, 4)] public float accuracyOffset;


    private bool _stunned;
    private bool _actionAvailable;
    private int _shootAnimation;
    public ParticleSystem _stunParticles;

    private Transform _playerTransform;
    private Animator _aiAnimator;

    private AudioSource _enemyAudio;
    public AudioClip[] enemySounds;


	void Awake ()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _aiAnimator = GetComponent<Animator>();
        _enemyAudio = GetComponent<AudioSource>();

        _actionAvailable = true;
        _stunned = false;

        _shootAnimation = Animator.StringToHash("Shooting");
        _stunParticles.Stop();
	}

    public void LookAtPlayer()
    {
        if (_stunned)
            return;

        // increase / decrease accuracy offset based on distance to the player

        // Make the archer shoot with some inaccuracy
        Vector3 direction = new Vector3(_playerTransform.position.x + Random.Range(-accuracyOffset, accuracyOffset), transform.position.y, _playerTransform.position.z + Random.Range(-accuracyOffset, accuracyOffset));
        firePoint.transform.LookAt(direction);

        // Look at the player when they are in range
        transform.LookAt(_playerTransform);

       
        // shoot at player if archer is able to 
        if (_actionAvailable)
        {
            _aiAnimator.SetTrigger(_shootAnimation);

            StartCoroutine(Reload());
        }
    }

    // Used in Animation Event 
    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        _enemyAudio.clip = enemySounds[0];
        _enemyAudio.Play();
    }

    IEnumerator Reload()
    {
        _actionAvailable = false;
        yield return new WaitForSeconds(Random.Range(fireDelay + .1f, fireDelay + .75f));
        _actionAvailable = true;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Bullet"))
            StartCoroutine(Stunned());
    }

    IEnumerator Stunned()
    {
        _stunned = true;
        _stunParticles.Play();
        yield return new WaitForSeconds(2f);
        _stunned = false;
        _stunParticles.Stop();
    }
}
