using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    public ParticleSystem blood;
    // Start is called before the first frame update

    public static FXManager instance;
    private void Awake()
    {
        instance = this;
    }

    public void EmitBlood(Vector2 position, Vector2 velocity,int amount)
    {
        blood.Emit(position, velocity,1,1,Color.red);
    }
}
