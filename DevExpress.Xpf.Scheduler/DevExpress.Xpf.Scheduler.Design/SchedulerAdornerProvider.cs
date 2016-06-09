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

#if DEBUG
extern alias Platform;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using Platform::DevExpress.Mvvm.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.Scheduler.Design {
	public abstract class SchedulerAdornerProviderBase : PrimarySelectionAdornerProvider {
		protected ModelItem AdornedElement {
			get { return adornedElement; }
			private set {
				if (adornedElement == value) return;
				var oldValue = adornedElement;
				adornedElement = value;
				OnAdornedElementChanged(oldValue);
			}
		}
		protected override void Activate(ModelItem item) {
			AdornedElement = item;
			SubscribeEvents();
#if SILVERLIGHT
#endif
			base.Activate(item);
		}
		protected override void Deactivate() {
			UnsubscribeEvents();
			base.Deactivate();
			AdornedElement = null;
		}
		protected virtual void SubscribeEvents() { }
		protected virtual void UnsubscribeEvents() { }
		protected virtual void OnAdornedElementChanged(ModelItem oldValue) { }
		ModelItem adornedElement;
	}
	public class SchedulerAdornerProvider : SchedulerAdornerProviderBase {
		DesignerView designerView;
		protected DesignerView DesignerView {
			get { return designerView; }
			private set {
				if (designerView == value) return;
				var oldValue = designerView;
				designerView = value;
				OnDesignerViewChanged(oldValue);
			}
		}
		protected override void Activate(ModelItem item) {
			DesignerView = DesignerView.FromContext(item.Context);
			base.Activate(item);
		}
		protected override void Deactivate() {
			DesignerView = null;
			base.Deactivate();
		}
		void OnDesignerViewChanged(DesignerView oldValue) {
			if (oldValue != null) {
				Mouse.RemovePreviewMouseDownHandler(oldValue, new MouseButtonEventHandler(OnDesignerViewPreviewMouseDown));
			}
			if (DesignerView != null) {
				Mouse.AddPreviewMouseDownHandler(DesignerView, new MouseButtonEventHandler(OnDesignerViewPreviewMouseDown));
			}
		}
		void OnDesignerViewPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (Mouse.DirectlyOver is DependencyObject) {
				DependencyObject root = LayoutTreeHelper.GetVisualParents(Mouse.DirectlyOver as DependencyObject).LastOrDefault();
				if (root != null && root.GetType().Name == "PopupRoot")
					return;
			}
			if (DesignerView.RootView == null)
				return;
			object hitTestObject = GetHitTestInfo(e);
		}
		object GetHitTestInfo(MouseButtonEventArgs e) {
			Point hitTestPoint = GetHitTestPoint(e);
			if (IsClickedOnAdorner(e))
				return null;
			return GetHitTestTarget(hitTestPoint);
		}
		Point GetHitTestPoint(MouseEventArgs e) {
			Point point = e.GetPosition(DesignerView);
			return DesignerView.RootView.TransformFromVisual(DesignerView).Transform(point);
		}
		ViewItem GetHitTestTarget(Point hitTestPoint) {
			var hitTestResult = DesignerView.RootView.HitTest(
					potentialHitTestTarget => {
						return HitTestFilterBehavior.Continue;
					},
					null, new PointHitTestParameters(hitTestPoint));
			if (hitTestResult != null) {
				return hitTestResult.ViewHit;
			}
			return AdornedElement.View;
		}
		bool IsClickedOnAdorner(MouseButtonEventArgs e) {
			if (DesignerView.Adorners.Any(adorner => adorner.InputHitTest(e.GetPosition(adorner)) != null))
				return true;
			if (Mouse.Captured is UIElement)
				return ((UIElement)Mouse.Captured).InputHitTest(e.GetPosition(Mouse.Captured)) != null;
			var source = e.Source as UIElement;
			var adornerPanel = source == null ? null : AdornerPanel.FromVisual((UIElement)e.Source);
			if (adornerPanel != null) {
				return DesignerView.Adorners.Contains(adornerPanel);
			}
			return false;
		}
	}
}
#endif
