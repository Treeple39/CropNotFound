using UnityEngine;



[CreateAssetMenu(fileName = "ShopCurrency_SO", menuName = "PlayerData/ShopCurrency_SO")]
public class ShopCurrency_SO : ScriptableObject, IInitializableSO
{
    public int coins;

    public void InitDefault()
    {
        coins = 0;
    }
}