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

namespace DevExpress.Design.DataAccess.UI {
	using System.Collections.Generic;
	using DevExpress.Design.UI;
	class DataAccessTechnologyConfiguratorViewModel : BaseDataAccessConfiguratorPageViewModel, IDataAccessTechnologyConfiguratorViewModel {
		IEnumerable<IDataAccessTechnologyInfo> infosCore;
		public DataAccessTechnologyConfiguratorViewModel(IViewModelBase parentViewModel, IDataAccessConfiguratorContext context)
			: base(parentViewModel, context) {
			InitNewItemCommand();
			this.infosCore = InitInfos(context);
			TechnologyNames = System.Linq.Enumerable.Select(Infos, (info) => info.Name);
			StartInitializationAsync(context);
		}
		protected IEnumerable<IDataAccessTechnologyInfo> Infos {
			get { return infosCore; }
		}
		IEnumerable<IDataAccessTechnologyInfo> InitInfos(IDataAccessConfiguratorContext context) {
			var configurationService = ServiceContainer.Resolve<IDataAccessConfigurationService>();
			return configurationService.InitTechnologyInfos(ServiceContainer, context);
		}
#if DEBUGTEST
		internal static bool avoidAsync;
#endif
		static InitializationInfo inititalizationInfo;
		void StartInitializationAsync(IDataAccessConfiguratorContext context) {
#if DEBUGTEST
			if(avoidAsync) {
				InitSelectedTechnologyNameAndItems(context);
				RaisePropertyChanged("SelectedTechnologyName");
				RaisePropertyChanged("CanCreateItems");
				RaisePropertyChanged("HasItems");
				RaisePropertyChanged("Items");
				return;
			}
#endif
			if(inititalizationInfo == null) {
				inititalizationInfo = new InitializationInfo(context);
				System.Threading.ThreadPool.RegisterWaitForSingleObject(inititalizationInfo.Completed, InitializationComplete, inititalizationInfo, -1, true);
				System.Threading.ThreadPool.QueueUserWorkItem(InitializationRoutine, inititalizationInfo);
			}
		}
		class InitializationInfo {
			public InitializationInfo(IDataAccessConfiguratorContext context) {
				this.Context = context;
				this.Completed = new System.Threading.ManualResetEvent(false);
			}
			public readonly System.Threading.ManualResetEvent Completed;
			public IDataAccessConfiguratorContext Context { get; private set; }
		}
		void InitializationRoutine(object state) {
			InitializationInfo info = state as InitializationInfo;
			try {
				InitSelectedTechnologyNameAndItems(info.Context);
			}
			finally { info.Completed.Set(); }
		}
		void InitializationComplete(object state, bool timeout) {
			InitializationInfo info = state as InitializationInfo;
			try {
				RaisePropertyChanged("SelectedTechnologyName");
				RaisePropertyChanged("CanCreateItems");
				RaisePropertyChanged("HasItems");
				RaisePropertyChanged("Items");
				inititalizationInfo = null;
			}
			finally { info.Completed.Close(); }
		}
		void InitSelectedTechnologyNameAndItems(IDataAccessConfiguratorContext context) {
			var selectedInfoFilter = (context.TechnologyName != null) ?
				new System.Func<IDataAccessTechnologyInfo, bool>((info) => object.Equals(info.Name, context.TechnologyName)) :
				new System.Func<IDataAccessTechnologyInfo, bool>((info) => System.Linq.Enumerable.Any(info.Items) || !info.CanCreateItems);
			IDataAccessTechnologyInfo selectedInfo = System.Linq.Enumerable.FirstOrDefault(Infos, selectedInfoFilter);
			if(selectedInfo != null) {
				selectedTechnologyNameCore = selectedInfo.Name;
				canCreateItemsCore = selectedInfo.CanCreateItems;
				hasItemsCore = selectedInfo.HasItems;
				Items = GetItems(selectedInfo);
				if(HasItems) {
					var selectedItemFilter = (context.TechnologyItem != null && !(context.TechnologyItem is ITypesProviderDataAccessTechnologyInfoItem)) ?
						new System.Func<IDataAccessTechnologyInfoItem, bool>((item) => object.Equals(item.Type, context.TechnologyItem.Type)) :
						new System.Func<IDataAccessTechnologyInfoItem, bool>((item) => true);
					SelectedItem = System.Linq.Enumerable.FirstOrDefault(Items, selectedItemFilter);
				}
			}
			else Items = new IDataAccessTechnologyInfoItem[0];
		}
		IEnumerable<IDataAccessTechnologyInfoItem> GetItems(IDataAccessTechnologyInfo selectedInfo) {
			return System.Linq.Enumerable.OrderBy(selectedInfo.Items, (item) => item.Name);
		}
		void InitNewItemCommand() {
			NewItemCommand = new WpfDelegateCommand<IDataAccessTechnologyName>(CreateNewItem, CanCreateNewItem);
		}
		bool CanCreateNewItem(IDataAccessTechnologyName technologyName) {
			var selectedInfo = System.Linq.Enumerable.FirstOrDefault(Infos,
				(info) => info.Name == technologyName);
			return (selectedInfo != null) && selectedInfo.CanCreateItems;
		}
		void CreateNewItem(IDataAccessTechnologyName technologyName) {
			IDXDesignWindowViewModel designWindowViewModel = GetParentViewModel<IDXDesignWindowViewModel>();
			var codeName = technologyName.GetCodeName();
			bool? messageResult = MessageWindow.Show(GetMessageWindowViewModel(designWindowViewModel, codeName));
			if(messageResult.GetValueOrDefault()) {
				if(Design.UI.Platform.IsDesignMode) {
					var configuratorViewModel = GetParentViewModel<IDataAccessConfiguratorViewModel>();
					configuratorViewModel.Context.TechnologyName = technologyName;
					var newItemService = ServiceContainer.Resolve<IDataAccessTechnologyNewItemService>();
					newItemService.Create(codeName);
				}
			}
		}
		IMessageWindowViewModel GetMessageWindowViewModel(IDXDesignWindowViewModel designWindowViewModel, DataAccessTechnologyCodeName codeName) {
			switch(codeName) {
				case DataAccessTechnologyCodeName.SQLDataSource:
					return new NewSQLDataSourceMessageWindowViewModel(designWindowViewModel);
				case DataAccessTechnologyCodeName.ExcelDataSource:
					return new NewExcelDataSourceMessageWindowViewModel(designWindowViewModel);
			}
			return new NewDataSourceMessageWindowViewModel(designWindowViewModel);
		}
		#region Properties
		public IEnumerable<IDataAccessTechnologyName> TechnologyNames {
			get;
			private set;
		}
		IDataAccessTechnologyName selectedTechnologyNameCore;
		public IDataAccessTechnologyName SelectedTechnologyName {
			get { return selectedTechnologyNameCore; }
			set { SetProperty(ref selectedTechnologyNameCore, value, "SelectedTechnologyName", OnSelectedTechnologyNameChanged); }
		}
		public IEnumerable<IDataAccessTechnologyInfoItem> Items {
			get;
			private set;
		}
		IDataAccessTechnologyInfoItem selectedItemCore;
		public IDataAccessTechnologyInfoItem SelectedItem {
			get { return selectedItemCore; }
			set { SetProperty(ref selectedItemCore, value, "SelectedItem", OnSelectedItemChanged); }
		}
		bool hasItemsCore;
		public bool HasItems {
			get { return hasItemsCore; }
			private set { SetProperty(ref hasItemsCore, value, "HasItems"); }
		}
		bool canCreateItemsCore;
		public bool CanCreateItems {
			get { return canCreateItemsCore; }
			private set { SetProperty(ref canCreateItemsCore, value, "CanCreateItems", OnCanCreateItemsChanged); }
		}
		#endregion Properties
		#region Commands
		public ICommand<IDataAccessTechnologyName> NewItemCommand {
			get;
			private set;
		}
		#endregion Commands
		void OnSelectedTechnologyNameChanged() {
			var selectedInfo = System.Linq.Enumerable.FirstOrDefault(Infos,
				(info) => info.Name == SelectedTechnologyName);
			UpdateItemsCore(selectedInfo);
		}
		void OnCanCreateItemsChanged() {
			UpdateIsCompleted();
		}
		void OnSelectedItemChanged() {
			UpdateIsCompleted();
		}
		void UpdateItemsCore(IDataAccessTechnologyInfo selectedInfo) {
			Items = (selectedInfo != null) ? GetItems(selectedInfo) : new IDataAccessTechnologyInfoItem[0];
			RaisePropertyChanged("Items");
			CanCreateItems = (selectedInfo != null) && selectedInfo.CanCreateItems;
			HasItems = (selectedInfo != null) && selectedInfo.HasItems;
			if(HasItems)
				SelectedItem = System.Linq.Enumerable.FirstOrDefault(Items);
			else
				SelectedItem = null;
		}
		protected override bool CalcIsCompleted(IDataAccessConfiguratorContext context) {
			return SelectedTechnologyName != null && (SelectedItem != null || !CanCreateItems);
		}
		protected override void OnEnter(IDataAccessConfiguratorContext context) {
			StartInitializationAsync(context);
		}
		protected override void OnLeave(IDataAccessConfiguratorContext context) {
			context.TechnologyName = SelectedTechnologyName;
			switch(SelectedTechnologyName.GetCodeName()) {
				case DataAccessTechnologyCodeName.IEnumerable:
					var typesProvider = ServiceContainer.Resolve<IDataAccessTechnologyTypesProvider>();
					context.TechnologyItem = new TypesProviderDataAccessTechnologyInfoItem(typesProvider);
					break;
				case DataAccessTechnologyCodeName.Enum:
					var enumTypesProvider = ServiceContainer.Resolve<IDataAccessTechnologyTypesProvider>();
					context.TechnologyItem = new EnumTypesProviderDataAccessTechnologyInfoItem(enumTypesProvider);
					break;
				default:
					context.TechnologyItem = SelectedItem;
					break;
			}
		}
	}
}
