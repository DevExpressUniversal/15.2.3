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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Mouse {
	#region RichEditMouseHandler
	public class RichEditMouseHandler : MouseHandler {
	#region Fields
		readonly IRichEditControl control;
		readonly RichEditMouseHandlerStrategy platformStrategy;
		object activeObject;
		TableViewInfo tableViewInfo;
	#endregion
		public RichEditMouseHandler(IRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.platformStrategy = CreatePlatformStrategy();
		}
	#region Properties
		protected override bool SupportsTripleClick { get { return true; } }
		protected internal bool IsClicked { get { return ClickCount == 1; } }
		public IRichEditControl Control { get { return control; } }
		public RichEditMouseHandlerStrategy PlatformStrategy { get { return platformStrategy; } }
		public object ActiveObject { get { return activeObject; } }
		internal TableViewInfo TableViewInfo {
			get { return tableViewInfo; }
			set { tableViewInfo = value; }
		}
	#endregion
		protected internal virtual RichEditMouseHandlerStrategyFactory GetPlatformStrategyFactory() {
			return control.CreateRichEditMouseHandlerStrategyFactory();
		}
		protected virtual RichEditMouseHandlerStrategy CreatePlatformStrategy() {
			RichEditMouseHandlerStrategyFactory factory = GetPlatformStrategyFactory();
			return factory.CreateMouseHandlerStrategy(this);
		}
		protected internal virtual MouseEventArgs CreateFakeMouseMoveEventArgs() {
			return platformStrategy.CreateFakeMouseMoveEventArgs();
		}
		protected internal virtual RichEditHitTestResult CalculateHitTest(Point point) {
			return Control.InnerControl.ActiveView.CalculateNearestCharacterHitTest(point, Control.InnerControl.DocumentModel.ActivePieceTable);
		}
		protected override void CalculateAndSaveHitInfo(MouseEventArgs e) {
		}
		protected override void StartOfficeScroller(Point clientPoint) {
			platformStrategy.StartOfficeScroller(clientPoint);
		}
		protected override IOfficeScroller CreateOfficeScroller() {
			return platformStrategy.CreateOfficeScroller();
		}
		protected override MouseEventArgs ConvertMouseEventArgs(MouseEventArgs screenMouseEventArgs) {
			return platformStrategy.ConvertMouseEventArgs(screenMouseEventArgs);
		}
		protected override void HandleMouseWheel(MouseEventArgs e) {
			RichEditMouseHandlerState state = State as RichEditMouseHandlerState;
			if (state == null || !state.SuppressDefaultMouseWheelProcessing) {
				if(KeyboardHandler.IsControlPressed)
					PerformWheelZoom(e);
				else {
					BeginMouseDragHelperState beginMouseDragState = State as BeginMouseDragHelperState;
					bool suppressWheelScroll = beginMouseDragState != null && beginMouseDragState.DragState as DragContentStandardMouseHandlerState != null;
					if (!suppressWheelScroll)
						PerformWheelScroll(e);
				}
			}
			State.OnMouseWheel(e);
		}
		protected override void HandleClickTimerTick() {
			StopClickTimer();
			if (Suspended)
				return;
			State.OnLongMouseDown();
		}
	#region SwitchToDefaultState
		public override void SwitchToDefaultState() {
			ClearOutdatedSelectionItems();
			MouseHandlerState newState = CreateDefaultState();
			SwitchStateCore(newState, Point.Empty);
		}
	#endregion
		void ClearOutdatedSelectionItems() {
			Selection selection = Control.InnerControl.DocumentModel.Selection;
			selection.BeginUpdate();
			selection.ClearOutdatedItems();
			selection.EndUpdate();
		}
		protected internal virtual DefaultMouseHandlerState CreateDefaultState() {
			return new DefaultMouseHandlerState(this);
		}
		protected internal virtual RichEditRectangularObjectResizeMouseHandlerState CreateRectangularObjectResizeState(RectangularObjectHotZone hotZone, RichEditHitTestResult result) {
			return new RichEditRectangularObjectResizeMouseHandlerState(this, hotZone, result);
		}
		protected internal virtual RichEditRectangularObjectRotateMouseHandlerState CreateRectangularObjectRotateState(RectangularObjectRotationHotZone hotZone, RichEditHitTestResult result) {
			return new RichEditRectangularObjectRotateMouseHandlerState(this, hotZone, result);
		}
		protected override AutoScroller CreateAutoScroller() {			
			return new RichEditAutoScroller(this);
		}
		protected internal virtual void PerformWheelScroll(MouseEventArgs e) {
			OfficeMouseEventArgs eventArgs = OfficeMouseEventArgs.Convert(e);
			PerformWheelScroll(eventArgs);
		}
		void PerformWheelScroll(OfficeMouseEventArgs e) {
			if (e.Horizontal) {
				if (e.Delta > 0)
					SmallScrollRight(e.Delta);
				else
					SmallScrollLeft(e.Delta);
			}
			else {
				if (e.Delta > 0)
					SmallScrollUp(e.Delta);
				else
					SmallScrollDown(e.Delta);
			}
		}
		protected internal virtual void PerformWheelZoom(MouseEventArgs e) {
			Command command = CreateZoomCommand(e);
			command.Execute();
		}
		protected internal virtual Command CreateZoomCommand(MouseEventArgs e) {
			ZoomCommandBase zoomCommand;
			if (e.Delta > 0)
				zoomCommand = new ZoomInCommand(Control);
			else
				zoomCommand = new ZoomOutCommand(Control);
#if !SL && !DXPORTABLE
			float mouseWheelScrollDelta = (float)SystemInformation.MouseWheelScrollDelta;
#else
			float mouseWheelScrollDelta = 120f;
#endif
			double delta = ZoomCommandBase.DefaultZoomFactorDelta * ((double)Math.Abs(e.Delta) / (double)mouseWheelScrollDelta);
			zoomCommand.Delta = (float)Math.Round(delta, 2);
			return zoomCommand;
		}
		protected internal virtual void SmallScrollDown(int wheelDelta) {
			float scrollRate = CalculateMouseWheelScrollRate(wheelDelta); 
			SmallScrollVerticallyCore(scrollRate);
		}
		protected internal virtual void SmallScrollUp(int wheelDelta) {
			float scrollRate = CalculateMouseWheelScrollRate(wheelDelta);
			SmallScrollVerticallyCore(-scrollRate);
		}
		protected internal virtual void SmallScrollRight(int wheelDelta) {
			float scrollRate = CalculateMouseWheelScrollRate(wheelDelta);
			SmallScrollHorizontallyCore(scrollRate);
		}
		protected internal virtual void SmallScrollLeft(int wheelDelta) {
			float scrollRate = CalculateMouseWheelScrollRate(wheelDelta);
			SmallScrollHorizontallyCore(-scrollRate);
		}
		float CalculateMouseWheelScrollRate(int wheelDelta) {
#if !SL && !DXPORTABLE
			float mouseWheelScrollDelta = (float)SystemInformation.MouseWheelScrollDelta;
			float scrollLines = (float)SystemInformation.MouseWheelScrollLines;
#else
			float mouseWheelScrollDelta = 120f;
			float scrollLines = 1;
#endif
			return scrollLines * (float)Math.Abs(wheelDelta) / mouseWheelScrollDelta / 3f;
		}
		protected internal virtual void SmallScrollVerticallyCore(float scrollRate) {
			PageViewInfoGenerator generator = Control.InnerControl.ActiveView.PageViewInfoGenerator;
			int logicalVisibleHeight = (int)Math.Round(generator.ViewPortBounds.Height / generator.ZoomFactor);
			ScrollVerticallyByLogicalOffsetCommand command = new ScrollVerticallyByLogicalOffsetCommand(Control);
			int logicalOffsetInModelUnits = (int)Math.Max(1, Control.InnerControl.DocumentModel.UnitConverter.DocumentsToModelUnits(150) * Math.Abs(scrollRate));
			command.LogicalOffset = Math.Sign(scrollRate) * Math.Min(logicalVisibleHeight, Control.InnerControl.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(logicalOffsetInModelUnits));
			command.Execute();
		}
		protected internal virtual void SmallScrollHorizontallyCore(float scrollRate) {
			PageViewInfoGenerator generator = Control.InnerControl.ActiveView.PageViewInfoGenerator;
			int visibleWidth = (int)Math.Round(generator.ViewPortBounds.Width / generator.ZoomFactor);
			ScrollHorizontallyByPhysicalOffsetCommand command = new ScrollHorizontallyByPhysicalOffsetCommand(Control);
			int physicalOffsetInModelUnits = (int)Math.Max(1, Control.InnerControl.DocumentModel.UnitConverter.DocumentsToModelUnits(150) * Math.Abs(scrollRate));
			command.PhysicalOffset = Math.Sign(scrollRate) * Math.Min(visibleWidth, Control.InnerControl.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(physicalOffsetInModelUnits));
			command.Execute();
		}
		protected internal virtual DragEventArgs ConvertDragEventArgs(DragEventArgs screenDragEventArgs) {
			return platformStrategy.ConvertDragEventArgs(screenDragEventArgs);
		}
		protected internal virtual void ApplyDragEventArgs(DragEventArgs modifiedArgs, DragEventArgs originalArgs) {
			originalArgs.Effect = modifiedArgs.Effect;
		}
		Point lastDragPoint;
		public virtual void OnDragEnter(DragEventArgs e) {
			lastDragPoint = Point.Empty;
			DragEventArgs args = ConvertDragEventArgs(e);
			State.OnDragEnter(args);
			ApplyDragEventArgs(args, e);
		}
		public virtual void OnDragOver(DragEventArgs e) {
			Point pt = new Point(e.X, e.Y);
			if (pt != lastDragPoint) {
				DragEventArgs args = ConvertDragEventArgs(e);
				State.OnDragOver(args);
				ApplyDragEventArgs(args, e);
				AutoScrollerOnDragOver(pt);
				lastDragPoint = pt;
			}
		}
		protected internal virtual void AutoScrollerOnDragOver(Point pt) {
			platformStrategy.AutoScrollerOnDragOver(pt);
		}
		public virtual void OnDragDrop(DragEventArgs e) {
			lastDragPoint = Point.Empty;
			DragEventArgs args = ConvertDragEventArgs(e);
			State.OnDragDrop(args);
			ApplyDragEventArgs(args, e);
		}
		public virtual void OnDragLeave(EventArgs e) {
			lastDragPoint = Point.Empty;
			State.OnDragLeave();
		}
		public virtual void OnGiveFeedback(GiveFeedbackEventArgs e) {
			State.OnGiveFeedback(e);
		}
		public virtual void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			State.OnQueryContinueDrag(e);
		}
		protected internal virtual void ChangeActivePieceTable(PieceTable pieceTable) {
			ChangeActivePieceTable(pieceTable, null);
		}
		protected internal virtual void ChangeActivePieceTable(PieceTable pieceTable, RichEditHitTestResult hitTestResult) {
			Section section = null;
			if (pieceTable.IsHeaderFooter) {
				if (hitTestResult == null || hitTestResult.PageArea == null) {
					PageArea area = TryFindPageArea(hitTestResult, pieceTable);
					if (area == null)
						pieceTable = pieceTable.DocumentModel.MainPieceTable;
					else
						section = area.Section;
				}
				else
					section = hitTestResult.PageArea.Section;
			}
			else {
				if (pieceTable.IsTextBox && hitTestResult.PageArea != null)
					section = hitTestResult.PageArea.Section;
				else if(pieceTable.IsComment) {
					SectionIndex sectionIndex = hitTestResult.DocumentModel.FindSectionIndex(hitTestResult.CommentViewInfo.Comment.Start);
					section = hitTestResult.DocumentModel.Sections[sectionIndex];
				}
			}
			ChangeActivePieceTableCommand command = new ChangeActivePieceTableCommand(Control, pieceTable, section, 0);
			command.ActivatePieceTable(pieceTable, section);
		}
		protected PageArea TryFindPageArea(RichEditHitTestResult hitTestResult, PieceTable pieceTable) {
			if (hitTestResult == null || hitTestResult.Page == null)
				return null;
			return hitTestResult.Page.GetActiveFirstArea(pieceTable);
		}
		protected internal static bool IsInlinePictureBoxHit(RichEditHitTestResult hitTestResult) {
			if (hitTestResult.Character != null) {
				InlinePictureBox inlinePictureBox = hitTestResult.Box as InlinePictureBox;
				if (inlinePictureBox == null)
					return false;
				if(hitTestResult.FloatingObjectBox == null)
					return true;
				FloatingObjectAnchorRun anchorRun = hitTestResult.FloatingObjectBox.GetFloatingObjectRun();
				if (IsInlinePictureInsideFloatingObject(hitTestResult))
					return true;
				return anchorRun.FloatingObjectProperties.IsBehindDoc;
			}
			return false;
		}
		static bool IsInlinePictureInsideFloatingObject(RichEditHitTestResult hitTestResult) {
			if (hitTestResult.FloatingObjectBox.DocumentLayout == null)
				return false;
			Page page = hitTestResult.FloatingObjectBox.DocumentLayout.Pages.First;
			return Object.ReferenceEquals(hitTestResult.Page, page);				
		}
		protected internal bool DeactivateTextBoxPieceTableIfNeed(PieceTable pieceTable, RichEditHitTestResult hitTestResult) {
			TextBoxContentType textBoxPieceTable = pieceTable.ContentType as TextBoxContentType;
			if (textBoxPieceTable != null) {
				if (IsInlinePictureBoxHit(hitTestResult))
					return false;
				Control.BeginUpdate();
				try {
					ChangeActivePieceTable(textBoxPieceTable.AnchorRun.PieceTable, hitTestResult);
					textBoxPieceTable.AnchorRun.Select();
				}
				finally {
					Control.EndUpdate();
				}
				return true;
			}
			return false;
		}
		public override void OnMouseUp(MouseEventArgs e) {
			platformStrategy.OnMouseUp(e);
			base.OnMouseUp(e);
		}
		public virtual DragContentMouseHandlerStateBase CreateInternalDragState() {
			return platformStrategy.CreateInternalDragState();
		}
		protected virtual RichEditHitTestResult CalculateNearestPageHitTest(Point physicalPoint, bool strictHitInfoPageBounds) {
			return Control.InnerControl.ActiveView.CalculateNearestPageHitTest(physicalPoint, strictHitInfoPageBounds);
		}
		protected internal virtual DragContentMouseHandlerStateCalculator CreateDragContentMouseHandlerStateCalculator() {
			IInnerRichEditControlOwner innerRichEditControlOwner = Control != null && Control.InnerControl != null ? Control.InnerControl.Owner : null;
			return new DragContentMouseHandlerStateCalculator(innerRichEditControlOwner);
		}
		protected internal virtual DragFloatingObjectMouseHandlerStateCalculator CreateDragFloatingObjectMouseHandlerStateCalculator() {
			return new DragFloatingObjectMouseHandlerStateCalculator();
		}
		protected internal void SetActiveObject(object modelObject) {
			this.activeObject = modelObject;
		}
	}
	#endregion
}
