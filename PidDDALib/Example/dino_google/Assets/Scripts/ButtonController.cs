using System.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public async void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public async void AcceptGame()
    {
        SceneManager.LoadScene("Start");
    }

    public async void FinishGame()
    {
        var formGoogle = "https://docs.google.com/forms/d/e/1FAIpQLSdtcLMCTOWflHhXFXT-h5fgjhVOCbcERlpcNe6dEbjqtzWY7g/viewform?usp=pp_url&entry.707380848=";

        formGoogle = formGoogle + GameManager.Instance.userId.ToString().Trim().Replace(" ", "%20"); 

        Application.OpenURL(formGoogle);
    }

    public async void Tcle()
    {
        var formGoogle = "https://joccom.uniriotec.br/games/dino/tcle.html";        

        Application.OpenURL(formGoogle);
    }
}
