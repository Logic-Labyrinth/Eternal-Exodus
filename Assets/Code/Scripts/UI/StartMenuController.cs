using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject mainMenu;
    public bool canClick = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!animator) {
            animator = transform.GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("canClick", canClick);

        if (animator.GetBool("canClick")) {
            if (Input.GetButtonDown("Basic Attack") || Input.GetButtonDown("Jump")) {
                animator.SetBool("hasClicked", true);
                mainMenu.SetActive(true);
                this.gameObject.SetActive(false);
            }
        }
    }
}
