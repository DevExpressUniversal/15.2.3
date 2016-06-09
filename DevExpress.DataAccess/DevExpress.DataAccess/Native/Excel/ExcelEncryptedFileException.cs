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
using DevExpress.DataAccess.Localization;
using DevExpress.SpreadsheetSource;
namespace DevExpress.DataAccess.Native.Excel {
	public class ExcelEncryptedFileException : Exception {
		public ExcelEncryptedFileException(string message, EncryptedFileException innerException) : base(message, innerException) {}
	}
	public static class ExcelExceptionWrapper {
		public static ExcelEncryptedFileException Wrap(this EncryptedFileException spreadsheetException) {
			switch(spreadsheetException.Error) {
				case EncryptedFileError.PasswordRequired:
					return new ExcelEncryptedFileException(DataAccessLocalizer.GetString(DataAccessStringId.ExcelEncryptedFileException_PasswordRequired), spreadsheetException);
				case EncryptedFileError.WrongPassword:
					return new ExcelEncryptedFileException(DataAccessLocalizer.GetString(DataAccessStringId.ExcelEncryptedFileException_WrongPassword), spreadsheetException);
				case EncryptedFileError.EncryptionTypeNotSupported:
					return new ExcelEncryptedFileException(DataAccessLocalizer.GetString(DataAccessStringId.ExcelEncryptedFileException_EncryptionTypeNotSupported), spreadsheetException);
				default:
					return new ExcelEncryptedFileException(spreadsheetException.Message, spreadsheetException);
			}
		}
	}
}
