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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard.Templates;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using System.ComponentModel;
using System.Windows.Markup;
using DevExpress.Design;
using DevExpress.Design.SmartTags;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
#if !SL
using System.Windows.Data;
using DevExpress.Xpf.Core.DataSources;
using DesignDataSettings = DevExpress.Xpf.Core.DataSources.DesignDataSettingsExtension;
using DevExpress.Mvvm.UI;
#else
using Platform::System.Windows.Data;
using Platform::DevExpress.Xpf.Core.DataSources;
using PropertyGroupDescription = Platform::System.Windows.Data.PropertyGroupDescription;
using SortDescription = Platform::System.ComponentModel.SortDescription;
using DesignDataSettings = Platform::DevExpress.Xpf.Core.DataSources.DesignDataSettings;
using Panel = Platform::System.Windows.Controls.Panel;
using UIElement = Platform::System.Windows.UIElement;
using FrameworkElement = Platform::System.Windows.FrameworkElement;
using System.Windows.Controls;
using Microsoft.Windows.Design.Services;
using Microsoft.Windows.Design.Interaction;
using Platform::DevExpress.Mvvm.UI;
#endif
namespace DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies {
	public abstract class DataSourceGenerator : DataSourceGeneratorBase {
		protected string resourceName;
		protected override ModelItem PreGenerate(object settings, ModelItem item) {
			ConfigurationViewModelBase configurationSettings = (ConfigurationViewModelBase)((IEnumerable<object>)settings).ElementAt(0);
			this.resourceName = GeneratedObjectType.Name;
			ModelItem dataSourceItem = DesignTimeObjectModelCreateService.AddTopLevelResource(item, ref this.resourceName, GeneratedObjectType);
			if(Settings.IsStandartSource) {
				DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "ContextType", SourceItem.Type);
				DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "Path", configurationSettings.SelectedTable.Name);
			}
			return dataSourceItem;
		}
	}
	public class GridDataSourceGenerator : DataSourceGenerator {
		public static void GenerateDesignData(ModelItem item, ModelItem generateItem, ConfigurationViewModelBase configurationSettings, GeneratorSettings settings) {
			if(!configurationSettings.DesignData.IsEnable || !settings.AllowDesignData) return;
			PropertyIdentifier id = new PropertyIdentifier(typeof(DesignDataManager), "DesignData");
			ModelItem designDataItem = ModelFactory.CreateItem(item.Context, typeof(DesignDataSettings));
			if(settings.AllowTypedDesignData)
				DesignTimeObjectModelCreateService.SetProperty(designDataItem, "DataObjectType", configurationSettings.SelectedType);
			DesignTimeObjectModelCreateService.SetProperty(designDataItem, "RowCount", configurationSettings.DesignData.RowCount);
			DesignTimeObjectModelCreateService.SetProperty(designDataItem, "UseDistinctValues", configurationSettings.DesignData.IsDifferentValues);
			DesignTimeObjectModelCreateService.SetProperty(designDataItem, "FlattenHierarchy", configurationSettings.DesignData.FlattenHierarchy);
			generateItem.Properties[id].SetValue(designDataItem);
		}
		protected override void PostGenerate(object settings, ModelItem item, ModelItem generateItem) {
			ConfigurationViewModelBase configurationSettings = (ConfigurationViewModelBase)((IEnumerable<object>)settings).ElementAt(0);
			GenerateDesignData(item, generateItem, configurationSettings, Settings);
			ModelItem bindingItem = DesignTimeObjectModelCreateService.CreateBindingItem(item, Settings.BindingPath, this.resourceName);
			item.Properties["ItemsSource"].SetValue(bindingItem);
			if(InternalGenerator != null && InternalGenerator is CollectionViewDataSourceGenerator) {
				CollectionViewConfigurationViewModel cvvm = (CollectionViewConfigurationViewModel)configurationSettings;
				DesignTimeObjectModelCreateService.SetProperty(item.Properties["View"].Value, "IsSynchronizedWithCurrentItem", cvvm.IsSynchronizedWithCurrentItem, true);
			}
			item.Properties["ShowLoadingPanel"].ClearValue();
			if(Settings.IsSyncLoading) {
				ModelItem loadingBindingItem = DesignTimeObjectModelCreateService.CreateBindingItem(item, "IsLoading", this.resourceName);
				DesignTimeObjectModelCreateService.SetProperty(item, "ShowLoadingPanel", loadingBindingItem);
			}
		}
	}
	public class SelectorEditSourceGenerator : DataSourceGenerator {
		protected override void PostGenerate(object settings, ModelItem item, ModelItem generateItem) {
			CollectionViewConfigurationViewModel configurationSettings = ((IEnumerable<object>)settings).ElementAt(0) as CollectionViewConfigurationViewModel;
			if(configurationSettings != null)
				DesignTimeObjectModelCreateService.SetProperty(item, "AllowCollectionView", true);
			ModelItem bindingItem = DesignTimeObjectModelCreateService.CreateBindingItem(item, "Data", GeneratedObjectType.Name);
			item.Properties["ItemsSource"].SetValue(bindingItem);
		}
	}
	public class SelectorEditTypedSourceGenerator : SelectorEditSourceGenerator {
		protected override ModelItem PreGenerate(object settings, ModelItem item) {
			ModelItem dataSourceItem = base.PreGenerate(settings, item);
			ConfigurationViewModelBase configurationSettings = (ConfigurationViewModelBase)((IEnumerable<object>)settings).ElementAt(0);
			PropertyInfo tableInfo = (PropertyInfo)SourceItem.Members.First(p => p.Name == configurationSettings.SelectedTable.Name);
			TypeInfoDescriptor adapterDescriptor = new MethodInfoDescriptor("System.ComponentModel.Component", "System.Int32", new List<string>() { tableInfo.PropertyType.GetFullName() });
			SourceItem adapter = TypeFinder.SearchTypes(adapterDescriptor)[0];
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "AdapterType", adapter.Type);
			return dataSourceItem;
		}
	}
#if SL
	public class DomainDataSourceGenerator : DataSourceGeneratorBase {
		public DomainDataSourceGenerator(SourceItem sourceItem, Type generatedObjectType) {
			Initialize(sourceItem, null, generatedObjectType);
		}
		protected override ModelItem GenerateCore(object settings, ModelItem item) {
			ViewItem panelView = GetView(item);
			if(!IsPanelView(panelView)) {
				MessageBox.Show("Container is not valid for generation. Only panel containers are supported.", "Binding Data to Control",
					MessageBoxButton.OK, MessageBoxImage.Error);
				return null;
			}
			Panel panel = (Panel)panelView.PlatformObject;
			DomainDataSourceConfigurationViewModel sourceSettings = (DomainDataSourceConfigurationViewModel)((IEnumerable<object>)settings).ElementAt(0);
			ModelItem dataSourceItem = ModelFactory.CreateItem(item.Context, GeneratedObjectType);
			IEnumerable<string> childrenNames = panel.Children.Select(p => p is FrameworkElement ? ((FrameworkElement)p).Name : string.Empty);
			string name = DesignTimeObjectModelCreateService.GenerateKey(GeneratedObjectType.Name, childrenNames);
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "Name", name);
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "AutoLoad", sourceSettings.AutoLoad, true, true);
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "QueryName", sourceSettings.SelectedTable.Name);
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "DomainContext", ModelFactory.CreateItem(dataSourceItem.Context, SourceItem.Type));
			IList sortDescriptions = (IList)dataSourceItem.Properties["SortDescriptors"].Collection;
			IList groupDescriptions = (IList)dataSourceItem.Properties["GroupDescriptors"].Collection;
			foreach(System.ComponentModel.SortDescription sortDescription in sourceSettings.SortDescriptions)
				sortDescriptions.Add(new SortDescriptor(sortDescription.PropertyName, sortDescription.Direction));
			foreach(System.Windows.Data.PropertyGroupDescription groupDescription in sourceSettings.GroupDescriptions)
				groupDescriptions.Add(new GroupDescriptor(groupDescription.PropertyName));
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "PageSize", sourceSettings.PageSize);
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "LoadSize", sourceSettings.LoadSize);
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "LoadDelay", new TimeSpan(sourceSettings.LoadDelay));
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "LoadInterval", new TimeSpan(sourceSettings.LoadInterval));
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "RefreshInterval", new TimeSpan(sourceSettings.RefreshInterval));
			GridDataSourceGenerator.GenerateDesignData(item, dataSourceItem, sourceSettings, Settings);
			ModelItem panelItem = item.Context.Services.GetService<ViewService>().GetModel(panelView);
			panelItem.Properties["Children"].Collection.Add(dataSourceItem);
			DesignTimeObjectModelCreateService.SetProperty(item, "ItemsSource", DesignTimeObjectModelCreateService.CreateBindingItem(item, "Data", null, name));
			DesignTimeObjectModelCreateService.SetProperty(item, "ShowLoadingPanel", DesignTimeObjectModelCreateService.CreateBindingItem(item, "IsBusy", null, name));
			DesignTimeObjectModelCreateService.SetProperty(item.Properties["View"].Value, "IsSynchronizedWithCurrentItem", sourceSettings.IsSynchronizedWithCurrentItem, true);
			return dataSourceItem;
		}
		static bool IsPanelView(ViewItem view) {
			return view != null && view.PlatformObject is Panel;
		}
		static ViewItem GetView(ModelItem item) {
			ViewItem view = item.View != null ? item.View.VisualParent : null;
			return IsPanelView(view) ? view : GetFirstPanelItem(item);
		}
		static ViewItem GetFirstPanelItem(ModelItem item) {
			ModelService service = item.Context.Services.GetService<ModelService>();
			IEnumerable<ModelItem> panels = service.Find(item.Root, typeof(Panel));
			if(panels == null || panels.Count() == 0) return null;
			return panels.FirstOrDefault().View;
		}
	}
#endif
	public class CollectionViewDataSourceGenerator : DataSourceGeneratorBase {
		public CollectionViewDataSourceGenerator(SourceItem sourceItem, Type generatedObjectType) {
			Initialize(sourceItem, null, generatedObjectType);
		}
		protected override ModelItem GenerateCore(object settings, ModelItem dataSourceItem) {
			CollectionViewConfigurationViewModel collectionViewSettings = (CollectionViewConfigurationViewModel)((IEnumerable<object>)settings).ElementAt(0);
			if(dataSourceItem.Properties.Any(p => p.Name == "SortDescriptions")) {
				IList sortDescriptions = (IList)dataSourceItem.Properties["SortDescriptions"].Collection;
				foreach(System.ComponentModel.SortDescription item in collectionViewSettings.SortDescriptions)
					sortDescriptions.Add(new SortDescription(item.PropertyName, item.Direction));
			}
			if(dataSourceItem.Properties.Any(p => p.Name == "GroupDescriptions")) {
				IList groupDescriptions = (IList)dataSourceItem.Properties["GroupDescriptions"].Collection;
				foreach(System.Windows.Data.PropertyGroupDescription item in collectionViewSettings.GroupDescriptions)
					groupDescriptions.Add(new PropertyGroupDescription(item.PropertyName));
			}
			object dataSource = dataSourceItem.GetCurrentValue();
			if(dataSource is ISupportInitialize)
				((ISupportInitialize)dataSource).BeginInit();
			if(collectionViewSettings.SelectedCollectionViewType != null && dataSourceItem.Properties.Any(p => p.Name == "CollectionViewType"))
				dataSourceItem.Properties["CollectionViewType"].ComputedValue = collectionViewSettings.SelectedCollectionViewType;
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "Culture", collectionViewSettings.SelectedCulture);
			if(collectionViewSettings.IsPageSizeEnable)
				DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "PageSize", collectionViewSettings.PageSize);
			return dataSourceItem;
		}
		protected override void Destroy(ModelItem item) {
			object dataSource = item.GetCurrentValue();
			if(dataSource is ISupportInitialize)
				((ISupportInitialize)dataSource).EndInit();
		}
	}
	public class SimpleDataSourceGenerator : DataSourceGeneratorBase {
		public SimpleDataSourceGenerator(SourceItem sourceItem, Type generatedObjectType) {
			Initialize(sourceItem, null, generatedObjectType);
		}
		protected override ModelItem GenerateCore(object settings, ModelItem item) { return item; }
	}
	public class InstantFeedbackDataSourceGenerator : DataSourceGeneratorBase {
		public InstantFeedbackDataSourceGenerator(SourceItem sourceItem, Type generatedObjectType) {
			Initialize(sourceItem, null, generatedObjectType);
		}
		protected override ModelItem GenerateCore(object settings, ModelItem dataSourceItem) {
			InstantFeedBackConfigurationViewModel ifbSettings = (InstantFeedBackConfigurationViewModel)((IEnumerable<object>)settings).ElementAt(0);
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "AreSourceRowsThreadSafe", ifbSettings.AreSourceRowsThreadSafe);
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "DefaultSorting", ifbSettings.DefaultSorting);
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "KeyExpression", ifbSettings.SelectedKeyExpression);
			return dataSourceItem;
		}
	}
	public class ServerModeDataSourceGenerator : DataSourceGeneratorBase {
		public ServerModeDataSourceGenerator(SourceItem sourceItem, Type generatedObjectType) {
			Initialize(sourceItem, null, generatedObjectType);
		}
		protected override ModelItem GenerateCore(object settings, ModelItem dataSourceItem) {
			ServerModeConfigurationViewModel smSettings = (ServerModeConfigurationViewModel)((IEnumerable<object>)settings).ElementAt(0);
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "DefaultSorting", smSettings.DefaultSorting);
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "KeyExpression", smSettings.SelectedKeyExpression);
			return dataSourceItem;
		}
	}
	public class PLinqDataSourceGenerator : DataSourceGeneratorBase {
		public PLinqDataSourceGenerator(SourceItem sourceItem, Type generatedObjectType) {
			Initialize(sourceItem, null, generatedObjectType);
		}
		protected override ModelItem GenerateCore(object settings, ModelItem dataSourceItem) {
			ServerModeConfigurationViewModelBase smSettings = (ServerModeConfigurationViewModelBase)((IEnumerable<object>)settings).ElementAt(0);
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "DefaultSorting", smSettings.DefaultSorting);
			return dataSourceItem;
		}
	}
	public class PLinqServerModeDataSourceGenerator : DataSourceGeneratorBase {
		public PLinqServerModeDataSourceGenerator(SourceItem sourceItem, Type generatedObjectType) {
			Initialize(sourceItem, null, generatedObjectType);
		}
		protected override ModelItem GenerateCore(object settings, ModelItem dataSourceItem) {
			ServerModeConfigurationViewModelBase smSettings = (ServerModeConfigurationViewModelBase)((IEnumerable<object>)settings).ElementAt(0);
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "DefaultSorting", smSettings.DefaultSorting);
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "ElementType", smSettings.SelectedType);
			return dataSourceItem;
		}
	}
	public class TypedDataSourceGenerator : GridDataSourceGenerator {
		protected override ModelItem PreGenerate(object settings, ModelItem item) {
			ModelItem dataSourceItem = base.PreGenerate(settings, item);
			ConfigurationViewModelBase configurationSettings = (ConfigurationViewModelBase)((IEnumerable<object>)settings).ElementAt(0);
			PropertyInfo tableInfo = (PropertyInfo)SourceItem.Members.First(p => p.Name == configurationSettings.SelectedTable.Name);
			TypeInfoDescriptor adapterDescriptor = new MethodInfoDescriptor("System.ComponentModel.Component", "System.Int32", new List<string>() { tableInfo.PropertyType.GetFullName() });
			IList<SourceItem> items = TypeFinder.SearchTypes(adapterDescriptor);
			if(items.Count > 0) {
				SourceItem adapter = items[0];
				DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "AdapterType", adapter.Type);
			}
			return dataSourceItem;
		}
	}
	public class WcfDataSourceGenerator : GridDataSourceGenerator {
		public WcfDataSourceGenerator(SourceItem sourceItem, DataSourceGeneratorBase internalGenerator) {
			Initialize(sourceItem, internalGenerator);
		}
		protected override ModelItem PreGenerate(object settings, ModelItem item) {
			ModelItem dataSourceItem = base.PreGenerate(settings, item);
			ConfigurationViewModelBase configurationSettings = (ConfigurationViewModelBase)((IEnumerable<object>)settings).ElementAt(0);
			DesignTimeObjectModelCreateService.SetProperty(dataSourceItem, "ServiceRoot", configurationSettings.ServiceUriString);
			return dataSourceItem;
		}
	}
	class DataSourceGeneratorContainerBuilder {
		private readonly DataSourceGeneratorBase generator;
		private readonly string name;
		private readonly List<DataSourcePropertyTemplate> properties;
		private readonly ResourceDictionary dict;
		public DataSourceGeneratorContainerBuilder(string name, DataSourceGeneratorBase generator) {
			this.generator = generator;
			this.name = name;
			this.properties = new List<DataSourcePropertyTemplate>();
			this.dict = WizardResourceHelper.GetResourceDictionary();
		}
		public void BuildCollectionView(CollectionViewConfigurationViewModel viewModel) {
			this.properties.Add(new DataSourcePropertyTemplate("Descriptions", GetTemplate(dict, "CollectionViewDescriptionsTemplate")) { DataContext = viewModel });
			this.properties.Add(new DataSourcePropertyTemplate("Culture", GetTemplate(dict, "ComboBoxCulturesTemlate")) { DataContext = viewModel });
			this.properties.Add(new DataSourcePropertyTemplate("", GetTemplate(dict, "IsSynchronizedWithCurrentItemTemplate")) {
				DataContext = viewModel,
				Help = WizardResourceHelper.GetHelpStream(WizardResourceHelper.PropertiesResourceNameRoot, "SyncCurrentItem.rtf")
			});
		}
		public void BuildDesignData(ConfigurationViewModelBase viewModel) {
			List<Stream> helps = new List<Stream>();
			helps.Add(WizardResourceHelper.GetHelpStream(WizardResourceHelper.PropertiesResourceNameRoot, "ShowSampleData.rtf"));
			helps.Add(WizardResourceHelper.GetHelpStream(WizardResourceHelper.PropertiesResourceNameRoot, "DistinctValues.rtf"));
			helps.Add(WizardResourceHelper.GetHelpStream(WizardResourceHelper.PropertiesResourceNameRoot, "RowsCount.rtf"));
			helps.Add(WizardResourceHelper.GetHelpStream(WizardResourceHelper.PropertiesResourceNameRoot, "FlattenHierarchy.rtf"));
			this.properties.Add(new DataSourcePropertyTemplate("Design Time Settings", GetTemplate(dict, "DesignDataConfiguratorTemplate")) { DataContext = viewModel, ConfigurationPropertiesHelp = helps });
		}
		public void BuildDesignDataSwitcher(ConfigurationViewModelBase viewModel) {
			this.properties.Add(new DataSourcePropertyTemplate("Design Time Settings", GetTemplate(dict, "DesignDataSwitcherTemplate")) {
				DataContext = viewModel,
				Help = WizardResourceHelper.GetHelpStream(WizardResourceHelper.PropertiesResourceNameRoot, "ShowSampleData.rtf")
			});
		}
		public void BuildTables(ConfigurationViewModelBase viewModel) {
			this.properties.Add(new DataSourcePropertyTemplate("Table", GetTemplate(dict, "TablesTemplate")) { DataContext = viewModel });
		}
		public void BuildServiceRoot(ConfigurationViewModelBase viewModel) {
			DataSourcePropertyTemplate template = new DataSourcePropertyTemplate("Service Root", GetTemplate(dict, "TextBoxServiceRoot")) {
				DataContext = viewModel,
				Help = WizardResourceHelper.GetHelpStream(WizardResourceHelper.PropertiesResourceNameRoot, "ServiceRoot.rtf")
			};
			this.properties.Add(template);
		}
		public void BuildDefaultSorting(ServerModeConfigurationViewModelBase viewModel) {
			properties.Add(new DataSourcePropertyTemplate("Default Sorting", GetTemplate(dict, "DefaultSortingTemplate")) {
				DataContext = viewModel,
				Help = WizardResourceHelper.GetHelpStream(WizardResourceHelper.PropertiesResourceNameRoot, "DefaultSorting.rtf")
			});
		}
		public void BuildElementType(ConfigurationViewModelBase viewModel) {
			properties.Add(new DataSourcePropertyTemplate("Element Type", GetTemplate(dict, "TypeNameTemplate")) { DataContext = viewModel });
		}
		public void BuildServerMode(ServerModeConfigurationViewModel viewModel) {
			properties.Add(new DataSourcePropertyTemplate("Key Expression", GetTemplate(dict, "ComboBoxKeyExpressionTemlate")) {
				DataContext = viewModel,
				Help = WizardResourceHelper.GetHelpStream(WizardResourceHelper.PropertiesResourceNameRoot, "KeyExpression.rtf")
			});
			BuildDefaultSorting(viewModel);
		}
		public void BuildInstantFeedback(InstantFeedBackConfigurationViewModel viewModel) {
			BuildServerMode(viewModel);
			properties.Add(new DataSourcePropertyTemplate("", GetTemplate(dict, "CheckBoxRowsThreadSafeTemplate")) {
				DataContext = viewModel,
				Help = WizardResourceHelper.GetHelpStream(WizardResourceHelper.PropertiesResourceNameRoot, "CheckThreading.rtf")
			});
		}
		public void BuildDomainDataSource(DomainDataSourceConfigurationViewModel viewModel) {
			this.properties.Add(new DataSourcePropertyTemplate("Query", GetTemplate(dict, "QueryTemplate")) { DataContext = viewModel });
			this.properties.Add(new DataSourcePropertyTemplate("Descriptions", GetTemplate(dict, "DomainDataSourceDescriptionsTemplate")) { DataContext = viewModel });
			this.properties.Add(new DataSourcePropertyTemplate("AutoLoad", GetTemplate(dict, "AutoLoadTemplate")) { DataContext = viewModel });
			this.properties.Add(new DataSourcePropertyTemplate("Load Delay", GetTemplate(dict, "LoadDelayTemplate")) { DataContext = viewModel });
			this.properties.Add(new DataSourcePropertyTemplate("Load Interval", GetTemplate(dict, "LoadIntervalTemplate")) { DataContext = viewModel });
			this.properties.Add(new DataSourcePropertyTemplate("Load Size", GetTemplate(dict, "LoadSizeTemplate")) { DataContext = viewModel });
			this.properties.Add(new DataSourcePropertyTemplate("Refresh Interval", GetTemplate(dict, "RefreshIntervalTemplate")) { DataContext = viewModel });
		}
		public void BuildSimpleComplete(ConfigurationViewModelBase viewModel) {
			BuildTables(viewModel);
			BuildDesignData(viewModel);
		}
		public void BuildCollectionViewComplete(CollectionViewConfigurationViewModel viewModel) {
			BuildTables(viewModel);
			BuildCollectionView(viewModel);
			BuildDesignData(viewModel);
		}
		public void BuildInstantFeedbackComplete(InstantFeedBackConfigurationViewModel viewModel) {
			BuildTables(viewModel);
			BuildInstantFeedback(viewModel);
			BuildDesignData(viewModel);
		}
		public void BuildServerModeComplete(ServerModeConfigurationViewModel viewModel) {
			BuildTables(viewModel);
			BuildServerMode(viewModel);
			BuildDesignData(viewModel);
		}
		public void BuildPLinqComplete(ServerModeConfigurationViewModelBase viewModel) {
			BuildTables(viewModel);
			BuildDefaultSorting(viewModel);
			BuildDesignData(viewModel);
		}
		public DataSourceGeneratorContainer GetResult() {
			return GetDataSourceGeneratorContainer(this.name, this.properties, this.generator);
		}
		private static DataSourceGeneratorContainer GetDataSourceGeneratorContainer(string dataSourceTypeName, List<DataSourcePropertyTemplate> properties, DataSourceGeneratorBase generator) {
			return new DataSourceGeneratorContainer(dataSourceTypeName, properties, null, generator);
		}
		private static DataTemplate GetTemplate(ResourceDictionary dict, string templateName) {
			return (DataTemplate)dict[templateName];
		}
		public static DataSourceGeneratorContainer BuildAction(string name, DataSourceGeneratorBase generator, Action<DataSourceGeneratorContainerBuilder> buildAction) {
			DataSourceGeneratorContainerBuilder builder = new DataSourceGeneratorContainerBuilder(name, generator);
			buildAction(builder);
			return builder.GetResult();
		}
	}
	public static class DesignTimeObjectModelCreateService {
		public static IModelItem FindObjectInTreeByName(IModelItem root, string name) {
			if(root.Name == name)
				return root;
			foreach(IModelItem property in DesignTimeObjectModelCreateService.GetChildren(root)) {
				IModelItem result = FindObjectInTreeByName(property, name);
				if(result != null)
					return result;
			}
			return null;
		}
		public static IEnumerable<IModelItem> GetChildren(IModelItem modelItem) {
			ContentPropertyAttribute attribute = modelItem.GetAttributes(typeof(ContentPropertyAttribute)).FirstOrDefault() as ContentPropertyAttribute;
			if(attribute == null || attribute.Name == null)
				return new IModelItem[0];
			IModelProperty content = modelItem.Properties.Find(attribute.Name);
			if(content.IsSet)
				if(content.Collection != null) {
					return content.Collection;
				} else if(content.Value != null) {
					return new IModelItem[] { content.Value };
				}
			return new IModelItem[0];
		}
		public static IEnumerable<IModelItem> GetFrameworkChildren(IModelItem modelItem) {
#if DEBUGTEST
			if(modelItem is DevExpress.Xpf.Core.Design.Tests.TestModelItem)
				return GetChildren(modelItem).Where(mi => mi.ItemType.IsSubclassOf(typeof(FrameworkElement)) || mi.ItemType.IsSubclassOf(typeof(FrameworkContentElement)));
#endif
			List<IModelItem> result = new List<IModelItem>();
			foreach(IModelProperty property in modelItem.Properties) {
				if(property.IsSet)
					if(property.Collection != null) {
						result.AddRange(property.Collection.Where(mi => IsFrameworkElement(mi)));
					} else if(property.Value != null && IsFrameworkElement(property.Value))
						result.Add(property.Value);
			}
			return result;
		}
		static bool IsFrameworkElement(IModelItem modelItem) {
			return modelItem.ItemType.IsSubclassOf(typeof(FrameworkElement)) || modelItem.ItemType.IsSubclassOf(typeof(FrameworkContentElement));
		}
		public static string GenerateKey(string key, IEnumerable<string> names) {
			if(!names.Contains(key))
				return key;
			int num = 1;
			while(names.Contains(key + num))
				num++;
			return key + num;
		}
		public static IModelItem AddTopLevelResource(IModelItem item, ref string key, Type itemType) {
			IModelItemDictionary resources = item.Root.Properties["Resources"].Dictionary;
			key = GenerateKey(key, resources.Keys.Where(p => p.ItemType == typeof(string)).Select(p => (string)p.GetCurrentValue()));
			IModelItem result = item.Context.CreateItem(itemType);
			resources.Add(key, result);
			return result;
		}
		public static IModelItem AddDataContext(IModelItem item, Type dataContextType) {
			using(IModelEditingScope scope = item.BeginEdit("Set DataContext")) {
				IModelItem result = item.Context.CreateItem(dataContextType);
				SetProperty(item, "DataContext", result);
				scope.Complete();
				return result;
			}
		}
		public static string SetName(IModelItem item) {
			string name = item.ItemType.Name;
			name = char.ToLower(name[0]) + name.Substring(1);
			SetName(item, name);
			return (string)item.Name;
		}
		static IModelItem SetName(IModelItem item, string name) {
			List<string> allNames = GetAllNames(item.Root);
			if(item.Name == name)
				return item;
			string generatedName = GenerateKey(name, allNames);
			item.Name = generatedName;
			return item;
		}
		static List<string> GetAllNames(IModelItem item) {
			List<string> allNames = new List<string>();
			if(item.Name != null)
				allNames.Add(item.Name);
			foreach(IModelItem childModel in DesignTimeObjectModelCreateService.GetChildren(item))
				allNames.AddRange(GetAllNames(childModel));
			return allNames;
		}
		public static IModelItem SetProperty<T>(IModelItem item, string property, T value, T defaultValue = default(T), bool allowDefaultValues = false) {
			if(!allowDefaultValues && EqualityComparer<T>.Default.Equals(value, defaultValue) || (value is ICollection && ((ICollection)value).Count == 0))
				return item;
			return item.Properties[property].SetValue(value);
		}
#if SL
		const BindingMode DefaultBindingMode = BindingMode.OneWay;
#else
		const BindingMode DefaultBindingMode = BindingMode.Default;
#endif
		public static IModelItem CreateViewModelItem(IModelItem item, IDXTypeMetadata viewModelType) {
			if(viewModelType.IsPocoViewModel) {
				IModelItem viewModelSource = item.Context.CreateItem(typeof(ViewModelSourceExtension));
				SetProperty(viewModelSource, "Type", viewModelType.GetRuntimeType());
				return viewModelSource;
			}
			return item.Context.CreateItem(viewModelType.GetRuntimeType());
		}
		public static IModelItem CreateBindingItem(IModelItem item, string path, object staticResourceForCreating, string elementName = null, string converterResourceKey = null,
			string converterParameter = null, BindingMode mode = DefaultBindingMode, RelativeSourceMode? relativeSourceMode = null, string staticResourceKey = null,
			UpdateSourceTrigger updateSourceTrigger = default(UpdateSourceTrigger)) {
#if SL
			IModelItem bindingItem = item.Context.CreateItem(typeof(Platform::System.Windows.Data.Binding));
#else
			IModelItem bindingItem = item.Context.CreateItem(typeof(Binding));
#endif
			if(!string.IsNullOrWhiteSpace(path))
				SetProperty(bindingItem, "Path", path);
			if(staticResourceForCreating != null)
				SetProperty(bindingItem, "Source", CreateStaticResource(item, staticResourceForCreating));
			if(converterResourceKey != null)
				SetProperty(bindingItem, "Converter", CreateStaticResource(item, converterResourceKey));
			if(!string.IsNullOrEmpty(converterParameter))
				SetProperty(bindingItem, "ConverterParameter", converterParameter);
			if(relativeSourceMode != null) {
				IModelItem relativeSourceContext = bindingItem.Context.CreateItem(typeof(RelativeSource));
				SetProperty(relativeSourceContext, "Mode", relativeSourceMode);
				SetProperty(bindingItem, "RelativeSource", relativeSourceContext);
			}
			if(staticResourceKey != null) {
				IModelItem staticResource = bindingItem.Context.CreateItem(typeof(StaticResourceExtension));
				SetProperty(staticResource, "ResourceKey", staticResourceKey);
				SetProperty(bindingItem, "Source", staticResource);
			}
			if(updateSourceTrigger != UpdateSourceTrigger.Default) {
				SetProperty(bindingItem, "UpdateSourceTrigger", updateSourceTrigger);
			}
			SetProperty(bindingItem, "Mode", mode, DefaultBindingMode);
			SetProperty(bindingItem, "ElementName", elementName);
			return bindingItem;
		}
		public static IModelItem CreateReferenceBindingItem(IModelItem item, string elementName) {
#if SL
			throw new NotImplementedException();
#else
			IModelItem bindingItem = item.Context.CreateItem(typeof(Reference));
			SetProperty(bindingItem, "Name", elementName);
			return bindingItem;
#endif
		}
		public static IModelItem CreateStaticResource(IModelItem item, object resourceKey) {
#if SL
			IModelItem staticResource = item.Context.CreateItem(new DXTypeIdentifier("MS.Internal.Metadata.ExposedTypes.Presentation.StaticResourceExtension")) ??
				item.Context.CreateItem(typeof(StaticResourceExtension));
#else
			IModelItem staticResource = item.Context.CreateItem(typeof(StaticResourceExtension));
#endif
			staticResource.Properties["ResourceKey"].SetValue(resourceKey);
			return staticResource;
		}
		public static ModelItem CreateBindingItem(ModelItem item, string path, object source, string elementName = null) {
			var runtimeModelItem = (XpfModelItem)CreateBindingItem(XpfModelItem.FromModelItem(item), path, source, elementName);
			return runtimeModelItem.Value;
		}
		public static ModelItem SetProperty<T>(ModelItem item, string property, T value, T defaultValue = default(T), bool allowDefaultValues = false) {
			var runtimeModelItem = (XpfModelItem)SetProperty(XpfModelItem.FromModelItem(item), property, value, defaultValue, allowDefaultValues);
			return runtimeModelItem.Value;
		}
		public static ModelItem CreateStaticResource(ModelItem item, object resourceKey) {
			var runtimeModelItem = (XpfModelItem)CreateStaticResource(XpfModelItem.FromModelItem(item), resourceKey);
			return runtimeModelItem.Value;
		}
		public static ModelItem AddTopLevelResource(ModelItem item, ref string key, Type itemType) {
			var runtimeModelItem = (XpfModelItem)AddTopLevelResource(XpfModelItem.FromModelItem(item), ref key, itemType);
			return runtimeModelItem.Value;
		}
#if DEBUGTEST
		internal static string GenerateKeyInternal(string key, IEnumerable<string> names) {
			return GenerateKey(key, names);
		}
#endif
	}
}
