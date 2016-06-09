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
using DevExpress.Utils;
using DevExpress.Utils.Crypt;
namespace DevExpress.XtraSpreadsheet.Internal {
	#region ARC4KeyGen
	public class ARC4KeyGen {
		const int saltLength = 16;
		const int truncatedHashLength = 5;
		const int blockLength = truncatedHashLength + saltLength;
		byte[] salt = new byte[saltLength];
		byte[] truncatedHash = new byte[truncatedHashLength];
		public ARC4KeyGen(string password) {
			Guard.ArgumentNotNull(password, "password");
			Random random = new Random();
			random.NextBytes(this.salt);
			CalcTruncatedHash(password);
		}
		public ARC4KeyGen(string password, byte[] salt) {
			Guard.ArgumentNotNull(password, "password");
			Guard.ArgumentNotNull(salt, "salt");
			if(salt.Length != saltLength)
				throw new ArgumentException("Salt must be 16 bytes long");
			Array.Copy(salt, this.salt, saltLength);
			CalcTruncatedHash(password);
		}
		public byte[] Salt { get { return salt; } }
		public byte[] DeriveKey(int blockNumber) {
			byte[] buf = new byte[truncatedHashLength + 4];
			Array.Copy(this.truncatedHash, buf, truncatedHashLength);
			Array.Copy(BitConverter.GetBytes(blockNumber), 0, buf, truncatedHashLength, 4);
			return MD5Hash.ComputeHash(buf);
		}
		void CalcTruncatedHash(string password) {
			byte[] pwdBytes = Encoding.Unicode.GetBytes(password);
			byte[] pwdHash = MD5Hash.ComputeHash(pwdBytes);
			byte[] intermediateBuffer = new byte[blockLength * 16]; 
			for(int i = 0; i < 16; i++) {
				int offset = i * blockLength;
				Array.Copy(pwdHash, 0, intermediateBuffer, offset, truncatedHashLength);
				Array.Copy(this.salt, 0, intermediateBuffer, offset + truncatedHashLength, saltLength);
			}
			byte[] hash = MD5Hash.ComputeHash(intermediateBuffer);
			Array.Copy(hash, this.truncatedHash, truncatedHashLength);
		}
	}
	#endregion
	#region ARC4PasswordVerifier
	public class ARC4PasswordVerifier {
		const int bytesLength = 16;
		byte[] salt = new byte[bytesLength];
		byte[] encryptedVerifier = new byte[bytesLength];
		byte[] encryptedVerifierHash = new byte[bytesLength];
		public ARC4PasswordVerifier(ARC4KeyGen keygen) {
			Guard.ArgumentNotNull(keygen, "keygen");
			Array.Copy(keygen.Salt, this.salt, bytesLength);
			byte[] verifier = new byte[bytesLength];
			Random random = new Random();
			random.NextBytes(verifier);
			byte[] verifierHash = MD5Hash.ComputeHash(verifier);
			ARC4Cipher cipher = new ARC4Cipher(keygen.DeriveKey(0));
			encryptedVerifier = cipher.Encrypt(verifier);
			encryptedVerifierHash = cipher.Encrypt(verifierHash);
		}
		public ARC4PasswordVerifier(byte[] salt, byte[] encryptedVerifier, byte[] encryptedVerifierHash) {
			Guard.ArgumentNotNull(salt, "salt");
			Guard.ArgumentNotNull(encryptedVerifier, "encryptedVerifier");
			Guard.ArgumentNotNull(encryptedVerifierHash, "encryptedVerifierHash");
			Array.Copy(salt, this.salt, bytesLength);
			Array.Copy(encryptedVerifier, this.encryptedVerifier, bytesLength);
			Array.Copy(encryptedVerifierHash, this.encryptedVerifierHash, bytesLength);
		}
		#region Properties
		public byte[] Salt {
			get { return salt; }
		}
		public byte[] EncryptedVerifier {
			get { return encryptedVerifier; }
		}
		public byte[] EncryptedVerifierHash {
			get { return encryptedVerifierHash; }
		}
		#endregion
		public bool VerifyPassword(string password) {
			ARC4KeyGen keygen = new ARC4KeyGen(password, this.salt);
			ARC4Cipher cipher = new ARC4Cipher(keygen.DeriveKey(0));
			byte[] verifier = cipher.Decrypt(encryptedVerifier);
			byte[] verifierHash = cipher.Decrypt(encryptedVerifierHash);
			byte[] hash = MD5Hash.ComputeHash(verifier);
			for(int i = 0; i < bytesLength; i++) {
				if(hash[i] != verifierHash[i])
					return false;
			}
			return true;
		}
	}
	#endregion
}
