using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataMenu : MonoBehaviour
{
    public bool active = false;
    public Animator shadeAnimator;
    public GameObject UI;
    private void Awake()
    {
        if (LanguageManager.Instance != null)
        {
            transform.GetChild(2).GetComponent<Text>().text = LanguageManager.Instance.deleteTexts[LanguageManager.language];
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.C))
        { 
            MenuOpen();
        }
        if (active)
        {
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                DeleteYes();
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                MenuClose();
            }
        }
    }
    public void MenuOpen()
    {
        if (active == false && FindAnyObjectByType<QuitMenu>().active == false && GameObject.Find("UI_options/options_menu") == null)
        {
            transform.Find("Text").GetComponent<Text>().text = LanguageManager.Instance.deleteTexts[LanguageManager.language];
            UI.GetComponent<Canvas>().sortingOrder = 5;
            active = true;
            EventController.RaiseOnPlayAudio(AudioType.Show_UI, true);
            GetComponent<Animator>().Play("data_show");
            if (shadeAnimator != null)
            {
                shadeAnimator.Play("Shade");
            }
            FindAnyObjectByType<Player>().readyToMove = false;
        }
    }
    public void MenuClose()
    {
        UI.GetComponent<Canvas>().sortingOrder = 2;
        active = false;
        EventController.RaiseOnPlayAudio(AudioType.Select, true);
        GetComponent<Animator>().Play("data_close");
        if (shadeAnimator != null)
        {
            shadeAnimator.Play("Hide");
        }
        FindAnyObjectByType<Player>().readyToMove = true;
    }
    public void DeleteYes()
    {
        EventController.RaiseOnPlayAudio(AudioType.BGM, false);
        EventController.RaiseOnPlayAudio(AudioType.Select, true);
        FindAnyObjectByType<MainManager>().DeleteData();
    }
}
