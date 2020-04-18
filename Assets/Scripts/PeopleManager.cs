using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleManager : MonoBehaviour
{
    public List<Person> people;
    public Person personTemplate;

    private void Start()
    {
        foreach (var cell in Map.map.Keys)
        {
            if(Map.map[cell].type == MapTile.Type.Walk)
            {
                
                Person p = Instantiate<Person>(personTemplate);
                p.transform.position = Map.GetWorldPosition(cell);
                people.Add(p);
                p.transform.parent = this.transform;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        foreach(Person p in people)
        {
            if (p.alive)
            {
                p.Sense();
                p.Think();
                p.Act();
            }
        }
    }
}
