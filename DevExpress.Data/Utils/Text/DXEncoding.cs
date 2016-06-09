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
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
#if !DXWINDOW
using DevExpress.Utils.Internal;
#if !DXPORTABLE
using DevExpress.Data.Helpers;
#endif
#endif
#if !DXPORTABLE
using System.Security.Permissions;
#endif
using System.Security;
using DevExpress.Compatibility.System.Text;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Utils {
#endif
#region DXEncoding
	public static class DXEncoding {
		public static readonly Encoding UTF8NoByteOrderMarks = new UTF8Encoding(false);
#if DXRESTRICTED
		public static readonly Encoding Default = CustomEncoding.Default;
		public static readonly Encoding ASCII = Encoding.ASCII; 
		static readonly Encoding utf32be = Encoding.GetEncoding("utf-32BE");
		public static Encoding GetEncoding(int codePage) {
			if (codePage == 1201)
				return Encoding.BigEndianUnicode;
			else if (codePage == 1200)
				return Encoding.Unicode;
			else if (codePage == 65001)
				return Encoding.UTF8;
			else if (codePage == 12000)
				return Encoding.UTF32;
			else if (codePage == 12001)
				return utf32be;
			Encoding result = CustomEncoding.GetCustomEncodingCore(codePage);
			if (result != null)
				return result;
			result = CodePagesEncodingProvider.Instance.GetEncoding(codePage);
			if (result == null)
				throw new ArgumentException("CodePageNotSupported");
			return result;
		}
		public static Encoding GetEncoding(string name) {
			Encoding result = CustomEncoding.GetCustomEncodingCore(name);
			if (result != null)
				return result;
			return Encoding.GetEncoding(name);
		}
		public static EncodingInfo[] GetEncodings() {
			return CustomEncoding.GetEncodings();
		}
		public static int GetEncodingCodePage(Encoding encoding) {
			CustomEncoding customEncoding = encoding as CustomEncoding;
			if (customEncoding != null)
				return customEncoding.CodePage;
			return encoding.CodePage;
		}
		public static bool IsSingleByteEncoding(Encoding encoding) {
			CustomEncoding customEncoding = encoding as CustomEncoding;
			if (customEncoding != null)
				return customEncoding.IsSingleByte;
			return encoding.IsSingleByte;
		}
#else
		public static readonly Encoding Default = Encoding.Default;
		public static readonly Encoding ASCII = Encoding.ASCII;
		public static Encoding GetEncoding(int codePage) {
			return Encoding.GetEncoding(codePage);
		}
		public static EncodingInfo[] GetEncodings() {
			return Encoding.GetEncodings();
		}
		public static int GetEncodingCodePage(Encoding encoding) {
			return encoding.CodePage;
		}
		public static bool IsSingleByteEncoding(Encoding encoding) {
			return encoding.IsSingleByte;
		}
		public static Encoding GetEncoding(string name) {
			return Encoding.GetEncoding(name);
		}
#endif
#if DXRESTRICTED
		public static Encoding GetEncodingFromCodePage(int codePage) {
			const int symbolCodePage = 42;
			if (codePage == symbolCodePage)
				return EmptyEncoding.Instance;
			else
				return DXEncoding.GetEncoding(codePage);
		}
#else
		public static Encoding GetEncodingFromCodePage(int codePage) {
			const int symbolCodePage = 42;
			if (codePage == symbolCodePage)
				return EmptyEncoding.Instance;
			else
				return Encoding.GetEncoding(codePage);
		}
#endif
		public static int CodePageFromCharset(int charset) {
			return DXCharsetAndCodePageTranslator.Instance.CodePageFromCharset(charset);
		}
		public static int CharsetFromCodePage(int codePage) {
			return DXCharsetAndCodePageTranslator.Instance.CharsetFromCodePage(codePage);
		}
	}
#endregion
#region EmptyEncoding
	public class EmptyEncoding : Encoding {
		static readonly EmptyEncoding instance = new EmptyEncoding();
		public static EmptyEncoding Instance { get { return instance; } }
		public override int GetByteCount(char[] chars, int index, int count) {
			return count;
		}
		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) {
			int maxIndex = charIndex + charCount;
			while (charIndex < maxIndex) {
				char ch = chars[charIndex++];
				if (ch >= '\x0100')
					ch = '?';
				bytes[byteIndex++] = (byte)ch;
			}
			return charCount;
		}
		public override bool IsSingleByte {
			get {
				return true;
			}
		}
		public override int GetCharCount(byte[] bytes, int index, int count) {
			return count;
		}
		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) {
			int maxIndex = byteIndex + byteCount;
			while (byteIndex < maxIndex)
				chars[charIndex++] = (char)bytes[byteIndex++];
			return byteCount;
		}
		public override int GetMaxByteCount(int charCount) {
			return charCount;
		}
		public override int GetMaxCharCount(int byteCount) {
			return byteCount;
		}
	}
#endregion
#region QuotedPrintableEncoding
	public class QuotedPrintableEncoding {
		public static readonly QuotedPrintableEncoding Instance = new QuotedPrintableEncoding();
		public string ToMultilineQuotedPrintableString(string value, int maxLineWidth) {
			string[] lines = GetPlainTextLines(value);
			StringBuilder sb = new StringBuilder();
			int count = lines.Length;
			for (int i = 0; i < count; i++) {
				if (i > 0) {
					sb.Append("\r\n");
				}
				sb.Append(ToMultilineQuotedPrintableStringCore(lines[i], maxLineWidth - 1));
			}
			return sb.ToString();
		}
		public string FromMultilineQuotedPrintableString(string value) {
			string[] lines = GetPlainTextLines(value);
			StringBuilder sb = new StringBuilder();
			int count = lines.Length;
			for (int i = 0; i < count; i++) {
				string line = lines[i];
				if (line.EndsWith("="))
					sb.Append(line.Substring(0, line.Length - 1));
				else {
					if (i < count - 1) {
						sb.Append(line);
						sb.Append("\r\n");
					}
					else
						sb.Append(line);
				}
			}
			return sb.ToString();
		}
		public string ToQuotedPrintableString(string value, Encoding encoding) {
			return ToQuotedPrintableString(encoding.GetBytes(value));
		}
		public string ToQuotedPrintableString(byte[] bytes) {
			StringBuilder sb = new StringBuilder();
			int count = bytes.Length;
			for (int i = 0; i < count; i++)
				sb.Append(ConvertToQuotedPrintable(bytes[i]));
			return sb.ToString();
		}
		public string FromQuotedPrintableString(string value, Encoding encoding) {
			List<byte> bytes = new List<byte>();
			int count = value.Length;
			for (int i = 0; i < count; i++) {
				char ch = value[i];
				if (ch != '=')
					AppendCharNoDecode((byte)ch, bytes);
				else
					i += DecodeQuotedChar(value, i, encoding, bytes);
			}
			byte[] byteArray = bytes.ToArray();
			return encoding.GetString(byteArray, 0, byteArray.Length);
		}
		public string FromQuotedPrintableString(string value) {
			StringBuilder sb = new StringBuilder();
			int count = value.Length;
			for (int i = 0; i < count; i++) {
				char ch = value[i];
				if (ch != '=')
					AppendCharNoDecode(ch, sb);
				else
					i += DecodeQuotedChar(value, i, sb);
			}
			return sb.ToString();
		}
		protected internal virtual void AppendCharNoDecode(char ch, StringBuilder sb) {
			sb.Append(ch);
		}
		protected internal virtual void AppendCharNoDecode(byte ch, List<byte> sb) {
			sb.Add(ch);
		}
		protected internal string ToMultilineQuotedPrintableStringCore(string value, int maxLineWidth) {
			StringBuilder sb = new StringBuilder();
			int count = value.Length;
			for (int i = 0; i < count; i += maxLineWidth) {
				if (i > 0) {
					sb.Append("=");
					sb.Append("\r\n");
				}
				string line = value.Substring(i, Math.Min(count - i, maxLineWidth));
				int equalSignIndex = line.LastIndexOf('=');
				if (equalSignIndex >= 0) {
					int lengthToEqualSign = line.Length - equalSignIndex;
					if (lengthToEqualSign <= 3 && equalSignIndex + 3 > maxLineWidth) {
						i -= lengthToEqualSign;
						line = line.Substring(0, line.Length - lengthToEqualSign);
					}
				}
				sb.Append(line);
			}
			return sb.ToString();
		}
		protected internal bool ShouldConvertByte(byte value) {
			return value <= 32 || value >= 128 || value == (byte)'=';
		}
		protected internal string ConvertToQuotedPrintable(byte value) {
			if (ShouldConvertByte(value))
				return String.Format("={0:X2}", value);
			else
				return new String((char)value, 1);
		}
		protected internal int DecodeQuotedChar(string value, int index, Encoding encoding, List<byte> target) {
			if (index + 2 >= value.Length) { 
				target.Add((byte)value[index]);
				return 0;
			}
			string hexValue = value.Substring(index + 1, 2);
			int charValue;
			if (!Int32.TryParse(hexValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out charValue)) {
				target.Add((byte)value[index]);
				return 0;
			}
			target.Add((byte)charValue);
			return 2;
		}
		protected internal int DecodeQuotedChar(string value, int index, StringBuilder target) {
			if (index + 2 >= value.Length) { 
				target.Append(value[index]);
				return 0;
			}
			string hexValue = value.Substring(index + 1, 2);
			int charValue;
			if (!Int32.TryParse(hexValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out charValue)) {
				target.Append(value[index]);
				return 0;
			}
			target.Append((char)charValue);
			return 2;
		}
		static readonly String[] newLineStrings = new String[] { "\r\n", "\n\r", "\r", "\n" };
		public static string[] GetPlainTextLines(string text) {
			if (String.IsNullOrEmpty(text))
				return new String[] { String.Empty };
			return text.Split(newLineStrings, StringSplitOptions.None);
		}
	}
#endregion
#region QEncoding
	public class QEncoding : QuotedPrintableEncoding {
		public static new readonly QEncoding Instance = new QEncoding();
		protected internal override void AppendCharNoDecode(char ch, StringBuilder sb) {
			if (ch == '_')
				base.AppendCharNoDecode('\x20', sb);
			else
				base.AppendCharNoDecode(ch, sb);
		}
		protected internal override void AppendCharNoDecode(byte ch, List<byte> sb) {
			if (ch == (byte)'_')
				base.AppendCharNoDecode((byte)'\x20', sb);
			else
				base.AppendCharNoDecode(ch, sb);
		}
	}
#endregion
}
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Utils.Internal {
#endif
#region DXCharsetAndCodePageTranslator (abstract class)
	public abstract class DXCharsetAndCodePageTranslator {
		static DXCharsetAndCodePageTranslator instance;
		public static DXCharsetAndCodePageTranslator Instance {
			get {
				if (instance == null)
					instance = CreateInstance();
				return instance;
			}
		}
		public static void ClearInstance() {
			instance = null;
		}
		static DXCharsetAndCodePageTranslator CreateInstance() {
#if !DXPORTABLE
			if (SecurityHelper.IsPermissionGranted(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode)))
				return new FullTrustCharsetAndCodePageTranslator();
			else
				return new PartialTrustCharsetAndCodePageTranslator();
#else
			return new PartialTrustCharsetAndCodePageTranslator();
#endif
		}
		public abstract int CodePageFromCharset(int charset);
		public abstract int CharsetFromCodePage(int codePage);
	}
	#endregion
#if !DXPORTABLE
	#region FullTrustCharsetAndCodePageTranslator
	public class FullTrustCharsetAndCodePageTranslator : DXCharsetAndCodePageTranslator {
	#region TranslateCharsetInfo
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Size = (4 + 2) * 4)]
		struct FONTSIGNATURE {
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public Int32[] fsUsb;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public Int32[] fsCsb;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		struct CHARSETINFO {
			public int ciCharset;
			public int ciACP;
			[MarshalAs(UnmanagedType.Struct)]
			public FONTSIGNATURE fSig;
		}
		const int TCI_SRCCHARSET = 1;
		const int TCI_SRCCODEPAGE = 2;
		const int TCI_SRCFONTSIG = 3;
		const int TCI_SRCLOCALE = 0x1000;
		[DllImport("Gdi32.dll")]
		extern static int TranslateCharsetInfo([In, Out] IntPtr pSrc, [In, Out] ref CHARSETINFO lpSc, [In] int dwFlags);
	#endregion
		static Dictionary<int, int> charsetToCodePage = new Dictionary<int, int>();
		static Dictionary<int, int> codePageToCharset = new Dictionary<int, int>();
		[SecuritySafeCritical]
		int CodePageFromCharsetCore(int charset) {
			IntPtr pSrc = new IntPtr(charset);
			CHARSETINFO charSetInfo = new CHARSETINFO();
			int b = TranslateCharsetInfo(pSrc, ref charSetInfo, TCI_SRCCHARSET);
			if (b == 0)
				return System.Text.Encoding.Default.CodePage;
			pSrc = IntPtr.Zero;
			return charSetInfo.ciACP;
		}
		[SecuritySafeCritical]
		int CharsetFromCodePageCore(int codePage) {
			IntPtr pSrc = new IntPtr(codePage);
			CHARSETINFO charSetInfo = new CHARSETINFO();
			int b = TranslateCharsetInfo(pSrc, ref charSetInfo, TCI_SRCCODEPAGE);
			if (b == 0)
				return 0;
			pSrc = IntPtr.Zero;
			return charSetInfo.ciCharset;
		}
		public override int CodePageFromCharset(int charset) {
			int result;
			if (!charsetToCodePage.TryGetValue(charset, out result)) {
				result = CodePageFromCharsetCore(charset);
				lock (charsetToCodePage) {
					charsetToCodePage[charset] = result; 
				}
			}
			return result;
		}
		public override int CharsetFromCodePage(int codePage) {
			int result;
			if (!codePageToCharset.TryGetValue(codePage, out result)) {
				result = CharsetFromCodePageCore(codePage);
				lock (codePageToCharset) {
					codePageToCharset[codePage] = result; 
				}
			}
			return result;
		}
	}
	#endregion
#endif
	#region PartialTrustCharsetAndCodePageTranslator
	public class PartialTrustCharsetAndCodePageTranslator : DXCharsetAndCodePageTranslator {
		static Dictionary<int, int> CharsetToCodePage = InitializeCharsetTable();
		static Dictionary<int, int> InitializeCharsetTable() {
			Dictionary<int, int> result = new Dictionary<int, int>();
			result.Add(0, 1252);
			result.Add(2, 42);
			result.Add(128, 932);
			result.Add(129, 949);
			result.Add(130, 1361);
			result.Add(134, 936);
			result.Add(136, 950);
			result.Add(161, 1253);
			result.Add(162, 1254);
			result.Add(163, 1258);
			result.Add(177, 1255);
			result.Add(178, 1256);
			result.Add(186, 1257);
			result.Add(204, 1251);
			result.Add(222, 874);
			result.Add(238, 1250);
			return result;
		}
		public override int CodePageFromCharset(int charset) {
			int codepage = -1;
			if (CharsetToCodePage.TryGetValue(charset, out codepage))
				return codepage;
			else
				return DXEncoding.GetEncodingCodePage(DXEncoding.Default);
		}
		public override int CharsetFromCodePage(int codePage) {
			foreach (int key in CharsetToCodePage.Keys) {
				if (CharsetToCodePage[key] == codePage)
					return key;
			}
			return 0;
		}
	}
#endregion
}
