using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialog;
    public Button nextButton;

    public Image dialogBackground;
    public Image optionsBackground;

    public SOText dialogText;
    public SOText dialogName;

    [SerializeField]
    private DialogOption[] _options;

    public void Init()
    {
        SetBoxVisibility(false);
        ResetFields();
    }

    public void SetBoxVisibility(bool isVisible)
    {
        ResetFields();

        this.gameObject.SetActive(isVisible);
        dialogBackground.gameObject.SetActive(isVisible);
        optionsBackground.gameObject.SetActive(isVisible);

        if (isVisible)
        {
            nextButton.interactable = true;
        }          
        else
        {
            nextButton.interactable = false;
        }
    }

    public void Update()
    {
        if(dialogText.value != "")
            dialog.text = dialogText.value;

        if(dialogName.value != "")
            characterName.text = dialogName.value;
    }

    private void ResetFields()
    {
        dialogName.value = "";
        characterName.text = dialogName.value;

        dialogText.value = "";
        dialog.text = dialogText.value;

        dialogBackground.gameObject.SetActive(false);

        ResetOptions();

        EnableDialogOptions(false);
    }

    private void ResetOptions()
    {
        if(_options.Length > 0)
        {
            for (int i = 0; i < _options.Length; i++)
            {
                _options[i].Reset();
            }
        }
    }

    private void EnableDialogOptions(bool enable)
    {
        if (_options.Length > 0)
        {
            for (int i = 0; i < _options.Length; i++)
            {
                _options[i].EnableOption(enable);
            }
        }
    }

    public void SetOptions(List<Option> options)
    {
        optionsBackground.gameObject.SetActive(true);

        for (int i = 0; i < _options.Length; i++)
        {
            _options[i].Reset();
            _options[i].SetText(options[i].text);
            _options[i].SetIndex(options[i].index);
            _options[i].EnableOption(true);
        }           
    }
}
