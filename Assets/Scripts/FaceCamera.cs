using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FaceCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		transform.rotation = Camera.main.transform.rotation;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
