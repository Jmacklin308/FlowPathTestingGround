using UnityEngine;

public class Player : MonoBehaviour
{
    public EnemyManager enemyManager;
    
    // Start is called before the first frame update
    void Awake()
    {
        //find the enemy manager
        enemyManager = GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>();
        
        //add the player to the playerlist
        enemyManager.listOfPlayers.Add(this);
        
    }
}
