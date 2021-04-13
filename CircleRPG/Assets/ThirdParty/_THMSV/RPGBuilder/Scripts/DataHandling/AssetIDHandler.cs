
    [System.Serializable]
    public class AssetIDHandler
    {
        public enum ASSET_TYPE_ID
        {
            ability,
            effect,
            npc,
            treePoint,
            item,
            skill,
            levelTemplate,
            race,
            _class,
            lootTable,
            talentTree,
            worldPosition,
            stat,
            merchantTable,
            currency,
            task,
            quest,
            craftingRecipe,
            craftingStation,
            resourceNode,
            bonus,
            abilityRank,
            recipeRank,
            resourceNodeRank,
            bonusRank,
            gameScene
        }

        public ASSET_TYPE_ID assetType;
        public int id;

        public AssetIDHandler(ASSET_TYPE_ID _assetType, int _id)
        {
            assetType = _assetType;
            id = _id;
        }
    }
