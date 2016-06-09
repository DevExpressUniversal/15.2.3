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
using DevExpress.XtraRichEdit.Internal.DraftLayout;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.ComponentModel;
#if !SL
using System.Windows.Forms;
#else
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit {
	#region DraftView
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class DraftView : PageBasedRichEditView {
		#region Fields
		static readonly Padding defaultPadding = new Padding(15, 4, 0, 0);
		Padding padding = defaultPadding;
		#endregion
		public DraftView(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DraftViewType")]
#endif
		public override RichEditViewType Type { get { return RichEditViewType.Draft; } }
		protected internal override bool ShowHorizontalRulerByDefault { get { return true; } }
		protected internal override bool ShowVerticalRulerByDefault { get { return false; } }
		protected internal override bool MatchHorizontalTableIndentsToTextEdge { get { return Control.InnerControl.Options.Layout.DraftView.MatchHorizontalTableIndentsToTextEdge; } }
		#region Padding
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DraftViewPadding"),
#endif
NotifyParentProperty(true)]
		public Padding Padding {
			get { return padding; }
			set {
				if (padding == value)
					return;
				padding = value;
				OnViewPaddingChanged();
			}
		}
		protected internal virtual bool ShouldSerializePadding() {
			return Padding != defaultPadding;
		}
		protected internal virtual void ResetPadding() {
			Padding = defaultPadding;
		}
		#endregion
		protected internal override Padding ActualPadding {
			get {
				Padding result = Padding;
				IRulerControl horizontalRuler = Control.InnerControl.HorizontalRuler;
				if (horizontalRuler != null && horizontalRuler.IsVisible) {
					int offset = horizontalRuler.GetRulerSizeInPixels();
					if (Control.InnerControl.VerticalRuler != null && Control.InnerControl.VerticalRuler.IsVisible)
						result.Left = Math.Max(result.Left - offset, offset / 3);
					else
						result.Left = Math.Max(result.Left, 4 * offset / 3);
				}
				return result;
			}
		}
		protected internal override HitTestAccuracy DefaultHitTestPageAccuracy { get { return HitTestAccuracy.NearestPage; } }
		#endregion
		protected internal override DocumentFormattingController CreateDocumentFormattingController() {
			return new DraftViewDocumentFormattingController(DocumentLayout, DocumentModel.MainPieceTable);
		}
		protected internal override PageViewInfoGenerator CreatePageViewInfoGenerator() {
			return new DraftViewPageViewInfoGenerator(this);
		}
		protected internal override DocumentLayoutExporter CreateDocumentLayoutExporter(Painter painter, GraphicsDocumentLayoutExporterAdapter adapter, PageViewInfo pageViewInfo, Rectangle bounds) {
			ScreenOptimizedGraphicsDocumentLayoutExporter result;
			if (AllowDisplayLineNumbers)
				result = new DraftScreenOptimizedGraphicsDocumentLayoutExporter(DocumentModel, painter, adapter, bounds, TextColors);
			else
				result = new DraftScreenOptimizedGraphicsDocumentLayoutExporterNoLineNumbers(DocumentModel, painter, adapter, bounds, TextColors);
			result.VisibleBounds = CalculateVisiblePageBounds(bounds, pageViewInfo);
			return result;
		}
		public override void Visit(IRichEditViewVisitor visitor) {
			visitor.Visit(this);
		}
		protected internal override PageViewInfoRow GetPageViewInfoRowFromPoint(Point point, bool strictSearch) {
			return base.GetPageViewInfoRowFromPoint(point, false);
		}
		protected internal override bool PerformStrictPageViewInfoHitTest(PageViewInfo pageViewInfo, Point pt) {
			Rectangle bounds = pageViewInfo.ClientBounds;
			return bounds.Left <= pt.X && pt.X <= bounds.Right;
		}
		protected internal override Rectangle CalculatePageContentClipBounds(PageViewInfo page) {
			Rectangle clipBounds = base.CalculatePageContentClipBounds(page);
			clipBounds.X = Math.Min(clipBounds.X, 0);
			clipBounds.Width = (Int32.MaxValue) / 2;
			clipBounds.Height = (Int32.MaxValue) / 2;
			return clipBounds;
		}
	}
	#endregion
}
