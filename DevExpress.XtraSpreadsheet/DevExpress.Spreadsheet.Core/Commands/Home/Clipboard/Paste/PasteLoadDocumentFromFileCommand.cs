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
using DevExpress.Office.Utils;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Localization;
using System.IO;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region PasteLoadDocumentFromFileCommand
	public class PasteLoadDocumentFromFileCommand : PasteCommandBase {
		public PasteLoadDocumentFromFileCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override DocumentFormat Format { get { return DocumentFormat.Undefined; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		protected internal override void PerformModifyModel() {
			FileInfo[] files = GetFileDropListData();
			if (files == null || files.Length != 1)
				return;
			try {
#if !SL
				if (DocumentServer.CanCloseExistingDocument())
					DocumentServer.LoadDocument(files[0].FullName);
#else
					Stream stream = files[0].OpenRead();
					DocumentFormat format = DocumentModel.AutodetectDocumentFormat(files[0].Name);
					DocumentModel.LoadDocument(stream, format, String.Empty);
#endif
			}
			catch {
			}
		}
		protected internal override bool ChangeSelection() {
			return false;
		}
		protected internal virtual bool CanLoadFile(string fileName) {
			return DocumentModel.AutodetectDocumentFormat(fileName, false) != DocumentFormat.Undefined;
		}
		protected internal FileInfo[] GetFileDropListData() {
#if !SL
			object data = PasteSource.GetData(OfficeDataFormats.FileDrop, true);
			FileInfo[] result = data as FileInfo[];
			if (result != null)
				return result;
			string[] fileNames = data as string[];
			if (fileNames == null)
				return null;
			List<FileInfo> files = new List<FileInfo>();
			int count = fileNames.Length;
			for (int i = 0; i < count; i++)
				files.Add(new FileInfo(fileNames[i]));
			return files.ToArray();
#else
				return null;
#endif
		}
		protected internal override bool IsDataAvailable() {
#if !SL
			if (!PasteSource.ContainsData(OfficeDataFormats.FileDrop, true))
				return false;
			FileInfo[] files = GetFileDropListData();
			if (files == null || files.Length != 1)
				return false;
			return CanLoadFile(files[0].Name);
#else
				return true;
#endif
		}
	}
	#endregion
}
