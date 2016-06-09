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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.NavBar {
	public class ThumbDragManager : FrameworkContentElement {
		public static readonly DependencyProperty UpDragDeltaProperty;
		public static readonly DependencyProperty DownDragDeltaProperty;
		public static readonly DependencyProperty ThumbProperty;
		public static readonly DependencyProperty ThumbDragInfoProperty;
		public static readonly DependencyProperty OrientationProperty;
		static ThumbDragManager() {
			UpDragDeltaProperty = DependencyPropertyManager.RegisterAttached("UpDragDelta", typeof(double), typeof(ThumbDragManager), new FrameworkPropertyMetadata(UpDragDeltaPropertyChanged));
			DownDragDeltaProperty = DependencyPropertyManager.RegisterAttached("DownDragDelta", typeof(double), typeof(ThumbDragManager), new FrameworkPropertyMetadata(DownDragDeltaPropertyChanged));
			ThumbProperty = DependencyPropertyManager.RegisterAttached("Thumb", typeof(NavPaneSplitter), typeof(ThumbDragManager), new FrameworkPropertyMetadata(ThumbPropertyChanged));
			ThumbDragInfoProperty = DependencyPropertyManager.RegisterAttached("ThumbDragInfo", typeof(ThumbDragInfo), typeof(ThumbDragManager));
			OrientationProperty = DependencyPropertyManager.RegisterAttached("Orientation", typeof(Orientation), typeof(ThumbDragManager), new FrameworkPropertyMetadata(OrientationPropertyChanged));
		}
		static void OrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ThumbDragInfo info = GetThumbDragInfo(d);
			info.Orientation = (Orientation)e.NewValue;			
		}
		static void UpDragDeltaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ThumbDragInfo info = GetThumbDragInfo(d);
		}
		static void DownDragDeltaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ThumbDragInfo info = GetThumbDragInfo(d);
		}
		static void ThumbPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ThumbDragInfo info = GetThumbDragInfo(d);
			info.Thumb = (NavPaneSplitter)e.NewValue;			
		}		
		public static ThumbDragInfo GetThumbDragInfo(DependencyObject d) {
			var info = (ThumbDragInfo)d.GetValue(ThumbDragInfoProperty);
			if (info == null) {
				info = new ThumbDragInfo();
				SetThumbDragInfo(d, info);
			}
			return info;
		}
		public static void SetThumbDragInfo(DependencyObject d, ThumbDragInfo info) {
			d.SetValue(ThumbDragInfoProperty, info);
		}
		public static double GetUpDragDelta(DependencyObject d) {
			return (double)d.GetValue(UpDragDeltaProperty);
		}
		public static void SetUpDragDelta(DependencyObject d, double value) {
			d.SetValue(UpDragDeltaProperty,	value);
		}
		public static double GetDownDragDelta(DependencyObject d) {
			return (double)d.GetValue(DownDragDeltaProperty);
		}
		public static void SetDownDragDelta(DependencyObject d, double value) {
			d.SetValue(DownDragDeltaProperty, value);
		}
		public static NavPaneSplitter GetThumb(DependencyObject d) {
			return (NavPaneSplitter)d.GetValue(ThumbProperty);
		}
		public static void SetThumb(DependencyObject d, double value) {
			d.SetValue(ThumbProperty, value);
		}
		public static Orientation GetOrientation(DependencyObject d) {
			return (Orientation)d.GetValue(OrientationProperty);
		}
		public static void SetOrientation(DependencyObject d, Orientation value) {
			d.SetValue(OrientationProperty, value);
		}
	}
	public class ThumbDragInfo {
		NavBarControl navbar;
		NavPaneSplitter thumb;
		Point cursorPosition;
		public Orientation Orientation { get; set; }
		public NavPaneSplitter Thumb {
			get { return thumb; }
			set { SetThumb(value); }
		}
		void SetThumb(NavPaneSplitter thumb) {
			if (this.thumb != null) {
				this.thumb.MouseLeftButtonDown -= new MouseButtonEventHandler(OnMouseDown);
				this.thumb.MouseMove -= new MouseEventHandler(OnMouseMove);
				this.thumb.DragStarted -= new DragStartedEventHandler(OnDragStarted);
				this.thumb.DragCompleted -= new DragCompletedEventHandler(OnDragCompleted);
				this.thumb.DragDelta -= new DragDeltaEventHandler(OnDragDelta);
				this.thumb.TouchDown -= OnTouchDown;
				this.thumb.TouchUp -= OnTouchUp;
				this.thumb.TouchMove -= OnTouchMove;
			}
			this.thumb = thumb;
			if(thumb == null) 
				return;
			thumb.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseDown);
			thumb.MouseMove += new MouseEventHandler(OnMouseMove);
			thumb.DragStarted += new DragStartedEventHandler(OnDragStarted);
			thumb.DragCompleted += new DragCompletedEventHandler(OnDragCompleted);
			thumb.DragDelta += new DragDeltaEventHandler(OnDragDelta);
			thumb.TouchDown += OnTouchDown;
			thumb.TouchUp += OnTouchUp;
			thumb.TouchMove += OnTouchMove;
			navbar = LayoutHelper.FindParentObject<NavBarControl>(thumb);
		}
		bool performingTouchDrag = false;
		void OnTouchMove(object sender, TouchEventArgs e) {
			cursorPosition = e.GetTouchPoint(navbar).Position;
			if(performingTouchDrag) OnDragDelta(sender, null);
		}
		void OnTouchUp(object sender, TouchEventArgs e) {
			performingTouchDrag = false;
		}
		void OnTouchDown(object sender, TouchEventArgs e) {
			cursorPosition = e.GetTouchPoint(navbar).Position;
			OnMouseDown(sender, null);
			performingTouchDrag = true;
		}
		void OnMouseDown(object sender, MouseButtonEventArgs e) {
			NavigationPaneView view = (NavigationPaneView)navbar.View;
			SizeHelperBase sizeHelper = SizeHelperBase.GetDefineSizeHelper(Orientation);
			view.MaxVisibleGroupCount = view.ActualVisibleGroupCount;
			ThumbDelta = sizeHelper.GetDefinePoint(navbar.TranslatePoint(cursorPosition, view.Panel.Splitter));
		}
		void OnMouseMove(object sender, MouseEventArgs e) {
			cursorPosition = e.GetPosition(navbar);
		}
		void OnDragStarted(object sender, DragStartedEventArgs e) {
		}
		void OnDragCompleted(object sender, DragCompletedEventArgs e) {			
		}
		double GetHeight(Size size) {
			if(navbar.View.Orientation == System.Windows.Controls.Orientation.Vertical)
				return size.Height;
			return size.Width;
		}
		double ThumbDelta { get; set; }
		void OnDragDelta(object sender, DragDeltaEventArgs e) {
			Rect thumbRect = LayoutHelper.GetRelativeElementRect(thumb, navbar);
			int groupsCount = 0;
			if(!performingTouchDrag)
				cursorPosition = Mouse.GetPosition(navbar);
			foreach(NavBarGroup group in navbar.Groups) {
				if(group.IsNavPaneGroup)
					groupsCount++;
			}
			NavigationPaneView view = (NavigationPaneView)navbar.View;
			SizeHelperBase sizeHelper = SizeHelperBase.GetDefineSizeHelper(Orientation);
			double maxHeight = sizeHelper.GetDefinePoint(view.Panel.GroupsControl.TranslatePoint(new Point(0, view.Panel.GroupsControl.ActualHeight), navbar)) - sizeHelper.GetDefinePoint(cursorPosition) - ThumbDelta;
			double totalHeight = 0;
			int maxVisibleGroupCount = 0;
			for(int i = 0; i < groupsCount; i++) {
				UIElement button = (UIElement)view.Panel.GroupsControl.ItemContainerGenerator.ContainerFromIndex(i);
				if(totalHeight + sizeHelper.GetDefineSize(button.DesiredSize) > maxHeight)
					break;
				totalHeight += sizeHelper.GetDefineSize(button.DesiredSize);
				maxVisibleGroupCount++;
			}
			view.InternalSetMaxVisibleGroupCount(maxVisibleGroupCount);
		}		
	}
}
