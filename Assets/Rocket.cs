using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float rcsThrust = 20f; // default value when the object is first initialized, it could be different from prefab, Give lever to designer
    [SerializeField] float mainThrust = 3f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip levelLoadSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem levelLoadParticles;

    Rigidbody rigidbody;
    AudioSource audioSource;
    bool isDetectingCollision;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        isDetectingCollision = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Debug.isDebugBuild) { //debug keys work if this is the debug build
            DebugCollisionToggle();
            DebugLoadLevel();
        }
        if (state == State.Alive){
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (state != State.Alive) { return; } //do nothing after colliding once

        switch (collision.gameObject.tag) {
            case "Friendly":
                //do nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;

            default:
                if (isDetectingCollision)
                {
                    StartDeathSequence();
                }
                break;
        }
    }

    private void DebugLoadLevel() {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
               
        }

    }

    private void DebugCollisionToggle() {
        if (Input.GetKeyDown(KeyCode.C)) {
            isDetectingCollision = !isDetectingCollision;
            print("troggled");
        }
    }

    private void StartSuccessSequence() {
        state = State.Transcending;
        audioSource.Stop(); //stop the thrusting sound
        audioSource.PlayOneShot(levelLoadSound);
        levelLoadParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay); //load next scene after 1 second

        print("hit finish");
    }

    private void StartDeathSequence() {
        //if (isDetectingCollision)
        //{
            state = State.Dying;
            audioSource.Stop();
            audioSource.PlayOneShot(deathSound);
            deathParticles.Play();
            Invoke("LoadFirstScene", levelLoadDelay);
            print("dead");
        //}
    }

    private void LoadNextScene() {
        int current = SceneManager.GetActiveScene().buildIndex;
        int totalNumScenes = SceneManager.sceneCountInBuildSettings;

        if ((current + 1) == totalNumScenes)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(current + 1);
        }

        //SceneManager.LoadScene(1);
    }

    private void LoadFirstScene() {
        SceneManager.LoadScene(0);
    }

    private void RespondToThrustInput() {
        //float thrustSpeed = Time.deltaTime * mainThrust;

        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust() {
        print("space pressed");
        rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine); //provide audio clip to be played
        }
        if (!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
    }

    private void RespondToRotateInput(){
        rigidbody.angularVelocity = Vector3.zero; //stop physics due to unity engine
        float rotationSpeed = Time.deltaTime * rcsThrust;
        if(Input.GetKey(KeyCode.A)){
            print("rotating left");
            transform.Rotate(Vector3.forward * rotationSpeed);
        }

        else if(Input.GetKey(KeyCode.D)){
            print("Rotating right");
            transform.Rotate(-Vector3.forward * rotationSpeed);
        }
    }

}

