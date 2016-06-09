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
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
namespace DevExpress.Pdf.Native {
	public partial class PdfEncryptionInfo {
		[SecuritySafeCritical]
		static byte[] GetAnsiPasswordString(SecureString password) {
			if (password == null)
				return new byte[0];
			int passwordLength = password.Length;
			byte[] passwordBytes = new byte[passwordLength];
			IntPtr ptr = Marshal.SecureStringToGlobalAllocAnsi(password);
			try {
				Marshal.Copy(ptr, passwordBytes, 0, passwordLength);
			}
			finally {
				Marshal.ZeroFreeCoTaskMemAnsi(ptr);
			}
			return passwordBytes;
		}
		[SecuritySafeCritical]
		static byte[] GetUnicodePasswordString(SecureString password) {
			if (password == null)
				return new byte[0];
			int passwordLength = password.Length;
			short[] passwordCodes = new short[passwordLength];
			IntPtr ptr = Marshal.SecureStringToGlobalAllocUnicode(password);
			try {
				Marshal.Copy(ptr, passwordCodes, 0, passwordLength);
			}
			finally {
				Marshal.ZeroFreeCoTaskMemUnicode(ptr);
			}
			StringBuilder sb = new StringBuilder();
			foreach (short code in passwordCodes) 
				sb.Append((char)code);
			return Encoding.UTF8.GetBytes(sb.ToString());
		}
		static byte[] EncryptAesData(byte[] key, byte[] initializationVector, byte[] data) {
			using (AesCryptoServiceProvider provider = new AesCryptoServiceProvider()) {
				provider.Padding = PaddingMode.None;
				using (ICryptoTransform transform = provider.CreateEncryptor(key, initializationVector))
					using (MemoryStream stream = new MemoryStream())
						using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write)) {
							cryptoStream.Write(data, 0, data.Length);
							cryptoStream.FlushFinalBlock();
							return stream.ToArray();
						}
			}
		}
		static byte[] DecryptAesData(CipherMode mode, PaddingMode padding, byte[] key, byte[] initializationVector, byte[] data, int dataPosition) {
			using (AesCryptoServiceProvider provider = new AesCryptoServiceProvider()) {
				provider.Mode = mode;
				provider.Padding = padding;
				using (ICryptoTransform transform = provider.CreateDecryptor(key, initializationVector))
					using (MemoryStream input = new MemoryStream(data)) {
						input.Position = dataPosition;
						using (CryptoStream cryptoStream = new CryptoStream(input, transform, CryptoStreamMode.Read))
							using (MemoryStream output = new MemoryStream()) {
								cryptoStream.CopyTo(output);
								return output.ToArray();
							}
					}
			}
		}
	}
}
