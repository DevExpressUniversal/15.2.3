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

namespace DevExpress.Design.Wpf.ModelGeneration {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.Design;
	using DevExpress.Design.DataAccess;
	using DevExpress.Design.CodeGenerator;
	class ModelGenerationServiceContainer : IModelGenerationServiceContainer, IModelServiceProvider, IModelService {
		IEditingContext editingContext;
		public ModelGenerationServiceContainer() {
			editingContext = new EditingContext(this);
		}
		void IDisposable.Dispose() {
			this.externalServiceProvider = null;
			GC.SuppressFinalize(this);
		}
		IServiceProvider externalServiceProvider;
		void IModelGenerationServiceContainer.Initialize(IServiceProvider externalServiceProvider) {
			this.externalServiceProvider = externalServiceProvider;
		}
		public TService GetService<TService>() where TService : class {
			if(typeof(TService) == typeof(IModelService))
				return this as TService;
			return this.externalServiceProvider.GetService(typeof(TService)) as TService;
		}
		IModelItem IModelService.CreateModelItem(object value) {
			return new ModelItem(editingContext, (Microsoft.Windows.Design.Model.ModelItem)value);
		}
		IModelItem IModelService.CreateModelItem(Type type) {
			throw new NotSupportedException();
		}
		IModelItemExpression IModelService.CreateModelItemExpression(IModelItem item, string expressionString) {
			throw new NotSupportedException();
		}
		#region Model Classes
		class EditingContext : IEditingContext {
			IModelServiceProvider provider;
			public EditingContext(IModelServiceProvider provider) {
				this.provider = provider;
			}
			public IModelItem CreateItem(Type type) {
				return ServiceProvider.GetService<IModelService>().CreateModelItem(type);
			}
			public IModelItem CreateItem(Type type, object parameter) {
				return ServiceProvider.GetService<IModelService>().CreateModelItem(type);
			}
			public IModelItemExpression CreateExpression(IModelItem item, string expressionString) {
				return ServiceProvider.GetService<IModelService>().CreateModelItemExpression(item, expressionString);
			}
			public IModelServiceProvider ServiceProvider {
				get { return provider; }
			}
		}
		class ModelItem : IModelItem {
			IEditingContext context;
			Microsoft.Windows.Design.Model.ModelItem modelItemCore;
			public ModelItem(IEditingContext context, Microsoft.Windows.Design.Model.ModelItem modelItem) {
				this.context = context;
				this.modelItemCore = modelItem;
			}
			public IModelEditingScope BeginEdit() {
				return new ModelEditingScope(modelItemCore);
			}
			public IModelEditingScope BeginEdit(string description) {
				return new ModelEditingScope(modelItemCore, description);
			}
			public IEditingContext EditingContext {
				get { return context; }
			}
			public IModelServiceProvider ServiceProvider {
				get { return context.ServiceProvider; }
			}
			IModelPropertyCollection propertiesCore;
			public IModelPropertyCollection Properties {
				get {
					if(propertiesCore == null)
						propertiesCore = new ModelPropertyCollection(this);
					return propertiesCore;
				}
			}
			public string Name {
				get { return modelItemCore.Name; }
			}
			public Type ItemType {
				get { return modelItemCore.ItemType; }
			}
			public object Value {
				get { return modelItemCore; }
			}
		}
		class ModelPropertyCollection : IModelPropertyCollection {
			IModelItem item;
			IDictionary<string, IModelProperty> properties;
			public ModelPropertyCollection(IModelItem item) {
				this.item = item;
				this.properties = new Dictionary<string, IModelProperty>();
			}
			Microsoft.Windows.Design.Model.ModelPropertyCollection modelPropertyCollection;
			public IModelProperty this[string propertyName] {
				get {
					IModelProperty property;
					if(!properties.TryGetValue(propertyName, out property)) {
						var modelItem = item.Value as Microsoft.Windows.Design.Model.ModelItem;
						if(modelPropertyCollection == null)
							modelPropertyCollection = modelItem.Properties;
						var modelProperty = modelPropertyCollection[propertyName];
						if(modelProperty != null) {
							property = new ModelProperty(item, modelProperty);
							properties.Add(propertyName, property);
						}
					}
					return property;
				}
			}
		}
		class ModelProperty : IModelProperty {
			IModelItem item;
			Microsoft.Windows.Design.Model.ModelProperty modelProperty;
			public ModelProperty(IModelItem item, Microsoft.Windows.Design.Model.ModelProperty modelProperty) {
				this.item = item;
				this.modelProperty = modelProperty;
			}
			#region IModelProperty Members
			public string Name {
				get { return modelProperty.Name; }
			}
			IModelItem itemValue;
			public IModelItem Value {
				get {
					if(itemValue == null)
						itemValue = new ModelItem(item.EditingContext, modelProperty.Value);
					return itemValue;
				}
			}
			object localValueCore;
			public object ReadLocalValue() {
				return localValueCore;
			}
			public IModelItem SetValue(object value) {
				if(!modelProperty.IsReadOnly) {
					this.localValueCore = value;
					var valueItem = modelProperty.SetValue(value);
					this.itemValue = new ModelItem(item.EditingContext, valueItem);
				}
				return itemValue;
			}
			public void ClearValue() {
				if(modelProperty.IsReadOnly) return;
				modelProperty.ClearValue();
				this.itemValue = null;
				this.localValueCore = null;
			}
			IModelItemCollection collectionCore;
			public IModelItemCollection Collection {
				get {
					if(collectionCore == null)
						collectionCore = new ModelItemCollection(item, modelProperty);
					return collectionCore;
				}
			}
			#endregion
		}
		class ModelItemCollection : IModelItemCollection {
			IModelItem item;
			Microsoft.Windows.Design.Model.ModelProperty modelProperty;
			public ModelItemCollection(IModelItem item, Microsoft.Windows.Design.Model.ModelProperty modelProperty) {
				this.item = item;
				this.modelProperty = modelProperty;
			}
			#region IModelItemCollection Members
			public void Add(object value) {
				if(modelProperty.IsCollection)
					modelProperty.Collection.Add(value);
			}
			public void Add(IModelItem valueItem) {
				if(modelProperty.IsCollection)
					modelProperty.Collection.Add((Microsoft.Windows.Design.Model.ModelItem)valueItem.Value);
			}
			public void Remove(object value) {
				if(modelProperty.IsCollection)
					modelProperty.Collection.Remove(value);
			}
			public void Remove(IModelItem valueItem) {
				if(modelProperty.IsCollection)
					modelProperty.Collection.Remove((Microsoft.Windows.Design.Model.ModelItem)valueItem.Value);
			}
			#endregion
			#region IEnumerable<IModelItem> Members
			public IEnumerator<IModelItem> GetEnumerator() {
				if(modelProperty.IsCollection) {
					using(var e = modelProperty.Collection.GetEnumerator()) {
						while(e.MoveNext())
							yield return new ModelItem(item.EditingContext, e.Current);
					}
				}
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
			#endregion
		}
		class ModelEditingScope : IModelEditingScope {
			Microsoft.Windows.Design.Model.ModelItem modelItem;
			Microsoft.Windows.Design.Model.ModelEditingScope scope;
			public ModelEditingScope(Microsoft.Windows.Design.Model.ModelItem modelItem) {
				this.modelItem = modelItem;
				scope = modelItem.BeginEdit();
			}
			public ModelEditingScope(Microsoft.Windows.Design.Model.ModelItem modelItem, string description) {
				this.modelItem = modelItem;
				scope = modelItem.BeginEdit(description);
			}
			bool isComplete;
			void IDisposable.Dispose() {
				if(!isComplete)
					Complete();
				this.scope = null;
				this.modelItem = null;
				GC.SuppressFinalize(this);
			}
			public void Complete() {
				if(scope != null)
					scope.Complete();
				isComplete = true;
			}
		}
		#endregion Model Classes
	}
}
