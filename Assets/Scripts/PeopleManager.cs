using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleManager : MonoBehaviour
{
    public List<Person> people;

    // Update is called once per frame
    void Update()
    {
        foreach(Person p in people)
        {
            p.Think();
            p.Act();
        }
    }
}
