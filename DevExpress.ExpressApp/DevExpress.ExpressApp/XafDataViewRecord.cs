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
using System.Linq;
using System.ComponentModel;
namespace DevExpress.ExpressApp {
	public abstract class XafDataViewRecord : Object, ICustomTypeDescriptor {
		private XafDataView dataView;
		public XafDataViewRecord(XafDataView dataView) {
			this.dataView = dataView;
		}
		public abstract Object this[Int32 index] {
			get;
		}
		public abstract Object this[String name] {
			get;
		}
		public XafDataView DataView {
			get { return dataView; }
		}
		public Type ObjectType {
			get { return dataView.ObjectType; }
		}
		public Boolean ContainsMember(String name) {
			return (dataView.FindExpression(name) != null);
		}
		public override String ToString() {
			String result = "";
			for(Int32 i = 0; i < dataView.Expressions.Count; i++) {
				if(this[i] != null) {
					result = result + this[i].ToString();
				}
				result = result + ";";
			}
			return result;
		}
		public AttributeCollection GetAttributes() {
			return null;
		}
		public String GetClassName() {
			return "";
		}
		public String GetComponentName() {
			return "";
		}
		public TypeConverter GetConverter() {
			return null;
		}
		public EventDescriptor GetDefaultEvent() {
			return null;
		}
		public PropertyDescriptor GetDefaultProperty() {
			return null;
		}
		public Object GetEditor(Type editorBaseType) {
			return null;
		}
		public EventDescriptorCollection GetEvents(Attribute[] attributes) {
			return null;
		}
		public EventDescriptorCollection GetEvents() {
			return null;
		}
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
			return ((ITypedList)dataView).GetItemProperties(null);
		}
		public PropertyDescriptorCollection GetProperties() {
			return ((ITypedList)dataView).GetItemProperties(null);
		}
		public Object GetPropertyOwner(PropertyDescriptor propertyDescriptor) {
			return null;
		}
	}
}
