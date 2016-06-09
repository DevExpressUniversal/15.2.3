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

using System.Collections.Generic;
namespace DevExpress.Utils.Crypt {
	public class ARC4Cipher {
		const int bufferSize = 256;
		static byte Swap(byte[] buffer, int i, int j) {
			byte valuei = buffer[i];
			byte valuej = buffer[j];
			buffer[i] = valuej;
			buffer[j] = valuei;
			return (byte)((valuei + valuej) & 0xff);
		}
		readonly byte[] keyBuffer = new byte[bufferSize];
		readonly byte[] s = new byte[bufferSize];
		int x = 0;
		int y = 0;
		public ARC4Cipher(byte[] key) {
			UpdateKey(key);
		}
		public void UpdateKey(byte[] key) {
			for (int index = 0; index < bufferSize; index++)
				keyBuffer[index] = (byte)index;
			int i = 0;
			int j = 0;
			int keyIndex = 0;
			int keyLength = key.Length;
			IEnumerator<byte> keyEnumerator = ((IEnumerable<byte>)key).GetEnumerator();
			foreach (byte element in keyBuffer) {
				keyEnumerator.MoveNext();
				j = (j + element + keyEnumerator.Current) & 0xff;
				Swap(keyBuffer, i++, j);
				if (++keyIndex == keyLength) {
					keyIndex = 0;
					keyEnumerator.Reset();
				}
			}
			Reset();
		}
		public void Reset() {
			keyBuffer.CopyTo(s, 0);
			x = 0;
			y = 0;
		}
		public void Reset(int position) {
			Reset();
			for (int i = 0; i < position; i++)
				GenerateKeyWord();
		}
		public void EncryptSelf(byte[] data) {
			int length = data.Length;
			for (int index = 0; index < length; index++)
				data[index] ^= GenerateKeyWord();
		}
		public void Encrypt(byte[] input, int inputOffset, byte[] output, int outputOffset, int count) {
			Guard.ArgumentNotNull(input, "input");
			Guard.ArgumentNotNull(output, "output");
			for (int i = 0; i < count; i++)
				output[i + outputOffset] = (byte)(input[i + inputOffset] ^ GenerateKeyWord());
		}
		public byte[] Encrypt(byte[] input) {
			Guard.ArgumentNotNull(input, "input");
			int count = input.Length;
			byte[] result = new byte[count];
			Encrypt(input, 0, result, 0, count);
			return result;
		}
		public byte Encrypt(byte input) {
			return (byte)(input ^ GenerateKeyWord());
		}
		public void Decrypt(byte[] input, int inputOffset, byte[] output, int outputOffset, int count) {
			Encrypt(input, inputOffset, output, outputOffset, count);
		}
		public byte[] Decrypt(byte[] input) {
			return Encrypt(input);
		}
		public byte Decrypt(byte input) {
			return (byte)(input ^ GenerateKeyWord());
		}
		byte GenerateKeyWord() {
			x = (x + 1) & 0xff;
			y = (y + s[x]) & 0xff;
			return s[Swap(s, x, y)];
		}
	}
}
