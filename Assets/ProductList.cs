using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Networking;
using OAuth;
using System;

public class ProductList : MonoBehaviour {

    void Start()
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        string url = "http://localhost/restserver-game/services/game/product";

        UnityWebRequest www = UnityWebRequest.Get(url);


        string oauthConsumerKey = "0kJ83wF2s4Y-kRp";
        string oauthConsumerSecret = "SeOAsjPNYs1z_f2";
        string oauthToken = null;
        string oauthTokenSecret = null;



        OAuthBase oauthBase = new OAuthBase();
        

        string oauthRealm = "";

        string formattedUri = String.Format(System.Globalization.CultureInfo.InvariantCulture, url, "");
        Uri urlUri = new Uri(formattedUri);

        string oauthSignature = oauthBase.GenerateSignature(
            urlUri,
            oauthConsumerKey,
            oauthConsumerSecret,
            oauthToken,
            oauthTokenSecret,
            "GET",
            oauthBase.GenerateTimeStamp(),
            oauthBase.GenerateNonce(),
            out oauthRealm
            );

        www.SetRequestHeader("Authorization", oauthRealm);

        yield return www.Send();

        Debug.Log(oauthSignature);
        Debug.Log(oauthRealm);


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
            Product [] product = JsonHelper.FromJson(json);
            foreach(Product p in product)
            {
                Debug.Log("Name: " + p.name);
                Debug.Log("Value: " + p.price);

            }
        }
    }
}
