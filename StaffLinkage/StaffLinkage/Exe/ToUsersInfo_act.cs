using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace StaffLinkage.Exe
{
	/// <summary>
	/// UserInfo_Dataset->Entity.ToUsersInfoEntity 変換クラス
	/// </summary>
	class ToUsersInfo_act
	{
		/// <summary>
		/// ログ出力
		/// </summary>
		private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
				MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// フィールド名の定義
		/// このフィールド名順序を変更しないでください。
		/// ToUsersInfoEntityの項目への処理順序とリンクさせています。
		/// フィールドを追加する場合は配列末尾へ行い、処理順序末尾に当該処理コードを入れてください。
		/// なお、フィールド名の変更は、Util.CommonParameterで行ってください。
		/// </summary>
		private string[] arrfldnm = new string[] {
			Util.CommonParameter.xmlFLD_REQUESTID
			, Util.CommonParameter.xmlFLD_REQUESTDATE 
			, Util.CommonParameter.xmlFLD_DB
			, Util.CommonParameter.xmlFLD_APPCODE
			, Util.CommonParameter.xmlFLD_USERID 
			, Util.CommonParameter.xmlFLD_HOSPITALID
			, Util.CommonParameter.xmlFLD_PASSWORD
			, Util.CommonParameter.xmlFLD_USERNAMEKANJI
			, Util.CommonParameter.xmlFLD_USERNAMEENG
			, Util.CommonParameter.xmlFLD_SECTION_ID
			, Util.CommonParameter.xmlFLD_TANTO_SECTION_ID
			, Util.CommonParameter.xmlFLD_STAFFID
			, Util.CommonParameter.xmlFLD_SYOKUIN_KBN
			, Util.CommonParameter.xmlFLD_TEL
			, Util.CommonParameter.xmlFLD_PASSWORDEXPIRYDATE
			, Util.CommonParameter.xmlFLD_PASSWORDWARNINGDATE
			, Util.CommonParameter.xmlFLD_USERIDVALIDITYFLAG
			, Util.CommonParameter.xmlFLD_REQUESTTYPE
			, Util.CommonParameter.xmlFLD_MESSAGEID1
			, Util.CommonParameter.xmlFLD_MESSAGEID2
			, Util.CommonParameter.xmlFLD_MESSAGEID3
			, Util.CommonParameter.xmlFLD_TRANSFERSTATUS
			, Util.CommonParameter.xmlFLD_TRANSFERDATE
			, Util.CommonParameter.xmlFLD_TRANSFERRESULT
			, Util.CommonParameter.xmlFLD_TRANSFERTEXT
			// , Util.CommonParameter.xmlFLD_SECTION_NAME // さいたま赤十字特注
		};

		/// <summary>
		/// １ユーザ分の変換
		/// </summary>
		/// <param name="today">システム日付</param>
		/// <param name="rowno">データ数目</param>
		/// <param name="UsrInf">UserInfo_Dataset</param>
		/// <param name="tui">Entity.ToUsersInfoEntity</param>
		private void Conv_UsrInf_to_TuiEnt(DateTime today, int rowno, UserInfo_Dataset UsrInf, ref Entity.ToUsersInfoEntity tui)
		{
			string strval = "";
			string strdummy = string.Empty;
			List<string> lstval = new List<string>();

			// 全フィールドを見る
			for (int arrno = 0; arrno < arrfldnm.Length; arrno++)
			{
				// 値を取得
				strval = UsrInf.Get_DataV(rowno, arrfldnm[arrno]);
				if (strval == Util.CommonParameter.tgmNO_DATA)
				{
					// 電文からの変換が前提となっている項目は、ココに入る事は無いはず。
					// 電文マッピングで指定されない項目は、
					// 必ずCtlファイルによる変換を定義しておくこと！
					strval = strdummy;
				}
				switch (arrno)
				{
					case 0:
						// 送信要求番号 -->> Ctlファイル：SEQUENCE指定
						tui.RequestId = strval;
						break;
					case 1:
						// 送信要求日時 -->> Ctlファイル：SYSDATE
						tui.RequestDate = strval;
						break;
					case 2:
						// DB -->> Ctlファイル：固定値
						tui.Db = strval;
						break;
					case 3:
						// APPCODE -->> Ctlファイル：固定値
						tui.AppCode = strval;
						break;
					case 4:
						// ユーザID
						tui.UserId = strval;
						break;
					case 5:
						// 病院ID -->> Ctlファイル：固定値
						tui.HospitalId = strval;
						break;
					case 6:
						// パスワード
						tui.Password = strval;
						break;
					case 7:
						// ユーザ名(表示用)
						// 2017-12-01 mod start
						//tui.UserNameKanji = strval;
						tui.UserNameKanji = strval.Replace("　", " ");
						// 2017-12-01 mod end
						break;
					case 8:
						// ユーザ名(ローマ字)
						tui.UserNameEng = strval;
						break;
					case 9:
						// 診療科ID
						tui.Section_Id = strval;
						break;
					case 10:
						// 担当_診療科ID

						// 該当する項目をカンマ区切りで並べてから設定
						lstval.Clear();
						lstval = UsrInf.Get_DataL(rowno, arrfldnm[arrno]);
						strval = "  ";
						foreach (string tsi in lstval)
						{
							if (!(tsi == ""))
							{
								if (strval.IndexOf(tsi) < 0)
								{
									strval += tsi + ",";
								}
							}
						}
						strval = strval.Substring(0, strval.Length - 1);
						tui.Tanto_Section_Id = strval.Trim();

						break;
					case 11:
						// 職員コード
						tui.StaffId = strval;
						break;
					case 12:
						// 職員区分
						tui.Syokuin_Kbn = strval;
						break;
					case 13:
						// 内線番号
						tui.Tel = strval;
						break;
					case 14:
						// パスワード有効期限 -->> Ctlファイル：判定変換

						// パスワード有効期限が日付形式の場合
						DateTime pwddate = DateTime.MinValue;
						if (Util.CommonUtil.ConvertDateTime(ToUsersInfo.ConvertMaxDate(strval), Util.CommonParameter.YYYYMMDD, ref pwddate))
						{
							// パスワード有効期限を格納
							tui.PasswordExpiryDate = pwddate.ToString(Util.CommonParameter.YYYYMMDD);
							// 1ヵ月前の日時を格納
							tui.PasswordWarningDate = pwddate.AddMonths(-1).ToString(Util.CommonParameter.YYYYMMDD);
						}
						else
						{
							// 空文字を格納
							tui.PasswordExpiryDate = string.Empty;
							// 空文字を格納
							tui.PasswordWarningDate = string.Empty;
						}

						// ユーザID有効フラグ
						tui.UserIdValidityFlag = "1";
						// 有効期限終了日がシステム日時より過去の場合
						if (pwddate != DateTime.MinValue && pwddate < today)
						{
							// 有効フラグ「無効：0」を格納
							tui.UserIdValidityFlag = "0";
						}

						break;
					case 15:
						// パスワード警告開始日 -->> Ctlファイル：判定変換
						// パスワード有効期限の方で設定済なので何もしない
						// tui.PasswordWarningDate = strval;
						break;
					case 16:
						// ユーザID有効フラグ
						// パスワード有効期限の方で設定済なので何もしない
						// tui.UserIdValidityFlag = strval;
						break;
					case 17:
						// 処理種別ID

						// 使用停止フラグが「使用停止：1」の場合
						if (strval == "1")
						{
							// 処理種別ID「削除：US99」を格納
							tui.RequestType = "US99";
						}
						else
						{
							// 処理種別ID「登録：US01」を格納
							tui.RequestType = "US01";
						}

						break;
					case 18:
						// 将来用カラム -->> Ctlファイル：固定値
						tui.MessageId1 = strval;
						break;
					case 19:
						// 将来用カラム -->> Ctlファイル：固定値
						tui.MessageId1 = strval;
						break;
					case 20:
						// 将来用カラム -->> Ctlファイル：固定値
						tui.MessageId1 = strval;
						break;
					case 21:
						// 送信ステータス -->> Ctlファイル：固定値
						tui.TransferStatus = strval;
						break;
					case 22:
						// 送信処理日時 -->> Ctlファイル：固定値
						tui.TransferDate = strval;
						break;
					case 23:
						// 送信結果 -->> Ctlファイル：固定値
						tui.TransferResult = strval;
						break;
					case 24:
						// 送信電文 -->> Ctlファイル：固定値
						tui.TransferText = strval;
						break;
					default:
						throw new Exception("ToUsersInfoテーブルに存在しないはずのフィールドが指定されました。");
				} // switch文

			} // for文 フィールド全部を見る
		}

		/// <summary>
		/// UserInfo_Datasetのユーザ情報をEntity.ToUsersInfoEntityのユーザ情報へ変換する
		/// </summary>
		/// <param name="today">システム日付</param>
		/// <param name="UsrInf">UserInfo_Dataset</param>
		/// <param name="toUsersList">Entity.ToUsersInfoEntity</param>
		/// <returns>OK=true, ERR=false</returns>
		public bool Convert_ToUsersInfo(DateTime today, UserInfo_Dataset UsrInf, ref List<Entity.ToUsersInfoEntity> toUsersList)
		{
			// 全ユーザ情報を変換していく
			for (int rowno = 0; rowno < UsrInf.RowCount; rowno++)
			{
				int intOutflg = UsrInf.intOutflg(rowno);
				if (intOutflg == 0)
				{
					try
					{
						// ToUsersInfoEntity クラス・インスタンス作成
						Entity.ToUsersInfoEntity tui = new Entity.ToUsersInfoEntity();

						// １ユーザ分変換
						Conv_UsrInf_to_TuiEnt(today, rowno, UsrInf, ref tui);

						// 引数のToUsersInfoEntity のリストへ追加する
						toUsersList.Add(tui);

						// ログ記録
						_log.DebugFormat("出力電文ORG={0}", UsrInf.strUsrInfo(rowno, false));
						_log.DebugFormat("　　　　CHG={0}", UsrInf.strUsrInfo(rowno));
					}
					catch (Exception ex)
					{
						// 出力フラグを「-2：エラー」に設定します。
						UsrInf.intOutflg(rowno, -2);
						// エラーログ記録
						_log.ErrorFormat("ToUsersInfoへのマッピングエラーのため、処理対象外です。ORG={0}", UsrInf.strUsrInfo(rowno, false));
						_log.ErrorFormat("CHG={0}", UsrInf.strUsrInfo(rowno));
					}
				}
				else if (intOutflg == -1)
				{
					// 出力対象外：更新日が期間外
					_log.InfoFormat("更新日が取込期間外のため、処理対象外です。ORG={0}", UsrInf.strUsrInfo(rowno, false));
				}
				else
				{
					// 出力対象外：電文解析エラー
					_log.ErrorFormat("電文解析でエラーのため、処理対象外です。ORG={0}", UsrInf.strUsrInfo(rowno, false));
				}
			} // for文 全ユーザ情報を変換していく

			return true;
		} // Convert_ToUsersInfo()

	} // ToUsersInfo_act
	
} // namespace StaffLinkage.Exe
