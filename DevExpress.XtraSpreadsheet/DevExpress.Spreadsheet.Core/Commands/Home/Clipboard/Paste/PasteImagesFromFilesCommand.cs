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

#if !SL
using System;
using System.Collections.Generic;
using DevExpress.Office.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Localization;
using System.IO;
namespace DevExpress.XtraSpreadsheet.Commands {
#if !SL && !DXPORTABLE
	#region PasteImagesFromFilesCommand
	public class PasteImagesFromFilesCommand : PasteCommandBase {
		public PasteImagesFromFilesCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override DocumentFormat Format { get { return DocumentFormat.Undefined; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		protected internal override void PerformModifyModel() {
			string[] files = GetFileDropListData();
			if (files == null || files.Length == 0)
				return;
			foreach (string fileName in files) {
				if (File.Exists(fileName))
					PasteImageFromFile(fileName);
			}
		}
		protected internal override bool ChangeSelection() {
			return false;
		}
		protected internal virtual void PasteImageFromFile(string fileName) {
			MemoryStreamBasedImage image = null;
			try {
				image = ImageLoaderHelper.ImageFromFile(fileName);
			}
			catch {
			}
			if (image != null) {
				PasteImageCommand command = new PasteImageCommand(Control);
				command.InsertPicture(image);
			}
		}
		protected internal string[] GetFileDropListData() {
			return PasteSource.GetData(OfficeDataFormats.FileDrop, true) as string[];
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Pictures, state.Enabled);
		}
		protected internal override bool IsDataAvailable() {
			return PasteSource.ContainsData(OfficeDataFormats.FileDrop, true);
		}
	}
	#endregion
#endif
}
#endif
