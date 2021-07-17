using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SniperRenderTexture : MonoBehaviourPunCallbacks
{
    [SerializeField] private Camera cam;
    [SerializeField] private RenderTexture sniperRenderTexture;


    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            cam.targetTexture = sniperRenderTexture;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
