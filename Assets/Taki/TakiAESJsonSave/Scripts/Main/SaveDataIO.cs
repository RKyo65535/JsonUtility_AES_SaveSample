using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Taki.TakiAESJsonSave
{
    /// <summary>
    /// 実際にセーブデータを管理するクラスです。
    /// </summary>
    public class SaveDataIO
    {
        //実行時定数
        private static readonly string SavePath = Application.persistentDataPath + "/";

        //鍵などのデータはコンパイル時定数とすることで、実行速度を上昇を狙う。
        //ここは手動で入力する必要がある。既にある値から好きに変えてください。
        private const string DefaultAesKey32byte = @"11112222333344445555666677778888";
        private const string DefaultAesIV16byte = @"1111222233334444";

        private const int KeySize = 256;
        private const int BlockSize = 128;

        /// <summary>
        /// UnityのJsonUtilityを利用することで、JsonUtilityが対応したクラスをJson形式で保存します。
        /// ハードコーディングされた鍵と初期化ベクトルを使用します。保存場所も既定です。
        /// </summary>
        /// <typeparam name="ClassType">セーブするクラスの型。すべてのメンバ変数はシリアライズ可能である必要があります。</typeparam>
        /// <param name="instance">セーブするクラスのインスタンス。</param>
        /// <param name="fileName">ファイル名。パスは必要ありません。</param>
        static public void SavePlayerData<ClassType>(ClassType instance, string fileName)
        {
            string path = SavePath + fileName;
            SavePlayerData(instance, path, DefaultAesIV16byte, DefaultAesKey32byte);
        }
        /// <summary>
        /// ファイルを読み込み、UnityのJsonUtilityを利用することで、指定したクラスにパースします。
        /// </summary>
        /// <typeparam name="ClassType">セーブするクラスの型。すべてのメンバ変数はシリアライズ可能である必要があります。</typeparam>
        /// <param name="fileName">読み込むファイル名。パスは必要ありません。</param>
        /// <returns>パースされたインスタンス</returns>
        static public ClassType LoadPlayerData<ClassType>(string fileName)
        {
            string path = SavePath + fileName;
            return LoadPlayerData<ClassType>(path, DefaultAesIV16byte, DefaultAesKey32byte);
        }

        /// <summary>
        /// UnityのJsonUtilityを利用することで、JsonUtilityが対応したクラスをJson形式で保存します。
        /// Taskに対応しています。
        /// ハードコーディングされた鍵と初期化ベクトルを使用します。保存場所も既定です。
        /// </summary>
        /// <typeparam name="ClassType">セーブするクラスの型。すべてのメンバ変数はシリアライズ可能である必要があります。</typeparam>
        /// <param name="instance">セーブするクラスのインスタンス。</param>
        /// <param name="fileName">ファイル名。パスは必要ありません。</param>
        static public async Task SavePlayerDataAsync<ClassType>(ClassType instance, string fileName)
        {
            string path = SavePath + fileName;
            await Task.Run(()=>SavePlayerData(instance, path, DefaultAesIV16byte, DefaultAesKey32byte));
        }
        /// <summary>
        /// ファイルを読み込み、UnityのJsonUtilityを利用することで、指定したクラスにパースします。
        /// Taskに対応しています。
        /// </summary>
        /// <typeparam name="ClassType">セーブするクラスの型。すべてのメンバ変数はシリアライズ可能である必要があります。</typeparam>
        /// <param name="fileName">読み込むファイル名。パスは必要ありません。</param>
        /// <returns>パースされたインスタンス</returns>
        static public async Task<ClassType> LoadPlayerDataAsync<ClassType>(string fileName)
        {
            string path = SavePath + fileName;
            
            return await Task.Run(()=>LoadPlayerData<ClassType>(path, DefaultAesIV16byte, DefaultAesKey32byte));
        }

        /// <summary>
        /// UnityのJsonUtilityを利用することで、JsonUtilityが対応したクラスをJson形式で保存します。
        /// </summary>
        /// <typeparam name="ClassType">セーブするクラスの型。すべてのメンバ変数はシリアライズ可能である必要があります。</typeparam>
        /// <param name="instance">セーブするクラスのインスタンス。</param>
        /// <param name="path">セーブするパス。ファイルの名前も含む必要があります。</param>
        /// <param name="aesIV">AESによる暗号化の初期化ベクトル。</param>
        /// <param name="aesKey">AESによる暗号化キー。</param>
        static public void SavePlayerData<ClassType>(ClassType instance, string path, string aesIV, string aesKey)
        {
            //条件を満たさないなら早期リターン
            if (!IsSatisfyStringLength(aesKey, aesIV))
            {
                Debug.Log("初期化エラー");
                return;
            }

            //セーブしたいクラスをJsonに変換
            string jsonData = JsonUtility.ToJson(instance);
            //Jsonテキストデータの暗号化
            string cipher = Encrypt(jsonData, aesIV, aesKey);
            //StreamWriterで書き込む
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.GetEncoding("utf-8")))
            {
                writer.WriteLine(cipher);
            }
        }

        /// <summary>
        /// ファイルを読み込み、UnityのJsonUtilityを利用することで、指定したクラスにパースします。
        /// </summary>
        /// <typeparam name="ClassType">セーブするクラスの型。すべてのメンバ変数はシリアライズ可能である必要があります。</typeparam>
        /// <param name="path">セーブするパス。ファイルの名前も含む必要があります。</param>
        /// <param name="aesIV">AESによる暗号化の初期化ベクトル。</param>
        /// <param name="aesKey">AESによる暗号化キー。</param>
        /// <returns>パースされたインスタンス</returns>
        static public ClassType LoadPlayerData<ClassType>(string path, string aesIV, string aesKey)
        {
            //ファイルが存在しないなら早期リターン
            if (!File.Exists(path))
            {
                Debug.Log("ファイルは存在しません");
                return default;
            }
            if (!IsSatisfyStringLength(aesKey, aesIV))
            {
                Debug.Log("初期化エラー");
                return default;
            }
            //暗号文を読み取った文章を保存する変数
            string cipher;
            //StreamReaderで暗号文の読み込み
            using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("utf-8")))
            {
                cipher = sr.ReadToEnd();
            }
            //復号を行う。
            string plain = Decrypt(cipher, aesIV, aesKey);

            //複合したデータのパースを試みる。
            ClassType plainedInstance = default;
            try
            {
                plainedInstance = JsonUtility.FromJson<ClassType>(plain);//ref したクラスのインスタンスに情報を格納する
            }
            catch
            {
                Debug.Log("セーブデータのパースに失敗しました。");
            }

            //いずれにせよ結果をリターンする。
            return plainedInstance;
        }

        #region 以下は非公開の関数です。このクラスを使う側は特に意識しなくても良いと思います。

        /// <summary>
        /// AESを使って文字列を暗号化します
        /// </summary>
        /// <param name="text">暗号化する文字列</param>
        /// <param name="iv">初期化ベクトル</param>
        /// <param name="key">暗号化キー</param>
        /// <returns>暗号化された文字列</returns>
        static string Encrypt(string text, string iv, string key)
        {

            using (AesManaged aes = CreateAESManager(key, iv))
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                byte[] encrypted;
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream ctStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(ctStream))
                        {
                            sw.Write(text);
                        }
                        encrypted = mStream.ToArray();
                    }
                }
                return System.Convert.ToBase64String(encrypted);
            }
        }

        /// <summary>
        /// AESを使って暗号文を復号します
        /// </summary>
        /// <param name="cipher">暗号化された文字列</param>
        /// <param name="iv">初期化ベクトル</param>
        /// <param name="key">暗号化キー</param>
        /// <returns>復号された文字列</returns>
        static string Decrypt(string cipher, string iv, string key)
        {
            using (AesManaged aes = CreateAESManager(key, iv))
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                string plain = string.Empty;
                using (MemoryStream mStream = new MemoryStream(System.Convert.FromBase64String(cipher)))
                {
                    using (CryptoStream ctStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(ctStream))
                        {
                            plain = sr.ReadLine();
                        }
                    }
                }
                return plain;
            }
        }

        /// <summary>
        /// AES管理クラスを作成する関数。必要な初期化をまとめた。
        /// </summary>
        /// <param name="key">暗号化キー</param>
        /// <param name="iv">初期化ベクトル</param>
        /// <returns></returns>
        static AesManaged CreateAESManager(string key, string iv)
        {
            AesManaged aes = new AesManaged();
            aes.BlockSize = BlockSize;
            aes.KeySize = KeySize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            aes.IV = Encoding.UTF8.GetBytes(iv);
            aes.Key = Encoding.UTF8.GetBytes(key);

            return aes;

        }

        /// <summary>
        /// キーや初期化ベクトルのサイズが合っているか確認します。
        /// </summary>
        static bool IsSatisfyStringLength(string key, string iv)
        {
            return key.Length *8== KeySize && iv.Length * 8 == BlockSize;
        }

        #endregion
    }
}