using UnityEngine;

namespace Chess2D
{
    public class PersistentManager : MonoBehaviour
    {
        private static PersistentManager _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}