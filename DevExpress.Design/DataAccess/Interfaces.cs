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

namespace DevExpress.Design.DataAccess {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using DevExpress.Design.ComponentModel;
	public interface ILocalizableDataAccessObject<TCodeName>
		where TCodeName : struct {
		string Name { get; }
		string Description { get; }
		TCodeName GetCodeName();
	}
	#region DataAccessTechnology
	public enum DataAccessTechnologyCodeName {
		TypedDataSet,
		SQLDataSource,
		ExcelDataSource,
		EntityFramework,
		LinqToSql,
		Wcf,
		Ria,
		XPO,
		IEnumerable,
		XmlDataSet,
		OLAP,
		Enum
	}
	public interface IDataAccessTechnologyName :
		ILocalizableDataAccessObject<DataAccessTechnologyCodeName> {
	}
	public interface IDataAccessTechnologyInfo {
		IDataAccessTechnologyName Name { get; }
		IDataAccessTechnologyConstraint Constraint { get; }
		IEnumerable<IDataAccessTechnologyInfoItem> Items { get; }
		bool HasItems { get; }
		bool CanCreateItems { get; }
		bool ComponentsAsItems { get; }
		bool TypeSetAsItems { get; }
	}
	public interface IDataAccessTechnologyConstraint : IEnumerable<Metadata.TypeConstraint> {
		bool TryGetMembers(Type type, out IEnumerable<System.Reflection.MemberInfo> members);
	}
	public interface IDataAccessTechnologyInfoItem {
		Type Type { get; }
		string Name { get; }
		IEnumerable<System.Reflection.MemberInfo> Members { get; }
	}
	public interface IComponentDataAccessTechnologyInfoItem : IDataAccessTechnologyInfoItem {
		object Component { get; }
	}
	public interface ITypeSetDataAccessTechnologyInfoItem :
		ITypesProviderDataAccessTechnologyInfoItem {
		string Namespace { get; }
	}
	public interface ITypesProviderDataAccessTechnologyInfoItem :
		IDataAccessTechnologyInfoItem, IDataAccessTechnologyTypesProvider {
	}
	#endregion
	#region DataProcessingModes
	public enum DataProcessingModeCodeName {
		DirectBinding,
		SimpleBinding,
		InMemoryCollectionView,
		InMemoryBindingSource,
		ServerMode,
		InstantFeedback,
		PLinqServerMode,
		PLinqInstantFeedback,
		XMLtoDataSet,
		XPCollectionForXPO,
		XPViewForXPO,
		OLEDBforOLAP,
		ADOMDforOLAP,
		XMLAforOLAP,
	}
	public interface IDataProcessingMode : ILocalizableDataAccessObject<DataProcessingModeCodeName> {
		bool IsInMemoryProcessing { get; }
		bool IsServerSide { get; }
		bool IsAsynchronous { get; }
		bool IsParallel { get; }
		bool IsOLAP { get; }
	}
	public interface IDataProcessingModesSet :
		IEnumerable<IDataProcessingMode> {
	}
	public interface IDataProcessingModesInfo {
		IDataProcessingModesSet Modes { get; }
	}
	#endregion
	#region DataSourceSettings
	public interface IDataSourceInfo {
		Type SourceType { get; }
		IEnumerable<IDataSourceElementInfo> Elements { get; }
	}
	public interface IXmlDataSourceInfo : IDataSourceInfo {
		void UpdateElements(string xmlPath);
	}
	public interface IEnumDataSourceInfo : IDataSourceInfo {
	}
	public interface IComponentDataSourceInfo : IDataSourceInfo {
		object Component { get; }
	}
	public interface IExcelDataSourceInfo : IDataSourceInfo, IComponentDataSourceInfo {
		string ExcelPath { get; }
	}
	public interface IDataSourceElementInfo {
		string Name { get; }
		IEnumerable<string> Fields { get; }
	}
	public interface IDataTypeInfo : IDataSourceElementInfo {
		Type ElementType { get; }
	}
	public interface IKeyExpressionsInfo {
		IEnumerable<string> KeyExpressions { get; }
	}
	public interface IDataTableInfo : IDataSourceElementInfo, IKeyExpressionsInfo {
		Type TableType { get; }
		Type RowType { get; }
	}
	public interface IDataServiceTableInfo : IDataTableInfo {
		string ServiceUri { get; }
	}
	public interface IQueryInfo : IDataSourceElementInfo {
		object Component { get; }
	}
	public enum DataSourcePropertyCodeName {
		Tables,
		ElementType,
		ObjectType,
		ServiceRoot,
		DefaultSorting,
		SortDescriptions,
		Filter,
		GroupAndSortDescriptions,
		Culture,
		IsSynchronizedWithCurrentItem,
		KeyExpression,
		AreSourceRowsThreadSafe,
		Query,
		AutoLoad,
		LoadDelay,
		LoadInterval,
		LoadSize,
		RefreshInterval,
		Provider,
		Server,
		Catalog,
		Cube,
		ConnectionTimeout,
		QueryTimeout,
		UserId,
		Password,
		ConnectionString,
		ShowDesignData,
		ShowCodeBehind,
		CustomBindingProperty,
		XmlPath,
		ExcelPath,
	}
	public interface IDataSourceSettings : IEnumerable<IDataSourceProperty> {
		IDataSourceProperty this[DataSourcePropertyCodeName codeName] { get; }
	}
	public interface IDataSourceProperty : ILocalizableDataAccessObject<DataSourcePropertyCodeName> {
		IDataSourceSettingsModel SettingsModel { get; }
		bool ShowName { get; }
	}
	public interface ICustomBindingDataSourceProperty : IDataSourceProperty, INotifyPropertyChanged {
		string Value { get; set; }
		bool HasValue { get; }
		Design.UI.ICommand<object> ResetValueCommand { get; }
	}
	public interface IServiceSettingsModel {
		string ServiceRoot { get; set; }
		bool HasServiceRoot { get; }
		bool IsValidServiceRootUri { get; }
		bool IsValidServiceRoot { get; }
		Design.UI.ICommand<object> ResetServiceRootCommand { get; }
		Design.UI.ICommand<object> TestServiceRootCommand { get; }
	}
	public interface IDataSourceSettingsModel : System.ComponentModel.INotifyPropertyChanged, IDataErrorInfo {
		Type Key { get; }
		Type SourceType { get; }
		object Component { get; }
		IEnumerable<IDataSourceElementInfo> Elements { get; }
		IDataSourceElementInfo SelectedElement { get; set; }
		IDictionary<string, string> CustomBindingProperties { get; }
		IEnumerable<string> Fields { get; }
		bool IsDesignDataAllowed { get; }
		bool ShowDesignData { get; set; }
		int DesignDataRowCount { get; set; }
		bool ShowCodeBehind { get; set; }
		bool SelectedElementIsDataTable { get; }
		bool SelectedElementIsDataType { get; }
		void Enter();
	}
	public interface ISortingSettingsModel : IDataSourceSettingsModel {
		bool AllowSorting { get; }
		PropertySortDescriptionCollection SortDescriptions { get; }
		PropertySortDescription SortDescription { get; set; }
		string SortField { get; set; }
		ListSortDirection SortDirection { get; set; }
		bool IsSortingAvailable { get; }
		Design.UI.ICommand<string> AddSortCommand { get; }
		Design.UI.ICommand<PropertySortDescription> DeleteSortCommand { get; }
		Design.UI.ICommand<PropertySortDescription> InvertSortDirectionCommand { get; }
	}
	public interface IDefaultSortingSettingsModel : IDataSourceSettingsModel {
		string SortField { get; set; }
		ListSortDirection SortDirection { get; set; }
		bool IsDefaultSortingAvailable { get; }
		string DefaultSorting { get; }
		Design.UI.ICommand<string> ResetDefaultSortingCommand { get; }
	}
	public interface IGroupingSettingsModel : IDataSourceSettingsModel {
		bool AllowGrouping { get; }
		PropertyGroupDescriptionCollection GroupDescriptions { get; }
		GroupDescription GroupDescription { get; set; }
		string GroupField { get; set; }
		Design.UI.ICommand<string> AddGroupCommand { get; }
		Design.UI.ICommand<GroupDescription> DeleteGroupCommand { get; }
	}
	public interface IDirectBindingSettingsModel :
		IDataSourceSettingsModel {
	}
	public interface ISimpleBindingSettingsModel :
		IDataSourceSettingsModel {
	}
	public interface IBindingSourceSettingsModelBase :
		IDataSourceSettingsModel {
	}
	public interface ITypedListSourceSettingsModel :
		IBindingSourceSettingsModelBase, IDirectBindingSettingsModel {
	}
	public interface IXPObjectSourceSettingsModel :
		ITypedListSourceSettingsModel {
	}
	public interface IXPCollectionSourceSettingsModel :
		IXPObjectSourceSettingsModel {
	}
	public interface IXPViewSourceSettingsModel :
		IXPObjectSourceSettingsModel, ISortingSettingsModel {
	}
	public interface IXPServerCollectionSourceSettingsModel :
		IXPObjectSourceSettingsModel, IServerModeSettingsModelBase {
	}
	public interface IXPInstantFeedbackSourceSettingsModel :
		IXPObjectSourceSettingsModel, IServerModeSettingsModelBase {
	}
	public interface IBindingListViewSourceSettingsModel :
		IBindingSourceSettingsModelBase, ISortingSettingsModel {
		string Filter { get; set; }
		Design.UI.ICommand<string> ResetFilterCommand { get; }
	}
	public interface IXmlDataSetSettingsModel : IDirectBindingSettingsModel {
		string XmlPath { get; set; }
		bool HasXmlPath { get; }
		Design.UI.ICommand<object> ResetXmlPathCommand { get; }
		Design.UI.ICommand<object> SetXmlPathCommand { get; }
	}
	public interface ICollectionSettingsModelBase : ISortingSettingsModel, IGroupingSettingsModel {
		bool AllowSortingOrGrouping { get; }
		bool IsSynchronizedWithCurrentItem { get; set; }
		bool AllowPaging { get; }
		int PageSize { get; set; }
	}
	public interface ICollectionViewSettingsModel : ICollectionSettingsModelBase {
		IEnumerable<System.Globalization.CultureInfo> Cultures { get; }
		System.Globalization.CultureInfo SelectedCulture { get; set; }
		IEnumerable<string> CollectionViewTypes { get; }
		string SelectedCollectionViewType { get; set; }
	}
	public interface ICollectionViewTypesProvider :
		IEnumerable<string> {
	}
	public interface IServerModeSettingsModelBase
		: IDefaultSortingSettingsModel {
	}
	public interface IServerModeSettingsModel : IServerModeSettingsModelBase {
		string KeyExpression { get; set; }
		IEnumerable<string> KeyExpressions { get; }
	}
	public interface IPLinqServerModeSettingsModel :
		IServerModeSettingsModelBase {
	}
	public interface IInstantFeedbackSettingsModel : IServerModeSettingsModel {
		bool AreSourceRowsThreadSafe { get; set; }
	}
	public interface IPLinqInstantFeedbackSettingsModel :
		IServerModeSettingsModelBase {
	}
	public interface IDomainDataSourceSettingsModel : ICollectionSettingsModelBase {
		bool AutoLoad { get; set; }
		int LoadDelay { get; set; }
		int LoadInterval { get; set; }
		int LoadSize { get; set; }
		int RefreshInterval { get; set; }
	}
	public interface IOLAPDataSourceSettingsModel : IDataSourceSettingsModel {
		string ConnectionString { get; }
		object DataProvider { get; }
		IEnumerable<string> Providers { get; }
		string SelectedProvider { get; set; }
		bool HasProvider { get; }
		string Server { get; set; }
		bool HasServer { get; }
		IEnumerable<string> Catalogs { get; }
		string SelectedCatalog { get; set; }
		bool HasCatalog { get; }
		IEnumerable<string> Cubes { get; }
		string SelectedCube { get; set; }
		bool HasCube { get; }
		int ConnectionTimeout { get; set; }
		IEnumerable<System.Globalization.CultureInfo> Cultures { get; }
		System.Globalization.CultureInfo SelectedCulture { get; set; }
		int QueryTimeout { get; set; }
		string UserId { get; set; }
		string Password { get; set; }
		Design.UI.ICommand<object> RetrieveSchemaCommand { get; }
		Design.UI.ICommand<object> ResetProviderCommand { get; }
		Design.UI.ICommand<object> ResetServerCommand { get; }
		Design.UI.ICommand<object> ResetCatalogCommand { get; }
		Design.UI.ICommand<object> ResetCubeCommand { get; }
	}
	public interface IDataSourceSettingsBuilder {
		void Build(IDataSourceSettingsBuilderContext context);
	}
	public interface IDataSourceSettingsBuilderContext {
		IDataSourceSettingsModel Model { get; }
		IDataProcessingMode ProcessingMode { get; }
		IDataAccessMetadata Metadata { get; set; }
		IEnumerable<IDataSourceProperty> Result { get; }
		IDataSourceSettingsBuilderContext BuildXmlPath();
		IDataSourceSettingsBuilderContext BuildExcelPath();
		IDataSourceSettingsBuilderContext BuildTables();
		IDataSourceSettingsBuilderContext BuildQuery();
		IDataSourceSettingsBuilderContext BuildElementType();
		IDataSourceSettingsBuilderContext BuildObjectType();
		IDataSourceSettingsBuilderContext BuildServiceRoot();
		IDataSourceSettingsBuilderContext BuildDefaultSorting();
		IDataSourceSettingsBuilderContext BuildSortDescriptions();
		IDataSourceSettingsBuilderContext BuildCollectionViewSettings();
		IDataSourceSettingsBuilderContext BuildBindingSourceSettings();
		IDataSourceSettingsBuilderContext BuildServerModeSettings();
		IDataSourceSettingsBuilderContext BuildInstantFeedbackSettings();
		IDataSourceSettingsBuilderContext BuildDomainDataSourceSettings();
		IDataSourceSettingsBuilderContext BuildOLAPDataSourceSettingsOLEDB();
		IDataSourceSettingsBuilderContext BuildOLAPDataSourceSettingsADOMD();
		IDataSourceSettingsBuilderContext BuildOLAPDataSourceSettingsXMLA();
		IDataSourceSettingsBuilderContext BuildCustomBindingProperties();
		IDataSourceSettingsBuilderContext BuildDesignSettings();
		void ThrowNotSupported();
	}
	#endregion DataSourceSettings
	#region DataSourceGenerator
	public interface IDataSourceGenerator {
		void Generate(IDataSourceGeneratorContext context);
	}
	public interface IDataSourceGeneratorContext {
		IModelItem ModelItem { get; }
		string DataSourceProperty { get; }
		string DataMemberProperty { get; }
		string OLAPConnectionStringProperty { get; }
		string OLAPDataProviderProperty { get; }
		string DesignTimeElementTypeProperty { get; }
		IDataSourceSettingsModel SettingsModel { get; }
		IModelItem CreateDataSource();
		IModelItem CreateDataSource(Type dataSourceType);
		void SetDataMember();
		void ClearDataMember();
		void SetDataSource(IModelItem dataSourceItem);
		void ClearDataSource();
		void SetCustomBindingProperty(string name, object value);
		void ClearCustomBindingProperties();
		IModelItemExpression GenerateDataSourceBindingExpression(object[] parameters, string format = null);
		IModelItemExpression GenerateBindingExpression(Type dataSourceType, object[] parameters, string format = null);
		IModelItemExpression GenerateExpression(object[] parameters, string format = null);
		void GenerateDataSourceAssignment(IModelItemExpression expression);
		void GenerateDataSourceAssignment(IModelItem modelItem, IModelItemExpression expression);
		void GenerateParameterAssignment(IModelItem modelItem, string parameterName, IModelItemExpression expression);
		void GenerateEvent(IModelItem modelItem, string eventName, Type eventArgsType, IModelItemExpression expression);
		void GenerateCode(IModelItemExpression expression);
		void GenerateUsing(string namespaceString);
		void SaveActiveDocument();
		void ShowCode();
	}
	#endregion
	#region Services
	public interface IDataAccessTechnologyTypesProviderFactory {
		IDataAccessTechnologyTypesProvider Create(System.IServiceProvider serviceProvider);
	}
	public interface IDataAccessTechnologyTypesProvider :
		IEnumerable<Type> {
	}
	public interface IDataAccessTechnologyComponentsProviderFactory {
		IDataAccessTechnologyComponentsProvider Create(System.IServiceProvider serviceProvider);
	}
	public interface IDataAccessTechnologyComponentsProvider : IEnumerable<IComponent> {
		object CreateComponent(string componentTypeName, System.Collections.IDictionary defaultValues);
	}
	public interface IEnumTypesProviderInfo {
		Type[] RootTypes { get; }
		string[] SkipList { get; }
	}
	public interface IDataAccessTechnologyInfoFactory {
		IDataAccessTechnologyInfo GetInfo(DataAccessTechnologyCodeName codeName, IEnumerable<Type> types);
	}
	public interface IDataAccessTechnologyNewItemService {
		void Create(DataAccessTechnologyCodeName codeName);
	}
	public interface IDataProcessingModesInfoFactory {
		IDataProcessingModesInfo GetInfo(DataAccessTechnologyCodeName codeName, IEnumerable<DataProcessingModeCodeName> allowedModes);
	}
	public interface IDataSourceInfoFactory {
		IDataSourceInfo GetInfo(IDataAccessTechnologyName technologyName, IDataAccessTechnologyInfoItem item);
	}
	public interface IDataSourceSettingsFactory {
		IDataSourceSettings GetSettings(IDataAccessTechnologyName technologyName, IDataSourceSettingsBuilderContext context);
	}
	public interface IDataSourceSettingsBuilderContextFactory {
		IDataSourceSettingsBuilderContext GetContext(IDataAccessTechnologyName technologyName, IDataProcessingMode processingMode, IDataSourceInfo info);
	}
	public interface IDataSourceGeneratorFactory {
		IDataSourceGenerator GetGenerator(IDataAccessTechnologyName technologyName, IDataSourceSettingsModel settingsModel);
	}
	public interface IDataSourceGeneratorContextFactory {
		IDataSourceGeneratorContext GetContext(IModelItem modelItem, IDataSourceSettingsModel settingsModel, IDataAccessMetadata metadata);
	}
	public static class DataSourceConfigurationConstants {
		public const string ForceInitializeNewComponent = "ForceInitializeNewComponent";
		public const string NewComponentDesignerCallback = "NewComponentDesignerCallback";
	}
	public interface IDataAccessConfigurationService {
		IEnumerable<IDataAccessTechnologyInfo> InitTechnologyInfos(Design.UI.IServiceContainer serviceContainer, UI.IDataAccessConfiguratorContext context);
		IEnumerable<IDataProcessingMode> InitProcessingModes(Design.UI.IServiceContainer serviceContainer, UI.IDataAccessConfiguratorContext configuratorContext);
		IDataSourceSettings InitSettings(Design.UI.IServiceContainer serviceContainer, UI.IDataAccessConfiguratorContext configuratorContext);
		void Configure(System.IServiceProvider serviceProvider, UI.IDataAccessConfiguratorContext configuratorContext);
		void Configure(object dataSourceComponent, Design.UI.IServiceContainer serviceContainer, UI.IDataAccessConfiguratorContext configuratorContext);
	}
	#endregion Services
}
