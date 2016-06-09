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
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Windows.Forms;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using System.Diagnostics;
#else
using System.Windows.Media;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using System.Collections.ObjectModel;
#endif
namespace DevExpress.XtraRichEdit {
	#region RichEditViewType
	[ComVisible(true)]
	public enum RichEditViewType {
		Simple,
		Draft,
		PrintLayout,
	}
	#endregion
	public class PageLayoutPosition {
		int pageIndex;
		Point position;
		public PageLayoutPosition(int pageIndex, Point position) {
			this.pageIndex = pageIndex;
			this.position = position;
		}
		public int PageIndex { get { return pageIndex; } }
		public Point Position { get { return position; } }
	}
	public struct PageLayoutInfo {
		int pageIndex;
		Rectangle bounds;
		internal PageLayoutInfo(int pageIndex, Rectangle bounds) {
			this.pageIndex = pageIndex;
			this.bounds = bounds;
		}
		public int PageIndex { get { return pageIndex; } }
		public Rectangle Bounds { get { return bounds; } }
	}
	#region RichEditView (abstract class)
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	[ComVisible(true)]
	public abstract partial class RichEditView : IDisposable {
		#region Fields
		Color backColor = DXSystemColors.Window;
		bool isDisposed;
		IRichEditControl control;
		Rectangle bounds;
		const float minZoomFactor = 0.0001f;
		const float maxZoomFactor = 10000f;
		const float defaultZoomFactor = 1.0f;
		const int minWidth = 5;
		const int minHeight = 5;
		float zoomFactor;
		bool allowDisplayLineNumbers;
		bool adjustColorsToSkins;
		DocumentLayout documentLayout;
		readonly DocumentFormattingController formattingController;
		readonly RichEditViewVerticalScrollController verticalScrollController;
		readonly RichEditViewHorizontalScrollController horizontalScrollController;
		readonly PageViewInfoCollection pageViewInfos;
		readonly PageViewInfoGenerator pageViewInfoGenerator;
		SelectionLayout selectionLayout;
		IHoverLayoutItem hoverLayout;
		CaretPosition caretPosition;
		TableViewInfoController tableController;
		#endregion
		protected RichEditView(IRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.zoomFactor = 1;
			this.Bounds = new Rectangle(0, 0, MinWidth, MinHeight);
			this.documentLayout = CreateDocumentLayout(DocumentModel, control.InnerControl);
			this.formattingController = CreateDocumentFormattingController();
			this.verticalScrollController = control.CreateRichEditViewVerticalScrollController(this); 
			this.horizontalScrollController = control.CreateRichEditViewHorizontalScrollController(this); 
			this.pageViewInfos = new RichEditViewPageViewInfoCollection(DocumentLayout);
			this.pageViewInfoGenerator = CreatePageViewInfoGenerator();
			this.selectionLayout = new SelectionLayout(this, 0);
			this.caretPosition = new CaretPosition(this, 0);
			this.formattingController.ResetSecondaryFormattingForPage += OnResetSecondaryFormattingForPage;
			this.DocumentModel.LayoutOptions.Changed += LayoutOptions_Changed;
		}
		#region Properties
		#region Control
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public IRichEditControl Control { get { return control; } }
		#endregion
		#region Type
		[Browsable(false)]
		public abstract RichEditViewType Type { get; }
		#endregion
		#region Bounds
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		protected internal Rectangle Bounds {
			get { return bounds; }
			set {
				if (bounds == value)
					return;
				if (value.Width < MinWidth)
					value.Width = MinWidth;
				if (value.Height < MinHeight)
					value.Height = MinHeight;
				bounds = value;
			}
		}
		#endregion
		#region ZoomFactor
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditViewZoomFactor"),
#endif
DefaultValue(defaultZoomFactor)]
		public float ZoomFactor {
			get { return zoomFactor; }
			set {
				if (value < minZoomFactor)
					value = minZoomFactor;
				if (value > maxZoomFactor)
					value = maxZoomFactor;
				if (zoomFactor == value)
					return;
				OnZoomFactorChanging();
				float oldValue = zoomFactor;
				zoomFactor = value;
				OnZoomFactorChanged(oldValue, zoomFactor);
			}
		}
		#endregion
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditViewAdjustColorsToSkins"),
#endif
		DefaultValue(false)]
		public virtual bool AdjustColorsToSkins {
			get {
				return adjustColorsToSkins;
			}
			set {
				if (adjustColorsToSkins == value)
					return;
				adjustColorsToSkins = value;
				Control.InnerControl.Owner.Redraw(RefreshAction.AllDocument);
			}
		}
		protected internal virtual TextColors TextColors { get { return adjustColorsToSkins ? Control.SkinTextColors : TextColors.Defaults; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditViewAllowDisplayLineNumbers"),
#endif
DefaultValue(false)]
		public virtual bool AllowDisplayLineNumbers {
			get { return allowDisplayLineNumbers; }
			set {
				if (allowDisplayLineNumbers == value)
					return;
				SetAllowDisplayLineNumbersCore(value);
				Control.InnerControl.Owner.Redraw(RefreshAction.AllDocument);
			}
		}
		protected internal void SetAllowDisplayLineNumbersCore(bool value) {
			allowDisplayLineNumbers = value;
		}
		protected internal virtual Padding ActualPadding { get { return Padding.Empty; } }
		protected internal virtual Color ActualBackColor {
			get {
				if (AdjustColorsToSkins)
					return Control.SkinTextColors.DefaultBackgroundColor;
				return BackColor;
			}
		}
		#region BackColor
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditViewBackColor")]
#endif
		public Color BackColor {
			get { return backColor; }
			set {
				if (backColor == value)
					return;
				backColor = value;
				RaiseBackColorChanged();
			}
		}
		protected internal virtual bool ShouldSerializeBackColor() {
			return BackColor != DXSystemColors.Window;
		}
		protected internal virtual void ResetBackColor() {
			BackColor = DXSystemColors.Window;
		}
		#endregion
		protected internal virtual int FixedLeftTextBorderOffset { get { return 0; } }
		internal bool IsDisposed { get { return isDisposed; } }
		protected internal DocumentModel DocumentModel { get { return control.InnerControl.DocumentModel; } }
		[Browsable(false)]
		protected internal DocumentLayout DocumentLayout { get { return documentLayout; } }
		protected internal DocumentFormattingController FormattingController { get { return formattingController; } }
		protected internal RichEditViewVerticalScrollController VerticalScrollController { get { return verticalScrollController; } }
		protected internal RichEditViewHorizontalScrollController HorizontalScrollController { get { return horizontalScrollController; } }
		protected internal PageViewInfoCollection PageViewInfos { get { return pageViewInfos; } }
		protected internal PageViewInfoGenerator PageViewInfoGenerator { get { return pageViewInfoGenerator; } }
		protected internal BackgroundFormatter Formatter { get { return control.InnerControl.Formatter; } }
		protected internal virtual int MinWidth { get { return minWidth; } }
		protected internal virtual int MinHeight { get { return minHeight; } }
		protected internal CaretPosition CaretPosition { get { return caretPosition; } set { caretPosition = value; } }
		protected internal SelectionLayout SelectionLayout { get { return selectionLayout; } set { selectionLayout = value; } }
		protected internal IHoverLayoutItem HoverLayout { get { return hoverLayout; } set { hoverLayout = value; } }
		protected internal TableViewInfoController TableController { get { return tableController; } set { tableController = value; } }
		protected internal abstract bool ShowHorizontalRulerByDefault { get; }
		protected internal abstract bool ShowVerticalRulerByDefault { get; }
#if DEBUGTEST
		[Browsable(false)]
		public bool FormattingSuspended { get { return formattingSuspended; } }
#endif
		protected internal virtual HitTestAccuracy DefaultHitTestPageAccuracy { get { return HitTestAccuracy.ExactPage; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (Formatter != null && !Formatter.IsDisposed) {
					SuspendFormatting();
					try {
						UnsubscribePageFormattingComplete();
					}
					finally {
						ResumeFormatting();
					}
				}
				if (documentLayout != null) {
					UnsubscribeDocumentLayoutEvents();
					documentLayout = null;
				}
				if (DocumentModel != null && DocumentModel.LayoutOptions != null) {
					DocumentModel.LayoutOptions.Changed -= LayoutOptions_Changed;
				}
				if (verticalScrollController != null) {
					verticalScrollController.Deactivate();
				}
				if (horizontalScrollController != null) {
					horizontalScrollController.Deactivate();
				}
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		#region Events
		#region ZoomChanging
		EventHandler onZoomChanging;
		internal event EventHandler ZoomChanging { add { onZoomChanging += value; } remove { onZoomChanging -= value; } }
		protected internal virtual void RaiseZoomChanging() {
			if (onZoomChanging != null)
				onZoomChanging(this, EventArgs.Empty);
		}
		#endregion
		#region ZoomChanged
		EventHandler onZoomChanged;
		internal event EventHandler ZoomChanged { add { onZoomChanged += value; } remove { onZoomChanged -= value; } }
		protected internal virtual void RaiseZoomChanged() {
			if (onZoomChanged != null)
				onZoomChanged(this, EventArgs.Empty);
		}
		#endregion
		#region BackColorChanged
		EventHandler onBackColorChanged;
		protected internal event EventHandler BackColorChanged { add { onBackColorChanged += value; } remove { onBackColorChanged -= value; } }
		protected internal virtual void RaiseBackColorChanged() {
			if (onBackColorChanged != null)
				onBackColorChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal virtual DocumentLayout CreateDocumentLayout(DocumentModel documentModel, IBoxMeasurerProvider measurerProvider) {
			return new DocumentLayout(documentModel, measurerProvider);
		}
		void LayoutOptions_Changed(object sender, DevExpress.Utils.Controls.BaseOptionChangedEventArgs e) {
			if (e.Name == ViewLayoutOptionsBase.MatchHorizontalTableIndentsToTextEdgePropertyName) {
				DocumentModel.BeginUpdate();
				DocumentModel.OnMatchHorizontalTableIndentsToTextEdgeOptionsChanged();
				formattingController.RowsController.MatchHorizontalTableIndentsToTextEdge = (bool)e.NewValue;
				DocumentModel.EndUpdate();
			}
		}
		protected internal abstract DocumentFormattingController CreateDocumentFormattingController();
		protected internal abstract PageViewInfoGenerator CreatePageViewInfoGenerator();
		protected internal BoxHitTestCalculator CreateHitTestCalculator(RichEditHitTestRequest request, RichEditHitTestResult result) {
			return this.FormattingController.PageController.CreateHitTestCalculator(request, result);
		}
		protected internal virtual void Deactivate() {
			UnsubscribePageFormattingComplete();
			UnsubscribeDocumentLayoutEvents();
			VerticalScrollController.Deactivate();
			HorizontalScrollController.Deactivate();
		}
		protected internal virtual void Activate(Rectangle viewBounds) {
			Debug.Assert(viewBounds.Location == Point.Empty);
			CaretPosition.Invalidate();
			CaretPosition.InvalidateInputPosition();
			SelectionLayout.Invalidate();
			ResetPages(PageGenerationStrategyType.RunningHeight);
			FormattingController.Reset(false);
			VerticalScrollController.Activate();
			HorizontalScrollController.Activate();
			SubscribeDocumentLayoutEvents();
			SubscribePageFormattingComplete();
			PerformResize(viewBounds);
			PieceTable pieceTable = DocumentModel.MainPieceTable;
			DocumentModelPosition from = new DocumentModelPosition(pieceTable); 
			DocumentModelPosition to = DocumentModelPosition.FromParagraphEnd(pieceTable, pieceTable.Paragraphs.Last.Index);
			Formatter.UpdateSecondaryPositions(from, to);
			ApplyChanges(pieceTable);
			Formatter.NotifyDocumentChanged(from, to, DocumentLayoutResetType.AllPrimaryLayout);
		}
		protected internal virtual void ApplyChanges(PieceTable pieceTable) {
			DocumentModel.BeginUpdate();
			try {
				DocumentModelChangeActions changeActions = DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler;
				pieceTable.ApplyChangesCore(changeActions, RunIndex.Zero, RunIndex.MaxValue);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void OnViewPaddingChanged() {
			Control.OnViewPaddingChanged();
		}
		protected internal virtual void OnZoomFactorChanging() {
			RaiseZoomChanging();
		}
		protected internal virtual void OnZoomFactorChanged(float oldValue, float newValue) {
			PerformZoomFactorChanged();
			RaiseZoomChanged();
			Control.RedrawEnsureSecondaryFormattingComplete(RefreshAction.Zoom);
			Control.InnerControl.RaiseUpdateUI();
		}
		protected internal virtual void PerformZoomFactorChanged() {
			SuspendFormatting();
			try {
				ResetPages(PageGenerationStrategyType.FirstPageOffset);
				OnZoomFactorChangedCore();
				GeneratePages();
			}
			finally {
				ResumeFormatting();
			}
			Control.InnerControl.BeginDocumentRendering();
			Control.InnerControl.EndDocumentRendering();
			SetOptimalHorizontalScrollbarPosition();
		}
		protected internal virtual void OnZoomFactorChangedCore() {
		}
		protected internal virtual void CorrectZoomFactor() {
			RichEditBehaviorOptions options = Control.InnerControl.Options.Behavior;
			float zoom = Math.Max(defaultZoomFactor, options.MinZoomFactor);
			if (options.MaxZoomFactor == options.DefaultMaxZoomFactor)
				this.zoomFactor = zoom;
			else
				this.zoomFactor = Math.Min(zoom, options.MaxZoomFactor);
		}
		protected internal virtual void OnSelectionChanged() {
			HideCaret();
			CaretPosition.Invalidate();
			CaretPosition.InvalidateInputPosition();
			SelectionLayout.Invalidate();
			ShowCaret();
		}
		protected internal virtual void ShowCaret() {
			Control.ShowCaret();
		}
		protected internal virtual void HideCaret() {
			Control.HideCaret();
		}
		protected internal virtual void OnBeginDocumentUpdate() {
			SuspendFormatting();
		}
		protected internal virtual void ValidateSelectionInterval() {
			Selection selection = DocumentModel.Selection;
			DocumentLogPosition start = selection.Start;
			DocumentLogPosition end = selection.End;
			if (selection.Length > 0) {
				DocumentLogPosition visibleStart = GetVisibleSelectionStart();
				DocumentLogPosition visibleEnd = GetVisibleSelectionEnd();
				bool nonEmptyVisibleSelection = (start < end && visibleStart < visibleEnd) || (start > end && visibleStart > visibleEnd);
				if (!nonEmptyVisibleSelection)
					visibleEnd = visibleStart;
				DocumentModel.Selection.Start = visibleStart;
				DocumentModel.Selection.End = visibleEnd;
			}
			else {
				DocumentLogPosition newPosition = GetVisibleSelectionPosition();
				DocumentModel.Selection.Start = newPosition;
				DocumentModel.Selection.End = newPosition;
			}
		}
		protected virtual DocumentLogPosition GetVisibleSelectionPosition() {
			Selection selection = DocumentModel.Selection;
			DocumentModelPosition position = selection.Interval.Start;
			RunIndex runIndex = position.RunIndex;
			IVisibleTextFilter visibleTextFilter = selection.PieceTable.VisibleTextFilter;
			bool startRunVisible = visibleTextFilter.IsRunVisible(runIndex);
			bool positionVisible = position.RunOffset == 0 && runIndex > RunIndex.Zero && visibleTextFilter.IsRunVisible(runIndex - 1);
			if (startRunVisible || positionVisible)
				return position.LogPosition;
			return visibleTextFilter.GetNextVisibleLogPosition(position, false);
		}
		protected virtual DocumentLogPosition GetVisibleSelectionStart() {
			Selection selection = DocumentModel.Selection;
			DocumentLogPosition start = selection.Start;
			bool startVisible = selection.PieceTable.VisibleTextFilter.IsRunVisible(selection.Interval.Start.RunIndex);
			if (startVisible)
				return start;
			else
				return selection.PieceTable.VisibleTextFilter.GetNextVisibleLogPosition(start, false);
		}
		protected virtual DocumentLogPosition GetVisibleSelectionEnd() {
			Selection selection = DocumentModel.Selection;
			DocumentLogPosition end = selection.End;
			DocumentLogPosition lastSelectedSymbol = selection.Length > 0 ? Algorithms.Max(end - 1, selection.PieceTable.DocumentStartLogPosition) : selection.Start;
			RunIndex endRunIndex = selection.Interval.End.RunIndex;
			if (selection.Interval.End.RunOffset == 0 && endRunIndex > RunIndex.Zero) {
				endRunIndex--;
			}
			bool endVisible = selection.PieceTable.VisibleTextFilter.IsRunVisible(endRunIndex);
			if (endVisible)
				return end;
			else
				return selection.PieceTable.VisibleTextFilter.GetNextVisibleLogPosition(lastSelectedSymbol, false);
		}
		protected virtual void OnResetSecondaryFormattingForPage(object sender, ResetSecondaryFormattingForPageArgs e) {
			Formatter.ResetSecondaryFormattingForPage(e.Page, e.PageIndex);
		}
		protected internal virtual void OnEndDocumentUpdate() {
			ResumeFormatting();
		}
		protected internal virtual void OnActivePieceTableChanged() {
			if (DocumentModel.ActivePieceTable.IsTextBox) {
				SelectionLayout = new TextBoxSelectionLayout(this, 0);
				CaretPosition = new TextBoxCaretPosition(this, 0);
			}
			else {
				if (DocumentModel.ActivePieceTable.IsComment) {
					SelectionLayout = new CommentSelectionLayout(this, 0, DocumentModel.ActivePieceTable);
					CaretPosition = new CommentCaretPosition(this, 0, DocumentModel.ActivePieceTable);
				}
				else {
					SelectionLayout = new SelectionLayout(this, 0);
					CaretPosition = new CaretPosition(this, 0);
				}
			}
		}
		bool IsPrimaryFormattingCompleteForVisibleHeight2(DocumentModelPosition currentFormatterPosition) {
			return  PageViewInfoGenerationComplete;
		}
		protected internal virtual void BeginDocumentRendering() {
			BeginDocumentRendering(IsPrimaryFormattingCompleteForVisibleHeight2);
		}
		protected internal virtual void BeginDocumentRendering(Predicate<DocumentModelPosition> performPrimaryLayoutUntil) {
			if (Control.UseSkinMargins) {
				PageViewInfoGenerator.HorizontalPageGap = (int)Math.Ceiling((Control.SkinLeftMargin + Control.SkinRightMargin) / ZoomFactor);
				PageViewInfoGenerator.VerticalPageGap = (int)Math.Ceiling((Control.SkinTopMargin + Control.SkinBottomMargin) / ZoomFactor);
			}
			Formatter.BeginDocumentRendering(performPrimaryLayoutUntil);
			if (this.PageViewInfos.Count <= 0) {
				ResetPages(PageGenerationStrategyType.FirstPageOffset);
				Page lastPage = DocumentLayout.Pages.Last;
				PageViewInfoGenerator.FirstPageOffsetAnchor.PageIndex = DocumentLayout.Pages.Count - 1;
				Row lastRow = lastPage.Areas.Last.Columns.Last.Rows.Last;
				pageViewInfoGenerator.FirstPageOffsetAnchor.VerticalOffset = lastRow.Bounds.Top;
				GeneratePages();
			}
			int count = PageViewInfos.Count;
			for (int i = 0; i < count; i++) {
				Page page = PageViewInfos[i].Page;
				Debug.Assert(page.PrimaryFormattingComplete);
				if (!page.SecondaryFormattingComplete)
					Formatter.PerformPageSecondaryFormatting(page);
				Debug.Assert(page.SecondaryFormattingComplete);
			}
			Debug.Assert(PageViewInfos.Count > 0);
		}
		protected internal virtual void EndDocumentRendering() {
			Formatter.EndDocumentRendering();
		}
		bool pageViewInfoGenerationIsAboutToComplete;
		bool PageViewInfoGenerationIsAboutToComplete { get { return pageViewInfoGenerationIsAboutToComplete; } set { pageViewInfoGenerationIsAboutToComplete = value; } }
		bool pageViewInfoGenerationComplete;
		bool PageViewInfoGenerationComplete {
			get { return pageViewInfoGenerationComplete; }
			set {
#if DEBUGTEST
				if (!pageViewInfoGenerationComplete && value) {
					int count = PageViewInfos.Count;
					for (int i = 0; i < count; i++) {
						PageViewInfo page = PageViewInfos[i];
						Debug.Assert(page.Page.PrimaryFormattingComplete);
					}
				}
#endif
				pageViewInfoGenerationComplete = value;
			}
		}
		protected internal virtual void OnPageFormattingStarted(object sender, PageFormattingCompleteEventArgs e) {
			Page lastPage = e.Page;
			Debug.Assert(Object.ReferenceEquals(lastPage, FormattingController.PageController.Pages.Last));
			if (!PageViewInfoGenerationIsAboutToComplete)
				PageViewInfoGenerationIsAboutToComplete = (PageViewInfoGenerator.PreProcessPage(lastPage, FormattingController.PageController.Pages.Count - 1) == ProcessPageResult.VisiblePagesGenerationComplete);
		}
		protected internal virtual void OnPageFormattingComplete(object sender, PageFormattingCompleteEventArgs e) {
			Page lastPage = e.Page;
			Debug.Assert(Object.ReferenceEquals(lastPage, FormattingController.PageController.Pages.Last));
			Debug.Assert(lastPage.PrimaryFormattingComplete);
			PageViewInfo lastPageViewInfo = this.PageViewInfos.Last;
			if (lastPageViewInfo != null && lastPageViewInfo.Page == lastPage) {
				if (e.DocumentFormattingComplete)
					Control.UpdateUIFromBackgroundThread(VerticalScrollController.ScrollBarAdapter.EnsureSynchronized);
				return;
			}
			bool prevPageViewInfoGenerationComplete = PageViewInfoGenerationComplete;
			PageViewInfoGenerationComplete = (PageViewInfoGenerator.ProcessPage(lastPage, FormattingController.PageController.Pages.Count - 1) == ProcessPageResult.VisiblePagesGenerationComplete);
			if (PageViewInfoGenerationComplete != prevPageViewInfoGenerationComplete || e.DocumentFormattingComplete) {
				PageViewInfoGenerator.CalculateWidthParameters();
				Control.UpdateUIFromBackgroundThread(UpdateHorizontalScrollbar);
			}
			if (PageViewInfoGenerationComplete) {
				Formatter.UpdateSecondaryPositions(Formatter.SecondaryLayoutStart, lastPage.GetLastPosition(DocumentModel.MainPieceTable));
				Formatter.ResetSecondaryLayout();
			}
			Control.UpdateUIFromBackgroundThread(VerticalScrollController.UpdateScrollBar);
			if (e.DocumentFormattingComplete)
				Control.UpdateUIFromBackgroundThread(VerticalScrollController.ScrollBarAdapter.EnsureSynchronized);
		}
		protected internal virtual void SetOptimalHorizontalScrollbarPosition() {
			DocumentModel.BeginUpdate();
			try {
				SetOptimalHorizontalScrollbarPositionCore();
				if (DocumentModel.IsUpdateLocked)
					DocumentModel.MainPieceTable.ApplyChangesCore(DocumentModelChangeActions.Redraw, RunIndex.DontCare, RunIndex.DontCare);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void SetOptimalHorizontalScrollbarPositionCore() {
			if (PageViewInfoGenerator.VisibleWidth >= PageViewInfoGenerator.TotalWidth)
				PageViewInfoGenerator.LeftInvisibleWidth = 0;
			else {
				PageViewInfo pageViewInfo = ChoosePageViewInfoForOptimalHorizontalScrollbarPosition();
				if (pageViewInfo == null)
					PageViewInfoGenerator.LeftInvisibleWidth = 0;
				else
					PageViewInfoGenerator.LeftInvisibleWidth = Math.Max(0, CalculateOptimalHorizontalScrollbarPosition(pageViewInfo));
			}
			HorizontalScrollController.UpdateScrollBar(); 
		}
		protected internal virtual long CalculateOptimalHorizontalScrollbarPosition(PageViewInfo pageViewInfo) {
			long logicalVisibleWidth = (long)Math.Round(this.Bounds.Width / ZoomFactor);
			Rectangle pageClientBounds = pageViewInfo.Page.ClientBounds;
			int halfOfHorizontalPageGap = PageViewInfoGenerator.HorizontalPageGap / 2;
			if (DocumentModel.ActivePieceTable.IsComment)
				return pageViewInfo.Page.CommentBounds.Right - logicalVisibleWidth - FixedLeftTextBorderOffset;
			if (logicalVisibleWidth > pageViewInfo.Page.Bounds.Width + PageViewInfoGenerator.HorizontalPageGap)
				return 0;
			else if (logicalVisibleWidth > pageViewInfo.Page.Bounds.Width)
				return (pageViewInfo.Page.Bounds.Width + PageViewInfoGenerator.HorizontalPageGap - logicalVisibleWidth) / 2 - FixedLeftTextBorderOffset;
			else if (logicalVisibleWidth > pageViewInfo.Page.Bounds.Right - pageClientBounds.Left + halfOfHorizontalPageGap)
				return pageViewInfo.Page.Bounds.Width + PageViewInfoGenerator.HorizontalPageGap - logicalVisibleWidth - FixedLeftTextBorderOffset;
			else
				return halfOfHorizontalPageGap + pageClientBounds.Left - FixedLeftTextBorderOffset;
		}
		protected internal virtual PageViewInfo ChoosePageViewInfoForOptimalHorizontalScrollbarPosition() {
			PageViewInfoRowCollection pageViewInfoRows = PageViewInfoGenerator.ActiveGenerator.PageRows;
			PageViewInfoRow row = pageViewInfoRows[0];
			if (row.Count <= 0)
				return null;
			PageViewInfo result = row[0];
			float x = result.Page.ClientBounds.Left * ZoomFactor + result.Bounds.Left;
			int count = pageViewInfoRows.Count;
			for (int i = 0; i < count; i++) {
				PageViewInfo pageViewInfo = pageViewInfoRows[i][0];
				float offset = pageViewInfo.Page.ClientBounds.Left * ZoomFactor + pageViewInfo.Bounds.Left;
				if (offset > x) {
					x = offset;
					result = pageViewInfo;
				}
			}
			return result;
		}
		[System.Diagnostics.Conditional("DEBUG")]
		protected internal void CheckExecutedAtUIThread() {
#if DEBUG
#if(!SL)
			Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == Control.InnerControl.ThreadId);
#endif
#endif
		}
		[System.Diagnostics.Conditional("DEBUG")]
		protected internal void CheckExecutedAtBackgroundThread() {
#if DEBUG
#if(!SL)
			Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId != Control.InnerControl.ThreadId);
#endif
#endif
		}
		protected internal void EnforceFormattingCompleteForVisibleArea() {
			if (Control.InnerControl.DocumentModel.IsUpdateLocked)
				return;
			BeginDocumentRendering();
			EndDocumentRendering();
		}
		protected internal virtual void SubscribeDocumentLayoutEvents() {
			this.DocumentLayout.BeforeCreateDetailRow += new EventHandler(OnBeforeCreateDetailRow);
			this.DocumentLayout.AfterCreateDetailRow += new EventHandler(OnAfterCreateDetailRow);
		}
		protected internal virtual void UnsubscribeDocumentLayoutEvents() {
			this.DocumentLayout.BeforeCreateDetailRow -= new EventHandler(OnBeforeCreateDetailRow);
			this.DocumentLayout.AfterCreateDetailRow -= new EventHandler(OnAfterCreateDetailRow);
		}
		protected internal virtual void OnBeforeCreateDetailRow(object sender, EventArgs e) {
			SuspendFormatting();
		}
		protected internal virtual void OnAfterCreateDetailRow(object sender, EventArgs e) {
			ResumeFormatting();
		}
		protected internal virtual void ScrollToPage(int pageIndex) {
			SuspendFormatting();
			try {
				ResetPages(PageGenerationStrategyType.FirstPageOffset);
				PageViewInfoGenerator.FirstPageOffsetAnchor.PageIndex = pageIndex;
				PageViewInfoGenerator.FirstPageOffsetAnchor.VerticalOffset = 0;
				GeneratePages();
				ResetPages(PageGenerationStrategyType.RunningHeight);
				GeneratePages();
			}
			finally {
				ResumeFormatting();
			}
			VerticalScrollController.UpdateScrollBar();
			Control.RedrawEnsureSecondaryFormattingComplete(RefreshAction.Transforms);
		}
		protected internal virtual void OnVerticalScroll() {
			SuspendFormatting();
			try {
				ResetPages(PageGenerationStrategyType.RunningHeight);
				PageViewInfoGenerator.TopInvisibleHeight = VerticalScrollController.GetTopInvisibleHeight();
				GeneratePages();
			}
			finally {
				ResumeFormatting();
			}
			Control.RedrawEnsureSecondaryFormattingComplete(RefreshAction.Transforms); 
		}
		protected internal virtual void OnHorizontalScroll() {
			PageViewInfoGenerator.LeftInvisibleWidth = HorizontalScrollController.GetLeftInvisibleWidth();
			Control.RedrawEnsureSecondaryFormattingComplete(RefreshAction.Transforms); 
		}
		protected internal virtual PageViewInfo LookupPageViewInfoByPage(Page page) {
			PageViewInfoCollection pageViewInfos = PageViewInfos;
			int count = pageViewInfos.Count;
			for (int i = 0; i < count; i++) {
				if (pageViewInfos[i].Page == page)
					return pageViewInfos[i];
			}
			return null;
		}
		protected internal virtual void EnsureFormattingCompleteForSelection() {
			EnsureFormattingCompleteForLogPosition(DocumentModel.Selection.NormalizedEnd);
		}
		protected internal virtual void EnsureFormattingCompleteForLogPosition(DocumentLogPosition logPosition) {
			CheckExecutedAtUIThread();
			Formatter.WaitForPrimaryLayoutReachesLogPosition(logPosition);
		}
		protected internal virtual void EnsureFormattingCompleteForPreferredPage(int preferredPageIndex) {
			CheckExecutedAtUIThread();
			Formatter.WaitForPrimaryLayoutReachesPreferredPage(preferredPageIndex);
		}
		protected internal virtual void EnsurePageSecondaryFormattingComplete(Page page) {
			CheckExecutedAtUIThread();
			if (page.PrimaryFormattingComplete && page.SecondaryFormattingComplete)
				return;
			Formatter.WaitForPagePrimaryLayoutComplete(page);
			Debug.Assert(page.PrimaryFormattingComplete);
			DocumentModelPosition pos = page.GetLastPosition(DocumentModel.ActivePieceTable);
			Formatter.WaitForSecondaryLayoutReachesPosition(pos);
		}
		protected internal virtual Matrix CreateTransformMatrix(Rectangle clientBounds) {
			Matrix mat = new Matrix();
			mat.Translate(clientBounds.X - HorizontalScrollController.GetPhysicalLeftInvisibleWidth(), clientBounds.Y);
			mat.Scale(ZoomFactor, ZoomFactor);
			return mat;
		}
		protected internal virtual Point CreateLogicalPoint(Rectangle clientBounds, Point point) {
			Matrix mat = CreateTransformMatrix(clientBounds);
			mat.Invert();
			Point[] result = new Point[1] { point };
			mat.TransformPoints(result);
			return result[0];
		}
		protected internal virtual Rectangle CreateLogicalRectangle(PageViewInfo pageViewInfo, Rectangle bounds) {
			Matrix mat = CreateTransformMatrix(pageViewInfo.ClientBounds);
			mat.Invert();
			Point[] resultPoints = new Point[2] { bounds.Location, new Point(bounds.Right, bounds.Bottom) };
			mat.TransformPoints(resultPoints);
			return new Rectangle(resultPoints[0], new Size(resultPoints[1].X - resultPoints[0].X, resultPoints[1].Y - resultPoints[0].Y));
		}
		protected internal virtual Rectangle CreatePhysicalRectangleFast(PageViewInfo pageViewInfo, Rectangle bounds) {
			Rectangle clientBounds = pageViewInfo.ClientBounds;
			float x = zoomFactor * bounds.X + clientBounds.X - HorizontalScrollController.GetPhysicalLeftInvisibleWidth();
			float y = zoomFactor * bounds.Y + clientBounds.Y;
			float w = zoomFactor * bounds.Width;
			float h = zoomFactor * bounds.Height;
			return new Rectangle((int)x, (int)y, (int)w, (int)h);
		}
		protected internal virtual Point CreatePhysicalPoint(PageViewInfo pageViewInfo, Point point) {
			Matrix mat = CreateTransformMatrix(pageViewInfo.ClientBounds);
			Point[] result = new Point[1] { point };
			mat.TransformPoints(result);
			return result[0];
		}
		protected internal virtual Rectangle CreatePhysicalRectangle(PageViewInfo pageViewInfo, Rectangle bounds) {
			return CreatePhysicalRectangle(pageViewInfo.ClientBounds, bounds);
		}
		protected internal virtual Rectangle CreatePhysicalRectangle(Rectangle pageViewInfoClientBounds, Rectangle bounds) {
			Matrix mat = CreateTransformMatrix(pageViewInfoClientBounds);
			Point[] resultPoints = new Point[2] { bounds.Location, new Point(bounds.Right, bounds.Bottom) };
			mat.TransformPoints(resultPoints);
			return new Rectangle(resultPoints[0], new Size(resultPoints[1].X - resultPoints[0].X, resultPoints[1].Y - resultPoints[0].Y));
		}
		protected internal virtual void UpdateCaretPosition() {
			this.CaretPosition.Update(DocumentLayoutDetailsLevel.Character);
		}
		int oldFirstPageIndex = -1;
		int oldLastPageIndex = -1;
		protected internal virtual void ResetPages(PageGenerationStrategyType strategy) {
			if (PageViewInfos.First != null && PageViewInfos.Last != null) {
				oldFirstPageIndex = PageViewInfos.First.Index;
				oldLastPageIndex = PageViewInfos.Last.Index;
			}
			Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId != Formatter.WorkerThread.ManagedThreadId);
			this.CaretPosition.InvalidatePageViewInfo();
			this.PageViewInfos.Clear();
			this.PageViewInfoGenerationIsAboutToComplete = false;
			this.PageViewInfoGenerationComplete = false;
			this.PageViewInfoGenerator.Reset(strategy);
		}
		protected internal virtual void GeneratePages() {
			Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId != Formatter.WorkerThread.ManagedThreadId);
			PageCollection pages = FormattingController.PageController.Pages;
			int count = pages.Count;
			for (int i = 0; i < count; i++) {
				Page page = pages[i];
				if (page.PrimaryFormattingComplete)
					PageViewInfoGenerationComplete = (PageViewInfoGenerator.ProcessPage(page, i) == ProcessPageResult.VisiblePagesGenerationComplete);
			}
			PageViewInfo lastProcessedPage = PageViewInfos.Last;
			if (lastProcessedPage != null) {
				PageViewInfoGenerator.CalculateWidthParameters();
				VerticalScrollController.UpdateScrollBar();
				UpdateHorizontalScrollbar();
				PageViewInfo firstProcessedPage = PageViewInfos.First;
				Debug.Assert(firstProcessedPage.Page.PrimaryFormattingComplete);
				Debug.Assert(lastProcessedPage.Page.PrimaryFormattingComplete);
				DocumentModelPosition from = firstProcessedPage.Page.GetFirstPosition(DocumentModel.MainPieceTable);
				DocumentModelPosition to = lastProcessedPage.Page.GetLastPosition(DocumentModel.MainPieceTable);
				Formatter.UpdateSecondaryPositions(from, to);
				Formatter.NotifyDocumentChanged(from, to, DocumentLayoutResetType.SecondaryLayout);
				if (oldFirstPageIndex != PageViewInfos.First.Index || oldLastPageIndex != PageViewInfos.Last.Index)
					this.Control.InnerControl.RaiseVisiblePagesChanged();
			}
			oldFirstPageIndex = -1;
			oldLastPageIndex = -1;
		}
		protected internal virtual PageViewInfo GetPageViewInfoFromPoint(Point point, bool strictSearch) {
			PageViewInfoRow pageViewInfoRow = GetPageViewInfoRowFromPoint(point, true);
			if (pageViewInfoRow == null)
				return null;
			return pageViewInfoRow.GetPageAtPoint(point, strictSearch);
		}
		protected internal virtual PageViewInfoRow GetPageViewInfoRowFromPoint(Point point, bool strictSearch) {
			return PageViewInfoGenerator.GetPageRowAtPoint(point, strictSearch);
		}
		protected internal virtual void UpdateHorizontalScrollbar() {
			HorizontalScrollController.UpdateScrollBar();
		}
		protected internal virtual RichEditHitTestResult CalculateNearestCharacterHitTest(Point point, PieceTable pieceTable) {
			RichEditHitTestRequest request = new RichEditHitTestRequest(pieceTable);
			request.PhysicalPoint = point;
			request.DetailsLevel = DocumentLayoutDetailsLevel.Character;
			request.Accuracy = DefaultHitTestPageAccuracy | HitTestAccuracy.NearestPageArea | HitTestAccuracy.NearestColumn | HitTestAccuracy.NearestTableRow | HitTestAccuracy.NearestTableCell | HitTestAccuracy.NearestRow | HitTestAccuracy.NearestBox | HitTestAccuracy.NearestCharacter;
			RichEditHitTestResult result = HitTestCore(request, (DefaultHitTestPageAccuracy & HitTestAccuracy.ExactPage) != 0);
			if (!result.IsValid(DocumentLayoutDetailsLevel.Character))
				return null;
			else
				return result;
		}
		protected internal virtual RichEditHitTestResult CalculateNearestPageHitTest(Point point, bool strictHitIntoPageBounds) {
			RichEditHitTestRequest request = new RichEditHitTestRequest(DocumentModel.ActivePieceTable);
			request.PhysicalPoint = point;
			request.DetailsLevel = DocumentLayoutDetailsLevel.Page;
			request.Accuracy = HitTestAccuracy.NearestPage | HitTestAccuracy.NearestPageArea | HitTestAccuracy.NearestColumn | HitTestAccuracy.NearestTableRow | HitTestAccuracy.NearestTableCell | HitTestAccuracy.NearestRow | HitTestAccuracy.NearestBox | HitTestAccuracy.NearestCharacter;
			RichEditHitTestResult result = HitTestCore(request, strictHitIntoPageBounds);
			if (!result.IsValid(DocumentLayoutDetailsLevel.Page))
				return null;
			else
				return result;
		}
		protected internal virtual RichEditHitTestResult HitTestCore(RichEditHitTestRequest request, bool strictHitIntoPageBounds) {
			PieceTable pieceTable = request.PieceTable;
			RichEditHitTestResult result = new RichEditHitTestResult(DocumentLayout, pieceTable);
			result.PhysicalPoint = request.PhysicalPoint;
			PageViewInfo pageViewInfo = GetPageViewInfoFromPoint(request.PhysicalPoint, strictHitIntoPageBounds);
			if (pageViewInfo == null) {
				return result;
			}
			Point pt = request.PhysicalPoint;
			pt.X += HorizontalScrollController.GetPhysicalLeftInvisibleWidth();
			if (strictHitIntoPageBounds && !PerformStrictPageViewInfoHitTest(pageViewInfo, pt))
				return result;
			result.Page = pageViewInfo.Page;
			result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Page);
			if (request.DetailsLevel <= DocumentLayoutDetailsLevel.Page) {
				result.LogicalPoint = CreateLogicalPoint(pageViewInfo.ClientBounds, request.PhysicalPoint);
				result.FloatingObjectBox = CalculateFloatingObjectHitTest(result.Page, result.LogicalPoint, IsActivePieceTableFloatingObjectBox);
				result.CommentViewInfo = CalculateCommentViewInfoHitTest(result.Page, result.LogicalPoint);
				result.CommentLocation = CalculateCommentLocationHitTest(result.Page, result.LogicalPoint);
				result.FloatingObjectBoxPage = result.Page;
				return result;
			}
			request.LogicalPoint = CreateLogicalPoint(pageViewInfo.ClientBounds, request.PhysicalPoint);
			if (pieceTable.IsTextBox) {
				result = DocumentModelActivePieceTableTextBoxHitTest(request, result, pageViewInfo);
			}
			else {
				if (pieceTable.IsComment)
					result = DocumentModelActivePieceTableIsCommentHitTest(request, result, pageViewInfo);
				else {
					result.FloatingObjectBox = CalculateFloatingObjectHitTest(result.Page, request.LogicalPoint, IsActivePieceTableFloatingObjectBox);
					result.CommentViewInfo = CalculateCommentViewInfoHitTest(result.Page, request.LogicalPoint);
					result.CommentLocation = CalculateCommentLocationHitTest(result.Page, request.LogicalPoint);
					HitTest(pageViewInfo.Page, request, result);
				}
			}
			return result;
		}
		RichEditHitTestResult DocumentModelActivePieceTableTextBoxHitTest(RichEditHitTestRequest request, RichEditHitTestResult result, PageViewInfo pageViewInfo) {
			FloatingObjectBox box = CalculateFloatingObjectHitTest(result.Page, request.LogicalPoint, IsActiveFloatingObjectTextBox);
			if (box != null) {
				Page page = box.DocumentLayout.Pages.First;
				request.SearchAnyPieceTable = true;
				result.FloatingObjectBox = box;
				result.FloatingObjectBoxPage = result.Page;
				HitTest(page, request, result);
			}
			else {
				HitTest(pageViewInfo.Page, request, result);
				if (result.PageArea != null && !Object.ReferenceEquals(result.PageArea.PieceTable, result.PieceTable)) {
					RichEditHitTestResult copy = new RichEditHitTestResult(DocumentLayout, result.PageArea.PieceTable);
					copy.CopyFrom(result);
					result = copy;
				}
			}
			return result;
		}
		bool IsActivePieceTableFloatingObjectBox(FloatingObjectBox box) {
			if (Object.ReferenceEquals(box.PieceTable, DocumentModel.ActivePieceTable))
				return true;
			TextBoxFloatingObjectContent content = box.GetFloatingObjectRun().FloatingObject.Content as TextBoxFloatingObjectContent;
			if (content != null)
				return Object.ReferenceEquals(content.TextBox.PieceTable, DocumentModel.ActivePieceTable);
			else
				return false;
		}
		bool IsActiveFloatingObjectTextBox(FloatingObjectBox box) {
			FloatingObjectAnchorRun run = box.GetFloatingObjectRun();
			TextBoxFloatingObjectContent content = run.Content as TextBoxFloatingObjectContent;
			return box.DocumentLayout != null && content != null && Object.ReferenceEquals(content.TextBox.PieceTable, DocumentModel.ActivePieceTable);
		}
		RichEditHitTestResult DocumentModelActivePieceTableIsCommentHitTest(RichEditHitTestRequest request, RichEditHitTestResult result, PageViewInfo pageViewInfo) {
			CommentViewInfo commentViewInfo = CalculateCommentViewInfoHitTest(result.Page, request.LogicalPoint);
			if (commentViewInfo != null) {
				Page page = commentViewInfo.CommentDocumentLayout.Pages.First;
				request.SearchAnyPieceTable = true;
				result.CommentViewInfo = commentViewInfo;
				result.CommentLocation = CalculateCommentLocationHitTest(result.Page, request.LogicalPoint);
				Point newLogicalPoint = request.LogicalPoint;
				newLogicalPoint.X -= commentViewInfo.ContentBounds.X;
				newLogicalPoint.Y -= commentViewInfo.ContentBounds.Y;
				request.LogicalPoint = newLogicalPoint;
				HitTest(page, request, result);
			}
			else {
				HitTest(pageViewInfo.Page, request, result);
				if (result.PageArea != null && !Object.ReferenceEquals(result.PageArea.PieceTable, result.PieceTable)) {
					RichEditHitTestResult copy = new RichEditHitTestResult(DocumentLayout, result.PageArea.PieceTable);
					copy.CopyFrom(result);
					result = copy;
				}
			}
			return result;
		}
		protected internal virtual FloatingObjectBox CalculateFloatingObjectHitTest(Page page, Point point, Predicate<FloatingObjectBox> predicate) {
			IList<FloatingObjectBox> floatingObjects = page.GetSortedNonBackgroundFloatingObjects(new FloatingObjectBoxZOrderComparer());
			FloatingObjectBox result = CalculateFloatingObjectHitTestCore(point, floatingObjects, predicate);
			if (result != null)
				return result;
			return CalculateFloatingObjectHitTestCore(point, page.InnerBackgroundFloatingObjects, predicate);
		}
		protected internal virtual FloatingObjectBox CalculateFloatingObjectHitTestCore(Point point, IList<FloatingObjectBox> list, Predicate<FloatingObjectBox> predicate) {
			if (list == null)
				return null;
			int count = list.Count;
			for (int i = count - 1; i >= 0; i--)
				if (IsFloatingObjectHit(list[i], point) && predicate(list[i]) )
					return list[i];
			return null;
		}
		bool IsFloatingObjectHit(FloatingObjectBox box, Point point) {
			return box.Bounds.Contains(box.TransformPointBackward(point));
		}
		protected internal virtual CommentViewInfo CalculateCommentViewInfoHitTest(Page page, Point point) {
			IList<CommentViewInfo> comments = page.InnerComments;
			if (comments == null)
				return null;
			return CalculateCommentViewInfoHitTestCore(point, comments);
		}
		protected internal virtual CommentLocationType CalculateCommentLocationHitTest(Page page, Point point) {
			IList<CommentViewInfo> comments = page.InnerComments;
			if (comments == null)
				return CommentLocationType.None;
			return CalculateCommentLocationHitTestCore(point, comments);
		}
		protected internal virtual CommentViewInfo CalculateCommentViewInfoHitTestCore(Point point, IList<CommentViewInfo> list) {
			if (list == null)
				return null;
			int count = list.Count;
			for (int i = count - 1; i >= 0; i--)
				if (IsCommentViewInfoHit(list[i], point))
					return list[i];
			return null;
		}
		protected internal virtual CommentLocationType CalculateCommentLocationHitTestCore(Point point, IList<CommentViewInfo> list) {
			if (list == null)
				return CommentLocationType.None;
			int count = list.Count;
			for (int i = count - 1; i >= 0; i--) {
				if (IsCommentMoreButtonHit(list[i], point))
					return CommentLocationType.CommentMoreButton;
				if (IsCommentContentHit(list[i], point))
					return CommentLocationType.CommentContent;
			}
			return CommentLocationType.None;
		}
		bool IsCommentViewInfoHit(CommentViewInfo commentViewInfo, Point point) {
			return commentViewInfo.Bounds.Contains(point);
		}
		bool IsCommentMoreButtonHit(CommentViewInfo commentViewInfo, Point point) {
			return commentViewInfo.CommentMoreButtonBounds.Contains(point);
		}
		bool IsCommentContentHit(CommentViewInfo commentViewInfo, Point point) {
			return commentViewInfo.ContentBounds.Contains(point);
		}
		protected internal virtual void HitTest(Page page, RichEditHitTestRequest request, RichEditHitTestResult result) {
			BoxHitTestCalculator calculator = CreateHitTestCalculator(request, result);
			if (request.DetailsLevel != DocumentLayoutDetailsLevel.Character) {
				calculator.CalcHitTest(page);
				return;
			}
			RichEditHitTestRequest rowRequest = request.Clone();
			rowRequest.DetailsLevel = DocumentLayoutDetailsLevel.Row;
			calculator = CreateHitTestCalculator(rowRequest, result);
			calculator.CalcHitTest(page);
			if ((result.CommentViewInfo != null) && (result.CommentLocation != CommentLocationType.CommentContent) && (result.IsValid(DocumentLayoutDetailsLevel.Row)))
				DecreaseDetailLevel(result);
			if (!result.IsValid(DocumentLayoutDetailsLevel.Row))
				return;
			RichEditHitTestResult boxResult = new RichEditHitTestResult(DocumentLayout, DocumentModel.ActivePieceTable);
			boxResult.CopyFrom(result);
			RichEditHitTestRequest boxRequest = request.Clone();
			boxRequest.DetailsLevel = DocumentLayoutDetailsLevel.Box;
			calculator = CreateHitTestCalculator(boxRequest, boxResult);
			calculator.CalcHitTest(boxResult.Row);
			if (!boxResult.IsValid(DocumentLayoutDetailsLevel.Box))
				return;
			result.CopyFrom(boxResult);
			DetailRow detailRow = DocumentLayout.CreateDetailRow(result.Row);
			bool strictHitTest = ((request.Accuracy & HitTestAccuracy.ExactCharacter) != 0);
			calculator = CreateHitTestCalculator(request, result);
			calculator.FastHitTestCharacter(detailRow.Characters, strictHitTest);
		}
		void DecreaseDetailLevel(RichEditHitTestResult result) {
			result.DecreaseDetailLevel(DocumentLayoutDetailsLevel.Column);
			result.Row = null;
			result.Box = null;
			result.Character = null;
		}
		protected internal virtual RichEditHitTestResult CalculateHitTest(Point point, DocumentLayoutDetailsLevel detailsLevel) {
			RichEditHitTestRequest request = new RichEditHitTestRequest(DocumentModel.ActivePieceTable);
			request.PhysicalPoint = point;
			request.DetailsLevel = detailsLevel;
			request.Accuracy = DefaultHitTestPageAccuracy | HitTestAccuracy.NearestPageArea | HitTestAccuracy.NearestColumn | HitTestAccuracy.NearestTableRow | HitTestAccuracy.NearestTableCell | HitTestAccuracy.NearestRow | HitTestAccuracy.NearestBox | HitTestAccuracy.NearestCharacter;
			RichEditHitTestResult result = HitTestCore(request, true);
			if (!result.IsValid(detailsLevel))
				return null;
			else
				return result;
		}
		protected internal virtual bool PerformStrictPageViewInfoHitTest(PageViewInfo pageViewInfo, Point pt) {
			return pageViewInfo.ClientBounds.Contains(pt);
		}
		protected internal virtual void OnResize(Rectangle bounds, bool ensureCaretVisibleonResize) {
			Debug.Assert(bounds.Location == Point.Empty);
			PerformResize(bounds);
			Control.InnerControl.BeginDocumentRendering();
			Control.InnerControl.EndDocumentRendering();
			SetOptimalHorizontalScrollbarPosition();
		}
		protected internal virtual void PerformResize(Rectangle bounds) {
			Debug.Assert(bounds.Location == Point.Empty);
			DocumentModel.BeginUpdate();
			try {
				if (DocumentModel.IsUpdateLocked) {
					this.Bounds = bounds;
					ResetPages(PageGenerationStrategyType.FirstPageOffset);
					OnResizeCore();
					Debug.Assert(PageViewInfoGenerator.ActiveGenerator.Pages.Count <= 0);
					GeneratePages();
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void OnResizeCore() {
		}
		protected internal virtual void SubscribePageFormattingComplete() {
			this.FormattingController.PageFormattingStarted += OnPageFormattingStarted;
			this.FormattingController.PageFormattingComplete += OnPageFormattingComplete;
		}
		protected internal virtual void UnsubscribePageFormattingComplete() {
			this.FormattingController.PageFormattingStarted -= OnPageFormattingStarted;
			this.FormattingController.PageFormattingComplete -= OnPageFormattingComplete;
		}
#if DEBUGTEST
		bool formattingSuspended;
#endif
		int suspendFormattingCount;
		protected internal virtual void SuspendFormatting() {
			if (suspendFormattingCount == 0) {
				Formatter.BeginDocumentUpdate();
#if DEBUGTEST
				formattingSuspended = true;
#endif
			}
			suspendFormattingCount++;
		}
		protected internal virtual void ResumeFormatting() {
			suspendFormattingCount--;
			if (suspendFormattingCount == 0) {
				Formatter.EndDocumentUpdate();
#if DEBUGTEST
				formattingSuspended = false;
#endif
			}
		}
		protected internal virtual int CalculateFirstInvisiblePageIndexBackward() {
			PageViewInfoRow firstRow = PageViewInfoGenerator.ActiveGenerator.PageRows.First;
			PageViewInfo pageViewInfo = firstRow.First;
			if (pageViewInfo == null)
				return -1;
			else
				return FormattingController.PageController.Pages.IndexOf(pageViewInfo.Page) - 1;
		}
		protected internal virtual int CalculateFirstInvisiblePageIndexForward() {
			PageViewInfoRow lastRow = PageViewInfoGenerator.ActiveGenerator.PageRows.Last;
			PageViewInfo pageViewInfo = lastRow.Last;
			if (pageViewInfo == null)
				return -1;
			else
				return 1 + FormattingController.PageController.Pages.IndexOf(pageViewInfo.Page);
		}
		protected internal virtual void EnsureCaretVisible() {
			RichEditCommand command = new EnsureCaretVisibleVerticallyCommand(Control);
			command.Execute();
			command = new EnsureCaretVisibleHorizontallyCommand(Control);
			command.Execute();
		}
		protected internal virtual void EnsureCaretVisibleOnResize() {
		}
		public override string ToString() {
			return String.Empty;
		}
		protected internal abstract DocumentLayoutExporter CreateDocumentLayoutExporter(Painter painter, GraphicsDocumentLayoutExporterAdapter adapter, PageViewInfo pageViewInfo, Rectangle bounds);
		protected internal Field GetHyperlinkField(RichEditHitTestResult hitTestResult) {
			if (hitTestResult.DetailsLevel < DocumentLayoutDetailsLevel.Box)
				return null;
			if ((hitTestResult.Accuracy & HitTestAccuracy.ExactBox) == 0)
				return null;
			return hitTestResult.PieceTable.GetHyperlinkField(hitTestResult.Box.StartPos.RunIndex);
		}
		protected internal virtual void OnLayoutUnitChanging() {
			ResetPages(PageGenerationStrategyType.FirstPageOffset); 
			PageViewInfoGenerator.OnLayoutUnitChanging(DocumentModel.LayoutUnitConverter); 
		}
		protected internal virtual void OnLayoutUnitChanged() {
			CaretPosition.Invalidate();
			CaretPosition.InvalidateInputPosition();
			PageViewInfoGenerator.OnLayoutUnitChanged(DocumentModel.LayoutUnitConverter); 
			FormattingController.Reset(false);
			PieceTable pieceTable = DocumentModel.MainPieceTable;
			DocumentModelPosition from = new DocumentModelPosition(pieceTable); 
			DocumentModelPosition to = DocumentModelPosition.FromParagraphEnd(pieceTable, pieceTable.Paragraphs.Last.Index);
			Formatter.UpdateSecondaryPositions(from, to);
			Formatter.NotifyDocumentChanged(from, to, DocumentLayoutResetType.AllPrimaryLayout);
		}
		public virtual Rectangle GetCursorBounds() {
			if (DocumentModel.Selection.Length > 0)
				return Rectangle.Empty;
			return GetCursorBoundsCore();
		}
		internal Rectangle GetCursorBoundsCore() {
			if (!CaretPosition.Update(DocumentLayoutDetailsLevel.Character))
				return Rectangle.Empty;
			Rectangle caretBounds = CaretPosition.CalculateCaretBounds();
			return GetPhysicalBounds(CaretPosition.PageViewInfo, caretBounds);
		}
		public virtual DevExpress.XtraRichEdit.API.Native.DocumentRange GetVisiblePagesRange() {
			BeginDocumentRendering();
			try {
				return GetVisiblePagesRangeCore();
			}
			finally {
				EndDocumentRendering();
			}
		}
		protected internal virtual DevExpress.XtraRichEdit.API.Native.DocumentRange GetVisiblePagesRangeCore() {
			DocumentLogPosition start = PageViewInfos.First.Page.GetFirstPosition(DocumentModel.MainPieceTable).LogPosition;
			DocumentLogPosition end = PageViewInfos.Last.Page.GetLastPosition(DocumentModel.MainPieceTable).LogPosition;
			if (end <= start)
				return Control.InnerControl.NativeDocument.CreateZeroLengthRange(end);
			else
				return Control.InnerControl.NativeDocument.CreateRange(start, end - start);
		}
		protected internal virtual Rectangle HitTestByPhysicalPoint(Point pt, DocumentLayoutDetailsLevel detailsLevel) {
			RichEditHitTestResult hitTest = CalculateHitTest(pt, detailsLevel);
			if (hitTest == null) return new Rectangle();
			Row row = hitTest.Row;
			PageViewInfo pageViewInfo = LookupPageViewInfoByPage(hitTest.Page);
			return GetPhysicalBounds(pageViewInfo, row.Bounds);
		}
		protected internal virtual Rectangle GetPhysicalBounds(PageViewInfo pageViewInfo, Rectangle bounds) {
			Rectangle viewBounds = CreateLogicalRectangle(pageViewInfo, this.Bounds);
			int x = (int)((bounds.Location.X - viewBounds.Location.X) * ZoomFactor);
			int y = (int)((bounds.Location.Y - viewBounds.Location.Y) * ZoomFactor);
			Point position = new Point(x, y);
			bounds.Width = (int)(bounds.Width * ZoomFactor);
			bounds.Height = (int)(bounds.Height * ZoomFactor);
			DocumentLayoutUnitConverter unitConverter = documentLayout.UnitConverter;
			Point screenPosition = unitConverter.LayoutUnitsToPixels(position);
			int screenWidth = unitConverter.LayoutUnitsToPixels(bounds.Width);
			int screenHeight = unitConverter.LayoutUnitsToPixels(bounds.Height);
			Point rulersOffset = CalculateRulersOffet();
			return new Rectangle(screenPosition.X + this.ActualPadding.Left + rulersOffset.X, screenPosition.Y + this.ActualPadding.Top + rulersOffset.Y, screenWidth, screenHeight);
		}
		Point CalculateRulersOffet() {
			int horizontalRulerHeight = 0;
			IRulerControl horizontalRuler = Control.InnerControl.HorizontalRuler;
			if (horizontalRuler != null && horizontalRuler.IsVisible)
				horizontalRulerHeight = horizontalRuler.GetRulerSizeInPixels();
			int verticalRulerWidth = 0;
			IRulerControl verticalRuler = Control.InnerControl.VerticalRuler;
			if (verticalRuler != null && verticalRuler.IsVisible)
				verticalRulerWidth = verticalRuler.GetRulerSizeInPixels();
			return new Point(verticalRulerWidth, horizontalRulerHeight);
		}
		Rectangle GetActualClientBoundsInPixels(PageViewInfo viewInfo) {
			Rectangle result = DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(viewInfo.ClientBounds);
			result.Offset(CalculateRulersOffet());
			return result;
		}
		public PageLayoutPosition GetDocumentLayoutPosition(Point point) {
			int pageIndex = -1;
			Rectangle bounds = Rectangle.Empty;
			List<PageLayoutInfo> infos = GetVisiblePageLayoutInfos();
			for (int i = 0; i < infos.Count; i++) {
				bounds = infos[i].Bounds;
				if (bounds.Contains(point)) {
					pageIndex = infos[i].PageIndex;
					break;
				}
			}
			if (pageIndex == -1)
				return null;
			Point position = CreateLogicalPoint(bounds, point);
			position.X = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(position.X);
			position.Y = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(position.Y);
			return new PageLayoutPosition(pageIndex, position);
		}
		public List<PageLayoutInfo> GetVisiblePageLayoutInfos() {
			List<PageLayoutInfo> result = new List<PageLayoutInfo>();
			for (int i = 0; i < PageViewInfos.Count; i++) {
				int pageIndex = PageViewInfos[i].Index;
				Rectangle bounds = GetActualClientBoundsInPixels(PageViewInfos[i]);
				bounds.Offset(-(int)PageViewInfoGenerator.LeftInvisibleWidth, 0);
				result.Add(new PageLayoutInfo(pageIndex, bounds));
			}
			return result;
		}
		public Point? PageLayoutPositionToPoint(PageLayoutPosition position) {
			List<PageLayoutInfo> pageInfos = GetVisiblePageLayoutInfos();
			int viewInfoIndex = -1;
			for (int i = 0; i < pageInfos.Count; i++)
				if (pageInfos[i].PageIndex == position.PageIndex) {
					viewInfoIndex = i;
					break;
				}
			if (viewInfoIndex < 0)
				return null;
			Point result = DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(CreatePhysicalPoint(PageViewInfos[viewInfoIndex], position.Position));
			result.Offset(CalculateRulersOffet());
			return result;
		}
		protected internal virtual Rectangle CalculatePageContentClipBounds(PageViewInfo page) {
			int clipWidth = page.ClientBounds.Width;
			int clipHeight = page.ClientBounds.Height;
			float zoomFactor = ZoomFactor;
			Rectangle clipBounds = new Rectangle(0, 0, clipWidth, clipHeight);
			clipBounds.X = (int)Math.Ceiling(clipBounds.X / zoomFactor);
			clipBounds.Y = (int)Math.Ceiling(clipBounds.Y / zoomFactor);
			clipBounds.Width = (int)Math.Ceiling(clipBounds.Width / zoomFactor);
			clipBounds.Height = (int)Math.Ceiling(clipBounds.Height / zoomFactor) + 1;
			clipBounds = ExpandClipBoundsToPaddings(clipBounds);
			return clipBounds;
		}
		protected internal virtual Rectangle ExpandClipBoundsToPaddings(Rectangle clipBounds) {
			Padding padding = ActualPadding;
			int leftPadding = 3 * DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(padding.Left) / 4;
			int topPadding = 3 * DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(padding.Top) / 4;
			clipBounds.Height += topPadding;
			clipBounds.Y -= topPadding;
			clipBounds.Width += leftPadding;
			clipBounds.X -= leftPadding;
			return clipBounds;
		}
		protected internal virtual Rectangle CalculateVisiblePageBounds(Rectangle pageBounds, PageViewInfo pageViewInfo) {
			int pageViewInfoTotalHeight = pageViewInfo.Bounds.Height;
			if (pageViewInfoTotalHeight > 0) {
				Rectangle viewPort = PageViewInfoGenerator.ViewPortBounds;
				Rectangle extendedViewPortBounds = new Rectangle(0, viewPort.Top, int.MaxValue, viewPort.Height);
				Rectangle pageViewInfoVisibleBounds = Rectangle.Intersect(extendedViewPortBounds, pageViewInfo.Bounds);
				Rectangle visibleBounds = pageBounds;
				visibleBounds.Y = (int)Math.Floor(pageBounds.Height / (double)pageViewInfoTotalHeight * (pageViewInfoVisibleBounds.Y - pageViewInfo.Bounds.Y));
				visibleBounds.Height = (int)Math.Ceiling(pageBounds.Height / (double)pageViewInfoTotalHeight * pageViewInfoVisibleBounds.Height);
				return visibleBounds;
			}
			else
				return Rectangle.Empty;
		}
		public abstract void Visit(IRichEditViewVisitor visitor);
		protected internal abstract Size CalcBestSize(bool fixedWidth);
		protected internal abstract void OnAutoSizeModeChanged();
		protected internal abstract bool MatchHorizontalTableIndentsToTextEdge { get; }
		protected internal virtual void ApplyNewCommentPadding(CommentPadding commentPadding) {
			Formatter.BeginDocumentUpdate();
			try {
				Formatter.ApplyNewCommentPadding(commentPadding);
				DocumentModel.MainPieceTable.ApplyChangesCore(DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.Redraw, RunIndex.Zero, RunIndex.Zero);
			}
			finally {
				Formatter.EndDocumentUpdate();
			}
		}
		protected internal void ProcessSelectionChanges(DocumentModelChangeActions changeActions) {
			CaretPosition.Invalidate();
			if ((changeActions & DocumentModelChangeActions.ResetCaretInputPositionFormatting) != 0)
				CaretPosition.InvalidateInputPosition();
			if ((changeActions & DocumentModelChangeActions.ValidateSelectionInterval) != 0)
				ValidateSelectionInterval();
		}
	}
	#endregion
	public enum AutoSizeMode { None, Horizontal, Vertical, Both }
	[ComVisible(true)]
	public interface IRichEditViewVisitor {
		void Visit(SimpleView view);
		void Visit(DraftView view);
		void Visit(PrintLayoutView view);
		void Visit(ReadingLayoutView view);
	}
	#region PageBasedRichEditView (abstract class)
	[ComVisible(true)]
	public abstract class PageBasedRichEditView : RichEditView {
		protected PageBasedRichEditView(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		#region CurrentPageIndex
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int CurrentPageIndex {
			get {
				if (CaretPosition == null)
					return -1;
				DocumentLayoutPosition position = CaretPosition.LayoutPosition.Clone();
				if (!position.IsValid(DocumentLayoutDetailsLevel.Page))
					position.Update(FormattingController.PageController.Pages, DocumentLayoutDetailsLevel.Page);
				if (position.IsValid(DocumentLayoutDetailsLevel.Page))
					return position.Page.PageIndex;
				else
					return (FormattingController.PageController.Pages.Count == 1) ? 0 : -1;
			}
		}
		#endregion
		#region PageCount
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int PageCount { get { return FormattingController != null ? FormattingController.PageCount : 0; } }
		#endregion
		#endregion
		#region Events
		#region PageCountChanged
		EventHandler onPageCountChanged;
		public event EventHandler PageCountChanged { add { onPageCountChanged += value; } remove { onPageCountChanged -= value; } }
		protected internal void RaisePageCountChanged() {
			if (onPageCountChanged != null)
				onPageCountChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal void OnPageCountChanged(object sender, EventArgs e) {
			Control.UpdateUIFromBackgroundThread(delegate {
				RaisePageCountChanged();
			});
		}
		protected internal override void Activate(Rectangle viewBounds) {
			base.Activate(viewBounds);
			FormattingController.PageCountChanged += OnPageCountChanged;
		}
		protected internal override void Deactivate() {
			FormattingController.PageCountChanged -= OnPageCountChanged;
			base.Deactivate();
		}
		protected internal override Size CalcBestSize(bool fixedWidth) {
			return Size.Empty;
		}
		protected internal override void OnAutoSizeModeChanged() {
		}
	}
	#endregion
	#region RichEditViewRepository (abstract class)
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	[ComVisible(true)]
	public abstract class RichEditViewRepository : IDisposable {
		#region Fields
		bool isDisposed;
		readonly IRichEditControl control;
		readonly Dictionary<RichEditViewType, RichEditView> views;
		#endregion
		protected RichEditViewRepository(IRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			views = new Dictionary<RichEditViewType, RichEditView>();
			CreateViews();
		}
		#region Properties
		internal bool IsDisposed { get { return isDisposed; } }
		protected internal Dictionary<RichEditViewType, RichEditView> Views { get { return views; } }
		protected internal IRichEditControl Control { get { return control; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditViewRepositoryPrintLayoutView"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PrintLayoutView PrintLayoutView { get { return (PrintLayoutView)Views[RichEditViewType.PrintLayout]; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditViewRepositoryDraftView"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public DraftView DraftView { get { return (DraftView)Views[RichEditViewType.Draft]; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditViewRepositorySimpleView"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public SimpleView SimpleView { get { return (SimpleView)Views[RichEditViewType.Simple]; } }
		#endregion
		protected internal virtual void CreateViews() {
			RegisterView(CreatePrintLayoutView());
			RegisterView(CreateDraftView());
			RegisterView(CreateSimpleView());
		}
		protected internal abstract PrintLayoutView CreatePrintLayoutView();
		protected internal abstract DraftView CreateDraftView();
		protected internal abstract SimpleView CreateSimpleView();
		protected internal abstract ReadingLayoutView CreateReadingLayoutView();
		protected internal virtual void RegisterView(RichEditView view) {
			Guard.ArgumentNotNull(view, "view");
#if DEBUG
			RichEditView result;
			Debug.Assert(Views.TryGetValue(view.Type, out result) == false);
#endif
			Views.Add(view.Type, view);
		}
		protected internal virtual RichEditView GetViewByType(RichEditViewType type) {
			return Views[type];
		}
		protected internal virtual void DisposeViews() {
			foreach (RichEditViewType type in Views.Keys)
				Views[type].Dispose();
			Views.Clear();
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing)
				DisposeViews();
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		public override string ToString() {
			return String.Empty;
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Internal {
	#region RefreshAction
	[Flags]
	public enum RefreshAction {
		AllDocument = 1,
		Zoom = 2,
		Transforms = 4,
		Selection = 8,
		CommentViewInfo = 16
	}
	#endregion
}
