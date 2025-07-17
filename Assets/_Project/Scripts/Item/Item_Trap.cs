using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item_Trap : MonoBehaviour
{
    [SerializeField]
    private SkeletonAnimation sAnim;
    [SerializeField]
    private Collider2D coll2D;

    public GameObject fxPfb;
    public GameObject fxItem;
    public float EffectiveTime;

    private bool trapped;

    public void TrapStart()
    {
        sAnim.state.SetAnimation(0, "start", false);
        sAnim.state.AddAnimation(0, "idle", true, 0);
    }

    private void Start()
    {
        TrapStart();
    }

    public void SetEmpty()
    {
        trapped = false;
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (trapped || !coll.CompareTag("Enemy"))
        {
            return;
        }

        trapped = true;

        if (coll.GetComponent<EnemyData>())
        {
            CoinManager.Instance.CreateDeadCoin(coll.transform.position, coll.GetComponent<EnemyData>().BigCoinCount);
            Score.itemCount++;
            Destroy(coll.gameObject);
            Out();
        }
    }

    private void DestoryAtTimeOff()
    {
        if (EffectiveTime > 0)
        {
            EffectiveTime -= Time.deltaTime;
        }
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        DestoryAtTimeOff();
    }

    public void Out()
    {
        sAnim.state.SetAnimation(0, "spell", false);
        sAnim.AnimationState.Complete += (t) =>
        {
            sAnim.state.SetAnimation(0, "end", false);
        };
    }

    public void SetFxPfb()
    {
        Instantiate(fxPfb, this.transform.position, Quaternion.identity, this.transform);
    }

    public void SetFxItem()
    {
        Instantiate(fxItem, this.transform.position, Quaternion.identity, this.transform);
    }

}
