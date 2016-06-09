#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using System.Net.Mime;
using System.Text;
using DevExpress.Web.Internal;
namespace DevExpress.XtraReports.Web.Native {
	static class ContentDispositionUtil {
		const string hexDigits = "0123456789ABCDEF";
		static void AddByteToStringBuilder(byte b, StringBuilder builder) {
			builder.Append('%');
			int i = b;
			AddHexDigitToStringBuilder(i >> 4, builder);
			AddHexDigitToStringBuilder(i % 16, builder);
		}
		static void AddHexDigitToStringBuilder(int digit, StringBuilder builder) {
			builder.Append(hexDigits[digit]);
		}
		static string CreateRfc2231HeaderValue(string dispositionType, string filename) {
			return CreateHeaderValue(dispositionType, "filename*=UTF-8''", GetEncodedString(filename));
		}
		static string CreateSimpleEncodedHeaderValue(string dispositionType, string filename) {
			return CreateHeaderValue(dispositionType, "filename=", GetEncodedString(filename));
		}
		static string CreatePlainHeaderValue(string dispositionType, string filename) {
			return CreateHeaderValue(dispositionType, "filename=", filename);
		}
		static string CreateHeaderValue(string dispositionType, string filenamePrefix, string value) {
			return dispositionType + " ; " + filenamePrefix + value;
		}
		static string GetEncodedString(string filename) {
			var builder = new StringBuilder();
			byte[] filenameBytes = Encoding.UTF8.GetBytes(filename);
			foreach(byte b in filenameBytes) {
				if(IsByteValidHeaderValueCharacter(b)) {
					builder.Append((char)b);
				} else {
					AddByteToStringBuilder(b, builder);
				}
			}
			return builder.ToString();
		}
		internal static string GetHeaderValue(string dispositionType, string fileName) {
			foreach(char ch in fileName) {
				if(ch > '\x007f')
					return CustomEncode(dispositionType, fileName);
			}
			return EncodeByContentDisposition(dispositionType, fileName);
		}
		static string EncodeByContentDisposition(string dispositionType, string fileName) {
			var contentDisposition = new ContentDisposition {
				FileName = fileName,
				DispositionType = dispositionType
			};
			return contentDisposition.ToString();
		}
		static string CustomEncode(string dispositionType, string fileName) {
			if(RenderUtils.Browser.IsIE && RenderUtils.Browser.Version < 9)
				return CreateSimpleEncodedHeaderValue(dispositionType, fileName);
			if(RenderUtils.Browser.IsSafari)
				return CreatePlainHeaderValue(dispositionType, fileName);
			return CreateRfc2231HeaderValue(dispositionType, fileName);
		}
		static bool IsByteValidHeaderValueCharacter(byte b) {
			if((byte)'0' <= b && b <= (byte)'9') {
				return true;
			}
			if((byte)'a' <= b && b <= (byte)'z') {
				return true;
			}
			if((byte)'A' <= b && b <= (byte)'Z') {
				return true;
			}
			switch(b) {
				case (byte)'-':
				case (byte)'.':
				case (byte)'_':
				case (byte)'~':
				case (byte)':':
				case (byte)'!':
				case (byte)'$':
				case (byte)'&':
				case (byte)'+':
					return true;
			}
			return false;
		}
	}
}
