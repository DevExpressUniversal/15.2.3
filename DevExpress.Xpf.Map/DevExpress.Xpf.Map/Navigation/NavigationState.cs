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
using System.Windows.Input;
namespace DevExpress.Xpf.Map.Native {
	public abstract class NavigationState {
		readonly NavigationController controller;
		protected NavigationController Controller { get { return controller; } }
		protected MapControl Map { get { return Controller.Map; } }
		public virtual bool AllowMouseCapture { get { return true; } }
		public virtual bool AllowHandleMouseLButtonDown { get { return true; } }
		protected NavigationState(NavigationController controller) {
			this.controller = controller;
		}
		public virtual void Activate() {
		}
		public virtual void Deactivate() {
		}
		public virtual void OnMouseLeftButtonDown(MouseEventArgs e) {
		}
		public virtual void OnMouseLeftButtonUp(MouseEventArgs e) {
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
		}
		public virtual void OnMouseWheel(MouseWheelEventArgs e) {
		}
		public virtual void OnManipulationDelta(ManipulationDeltaEventArgs e) {
		}
		public virtual void OnManipulationComplete(ManipulationCompletedEventArgs e) {
		}
		public virtual void OnManipulationStarted(ManipulationStartedEventArgs e) {
		}
		public virtual void OnTouchFrameReported(TouchFrameEventArgs e) {
		}
	}
	public class DefaultState : NavigationState {
		double mouseWheelDeltaStore;
		public DefaultState(NavigationController controller)
			: base(controller) {
		}
		public override void OnMouseLeftButtonUp(MouseEventArgs e) {
		}
		public override void OnMouseWheel(MouseWheelEventArgs e) {
			if(!Controller.CanZoom())
				return;
			double zoomValue = UpdateWheelDelta((double)e.Delta / (double)120)*Map.MouseWheelZoomingStep;
			if(zoomValue == 0)
				return;
			double newZoomLevel = Map.ZoomLevel + zoomValue;
			mouseWheelDeltaStore = 0.0f;
			Point mousePosition = e.GetPosition(Map);
			Map.SetZoomLevel(newZoomLevel, mousePosition);
		}
		internal int UpdateWheelDelta(double delta) {
			mouseWheelDeltaStore += delta;
			double absValue = Math.Abs(mouseWheelDeltaStore);
			int quants = (int)Math.Floor(absValue) * Math.Sign(mouseWheelDeltaStore);
			mouseWheelDeltaStore -= quants;
			return quants;
		}
		public override void OnMouseLeftButtonDown(MouseEventArgs e) {
			Point pos = e.GetPosition(Map);
			Map.LeftMouseClick(pos);
		}
	}
	public class MoveCenterPointState : NavigationState {
		bool moved;
		public Point StartPoint { get; set; }
		public MoveCenterPointState(NavigationController controller)
			: base(controller) {
		}
		public override void OnMouseMove(MouseEventArgs e) {
			if (!Controller.CanScroll())
				Controller.SetDefaultState();
			Point point = e.GetPosition(Map);
			if(StartPoint == point)
				return;
			moved = true;
			Controller.Move(new Point(StartPoint.X - point.X, StartPoint.Y - point.Y));
			StartPoint = point;
		}
		public override void OnMouseLeftButtonUp(MouseEventArgs e) {
			if(!moved) {
				Point pos = e.GetPosition(Map);
				Map.LeftMouseClick(pos);
			}
			Controller.SetDefaultState();
		}
	}
	public abstract class RegionSelectionStateBase : NavigationState {
		Point startPos;
		Point endPos;
		protected Point StartPos { get { return startPos; } }
		protected Point EndPos { get { return endPos; } }
		protected RegionSelectionStateBase(NavigationController controller)
			: base(controller) {
		}
		public override void Deactivate() {
			Controller.HideZoomRegionPresenter();
			ApplyActionOnComplete();
		}
		public override void OnMouseLeftButtonDown(MouseEventArgs e) {
			startPos = e.GetPosition(Map);
		}
		public override void OnMouseLeftButtonUp(MouseEventArgs e) {
			endPos = e.GetPosition(Map);
		}
		protected abstract void ApplyActionOnComplete();
		public override void OnMouseMove(MouseEventArgs e) {
			endPos = e.GetPosition(Map);
			Controller.UpdateZoomRegion(StartPos, EndPos);
		}
	}
	public class SelectItemsRegionSelectionState : RegionSelectionStateBase {
		public SelectItemsRegionSelectionState(NavigationController controller)
			: base(controller) {
		}
		protected override void ApplyActionOnComplete() {
			Controller.SelectItemsByRegion(StartPos, EndPos);
		}
	}
	public class ZoomRegionSelectionState : RegionSelectionStateBase {
		public ZoomRegionSelectionState(NavigationController controller)
			: base(controller) {
		}
		protected override void ApplyActionOnComplete() {
			Controller.ZoomIntoRegion(StartPos, EndPos);
		}
	}
	public class TouchState : NavigationState {
		public override bool AllowMouseCapture { get { return false; } }
		public TouchState(NavigationController controller)
			: base(controller) {
		}
		public override void Activate() {
			Map.CaptureStylus();
		}
		public override void Deactivate() {
			Map.ReleaseStylusCapture();
		}
		public override void OnManipulationComplete(ManipulationCompletedEventArgs e) {
			if(e.TotalManipulation.Translation.Length == 0.0)
				Map.LeftMouseClick(e.ManipulationOrigin);
		}
		public override void OnManipulationDelta(ManipulationDeltaEventArgs e) {
			double scaleX = e.CumulativeManipulation.Scale.X;
			double scaleY = e.CumulativeManipulation.Scale.Y;
			double zoomFactor = (scaleX + scaleY) / 2.0;
			Controller.ZoomAndMove(zoomFactor, e.ManipulationOrigin, new Point(-e.DeltaManipulation.Translation.X, -e.DeltaManipulation.Translation.Y));
		}
	}
}
