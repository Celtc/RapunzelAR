using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class CharacterRootMotion : MonoBehaviour
{
    #region Variables (private)

    [SerializeField]
    public Animator _animator;

    [SerializeField]
    public List<AnimationCustomCurves> _statesAnimationsCurves;

    #endregion

    #region Properties (public)

    #endregion

    #region Unity event functions

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
    }

    /// <summary>
    /// Overrides animation motion.
    /// </summary>
    void OnAnimatorMove()
    {
        if (_animator)
        {
            AnimatorStateInfo mainStateInfo;

            if (_animator.IsInTransition(0))
                mainStateInfo = _animator.GetNextAnimatorStateInfo(0);
            else
                mainStateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            var stateAnimationCurves = _statesAnimationsCurves.Find(x => x.nameHash == mainStateInfo.nameHash);
            if (stateAnimationCurves != null)
            {
                var animProgress = mainStateInfo.normalizedTime;
                
                Vector3 deltaPosition = new Vector3(
                    stateAnimationCurves.deltaX.Evaluate(animProgress) * Time.deltaTime,
                    stateAnimationCurves.deltaY.Evaluate(animProgress) * Time.deltaTime,
                    stateAnimationCurves.deltaZ.Evaluate(animProgress) * Time.deltaTime
                );

                deltaPosition = transform.rotation * deltaPosition;

                transform.position += deltaPosition;
            }            
        }
    }

    #endregion

    #region Methods

    #endregion    
}
