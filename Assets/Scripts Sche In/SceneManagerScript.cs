using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PhotonNetwork.ConnectUsingSettings())
        {
            PhotonNetwork.Disconnect();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

   
}
