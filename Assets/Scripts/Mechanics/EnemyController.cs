using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// A simple controller for enemies. Provides movement control over a patrol path.
    /// </summary>
    [RequireComponent(typeof(AnimationController), typeof(Collider2D))]
    public class EnemyController : MonoBehaviour
    {
        public enum Behavior
        {
            PATH,
            FOLLOW
        }

        public PatrolPath path;
        public AudioClip ouch;
        public Behavior movementBehavior;
        public int followRange = 20;
        public float movementSpeed = 1f;

        internal PatrolPath.Mover mover;
        internal AnimationController control;
        internal Collider2D _collider;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;

        public Bounds Bounds => _collider.bounds;

        void Awake()
        {
            control = GetComponent<AnimationController>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                var ev = Schedule<PlayerEnemyCollision>();
                ev.player = player;
                ev.enemy = this;
            }
        }

        void Update()
        {
            if(movementBehavior == Behavior.PATH)
            {
                if (path != null)
                {
                    if (mover == null) mover = path.CreateMover(control.maxSpeed * 0.5f);
                    control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -movementSpeed, movementSpeed);
                }
            }else if (movementBehavior == Behavior.FOLLOW)
            {
                var player = FindObjectOfType<PlayerController>().gameObject;
                if(player.transform.position.x < this.transform.position.x + followRange && player.transform.position.x > this.transform.position.x - followRange)
                {
                    control.move.x = Mathf.Clamp(player.transform.position.x - transform.position.x, -movementSpeed, movementSpeed);
                }
            }
        }

        

    }
}