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
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.Services;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.API.Native.Implementation;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Internal;
using DevExpress.Office.Utils;
using DevExpress.Office.Services.Implementation;
using ApiDocumentPosition = DevExpress.XtraRichEdit.API.Native.DocumentPosition;
using Shape = DevExpress.XtraRichEdit.API.Native.Shape;
using ShapeCollection = DevExpress.XtraRichEdit.API.Native.ShapeCollection;
using DevExpress.XtraRichEdit.Mouse;
using DevExpress.XtraRichEdit.Keyboard;
using DevExpress.Compatibility.System.Drawing;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Office.Layout;
using System.Collections.Generic;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.ComponentModel.Design;
using System.Diagnostics;
#else
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Drawing;
#endif
namespace DevExpress.XtraRichEdit.Internal {
	public partial class InnerRichEditControl : InnerRichEditDocumentServer, IGestureStateIndicator {
#region Fields
		BackgroundThreadUIUpdater backgroundThreadUIUpdater;
		RichEditView activeView;
		RichEditViewRepository views;
		RichEditViewType? activeViewTypeBeforeActiveViewCreation;
		bool readOnly;
		RichEditControlDeferredChanges deferredChanges;
		bool updateUIOnIdle;
		bool forceUpdateUIOnIdle;
		IOfficeScrollbar verticalScrollbar;
		IOfficeScrollbar horizontalScrollbar;
		IRulerControl horizontalRuler;
		IRulerControl verticalRuler;
		bool gestureActivated = false;
		bool overtype;
#endregion
		public InnerRichEditControl(IInnerRichEditControlOwner owner)
			: base(owner) {
		}
		internal InnerRichEditControl(IInnerRichEditControlOwner owner, float dpiX, float dpiY)
			: base(owner, dpiX, dpiY) {
		}
#region Properties
		public override DocumentFormattingController FormattingController { get { return ActiveView != null ? ActiveView.FormattingController : null; } }
		public new IInnerRichEditControlOwner Owner { get { return (IInnerRichEditControlOwner)base.Owner; } }
		protected internal BackgroundThreadUIUpdater BackgroundThreadUIUpdater {
			get { return backgroundThreadUIUpdater; }
			set {
				Guard.ArgumentNotNull(value, "backgroundThreadUIUpdater");
				backgroundThreadUIUpdater = value;
			}
		}
		protected internal BackgroundFormatter Formatter { get { return this.BackgroundFormatter; } }
		public RichEditView ActiveView { get { return activeView; } }
		public RichEditViewType ActiveViewType {
			get {
				if (ActiveView != null)
					return ActiveView.Type;
				else
					return (activeViewTypeBeforeActiveViewCreation != null) ? (RichEditViewType)activeViewTypeBeforeActiveViewCreation : DefaultViewType;
			}
			set {
				if (ActiveView == null) {
					activeViewTypeBeforeActiveViewCreation = value;
					return;
				}
				if (ActiveViewType == value)
					return;
				SetActiveView(views.GetViewByType(value));
				OnUpdateUI();
			}
		}
		public virtual RichEditViewType DefaultViewType { get { return RichEditViewType.PrintLayout; } }
		public RichEditViewRepository Views { get { return views; } }
		public Font Font { get { return Owner.Font; } }
		public Color ForeColor { get { return Owner.ForeColor; } }
#region ReadOnly
		public override bool ReadOnly {
			get { return readOnly; }
			set {
				if (readOnly == value)
					return;
				readOnly = value;
				OnReadOnlyChanged();
			}
		}
		protected internal override bool ActualReadOnly { get { return ReadOnly; } }
#endregion
#region Overtype
		public bool Overtype {
			get { return overtype; }
			set {
				if (overtype == value)
					return;
				overtype = value;
				OnOvertypeChanged();
			}
		}
#endregion
		public override bool Enabled { get { return Owner.Enabled; } }
		protected internal override void SetUnit(DocumentUnit value) {
			base.SetUnit(value);
			UpdateHorizontalRuler();
			UpdateVerticalRuler();
		}
		protected internal RichEditControlDeferredChanges ControlDeferredChanges { get { return deferredChanges; } }
		protected internal bool ForceUpdateUIOnIdle { get { return forceUpdateUIOnIdle; } set { forceUpdateUIOnIdle = value; } }
		protected internal bool UpdateUIOnIdle { get { return updateUIOnIdle; } set { updateUIOnIdle = value; } }
		protected internal Rectangle ViewBounds { get { return Owner.ViewBounds; } }
		protected internal IOfficeScrollbar VerticalScrollBar { get { return verticalScrollbar; } }
		protected internal IOfficeScrollbar HorizontalScrollBar { get { return horizontalScrollbar; } }
		protected internal IRulerControl HorizontalRuler { get { return horizontalRuler; } }
		protected internal IRulerControl VerticalRuler { get { return verticalRuler; } }
		public bool CanUndo {
			get {
				UndoCommand command = new UndoCommand(Owner);
				return command.CanExecute();
			}
		}
		public bool CanRedo {
			get {
				RedoCommand command = new RedoCommand(Owner);
				return command.CanExecute();
			}
		}
		public long VerticalScrollPosition { get { return GetVerticalScrollPosition(); } set { SetVerticalScrollPosition(value); } }
		public long VerticalScrollValue { get { return GetVerticalScrollValue(); } set { SetVerticalScrollValue(value); } }
		public override DocumentLayout ModelDocumentLayout { get { return ActiveView != null ? ActiveView.DocumentLayout : null; } }
#endregion
#region IDisposable implementation
		protected internal override void Dispose(bool disposing) {
			try {
				if (disposing) {
					DisposeViews();
					if (keyboardHandlers != null) {
						keyboardHandlers.Clear();
						keyboardHandlers = null;
					}
					if (mouseHandlers != null) {
						mouseHandlers.Clear();
						mouseHandlers = null;
					}
					if (defaultMouseHandler != null) {
						defaultMouseHandler.Dispose();
						defaultMouseHandler = null;
					}
					this.horizontalScrollbar = null;
					this.verticalScrollbar = null;
					this.horizontalRuler = null;
					this.verticalRuler = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
#endregion
		public override void BeginInitialize() {
			base.BeginInitialize();
			this.backgroundThreadUIUpdater = new DeferredBackgroundThreadUIUpdater();
		}
		public override void EndInitialize() { 
			if (Formatter != null)
				Formatter.SuspendWorkerThread(); 
			base.EndInitialize();
			if (Formatter != null)
				Formatter.ResumeWorkerThread();
			this.verticalScrollbar = Owner.CreateVerticalScrollBar();
			this.horizontalScrollbar = Owner.CreateHorizontalScrollBar();
			InitializeKeyboardHandlers();
			InitializeMouseHandlers();
			AddServices();
			CreateViews();
			this.horizontalRuler = Owner.CreateHorizontalRuler();
			this.verticalRuler = Owner.CreateVerticalRuler();
		}
		protected internal override void CreateNewMeasurementAndDrawingStrategy() {
			base.CreateNewMeasurementAndDrawingStrategy();
			if (Formatter != null)
				Formatter.OnNewMeasurementAndDrawingStrategyChanged();
		}
		protected internal override void OnFirstBeginUpdateCore() {
			base.OnFirstBeginUpdateCore();
			this.deferredChanges = new RichEditControlDeferredChanges();
			Owner.HideCaret();
			BeginScrollbarUpdate(HorizontalScrollBar);
			BeginScrollbarUpdate(VerticalScrollBar);
		}
		protected internal override void OnLastEndUpdateCore() {
			base.OnLastEndUpdateCore();
			EndScrollbarUpdate(VerticalScrollBar);
			EndScrollbarUpdate(HorizontalScrollBar);
#if !SL
			if (deferredChanges.Resize)
				OnResizeCore();
#endif
			if (deferredChanges.Redraw) {
				if (deferredChanges.RedrawAction == RefreshAction.Selection)
					Owner.Redraw(RefreshAction.Selection);
				else
					Owner.RedrawAfterEndUpdate();
			}
			Owner.ShowCaret();
			if (deferredChanges.RaiseUpdateUI)
				RaiseUpdateUI();
			this.deferredChanges = null;
		}
#region DocumentModel creation and event handling
		protected internal override void OnBeginDocumentUpdateCore() {
			base.OnBeginDocumentUpdateCore();
			if (ActiveView != null)
				ActiveView.OnBeginDocumentUpdate();
		}
		protected internal override DocumentModelChangeActions OnEndDocumentUpdateCore(object sender, DocumentUpdateCompleteEventArgs e) {
			DocumentModelChangeActions changeActions = ProcessEndDocumentUpdateCore(sender, e);
			if (ActiveView != null) {
				OnDocumentLayoutChanged(this, e);
				if (!DocumentModel.IsUpdateLocked) {
					if ((changeActions & DocumentModelChangeActions.ResetSelectionLayout) != 0)
						ActiveView.SelectionLayout.Invalidate();
					if ((changeActions & DocumentModelChangeActions.ScrollToBeginOfDocument) != 0) {
						ActiveView.PageViewInfoGenerator.ResetAnchors();
					}
				}
				ActiveView.OnEndDocumentUpdate();
				ApplyChangesCore(changeActions);
			}
			return changeActions;
		}
		protected internal override void OnInnerSelectionChanged(object sender, EventArgs e) {
			base.OnInnerSelectionChanged(sender, e);
			if (ActiveView != null)
				ActiveView.OnSelectionChanged();
		}
		protected internal override void ApplyChangesCore(DocumentModelChangeActions changeActions) {
			if (!DocumentModel.IsUpdateLocked) {
				ApplyChangesCorePlatformSpecific(changeActions);
				if (DocumentModel.DeferredChanges.EnsureCaretVisible)
					ActiveView.EnsureCaretVisible();
			}
			base.ApplyChangesCore(changeActions);
			if ((changeActions & DocumentModelChangeActions.DeleteComment) != 0) {
				ActiveView.UpdateCaretPosition();
			}
			if (((changeActions & DocumentModelChangeActions.RaiseSelectionChanged) != 0) &&
				ActiveView.DocumentModel.ActivePieceTable.IsComment)
				PerformRaiseReviewingForm();
		}
		void PerformRaiseReviewingForm() {
			CommentCaretPosition position = ActiveView.CaretPosition as CommentCaretPosition;
			if (position != null) {
				CommentViewInfo comment = GetCommentViewInfoFromCaretPosition(position);
				if (comment != null && CommentViewInfoNoContainsCursorCore(position, comment) )
					ActiveView.Control.ShowReviewingPaneForm(DocumentModel, comment, DocumentModel.Selection.Start, position.LogPosition, true);
			}
		}
		protected internal bool CommentViewInfoNoContainsCursor(CommentCaretPosition position) {
			CommentViewInfo comment = GetCommentViewInfoFromCaretPosition(position);
			return CommentViewInfoNoContainsCursorCore(position, comment);
		}
		protected internal CommentViewInfo GetCommentViewInfoFromCaretPosition(CommentCaretPosition position) {
			position.Update(DocumentLayoutDetailsLevel.Row);
			return position.FindCommentViewInfo();
		}
		protected internal bool CommentViewInfoNoContainsCursorCore(CommentCaretPosition position, CommentViewInfo comment) {
			Rectangle commentContentPhisicalBounds = ActiveView.GetPhysicalBounds(position.PageViewInfo, comment.ContentBounds);
			Rectangle cursorBounds = ActiveView.GetCursorBoundsCore();
			if ((comment != null) && (!commentContentPhisicalBounds.Contains(cursorBounds)))
				return true;
			return false;
		}
		protected internal void ActivateMainPieceTable(IRichEditControl control, DocumentLogPosition start) {
			DocumentModel.BeginUpdate();
			try {
				ChangeActivePieceTableCommand command = new ChangeActivePieceTableCommand(control, DocumentModel.MainPieceTable, null, -1);
				command.Execute();
				DocumentModel.Selection.Start = start;
				DocumentModel.Selection.End = start;
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal override void PerformRaiseDeferredEventsCore(DocumentModelChangeActions changeActions) {
#if !SL
			if ((changeActions & DocumentModelChangeActions.ForceResize) != 0)
				OnResize();
			else {
#endif
				if ((changeActions & DocumentModelChangeActions.ResetRuler) != 0)
					UpdateRulers();
				if ((changeActions & DocumentModelChangeActions.ForceResetHorizontalRuler) != 0)
					UpdateHorizontalRuler();
				if ((changeActions & DocumentModelChangeActions.ForceResetVerticalRuler) != 0)
					UpdateVerticalRuler();
#if !SL
			}
#endif
			base.PerformRaiseDeferredEventsCore(changeActions);
		}
#endregion
#region Options creation and event handling
		protected internal override void OnOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			base.OnOptionsChanged(sender, e);
			if (e.Name == "HighlightColor" || e.Name == "HighlightMode")
				OnOptionsFieldsChanged();
			if (e.Name == "BookmarkVisibility" || e.Name == "BookmarkColor")
				OnOptionsBookmarksChanged();
			if (e.Name == "RangePermissionVisibility" || e.Name == "RangePermissionColor")
				OnOptionsRangePermissionsChanged();
			if (e.Name == "CommentVisibility" || e.Name == "CommentColor" || e.Name == "CommentBracketsColor")
				OnOptionsCommentsChanged();
			if (e.Name == "ParagraphMark" || e.Name == "TabCharacter" || e.Name == "Space")
				OnOptionsFormattingMarkChanged();
			if (e.Name == "HiddenText")
				OnHiddenTextOptionChanged();
			if (e.Name == "ForeColorSource" || e.Name == "FontSource")
				ApplyFontAndForeColor();
			if (e.Name == "ShowLeftIndent" || e.Name == "ShowRightIndent" || e.Name == "ShowTabs")
				OnOptionsHorizontalRulerChanged();
			if (e.Name == "ParagraphTabs" || e.Name == "Sections" || e.Name == "Tables" || e.Name == "ParagraphFormatting")
				OnOptionsDocumentCapabilitiesChanged();
			if (e.Name == "EMail" || e.Name == "UserName" || e.Name == "Group")
				DocumentModelApplyChanges(DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetSecondaryLayout);
			if (e.Name == "AutoDetectDocumentCulture")
				OnSpellCheckerOptionsChanged();
			OnOptionsChangedPlatformSpecific(e);
			OnUpdateUI();
		}
		void OnSpellCheckerOptionsChanged() {
			System.Collections.Generic.List<PieceTable> pieceTables = DocumentModel.GetPieceTables(false);
			foreach (PieceTable pieceTable in pieceTables)
				DocumentModel.ResetSpellCheck(pieceTable, pieceTable.Runs.First.GetRunIndex(), pieceTable.Runs.Last.GetRunIndex(), false);
		}
		void OnHiddenTextOptionChanged() {
			DocumentModelApplyChanges(DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ResetRuler);
		}
		void OnOptionsFormattingMarkChanged() {
			DocumentModelApplyChanges(DocumentModelChangeActions.Redraw);
		}
		void OnOptionsBookmarksChanged() {
			DocumentModelApplyChanges(DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout);
		}
		void OnOptionsRangePermissionsChanged() {
			DocumentModelApplyChanges(DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout);
		}
		void OnOptionsCommentsChanged() {
			DocumentModelApplyChanges(DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout);
		}
		void OnOptionsFieldsChanged() {
			DocumentModelApplyChanges(DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ResetRuler);
		}
		void OnOptionsDocumentCapabilitiesChanged() {
			DocumentModelApplyChanges(DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetRuler);
		}
		void OnOptionsHorizontalRulerChanged() {
			DocumentModelApplyChanges(DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetRuler);
		}
		void DocumentModelApplyChanges(DocumentModelChangeActions changeActions) {
			BeginUpdate();
			try {
				DocumentModel.BeginUpdate();
				try {
					PieceTable pieceTable = DocumentModel.MainPieceTable;
					pieceTable.ApplyChangesCore(changeActions, RunIndex.Zero, RunIndex.MaxValue);
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
			finally {
				EndUpdate();
			}
		}
#endregion
		protected internal virtual void RedrawEnsureSecondaryFormattingComplete() {
			Owner.RedrawEnsureSecondaryFormattingComplete();
		}
		protected internal virtual void RedrawEnsureSecondaryFormattingComplete(RefreshAction action) {
			Owner.RedrawEnsureSecondaryFormattingComplete(action);
		}
		protected internal virtual void UpdateRulers() {
			Owner.UpdateRulers();
		}
		protected internal virtual void UpdateHorizontalRuler() {
			Owner.UpdateHorizontalRuler();
		}
		protected internal virtual void UpdateVerticalRuler() {
			Owner.UpdateVerticalRuler();
		}
		protected internal virtual void OnResize() {
			Owner.OnResize();
		}
#region ActiveView
		protected internal virtual RichEditViewRepository CreateViewRepository() {
			return Owner.CreateViewRepository();
		}
		protected internal virtual void CreateViews() {
			this.views = CreateViewRepository();
			if (activeViewTypeBeforeActiveViewCreation != null)
				SetActiveViewCore(views.GetViewByType((RichEditViewType)activeViewTypeBeforeActiveViewCreation));
			else
				SetActiveViewCore(views.GetViewByType(DefaultViewType));
		}
		protected internal virtual void DisposeViews() {
			if (views != null) {
				views.Dispose();
				views = null;
			}
		}
		protected internal virtual void SetActiveView(RichEditView newView) {
			BeginUpdate();
			try {
				SetActiveViewCore(newView);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void SetActiveViewCore(RichEditView newView) {
			Guard.ArgumentNotNull(newView, "newView");
			Rectangle viewBounds;
			if (this.activeView != null)
				viewBounds = DeactivateView(this.activeView);
			else
				viewBounds = new Rectangle(0, 0, 10, 10);
			this.activeView = newView;
			viewBounds = CalculateActualViewBounds(viewBounds);
			viewBounds.X = 0;
			viewBounds.Y = 0;
			ActivateView(this.activeView, viewBounds);
			RaiseActiveViewChanged();
			RedrawEnsureSecondaryFormattingComplete();
			ActiveView.CorrectZoomFactor();
		}
		protected internal virtual Rectangle DeactivateView(RichEditView view) {
			if (view.IsDisposed)
				return Rectangle.Empty;
			if (!DocumentModel.ActivePieceTable.IsMain) {
				DocumentModel.SetActivePieceTableCore(DocumentModel.MainPieceTable, null);
				view.OnActivePieceTableChanged();
				RaiseFinishHeaderFooterEditing();
			}
			UnsubscribeActiveViewEvents();
			DeactivateViewPlatformSpecific(view);
			view.Deactivate();
			Formatter.Dispose();
			return view.Bounds;
		}
		protected internal virtual void DeactivateViewAndClearActiveView(RichEditView view) {
			activeViewTypeBeforeActiveViewCreation = ActiveViewType;
			DeactivateView(view);
			activeView = null;
		}
		protected internal virtual void ActivateView(RichEditView view, Rectangle viewBounds) {
			Debug.Assert(viewBounds.Location == Point.Empty);
			ActiveView.CorrectZoomFactor();
			ActivateViewPlatformSpecific(view);
			InitializeBackgroundFormatter();
			Formatter.SpellCheckerController = CreateSpellCheckerController();
			view.Activate(viewBounds);
			Formatter.Start();
			SubscribeActiveViewEvents();
			if (IsHandleCreated) {
				ActiveView.EnsureCaretVisible();
				OnResizeCore();
			}
		}
		protected override CalculationModeType GetDefaultLayoutCalculationMode() {
			return CalculationModeType.Automatic;
		}
		internal override DocumentModelPosition NotifyDocumentLayoutChanged(PieceTable pieceTable, DocumentModelDeferredChanges changes, DocumentLayoutResetType documentLayoutResetType) {
			ControlDocumentChangesHandler handler = new ControlDocumentChangesHandler(ActiveView);
			return handler.NotifyDocumentChanged(pieceTable, changes, false, documentLayoutResetType);
		}
		internal override void InitializeBackgroundFormatter() {
			InitializeDocumentLayout();
			BackgroundFormatter = CreateBackgroundFormatter(ActiveView.FormattingController);
			BackgroundFormatter.PageFormattingComplete += OnPageFormattingComplete;
		}
		protected virtual BackgroundFormatter CreateBackgroundFormatter(DocumentFormattingController controller) {
			return new BackgroundFormatter(controller, Owner.CommentPadding);
		}
		protected internal virtual void SubscribeActiveViewEvents() {
			ActiveView.ZoomChanging += OnActiveViewZoomChanging;
			ActiveView.ZoomChanged += OnActiveViewZoomChanged;
			ActiveView.BackColorChanged += OnActiveViewBackColorChanged;
		}
		protected internal virtual void UnsubscribeActiveViewEvents() {
			ActiveView.ZoomChanging -= OnActiveViewZoomChanging;
			ActiveView.ZoomChanged -= OnActiveViewZoomChanged;
			ActiveView.BackColorChanged -= OnActiveViewBackColorChanged;
		}
		protected internal virtual void OnActiveViewZoomChanging(object sender, EventArgs e) {
			BeginUpdate();
			OnZoomFactorChangingPlatformSpecific();
		}
		protected internal virtual void OnActiveViewZoomChanged(object sender, EventArgs e) {
			OnResizeCore();
			UpdateVerticalScrollBar(false);
			EndUpdate();
			RaiseZoomChanged();
		}
		protected internal virtual void OnActiveViewBackColorChanged(object sender, EventArgs e) {
			Owner.OnActiveViewBackColorChanged();
		}
		protected internal virtual void ActivateViewPlatformSpecific(RichEditView view) {
			Owner.ActivateViewPlatformSpecific(view);
		}
		protected internal virtual void DeactivateViewPlatformSpecific(RichEditView view) {
			Owner.DeactivateViewPlatformSpecific(view);
		}
		protected internal bool IsHandleCreated { get { return Owner.IsHandleCreated; } }
		protected internal virtual void OnResizeCore() {
			Owner.OnResizeCore();
		}
		protected internal virtual Rectangle CalculateActualViewBounds(Rectangle previousViewBounds) {
			return Owner.CalculateActualViewBounds(previousViewBounds);
		}
#endregion
		protected internal override void OnUpdateUI() {
			if (UpdateUIOnIdle)
				ForceUpdateUIOnIdle = true;
			else
				OnUpdateUICore();
		}
		protected internal virtual void OnUpdateUICore() {
			if (IsUpdateLocked)
				deferredChanges.RaiseUpdateUI = true;
			else
				RaiseUpdateUI();
		}
		protected internal override void OnApplicationIdle() {
			base.OnApplicationIdle();
			if (forceUpdateUIOnIdle) {
				OnUpdateUICore();
				forceUpdateUIOnIdle = false;
			}
		}
		protected internal virtual void OnOptionsChangedPlatformSpecific(BaseOptionChangedEventArgs e) {
			Owner.OnOptionsChangedPlatformSpecific(e);
		}
		protected internal override void ApplyFontAndForeColor() {
			base.ApplyFontAndForeColor();
			DevExpress.XtraRichEdit.Model.CharacterProperties characterProperties = GetDefaultCharacterProperties();
			if (characterProperties == null)
				return;
			DocumentModel.History.DisableHistory();
			try {
				ApplyFontAndForeColorCore(characterProperties);
			}
			finally {
				DocumentModel.History.EnableHistory();
			}
		}
		protected internal virtual void ApplyFontAndForeColorCore(DevExpress.XtraRichEdit.Model.CharacterProperties characterProperties) {
			characterProperties.BeginUpdate();
			try {
				Font font = this.Font;
				if (!ShouldApplyForeColor() || !ShouldApplyFont())
					characterProperties.Reset();
				if (ShouldApplyForeColor())
					characterProperties.ForeColor = ForeColor;
				if (ShouldApplyFont() && font != null) {
					characterProperties.BeginUpdate();
					try {
						ApplyFont(characterProperties, font);
					}
					finally {
						characterProperties.EndUpdate();
					}
				}
			}
			finally {
				characterProperties.EndUpdate();
			}
		}
		protected internal virtual DevExpress.XtraRichEdit.Model.CharacterProperties GetDefaultCharacterProperties() {
			if (DocumentModel == null)
				return null;
			if (DocumentModel.CharacterStyles.Count <= 0)
				return null;
			return DocumentModel.CharacterStyles.DefaultItem.CharacterProperties;
		}
		protected internal virtual bool ShouldApplyForeColor() {
			return Owner.ShouldApplyForeColor();
		}
		protected internal virtual bool ShouldApplyFont() {
			return Owner.ShouldApplyFont();
		}
		protected internal virtual void ApplyFont(CharacterProperties characterProperties, Font font) {
			Owner.ApplyFont(characterProperties, font);
		}
		protected internal virtual void ApplyNewCommentPadding() {
			DocumentModel.BeginUpdate();
			try {
				ActiveView.ApplyNewCommentPadding(Owner.CommentPadding);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void OnReadOnlyChanged() {
			RaiseReadOnlyChanged();
			if (ReadOnly) {
			}
			else {
			}
			OnUpdateUI();
		}
		protected internal virtual void OnOvertypeChanged() {
			RaiseOvertypeChanged();
			OnUpdateUI();
		}
		protected internal virtual void ApplyChangesCorePlatformSpecific(DocumentModelChangeActions changeActions) {
			Owner.ApplyChangesCorePlatformSpecific(changeActions);
		}
		protected internal virtual void OnZoomFactorChangingPlatformSpecific() {
			Owner.OnZoomFactorChangingPlatformSpecific();
		}
		NativeHyperlinkCollection GetHyperlinkCollection(Field field) {
			if (NativeDocument.PieceTable == field.PieceTable)
				return (NativeHyperlinkCollection)NativeDocument.Hyperlinks;
			else if (field.PieceTable.IsTextBox) {
				DevExpress.XtraRichEdit.API.Native.ReadOnlyShapeCollection shapes = NativeDocument.Shapes.Get(NativeDocument.Range);
				foreach (Shape shape in shapes) {
					if (shape.TextBox == null)
						continue;
					NativeSubDocument textBoxDocument = shape.TextBox.Document as NativeSubDocument;
					if (textBoxDocument.PieceTable == field.PieceTable)
						return (NativeHyperlinkCollection)textBoxDocument.Hyperlinks;
				}
			}
			return new NativeHyperlinkCollection(NativeDocument);
		}
		protected internal virtual bool OnHyperlinkClick(Field field, bool allowForModifiers) {
			NativeHyperlinkCollection hyperlinks = GetHyperlinkCollection(field);
			DevExpress.XtraRichEdit.API.Native.Hyperlink hyperlink = hyperlinks.Find(field.Index);
			if (hyperlink == null)
				hyperlink = new NativeHyperlink(NativeDocument, DocumentModel.ActivePieceTable, field); 
			Debug.Assert(hyperlink != null);
			Keys modifiers = KeyboardHandler.GetModifierKeys();
			HyperlinkClickEventArgs e = new HyperlinkClickEventArgs(hyperlink, modifiers);
			RaiseHyperlinkClick(e);
			if (e.Handled)
				return true;
			if (!allowForModifiers || IsHyperlinkModifierKeysPress()) {
				OpenHyperlink(field);
				return true;
			}
			return false;
		}
		protected internal virtual bool IsHyperlinkModifierKeysPress() {
#if !SL
			return KeyboardHandler.GetModifierKeys() == Options.Hyperlinks.ModifierKeys;
#else
			return KeyboardHandler.GetModifierKeys().Value == Options.Hyperlinks.ModifierKeys.Value;
#endif
		}
		void OpenHyperlink(DevExpress.XtraRichEdit.Model.Field field) {
			FollowHyperlinkCommand command = new FollowHyperlinkCommand(Owner, field);
			command.Execute();
		}
		Point UnitsToLayoutUnits(PointF point) {
			return new Point(NativeDocument.UnitsToLayoutUnits(point.X), NativeDocument.UnitsToLayoutUnits(point.Y));
		}
		internal SizeF LayoutUnitsToUnits(Size size) {
			return new SizeF(NativeDocument.LayoutUnitsToUnits(size.Width), NativeDocument.LayoutUnitsToUnits(size.Height));
		}
		RectangleF LayoutUnitsToUnits(Rectangle bounds) {
			return new RectangleF(NativeDocument.LayoutUnitsToUnits(bounds.X), NativeDocument.LayoutUnitsToUnits(bounds.Y), NativeDocument.LayoutUnitsToUnits(bounds.Width), NativeDocument.LayoutUnitsToUnits(bounds.Height));
		}
		public ApiDocumentPosition GetPositionFromPoint(PointF clientPoint) {
			RichEditHitTestRequest request = new RichEditHitTestRequest(DocumentModel.ActivePieceTable);
			request.DetailsLevel = DocumentLayoutDetailsLevel.Character;
			request.Accuracy = HitTestAccuracy.ExactPage | HitTestAccuracy.NearestPageArea | HitTestAccuracy.NearestColumn | HitTestAccuracy.ExactTableRow | HitTestAccuracy.ExactTableCell | HitTestAccuracy.ExactRow | HitTestAccuracy.ExactBox | HitTestAccuracy.ExactCharacter;
			Point physicalPoint = UnitsToLayoutUnits(clientPoint);
			physicalPoint.X -= ViewBounds.Left;
			physicalPoint.Y -= ViewBounds.Top;
			request.PhysicalPoint = physicalPoint;
			RichEditHitTestResult result = ActiveView.HitTestCore(request, true);
			if (!result.IsValid(DocumentLayoutDetailsLevel.Character))
				return null;
			else {
				DocumentLogPosition logPosition = result.Character.GetFirstPosition(DocumentModel.ActivePieceTable).LogPosition;
				return NativeDocument.CreatePositionCore(logPosition);
			}
		}
		public RectangleF GetBoundsFromPosition(ApiDocumentPosition pos) {
			Rectangle physicalBounds = GetLayoutPhysicalBoundsFromPositionCore(pos);
			if (physicalBounds == Rectangle.Empty)
				return physicalBounds;
			physicalBounds.X += ViewBounds.Left;
			physicalBounds.Y += ViewBounds.Top;
			return LayoutUnitsToUnits(physicalBounds);
		}
		public Rectangle GetLayoutPhysicalBoundsFromPosition(ApiDocumentPosition pos) {
			Rectangle physicalBounds = GetLayoutPhysicalBoundsFromPositionCore(pos);
			physicalBounds.X += ActiveView.HorizontalScrollController.GetPhysicalLeftInvisibleWidth();
			return physicalBounds;
		}
		protected internal virtual Rectangle GetLayoutPhysicalBoundsFromPositionCore(ApiDocumentPosition pos) {
			PageViewInfo pageViewInfo;
			Rectangle logicalBounds = GetLayoutLogicalBoundsFromPositionCore(pos, out pageViewInfo);
			if (pageViewInfo == null)
				return Rectangle.Empty;
			Rectangle physicalBounds = ActiveView.CreatePhysicalRectangle(pageViewInfo, logicalBounds);
			return physicalBounds;
		}
		public Rectangle GetLayoutLogicalBoundsFromPosition(ApiDocumentPosition pos) {
			PageViewInfo pageViewInfo;
			return GetLayoutLogicalBoundsFromPositionCore(pos, out pageViewInfo);
		}
		protected internal virtual Rectangle GetLayoutLogicalBoundsFromPositionCore(ApiDocumentPosition pos, out PageViewInfo pageViewInfo) {
			pageViewInfo = null;
			NativeDocumentPosition nativePosition = (NativeDocumentPosition)pos;
			DocumentLayoutPosition layoutPosition = ActiveView.DocumentLayout.CreateLayoutPosition(nativePosition.Position.PieceTable, nativePosition.LogPosition, 0);
			if (!layoutPosition.Update(ActiveView.DocumentLayout.Pages, DocumentLayoutDetailsLevel.Character))
				return Rectangle.Empty;
			pageViewInfo = ActiveView.LookupPageViewInfoByPage(layoutPosition.Page);
			if (pageViewInfo == null)
				return Rectangle.Empty;
			return layoutPosition.Character.Bounds;
		}
		public virtual RichEditCommand CreateCommand(RichEditCommandId commandId) {
			IRichEditCommandFactoryService service = GetService<IRichEditCommandFactoryService>();
			if (service == null)
				return null;
			return service.CreateCommand(commandId);
		}
		public virtual RichEditCommand CreatePasteDataObjectCommand(IDataObject dataObject) {
			return new DevExpress.XtraRichEdit.Commands.Internal.PasteDataObjectCoreCommand(Owner, dataObject);
		}
		public void ScrollToCaret(float relativeVerticalPosition) {
			BeginUpdate();
			try {
				EnsureCaretVisibleVerticallyCommand scrollVertically = new EnsureCaretVisibleVerticallyCommand(Owner);
				scrollVertically.RelativeCaretPosition = relativeVerticalPosition;
				scrollVertically.Execute();
				EnsureCaretVisibleHorizontallyCommand scrollHorizontally = new EnsureCaretVisibleHorizontallyCommand(Owner);
				scrollHorizontally.Execute();
			}
			finally {
				EndUpdate();
			}
		}
		public void Undo() {
			UndoCommand command = new UndoCommand(Owner);
			command.Execute();
		}
		public void Redo() {
			RedoCommand command = new RedoCommand(Owner);
			command.Execute();
		}
		public void ClearUndo() {
			ClearUndoCommand command = new ClearUndoCommand(Owner);
			command.Execute();
		}
		public void Cut() {
			CutSelectionCommand command = new CutSelectionCommand(Owner);
			command.Execute();
		}
		public void Copy() {
			CopySelectionCommand command = new CopySelectionCommand(Owner);
			command.Execute();
		}
		public void Paste() {
			PasteSelectionCommand command = new PasteSelectionCommand(Owner);
			command.Execute();
		}
		public void SelectAll() {
			SelectAllCommand command = new SelectAllCommand(Owner);
			command.Execute();
		}
		public void DeselectAll() {
			DeselectAllCommand command = new DeselectAllCommand(Owner);
			command.Execute();
		}
		protected internal virtual void UpdateVerticalScrollBar(bool avoidJump) {
			if (avoidJump)
				ActiveView.VerticalScrollController.ScrollBarAdapter.SynchronizeScrollBarAvoidJump();
			else
				ActiveView.VerticalScrollController.ScrollBarAdapter.EnsureSynchronized();
		}
		protected internal virtual void BeginDocumentRendering() {
			if (ActiveView != null)
				ActiveView.BeginDocumentRendering();
		}
		protected internal virtual void EndDocumentRendering() {
			if (ActiveView != null)
				ActiveView.EndDocumentRendering();
		}
		protected internal override void SetLayoutUnitCore(DocumentLayoutUnit unit) {
			base.SetLayoutUnitCore(unit);
			if (ActiveView == null)
				return;
			Owner.ResizeView(true); 
			UpdateHorizontalRuler();
			UpdateVerticalRuler();
			UpdateVerticalScrollBar(false);
			ActiveView.UpdateHorizontalScrollbar();
			Owner.Redraw();
		}
		protected internal override void SetDocumentModelLayoutUnitCore(DocumentLayoutUnit unit) {
			if (ActiveView != null)
				ActiveView.OnLayoutUnitChanging();
			base.SetDocumentModelLayoutUnitCore(unit);
			RaiseLayoutUnitChanged();
			if (ActiveView != null)
				ActiveView.OnLayoutUnitChanged();
		}
		protected internal virtual void BeginScrollbarUpdate(IOfficeScrollbar scrollbar) {
			if (scrollbar != null)
				scrollbar.BeginUpdate();
		}
		protected internal virtual void EndScrollbarUpdate(IOfficeScrollbar scrollbar) {
			if (scrollbar != null)
				scrollbar.EndUpdate();
		}
#region Load/Save content
		public virtual void LoadDocument() {
			LoadDocument(Owner);
		}
		public virtual void LoadDocument(IWin32Window parent) {
			LoadDocumentCore(parent);
		}
		protected internal virtual void LoadDocumentCore(IWin32Window parent) {
			IDocumentImportManagerService importManagerService = GetService<IDocumentImportManagerService>();
			DocumentImportHelper importHelper = new DocumentImportHelper(DocumentModel);
			ImportSource<DocumentFormat, bool> importSource = importHelper.InvokeImportDialog(parent, importManagerService);
			if (importSource == null)
				return;
#if (!SL)
			Cursor.Current = RichEditCursors.WaitCursor.Cursor;
#else
			string previousFileName = Options.DocumentSaveOptions.CurrentFileName;
			if (!Options.DocumentSaveOptions.CanSaveToCurrentFileName)
				Options.DocumentSaveOptions.CurrentFileName = importSource.FileName;
			try {
#endif
			LoadDocument(importSource.Storage, importSource.Importer.Format);
#if (!SL)
			Cursor.Current = RichEditCursors.Default.Cursor;
#else
			}
			catch {
				if (!Options.DocumentSaveOptions.CanSaveToCurrentFileName)
					Options.DocumentSaveOptions.CurrentFileName = previousFileName;
				throw;
			}
#endif
		}
		public virtual bool SaveDocumentAs() {
			return SaveDocumentAs(Owner);
		}
		public virtual bool SaveDocumentAs(IWin32Window parent) {
			return ExportDocumentCore(parent, null);
		}
		protected bool ExportDocumentCore(IWin32Window parent, ExportersCalculator<DocumentFormat, bool> calc) {
			return ExportDocumentCore(parent, calc, DocumentModel.DocumentSaveOptions);
		}
		internal bool ExportDocumentCore(IWin32Window parent, ExportersCalculator<DocumentFormat, bool> calc, IDocumentSaveOptions<DocumentFormat> options) {
			IDocumentExportManagerService exportManagerService = GetService<IDocumentExportManagerService>();
			DocumentExportHelper exportHelper = new DocumentExportHelper(DocumentModel);
			ExportTarget<DocumentFormat, bool> target = exportHelper.InvokeExportDialog(parent, exportManagerService, calc, options);
			if (target == null)
				return false;
#if (!SL)
			Cursor oldCursor = Cursor.Current;
#endif
			try {
#if (!SL)
				Cursor.Current = RichEditCursors.WaitCursor.Cursor;
#endif
				SaveDocument(target.Storage, target.Exporter.Format, options);
			}
			finally {
#if (!SL)
				Cursor.Current = oldCursor;
#else
				if (target.Storage != null)
					target.Storage.Close();
#endif
			}
			return true;
		}
		public virtual bool SaveDocument() {
			return SaveDocument(Owner);
		}
		public virtual bool SaveDocument(IWin32Window parent) {
			DocumentSaveOptions documentSaveOptions = DocumentModel.DocumentSaveOptions;
			if (!documentSaveOptions.CanSaveToCurrentFileName || String.IsNullOrEmpty(documentSaveOptions.CurrentFileName) || documentSaveOptions.CurrentFormat == DocumentFormat.Undefined)
				return SaveDocumentAs(parent);
			else {
				SaveDocument(documentSaveOptions.CurrentFileName, documentSaveOptions.CurrentFormat);
				return true;
			}
		}
#endregion
		public void AssignShortcutKeyToCommand(Keys key, Keys modifier, RichEditCommandId commandId) {
			AssignShortcutKeyToCommand(key, modifier, commandId, RichEditViewType.Draft);
			AssignShortcutKeyToCommand(key, modifier, commandId, RichEditViewType.Simple);
			AssignShortcutKeyToCommand(key, modifier, commandId, RichEditViewType.PrintLayout);
		}
		public void AssignShortcutKeyToCommand(Keys key, Keys modifier, RichEditCommandId commandId, RichEditViewType viewType) {
			NormalKeyboardHandler handler = KeyboardHandler as NormalKeyboardHandler;
			if (handler == null)
				return;
			RichEditKeyHashProvider provider = new RichEditKeyHashProvider(viewType);
			handler.UnregisterKeyHandler(provider, key, modifier);
			handler.RegisterKeyHandler(provider, key, modifier, commandId);
		}
		public void RemoveShortcutKey(Keys key, Keys modifier) {
			RemoveShortcutKey(key, modifier, RichEditViewType.Draft);
			RemoveShortcutKey(key, modifier, RichEditViewType.Simple);
			RemoveShortcutKey(key, modifier, RichEditViewType.PrintLayout);
		}
		public void RemoveShortcutKey(Keys key, Keys modifier, RichEditViewType viewType) {
			NormalKeyboardHandler handler = KeyboardHandler as NormalKeyboardHandler;
			if (handler == null)
				return;
			handler.UnregisterKeyHandler(new RichEditKeyHashProvider(viewType), key, modifier);
		}
		protected internal virtual void AddServices() {
			AddService(typeof(IThreadSyncService), ThreadSyncService.Create());
			AddService(typeof(IKeyboardHandlerService), new RichEditKeyboardHandlerService(this));
			AddService(typeof(IMouseHandlerService), new RichEditMouseHandlerService(this));
			AddService(typeof(IAutoCorrectService), CreateAutoCorrectService);
		}
		protected internal virtual object CreateAutoCorrectService(IServiceContainer container, Type serviceType) {
			return new AutoCorrectService(this);
		}
		protected internal virtual long GetVerticalScrollValue() {
			return ActiveView.VerticalScrollController.ScrollBarAdapter.Value;
		}
		protected internal virtual void SetVerticalScrollValue(long value) {
			ActiveView.VerticalScrollController.ScrollToAbsolutePosition(value);
			ActiveView.OnVerticalScroll();
		}
		protected internal virtual long GetVerticalScrollPosition() {
			return ActiveView.VerticalScrollController.ScrollBarAdapter.Value;
		}
		protected internal virtual void SetVerticalScrollPosition(long value) {
			long previousValue = GetVerticalScrollPosition();
			ScrollVerticallyByPhysicalOffsetEnsurePageGenerationCommand command = new ScrollVerticallyByPhysicalOffsetEnsurePageGenerationCommand(Owner);
			command.PhysicalOffset = (int)(value - previousValue);
			command.Execute();
		}
#region IGestureStateIndicator members
		void IGestureStateIndicator.OnGestureBegin() {
			gestureActivated = true;
		}
		void IGestureStateIndicator.OnGestureEnd() {
			gestureActivated = false;
		}
		bool IGestureStateIndicator.GestureActivated { get { return gestureActivated; } }
#endregion
	}
	public interface IBoxMeasurerProvider {
		BoxMeasurer Measurer { get; }
	}
	public class ExplicitBoxMeasurerProvider : IBoxMeasurerProvider {
		readonly BoxMeasurer measurer;
		public ExplicitBoxMeasurerProvider(BoxMeasurer measurer) {
			Guard.ArgumentNotNull(measurer, "measurer");
			this.measurer = measurer;
		}
		public BoxMeasurer Measurer { get { return measurer; } }
	}
	public interface IInnerRichEditDocumentServerOwner {
		MeasurementAndDrawingStrategy CreateMeasurementAndDrawingStrategy(DocumentModel documentModel);
		RichEditControlOptionsBase CreateOptions(InnerRichEditDocumentServer documentServer);
		void RaiseDeferredEvents(DocumentModelChangeActions changeActions);
		DocumentFormatsDependencies DocumentFormatsDependencies { get; }
	}
	internal interface IRichEditDocumentLayoutProvider {
		DocumentLayout GetDocumentLayout();
		DocumentLayout GetDocumentLayoutAsync();
		void PerformPageSecondaryFormatting(Page page);
		DevExpress.XtraRichEdit.API.Native.Document Document { get; }
		CalculationModeType LayoutCalculationMode { get; }
		event DevExpress.XtraRichEdit.API.Layout.DocumentLayoutInvalidatedEventHandler DocumentLayoutInvalidated;
		event DevExpress.XtraRichEdit.API.Layout.PageFormattedEventHandler PageFormatted;
		event EventHandler DocumentFormatted;
	}
	public interface IInnerRichEditControlOwner : IInnerRichEditDocumentServerOwner, IRichEditControl, IWin32Window {
		void ActivateViewPlatformSpecific(RichEditView view);
		void DeactivateViewPlatformSpecific(RichEditView view);
		void RedrawEnsureSecondaryFormattingComplete();
		void OnResizeCore();
		Rectangle CalculateActualViewBounds(Rectangle previousViewBounds);
		void OnOptionsChangedPlatformSpecific(BaseOptionChangedEventArgs e);
		bool Focused { get; }
		bool IsHandleCreated { get; }
		Font Font { get; }
		Color ForeColor { get; }
		bool Enabled { get; }
		bool ShouldApplyForeColor();
		bool ShouldApplyFont();
		void ApplyFont(CharacterProperties characterProperties, Font font);
		RichEditMouseHandler CreateMouseHandler();
		RichEditViewRepository CreateViewRepository();
		IOfficeScrollbar CreateVerticalScrollBar();
		IOfficeScrollbar CreateHorizontalScrollBar();
		IRulerControl CreateVerticalRuler();
		IRulerControl CreateHorizontalRuler();
		void UpdateRulers();
		void UpdateHorizontalRuler();
		void UpdateVerticalRuler();
		void OnResize();
		void Redraw();
		void RedrawAfterEndUpdate();
		void Redraw(RefreshAction action);
		void ApplyChangesCorePlatformSpecific(DocumentModelChangeActions changeActions);
		void OnZoomFactorChangingPlatformSpecific();
		void OnActiveViewBackColorChanged();
		void ResizeView(bool ensureCaretVisibleonResize);
		CommentPadding CommentPadding { get; }
	}
}
namespace DevExpress.XtraRichEdit.Drawing {
	public interface IDrawingSurface {
	}
#region MeasurementAndDrawingStrategy (abstract class)
	public abstract class MeasurementAndDrawingStrategy : IDisposable {
#region Fields
		bool isDisposed;
		readonly DocumentModel documentModel;
		BoxMeasurer measurer;
#endregion
		protected MeasurementAndDrawingStrategy(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		public virtual void Initialize() {
			this.measurer = CreateBoxMeasurer();
		}
#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public BoxMeasurer Measurer { get { return measurer; } }
		public bool IsDisposed { get { return isDisposed; } }
#endregion
#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (measurer != null) {
					measurer.Dispose();
					measurer = null;
				}
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
#endregion
		protected internal virtual void OnLayoutUnitChanged() {
			Measurer.OnLayoutUnitChanged();
		}
		public abstract BoxMeasurer CreateBoxMeasurer();
		public abstract Painter CreateDocumentPainter(IDrawingSurface surface);
		public abstract FontCacheManager CreateFontCacheManager();
	}
#endregion
}
