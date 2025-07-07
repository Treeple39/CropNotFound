using Assets._Project.Scripts.EnemyAnim;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]public Rigidbody2D rb {  get; private set; }
    public Animator anim { get; private set; }

    public BaseMovement movement { get; private set; }

    public EnemyStateMachine stateMachine { get; private set; }

    #region AnimationSet
    public EnemyIdleState idleState { get; private set; }
    public EnemyDashState dashState { get; private set; }
    public EnemyFleeState fleeState { get; private set; }

    public EnemyDieState dieState { get; private set; }
    public EnemyShineState shineState { get; private set; }
    public EnemyDizzState dizzState { get; private set; }
    #endregion

    public bool dead;

    private void OnEnable()
    {
        EventHandler.OnRarityChanged += OnRarityChangedEvent;
        EventHandler.OnRarityUpgraded += OnRarityUpgradedEvent;
    }


    private void OnDisable()
    {
        EventHandler.OnRarityChanged -= OnRarityChangedEvent;
        EventHandler.OnRarityUpgraded -= OnRarityUpgradedEvent;
    }

    [SerializeField] private EnemyData enemyData;

    [Header("œ°”–∂»≈‰÷√")]
    #region RaritySet
    [SerializeField] private RarityDatabase rarityDatabase;
    [SerializeField] private Renderer[] outlineRenderers;

    private Rarity _currentRarity;
    private MaterialPropertyBlock _propBlock;

    [HideInInspector] public bool canChangeRarity;

    private void OnRarityChangedEvent(Rarity rarity)
    {
        if(!canChangeRarity)
            return;
        canChangeRarity = false;
        ResetRarity(rarity);
        UpdateRarityVisuals();
        UpdateBigCoinCount();
    }

    private void OnRarityUpgradedEvent(int num, float rate)
    {
        if (!canChangeRarity)
            return;
        canChangeRarity = false;
        UpgradeRarity(num, rate);
        UpdateRarityVisuals();
        UpdateBigCoinCount();
    }

    private void UpdateRarityVisuals()
    {
        var rarityData = rarityDatabase.GetRarityData(_currentRarity);    

        foreach (var renderer in outlineRenderers)
        {
            renderer.material.SetColor("_OutlineColor", rarityData.outlineColor);
            //renderer.material.SetFloat("_OutlineWidth", rarityData.outlineWidth);
        }
    }

    private void UpdateBigCoinCount()
    {
        enemyData.BigCoinCount += (int)GetStatMultiplier();
    }

    public void UpgradeRarity(int num, float rate)
    {
        if (_currentRarity < Rarity.Legendary)
        {
            if (Random.value < rate)
            {
                _currentRarity += num;
                enemyData.Rarity += num;
            }
        }
    }

    public void ResetRarity(Rarity rarity)
    {
        _currentRarity = rarity;
        enemyData.Rarity = rarity;
    }

    public float GetStatMultiplier()
    {
        return rarityDatabase.GetRarityData(_currentRarity).statMultiplier;
    }
    #endregion

    public virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        anim = GetComponent<Animator>();
        movement = GetComponent<BaseMovement>();
        enemyData = GetComponent<EnemyData>();

        idleState = new EnemyIdleState(stateMachine, this, "Idle");
        dashState = new EnemyDashState(stateMachine, this, "Dash");
        fleeState = new EnemyFleeState(stateMachine, this, "Flee");
        dieState = new EnemyDieState(stateMachine, this, "Die");
        shineState = new EnemyShineState(stateMachine, this, "Shine");
        dizzState = new EnemyDizzState(stateMachine, this, "Dizz");
        stateMachine.Initialize(idleState);
    }

    public virtual void Die()
    {
        stateMachine.ChangeState(dieState);
    }

}
