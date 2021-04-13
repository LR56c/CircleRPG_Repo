using System;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Create RPG Builder Editor DATA", menuName = "RPG BUILDER/RPG Builder")]
public class RPGBuilderEditorDATA : ScriptableObject
{
    public enum ThemeTypes
    {
        Dark,
        Light
    }

    public ThemeTypes curEditorTheme;

    public bool increasedEditorUpdate;
    
    public float CategoriesX, CategoriesY;
    public float CategoriesLeftMargin, CategoriesTopMargin;
    public float SubCategoriesX, SubCategoriesY;
    public float SubCategoriesLeftMargin, SubCategoriesTopMargin;
    public float searchSlotX, searchSlotY;
    public float searchSlotLeftMargin, searchSlotTopMargin;
    public float searchViewX, searchViewY;
    public float ViewX, ViewY;
    public float ViewLeftMargin, ViewTopMargin;
    public float FullViewX, FullViewY;
    public float FullViewLeftMargin, FullViewTopMargin;
    public float actionButtonsX, actionButtonsY;
    public float actionButtonsLeftMargin, actionButtonsTopMargin;

    public float abilityListYMargin,
        baseAbilityElementHeight,
        abilityRequirementHeightTier1,
        abilityRequirementHeightTier2,
        abilityRequirementHeightTier3;

    public Texture2D DarkThemeBackground1, DarkThemeBackground2, LightThemeBackground1, LightThemeBackground2, HoverTexture;
    public Texture2D searchIcon;
    public Sprite defaultElementIcon;
    public Texture2D abilityElementBackground,
        abilityRequirementElementBackground,
        abilityNullSprite,
        abilitySeparationBackground;

    public Font defaultFont;

    public List<GameObject> polytopeStudioAssets_GO = new List<GameObject>();
    public List<AudioClip> cafofoAssets_AUDIO = new List<AudioClip>();
    public List<GameObject> GabbrielAguiarAsset_GO = new List<GameObject>();
    public List<GameObject> RDRAssets_GO = new List<GameObject>();
    public List<Sprite> RDRAssets_SPRITE = new List<Sprite>();
    public List<GameObject> TitanForgeAssets_GO = new List<GameObject>();
    public List<Sprite> PONETIAssets_SPRITE = new List<Sprite>();
    public List<GameObject> MalbersAnimationAssets_GO = new List<GameObject>();

    public Sprite polytopePartnerImage,
        cafofoPartnerImage,
        GabrielAguiarPartnerImage,
        RDRPartnerImage,
        TitanForgePartnerImage,
        PONETIPartnerImage,
        MalbersAnimationParterImage;

    public enum ScalingType
    {
        none,
        width,
        height
    }


    public enum PanelType
    {
        search,
        view
    }

    [Serializable]
    public class ContainersDATA
    {
        public string containerName;
        public Rect containerRect;

        public ScalingType widthScalingType, heightScalingType;

        public bool Draw = true;
    }

    public ContainersDATA[] containersData;


    [Serializable]
    public class CategoriesDATA
    {
        public string CategoryName;

        [Serializable]
        public class SubCategoriesDATA
        {
            public string SubCategoryName;
            public bool Active = true;

            [Serializable]
            public class ContainersDATA
            {
                public string containerName;
                public Rect containerRect;

                public ScalingType widthScalingType, heightScalingType;
                public PanelType panelType;
                public bool Draw = true;
            }

            public ContainersDATA[] containersData;
        }

        public SubCategoriesDATA[] subCategoriesData;

        public bool Active = true;
    }

    public CategoriesDATA[] categoriesData;
}