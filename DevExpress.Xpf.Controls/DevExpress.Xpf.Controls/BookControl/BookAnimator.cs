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
using System.Windows;
using System.Windows.Threading;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Controls.Internal {
	public class BookAnimator {
		#region Property
		public bool IsUninterruptedAnimation {
			get {
				return AnimationType != BookAnimationType.None && AnimationType != BookAnimationType.LeaveActiveArea;
			}
		}
		protected internal bool IsTurnPageAnimation {
			get {
				return AnimationType == BookAnimationType.TurnPage ||
					AnimationType == BookAnimationType.TurnOnePageForward ||
					AnimationType == BookAnimationType.TurnOnePageBackward;
			}
		}
		protected internal bool IsPartialTurnAnimation {
			get {
				return AnimationType == BookAnimationType.PartialTurnForward || AnimationType == BookAnimationType.PartialTurnBackward;
			}
		}
		protected internal Book Book { get; set; }
		protected internal BookGeometryParams Params { get { return Book.GeometryParams; } }
		protected internal BookEventHandler Handler { get { return Book.EventHandler; } }
		protected internal BookDragTracker Tracker { get { return Book.DragTracker; } }
		protected internal BookViewState ViewState { get { return Handler.ViewState; } set { Handler.ViewState = value; } }
		protected internal Point OddPoint { get { return OddPageRect.BottomLeft(); } }
		protected internal Point EvenPoint { get { return EvenPageRect.BottomRight(); } }
		protected internal virtual Point PartialTurnForwardEndPoint {
			get {
				return new Point() {
					X = EvenPoint.X - Book.BookWidth * Book.PartialTurnEndPoint.X,
					Y = EvenPoint.Y - Book.BookHeight * Book.PartialTurnEndPoint.Y
				};
			}
		}
		protected internal virtual Point PartialTurnBackwardEndPoint {
			get {
				return new Point() {
					X = OddPoint.X + Book.BookWidth * Book.PartialTurnEndPoint.X,
					Y = OddPoint.Y - Book.BookHeight * Book.PartialTurnEndPoint.Y
				};
			}
		}
		protected internal double TurnPageRadius { get { return Math.Sqrt(AvgPageWidth * AvgPageWidth + AvgPageHeight * AvgPageHeight); } }
		protected internal double AvgPageWidth {
			get {
				double oddWidth = double.IsNaN(OddPageRect.Width) ? 0.0 : OddPageRect.Width;
				double evenWidth = double.IsNaN(EvenPageRect.Width) ? 0.0 : EvenPageRect.Width;
				return oddWidth > evenWidth ? oddWidth : evenWidth;
			}
		}
		protected internal double AvgPageHeight {
			get {
				double oddHeight = double.IsNaN(OddPageRect.Height) ? 0.0 : OddPageRect.Height;
				double evenHeight = double.IsNaN(EvenPageRect.Height) ? 0.0 : EvenPageRect.Height;
				return oddHeight > evenHeight ? oddHeight : evenHeight;
			}
		}
		protected internal double AnimationRate { get { return Book.AnimationRate; } }
		protected internal double AnimationSpeed { get { return Book.AnimationSpeed; } }
		protected internal double ShortAnimationSpeed { get { return Book.ShortAnimationSpeed; } }
		public virtual Rect OddPageRect { get { return Params.OddPageRect; } }
		public virtual Rect EvenPageRect { get { return Params.EvenPageRect; } }
		protected internal DispatcherTimer Timer { get; set; }
		protected internal PathBuilder Builder { get; set; }
		protected internal BookAnimationType AnimationType { get; set; }
		protected internal int AnimationCounter { get; set; }
		#endregion
		public BookAnimator(Book book) {
			Book = book;
			Timer = new DispatcherTimer();
			Timer.Interval = new TimeSpan((long)(1000000.0 / AnimationRate));
			Timer.Tick += (d, e) => TimerTick();
			AnimationType = BookAnimationType.None;
		}
		public void Start(BookAnimationType type) {
			Cancel();
			AnimationType = type;
			switch(type) {
			case BookAnimationType.LeaveActiveArea:
				CreateLinePathBuilder(Tracker.BaseCornerPoint, ShortAnimationSpeed);
				break;
			case BookAnimationType.TurnPage:
				CreateLinePathBuilder(Tracker.TurnPageEndPoint, AnimationSpeed);
				break;
			case BookAnimationType.ReturnPage:
				CreateLinePathBuilder(Tracker.BaseCornerPoint, AnimationSpeed);
				break;
			case BookAnimationType.TurnOnePageForward:
				TurnOnePage(BookViewState.Next, EvenPoint, OddPoint);
				break;
			case BookAnimationType.TurnOnePageBackward:
				TurnOnePage(BookViewState.Prev, OddPoint, EvenPoint);
				break;
			case BookAnimationType.PartialTurnForward:
				TurnOnePage(BookViewState.Next, EvenPoint, PartialTurnForwardEndPoint);
				break;
			case BookAnimationType.PartialTurnBackward:
				TurnOnePage(BookViewState.Prev, OddPoint, PartialTurnBackwardEndPoint);
				break;
			case BookAnimationType.PartialReturn:
				Book.EventHandler.IsPartialTurn = false;
				CreateLinePathBuilder(Tracker.BaseCornerPoint, AnimationSpeed);
				break;
			default:
				return;
			}
			BeginAnimation();
		}
		public void Cancel() {
			Timer.Stop();
			AnimationType = BookAnimationType.None;
		}
		protected internal void CreateLinePathBuilder(Point endPoint, double speed) {
			Builder = new PathBuilder(Tracker.DragPoint, endPoint, AnimationRate / speed);
			Builder.CreateLinePath();
		}
		protected internal void TurnOnePage(BookViewState state, Point startPoint, Point endPoint) {
			Handler.ViewState = state;
			Builder = new PathBuilder(startPoint, endPoint, AnimationRate / AnimationSpeed);
			Point outsidePoint = Params.BookRect.TopMiddle();
			Builder.CreateArcPath(TurnPageRadius, outsidePoint, false);
			Tracker.StartDrag(Builder.StartPoint);
		}
		protected internal void BeginAnimation() {
			bool isArcPath =
				AnimationType == BookAnimationType.TurnOnePageBackward || AnimationType == BookAnimationType.TurnOnePageForward ||
				AnimationType == BookAnimationType.PartialTurnBackward || AnimationType == BookAnimationType.PartialTurnForward;
			if(isArcPath) {
				Point consumeStartPoint = Builder.GetPoint();
				AnimationCounter = Builder.NumberOfPoints - 1;
			}
			else
				AnimationCounter = Builder.NumberOfPoints;
			Timer.Start();
		}
		protected internal virtual void EndAnimation() {
			if(IsTurnPageAnimation)
				TurnThePage();
			if (IsPartialTurnAnimation)
				Book.EventHandler.IsPartialTurn = true;
			else
				Handler.ViewState = BookViewState.Current;
			Cancel();
		}
		protected internal virtual void TimerTick() {
			Point point = Builder.GetPoint();
			if(AnimationCounter != 0)
				AnimationCounter--;
			else
				EndAnimation();
			try {
				UpdateDragPoint(point);
			} catch { }
		}
		protected internal void UpdateDragPoint(Point point) {
			Tracker.ContinueDrag(point);
			Book.UpdateAllProperties();
		}
		protected internal void TurnThePage() { Book.PageIndex += Handler.ViewState == BookViewState.Next ? 2 : -2; }
	}
}
