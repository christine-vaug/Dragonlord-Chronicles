using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animator2D : MonoBehaviour {

    [System.Serializable]
    public struct AnimationPair {
        public Sprite[] sprites;
        public string name;
    }

    public float secondsPerUpdate;
    public AnimationPair[] pairs;
    

    Dictionary<string, Animation2D> animations;

    Animation2D currentClip;
    int currentFrame;

    new SpriteRenderer renderer;


    float timer;

	// Use this for initialization
    void Awake () {

        renderer = GetComponent<SpriteRenderer>();
        animations = new Dictionary<string, Animation2D>();

        //Load animation
        for (int i = 0; i < pairs.Length; i++) {
            animations.Add(pairs[i].name, new Animation2D {
                sprites = pairs[i].sprites,
                secondsPerUpdate = secondsPerUpdate
            });
        }


        pairs = null;
    }

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        PlayAnimation(Time.deltaTime);
	}

    void PlayAnimation (float dt) {

        if (currentClip == null) return;

        timer += dt;

        if (timer >= currentClip.secondsPerUpdate) {
            timer = 0;
            currentFrame++;
            currentFrame = currentFrame % currentClip.sprites.Length;
        }

        Sprite sp = currentClip.sprites[currentFrame];
        renderer.sprite = sp;
    }

    public void SetAnimationClip (string name, bool flip = false) {

        renderer.flipX = flip;
        currentClip = animations[name];
    }

    public void StopAnimations () {

        currentClip = null;
    }
    
}
