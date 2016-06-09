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
using DevExpress.XtraRichEdit.Model;
using System.Globalization;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Commands;
namespace DevExpress.XtraRichEdit.Fields {
	public partial class PageRefField : CalculatedFieldBase {
		#region FieldInitialization
		#region static
		public static readonly string FieldType = "PAGEREF";
		static readonly Dictionary<string, bool> switchesWithArgument = CreateSwitchesWithArgument();
		public static CalculatedFieldBase Create() {
			return new PageRefField();
		}
		#endregion
		bool createHyperlink;
		bool relativePosition;
		string bookmarkName;
		int targetPageNumber = -1;
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArgument; } }
		protected override string FieldTypeName { get { return FieldType; } }
		public override bool CanPrepare { get { return true; } }
		public bool CreateHyperlink { get { return createHyperlink; } }
		public bool RelativePosition { get { return relativePosition; } }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
			createHyperlink = instructions.GetBool("h");
			relativePosition = instructions.GetBool("p");
			bookmarkName = instructions.GetArgumentAsString(0);
		}
		#endregion
		protected internal override UpdateFieldOperationType GetAllowedUpdateFieldTypes(FieldUpdateOnLoadOptions options) {
			return UpdateFieldOperationType.Normal;
		}
		public override void BeforeCalculateFields(PieceTable sourcePieceTable, Field documentField) {
			targetPageNumber = -1;
			IDocumentLayoutService documentLayoutService = sourcePieceTable.DocumentModel.GetService<IDocumentLayoutService>();
			if (documentLayoutService == null)
				return;
			DocumentLayout documentLayout = documentLayoutService.CalculateDocumentLayout();
			Bookmark bookmark = FindBookmarkByName(sourcePieceTable, bookmarkName);
			if (bookmark == null)
				return;
			DocumentLayoutPosition layoutPosition = documentLayout.CreateLayoutPosition(bookmark.PieceTable, bookmark.NormalizedStart, -1);
			layoutPosition.Update(documentLayout.Pages, DocumentLayoutDetailsLevel.Page);
			if (!layoutPosition.IsValid(DocumentLayoutDetailsLevel.Page))
				return;
			targetPageNumber = layoutPosition.Page.PageOrdinal;
		}
		Bookmark FindBookmarkByName(PieceTable sourcePieceTable, string bookmarkName) {
			IBookmarkResolutionService resolutionService = sourcePieceTable.DocumentModel.GetService<IBookmarkResolutionService>();
			if (resolutionService == null)
				return sourcePieceTable.Bookmarks.FindByName(bookmarkName);
			else
				return resolutionService.FindBookmarkByName(bookmarkName);
		}
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			if (targetPageNumber < 0)
				return new CalculatedFieldValue(String.Empty);
			if (createHyperlink) {
				String code = GetHyperlinkCode();
				String text = GetHyperlinkText();
				DocumentModel targetModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
				PieceTable targetPieceTable = targetModel.MainPieceTable;
				targetPieceTable.InsertText(DocumentLogPosition.Zero, code);
				Field field = targetPieceTable.CreateField(DocumentLogPosition.Zero, code.Length);
				targetPieceTable.InsertText(new DocumentLogPosition(code.Length + 2), text);
				targetPieceTable.FieldUpdater.UpdateField(field, mailMergeDataMode);
				return new CalculatedFieldValue(targetModel);
			}
			else
				return new CalculatedFieldValue(targetPageNumber);
		}
		private string GetHyperlinkText() {
			return String.Format(CultureInfo.InvariantCulture, "{0}", targetPageNumber);
		}
		private string GetHyperlinkCode() {
			return String.Format("HYPERLINK \\l \"{0}\"", bookmarkName);
		}
	}
}
