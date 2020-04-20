using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    public ParticleSystem blood;
    public SpriteRenderer[] bloodDecal;
    public List<SpriteRenderer> decals = new List<SpriteRenderer>();
    // Start is called before the first frame update

    public static FXManager instance;
    private void Awake()
    {
        instance = this;
    }

    public void EmitBlood(Vector2 position, Vector2 velocity,int amount)
    {
        var emitParams = new ParticleSystem.EmitParams();
        emitParams.velocity = velocity;
        emitParams.ResetStartSize();
        emitParams.position = position;
        Vector3 baseVelocity = velocity!=Vector2.zero?velocity.normalized: Vector2.zero;
        for (int i = 0; i < amount; ++i)
        {
           baseVelocity = baseVelocity + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f))*velocity.magnitude*0.25f;
           emitParams.velocity = baseVelocity;
           blood.Emit(emitParams, amount);
        }
    }
    public void EmitBloodStain(Vector2 position)
    {
        if (decals.Count > 4096)
        {
            var d = decals[0];
            Destroy(d.gameObject);
            decals.RemoveAt(0);
        }
        var stain = Instantiate<SpriteRenderer>(bloodDecal[Random.Range(0, bloodDecal.Length)], position, Quaternion.Euler(0, 0, Random.Range(0, 4) * 90), this.transform);
        decals.Add(stain);
    }
}
