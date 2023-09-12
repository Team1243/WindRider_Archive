using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public enum TransitionsEffect
{
    none = -1,
    fade,
    cross,
    circleWap
}
public class sceneManager : MonoBehaviour
{
    public static sceneManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void ChangeNextSceen(TransitionsEffect transitionsEffect = TransitionsEffect.none) => StartCoroutine(SceneChangeTransitions(SceneManager.GetActiveScene().buildIndex + 1, transitionsEffect));

    public void ChangeNextSceenAndTransitionsEffectNum(int num) => StartCoroutine(SceneChangeTransitions(SceneManager.GetActiveScene().buildIndex + 1, (TransitionsEffect)num));

    public void ReloadSceen() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void ChangeSceenAsNumber(int index, TransitionsEffect transitionsEffect = TransitionsEffect.none) => StartCoroutine(SceneChangeTransitions(index, transitionsEffect));

    IEnumerator SceneChangeTransitions(int index, TransitionsEffect transitionsEffect)
    {
        if ((int)transitionsEffect == -1)
        {
            SceneManager.LoadScene(index);
            yield return null;
        }

        GameObject chidObj = transform.GetChild((int)transitionsEffect).gameObject;
        chidObj.gameObject.SetActive(true);
        Animator ani = chidObj.GetComponent<Animator>();
        ani.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(index);
        yield return new WaitForSeconds(1.1f);
        chidObj.gameObject.SetActive(false);
        // ani.SetTrigger("End");
    }
}
