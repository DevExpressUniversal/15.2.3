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
using System.Windows.Input;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.DragAndDrop;
using DevExpress.Data.Utils;
namespace DevExpress.Xpf.Bars {
	public class DragWidget : Thumb {
		FrameworkElement topElement = null;
		FrameworkElement subscribedTopElement = null;
		public DragWidget() { }
		public FrameworkElement TopElement {
			get { return topElement; }
			set {
				if (value == topElement) return;
				FrameworkElement oldValue = topElement;
				topElement = value;
				OnTopElementChanged(oldValue);
			}
		}
		protected virtual void OnTopElementChanged(FrameworkElement oldValue) {
			UnsubscribeTopElement();
			SubscribeTopElement(TopElement);
		}		
		public void StartDrag() {
			if(Visibility == Visibility.Collapsed)
				return;
			RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, DateTime.Now.Millisecond, MouseButton.Left) { RoutedEvent = DragWidget.MouseLeftButtonDownEvent });
			ClientPoint = new Point(FlowDirection == FlowDirection.RightToLeft ? ActualWidth - 10 : 10, ActualHeight / 2);
		}
		public Point OwnerPoint { get; set; }
		public Point DragOffset { get { return new Point(OwnerPoint.X - ClientPoint.X, OwnerPoint.Y - ClientPoint.Y); } }
		protected internal Point ClientPoint { get; set; }
		protected virtual Point GetOwnerPoint(MouseEventArgs e) {
			return e.GetPosition(TopElement);
		}		
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			ClientPoint = e.GetPosition(this);
			if (FlowDirection == FlowDirection.RightToLeft)
				ClientPoint = new Point(ActualWidth - ClientPoint.X, ClientPoint.Y);
			base.OnMouseLeftButtonDown(e);
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			CancelDrag();
		}
		protected override void StartDrag(MouseButtonEventArgs e) {			
			if(!IsDragging && AllowDrag)
				base.StartDrag(e);
		}
		protected internal MouseEventArgs MouseArgs { get; set; }
		protected override void OnMouseMove(MouseEventArgs e) {
			MouseArgs = e;
			OwnerPoint = GetOwnerPoint(e);
			base.OnMouseMove(e);
		}
		protected internal virtual void OnBarManagerChanged(DependencyPropertyChangedEventArgs e) {
			BarManager prev = e.OldValue as BarManager;
			BarManager curr = e.NewValue as BarManager;
			UnsubscribeTopElement();
			SubscribeTopElement(curr);
		}		  
		WeakEventHandler<FrameworkElement, KeyEventArgs, KeyEventHandler> previewKeyDownHandler;
		protected virtual void SubscribeTopElement(FrameworkElement topElement) {
			if (topElement == null) return;
			UnsubscribeTopElement();
			subscribedTopElement = topElement;
			topElement.PreviewKeyDown += (previewKeyDownHandler ?? (previewKeyDownHandler =
				new WeakEventHandler<FrameworkElement, KeyEventArgs, KeyEventHandler>(
					this,
					(sender, owner, args) => ((DragWidget)sender).OnPreviewKeyDown(args),
					(wh, owner) => ((FrameworkElement)owner).PreviewKeyDown -= wh.Handler,
					(wh) => wh.OnEvent))).Handler;
		}
		protected virtual void UnsubscribeTopElement() {
			if (subscribedTopElement == null || previewKeyDownHandler == null) return;
			subscribedTopElement.PreviewKeyDown -= previewKeyDownHandler.Handler;
			subscribedTopElement = null;
		}
		protected virtual void OnManagerOwnerPreviewKeyDown(object sender, KeyEventArgs e) {
			if(e.Key == Key.Escape) {
				CancelDrag();
			}
		}
	}
}
