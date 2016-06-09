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
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Import;
using System.IO;
using DevExpress.Snap.Core.API;
using DevExpress.Office.Utils;
namespace DevExpress.Snap.Core.Commands {
	public class PasteSnxCommand : PasteContentConvertedToDocumentModelCommandBase {
		public PasteSnxCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return SnapCommandId.PasteSnxCommand; } }
		protected internal override PieceTablePasteContentConvertedToDocumentModelCommandBase CreateInnerCommandCore() {
			return new PieceTablePasteSnxCommand((SnapPieceTable)ActivePieceTable);
		}
	}
	public class PieceTablePasteSnxCommand : PieceTablePasteContentConvertedToDocumentModelCommandBase {
		public PieceTablePasteSnxCommand(SnapPieceTable pieceTable)
			: base(pieceTable) {
				this.CopyBetweenInternalModels = true;
		}
		public override DocumentFormat Format { get { return SnapDocumentFormat.Snap; } }
		protected internal override DocumentModel CreateSourceDocumentModel(string sizeCollection) {
			byte[] content = GetContentBytes();
			return CreateDocumentModelFromContentBytes(content, sizeCollection);
		}
		protected internal virtual byte[] GetContentBytes() {
			return PasteSource.GetData(SnapDataFormats.Snx) as byte[];
		}
		protected internal virtual DocumentModel CreateDocumentModelFromContentBytes(byte[] content, string sizeCollection) {
			if (content == null || content.Length == 0)
				return null;
			DocumentModel result = PieceTable.DocumentModel.CreateNew();
			result.IntermediateModel = true;
			if (!PieceTable.IsMain) {
				result.DocumentCapabilities.HeadersFooters = DocumentCapability.Disabled;
				result.DocumentCapabilities.Sections = DocumentCapability.Disabled;
			}
			PopulateDocumentModelFromContentBytesCore(result, content, sizeCollection);
			return result;
		}
		protected internal virtual void PopulateDocumentModelFromContentBytesCore(DocumentModel model, byte[] content, string sizeCollection) {
			model.DeleteDefaultNumberingList(model.NumberingLists);
			SnapDocumentImporterOptions options = new SnapDocumentImporterOptions();
			using (SnapImporter importer = new SnapImporter((SnapDocumentModel)model, options)) {
				importer.Import(new MemoryStream(content));
			}
			SetSuppressStoreImageSize(model, sizeCollection);
		}
		protected internal override bool IsDataAvailable() {
			return PasteSource.ContainsData(SnapDataFormats.Snx);
		}
		protected internal override string GetAdditionalContentString() {
			return PasteSource.GetData(OfficeDataFormats.SuppressStoreImageSize) as string;
		}
	}
}
