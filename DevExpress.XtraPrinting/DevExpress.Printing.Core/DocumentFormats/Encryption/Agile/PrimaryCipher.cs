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
using System.Security.Cryptography;
using DevExpress.Utils;
namespace DevExpress.Office.Crypto.Agile {
	class PrimaryCipher : ICipherProvider {
		HashInfo hashInfo;
		SymmetricAlgorithm cipherAlgorithm;
		byte[] secretKey;
		byte[] saltValue;
		public PrimaryCipher(HashInfo hashInfo, CipherInfo cipherInfo, byte[] saltValue, byte[] secretKey) {
			this.hashInfo = hashInfo;
			this.cipherAlgorithm = cipherInfo.GetAlgorithm();
			this.secretKey = (byte[])secretKey.Clone();
			this.saltValue = (byte[])saltValue.Clone();
		}
		public byte[] SaltValue { get { return (byte[])saltValue.Clone(); } }
		public byte[] SecretKey { get { return (byte[])secretKey.Clone(); } }
		public int BlockBytes { get { return cipherAlgorithm.BlockSize / 8; } }
		public HashAlgorithm GetHashAlgorithm() { return hashInfo.GetAlgorithm(); }
		public ICryptoTransform GetCryptoTransform(byte[] blockKey, bool isEncryption) {
			byte[] iv;
			using (HashAlgorithm hashAlgorithm = GetHashAlgorithm()) {
				iv = hashAlgorithm.Transform2Blocks(saltValue, blockKey).CloneToFit(BlockBytes);
			}
			if (isEncryption)
				return cipherAlgorithm.CreateEncryptor(secretKey, iv);
			return cipherAlgorithm.CreateDecryptor(secretKey, iv);
		}
	}
}
