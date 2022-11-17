using System.Collections; 
using System.Collections.Generic;
using UnityEngine; 
public class box :MonoBehavior 
{   
    [SerializeField]
    private float forcevalue;
    void start(){

    }
    void update(){

    }
   private void OnControllerColliderHit (ControllerColliderHit hit){
       Rigidbody rigidbody = hit.collider.attachedRigidbody; 

    if (rigidbody!=null){
        Vector3 forceDirection=hit.gameObject.transfrom.postion- transfrom.postion;
        forceDiection.Normalize();
        rigidbody.AddForceAtPosition(forceDirection*forcevalue,transfrom.postion, ForceMode.Impulse);
    }
} 
 

}