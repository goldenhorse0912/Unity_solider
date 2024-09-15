using UnityEngine;
#if UNITY_ANDROID
//using Google.Play.Review;
#else
using UnityEngine.iOS;
#endif
using System.Collections;

namespace VIRTUE
{
    public sealed partial class GameController : MonoBehaviour
    {
        public void RequestReviewNow()
        {
            //Request for Review
#if UNITY_ANDROID
            //   StartCoroutine (RequestReview ());
#else
			Device.RequestStoreReview ();
#endif
        }

#if UNITY_ANDROID
        /*ReviewManager _reviewManager;
        PlayReviewInfo _playReviewInfo;

        IEnumerator RequestReview () {
            _reviewManager = new ReviewManager ();
            var requestFlowOperation = _reviewManager.RequestReviewFlow ();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError) {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                yield break;
            }
            _playReviewInfo = requestFlowOperation.GetResult ();

            //Launch review
            var launchFlowOperation = _reviewManager.LaunchReviewFlow (_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError) {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                yield break;
            }
        }*/
#endif
    }
}