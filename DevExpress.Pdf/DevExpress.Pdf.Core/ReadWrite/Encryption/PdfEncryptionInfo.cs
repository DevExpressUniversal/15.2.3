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
using System.Numerics;
using System.Security;
using System.Security.Cryptography;
using System.Collections.Generic;
using DevExpress.Utils.Crypt;
namespace DevExpress.Pdf.Native {
	public partial class PdfEncryptionInfo {
		enum EncryptionAlgorithm {
			Undocumented = 0,
			KeyLength40 = 1,
			KeyLengthGreaterThan40 = 2,
			Unpublished = 3,
			Pdf15 = 4,
			Pdf20 = 5
		}
		enum CryptMethod { V2, AESV2, AESV3 };
		const int hashLength = 32;
		const int validationSoltLength = 8;
		const int keySoltLength = 8;
		const int keySoltPosition = hashLength + validationSoltLength;
		const int aesV3HashLength = hashLength + validationSoltLength + keySoltLength;
		const int passwordLimit = 127;
		const int initializationVectorLength = 16;
		const string filterNameDictionaryKey = "Filter";
		const string algorithmTypeDictionaryKey = "V";
		const string keyLengthDictionaryKey = "Length";
		const string securityHandlerRevisionDictionaryKey = "R";
		const string ownerPasswordHashDictionaryKey = "O";
		const string userPasswordHashDictionaryKey = "U";
		const string encryptionFlagsDictionaryKey = "P";
		const string encryptMetadataDictionaryKey = "EncryptMetadata";
		const string cryptFiltersDictionaryKey = "CF";
		const string streamFilterDictionaryKey = "StmF";
		const string stringFilterDictionaryKey = "StrF";
		const string cryptFilterMethodDictionaryKey = "CFM";
		const string standardFilterName = "Standard";
		const string identityCryptFilterName = "Identity";
		const string standardCryptFilterName = "StdCF";
		static readonly byte[] passwordPadding = new byte[] { (byte)0x28, (byte)0xbf, (byte)0x4e, (byte)0x5e, (byte)0x4e, (byte)0x75, (byte)0x8a, (byte)0x41, 
															  (byte)0x64, (byte)0x00, (byte)0x4e, (byte)0x56, (byte)0xff, (byte)0xfa, (byte)0x01, (byte)0x08,
															  (byte)0x2e, (byte)0x2e, (byte)0x00, (byte)0xb6, (byte)0xd0, (byte)0x68, (byte)0x3e, (byte)0x80, 
															  (byte)0x2f, (byte)0x0c, (byte)0xa9, (byte)0xfe, (byte)0x64, (byte)0x53, (byte)0x69, (byte)0x7a };
		static bool CheckFilterExistence(PdfDictionary filterDecsriptions, string filterName) {
			return filterName == identityCryptFilterName ||
				(filterName == standardCryptFilterName && filterDecsriptions != null && filterDecsriptions.ContainsKey(standardCryptFilterName));
		}
		public static PdfEncryptionInfo Create(byte[][] documentID, PdfEncryptionOptions options) {
			return options != null && (options.UserPassword != null || options.OwnerPassword != null) ? new PdfEncryptionInfo(documentID, options) : null;
		}
		readonly EncryptionAlgorithm algorithm;
		readonly int securityHandlerRevision;
		readonly byte[] ownerPasswordHash;
		readonly byte[] ownerValidationSolt;
		readonly byte[] ownerKeySolt;
		readonly byte[] userPasswordHash;
		readonly byte[] userValidationSolt;
		readonly byte[] userKeySolt;
		readonly bool encryptMetadata;
		readonly byte[][] documentID;
		readonly long encryptionFlags;
		readonly PdfDocumentPermissionsFlags permissionsFlags;
		readonly int extendedKeyLength;
		readonly int actualKeyLength;
		int keyLength;
		string streamFilterName;
		string stringFilterName;
		string embeddedFileFilterName;
		byte[] encodedOwnerPassword;
		byte[] encodedUserPassword;
		byte[] encryptedPermissions;
		CryptMethod cryptMethod;
		byte[] encryptionKey;
		public bool EncryptMetadata { get { return encryptMetadata; } }
		internal PdfDocumentPermissionsFlags PermissionsFlags { get { return permissionsFlags; } }
		[SecuritySafeCritical]
		internal PdfEncryptionInfo(PdfReaderDictionary dictionary, byte[][] documentID, PdfGetPasswordAction getPasswordAction) {
			this.documentID = documentID;
			string filter = dictionary.GetName(filterNameDictionaryKey);
			int? v = dictionary.GetInteger(algorithmTypeDictionaryKey);
			if (!v.HasValue)
				PdfDocumentReader.ThrowIncorrectDataException();
			algorithm = (EncryptionAlgorithm)v.Value;
			keyLength = dictionary.GetInteger(keyLengthDictionaryKey) ?? 40;
			int? r = dictionary.GetInteger(securityHandlerRevisionDictionaryKey);
			ownerPasswordHash = dictionary.GetBytes(ownerPasswordHashDictionaryKey);
			userPasswordHash = dictionary.GetBytes(userPasswordHashDictionaryKey);
			int? p = dictionary.GetInteger(encryptionFlagsDictionaryKey);
			encryptMetadata = dictionary.GetBoolean(encryptMetadataDictionaryKey) ?? true;
			if (filter != standardFilterName || !r.HasValue || ownerPasswordHash == null || userPasswordHash == null || !p.HasValue || (algorithm < EncryptionAlgorithm.Pdf20 && documentID == null))
				PdfDocumentReader.ThrowIncorrectDataException();
			securityHandlerRevision = r.Value;
			encryptionFlags = p.Value;
			permissionsFlags = (PdfDocumentPermissionsFlags)encryptionFlags;
			ownerPasswordHash = ValidateHash(ownerPasswordHash);
			userPasswordHash = ValidateHash(userPasswordHash);
			switch (algorithm) {
				case EncryptionAlgorithm.KeyLength40:
					if (securityHandlerRevision != 2)
						PdfDocumentReader.ThrowIncorrectDataException();
					keyLength = 40;
					break;
				case EncryptionAlgorithm.KeyLengthGreaterThan40:
					if (keyLength < 40 || keyLength > 128 || (keyLength % 8) != 0 || securityHandlerRevision < 2 || securityHandlerRevision > 3)
						PdfDocumentReader.ThrowIncorrectDataException();
					break;
				case EncryptionAlgorithm.Pdf15:
					if (securityHandlerRevision != 4)
						PdfDocumentReader.ThrowIncorrectDataException();
					InitializeCryptFilter(dictionary);
					break;
				case EncryptionAlgorithm.Pdf20:
					if (securityHandlerRevision != 5 && securityHandlerRevision != 6)
						PdfDocumentReader.ThrowIncorrectDataException();
					InitializeCryptFilter(dictionary);
					ownerValidationSolt = new byte[validationSoltLength];
					ownerKeySolt = new byte[keySoltLength];
					Array.Copy(ownerPasswordHash, hashLength, ownerValidationSolt, 0, validationSoltLength);
					Array.Copy(ownerPasswordHash, keySoltPosition, ownerKeySolt, 0, keySoltLength);
					userValidationSolt = new byte[validationSoltLength];
					userKeySolt = new byte[keySoltLength];
					Array.Copy(userPasswordHash, hashLength, userValidationSolt, 0, validationSoltLength);
					Array.Copy(userPasswordHash, keySoltPosition, userKeySolt, 0, keySoltLength);
					break;
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					break;
			}
			keyLength /= 8;
			extendedKeyLength = keyLength + 5;
			actualKeyLength = Math.Min(extendedKeyLength, 16);
			if (!CheckUserPassword(algorithm == EncryptionAlgorithm.Pdf20 ? new byte[0] : passwordPadding)) {
				if (getPasswordAction == null)
					throw new PdfIncorrectPasswordException();
				for (int tryNumber = 1;; tryNumber++) {
					SecureString password = getPasswordAction(tryNumber);
					if (password == null)
						throw new PdfIncorrectPasswordException();
					byte[] passwordString = GetPasswordString(password);
					if (CheckUserPassword(passwordString))
						break;
					if (algorithm == EncryptionAlgorithm.Pdf20) {
						List<byte> ownerPassword = new List<byte>(passwordString);
						ownerPassword.AddRange(ownerValidationSolt);
						ownerPassword.AddRange(userPasswordHash);
						if (CheckPassword(ownerPasswordHash, ComputeHash(passwordString, ownerPassword.ToArray(), userPasswordHash))) {
							ownerPassword = new List<byte>(passwordString);
							ownerPassword.AddRange(ownerKeySolt);
							ownerPassword.AddRange(userPasswordHash);
							encryptionKey = DecryptAesData(CipherMode.CBC, ComputeHash(passwordString, ownerPassword.ToArray(), userPasswordHash), encodedOwnerPassword);
							if (CheckPermissions())
								break;
						}
					}
					else {
						byte[] key = ComputeOwnerDecryptionKey(passwordString);
						byte[] userPasswordString;
						if (securityHandlerRevision >= 3) {
							userPasswordString = ownerPasswordHash;
							for (int i = 19; i >= 0; i--)
								userPasswordString = new ARC4Cipher(XorKey(key, i)).Encrypt(userPasswordString);
						}
						else
							userPasswordString = new ARC4Cipher(key).Encrypt(ownerPasswordHash);
						if (CheckUserPassword(userPasswordString))
							break;
					}
				}
			}
			if (securityHandlerRevision < 3)
				permissionsFlags |= PdfDocumentPermissionsFlags.FormFilling | PdfDocumentPermissionsFlags.Accessibility |
									PdfDocumentPermissionsFlags.DocumentAssembling | PdfDocumentPermissionsFlags.HighQualityPrinting;
		}
		PdfEncryptionInfo(byte[][] documentID, PdfEncryptionOptions options) {
			this.documentID = documentID;
			algorithm = EncryptionAlgorithm.Pdf15;
			keyLength = 16;
			extendedKeyLength = 21;
			actualKeyLength = 16;
			streamFilterName = standardCryptFilterName;
			stringFilterName = standardCryptFilterName;
			securityHandlerRevision = 4;
			encryptMetadata = true;
			encryptionFlags = options.PermissionsValue;
			permissionsFlags = (PdfDocumentPermissionsFlags)encryptionFlags;
			SecureString userPassword = options.UserPassword;
			byte[] userPasswordString = GetPasswordString(userPassword);
			SecureString ownerPassword = options.OwnerPassword;
			byte[] key = ComputeOwnerDecryptionKey(ownerPassword == null ? userPasswordString : GetPasswordString(ownerPassword));
			ownerPasswordHash = new ARC4Cipher(key).Encrypt(userPasswordString);
			for (int i = 1; i <= 19; i++)
				ownerPasswordHash = new ARC4Cipher(XorKey(key, i)).Encrypt(ownerPasswordHash);
			userPasswordHash = ComputeUserPasswordHash(userPasswordString);
		}
		byte[] ValidateHash(byte[] hash) {
			switch (securityHandlerRevision) {
				case 6:
					if (hash.Length != passwordLimit)
						goto case 5;
					Array.Resize<byte>(ref hash, aesV3HashLength);
					break;
				case 5:
					if (hash.Length != aesV3HashLength)
						PdfDocumentReader.ThrowIncorrectDataException();
					break;
				default:
					if (hash.Length != hashLength)
						PdfDocumentReader.ThrowIncorrectDataException();
					break;
			}
			return hash;
		}
		void InitializeCryptFilter(PdfReaderDictionary dictionary) {
			PdfReaderDictionary cryptFilters = dictionary.GetDictionary(cryptFiltersDictionaryKey);
			streamFilterName = dictionary.GetName(streamFilterDictionaryKey);
			stringFilterName = dictionary.GetName(stringFilterDictionaryKey);
			embeddedFileFilterName = dictionary.GetName("EFF");
			int maxKeyLength;
			int maxKeyBits;
			if (securityHandlerRevision >= 5) {
				encodedOwnerPassword = dictionary.GetBytes("OE");
				encodedUserPassword = dictionary.GetBytes("UE");
				encryptedPermissions = dictionary.GetBytes("Perms");
				if (encodedOwnerPassword == null || encodedOwnerPassword.Length != hashLength || encodedUserPassword == null || encodedUserPassword.Length != hashLength ||
					encryptedPermissions == null || encryptedPermissions.Length != 16)
						PdfDocumentReader.ThrowIncorrectDataException();
				maxKeyLength = 32;
				maxKeyBits = 256;
			}
			else {
				maxKeyLength = 16;
				maxKeyBits = 128;
			}
			if (!CheckFilterExistence(cryptFilters, streamFilterName) || !CheckFilterExistence(cryptFilters, stringFilterName) || 
				(embeddedFileFilterName != null && !CheckFilterExistence(cryptFilters, embeddedFileFilterName)))
					PdfDocumentReader.ThrowIncorrectDataException();
			if (streamFilterName == standardCryptFilterName || stringFilterName == standardCryptFilterName) {
				PdfReaderDictionary standardFilterDictionary = cryptFilters.GetDictionary(standardCryptFilterName);
				cryptMethod = PdfEnumToStringConverter.Parse<CryptMethod>(standardFilterDictionary.GetName(cryptFilterMethodDictionaryKey), false);
				int? length = standardFilterDictionary.GetInteger(keyLengthDictionaryKey);
				if (!length.HasValue)
					PdfDocumentReader.ThrowIncorrectDataException();
				keyLength = length.Value;
				if (keyLength >= 5 && keyLength <= maxKeyLength)
					keyLength *= 8;
				else if ((keyLength < 40 && keyLength > maxKeyBits) || keyLength % 8 != 0)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
		}
		byte[] PadOrTruncatePassword(byte[] passwordString) {
			List<byte> password = new List<byte>(passwordString);
			password.AddRange(passwordPadding);
			password.RemoveRange(hashLength, password.Count - hashLength);
			return password.ToArray();
		}
		byte[] GetPasswordString(SecureString password) {
			if (algorithm != EncryptionAlgorithm.Pdf20)
				return PadOrTruncatePassword(GetAnsiPasswordString(password));
			List<byte> actualPassword = new List<byte>(GetUnicodePasswordString(password));
			int excessiveSize = actualPassword.Count - passwordLimit;
			if (excessiveSize > 0)
				actualPassword.RemoveRange(passwordLimit, excessiveSize);
			return actualPassword.ToArray();
		}
		byte[] XorKey(byte[] key, int value) {
			byte[] result = new byte[keyLength];
			for (int i = 0; i < keyLength; i++)
				result[i] = (byte)(key[i] ^ value);
			return result;
		}
		byte[] ComputeOwnerDecryptionKey(byte[] ownerPasswordString) {
			byte[] key = ownerPasswordString;
			using (MD5 provider = MD5.Create())
				key = provider.ComputeHash(key);
			Array.Resize(ref key, keyLength);
			if (securityHandlerRevision >= 3)
				for (int i = 0; i < 50; i++)
					using (MD5 provider = MD5.Create())
						key = provider.ComputeHash(key);
			return key;
		}
		byte[] ComputeUserPasswordHash(byte[] userPasswordString) {
			List<byte> key = new List<byte>(userPasswordString);
			key.AddRange(ownerPasswordHash);
			long flags = encryptionFlags;
			key.Add((byte)(flags & 0xff));
			key.Add((byte)((flags & 0xff00) >> 8));
			key.Add((byte)((flags & 0xff0000) >> 16));
			key.Add((byte)((flags & 0xff000000) >> 24));
			key.AddRange(documentID[0]);
			if (securityHandlerRevision >= 4 && !encryptMetadata)
				key.AddRange(new byte[] { 0xff, 0xff, 0xff, 0xff });
			using (MD5 provider = MD5.Create())
				encryptionKey = provider.ComputeHash(key.ToArray());
			if (securityHandlerRevision >= 3)
				for (int i = 0; i < 50; i++)
					using (MD5 provider = MD5.Create()) {
						Array.Resize<byte>(ref encryptionKey, keyLength);
						encryptionKey = provider.ComputeHash(encryptionKey);
					}
			Array.Resize<byte>(ref encryptionKey, keyLength);
			switch (cryptMethod) {
				case CryptMethod.V2:
					Array.Resize<byte>(ref encryptionKey, extendedKeyLength);
					break;
				case CryptMethod.AESV2:
					Array.Resize<byte>(ref encryptionKey, extendedKeyLength + 4);
					int position = extendedKeyLength;
					encryptionKey[position++] = 0x73;
					encryptionKey[position++] = 0x41;
					encryptionKey[position++] = 0x6c;
					encryptionKey[position] = 0x54;
					break;
			}
			if (securityHandlerRevision < 3) {
				byte[] arc4Key = new byte[keyLength];
				Array.Copy(encryptionKey, 0, arc4Key, 0, keyLength);
				return new ARC4Cipher(arc4Key).Encrypt(passwordPadding);
			}
			List<byte> hashInput = new List<byte>(passwordPadding);
			hashInput.AddRange(documentID[0]);
			byte[] userHash;
			using (MD5 provider = MD5.Create())
				userHash = provider.ComputeHash(hashInput.ToArray());
			for (int i = 0; i < 20; i++)
				userHash = new ARC4Cipher(XorKey(encryptionKey, i)).Encrypt(userHash);
			return PadOrTruncatePassword(userHash);
		}
		bool CheckPassword(byte[] expectedHash, byte[] actualHash) {
			int sizeToCheck = securityHandlerRevision == 2 ? hashLength : 16;
			for (int i = 0; i < sizeToCheck; i++)
				if (actualHash[i] != expectedHash[i])
					return false;
			return true;
		}
		byte[] ComputeHash(byte[] passwordString, byte[] data, byte[] userKey) {
			using (SHA256 provider = SHA256.Create()) {
				byte[] k = provider.ComputeHash(data);
				if (securityHandlerRevision == 5)
					return k;
				for (int round = 0;; round++) {
					List<byte> k1 = new List<byte>();
					for (int i = 0; i < 64; i++) {
						k1.AddRange(passwordString);
						k1.AddRange(k);
						k1.AddRange(userKey);
					}
					byte[] key = new byte[initializationVectorLength];
					byte[] initializationVector = new byte[initializationVectorLength];
					Array.Copy(k, key, initializationVectorLength);
					Array.Copy(k, initializationVectorLength, initializationVector, 0, initializationVectorLength);
					byte[] e = EncryptAesData(key, initializationVector, k1.ToArray());
					BigInteger value = 0;
					for (int i = 0; i < 16; i++)
						value = value * 256 + e[i];
					HashAlgorithm hashAlgorithm;
					switch ((int)(value % 3)) {
						case 1:
							hashAlgorithm = SHA384.Create();
							break;
						case 2:
							hashAlgorithm = SHA512.Create();
							break;
						default:	
							hashAlgorithm = SHA256.Create();
							break;
					}
					using (hashAlgorithm)
						k = hashAlgorithm.ComputeHash(e);
					if (round >= 63 && e[e.Length - 1] <= round - 32) {
						Array.Resize<byte>(ref k, 32);
						return k;
					}
				}
			}
		}
		byte[] DecryptAesData(CipherMode mode, byte[] key, byte[] data) {
			return DecryptAesData(mode, PaddingMode.None, key, new byte[initializationVectorLength], data, 0);
		}
		bool CheckPermissions() {
			byte[] decryptedPermissions = DecryptAesData(CipherMode.ECB, encryptionKey, encryptedPermissions);
			return decryptedPermissions[9] == (byte)'a' && decryptedPermissions[10] == (byte)'d' && decryptedPermissions[11] == (byte)'b' && 
				((decryptedPermissions[2] << 16) + (decryptedPermissions[1] << 8) + decryptedPermissions[0]) == (encryptionFlags & 0xffffff);
		}
		bool CheckUserPassword(byte[] passwordString) {
			if (algorithm != EncryptionAlgorithm.Pdf20)
				return CheckPassword(userPasswordHash, ComputeUserPasswordHash(passwordString));
			List<byte> password = new List<byte>(passwordString);
			password.AddRange(userValidationSolt);
			byte[] hash = ComputeHash(passwordString, password.ToArray(), new byte[0]);
			if (!CheckPassword(userPasswordHash, hash))
				return false;
			password = new List<byte>(passwordString);
			password.AddRange(userKeySolt);
			encryptionKey = DecryptAesData(CipherMode.CBC, ComputeHash(passwordString, password.ToArray(), new byte[0]), encodedUserPassword);
			return CheckPermissions();
		}
		byte[] ComputeActualEncryptionKey(int number) {
			int generation = 0;
			int index = keyLength;
			encryptionKey[index++] = (byte)(number & 0xff);
			encryptionKey[index++] = (byte)((number & 0xff00) >> 8);
			encryptionKey[index++] = (byte)((number & 0xff0000) >> 16);
			encryptionKey[index++] = (byte)(generation & 0xff);
			encryptionKey[index] = (byte)((generation & 0xff00) >> 8);
			byte[] key;
			using (MD5 provider = MD5.Create())
				key = provider.ComputeHash(encryptionKey);
			Array.Resize<byte>(ref key, actualKeyLength);
			return key;
		}
		byte[] DecryptAesData(byte[] key, byte[] data) {
			if (data.Length - initializationVectorLength < 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			byte[] initalizationVector = new byte[initializationVectorLength];
			Array.Copy(data, initalizationVector, initializationVectorLength);
			return DecryptAesData(CipherMode.CBC, PaddingMode.PKCS7, key, initalizationVector, data, initializationVectorLength);
		}
		public byte[] EncryptData(byte[] data, int number) {
			return new ARC4Cipher(ComputeActualEncryptionKey(number)).Encrypt(data);
		}
		public byte[] DecryptData(byte[] data, int number, int generation) {
			switch (algorithm) {
				case EncryptionAlgorithm.Pdf20:
					return DecryptAesData(encryptionKey, data);
				case EncryptionAlgorithm.Pdf15:
					if (streamFilterName == identityCryptFilterName)
						return data;
					byte[] key = ComputeActualEncryptionKey(number);
					return cryptMethod == CryptMethod.AESV2 ? DecryptAesData(key, data) : new ARC4Cipher(key).Encrypt(data);
				default:
					return new ARC4Cipher(ComputeActualEncryptionKey(number)).Encrypt(data);
			}
		}
		public PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.AddName(filterNameDictionaryKey, standardFilterName);
			int algorithmNumber = (int)algorithm;
			dictionary.Add(algorithmTypeDictionaryKey, algorithmNumber);
			dictionary.Add(keyLengthDictionaryKey, keyLength * 8);
			dictionary.Add(securityHandlerRevisionDictionaryKey, securityHandlerRevision);
			dictionary.Add(ownerPasswordHashDictionaryKey, ownerPasswordHash);
			dictionary.Add(userPasswordHashDictionaryKey, userPasswordHash);
			dictionary.Add(encryptionFlagsDictionaryKey, (int)encryptionFlags);
			dictionary.Add(encryptMetadataDictionaryKey, encryptMetadata);
			if (algorithm == EncryptionAlgorithm.Pdf15) {
				PdfWriterDictionary standardFilterDictionary = new PdfWriterDictionary(objects);
				standardFilterDictionary.AddEnumName(cryptFilterMethodDictionaryKey, cryptMethod);
				standardFilterDictionary.Add(keyLengthDictionaryKey, keyLength);
				PdfWriterDictionary cryptFilters = new PdfWriterDictionary(objects);
				cryptFilters.Add(standardCryptFilterName, standardFilterDictionary);
				dictionary.Add(cryptFiltersDictionaryKey, cryptFilters);
				dictionary.AddName(streamFilterDictionaryKey, streamFilterName);
				dictionary.AddName(stringFilterDictionaryKey, stringFilterName);
			}
			return dictionary;
		}
	}
}
