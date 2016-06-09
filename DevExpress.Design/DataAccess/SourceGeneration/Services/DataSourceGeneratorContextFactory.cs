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

namespace DevExpress.Design.DataAccess.Win {
	using System.Collections.Generic;
	using DevExpress.Design.CodeGenerator;
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
		class DataSourceGeneratorContext : BaseDataSourceGeneratorContext {
			IList<object> generatedComponents = new List<object>();
			public DataSourceGeneratorContext(IModelItem modelItem, IDataSourceSettingsModel settingsModel, IDataAccessMetadata metadata) :
				base(modelItem, settingsModel, metadata) {
			}
			public sealed override IModelItem CreateDataSource(System.Type dataSourceType) {
				IModelItem modelItem = ModelItem.EditingContext.CreateItem(dataSourceType);
				if(typeof(System.ComponentModel.IComponent).IsAssignableFrom(dataSourceType))
					generatedComponents.Add(new System.WeakReference(modelItem.Value));
				return modelItem;
			}
			public sealed override void SetDataSource(IModelItem dataSourceItem) {
				NestedPropertyHelper.Set(ModelItem, DataSourceProperty, dataSourceItem.Value);
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
			public sealed override void SetDataMember() {
				var dataMemberProperty = NestedPropertyHelper.GetProperty(ModelItem, DataMemberProperty);
				if(dataMemberProperty == null) return;
				try {
					object dataSourceLocalValue = null;
					var dataSourceProperty = NestedPropertyHelper.GetProperty(ModelItem, DataSourceProperty);
					if(dataSourceProperty != null && dataSourceProperty.Value != null)
						dataSourceLocalValue = dataSourceProperty.ReadLocalValue();
					if(dataSourceLocalValue != null)
						dataMemberProperty.SetValue(CheckDataTableName(dataSourceLocalValue, SettingsModel.SelectedElement.Name));
					else
						dataMemberProperty.SetValue(SettingsModel.SelectedElement.Name);
				}
				catch(System.ArgumentException) { dataMemberProperty.ClearValue(); }
			}
			public sealed override void ClearDataMember() {
				var dataMemberProperty = NestedPropertyHelper.GetProperty(ModelItem, DataMemberProperty);
				if(dataMemberProperty != null) dataMemberProperty.ClearValue();
			}
			public sealed override IModelItemExpression GenerateBindingExpression(System.Type dataSourceType, object[] parameters, string format) {
				string expressionString = DataSourceGeneratorFormatter.GetExpressionString(dataSourceType, parameters, format);
				return ModelItem.EditingContext.CreateExpression(ModelItem, expressionString);
			}
			public sealed override IModelItemExpression GenerateExpression(object[] parameters, string format) {
				CheckModelItemNames(parameters);
				return ModelItem.EditingContext.CreateExpression(ModelItem, string.Format(format, parameters));
			}
			void CheckModelItemNames(object[] parameters) {
				for(int i = 0; i < parameters.Length; i++) {
					IModelItem modelItem = parameters[i] as IModelItem;
					if(modelItem != null)
						parameters[i] = modelItem.Name ?? modelItem.ItemType.Name;
				}
			}
			int generatedLine = -1;
			public override void GenerateParameterAssignment(IModelItem modelItem, string parameterName, IModelItemExpression expression) {
				IModelServiceProvider serviceProvider = ModelItem.EditingContext.ServiceProvider;
				string assignmentString = DataSourceGeneratorFormatter.GetAssignmentFormat(modelItem.Name, parameterName, expression, metadata.Platform);
				generatedLine = CodeGenerator.AppendConstructorBody(serviceProvider, assignmentString);
			}
			public override void GenerateCode(IModelItemExpression expression) {
				IModelServiceProvider serviceProvider = ModelItem.EditingContext.ServiceProvider;
				string expressionString = DataSourceGeneratorFormatter.GetCodeFormat(expression, metadata.Platform);
				generatedLine = CodeGenerator.AppendConstructorBody(serviceProvider, expressionString);
				generatedComponents.Clear();
			}
			public override void GenerateEvent(IModelItem modelItem, string eventName, System.Type eventArgsType, IModelItemExpression expression) {
				IModelServiceProvider serviceProvider = ModelItem.EditingContext.ServiceProvider;
				var eventService = serviceProvider.GetService<System.ComponentModel.Design.IEventBindingService>();
				if(eventService != null) {
					var component = (System.ComponentModel.IComponent)modelItem.Value;
					var eventDescriptors = System.ComponentModel.TypeDescriptor.GetEvents(component);
					System.ComponentModel.EventDescriptor eventDescriptor = eventDescriptors[eventName];
					if(eventDescriptor != null) {
						string eventHandlerName = eventService.CreateUniqueMethodName(component, eventDescriptor);
						generatedLine = CodeGenerator.AddEventHandler(serviceProvider, modelItem.Name,
							eventName, eventHandlerName, eventArgsType, expression.ExpressionString, metadata.Platform);
					}
				}
			}
			public override void GenerateUsing(string namespaceString) {
				IModelServiceProvider serviceProvider = ModelItem.EditingContext.ServiceProvider;
				string expressionString = DataSourceGeneratorFormatter.GetUsingFormat(namespaceString);
				generatedLine = CodeGenerator.AddUsing(serviceProvider, namespaceString, expressionString);
			}
			public override void ShowCode() {
				IModelServiceProvider serviceProvider = ModelItem.EditingContext.ServiceProvider;
				var eventService = serviceProvider.GetService<System.ComponentModel.Design.IEventBindingService>();
				if(eventService != null) {
					try {
						if(generatedLine != -1) {
							eventService.ShowCode(generatedLine);
						}
						else {
							EnvDTE.CodeClass rootClass = CodeNavigator.GetRootComponentClass(serviceProvider);
							if(rootClass != null) {
								EnvDTE.CodeFunction rootClassConstructor = CodeNavigator.GetCodeClassConstructor(rootClass);
								if(rootClassConstructor != null)
									eventService.ShowCode(rootClassConstructor.EndPoint.Line);
								else
									eventService.ShowCode(rootClass.StartPoint.Line);
							}
						}
					}
					catch { }
				}
			}
		}
		#region internal
		internal static string CheckDataTableName(object dataSourceValue, string tableName) {
			var dataSet = dataSourceValue as System.Data.DataSet;
			if(dataSet != null) {
				if(!dataSet.Tables.Contains(tableName)) {
					if(tableName.Contains("_")) {
						return CheckDataTableName(dataSourceValue, tableName.Replace("_", " "));
					}
				}
			}
			return tableName;
		}
		#endregion internal
	}
}
