using UnityEngine;

public class BobReset : StateMachineBehaviour {
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (animator.TryGetComponent(out SpriteBobber bob)) {
            bob.ResetTween();
        }
    }
}
