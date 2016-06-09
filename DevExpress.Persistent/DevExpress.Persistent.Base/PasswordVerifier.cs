#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
namespace DevExpress.Persistent.Base {
	public class PasswordCryptographer {
		const int saltLength = 6;
		const string delim = "*";
		private string SaltPassword(string password, string salt) {
			SHA512 hashAlgorithm = SHA512.Create();
			return Convert.ToBase64String(hashAlgorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(salt + password)));
		}
		public virtual string GenerateSaltedPassword(string password) {
			if(string.IsNullOrEmpty(password))
				return password;
			byte[] randomSalt = new byte[saltLength];
			new RNGCryptoServiceProvider().GetBytes(randomSalt);
			string salt = Convert.ToBase64String(randomSalt);
			return salt + delim + SaltPassword(password, salt);
		}
		public virtual bool AreEqual(string saltedPassword, string password) {
			if(string.IsNullOrEmpty(saltedPassword))
				return string.IsNullOrEmpty(password);
			if(string.IsNullOrEmpty(password)) {
				return false;
			}
			int delimPos = saltedPassword.IndexOf(delim);
			if(delimPos <= 0) {
				return saltedPassword.Equals(password);
			}
			else {
				string calculatedSaltedPassword = SaltPassword(password, saltedPassword.Substring(0, delimPos));
				string expectedSaltedPassword = saltedPassword.Substring(delimPos + delim.Length);
				if(expectedSaltedPassword.Equals(calculatedSaltedPassword)) {
					return true;
				}
				return expectedSaltedPassword.Equals(SaltPassword(password, "System.Byte[]"));
			}
		}
	}
}
