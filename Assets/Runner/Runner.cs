using UnityEngine;
using System.Collections;


public class Runner : MonoBehaviour {

    public static float distanceTraveled;
    public float acceleration;
    private bool touchingPlatform;
    private bool candobulejump;
    public Rigidbody rb;
    private Renderer rend;
    public Vector3 boostVelocity, jumpVelocity;
    public float gameOverY;
    private Vector3 startPosition;
    private static int boosts;

   // private static Runner instance;

    // Use this for initialization
    void Start () {
        

        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();

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
        GUIManager.SetDistance(distanceTraveled);
        if (transform.localPosition.y < gameOverY)
        {
            GameEventManager.TriggerGameOver();
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
