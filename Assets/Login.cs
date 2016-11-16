using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Networking;

public class Login : MonoBehaviour
{

    private string email ="";
    private string password = "";
    private string message = "";

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 5, 500, 30), message);

        GUI.Label(new Rect(10, 30, 100, 30), "Email");
        email = GUI.TextField(new Rect(120, 30, 200, 20), email, 25);

        GUI.Label(new Rect(10, 70, 100, 30), "Password");
        password = GUI.PasswordField(new Rect(120, 70, 200, 20), password, "*"[0], 25);

        if (GUI.Button(new Rect(10, 90, 90, 20), "Entrar"))
        {
            message = "";
            Debug.Log("Started login");
            StartCoroutine(invokeLogin());
            Debug.Log("Ended login");

        }
    }

    IEnumerator invokeLogin()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/restserver-game/services/player/login/" + email + "/" + password);
        yield return www.Send();

        if (www.isError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show json results as text
            string json = www.downloadHandler.text;
            Debug.Log(json);

            // Or convert result from json
            //Player player = JsonUtility.FromJson<Player>(json);
            Player player = JsonHelper.FromJson<Player>(json);
            if (player != null)
            {
                Debug.Log("Name: " + player.name);
                Debug.Log("Value: " + player.email);
                if(player.itens != null)
                {
                    Debug.Log("qtd itens: " + player.itens.Count);
                }
            }else
            {
                message = "User or password not found";
                Debug.Log(message);
            }
        }
    }

}
