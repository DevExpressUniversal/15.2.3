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
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Services.Implementation;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Xpf.Controls.Internal;
using DevExpress.Xpf.Controls.Localization;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Controls {
	#region BookCommand (abstract class)
	public abstract class BookCommand : Command {
		readonly Book control;
		protected BookCommand(Book control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		protected internal Book Control { get { return control; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
		protected internal abstract BookStringId MenuCaptionStringId { get; }
		protected internal abstract BookStringId DescriptionStringId { get; }
		public override string MenuCaption { get { return BookLocalizer.GetString(MenuCaptionStringId); } }
		public override string Description { get { return BookLocalizer.GetString(DescriptionStringId); } }
	}
	#endregion
	#region BookPreviousPageCommand
	public class BookPreviousPageCommand : BookCommand {
		public BookPreviousPageCommand(Book control) : base(control) { }
		protected internal override BookStringId DescriptionStringId {
			get { return BookStringId.MenuCmd_PreviousPage; }
		}
		protected internal override BookStringId MenuCaptionStringId {
			get { return BookStringId.MenuCmd_PreviousPageDescription; }
		}
		public override void ForceExecute(ICommandUIState state) {
			Control.Animator.Start(BookAnimationType.TurnOnePageBackward);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled = Control.PageManager.IsPrevPage && !Control.EventHandler.IsUninterruptedAnimation;
			state.Visible = true;
		}
	}
	#endregion
	#region BookNextPageCommand
	public class BookNextPageCommand : BookCommand {
		public BookNextPageCommand(Book control) : base(control) { }
		protected internal override BookStringId DescriptionStringId {
			get { return BookStringId.MenuCmd_NextPage; }
		}
		protected internal override BookStringId MenuCaptionStringId {
			get { return BookStringId.MenuCmd_NextPageDescription; }
		}
		public override void ForceExecute(ICommandUIState state) {
			Control.Animator.Start(BookAnimationType.TurnOnePageForward);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled = Control.PageManager.IsNextPage && !Control.EventHandler.IsUninterruptedAnimation;
			state.Visible = true;
		}
	}
	#endregion
	#region BookPartialTurnBackwardCommand
	public class BookPartialTurnBackwardCommand : BookCommand {
		public BookPartialTurnBackwardCommand(Book control) : base(control) { }
		protected internal override BookStringId DescriptionStringId {
			get { return BookStringId.MenuCmd_PreviousPage; }
		}
		protected internal override BookStringId MenuCaptionStringId {
			get { return BookStringId.MenuCmd_PreviousPageDescription; }
		}
		public override void ForceExecute(ICommandUIState state) {
			Control.Animator.Start(BookAnimationType.PartialTurnBackward);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled = Control.PageManager.IsPrevPage && !Control.EventHandler.IsUninterruptedAnimation && Control.EventHandler.ViewState == BookViewState.Current;
			state.Visible = true;
		}
	}
	#endregion
	#region BookPartialTurnForwardCommand
	public class BookPartialTurnForwardCommand : BookCommand {
		public BookPartialTurnForwardCommand(Book control) : base(control) { }
		protected internal override BookStringId DescriptionStringId {
			get { return BookStringId.MenuCmd_NextPage; }
		}
		protected internal override BookStringId MenuCaptionStringId {
			get { return BookStringId.MenuCmd_NextPageDescription; }
		}
		public override void ForceExecute(ICommandUIState state) {
			Control.Animator.Start(BookAnimationType.PartialTurnForward);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled = Control.PageManager.IsNextPage && !Control.EventHandler.IsUninterruptedAnimation && Control.EventHandler.ViewState == BookViewState.Current;
			state.Visible = true;
		}
	}
	#endregion
	#region BookPartialReturnCommand
	public class BookPartialReturnCommand : BookCommand {
		public BookPartialReturnCommand(Book control) : base(control) { }
		protected internal override BookStringId DescriptionStringId {
			get { return BookStringId.MenuCmd_NextPage; }
		}
		protected internal override BookStringId MenuCaptionStringId {
			get { return BookStringId.MenuCmd_NextPageDescription; }
		}
		public override void ForceExecute(ICommandUIState state) {
			Control.Animator.Start(BookAnimationType.PartialReturn);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Enabled = !Control.EventHandler.IsUninterruptedAnimation && Control.EventHandler.ViewState != BookViewState.Current;
			state.Visible = true;
		}
	}
	#endregion
	#region BookKeyHashProvider
	public class BookKeyHashProvider : IKeyHashProvider {
		public BookKeyHashProvider() {
		}
		#region IKeyHashProvider Members
		public Int64 CreateHash(Int64 keyData) {
			Int64 result = keyData; 
			return result;
		}
		#endregion
	}
	#endregion
	#region NormalKeyboardHandler
	public class NormalKeyboardHandler : CommandBasedKeyboardHandler<Type> {
		Book Control { get; set; }
		public NormalKeyboardHandler(Book control) {
			Control = control;
		}
		public override Command CreateHandlerCommand(Type handlerType) {
			ConstructorInfo ci = handlerType.GetConstructor(new Type[] { typeof(Book) });
			if (ci != null)
				return (Command)ci.Invoke(new object[] { Control });
			else
				return null;
		}
		protected override IKeyHashProvider CreateKeyHashProviderFromContext() {
			return new BookKeyHashProvider();
		}
		protected override void ValidateHandlerId(Type handlerType) {
			if (!typeof(BookCommand).IsAssignableFrom(handlerType))
				throw new ArgumentException("handlerType");
		}
	}
	#endregion
	public class BookKeyboardHandlerService : KeyboardHandlerService {
		public BookKeyboardHandlerService(KeyboardHandler handler) : base(handler) { }
		public override object CreateContext() {
			return null;
		}
	}
}
namespace DevExpress.Xpf.Controls.Internal {
	public enum BookViewState { Current, Next, Prev }
	public enum BookPageLayout { PrevOdd, PrevEven, Odd, Even, NextOdd, NextEven }
	public enum BookTemplateElementType { Grid, Content, BaseForeShadow, OverlayForeShadow, BackShadow }
	public enum BookAnimationType { None, LeaveActiveArea, TurnPage, ReturnPage, TurnOnePageForward, TurnOnePageBackward, PartialTurnForward, PartialTurnBackward, PartialReturn }
	public class BookGeometryParams {
		const double shadowHeight = 10000.0;
		#region Property
		public virtual double BookWidth { get { return Book.ActualWidth; } }
		public virtual double BookHeight { get { return Book.ActualHeight; } }
		public virtual Rect OddPageRect { get { return PageManager.OddPageRect; } }
		public virtual Rect EvenPageRect { get { return PageManager.EvenPageRect; } }
		public double ShadowHeight { get { return shadowHeight; } }
		public double ShadowTop { get { return -ShadowHeight / 2; } }
		public double BaseForeShadowWidth { get { return PageManager.BaseForeShadowWidth; } }
		public double OverlayForeShadowWidth { get { return PageManager.OverlayForeShadowWidth; } }
		public double BackShadowWidth { get { return PageManager.BackShadowWidth; } }
		public Size ActiveAreaSize { get { return Book.ActiveAreaSize; } }
		public Rect BookRect { get { return new Rect(0, 0, BookWidth, BookHeight); } }
		protected internal Book Book { get; set; }
		protected internal BookPageManager PageManager { get { return Book.PageManager; } }
		#endregion
		public BookGeometryParams(Book book) { Book = book; }
	}
	public class BookEventHandler {
		#region Property
		protected internal Book Book { get; set; }
		protected internal BookViewState ViewState { get; set; }
		protected internal bool IsPartialTurn { get; set; }
		protected internal BookPageManager PageManager { get { return Book.PageManager; } }
		protected internal BookDragTracker Tracker { get { return Book.DragTracker; } }
		protected internal BookAnimator Animator { get { return Book.Animator; } }
		protected internal BookActiveAreaHelper ActiveAreaHelper { get; set; }
		protected internal bool IsMouseTrackedEverywhere { get; set; }
		protected internal bool IsUninterruptedAnimation { get { return Animator.IsUninterruptedAnimation; } }
		#endregion
		public BookEventHandler(Book book) {
			Book = book;
			ActiveAreaHelper = new BookActiveAreaHelper(book);
			ViewState = BookViewState.Current;
			IsMouseTrackedEverywhere = false;
		}
		public void OnMouseMove(Point point) {
			if(IsUninterruptedAnimation) {
				ActiveAreaHelper.Reset();
				return;
			}
			if(IsMouseTrackedEverywhere) {
				TrackPointWithoutAnimation(point);
				return;
			}
			ActiveAreaHelper.Track(point);
			if(ActiveAreaHelper.IsInsideAnyActiveArea && CheckAndChangeViewState(point)) {
				TrackPointWithoutAnimation(point);
				return;
			}
			if(ActiveAreaHelper.IsLeaveActiveArea)
				OnLeaveActiveArea();
		}
		public void OnMouseLeave() {
			ActiveAreaHelper.Reset();
			if(!IsUninterruptedAnimation && !IsMouseTrackedEverywhere && Book.ActualWidth != 0 && Book.ActualHeight != 0)
				OnLeaveActiveArea();
		}
		public void OnMouseLeftButtonDown(Point point) {
			if(IsUninterruptedAnimation)
				return;
			IsMouseTrackedEverywhere = CheckAndChangeViewState(point);
			if(IsMouseTrackedEverywhere) {
				Book.CaptureMouse();
				TrackPointWithoutAnimation(point);
			}
		}
		public void OnMouseLeftButtonUp(Point point) {
			if(!IsMouseTrackedEverywhere)
				return;
			Book.ReleaseMouseCapture();
			IsMouseTrackedEverywhere = false;
			BookAnimationType type = Tracker.EndDrag(point) ? BookAnimationType.TurnPage : BookAnimationType.ReturnPage;
			Animator.Start(type);
		}
		protected internal void OnLeaveActiveArea() { Animator.Start(BookAnimationType.LeaveActiveArea); }
		public bool CheckAndChangeViewState(Point point) {
			BookViewState state = GetDesiredViewState(point);
			bool canChange = PageManager.CanChangePagesViewStateTo(state);
			if(canChange) {
				ViewState = state;
				Tracker.StartDrag(point);
			}
			return canChange;
		}
		protected internal void TrackPointWithoutAnimation(Point point) {
			Animator.Cancel();
			Tracker.ContinueDrag(point);
			Book.UpdateAllProperties();
		}
		protected internal BookViewState GetDesiredViewState(Point point) {
			RectCorner corner = Book.GeometryParams.BookRect.NearestCorner(point);
			return corner.IsRight() ? BookViewState.Next : BookViewState.Prev;
		}
	}
	public class BookActiveAreaHelper {
		protected internal enum BookActiveAreaHelperResult { Outside, Entered, Leaved, Changed, Inside }
		#region Property
		public bool IsInsideAnyActiveArea {
			get {
				return Result == BookActiveAreaHelperResult.Entered ||
					Result == BookActiveAreaHelperResult.Inside ||
					Result == BookActiveAreaHelperResult.Changed;
			}
		}
		public bool IsLeaveActiveArea { get { return Result == BookActiveAreaHelperResult.Leaved; } }
		protected internal BookActiveAreaHelperResult Result { get; set; }
		protected internal BookGeometryParams Params { get; set; }
		protected internal Size ActiveAreaSize { get { return Params.ActiveAreaSize; } }
		protected internal double Right { get { return Params.BookWidth - ActiveAreaSize.Width; } }
		protected internal double Bottom { get { return Params.BookHeight - ActiveAreaSize.Height; } }
		protected internal RectCorner? CurrentCorner { get; set; }
		#endregion
		public BookActiveAreaHelper(Book book) {
			Params = book.GeometryParams;
			Reset();
		}
		public void Reset() { CurrentCorner = null; }
		public void Track(Point point) { Result = CheckPoint(point); }
		protected internal BookActiveAreaHelperResult CheckPoint(Point point) {
			RectCorner? lastCorner = CurrentCorner;
			CurrentCorner = GetActiveAreaCorner(point);
			if(!lastCorner.HasValue)
				return CurrentCorner.HasValue ? BookActiveAreaHelperResult.Entered : BookActiveAreaHelperResult.Outside;
			if(!CurrentCorner.HasValue)
				return BookActiveAreaHelperResult.Leaved;
			return CurrentCorner.Value == lastCorner.Value ? BookActiveAreaHelperResult.Inside : BookActiveAreaHelperResult.Changed;
		}
		protected internal RectCorner? GetActiveAreaCorner(Point point) {
			Array corners = DevExpress.Utils.EnumExtensions.GetValues(typeof(RectCorner));
			foreach(RectCorner corner in corners)
				if(IsInsideArea(point, corner))
					return corner;
			return null;
		}
		protected internal bool IsInsideArea(Point point, RectCorner corner) {
			Rect rect = CreateRect(corner);
			return rect.IsInside(point);
		}
		protected internal Rect CreateRect(RectCorner corner) { return new Rect(CreateLeftTopPoint(corner), ActiveAreaSize); }
		protected internal Point CreateLeftTopPoint(RectCorner corner) {
			switch(corner) {
				case RectCorner.TopRight:
					return new Point(Right, 0);
				case RectCorner.BottomRight:
					return new Point(Right, Bottom);
				case RectCorner.BottomLeft:
					return new Point(0, Bottom);
				default:
					return new Point(0, 0);
			}
		}
	}
	public class BookDragTracker {
		const double EdgeRadiusSafeDecrement = 0.01;
		#region Property
		public Point DragPoint { get; private set; }
		public RectCorner BaseCorner { get; private set; }
		public Point BaseCornerPoint { get { return BasePageRect.Corner(BaseCorner); } }
		public Point BaseNearCornerPoint { get { return BasePageRect.Corner(BaseNearCorner); } }
		public MathLine BaseLine { get { return new MathLine(BaseCornerPoint, BaseNearCornerPoint); } }
		public MathLine DraggingLine { get { return new MathLine(BaseCornerPoint, DragPoint); } }
		public MathLine CuttingLine { get { return DraggingLine.Perpendicular(BaseCornerPoint.MiddlePoint(DragPoint)); } }
		public double PageHeight { get { return BasePageRect.Height; } }
		public double PageWidth { get { return BasePageRect.Width; } }
		public Rect BasePageRect { get { return BaseCorner.IsRight() ? EvenPageRect : OddPageRect; } }
		public Point TurnPageEndPoint { get { return MirrorPageRect.Corner(BaseNearCorner); } }
		protected internal Book Book { get; set; }
		protected internal BookGeometryParams Params { get { return Book.GeometryParams; } }
		protected internal Rect BookRect { get { return Params.BookRect; } }
		protected internal Rect OddPageRect { get { return Params.OddPageRect; } }
		protected internal Rect EvenPageRect { get { return Params.EvenPageRect; } }
		protected internal double SmallRadius { get { return PageWidth; } }
		protected internal double BigRadius { get { return Math.Sqrt(PageWidth * PageWidth + PageHeight * PageHeight); } }
		protected internal Point BaseFarCornerPoint { get { return BasePageRect.Corner(BaseCorner.DiagonalMirror()); } }
		protected internal Rect MirrorPageRect { get { return BaseCorner.IsRight() ? OddPageRect : EvenPageRect; } }
		protected internal RectCorner BaseNearCorner { get { return BaseCorner.HorizontalMirror(); } }
		#endregion
		public BookDragTracker(Book book) { Book = book; }
		public void StartDrag(Point point) {
			BaseCorner = BookRect.NearestCorner(point);
			SetDragPoint(point);
		}
		public void ContinueDrag(Point point) { SetDragPoint(point); }
		public bool EndDrag(Point point) {
			SetDragPoint(point);
			RectCorner corner = BookRect.NearestCorner(DragPoint);
			return !corner.IsSameHorizontalSide(BaseCorner);
		}
		protected internal void SetDragPoint(Point point) { DragPoint = CalcActualDraggingPoint(point); }
		protected internal Point CalcActualDraggingPoint(Point point) {
			if(IsDragPointValid(point))
				return point;
			if(BaseLine.IsSameSide(point, BaseFarCornerPoint))
				return BaseNearCornerPoint.RadialEdgePoint(point, SmallRadius - EdgeRadiusSafeDecrement);
			MathLine diagonal = new MathLine(BaseCornerPoint, BaseFarCornerPoint);
			if(diagonal.IsDifferentSide(point, BaseNearCornerPoint))
				return BaseCornerPoint;
			MathLine vertical = new MathLine(BaseNearCornerPoint, BaseFarCornerPoint);
			Point BaseMirrorPoint = BaseCornerPoint.MirrorPoint(vertical);
			diagonal = new MathLine(BaseMirrorPoint, BaseFarCornerPoint);
			if(diagonal.IsDifferentSide(point, BaseNearCornerPoint))
				return BaseMirrorPoint;
			return BaseFarCornerPoint.RadialEdgePoint(point, BigRadius - EdgeRadiusSafeDecrement);
		}
		protected internal bool IsDragPointValid(Point point) {
			return point.Distance(BaseNearCornerPoint) <= SmallRadius && point.Distance(BaseFarCornerPoint) <= BigRadius;
		}
	}
}
