using JFM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFM
{
    public class VillageChaosConfig : MonoBehaviour
    {

        //Faire une autre liste de gameobject a desactiver dependant du niveau de chaos
        public ObjectList[] activeObjects = new ObjectList[ChaosOrderSystem.maxChaosAmount];

        //Faire une liste de gameobject pour les gameobject dans le village qui pourrais dependant du niveau du chaos changer de couleur
        public List<GameObject> colorChangingObjects = new List<GameObject>();
        public Color[] colors = new Color[ChaosOrderSystem.maxChaosAmount];

        //Faire en sorte que si on gameover tout ce reactive et revienne comme couleur normal
        //Faire en sorte de changer le fullscreenpass material dependant du shader \


      }
}