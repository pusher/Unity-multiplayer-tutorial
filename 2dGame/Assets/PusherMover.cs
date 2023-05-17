using UnityEngine;

public class PusherMover : MonoBehaviour
{
    private PusherManager _pusherManager;
    private Animator anim;
    private Vector2 targetPos;
    public float stepSize = 0.1f;
    public int direction = -1;
    AudioSource audioData;
    public int playerNumber;
    public Rigidbody2D rb;

    void Start()
    {
        //anim = GetComponents<Animator>();
        anim = this.gameObject.GetComponent<Animator>();
        //rb = GetComponent<Rigidbody2D>();
        if (_pusherManager == null)
        {
            //_pusherManager = GetComponent<PusherManager>();
            var go = GameObject.Find("PusherManager");
            _pusherManager = go.GetComponent<PusherManager>();
        }
        transform.localScale = new Vector3(-1, 1, 1);
        audioData = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        audioData.Pause();

        // control the character manually
        /*
                State currentState = State.IDLE;
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    currentState = State.RUNRIGHT;
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    currentState = State.RUNLEFT;
                }
                else if (Input.GetKey(KeyCode.Space))
                {
                    currentState = State.ATTACK;
                }
                else
                {
                    currentState = State.IDLE;
                }
        */
        State currentState = _pusherManager.CurrentState(playerNumber);

        if (currentState == State.RUNRIGHT)
        {
            run(1);
        }
        else if (currentState == State.RUNLEFT)
        {
            run(-1);
        }
        else if (currentState == State.ATTACK)
        {
            attack();
            //jump();
        }
        else if (currentState == State.IDLE)
        {
            idle();
        }

    }

    void attack()
    {
        audioData.Play(0);
        anim.SetBool("IsRunning", false);
        anim.SetBool("IsAttacking", true);
        anim.SetBool("IsIdle", false);
    }

    void run(int direction)
    {
        anim.SetBool("IsRunning", true);
        anim.SetBool("IsAttacking", false);
        anim.SetBool("IsIdle", false);
        targetPos = new Vector2(transform.position.x + stepSize * direction, transform.position.y);
        transform.localScale = new Vector3(-direction, 1, 1);
        transform.position = targetPos;
    }

    void idle()
    {
        anim.SetBool("IsRunning", false);
        anim.SetBool("IsAttacking", false);
        anim.SetBool("IsIdle", true);
    }

}
