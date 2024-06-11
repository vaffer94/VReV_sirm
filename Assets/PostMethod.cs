using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class PostMethod : MonoBehaviour
{
    TMP_InputField outputArea;

    void Start()
    {
        outputArea = GameObject.Find("OutputArea").GetComponent<TMP_InputField>();
        GameObject.Find("PostButton").GetComponent<Button>().onClick.AddListener(PostData);
    }

    void PostData() => StartCoroutine(PostData_Coroutine());

    IEnumerator PostData_Coroutine()
    {
        outputArea.text = "Loading...";
        // string uri = "https://my-json-server.typicode.com/typicode/demo/posts";
        string uri = "https://663b7e99fee6744a6ea1d937.mockapi.io/api/vr/vf/1/TestGame";
        WWWForm form = new WWWForm();
        form.AddField("title", "test data");
        using (UnityWebRequest request = UnityWebRequest.Post(uri, form))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                outputArea.text = request.error;
            else
                outputArea.text = request.downloadHandler.text;
        }
    }
}
