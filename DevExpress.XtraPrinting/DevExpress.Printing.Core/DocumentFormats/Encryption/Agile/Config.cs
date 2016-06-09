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

using System.IO;
using System.Security.Cryptography;
namespace DevExpress.Office.Crypto.Agile {
	public class EncryptionInfo {
		public CipherInfo CipherInfo { get; set; }
		public HashInfo HashInfo { get; set; }
		public PasswordKeyEncryptorInfo PasswordKeyEncryptorInfo { get; set; }
		public int SaltSize { get; set; }
	}
	public class CipherInfo {
		public int BlockBits { get; set; }
		public int KeyBits { get; set; }
		public string Name { get; set; }
		public string Mode { get; set; }
		public CipherInfo Clone() {
			return (CipherInfo)this.MemberwiseClone();
		}
		public SymmetricAlgorithm GetAlgorithm() {
			SymmetricAlgorithm cipherAlgorithm = CreateSymmetricAlgorithm(this.Name);
			cipherAlgorithm.BlockSize = BlockBits;
			cipherAlgorithm.KeySize = KeyBits;
			cipherAlgorithm.Padding = PaddingMode.None;
			if (Mode == "ChainingModeCBC")
				cipherAlgorithm.Mode = CipherMode.CBC;
			else if (Mode == "ChainingModeCFB")
#if DXPORTABLE
				cipherAlgorithm.Mode = (CipherMode)4; 
#else
				cipherAlgorithm.Mode = CipherMode.CFB;
#endif
			else
				throw new InvalidDataException("Unexpected chaining mode");
			return cipherAlgorithm;
		}
		SymmetricAlgorithm CreateSymmetricAlgorithm(string name) {
#if DXPORTABLE
			switch (name) {
				default:
					return null;
				case "AES":
					return Aes.Create();
			}
#else
			return SymmetricAlgorithm.Create(name);
#endif
		}
	}
	public class HashInfo {
		public int HashBits { get; set; }
		public string Name { get; set; }
		public HashInfo Clone() {
			return (HashInfo)this.MemberwiseClone();
		}
		public HashAlgorithm GetAlgorithm() {
			HashAlgorithm hashAlgorithm;
#if DXRESTRICTED
			switch (Name) {
				case "SHA256":
					hashAlgorithm = SHA256.Create();
					break;
				case "SHA384":
					hashAlgorithm = SHA384.Create();
					break;
				case "SHA512":
					hashAlgorithm = SHA512.Create();
					break;
				case "SHA1":
					hashAlgorithm = SHA1.Create();
					break;
				case "MD5":
					hashAlgorithm = MD5.Create();
					break;
				default:
					hashAlgorithm = null;
					break;
			}
			if (hashAlgorithm == null)
				return null;
#else
			switch (Name) {
				case "SHA256":
					hashAlgorithm = new SHA256CryptoServiceProvider();
					break;
				case "SHA384":
					hashAlgorithm = new SHA384CryptoServiceProvider();
					break;
				case "SHA512":
					hashAlgorithm = new SHA512CryptoServiceProvider();
					break;
				default:
					hashAlgorithm = HashAlgorithm.Create(this.Name);
					break;
			}
#endif
			if (hashAlgorithm.HashSize != this.HashBits)
				throw new InvalidDataException("Unexpected hash size");
			hashAlgorithm.Initialize();
			return hashAlgorithm;
		}
	}
	public class PasswordKeyEncryptorInfo {
		public HashInfo HashInfo { get; set; }
		public CipherInfo CipherInfo { get; set; }
		public int SaltSize { get; set; }
		public int SpinCount { get; set; }
	}
}
