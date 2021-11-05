using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Taki.TakiAESJsonSave.Sample
{
    /// <summary>
    /// 実際にUnityの上で動作する例
    /// </summary>
    class MainManager : MonoBehaviour
    {
        //それぞれの入力場所への参照
        [SerializeField] InputField nameInput;
        [SerializeField] InputField moneyInput;
        [SerializeField] InputField positionXInput;
        [SerializeField] InputField positionYInput;
        [SerializeField] InputField skillNameInput;
        [SerializeField] InputField luckyNumInput;

        //それぞれの決定ボタンへの参照
        [SerializeField] Button nameSubmitButton;
        [SerializeField] Button moneySubmitButton;
        [SerializeField] Button advancedSubmitButton;
        [SerializeField] Button positionSubmitButton;
        [SerializeField] Button skillSubmitButton;
        [SerializeField] Button luckyNumSubmitButton;

        //表示場所
        [SerializeField] Text nameText;
        [SerializeField] Text moneyText;
        [SerializeField] Text advancedText;
        [SerializeField] Text positionText;
        [SerializeField] Text skillText;
        [SerializeField] Text luckyNumText;

        //セーブ関係
        [SerializeField] Button saveButton;
        [SerializeField] InputField loadNameInput;
        [SerializeField] Button loadButton;

        //おおもとのデータ
        SampleSaveClass sampleData;


        private void Awake()
        {
            sampleData = new SampleSaveClass();

            //ボタンへのイベントを登録
            nameSubmitButton.onClick.AddListener(SetName);
            moneySubmitButton.onClick.AddListener(SetMoney);
            advancedSubmitButton.onClick.AddListener(Advanced);
            positionSubmitButton.onClick.AddListener(SetPosition);
            skillSubmitButton.onClick.AddListener(AddSkill);
            luckyNumSubmitButton.onClick.AddListener(AddLuckyNumber);

            saveButton.onClick.AddListener(() => _ = SaveAllData());
            loadButton.onClick.AddListener(() => _ = LoadAllData());

        }

        /// <summary>
        /// 名前決定ボタンが押されたときに実行
        /// </summary>
        void SetName()
        {
            sampleData.Name = nameInput.text;
            UpdateText();
        }

        /// <summary>
        /// お金決定ボタンが押されたときに実行
        /// </summary>
        void SetMoney()
        {
            int money;
            if (int.TryParse(moneyInput.text, out money))
            {
                sampleData.Money = money;
            }
            else
            {
                moneyInput.text = "無効なデータでした。Int型整数を入力してください。";
            }
            UpdateText();
        }

        /// <summary>
        /// 進化ボタンが押されたときに実行する。
        /// </summary>
        void Advanced()
        {
            sampleData.Advanced();
            UpdateText();
        }

        /// <summary>
        /// 位置決定ボタンが押されたときに実行する
        /// </summary>
        void SetPosition()
        {
            float x, y;
            if (float.TryParse(positionXInput.text, out x) && float.TryParse(positionYInput.text, out y))
            {

                sampleData.Position = new Vector2(x, y);

            }
            else
            {
                positionXInput.text = "無効なデータでした。";
                positionYInput.text = "float数値を入力してください。";

            }
            UpdateText();

        }

        /// <summary>
        /// スキル追加ボタンが押されたときに実行する
        /// </summary>
        void AddSkill()
        {
            for (int i = 0; i < sampleData.Skills.Count; i++)
            {
                if (string.IsNullOrEmpty(sampleData.Skills[i]))
                {
                    Debug.Log(sampleData.Skills[i]);
                    sampleData.SetSkill(i, skillNameInput.text);
                    break;
                }
            }
            UpdateText();

        }

        /// <summary>
        /// ラッキーナンバー追加ボタンが押されたときに実行する。
        /// </summary>
        void AddLuckyNumber()
        {
            StringBuilder sb = new StringBuilder();
            int luckyNumber;
            if (int.TryParse(luckyNumInput.text, out luckyNumber))
            {
                sampleData.AddLuckyNumbers(luckyNumber);
            }
            else
            {
                moneyInput.text = "無効なデータでした。Int型整数を入力してください。";
            }

            UpdateText();
        }



        /// <summary>
        /// 非同期でデータをセーブする。
        /// </summary>
        /// <returns></returns>
        async Task SaveAllData()
        {
            await SaveDataIO.SavePlayerDataAsync(sampleData,sampleData.Name);                
        }

        /// <summary>
        /// 非同期でデータをロードする。
        /// </summary>
        /// <returns></returns>
        async Task LoadAllData()
        {
            sampleData = await SaveDataIO.LoadPlayerDataAsync<SampleSaveClass>(loadNameInput.text);

            if (sampleData == null)
            {
                sampleData = new SampleSaveClass();
            }
            UpdateText();
        }

        void UpdateText()
        {
            nameText.text = sampleData.Name;
            moneyText.text = sampleData.Money.ToString();
            advancedText.text = sampleData.IsAdvanced.ToString();
            positionText.text = sampleData.Position.x + " " + sampleData.Position.y;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < sampleData.Skills.Count; i++)
            {
                sb.Append(sampleData.Skills[i]).Append("\n");
            }
            skillText.text = sb.ToString();

            sb.Clear();
            for (int i = 0; i < sampleData.LuckyNumbers.Count; i++)
            {
                sb.Append(sampleData.LuckyNumbers[i]).Append("\n");
            }
            luckyNumText.text = sb.ToString();
        }
    }
}
