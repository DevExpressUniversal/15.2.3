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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Services;
using DevExpress.Xpf.Layout.Core;
using DTSelection = Microsoft.Windows.Design.Interaction.Selection;
namespace DevExpress.Xpf.Docking.Design {
	[FeatureConnector(typeof(DockLayoutManagerDesignFeatureConnector))]
	[UsesItemPolicy(typeof(DockLayoutManagerPolicy))]
	public class DockLayoutManagerAdornerProvider : PrimarySelectionAdornerProvider {
		IDisposable actionSubscription;
		DockLayoutManagerDesignTimeModel designTimeModel;
		protected ModelItem adornedElement;
		public DockLayoutManager Manager { get; private set; }
		public DockLayoutManagerDesignTimeModel DesignTimeModel {
			get { return designTimeModel; }
		}
		public bool IsActive { get; private set; }
		protected override void Activate(ModelItem item) {
			base.Activate(item);
			adornedElement = item;
			Manager = EnsureManager(item);
			IsActive = Manager != null;
			if(!IsActive) return;
			EnsureDesignTimeModel();
			DesignTimeModel.ActualizeModel(Context, adornedElement, Manager);
			CreateCustomizationSurface();
			SubcribeEvents();
		}
		protected override void Deactivate() {
			if(IsActive) {
				UnSubcribeEvents();
				DestroyCustomizationSurface();
				IsActive = false;
			}
			base.Deactivate();
		}
		public override bool IsToolSupported(Tool tool) {
			return (tool is SelectionTool || tool is CreationTool);
		}
		DockLayoutManager EnsureManager(ModelItem modelItem) {
			return (modelItem.View != null) ? modelItem.View.PlatformObject as DockLayoutManager : null;
		}
		protected virtual void CreateCustomizationSurface() {
			HookPanel = CreateHookPanel();
			SelectionSurface = CreateSelectionSurface();
			Adorners.Add(HookPanel);
			SubcribeHookPanelEvents();
		}
		protected virtual void DestroyCustomizationSurface() {
			UnSubcribeHookPanelEvents();
			Adorners.Remove(HookPanel);
			Adorners.Remove(SelectionSurface);
			HookPanel.DataContext = null;
		}
		protected virtual void SubcribeEvents() {
			SubscribeKeyboardEvents();
			SubcribeDesignTimeModelEvents();
			SubcribeEditingContextEvents();
		}
		protected virtual void UnSubcribeEvents() {
			UnSubcribeEditingContextEvents();
			UnSubcribeDesignTimeModelEvents();
			UnSubscribeKeyboardEvents();
		}
		protected virtual void SubcribeDesignTimeModelEvents() {
			actionSubscription = NotificationHelper.Subscribe(Manager, DesignTimeModel);
		}
		protected virtual void UnSubcribeDesignTimeModelEvents() {
			Ref.Dispose(ref actionSubscription);
		}
		protected virtual void SubcribeEditingContextEvents() {
			Context.Items.Subscribe<DTSelection>(new SubscribeContextCallback<DTSelection>(OnSelectionChanged));
			Context.Services.GetService<ModelService>().ModelChanged += OnModelChanged;
		}
		protected virtual void UnSubcribeEditingContextEvents() {
			Context.Items.Unsubscribe<DTSelection>(new SubscribeContextCallback<DTSelection>(OnSelectionChanged));
			Context.Services.GetService<ModelService>().ModelChanged -= OnModelChanged;
		}
		protected void SubcribeHookPanelEvents() {
			HookPanel.MouseMove += AdornerPanel_MouseMove;
			HookPanel.MouseUp += AdornerPanel_MouseUp;
			HookPanel.PreviewMouseDown += AdornerPanel_PreviewMouseDown;
		}
		protected void UnSubcribeHookPanelEvents() {
			HookPanel.MouseUp -= AdornerPanel_MouseUp;
			HookPanel.PreviewMouseDown -= AdornerPanel_PreviewMouseDown;
			HookPanel.MouseMove -= AdornerPanel_MouseMove;
		}
		protected UIElement KeyboardHolder { get; private set; }
		protected void SubscribeKeyboardEvents() {
			UnSubscribeKeyboardEvents();
			KeyboardHolder = DesignerView.FromContext(Context);
			if(KeyboardHolder != null)
				KeyboardHolder.PreviewKeyDown += root_PreviewKeyDown;
		}
		protected void UnSubscribeKeyboardEvents() {
			if(KeyboardHolder != null) {
				KeyboardHolder.PreviewKeyDown -= root_PreviewKeyDown;
				KeyboardHolder = null;
			}
		}
		void DesignTimeRaiseEvent(LayoutElementHitInfo hitInfo, RoutedEventArgs e) {
			HitTestResult result = hitInfo.Tag as HitTestResult;
			if(result != null)
				Manager.GetCustomizationController().DesignTimeRaiseEvent(result.VisualHit, e);
		}
		protected UIElement GetRootElement() {
			UIElement result = HookPanel;
			for(UIElement element = HookPanel; element != null; element = VisualTreeHelper.GetParent(element) as UIElement)
				result = element;
			return result;
		}
		protected bool CanProcessMouseInput(MouseEventArgs e) {
			UIElement rootElement = GetRootElement();
			HitTestResult hitTestResult = VisualTreeHelper.HitTest(rootElement, e.GetPosition(rootElement));
			return hitTestResult.VisualHit == HookPanel;			
		}
		bool IsMousePressed;
		void AdornerPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if(!CanProcessMouseInput(e)) return;
			LayoutElementHitInfo hitInfo = CalcHitInfo(e);
			if(!hitInfo.InResizeBounds && !hitInfo.InContent)
				DesignTimeRaiseEvent(hitInfo, e);
			object selected = DesignTimeModel.Select(hitInfo);
			IsMousePressed = true;
		}
		void AdornerPanel_MouseUp(object sender, MouseButtonEventArgs e) {
			if(!IsMousePressed) return;
			LayoutElementHitInfo hitInfo = CalcHitInfo(e);
			if(!hitInfo.InResizeBounds && !hitInfo.InContent)
				DesignTimeRaiseEvent(hitInfo, e);
			object selected = DesignTimeModel.Select(hitInfo);
			IsMousePressed = false;
		}
		void AdornerPanel_MouseMove(object sender, MouseEventArgs e) {
			if(!IsMousePressed) return;
			LayoutElementHitInfo hitInfo = CalcHitInfo(e);
			DesignTimeRaiseEvent(hitInfo, e);
		}
		void root_PreviewKeyDown(object sender, KeyEventArgs e) {
		}
		protected AdornerPanel HookPanel { get; private set; }
		protected AdornerPanel CustomizationAdornerPanel { get; private set; }
		protected SelectionBorder SelectionBorder { get; private set; }
		protected FrameworkElement SelectionSurface { get; private set; }
		protected virtual AdornerPanel CreateHookPanel() {
			var panel = new AdornerPanel() { Background = System.Windows.Media.Brushes.Transparent };
			return panel;
		}
		protected virtual AdornerPanel CreateCustomizationAdornerPanel() {
			return new AdornerPanel() { IsContentFocusable = true };
		}
		private FrameworkElement CreateSelectionSurface() {
			Canvas canvas = new Canvas();
			SelectionBorder = new SelectionBorder() { Root = adornedElement };
			canvas.Children.Add(SelectionBorder);
			Adorners.Add(canvas);
			return canvas;
		}
		protected virtual CustomizationPanel CreateCustomizationPanel() {
			CustomizationPanel panel = new CustomizationPanel() { Model = DesignTimeModel };
			AdornerPanel.SetAdornerHorizontalAlignment(panel, AdornerHorizontalAlignment.OutsideRight);
			AdornerPanel.SetAdornerVerticalAlignment(panel, AdornerVerticalAlignment.OutsideTop);
			return panel;
		}
		void PART_DockTypeEditor_Activating(object sender, DockTypeEditorActivatingEventArgs e) {
			BaseLayoutItem item = DesignTimeModel.SelectedLayoutItem;
			if(item != null) {
				e.AreSideButtonsEnabled = !item.IsLayoutItem();
				e.IsFillButtonEnabled = item.AcceptFill(DesignTimeModel.SelectedType);
				e.AreCenterButtonsEnabled = item.AcceptSideDock();
			}
			if(DesignTimeModel.SelectedModelItem == null) {
				e.AreSideButtonsEnabled = false;
				e.IsFillButtonEnabled = true;
				e.AreCenterButtonsEnabled = false;
			}
		}
		void OnSelectionChanged(DTSelection newSelection) {
			SelectionBorder.PrimarySelection = newSelection.PrimarySelection;
			foreach(ModelItem selection in newSelection.SelectedObjects) {
				if(selection != null && selection.Is<BaseLayoutItem>()) {
					DesignTimeModel.SelectedModelItem = selection;
					return;
				}
			}
		}
		void OnModelChanged(object sender, ModelChangedEventArgs e) {
			bool wasAdding = (e.ItemsAdded != null) && e.ItemsAdded.Count() > 0;
			bool wasRemoving = (e.ItemsRemoved != null) && e.ItemsRemoved.Count() > 0;
			if(wasAdding || wasRemoving) {
				if(adornedElement.View != null && adornedElement.View.PlatformObject != Manager)
					ForceManagerUpdate();
			}
			Manager.Update();
			SelectionBorder.UpdateBounds();
		}
		void ForceManagerUpdate() {
			UnSubcribeEvents();
			Manager = EnsureManager(adornedElement);
			Manager.EnsureCustomizationRoot(Manager.LayoutRoot);
			EnsureDesignTimeModel();
			DesignTimeModel.ActualizeModel(Context, adornedElement, Manager);
			SubcribeEvents();
		}
		static Dictionary<DockLayoutManager, DockLayoutManagerDesignTimeModel> models;
		void EnsureDesignTimeModel() {
			if(models == null)
				models = new Dictionary<DockLayoutManager, DockLayoutManagerDesignTimeModel>();
			if(!models.TryGetValue(Manager, out designTimeModel)) {
				designTimeModel = CreateModel();
				models.Add(Manager, designTimeModel);
			}
			else
				designTimeModel.SelectedModelItem = null;
		}
		protected virtual DockLayoutManagerDesignTimeModel CreateModel() {
			return new DockLayoutManagerDesignTimeModel();
		}
		Point GetManagerMousePosition(MouseEventArgs e) {
			return e.GetPosition(Manager);
		}
		LayoutElementHitInfo CalcHitInfo(MouseEventArgs e) {
			return Manager.CalcHitInfo(GetManagerMousePosition(e));
		}
	}
}
