using UnityEngine;
using System.Collections;

public class Runner : MonoBehaviour {

    public static float distanceTraveled;
    public static float runnerHeight;
    public static float runnerVelocity;
    public float acceleration;
    private bool touchingPlatform;
    private bool candobulejump;
    public float zoomSpeed = 5f;
    public float minZoomFOV = 30f;
    public float maxZoomFOV = 30f;
    public Rigidbody rb;
    private Renderer rend;
    private Camera cam;
    public Vector3 boostVelocity, jumpVelocity;
    public float gameOverY;
    private Vector3 startPosition;
    private static int boosts;
    private static int setResize;
    float timeLeft = 5.0f;

    // private static Runner instance;
    // Use this for initialization
    void Start () {
       
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        GameEventManager.GameStart += GameStart;
        GameEventManager.GameOver += GameOver;
        startPosition = transform.localPosition;
        rend.enabled = false;
        rb.isKinematic = true;
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (touchingPlatform)
            {
                rb.AddForce(jumpVelocity, ForceMode.VelocityChange);
                AudioManager.PlayJump();
                touchingPlatform = false;
                candobulejump = true;
            }
            else
            {
                if (boosts > 0)
                {
                    rb.AddForce(boostVelocity, ForceMode.VelocityChange);
                    AudioManager.PlayBoostJump();
                    boosts -= 1;
                    GUIManager.SetBoosts(boosts);
                } else {
                    if (candobulejump)
                    {
                        rb.AddForce(jumpVelocity, ForceMode.VelocityChange);
                        AudioManager.PlayJump();
                        candobulejump = false;
                    }
                }
            }
        }
        distanceTraveled = transform.localPosition.x;
        runnerVelocity = rb.velocity.x;
        runnerHeight = transform.localPosition.y;

        GUIManager.SetDistance(distanceTraveled);
        if (transform.localPosition.y < gameOverY)
        {
            GameEventManager.TriggerGameOver();
        }
        
        if (setResize != 1) { 
            if (runnerHeight > 10 || runnerVelocity > 20)
            {
                cam.fieldOfView += zoomSpeed/40;
                if (cam.fieldOfView > maxZoomFOV)
                {
                    cam.fieldOfView = maxZoomFOV;
                }
            }
            if (runnerHeight <= 10 && runnerVelocity <= 20)
            {
                cam.fieldOfView -= zoomSpeed/40;
                if (cam.fieldOfView < minZoomFOV)
                {
                    cam.fieldOfView = minZoomFOV;
                }
            }
        }

        if (setResize == 1)
        {
            transform.localScale = new Vector3(3f, 3f, 3f);
            cam.fieldOfView = 30f;
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            { 
                setResize = 0;
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        
    }

    void FixedUpdate()
    {
        if (touchingPlatform)
        {          
            rb.AddForce(acceleration, 0f, 0f, ForceMode.Acceleration);
        }
    }

    public static void AddBoost()
    {
        boosts += 1;
        AudioManager.PlayCollect();
        GUIManager.SetBoosts(boosts);
    }

    public static void rbResize()
    {
        AudioManager.PlayCollect();
        setResize = 1;
    }

    void OnCollisionEnter()
    {
        touchingPlatform = true;
    }

    void OnCollisionExit()
    {
        touchingPlatform = false;
    }

    private void GameStart()
    {
        boosts = 0;
        distanceTraveled = 0f;
        GUIManager.SetBoosts(boosts);
        GUIManager.SetDistance(distanceTraveled);
        transform.localPosition = startPosition;
        rend.enabled = true;
        rb.isKinematic = false;
        enabled = true;
    }

    private void GameOver()
    {
        rend.enabled = false;
        rb.isKinematic = true;
        enabled = false;
    }
}
