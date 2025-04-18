using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for UI elements like Text

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float groundCheckRadius = 0.2f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isGrounded;

    public float growthRate = 0.02f;
    public float shrinkRate = 0.02f;
    public float timeToGrow = 3f;
    private float growthTimer;
    private float shrinkTimer = 0f;
    public float timeToShrink = 1f;  // Shrink once every second on bricks

    public GameObject winPanel;
     public GameObject gameOverPanel; // Assign in the inspector
    public AudioClip jumpSound, growSound, shrinkSound, winSound;
    private AudioSource audioSource;
    public string mainMenuSceneName = "MainMenu"; // Set this in the Inspector
    public float timeLimit = 50f; // Time limit in seconds
    private float timer;

    // public TMPro.TextMeshProUGUI timerText;

     public Text timerText; // Assign this in the inspector

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
         timer = timeLimit; // Initialize the timer

        // Ensure the GameOver panel is hidden at the start
         if (gameOverPanel != null)
         {
            gameOverPanel.SetActive(false);
        }
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            audioSource.PlayOneShot(jumpSound);
        }

        // Rotate snowball
        if (movement.x != 0)
        {
            float rotationSpeed = 360f;
            transform.Rotate(0f, 0f, -movement.x * rotationSpeed * Time.deltaTime);
        }

        // Grow while rolling on ground
        if (isGrounded && Mathf.Abs(movement.x) > 0.1f)
        {
            growthTimer += Time.deltaTime;
            if (growthTimer >= timeToGrow)
            {
                Grow();
                growthTimer = 0f;
            }
        }
        else
        {
            growthTimer = 0f;
        }
         // Update Timer
        timer -= Time.deltaTime;
        UpdateTimerUI();

        // Check if time is up
        if (timer <= 0)
        {
            timer = 0;
            GameOver();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
    }

    void Grow()
    {
        Vector3 newScale = transform.localScale + new Vector3(growthRate, growthRate, 0f);
        float maxSize = 3f;
        newScale = Vector3.Min(newScale, new Vector3(maxSize, maxSize, 1f));
        transform.localScale = newScale;
        moveSpeed = Mathf.Max(1f, moveSpeed - 0.1f);
        audioSource.PlayOneShot(growSound);
    }

    void Shrink()
    {
        Vector3 newScale = transform.localScale - new Vector3(shrinkRate, shrinkRate, 0f);
        float minSize = 0.5f;
        newScale = Vector3.Max(newScale, new Vector3(minSize, minSize, 1f));
        transform.localScale = newScale;
        moveSpeed = Mathf.Min(5f, moveSpeed + 0.1f);
        audioSource.PlayOneShot(shrinkSound);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Goal"))
        {
            audioSource.PlayOneShot(winSound);
            winPanel.SetActive(true);
            Time.timeScale = 0f; // Optional: pause the game
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Brick"))
        {
            shrinkTimer += Time.deltaTime;
            if (shrinkTimer >= timeToShrink)
            {
                Shrink();
                shrinkTimer = 0f;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Brick"))
        {
            shrinkTimer = 0f;
        }
    }
     void GameOver()
    {
        // Show the game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Stop the game
        Time.timeScale = 0f;
    }
     public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Ensure time is not frozen
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.RoundToInt(timer).ToString();
        }
    }
}
