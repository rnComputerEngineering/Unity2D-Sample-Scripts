using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RECircle : Enemy
{
    // Start is called before the first frame update
    //Seems empty because most of the work is done in the inherited Enemy Class
    protected override void FixedUpdate()
    {
        if (hasSight)
        {
            GoToPlayerDirectly();
        }
        else 
        {
            GetAStarPathToPlayer();
            GoToAStarPath();
        }
    }
}