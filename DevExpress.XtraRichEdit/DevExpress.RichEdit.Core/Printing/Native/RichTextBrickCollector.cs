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
using DevExpress.XtraPrinting.Native.RichText;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Layout;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraRichEdit.Printing;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraPrinting.Native {
	public class BreaksFreeDocumentPrinter : GraphicsDocumentPrinter {
		public BreaksFreeDocumentPrinter(DocumentModel documentModel)
			: base(documentModel) {
		}
		protected internal override DocumentFormattingController CreateDocumentFormattingController(DocumentLayout documentLayout) {
			DocumentFormattingController controller = base.CreateDocumentFormattingController(documentLayout);
			controller.RowsController.SupportsColumnAndPageBreaks = false;
			return controller;
		}
	}
	class DocumentModelBrickCollector {
		#region inner classes
		class CustomExporter : PrintingDocumentExporter {
			List<Brick> bricks;
			public CustomExporter(DocumentModel documentModel, List<Brick> bricks)
				: base(documentModel, TextColors.Defaults) {
				this.bricks = bricks;
			}
			protected internal override void AddBrickToCurrentContainer(Brick brick) {
				if(brick is UnderlineBrick)
					return;
				bricks.Add(brick);
				System.Diagnostics.Debug.Assert(brick.PrintingSystem != null);
			}
			protected override Font GetFont(Box box) {
				Font font = base.GetFont(box);
				Nullable<FontStyle> fontStyle = GetFontStyle(box.GetRun(this.PieceTable));
				return fontStyle != null ? CreateFont(font, fontStyle.Value) : font;
			}
			static Font CreateFont(Font prototype, FontStyle fontStyle) {
				return new Font(prototype, fontStyle);
			}
			static Nullable<FontStyle> GetFontStyle(TextRunBase textRun) {
				FontStyle fontStyle = 0;
				if(textRun.FontUnderlineType != UnderlineType.None)
					fontStyle |= FontStyle.Underline;
				if(textRun.FontStrikeoutType != StrikeoutType.None)
					fontStyle |= FontStyle.Strikeout;
				if(textRun.FontBold)
					fontStyle |= FontStyle.Bold;
				if(textRun.FontItalic)
					fontStyle |= FontStyle.Italic;
				return fontStyle != 0 ? new Nullable<FontStyle>(fontStyle) : null;
			}
		}
		#endregion
		DocumentModel documentModel;
		GraphicsDocumentPrinter printer;
		public DocumentModelBrickCollector(DocumentModel documentModel, GraphicsDocumentPrinter printer) {
			this.documentModel = documentModel;
			this.printer = printer;
		}
		public void Collect(PrintingSystemBase ps, List<Brick> bricks) {
			ps.Graph.DefaultBrickStyle.BorderWidth = 0;
			ps.Graph.DefaultBrickStyle.Sides = BorderSide.None;
			PrintingDocumentExporter exporter = new CustomExporter(documentModel, bricks);
			exporter.PrintingSystem = ps;
			IPageFloatingObjectExporter floatingObjectExporter = exporter as IPageFloatingObjectExporter;
			if (floatingObjectExporter != null && printer.DocumentLayout.Pages.Count > 0) {
				floatingObjectExporter.ExportPageAreaCore(printer.DocumentLayout.Pages[0], printer.DocumentLayout.DocumentModel.MainPieceTable, page => printer.Columns.ExportTo(exporter));
			}
			else
				printer.Columns.ExportTo(exporter);
		}
	}
}
