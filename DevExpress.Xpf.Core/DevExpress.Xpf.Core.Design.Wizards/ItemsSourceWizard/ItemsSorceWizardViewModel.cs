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
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;
using System.IO;
using System.Windows.Input;
using System.Windows.Documents;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard.Templates;
using DevExpress.Xpf.Core.Design.Wizards.Utils;
using System.Windows.Controls;
using DevExpress.Utils.Design;
using DevExpress.Mvvm;
namespace DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard {
	public class ItemsSourceWizardViewModelBase : DependencyObject, INotifyPropertyChanged {
		public string Title { get; set; }
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if(handler != null) {
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	public interface IItemsSourceWizard {
		void OnNavigationPropertiesChanged();
		string HelpMeChooseLink { get; }
	}
	class DummyItemsSourceWizard : IItemsSourceWizard {
		public void OnNavigationPropertiesChanged() { }
		public string HelpMeChooseLink { get { return string.Empty; } }
	}
	public class ItemsSourceWizardViewModel : ItemsSourceWizardViewModelBase, IItemsSourceWizard {
		readonly string helpMeChooseLink;
		readonly SelectTechnologyViewModel selectTechnologyViewModel;
		readonly DataSourceConfigurationViewModel dataSourceConfigurationViewModel;
		bool isFinish;
		#region static
		public static readonly DependencyProperty CurrentPageIndexProperty;
		public static readonly DependencyProperty IsFinishStepEnabledProperty;
		public static readonly DependencyProperty IsPrevStepEnabledProperty;
		public static readonly DependencyProperty IsNextStepEnabledProperty;
		public static readonly DependencyProperty NextStepButtonNameProperty;
		static ItemsSourceWizardViewModel() {
			Type ownerType = typeof(ItemsSourceWizardViewModel);
			CurrentPageIndexProperty = DependencyProperty.Register("CurrentPageIndex", typeof(int), ownerType,
				new FrameworkPropertyMetadata((o, e) => ((ItemsSourceWizardViewModel)o).OnCurrentPageIndexChanged()));
			IsFinishStepEnabledProperty = DependencyProperty.Register("IsFinishStepEnabled", typeof(bool), ownerType, new FrameworkPropertyMetadata());
			IsPrevStepEnabledProperty = DependencyProperty.Register("IsPrevStepEnabled", typeof(bool), ownerType, new FrameworkPropertyMetadata());
			IsNextStepEnabledProperty = DependencyProperty.Register("IsNextStepEnabled", typeof(bool), ownerType, new FrameworkPropertyMetadata());
			NextStepButtonNameProperty = DependencyProperty.Register("NextStepButtonName", typeof(string), ownerType, new FrameworkPropertyMetadata());
		}
		#endregion
		public ItemsSourceWizardViewModel(IEnumerable<IDataAccessTechnologyInfo> technologies, IEnumerable<DataSource> avaliableDataSources, string helpMeChooseLink) {
			this.selectTechnologyViewModel = new SelectTechnologyViewModel(technologies, avaliableDataSources) { Owner = (IItemsSourceWizard)this };
			this.dataSourceConfigurationViewModel = new DataSourceConfigurationViewModel() { Owner = (IItemsSourceWizard)this };
			this.helpMeChooseLink = helpMeChooseLink;
			CloseCmd = new DelegateCommand<Window>(act => CloseWindow(act, false));
			FinishCmd = new DelegateCommand<Window>(act => CloseWindow(act, true));
			NextStepCmd = new DelegateCommand<Window>(act => NavigateNextStep(act, true));
			PreviousStepCmd = new DelegateCommand(NavigatePreviousStep);
			MailToSupportCmd = new DelegateCommand(MailToSupport);
			OnNavigationPropertiesChanged();
		}
		public int CurrentPageIndex {
			get { return (int)GetValue(CurrentPageIndexProperty); }
			set { SetValue(CurrentPageIndexProperty, value); }
		}
		public SelectTechnologyViewModel SelectTechnologyViewModel { get { return selectTechnologyViewModel; } }
		public DataSourceConfigurationViewModel DataSourceConfigurationViewModel { get { return dataSourceConfigurationViewModel; } }
		public ICommand CloseCmd { get; protected set; }
		public ICommand FinishCmd { get; protected set; }
		public ICommand NextStepCmd { get; protected set; }
		public ICommand PreviousStepCmd { get; protected set; }
		public ICommand MailToSupportCmd { get; protected set; }
		public bool IsApply { get { return CanApplySetting(); } }
		public bool IsFinishStepEnabled {
			get { return (bool)GetValue(IsFinishStepEnabledProperty); }
			private set { SetValue(IsFinishStepEnabledProperty, value); }
		}
		public bool IsPrevStepEnabled {
			get { return (bool)GetValue(IsPrevStepEnabledProperty); }
			private set { SetValue(IsPrevStepEnabledProperty, value); }
		}
		public bool IsNextStepEnabled {
			get { return (bool)GetValue(IsNextStepEnabledProperty); }
			private set { SetValue(IsNextStepEnabledProperty, value); }
		}
		public string NextStepButtonName {
			get { return (string)GetValue(NextStepButtonNameProperty); }
			private set { SetValue(NextStepButtonNameProperty, value); }
		}
		private bool CanApplySetting() {
			return selectTechnologyViewModel.SelectedTechnology != null && dataSourceConfigurationViewModel.ConfiguratedDataSourceType != null && isFinish;
		}
		private void OnCurrentPageIndexChanged() {
			UpdateNavigationProperties();
			if(CurrentPageIndex == 2 && selectTechnologyViewModel != null) {
				dataSourceConfigurationViewModel.ConfiguratedDataSourceType = selectTechnologyViewModel.GetSelectedDataSourceType();
			}
		}
		internal void CloseWindow(Window wnd, bool success) {
			if(success == true) {
				if(!dataSourceConfigurationViewModel.IsConfigurationValid) {
					return;
				}
			}
			isFinish = success;
			if(wnd != null) {
				wnd.Close();
			}
		}
		private void NavigateNextStep(Window wnd, bool success) {
			if(CurrentPageIndex < 2)
				CurrentPageIndex++;
			else {
				if(success == true && !dataSourceConfigurationViewModel.IsConfigurationValid) return;
				this.isFinish = success;
				if(wnd != null)
					wnd.Close();
			}
		}
		private void NavigatePreviousStep() {
			if(CurrentPageIndex > 0)
				CurrentPageIndex--;
		}
		private void MailToSupport() {
			System.Diagnostics.Process.Start("mailto:support@devexpress.com");
		}
		#region IItemsSourceWizard Members
		public void OnNavigationPropertiesChanged() {
			UpdateNavigationProperties();
		}
		private void UpdateNavigationProperties() {
			NextStepButtonName = CurrentPageIndex == 2 ? ItemsSourceWizardLocalizer.GetString(ItemsSourceWizardStringId.Finish) : ItemsSourceWizardLocalizer.GetString(ItemsSourceWizardStringId.Next);
			IsPrevStepEnabled = CurrentPageIndex > 0;
			IsNextStepEnabled = CanEnableNextStep();
		}
		private bool CanEnableNextStep() {
			switch(CurrentPageIndex) {
				case 0:
					bool isSelectedDataSourceValid = false;
					if(SelectTechnologyViewModel != null && SelectTechnologyViewModel.SelectedTechnology.Technology.Name == TechnologiesInfo.IEnumerableTechName)
						isSelectedDataSourceValid = true;
					else
						isSelectedDataSourceValid = selectTechnologyViewModel.SelectedDataSource != null;
					return selectTechnologyViewModel.SelectedTechnology != null && isSelectedDataSourceValid;
				case 1:
					return SelectTechnologyViewModel != null && SelectTechnologyViewModel.SelectedDataSourceType != null;
				case 2:
					bool result = CurrentPageIndex > 0 && dataSourceConfigurationViewModel.ConfiguratedDataSourceType != null
						&& dataSourceConfigurationViewModel.IsConfigurationValid;
					return result;
				default:
					return false;
			}
		}
		public string HelpMeChooseLink { get { return this.helpMeChooseLink; } }
		#endregion
	}
	public class SelectTechnologyViewModel : ItemsSourceWizardViewModelBase {
		readonly IEnumerable<IDataAccessTechnologyInfo> technologies;
		readonly IEnumerable<DataSource> dataSources;
		#region static
		public static readonly DependencyProperty SelectedTechnologyProperty;
		public static readonly DependencyProperty SelectedDataSourceTypeProperty;
		public static readonly DependencyProperty AvaliableDataSourcesProperty;
		public static readonly DependencyProperty SelectedDataSourceProperty;
		public static readonly DependencyProperty IsAvaliableDataSourcesEnabledProperty;
		public static readonly DependencyProperty SelectedDataSourceTypeHelpProperty;
		public static readonly DependencyProperty HelpTextForTechWithoutSourcesProperty;
		static SelectTechnologyViewModel() {
			Type ownerType = typeof(SelectTechnologyViewModel);
			SelectedTechnologyProperty = DependencyProperty.Register("SelectedTechnology", typeof(DataAccessTechnologyViewModel), ownerType,
			new FrameworkPropertyMetadata((o, e) => ((SelectTechnologyViewModel)o).OnSelectedTechnologyChanged()));
			SelectedDataSourceTypeProperty = DependencyProperty.Register("SelectedDataSourceType", typeof(DataSourceType), ownerType,
new FrameworkPropertyMetadata((o, e) => ((SelectTechnologyViewModel)o).OnSelectedDataSourceTypeChanged()));
			AvaliableDataSourcesProperty = DependencyProperty.Register("AvaliableDataSources", typeof(ObservableCollection<DataSource>), ownerType,
			new FrameworkPropertyMetadata());
			SelectedDataSourceTypeHelpProperty = DependencyProperty.Register("SelectedDataSourceTypeHelp", typeof(Stream), ownerType,
			new FrameworkPropertyMetadata());
			SelectedDataSourceProperty = DependencyProperty.Register("SelectedDataSource", typeof(DataSource), ownerType,
			new FrameworkPropertyMetadata((o, e) => ((SelectTechnologyViewModel)o).OnSelectedDataSourceChanged()));
			IsAvaliableDataSourcesEnabledProperty = DependencyProperty.Register("IsAvaliableDataSourcesEnabled", typeof(bool), ownerType, new FrameworkPropertyMetadata());
			HelpTextForTechWithoutSourcesProperty = DependencyProperty.Register("HelpTextForTechWithoutSources", typeof(string), ownerType, new FrameworkPropertyMetadata());
		}
		#endregion
		public SelectTechnologyViewModel(IEnumerable<IDataAccessTechnologyInfo> technologies, IEnumerable<DataSource> dataSources) {
			Owner = new DummyItemsSourceWizard();
			this.dataSources = dataSources;
			this.technologies = technologies;
			HelpMeChooseCmd = new DelegateCommand(HelpMeChoose);
			CreateSourceCmd = new DelegateCommand<Window>(act => CreateNewSource(act));
			SetTechnologiesCollectionView();
		}
		public IItemsSourceWizard Owner { get; set; }
		public DataAccessTechnologyViewModel SelectedTechnology {
			get { return (DataAccessTechnologyViewModel)GetValue(SelectedTechnologyProperty); }
			set { SetValue(SelectedTechnologyProperty, value); }
		}
		public DataSource SelectedDataSource {
			get { return (DataSource)GetValue(SelectedDataSourceProperty); }
			set { SetValue(SelectedDataSourceProperty, value); }
		}
		public Stream SelectedDataSourceTypeHelp {
			get { return (Stream)GetValue(SelectedDataSourceTypeHelpProperty); }
			set { SetValue(SelectedDataSourceTypeHelpProperty, value); }
		}
		public DataSourceType SelectedDataSourceType {
			get { return (DataSourceType)GetValue(SelectedDataSourceTypeProperty); }
			set { SetValue(SelectedDataSourceTypeProperty, value); }
		}
		public ObservableCollection<DataSource> AvaliableDataSources {
			get { return (ObservableCollection<DataSource>)GetValue(AvaliableDataSourcesProperty); }
			set { SetValue(AvaliableDataSourcesProperty, value); }
		}
		public ObservableCollection<DataAccessTechnologyViewModel> AvaliableTechnologies { get; set; }
		public bool IsAvaliableDataSourcesEnabled {
			get { return (bool)GetValue(IsAvaliableDataSourcesEnabledProperty); }
			set { SetValue(IsAvaliableDataSourcesEnabledProperty, value); }
		}
		public string HelpTextForTechWithoutSources {
			get { return (string)GetValue(HelpTextForTechWithoutSourcesProperty); }
			set { SetValue(HelpTextForTechWithoutSourcesProperty, value); }
		}
		public ICommand HelpMeChooseCmd { get; protected set; }
		public ICommand CreateSourceCmd { get; protected set; }
		private void HelpMeChoose() {
			System.Diagnostics.Process.Start(Owner.HelpMeChooseLink);
		}
		private void CreateNewSource(Window window) {
			EnvDTE.DTE dte = DTEHelper.GetCurrentDTE();
			if(dte == null) return;
			using(new MessageFilter()) {
				MessageWindow wnd = new MessageWindow();
				wnd.ShowMessageDialog(ItemsSourceWizardLocalizer.GetString(ItemsSourceWizardStringId.CreateNewItemText));
				if(window != null) window.Close();
				SelectedTechnology.Technology.CreateItem(dte);
			}
		}
		private void OnSelectedTechnologyChanged() {
			SelectedDataSourceType = null;
			IsAvaliableDataSourcesEnabled = SelectedTechnology.Technology.Name != TechnologiesInfo.IEnumerableTechName;
			UpdateAvaliableDataSources();
			SetHelpTextForTechWithoutSources();
			Owner.OnNavigationPropertiesChanged();
		}
		private void SetHelpTextForTechWithoutSources() {
			if(SelectedTechnology.Technology.Name == TechnologiesInfo.IEnumerableTechName)
				HelpTextForTechWithoutSources = ItemsSourceWizardLocalizer.GetString(ItemsSourceWizardStringId.IEnumerableText);
			else {
				if(AvaliableDataSources != null)
					HelpTextForTechWithoutSources = AvaliableDataSources.Count > 0 ? "" : ItemsSourceWizardLocalizer.GetString(ItemsSourceWizardStringId.AvaliableDataSourceText);
			}
		}
		private void OnSelectedDataSourceTypeChanged() {
			SelectedDataSourceTypeHelp = SelectedDataSourceType != null ? SelectedDataSourceType.Help : null;
			Owner.OnNavigationPropertiesChanged();
		}
		private void OnSelectedDataSourceChanged() {
			Owner.OnNavigationPropertiesChanged();
		}
		private void UpdateAvaliableDataSources() {
			AvaliableDataSources = GetAvaliebleDataSourcesBySelectedDataSourceType();
			if(AvaliableDataSources.Count > 0)
				SelectedDataSource = AvaliableDataSources[0];
		}
		private ObservableCollection<DataSource> GetAvaliebleDataSourcesBySelectedDataSourceType() {
			ObservableCollection<DataSource> result = new ObservableCollection<DataSource>();
			if(SelectedTechnology != null) {
				foreach(DataSource dataSource in dataSources) {
					if(dataSource.Technology.Name == SelectedTechnology.Technology.Name)
						result.Add(dataSource);
				}
			}
			return result;
		}
		private void SetTechnologiesCollectionView() {
			AvaliableTechnologies = new ObservableCollection<DataAccessTechnologyViewModel>();
			foreach(IDataAccessTechnologyInfo tech in technologies) {
				DataAccessTechnologyViewModel viewModel = tech.CreateViewModel();
				viewModel.HasDataSource = IsTechnologyHasDataSource(tech);
				AvaliableTechnologies.Add(viewModel);
			}
			SelectedTechnology = AvaliableTechnologies[0];
		}
		private bool IsTechnologyHasDataSource(IDataAccessTechnologyInfo tech) {
			foreach(DataSource dataSource in dataSources) {
				if(dataSource.Technology.Name == tech.Name)
					return true;
			}
			return false;
		}
		internal DataSourceType GetSelectedDataSourceType() {
			if(SelectedTechnology.Technology.Name == TechnologiesInfo.IEnumerableTechName) return SelectedDataSourceType;
			if(SelectedDataSource == null) return null;
			return SelectedDataSource.Technology.DataSourceTypes.FirstOrDefault(e => e.Name == SelectedDataSourceType.Name);
		}
		internal IDataAccessTechnologyInfo GetTechnologyByName(string techName) {
			return technologies.Where(e => e.Name == techName).FirstOrDefault();
		}
		internal DataSource GetAvaliableDataSourceByName(string dataSourceName) {
			return AvaliableDataSources.Where(e => e.Name == dataSourceName).FirstOrDefault();
		}
	}
	public class DataSourceConfigurationViewModel : ItemsSourceWizardViewModelBase {
		#region static
		public static readonly DependencyProperty ConfiguratedDataSourceTypeProperty;
		public static readonly DependencyProperty IsConfigurationValidProperty;
		static DataSourceConfigurationViewModel() {
			Type ownerType = typeof(DataSourceConfigurationViewModel);
			ConfiguratedDataSourceTypeProperty =
				DependencyProperty.Register("ConfiguratedDataSourceType", typeof(DataSourceType), ownerType,
				new FrameworkPropertyMetadata((o, e) => ((DataSourceConfigurationViewModel)o).OnConfiguratedDataSourceTypeChanged()));
			IsConfigurationValidProperty =
				DependencyProperty.Register("IsConfigurationValid", typeof(bool), ownerType,
				new FrameworkPropertyMetadata((o, e) => ((DataSourceConfigurationViewModel)o).OnConfigurationValidationChanged()));
		}
		#endregion
		public DataSourceConfigurationViewModel() {
			Owner = new DummyItemsSourceWizard();
			this.Title = "DataSource Configuration";
		}
		public IItemsSourceWizard Owner { get; set; }
		public DataSourceType ConfiguratedDataSourceType {
			get { return (DataSourceType)GetValue(ConfiguratedDataSourceTypeProperty); }
			set { SetValue(ConfiguratedDataSourceTypeProperty, value); }
		}
		public bool IsConfigurationValid {
			get { return (bool)GetValue(IsConfigurationValidProperty); }
			set { SetValue(IsConfigurationValidProperty, value); }
		}
		private void OnConfiguratedDataSourceTypeChanged() {
			SetConfigurationValidateBinding();
			Owner.OnNavigationPropertiesChanged();
		}
		private void OnConfigurationValidationChanged() {
			Owner.OnNavigationPropertiesChanged();
		}
		private void SetConfigurationValidateBinding() {
			Binding bind = new Binding("IsConfigurationValid");
			bind.Mode = BindingMode.OneWayToSource;
			bind.Source = this;
			BindingOperations.SetBinding((ConfigurationViewModelBase)ConfiguratedDataSourceType.Properties[0].DataContext, ConfigurationViewModelBase.IsValidProperty, bind);
#if DEBUGTEST
			System.Diagnostics.Debug.WriteLine("ISVALIDPROP: " + ((ConfigurationViewModelBase)ConfiguratedDataSourceType.Properties[0].DataContext).IsValid);
#endif
		}
	}
	public class DataAccessTechnologyViewModel : ItemsSourceWizardViewModelBase {
		readonly IDataAccessTechnologyInfo technology;
		#region  static
		public static readonly DependencyProperty TechnologyHelpProperty;
		public static readonly DependencyProperty CreateNewSourceHelpProperty;
		static DataAccessTechnologyViewModel() {
			Type ownerType = typeof(DataAccessTechnologyViewModel);
			CreateNewSourceHelpProperty = DependencyProperty.Register("CreateNewSourceHelp", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty));
			TechnologyHelpProperty = DependencyProperty.Register("TechnologyHelp", typeof(Stream), ownerType, new FrameworkPropertyMetadata());
		}
		#endregion
		public DataAccessTechnologyViewModel(IDataAccessTechnologyInfo technology) {
			this.technology = technology;
			TechnologyHelp = technology.GetTechnologyHelp();
		}
		public Stream TechnologyHelp {
			get { return (Stream)GetValue(TechnologyHelpProperty); }
			set { SetValue(TechnologyHelpProperty, value); }
		}
		public string CreateNewSourceHelp {
			get { return (string)GetValue(CreateNewSourceHelpProperty); }
			set { SetValue(CreateNewSourceHelpProperty, value); }
		}
		public IDataAccessTechnologyInfo Technology { get { return technology; } }
		public bool HasDataSource { get; set; }
		public virtual bool CanCreateNewSource { get { return true; } }
	}
	public class RiaTechnologyViewModel : DataAccessTechnologyViewModel {
		public RiaTechnologyViewModel(IDataAccessTechnologyInfo technology) : base(technology) { }
		public override bool CanCreateNewSource { get { return false; } }
	}
	public class DataSourceType : IEquatable<DataSourceType> {
		string name;
		readonly List<DataSourcePropertyTemplate> properties;
		public DataSourceType(string name) : this(name, null) { }
		public DataSourceType(string name, IEnumerable<DataSourcePropertyTemplate> properties) : this(name, properties, null) { }
		public DataSourceType(string name, IEnumerable<DataSourcePropertyTemplate> properties, Stream help) {
			this.name = name;
			this.properties = (List<DataSourcePropertyTemplate>)properties;
			this.Help = help;
		}
		public string Name { get { return name; } set { name = value; } }
		public string DisplayName { get; set; }
		public List<DataSourcePropertyTemplate> Properties { get { return properties; } }
		public Stream Help { get; set; }
		public override int GetHashCode() {
			int hashProductName = Name == null ? 0 : Name.GetHashCode();
			return hashProductName;
		}
		#region IEquatable<DataSourceType> Members
		public bool Equals(DataSourceType other) {
			if(Object.ReferenceEquals(other, null)) return false;
			if(Object.ReferenceEquals(this, other)) return true;
			return Name.Equals(other.Name);
		}
		#endregion
	}
	public class DataSourcePropertyTemplate {
		readonly string propertyName;
		object dataContext;
		DataTemplate contentTemplate;
		public DataSourcePropertyTemplate(string propertyName, DataTemplate contentTemplate) {
			this.propertyName = propertyName;
			this.contentTemplate = contentTemplate;
		}
		public string PropertyName { get { return propertyName; } }
		public object DataContext { get { return dataContext; } set { dataContext = value; } }
		public DataTemplate ContentTemplate { get { return contentTemplate; } set { contentTemplate = value; } }
		public Stream Help { get; set; }
		public IList<Stream> ConfigurationPropertiesHelp { get; set; }
		public string DisplayName { get { return string.IsNullOrEmpty(propertyName) ? "" : propertyName + ":"; } }
	}
	public class DataSource {
		readonly string name;
		readonly IDataAccessTechnologyInfo technology;
		public DataSource(string name, IDataAccessTechnologyInfo technology) {
			this.name = name;
			this.technology = technology;
		}
		public IDataAccessTechnologyInfo Technology { get { return technology; } }
		public string Name { get { return name; } }
	}
	class TechnologiesInfo {
		public const string IEnumerableTechName = "IEnumerable";
		public const string LinqToSqlTechName = "LinqToSQL";
		public const string EntityFrameworkTechName = "Entity Framework";
		public const string WcfTechName = "WCF";
		public const string RiaTechName = "RIA";
		public const string DataSetNameTechName = "Typed DataSet";
	}
}
