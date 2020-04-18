using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleManager : MonoBehaviour
{
    public List<Person> people;
    public Person personTemplate;

    private void Start()
    {
        for (int i = 0; i < 100; ++i)
        {
            Person p = Instantiate<Person>(personTemplate);
            people.Add(p);
            p.transform.parent = this.transform;
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
