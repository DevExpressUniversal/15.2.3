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
using DevExpress.Entity.ProjectModel;
using DevExpress.Utils.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Utils;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	class ConstructorStartPageViewModel : MvvmConstructorPageViewModelBase {
		bool loadingTypes;
		private ConstructorEntryViewModel selectedEntry;
		IAppLayerConstructor appLayerConstructor;
		IEnumerable<ConstructorEntryViewModel> availableEntries;
		public ConstructorStartPageViewModel(IViewModelBase parentViewModel, MvvmConstructorContext context, IAppLayerConstructor appLayerConstructor)
			: base(parentViewModel, context) {
			this.appLayerConstructor = appLayerConstructor;
			AssertionException.IsNotNull(this.appLayerConstructor);
			NewItemCommand = new WpfDelegateCommand(OnNewItemRequest);
			LoadingTypes = true;
		}
		IWizardTaskManager TaskManager { get { return this.ServiceContainer.Resolve<IWizardTaskManager>(); } }
		void OnNewItemRequest() {
			IItemCreator itemCreator = this.AppLayerConstructor.GetEntryItemCreator(this.SelectedEntry.Entry);
			if(itemCreator == null)
				return;
			if(!string.IsNullOrEmpty(itemCreator.Message))
				using(new MessageFilter())
					MessageDialog.Show(TaskManager.MainWindow, itemCreator.Message, MessageBoxButton.OK);
			if(itemCreator.IsCloseDialogNeeded) {
				IDXDesignWindowViewModel windowModel = GetParentViewModel<IDXDesignWindowViewModel>();
				if(windowModel != null && windowModel.Window != null)
					windowModel.Window.DialogResult = true;
			}
			itemCreator.Create();
		}
		void BuildStepsLineItems() {
			int count = -1;
			IEnumerable<string> names = new string[0];
			foreach(ConstructorEntryViewModel availableEntry in AvailableEntries) {
				IEnumerable<IMvvmConstructorPageViewModel> pages = this.AppLayerConstructor.GetPages(StepByStepConfigurator, availableEntry.Entry);
				if(count < pages.Count())
					names = pages.Select<IMvvmConstructorPageViewModel, string>(p => p.StepDescription);
			}
			List<IStepsLineItem> result = new List<IStepsLineItem>();
			result.Add(new StepsLineItem(StepByStepConfigurator, this.StepDescription) { IsActive = true });
			result.AddRange(names.Select<string, IStepsLineItem>(name => new StepsLineItem(StepByStepConfigurator, name)));
			StepByStepConfigurator.StepsLineItems = result;
		}
		internal void Initialize() {
			LoadingTypes = true;
			try {
				BuildAvailableEntries();
				BuildStepsLineItems();
			}
			finally {
				LoadingTypes = false;
			}
		}
		void BuildAvailableEntries() {
			AvailableEntries = appLayerConstructor.GetAvailableEntries().Select(x => new ConstructorEntryViewModel(this, x, appLayerConstructor)).Where(i => i.IsVisible).ToArray();
			if(AvailableEntries.Any())
				SelectedEntry = AvailableEntries.First();
		}
		protected override bool CalcIsCompletedCore() {
			return SelectedEntry != null && SelectedEntry.SelectedItem != null && !SelectedEntry.SelectedItem.IsUnsupported;
		}
		protected override void OnLeave(MvvmConstructorContext context) {
			this.appLayerConstructor.EntryItemSelected(this.SelectedEntry.Entry, this.SelectedEntry.SelectedItem.Source);
		}
		IMvvmConstructorViewModel StepByStepConfigurator {
			get {
				return this.GetParentViewModel<IMvvmConstructorViewModel>();
			}
		}
		void UpdateConstructorPages() {
			StepByStepConfigurator.BeginUpdate();
			try {
				StepByStepConfigurator.RemoveLastPages();
				if(this.SelectedEntry != null)
					StepByStepConfigurator.Add(this.appLayerConstructor.GetPages(this, SelectedEntry.Entry));
			}
			finally {
				StepByStepConfigurator.EndUpdate();
			}
		}
		public void UpdateCompleted() {
			base.UpdateIsCompleted();
		}
		void UpdateCanCreateNewItems() {
			CanCreateNewItems = selectedEntry != null && selectedEntry.ItemsCount == 0 && AppLayerConstructor.CanCreateNewItemsOf(selectedEntry.Entry);
		}
		public IAppLayerConstructor AppLayerConstructor {
			get { return appLayerConstructor; }
		}
		public ConstructorEntryViewModel SelectedEntry {
			get {
				return selectedEntry;
			}
			set {
				if(SetProperty<ConstructorEntryViewModel>(ref selectedEntry, value, "SelectedEntry")) {
					UpdateConstructorPages();
					UpdateCanCreateNewItems();
					UpdateIsCompleted();
				}
			}
		}
		public IEnumerable<ConstructorEntryViewModel> AvailableEntries {
			get {
				return availableEntries;
			}
			set {
				SetProperty<IEnumerable<ConstructorEntryViewModel>>(ref availableEntries, value, "AvailableEntries");
			}
		}
		public ICommand NewItemCommand {
			get;
			private set;
		}
		public override string StepDescription {
			get { return SR_Mvvm.StartPageDescription; }
		}
		public bool LoadingTypes {
			get { return loadingTypes; }
			set {
				SetProperty<bool>(ref loadingTypes, value, "LoadingTypes");
			}
		}
		bool canCreateNewItems;
		public bool CanCreateNewItems {
			get {
				return canCreateNewItems;
			}
			set {
				SetProperty<bool>(ref canCreateNewItems, value, "CanCreateNewItems");
			}
		}
		protected override void OnEnter(MvvmConstructorContext context) {
			List<IStepsLineItem> result = new List<IStepsLineItem>();
			result.Add(new StepsLineItem(StepByStepConfigurator, this.StepDescription));
			StepByStepConfigurator.StepsLineItems = result;
		}
	}
	class ConstructorEntryItemViewModel {
		public ConstructorEntryItemViewModel(IHasName hasName, bool isUnsupported) {
			Guard.ArgumentNotNull(hasName, "hasName");
			Source = hasName;
			IsUnsupported = isUnsupported;
			Name = hasName.Name;
		}
		public IHasName Source { get; private set; }
		public string Name { get; private set; }
		public bool IsUnsupported { get; private set; }
	}
	class ConstructorEntryViewModel : ViewModelBase {
		ConstructorEntryItemViewModel selectedItem;
		private int itemsCount;
		readonly ConstructorEntry entry;
		IEnumerable<ConstructorEntryItemViewModel> items;
		IAppLayerConstructor constructor;
		bool isVisibleWithoutItems;
		public ConstructorEntryViewModel(ConstructorStartPageViewModel parentViewModel, ConstructorEntry entry, IAppLayerConstructor constructor)
			: base(parentViewModel) {
			this.entry = entry;
			this.constructor = constructor;
			this.isVisibleWithoutItems = constructor.IsVisibleWithoutItems(entry);
			BuildItems();
		}
		void BuildItems() {
			ConstructorStartPageViewModel parent = this.GetParentViewModel<ConstructorStartPageViewModel>();
			IEnumerable<ConstructorEntryItemViewModel> entryItems = constructor.GetEntryItems(entry, true).Select(i => new ConstructorEntryItemViewModel(i, constructor.IsUnsupported(i)));
			List<ConstructorEntryItemViewModel> list = new List<ConstructorEntryItemViewModel>();
			if(entryItems != null)
				list.AddRange(entryItems);
			Items = list;
			ItemsCount = list.Count;
			if(ItemsCount > 0)
				this.SelectedItem = list[0];
		}
		public bool IsVisible { get { return isVisibleWithoutItems || ItemsCount != 0; } }
		public string Name {
			get {
				switch(entry) {
					case ConstructorEntry.WCFDataService:
						return SR_Mvvm.WCFDataServiceModel;
					case ConstructorEntry.EntityFrameworkModel:
						return SR_Mvvm.EntityFrameworkModel;
					case ConstructorEntry.DataModel:
						return SR_Mvvm.DataModel;
					case ConstructorEntry.ViewModel:
						return SR_Mvvm.ViewModel;
				}
				return entry.ToString();
			}
		}
		public ConstructorEntry Entry {
			get {
				return entry;
			}
		}
		public int ItemsCount {
			get {
				return itemsCount;
			}
			private set {
				SetProperty<int>(ref itemsCount, value, "ItemsCount");
			}
		}
		public IEnumerable<ConstructorEntryItemViewModel> Items {
			get { return items; }
			set { SetProperty<IEnumerable<ConstructorEntryItemViewModel>>(ref items, value, "Items"); }
		}
		public ConstructorEntryItemViewModel SelectedItem {
			get { return selectedItem; }
			set {
				if(SetProperty<ConstructorEntryItemViewModel>(ref selectedItem, value, "SelectedItem"))
					this.GetParentViewModel<ConstructorStartPageViewModel>().UpdateCompleted();
			}
		}
	}
	public class ToolTipData {
		public static ToolTipData Create(object data) {
			IDXTypeInfo info = data as IDXTypeInfo;
			if(info == null && data is IDataModel)
				info = ((IDataModel)data).DbContainer;
			if(info != null) {
				Type type = info.ResolveType();
				ToolTipData result = new ToolTipData();
				result.Init(type.Assembly.GetName().Name, info.NamespaceName, info.Name);
				return result;
			}
			IViewModelInfo viewModelInfo = data as IViewModelInfo;
			if(viewModelInfo != null) {
				ToolTipData result = new ToolTipData();
				result.Init(viewModelInfo.AssemblyName, viewModelInfo.Namespace, viewModelInfo.Name);
				return result;
			}
			return null;
		}
		void Init(string assembly, string @namespace, string name) {
			Assembly = assembly;
			Namespace = @namespace;
			Name = name;
		}
		public string Assembly { get; private set; }
		public string Name { get; private set; }
		public string Namespace { get; private set; }
	}
	public enum ToolTipDataType {
		Assembly,
		Namespace,
		Name
	}
	public sealed class ToolTipDataConverter : IValueConverter {
		public ToolTipDataType ToolTipDataType { get; set; }
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			switch(ToolTipDataType) {
				case ToolTipDataType.Assembly: {
						IDXTypeInfo info = value as IDXTypeInfo;
						if(info == null && value is IDataModel)
							info = ((IDataModel)value).DbContainer;
						if(info != null) {
							Type type = info.ResolveType();
							return type.Assembly.GetName().Name;
						}
						IViewModelInfo viewModelInfo = value as IViewModelInfo;
						if(viewModelInfo != null)
							return viewModelInfo.AssemblyName;
					}
					break;
				case ToolTipDataType.Namespace: {
						IDXTypeInfo info = value as IDXTypeInfo;
						if(info == null && value is IDataModel)
							info = ((IDataModel)value).DbContainer;
						if(info != null)
							return info.NamespaceName;
						IViewModelInfo viewModelInfo = value as IViewModelInfo;
						if(viewModelInfo != null)
							return viewModelInfo.Namespace;
					}
					break;
				case ToolTipDataType.Name: {
						IHasName hasName = value as IHasName;
						if(hasName != null)
							return hasName.Name;
					}
					break;
			}
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
	}
}
