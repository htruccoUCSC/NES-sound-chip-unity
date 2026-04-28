using UnityEngine;

public class Music : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OSCHandler.Instance.Init ();
        
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/music", 1);
        OSCHandler.Instance.UpdateLogs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
