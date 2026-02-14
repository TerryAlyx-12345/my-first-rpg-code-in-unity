using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;


    public Dash_Skill dash {  get; private set; }
    public Clone_Skill clone { get; private set; }
    public Sword_Skill sword { get; private set; }
    public Blackhole_Skill blackhole { get; private set; }
    public Crystal_Skill crystal { get; private set; }
    public Parry_Skill parry { get; private set; }
    public Dodge_Skill dodge { get; private set; }
    private void Awake() {
        if (instance != null) {
            Destroy(instance.gameObject);
        }
        else {
            instance = this;
        }
    }
    public void RegisterDash(Dash_Skill skill) => dash = skill;
    public void RegisterClone(Clone_Skill skill) => clone = skill;
    public void RegisterSword(Sword_Skill skill) => sword = skill;
    public void RegisterBlackhole(Blackhole_Skill skill) => blackhole = skill;
    public void RegisterCrystal(Crystal_Skill skill) => crystal = skill;
    public void RegisterParry(Parry_Skill skill) => parry = skill;
    public void RegisterDodge(Dodge_Skill skill) => dodge = skill;


    public void RefreshAllPassiveEffects() {
        dodge?.RefreshPassiveEffect();
        // 如果有其他被动技能（如加生命、加攻击等），同样调用它们的刷新方法
    }
}
