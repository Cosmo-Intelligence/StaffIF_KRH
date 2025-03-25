using System.Collections;
using System.IO;
using System.Xml;

namespace Serv_UsersIFLinkage.Util
{
	class XmlUtil
	{
		// ファイルパス
		public string strFilename
		{
			get; set;
		}

		/// <summary>
		/// XML読込
		/// </summary>
		/// <param name="strNode">ノード名</param>
		/// <param name="table">読込値</param>
		public bool xmlRead(string strNode, Hashtable table)
		{
			if (!File.Exists(strFilename))
			{   // 指定されたファイルが存在しない
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
