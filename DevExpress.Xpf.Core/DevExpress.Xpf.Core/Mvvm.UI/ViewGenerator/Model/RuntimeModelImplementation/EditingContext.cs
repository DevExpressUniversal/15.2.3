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

using DevExpress.Mvvm.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Mvvm.UI.Native.ViewGenerator.Model {
	public class RuntimeModelEditingScope : IModelEditingScope {
		public RuntimeModelEditingScope(string description) {
		}
		string IModelEditingScope.Description {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		void IModelEditingScope.Complete() { }
		void IModelEditingScope.Update() {
			throw new NotImplementedException();
		}
		void IDisposable.Dispose() { }
	}
	public class RuntimeModelItem : ModelItemBase {
		public RuntimeModelItem(EditingContextBase context, object element, IModelItem parent)
			: base(context, element, parent) {
		}
	}
	public class RuntimeModelItemCollection : ModelItemCollectionBase {
		public RuntimeModelItemCollection(EditingContextBase context, IEnumerable computedValue, IModelItem parent)
			: base(context, computedValue, parent) {
		}
	}
	public class RuntimeModelProperty : ModelPropertyBase {
		public RuntimeModelProperty(EditingContextBase context, object obj, PropertyDescriptor property, IModelItem parent)
			: base(context, obj, property, parent) {
		}
		protected override IModelItem GetValue() {
			return context.CreateModelItem(GetComputedValue(), Parent);
		}
		protected override IModelItem SetValueCore(object value) {
			if(value is BindingBase) {
				DependencyPropertyDescriptor dependencyProperty = DependencyPropertyDescriptor.FromProperty(property);
				DependencyObject dependencyObject = obj as DependencyObject;
				if(dependencyProperty != null && dependencyObject != null) {
					BindingOperations.SetBinding(dependencyObject, dependencyProperty.DependencyProperty, (BindingBase)value);
					return null;
				}
			}
			if(value is IModelItem) value = ((IModelItem)value).GetCurrentValue();
			if(value is MarkupExtension) value = ((MarkupExtension)value).ProvideValue(null);
			property.SetValue(obj, value);
			return null;
		}
		protected override bool IsPropertySet() {
			DependencyPropertyDescriptor dependencyProperty = DependencyPropertyDescriptor.FromProperty(property);
			DependencyObject dependencyObject = obj as DependencyObject;
			if(dependencyProperty != null && dependencyObject != null) {
				return DependencyObjectPropertyHelper.IsPropertyAssigned(dependencyObject, dependencyProperty.DependencyProperty);
			}
			return base.IsPropertySet();
		}
	}
	public class RuntimeModelPropertyCollection : ModelPropertyCollectionBase {
		readonly PropertyDescriptorCollection properties;
		public RuntimeModelPropertyCollection(EditingContextBase context, object obj, IModelItem parent)
			: base(context, obj, parent) {
			properties = obj.Return(x => TypeDescriptor.GetProperties(x.GetType()), () => PropertyDescriptorCollection.Empty);
		}
		protected override IModelProperty FindCore(string propertyName, Type type) {
			PropertyDescriptor property = properties[propertyName];
			return property != null ? context.CreateModelProperty(obj, property, parent) : null;
		}
		public override IEnumerator<IModelProperty> GetEnumerator() {
			List<IModelProperty> result = new List<IModelProperty>();
			foreach(PropertyDescriptor pd in properties) {
			   result.Add(new RuntimeModelProperty(context, obj, pd, parent));
			}
			return result.GetEnumerator();
		}
	}
	public class RuntimeEditingContext : EditingContextBase, IEditingContext {
		class RuntimeModelService : ModelServiceBase {
			public RuntimeModelService(EditingContextBase editingContext)
				: base(editingContext) {
			}
		}
		public RuntimeEditingContext(object root)
			: base(root) {
		}
		public override IModelEditingScope CreateEditingScope(string description) {
			return new RuntimeModelEditingScope(description);
		}
		public override IModelItem CreateModelItem(object obj, IModelItem parent) {
			return new RuntimeModelItem(this, obj, parent);
		}
		public override IModelItemCollection CreateModelItemCollection(IEnumerable computedValue, IModelItem parent) {
			return new RuntimeModelItemCollection(this, computedValue, parent);
		}
		public override IModelItemDictionary CreateModelItemDictionary(IDictionary computedValue) {
			return new ModelItemDictionaryBase(this, computedValue);
		}
		public override IModelProperty CreateModelProperty(object obj, PropertyDescriptor property, IModelItem parent) {
			return new RuntimeModelProperty(this, obj, property, parent);
		}
		public override IModelPropertyCollection CreateModelPropertyCollection(object element, IModelItem parent) {
			return new RuntimeModelPropertyCollection(this, element, parent);
		}
		protected override IModelService CreateModelService() {
			return new RuntimeModelService(this);
		}
		public override IViewItem CreateViewItem(IModelItem modelItem) {
			return null; 
		}
	}
}
