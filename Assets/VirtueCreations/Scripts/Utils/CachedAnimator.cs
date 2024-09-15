using UnityEngine;

namespace VIRTUE {
    public abstract class CachedAnimator : CachedMonoBehaviour {
        protected Animator Anim;

        protected override void OnAwake () {
            Anim = GetComponentInChildren<Animator> ();
        }

        /// <summary>
        ///   <para>Returns the value of the given float parameter.</para>
        /// </summary>
        /// <param name="id">The parameter ID.</param>
        /// <returns>
        ///   <para>The value of the parameter.</para>
        /// </returns>
        public float GetFloat (int id) => Anim.GetFloat (id);

        /// <summary>
        ///   <para>Send float values to the Animator to affect transitions.</para>
        /// </summary>
        /// <param name="id">The parameter ID.</param>
        /// <param name="value">The new parameter value.</param>
        public void PlayAnim (int id, float value) => Anim.SetFloat (id, value);

        /// <summary>
        ///   <para>Send float values to the Animator to affect transitions.</para>
        /// </summary>
        /// <param name="id">The parameter ID.</param>
        /// <param name="value">The new parameter value.</param>
        /// <param name="dampTime">The damper total time.</param>
        /// <param name="deltaTime">The delta time to give to the damper.</param>
        public void PlayAnim (int id, float value, float dampTime, float deltaTime) => Anim.SetFloat (id, value, dampTime, deltaTime);

        /// <summary>
        ///   <para>Returns the value of the given boolean parameter.</para>
        /// </summary>
        /// <param name="id">The parameter ID.</param>
        /// <returns>
        ///   <para>The value of the parameter.</para>
        /// </returns>
        public bool GetBool (int id) => Anim.GetBool (id);

        /// <summary>
        ///   <para>Sets the value of the given boolean parameter.</para>
        /// </summary>
        /// <param name="id">The parameter ID.</param>
        /// <param name="value">The new parameter value.</param>
        public void PlayAnim (int id, bool value) => Anim.SetBool (id, value);

        /// <summary>
        ///   <para>Returns the value of the given integer parameter.</para>
        /// </summary>
        /// <param name="id">The parameter ID.</param>
        /// <returns>
        ///   <para>The value of the parameter.</para>
        /// </returns>
        public int GetInteger (int id) => Anim.GetInteger (id);

        /// <summary>
        ///   <para>Sets the value of the given integer parameter.</para>
        /// </summary>
        /// <param name="id">The parameter ID.</param>
        /// <param name="value">The new parameter value.</param>
        public void PlayAnim (int id, int value) => Anim.SetInteger (id, value);

        /// <summary>
        ///   <para>Sets the value of the given trigger parameter.</para>
        /// </summary>
        /// <param name="id">The parameter ID.</param>
        public void PlayAnim (int id) => Anim.SetTrigger (id);

        /// <summary>
        ///   <para>Resets the value of the given trigger parameter.</para>
        /// </summary>
        /// <param name="id">The parameter ID.</param>
        public void ResetTrigger (int id) => Anim.ResetTrigger (id);

        /// <summary>
        ///   <para>Rebind all the animated properties and mesh data with the Animator.</para>
        /// </summary>
        public void Rebind () => Anim.Rebind ();
    }
}