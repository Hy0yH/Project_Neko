using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossEncounterTrigger : MonoBehaviour
{
    [Header("Encounter")]
    [SerializeField] private BossEnemy boss;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera gameplayCamera;
    [SerializeField] private Transform bossCameraFocus;

    [Header("Timing")]
    [SerializeField, Min(0f)] private float cameraMoveTime = 1f;
    [SerializeField, Min(0f)] private float bossRevealTime = 1.5f;
    [SerializeField, Min(0f)] private float cameraReturnTime = 1f;

    private bool hasTriggered;

    private void Awake()
    {
        boss?.StopPattern();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered)
            return;

        PlayerController playerController =
            other.GetComponentInParent<PlayerController>();

        if (playerController == null)
            return;

        hasTriggered = true;

        // 연출 도중 다른 플레이어 콜라이더가 재진입하는 것을 방지
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(PlayBossIntro(playerController));
    }

    private IEnumerator PlayBossIntro(PlayerController playerController)
    {
        PlayerCombat playerCombat =
            playerController.GetComponent<PlayerCombat>();

        Transform playerTransform = playerController.transform;
        Transform originalFollowTarget = gameplayCamera.Follow;

        // 1. 플레이어 정지
        playerController.SetControlLocked(true);

        if (playerCombat != null)
            playerCombat.SetCombatLocked(true);

        // 2. 카메라를 보스에게 이동
        gameplayCamera.Follow = bossCameraFocus;

        yield return new WaitForSeconds(cameraMoveTime);

        // 이 시점에 보스 이름, 포효 애니메이션, 효과음 등을 실행
        yield return new WaitForSeconds(bossRevealTime);

        // 3. 카메라를 원래 플레이어에게 복귀
        gameplayCamera.Follow =
            originalFollowTarget != null
                ? originalFollowTarget
                : playerTransform;

        yield return new WaitForSeconds(cameraReturnTime);

        // 4. 플레이어 조작 복구 및 보스전 시작
        playerController.SetControlLocked(false);

        if (playerCombat != null)
            playerCombat.SetCombatLocked(false);

        boss.BeginBattle(playerTransform);

        gameObject.SetActive(false);
    }
}