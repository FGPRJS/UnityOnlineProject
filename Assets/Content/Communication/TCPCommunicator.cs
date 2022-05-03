using UnityEngine;

namespace Content.Communication
{
    public class TCPCommunicator : MonoBehaviour
    {
        public static TCPCommunicator instance;

        public string ip = "127.0.0.1";
        public int port = 8080;
        private Client _client;

        private void Awake()
        {
            _client = new Client(ip, port);
            //Singleton
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("Instance already exists");
                Destroy(this);
            }
        }

        public void ConnectToServer()
        {
            _client.Connect();
        }
    }
}
