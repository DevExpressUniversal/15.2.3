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
using System.Security.Cryptography;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.Office.Crypto.Agile {
	public interface ICipherProvider {
		int BlockBytes { get; }
		ICryptoTransform GetCryptoTransform(byte[] blockKey, bool isEncryption);
	}
	class PasswordKeyEncryptor {
		static readonly byte[] hashInputBlockKey = new byte[] { 0xfe, 0xa7, 0xd2, 0x76, 0x3b, 0x4b, 0x9e, 0x79, };
		static readonly byte[] hashValueBlockKey = new byte[] { 0xd7, 0xaa, 0x0f, 0x6d, 0x30, 0x61, 0x34, 0x4e, };
		static readonly byte[] secretKeyBlockKey = new byte[] { 0x14, 0x6e, 0x0b, 0xe7, 0xab, 0xac, 0xd0, 0xd6, };
		byte[] encryptedHashInput;
		byte[] encryptedHashValue;
		byte[] encryptedSecretKey;
		readonly PasswordBasedKey passwordEncryptor;
		public PasswordKeyEncryptor(PasswordKeyEncryptorInfo config) {
			this.passwordEncryptor = new PasswordBasedKey(config);
		}
		public PasswordKeyEncryptor(PasswordKeyEncryptorInfo config, PasswordKeyEncryptorData data) {
			this.passwordEncryptor = new PasswordBasedKey(config, data);
			this.encryptedHashInput = (byte[])data.EncryptedHashInput.Clone();
			this.encryptedHashValue = (byte[])data.EncryptedHashValue.Clone();
			this.encryptedSecretKey = (byte[])data.EncryptedKeyValue.Clone();
		}
		internal byte[] EncryptedHashInput { get { return encryptedHashInput; } }
		internal byte[] EncryptedHashValue { get { return encryptedHashValue; } }
		internal byte[] EncryptedSecretKey { get { return encryptedSecretKey; } }
		internal PasswordBasedKey PasswordEncryptor { get { return passwordEncryptor; } }
		public static byte[] CreateEncryptedHashInput(PasswordBasedKey passwordEncryptor, string password) {
			byte[] encryptedHashInput = new byte[Utils.RoundUp(passwordEncryptor.SaltValue.Length, passwordEncryptor.BlockBytes)];
			EncryptionSession.GetRandomBytes(encryptedHashInput);
			using (ICryptoTransform encryptor = passwordEncryptor.GetEncryptor(hashValueBlockKey)) {
				encryptor.TransformInPlace(encryptedHashInput, 0, encryptedHashInput.Length);
			}
			return encryptedHashInput;
		}
		public static byte[] CreateVerifier(PasswordBasedKey passwordEncryptor, byte[] encryptedHashInput) {
			byte[] decryptedHashInput = (byte[])encryptedHashInput.Clone();
			using (ICryptoTransform decryptor = passwordEncryptor.GetDecryptor(hashInputBlockKey)) {
				decryptor.TransformInPlace(decryptedHashInput, 0, decryptedHashInput.Length);
			}
			byte[] encryptedHashValue;
			using (HashAlgorithm hashAlgorithm = passwordEncryptor.GetHashAlgorithm()) {
				int hashValueBytes = hashAlgorithm.HashSize / 8;
				encryptedHashValue = new byte[Utils.RoundUp(hashValueBytes, passwordEncryptor.BlockBytes)];
				byte[] hash = hashAlgorithm.ComputeHash(decryptedHashInput);
				Buffer.BlockCopy(hash, 0, encryptedHashValue, 0, hashValueBytes);
			}
			using (ICryptoTransform encryptor = passwordEncryptor.GetEncryptor(hashValueBlockKey)) {
				encryptor.TransformInPlace(encryptedHashValue, 0, encryptedHashValue.Length);
			}
			return encryptedHashValue;
		}
		public void SetPassword(string password, byte[] secretKey) {
			passwordEncryptor.SetPassword(password);
			encryptedHashInput = CreateEncryptedHashInput(passwordEncryptor, password);
			encryptedHashValue = CreateVerifier(passwordEncryptor, encryptedHashInput);
			byte[] newEncryptedSecretKey = (byte[])secretKey.Clone();
			using (ICryptoTransform encryptor = passwordEncryptor.GetEncryptor(secretKeyBlockKey)) {
				encryptor.TransformInPlace(newEncryptedSecretKey, 0, newEncryptedSecretKey.Length);
			}
			encryptedSecretKey = newEncryptedSecretKey;
		}
		public bool TryUnlock(string password, out byte[] secretKey) {
			secretKey = null;
			if (encryptedHashInput == null || encryptedHashValue == null)
				throw new InvalidOperationException("No encrypted verifier");
			if (encryptedSecretKey == null)
				throw new InvalidOperationException("No encrypted secret to unlock");
			passwordEncryptor.SetPassword(password);
			byte[] encryptedHashValueNew = CreateVerifier(passwordEncryptor, encryptedHashInput);
			if (!encryptedHashValueNew.EqualBytes(encryptedHashValue))
				return false;
			byte[] decryptedSecretKey = (byte[])encryptedSecretKey.Clone();
			using (ICryptoTransform decryptor = passwordEncryptor.GetDecryptor(secretKeyBlockKey)) {
				decryptor.TransformInPlace(decryptedSecretKey, 0, decryptedSecretKey.Length);
			}
			secretKey = decryptedSecretKey;
			return true;
		}
		internal void Save(PasswordKeyEncryptorInfo config, PasswordKeyEncryptorData data) {
			this.passwordEncryptor.Save(config, data);
			data.EncryptedHashInput = (byte[])encryptedHashInput.Clone();
			data.EncryptedHashValue = (byte[])encryptedHashValue.Clone();
			data.EncryptedKeyValue = (byte[])encryptedSecretKey.Clone();
		}
	}
	class PasswordBasedKey : ICipherProvider {
		HashInfo hashInfo;
		CipherInfo cipherInfo;
		int spinCount;
		byte[] saltValue;
		byte[] passwordHash;
		public PasswordBasedKey(PasswordKeyEncryptorInfo config) {
			this.cipherInfo = config.CipherInfo.Clone();
			this.hashInfo = config.HashInfo.Clone();
			this.spinCount = config.SpinCount;
			this.saltValue = new byte[config.SaltSize];
			EncryptionSession.GetRandomBytes(saltValue);
		}
		public PasswordBasedKey(PasswordKeyEncryptorInfo config, PasswordKeyEncryptorData data) {
			this.cipherInfo = config.CipherInfo.Clone();
			this.hashInfo = config.HashInfo.Clone();
			this.spinCount = config.SpinCount;
			this.saltValue = (byte[])data.SaltValue.Clone();
		}
		public byte[] SaltValue { get { return (byte[])saltValue.Clone(); } }
		public int BlockBytes { get { return cipherInfo.BlockBits / 8; } }
		public int SpinCount { get { return spinCount; } }
		public HashAlgorithm GetHashAlgorithm() {
			return this.hashInfo.GetAlgorithm();
		}
		public void SetPassword(string password) {
			using (HashAlgorithm hashAlgorithm = hashInfo.GetAlgorithm()) {
				byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
				byte[] hashValue = hashAlgorithm.Transform2Blocks(saltValue, passwordBytes);
				for (int i = 0; i < this.spinCount; i++)
					hashValue = hashAlgorithm.Transform2Blocks(BitConverter.GetBytes(i), hashValue);
				this.passwordHash = hashValue;
			}
		}
		public ICryptoTransform GetCryptoTransform(byte[] blockKey, bool isEncryption) {
			using (HashAlgorithm hashAlgorithm = hashInfo.GetAlgorithm()) {
				byte[] hash = hashAlgorithm.Transform2Blocks(passwordHash, blockKey);
				byte[] secretKey = hash.CloneToFit(cipherInfo.KeyBits / 8);
				byte[] iv = SaltValue.CloneToFit(cipherInfo.BlockBits / 8);
				using (SymmetricAlgorithm cipherAlgorithm = cipherInfo.GetAlgorithm()) {
					if (isEncryption)
						return cipherAlgorithm.CreateEncryptor(secretKey, iv);
					else
						return cipherAlgorithm.CreateDecryptor(secretKey, iv);
				}
			}
		}
		internal void Save(PasswordKeyEncryptorInfo config, PasswordKeyEncryptorData data) {
			config.CipherInfo = cipherInfo.Clone();
			config.HashInfo = hashInfo.Clone();
			config.SaltSize = saltValue.Length;
			config.SpinCount = spinCount;
			data.SaltValue = (byte[])SaltValue.Clone();
		}
	}
}
