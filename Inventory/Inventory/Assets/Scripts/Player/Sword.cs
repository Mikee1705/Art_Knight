using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private float swordOffsetX = 1f;
    [SerializeField] private float swordOffsetY = 0.5f;
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;

    [SerializeField] private float swordAttackCD = .5f;

    //[SerializeField] private Transform weaponCollider;
    private PlayerControls playerControls;
    private Animator myAnimator;
    private PlayerController playerController;
    private ActiveWeapon activeWeapon;
    private bool attackButtonDown, isAttacking = false;
    private Vector2 lastinput;

    private GameObject slashAnim;


    private void Awake() {
        playerController = GetComponentInParent<PlayerController>();
        activeWeapon = GetComponentInParent<ActiveWeapon>();
        myAnimator = GetComponent<Animator>();
        playerControls = new PlayerControls();
    }

    private void OnEnable() {
        playerControls.Enable();
    }

    void Start() {
        playerControls.Combat.Attack.started += _ => StartAttacking();
        playerControls.Combat.Attack.canceled += _ => StopAttacking();
    }

    private void StartAttacking()
    {
        attackButtonDown = true;
    }

    private void StopAttacking()
    {
        attackButtonDown = false;
    }

    private void Update()
    {
        RotateSword();
        UpdateSwordPosition();
        Attack();
    }

    private void Attack()
    {
        if (attackButtonDown && !isAttacking)
        {
            isAttacking = true;
            myAnimator.SetTrigger("Attack");
           // weaponCollider.gameObject.SetActive(true);
            slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
            slashAnim.transform.parent = this.transform.parent;
            StartCoroutine(AttackCDRoutine());
        }

    }

    private IEnumerator AttackCDRoutine()
    {
        yield return new WaitForSeconds(swordAttackCD);
        isAttacking = false;
    }

    public void DoneAttackingAnimEvent()
    {
        //weaponCollider.gameObject.SetActive(false);
    }

    public void SwingUpFlipAnimEvent()
    {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(-180, 0, 0);

        if (playerController.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    public void SwingDownFlipAnimEvent()
    {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (playerController.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = false;
        }
    }


    private void UpdateSwordPosition()
    {
        
        Vector2 playerFacingDirection = playerController.GetLastInput();

        
        float offsetX = playerFacingDirection.x < 0 ? -swordOffsetX : swordOffsetX;
        Vector3 newPosition = playerController.transform.position + new Vector3(offsetX, swordOffsetY, 0f);

        
        transform.position = newPosition;
    }


    private void RotateSword()
    {
        
        Vector2 playerFacingDirection = playerController.GetLastInput();

        
        float angle = Mathf.Atan2(playerFacingDirection.y, playerFacingDirection.x) * Mathf.Rad2Deg;

        if (playerController.isFlip)
        {
            activeWeapon.transform.rotation = Quaternion.Euler(0, -180, angle);
           // weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            activeWeapon.transform.rotation = Quaternion.Euler(0, 0, angle);
            //weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
