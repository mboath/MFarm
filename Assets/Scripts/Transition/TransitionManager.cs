using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFarm.Transition
{
    public class TransitionManager : MonoBehaviour
    {
        public string startSceneName = string.Empty;
        public Vector3 startPosition;

/*        private void Awake()
        {
            SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive); //开始时加载UI
        }*/

        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
        }

        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }

        private void Start()
        {
            StartCoroutine(LoadSceneSetActive(startSceneName, startPosition));
        }

        private void OnTransitionEvent(string sceneToGo, Vector3 positionToGo)
        {
            StartCoroutine(TransitScene(sceneToGo, positionToGo));
        }

        /// <summary>
        /// 加载场景并设置为激活
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="targetPosition">人物位置</param>
        /// <returns></returns>
        private IEnumerator LoadSceneSetActive(string sceneName, Vector3 targetPosition)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            SceneManager.SetActiveScene(newScene);

            //移动人物坐标
            EventHandler.CallMoveToPosition(targetPosition);

            EventHandler.CallAfterSceneLoadEvent();
        }

        /// <summary>
        /// 场景切换
        /// </summary>
        /// <param name="sceneName">目标场景</param>
        /// <param name="targetPosition">目标位置</param>
        /// <returns></returns>
        private IEnumerator TransitScene(string sceneName, Vector3 targetPosition)
        {
            EventHandler.CallBeforeSceneUnloadEvent();

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return LoadSceneSetActive(sceneName, targetPosition);
        }
    }
}
