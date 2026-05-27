using System;
using UnityEngine;

public class SpellBookItem : MonoBehaviour
{
    [SerializeField] private SpellStats spellStats;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SpellBook>())
        {
            var spellBook = other.GetComponent<SpellBook>();
            spellBook.spellStats = spellStats;
            spellBook.AssignStats();
            Destroy(gameObject);
        }
        
    }
}
