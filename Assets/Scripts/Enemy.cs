﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float _speed = 4f;
    [SerializeField] float _top = 7f;
    [SerializeField] float _bottom = -7f;
    [SerializeField] float _right = 11f;
    [SerializeField] float _left = -11f;
    [SerializeField] int _enemyScoreValue = 10;
    [SerializeField] int _shotMaxTime = 10;
    [SerializeField] int _shotMinTime = 0;
    [SerializeField] Laser _laser = null;
    [SerializeField] AudioClip _laserSound = null;
    private bool _enemyAlive = true;


    AudioSource _audioSource = null;

    private Player _playerRef = null;
    private Animator _animator = null;
    private Collider2D _collider = null;

    private void Awake()
    {
        ResetPosition();
        _playerRef = FindObjectOfType<Player>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Shoot());
    }

    // Update is called once per frame
    void Update()
    {
        if (_enemyAlive)
        {
            MoveDownward();
            CheckEnemyPosition();
        }
    }

    private IEnumerator Shoot()
    {
        while (_enemyAlive)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(_shotMinTime, _shotMaxTime + 1));
            if (_enemyAlive)
            {
                Laser laser = Instantiate(_laser, transform.position, Quaternion.identity);
                AudioSource.PlayClipAtPoint(_laserSound, Camera.main.transform.position);
            }
        }
    }

    private void ResetPosition()
    {
        transform.position = new Vector3(UnityEngine.Random.Range(_left + 1, _right - 1), _top, 0);
    }

    private void MoveDownward()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    private void CheckEnemyPosition()
    {
        if (transform.position.y < _bottom)
        {
            ResetPosition();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string collisionTag = other.gameObject.tag;

        CheckLaserCollision(collisionTag, other);
        CheckPlayerCollision(collisionTag, other);
    }

    private void CheckLaserCollision(string collisionTag, Collider2D other)
    {
        if (collisionTag == "Laser")
        {
            FindObjectOfType<Player>().AddToScore(_enemyScoreValue);
            Destroy(other.gameObject);
            StartDestruction();
        }
    }

    private void CheckPlayerCollision(string collisionTag, Collider2D other)
    {
        if (collisionTag == "Player")
        {
            Debug.Log("Collision with player!");
            _playerRef.Damage();
            StartDestruction();
        }
    }

    private void StartDestruction()
    {
        _enemyAlive = false;
        _collider.enabled = false;
        _animator.SetTrigger("OnDestroy");
        _audioSource.Play();
        // destroy object will be called at the end of the animation
    }

    private void EndDestruction()
    {
        Destroy(gameObject);
    }
}