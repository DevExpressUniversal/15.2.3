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

extern alias Platform;
using Platform::DevExpress.Xpf.Bars;
using System.Windows.Controls;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using Microsoft.Windows.Design.Services;
using Microsoft.Windows.Design.Features;
#if !SL
using System.Windows.Media;
#endif
namespace DevExpress.Xpf.Core.Design {
	public abstract class SmartTagAdornerBase : BarManagerAdornerProviderBase {
		public UserControl SmartTagControl { get; private set; }
		public ModelItem PrimarySelection {
			get { return primarySelection; }
			set {
				if(primarySelection == value) return;
				primarySelection = value;
				OnPrimarySelectionChanged();
			}
		}
		public Dictionary<Type, Type> ViewModelViewDictionary { get; private set; }
		protected SmartTagAdornerButton SmartTagButton { get; private set; }
		public DesignerView DesignerView { get; private set; }
		public Visibility SmartTagButtonVisibility {
			get { return smartTagButtonVisibilityCore; }
			set {
				if(smartTagButtonVisibilityCore == value) return;
				Visibility oldValue = smartTagButtonVisibilityCore;
				smartTagButtonVisibilityCore = value;
				OnSmartTagButtonVisibilityChanged(oldValue);
			}
		}
		protected AdornerPanel Panel {
			get {
				if(panel == null)
					panel = CreateAdornerPanel();
				return panel;
			}
		}
		public SmartTagAdornerBase() {
			SmartTagButton = new SmartTagAdornerButton();
			Canvas canvas = new Canvas();
			canvas.HorizontalAlignment = HorizontalAlignment.Stretch;
			canvas.VerticalAlignment = VerticalAlignment.Stretch;
			canvas.Children.Add(SmartTagButton);
			Panel.Children.Add(canvas);
		}
		protected abstract object GetViewModel(ModelItem primarySelection);
		protected override void Activate(ModelItem item) {
			base.Activate(item);
			DesignerView = DesignerView.FromContext(Context);
			Panel.SizeChanged += OnAdornerPanelSizeChanged;
			UpdateSmartTagButtonPosition();
		}
		protected override void Deactivate() {
			DesignerView = null;
			Panel.SizeChanged -= OnAdornerPanelSizeChanged;
			base.Deactivate();
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			SelectionOperations.Subscribe(Context, OnSelectionChanged);
		}
		protected override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			SelectionOperations.Unsubscribe(Context, OnSelectionChanged);
		}
		protected virtual AdornerPanel CreateAdornerPanel() {
			AdornerPanel panel = new AdornerPanel();
			panel.IsContentFocusable = true;
			AdornerProperties.SetOrder(panel, AdornerOrder.Foreground);
			Adorners.Add(panel);
			return panel;
		}
		protected virtual void OnAdornerPanelSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateSmartTagButtonPosition();
		}
		protected virtual void OnPrimarySelectionChanged() {
			SmartTagButton.IsPressed = false;
			UpdateSmartTagContent();
			UpdateSmartTagButtonPosition();
		}
		protected virtual void OnSmartTagButtonVisibilityChanged(Visibility oldValue) {
			if(SmartTagButtonVisibility == Visibility.Visible) {
				SmartTagButton.Opacity = 1;
				SmartTagButton.IsHitTestVisible = true;
			} else {
				SmartTagButton.Opacity = 0;
				SmartTagButton.IsHitTestVisible = false;
			}
		}
		int minVisibleWidth = 5;
		int minVisibleHeight = 10;
		protected virtual void UpdateSmartTagButtonPosition() {
			if(AdornedElement == null || DesignerView == null)
				return;
			Rect bounds = GetSelectedElementBounds();
			Rect maxBounds = GetAdornedElementBounds();
			Point buttonPosition = GetButtonPosition();
			Point position = new Point();
			double level = DesignerView.ZoomLevel;
			if(maxBounds.Right - bounds.Left < minVisibleWidth || maxBounds.Bottom - bounds.Top < minVisibleHeight || 
				bounds.Right - maxBounds.Left < minVisibleWidth || bounds.Bottom - maxBounds.Top < minVisibleHeight) {
				SmartTagButton.Visibility = Visibility.Collapsed;
			} else {
				SmartTagButton.Visibility = Visibility.Visible;
				double right = AdornedElement.View != null ? Math.Min(bounds.Right, maxBounds.Right) : bounds.Right;
				double top = AdornedElement.View != null ? Math.Max(bounds.Top, maxBounds.Top) : bounds.Top;
				position.X = right * level - buttonPosition.X;
				position.Y = top * level - buttonPosition.Y;
#if SILVERLIGHT
				double x, y;
				ResolutionHelper.GetTransformCoefficient(Adorners[0], out x, out y);
				position.X /= x;
				position.Y /= y;
#endif
				PositionButton(position);
			}
		}
		protected virtual Rect GetSelectedElementBounds() {
			return GetAdornedElementBounds();
		}
		protected Rect GetAdornedElementBounds() {
			return AdornedElement.View != null ? new Rect(AdornedElement.View.RenderSizeBounds.Size) : default(Rect);
		}
		Point buttonPosition = new Point(22, 7);
		protected virtual Point GetButtonPosition() {
			 return buttonPosition;
		}
		protected virtual void PositionButton(Point point) {
			Canvas.SetLeft(SmartTagButton, point.X);
			Canvas.SetTop(SmartTagButton, point.Y);
		}
		protected virtual void OnSelectionChanged(Selection newSelection) {
			PrimarySelection = newSelection.PrimarySelection;
		}
		protected virtual void UpdateSmartTagContent() {
			FrameworkElementSmartTagViewModel oldValue = SmartTagButton.DataContext as FrameworkElementSmartTagViewModel;
			if(oldValue != null && oldValue.PropertiesViewModel != null) {
				oldValue.PropertiesViewModel.Dispose();
			}
			UpdateSmartTagButtonVisibility();
			if(PrimarySelection != null && IsChild() && SmartTagButtonVisibility == Visibility.Visible) {
				FrameworkElementSmartTagViewModel viewModel = GetViewModel(PrimarySelection) as FrameworkElementSmartTagViewModel;
				SmartTagButton.DataContext = viewModel;
				SmartTagButtonVisibility = (viewModel != null && viewModel.PropertiesViewModel != null && viewModel.PropertiesViewModel.Lines.Count() > 0) ? Visibility.Visible : Visibility.Collapsed;
			} else
				SmartTagButton.DataContext = null;
		}
		protected void UpdateSmartTagButtonVisibility() {
			SmartTagButtonVisibility = GetSmartTagButtonVisibility();
		}
		protected virtual Visibility GetSmartTagButtonVisibility() {
			var desginerView = Context == null ? null : DesignerView.FromContext(Context);
			return PrimarySelection == null || desginerView == null || desginerView.RootView == null || IsInTemplate(PrimarySelection) ? Visibility.Collapsed : Visibility.Visible;
		}
		protected bool IsInTemplate(ModelItem item) {
			return BarManagerDesignTimeHelper.FindParentByType<FrameworkTemplate>(item) != null;
		}
		protected virtual bool IsChild() {
			if(AdornedElement == null)
				return false;
			return BarManagerDesignTimeHelper.FindParentByType(AdornedElement.ItemType, PrimarySelection) == AdornedElement;
		}
		ModelItem primarySelection;
		Visibility smartTagButtonVisibilityCore = Visibility.Visible;
		AdornerPanel panel;
	}
}
