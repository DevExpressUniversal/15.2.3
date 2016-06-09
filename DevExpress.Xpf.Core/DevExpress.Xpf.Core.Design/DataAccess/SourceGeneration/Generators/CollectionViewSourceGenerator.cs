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
	using DevExpress.Xpf.Core.DataSources;
	using DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies;
	using DataAccessPlatform = DevExpress.Design.DataAccess.Wpf;
	using DesignModel = Microsoft.Windows.Design.Model;
	using DesignTimeService = DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies;
	abstract class BaseCollectionViewSourceGenerator : BaseXamlDataSourceGenerator {
		protected override void SetDataSource(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			if(dataSourceItem.ItemType == typeof(System.Windows.Data.CollectionViewSource)) {
				var xamlGeneratorContext = context as DataAccessPlatform.IXAMLDataSourceGeneratorContext;
				var bindingItem = DesignTimeService.DesignTimeObjectModelCreateService.CreateBindingItem(
					((DesignModel.ModelItem)context.ModelItem.Value), null, xamlGeneratorContext.DataSourceResourceName);
				context.SetCustomBindingProperty(context.DataSourceProperty, bindingItem);
			}
			else base.SetDataSource(dataSourceItem, context);
		}
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			ICollectionViewSettingsModel collectionViewSettingsModel = context.SettingsModel as ICollectionViewSettingsModel;
			var collectionViewType = GetCollectionViewType(collectionViewSettingsModel.SelectedCollectionViewType);
			SetCollectionViewType(dataSourceItem, collectionViewType);
			SetupCollectionViewSource(dataSourceItem, collectionViewSettingsModel);
			SetIsSynchronizedWithCurrentItem(context.ModelItem, collectionViewSettingsModel.IsSynchronizedWithCurrentItem);
		}
		protected override bool GenerateCodeBehind(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			if(dataSourceItem.ItemType == typeof(System.Windows.Data.CollectionViewSource)) {
				var dataSourceModelItem = dataSourceItem.Value as Microsoft.Windows.Design.Model.ModelItem;
				if(dataSourceModelItem != null) {
					var supportInitializeDataSource = dataSourceModelItem.GetCurrentValue() as System.ComponentModel.ISupportInitialize;
					if(supportInitializeDataSource != null)
						supportInitializeDataSource.EndInit();
				}
			}
			return base.GenerateCodeBehind(dataSourceItem, context);
		}
		static void SetCollectionViewType(IModelItem dataSourceItem, System.Type collectionViewType) {
			if(dataSourceItem.ItemType == typeof(System.Windows.Data.CollectionViewSource)) {
				var dataSourceModelItem = dataSourceItem.Value as Microsoft.Windows.Design.Model.ModelItem;
				if(dataSourceModelItem != null) {
					using(var collectionViewSourceEditingScope = dataSourceItem.BeginEdit()) {
						var supportInitializeDataSource = dataSourceModelItem.GetCurrentValue() as System.ComponentModel.ISupportInitialize;
						if(supportInitializeDataSource != null)
							supportInitializeDataSource.BeginInit();
						dataSourceModelItem.Properties["CollectionViewType"].ComputedValue = collectionViewType;
					}
				}
			}
			else SetProperty(dataSourceItem, "CollectionViewType", collectionViewType);
		}
		static void SetupCollectionViewSource(IModelItem dataSourceItem, ICollectionViewSettingsModel collectionViewSettingsModel) {
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.Culture, collectionViewSettingsModel.SelectedCulture);
			var sortDescriptions = dataSourceItem.Properties["SortDescriptions"].Collection;
			foreach(var sortItem in collectionViewSettingsModel.SortDescriptions)
				sortDescriptions.Add(new System.ComponentModel.SortDescription(sortItem.PropertyName, sortItem.Direction));
			var groupDescriptions = dataSourceItem.Properties["GroupDescriptions"].Collection;
			foreach(var groupItem in collectionViewSettingsModel.GroupDescriptions)
				groupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription(groupItem.PropertyName));
		}
		static System.Type GetCollectionViewType(string selectedCollectionViewType) {
			switch(selectedCollectionViewType) {
				case "ListCollectionView":
					return typeof(System.Windows.Data.ListCollectionView);
				case "BindingListCollectionView":
					return typeof(System.Windows.Data.BindingListCollectionView);
				default:
					return typeof(System.Windows.Data.CollectionView);
			}
		}
		static void SetIsSynchronizedWithCurrentItem(IModelItem modelItem, bool isSynchronizedWithCurrentItem) {
			IModelItem syncTargetModelItem = null;
			if(IsAssignableFrom(modelItem.ItemType, typeof(Xpf.Editors.LookUpEditBase)))
				syncTargetModelItem = modelItem;
			if(IsAssignableFrom(modelItem.ItemType, "DevExpress.Xpf.Grid.DataControlBase"))
				syncTargetModelItem = modelItem.Properties["View"].Value;
			if(syncTargetModelItem != null)
				SetProperty(syncTargetModelItem, DataSourcePropertyCodeName.IsSynchronizedWithCurrentItem, isSynchronizedWithCurrentItem);
		}
		static bool IsAssignableFrom(System.Type type, System.Type baseType) {
			return baseType.IsAssignableFrom(type);
		}
		static bool IsAssignableFrom(System.Type type, string baseTypeName) {
			while(type != null) {
				if(type.FullName == baseTypeName)
					return true;
				type = type.BaseType;
			}
			return false;
		}
	}
	class TypedDataSetCollectionViewSourceGenerator : BaseCollectionViewSourceGenerator {
		protected override System.Type GetDataSourceType() {
			return typeof(Xpf.Core.DataSources.TypedCollectionViewSource);
		}
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			base.InitializeDataSourceItem(dataSourceItem, context);
			SetContextType(dataSourceItem, context);
			SetPath(dataSourceItem, context);
			SetProperty(dataSourceItem, AdapterTypeProperty, GetTableAdapterType(context));
		}
	}
	class EntityFrameworkCollectionViewSourceGenerator : BaseCollectionViewSourceGenerator {
		protected override System.Type GetDataSourceType() {
			return typeof(Xpf.Core.DataSources.EntityCollectionViewSource);
		}
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			base.InitializeDataSourceItem(dataSourceItem, context);
			SetContextType(dataSourceItem, context);
			SetPath(dataSourceItem, context);
		}
	}
	class LinqToSqlCollectionViewSourceGenerator : BaseCollectionViewSourceGenerator {
		protected override System.Type GetDataSourceType() {
			return typeof(Xpf.Core.DataSources.LinqCollectionViewDataSource);
		}
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			base.InitializeDataSourceItem(dataSourceItem, context);
			SetContextType(dataSourceItem, context);
			SetPath(dataSourceItem, context);
		}
	}
	class WcfCollectionViewSourceGenerator : BaseCollectionViewSourceGenerator {
		protected override System.Type GetDataSourceType() {
			return typeof(WcfCollectionViewSource);
		}
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			base.InitializeDataSourceItem(dataSourceItem, context);
			SetContextType(dataSourceItem, context);
			SetPath(dataSourceItem, context);
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.ServiceRoot, GetServiceUri(context));
		}
	}
	class IEnumerableCollectionViewSourceGenerator : BaseCollectionViewSourceGenerator {
		protected override System.Type GetDataSourceType() {
			return typeof(System.Windows.Data.CollectionViewSource);
		}
		protected override void InitializeDesignData(IModelItem designDataItem, IDataSourceGeneratorContext context) {
			SetProperty(designDataItem, DataObjectTypeProperty, GetElementType(context));
			base.InitializeDesignData(designDataItem, context);
		}
	}
}
