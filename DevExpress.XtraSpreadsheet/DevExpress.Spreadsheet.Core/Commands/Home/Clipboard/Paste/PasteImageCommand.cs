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
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Localization;
using System.Drawing;
namespace DevExpress.XtraSpreadsheet.Commands {
#if !SL && !DXPORTABLE
	#region PasteImageCommand
	public class PasteImageCommand : PasteCommandBase {
		public PasteImageCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override DocumentFormat Format { get { return DocumentFormat.Undefined; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		protected internal override void PerformModifyModel() {
			Image image = AquireImage();
			if (image != null)
				InsertPicture(new MemoryStreamBasedImage(image, null));
		}
		protected internal override bool ChangeSelection() {
			return false;
		}
		protected internal virtual Image AquireImage() {
			return PasteSource.GetData(OfficeDataFormats.Bitmap, true) as Image;
		}
		protected internal override bool IsDataAvailable() {
			return PasteSource.ContainsData(OfficeDataFormats.Bitmap, true);
		}
		protected internal virtual bool InsertPicture(MemoryStreamBasedImage streamBasedImage) {
			if (streamBasedImage == null || streamBasedImage.Image == null)
				return false;
			if (!DocumentModel.DocumentCapabilities.PicturesAllowed)
				return false;
			InsertPictureCommand command = new InsertPictureCommand(Control);
			OfficeImage officeImage = new OfficeReferenceImage(DocumentModel, OfficeImage.CreateImage(streamBasedImage));
			command.InsertPicture(officeImage);
			return true;
		}
	}
	#endregion
#endif
}
#endif
