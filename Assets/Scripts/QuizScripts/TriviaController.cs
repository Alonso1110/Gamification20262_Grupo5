using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriviaController : MonoBehaviour
{
    [SerializeField] private List<Button> OptionsReference;

    private void Start()
    {
        SetAnswers();
    }

    public void SetAnswers()
    {
        int totalOptions = OptionsReference.Count;
        int rightOption = Random.Range(0, totalOptions);
        for (int i = 0; i < totalOptions; i++)
        {
            if (i == rightOption) SetRightAnswer(OptionsReference[i]);
            else SetWrongAnswer(OptionsReference[i]);
        }
    }

    private void SetWrongAnswer(Button answer)
    {
        ChangeButtonSelectedColor(answer, Color.red);
    }

    private void SetRightAnswer(Button answer)
    {
        ChangeButtonSelectedColor(answer, Color.green);
    }

    private void ChangeButtonSelectedColor(Button b, Color c)
    {
        ColorBlock cb = b.colors;

        cb.selectedColor = c;

        b.colors = cb;
    }
}
