#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfDocumentWritableStream {
		const string doubleMask = "0.################";
		const byte zero = (byte)'0';
		static readonly Encoding utf8encoding = Encoding.UTF8;
		static readonly Encoding unicodeEncoding = Encoding.BigEndianUnicode;
		static readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;
		readonly Stream stream;
		PdfEncryptionInfo encryptionInfo;
		protected Stream Stream { get { return stream; } }
		public PdfEncryptionInfo EncryptionInfo {
			get { return encryptionInfo; }
			set { encryptionInfo = value; }
		}
		public PdfDocumentWritableStream(Stream stream) {
			this.stream = stream;
		}
		public void WriteStringFormat(string format, params object[] args) {
			WriteString(String.Format(invariantCulture, format, args));
		}
		public void WriteSpace() {
			WriteByte(0x20);
		}
		public void WriteOpenBracket() {
			WriteByte(0x5b);
		}
		public void WriteCloseBracket() {
			WriteByte(0x5d);
		}
		public void WriteOpenDictionary() {
			WriteBytes(new byte[] { 0x3c, 0x3c }, 2);
		}
		public void WriteCloseDictionary() {
			WriteBytes(new byte[] { 0x3e, 0x3e, 0x0d, 0x0a }, 4);
		}
		public void WriteString(string s) {
			byte[] bytes = utf8encoding.GetBytes(s);
			int bytesLength = bytes.Length;
			WriteBytes(bytes, bytesLength);
		}
		public void WriteDouble(double value) {
			if (Math.Abs(value) < 0.000015) {
				WriteByte((byte)'0');
				return;
			}
			if (value < 0) {
				WriteByte((byte)'-');
				value = -value;
			}
			if (value < 1.0) {
				value += 0.000005;
				if (value >= 1) {
					WriteByte((byte)'1');
					return;
				}
				int v = (int)(value * 100000);
				WriteByte((byte)'0');
				WriteByte((byte)'.');
				WriteByte((byte)(v / 10000 + zero));
				if (v % 10000 != 0) {
					WriteByte((byte)((v / 1000) % 10 + zero));
					if (v % 1000 != 0) {
						WriteByte((byte)((v / 100) % 10 + zero));
						if (v % 100 != 0) {
							WriteByte((byte)((v / 10) % 10 + zero));
							if (v % 10 != 0) {
								WriteByte((byte)((v) % 10 + zero));
							}
						}
					}
				}
				return;
			}
			else if (value <= 32767) {
				value += 0.00005;
				int v = (int)(value * 10000);
				if (v >= 100000000) {
					WriteByte((byte)((v / 100000000) % 10 + zero));
				}
				if (v >= 10000000) {
					WriteByte((byte)((v / 10000000) % 10 + zero));
				}
				if (v >= 1000000) {
					WriteByte((byte)((v / 1000000) % 10 + zero));
				}
				if (v >= 100000) {
					WriteByte((byte)((v / 100000) % 10 + zero));
				}
				if (v >= 10000) {
					WriteByte((byte)((v / 10000) % 10 + zero));
				}
				if (v % 10000 != 0) {
					WriteByte((byte)'.');
					WriteByte((byte)((v / 1000) % 10 + zero));
					if (v % 1000 != 0) {
						WriteByte((byte)((v / 100) % 10 + zero));
						if (v % 100 != 0) {
							WriteByte((byte)((v / 10) % 10 + zero));
							if (v % 10 != 0) {
								WriteByte((byte)(v % 10 + zero));
							}
						}
					}
				}
				return;
			}
			WriteString(((float)value).ToString(doubleMask, invariantCulture));
		}
		public void WriteInt(int value) {
			WriteString(value.ToString(invariantCulture));
		}
		public void WriteName(PdfName name) {
			if (name == null)
				WriteString("null");
			else
				name.Write(this);
		}
		public void WriteObject(object value, int number) {
			IPdfWritableObject writableObject = value as IPdfWritableObject;
			if (writableObject != null) {
				writableObject.Write(this, number);
				return;
			}
			PdfRectangle rect = value as PdfRectangle;
			if (rect != null){
				WriteObject(rect.ToWritableObject(), number);
				return;
			}
			if (value is int) {
				WriteInt((int)value);
				return;
			}
			if (value is double) {
				WriteDouble((double)value);
				return;
			}
			string str = value as string;
			if (str != null) {
				byte[] data = unicodeEncoding.GetBytes(str);
				WriteUnicodeHexadecimalString(data, number);
				return;
			}
			byte[] bytes = value as byte[];
			if (bytes != null) {
				WriteHexadecimalString(bytes, number);
				return;
			}
			IEnumerable enumerable = value as IEnumerable;
			if (enumerable != null) {
				new PdfWritableArray(enumerable).Write(this, number);
				return;
			}
			if (value is DateTimeOffset) {
				DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
				int offset = Convert.ToInt32(dateTimeOffset.Offset.TotalMinutes);
				string dateString;
				if (offset == 0)
					dateString = String.Format("D:{0:yyyyMMddHHmmss}Z", dateTimeOffset);
				else {
					bool positive = offset > 0;
					if (!positive)
						offset = -offset;
					dateString = String.Format("D:{0:yyyyMMddHHmmss}{1}{2:00}'{3:00}'", dateTimeOffset, positive ? '+' : '-', offset / 60, offset % 60);
				}
				WriteHexadecimalString(utf8encoding.GetBytes(dateString), number);
				return;
			}
			if (value is bool) {
				WriteString((bool)value ? "true" : "false");
				return;
			}
			if (value == null) {
				WriteString("null");
				return;
			}
			throw new NotSupportedException();
		}
		public void WriteBytes(byte[] value) {
			int valueLength = value.Length;
			WriteBytes(value, valueLength);
		}
		public void WriteHexadecimalString(byte[] data, int number) {
			if (encryptionInfo != null && number != PdfObject.DirectObjectNumber)
				data = encryptionInfo.EncryptData(data, number);
			byte b;
			int l = data.Length;
			int length = l * 2 + 2;
			byte[] bytes = new byte[length];
			bytes[0] = 60;
			bytes[length - 1] = 62;
			for (int i = 0, j = 0; i < l; ++i) {
				b = data[i];
				int k = b >> 4;
				bytes[++j] = (byte)(k > 9 ? k + 0x37 : k + 0x30);
				k = b & 15;
				bytes[++j] = (byte)(k > 9 ? k + 0x37 : k + 0x30);
			}
			int bytesLength = bytes.Length;
			WriteBytes(bytes, bytesLength);
		}
		void WriteUnicodeHexadecimalString(byte[] data, int number) {
			List<byte> bigEndianData = new List<byte>() { 254, 255 };
			bigEndianData.AddRange(data);
			WriteHexadecimalString(bigEndianData.ToArray(), number);
		}
		public virtual void WriteByte(byte b) {
			stream.WriteByte(b);
		}
		protected virtual void WriteBytes(byte[] bytes, int count) {
			stream.Write(bytes, 0, count);
		}
	}
}
