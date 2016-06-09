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
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Services;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Internal;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Mouse {
	#region RichEditRectangularObjectResizeMouseHandlerState
	public class RichEditRectangularObjectResizeMouseHandlerState : RichEditMouseHandlerState {
		#region Fields
		readonly RectangularObjectHotZone hotZone;
		readonly RichEditHitTestResult initialHitTestResult;
		readonly RichEditRectangularObjectResizeMouseHandlerStateStrategy platformStrategy;
		Rectangle objectActualBounds;
		PageViewInfo pageViewInfo;
		Point initialOffset;
		IKeyboardHandlerService oldKeyboardHandler;
		Point lastLogicalPoint;
		Rectangle initialBounds;
		Rectangle initialActualSizeBounds;
		float rotationAngle;
		RunIndex minAffectedRunIndex;
		#endregion
		public RichEditRectangularObjectResizeMouseHandlerState(RichEditMouseHandler mouseHandler, RectangularObjectHotZone hotZone, RichEditHitTestResult initialHitTestResult)
			: base(mouseHandler) {
			Guard.ArgumentNotNull(hotZone, "hotZone");
			Guard.ArgumentNotNull(initialHitTestResult, "initialHitTestResult");
			this.platformStrategy = CreatePlatformStrategy();
			this.hotZone = hotZone;
			this.initialHitTestResult = initialHitTestResult;
			Box box = hotZone.Box;
			this.initialBounds = box.Bounds;
			this.initialActualSizeBounds = box.ActualSizeBounds;
			this.rotationAngle = DocumentModel.GetBoxEffectiveRotationAngleInDegrees(box);
			this.minAffectedRunIndex = initialHitTestResult.Page != null ? initialHitTestResult.Page.GetFirstPosition(ActivePieceTable).RunIndex : RunIndex.Zero;
		}
		#region Properties
		public RectangularObjectHotZone HotZone { get { return hotZone; } }
		public override bool SuppressDefaultMouseWheelProcessing { get { return true; } }
		public Rectangle ObjectActualBounds { get { return objectActualBounds; } protected set { objectActualBounds = value; } }
		public Rectangle InitialActualSizeBounds { get { return initialActualSizeBounds; } protected set { initialActualSizeBounds = value; } }
		public PageViewInfo PageViewInfo { get { return pageViewInfo; } }
		public float RotationAngle { get { return rotationAngle; } protected set { rotationAngle = value; } }
		protected Point InitialOffset { get { return initialOffset; } }
		#endregion
		public virtual Rectangle CalculateBoxBounds() {
			Rectangle result = initialBounds;
			result.X += ObjectActualBounds.X - initialActualSizeBounds.X;
			result.Y += ObjectActualBounds.Y - initialActualSizeBounds.Y;
			result.Width += ObjectActualBounds.Width - initialActualSizeBounds.Width;
			result.Height += ObjectActualBounds.Height - initialActualSizeBounds.Height;
			if (initialHitTestResult.CommentViewInfo != null)
			{
				return CalculateCommentViewInfoBoxBounds(result);
			}
			return result;
		}
		Rectangle CalculateCommentViewInfoBoxBounds(Rectangle boxBounds)
		{
			CommentViewInfo commentViewInfo = initialHitTestResult.CommentViewInfo;
			return new Rectangle(new Point(boxBounds.X + commentViewInfo.ContentBounds.Left, boxBounds.Y + commentViewInfo.ContentBounds.Top), boxBounds.Size);
		}
		protected virtual RichEditRectangularObjectResizeMouseHandlerStateStrategy CreatePlatformStrategy() {
			return MouseHandler.GetPlatformStrategyFactory().CreateRichEditRectangularObjectResizeMouseHandlerStateStrategy(this);
		}
		public override void Start() {
			base.Start();
			SetActionCursor();
			this.initialOffset = CalculateInitialOffset(initialHitTestResult.LogicalPoint);
			UpdateObjectProperties(initialHitTestResult.LogicalPoint);
			Page topLevelPage = GetTopLevelPage(initialHitTestResult);
			this.pageViewInfo = Control.InnerControl.ActiveView.LookupPageViewInfoByPage(topLevelPage);
			Debug.Assert(pageViewInfo != null);
			BeginVisualFeedback(); 
			InstallKeyboardHandler();
		}
		public override void Finish() {
			EndVisualFeedback(); 
			RestoreKeyboardHandler();
			base.Finish();
		}
		protected internal virtual void SetActionCursor() {
			SetMouseCursor(RichEditCursors.Cross);
		}
		protected internal virtual void InstallKeyboardHandler() {
			this.oldKeyboardHandler = (IKeyboardHandlerService)Control.GetService(typeof(IKeyboardHandlerService));
			if (oldKeyboardHandler != null) {
				Control.RemoveService(typeof(IKeyboardHandlerService));
				Control.AddService(typeof(IKeyboardHandlerService), CreateKeyboardHandlerService(oldKeyboardHandler));
			}
		}
		protected internal virtual void RestoreKeyboardHandler() {
			if (oldKeyboardHandler != null) {
				Control.RemoveService(typeof(IKeyboardHandlerService));
				Control.AddService(typeof(IKeyboardHandlerService), oldKeyboardHandler);
			}
		}
		protected internal virtual KeyboardHandlerServiceWrapper CreateKeyboardHandlerService(IKeyboardHandlerService oldKeyboardHandler) {
			return new RichEditRectangularObjectResizeKeyboardHandlerService(this, oldKeyboardHandler);
		}
		public override void OnMouseMove(MouseEventArgs e) {
			SetActionCursor();
			UpdateLastLogicalPoint(new Point(e.X, e.Y));
			UpdateObjectProperties(lastLogicalPoint);
			ShowVisualFeedback(); 
		}
		protected internal virtual void UpdateLastLogicalPoint(Point physicalPoint) {
			PageViewInfo viewInfo = Control.InnerControl.ActiveView.GetPageViewInfoFromPoint(physicalPoint, true);
			if (viewInfo != null && pageViewInfo != null && viewInfo.Page.PageIndex == pageViewInfo.Page.PageIndex)
				this.pageViewInfo = viewInfo;
			if (initialHitTestResult.CommentViewInfo != null)
				this.lastLogicalPoint = Control.InnerControl.ActiveView.CreateLogicalPoint(initialHitTestResult.CommentViewInfo.ContentBounds, physicalPoint);
			else
			this.lastLogicalPoint = Control.InnerControl.ActiveView.CreateLogicalPoint(pageViewInfo.ClientBounds, physicalPoint);
		}
		public void Update() {
			UpdateObjectProperties(lastLogicalPoint);
			ShowVisualFeedback(); 
		}
		public override void OnMouseWheel(MouseEventArgs e) {
		}
		public override void OnMouseUp(MouseEventArgs e) {
			UpdateLastLogicalPoint(new Point(e.X, e.Y));
			UpdateObjectProperties(lastLogicalPoint);
			MouseHandler.SwitchToDefaultState();
			CommitChanges();
		}
		protected void CommitChanges() {
			TextRunBase run = HotZone.Box.GetRun(ActivePieceTable);
			if (!run.PieceTable.CanEditSelection())
				return;
			IRectangularObject rectangularObject = run.GetRectangularObject();
			if (rectangularObject == null)
				return;
			FloatingObjectAnchorRun anchorRun = run as FloatingObjectAnchorRun;
			if (anchorRun != null)
				CommitFloatingObjectChanges(anchorRun);
			else
				ApplySizeChanges(rectangularObject);
		}
		void CommitFloatingObjectLocationAndSizeChanges(Point offset) {
			TextRunBase run = HotZone.Box.GetRun(ActivePieceTable);
			FloatingObjectAnchorRun anchorRun = run as FloatingObjectAnchorRun;
			if (anchorRun == null)
				return;
			Selection selection = this.DocumentModel.Selection;
			RunInfo runInfo = selection.PieceTable.FindRunInfo(selection.NormalizedStart, selection.Length);
			RunIndex floatingObjectAnchorRunIndex = runInfo.Start.RunIndex;
			Control.BeginUpdate();
			try {
				FloatingObjectResizeLayoutModifier modifier = new FloatingObjectResizeLayoutModifier(this, Control, anchorRun, floatingObjectAnchorRunIndex);
				modifier.OldTopLeftCorner = initialActualSizeBounds.Location; 
				Point currentTopLeftCorner = modifier.OldTopLeftCorner;
				currentTopLeftCorner.X += offset.X;
				currentTopLeftCorner.Y += offset.Y;
				modifier.CurrentTopLeftCorner = currentTopLeftCorner; 
				modifier.MinAffectedRunIndex = this.minAffectedRunIndex;
				Point topLeftPhysicalPoint = Control.InnerControl.ActiveView.CreatePhysicalPoint(this.pageViewInfo, modifier.CurrentTopLeftCorner);
				modifier.Commit(topLeftPhysicalPoint);
				anchorRun.Select();
			}
			finally {
				Control.EndUpdate();
			}
		}
		protected internal virtual void CommitFloatingObjectChanges(FloatingObjectAnchorRun anchorRun) {
			Point offset = CalculateOffset();
			if (anchorRun.FloatingObjectProperties.HorizontalPositionAlignment != FloatingObjectHorizontalPositionAlignment.None)
				offset.X = 0;
			if (anchorRun.FloatingObjectProperties.VerticalPositionAlignment != FloatingObjectVerticalPositionAlignment.None)
				offset.Y = 0;
			if (offset.X != 0 || offset.Y != 0)
				CommitFloatingObjectLocationAndSizeChanges(offset);
			else
				CommitFloatingObjectSizeChanges(anchorRun);
		}
		protected internal virtual void CommitFloatingObjectSizeChanges(FloatingObjectAnchorRun anchorRun) {
			DocumentModel.BeginUpdate();
			try {
				CommitFloatingObjectSizeChangesCore(anchorRun);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void CommitFloatingObjectSizeChangesCore(FloatingObjectAnchorRun anchorRun) {
			TextBoxFloatingObjectContent textBoxContent = anchorRun.Content as TextBoxFloatingObjectContent;
			if (textBoxContent != null)
				textBoxContent.TextBoxProperties.ResizeShapeToFitText = false;
			ApplySizeChanges(anchorRun);
			ApplyRunChanges(anchorRun);
		}
		protected virtual void ApplySizeChanges(IRectangularObject rectangularObject) {
			DocumentModel.BeginUpdate();
			try {
				ApplySizeChangesCore(rectangularObject);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected virtual void ApplySizeChangesCore(IRectangularObject rectangularObject) {
			ResizeInlinePictureCommand command = Control.CreateCommand(RichEditCommandId.ResizeInlinePicture) as ResizeInlinePictureCommand;
			if (command != null) {
				command.RectangularObject = rectangularObject;
				command.ObjectActualBounds = ObjectActualBounds;
				command.Execute();
			}
		}
		protected virtual void ApplyRunChanges(FloatingObjectAnchorRun anchorRun) {
		}
		protected internal Point CalculateOffset() {
			Matrix transform = new Matrix();
			transform.RotateAt(RotationAngle, RectangleUtils.CenterPoint(initialActualSizeBounds));
			Point topLeft = transform.TransformPoint(objectActualBounds.Location);
			Point center = transform.TransformPoint(RectangleUtils.CenterPoint(objectActualBounds));
			transform = new Matrix();
			transform.RotateAt(-RotationAngle, center);
			Point newTopLeft = transform.TransformPoint(topLeft);
			return new Point(newTopLeft.X - initialActualSizeBounds.X, newTopLeft.Y - initialActualSizeBounds.Y);
		}
		protected internal virtual Point CalculateInitialOffset(Point point) {
			return hotZone.CalculateOffset(point);
		}
		protected internal virtual void UpdateObjectProperties(Point point) {
			point.Offset(initialOffset);
			Rectangle result = hotZone.CreateValidBoxBounds(point);
			if (HotZone.CanKeepAspectRatio) {
				result = ForceKeepOriginalAspectRatio(result);
			}
			this.objectActualBounds = result;
		}
		protected internal virtual void BeginVisualFeedback() {
			platformStrategy.BeginVisualFeedback();
		}
		protected internal virtual void ShowVisualFeedback() {
			platformStrategy.ShowVisualFeedback();
		}
		protected internal virtual void HideVisualFeedback() {
			platformStrategy.HideVisualFeedback();
		}
		protected internal virtual void EndVisualFeedback() {
			platformStrategy.EndVisualFeedback();
		}
		protected internal virtual Matrix CreateVisualFeedbackTransform() {
			return FloatingObjectBox.CreateTransformUnsafe(rotationAngle, initialBounds);
		}
		protected internal virtual Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds) {
			Size actualSize = InitialActualSizeBounds.Size;
			if (actualSize == bounds.Size)
				return bounds;
			return HotZone.ForceKeepOriginalAspectRatio(bounds, actualSize);
		}
		protected internal virtual Size GetOriginalSize(Rectangle bounds) {
			IRectangularObject rectangularObject = HotZone.Box.GetRun(ActivePieceTable).GetRectangularObject();
			if (rectangularObject == null)
				return bounds.Size;
			IRectangularScalableObject rectangularScalingObject = rectangularObject as IRectangularScalableObject;
			if (rectangularScalingObject != null)
				return rectangularScalingObject.OriginalSize;
			else
				return rectangularObject.ActualSize;
		}
	}
	#endregion
	#region RichEditRectangularObjectRotateMouseHandlerState
	public class RichEditRectangularObjectRotateMouseHandlerState : RichEditRectangularObjectResizeMouseHandlerState {
		float initialRotationAngle;
		public RichEditRectangularObjectRotateMouseHandlerState(RichEditMouseHandler mouseHandler, RectangularObjectHotZone hotZone, RichEditHitTestResult initialHitTestResult)
			: base(mouseHandler, hotZone, initialHitTestResult) {
			this.initialRotationAngle = RotationAngle;
		}
		protected internal float InitialRotationAngle { get { return initialRotationAngle; } set { initialRotationAngle = value; } }
		protected internal override void SetActionCursor() {
			SetMouseCursor(RichEditCursors.Rotate);
		}
		protected internal override void UpdateObjectProperties(Point point) {
			point.Offset(InitialOffset);
			ObjectActualBounds = HotZone.CreateValidBoxBounds(point); 
			RotationAngle = ObjectRotationAngleCalculator.CalculateAngle(point, ObjectActualBounds, InitialRotationAngle, HotZone.HitTestTransform);
		}
		protected override void ApplySizeChanges(IRectangularObject rectangularObject) {
		}
		protected override void ApplyRunChanges(FloatingObjectAnchorRun anchorRun) {
			anchorRun.Shape.Rotation = DocumentModel.UnitConverter.DegreeToModelUnits(RotationAngle);
		}
		float SnapAngle(float angle) {
			if ((KeyboardHandler.IsShiftPressed))
				return SnapAngle(angle, 15, 7.5f);
			else
				return SnapAngle(angle, 90, 9);
		}
		float SnapAngle(float angle, int step, float delta) {
			angle = NormalizeAngle(angle);
			for (int i = 0; i <= 360; i += step)
				angle = CalculateSnap(angle, i, delta);
			return NormalizeAngle(angle);
		}
		public static float NormalizeAngle(float angle) {
			angle %= 360f;
			if (angle < 0)
				angle += 360;
			return angle;
		}
		float CalculateSnap(float angle, float baseAngle, float delta) {
			if (baseAngle - delta <= angle && angle < baseAngle + delta)
				return baseAngle;
			else
				return angle;
		}
	}
	#endregion
	#region RichEditRectangularObjectRotateByGestureMouseHandlerState
	public class RichEditRectangularObjectRotateByGestureMouseHandlerState : RichEditRectangularObjectRotateMouseHandlerState {
		public RichEditRectangularObjectRotateByGestureMouseHandlerState(RichEditMouseHandler mouseHandler, RectangularObjectHotZone hotZone, RichEditHitTestResult initialHitTestResult)
			: base(mouseHandler, hotZone, initialHitTestResult) {
		}
		protected internal override KeyboardHandlerServiceWrapper CreateKeyboardHandlerService(IKeyboardHandlerService oldKeyboardHandler) {
			return new RichEditRectangularObjectResizeByGestureKeyboardHandlerService(this, oldKeyboardHandler);
		}
		protected internal override void UpdateObjectProperties(Point point) {
			base.UpdateObjectProperties(point);
		}
		protected internal override void UpdateLastLogicalPoint(Point physicalPoint) {
		}
		public void Rotate(float degreeDelta) {
			RotationAngle = RotationAngle + degreeDelta; 
			ShowVisualFeedback();
		}
		public void Terminate() {
			CommitChanges();
			Finish();
		}
		public override Rectangle CalculateBoxBounds() {
			Rectangle result = base.CalculateBoxBounds();
			return result;
		}
		public override void Start() {
			float oldRotationAngle = RotationAngle;
			base.Start();
			RotationAngle = oldRotationAngle; 
		}
	}
	#endregion
	#region RichEditRectangularObjectResizeKeyboardHandlerService
	public class RichEditRectangularObjectResizeKeyboardHandlerService : KeyboardHandlerServiceWrapper {
		RichEditRectangularObjectResizeMouseHandlerState owner;
		bool shiftPressed;
		public RichEditRectangularObjectResizeKeyboardHandlerService(RichEditRectangularObjectResizeMouseHandlerState owner, IKeyboardHandlerService service)
			: base(service) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
		}
		public override void OnKeyDown(KeyEventArgs e) {
			Keys key = e.KeyCode;
			if (key.Equals(Keys.Escape))
				owner.MouseHandler.SwitchToDefaultState();
			else if (key.Equals(Keys.ShiftKey) && !shiftPressed) {
				owner.Update();
				shiftPressed = true;
			}
		}
		public override void OnKeyUp(KeyEventArgs e) {
			if (e.KeyCode.Equals(Keys.ShiftKey)) {
				owner.Update();
				shiftPressed = false;
			}
		}
		public override void OnKeyPress(KeyPressEventArgs e) {
		}
	}
	#endregion
	#region RichEditRectangularObjectResizeByGestureKeyboardHandlerService
	public class RichEditRectangularObjectResizeByGestureKeyboardHandlerService : KeyboardHandlerServiceWrapper {
		RichEditRectangularObjectRotateByGestureMouseHandlerState owner;
		public RichEditRectangularObjectResizeByGestureKeyboardHandlerService(RichEditRectangularObjectRotateByGestureMouseHandlerState owner, IKeyboardHandlerService service)
			: base(service) {
			this.owner = owner;
		}
		public override void OnKeyDown(KeyEventArgs e) {
			owner.Terminate();
		}
		public override void OnKeyPress(KeyPressEventArgs e) {
			owner.Terminate();
		}
		public override void OnKeyUp(KeyEventArgs e) {
			owner.Terminate();
		}
	}
	#endregion
	#region RichEditRectangularObjectResizeByGestureMouseHandlerService
	public class RichEditRectangularObjectResizeByGestureMouseHandlerService : RichEditMouseHandlerService {
		readonly RichEditRectangularObjectRotateByGestureMouseHandlerState owner;
		public RichEditRectangularObjectResizeByGestureMouseHandlerService(InnerRichEditControl control, RichEditRectangularObjectRotateByGestureMouseHandlerState owner)
			: base(control) {
			this.owner = owner;
		}
		public override void OnMouseDown(MouseEventArgs e) {
			owner.Terminate();
		}
		public override void OnMouseUp(MouseEventArgs e) {
			owner.Terminate();
		}
		public override void OnMouseMove(MouseEventArgs e) {
		}
	}
	#endregion
	#region FloatingObjectResizeLayoutModifier
	public class FloatingObjectResizeLayoutModifier : FloatingObjectLayoutModifier {
		readonly RichEditRectangularObjectResizeMouseHandlerState state;
		public FloatingObjectResizeLayoutModifier(RichEditRectangularObjectResizeMouseHandlerState state, IRichEditControl control, FloatingObjectAnchorRun run, RunIndex floatingObjectAnchorRunIndex)
			: base(control, run, floatingObjectAnchorRunIndex) {
			Guard.ArgumentNotNull(state, "state");
			this.state = state;
		}
		protected override void CommitCore(Point physicalPoint) {
			base.CommitCore(physicalPoint);
			state.CommitFloatingObjectSizeChangesCore(AnchorRun);
		}
	}
	#endregion
}
