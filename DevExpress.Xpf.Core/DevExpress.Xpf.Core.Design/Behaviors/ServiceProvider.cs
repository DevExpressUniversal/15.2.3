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

using DevExpress.Design.UI;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core.Native;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using DevExpress.Mvvm;
using System.Windows.Input;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Bars;
using System.Windows.Controls;
namespace DevExpress.Xpf.Core.Design.Services {
	public class ServiceProvider : WpfBindableBase, IDisposable {
		static List<DXServiceInfo> services;
		static List<DXServiceInfo> behaviors;
		static ServiceProvider() {
			services = new List<DXServiceInfo>();
			services.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(CurrentWindowService))));
			services.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(DialogService))));
			services.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(DispatcherService))));
			services.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(DXMessageBoxService))));
			services.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(DXSplashScreenService))));
			services.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(NotificationService))));
			services.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(ApplicationJumpListService))));
			services.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(TaskbarButtonService))));
			services.Add(new DXServiceInfo(new DXTypeInfo("TabbedDocumentUIService", XmlNamespaceConstants.DockingNamespace, AssemblyInfo.SRAssemblyXpfDocking)));
			services.Add(new DXServiceInfo(new DXTypeInfo("DockingDocumentUIService", XmlNamespaceConstants.DockingNamespace, AssemblyInfo.SRAssemblyXpfDocking)));
			services.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(WindowedDocumentUIService))));
			services.Add(new DXServiceInfo(new DXTypeInfo("FrameDocumentUIService", XmlNamespaceConstants.WindowsUINavigationNamespace, AssemblyInfo.SRAssemblyXpfControls)));
			services.Add(new DXServiceInfo(new DXTypeInfo("FrameNavigationService", XmlNamespaceConstants.WindowsUINavigationNamespace, AssemblyInfo.SRAssemblyXpfControls)));
			services.Add(new DXServiceInfo(new DXTypeInfo("WinUIDialogService", XmlNamespaceConstants.WindowsUINamespace, AssemblyInfo.SRAssemblyXpfControls)));
			services.Add(new DXServiceInfo(new DXTypeInfo("WinUIMessageBoxService", XmlNamespaceConstants.WindowsUINamespace, AssemblyInfo.SRAssemblyXpfControls)));
			services.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(OpenFileDialogService))));
			services.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(SaveFileDialogService))));
			services.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(FolderBrowserDialogService))));
			services.Add(new DXServiceInfo(new DXTypeInfo("GridReportManagerService", XmlNamespaceConstants.ReportDesignerExtensionsNamespace, AssemblyInfo.SRAssemblyXpfReportDesigner)));
			behaviors = new List<DXServiceInfo>();
			behaviors.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(EventToCommand))));
			behaviors.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(KeyToCommand))));
			behaviors.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(FilteringBehavior))));
			behaviors.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(FocusBehavior))));
			behaviors.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(EnumItemsSourceBehavior))));
			behaviors.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(FunctionBindingBehavior))));
			behaviors.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(MethodToCommandBehavior))));
			behaviors.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(ValidationErrorsHostBehavior))));
			behaviors.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(DependencyPropertyBehavior))));
			behaviors.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(ConfirmationBehavior))));
			behaviors.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(CompositeCommandBehavior))));
			behaviors.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(BarSplitItemThemeSelectorBehavior))));
			behaviors.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(BarSubItemThemeSelectorBehavior))));
			behaviors.Add(new DXServiceInfo(DXTypeInfo.FromType(typeof(GalleryThemeSelectorBehavior))));
			behaviors.Add(new DXServiceInfo(new DXTypeInfo("RibbonGalleryItemThemeSelectorBehavior", XmlNamespaceConstants.RibbonNamespace, AssemblyInfo.SRAssemblyXpfRibbon)));
			behaviors.Add(new DXServiceInfo(new DXTypeInfo("RibbonAutoHideModeBehavior", XmlNamespaceConstants.RibbonNamespace, AssemblyInfo.SRAssemblyXpfRibbon)));
			behaviors.Add(new DXServiceInfo(new DXTypeInfo("GridDragDropManager", XmlNamespaceConstants.GridNamespace, AssemblyInfo.SRAssemblyXpfGridExtensions)));
			behaviors.Add(new DXServiceInfo(new DXTypeInfo("TreeListDragDropManager", XmlNamespaceConstants.GridNamespace, AssemblyInfo.SRAssemblyXpfGridExtensions)));
			behaviors.Add(new DXServiceInfo(new DXTypeInfo("ListBoxDragDropManager", XmlNamespaceConstants.GridNamespace, AssemblyInfo.SRAssemblyXpfGridExtensions)));
			behaviors.Add(new DXServiceInfo(new DXTypeInfo("ReportManagerBehavior", XmlNamespaceConstants.ReportDesignerExtensionsNamespace, AssemblyInfo.SRAssemblyXpfReportDesigner)));
		}
		public ICommand ShowContextMenu { get; private set; }
		public ICommand AddBehavior { get; private set; }
		public ICommand DeleteBehavior { get; private set; }
		public static void RegisterService(Type serviceType) {
			RegisterService(DXTypeInfo.FromType(serviceType));
		}
		public static void RegisterService(DXTypeInfo serviceTypeInfo) {
			services.Add(new DXServiceInfo(serviceTypeInfo));
		}
		public static void RegisterBehavior(Type behaviorType) {
			RegisterBehavior(DXTypeInfo.FromType(behaviorType));
		}
		public static void RegisterBehavior(DXTypeInfo behaviorTypeInfo) {
			behaviors.Add(new DXServiceInfo(behaviorTypeInfo));
		}
		public IModelItem SelectedItem {
			get { return selectedItem; }
			private set { SetProperty(ref selectedItem, value, () => SelectedItem, OnSelctedItemPropertyChanged); }
		}
		public IModelItem SelectedBehavior {
			get { return selectedBehavior; }
			set { SetProperty(ref selectedBehavior, value, () => SelectedBehavior, OnSelectedBehaviorPropertyChanged); }
		}
		public IEditingContext Context {
			get { return context; }
			private set {
				if(context != value) {
					var oldValue = context;
					context = value;
					OnContextChanged(oldValue);
				}
			}
		}
		public bool IsPropertiesVisible {
			get { return isPropertiesVisible; }
			set { SetProperty(ref isPropertiesVisible, value, () => IsPropertiesVisible); }
		}
		public IEnumerable<DXServiceInfo> AvailableServices {
			get {
				if(availableServices == null)
					availableServices = GetFilteredCollection(services);
				return availableServices;
			}
		}
		public IEnumerable<DXServiceInfo> AvailableBehaviors {
			get {
				if(availableBehaviors == null)
					availableBehaviors = GetFilteredCollection(behaviors);
				return availableBehaviors;
			}
		}
		public ObservableCollection<IModelItem> ExistingBehaviors {
			get { return existingBehaviors; }
			set { SetProperty(ref existingBehaviors, value, () => ExistingBehaviors); }
		}
		public FrameworkElementSmartTagPropertiesViewModel PropertiesViewModel {
			get { return propertiesViewModel; }
			set { SetProperty(ref propertiesViewModel, value, () => PropertiesViewModel); }
		}
		public object ServicesActionInfo { get; set; }
		public object BehaviorsActionInfo { get; set; }
		public ServiceProvider(IModelItem selection) {
			if(selection == null)
				throw new ArgumentNullException();
			SelectedItem = selection;
			AddBehavior = new WpfDelegateCommand<DXServiceInfo>(OnAddBehaviorCommandExecute);
			DeleteBehavior = new DelegateCommand<IModelItem>(OnDeleteBehavior);
			ShowContextMenu = new DelegateCommand<ContextMenu>(OnShowContextMenu);
			ServicesActionInfo = new { Text = "Add Service", Command = ShowContextMenu, Items = AvailableServices, IsContextMenuNotEmpty = AvailableServices.Count() > 0 };
			BehaviorsActionInfo = new { Text = "Add Behavior", Command = ShowContextMenu, Items = AvailableBehaviors, IsContextMenuNotEmpty = AvailableBehaviors.Count() > 0 };
			UpdateExistingBehaviors();
			SelectedBehavior = GetItemToSelect(SelectedItem.GetBehaviorsCollection(), 0);
		}
		IModelItem GetItemToSelect(IEnumerable<IModelItem> itemsCollection, int index) {
			if(itemsCollection.Count() == 0)
				return null;
			if(itemsCollection.Count() > index && index >= 0)
				return itemsCollection.ElementAt(index);
			else
				return index > 0 ? itemsCollection.ElementAt(index - 1) : null;
		}
		IEnumerable<DXServiceInfo> GetFilteredCollection(IEnumerable<DXServiceInfo> behaviorsList) {
			return behaviorsList.Where((info) => info.GetIsTypeApplicable(SelectedItem.ItemType));
		}
		void OnAddBehaviorCommandExecute(DXServiceInfo serviceInfo) {
			using(IModelEditingScope scope = SelectedItem.BeginEdit("Add " + serviceInfo.TypeInfo.Name)) {
				IModelItem service = selectedItem.Context.CreateItem(serviceInfo.TypeInfo.ResolveType(), true);
				SelectedItem.GetBehaviorsCollection().Add(service);
				scope.Complete();
			}
		}
		void OnDeleteBehavior(IModelItem service) {
			int index = -1;
			using(var scope = SelectedItem.BeginEdit("Delete " + service.ItemType.Name)) {
				if(!typeof(DevExpress.Mvvm.UI.Interactivity.Behavior).IsAssignableFrom(service.ItemType))
					throw new ArgumentException("Incorrect type");
				IModelItemCollection itemsCollection = SelectedItem.GetBehaviorsCollection();
				index = itemsCollection.IndexOf(service);
				itemsCollection.Remove(service);
				scope.Complete();
			}
			SelectedBehavior = GetItemToSelect(SelectedItem.GetBehaviorsCollection(), index);
		}
		void OnSelectedBehaviorPropertyChanged() {
			PropertiesViewModel = SelectedBehavior == null ? null : new FrameworkElementSmartTagPropertiesViewModel(SelectedBehavior);
			IsPropertiesVisible = SelectedBehavior != null && PropertiesViewModel.Lines.Count() > 0;
		}
		void OnSelctedItemPropertyChanged() {
			Context = SelectedItem == null ? null : SelectedItem.Context;
		}
		void OnShowContextMenu(ContextMenu menu) {
			menu.IsOpen = true;
		}
		void OnContextChanged(IEditingContext oldValue) {
			if(oldValue != null && modelChangedEvent != null) {
				IModelService modelService = oldValue.Services.GetService(typeof(IModelService)) as IModelService;
				modelService.UnsubscribeFromModelChanged(modelChangedEvent);
			}
			if(Context != null) {
				IModelService modelService = Context.Services.GetService(typeof(IModelService)) as IModelService;
				modelChangedEvent = modelService.SubscribeToModelChanged(OnModelChanged);
			}
		}
		void OnModelChanged(object sender, EventArgs e) {
			UpdateExistingBehaviors();
		}
		void UpdateExistingBehaviors() {
			IEnumerable<IModelItem> behaviorsCollection = null;
			try {
				behaviorsCollection = SelectedItem.GetBehaviorsCollection().OfType<IModelItem>();
			} catch {
				ProjectReferencesHelper.EnsureAssemblyReferenced(SelectedItem, AssemblyInfo.SRAssemblyXpfMvvm, true);
				behaviorsCollection = Enumerable.Empty<IModelItem>();
			}
			List<IModelItem> actualBehaviors = new List<IModelItem>(behaviorsCollection);
			if(ExistingBehaviors == null) {
				ExistingBehaviors = new ObservableCollection<IModelItem>(actualBehaviors);
			} else {
				List<IModelItem> removedItems = new List<IModelItem>(ExistingBehaviors.Except(actualBehaviors));
				foreach(var item in removedItems) {
					ExistingBehaviors.Remove(item);
				}
				foreach(var item in actualBehaviors) {
					if(ExistingBehaviors.Contains(item))
						continue;
					ExistingBehaviors.Insert(actualBehaviors.IndexOf(item), item);
				}
			}
			if(!ExistingBehaviors.Contains(SelectedBehavior))
				SelectedBehavior = ExistingBehaviors.FirstOrDefault();
		}
		void IDisposable.Dispose() {
			SelectedItem = null;
			ExistingBehaviors.Clear();
			SelectedBehavior = null;
		}
		IModelSubscribedEvent modelChangedEvent = null;
		FrameworkElementSmartTagPropertiesViewModel propertiesViewModel;
		ObservableCollection<IModelItem> existingBehaviors;
		bool isPropertiesVisible;
		IEditingContext context;
		IModelItem selectedItem, selectedBehavior;
		IEnumerable<DXServiceInfo> availableServices, availableBehaviors;
	}
}
