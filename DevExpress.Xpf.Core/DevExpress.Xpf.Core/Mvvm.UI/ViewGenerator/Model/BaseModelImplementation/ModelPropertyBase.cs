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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Collections.Specialized;
using System.Reflection;
using System.Collections;
using DevExpress.Utils;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Mvvm.UI.Native.ViewGenerator.Model {
	public abstract class ModelPropertyBase : IModelProperty {
		protected readonly object obj;
		protected readonly PropertyDescriptor property;
		protected readonly EditingContextBase context;
		readonly IModelItem parent;
		public ModelPropertyBase(EditingContextBase context, object obj, PropertyDescriptor property, IModelItem parent) {
			Guard.ArgumentNotNull(context, "context");
			Guard.ArgumentNotNull(property, "property");
			Guard.ArgumentNotNull(parent, "parent");
			this.obj = obj;
			this.property = property;
			this.context = context;
			this.parent = parent;
		}
		public IModelItem Parent {
			get { return parent; }
		}
		string IModelProperty.Name {
			get { return property.Name; }
		}
		bool IModelProperty.IsSet {
			get { return IsPropertySet(); }
		}
		bool IModelProperty.IsReadOnly {
			get { return property.IsReadOnly; }
		}
		void IModelProperty.ClearValue() {
			property.ResetValue(obj);
		}
		IModelItem IModelProperty.SetValue(object value) {
			return SetValueCore(value);
		}
		object IModelProperty.ComputedValue {
			get { return GetComputedValue(); }
		}
		protected virtual object GetComputedValue() {
			object parentObj = ((ModelItemBase)parent).element;
			return property.GetValue(parentObj);
		}
		IModelItemCollection IModelProperty.Collection {
			get {
				IEnumerable computedValue = GetComputedValue() as IEnumerable;
				return computedValue == null ? null : context.CreateModelItemCollection(computedValue, parent);
			}
		}
		IModelItem IModelProperty.Value {
			get {
				return GetValue();
			}
		}
		IModelItemDictionary IModelProperty.Dictionary {
			get {
				IDictionary computedValue = GetComputedValue() as IDictionary;
				return computedValue == null ? null : context.CreateModelItemDictionary(computedValue);
			}
		}
		Type IModelProperty.PropertyType {
			get {
				return property.PropertyType;
			}
		}
		protected virtual bool IsPropertySet() {
			return ((IModelProperty)this).ComputedValue != null;
		}
		protected abstract IModelItem GetValue();
		protected abstract IModelItem SetValueCore(object value);
	}
}
