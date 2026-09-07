using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
    [SerializeField] GameObject player;
    PlayerMovement playerMovement;
    CharacterController playerController;

    Weapon weapon;
    [SerializeField] Text speedDebug;
    [SerializeField] Text stateDebug;
    [SerializeField] Text attackDebug;

    void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        playerController = player.GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {

        stateDebug.text = $"{playerMovement.IsGrounded()}, {playerMovement.isCrouching}, {playerMovement.isSliding}, {playerMovement.isRunning}";
        speedDebug.text = $"{Mathf.Round(playerController.velocity.magnitude)}";
        //attackDebug.text = $"attack ({weapon.attack}, 0)";
    }
}
