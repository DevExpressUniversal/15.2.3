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
using System.Drawing;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Services;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Internal;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using DevExpress.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Mouse {
	public abstract class DragCaretVisualizer {
		public virtual void Start() {
		}
		public virtual void Finish() {
		}
		public abstract void ShowCaret(DocumentLogPosition caretLogPosition);
		public abstract void HideCaret(DocumentLogPosition caretLogPosition);
	}
	#region CancellableDragMouseHandlerStateBase (abstract class)
	public abstract class CancellableDragMouseHandlerStateBase : RichEditMouseHandlerState, IKeyboardHandlerService {
		IKeyboardHandlerService previousKeyboardService;
		DataObject dataObject;
		protected CancellableDragMouseHandlerStateBase(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		#region Properties
		public override bool AutoScrollEnabled { get { return true; } }
		public override bool CanShowToolTip { get { return true; } }
		public override bool StopClickTimerOnStart { get { return false; } }
		protected internal abstract DocumentLayoutDetailsLevel HitTestDetailsLevel { get; }
		protected internal virtual bool HandleDragDropManually { get { return true; } }
		protected internal DataObject DataObject { get { return dataObject; } }
		#endregion
		public override void Start() {
			base.Start();
			this.previousKeyboardService = Control.InnerControl.GetService<IKeyboardHandlerService>();
			Control.RemoveService(typeof(IKeyboardHandlerService));
			Control.AddService(typeof(IKeyboardHandlerService), this);
			this.dataObject = CreateDataObject();
			BeginVisualFeedback();
		}
		public override void Finish() {
			EndVisualFeedback();
			Control.RemoveService(typeof(IKeyboardHandlerService));
			Control.AddService(typeof(IKeyboardHandlerService), previousKeyboardService);
			base.Finish();
		}
		#region IKeyboardHandlerService Members
		public void OnKeyDown(KeyEventArgs e) {
			Keys key = e.KeyCode;
			if (key.Equals(Keys.Escape)) {
				HideVisualFeedback();
				MouseHandler.SwitchToDefaultState();
				MouseHandler.State.OnMouseMove(MouseHandler.CreateFakeMouseMoveEventArgs());
			}
			if (key.Equals(Keys.Control))
				SetMouseCursor(CalculateMouseCursor());
		}
		public void OnKeyPress(KeyPressEventArgs e) {
		}
		public void OnKeyUp(KeyEventArgs e) {
			SetMouseCursor(CalculateMouseCursor());
		}
		#endregion
		protected internal virtual RichEditHitTestResult CalculateHitTest(Point point) {
			RichEditHitTestRequest request = new RichEditHitTestRequest(DocumentModel.ActivePieceTable);
			request.PhysicalPoint = point;
			request.DetailsLevel = HitTestDetailsLevel;
			request.Accuracy = Control.InnerControl.ActiveView.DefaultHitTestPageAccuracy | HitTestAccuracy.NearestPageArea | HitTestAccuracy.NearestColumn | HitTestAccuracy.NearestTableRow | HitTestAccuracy.NearestTableCell | HitTestAccuracy.NearestRow | HitTestAccuracy.NearestBox | HitTestAccuracy.NearestCharacter;
			RichEditHitTestResult result = Control.InnerControl.ActiveView.HitTestCore(request, true);
			if (!result.IsValid(HitTestDetailsLevel))
				return null;
			else
				return result;
		}
		public override void OnMouseUp(MouseEventArgs e) {
			if (!HandleDragDropManually)
				return;
			HideVisualFeedback();
			CommitDrag(new Point(e.X, e.Y), null); 
			MouseHandler.SwitchToDefaultState();
		}
		public override void OnMouseMove(MouseEventArgs e) {
			if (HandleDragDropManually)
				ContinueDrag(new Point(e.X, e.Y), DragDropEffects.Move | DragDropEffects.Copy, DataObject);
		}
		public override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			ContinueDrag(new Point(e.X, e.Y), DragDropEffects.Move | DragDropEffects.Copy, null);
		}
		protected internal virtual void BeginVisualFeedback() {
		}
		protected internal virtual void EndVisualFeedback() {
		}
		protected internal abstract RichEditCursor CalculateMouseCursor();
		protected internal abstract DataObject CreateDataObject();
		protected internal abstract bool CommitDrag(Point point, IDataObject dataObject);
		protected internal abstract DragDropEffects ContinueDrag(Point point, DragDropEffects allowedEffects, IDataObject dataObject);
		protected internal abstract void ShowVisualFeedback();
		protected internal abstract void HideVisualFeedback();
	}
	#endregion
	#region DragContentMouseHandlerStateBase
	public class DragContentMouseHandlerStateBase : CancellableDragMouseHandlerStateBase {
		#region Fields
		public const string RichEditDataFormatSelection = "XtraRichEditSelection";
		DocumentLogPosition caretLogPosition;
		readonly DragCaretVisualizer caretVisualizer;
		readonly DragContentMouseHandlerStateBaseStrategy platformStrategy;
		readonly DragContentMouseHandlerStateCalculator calculator;
		#endregion
		public DragContentMouseHandlerStateBase(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
			this.platformStrategy = CreatePlatformStrategy();
			this.caretVisualizer = CreateCaretVisualizer();
			this.calculator = mouseHandler.CreateDragContentMouseHandlerStateCalculator();
		}
		#region Properties
		public DocumentLogPosition CaretLogPosition { get { return caretLogPosition; } set { caretLogPosition = value; } }
		protected internal override DocumentLayoutDetailsLevel HitTestDetailsLevel { get { return DocumentLayoutDetailsLevel.Character; } }
		protected DragCaretVisualizer CaretVisualizer { get { return caretVisualizer; } }
		protected DragContentMouseHandlerStateCalculator Calculator { get { return calculator; } }
		#endregion
		protected DragContentMouseHandlerStateBaseStrategy CreatePlatformStrategy() {
			return MouseHandler.GetPlatformStrategyFactory().CreateDragContentMouseHandlerStateBaseStrategy(this);
		}
		public override void Start() {
			base.Start();
			this.caretLogPosition = new DocumentLogPosition(-1);
		}
		public override void Finish() {
			base.Finish();
			this.platformStrategy.Finish();
		}
		protected internal virtual bool CanDropContentTo(RichEditHitTestResult hitTestResult) {
			return calculator.CanDropContentTo(hitTestResult, DocumentModel.ActivePieceTable);
		}
		protected internal virtual DocumentModelPosition UpdateModelPosition(DocumentModelPosition pos) {
			return calculator.UpdateDocumentModelPosition(pos);
		}
		protected internal virtual void UpdateVisualState() {
			calculator.UpdateVisualState();
		}
		protected internal override DragDropEffects ContinueDrag(Point point, DragDropEffects allowedEffects, IDataObject dataObject) {
			RichEditHitTestResult hitTestResult = CalculateHitTest(point);
			if (!CanDropData(dataObject) || hitTestResult == null || !CanDropContentTo(hitTestResult)) {
				HideVisualFeedback();
				SetMouseCursor(RichEditCursors.GetCursor(DragDropEffects.None));
				return DragDropEffects.None;
			}
			DragDropEffects dragDropEffect = allowedEffects & CalculateDragDropEffects();
			DocumentModelPosition pos = GetHitTestDocumentModelPosition(hitTestResult);
			bool shouldShowVisualFeedback = ShouldShowVisualFeedback(hitTestResult.LogicalPoint);
			if (pos.LogPosition == CaretLogPosition) {
				if (shouldShowVisualFeedback)
					ShowVisualFeedback();
				else
					HideVisualFeedback();
				return dragDropEffect;
			}
			HideVisualFeedback();
			this.caretLogPosition = pos.LogPosition;
			UpdateVisualState();
			if (shouldShowVisualFeedback)
				ShowVisualFeedback();
			SetMouseCursor(RichEditCursors.GetCursor(dragDropEffect));
			return dragDropEffect;
		}
		protected virtual bool ShouldShowVisualFeedback(Point point) {
			return true;
		}
		protected internal override void BeginVisualFeedback() {
			this.caretVisualizer.Start();
		}
		protected internal override void EndVisualFeedback() {
			this.caretVisualizer.Finish();
		}
		protected internal override void ShowVisualFeedback() {
			this.caretVisualizer.ShowCaret(CaretLogPosition);
		}
		protected internal override void HideVisualFeedback() {
			this.caretVisualizer.HideCaret(CaretLogPosition);
		}
		protected internal override RichEditCursor CalculateMouseCursor() {
			return RichEditCursors.GetCursor(CalculateDragDropEffects());
		}
		protected internal virtual DragDropEffects CalculateDragDropEffects() {
			if (KeyboardHandler.IsControlPressed)
				return DragDropEffects.Copy;
			else
				return DragDropEffects.Move;
		}
		protected internal virtual DocumentModelPosition GetHitTestDocumentModelPosition(RichEditHitTestResult hitTestResult) {
			DocumentModelPosition result = hitTestResult.Character != null ? hitTestResult.Character.GetFirstPosition(hitTestResult.PieceTable)
				: hitTestResult.Box.GetFirstPosition(hitTestResult.PieceTable);
			return UpdateModelPosition(result);
		}
		protected internal override bool CommitDrag(Point point, IDataObject dataObject) {
			RichEditHitTestResult hitTestResult = CalculateHitTest(point);
			if (hitTestResult == null || !CanDropContentTo(hitTestResult))
				return false;
			HideVisualFeedback();
			DocumentModelPosition pos = GetHitTestDocumentModelPosition(hitTestResult);
			Command command = CreateDropCommand(pos, dataObject);
			command.Execute();
			return true;
		}
		protected internal virtual Command CreateDropCommand(DocumentModelPosition pos, IDataObject dataObject) {
			if (dataObject == null) {
				if (KeyboardHandler.IsControlPressed)
					return new DragCopyContentCommand(Control, pos);
				else
					return new DragMoveContentCommand(Control, pos);
			}
			else
				return new DragCopyExternalContentCommand(Control, pos, dataObject);
		}
		protected internal virtual bool CanDropData(IDataObject dataObject) {
			if (dataObject == null)
				return false;
			RichEditCommand command = Control.InnerControl.CreatePasteDataObjectCommand(dataObject);
			return command.CanExecute();
		}
		protected internal override DataObject CreateDataObject() {
			CopySelectionManager manager = new CopySelectionManager(Control.InnerDocumentServer);
			Selection selection = DocumentModel.Selection;
			DataObject dataObject = new DataObject();
			DevExpress.XtraRichEdit.Export.RtfDocumentExporterOptions options = new DevExpress.XtraRichEdit.Export.RtfDocumentExporterOptions();
			options.ExportFinalParagraphMark = DevExpress.XtraRichEdit.Export.Rtf.ExportFinalParagraphMark.Never;
			dataObject.SetData(OfficeDataFormats.Rtf, manager.GetRtfText(selection.PieceTable, selection.GetSortedSelectionCollection(), options, true, true));
			dataObject.SetData(OfficeDataFormats.UnicodeText, manager.GetPlainText(selection.PieceTable, selection.GetSortedSelectionCollection()));
			dataObject.SetData(RichEditDataFormatSelection, Control.GetHashCode().ToString());
			string data = manager.GetSuppressStoreImageSizeCollection(selection.PieceTable, selection.GetSortedSelectionCollection());
			dataObject.SetData(OfficeDataFormats.SuppressStoreImageSize, data);
			return dataObject;
		}
		protected internal virtual DragCaretVisualizer CreateCaretVisualizer() {
			return platformStrategy.CreateCaretVisualizer();
		}
	}
	#endregion
	public class DragContentMouseHandlerStateCalculator {
		readonly IInnerRichEditControlOwner owner;
		internal DragContentMouseHandlerStateCalculator(IInnerRichEditControlOwner owner) {
			this.owner = owner;
		}
		public DragContentMouseHandlerStateCalculator() : this(null) { }
		public virtual bool CanDropContentTo(RichEditHitTestResult hitTestResult, PieceTable pieceTable) {
			if (hitTestResult.DetailsLevel < DocumentLayoutDetailsLevel.Box)
				return false;
			if (Object.ReferenceEquals(hitTestResult.PageArea.PieceTable, pieceTable))
				return true;
			if (!pieceTable.ContentType.IsTextBox || hitTestResult.FloatingObjectBox == null)
				return false;
			FloatingObjectAnchorRun run = hitTestResult.FloatingObjectBox.GetRun(hitTestResult.FloatingObjectBox.PieceTable) as FloatingObjectAnchorRun;
			if (run == null)
				return false;
			TextBoxFloatingObjectContent content = run.Content as TextBoxFloatingObjectContent;
			if (content == null)
				return false;
			return Object.ReferenceEquals(content.TextBox.PieceTable, pieceTable);
		}
		public virtual DocumentModelPosition UpdateDocumentModelPosition(DocumentModelPosition pos) {
			return pos;
		}
		public virtual void UpdateVisualState() {
			RichEditMouseHandler richEditMouseHandler = owner.InnerControl.MouseHandler as RichEditMouseHandler;
			bool isTableViewInfoInitialized = richEditMouseHandler != null && richEditMouseHandler.TableViewInfo != null;
			bool visibleWhileDragging = owner != null && owner.InnerControl != null && owner.InnerControl.DocumentModel.TableOptions.GridLines == RichEditTableGridLinesVisibility.VisibleWhileDragging;
			if (isTableViewInfoInitialized && visibleWhileDragging)
				owner.Redraw();
		}
		public virtual void OnInternalDragStart() {
		}
	}
	#region DragContentManuallyMouseHandlerState
	public class DragContentManuallyMouseHandlerState : DragContentMouseHandlerStateBase {
		public DragContentManuallyMouseHandlerState(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public override void Start() {
			base.Start();
			Calculator.OnInternalDragStart();
		}
	}
	#endregion
	#region DragContentStandardMouseHandlerStateBase (abstract class)
	public abstract class DragContentStandardMouseHandlerStateBase : DragContentMouseHandlerStateBase {
		protected DragContentStandardMouseHandlerStateBase(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected internal override bool HandleDragDropManually { get { return false; } }
		public override void OnDragOver(DragEventArgs e) {
			e.Effect = ContinueDrag(new Point(e.X, e.Y), e.AllowedEffect, e.Data);
		}
		public override void OnDragDrop(DragEventArgs e) {
			base.OnDragDrop(e);
			OnDragDropCore(e);
			MouseHandler.SwitchToDefaultState();
		}
		protected virtual void OnDragDropCore(DragEventArgs e) {
			if (Control.InnerControl.Options.Behavior.DropAllowed)
				CommitDrag(new Point(e.X, e.Y), GetDataObject(e));
		}
		public override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			bool isCommandDisabled = !Control.InnerControl.Options.Behavior.DragAllowed;
			if (e.EscapePressed || isCommandDisabled) {
				e.Action = DragAction.Cancel;
				MouseHandler.SwitchToDefaultState();
			}
		}
		protected internal override void SetMouseCursor(RichEditCursor cursor) {
		}
		protected internal abstract IDataObject GetDataObject(DragEventArgs e);
	}
	#endregion
	#region DragContentStandardMouseHandlerState
	public class DragContentStandardMouseHandlerState : DragContentStandardMouseHandlerStateBase {
		bool dragToExternalTarget = true;
		public DragContentStandardMouseHandlerState(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public override void Start() {
			base.Start();
			Calculator.OnInternalDragStart();
			Control.DoDragDrop(DataObject, DragDropEffects.Move | DragDropEffects.Copy);
		}
		public override void OnDragEnter(DragEventArgs e) {
			dragToExternalTarget = false;
			e.Effect = ContinueDrag(new Point(e.X, e.Y), e.AllowedEffect, e.Data);
		}
		public override void OnDragLeave() {
			dragToExternalTarget = true;
			base.OnDragLeave();
			MouseHandler.SwitchToDefaultState();
		}
		public override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			base.OnQueryContinueDrag(e);
			if (e.Action == DragAction.Drop && dragToExternalTarget)
				MouseHandler.SwitchToDefaultState();
		}
		protected internal override IDataObject GetDataObject(DragEventArgs e) {
			return null; 
		}
	}
	#endregion
	#region DragExternalContentMouseHandlerState
	public class DragExternalContentMouseHandlerState : DragContentStandardMouseHandlerStateBase {
		public DragExternalContentMouseHandlerState(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public override void OnDragOver(DragEventArgs e) {
			base.OnDragOver(e);
			Point physicalPoint = new Point(e.X, e.Y);
			RichEditHitTestResult hitTestResult = CalculateNearestCharacterHitTest(physicalPoint, ActivePieceTable);
			if (hitTestResult != null && hitTestResult.TableCell != null)
				MouseHandler.TableViewInfo = hitTestResult.TableCell.TableViewInfo;
			if (UseHover) {
				UpdateHover(hitTestResult);
				UpdateTableViewInfoController(physicalPoint, hitTestResult);
			}
		}
		protected internal virtual void UpdateTableViewInfoController(Point physicalPoint, RichEditHitTestResult hitTestResult) {
			TableViewInfoController controller = Control.InnerControl.ActiveView.TableController;
			if (controller != null)
				controller.Update(physicalPoint, hitTestResult);
		}
		public override void OnDragLeave() {
			MouseHandler.SwitchToDefaultState();
			MouseHandler.TableViewInfo = null;
		}
		protected internal override DragDropEffects CalculateDragDropEffects() {
			return DragDropEffects.Copy;
		}
		protected internal override IDataObject GetDataObject(DragEventArgs e) {
			return e.Data;
		}
#if !SL
		protected internal override DataObject CreateDataObject() {
			return null;
		}
#endif
		protected internal override DragDropEffects ContinueDrag(Point point, DragDropEffects allowedEffects, IDataObject dataObject) {
			DragDropEffects result = base.ContinueDrag(point, allowedEffects, dataObject);
			if (result == DragDropEffects.None) {
				PasteLoadDocumentFromFileCommand command = new PasteLoadDocumentFromFileCommand(Control);
				command.PasteSource = new DataObjectPasteSource(dataObject);
				if (command.CanExecute())
					result = CalculateDragDropEffects();
			}
			return result;
		}
		protected internal override bool CommitDrag(Point point, IDataObject dataObject) {
			bool result = base.CommitDrag(point, dataObject);
			if (!result) {
				PasteLoadDocumentFromFileCommand command = new PasteLoadDocumentFromFileCommand(Control);
				command.PasteSource = new DataObjectPasteSource(dataObject);
				command.Execute();
			}
			return result;
		}
	}
	#endregion
	#region DragFloatingObjectManuallyMouseHandlerState
	public class DragFloatingObjectManuallyMouseHandlerState : CancellableDragMouseHandlerStateBase {
		#region Fields
		static readonly Point Unassigned = new Point(Int32.MinValue, Int32.MinValue);
		Point currentTopLeftCorner;
		PageViewInfo pageViewInfo;
		Point initialLogicalClickPoint;
		OfficeImage image;
		Point clickPointLogicalOffset;
		RichEditHitTestResult currentHitTestResult;
		FloatingObjectAnchorRun run;
		RunIndex floatingObjectAnchorRunIndex;
		Point oldTopLeftCorner;
		Rectangle initialShapeBounds;
		Rectangle initialContentBounds;
		RunIndex minAffectedRunIndex;
		readonly DragFloatingObjectManuallyMouseHandlerStateStrategy platformStrategy;
		float rotationAngle;
		readonly DragFloatingObjectMouseHandlerStateCalculator calculator;
		#endregion
		public DragFloatingObjectManuallyMouseHandlerState(RichEditMouseHandler mouseHandler, RichEditHitTestResult hitTestResult)
			: base(mouseHandler) {
			FloatingObjectBox box = hitTestResult.FloatingObjectBox;
			Debug.Assert(box != null);
			this.platformStrategy = CreatePlatformStrategy();
			this.initialLogicalClickPoint = hitTestResult.LogicalPoint;
			this.oldTopLeftCorner = box.Bounds.Location;
			this.clickPointLogicalOffset = new Point(oldTopLeftCorner.X - initialLogicalClickPoint.X, oldTopLeftCorner.Y - initialLogicalClickPoint.Y);
			this.initialShapeBounds = box.Bounds;
			this.initialContentBounds = box.ContentBounds;
			this.minAffectedRunIndex = CalculateMinAffectedRunIndex(hitTestResult.Page);
			this.calculator = mouseHandler.CreateDragFloatingObjectMouseHandlerStateCalculator();
			this.rotationAngle = DocumentModel.GetBoxEffectiveRotationAngleInDegrees(box);
		}
		#region Properties
		protected internal override DocumentLayoutDetailsLevel HitTestDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		public OfficeImage FeedbackImage { get { return image; } }
		public Rectangle InitialShapeBounds { get { return initialShapeBounds; } }
		public Rectangle InitialContentBounds { get { return initialContentBounds; } }
		public FloatingObjectAnchorRun Run { get { return run; } }
		public float RotationAngle { get { return rotationAngle; } }
		#endregion
		protected internal virtual DragFloatingObjectManuallyMouseHandlerStateStrategy CreatePlatformStrategy() {
			return MouseHandler.GetPlatformStrategyFactory().CreateDragFloatingObjectManuallyMouseHandlerStateStrategy(this);
		}
		public override void Start() {
			if (Control.InnerControl.IsEditable) {
				currentTopLeftCorner = Unassigned;
				Selection selection = this.DocumentModel.Selection;
				Debug.Assert(selection.Length == 1);
				RunInfo runInfo = selection.PieceTable.FindRunInfo(selection.NormalizedStart, selection.Length);
				this.floatingObjectAnchorRunIndex = runInfo.Start.RunIndex;
				TextRunBase selectionRun = selection.PieceTable.Runs[floatingObjectAnchorRunIndex];
				this.run = (FloatingObjectAnchorRun)selectionRun;
				PictureFloatingObjectContent content = run.Content as PictureFloatingObjectContent;
				if (content != null)
					image = CreateFeedbackImage(content.Image);
				else
					image = CreateFeedbackImage(null); 
				calculator.Init(run, this.clickPointLogicalOffset);
			}
			base.Start();
		}
		protected internal virtual OfficeImage CreateFeedbackImage(OfficeImage originalImage) {
			return platformStrategy.CreateFeedbackImage(originalImage);
		}
		protected internal override RichEditCursor CalculateMouseCursor() {
			return RichEditCursors.Default;
		}
		protected internal override DataObject CreateDataObject() {
			return null;
		}
		bool CalculateHitTestAndCurrentPoint(Point point) {
			RichEditHitTestRequest request = new RichEditHitTestRequest(DocumentModel.ActivePieceTable);
			request.PhysicalPoint = point;
			request.Accuracy = HitTestAccuracy.NearestPage | HitTestAccuracy.NearestPageArea | HitTestAccuracy.NearestColumn | HitTestAccuracy.NearestRow | HitTestAccuracy.NearestBox | HitTestAccuracy.NearestTableRow | HitTestAccuracy.NearestTableCell | HitTestAccuracy.NearestCharacter;
			request.DetailsLevel = DocumentLayoutDetailsLevel.Column;
			pageViewInfo = Control.InnerControl.ActiveView.GetPageViewInfoFromPoint(point, false);
			currentHitTestResult = Control.InnerControl.ActiveView.HitTestCore(request, false);
			if (currentHitTestResult == null)
				return false;
			currentTopLeftCorner = currentHitTestResult.LogicalPoint;
			currentTopLeftCorner.X += clickPointLogicalOffset.X;
			currentTopLeftCorner.Y += clickPointLogicalOffset.Y;
			return true;
		}
		protected internal RunIndex CalculateMinAffectedRunIndex(Page page) {
			Selection selection = this.DocumentModel.Selection;
			Debug.Assert(selection.Length == 1);
			RunInfo runInfo = selection.PieceTable.FindRunInfo(selection.NormalizedStart, selection.Length);
			this.run = selection.PieceTable.Runs[runInfo.Start.RunIndex] as FloatingObjectAnchorRun;
			Paragraph paragraph = selection.PieceTable.Runs[runInfo.Start.RunIndex].Paragraph;
			PieceTable pieceTable = run.PieceTable;
			if (this.run == null && paragraph.GetMergedFrameProperties() != null && !paragraph.IsInCell())
				pieceTable = selection.PieceTable;
			RichEditHitTestRequest request = new RichEditHitTestRequest(pieceTable);
			request.LogicalPoint = this.oldTopLeftCorner;
			request.DetailsLevel = DocumentLayoutDetailsLevel.Row;
			request.Accuracy = HitTestAccuracy.NearestPageArea | HitTestAccuracy.NearestColumn | HitTestAccuracy.NearestRow;
			DevExpress.XtraRichEdit.Layout.Engine.PageController pageController = Control.InnerControl.Formatter.DocumentFormatter.Controller.PageController;
			RichEditHitTestResult result = new RichEditHitTestResult(pageController.DocumentLayout, pieceTable);
			result.Page = page;
			result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Page);
			BoxHitTestCalculator calculator = pageController.CreateHitTestCalculator(request, result);
			calculator.CalcHitTest(page);
			if (result.Row != null)
				return result.Row.GetFirstPosition(pieceTable).RunIndex;
			else
				return RunIndex.Zero;
		}
		protected internal override bool CommitDrag(Point point, IDataObject dataObject) {
			if (!this.run.PieceTable.CanEditSelection() || !Control.InnerControl.IsEditable)
				return false;
			if (!CalculateHitTestAndCurrentPoint(point))
				return false;
			if (!calculator.CanDropTo(currentHitTestResult))
				return false;
			HideVisualFeedback();
			Control.BeginUpdate();
			try {
				float zoomFactor = Control.InnerControl.ActiveView.ZoomFactor;
				Point topLeftPhysicalPoint = point;
				topLeftPhysicalPoint.X += (int)Math.Round(clickPointLogicalOffset.X * zoomFactor);
				topLeftPhysicalPoint.Y += (int)Math.Round(clickPointLogicalOffset.Y * zoomFactor);
				FloatingObjectLayoutModifier modifier = new FloatingObjectLayoutModifier(Control, run, floatingObjectAnchorRunIndex);
				modifier.OldTopLeftCorner = this.oldTopLeftCorner;
				modifier.CurrentTopLeftCorner = this.currentTopLeftCorner;
				modifier.MinAffectedRunIndex = this.minAffectedRunIndex;
				modifier.Commit(topLeftPhysicalPoint);
				run.Select();
			}
			finally {
				Control.EndUpdate();
			}
			return true;
		}
		protected internal override DragDropEffects ContinueDrag(Point point, DragDropEffects allowedEffects, IDataObject dataObject) {
			if (!CalculateHitTestAndCurrentPoint(point))
				return DragDropEffects.None;
			if (!calculator.CanDropTo(currentHitTestResult))
				return DragDropEffects.None;
			HideVisualFeedback();
			ShowVisualFeedback();
			return allowedEffects & DragDropEffects.Move;
		}
		protected internal virtual Matrix CreateVisualFeedbackTransform() {
			if (currentTopLeftCorner == Unassigned || pageViewInfo == null  || currentHitTestResult == null)
				return null;
			return FloatingObjectBox.CreateTransformUnsafe(rotationAngle, new Rectangle(currentTopLeftCorner, initialShapeBounds.Size));
		}
		protected internal override void ShowVisualFeedback() {
			if (currentTopLeftCorner == Unassigned || pageViewInfo == null  || currentHitTestResult == null)
				return;
			ShowVisualFeedbackCore(new Rectangle(currentTopLeftCorner, initialShapeBounds.Size), pageViewInfo, image);
		}
		protected internal override void HideVisualFeedback() {
			if (currentTopLeftCorner == Unassigned || pageViewInfo == null  || currentHitTestResult == null)
				return;
			HideVisualFeedbackCore(Rectangle.Empty, pageViewInfo);
		}
		protected internal virtual void ShowVisualFeedbackCore(Rectangle bounds, PageViewInfo pageViewInfo, OfficeImage image) {
			platformStrategy.ShowVisualFeedbackCore(bounds, pageViewInfo, image);
		}
		protected internal virtual void HideVisualFeedbackCore(Rectangle bounds, PageViewInfo pageViewInfo) {
			platformStrategy.HideVisualFeedbackCore(bounds, pageViewInfo);
		}
		protected internal override void BeginVisualFeedback() {
			platformStrategy.BeginVisualFeedback();
		}
		protected internal override void EndVisualFeedback() {
			platformStrategy.EndVisualFeedback();
		}
	}
	#endregion
	#region DragFloatingObjectMouseHandlerStateCalculator
	public class DragFloatingObjectMouseHandlerStateCalculator {
		FloatingObjectAnchorRun anchor;
		Point logicalPointOffset;
		public virtual bool CanDropTo(RichEditHitTestResult point) {
			return true;
		}
		protected FloatingObjectAnchorRun Anchor { get { return anchor; } }
		protected Point LogicalPointOffset { get { return logicalPointOffset; } }
		protected internal virtual void Init(FloatingObjectAnchorRun anchor, Point offset) {
			this.anchor = anchor;
			this.logicalPointOffset = offset;
		}
	}
	#endregion
	#region FloatingObjectLayoutModifier
	public class FloatingObjectLayoutModifier {
		#region Fields
		readonly IRichEditControl control;
		readonly FloatingObjectAnchorRun run;
		readonly RunIndex floatingObjectAnchorRunIndex;
		PageViewInfo pageViewInfo;
		RichEditHitTestResult currentHitTestResult;
		Point currentTopLeftCorner; 
		Point oldTopLeftCorner; 
		RunIndex minAffectedRunIndex;
		#endregion
		public FloatingObjectLayoutModifier(IRichEditControl control, FloatingObjectAnchorRun run, RunIndex floatingObjectAnchorRunIndex) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(run, "run");
			this.control = control;
			this.run = run;
			this.floatingObjectAnchorRunIndex = floatingObjectAnchorRunIndex;
		}
		#region Properties
		public IRichEditControl Control { get { return control; } }
		public DocumentModel DocumentModel { get { return run.DocumentModel; } }
		public PieceTable ActivePieceTable { get { return DocumentModel.ActivePieceTable; } }
		public Point CurrentTopLeftCorner { get { return currentTopLeftCorner; } set { currentTopLeftCorner = value; } }
		public Point OldTopLeftCorner { get { return oldTopLeftCorner; } set { oldTopLeftCorner = value; } }
		public RunIndex MinAffectedRunIndex { get { return minAffectedRunIndex; } set { minAffectedRunIndex = value; } }
		public FloatingObjectAnchorRun AnchorRun { get { return run; } }
		#endregion
		public void Commit(Point physicalPoint) {
			PerformDocumentLayout(true);
			DocumentModel.BeginUpdate();
			try {
				PerformDocumentLayoutCore(false);
				CommitCore(physicalPoint);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected virtual void CommitCore(Point physicalPoint) {
			DocumentLogPosition newLogPosition = CalculateLogPosition(physicalPoint, DocumentLayoutDetailsLevel.Row); 
			if (!CommitCore(newLogPosition)) {
				if (newLogPosition < run.PieceTable.DocumentEndLogPosition)
					CommitCore(newLogPosition + 1);
			}
		}
		bool CommitCore(DocumentLogPosition logPosition) {
			DocumentLogPosition oldPosition = run.PieceTable.GetRunLogPosition(run);
			if (ActivePieceTable.Runs[ActivePieceTable.FindRunInfo(logPosition, 1).Start.RunIndex] is FieldResultEndRun) {
				return false;
			}
			DocumentModel.BeforeFloatingObjectDrop(oldPosition, logPosition, run.PieceTable);
			FloatingObjectAnchorRunMovedHistoryItem item = CreateChangeParagraphIndexAndRunIndexHistoryItem(logPosition);
			if (item == null)
				return ApplyInsideParagraphDrag(logPosition);
			else
				return ApplyInterParagraphDrag(item, logPosition);
		}
		bool ApplyInsideParagraphDrag(DocumentLogPosition logPosition) {
			DocumentModelUnitToLayoutUnitConverter unitConverter = DocumentModel.ToDocumentLayoutUnitConverter;
			if (!ResetHorizontalPositionAlignment())
				run.FloatingObjectProperties.OffsetX += unitConverter.ToModelUnits(currentTopLeftCorner.X - oldTopLeftCorner.X);
			if (!ResetVerticalPositionAlignment(logPosition))
				run.FloatingObjectProperties.OffsetY += unitConverter.ToModelUnits(currentTopLeftCorner.Y - oldTopLeftCorner.Y);
			ParagraphIndex firstAffectedParagraphIndex = Algorithms.Max(ParagraphIndex.Zero, run.Paragraph.Index - 1);
			Paragraph firstAffectedParagraph = run.PieceTable.Paragraphs[firstAffectedParagraphIndex];
			ActivePieceTable.ApplyChangesCore(run.FloatingObjectProperties.GetBatchUpdateChangeActions(), firstAffectedParagraph.FirstRunIndex, run.Paragraph.LastRunIndex);
			return true;
		}
		bool ApplyInterParagraphDrag(FloatingObjectAnchorRunMovedHistoryItem item, DocumentLogPosition logPosition) {
			if (currentHitTestResult == null || !currentHitTestResult.IsValid(DocumentLayoutDetailsLevel.Row))
				return false;
			DocumentLayoutPosition documentLayoutPosition = Control.InnerControl.ActiveView.DocumentLayout.CreateLayoutPosition(ActivePieceTable, logPosition, currentHitTestResult.Page.PageIndex);
			documentLayoutPosition.Update(Control.InnerControl.ActiveView.DocumentLayout.Pages, DocumentLayoutDetailsLevel.Row);
			Debug.Assert(documentLayoutPosition.IsValid(DocumentLayoutDetailsLevel.Row));
			if (currentHitTestResult.Page != documentLayoutPosition.Page)
				return false; 
			bool horizontalAlignmentReset = ResetHorizontalPositionAlignment();
			bool verticalAlignmentReset = ResetVerticalPositionAlignment(logPosition);
			ApplyChangeParagraphIndexAndRunIndexHistoryItem(item);
			TryJoinRunsAfterFloatingObjectAnchorRunMoved(item);
			if (!horizontalAlignmentReset) {
				FloatingObjectTargetPlacementInfo placementInfo = CreateFloatingObjectTargetPlacementInfo(currentHitTestResult, currentHitTestResult.LogicalPoint.X, 0);
				FloatingObjectHorizontalPositionCalculator calculator = new FloatingObjectHorizontalPositionCalculator(DocumentModel.ToDocumentLayoutUnitConverter);
				run.FloatingObjectProperties.OffsetX = calculator.CalculateFloatingObjectOffsetX(run.FloatingObjectProperties.HorizontalPositionType, currentHitTestResult.LogicalPoint.X, placementInfo);
			}
			if (!verticalAlignmentReset) {
				FloatingObjectTargetPlacementInfo placementInfo = CreateFloatingObjectTargetPlacementInfo(currentHitTestResult, 0, documentLayoutPosition.Row.Bounds.Y);
				FloatingObjectVerticalPositionCalculator calculator = new FloatingObjectVerticalPositionCalculator(DocumentModel.ToDocumentLayoutUnitConverter);
				run.FloatingObjectProperties.OffsetY = calculator.CalculateFloatingObjectOffsetY(run.FloatingObjectProperties.VerticalPositionType, currentHitTestResult.LogicalPoint.Y, placementInfo);
			}
			return true;
		}
		void TryJoinRunsAfterFloatingObjectAnchorRunMoved(FloatingObjectAnchorRunMovedHistoryItem item) {
			if (item.RunIndex <= RunIndex.Zero)
				return;
			RunIndex firstRunIndex = item.RunIndex - 1;
			RunIndex lastRunIndex = item.RunIndex;
			TextRunCollection runs = run.PieceTable.Runs;
			if (runs[firstRunIndex].CanJoinWith(runs[lastRunIndex]))
				run.PieceTable.JoinTextRuns(item.ParagraphIndex, firstRunIndex);
		}
		void PerformDocumentLayout(bool excludeFloatingObjectRunFromLayout) {
			DocumentModel.BeginUpdate();
			try {
				PerformDocumentLayoutCore(excludeFloatingObjectRunFromLayout);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void PerformDocumentLayoutCore(bool excludeFloatingObjectRunFromLayout) {
			run.ExcludeFromLayout = excludeFloatingObjectRunFromLayout;
			ActivePieceTable.ApplyChangesCore(run.FloatingObjectProperties.GetBatchUpdateChangeActions(), run.Paragraph.FirstRunIndex, run.Paragraph.LastRunIndex);
		}
		FloatingObjectAnchorRunMovedHistoryItem CreateChangeParagraphIndexAndRunIndexHistoryItem(DocumentLogPosition newRunLogPosition) {
			DocumentLogPosition oldRunLogPosition = DocumentModelPosition.FromRunStart(run.PieceTable, floatingObjectAnchorRunIndex).LogPosition;
			if (oldRunLogPosition == newRunLogPosition)
				return null;
			FloatingObjectAnchorRunMovedHistoryItem item = new FloatingObjectAnchorRunMovedHistoryItem(run.PieceTable);
			DocumentModelPosition newDocumentModelPosition = PositionConverter.ToDocumentModelPosition(run.PieceTable, newRunLogPosition);
			if (newDocumentModelPosition.RunOffset > 0) {
				newDocumentModelPosition.PieceTable.SplitTextRun(newDocumentModelPosition.ParagraphIndex, newDocumentModelPosition.RunIndex, newDocumentModelPosition.RunOffset);
				newDocumentModelPosition.RunIndex++;
				newDocumentModelPosition.RunStartLogPosition = newRunLogPosition;
			}
			DocumentModelPosition oldDocumentModelPosition = PositionConverter.ToDocumentModelPosition(run.PieceTable, oldRunLogPosition);
			item.RunIndex = oldDocumentModelPosition.RunIndex;
			item.ParagraphIndex = oldDocumentModelPosition.ParagraphIndex;
			item.NewRunIndex = newDocumentModelPosition.RunIndex;
			item.NewParagraphIndex = newDocumentModelPosition.ParagraphIndex;
			return item;
		}
		DocumentLogPosition CalculateLogPosition(Point point, DocumentLayoutDetailsLevel documentLayoutDetailsLevel) {
			RichEditHitTestRequest request = new RichEditHitTestRequest(DocumentModel.ActivePieceTable);
			request.PhysicalPoint = point;
			request.Accuracy = HitTestAccuracy.NearestPage | HitTestAccuracy.NearestPageArea | HitTestAccuracy.NearestColumn | HitTestAccuracy.NearestRow | HitTestAccuracy.NearestBox | HitTestAccuracy.NearestTableRow | HitTestAccuracy.NearestTableCell | HitTestAccuracy.NearestCharacter;
			request.DetailsLevel = documentLayoutDetailsLevel;
			request.IgnoreInvalidAreas = true;
			this.pageViewInfo = Control.InnerControl.ActiveView.GetPageViewInfoFromPoint(point, false);
			this.currentHitTestResult = Control.InnerControl.ActiveView.HitTestCore(request, false);
			if (currentHitTestResult == null || !currentHitTestResult.IsValid(DocumentLayoutDetailsLevel.Row))
				return DocumentLogPosition.Zero;
			PieceTable pieceTable = run.PieceTable;
			Page page = this.pageViewInfo.Page;
			ParagraphIndex lastParagraphIndex = page.GetLastPosition(pieceTable).ParagraphIndex;
			DocumentModelPosition rowStartPosition = this.currentHitTestResult.Row.GetFirstPosition(pieceTable);
			ParagraphIndex paragraphIndex = rowStartPosition.ParagraphIndex;
			do {
				DocumentLogPosition paragraphStartPosition = pieceTable.Paragraphs[paragraphIndex].LogPosition;
				if (IsPageContainsLogPosition(page, pieceTable, paragraphStartPosition))
					return paragraphStartPosition;
				paragraphIndex++;
			} while (paragraphIndex <= lastParagraphIndex);
			DocumentModelPosition oldRunPosition = DocumentModelPosition.FromRunStart(run.PieceTable, floatingObjectAnchorRunIndex);
			if (IsPageContainsLogPosition(page, pieceTable, oldRunPosition.LogPosition) && oldRunPosition <= rowStartPosition)
				return oldRunPosition.LogPosition;
			return CalculatePageStartLogPosition(page, pieceTable);
		}
		DocumentLogPosition CalculatePageStartLogPosition(Page page, PieceTable pieceTable) {
			int pageIndex = page.PageIndex;
			if (pageIndex == 0 || !pieceTable.IsMain)
				return DocumentLogPosition.Zero;
			PageCollection pages = Control.InnerControl.ActiveView.DocumentLayout.Pages;
			Page prevPage = pages[pageIndex - 1];
			return prevPage.GetLastPosition(pieceTable).LogPosition + 1;
		}
		bool IsPageContainsLogPosition(Page page, PieceTable pieceTable, DocumentLogPosition logPosition) {
			PageCollection pages = Control.InnerControl.ActiveView.DocumentLayout.Pages;
			int index = Algorithms.BinarySearch(pages, new BoxAndLogPositionComparable<Page>(pieceTable, logPosition));
			if (index < 0)
				index = ~index;
			if (index == pages.Count)
				index--;
			return Object.ReferenceEquals(page, pages[index]);
		}
		void ApplyChangeParagraphIndexAndRunIndexHistoryItem(FloatingObjectAnchorRunMovedHistoryItem item) {
			DocumentModel.History.Add(item);
			item.Execute();
			DocumentModelChangeActions changeActions = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout;
			RunIndex startRunIndex = Algorithms.Min(minAffectedRunIndex, Algorithms.Min(item.RunIndex, item.NewRunIndex)); 
			RunIndex endRunIndex = Algorithms.Max(item.RunIndex, item.NewRunIndex); 
			ActivePieceTable.ApplyChangesCore(changeActions, startRunIndex, endRunIndex + 1);
			run.FloatingObjectProperties.LayoutInTableCell = ActivePieceTable.Paragraphs[item.NewParagraphIndex].IsInCell();
		}
		protected internal virtual bool ResetHorizontalPositionAlignment() {
			FloatingObjectProperties floatingObjectProperties = run.FloatingObjectProperties;
			if (floatingObjectProperties.HorizontalPositionAlignment != FloatingObjectHorizontalPositionAlignment.None) {
				SetHorizontalPositionRelatedToColumn(floatingObjectProperties);
				return true;
			}
			else
				return false;
		}
		protected internal virtual bool ResetVerticalPositionAlignment(DocumentLogPosition logPosition) {
			FloatingObjectProperties floatingObjectProperties = run.FloatingObjectProperties;
			if (floatingObjectProperties.VerticalPositionAlignment != FloatingObjectVerticalPositionAlignment.None) {
				SetVerticalPositionRelatedToParagraph(floatingObjectProperties, logPosition);
				return true;
			}
			else
				return false;
		}
		void SetHorizontalPositionRelatedToColumn(FloatingObjectProperties floatingObjectProperties) {
			floatingObjectProperties.HorizontalPositionAlignment = FloatingObjectHorizontalPositionAlignment.None;
			floatingObjectProperties.HorizontalPositionType = FloatingObjectHorizontalPositionType.Column;
			if (currentHitTestResult.IsValid(DocumentLayoutDetailsLevel.Column)) {
				if (currentHitTestResult.IsValid(DocumentLayoutDetailsLevel.TableCell) && currentHitTestResult.TableCell != null)
					floatingObjectProperties.OffsetX = DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(currentTopLeftCorner.X - currentHitTestResult.TableCell.GetBounds().X);
				else
					floatingObjectProperties.OffsetX = DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(currentTopLeftCorner.X - currentHitTestResult.Column.Bounds.X);
			}
			else
				floatingObjectProperties.OffsetX = 0;
		}
		void SetVerticalPositionRelatedToParagraph(FloatingObjectProperties floatingObjectProperties, DocumentLogPosition logPosition) {
			floatingObjectProperties.VerticalPositionAlignment = FloatingObjectVerticalPositionAlignment.None;
			floatingObjectProperties.VerticalPositionType = FloatingObjectVerticalPositionType.Paragraph;
			if (currentHitTestResult == null || !currentHitTestResult.IsValid(DocumentLayoutDetailsLevel.Row))
				return;
			DocumentLayoutPosition documentLayoutPosition = Control.InnerControl.ActiveView.DocumentLayout.CreateLayoutPosition(ActivePieceTable, logPosition, currentHitTestResult.Page.PageIndex);
			documentLayoutPosition.Update(Control.InnerControl.ActiveView.DocumentLayout.Pages, DocumentLayoutDetailsLevel.Row);
			Debug.Assert(documentLayoutPosition.IsValid(DocumentLayoutDetailsLevel.Row));
			if (currentHitTestResult.Page != documentLayoutPosition.Page)
				return; 
			FloatingObjectVerticalPositionCalculator calculator = new FloatingObjectVerticalPositionCalculator(DocumentModel.ToDocumentLayoutUnitConverter);
			FloatingObjectTargetPlacementInfo placementInfo = CreateFloatingObjectTargetPlacementInfo(currentHitTestResult, 0, documentLayoutPosition.Row.Bounds.Y);
			run.FloatingObjectProperties.OffsetY = calculator.CalculateFloatingObjectOffsetY(run.FloatingObjectProperties.VerticalPositionType, currentHitTestResult.LogicalPoint.Y, placementInfo);
		}
		internal FloatingObjectTargetPlacementInfo CreateFloatingObjectTargetPlacementInfo(RichEditHitTestResult hitTestResult, int originX, int originY) {
			Debug.Assert(hitTestResult.IsValid(DocumentLayoutDetailsLevel.Column));
			FloatingObjectTargetPlacementInfo result = new FloatingObjectTargetPlacementInfo();
			if (hitTestResult.IsValid(DocumentLayoutDetailsLevel.TableCell) && hitTestResult.TableCell != null) {
				Rectangle cellBounds = hitTestResult.TableCell.GetBounds();
				result.PageBounds = cellBounds;
				result.PageClientBounds = cellBounds;
				result.ColumnBounds = cellBounds;
			}
			else {
				result.PageBounds = hitTestResult.Page.Bounds;
				result.PageClientBounds = hitTestResult.Page.ClientBounds;
				result.ColumnBounds = hitTestResult.Column.Bounds;
			}
			result.OriginX = originX;
			result.OriginY = originY;
			return result;
		}
	}
	#endregion
}
