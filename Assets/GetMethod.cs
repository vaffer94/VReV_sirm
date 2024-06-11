using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.Experimental.Rendering;
using System.IO;

public class GetMethod : MonoBehaviour
{
    TMP_InputField outputArea;
    public int counterGetInvocation = 0;
    private bool isRequestInProgress = false; // Flag to indicate if a request is in progress

    // Coordinate piani
    public float lastCoord_CoronalEv = 0.1f; // coordinata piano CORONALE eV
    public float lastCoord_SagitalEv = 0.1f; // coordinata piano SAGITALE eV
    public float lastCoord_AxialEv = 0.1f; // coordinata piano ASSIALE eV

    public float lastCoord_XVr = 0;
    public float lastCoord_YVr = 0;
    public float lastCoord_ZVr = 0;
    // public float newCoordZ = 0.1f; // TEMP

    // Coordinate Max e Min piani (TODO - prenderle da una GET quando potremo caricare altri volumi, questi dati sono relativi ai dati precaricati per questa demo)
    private float sagital_min = -110.00000169970703f;
    private float sagital_max = 109.57056969306755f;
    private float coronal_min = -103.50000169970703f;
    private float coronal_max = 116.07056969306755f;
    private float axial_min = 1.0f;
    private float axial_max = 161.625f;

    private float z_minVol_vr;
    private float z_maxVol_vr;
    private float y_minVol_vr;
    private float y_maxVol_vr;
    private float x_minVol_vr;
    private float x_maxVol_vr;

    public bool areNewCoord = false;

    public GameObject volume; // volume di cui vogliamo calcolare le misure max e min
    public GameObject pianoCoronaleZ; // sezione di taglio che vogliamo muovere con le coordinate mandate da eV
    public GameObject pianoSagitaleX;
    public GameObject pianoAssialeY;

    private AudioSource audioSource;

    void Start()
    {
        outputArea = GameObject.Find("OutputArea").GetComponent<TMP_InputField>();
        GameObject.Find("GetButton").GetComponent<Button>().onClick.AddListener(GetData);
        audioSource = GameObject.Find("SoundManager").GetComponent<AudioSource>();

        if (audioSource == null )
        {
            Debug.LogError("AudioSource component missing!!!");
        }

        CalculateVolumeInfo(volume);

        // Start the coroutine that calls GetData every second
        StartCoroutine(CallGetDataEverySecond());
    }

    // this method was used before adding the flag
    // void GetData() => StartCoroutine(GetData_Coroutine());

    void GetData()
    {
        if (!isRequestInProgress)
        {
            StartCoroutine(GetData_Coroutine());
        }
    }


    IEnumerator GetData_Coroutine()
    {
        isRequestInProgress = true; // Set flag to true when request starts
        ++counterGetInvocation;
        // outputArea.text = "Loading...";
        // string uri = "https://my-json-server.typicode.com/typicode/demo/posts"; // example of original code
        // string uri = "https://663b7e99fee6744a6ea1d937.mockapi.io/api/vr/vf/1/TestGame"; // link to mockAPI used to excange info
        string uri = GetUriFromFile("uri_toMod.txt"); // prendiamo l'endpoint dall'indirizzo scritto in questo file
        // string uri = "https://042a9edd89ce492e9540f89e13c5ac41.api.mockbin.io/"; // coordinata coronale a 0
        // string uri = "https://23c85749f1f14c999c5ed0aee82261b4.api.mockbin.io/"; // coordinata coronale a 100
        // Edit -> Project Settings -> Player -> Other Settings -> Configuration -> Allow downloads over HTTP
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();
            //if (request.isNetworkError || request.isHttpError)
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                outputArea.text = request.error;

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    outputArea.text += "-- NetworkError --";
                }
                else
                {
                    outputArea.text += "-- HttpError --";
                }
            }
            else
            {
                outputArea.text = "Connesso! ";
                // outputArea.text = counterGetInvocation.ToString() + request.downloadHandler.text;

                // string jsonMock = adjustJsonStringArray(request.downloadHandler.text); // read the txt from the json received 
                Debug.Log(request.downloadHandler.text);
                string jsonMock = request.downloadHandler.text; // < ------------testiamo che il parsing avvenga giusto quando prendiamo il json da online
                // string filePathMock = Path.Combine(Application.persistentDataPath, "coordinateGet.txt"); // TO REMOVE _ to test json parser
                // string jsonMock = File.ReadAllText(filePathMock); // TO REMOVE _ for test json parser

                Debug.Log("----------------------HO TROVATO IL FILE");
                Debug.Log(jsonMock);

                /*
                CoordinateInfo[] coordAll = JsonHelper.FromJson<CoordinateInfo>(jsonMock); // OLD METHOD
                int coordLastElem = coordAll.Length - 1;
                */
                RootObject coordAll = JsonUtility.FromJson<RootObject>(jsonMock);

                /* OLD JSON STRUCTURE
                float newCoordX = coordAll[coordLastElem].pianoCoronale;
                float newCoordY = coordAll[coordLastElem].pianoSagitale;
                float newCoordZ = coordAll[coordLastElem].pianoAssiale;
                */
                float newCoord_CoronalEv = coordAll.sliceCoordinates[1].Coronal;
                float newCoord_SagitalEv = coordAll.sliceCoordinates[0].Sagittal;
                float newCoord_AxialEv = coordAll.sliceCoordinates[2].Axial;

                Debug.Log("Coord Coronali - Zvr:" + newCoord_CoronalEv);
                Debug.Log("Coord Sagittali - Xvr:" + newCoord_SagitalEv);
                Debug.Log("Coord Assiali - Yvr:" + newCoord_AxialEv);

                // Check if the new coords are different from the previous one
                if ((newCoord_CoronalEv != lastCoord_CoronalEv) || (newCoord_SagitalEv != lastCoord_SagitalEv) || (newCoord_AxialEv != lastCoord_AxialEv))
                {
                    // we do have new coords
                    areNewCoord = true;

                    // conversion
                    float newCoronalCoordVr = convertCoordinate(newCoord_CoronalEv, coronal_max, coronal_min, z_maxVol_vr, z_minVol_vr);
                    float newAxialCoordVr = convertCoordinate(newCoord_AxialEv, axial_max, axial_min, y_maxVol_vr, y_minVol_vr);
                    float newSagitalCoordVr = convertCoordinate(newCoord_SagitalEv, sagital_max, sagital_min, x_maxVol_vr, x_minVol_vr);

                    // set the position of the object
                    pianoCoronaleZ.transform.position = new Vector3(pianoCoronaleZ.transform.position.x, pianoCoronaleZ.transform.position.y, newCoronalCoordVr);
                    pianoAssialeY.transform.position = new Vector3(pianoAssialeY.transform.position.x, newAxialCoordVr, pianoAssialeY.transform.position.z);
                    pianoSagitaleX.transform.position = new Vector3(newSagitalCoordVr, pianoSagitaleX.transform.position.y, pianoSagitaleX.transform.position.z);
                    audioSource.Play();

                    lastCoord_ZVr = newCoronalCoordVr;
                    lastCoord_YVr = newAxialCoordVr;
                    lastCoord_XVr = newSagitalCoordVr;

                    // update last received coordinate
                    lastCoord_CoronalEv = newCoord_CoronalEv;
                    lastCoord_SagitalEv = newCoord_SagitalEv;
                    lastCoord_AxialEv = newCoord_AxialEv;

                    outputArea.text = "\r\nUPDATE:" + counterGetInvocation + "\r\nCoord eV: " + lastCoord_CoronalEv + " - Coord VR: " + newCoronalCoordVr + "\r\n";
                }
                else
                {
                    areNewCoord = false;
                    outputArea.text += (counterGetInvocation +" - ");
                }
            }
        }

        isRequestInProgress = false; // Set flag to false when request ends
    }

    IEnumerator CallGetDataEverySecond()
    {
        while (true)
        {
            GetData();
            yield return new WaitForSeconds(1f); // Wait for # second
        }
    }

    private string adjustJsonStringArray(string jsonArray)
    {
        string adjJson = "{\r\n    \"Items\":" + jsonArray + "}";
        return adjJson;
    }

    private string GetUriFromFile(string fileName)
    {
        // BUILD per Window or Play mode
        // string filePath = Path.Combine(Application.persistentDataPath, fileName);

        // BUILD per Oculus
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        // https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html
        // Android: Application.persistentDataPath points to /storage/emulated/<userid>/Android/data/<packagename>/files on most devices (some older phones might point to location on SD card if present), the path is resolved using android.content.Context.getExternalFilesDir

        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError("URI file not found!");
            return string.Empty;
        }
    }

    private float convertCoordinate(float receivedCoordEv, float maxCoordEv, float minCoordEv, float maxCoordVr, float minCoordVr)
    {
        float proportionFactor = (maxCoordVr - minCoordVr) / (maxCoordEv - minCoordEv);
        Debug.Log("Conversion Factor: " + proportionFactor);
        float result = minCoordVr + proportionFactor * (receivedCoordEv - minCoordEv);

        Debug.Log("------> Coord ricevuta eV: " + receivedCoordEv.ToString() + " - Coord convertita VR: " + result.ToString());

        return result;
    }

    void CalculateVolumeInfo(GameObject volume)
    {
        Bounds bounds = new Bounds();

        Renderer renderer = volume.GetComponent<Renderer>();
        if (renderer != null)
        {
            bounds = renderer.bounds;
        }
        else
        {
            Collider collider = volume.GetComponent<Collider>();
            if (collider != null)
            {
                bounds = collider.bounds;
            }
            else
            {
                Debug.LogWarning("No Renderer or Collider found on the object.");
                return;
            }
        }

        // Calculate and store the size and coordinates
        x_minVol_vr = bounds.min.x;
        x_maxVol_vr = bounds.max.x;

        y_minVol_vr = bounds.min.y;
        y_maxVol_vr = bounds.max.y;

        z_minVol_vr = bounds.min.z;
        z_maxVol_vr = bounds.max.z;

        // Print the values to the console for verification
        Debug.Log("X Min: " + x_minVol_vr);
        Debug.Log("X Max: " + x_maxVol_vr);

        Debug.Log("Y Min: " + y_minVol_vr);
        Debug.Log("Y Max: " + y_maxVol_vr);

        Debug.Log("Z Min: " + z_minVol_vr);
        Debug.Log("Z Max: " + z_maxVol_vr);
    }

    public void ResetPlaneCoord()
    {
        pianoCoronaleZ.transform.position = new Vector3(pianoCoronaleZ.transform.position.x, pianoCoronaleZ.transform.position.y, z_minVol_vr);
        pianoAssialeY.transform.position = new Vector3(pianoAssialeY.transform.position.x, y_maxVol_vr, pianoAssialeY.transform.position.z);
        pianoSagitaleX.transform.position = new Vector3(x_maxVol_vr, pianoSagitaleX.transform.position.y, pianoSagitaleX.transform.position.z);

        lastCoord_ZVr = z_minVol_vr;
        lastCoord_YVr = y_maxVol_vr;
        lastCoord_XVr = x_maxVol_vr;
    }

    public void SeeOnlyRed()
    {
        pianoCoronaleZ.transform.position = new Vector3(pianoCoronaleZ.transform.position.x, pianoCoronaleZ.transform.position.y, z_minVol_vr);
        pianoSagitaleX.transform.position = new Vector3(x_maxVol_vr, pianoSagitaleX.transform.position.y, pianoSagitaleX.transform.position.z);

        lastCoord_ZVr = z_minVol_vr;
        lastCoord_XVr = x_maxVol_vr;
    }

    public void SeeOnlyGreen()
    {
        //pianoCoronaleZ.transform.position = new Vector3(pianoCoronaleZ.transform.position.x, pianoCoronaleZ.transform.position.y, z_minVol_vr);
        pianoAssialeY.transform.position = new Vector3(pianoAssialeY.transform.position.x, y_maxVol_vr, pianoAssialeY.transform.position.z);
        pianoSagitaleX.transform.position = new Vector3(x_maxVol_vr, pianoSagitaleX.transform.position.y, pianoSagitaleX.transform.position.z);

        //lastCoord_ZVr = z_minVol_vr;
        lastCoord_YVr = y_maxVol_vr;
        lastCoord_XVr = x_maxVol_vr;
    }

    public void SeeOnlyBlue()
    {
        pianoCoronaleZ.transform.position = new Vector3(pianoCoronaleZ.transform.position.x, pianoCoronaleZ.transform.position.y, z_minVol_vr);
        pianoAssialeY.transform.position = new Vector3(pianoAssialeY.transform.position.x, y_maxVol_vr, pianoAssialeY.transform.position.z);
        //pianoSagitaleX.transform.position = new Vector3(x_maxVol_vr, pianoSagitaleX.transform.position.y, pianoSagitaleX.transform.position.z);

        lastCoord_ZVr = z_minVol_vr;
        lastCoord_YVr = y_maxVol_vr;
        //lastCoord_XVr = x_maxVol_vr;
    }
}