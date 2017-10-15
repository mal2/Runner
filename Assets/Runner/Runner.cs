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
    private Transform[] children = null;

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

        children = new Transform[transform.childCount];
        int i = 0;
        foreach (Transform T in transform)
            children[i++] = T;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            rbResize();
        }
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
            if (cam.fieldOfView < maxZoomFOV)
            {
                cam.fieldOfView += zoomSpeed / 40;
            }

            transform.DetachChildren();                     // Detach
            if (transform.localScale.x < 3) {
                transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * 0.5f;
            }
            //transform.localScale = new Vector3(3f, 3f, 3f);  // Scale        
            foreach (Transform T in children)              // Re-Attach
                T.parent = transform;

            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            { 
                timeLeft = 5.0f;
                setResize = 2;
                //transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        if (setResize == 2)
        {
            transform.DetachChildren();
            if (transform.localScale.x > 1) {
                transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * 0.5f;
            }
            foreach (Transform T in children)              // Re-Attach
                T.parent = transform;
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
