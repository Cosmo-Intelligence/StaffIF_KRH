using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace KanjiConversion
{
	/// <summary>
	/// 漢字変換クラス
	/// </summary>
	public class clsKanjiConversion
	{
		#region private_static_declare
		// 漢字コード参照用：app.config-key値:Default
		private static string strKjcode = "KanjiCode";
		// 変換漢字用：app.config-key値:Default
		private static string strKjset = "KanjiSet";
		// Encodingクラスへの参照
		private static Encoding sysEnc = System.Text.Encoding.Unicode;
		private static Encoding myEnc = null;
		private static List<string> oldKj = new List<string>();
		private static List<string> newKj = new List<string>();
		private static List<string> oldKjm = new List<string>();
		private static List<string> newKjm = new List<string>();
		//文字コード名/コードページNo.
		public static string mojicode_nm { get; set; }		
		public static int mojicode_pg { get; set; }
		#endregion private_static_declare

		#region public_method

		/// <summary>
		/// 使用領域の破棄()
		/// </summary>
		public static void Clear()
		{
			oldKj.Clear();
			newKj.Clear();
			oldKjm.Clear();
			newKjm.Clear();
		}

		/// <summary>
		/// 設定ファイルの指定：１回だけ実行すればＯＫ
		/// </summary>
		/// <param name="Config_filename">xml設定ファイル名(フルパス付き)</param>
		/// <returns>true=成功, false=失敗</returns>
		public static bool boolSet_Config(string Config_filename)
		{
			bool boolRet = true;
			XmlUtil conf_xml = new XmlUtil();
			Hashtable kjcd = new Hashtable();
			Hashtable kjset = new Hashtable();
			try
			{
				// 設定ファイル(xml)を読み込む準備
				conf_xml.strFilename = Config_filename;

				// 漢字コードの指定を参照
				boolRet = conf_xml.xmlRead(strKjcode, kjcd);
				if (!boolRet)
				{
					return false;
				}
				if (kjcd[strKjcode] == null)
				{
					return false;
				}

				// Encodingクラスへの参照初期化
				boolRet = boolInitEncoding(kjcd[strKjcode].ToString());
				if (!boolRet)
				{
					return false;
				}

				// 漢字変換文字セットの指定を参照
				boolRet = conf_xml.xmlRead(strKjset, kjset);
				if (!boolRet)
				{
					return false;
				}

				// 漢字変換文字セットの定義読込
				boolRet = boolRead_Kjsets(kjset);

				return boolRet;
			}
			finally
			{
				kjset.Clear();
				kjcd.Clear();
				kjset = null;
				kjcd = null;
				conf_xml = null;
			}
		}

		/// <summary>
		/// 文字列配列の漢字変換(設定ファイル規定)
		/// </summary>
		/// <param name="strOld">変換したい文字列が入った配列</param>
		/// <returns>変換後の文字列の配列</returns>
		public static string[] ConvertAll(string[] strOld)
		{
			List<string> lstNew = new List<string>();
			try
			{
				foreach (string strBefore in strOld)
				{
					string strAfter = Convert(strBefore);
					lstNew.Add(strAfter);
				}
				string[] strRet = lstNew.ToArray();
				return strRet;
			}
			finally
			{
				lstNew.Clear();
				lstNew = null;
			}
		}

		/// <summary>
		/// 漢字変換(設定ファイル規定)
		/// </summary>
		/// <param name="strOld">変換元文字列</param>
		/// <returns>変換後文字列(エラーの場合は"#"半角シャープ１文字を返す)</returns>
		public static string Convert(string strOld)
		{
			try
			{
				// システム文字コードへEncoding
				byte[] btOld = System.Text.Encoding.Convert(myEnc, sysEnc, myEnc.GetBytes(strOld));
				string strOld_unicode = sysEnc.GetString(btOld);

				string strNew_unicode = "";
				// 複数字変換
				strNew_unicode = strConv_MulKj(strOld_unicode);
				// 単文字変換
				strNew_unicode = strConv_UniKj(strNew_unicode);

				// 指定文字コードへEncoding
				byte[] btNew = System.Text.Encoding.Convert(sysEnc, myEnc, sysEnc.GetBytes(strNew_unicode));
				string strNew = myEnc.GetString(btNew);
				return strNew;
			}
			catch
			{
				return "#";
			}
		}		
		#endregion public_method

		#region private_method
		/// <summary>
		/// 複数文字変換
		/// </summary>
		/// <param name="strOld"></param>
		/// <returns></returns>
		private static string strConv_MulKj(string strOld)
		{
			string strNew = strOld;
			for (int intNo = 0; intNo < oldKjm.Count; intNo++ )
			{
				// 該当する文字列は置換
				strNew = strNew.Replace(oldKjm[intNo], newKjm[intNo]);
			}
			return strNew;
		}

		/// <summary>
		/// 単文字変換
		/// </summary>
		/// <param name="strOld">変換する文字列</param>
		/// <returns>変換後の文字列</returns>
		private static string strConv_UniKj(string strOld)
		{
			string strNew = "";
			IEnumerator chrOld = strOld.GetEnumerator();
			while (chrOld.MoveNext())
			{
				int intNo = oldKj.IndexOf(chrOld.Current.ToString());
				if (intNo < 0)
				{
					strNew += chrOld.Current;
				}
				else
				{
					strNew += newKj[intNo];
				}
			}
			return strNew;
		}

		/// <summary>
		/// 指定されたEncodingクラスへの参照を取得する
		/// 先にコードページNoを試してNGならば
		/// 次にコード名でEncodingクラス参照を取得する
		/// </summary>
		/// <param name="strKjcd">文字コード指定(コード名;コードページNo)</param>
		/// <returns>true=成功, false=失敗</returns>
		private static bool boolInitEncoding(string strKjcd)
		{
			string[] strarrMojicd = strKjcd.Split(';');
			if (strarrMojicd.Length < 2)
			{
				return false;
			}

			// 漢字コード名
			mojicode_nm = strarrMojicd[0];

			// 漢字コードページNo
			mojicode_pg = 0; // 環境デフォルトをとりあえず設定
			int intBuf = 0;
			if (!Int32.TryParse(strarrMojicd[1], out intBuf))
			{
				return false;
			}
			mojicode_pg = intBuf;

			myEnc = null;
			try
			{
				// Code指定
				myEnc = Encoding.GetEncoding(mojicode_pg);
			}
			catch 
			{
				// Code でダメなら
				myEnc = null;
				try
				{
					// Name指定
					myEnc = Encoding.GetEncoding(mojicode_nm);
				}
				catch 
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 漢字変換文字セットの定義読込
		/// </summary>
		private static bool boolRead_Kjsets(Hashtable kjset)
		{
			oldKj.Clear();
			newKj.Clear();
			oldKjm.Clear();
			newKjm.Clear();
			try
			{
				for (int int999 = 0; int999 < 1000; int999++)
				{
					string strKey = strKjset + string.Format("{0:000}", int999);
					if (kjset[strKey] == null)
					{	// 該当するKeyが無い
						continue;
					}
					string strBuf = kjset[strKey].ToString();
					if (strBuf == "")
					{	// 設定値が空
						continue;
					}
					string[] strArr = strBuf.Split(';');
					if (strArr.Length <= 1)
					{	// ２項目読込出来なかったKeyは変換対象外として扱う
						continue;
					}
					if ((strArr[0].Length < 1) || (strArr[1].Length < 1))
					{	// key or value が空だったら変換対象外として扱う
						continue;
					}

					if (strArr[0].Length == 1)
					{
						// 単文字
						oldKj.Add(strArr[0]);
						newKj.Add(strArr[1]);
					}
					else
					{
						// 複数文字
						oldKjm.Add(strArr[0]);
						newKjm.Add(strArr[1]);
					} // if (strArr[0].Length = 1)
				} // for (int int999 = 0; int999 < 1000; int999++)
			}
			catch
			{
				oldKj.Clear();
				newKj.Clear();
				oldKjm.Clear();
				newKjm.Clear();
				return false;
			}
			return true;
		}
		#endregion private_method
	}

	/// <summary>
	/// 内部：XMLファイル読込クラス
	/// </summary>
	internal class XmlUtil
	{
		// ファイルパス
		public string strFilename { get; set; }

		/// <summary>
		/// XML読込
		/// </summary>
		/// <param name="strNode">ノード名</param>
		/// <param name="table">読込値</param>
		public bool xmlRead(string strNode, Hashtable table)
		{
			if (!File.Exists(strFilename))
			{	// 指定されたファイルが存在しない
				return false;
			}

			XmlDocument appConfigXmlDocument = null;
			XmlNode rootXmlNode = null;
			XmlNodeList messageNodeList = null;
			try
			{
				//XMLの読込み
				appConfigXmlDocument = new XmlDocument();
				appConfigXmlDocument.Load(@strFilename);
				rootXmlNode = appConfigXmlDocument.DocumentElement;

				// 読み込み
				messageNodeList = rootXmlNode.SelectNodes("/configuration/" + strNode);
				for (int i = 0; i < messageNodeList.Count; i++)
				{
					XmlNode node = messageNodeList[i];
					if (node != null)
					{
						XmlAttributeCollection attrs = node.Attributes;
						string key = attrs["id"].Value;
						string val = attrs["value"].Value.ToString();
						// 同一 Key は読み飛ばし
						if (!table.ContainsKey(key)) 
						{
							table.Add(key, val);
						}
					}
				}
			}
			catch
			{
				return false;
			}
			finally
			{
				// いちおうなんとなく解放入れてみた
				messageNodeList = null;
				rootXmlNode = null;
				appConfigXmlDocument = null;
			}
			return true;
		}
	}
}
