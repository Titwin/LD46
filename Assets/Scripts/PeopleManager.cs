using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPerson
{
    bool Active
    {
        get;
        set;
    }
    bool Alive
    {
        get;
    }
    Vector2 Position
    {
        get;
    }
    void Sense();
    void Think();
   
    void Act();
}
public class PeopleManager : MonoBehaviour
{
    public List<IPerson> people = new List<IPerson>();
    public Person[] personTemplate;
    public Ghoul[] ghoulTemplate;
    public List<Vector3> toSpawn;

    public float maxUpdateDistance = 25;
    [Range(0,1)]public float density;
    private void Start()
    {
        foreach (var cell in Map.map.Keys)
        {
            if(Map.map[cell].type == MapTile.Type.Walk)
            {
                if (Random.value < density)
                {
                    AddPerson(Map.GetWorldPosition(cell));
                }
            }
        }
    }
    IPerson AddPerson(Vector2 position)
    {
        IPerson p;
        if (Random.value < 0.1f)
        {
            Ghoul pg = Instantiate<Ghoul>(ghoulTemplate[Random.Range(0, ghoulTemplate.Length)]);
            pg.transform.position = position;
            pg.transform.parent = this.transform;
            pg.manager = this;
            p = pg;
        }
        else
        {
            Person pp = Instantiate<Person>(personTemplate[Random.Range(0,personTemplate.Length)]);
            pp.transform.position = position;
            pp.transform.parent = this.transform;
            pp.manager = this;
            p = pp;
        }
        
        people.Add(p);
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
                person.Active = false;
                toSpawn.RemoveAt(pos);
            }
        }
        for (int i = people.Count-1;i >=0;--i)
        {
            IPerson p = people[i];
            float d = Vector2.Distance(playerPosition, p.Position);
            {
                if (p.Active && d > maxUpdateDistance * 1.1f)
                {
                    p.Active = false;
                }
                else if (!p.Active && d < maxUpdateDistance)
                {
                    p.Active = true;
                }
            }
            if (p.Active && p.Alive)
            {
                p.Sense();
                p.Think();
                p.Act();
            }
        }

    }
    public void OnDied(IPerson p)
    {
        people.Remove(p);
        toSpawn.Add(p.Position);
    }
}
