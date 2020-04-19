using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleManager : MonoBehaviour
{
    public List<Person> people;
    public Person[] personTemplate;
    public List<Vector3> toSpawn;

    public float maxUpdateDistance = 25;
    private void Start()
    {
        foreach (var cell in Map.map.Keys)
        {
            if(Map.map[cell].type == MapTile.Type.Walk)
            {

                AddPerson(Map.GetWorldPosition(cell));
            }
        }
    }
    Person AddPerson(Vector2 position)
    {
        Person p = Instantiate<Person>(personTemplate[Random.Range(0,personTemplate.Length)]);
        p.transform.position = position;
        people.Add(p);
        p.transform.parent = this.transform;
        p.manager = this;
        return p;
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 playerPosition = Player.main.position;
        for (int pos = toSpawn.Count - 1; pos >= 0; --pos)
        {
            float d = Vector3.Distance(playerPosition, toSpawn[pos]);
            if (d > maxUpdateDistance * 1.1f)
            {
                var person = AddPerson(toSpawn[pos]);
                person.gameObject.SetActive(false);
                toSpawn.RemoveAt(pos);
            }
        }
        foreach (Person p in people)
        {
            float d = Vector2.Distance(playerPosition, p.rb.position);
            {
                if (p.gameObject.activeSelf && d > maxUpdateDistance * 1.1f)
                {
                    p.gameObject.SetActive(false);
                }
                else if (!p.gameObject.activeSelf && d < maxUpdateDistance)
                {
                    p.gameObject.SetActive(true);
                }
            }
            if (p.alive)
            {
                p.Sense();
                p.Think();
                p.Act();
            }
        }

    }
    public void OnDied(Person p)
    {
        people.Remove(p);
        toSpawn.Add(p.transform.position);
    }
}
