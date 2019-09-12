using System;
using System.Text;

/// <summary>
/// Encryption 的摘要说明
/// </summary>
public class Encryption
{
	public Encryption()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}

    //用于加密的关键字
    private const string KEY = "Pa$$w0rd01!@.@HarrisonCao850120";

    /// <summary>
    /// 对给定的字符串进行加密
    /// </summary>
    /// <param name="code">需要加密的字符串</param>
    /// <returns>返回一个加过密的字符串</returns>
    public static string Encrypt(string code)
    {
        StringBuilder strToRet = new StringBuilder();
        byte[] bArr = Encoding.Default.GetBytes(code);
        byte[] kArr = Encoding.Default.GetBytes(KEY);
        byte[] tmpArr = new byte[bArr.Length];
        for (int i = 0; i < bArr.Length; i++)
        {
            tmpArr[i] = (byte)(bArr[i] ^ kArr[i % kArr.Length]);
        }

        foreach (byte item in tmpArr)
        {
            strToRet.AppendFormat("{0:x2}", item);
        }

        return strToRet.ToString().ToUpper();
    }

    /// <summary>
    /// 对给定的字符串进行解密（要求字符串使用本类加密算法）
    /// </summary>
    /// <param name="code">需要解密的字符串</param>
    /// <returns>返回一个解密后的字符串</returns>
    public static string Decrypt(string code)
    {
        string strToRet = "";
        byte[] kArr = Encoding.Default.GetBytes(KEY);
        int codeLength = code.Length;
        byte[] bcode = new byte[codeLength / 2];

        for (int i = 0; i < codeLength / 2; i++)
        {
            bcode[i] = (byte)(Convert.ToInt32(code.Substring(i * 2, 2), 16));
        }

        byte[] bDeCode = new byte[bcode.Length];
        for (int i = 0; i < bcode.Length; i++)
        {
            bDeCode[i] = (byte)(bcode[i] ^ kArr[i % kArr.Length]);
        }
        strToRet = Encoding.Default.GetString(bDeCode);

        return strToRet;
    }

    /// <summary>
    /// 根据给定的数据库连接字符串和该字符串中需要加密的键来对该字符串加密
    /// </summary>
    /// <param name="strToProcess">给定的数据库连接字符串</param>
    /// <param name="keyNeedToEncrypt">
    /// 给定的字符串中需要加密的部分，
    /// 比如：给定字符串: server=172.16.50.8\xpdb;uid=lesSA;pwd=lesSA;database=les_live
    /// 如果该值设定为 pwd那么将对pwd的值lesSA进行加密操作</param>
    /// <returns>返回加密后的数据库连接字符串</returns>
    public static string EncryptConnectionStr(string strToProcess, string keyNeedToEncrypt)
    {
        string strToReturn = "";
        string[] arrStr = strToProcess.Split(';');
        string encryptCode = "";
        bool isEncrypt = false;

        int n = 0;
        foreach (string item in arrStr)
        {
            string[] arrKeyValue = item.Split('=');
            if (arrKeyValue.Length < 2)
            {
                if (isEncrypt == true)
                {
                    strToReturn = strToReturn.Substring(0, strToReturn.Length - 1);
                    string[] arrTmp = strToReturn.Split(';');
                    string tmp = arrTmp[arrTmp.Length - 1].Split('=')[1];
                    strToReturn = "";
                    for (int i = 0; i < arrTmp.Length - 1; i++)
                    {
                        strToReturn += arrTmp[i] + ";";
                    }
                    strToReturn += arrTmp[arrTmp.Length - 1].Split('=')[0] + "=";
                    tmp = Decrypt(tmp) + ";" + arrKeyValue[0];

                    strToReturn += Encrypt(tmp) + ";";
                }
            }
            if (arrKeyValue[0].ToLower() == keyNeedToEncrypt.ToLower())
            {

                isEncrypt = true;
                if (arrKeyValue.Length == 2)
                {
                    encryptCode = Encrypt(arrKeyValue[1]);
                    if (n == arrStr.Length - 1)
                    {
                        strToReturn += arrKeyValue[0] + "=" + encryptCode;
                    }
                    else
                    {
                        strToReturn += arrKeyValue[0] + "=" + encryptCode + ";";
                    }
                }
                if (arrKeyValue.Length > 2)
                {
                    strToReturn += arrKeyValue[0] + "=";
                    string tmp = "";
                    for (int i = 1; i < arrKeyValue.Length; i++)
                    {
                        if (i == arrKeyValue.Length - 1)
                        {
                            tmp += arrKeyValue[i];
                        }
                        else
                        {
                            tmp += arrKeyValue[i] + "=";
                        }
                    }
                    strToReturn += Encrypt(tmp) + ";";
                }

            }
            else
            {
                if (item.Split('=').Length > 1)
                {
                    if (n == arrStr.Length - 1)
                    {
                        strToReturn += item;
                    }
                    else
                    {
                        strToReturn += item + ";";
                    }
                }
                isEncrypt = false;
            }
            n++;
        }
        return strToReturn;
    }

    /// <summary>
    /// 根据给定的数据库连接字符串和该字符串中需要解密的键来对该字符串解密
    /// </summary>
    /// <param name="strToProcess">给定的数据库连接字符串</param>
    /// <param name="keyNeedToEncrypt">
    /// 给定的字符串中需要解密的部分，
    /// 比如：给定字符串: server=172.16.50.8\xpdb;uid=lesSA;pwd=2300;database=les_live
    /// 如果该值设定为 pwd那么将对pwd的值2300进行解密操作
    /// </param>
    /// <returns>返回解密后的数据库连接字符串</returns>
    public static string DecryptConnectionStr(string strToProcess, string keyNeedToEncrypt)
    {
        string strToReturn = "";
        string[] arrStr = strToProcess.Split(';');
        string encryptCode = "";
        int n = 0;
        foreach (string item in arrStr)
        {
            string[] arrKeyValue = item.Split('=');
            if (arrKeyValue[0].ToLower() == keyNeedToEncrypt.ToLower())
            {
                encryptCode = Decrypt(arrKeyValue[1]);
                if (n == arrStr.Length - 1)
                {
                    strToReturn += arrKeyValue[0] + "=" + encryptCode;
                }
                else
                {
                    strToReturn += arrKeyValue[0] + "=" + encryptCode + ";";
                }
            }
            else
            {
                if (n == arrStr.Length - 1)
                {
                    strToReturn += item;
                }
                else
                {
                    strToReturn += item + ";";
                }
            }
            n++;
        }
        return strToReturn;
    }
}
