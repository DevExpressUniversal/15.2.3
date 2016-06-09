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
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandFilePassword
	public class XlsCommandFilePassword : XlsCommandBase {
		#region Fields
		const int encryptionTypeSize = 2;
		public const string MagicMSPassword = "VelvetSweatshop";
		bool rc4Encrypted;
		XlsXORObfuscation xorObfuscation = new XlsXORObfuscation(0, 0);
		XlsRC4EncryptionHeaderBase rc4EncryptionHeader = new XlsRC4EncryptionHeader();
		#endregion
		#region Properties
		public bool RC4Encrypted { get { return rc4Encrypted;  } }
		public XlsXORObfuscation XORObfuscation {
			get { return this.xorObfuscation; }
			set {
				Guard.ArgumentNotNull(value, "XOR obfuscation value");
				this.xorObfuscation = value;
				this.rc4Encrypted = false;
			}
		}
		public XlsRC4EncryptionHeaderBase RC4EncryptionHeader {
			get { return this.rc4EncryptionHeader; }
			set {
				Guard.ArgumentNotNull(value, "RC4EncryptionHeader value");
				this.rc4EncryptionHeader = value;
				this.rc4Encrypted = true;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			this.rc4Encrypted = Convert.ToBoolean(reader.ReadUInt16());
			if(RC4Encrypted) {
				short versionMajor = reader.ReadInt16();
				short versionMinor = reader.ReadInt16();
				if(versionMajor == 1 && versionMinor == 1)
					this.rc4EncryptionHeader = new XlsRC4EncryptionHeader();
				else if(versionMajor >= 2 && versionMinor == 2)
					this.rc4EncryptionHeader = new XlsRC4CryptoAPIEncryptionHeader();
				else
					contentBuilder.ThrowInvalidFile("Unknown FilePass header version");
				reader.Seek(-4, SeekOrigin.Current);
				this.rc4EncryptionHeader.Read(reader);
			}
			else {
				this.xorObfuscation = XlsXORObfuscation.FromStream(reader);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)(this.rc4Encrypted ? 1 : 0));
			if(RC4Encrypted) {
				this.rc4EncryptionHeader.Write(writer);
			}
			else {
				this.xorObfuscation.Write(writer);
			}
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if(RC4Encrypted) {
				XlsRC4EncryptionHeader header = this.rc4EncryptionHeader as XlsRC4EncryptionHeader;
				if(header != null) { 
					ARC4PasswordVerifier verifier = new ARC4PasswordVerifier(header.Salt, header.EncryptedVerifier, header.EncryptedVerifierHash);
					string password = contentBuilder.Options.Password;
					bool tryingDefaultPassword = string.IsNullOrEmpty(password);
					if(tryingDefaultPassword)
						password = MagicMSPassword;
					if(!verifier.VerifyPassword(password)) {
						if (tryingDefaultPassword)
							throw new SpreadsheetDecryptionException(SpreadsheetDecryptionError.PasswordRequired, "Password required to import this file!");
						else
							throw new SpreadsheetDecryptionException(SpreadsheetDecryptionError.WrongPassword, "Wrong password!");
					}
					contentBuilder.SetupRC4Decryptor(password, header.Salt);
					if(contentBuilder.Options.Format == DevExpress.Spreadsheet.DocumentFormat.Xls && !tryingDefaultPassword)
						contentBuilder.DocumentModel.DocumentExportOptions.Xls.Password = password;
				}
				else {
					throw new SpreadsheetDecryptionException(SpreadsheetDecryptionError.EncryptionTypeNotSupported, 
						"Import of RC4 CryptoAPI encrypted files is not supported!");
				}
			}
			else {
				throw new SpreadsheetDecryptionException(SpreadsheetDecryptionError.EncryptionTypeNotSupported, 
					"Import of XOR obfuscated files is not supported!");
			}
		}
		protected override short GetSize() {
			return (short)(encryptionTypeSize + (RC4Encrypted ? this.rc4EncryptionHeader.GetSize() : this.xorObfuscation.GetSize()));
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandFilePassword();
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet {
	public enum SpreadsheetDecryptionError {
		PasswordRequired,
		WrongPassword,
		EncryptionTypeNotSupported
	}
	public class SpreadsheetDecryptionException : Exception {
		SpreadsheetDecryptionError error;
		public SpreadsheetDecryptionException(SpreadsheetDecryptionError error, string message)
			: base(message) {
			this.error = error;
		}
		public SpreadsheetDecryptionError Error { get { return error; } }
	}
}
