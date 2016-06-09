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
namespace DevExpress.Office.Crypto.Agile {
	class KeyManager {
		PasswordKeyEncryptor passwordKeyEncryptor;
		byte[] secretKey;
		public KeyManager(EncryptionInfo config) {
			this.secretKey = new byte[config.CipherInfo.BlockBits / 8];
			EncryptionSession.GetRandomBytes(this.secretKey);
			this.passwordKeyEncryptor = new PasswordKeyEncryptor(config.PasswordKeyEncryptorInfo);
		}
		public KeyManager(EncryptionInfo config, PasswordKeyEncryptorData data) {
			this.passwordKeyEncryptor = new PasswordKeyEncryptor(config.PasswordKeyEncryptorInfo, data);
			if (data.SecretKey != null)
				secretKey = (byte[])data.SecretKey.Clone();
		}
		public bool IsLocked { get { return secretKey == null; } }
		public bool IsUnlocked { get { return !IsLocked; } }
		public byte[] SecretKey { get { return secretKey; } }
		public PasswordKeyEncryptor PasswordKeyEncryptor { get { return passwordKeyEncryptor; } }
		public void SetPassword(string password) {
			if (secretKey == null)
				throw new InvalidOperationException("Not unlocked");
			passwordKeyEncryptor.SetPassword(password, secretKey);
		}
		public bool UnlockWithPassword(string password) {
			if (secretKey != null)
				throw new InvalidOperationException("Already unlocked");
			return passwordKeyEncryptor.TryUnlock(password, out secretKey);
		}
		internal void Save(EncryptionInfo config, EncryptionData data) {
			config.PasswordKeyEncryptorInfo = new PasswordKeyEncryptorInfo();
			data.PasswordEncryptorData = new PasswordKeyEncryptorData();
			data.PasswordEncryptorData.SecretKey = secretKey;
			passwordKeyEncryptor.Save(config.PasswordKeyEncryptorInfo, data.PasswordEncryptorData);
		}
	}
}
