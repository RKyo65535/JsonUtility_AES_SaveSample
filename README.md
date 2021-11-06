# JsonUtility_AES_SaveSample
Unityの機能JsonUtilityでクラスの変数をJSON化、それをAESで暗号化して保存し、読み込むときに復号するサンプル。

# 環境
Unity 2020.3.21f1 で作成しました。
プログラム自体はJsonUtilityに依存しています。

# 使い方など
SaveDataIOが本体となっております。これと、セーブするインスタンスさえあれば動作します。
## SaveDataIO
このクラスの内部の、`void SavePlayerData<ClassType>(ClassType instance, string fileName)`と
`ClassType LoadPlayerData<ClassType>(string fileName)`がメインとなります。

なお、暗号化キーと初期化ベクトルはここにハードコーディングされているので、お好きな値に書き換えてください。

### SavePlayerData
第一引数にはクラスインスタンスをとります。
第二引数にはファイル名をとります。

第一引数のインスタンスは、シリアライズ可能である必要があります。
また、セーブできる変数は、SerializeField属性でUnityのインスペクタに表示できるものに限られます。
詳しくは、JSONUtilityを仕様をご覧ください。

第二引数は、UnityのApplication.persistentDataPath+"/"以下のファイル名を入力する必要があります。

C#のTaskを用いたAsyncバージョンもあります。(作りに自身はありませんが……)

### LoadPlayerData
第一引数にはファイル名をとります。
また、型指定をする必要があります。
返り値に取得したJSONのデータを指定した型で取り出します。
失敗したらその方のデフォルト値が返ります(クラスだったら大体nullのはずです)。

C#のTaskを用いたAsyncバージョンもあります。(作りに自身はありませんが……)

## SampleSaveClass
セーブするクラスの例です。

string、int、boolなどシリアライズ可能なプリミティブ型、Listや配列といった複数のデータを格納する特定の型、Vector2といったシリアライズ可能な構造体が例としてあります。


## MainManager
SaveDataIOと、SampleSaveClassと、uGUIを使って実際にセーブするスクリプトです。






