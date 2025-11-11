using System.Threading.Tasks;
using System;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;

public class WebRequestController : MonoBehaviour
{
    const String urlApi = "https://script.google.com/a/macros/uniriotec.br/s/AKfycbwKVjh8fzcvpVMkTEFv_F_L9FWlWYGkQLr7tTuAPnekyVfSLq3YhZxEqdjn7yzptYrjXw/exec?";

    public static WebRequestController Instance { get; private set; }

    public static int Balance = -1;
    public static Guid UserID;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async Task<String> SetBalance(Guid userId)
    {
        try
        {
            UserID = userId;

            var urlRequest = urlApi + "type=setbalance";
            urlRequest += "&userid=" + UserID;
            urlRequest += "&difficult=" + Balance;

            var request = new UnityWebRequest(urlRequest, "GET");
            request.downloadHandler = new DownloadHandlerBuffer();
            //request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Content-Type", " text/plain;charset=utf-8");

            var operation = request.SendWebRequest();

            while (!operation.isDone) await Task.Yield();

            var responseString = "";
            if (request.result == UnityWebRequest.Result.Success)
            {
                responseString = request.downloadHandler.text;

                var json = JSON.Parse(responseString);
                responseString = "OK";
            }
            else
            {
                Debug.LogError("Erro: " + request.error);
            }

            return responseString;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<String> SetGameplay(Guid userId, float score, float speed, float time, string op,
                                          float pid, int counter, float hits, float distance, float level, float hi,
                                          float setpoint)
    {
        try
        {

             var urlRequest = urlApi + "type=setgameplay";
            urlRequest += "&userid=" + UserID;
            urlRequest += "&difficult=" + Balance;
            urlRequest += "&score=" + score;
            urlRequest += "&moveSpeed=" + speed;
            urlRequest += "&time=" + Convert.ToInt32(time);
            urlRequest += "&operation=" + op;
            urlRequest += "&pid=" + pid;
            urlRequest += "&counter=" + counter;
            urlRequest += "&hits=" + hits;
            urlRequest += "&distance=" + distance;
            urlRequest += "&level=" + level;
            urlRequest += "&hi=" + hi;
            urlRequest += "&setpoint=" + setpoint;
            

            var request = new UnityWebRequest(urlRequest, "GET");
            request.downloadHandler = new DownloadHandlerBuffer();
            //request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Content-Type", " text/plain;charset=utf-8");

            var operation = request.SendWebRequest();

            while (!operation.isDone) await Task.Yield();

            var responseString = "";
            if (request.result == UnityWebRequest.Result.Success)
            {
                responseString = request.downloadHandler.text;

                var json = JSON.Parse(responseString);
                responseString = "OK";
            }
            else
            {
                Debug.LogError("Erro: " + request.error);
            }

            return responseString;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<String> GetBalanceBegin()
    {
        try
        {
            var urlRequest = urlApi + "type=getbalancebegin&qtde=2";

            var request = new UnityWebRequest(urlRequest, "GET");
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", " text/plain;charset=utf-8");
            //request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();

            while (!operation.isDone) await Task.Yield();

            var responseString = "";
            if (request.result == UnityWebRequest.Result.Success)
            {
                responseString = request.downloadHandler.text;

                var json = JSON.Parse(responseString);
                var balance = json["balance"].AsInt;
                Balance = balance;
                responseString = balance.ToString();
            }
            else
            {
                Debug.LogError("Erro: " + request.error);
            }

            return responseString;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
