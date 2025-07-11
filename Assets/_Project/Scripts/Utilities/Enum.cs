
public enum ItemType
{
    Clock,Place,Powder,Furniture,Potion,Fruit,
    CaptureTool,AttackTool,MoveTool,
}

public enum EnemyType
{
    Doll, Slippers, Chair, Bottle, Pillow, Book,
}

public enum SkillType
{
    Unlock, ValueAdd, ItemAdd,
}

public enum SlotType
{

}

public enum Rarity
{
    Common,     // 普通
    Uncommon,   // 稀有
    Rare,       // 罕见
    Epic,       // 史诗
    Legendary   // 传说
}

public enum TechLevelUnlockEventType
{
    Default,
    UnlockMonster,   // 开怪物
    UnlockItem,      // 开道具
    PlayStory,       // 开剧情
    UnlockSkill      // 开技能（之后设计）
}

public enum InventoryLocation
{
    Player,Box
}

public enum UFX_UIPic
{
    UFX_Fade,
    UFX_Stretch,
    UFX_Color,
}
