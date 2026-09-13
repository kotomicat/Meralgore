using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerMovement;

public class PlayerInputController : MonoBehaviour
{

    private InputSystem_Actions action;
    private PlayerMovement player;
    private WeaponSwitcher weaponSwitcher;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        weaponSwitcher = GetComponentInChildren<WeaponSwitcher>();

        action = new InputSystem_Actions();
        action.Enable();
    }

    void OnEnable()
    {
        action.Player.Walk.performed += OnEnterWalk;
        action.Player.Walk.canceled += OnExitWalk;

        action.Player.Crouch.performed += OnEnterCrouch;
        action.Player.Crouch.canceled += OnExitCrouch;

        action.Player.Attack.performed += OnStartAttack;
        action.Player.Attack.canceled += OnEndAttack;

        action.Player.AltAttack.performed += OnStartAltAttack;

        action.Player.Reload.started += OnReload;

        action.Player.WeaponSlot1.performed += WeaponSlot1;
        action.Player.WeaponSlot2.performed += WeaponSlot2;
        action.Player.WeaponSlot3.performed += WeaponSlot3;
        action.Player.WeaponSlot4.performed += WeaponSlot4;
    }

    private void OnDisable()
    {
        action.Player.Walk.performed -= OnEnterWalk;
        action.Player.Walk.canceled -= OnExitWalk;

        action.Player.Crouch.performed -= OnEnterCrouch;
        action.Player.Crouch.canceled -= OnExitCrouch;

        action.Player.Attack.performed -= OnStartAttack;
        action.Player.Attack.canceled -= OnEndAttack;

        action.Player.AltAttack.performed -= OnStartAltAttack;

        action.Player.Reload.started -= OnReload;

        action.Player.WeaponSlot1.performed -= WeaponSlot1;
        action.Player.WeaponSlot2.performed -= WeaponSlot2;
        action.Player.WeaponSlot3.performed -= WeaponSlot3;
        action.Player.WeaponSlot4.performed -= WeaponSlot4;
    }

    // ходьба
    void OnEnterWalk(InputAction.CallbackContext obj) => player.EnterWalk();
    void OnExitWalk(InputAction.CallbackContext obj) => player.ExitWalk();

    // присед
    void OnEnterCrouch(InputAction.CallbackContext obj) => player.EnterCrouch();
    void OnExitCrouch(InputAction.CallbackContext obj) => player.ExitCrouch();

    // атака
    void OnStartAttack(InputAction.CallbackContext obj) => weaponSwitcher.weapon.StartAttack();
    void OnEndAttack(InputAction.CallbackContext obj) => weaponSwitcher.weapon.EndAttack();

    // альтернативная атака
    void OnStartAltAttack(InputAction.CallbackContext obj) => weaponSwitcher.weapon.StartAltAttack();

    // перезарядка
    void OnReload(InputAction.CallbackContext obj) => weaponSwitcher.weapon.Reload();

    // смена оружия
    void WeaponSlot1(InputAction.CallbackContext obj) => weaponSwitcher.ChangeWeapon(0);
    void WeaponSlot2(InputAction.CallbackContext obj) => weaponSwitcher.ChangeWeapon(1);
    void WeaponSlot3(InputAction.CallbackContext obj) => weaponSwitcher.ChangeWeapon(2);
    void WeaponSlot4(InputAction.CallbackContext obj) => weaponSwitcher.ChangeWeapon(3);

    void ReadJumpInput()
    {
        if (action.Player.Jump.IsPressed())
        {
            player.Jump();
            player.isJumping = true;
        }
        else player.isJumping = false;
    }

    void Update()
    {
        player.moveInput = action.Player.Move.ReadValue<Vector2>();
        ReadJumpInput();
    }

}