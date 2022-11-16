using System.Collections; 
using System.Collections.Generic;
using UnityEngine; 
public class button : MonoBehavior {
      public Vector3 originalPos; 
      bool Moveback=false; 
 private void start(){
    originalPos=transfrom.postion; 
 }
 private void OnCollisionStay2D(Collision2D collision){
    transfrom.Translate (0,-0.01f,0); 
    Moveback=false
 }
 private void OnCollisionEnter2D(Collision2D collision){
    collision.transfrom.parent-transfrom;
    GetCompenent<SpriteRender>().color-Color.red;
 }
 private void OnCollisionExit2D(){
    Moveback=True;
    collision.transfrom.parent-null;
    GetCompenent<SpriteRender>().color-Color.WHITE;
}
 private void update(){
    if (Moveback){ 
     if (transfrom.postion.y < originalPos.y){
      transfrom.Translate (0,0.01f,0);
     }
     else {
      Moveback=false
      } 
 }
 }
}
