
namespace StaffLinkage.Util
{
  /// <summary>
  /// アプリケーションコンフィグ定義
  /// </summary>
  public sealed class AppConfigParameter
  {
    public static string FtpIPAdress = "FtpIPAdress";
    public static string FtpUser = "FtpUser";
    public static string FtpPassword = "FtpPassword";
    public static string FtpRetryCount = "FtpRetryCount";
    public static string FtpEncode = "FtpEncode";
    public static string FtpFolder = "FtpFolder";
    public static string FtpFile = "FtpFile";
    public static string SqlldrConnectionString = "SqlldrConnectionString";
    public static string SqlldrFolder = "SqlldrFolder";
    public static string SqlldrFolderKeepDays = "SqlldrFolderKeepDays";
    public static string LogKeepDays = "LogKeepDays";
    public static string ConvKanjiFile = "ConvKanjiFile";

    // Y_Higuchi -- add --
    public static string TelegramMap = "TelegramMap";
    public static string PasswordChange = "PasswordChange";
    public static string ConvKanji = "ConvKanji";
    // Y_Higuchi -- add --

    public static string UserModymdFile = "UserModymdFile";

    public static string DB = "DB";
    public static string DEFAULT = "DEFAULT";
    public static string IMPORT_DB = "IMPORT_";

      // Y_Higuchi -- add -- 参考=sa_相模原協同病院 --
    /// <summary>
    /// スレッド待機時間(ミリ秒)
    /// </summary>
    public static string ThreadInterval = "ThreadInterval";
      // Y_Higuchi -- add -- 参考=sa_相模原協同病院 --

  }
}
