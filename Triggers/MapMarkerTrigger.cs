using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMarkerTrigger : Triggerable 
{
    public List<string> entityKeys;
    int frame = 0;

    void Update()
    {
        if (Game.isReady == false)
            return;

        frame++;
        if (frame == 3)
        {
            foreach (var key in entityKeys)
            {
                //if (EntityManager.Get(key) == null)
                {
                    Debug.LogWarningFormat("Map marker {0} could not be found", key);
                }
            }
        }
    }

    override protected void OnTrigger()
    {
        saveState = SaveData.EntitySaveState.Triggered;
        /*
        save.UnsetMarkers();

        foreach (var key in entityKeys)
        {
            save.SetMarker(key);
        }
        */

        var first = entityKeys[0];
        //var entity = EntityManager.Get(first);
        //SequenceManager.main.ShowMap(entity, OnShowMapComplete);
    }

    void OnShowMapComplete()
    {
        //Saver.main.Save();
    }
}
