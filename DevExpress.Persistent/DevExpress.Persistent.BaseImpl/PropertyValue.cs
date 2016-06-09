#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
namespace DevExpress.Persistent.BaseImpl {
	[Browsable(false)]
	public class PropertyValue : BaseObject, IPropertyValue {
		private PropertyValueImpl propertyValue;
		private PropertyDescriptor descriptor;
		private PropertyBag bag;
		public PropertyValue(Session session)
			: base(session) {
			propertyValue = new PropertyValueImpl(this);
		}
		public PropertyValue(PropertyDescriptor descriptor, PropertyBag bag)
			: this(bag.Session) {
			this.bag = bag;
			bag.PropertyValues.Add(this);
			this.descriptor = descriptor;
		}
		public PropertyDescriptor Descriptor {
			get { return descriptor; }
			set { SetPropertyValue("Descriptor", ref descriptor, value); }
		}
		IPropertyDescriptor IPropertyValue.Descriptor {
			get { return Descriptor as IPropertyDescriptor; }
		}
		[Association("PropertyBag-PropertyValue")]
		public PropertyBag Bag {
			get { return bag; }
			set { SetPropertyValue("Bag", ref bag, value); }
		}
		[NonPersistent]
		public object Value {
			get { return propertyValue.Value; }
			set {
				object oldValue = propertyValue.Value;
				propertyValue.Value = value;
				OnChanged("Value", oldValue, propertyValue.Value);
			}
		}
		public Type ValueType {
			get { return propertyValue.ValueType; }
		}
		public string StrValue {
			get { return propertyValue.StrValue; }
			set {
				string oldValue = propertyValue.StrValue;
				propertyValue.StrValue = value;
				OnChanged("StrValue", oldValue, propertyValue.StrValue);
			}
		}
		public XPWeakReference WeakReference {
			get { return propertyValue.WeakReference; }
			set {
				XPWeakReference oldValue = propertyValue.WeakReference;
				propertyValue.WeakReference = value;
				OnChanged("WeakReference", oldValue, propertyValue.WeakReference);
			}
		}
	}
}
