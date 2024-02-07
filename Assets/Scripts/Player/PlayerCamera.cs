using UnityEngine;
using Cinemachine;

namespace Player {
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class PlayerCamera : MonoBehaviour
    {
        // Make sure this Gameobject has a CinemachineVirtualCamera component
        CinemachineVirtualCamera vcam;
        // Start is called before the first frame update
        void Awake()
        {
            vcam = GetComponent<CinemachineVirtualCamera>();
            if (this.transform.parent != null)
            {
                vcam.Follow = this.transform.parent;
            }
        }
    }
}
