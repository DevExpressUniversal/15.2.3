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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Docking.Platform;
using DevExpress.Xpf.Layout.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
namespace DevExpress.Xpf.Docking {
	static class SizeToContentHelper {
		public static Size FitSizeToContent(SizeToContent sizeToContent, Size prevSize, Size newSize) {
			Size size;
			switch(sizeToContent) {
				case SizeToContent.Height:
					size = new Size(newSize.Width, prevSize.Height);
					break;
				case SizeToContent.Width:
					size = new Size(prevSize.Width, newSize.Height);
					break;
				case SizeToContent.WidthAndHeight:
					size = new Size(prevSize.Width, prevSize.Height);
					break;
				default:
					size = newSize;
					break;
			}
			return size;
		}
	}
	internal static class EnvironmentHelper {
#if DEBUGTEST
		public static bool? IsWinSevenOverride { get; set; }
#endif
		public static bool IsWinSevenOrLater {
			get {
#if DEBUGTEST
				if(IsWinSevenOverride.HasValue) return IsWinSevenOverride.Value;
#endif
				var version = Environment.OSVersion.Version;
				return (version.Major >= 6) && (version.Minor >= 1);
			}
		}
		public static bool IsWinXP {
			get {
				var version = Environment.OSVersion.Version;
				return (version.Major == 5) && (version.Minor == 1);
			}
		}
		public static bool IsNet45OrNewer {
			get {
				return Type.GetType("System.Reflection.ReflectionContext", false) != null;
			}
		}
	}
	public class LogicalContainer<T> : FrameworkElement
		where T : DependencyObject {
		List<T> Children;
#if DEBUGTEST
		public bool Contains(T item) {
			return Children.Contains(item);
		}
#endif
		public LogicalContainer() {
			Children = new List<T>();
		}
		public void Add(T item) {
			if(IsNotInTree(item)) {
				if(!Children.Contains(item))
					Children.Add(item);
				AddCore(item);
			}
		}
		public void Remove(T item) {
			Children.Remove(item);
			RemoveCore(item);
		}
		bool IsNotInTree(T item) {
			return LogicalTreeHelper.GetParent(item) == null;
		}
		void AddCore(T item) {
			AddLogicalChild(item);
		}
		void RemoveCore(T item) {
			RemoveLogicalChild(item);
		}
		protected override IEnumerator LogicalChildren {
			get { return Children.GetEnumerator(); }
		}
	}
	public class RenameHelper :IDisposable{
		DockLayoutManager Manager;
		BaseLayoutItem lastClickedItem;
		public RenameHelper(DockLayoutManager manager) {
			Manager = manager;
		}
		public void Dispose() {
			lastClickedItem = null;
			Manager = null;
			GC.SuppressFinalize(this);
		}
		public bool Rename(IDockLayoutElement element) {
			if(element == null || element.Item == null) return false;
			BaseLayoutItem item = element.Item;
			ResetClickedState();
			if(Manager.RaiseItemCancelEvent(item, DockLayoutManager.LayoutItemStartRenamingEvent)) return false;
			LayoutView view = Manager.GetView(item.GetRoot()) as LayoutView;
			if(view != null) {
				view.AdornerHelper.TryShowAdornerWindow(true);
				DockingHintAdorner adorner = view.AdornerHelper.GetDockingHintAdorner();
				if(adorner != null && CanRename(item)) {
					adorner.Update(true);
					adorner.RenameController.StartRenaming(element);
					return true;
				}
			}
			return false;
		}
		public bool Rename(BaseLayoutItem item) {
			IUIElement tabHeader = item.GetUIElement<VisualElements.ITabHeader>() as IUIElement;
			IDockLayoutElement element = null;
			if(tabHeader != null)
				element = Manager.GetViewElement(tabHeader) as IDockLayoutElement;
			if(element == null)
				element = Manager.GetViewElement(item) as IDockLayoutElement;
			return Rename(element);
		}
		public bool CancelRenamingAndResetClickedState() {
			ResetClickedState();
			return CancelRenaming();
		}
		public bool CancelRenaming() {
			if(!Manager.IsRenaming) return false;
			return TryCancelRenaming();
		}
		public bool EndRenaming() {
			if(!Manager.IsRenaming) return false;
			ResetClickedState();
			return TryEndRenaming();
		}
		public bool RenameByClick(IDockLayoutElement layoutElement) {
			BaseLayoutItem item = layoutElement.Item;
			if(!CanRename(item)) return false;
			if(item == lastClickedItem) {
				Rename(layoutElement);
				return true;
			}
			lastClickedItem = item;
			return false;
		}
		public void ResetClickedState() {
			lastClickedItem = null;
		}
		bool TryEndRenaming() {
			foreach(LayoutView view in Manager.ViewAdapter.Views) {
				RenameController controller = view.AdornerHelper.GetRenameController();
				if(controller != null && controller.IsRenamingStarted) {
					controller.EndRenaming();
					return true;
				}
			}
			return false;
		}
		bool TryCancelRenaming() {
			foreach(LayoutView view in Manager.ViewAdapter.Views) {
				RenameController controller = view.AdornerHelper.GetRenameController();
				if(controller != null && controller.IsRenamingStarted) {
					controller.CancelRenaming();
					return true;
				}
			}
			return false;
		}
		public bool CanRename(BaseLayoutItem item) {
			if(Manager.IsInDesignTime) return false;
			if(item == null || !item.AllowRename) return false;
			bool? value = LayoutItemsHelper.IsDockItem(item) ?
				Manager.AllowDockItemRename : Manager.AllowLayoutItemRename;
			return value.HasValue ? value.Value : Manager.IsCustomization;
		}
	}
	class ActivateBatch : IDisposable {
		DockLayoutManager Container;
		public ActivateBatch(DockLayoutManager container) {
			Container = container;
			if(Container != null) {
				Container.LockActivation();
			}
		}
		public void Dispose() {
			if (Container!= null)
				Container.UnlockActivation();
			Container = null;
		}
	}
	[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public class UpdateBatch : IDisposable {
		ISupportBatchUpdate Container;
		public UpdateBatch(ISupportBatchUpdate container) {
			Container = container;
			if(Container != null)
				Container.BeginUpdate();
		}
		public void Dispose() {
			if(Container != null)
				Container.EndUpdate();
			Container = null;
		}
	}
	class LogicalTreeLocker : IDisposable {
		DockLayoutManager Container;
		List<BaseLayoutItem> Items;
		List<BaseLayoutItem> CollectLayoutItems(BaseLayoutItem[] items) {
			List<BaseLayoutItem> list = new List<BaseLayoutItem>();
			if(items == null) return list;
			items.Accept((item) => { item.Do(x => x.Accept(list.Add)); });
			return list;
		}
		public LogicalTreeLocker(DockLayoutManager container, params BaseLayoutItem[] items) {
			Container = container;
			if(Container != null)
				Container.LockLogicalTreeChanging(this);
			Items = CollectLayoutItems(items);
			Items.Accept((item) => {
				item.LockInLogicalTree();
				DevExpress.Xpf.Bars.BarNameScope.GetService<DevExpress.Xpf.Bars.Native.IItemToLinkBinderService>(item).Lock();
			});
		}
		public void Dispose() {
			if(Container != null)
				Container.UnlockLogicalTreeChanging(this);
			Items.Accept((item) => {
				item.UnlockItemInLogicalTree();
				DevExpress.Xpf.Bars.BarNameScope.GetService<DevExpress.Xpf.Bars.Native.IItemToLinkBinderService>(item).Unlock();
			});
			Container = null;
			Items = null;
		}
		internal bool IsLocked(DependencyObject element) {
			BaseLayoutItem item = element as BaseLayoutItem;
			return item != null && Items.Contains(item);
		}
	}
	[MarkupExtensionReturnType(typeof(Uri))]
	public class RelativeUriExtension : MarkupExtension {
		public RelativeUriExtension() { }
		public RelativeUriExtension(string uriString) {
			UriString = uriString;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new Uri(UriString, UriKind.Relative);
		}
		[ConstructorArgument("uriString")]
		public string UriString { get; set; }
	}
	static class DockHintsExtensions {
		public static DockHint ToAutoHideDockHint(this DockVisualizerElement element) {
			switch(element) {
				case DockVisualizerElement.Right:
					return DockHint.AutoHideRight;
				case DockVisualizerElement.Top:
					return DockHint.AutoHideTop;
				case DockVisualizerElement.Bottom:
					return DockHint.AutoHideBottom;
				default:
					return DockHint.AutoHideLeft;
			}
		}
		public static DockHint ToSideDockHint(this DockVisualizerElement element) {
			switch(element) {
				case DockVisualizerElement.Right:
					return DockHint.SideRight;
				case DockVisualizerElement.Top:
					return DockHint.SideTop;
				case DockVisualizerElement.Bottom:
					return DockHint.SideBottom;
				default:
					return DockHint.SideLeft;
			}
		}
		public static DockHint ToCenterDockHint(this DockVisualizerElement element) {
			switch(element) {
				case DockVisualizerElement.Right:
					return DockHint.CenterRight;
				case DockVisualizerElement.Top:
					return DockHint.CenterTop;
				case DockVisualizerElement.Bottom:
					return DockHint.CenterBottom;
				case DockVisualizerElement.Center:
					return DockHint.Center;
				default:
					return DockHint.CenterLeft;
			}
		}
		public static DockGuide ToDockGuide(this DockVisualizerElement element) {
			switch(element) {
				case DockVisualizerElement.Right:
					return DockGuide.Right;
				case DockVisualizerElement.Top:
					return DockGuide.Top;
				case DockVisualizerElement.Bottom:
					return DockGuide.Bottom;
				case DockVisualizerElement.Center:
					return DockGuide.Center;
				default:
					return DockGuide.Left;
			}
		}
	}
	static class AnimationHelper {
		public static void BeginAnimation(DependencyObject dObj, DependencyProperty property, Timeline animation) {
			if(animation == null) return;
			Storyboard storyboard = new Storyboard();
			Storyboard.SetTarget(animation, dObj);
			Storyboard.SetTargetProperty(animation, new PropertyPath(property));
			storyboard.Children.Add(animation);
			storyboard.Begin();
		}
	}
	static class DesiredSizeHelper {
		static Dictionary<Type, bool> standardControlsList = new Dictionary<Type, bool>();
		static void InitControlsList() {
			standardControlsList.Add(typeof(System.Windows.Controls.ItemsControl), false);
			standardControlsList.Add(typeof(System.Windows.Controls.Image), false);
			standardControlsList.Add(typeof(System.Windows.Controls.TextBlock), true);
			standardControlsList.Add(typeof(System.Windows.Controls.TextBox), true);
			standardControlsList.Add(typeof(DevExpress.Xpf.Editors.ImageEdit), false);
			standardControlsList.Add(typeof(DockLayoutManager), false);
			standardControlsList.Add(typeof(DevExpress.Xpf.Ribbon.IRibbonControl), false);
		}
		static DesiredSizeHelper() {
			InitControlsList();
		}
		readonly static bool defaultValue = false;
		public static bool CanUseDesiredSizeAsMinSize(Type type) {
			var controlType = System.Linq.Enumerable.FirstOrDefault(standardControlsList, (pair) => object.Equals(pair.Key, type) || pair.Key.IsAssignableFrom(type));
			return controlType.Key == null ? defaultValue : controlType.Value;
		}
		public static bool CanUseDesiredSizeAsMinSize(UIElement control) {
			return control != null ? CanUseDesiredSizeAsMinSize(control.GetType()) : defaultValue;
		}
	}
	static class SelectionModeHelper {
		public static SelectionMode GetActualSelectionMode() {
			if(KeyHelper.IsCtrlPressed) return SelectionMode.MultipleItems;
			if(KeyHelper.IsShiftPressed) return SelectionMode.ItemRange;
			return SelectionMode.SingleItem;
		}
	}
	static class DisposeHelper {
		public  static void DisposeVisualTree(DependencyObject treeRoot) {
			if(treeRoot != null) {
				List<IDisposable> disposeList = new List<IDisposable>();
				var enumerator = new Core.Native.VisualTreeEnumerator(treeRoot);
				while(enumerator.MoveNext()) {
					IDisposable disposable = enumerator.Current as IDisposable;
					if(disposable != null)
						disposeList.Add(disposable);
				}
				foreach(IDisposable disposable in disposeList)
					disposable.Dispose();
			}
		}
	}
}
