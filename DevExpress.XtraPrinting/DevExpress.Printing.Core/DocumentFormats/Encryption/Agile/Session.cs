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
using System.Security.Cryptography;
namespace DevExpress.Office.Crypto.Agile {
	public class EncryptionSession {
		HashInfo hashInfo;
		CipherInfo cipherInfo;
		byte[] saltValue;
		KeyManager keyManager;
		PrimaryCipher primaryCipher;
		HmacData hmacData;
		public EncryptionSession(EncryptionInfo config) {
			this.cipherInfo = config.CipherInfo.Clone();
			this.hashInfo = config.HashInfo.Clone();
			this.saltValue = new byte[config.SaltSize];
			GetRandomBytes(this.saltValue);
			this.keyManager = new KeyManager(config);
		}
		internal EncryptionSession(EncryptionInfo config, EncryptionData data) {
			this.cipherInfo = config.CipherInfo.Clone();
			this.hashInfo = config.HashInfo.Clone();
			this.saltValue = data.SaltValue;
			this.keyManager = new KeyManager(config, data.PasswordEncryptorData);
			this.hmacData = data.HmacData;
			if (keyManager.SecretKey != null)
				this.primaryCipher = new PrimaryCipher(hashInfo, cipherInfo, saltValue, keyManager.SecretKey);
		}
		public bool IsUnlocked { get { return keyManager.IsUnlocked; } }
		public int CipherBlockBytes { get { return cipherInfo.BlockBits / 8; } }
#if DXRESTRICTED
		static readonly Random rnd = new Random((int)DateTime.Now.Ticks);
#endif
		public static void GetRandomBytes(byte[] bytes) {
#if DXRESTRICTED
			rnd.NextBytes(bytes);
#else
			using (RandomNumberGenerator generator = RandomNumberGenerator.Create()) {
				generator.GetBytes(bytes);
			}
#endif
		}
		public static EncryptionSession LoadFromStream(Stream stream, bool useHmac) {
			EncryptionInfo encryptionConfig;
			EncryptionData encryptionData = EncryptionData.LoadFromStream(stream, useHmac, out encryptionConfig);
			return new EncryptionSession(encryptionConfig, encryptionData);
		}
		public static EncryptionSession LoadFromStream(Stream stream) {
			return LoadFromStream(stream, true);
		}
		public bool UnlockWithPassword(string password) {
			if (primaryCipher != null)
				throw new InvalidOperationException("Already unlocked");
			if (!this.keyManager.UnlockWithPassword(password))
				return false;
			primaryCipher = new PrimaryCipher(hashInfo, cipherInfo, saltValue, keyManager.SecretKey);
			return true;
		}
		public void WriteToStream(Stream stream) {
			EncryptionInfo config = new EncryptionInfo();
			config.CipherInfo = cipherInfo.Clone();
			config.HashInfo = hashInfo.Clone();
			config.SaltSize = saltValue.Length;
			EncryptionData data = new EncryptionData();
			if (hmacData != null) {
				if (hmacData.EncryptedKey != null && hmacData.EncryptedValue != null)
					data.HmacData = hmacData.Clone();
			}
			data.SaltValue = saltValue;
			keyManager.Save(config, data);
			EncryptionData.WriteToStream(stream, config, data);
		}
		public EncryptedStream GetEncryptedStream(Stream stream) {
			if (primaryCipher == null)
				throw new InvalidOperationException("Not unlocked");
			return new EncryptedStream(primaryCipher, stream);
		}
		public void AddIntegrityCheck(Stream stream) {
			if (primaryCipher == null)
				throw new InvalidOperationException("Not unlocked");
			hmacData = Hmac.GetHmac(primaryCipher, hashInfo, stream);
		}
		public bool DoIntegrityCheck(Stream stream) {
			if (primaryCipher == null)
				throw new InvalidOperationException("Not unlocked");
			if (hmacData == null)
				return true;
			return Hmac.CheckStream(primaryCipher, hashInfo, hmacData, stream);
		}
		public ICryptoTransform GetEncryptor() {
			return primaryCipher.GetEncryptor(0, 0);
		}
		public ICryptoTransform GetDecryptor() {
			return primaryCipher.GetDecryptor(0, 0);
		}
		internal void Save(out EncryptionInfo config, out EncryptionData data) {
			config = new EncryptionInfo();
			config.CipherInfo = cipherInfo.Clone();
			config.HashInfo = hashInfo.Clone();
			config.SaltSize = saltValue.Length;
			data = new EncryptionData();
			if (hmacData != null) {
				if (hmacData.EncryptedKey != null && hmacData.EncryptedValue != null)
					data.HmacData = hmacData.Clone();
			}
			data.SaltValue = saltValue;
			keyManager.Save(config, data);
		}
	}
}
