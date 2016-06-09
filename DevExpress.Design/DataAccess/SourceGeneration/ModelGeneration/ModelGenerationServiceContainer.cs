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

namespace DevExpress.Design.Win.ModelGeneration {
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
			externalServiceProvider = null;
			GC.SuppressFinalize(this);
		}
		IServiceProvider externalServiceProvider;
		void IModelGenerationServiceContainer.Initialize(IServiceProvider externalServiceProvider) {
			this.externalServiceProvider = externalServiceProvider;
		}
		public TService GetService<TService>() where TService : class {
			if(typeof(TService) == typeof(IModelService))
				return this as TService;
			return externalServiceProvider.GetService(typeof(TService)) as TService;
		}
		IModelItem IModelService.CreateModelItem(object value) {
			return new ModelItem(editingContext, value, value.GetType());
		}
		IModelItem IModelService.CreateModelItem(Type type) {
			return new ModelItem(editingContext, type);
		}
		IModelItemExpression IModelService.CreateModelItemExpression(IModelItem item, string expressionString) {
			return new ModelItemExpression(item, expressionString);
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
			Type type;
			object value;
			public ModelItem(IEditingContext context, object value, Type type) {
				AssertionException.IsNotNull(value);
				AssertionException.IsTrue(type.IsAssignableFrom(value.GetType()));
				this.context = context;
				this.value = value;
				this.type = type;
			}
			public ModelItem(IEditingContext context, Type type) {
				this.context = context;
				this.type = type;
			}
			public IModelEditingScope BeginEdit() {
				return new ModelEditingScope(this);
			}
			public IModelEditingScope BeginEdit(string description) {
				return new ModelEditingScope(this, description);
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
				get {
					if(!typeof(IComponent).IsAssignableFrom(ItemType))
						return null;
					var rItem = value as DevExpress.XtraEditors.Repository.RepositoryItem;
					if(rItem != null && rItem.OwnerEdit != null)
						return GetName(rItem.OwnerEdit) + ".Properties";
					if(value == null)
						value = CreateComponent();
					return GetName(value as IComponent);
				}
			}
			static string GetName(IComponent component) {
				if(component != null && component.Site != null)
					return component.Site.Name;
				return null;
			}
			public Type ItemType {
				get { return type; }
			}
			public object Value {
				get {
					if(value == null)
						this.value = CreateValueObject();
					return value;
				}
			}
			protected object CreateValueObject() {
				if(typeof(IComponent).IsAssignableFrom(ItemType))
					return CreateComponent();
				return Activator.CreateInstance(ItemType);
			}
			IComponent CreateComponent() {
				return ServiceProvider.GetService<IDesignerHost>().CreateComponent(ItemType);
			}
		}
		class ModelItemExpression : IModelItemExpression {
			IModelItem sourceItem;
			string expressionString;
			public ModelItemExpression(IModelItem sourceItem, string expressionString) {
				this.sourceItem = sourceItem;
				this.expressionString = expressionString;
			}
			public string ExpressionString {
				get { return expressionString; }
			}
			public string Name {
				get { return sourceItem.Name; }
			}
			public Type ItemType {
				get { return sourceItem.ItemType; }
			}
			public object Value {
				get { return sourceItem.Value; }
			}
			public IEditingContext EditingContext {
				get { return sourceItem.EditingContext; }
			}
			#region IModelItem Members
			IModelEditingScope IModelItem.BeginEdit() {
				throw new NotImplementedException();
			}
			IModelEditingScope IModelItem.BeginEdit(string description) {
				throw new NotImplementedException();
			}
			IModelPropertyCollection IModelItem.Properties {
				get { throw new NotImplementedException(); }
			}
			#endregion
		}
		class ModelPropertyCollection : IModelPropertyCollection {
			IModelItem item;
			IDictionary<string, IModelProperty> properties;
			public ModelPropertyCollection(IModelItem item) {
				this.item = item;
				this.properties = new Dictionary<string, IModelProperty>();
			}
			PropertyDescriptorCollection propertyDescriptors;
			public IModelProperty this[string propertyName] {
				get {
					IModelProperty property;
					if(!properties.TryGetValue(propertyName, out property)) {
						if(propertyDescriptors == null)
							propertyDescriptors = TypeDescriptor.GetProperties(item.Value);
						PropertyDescriptor descriptor = propertyDescriptors[propertyName];
						if(descriptor != null) {
							property = new ModelProperty(item, descriptor);
							properties.Add(propertyName, property);
						}
					}
					return property;
				}
			}
		}
		class ModelProperty : IModelProperty {
			IModelItem item;
			PropertyDescriptor descriptor;
			public ModelProperty(IModelItem item, PropertyDescriptor descriptor) {
				this.item = item;
				this.descriptor = descriptor;
			}
			#region IModelProperty Members
			public string Name {
				get { return descriptor.Name; }
			}
			IModelItem itemValue;
			public IModelItem Value {
				get {
					if(itemValue == null)
						itemValue = new ModelItem(item.EditingContext, descriptor.GetValue(item.Value), descriptor.PropertyType);
					return itemValue;
				}
			}
			object localValueCore;
			public object ReadLocalValue() {
				return localValueCore;
			}
			public IModelItem SetValue(object value) {
				if(!descriptor.IsReadOnly) {
					descriptor.SetValue(item.Value, value);
					this.localValueCore = value;
					this.itemValue = new ModelItem(item.EditingContext, descriptor.PropertyType);
				}
				return itemValue;
			}
			public void ClearValue() {
				if(descriptor.IsReadOnly) return;
				descriptor.ResetValue(item.Value);
				this.localValueCore = null;
				this.itemValue = null;
			}
			IModelItemCollection collectionCore;
			public IModelItemCollection Collection {
				get {
					if(collectionCore == null)
						collectionCore = new ModelItemCollection(item, descriptor);
					return collectionCore;
				}
			}
			#endregion
		}
		class ModelItemCollection : IModelItemCollection {
			IModelItem item;
			PropertyDescriptor descriptor;
			public ModelItemCollection(IModelItem item, PropertyDescriptor descriptor) {
				this.item = item;
				this.descriptor = descriptor;
			}
			#region IModelItemCollection Members
			public void Add(object value) {
				IList list = descriptor.GetValue(item.Value) as IList;
				if(list != null)
					list.Add(value);
			}
			public void Add(IModelItem valueItem) {
				IList list = descriptor.GetValue(item.Value) as IList;
				if(list != null)
					list.Add(valueItem.Value);
			}
			public void Remove(object value) {
				IList list = descriptor.GetValue(item.Value) as IList;
				if(list != null)
					list.Remove(value);
			}
			public void Remove(IModelItem valueItem) {
				IList list = descriptor.GetValue(item.Value) as IList;
				if(list != null)
					list.Remove(valueItem.Value);
			}
			#endregion
			#region IEnumerable<IModelItem> Members
			public IEnumerator<IModelItem> GetEnumerator() {
				IEnumerable enumerable = descriptor.GetValue(item.Value) as IEnumerable;
				foreach(object childItem in enumerable)
					yield return new ModelItem(item.EditingContext, childItem, childItem.GetType());
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
			#endregion
		}
		class ModelEditingScope : IModelEditingScope {
			DesignerTransaction designerTransaction;
			public ModelEditingScope(IModelItem item) {
				var host = item.EditingContext.ServiceProvider.GetService<IDesignerHost>();
				if(host != null) designerTransaction = host.CreateTransaction();
			}
			public ModelEditingScope(IModelItem item, string description) {
				var host = item.EditingContext.ServiceProvider.GetService<IDesignerHost>();
				if(host != null) designerTransaction = host.CreateTransaction(description);
			}
			bool isComplete;
			void IDisposable.Dispose() {
				if(!isComplete)
					Complete();
				DisposeTransaction();
				GC.SuppressFinalize(this);
			}
			void DisposeTransaction() {
				if(designerTransaction != null)
					((IDisposable)designerTransaction).Dispose();
				designerTransaction = null;
			}
			public void Complete() {
				if(designerTransaction != null)
					designerTransaction.Commit();
				isComplete = true;
			}
		}
		#endregion Model Classes
	}
}
