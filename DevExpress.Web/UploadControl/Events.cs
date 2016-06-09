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
using System.Text;
namespace DevExpress.Web {
	public class FileUploadCompleteEventArgs : EventArgs {
		private string callbackData;
		private string errorText;
		private UploadedFile uploadedFile;
		public string CallbackData {
			get { return this.callbackData; }
			set { this.callbackData = value; }
		}
		public string ErrorText {
			get { return errorText; }
			set { errorText = value; }
		}
		public bool IsValid {
			get { return uploadedFile.IsValid; }
			set { uploadedFile.IsValid = value; }
		}
		public UploadedFile UploadedFile {
			get { return uploadedFile; }
		}
		public FileUploadCompleteEventArgs(string callBackData, string errorText, UploadedFile uploadedFile)
			: this(callBackData, true, errorText, uploadedFile) {
		}
		public FileUploadCompleteEventArgs(string callBackData, bool isValid, string errorText, UploadedFile uploadedFile)
			: base() {
			this.callbackData = callBackData;
			this.errorText = errorText;
			this.uploadedFile = uploadedFile;
		}
	}
	public class FilesUploadCompleteEventArgs : EventArgs {
		private string callbackData;
		private string errorText;
		public string CallbackData {
			get { return this.callbackData; }
			set { this.callbackData = value; }
		}
		public string ErrorText {
			get { return errorText; }
			set { errorText = value; }
		}
		public FilesUploadCompleteEventArgs(string callBackData, string errorText)
			: base() {
			this.callbackData = callBackData;
			this.errorText = errorText;
		}
	}
}
