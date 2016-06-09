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
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Utils.Themes;
using System.Windows.Input;
using DevExpress.Xpf.Utils;
using System.Windows.Media;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars.Customization {
	public class BarDragElementPopup : Popup, IDragElement {
		#region static
		public static readonly DependencyProperty CursorOffsetProperty;
		static BarDragElementPopup() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarDragElementPopup), new FrameworkPropertyMetadata(typeof(BarDragElementPopup)));
			CursorOffsetProperty = DependencyPropertyManager.Register("CursorOffset", typeof(Point), typeof(BarDragElementPopup), new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.None));
			IsOpenProperty.OverrideMetadata(typeof(BarDragElementPopup), new FrameworkPropertyMetadata(false, null, new CoerceValueCallback((d,v)=>((BarDragElementPopup)d).CoerceIsOpen((bool)v))));
		}		
		#endregion
		object barDragObject;
		[ThreadStatic]
		internal static BarDragElementPopup currentDragPopup;
		protected static BarDragElementPopup CurrentDragPopup {
			get { return currentDragPopup; }
			set {
				if (currentDragPopup == value)
					return;
				if (currentDragPopup != null) {					
					BarNameScope.GetService<ICustomizationService>(currentDragPopup).IsPreparedToQuickCustomizationMode = false;
					currentDragPopup.isDestroyed = true;
					currentDragPopup.IsOpen = false;
					currentDragPopup = null;
				}
				currentDragPopup = value;
			}
		}
		bool isDestroyed;
		public BarDragElementPopup(UIElement dragContent, UIElement placementTarget, object barDragObject, UIElement sourceElement) {
			this.barDragObject = barDragObject;
			SourceElement = sourceElement;
			Child = dragContent;
			PlacementTarget = placementTarget;
			Placement = PlacementMode.Absolute;
			isDestroyed = false;
			CurrentDragPopup = this;
		}
		protected virtual bool CoerceIsOpen(bool baseValue) {
			return baseValue && !isDestroyed;
		}
		protected UIElement SourceElement { get; set; }
		protected virtual void OnManagerChanged() { }
		public object BarDragObject { get { return barDragObject; } }
		#region IDragElement Members
		void IDragElement.Destroy() {
			DestroyCurrentPopup();
		}
		void DestroyCurrentPopup() {
			CurrentDragPopup = null;
		}
		void IDragElement.UpdateLocation(Point newPos) {
			Point offset = new Point();
			HorizontalOffset = newPos.X + CursorOffset.X - offset.X;
			VerticalOffset = newPos.Y + CursorOffset.Y - offset.Y;
		}
		#endregion
		public Point CursorOffset {
			get { return (Point)GetValue(CursorOffsetProperty); }
			set { SetValue(CursorOffsetProperty, value); }
		}
		protected internal virtual void Recreate() {
			IsOpen = false;
			IsOpen = true;
		}
		protected internal virtual void OnDragTypeChanged(DependencyPropertyChangedEventArgs e) {
			BarDragProvider.SetDragType(Child, (DragType)e.NewValue);
		}
	}
	public class BarItemDragElementContent : ContentControl {
		public BarItemDragElementContent() {
			DefaultStyleKey = typeof(BarItemDragElementContent);
			Loaded += new RoutedEventHandler(OnLoaded);
			IsHitTestVisible = false;
		}
		public BarItemDragElementContent(BarItem item) : this() {
			DataContext = item;
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			if(System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted) {
				VisualStateManager.GoToState(this, "BrowserHosted", false);
			}
		}
		protected DragIconControl DragIconControl { get; set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			DragIconControl = (DragIconControl)GetTemplateChild("PART_DragIcon");
		}
		protected internal virtual void OnDragTypeChanged(DependencyPropertyChangedEventArgs e) {
			if(DragIconControl != null)
				DragIconControl.DragType = BarDragProvider.GetDragType(this);
		}
	}
}
