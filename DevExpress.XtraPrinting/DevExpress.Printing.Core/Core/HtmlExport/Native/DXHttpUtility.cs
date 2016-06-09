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
using System.Text;
using System.IO;
using System.Globalization;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.HtmlExport.Native {
	public sealed class DXHttpUtility {
		static char[] entityEndingChars = new char[] { ';', '&' };
		internal static string AspCompatUrlEncode(string url) {
			url = UrlEncode(url);
			url = url.Replace("!", "%21");
			url = url.Replace("*", "%2A");
			url = url.Replace("(", "%28");
			url = url.Replace(")", "%29");
			url = url.Replace("-", "%2D");
			url = url.Replace(".", "%2E");
			url = url.Replace("_", "%5F");
			url = url.Replace(@"\", "%5C");
			return url;
		}
		internal static string CollapsePercentUFromStringInternal(string str, Encoding encoding) {
			UrlDecoder decoder = new UrlDecoder(str.Length, encoding);
			if(str.IndexOf("%u", StringComparison.Ordinal) == -1)
				return str;
			for(int i = 0; i < str.Length; i++) {
				char chr = str[i];
				if(chr == '%' && i < (str.Length - 5) && str[i + 1] == 'u') {
					int num4 = HexToInt(str[i + 2]);
					int num5 = HexToInt(str[i + 3]);
					int num6 = HexToInt(str[i + 4]);
					int num7 = HexToInt(str[i + 5]);
					if(num4 >= 0 && num5 >= 0 && num6 >= 0 && num7 >= 0) {
						chr = (char)((((num4 << 12) | (num5 << 8)) | (num6 << 4)) | num7);
						i += 5;
						decoder.AddChar(chr);
						continue;
					}
				}
				if((chr & 0xff80) == 0)
					decoder.AddByte((byte)chr);
				else
					decoder.AddChar(chr);
			}
			return decoder.GetString();
		}
		internal static string FormatHttpCookieDateTime(DateTime value) {
			if(value < DateTime.MaxValue.AddDays(-1.0) && value > DateTime.MinValue.AddDays(1.0))
				value = value.ToUniversalTime();
			return value.ToString("ddd, dd-MMM-yyyy HH':'mm':'ss 'GMT'", DateTimeFormatInfo.InvariantInfo);
		}
		internal static string FormatHttpDateTime(DateTime value) {
			if(value < DateTime.MaxValue.AddDays(-1.0) && value > DateTime.MinValue.AddDays(1.0))
				value = value.ToUniversalTime();
			return value.ToString("R", DateTimeFormatInfo.InvariantInfo);
		}
		internal static string FormatHttpDateTimeUtc(DateTime value) {
			return value.ToString("R", DateTimeFormatInfo.InvariantInfo);
		}
		internal static string FormatPlainTextAsHtml(string str) {
			if(str == null)
				return null;
			StringBuilder builder = new StringBuilder();
			using(StringWriter output = new StringWriter(builder))
				FormatPlainTextAsHtml(str, output);
			return builder.ToString();
		}
		internal static void FormatPlainTextAsHtml(string text, TextWriter output) {
			if(text == null)
				return;
			char previousChar = '\0';
			foreach(char chr in text) {
				switch(chr) {
					case '\n':
						output.Write("<br>");
						break;
					case '\r':
						break;
					case ' ':
						if(previousChar != ' ')
							break;
						output.Write("&nbsp;");
						break;
					case '"':
						output.Write("&quot;");
						break;
					case '&':
						output.Write("&amp;");
						break;
					case '<':
						output.Write("&lt;");
						break;
					case '>':
						output.Write("&gt;");
						break;
					default:
						if(chr >= '\x00a0' && chr < 'Ā') {
							output.Write("&#");
							output.Write(((int)chr).ToString(NumberFormatInfo.InvariantInfo));
							output.Write(';');
						} else
							output.Write(chr);
						break;
				}
				previousChar = chr;
			}
		}
		internal static string FormatPlainTextSpacesAsHtml(string text) {
			if(text == null)
				return null;
			StringBuilder builder = new StringBuilder();
			using(StringWriter writer = new StringWriter(builder))
				foreach(char chr in text)
					if(chr == ' ')
						writer.Write("&nbsp;");
					else
						writer.Write(chr);
			return builder.ToString();
		}
		private static int HexToInt(char value) {
			if(value >= '0' && value <= '9')
				return value - '0';
			if(value >= 'a' && value <= 'f')
				return value - 'a' + 10;
			if(value >= 'A' && value <= 'F')
				return value - 'A' + 10;
			return -1;
		}
		public static string HtmlAttributeEncode(string value) {
			if(value == null)
				return null;
			int htmlAttributeEncodingIndex = IndexOfHtmlAttributeEncodingChars(value, 0);
			if(htmlAttributeEncodingIndex == -1)
				return value;
			StringBuilder builder = new StringBuilder(value.Length + 5);
			int htmlAttributeEncodingStartIndex = 0;
			while(true) {
				if(htmlAttributeEncodingIndex > htmlAttributeEncodingStartIndex)
					builder.Append(value, htmlAttributeEncodingStartIndex, htmlAttributeEncodingIndex - htmlAttributeEncodingStartIndex);
				switch(value[htmlAttributeEncodingIndex]) {
					case '"':
						builder.Append("&quot;");
						break;
					case '&':
						builder.Append("&amp;");
						break;
					case '<':
						builder.Append("&lt;");
						break;
				}
				htmlAttributeEncodingStartIndex = htmlAttributeEncodingIndex + 1;
				if(htmlAttributeEncodingStartIndex < value.Length) {
					htmlAttributeEncodingIndex = IndexOfHtmlAttributeEncodingChars(value, htmlAttributeEncodingStartIndex);
					if(htmlAttributeEncodingIndex != -1)
						continue;
					builder.Append(value, htmlAttributeEncodingStartIndex, value.Length - htmlAttributeEncodingStartIndex);
				}
				break;
			}
			return builder.ToString();
		}
		public static void HtmlAttributeEncode(string html, TextWriter output) {
			if(html == null)
				return;
			int htmlAttributeEncodingIndex = IndexOfHtmlAttributeEncodingChars(html, 0);
			if(htmlAttributeEncodingIndex == -1) {
				output.Write(html);
				return;
			}
			int count = html.Length - htmlAttributeEncodingIndex;
			int index = 0;
			while(htmlAttributeEncodingIndex-- > 0) {
				index++;
				output.Write(html[index]);
			}
			while(count-- > 0) {
				index++;
				char chr = html[index];
				if(chr > '<') {
					output.Write(chr);
					return;
				}
				if(chr != '"') {
					if(chr == '&')
						output.Write("&amp;");
					else if(chr != '<')
						output.Write(chr);
					else
						output.Write("&lt;");
				} else
					output.Write("&quot;");
			}
		}
		internal static void HtmlAttributeEncodeInternal(string html, TextWriter writer) {
			int num = IndexOfHtmlAttributeEncodingChars(html, 0);
			if(num == -1) {
				writer.Write(html);
				return;
			}
			int index = 0;
			while(true) {
				if(num > index) {
					writer.Write(html.Substring(index, num - index));
				}
				switch(html[num]) {
					case '"':
						writer.Write("&quot;");
						break;
					case '&':
						writer.Write("&amp;");
						break;
					case '<':
						writer.Write("&lt;");
						break;
				}
				index = num + 1;
				if(index < html.Length) {
					num = IndexOfHtmlAttributeEncodingChars(html, index);
					if(num != -1)
						continue;
					writer.Write(html.Substring(index, html.Length - index));
				}
				break;
			}
		}
		public static string HtmlDecode(string value) {
			if(value == null)
				return null;
			if(value.IndexOf('&') < 0)
				return value;
			StringBuilder builder = new StringBuilder();
			using(StringWriter output = new StringWriter(builder))
				HtmlDecode(value, output);
			return builder.ToString();
		}
		public static void HtmlDecode(string html, TextWriter output) {
			if(html == null)
				return;
			if(html.IndexOf('&') < 0) {
				output.Write(html);
				return;
			}
			for(int i = 0; i < html.Length; i++) {
				char chr = html[i];
				if(chr == '&') {
					int entityEndingIndex = html.IndexOfAny(entityEndingChars, i + 1);
					if(entityEndingIndex > 0 && html[entityEndingIndex] == ';') {
						string entity = html.Substring(i + 1, (entityEndingIndex - i) - 1);
						if(entity.Length > 1 && entity[0] == '#') {
							try {
								if(entity[1] == 'x' || entity[1] == 'X')
									chr = (char)int.Parse(entity.Substring(2), NumberStyles.AllowHexSpecifier);
								else
									chr = (char)int.Parse(entity.Substring(1));
								i = entityEndingIndex;
							} catch(FormatException) {
								i++;
							} catch(ArgumentException) {
								i++;
							}
						} else {
							i = entityEndingIndex;
							char entityChar = DXHtmlEntities.Lookup(entity);
							if(entityChar != '\0')
								chr = entityChar;
							else {
								output.Write('&');
								output.Write(entity);
								output.Write(';');
								continue;
							}
						}
					}
				}
				output.Write(chr);
			}
		}
		public static string HtmlEncode(string value) {
			if(value == null)
				return null;
			int num = IndexOfHtmlEncodingChars(value, 0);
			if(num == -1)
				return value;
			StringBuilder builder = new StringBuilder(value.Length + 5);
			int startIndex = 0;
			while(true)
				if(num > startIndex) {
					builder.Append(value, startIndex, num - startIndex);
				char ch = value[num];
				if(ch > '>') {
					builder.Append("&#");
					builder.Append(((int)ch).ToString(NumberFormatInfo.InvariantInfo));
					builder.Append(';');
				} else {
					if(ch != '"') {
						switch(ch) {
							case '<':
								builder.Append("&lt;");
								break;
							case '>':
								builder.Append("&gt;");
								break;
							case '&':
								builder.Append("&amp;");
								break;
						}
					} else
						builder.Append("&quot;");
				}
				startIndex = num + 1;
				if(startIndex < value.Length) {
					num = IndexOfHtmlEncodingChars(value, startIndex);
					if(num != -1)
						continue;
					builder.Append(value, startIndex, value.Length - startIndex);
				}
				break;
			}
			return builder.ToString();
		}
		public static void HtmlEncode(string html, TextWriter output) {
			if(html == null)
				return;
			int htmlEncodingCharsIndex = IndexOfHtmlEncodingChars(html, 0);
			if(htmlEncodingCharsIndex == -1) {
				output.Write(html);
				return;
			}
			int count = html.Length - htmlEncodingCharsIndex;
			int index = 0;
			while(htmlEncodingCharsIndex-- > 0)
				output.Write(html[++index]);
			while(count-- > 0) {
				char chr = html[++index];
				if(chr <= '>') {
					if(chr != '"') {
						switch(chr) {
							case '<':
								output.Write("&lt;");
								break;
							case '=':
								output.Write(chr);
								break;
							case '>':
								output.Write("&gt;");
								break;
							case '&':
								output.Write("&amp;");
								break;
							default:
								output.Write(chr);
								break;
						}
						continue;
					}
					output.Write("&quot;");
				} else {
					if(chr >= '\x00a0' && chr < 'Ā') {
						output.Write("&#");
						output.Write(((int)chr).ToString(NumberFormatInfo.InvariantInfo));
						output.Write(';');
					} else
						output.Write(chr);
				}
			}
		}
		private static int IndexOfHtmlAttributeEncodingChars(string html, int startPosition) {
			int count = html.Length - startPosition;
			int index = startPosition;
			while(count > 0) {
				char chr = html[index];
				if(chr <= '<') {
					switch(chr) {
						case '"':
						case '&':
						case '<':
							return html.Length - count;
					}
				}
				index++;
				count--;
			}
			return -1;
		}
		private static int IndexOfHtmlEncodingChars(string str, int startPosition) {
			int num = str.Length - startPosition;
			int index = 0;
			while(num > 0) {
				char chr = str[index];
				if(chr <= '>') {
					switch(chr) {
						case '<':
						case '>':
						case '"':
						case '&':
							return str.Length - num;
					}
				} else if(chr >= '\x00a0' && chr < 'Ā')
					return str.Length - num;
				index++;
				num--;
			}
			return -1;
		}
		internal static char IntToHex(int value) {
			if(value <= 9)
				return (char)(value + 0x30);
			return (char)(value - 10 + 0x61);
		}
		private static bool IsNonAsciiByte(byte value) {
			if(value < 0x7f)
				return value < 0x20;
			return true;
		}
		internal static bool IsSafe(char value) {
			if((value >= 'a' && value <= 'z') || (value >= 'A' && value <= 'Z') || (value >= '0' && value <= '9'))
				return true;
			switch(value) {
				case '\'':
				case '(':
				case ')':
				case '*':
				case '-':
				case '.':
				case '_':
				case '!':
					return true;
			}
			return false;
		}
		public static string UrlDecode(string value) {
			if(value == null)
				return null;
			return UrlDecode(value, Encoding.UTF8);
		}
		public static string UrlDecode(byte[] bytes, Encoding encoding) {
			if(bytes == null)
				return null;
			return UrlDecode(bytes, 0, bytes.Length, encoding);
		}
		public static string UrlDecode(string url, Encoding encoding) {
			if(url == null)
				return null;
			return UrlDecodeStringFromStringInternal(url, encoding);
		}
		public static string UrlDecode(byte[] bytes, int offset, int count, Encoding encoding) {
			if(count == 0)
				return null;
			Guard.ArgumentNotNull(bytes, "bytes");
			if(offset < 0 || offset > bytes.Length)
				throw new ArgumentOutOfRangeException("offset");
			if(count < 0 || (offset + count) > bytes.Length)
				throw new ArgumentOutOfRangeException("count");
			return UrlDecodeStringFromBytesInternal(bytes, offset, count, encoding);
		}
		private static byte[] UrlDecodeBytesFromBytesInternal(byte[] buffer, int offset, int count) {
			int length = 0;
			byte[] sourceArray = new byte[count];
			for(int i = 0; i < count; i++) {
				int index = offset + i;
				byte currentByte = buffer[index];
				if(currentByte == 0x2b)
					currentByte = 0x20;
				else if((currentByte == 0x25) && (i < (count - 2))) {
					int byte1 = HexToInt((char)buffer[index + 1]);
					int byte2 = HexToInt((char)buffer[index + 2]);
					if((byte1 >= 0) && (byte2 >= 0)) {
						currentByte = (byte)((byte1 << 4) | byte2);
						i += 2;
					}
				}
				sourceArray[length++] = currentByte;
			}
			if(length < sourceArray.Length) {
				byte[] destinationArray = new byte[length];
				Array.Copy(sourceArray, destinationArray, length);
				sourceArray = destinationArray;
			}
			return sourceArray;
		}
		private static string UrlDecodeStringFromBytesInternal(byte[] buffer, int offset, int count, Encoding encoding) {
			UrlDecoder decoder = new UrlDecoder(count, encoding);
			for(int i = 0; i < count; i++) {
				int index = offset + i;
				byte currentByte = buffer[index];
				if(currentByte == 0x2b)
					currentByte = 0x20;
				else if(currentByte == 0x25 && i < (count - 2)) {
					if(buffer[index + 1] == 0x75 && i < (count - 5)) {
						int byte2 = HexToInt((char)buffer[index + 2]);
						int byte3 = HexToInt((char)buffer[index + 3]);
						int byte4 = HexToInt((char)buffer[index + 4]);
						int byte5 = HexToInt((char)buffer[index + 5]);
						if(byte2 < 0 || byte3 < 0 || byte4 < 0 || byte5 < 0) {
							decoder.AddByte(currentByte);
							continue;
						}
						char chr = (char)((((byte2 << 12) | (byte3 << 8)) | (byte4 << 4)) | byte5);
						i += 5;
						decoder.AddChar(chr);
						continue;
					}
					int byte6 = HexToInt((char)buffer[index + 1]);
					int byte7 = HexToInt((char)buffer[index + 2]);
					if((byte6 >= 0) && (byte7 >= 0)) {
						currentByte = (byte)((byte6 << 4) | byte7);
						i += 2;
					}
				}
				decoder.AddByte(currentByte);
			}
			return decoder.GetString();
		}
		private static string UrlDecodeStringFromStringInternal(string url, Encoding encoding) {
			UrlDecoder decoder = new UrlDecoder(url.Length, encoding);
			for(int i = 0; i < url.Length; i++) {
				char ch = url[i];
				if(ch == '+')
					ch = ' ';
				else if((ch == '%') && (i < (url.Length - 2))) {
					if((url[i + 1] == 'u') && (i < (url.Length - 5))) {
						int num3 = HexToInt(url[i + 2]);
						int num4 = HexToInt(url[i + 3]);
						int num5 = HexToInt(url[i + 4]);
						int num6 = HexToInt(url[i + 5]);
						if(num3 < 0 || num4 < 0 || num5 < 0 || num6 < 0) {
							if((ch & 0xff80) == 0)
								decoder.AddByte((byte)ch);
							else
								decoder.AddChar(ch);
							continue;
						}
						ch = (char)((((num3 << 12) | (num4 << 8)) | (num5 << 4)) | num6);
						i += 5;
						decoder.AddChar(ch);
						continue;
					}
					int num7 = HexToInt(url[i + 1]);
					int num8 = HexToInt(url[i + 2]);
					if((num7 >= 0) && (num8 >= 0)) {
						byte b = (byte)((num7 << 4) | num8);
						i += 2;
						decoder.AddByte(b);
						continue;
					}
				}
				if((ch & 0xff80) == 0)
					decoder.AddByte((byte)ch);
				else
					decoder.AddChar(ch);
			}
			return decoder.GetString();
		}
		public static byte[] UrlDecodeToBytes(byte[] value) {
			if(value == null)
				return null;
			return UrlDecodeToBytes(value, 0, value != null ? value.Length : 0);
		}
		public static byte[] UrlDecodeToBytes(string url) {
			if(url == null)
				return null;
			return UrlDecodeToBytes(url, Encoding.UTF8);
		}
		public static byte[] UrlDecodeToBytes(string url, Encoding encoding) {
			if(url == null)
				return null;
			return UrlDecodeToBytes(encoding.GetBytes(url));
		}
		public static byte[] UrlDecodeToBytes(byte[] bytes, int offset, int count) {
			if(count == 0)
				return null;
			Guard.ArgumentNotNull(bytes, "bytes");
			if(offset < 0 || offset > bytes.Length)
				throw new ArgumentOutOfRangeException("offset");
			if(count < 0 || (offset + count) > bytes.Length)
				throw new ArgumentOutOfRangeException("count");
			return UrlDecodeBytesFromBytesInternal(bytes, offset, count);
		}
		public static string UrlEncode(byte[] bytes) {
			if (bytes == null)
				return null;
			byte[] urlBytes = UrlEncodeToBytes(bytes);
			return DXEncoding.ASCII.GetString(urlBytes, 0, urlBytes.Length);
		}
		public static string UrlEncode(string url) {
			if(url == null)
				return null;
			return UrlEncode(url, Encoding.UTF8);
		}
		public static string UrlEncode(string url, Encoding encoding) {
			if (url == null)
				return null;
			byte[] urlBytes = UrlEncodeToBytes(url, encoding);
			return DXEncoding.ASCII.GetString(urlBytes, 0, urlBytes.Length);
		}
		public static string UrlEncode(byte[] bytes, int offset, int count) {
			if (bytes == null)
				return null;
			byte[] urlBytes = UrlEncodeToBytes(bytes, offset, count);
			return DXEncoding.ASCII.GetString(urlBytes, 0, urlBytes.Length);
		}
		private static byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue) {
			int spacesCount = 0;
			int unsafeCharsCount = 0;
			for(int i = 0; i < count; i++) {
				char chr = (char)bytes[offset + i];
				if(chr == ' ')
					spacesCount++;
				else if(!IsSafe(chr))
					unsafeCharsCount++;
			}
			if(!alwaysCreateReturnValue && spacesCount == 0 && unsafeCharsCount == 0)
				return bytes;
			byte[] buffer = new byte[count + unsafeCharsCount * 2];
			int num4 = 0;
			for(int j = 0; j < count; j++) {
				byte num6 = bytes[offset + j];
				char ch2 = (char)num6;
				if(IsSafe(ch2))
					buffer[num4++] = num6;
				else if(ch2 == ' ')
					buffer[num4++] = 0x2b;
				else {
					buffer[num4++] = 0x25;
					buffer[num4++] = (byte)IntToHex((num6 >> 4) & 15);
					buffer[num4++] = (byte)IntToHex(num6 & 15);
				}
			}
			return buffer;
		}
		private static byte[] UrlEncodeBytesToBytesInternalNonAscii(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue) {
			int num = 0;
			for(int i = 0; i < count; i++)
				if(IsNonAsciiByte(bytes[offset + i]))
					num++;
			if(!alwaysCreateReturnValue && (num == 0))
				return bytes;
			byte[] buffer = new byte[count + (num * 2)];
			int num3 = 0;
			for(int j = 0; j < count; j++) {
				byte b = bytes[offset + j];
				if(IsNonAsciiByte(b)) {
					buffer[num3++] = 0x25;
					buffer[num3++] = (byte)IntToHex((b >> 4) & 15);
					buffer[num3++] = (byte)IntToHex(b & 15);
				} else
					buffer[num3++] = b;
			}
			return buffer;
		}
		internal static string UrlEncodeNonAscii(string url, Encoding encoding) {
			if(string.IsNullOrEmpty(url))
				return url;
			if(encoding == null)
				encoding = Encoding.UTF8;
			byte[] bytes = encoding.GetBytes(url);
			bytes = UrlEncodeBytesToBytesInternalNonAscii(bytes, 0, bytes.Length, false);
			return DXEncoding.ASCII.GetString(bytes, 0, bytes.Length);
		}
		public static string UrlEncodeSpaces(string url) {
			if(url != null && url.IndexOf(' ') >= 0)
				url = url.Replace(" ", "%20");
			return url;
		}
		public static byte[] UrlEncodeToBytes(string url) {
			if(url == null)
				return null;
			return UrlEncodeToBytes(url, Encoding.UTF8);
		}
		public static byte[] UrlEncodeToBytes(byte[] bytes) {
			if(bytes == null)
				return null;
			return UrlEncodeToBytes(bytes, 0, bytes.Length);
		}
		public static byte[] UrlEncodeToBytes(string url, Encoding encoding) {
			if(url == null)
				return null;
			byte[] bytes = encoding.GetBytes(url);
			return UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);
		}
		public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count) {
			if(bytes == null && count == 0)
				return null;
			Guard.ArgumentNotNull(bytes, "bytes");
			if(offset < 0 || offset > bytes.Length)
				throw new ArgumentOutOfRangeException("offset");
			if(count < 0 || (offset + count) > bytes.Length)
				throw new ArgumentOutOfRangeException("count");
			return UrlEncodeBytesToBytesInternal(bytes, offset, count, true);
		}
		public static string UrlEncodeUnicode(string url) {
			if(url == null)
				return null;
			return UrlEncodeUnicodeStringToStringInternal(url, false);
		}
		private static string UrlEncodeUnicodeStringToStringInternal(string url, bool ignoreAscii) {
			StringBuilder builder = new StringBuilder(url.Length);
			foreach(char chr in url) {
				if((chr & 0xff80) == 0) {
					if(ignoreAscii || IsSafe(chr)) {
						builder.Append(chr);
					} else if(chr == ' ') {
						builder.Append('+');
					} else {
						builder.Append('%');
						builder.Append(IntToHex((chr >> 4) & '\x000f'));
						builder.Append(IntToHex(chr & '\x000f'));
					}
				} else {
					builder.Append("%u");
					builder.Append(IntToHex((chr >> 12) & '\x000f'));
					builder.Append(IntToHex((chr >> 8) & '\x000f'));
					builder.Append(IntToHex((chr >> 4) & '\x000f'));
					builder.Append(IntToHex(chr & '\x000f'));
				}
			}
			return builder.ToString();
		}
		public static byte[] UrlEncodeUnicodeToBytes(string url) {
			if (url == null)
				return null;
			return DXEncoding.ASCII.GetBytes(UrlEncodeUnicode(url));
		}
		public static string UrlPathEncode(string url) {
			if(url == null)
				return null;
			int argumentsIndex = url.IndexOf('?');
			if(argumentsIndex >= 0)
				return UrlPathEncode(url.Substring(0, argumentsIndex)) + url.Substring(argumentsIndex);
			return UrlEncodeSpaces(UrlEncodeNonAscii(url, Encoding.UTF8));
		}
		internal static bool IsSpecialSymbol(char ch) {
			return ch == '{'
				|| ch == '}'
				|| ch == '\\';
		}
		public static string UrlEncodeToUnicodeCompatible(string stringData) {
			string result = UrlPathEncode(stringData);
			for(int length = 0; length < result.Length; length++) {
				char ch = result[length];
				if(IsSpecialSymbol(ch)) {
					result = result.Insert(length, @"\");
					length++;
				}
			}
			return result;
		}
		class UrlDecoder {
			int bufferSize;
			byte[] byteBuffer;
			char[] charBuffer;
			Encoding encoding;
			int numBytes;
			int numChars;
			internal UrlDecoder(int bufferSize, Encoding encoding) {
				this.bufferSize = bufferSize;
				this.encoding = encoding;
				charBuffer = new char[bufferSize];
			}
			internal void AddByte(byte value) {
				if(byteBuffer == null)
					byteBuffer = new byte[bufferSize];
				byteBuffer[numBytes++] = value;
			}
			internal void AddChar(char value) {
				if(numBytes > 0)
					FlushBytes();
				charBuffer[numChars++] = value;
			}
			private void FlushBytes() {
				if(numBytes <= 0)
					return;
				numChars += encoding.GetChars(byteBuffer, 0, numBytes, charBuffer, numChars);
				numBytes = 0;
			}
			internal string GetString() {
				if(numBytes > 0)
					FlushBytes();
				if(numChars > 0)
					return new string(charBuffer, 0, numChars);
				return string.Empty;
			}
		}
	}
}
