using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Staff : MonoBehaviour
{
    [SerializeField]
    private SkeletonAnimation sAnim;
    [SerializeField]
    private CircleCollider2D coll2D;

    public GameObject fxPfb;
    public GameObject fxItem;
    public int round;

    [SerializeField]
    private float attractionForce = 10f;
    public void StaffStart()
    {
        sAnim.state.SetAnimation(0, "start", false);
        sAnim.state.AddAnimation(0, "spell", true, 0);
    }

    private void Start()
    {
        StaffStart();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        Rigidbody2D rb = coll.GetComponent<Rigidbody2D>();
        if(coll.GetComponent<Enemy>())
            coll.GetComponent<Enemy>().canChangeRarity = true;

        if (rb != null)
        {
            Vector2 direction = (transform.position - coll.transform.position);

            rb.AddForce(direction.normalized * attractionForce, ForceMode2D.Impulse);

            if (fxPfb != null)
            {
                Instantiate(fxItem, coll.transform.position, Quaternion.identity);
            }
        } 
    }


    public void SetFX()
    {
        if (round <= 0)
        {
            sAnim.state.AddAnimation(0, "end", false, 0);
            return;
        }

        Instantiate(fxPfb, this.transform.position, Quaternion.identity, this.transform);
        round--;
        EventHandler.CallRarityUpgrade(1, 0.3f);
        StartCoroutine(Attract());
    }

    IEnumerator Attract()
    {
        coll2D.enabled = true;
        yield return new WaitForSecondsRealtime(0.5f);
        coll2D.enabled = false;
    }
}
