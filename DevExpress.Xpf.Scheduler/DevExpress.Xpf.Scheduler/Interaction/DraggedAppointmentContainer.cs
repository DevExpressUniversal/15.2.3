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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduler;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Core.Native;
#if SL
using PlatformIndependenDragDropEffects = DevExpress.Utils.DragDropEffects;
using DevExpress.Xpf.Scheduler.WPFCompatibility;
using DevExpress.Xpf.Scheduler.Internal;
#else
using PlatformIndependenDragDropEffects = System.Windows.Forms.DragDropEffects;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#endif
namespace DevExpress.XtraScheduler.Native {
	public interface IDraggedAppointmentContainer {
		void Show(System.Drawing.Point point, VisualAppointmentViewInfo aptViewInfo);
		void Move(VisualAppointmentViewInfo aptViewInfo, System.Drawing.Point hitPoint, PlatformIndependenDragDropEffects effect);
		void Hide();
		void Close();
	}
	public class EmptyDraggedAppointmentContainer : IDraggedAppointmentContainer {
		public void Show(System.Drawing.Point point, VisualAppointmentViewInfo aptViewInfo) {
		}
		public void Move(VisualAppointmentViewInfo aptViewInfo, System.Drawing.Point hitPoint, PlatformIndependenDragDropEffects effect) {
		}
		public void Hide() {
		}
		public void Close() {
		}
	}
	public class DraggedAppointmentContainer : IDraggedAppointmentContainer {
		FloatingContainer floatingContainer;
		SchedulerControl control;
		List<VisualAppointmentInfo> appointmentDragInfos;
		Vector mouseOffset;
		AppointmentDragMouseHandlerStateBase mouseHandlerState;
		public DraggedAppointmentContainer(SchedulerControl control, AppointmentDragMouseHandlerStateBase mouseHandlerState) {
			this.control = control;
			this.mouseHandlerState = mouseHandlerState;
		}
		public FloatingContainer FloatingContainer { get { return floatingContainer; } }
		public SchedulerControl Control { get { return control; } }
		public List<VisualAppointmentInfo> AppointmentDragInfos { get { return appointmentDragInfos; } }
		public Vector MouseOffset { get { return mouseOffset; } }
		public AppointmentDragMouseHandlerStateBase MouseHandlerState { get { return mouseHandlerState; } }
		public void Show(System.Drawing.Point point, VisualAppointmentViewInfo aptViewInfo) {
			DragAppointmentChangeHelperState dragState = MouseHandlerState.AppointmentChangeHelper.State as DragAppointmentChangeHelperState;
			if (dragState == null)
				return;
			Appointment primaryAppointment = dragState.PrimaryAppointment.Appointment;
			AppointmentBaseCollection sourceAppointments = GetSourceAppointments();
			List<VisualAppointmentInfo> visualAppointmentViewInfos = GetVisualAppointmentInfos(Control, primaryAppointment, sourceAppointments);
			XpfMouseUtils.TranslateBoundsToControl(Control, visualAppointmentViewInfos);
			this.appointmentDragInfos = visualAppointmentViewInfos;
			ShowCore(point, aptViewInfo);
		}
		void ShowCore(System.Drawing.Point mousePosition, VisualAppointmentViewInfo aptViewInfo) {
			List<VisualAppointmentInfo> dragInfos = FilterDragInfos(AppointmentDragInfos, aptViewInfo);
			if (dragInfos.Count <= 0)
				return;
			if (Control.ActiveViewType == SchedulerViewType.Day || Control.ActiveViewType == SchedulerViewType.WorkWeek || Control.ActiveViewType == SchedulerViewType.FullWeek) {
				RestrictDragInfosSize(dragInfos);
				if (aptViewInfo != null)
					AdjustDragInfosBounds(dragInfos, aptViewInfo, mousePosition);
			}
			Rect totalBounds = CalcTotalBounds(dragInfos);
			if (totalBounds.Width <= 0) {
				this.appointmentDragInfos = null;
				return;
			}
			Canvas canvas = CreateCanvas(totalBounds.Size());
			control.RegisterDraggedContent(canvas);
			this.floatingContainer = CreateFloatingContainer();
			FloatingContainer.BeginUpdate();
			floatingContainer.Content = canvas;
			floatingContainer.FloatSize = totalBounds.Size();
			Point location = totalBounds.Location();
			this.floatingContainer.FloatLocation = location;
			this.mouseOffset = new Vector(mousePosition.X - location.X, mousePosition.Y - location.Y);
#if !SL
			if (Control.FlowDirection == FlowDirection.LeftToRight)
				this.mouseOffset.X += floatingContainer.FloatSize.Width;
#endif
			FloatingContainer.EndUpdate();
			FloatingContainer.IsOpen = true;
			PopulateWithAppointmentControls(dragInfos, totalBounds, canvas);
		}
		public void Move(VisualAppointmentViewInfo aptViewInfo, System.Drawing.Point hitPoint, PlatformIndependenDragDropEffects effect) {
			if (FloatingContainer != null) {
				if (!FloatingContainer.IsOpen)
					FloatingContainer.IsOpen = true;
				double offsetX = hitPoint.X - mouseOffset.X;
#if SL
				if (Control.FlowDirection == FlowDirection.RightToLeft)
					offsetX = mouseOffset.X - hitPoint.X - FloatingContainer.FloatSize.Width + Control.ActualWidth;
#else
				offsetX += FloatingContainer.ActualSize.Width;
#endif
				Point pt = new Point(offsetX, hitPoint.Y - mouseOffset.Y);
				this.floatingContainer.FloatLocation = pt;
			}
			else {
				Show(hitPoint, aptViewInfo);
				RequestVisualAppointmentInfoEventArgs e = new RequestVisualAppointmentInfoEventArgs(AppointmentDragState.Drag);
				e.AppointmentInfos.AddRange(AppointmentDragInfos);
				e.Copy = (effect & PlatformIndependenDragDropEffects.Copy) != 0;
				Control.RaiseRequestVisualAppointmentInfo(e);
			}
		}
		public void Hide() {
			if (FloatingContainer == null)
				return;
			FloatingContainer.IsOpen = false;
		}
		public void Close() {
			if (FloatingContainer == null)
				return;
			RequestVisualAppointmentInfoEventArgs e = new RequestVisualAppointmentInfoEventArgs(AppointmentDragState.Cancel);
			if (AppointmentDragInfos != null) {
				e.AppointmentInfos.AddRange(AppointmentDragInfos);
				Control.RaiseRequestVisualAppointmentInfo(e);
			}
			UIElement content = FloatingContainer.Content as UIElement;
			Control.UnregisterDraggedContent(content);
#if !SL
			FloatingContainer.Close();
#else
			FloatingContainer.IsOpen = false;
#endif
			this.floatingContainer = null;
		}
		List<VisualAppointmentInfo> GetVisualAppointmentInfos(SchedulerControl Control, Appointment primaryAppointment, AppointmentBaseCollection sourceAppointments) {
			RequestVisualAppointmentInfoEventArgs infoArgs = new RequestVisualAppointmentInfoEventArgs(AppointmentDragState.Begin);
			infoArgs.PrimaryAppointment = primaryAppointment;
			infoArgs.SourceAppointments = sourceAppointments;
			Control.RaiseRequestVisualAppointmentInfo(infoArgs);
			return infoArgs.AppointmentInfos;
		}
		protected virtual AppointmentBaseCollection GetSourceAppointments() {
			SafeAppointmentCollection safeCollection = MouseHandlerState.AppointmentChangeHelper.SourceAppointments;
			int count = safeCollection.Count;
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			for (int i = 0; i < count; i++) 
				result.Add(safeCollection[i].Appointment);
			return result;
		}
		protected virtual VisualAppointmentControl CreateActualClone(VisualAppointmentInfo aptDragInfo) {
			return aptDragInfo.Container.CreateCloneForDrag();
		}
		Canvas CreateCanvas(Size size) {
			Canvas canvas = new Canvas();
			canvas.FlowDirection = Control.FlowDirection;
			canvas.Width = size.Width;
			canvas.Height = size.Height;
			canvas.AllowDrop = true;
			canvas.IsHitTestVisible = true;
			return canvas;
		}
		void PopulateWithAppointmentControls(List<VisualAppointmentInfo> infos, Rect totalBounds, Canvas canvas) {
			int count = infos.Count;
			for (int i = 0; i < count; i++) {
				VisualAppointmentInfo aptDragInfo = infos[i];
				VisualAppointmentControl clone = CreateActualClone(aptDragInfo);
				VisualAppointmentViewInfo viewInfo = aptDragInfo.Container.ViewInfo.Clone();
				PrepareViewInfo(viewInfo);
				clone.ViewInfo = viewInfo;
				clone.Width = aptDragInfo.Bounds.Width;
				clone.Height = aptDragInfo.Bounds.Height;
				canvas.Children.Add(clone);
				aptDragInfo.Container.ApplyTemplate();
				clone.UpdateLayout();
				DependencyObject toolTipContainer = clone.GetToolTipContainer();
				if (toolTipContainer != null)
					ToolTipService.SetToolTip(toolTipContainer, null);
				Canvas.SetLeft(clone, aptDragInfo.Bounds.Left - totalBounds.Left);
				Canvas.SetTop(clone, aptDragInfo.Bounds.Top - totalBounds.Top);
			}
		}
		void PrepareViewInfo(VisualAppointmentViewInfo viewInfo) {
			viewInfo.HasLeftBorder = true;
			viewInfo.HasTopBorder = true;
			viewInfo.HasRightBorder = true;
			viewInfo.HasBottomBorder = true;
		}
		FloatingContainer CreateFloatingContainer() {
			FloatingContainer container = FloatingContainerFactory.Create(FloatingMode.Window);
			container.BeginUpdate();
			container.ShowActivated = false;
#if !SL
			container.ContainerTemplate = null;
#else
			container.ShowContentOnly = true;
#endif
			container.Owner = Control;
			LogicalTreeIntruder.AddLogicalChild(Control, container);
			container.EndUpdate();
			return container;
		}
		protected internal virtual Rect CalcTotalBounds(List<VisualAppointmentInfo> aptDragInfos) {
			int count = aptDragInfos.Count;
			double left = 0;
			double right = 0;
			double top = 0;
			double bottom = 0;
			for (int i = 0; i < count; i++) {
				VisualAppointmentInfo dragInfo = aptDragInfos[i];
				Rect bounds = dragInfo.Bounds;
				if (i == 0) {
					left = bounds.Left;
					right = bounds.Right;
					top = bounds.Top;
					bottom = bounds.Bottom;
				}
				else {
					left = Math.Min(bounds.Left, left);
					right = Math.Max(bounds.Right, right);
					top = Math.Min(bounds.Top, top);
					bottom = Math.Max(bounds.Bottom, bottom);
				}
			}
			return new Rect(new Point(left, top), new Point(right, bottom));
		}
		protected internal virtual List<VisualAppointmentInfo> FilterDragInfos(List<VisualAppointmentInfo> dragInfos, VisualAppointmentViewInfo aptViewInfo) {
			List<VisualAppointmentInfo> result = new List<VisualAppointmentInfo>();
			if (aptViewInfo != null) {
				VisualAppointmentInfo hitAppointment = GetHittedDragInfo(dragInfos, aptViewInfo);
				if (hitAppointment != null)
					result.Add(hitAppointment);
				int count = dragInfos.Count;
				for (int i = 0; i < count; i++)
					FilterDragInfo(result, dragInfos[i]);
			}
			else
				result.AddRange(dragInfos);
			return result;
		}
		protected internal virtual void FilterDragInfo(List<VisualAppointmentInfo> result, VisualAppointmentInfo currentDragInfo) {
			VisualAppointmentInfo processedDragInfo = GetDragInfo(result, currentDragInfo.Container.GetAppointment());
			if (processedDragInfo == null) {
				result.Add(currentDragInfo);
				return;
			}
			if (IsDragInfoVisible(processedDragInfo))
				return;
			result.Remove(processedDragInfo);
			result.Add(currentDragInfo);
		}
		protected internal virtual bool IsDragInfoVisible(VisualAppointmentInfo info) {
			return ((info.Bounds.Bottom > 0) && (info.Bounds.Top < control.ActualHeight));
		}
		protected internal virtual VisualAppointmentInfo GetHittedDragInfo(List<VisualAppointmentInfo> dragInfos, VisualAppointmentViewInfo aptViewInfo) {
			int count = dragInfos.Count;
			for (int i = 0; i < count; i++) {
				VisualAppointmentInfo dragInfo = dragInfos[i];
				if (dragInfo.Container.ViewInfo == aptViewInfo)
					return dragInfo;
			}
			return null;
		}
		protected internal virtual VisualAppointmentInfo GetDragInfo(List<VisualAppointmentInfo> dragInfos, Appointment apt) {
			int count = dragInfos.Count;
			for (int i = 0; i < count; i++) {
				VisualAppointmentInfo dragInfo = dragInfos[i];
				if (dragInfo.Container.GetAppointment() == apt)
					return dragInfo;
			}
			return null;
		}
		protected internal virtual void RestrictDragInfosSize(List<VisualAppointmentInfo> aptDragInfos) {
			IVisualDayView viewInfo = Control.ActiveView.VisualViewInfo as IVisualDayView;
			XtraSchedulerDebug.Assert(viewInfo != null);
			ScrollViewer scrollViewer = viewInfo.DayViewScrollViewer;
			System.Windows.Media.GeneralTransform gt = scrollViewer.TransformToVisual(Control);
			Rect viewportBounds = gt.TransformBounds(new Rect(0, 0, scrollViewer.ViewportWidth, scrollViewer.ViewportHeight));
			int count = aptDragInfos.Count;
			for (int i = 0; i < count; i++)
				RestrictDragInfoSize(aptDragInfos[i], viewportBounds);
		}
		protected internal virtual void RestrictDragInfoSize(VisualAppointmentInfo dragInfo, Rect viewportBounds) {
			Rect bounds = dragInfo.Bounds;
			bounds.Intersect(viewportBounds);
			if (bounds.IsEmpty)
				return;
			double dragHeight = Control.ActiveView.DraggedAppointmentHeightInternal;
			if (dragHeight > 0)
				bounds.Height = Math.Min(dragHeight, dragInfo.Bounds.Height);
			dragInfo.SetBounds(bounds, control);
		}
		protected internal virtual void AdjustDragInfosBounds(List<VisualAppointmentInfo> dragInfos, VisualAppointmentViewInfo aptViewInfo, System.Drawing.Point mousePosition) {
			if (dragInfos.Count == 0)
				return;
			XtraSchedulerDebug.Assert(dragInfos[0] == GetHittedDragInfo(dragInfos, aptViewInfo));
			AdjustHittedDragInfoBounds(dragInfos[0], aptViewInfo, mousePosition);
			int count = dragInfos.Count;
			for (int i = 1; i < count; i++) {
				VisualAppointmentInfo info = dragInfos[i];
				AdjustDragInfoBounds(info);
			}
		}
		protected internal virtual void AdjustHittedDragInfoBounds(VisualAppointmentInfo dragInfo, VisualAppointmentViewInfo aptViewInfo, System.Drawing.Point mousePosition) {
			Point hitPoint = new Point(mousePosition.X, mousePosition.Y);
			Rect bounds = dragInfo.Bounds;
			if (!bounds.Contains(hitPoint)) {
				bounds.X = bounds.Location().X;
				bounds.Y = hitPoint.Y;
				dragInfo.SetBounds(bounds, control);
			}
		}
		protected internal virtual void AdjustDragInfoBounds(VisualAppointmentInfo info) {
			if (IsDragInfoVisible(info))
				return;
			Rect bounds = info.Bounds;
			if (bounds.Bottom < 0)
				bounds.Y = 0;
			if (bounds.Y > control.ActualHeight)
				bounds.Y = control.ActualHeight - bounds.Height;
			info.SetBounds(bounds, control);
		}
	}
	public class ExternalDraggedAppointmentContainer : DraggedAppointmentContainer {
		public ExternalDraggedAppointmentContainer(SchedulerControl control, AppointmentDragMouseHandlerStateBase mouseHandlerState)
			: base(control, mouseHandlerState) {
		}
		protected override AppointmentBaseCollection GetSourceAppointments() {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			AppointmentControlCollection appointmentControls = Control.ActiveView.ViewInfo.DraggedAppointmentControls;
			int count = appointmentControls.Count;
			for (int i = 0; i < count; i++) {
				result.Add(appointmentControls[i].Appointment);
			}
			return result;
		}
		protected override VisualAppointmentControl CreateActualClone(VisualAppointmentInfo aptDragInfo) {
			VisualAppointmentControl result = aptDragInfo.Panel.CreateVisualAppointment();
			return result;
		}
	}
}
