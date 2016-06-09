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
using System.IO;
using System.Linq;
using System.Security.Cryptography;
namespace DevExpress.Office.Crypto.Agile {
	static class Hmac {
		static readonly byte[] hmacSaltBlockKey = { 0x5f, 0xb2, 0xad, 0x01, 0x0c, 0xb9, 0xe1, 0xf6, };
		static readonly byte[] hmacValueBlockKey = { 0xa0, 0x67, 0x7f, 0x02, 0xb2, 0x2c, 0x84, 0x33, };
		public static HmacData GetHmac(ICipherProvider cipher, HashInfo hashInfo, Stream stream) {
			byte[] hmacKey = new byte[hashInfo.HashBits / 8];
			EncryptionSession.GetRandomBytes(hmacKey);
			hmacKey = hmacKey.CloneToFit(Utils.RoundUp(hmacKey.Length, cipher.BlockBytes), 0);
			HMAC hmac = CreateHMAC(hashInfo.Name);
			hmac.HashName = hashInfo.Name;
			hmac.Key = hmacKey;
			hmac.Initialize();
			stream.Position = 0;
			byte[] hmacValue = hmac.ComputeHash(stream);
			hmacValue = hmacValue.CloneToFit(Utils.RoundUp(hmacValue.Length, cipher.BlockBytes), 0);
			using (ICryptoTransform encryptor = cipher.GetEncryptor(hmacSaltBlockKey)) {
				encryptor.TransformInPlace(hmacKey, 0, hmacKey.Length);
			}
			;
			using (ICryptoTransform encryptor = cipher.GetEncryptor(hmacValueBlockKey)) {
				encryptor.TransformInPlace(hmacValue, 0, hmacValue.Length);
			}
			HmacData hmacInfo = new HmacData();
			hmacInfo.EncryptedKey = hmacKey;
			hmacInfo.EncryptedValue = hmacValue;
			return hmacInfo;
		}
		public static bool CheckStream(ICipherProvider cipher, HashInfo hashInfo, HmacData hmacInfo, Stream stream) {
			byte[] decryptedKey = (byte[])hmacInfo.EncryptedKey.Clone();
			using (ICryptoTransform decryptor = cipher.GetDecryptor(hmacSaltBlockKey)) {
				decryptor.TransformInPlace(decryptedKey, 0, decryptedKey.Length);
			}
			byte[] decryptedValue = (byte[])hmacInfo.EncryptedValue.Clone();
			using (ICryptoTransform decryptor = cipher.GetDecryptor(hmacValueBlockKey)) {
				decryptor.TransformInPlace(decryptedValue, 0, decryptedValue.Length);
			}
			if (decryptedKey.All(b => b == 0) && decryptedValue.All(b => b == 0))
				return true;
			byte[] hmacKey = new byte[hashInfo.HashBits / 8];
			Buffer.BlockCopy(decryptedKey, 0, hmacKey, 0, hmacKey.Length);
			for (int i = hmacKey.Length; i < decryptedKey.Length; i++) {
				if (decryptedKey[i] != 0) {
					return false;
				}
			}
			HMAC hmac = CreateHMAC(hashInfo.Name);
			hmac.HashName = hashInfo.Name;
			hmac.Key = hmacKey;
			hmac.Initialize();
			byte[] hmacValue = new byte[hashInfo.HashBits / 8];
			Buffer.BlockCopy(decryptedValue, 0, hmacValue, 0, hmacValue.Length);
			for (int i = decryptedValue.Length; i < decryptedValue.Length; i++) {
				if (decryptedValue[i] != 0)
					return false;
			}
			byte[] actualHmacValue = hmac.ComputeHash(stream);
			return actualHmacValue.EqualBytes(hmacValue);
		}
		static HMAC CreateHMAC(string name) {
#if DXPORTABLE
			switch (name) {
				case "SHA256":
					return new HMACSHA256();
				case "SHA384":
					return new HMACSHA384();
				case "SHA512":
					return new HMACSHA512();
				case "SHA1":
					return new HMACSHA1();
				default:
					return null;
			}
#else
			return HMAC.Create();
#endif
		}
	}
}
