// Script này không gắn vào GameObject nào cả.
// Nó sẽ được thêm vào State Animation trong cửa sổ Animator.

using UnityEngine;

public class DeactivateOnExit : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Tự động tắt GameObject (chính là cái PaperUI) sau khi animation "Paper_Close" chạy xong.
        animator.gameObject.SetActive(false);
    }
}