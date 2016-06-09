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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Internal.SimpleLayout;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.ComponentModel;
#if !SL
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.LayoutEngine;
#else
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit {
	#region SimpleView
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SimpleView : RichEditView {
		#region Fields
		static readonly Padding defaultPadding = new Padding(15, 4, 4, 0);
		Padding padding = defaultPadding;
		bool hidePartiallyVisibleRow;
		bool wordWrap = true;
		bool internalWordWrap = true;
		#endregion
		public SimpleView(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SimpleViewType")]
#endif
		public override RichEditViewType Type { get { return RichEditViewType.Simple; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("SimpleViewWordWrap"),
#endif
		DefaultValue(true)]
		public bool WordWrap
		{
			get { return wordWrap; }
			set
			{
				wordWrap = value;
				InternalWordWrap = value;
			}
		}
		protected internal bool InternalWordWrap {
			get { return internalWordWrap; }
			set {
				DocumentModel.BeginUpdate();
				try {
					internalWordWrap = value;
					DocumentModelChangeActions changeActions = DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler | DocumentModelChangeActions.Redraw;
					PieceTable pieceTable = DocumentModel.ActivePieceTable;
					pieceTable.ApplyChangesCore(changeActions, RunIndex.Zero, RunIndex.MaxValue);
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		protected internal override bool ShowHorizontalRulerByDefault { get { return false; } }
		protected internal override bool ShowVerticalRulerByDefault { get { return false; } }
		protected internal override bool MatchHorizontalTableIndentsToTextEdge { get { return Control.InnerControl.Options.Layout.SimpleView.MatchHorizontalTableIndentsToTextEdge; } }
		#region Padding
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("SimpleViewPadding"),
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
		protected internal virtual bool HidePartiallyVisibleRow { get { return hidePartiallyVisibleRow; } set { hidePartiallyVisibleRow = value; } }
		protected internal override HitTestAccuracy DefaultHitTestPageAccuracy { get { return HitTestAccuracy.NearestPage; } }
		#endregion
		protected internal override DocumentFormattingController CreateDocumentFormattingController() {
			SimpleViewDocumentFormattingController result = new SimpleViewDocumentFormattingController(this, DocumentLayout, DocumentModel.MainPieceTable);
			UpdatePageWidthCore(result);
			return result;
		}
		protected internal override PageViewInfoGenerator CreatePageViewInfoGenerator() {
			return new SimpleViewPageViewInfoGenerator(this);
		}
		protected internal virtual bool ShouldUpdatePageWidth() {
			SimpleViewPageController pageController = (SimpleViewPageController)FormattingController.PageController;
			return pageController.VirtualPageWidth != Math.Max(DocumentLayout.UnitConverter.DocumentsToLayoutUnits(4), (int)(Bounds.Width / ZoomFactor));
		}
		protected internal virtual void UpdatePageWidthCore(DocumentFormattingController controller) {
			SimpleViewPageController pageController = (SimpleViewPageController)controller.PageController;
			pageController.ResetPageSize();
			if ((DocumentModel.CommentOptions.Visibility == RichEditCommentVisibility.Visible) && (DocumentModel.MainPieceTable.Comments.Count > 0)) {
				int commentWidth = DocumentLayout.UnitConverter.DocumentsToLayoutUnits(990);
				int commentOffset = DocumentLayout.UnitConverter.DocumentsToLayoutUnits(37);
				pageController.VirtualPageWidth = Math.Max(DocumentLayout.UnitConverter.DocumentsToLayoutUnits(4), (int)(Bounds.Width / ZoomFactor) - commentWidth - commentOffset);
			}
			else
			   pageController.VirtualPageWidth = Math.Max(DocumentLayout.UnitConverter.DocumentsToLayoutUnits(4), (int)(Bounds.Width / ZoomFactor) );
			if (Control.AutoSizeMode == AutoSizeMode.None || Control.AutoSizeMode == AutoSizeMode.Vertical) {
				pageController.MinPageWidth = pageController.VirtualPageWidth;
			}
			controller.Reset(false);
		}
		protected internal virtual void UpdatePageWidth() {
			DocumentModel.BeginUpdate();
			try {
				UpdatePageWidthCore(FormattingController);
				DocumentModelChangeActions changeActions = DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler;
				PieceTable pieceTable = DocumentModel.ActivePieceTable;
				pieceTable.ApplyChangesCore(changeActions, RunIndex.Zero, RunIndex.MaxValue);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal override void OnAutoSizeModeChanged() {
			DocumentModel.BeginUpdate();
			try {			  
				FormattingController.Reset(false);
				DocumentModelChangeActions changeActions = DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler;
				PieceTable pieceTable = DocumentModel.ActivePieceTable;
				pieceTable.ApplyChangesCore(changeActions, RunIndex.Zero, RunIndex.MaxValue);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal override Rectangle CalculatePageContentClipBounds(PageViewInfo page) {
			Rectangle clipBounds = base.CalculatePageContentClipBounds(page);
			clipBounds.X = Math.Min(clipBounds.X, 0);
			clipBounds.Width = (Int32.MaxValue) / 2;
			clipBounds.Height = (Int32.MaxValue) / 2;
			return clipBounds;
		}
		protected internal override void ApplyChanges(PieceTable pieceTable) {
		}
		protected internal override void OnResizeCore() {
			base.OnResizeCore();
			if (ShouldUpdatePageWidth())
				UpdatePageWidth();
		}
		protected internal override void OnResize(Rectangle bounds, bool ensureCaretVisibleonResize) {
			base.OnResize(bounds, ensureCaretVisibleonResize);
			if(ensureCaretVisibleonResize)
				EnsureCaretVisibleOnResize();
		}
		protected internal override void EnsureCaretVisibleOnResize() {
			if (Bounds.Height > 0 && Bounds.Width > 0)
				EnsureCaretVisible();
		}
		protected internal override void OnZoomFactorChangedCore() {
			base.OnZoomFactorChangedCore();
			UpdatePageWidth();
		}
		protected internal override void PerformZoomFactorChanged() {
			base.PerformZoomFactorChanged();
			EnsureCaretVisible();
		}
		protected internal override DocumentLayoutExporter CreateDocumentLayoutExporter(Painter painter, GraphicsDocumentLayoutExporterAdapter adapter, PageViewInfo pageViewInfo, Rectangle bounds) {
			ScreenOptimizedGraphicsDocumentLayoutExporter result;
			if (AllowDisplayLineNumbers)
				result = new ScreenOptimizedGraphicsDocumentLayoutExporter(DocumentModel, painter, adapter, bounds, TextColors);
			else
				result = new ScreenOptimizedGraphicsDocumentLayoutExporterNoLineNumbers(DocumentModel, painter, adapter, bounds, TextColors);
			result.VisibleBounds = CalculateVisiblePageBounds(bounds, pageViewInfo);
			result.HidePartiallyVisibleRow = this.HidePartiallyVisibleRow;
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
		protected internal override Size CalcBestSize(bool fixedWidth) {
			Control.BeginUpdate();
			bool oldInternalWordWrap = InternalWordWrap;
			long oldTopInvisibleHeight = PageViewInfoGenerator.TopInvisibleHeight;
			if (!fixedWidth)
				InternalWordWrap = false;
			BeginDocumentRendering(null);
			MaxWidthCalculator maxWidthCalculator = new MaxWidthCalculator();
			int width = maxWidthCalculator.GetMaxWidth(this.DocumentLayout.Pages[0]);
			int height = this.DocumentLayout.Pages[0].Bounds.Height;
			width = (int)Math.Ceiling(width * ZoomFactor);
			height = (int)Math.Ceiling(height * ZoomFactor);
			int count = this.DocumentLayout.Pages.Count;
			for (int i = 1; i < count; i++) {
				Page page = this.DocumentLayout.Pages[i];
				width = Math.Max(maxWidthCalculator.GetMaxWidth(page), width);
				height += page.Bounds.Height;
			}
			EndDocumentRendering();
			PageViewInfoGenerator.Reset(PageGenerationStrategyType.RunningHeight);
			PageViewInfoGenerator.TopInvisibleHeight = oldTopInvisibleHeight;
			InternalWordWrap = oldInternalWordWrap;
			Control.EndUpdate();
			return DocumentLayout.UnitConverter.LayoutUnitsToPixels(new Size(width,height), Control.InnerControl.DpiX, Control.InnerControl.DpiY);
		}
	}
	#endregion
}
