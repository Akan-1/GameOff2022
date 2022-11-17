using System.Collections; 
using System.Collections.Generic;
using UnityEngine; 
public class door:MonoBehavior {
    [SerializeField] private GameObject doorGameObject;
    private IDoor door; 
    private float timer; 

    private void awake() { 
        door=doorGameObject.GetComponent<IDoor>(); 
    }
    private void update (){ 
        if(timer>0){ 
            timer-=Time.deltaTime; 
            if (time <= 0f){
                door.CloseDoor(); 
            }
        }
    }

   private void OnTriggerEnter2D(Collider2D collider){
      if ( collider.GetComponent<CharaacterConter2D>()!=null) or (collider.GetComponent<rigibody>()!=null);
      { 
       door.OpenDoor();
       timer= 1f;
   }
  }
}


