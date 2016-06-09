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
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Office.Drawing;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Internal.ReadingLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit {
	#region ReadingLayoutView
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class ReadingLayoutView : PageBasedRichEditView {
		public ReadingLayoutView(IRichEditControl control)
			: base(control) {
		}
		public override RichEditViewType Type { get { return RichEditViewType.PrintLayout; } }
		protected internal override bool ShowHorizontalRulerByDefault { get { return false; } }
		protected internal override bool ShowVerticalRulerByDefault { get { return false; } }
		protected internal override bool MatchHorizontalTableIndentsToTextEdge { get { return Control.InnerControl.Options.Layout.PrintLayoutView.MatchHorizontalTableIndentsToTextEdge; } }
		protected internal override DocumentFormattingController CreateDocumentFormattingController() {
			return new ReadingLayoutViewDocumentFormattingController(DocumentLayout, DocumentModel.MainPieceTable);
		}
		protected internal override PageViewInfoGenerator CreatePageViewInfoGenerator() {
			return new ReadingLayoutViewPageViewInfoGenerator(this);
		}
		protected internal override DocumentLayoutExporter CreateDocumentLayoutExporter(Painter painter, GraphicsDocumentLayoutExporterAdapter adapter, PageViewInfo pageViewInfo, Rectangle bounds) {
			if (AllowDisplayLineNumbers)
				return new ScreenOptimizedGraphicsDocumentLayoutExporter(DocumentModel, painter, adapter, bounds, TextColors);
			else
				return new ScreenOptimizedGraphicsDocumentLayoutExporterNoLineNumbers(DocumentModel, painter, adapter, bounds, TextColors);
		}
		public override void Visit(IRichEditViewVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
}
