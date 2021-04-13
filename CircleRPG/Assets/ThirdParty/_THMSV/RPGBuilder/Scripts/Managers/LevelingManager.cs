using THMSV.RPGBuilder.LogicMono;
using UnityEngine;

namespace THMSV.RPGBuilder.Managers
{
    public class LevelingManager : MonoBehaviour
    {
        public static LevelingManager Instance { get; private set; }
        [SerializeField] private GameObject levelUpGO;
        
        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }


        public void GenerateMobEXP(RPGNpc npcDATA, CombatNode nodeRef)
        {
            var EXP = Random.Range(npcDATA.MinEXP, npcDATA.MaxEXP);
            EXP += npcDATA.EXPBonusPerLevel * nodeRef.NPCLevel;
            
            AddClassXP(EXP);
        }

        public void GenerateSkillEXP(int skillID, int Amount)
        {
            AddSkillXP(skillID, Amount);
        }

        public void AddSkillLevel(int skillID, int _amount)
        {
            var skillREF = RPGBuilderUtilities.GetSkillFromID(skillID);
            var levelTemplateREF = RPGBuilderUtilities.GetLevelTemplateFromID(skillREF.levelTemplateID);
            foreach (var t in CharacterData.Instance.skillsDATA)
            {
                t.currentSkillXP = 0;
                t.currentSkillLevel += _amount;
                t.maxSkillXP = levelTemplateREF
                    .allLevels[t.currentSkillLevel - 1].XPRequired;

                // EXECUTE POINTS GAIN REQUIREMENTS
                CharacterEventsManager.Instance.SkillLevelUp(skillREF.ID);
            }
        }

        public void AddClassLevel(int _amount)
        {
            CharacterData.Instance.classDATA.currentClassXP = 0;
            CharacterData.Instance.classDATA.currentClassLevel += _amount;
            CharacterData.Instance.classDATA.maxClassXP = RPGBuilderUtilities
                .GetLevelTemplateFromID(RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID)
                    .levelTemplateID).allLevels[CharacterData.Instance.classDATA.currentClassLevel - 1].XPRequired;

            CharacterEventsManager.Instance.ClassLevelUp();
        }

        void SpawnLevelUpGO()
        {
            GameObject lvlUpGo = Instantiate(levelUpGO, CombatManager.playerCombatInfo.transform.position,
                Quaternion.identity);
            lvlUpGo.transform.SetParent(CombatManager.playerCombatInfo.transform);
            Destroy(lvlUpGo, 5);
        }
        
        public void AddClassXP(int _amount)
        {
            if (CharacterData.Instance.classDATA.currentClassLevel == RPGBuilderUtilities
                .GetLevelTemplateFromID(RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID)
                    .levelTemplateID).levels) return;
            
            float totalAmt = _amount;
            float EXPMOD = CombatManager.Instance.GetTotalOfStatType(CombatManager.playerCombatInfo,
                RPGStat.STAT_TYPE.EXPERIENCE_MODIFIER);
            if (EXPMOD > 0) totalAmt += totalAmt * (EXPMOD / 100);
            
            ScreenTextDisplayManager.Instance.ScreenEventHandler("EXP", "Character: " + (int)totalAmt,
                CombatManager.playerCombatInfo.gameObject);
            
            while (totalAmt > 0)
            {
                var XPRemaining = CharacterData.Instance.classDATA.maxClassXP -
                                  CharacterData.Instance.classDATA.currentClassXP;
                if (totalAmt > XPRemaining)
                {
                    CharacterData.Instance.classDATA.currentClassXP = 0;
                    totalAmt -= XPRemaining;
                    CharacterData.Instance.classDATA.currentClassLevel++;
                    CharacterData.Instance.classDATA.maxClassXP = RPGBuilderUtilities
                        .GetLevelTemplateFromID(RPGBuilderUtilities.GetClassFromID(CharacterData.Instance.classDATA.classID)
                            .levelTemplateID).allLevels[CharacterData.Instance.classDATA.currentClassLevel - 1].XPRequired;

                    // EXECUTE POINTS GAIN REQUIREMENTS
                    CharacterEventsManager.Instance.ClassLevelUp();
                    if (levelUpGO != null)
                    {
                        SpawnLevelUpGO();
                    }
                }
                else
                {
                    CharacterData.Instance.classDATA.currentClassXP += (int)totalAmt;
                    totalAmt = 0;
                    if (CharacterData.Instance.classDATA.currentClassXP !=
                        CharacterData.Instance.classDATA.maxClassXP) continue;
                    CharacterData.Instance.classDATA.currentClassLevel++;
                    CharacterData.Instance.classDATA.currentClassXP = 0;
                    CharacterData.Instance.classDATA.maxClassXP = RPGBuilderUtilities
                        .GetLevelTemplateFromID(RPGBuilderUtilities
                            .GetClassFromID(CharacterData.Instance.classDATA.classID).levelTemplateID)
                        .allLevels[CharacterData.Instance.classDATA.currentClassLevel - 1].XPRequired;

                    // EXECUTE POINTS GAIN REQUIREMENTS
                    CharacterEventsManager.Instance.ClassLevelUp();
                    if (levelUpGO != null)
                    {
                        SpawnLevelUpGO();
                    }
                }
            }

            CombatManager.Instance.EXPBarUpdate();
        }

        public void AddSkillXP(int skillID, int _amount)
        {
            foreach (var t in CharacterData.Instance.skillsDATA)
                if (t.skillID == skillID)
                {
                    var skillREF = RPGBuilderUtilities.GetSkillFromID(skillID);
                    float totalAmt = _amount;
                    
                    float EXPMOD = CombatManager.Instance.GetTotalOfStatType(CombatManager.playerCombatInfo,
                        RPGStat.STAT_TYPE.EXPERIENCE_MODIFIER);
                    if (EXPMOD > 0) totalAmt += totalAmt * (EXPMOD / 100);
                    
                    ScreenTextDisplayManager.Instance.ScreenEventHandler("EXP",
                        RPGBuilderUtilities.GetSkillFromID(skillID)._name + ": + " + (int)totalAmt,
                        CombatManager.playerCombatInfo.gameObject);
                    
                    while (totalAmt > 0)
                    {
                        var XPRemaining = t.maxSkillXP -
                                          t.currentSkillXP;
                        var levelTemplateREF = RPGBuilderUtilities.GetLevelTemplateFromID(skillREF.levelTemplateID);
                        if (totalAmt > XPRemaining)
                        {
                            t.currentSkillXP = 0;
                            totalAmt -= XPRemaining;
                            t.currentSkillLevel++;
                            t.maxSkillXP = levelTemplateREF
                                .allLevels[t.currentSkillLevel - 1].XPRequired;

                            // EXECUTE POINTS GAIN REQUIREMENTS
                            CharacterEventsManager.Instance.SkillLevelUp(skillREF.ID);
                        }
                        else
                        {
                            t.currentSkillXP += (int)totalAmt;
                            totalAmt = 0;
                            if (t.currentSkillXP != t.maxSkillXP) continue;
                            t.currentSkillLevel++;
                            t.currentSkillXP = 0;
                            t.maxSkillXP = levelTemplateREF
                                .allLevels[t.currentSkillLevel - 1].XPRequired;

                            // EXECUTE POINTS GAIN REQUIREMENTS
                            CharacterEventsManager.Instance.SkillLevelUp(skillREF.ID);
                        }
                    }
                }
        }
    }
}