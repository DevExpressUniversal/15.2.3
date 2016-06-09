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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Docking.Base;
using System.Diagnostics;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Docking.Design {
	static class ModelServiceHelper {
		public static BaseLayoutItem[] Convert(IEnumerable<ModelItem> modelItems) {
			List<BaseLayoutItem> items = new List<BaseLayoutItem>();
			foreach(ModelItem modelItem in modelItems) {
				BaseLayoutItem item = modelItem.As<BaseLayoutItem>();
				if(item != null)
					items.Add(item);
			}
			return items.ToArray();
		}
		public static ModelItem FindModelItem<T>(EditingContext context, object item) {
			IEnumerable<ModelItem> list = Find<T>(context);
			foreach(ModelItem modelItem in list) {
				if(item == modelItem.GetCurrentValue())
					return modelItem;
			}
			return null;
		}
		public static ModelItem FindModelItem(EditingContext context, BaseLayoutItem item) {
			ModelService service = GetService(context);
			return service.Find(service.Root, item);
		}
		public static ModelItem Find(this ModelService modelService, ModelItem startingItem, object platformObject) {
			IEnumerable<ModelItem> items = modelService.Find(startingItem, platformObject.GetType());
			return items.FirstOrDefault<ModelItem>(item => item.GetCurrentValue() == platformObject);
		}
		public static IEnumerable<ModelItem> Find<T>(EditingContext context) {
			ModelService service = GetService(context);
			return (service != null) ? service.Find(service.Root, typeof(T)) : null;
		}
		public static ModelService GetService(EditingContext context) {
			return context.Services.GetService<ModelService>();
		}
	}
	static class DockLayoutManagerDesignServiceHelper {
		public static DockLayoutManagerDesignService GetService(EditingContext context) {
			return context.Services.GetService<DockLayoutManagerDesignService>();
		}
	}
	internal static class DesignTimeHelper {
		class VersionHelper {
			public static readonly VersionHelper Instance = new VersionHelper();
			int vsVersion;
			int VSVersion {
				get {
					if(vsVersion == 0) vsVersion = Process.GetCurrentProcess().MainModule.FileVersionInfo.FileMajorPart;
					return vsVersion;
				}
			}
			public bool IsVs2012OrGreater {
				get { return VSVersion > 10; }
			}
		}
		public static bool IsVS2012OrGreater {
			get { return VersionHelper.Instance.IsVs2012OrGreater; }
		}
		static int mouseUpRerised;
		static int mouseDownRerised;
		static int mouseMoveRerised;
		public static void ReRaiseMouseDown(MouseEventArgs e, LayoutElementHitInfo hi) {
			if(mouseDownRerised > 0) return;
			mouseDownRerised++;
			HitTestResult result = hi.Tag as HitTestResult;
			if(result == null) return;
			ReraiseEventHelper.ReraiseEvent<MouseButtonEventArgs>(e as MouseButtonEventArgs, result.VisualHit as UIElement,
					FrameworkElement.PreviewMouseDownEvent, FrameworkElement.MouseDownEvent,
					ReraiseEventHelper.CloneMouseButtonEventArgs
				);
			mouseDownRerised--;
			e.Handled = true;
		}
		public static void ReRaiseMouseUp(MouseEventArgs e, LayoutElementHitInfo hi) {
			if(mouseUpRerised > 0) return;
			mouseUpRerised++;
			HitTestResult result = hi.Tag as HitTestResult;
			if(result == null) return;
			ReraiseEventHelper.ReraiseEvent<MouseButtonEventArgs>(e as MouseButtonEventArgs, result.VisualHit as UIElement,
					FrameworkElement.PreviewMouseUpEvent, FrameworkElement.MouseUpEvent,
					ReraiseEventHelper.CloneMouseButtonEventArgs
				);
			mouseUpRerised--;
			e.Handled = true;
		}
		public static void ReRaiseMouseMove(MouseEventArgs e, LayoutElementHitInfo hi) {
			if(mouseMoveRerised > 0) return;
			mouseMoveRerised++;
			HitTestResult result = hi.Tag as HitTestResult;
			if(result == null) return;
			ReraiseEventHelper.ReraiseEvent<MouseEventArgs>(e, result.VisualHit as UIElement,
					FrameworkElement.PreviewMouseMoveEvent, FrameworkElement.MouseMoveEvent,
					ReraiseEventHelper.CloneMouseEventArgs
				);
			mouseMoveRerised--;
			e.Handled = true;
		}
		public static UIElement GetSelectionElement(string text) {
			return new Border()
			{
				BorderBrush = new SolidColorBrush(Color.FromArgb(80, 255, 0, 0)),
				BorderThickness = new System.Windows.Thickness(4),
				IsHitTestVisible = false,
				CornerRadius = new CornerRadius(2),
				Margin = new System.Windows.Thickness(-2),
				Child = new TextBlock()
				{
					Text = text,
					Foreground = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0)),
					TextWrapping = System.Windows.TextWrapping.Wrap,
					Margin = new System.Windows.Thickness(8),
					TextAlignment = System.Windows.TextAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				}
			};
		}
		internal static void TranslatePointToPlatform(FrameworkElement adorner, ref Point p) {
			DesignerView designerView = DesignerView.GetDesignerView(adorner);
			double x, y;
			ResolutionHelper.GetTransformCoefficient(designerView, out x, out y);
			p.X *= x; p.Y *= y;
			double zoomLevel = designerView.ZoomLevel;
			p.X /= zoomLevel;
			p.Y /= zoomLevel;
		}
		public static bool GetIsWizardEnabled() {
			return DevExpress.Xpf.CreateLayoutWizard.SharedMemoryDataHelper.GetSharedData().IsWizardEnabled;
		}
	}
	class MouseUtil {
		[System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
		static extern bool GetCursorPos([System.Runtime.InteropServices.In, System.Runtime.InteropServices.Out] POINT pt);
		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		class POINT {
			internal int x;
			internal int y;
		}
		public static Point Position {
			get {
				POINT pt = new POINT();
				GetCursorPos(pt);
				return new Point(pt.x, pt.y);
			}
		}
	}
	static class DockExtensions {
		public static Orientation ToOrientation(this Dock dock) {
			switch(dock) {
				case Dock.Left:
				case Dock.Right:
					return Orientation.Horizontal;
				default:
					return Orientation.Vertical;
			}
		}
		public static InsertType ToInsertType(this Dock dock) {
			switch(dock) {
				case Dock.Left:
				case Dock.Top:
					return InsertType.Before;
				default:
					return InsertType.After;
			}
		}
		public static MoveType ToMoveType(this DockType type) {
			switch(type) {
				case DockType.Bottom:
					return MoveType.Bottom;
				case DockType.Fill:
					return MoveType.InsideGroup;
				case DockType.Left:
					return MoveType.Left;
				case DockType.Right:
					return MoveType.Right;
				case DockType.Top:
					return MoveType.Top;
				default:
					return MoveType.None;
			}
		}
		public static DockType ToDockType(this LayoutPanelDock dock) {
			switch(dock) {
				case LayoutPanelDock.Left:
					return DockType.Left;
				case LayoutPanelDock.Right:
					return DockType.Right;
				case LayoutPanelDock.Top:
					return DockType.Top;
				case LayoutPanelDock.Bottom:
					return DockType.Bottom;
				case LayoutPanelDock.Fill:
					return DockType.Fill;
				default:
					return DockType.None;
			}
		}
	}
	static class DockLayoutManagerExtensions {
		public static void EnsureLayoutRoot(this DockLayoutManager manager) {
			if(manager != null && manager.LayoutRoot == null)
				manager.LayoutRoot = new LayoutGroup();
		}
	}
	static class LayoutItemsExtensions {
		public static bool AcceptFill(this BaseLayoutItem item, LayoutTypes newItemType) {
			bool isLayoutPanel = newItemType == LayoutTypes.LayoutPanel;
			bool isDocumentPanel = newItemType == LayoutTypes.DocumentPanel;
			LayoutItemType type = item.ItemType;
			if((type == LayoutItemType.Panel || type == LayoutItemType.TabPanelGroup) && isDocumentPanel)
				return false;
			if((type == LayoutItemType.Document || type == LayoutItemType.DocumentPanelGroup) && isLayoutPanel)
				return false;
			if(item.IsLayoutItem() && item.ItemType != LayoutItemType.Group)
				return false;
			return true;
		}
		public static bool IsLayoutItem(this BaseLayoutItem item) {
			if(item == null) return false;
			switch(item.ItemType) {
				case LayoutItemType.ControlItem:
				case LayoutItemType.FixedItem:
				case LayoutItemType.Label:
				case LayoutItemType.LayoutSplitter:
				case LayoutItemType.Separator:
				case LayoutItemType.EmptySpaceItem:
					return true;
				case LayoutItemType.Group:
					return item.IsControlItemsHost;
			}
			return false;
		}
		public static bool AcceptSideDock(this BaseLayoutItem item) {
			return !(item.Parent == null);
		}
		public static IList<BaseLayoutItem> GetChildren(this LayoutGroup group) {
			List<BaseLayoutItem> list = new List<BaseLayoutItem>();
			list.AddRange(group.Items);
			return new System.Collections.ObjectModel.ReadOnlyCollection<BaseLayoutItem>(list);
		}
	}
	static class ImageUriHelper {
		public const string AddDocument = "ContextMenu/AddDocument";
		public const string DeleteGroup = "ContextMenu/DeleteGroup";
		public const string Group = "ContextMenu/Group";
		public const string HideDocument = "ContextMenu/HideDocument";
		public const string HideItem = "ContextMenu/HideItem";
		public const string RemoveAll = "ContextMenu/RemoveAll";
		public const string RemoveDocument = "ContextMenu/RemoveDocument";
		public const string RemoveItem = "ContextMenu/RemoveItem";
		public const string Reset = "ContextMenu/Reset";
		public const string NewGroup = "NewItem/Group";
		public const string NewItem = "NewItem/Item";
		public const string NewPanel = "NewItem/Panel";
		public const string NewTab = "NewItem/Tab";
		public const string NewTabbedGroup = "NewItem/TabbedGroup";
		public static Uri GetImageUri(string image) {
			Assembly currentAssembly = Assembly.GetExecutingAssembly();
			string assemblyName = currentAssembly.GetName().Name;
			string path = string.Format("pack://application:,,,/{0};component/Images/{1}.png", assemblyName, image);
			return new Uri(path);
		}
	}
	public static class ResolutionHelper {
		public static void GetTransformCoefficient(DependencyObject dependencyObject, out double resX, out double resY) {
			Matrix matrix = PresentationSource.FromDependencyObject(dependencyObject).CompositionTarget.TransformToDevice;
			resX = matrix.M11;
			resY = matrix.M22;
		}
	}
	static class IDockControllerExtensions {
		public static void RemoveAll(this IDockController controller, LayoutGroup group) {
			if(group == null) return;
			using(new NotificationBatch(controller.Container)) {
				group.Items.Clear();
				NotificationBatch.Action(controller.Container, group.GetRoot());
			}
		}
	}
	static class ModelItemCollectionExtension {
		public static void AddRange(this ModelItemCollection collection, IEnumerable<ModelItem> items) {
			foreach(ModelItem item in items) {
				collection.Add(item);
			}
		}
		public static void RemoveAll(this ModelItemCollection collection) {
			var items = collection.ToArray();
			foreach(var item in items) {
				collection.Remove(item);
			}
		}
	}
	static class ModelItemExtension {
		public static void SetValueIfNotEqual(this ModelProperty property, object value) {
			object currentValue = !property.IsSet && property.DefaultValue != null ? property.DefaultValue : property.ComputedValue;
			if(!currentValue.Equals(value))
				property.SetValue(value);
		}
		public static void ResetLayout(this ModelItem item) {
			using(ModelEditingScope editingScope = item.BeginEdit(DockingLocalizer.GetString(DockingStringId.ResetLayoutOperation))) {
				item.Properties["HorizontalAlignment"].ClearValue();
				item.Properties["VerticalAlignment"].ClearValue();
				item.Properties["Margin"].ClearValue();
				item.Properties["Width"].ClearValue();
				item.Properties["Height"].ClearValue();
				editingScope.Complete();
			}
		}
		public static ModelItemCollection ItemsProperty(this ModelItem item) {
			return item.Properties["Items"].Collection;
		}
		public static bool Is<T>(this ModelItem item) {
			if(item == null) return false;
			return typeof(T).IsAssignableFrom(item.ItemType);
		}
		public static T As<T>(this ModelItem item) where T : class {
			if(item == null) return null;
			return item.GetCurrentValue() as T;
		}
		public static bool IsLayoutRoot(this ModelItem item) {
			if(item == null) return false;
			return (item.Parent.Is<DockLayoutManager>() && item.Parent.Content.Value.Equals(item));
		}
		internal static void CopyProperties(this ModelItem modelItem, BaseLayoutItem layoutItem) {
			PropertyDescriptor[] properties = GetProperties(layoutItem.GetType());
			foreach(PropertyDescriptor pd in properties) {
				if(object.Equals(modelItem.Properties[pd.Name].ComputedValue, pd.GetValue(layoutItem))) continue;
				try {
					if(!pd.ShouldSerializeValue(layoutItem))
						modelItem.Properties[pd.Name].ClearValue();
					else
						modelItem.Properties[pd.Name].SetValue(pd.GetValue(layoutItem));
				}
				catch {
				}
			}
		}
		public static ModelItem GetDockLayoutManager(this ModelItem model) {
			ModelItem parent = model;
			while(parent != null) {
				if(parent.Is<DockLayoutManager>())
					return parent;
				parent = parent.Parent;
			}
			return parent;
		}
		internal static void ResetUnneededProperties(ModelItem item) {
		}
		static Dictionary<Type, PropertyDescriptor[]> propertyCache = new Dictionary<Type, PropertyDescriptor[]>();
		static PropertyDescriptor[] GetProperties(Type type) {
			PropertyDescriptor[] result;
			if(!propertyCache.TryGetValue(type, out result)) {
				var properties = TypeDescriptor.GetProperties(type);
				List<PropertyDescriptor> props = new List<PropertyDescriptor>();
				foreach(PropertyDescriptor pd in properties) {
					if(pd.IsReadOnly) continue;
					if(pd.Name == "Items") continue;
					if(pd.Name == "ParentName") continue;
					if(pd.Name == "ParentCollectionName") continue;
					props.Add(pd);
				}
				result = props.ToArray();
				propertyCache.Add(type, result);
			}
			return result;
		}
	}
	internal static class DesignerState {
		class DesignTimeStateDictionary : Dictionary<object, object> { }
		class DesignerStateDictionary : Dictionary<ModelItem, DesignTimeStateDictionary> { }
		public static void ClearDesigneTimeProperty<T>(this ModelItem item, DesigneTimeProperty<T> property) {
			DesignerStateDictionary dictionary = item.Context.Services.GetRequiredService<DesignerStateDictionary>();
			if(dictionary == null) return;
			DesignTimeStateDictionary table;
			if(dictionary.TryGetValue(item, out table))
				table.Remove(property);
			if(table != null && table.Count == 0)
				dictionary.Remove(item);
		}
		public static T GetDesignTimeProperty<T>(this ModelItem item, DesigneTimeProperty<T> property) {
			return GetDesignTimeProperty<T>(item, property, null);
		}
		public static T GetDesignTimeProperty<T>(this ModelItem item, DesigneTimeProperty<T> property, Func<ModelItem, T> factory) {
			DesignerStateDictionary dictionary = item.Context.Services.GetService<DesignerStateDictionary>();
			if(dictionary == null) {
				dictionary = new DesignerStateDictionary();
				item.Context.Services.Publish<DesignerStateDictionary>(dictionary);
			}
			DesignTimeStateDictionary table;
			if(dictionary.TryGetValue(item, out table)) {
				object value;
				if(table.TryGetValue(property, out value)) {
					return (T)value;
				}
				else if(factory != null) {
					T v = factory(item);
					table[property] = v;
					return v;
				}
			}
			return default(T);
		}
		public static void SetDesignTimeProperty<T>(this ModelItem item, DesigneTimeProperty<T> property, T value) {
			DesignerStateDictionary dictionary = item.Context.Services.GetService<DesignerStateDictionary>();
			if(dictionary == null) {
				dictionary = new DesignerStateDictionary();
				item.Context.Services.Publish<DesignerStateDictionary>(dictionary);
			}
			DesignTimeStateDictionary table;
			if(!dictionary.TryGetValue(item, out table)) {
				table = new DesignTimeStateDictionary();
				dictionary[item] = table;
			}
			table[property] = value;
		}
	}
	internal class DesigneTimeProperty<T> {
		string Name { get; set; }
		internal DesigneTimeProperty(string name) {
			Name = name;
		}
		public override string ToString() {
			return Name;
		}
	}
	internal class PropertyUtil {
		internal static void InvalidateProperty(ModelItem item, PropertyIdentifier propertyIdentifier) {
			item.Context.Services.GetRequiredService<ValueTranslationService>().InvalidateProperty(item, propertyIdentifier);
		}
	}
	static class ToolTipHelper {
		public static readonly string AddGroup = "Add a new group";
		public static readonly string AddPanel = "Add a new panel";
		public static readonly string AddDocument = "Add a new document";
		public static readonly string AddDocumentGroup = "Add a new document group";
		public static readonly string AddItem = "Add a new layout item";
		public static string AddLayoutTab = "Add a new tabbed group";
	}
}
