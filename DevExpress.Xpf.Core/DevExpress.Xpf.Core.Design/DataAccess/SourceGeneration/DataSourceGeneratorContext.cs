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

namespace DevExpress.Design.DataAccess.Wpf {
	using DevExpress.Xpf.Core.DataSources;
	using DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies;
	using System;
	using DesignModel = Microsoft.Windows.Design.Model;
	using DesignTimeService = DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies;
	public interface IXAMLDataSourceGeneratorContext {
		string DataSourceResourceName { get; }
	}
	class DataSourceGeneratorContextFactory : BaseDataSourceGeneratorContextFactory {
#if DEBUGTEST
		protected override
#else
		protected sealed override
#endif
		IDataSourceGeneratorContext GetContextCore(IModelItem modelItem, IDataSourceSettingsModel settingsModel, IDataAccessMetadata metadata) {
			return new DataSourceGeneratorContext(modelItem, settingsModel, metadata);
		}
#if DEBUGTEST
	protected
#endif
	class DataSourceGeneratorContext : BaseDataSourceGeneratorContext, IXAMLDataSourceGeneratorContext {
			string resourceName;
			Type resourceType;
			public DataSourceGeneratorContext(IModelItem modelItem, IDataSourceSettingsModel settingsModel, IDataAccessMetadata metadata) :
				base(modelItem, settingsModel, metadata) {
			}
			public override IModelItem CreateDataSource(System.Type dataSourceType) {
				resourceName = dataSourceType.Name;
				resourceType = dataSourceType;
				var dataSourceModelItem = DesignTimeService.DesignTimeObjectModelCreateService.AddTopLevelResource(
					((DesignModel.ModelItem)ModelItem.Value), ref resourceName, dataSourceType);
				if(SettingsModel.IsDesignDataAllowed && SettingsModel.ShowDesignData)
					CreateDesignData(dataSourceModelItem);
				return ModelItem.EditingContext.ServiceProvider.GetService<IModelService>().CreateModelItem(dataSourceModelItem);
			}
			void CreateDesignData(Microsoft.Windows.Design.Model.ModelItem dataSourceModelItem) {
				var designDataPropertyID = new Microsoft.Windows.Design.Metadata.PropertyIdentifier(
					typeof(DevExpress.Xpf.Core.DataSources.DesignDataManager), "DesignData");
				var designDataItem = DesignModel.ModelFactory.CreateItem(
					dataSourceModelItem.Context, typeof(DesignDataSettings));
				dataSourceModelItem.Properties[designDataPropertyID].SetValue(designDataItem);
			}
			public sealed override void SetDataSource(IModelItem dataSourceItem) {
				var bindingItem = DesignTimeService.DesignTimeObjectModelCreateService.CreateBindingItem(
					((DesignModel.ModelItem)ModelItem.Value), GetDataMemberBindingProperty(dataSourceItem.ItemType), resourceName);
				NestedPropertyHelper.Set(ModelItem, DataSourceProperty, bindingItem);
			}
			string GetDataMemberBindingProperty(System.Type dataSourceItemType) {
				if(dataSourceItemType.GetProperty(DataMemberProperty) == null) return null;
				return DataMemberProperty;
			}
			public sealed override void ClearDataSource() {
				NestedPropertyHelper.Clear(ModelItem, DataSourceProperty);
			}
			public sealed override void SetCustomBindingProperty(string propertyName, object value) {
				if(!string.IsNullOrEmpty(propertyName))
					NestedPropertyHelper.Set(ModelItem, propertyName, value);
			}
			public sealed override void ClearCustomBindingProperties() {
				foreach(var property in metadata.CustomBindingProperties)
					NestedPropertyHelper.Clear(ModelItem, property.PropertyName);
			}
			#region Code Generation NotSupported
			public override void SetDataMember() {
				throw new System.NotSupportedException();
			}
			public override void ClearDataMember() {
				throw new System.NotImplementedException();
			}
			public override IModelItemExpression GenerateBindingExpression(System.Type dataSourceType, object[] parameters, string format) {
				throw new System.NotSupportedException();
			}
			public override IModelItemExpression GenerateExpression(object[] parameters, string format) {
				throw new System.NotSupportedException();
			}
			public override void GenerateCode(IModelItemExpression expression) {
				throw new System.NotImplementedException();
			}
			public override void GenerateParameterAssignment(IModelItem modelItem, string parameterName, IModelItemExpression expression) {
				throw new System.NotSupportedException();
			}
			public override void GenerateEvent(IModelItem modelItem, string eventName, System.Type eventArgsType, IModelItemExpression expression) {
				throw new System.NotSupportedException();
			}
			public override void GenerateUsing(string namespaceString) {
				throw new System.NotImplementedException();
			}
			#endregion Code Generation NotSupported
			public override void ShowCode() {
			}
			string IXAMLDataSourceGeneratorContext.DataSourceResourceName {
				get { return resourceName; }
			}
		}
	}
}
