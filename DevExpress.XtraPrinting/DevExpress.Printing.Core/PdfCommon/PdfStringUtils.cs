#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Globalization;
using System.Text;
namespace DevExpress.Pdf.Common {
	public class PdfStringUtils {
		static string ReplaceEscapeChar(char ch) {
			if(ch == '(') return "\\(";
			if(ch == ')') return "\\)";
			if(ch == '\\') return "\\\\";
			if(ch == '\r') return "\\r";
			if(ch == '\n') return "\\n";
			if(ch == '\t') return "\\t";
			if(ch == '\b') return "\\b";
			if(ch == '\f') return "\\f";
			return Convert.ToString(ch);
		}
		public static string EscapeString(string str) {
			if(string.IsNullOrEmpty(str))
				return string.Empty;
			string result = string.Empty;
			for(int i = 0; i < str.Length; i++)
				result += ReplaceEscapeChar(str[i]);
			return result;
		}
		public static string HexCharAsByte(char ch) {
			byte[] bytes = BitConverter.GetBytes(ch);
			return bytes[0].ToString("X2", CultureInfo.CurrentCulture);
		}
		public static string HexChar(char ch) {
			byte[] bytes = BitConverter.GetBytes(ch);
			string result = bytes[1].ToString("X2", CultureInfo.CurrentCulture);
			result += bytes[0].ToString("X2", CultureInfo.CurrentCulture);
			return result;
		}
		public static string ClearSpaces(string str) {
			StringBuilder result = new StringBuilder();
			for(int i = 0; i < str.Length; i++)
				if(str[i] != ' ' && str[i] != '\x000D' && str[i] != '\x000A')
					result.Append(str[i]);
			return result.ToString();
		}
		public static byte[] GetIsoBytes(string text) {
			if(string.IsNullOrEmpty(text))
				return new byte[0];
			byte[] bytes = new byte[text.Length];
			for(int i = 0; i < text.Length; i++)
				bytes[i] = (byte)text[i];
			return bytes;
		}
		public static string GetIsoString(byte[] bytes) {
			if(bytes == null || bytes.Length == 0)
				return string.Empty;
			StringBuilder builder = new StringBuilder();
			foreach(byte b in bytes)
				builder.Append((char)b);
			return builder.ToString();
		}
		public static string ArrayToHexadecimalString(byte[] data) {
			if(data == null || data.Length == 0)
				return string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			foreach(byte b in data)
				stringBuilder.Append(b.ToString("X2"));
			return stringBuilder.ToString();
		}
	}
}
