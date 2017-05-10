using UnityEngine;

[System.Serializable]
public class Statistics
{
    [System.Serializable]
    public struct Statistic
    {
        [Tooltip("Valor do stat")]
        [Range(0, 999)] public int value;

        [Tooltip("Valor máximo que o stat pode atingir")]
        [Range(0, 999)] public int max;

        [Tooltip("Se verdadeiro, stat não cresce por level e max é ignorado")]
        public bool constantGrowth;
    }
    public Statistic maxHP;
    public Statistic maxMP;
    public Statistic strength;
    public Statistic magic;
    public Statistic defense;
    public Statistic agility;

    public Statistics(bool allConst)
    {
        if (allConst)
        {
            maxHP.constantGrowth = true;
            maxMP.constantGrowth = true;
            strength.constantGrowth = true;
            magic.constantGrowth = true;
            defense.constantGrowth = true;
            agility.constantGrowth = true;
        }
    }

    public int GetValueAt(Statistic stat, int level)
    {
        return (((int) (stat.max/100)) * stat.value);
    }
}