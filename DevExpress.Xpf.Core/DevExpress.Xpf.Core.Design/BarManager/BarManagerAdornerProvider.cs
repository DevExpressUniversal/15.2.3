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
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Services;
using Platform::DevExpress.Xpf.Bars;
using System.Windows.Media;
using DevExpress.Mvvm.UI;
#if SILVERLIGHT
using Platform::DevExpress.Xpf.Core;
using FrameworkElement = Platform::System.Windows.FrameworkElement;
using UIElement = Platform::System.Windows.UIElement;
using VisualTreeHelper = Platform::System.Windows.Media.VisualTreeHelper;
using KeyGesture = System.Windows.Input.KeyGesture;
#endif
namespace DevExpress.Xpf.Core.Design {
	public abstract class BarManagerAdornerProviderBase : AdornerProvider {
		protected ModelItem AdornedElement {
			get { return adornedElement; }
			private set {
				if(adornedElement == value) return;
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
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class DesignerHitTestInfoAttribute : Attribute {
		public Type ProviderType { get; private set; }
		public DesignerHitTestInfoAttribute(Type providerType) {
			if(providerType == null)
				throw new ArgumentNullException();
			ProviderType = providerType;
		}
	}
	public interface IModelItemProvider {
		object GetModelItemObject(UIElement element);
	}
	[UsesItemPolicy(typeof(SmartTagAdornerSelectionPolicy))]
	public class HookAdornerProvider : BarManagerAdornerProviderBase {
		protected DesignerView DesignerView {
			get { return designerView;}
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
			if(oldValue != null) {
				Mouse.RemovePreviewMouseDownHandler(oldValue, new MouseButtonEventHandler(OnDesignerViewPreviewMouseDown));
			}
			if(DesignerView != null) {
				Mouse.AddPreviewMouseDownHandler(DesignerView, new MouseButtonEventHandler(OnDesignerViewPreviewMouseDown));
			}
		}
		void OnDesignerViewPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if(Mouse.DirectlyOver is DependencyObject) {
				DependencyObject root = LayoutTreeHelper.GetVisualParents(Mouse.DirectlyOver as DependencyObject).LastOrDefault();
				if(root != null && root.GetType().Name == "PopupRoot")
					return;
			}
			if(DesignerView.RootView == null)
				return;
			object hitTestObject = GetHitTestInfo(e);
			ModelItem itemToSelect = BarManagerDesignTimeHelper.GetModelItem(AdornedElement, hitTestObject);
			if(itemToSelect == null)
				return;
			SelectionOperations.SelectOnly(Context, itemToSelect);
			e.Handled = true;
		}
		object GetHitTestInfo(MouseButtonEventArgs e) {
			Point hitTestPoint = GetHitTestPoint(e);
			if(IsClickedOnAdorner(e))
				return null;
			ViewItem target = GetHitTestTarget(hitTestPoint);
			hitTestPoint = DesignerView.RootView.TransformToView(target).Transform(hitTestPoint);
			IEnumerable<ViewItem> elements = HitTest(target, hitTestPoint);
			return GetItemForSelect(elements);
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
			if(hitTestResult != null) {
				return hitTestResult.ViewHit;
			}
			return AdornedElement.View;
		}
		object GetItemForSelect(IEnumerable<ViewItem> children) {
			object result = null;
			for(int i = children.Count() - 1; i >= 0 && result == null; i--) {
				var child = children.ElementAt(i);
				result = GetModelItemObject(child);
			}
			return result;
		}
		object GetModelItemObject(ViewItem child) {
			object result = null;
			var attributes = DevExpress.Xpf.Core.Design.SmartTags.AttributeHelper.GetAttributes<DesignerHitTestInfoAttribute>(child.ItemType);
			foreach(var attr in attributes) {
				var provider = Activator.CreateInstance(attr.ProviderType) as IModelItemProvider;
				result = provider.GetModelItemObject(child.PlatformObject as UIElement);
				if(result != null)
					break;
			}
			return result;
		}
		IEnumerable<ViewItem> HitTest(ViewItem viewItem, Point hitTestPoint) {
			if(SkipItem(viewItem) || !IsPointInSize(viewItem.RenderSizeBounds.Size, hitTestPoint))
				return Enumerable.Empty<ViewItem>();
			List<ViewItem> children = new List<ViewItem>();
			children.Add(viewItem);
			children.AddRange(viewItem.VisualChildren.SelectMany(child => HitTest(child, viewItem.TransformToView(child).Transform(hitTestPoint))));
			children.TrimExcess();
			return children;
		}
		bool IsPointInSize(Size size, Point point) {
			Rect rect = new Rect(new Point(), size);
			return rect.Contains(point);
		}
		bool SkipItem(ViewItem viewItem) {
			var elem = viewItem.PlatformObject as UIElement;
			return !(elem != null && elem.IsVisible && elem.Opacity > 0d) || viewItem.Visibility != Visibility.Visible;
		}
		bool IsClickedOnAdorner(MouseButtonEventArgs e) {
			if(DesignerView.Adorners.Any(adorner => adorner.InputHitTest(e.GetPosition(adorner)) != null))
				return true;
			if(Mouse.Captured is UIElement)
				return ((UIElement)Mouse.Captured).InputHitTest(e.GetPosition(Mouse.Captured)) != null;
			var source = e.Source as UIElement;
			var adornerPanel = source == null ? null : AdornerPanel.FromVisual((UIElement)e.Source);
			if(adornerPanel != null) {
				return DesignerView.Adorners.Contains(adornerPanel);
			}
			return false;
		}
		DesignerView designerView;
	}
	[UsesItemPolicy(typeof(SmartTagAdornerSelectionPolicy))]
	class BarManagerAdornerProvider : BarManagerAdornerProviderBase {
		public BarManagerAdornerProvider() {
			barItems = new List<ModelItem>();
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			if(Context != null) {
				ModelService service = Context.Services.GetRequiredService<ModelService>();
				service.ModelChanged -= OnModelChanged;
				service.ModelChanged += OnModelChanged;
			}
		}
		protected override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			if(Context != null) {
				ModelService service = Context.Services.GetRequiredService<ModelService>();
				service.ModelChanged -= OnModelChanged;
			}
		}
		protected override void Deactivate() {
			if(Context.Items.GetValue<Selection>().PrimarySelection == null) {
				RemoveBarItemLinks(GetRemovedBarItems(), AdornedElement);
			}
			base.Deactivate();
		}
		protected override void OnAdornedElementChanged(ModelItem oldValue) {
			base.OnAdornedElementChanged(oldValue);
			barItems = GetBarItemsSnapshot(AdornedElement);
		}
		protected virtual void OnModelChanged(object sender, ModelChangedEventArgs e) {
			barItems = GetBarItemsSnapshot(AdornedElement);
			foreach(ModelProperty property in e.PropertiesChanged) {
				if(property.Name == "Name" && property.Parent.IsItemOfType(typeof(BarItem))) {
					BarManagerDesignTimeHelper.RenameBarItemLinks(property.Parent);
				}
			}
			if(IsBarItemRemoved(e.ItemsRemoved)) {
				ModelItem barManager = null;
				foreach(ModelProperty property in e.PropertiesChanged) {
					if(property.Parent.IsItemOfType(typeof(BarManager))) barManager = property.Parent;
					if(barManager != null)
						break;
				}
				if(IsBarManagerRemoved(e.ItemsRemoved) || barManager == null || IsRemovedByComment())
					return;
				RemoveBarItemLinks(e.ItemsRemoved, barManager);
			}
#if !SL
			CheckUpdateNewItemsLayoutVersion(e.ItemsAdded);
#endif
		}
#if !SL
		private static void CheckUpdateNewItemsLayoutVersion(IEnumerable<ModelItem> itemsAdded) {
			if(itemsAdded == null)
				return;
			foreach(ModelItem item in itemsAdded) {
				if(!item.IsItemOfType(typeof(Bar)) && !item.IsItemOfType(typeof(BarItemLinkBase)))
					continue;
				ModelItem barManager = BarManagerDesignTimeHelper.FindBarManagerInParent(item);
				PropertyIdentifier layoutVersionProp = new PropertyIdentifier(typeof(DevExpress.Xpf.Core.Serialization.DXSerializer), "LayoutVersion");
				if(barManager == null || barManager.Properties[layoutVersionProp].Value == null)
					continue;
				PropertyIdentifier addNewItemsProp = new PropertyIdentifier(typeof(BarManager), "AddNewItems");
				string vers = Convert.ToString(barManager.Properties[layoutVersionProp].ComputedValue);
				bool addNewItems = (bool)barManager.Properties[addNewItemsProp].ComputedValue;
				if(!string.IsNullOrEmpty(vers) && addNewItems) {
					bool allowSetValue = true;
					if(item.IsItemOfType(typeof(BarItemLinkBase))) {
						ModelItem parentLinksHolder = BarManagerDesignTimeHelper.FindParentByType<ILinksHolder>(item);
						if(parentLinksHolder != null && Convert.ToString(barManager.Properties[layoutVersionProp].ComputedValue) == vers) {
							allowSetValue = false;
						}
					}
					if(allowSetValue)
						item.Properties[layoutVersionProp].SetValue(vers);
				}
			}
		}
#endif
		void RemoveBarItemLinks(IEnumerable<ModelItem> removedItems, ModelItem barManager) {
			foreach(ModelItem barItem in removedItems) {
				if(!barItem.IsItemOfType(typeof(BarItem)))
					continue;
				using(ModelEditingScope scope = barManager.BeginEdit("Delete BarItemLinks")) {
					foreach(ModelItem link in BarManagerDesignTimeHelper.GetBarItemLinks(barItem)) {
						BarManagerDesignTimeHelper.RemoveBarItemLink(link);
					}
					scope.Complete();
				}
			}
		}
		bool IsBarManagerRemoved(IEnumerable<ModelItem> removedItems) {
			foreach(ModelItem item in removedItems) {
				if(item.IsItemOfType(typeof(BarManager))) return true;
			}
			return false;
		}
		bool IsBarItemRemoved(IEnumerable<ModelItem> removedItems) {
			foreach(ModelItem item in removedItems) {
				if(item.IsItemOfType(typeof(BarItem))) return true;
			}
			return false;
		}
		bool IsRemovedByComment() {
			System.Diagnostics.StackTrace stack = new System.Diagnostics.StackTrace(Thread.CurrentThread, false);
			return stack.GetFrames().Length == 31;
		}
		List<ModelItem> GetBarItemsSnapshot(ModelItem AdornedElement) {
			List<ModelItem> items = new List<ModelItem>();
			if(AdornedElement == null)
				return items;
			items.AddRange(AdornedElement.Properties["Items"].Collection);
			return items;
		}
		List<ModelItem> GetRemovedBarItems() {
			List<ModelItem> currentItems = GetBarItemsSnapshot(AdornedElement);
			return barItems.Except(currentItems).ToList();
		}
		List<ModelItem> barItems;
	}
	public class BarsModelItemProvider : IModelItemProvider {
		static Func<UIElement, object> additionalProvider;
		public static void RegisterAdditionalProvider(Func<UIElement, object> provider) {
			additionalProvider = provider;
		}
		public static void UnregisterAdditionalProvider() {
			additionalProvider = null;
		}
		public object GetModelItemObject(UIElement element) {
			object result = null;
			if(additionalProvider != null && (result = additionalProvider(element)) != null)
				return result;
			if(element is BarControl)
				return ((BarControl)element).Bar;
			else if(element is BarItemLinkInfo) {
				return ((BarItemLinkInfo)element).Item ?? (object)((BarItemLinkInfo)element).Link;
			}
			return result;
		}
	}
	class GalleryModelItemProvider : IModelItemProvider {
		public object GetModelItemObject(UIElement element) {
			if(element is GalleryControl)
				return ((GalleryControl)element).Gallery;
			if(element is GalleryItemGroupControl)
				return ((GalleryItemGroupControl)element).Group;
			if(element is GalleryItemControl)
				return ((GalleryItemControl)element).Item;
			return null;
		}
	}
	public abstract class DeleteItemTaskProviderBase : TaskProvider {
		public override bool IsToolSupported(Tool tool) {
			return true;
		}
		public DeleteItemTaskProviderBase() {
			Tasks.Add(new DeleteItemTask() { DeleteAction = OnDeleteKeyPressed });
		}
		protected virtual void OnDeleteKeyPressed(ModelItem selectedItem) { }
	}
	public class DeleteItemTask : Task {
		public Action<ModelItem> DeleteAction { get; set; }
		RoutedCommand Delete { get; set; }
		public DeleteItemTask() {
			Delete = new RoutedCommand();
			InputBindings.Add(new KeyBinding(Delete, new KeyGesture(Key.Delete)));
			CommandBindings.Add(new CommandBinding(Delete, OnDelete));
		}
		public void OnDelete(object sender, ExecutedRoutedEventArgs e) {
			if(DeleteAction == null) return;
			GestureData data = e.Parameter as GestureData;
			if(data == null) return;
			ModelItem parent = ModelParent.FindParent(data.TargetModel.ItemType, data);
			if(parent != null) {
				AdapterService adapterService = parent.Context.Services.GetService<AdapterService>();
				ParentAdapter adapter = adapterService.GetAdapter<ParentAdapter>(parent.ItemType);
				adapter.RemoveParent(parent, null, data.TargetModel);
			} else
				DeleteAction(data.TargetModel);
		}
	}
}
