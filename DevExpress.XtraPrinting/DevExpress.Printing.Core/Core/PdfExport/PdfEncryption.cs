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

using DevExpress.Pdf.Common;
using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
namespace DevExpress.XtraPrinting.Export.Pdf {
	public class PdfEncryption : PdfDocumentDictionaryObject {
		PrintingPermissions printingPermissions = PrintingPermissions.HighResolution;
		ChangingPermissions changingPermissions = ChangingPermissions.AnyExceptExtractingPages;
		string permissionsPassword;
		string openPassword;
		bool enableCoping = true;
		bool enableScreenReaders = true;
		PdfDocument document;
		string ownerPassword;
		string userPassword;
		byte[] key;
		int version = 4;
		int revision = 4;
		int keyLength = 128;
		bool encryptMetadata = true;
		int permissions;
		public PdfEncryption(PdfDocument document)
			: base(false) {
			this.document = document;
		}
		public PrintingPermissions PrintingPermissions {
			get { return printingPermissions; }
			set { printingPermissions = value; }
		}
		public ChangingPermissions ChangingPermissions {
			get { return changingPermissions; }
			set { changingPermissions = value; }
		}
		public string PermissionsPassword {
			get { return permissionsPassword; }
			set { permissionsPassword = value; }
		}
		public string OpenPassword {
			get { return openPassword; }
			set { openPassword = value; }
		}
		public bool EnableCoping {
			get { return enableCoping; }
			set { enableCoping = value; }
		}
		public bool EnableScreenReaders {
			get { return enableScreenReaders; }
			set { enableScreenReaders = value; }
		}
		public bool IsEncryptionOn { get { return !string.IsNullOrEmpty(openPassword) || !string.IsNullOrEmpty(permissionsPassword); } }
		public byte[] Key { get { return key; } }
		int GetPermissions() {
			BitArray bits = new BitArray(32);
			bits.Set(0, false);
			bits.Set(1, false);
			bits.Set(6, true);
			bits.Set(7, true);
			for(int i = 12; i < 32; i++)
				bits.Set(i, true);
			switch(PrintingPermissions) {
				case PrintingPermissions.None:
					break;
				case PrintingPermissions.LowResolution:
					bits.Set(2, true);
					break;
				case PrintingPermissions.HighResolution:
					bits.Set(2, true);
					bits.Set(11, true);
					break;
			}
			switch(ChangingPermissions) {
				case ChangingPermissions.None:
					break;
				case ChangingPermissions.FillingSigning:
					bits.Set(8, true);
					break;
				case ChangingPermissions.CommentingFillingSigning:
					bits.Set(5, true);
					break;
				case ChangingPermissions.InsertingDeletingRotating:
					bits.Set(10, true);
					break;
				case ChangingPermissions.AnyExceptExtractingPages:
					bits.Set(3, true);
					bits.Set(5, true);
					bits.Set(8, true);
					break;
				default:
					throw new ArgumentException("ChangingPermissions");
			}
			bits.Set(9, EnableScreenReaders);
			bits.Set(4, EnableCoping);
			byte[] bytes = new byte[32];
			bits.CopyTo(bytes, 0);
			return BitConverter.ToInt32(bytes, 0);
		}
		PdfDictionary GetAesCryptFilter() {
			PdfDictionary dic = new PdfDictionary();
			dic.Add("CFM", new PdfName("AESV2"));
			dic.Add("Length", new PdfNumber(keyLength / 8));
			if(!string.IsNullOrEmpty(openPassword))
				dic.Add("AuthEvent", new PdfName("DocOpen"));
			return dic;
		}
		public override void FillUp() {
			if(!IsEncryptionOn)
				return;
			Dictionary.Add("Filter", new PdfName("Standard"));
			Dictionary.Add("V", new PdfNumber(version));
			Dictionary.Add("Length", new PdfNumber(keyLength));
			PdfDictionary cfDictionary = new PdfDictionary();
			PdfDictionary aesCryptFilter = GetAesCryptFilter();
			string cryptFilterName = "StdCF";
			cfDictionary.Add(cryptFilterName, aesCryptFilter);
			Dictionary.Add("CF", cfDictionary);
			Dictionary.Add("StmF", new PdfName(cryptFilterName));
			Dictionary.Add("StrF", new PdfName(cryptFilterName));
			Dictionary.Add("R", new PdfNumber(revision));
			Dictionary.Add("O", new PdfLiteralString(ownerPassword, false));
			Dictionary.Add("U", new PdfLiteralString(userPassword, false));
			Dictionary.Add("P", new PdfNumber(permissions));
			Dictionary.Add("EncryptMetadata", new PdfBoolean(encryptMetadata));
		}
		public void Calculate() {
			if(!IsEncryptionOn)
				return;
			ownerPassword = PdfEncryptHelper.ComputeOwnerPassword(openPassword, permissionsPassword, revision, keyLength);
			permissions = GetPermissions();
			key = PdfEncryptHelper.ComputeEncryptionKey(openPassword, ownerPassword, permissions, revision, keyLength, encryptMetadata, document.ID);
			userPassword = PdfEncryptHelper.ComputeUserPassword(key, revision, document.ID);
		}
		public string EncryptString(string text, int objectNumber, int generationNumber) {
			if(!IsEncryptionOn)
				return text;
			return PdfEncryptHelper.EncryptString(text, key, objectNumber, generationNumber);
		}
		public MemoryStream EncryptStream(MemoryStream stream, int objectNumber, int generationNumber) {
			if(!IsEncryptionOn)
				return stream;
			return PdfEncryptHelper.EncryptStream(stream, key, objectNumber, generationNumber);
		}
	}
	public class PdfEncryptHelper {
		const string passwordPaddingString = "\x28\xBF\x4E\x5E\x4E\x75\x8A\x41\x64\x00\x4E\x56\xFF\xFA\x01\x08\x2E\x2E\x00\xB6\xD0\x68\x3E\x80\x2F\x0C\xA9\xFE\x64\x53\x69\x7A";
		static void ARCFOUR(byte[] bytes, byte[] key) {
			Byte[] s = new Byte[256];
			Byte[] k = new Byte[256];
			Byte temp;
			int i, j;
			for(i = 0; i < 256; i++) {
				s[i] = (Byte)i;
				k[i] = key[i % key.GetLength(0)];
			}
			j = 0;
			for(i = 0; i < 256; i++) {
				j = (j + s[i] + k[i]) % 256;
				temp = s[i];
				s[i] = s[j];
				s[j] = temp;
			}
			i = j = 0;
			for(int x = 0; x < bytes.GetLength(0); x++) {
				i = (i + 1) % 256;
				j = (j + s[i]) % 256;
				temp = s[i];
				s[i] = s[j];
				s[j] = temp;
				int t = (s[i] + s[j]) % 256;
				bytes[x] ^= s[t];
			}
		}
		static string AlignPassword(string password) {
			password = string.IsNullOrEmpty(password) ? string.Empty : password;
			if(password.Length > 32)
				return password.Substring(0, 32);
			else if(password.Length < 32)
				return password + passwordPaddingString.Substring(0, 32 - password.Length);
			else
				return password;
		}
		static SymmetricAlgorithm GetAesAlgorithm(byte[] key, int objectNumber, int generationNumber) {
			System.Diagnostics.Debug.Assert(key.Length == 16);
			byte[] newKey = new byte[key.Length + 9];
			key.CopyTo(newKey, 0);
			byte[] objectNumbersBytes = BitConverter.GetBytes(objectNumber);
			Array.Copy(objectNumbersBytes, 0, newKey, key.Length, 3);
			byte[] generationNumberBytes = BitConverter.GetBytes(generationNumber);
			Array.Copy(generationNumberBytes, 0, newKey, key.Length + 3, 2);
			byte[] aesIdBytes = PdfStringUtils.GetIsoBytes("sAlT");
			Array.Copy(aesIdBytes, 0, newKey, key.Length + 5, 4);
			MD5 md5 = new MD5CryptoServiceProvider();
			md5.ComputeHash(newKey);
			RijndaelManaged alg = new RijndaelManaged();
			alg.BlockSize = 128;
			alg.KeySize = 128;
			alg.Padding = PaddingMode.PKCS7;
			alg.Mode = CipherMode.CBC;
			alg.Key = md5.Hash;
			alg.GenerateIV();
			return alg;
		}
		static int CalcKeyLength(int revision, int length) {
			return revision == 2 ? 5 : length / 8;
		}
		static string ComputeUserPasswordR2(byte[] key) {
			byte[] passwordPaddingBytes = PdfStringUtils.GetIsoBytes(passwordPaddingString);
			ARCFOUR(passwordPaddingBytes, key);
			return PdfStringUtils.GetIsoString(passwordPaddingBytes);
		}
		static string ComputeUserPasswordR3(byte[] key, byte[] id) {
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] passwordPaddingBytes = PdfStringUtils.GetIsoBytes(passwordPaddingString);
			md5.TransformBlock(passwordPaddingBytes, 0, passwordPaddingBytes.Length, passwordPaddingBytes, 0);
			md5.TransformFinalBlock(id, 0, id.Length);
			byte[] hash = md5.Hash;
			ARCFOUR(hash, key);
			for(int i = 1; i <= 19; i++) {
				byte[] newKey = new byte[key.Length];
				for(int k = 0; k < key.Length; k++)
					newKey[k] = (byte)(key[k] ^ (int)i);
				ARCFOUR(hash, newKey);
			}
			byte[] result = new byte[32];
			Array.Copy(hash, result, hash.Length);
			return PdfStringUtils.GetIsoString(result);
		}
		public static byte[] GetISOBytes(string text) {
			if(text == null)
				return null;
			int len = text.Length;
			byte[] b = new byte[len];
			for(int k = 0; k < len; ++k)
				b[k] = (byte)text[k];
			return b;
		}
		public static byte[] ComputeEncryptionKey(string openPassword, string ownerPassword, int permissions, int revision, int length, bool encryptMetada, byte[] id) {
			openPassword = AlignPassword(openPassword);
			byte[] openPasswordBytes = PdfStringUtils.GetIsoBytes(openPassword);
			MD5 md5 = new MD5CryptoServiceProvider();
			md5.TransformBlock(openPasswordBytes, 0, openPasswordBytes.Length, openPasswordBytes, 0);
			byte[] ownerPasswordBytes = PdfStringUtils.GetIsoBytes(ownerPassword);
			md5.TransformBlock(ownerPasswordBytes, 0, ownerPasswordBytes.Length, ownerPasswordBytes, 0);
			byte[] permissionsBytes = BitConverter.GetBytes((uint)permissions);
			md5.TransformBlock(permissionsBytes, 0, permissionsBytes.Length, permissionsBytes, 0);
			if(id != null && id.Length > 0) {
				byte[] idBytes = (byte[])id.Clone();
				md5.TransformBlock(idBytes, 0, idBytes.Length, idBytes, 0);
			}
				if(revision >= 4 && !encryptMetada) {
				byte[] additionalBytes = BitConverter.GetBytes(0xffffffff);
				md5.TransformBlock(additionalBytes, 0, additionalBytes.Length, additionalBytes, 0);
			}
			md5.TransformFinalBlock(new byte[0], 0, 0);
			int n = CalcKeyLength(revision, length);
			byte[] key = new byte[n];
			Array.Copy(md5.Hash, key, n);
			if(revision >= 3) {
				md5.Initialize();
				for(int i = 0; i < 50; i++) {
					Array.Copy(md5.ComputeHash(key), key, key.Length);
					md5.Initialize();
				}
			}
			return key;
		}
		public static string ComputeUserPassword(byte[] key, int revision, byte[] id) {
			if(revision == 2)
				return ComputeUserPasswordR2(key);
			else
				return ComputeUserPasswordR3(key, id);
		}
		public static string ComputeOwnerPassword(string openPassword, string permissionsPassword, int revision, int length) {
			permissionsPassword = string.IsNullOrEmpty(permissionsPassword) ? openPassword : permissionsPassword;
			permissionsPassword = AlignPassword(permissionsPassword);
			byte[] permissionsPasswordBytes = PdfStringUtils.GetIsoBytes(permissionsPassword);
			MD5 md5 = new MD5CryptoServiceProvider();
			permissionsPasswordBytes = md5.ComputeHash(permissionsPasswordBytes);
			if(revision >= 3) {
				for(int i = 0; i < 50; i++)
					permissionsPasswordBytes = md5.ComputeHash(permissionsPasswordBytes);
			}
			int n = CalcKeyLength(revision, length);
			byte[] key = new byte[n];
			Array.Copy(permissionsPasswordBytes, key, key.Length);
			openPassword = AlignPassword(openPassword);
			byte[] openPasswordBytes = PdfStringUtils.GetIsoBytes(openPassword);
			PdfEncryptHelper.ARCFOUR(openPasswordBytes, key);
			if(revision >= 3) {
				for(int i = 1; i <= 19; i++) {
					byte[] newKey = new byte[key.Length];
					for(int k = 0; k < key.Length; k++)
						newKey[k] = (byte)((int)key[k] ^ i);
					ARCFOUR(openPasswordBytes, newKey);
				}
			}
			return PdfStringUtils.GetIsoString(openPasswordBytes);
		}
		public static MemoryStream EncryptStream(MemoryStream stream, byte[] key, int objectNumber, int generationNumber) {
			byte[] data = stream.ToArray();
			byte[] ecryptedData = EncryptData(data, key, objectNumber, generationNumber);
			return new MemoryStream(ecryptedData);
		}
		public static string EncryptString(string text, byte[] key, int objectNumber, int generationNumber) {
			byte[] data = PdfStringUtils.GetIsoBytes(text);
			byte[] encrypredBytes = EncryptData(data, key, objectNumber, generationNumber);
			return PdfStringUtils.GetIsoString(encrypredBytes);
		}
		public static byte[] EncryptData(byte[] data, byte[] key, int objectNumber, int generationNumber) {
			SymmetricAlgorithm alg = GetAesAlgorithm(key, objectNumber, generationNumber);
			using(MemoryStream encryptedStream = new MemoryStream())
			using(CryptoStream cryptoStream = new CryptoStream(encryptedStream, alg.CreateEncryptor(), CryptoStreamMode.Write)) {
				cryptoStream.Write(alg.IV, 0, alg.IV.Length);
				cryptoStream.Write(data, 0, data.Length);
				cryptoStream.FlushFinalBlock();
				return encryptedStream.ToArray();
			}
		}
	}
}
