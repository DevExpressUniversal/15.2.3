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

namespace DevExpress.Utils.OAuth {
	using System;
	using System.Text;
	using System.Globalization;
	public static class Escaping {
		static readonly byte[] s_NoBytes = new byte[] { };
		static readonly byte[] s_HexChars = new byte[] { 
					(byte)'0', 
					(byte)'1', 
					(byte)'2', 
					(byte)'3', 
					(byte)'4', 
					(byte)'5', 
					(byte)'6', 
					(byte)'7', 
					(byte)'8', 
					(byte)'9', 
					(byte)'A', 
					(byte)'B', 
					(byte)'C', 
					(byte)'D', 
					(byte)'E', 
					(byte)'F', 
				};
		public static bool IsHex(byte c) {
			return (c >= 'a' && c <= 'f') ||
				   (c >= 'A' && c <= 'F') ||
				   (c >= '0' && c <= '9');
		}
		public static bool IsUnreserved(byte c) {
			return (c >= 'a' && c <= 'z') ||
				   (c >= 'A' && c <= 'Z') ||
				   (c >= '0' && c <= '9') ||
				   (c == '-') ||
				   (c == '_') ||
				   (c == '.') ||
				   (c == '~');
		}
		public static string Unescape(string input) { return Unescape(input, true, false); }
		public static string Unescape(string input, bool unescape, bool unquote) {
			if (input == null || input.Length <= 0) {
				return String.Empty;
			}
			return Unescape(input, 0, input.Length, unescape, unquote);
		}
		public static string Unescape(string input, int index, int count) { return Unescape(input, index, count, true, false); }
		public static string Unescape(string input, int index, int count, bool unescape, bool unquote) {
			if (input == null) {
				throw new ArgumentNullException("token");
			}
			int length = input.Length;
			if (index < 0) {
				throw new ArgumentOutOfRangeException("index");
			}
			if (index > length) {
				throw new ArgumentOutOfRangeException("index");
			}
			if (length < 0) {
				throw new ArgumentOutOfRangeException("count");
			}
			if (index > (length - count)) {
				throw new ArgumentOutOfRangeException("count");
			}
			if (!unescape) {
				return input.Substring(index, count);
			}
			byte[] buffer = new byte[count];
			int run = 0;
			int i = index;
			while (i < index + count) {				
				byte b = (byte)input[i];
				int i1 = i + 1;
				int i2 = i + 2;
				if (b == '%' && i1 < index + count && i2 < index + count
						&& IsHex((byte)input[i1]) && IsHex((byte)input[i2])) {
					string hex
						= String.Format("{0}{1}", input[i1], input[i2]);
					buffer[run] = (byte)int.Parse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					i = i + 3;
				} else {
					buffer[run] = b;
					i = i + 1;
				}
				run++;
			}
			int start = 0;
			int end = run - 1;
			while (end >= 0 && char.IsWhiteSpace((char)buffer[end])) {
				end--;
			}
			while (start < end && char.IsWhiteSpace((char)buffer[start])) {
				start++;
			}
			if (unquote) {
				if (start >= 0 && start < end && buffer[start] == '"'
								&& end < run && buffer[end] == '"') {
					start++;
					end--;
				}
			}
			int len = (end - start) + 1;
			if (len <= 0) {
				return String.Empty;
			}
			return Encoding.UTF8.GetString(buffer, start, len);
		}
		public static string Escape(string value) {
			byte[] output
				= Escape(value, Encoding.UTF8);
			return Encoding.UTF8.GetString(output, 0, output.Length);
		}
		public static byte[] Escape(string value, Encoding encoding) {
			if (encoding == null) {
				throw new ArgumentNullException("encoding");
			}
			if (String.IsNullOrEmpty(value)) {
				return s_NoBytes;
			}
			byte[] input = encoding.GetBytes(value);
			byte[] buff = new byte[input.Length * 3];
			int length = 0;
			for (int i = 0; i < input.Length; i++) {
				byte ch = input[i];
				if (!IsUnreserved(ch)) {
					buff[length++] = (byte)'%';
					buff[length++] = s_HexChars[((ch >> 4) & 0x0F)];
					buff[length++] = s_HexChars[(ch & 0x0F)];
				} else {
					buff[length++] = ch;
				}
			}
			byte[] output = new byte[length];
			Buffer.BlockCopy(
				buff,
				0,
				output,
				0,
				length);
			return output;
		}
	}
}
