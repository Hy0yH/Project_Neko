using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerForm : MonoBehaviour
{
    [SerializeField] private PlayerFormSO[] forms;
    [SerializeField] private InputActionReference switchAction;

    private PlayerController player;
    private Animator anim;
    private BoxCollider2D boxCollider2D;

    private int currentIndex = 0;
    public bool IsTransforming { get; private set; }

    public PlayerFormSO CurrentForm { get; private set; }
    public event Action<PlayerFormSO> OnFormChanged;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        if (forms == null || forms.Length == 0)
        {
            Debug.LogError("Forms is empty", this);
            return;
        }

        ApplyForm();
    }

    private void OnEnable()
    {
        if (switchAction != null) switchAction.action.Enable();
    }

    private void OnDisable()
    {
        if (switchAction != null) switchAction.action.Disable();
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsTransforming) return;
        if(switchAction != null && switchAction.action.WasPressedThisFrame())
        {
            SwitchForm();
        }
    }

    private void ApplyForm()
    {
        CurrentForm = forms[currentIndex];

        anim.runtimeAnimatorController = CurrentForm.animatorController;

        player.maxMoveSpeed = CurrentForm.maxMoveSpeed;

        boxCollider2D.size = CurrentForm.colliderSize;
        boxCollider2D.offset = CurrentForm.colliderOffset;

        OnFormChanged?.Invoke(CurrentForm);
    }

    private void SwitchForm()
    {
        IsTransforming = true;
        anim.SetTrigger("Transform");

        player.playerAudio?.PlayFormChange();
    }
    public void OnTransformEnd()
    {
        currentIndex = (currentIndex + 1) % forms.Length;

        ApplyForm();

        IsTransforming = false;
    }
}
