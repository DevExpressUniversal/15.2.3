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
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
#else
using System.Windows;
#endif
namespace DevExpress.XtraRichEdit {
	#region PrintLayoutView
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class PrintLayoutView : PageBasedRichEditView {
		#region Fields
		int textBoxPageIndex = -1;
		#endregion
		public PrintLayoutView(IRichEditControl control)
			: base(control) {
			SetAllowDisplayLineNumbersCore(true);
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PrintLayoutViewType")]
#endif
		public override RichEditViewType Type { get { return RichEditViewType.PrintLayout; } }
		protected internal override bool ShowHorizontalRulerByDefault { get { return true; } }
		protected internal override bool ShowVerticalRulerByDefault { get { return true; } }
		protected internal override bool MatchHorizontalTableIndentsToTextEdge { get { return Control.InnerControl.Options.Layout.PrintLayoutView.MatchHorizontalTableIndentsToTextEdge; } }
		protected internal override int FixedLeftTextBorderOffset {
			get {
				IRulerControl horizontalRuler = Control.InnerControl.HorizontalRuler;
				IRulerControl verticalRuler = Control.InnerControl.VerticalRuler;
				int offset = 0;
				if (horizontalRuler != null)
					offset = Math.Max(offset, horizontalRuler.GetRulerSizeInPixels());
				if (verticalRuler != null)
					offset = Math.Max(offset, verticalRuler.GetRulerSizeInPixels());
				if (verticalRuler != null && !verticalRuler.IsVisible)
					offset *= 2;
				int logicalOffset = DocumentLayout.UnitConverter.PixelsToLayoutUnits(offset);
				return (int)Math.Round(logicalOffset / ZoomFactor);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PrintLayoutViewAllowDisplayLineNumbers"),
#endif
DefaultValue(true)]
		public override bool AllowDisplayLineNumbers { get { return base.AllowDisplayLineNumbers; } set { base.AllowDisplayLineNumbers = value; } }
		#region PageHorizontalAlignment
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PrintLayoutViewPageHorizontalAlignment"),
#endif
DefaultValue(HorizontalAlignment.Center)]
		public HorizontalAlignment PageHorizontalAlignment {
			get {
				PrintLayoutViewPageViewInfoGenerator generator = (PrintLayoutViewPageViewInfoGenerator)PageViewInfoGenerator;
				return generator.PageHorizontalAlignment;
			}
			set {
				if (PageHorizontalAlignment == value)
					return;
				PrintLayoutViewPageViewInfoGenerator generator = (PrintLayoutViewPageViewInfoGenerator)PageViewInfoGenerator;
				generator.PageHorizontalAlignment = value;
				PerformZoomFactorChanged();
			}
		}
		#endregion
		#region MaxHorizontalPageCount
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PrintLayoutViewMaxHorizontalPageCount"),
#endif
DefaultValue(0)]
		public int MaxHorizontalPageCount {
			get {
				PrintLayoutViewPageViewInfoGenerator generator = (PrintLayoutViewPageViewInfoGenerator)PageViewInfoGenerator;
				return generator.MaxHorizontalPageCount;
			}
			set {
				if (MaxHorizontalPageCount == value)
					return;
				PrintLayoutViewPageViewInfoGenerator generator = (PrintLayoutViewPageViewInfoGenerator)PageViewInfoGenerator;
				generator.MaxHorizontalPageCount = value;
				PerformZoomFactorChanged();
			}
		}
		#endregion
		#endregion
		protected internal override DocumentFormattingController CreateDocumentFormattingController() {
			return new PrintLayoutViewDocumentFormattingController(DocumentLayout, DocumentModel.MainPieceTable);
		}
		protected internal override PageViewInfoGenerator CreatePageViewInfoGenerator() {
			return new PrintLayoutViewPageViewInfoGenerator(this);
		}
		protected internal override DocumentLayoutExporter CreateDocumentLayoutExporter(Painter painter, GraphicsDocumentLayoutExporterAdapter adapter, PageViewInfo pageViewInfo, Rectangle bounds) {
			if (AllowDisplayLineNumbers)
				return new ScreenOptimizedGraphicsDocumentLayoutExporter(DocumentModel, painter, adapter, bounds, TextColors);
			else
				return new ScreenOptimizedGraphicsDocumentLayoutExporterNoLineNumbers(DocumentModel, painter, adapter, bounds, TextColors);
		}
		protected internal override void OnActivePieceTableChanged() {
			if (DocumentModel.ActivePieceTable.IsHeaderFooter) {
				int preferredPageIndex = CurrentPageIndex;
				if (preferredPageIndex < 0)
					preferredPageIndex = textBoxPageIndex > 0 ? textBoxPageIndex : FormattingController.PageController.Pages.Count - 1;
				Debug.Assert(preferredPageIndex >= 0);
				SelectionLayout = new HeaderFooterSelectionLayout(this, preferredPageIndex);
				CaretPosition = new HeaderFooterCaretPosition(this, preferredPageIndex);
			}
			else
				base.OnActivePieceTableChanged();
			textBoxPageIndex = (DocumentModel.ActivePieceTable.IsTextBox ) ? CurrentPageIndex : -1;
		}
		public override void Visit(IRichEditViewVisitor visitor) {
			visitor.Visit(this);
		}
		protected internal override Rectangle ExpandClipBoundsToPaddings(Rectangle clipBounds) {
			return clipBounds;
		}
		public void FitToPage() {
			new FitToPageCommand(Control).Execute();
		}
		protected internal override void UpdateHorizontalScrollbar() {
			base.UpdateHorizontalScrollbar();
			long leftInvisibleWidth = PageViewInfoGenerator.LeftInvisibleWidth;
			if (!HorizontalScrollController.ScrollBarAdapter.Enabled && leftInvisibleWidth > 0) {
				HorizontalScrollController.ScrollByLeftInvisibleWidthDelta(-leftInvisibleWidth);
				OnHorizontalScroll();
			}
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Internal.PrintLayout {
	#region FirstPageAnchor
	public abstract class FirstPageAnchor {
	}
	#endregion
	#region RunningHeightFirstPageAnchor
	public class RunningHeightFirstPageAnchor : FirstPageAnchor {
		long topInvisibleHeight;
		public long TopInvisibleHeight { get { return topInvisibleHeight; } set { topInvisibleHeight = value; } }
	}
	#endregion
	#region FirstPageOffsetFirstPageAnchor
	public class FirstPageOffsetFirstPageAnchor : FirstPageAnchor {
		int pageIndex;
		int verticalOffset;
		public int PageIndex { get { return pageIndex; } set { pageIndex = value; } }
		public int VerticalOffset { get { return verticalOffset; } set { verticalOffset = value; } }
	}
	#endregion
	#region PageViewInfo
	public class PageViewInfo {
		#region Fields
		Rectangle bounds;
		Rectangle clientBounds;
		Rectangle commentsBounds;
		int index;
		readonly Page page;
		#endregion
		public PageViewInfo(Page page) {
			Guard.ArgumentNotNull(page, "page");
			this.page = page;
		}
		#region Properties
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle ClientBounds { get { return clientBounds; } set { clientBounds = value; } }
		public Rectangle CommentsBounds { get { return commentsBounds; } set { commentsBounds = value; } }
		public Rectangle ClientCommentsBounds { get; set; }
		public int Index { get { return index; } set { index = value; } }
		public Page Page { get { return page; } }
		#endregion
#if DEBUG
		public override string ToString() {
			return String.Format("PageViewInfo. Bounds: {0}, ClientBounds: {1}, Page: {2}", Bounds, ClientBounds, Page.ToString());
		}
#endif
	}
	#endregion
	#region PageViewInfoCollection
	public class PageViewInfoCollection : DXCollectionWithSetItem<PageViewInfo> {
		public PageViewInfoCollection() {
			this.UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
		}
		#region Properties
		#region First
		public PageViewInfo First {
			get {
				if (Count <= 0)
					return null;
				else
					return this[0];
			}
		}
		#endregion
		#region Last
		public PageViewInfo Last {
			get {
				if (Count <= 0)
					return null;
				else
					return this[Count - 1];
			}
		}
		#endregion
		#endregion
		public void ExportTo(IDocumentLayoutExporter exporter) {
			int count = Count;
			for (int i = 0; i < count; i++)
				this[i].Page.ExportTo(exporter);
		}
	}
	#endregion
	#region RichEditViewPageViewInfoCollection
	public class RichEditViewPageViewInfoCollection : PageViewInfoCollection {
		readonly DocumentLayout documentLayout;
		public RichEditViewPageViewInfoCollection(DocumentLayout documentLayout) {
			Guard.ArgumentNotNull(documentLayout, "documentLayout");
			this.documentLayout = documentLayout;
		}
		public DocumentLayout DocumentLayout { get { return documentLayout; } }
		protected internal virtual void UpdateDocumentLayoutVisiblePages() {
			DocumentLayout.FirstVisiblePageIndex = First.Index;
			DocumentLayout.LastVisiblePageIndex = Last.Index;
		}
		protected internal virtual void ResetDocumentLayoutVisiblePages() {
			DocumentLayout.FirstVisiblePageIndex = -1;
			DocumentLayout.LastVisiblePageIndex = -1;
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			ResetDocumentLayoutVisiblePages();
		}
		protected override void OnInsertComplete(int index, PageViewInfo value) {
			base.OnInsertComplete(index, value);
			UpdateDocumentLayoutVisiblePages();
		}
		protected override void OnRemoveComplete(int index, PageViewInfo value) {
			base.OnRemoveComplete(index, value);
			UpdateDocumentLayoutVisiblePages();
		}
		protected override void OnSetComplete(int index, PageViewInfo oldValue, PageViewInfo newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			UpdateDocumentLayoutVisiblePages();
		}
	}
	#endregion
}
