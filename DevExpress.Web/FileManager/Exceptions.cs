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
using System.ComponentModel;
using System.Net;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public class FileManagerException : Exception {
		FileManagerErrors error;
		public FileManagerException(FileManagerErrors error)
			: this(error, null) { }
		public FileManagerException(FileManagerErrors error, Exception innerException)
			: this(error, innerException, string.Empty) {
		}
		public FileManagerException(FileManagerErrors error, Exception innerException, string message)
			: base(GetErrorMessage(error, message), innerException) {
			this.error = error;
		}
#if !SL
	[DevExpressWebLocalizedDescription("FileManagerExceptionError")]
#endif
		public FileManagerErrors Error { get { return error; } }
		protected static string GetErrorMessage(FileManagerErrors error, string message) {
			string errorText = GetErrorText(error);
			string separator = string.Empty;
			if(!(string.IsNullOrEmpty(errorText) || string.IsNullOrEmpty(message)))
				separator = "\n";
			return string.Format("{0}{1}{2}", errorText, separator, message);
		}
		protected static string GetErrorText(FileManagerErrors result) {
			switch(result) {
				case FileManagerErrors.UnspecifiedIO:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorIO);
				case FileManagerErrors.FileNotFound:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorFileNotFound);
				case FileManagerErrors.FolderNotFound:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorFolderNotFound);
				case FileManagerErrors.AccessDenied:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorNoAccess);
				case FileManagerErrors.EmptyName:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorNameCannotBeEmpty);
				case FileManagerErrors.CanceledOperation:
					return string.Empty;
				case FileManagerErrors.InvalidSymbols:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorInvalidSymbols);
				case FileManagerErrors.WrongExtension:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorWrongExtension);
				case FileManagerErrors.UsedByAnotherProcess:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorUsedByAnotherProcess);
				case FileManagerErrors.AlreadyExists:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorAlreadyExists);
				case FileManagerErrors.AccessProhibited:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorAccessProhibited);
				case FileManagerErrors.CloudAccessFailed:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorCloudAccessFailed);
				case FileManagerErrors.PathToLong:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorPathToLong);
				case FileManagerErrors.WrongIdPathLength:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorWrongIdPathLength);
				default:
					return ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorOther);
			}
		}
	}
	public class FileManagerIOException : FileManagerException {
		public FileManagerIOException(FileManagerErrors error)
			: base(error) { }
		public FileManagerIOException(FileManagerErrors error, Exception innerException)
			: base(error, innerException) { }
	}
	public class FileManagerAccessException : FileManagerException {
		public FileManagerAccessException()
			: base(FileManagerErrors.AccessProhibited) { }
	}
	public class FileManagerCancelException : FileManagerException {
		public FileManagerCancelException(string message)
			: base(FileManagerErrors.CanceledOperation, null, message) { }
	}
	public class FileManagerCloudAccessFailedException : FileManagerException {
		public FileManagerCloudAccessFailedException(HttpStatusCode responseCode, string requestResult)
			: base(FileManagerErrors.CloudAccessFailed, null, string.Format("StatusCode: {0}\nRequest result: {1}", responseCode, requestResult)) { }
	}
	public enum FileManagerErrors {
		FileNotFound = 0,
		FolderNotFound = 1,
		AccessDenied = 2,
		UnspecifiedIO = 3,
		Unspecified = 4,
		EmptyName = 5,
		CanceledOperation = 6,
		InvalidSymbols = 7,
		WrongExtension = 8,
		UsedByAnotherProcess = 9,
		AlreadyExists = 10,
		AccessProhibited = 11,
		CloudAccessFailed = 12,
		PathToLong = 13,
		WrongIdPathLength = 14
	}
}
