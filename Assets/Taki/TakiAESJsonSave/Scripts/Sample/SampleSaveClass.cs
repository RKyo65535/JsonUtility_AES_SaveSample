using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Taki.TakiAESJsonSave.Sample
{
    /// <summary>
    /// 実際にセーブするクラスの例
    /// </summary>
    [System.Serializable]
    public class SampleSaveClass
    {
        const int NumOfSkills = 2;

        [SerializeField] string name;//名前のデータ
        [SerializeField] int money;//所持金のデータ
        [SerializeField] bool isAdvanced;//進化したか否か
        [SerializeField] Vector2 position;//現在位置
        [SerializeField] List<int> luckyNumbers;//ラッキーナンバーたち。
        [SerializeField] string[] skills;//どんなスキルを持っているか。

        /// <summary>
        /// 名前を設定するプロパティ。
        /// 4文字までしかセーブされない設定。
        /// </summary>
        public string Name
        {
            get { return name; }
            set 
            {
                string returnString = value;
                if(value.Length > 4)
                {
                    returnString = value.Substring(0, 4);
                }
                name = returnString; 
            }
        }

        /// <summary>
        /// お金を設定するプロパティ。
        /// </summary>
        public int Money
        {
            get { return money; }
            set { money = value; }
        }

        /// <summary>
        /// 進化したか否かを返す(読み取り限定)
        /// </summary>
        public bool IsAdvanced
        {
            get { return isAdvanced; }
        }

        /// <summary>
        /// 現在位置のプロパティ
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// 読み取り専用のラッキーナンバーリストを返す。
        /// get限定でもlistを返しちゃうと、そのlistの持つ各々にはアクセスできちゃうので。
        /// </summary>
        public IReadOnlyList<int> LuckyNumbers
        {
            get 
            { 
                if(luckyNumbers == null)
                {
                    luckyNumbers = new List<int>();
                }
                return luckyNumbers; 
            }
        }
        /// <summary>
        /// 読み取り専用のスキルを返す。
        /// get限定でも配列を返しちゃうと、その配列の持つ各々にはアクセスできちゃうので。
        /// </summary>
        public IReadOnlyList<string> Skills
        {
            get 
            { 
                if(skills == null)
                {
                    skills = new string[NumOfSkills];
                }
                return skills; 
            }
        }

        public void Advanced()
        {
            isAdvanced = true;
        }


        /// <summary>
        /// ラッキーナンバーを追加します
        /// </summary>
        /// <param name="luckyNum">追加するラッキーナンバー</param>
        public void AddLuckyNumbers(int luckyNum)
        {
            if(luckyNumbers == null)
            {
                luckyNumbers = new List<int>();
            }
            luckyNumbers.Add(luckyNum);
        }

        /// <summary>
        /// スキルを追加します。
        /// </summary>
        /// <param name="index">スキル枠</param>
        /// <param name="skillName">スキル名</param>
        /// <returns>スキル追加に成功したか否か</returns>
        public bool SetSkill(int index,string skillName)
        {
            if(index < 0 || index >= NumOfSkills)
            {
                return false;
            }
            else
            {
                if(skills == null)
                {
                    skills = new string[NumOfSkills];
                }
                skills[index] = skillName;
                return true;
            }
        }

    }
}