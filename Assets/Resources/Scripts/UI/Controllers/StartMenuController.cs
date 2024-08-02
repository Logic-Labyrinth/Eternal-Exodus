using UnityEngine;

namespace TEE.UI.Controllers {
    public class StartMenuController : MonoBehaviour {
        [SerializeField] Animator   animator;
        [SerializeField] GameObject levelSelectMenu;
        public           bool       canClick;
        static readonly  int        AnimatorBoolHasClicked = Animator.StringToHash("hasClicked");

        void Awake() {
            animator ??= GetComponent<Animator>();
        }

        void Update() {
            if (!canClick) return;
            if (Input.GetButtonDown("Basic Attack") || Input.GetButtonDown("Jump")) {
                animator.SetBool(AnimatorBoolHasClicked, true);
            }
        }

        public void StartGame() {
            levelSelectMenu.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}