using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class EndingCutsceneTrigger : MonoBehaviour
{
    [Header("Boss Requirement")]
    [SerializeField] private BossHealth bossHealth;

    [Header("Cutscene Points")]
    [SerializeField] private Transform cutsceneStart;
    [SerializeField] private Transform exitPoint;

    [Header("Player Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 3f;
    [SerializeField, Min(0f)] private float jumpVelocity = 5f;
    [SerializeField, Min(0f)] private float runBeforeJump = 0.8f;
    [SerializeField, Min(0f)] private float maxExitTime = 4f;

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeOverlay;
    [SerializeField, Min(0f)] private float fadeDuration = 0.8f;

    [Header("Blur")]
    [SerializeField] private VillaFloorBlurController blurController;

    [Header("Ending UI")]
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool hasPlayed;

    private void Awake()
    {
        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
            fadeOverlay.blocksRaycasts = false;
            fadeOverlay.interactable = false;
        }

        if (endingPanel != null)
            endingPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasPlayed)
            return;

        PlayerController playerController =
            other.GetComponentInParent<PlayerController>();

        if (playerController == null)
            return;

        // 보스가 아직 살아 있다면 실행하지 않음
        if (bossHealth != null)
            return;

        hasPlayed = true;
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(PlayEnding(playerController));
    }

    private IEnumerator PlayEnding(PlayerController playerController)
    {
        PlayerCombat playerCombat = playerController.GetComponent<PlayerCombat>();

        Rigidbody2D playerRigidbody = playerController.GetComponent<Rigidbody2D>();

        PlayerInput playerInput = playerController.GetComponent<PlayerInput>();

        PlayerForm playerForm = playerController.GetComponent<PlayerForm>();

        PlayerInventory playerInventory = playerController.GetComponent<PlayerInventory>();

        // 플레이어 조작과 공격 잠금
        playerController.SetControlLocked(true);

        if (blurController != null)
            blurController.SetCinematicReveal(true);

        if (playerCombat != null)
            playerCombat.SetCombatLocked(true);

        if (playerRigidbody != null)
            playerRigidbody.linearVelocity = Vector2.zero;

        // 변신 잠금
        if (playerForm != null)
            playerForm.enabled = false;

        // 회복 아이템 입력 잠금
        if (playerInventory != null)
            playerInventory.enabled = false;

        // 플레이어의 모든 Input Action 잠금
        if (playerInput != null)
        {
            playerInput.actions.Disable();
            playerInput.enabled = false;
        }

        // 화면을 검게 페이드 아웃
        yield return Fade(0f, 1f);

        // 검은 화면일 때 플레이어 위치 정렬
        if (cutsceneStart != null)
            playerController.transform.position = cutsceneStart.position;

        // 오른쪽을 바라보게 설정
        if (playerController.facingDirection < 0)
            playerController.Flip();

        if (playerRigidbody != null)
            playerRigidbody.linearVelocity = Vector2.zero;

        // 옥상 화면 페이드 인
        yield return Fade(1f, 0f);

        if (playerController.anim != null)
        {
            playerController.anim.SetBool("isJumping", false);
            playerController.anim.SetBool("isWalking", true);
            playerController.anim.SetFloat("velocityY", 0f);
        }

        // 오른쪽으로 자동 이동
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocityX = moveSpeed;
        }

        yield return new WaitForSeconds(runBeforeJump);

        if (playerController.anim != null)
        {
            playerController.anim.SetBool("isWalking", false);
            playerController.anim.SetBool("isJumping", true);
            playerController.anim.SetFloat("velocityY", jumpVelocity);
        }

        // 오른쪽으로 자동 점프
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = new Vector2(moveSpeed, jumpVelocity);
        }

        // ExitPoint까지 계속 오른쪽으로 이동
        float elapsed = 0f;

        bool hasLeftGround = false;

        while (elapsed < maxExitTime)
        {
            elapsed += Time.deltaTime;

            if (playerRigidbody != null)
            {
                playerRigidbody.linearVelocityX = moveSpeed;

                bool isGrounded =
                    playerController.groundCheck != null &&
                    Physics2D.OverlapCircle(
                        playerController.groundCheck.position,
                        0.2f,
                        playerController.groundLayer
                    ) != null;

                // 실제로 지면에서 떨어진 적이 있는지 확인
                if (!isGrounded)
                    hasLeftGround = true;

                if (hasLeftGround && playerController.anim != null)
                {
                    if (isGrounded)
                    {
                        // 착지 후 다시 걷기
                        playerController.anim.SetBool("isJumping", false);
                        playerController.anim.SetBool("isWalking", true);
                        playerController.anim.SetFloat("velocityY", 0f);
                    }
                    else
                    {
                        // 공중에서는 점프 애니메이션
                        playerController.anim.SetBool("isWalking", false);
                        playerController.anim.SetBool("isJumping", true);
                        playerController.anim.SetFloat(
                            "velocityY",
                            playerRigidbody.linearVelocityY
                        );
                    }
                }
            }

            if (exitPoint != null &&
                playerController.transform.position.x >= exitPoint.position.x)
            {
                break;
            }

            yield return null;
        }

        if (playerRigidbody != null)
            playerRigidbody.linearVelocity = Vector2.zero;

        if (playerController.anim != null)
        {
            playerController.anim.SetBool("isWalking", false);
            playerController.anim.SetBool("isJumping", false);
            playerController.anim.SetFloat("velocityY", 0f);
        }
        // 엔딩 화면으로 페이드 아웃
        yield return Fade(0f, 1f);

        // 엔딩 메시지와 버튼 표시
        if (endingPanel != null)
            endingPanel.SetActive(true);
    }

    private IEnumerator Fade(float from, float to)
    {
        if (fadeOverlay == null)
            yield break;

        if (fadeDuration <= 0f)
        {
            fadeOverlay.alpha = to;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(elapsed / fadeDuration);

            fadeOverlay.alpha =
                Mathf.Lerp(from, to, progress);

            yield return null;
        }

        fadeOverlay.alpha = to;
    }

    public void ReturnToMainMenu()
    {
        if (PersistentObject.Instance != null)
        {
            Destroy(PersistentObject.Instance.gameObject);
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}