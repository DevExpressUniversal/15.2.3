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
using System.Text;
using System.ComponentModel;
using System.Collections;
using DevExpress.Data.Browsing;
namespace DevExpress.XtraReports.Native.Data {
	public class RelatedPropertyDescriptor : PropertyDescriptor {
		#region static
		public static RelatedPropertyDescriptor CreateInstance(DataBrowser parentBrowser, string dataMember) {
			PropertyDescriptor[] path = parentBrowser.GetPropertyPath(dataMember);
			return path != null && path.Length > 0 ? new RelatedPropertyDescriptor(path, dataMember) : null;
		}
		static object GetFirstItem(IList list, object defaultValue) {
			return list != null && list.Count > 0 ? list[0] : defaultValue;
		}
		static object GetFirstItem(IEnumerable enumerable, object defaultValue) {
			if(enumerable != null) {
				IEnumerator enumerator = enumerable.GetEnumerator();
				if(enumerator.MoveNext()) return enumerator.Current;
			}
			return defaultValue;
		}
		#endregion
		PropertyDescriptor[] propertyPath;
		PropertyDescriptor firstProperty;
		PropertyDescriptor lastProperty;
		string name;
		public override Type ComponentType { get { return firstProperty.ComponentType; } }
		public override bool IsReadOnly { get { return lastProperty.IsReadOnly; } }
		public override Type PropertyType { get { return lastProperty.PropertyType; } }
		public override TypeConverter Converter { get { return lastProperty.Converter; } }
		public override string Name { get { return name; } }
		public RelatedPropertyDescriptor(PropertyDescriptor[] propertyPath, string name)
			: base(propertyPath[propertyPath.Length - 1]) {
			if(propertyPath == null || propertyPath.Length == 0)
				throw new ArgumentException("propertyPath");
			this.propertyPath = propertyPath;
			firstProperty = propertyPath[0];
			lastProperty = propertyPath[propertyPath.Length - 1];
			this.name = name;
		}
		public override bool Equals(object other) {
			if(other is RelatedPropertyDescriptor) {
				RelatedPropertyDescriptor otherDescriptor = (RelatedPropertyDescriptor)other;
				if(propertyPath.Length == otherDescriptor.propertyPath.Length) {
					for(int i = 0; i < propertyPath.Length; i++)
						if(!Object.Equals(propertyPath[i], otherDescriptor.propertyPath[i]))
							return false;
					return true;
				}
			}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override object GetValue(object component) {
			object actualComponent = GetActualComponent(component);
			return actualComponent != null ? lastProperty.GetValue(actualComponent) : null;
		}
		object GetActualComponent(object component) {
			for(int i = 0; i < propertyPath.Length - 1; i++) {
				component = propertyPath[i].GetValue(component);
				if(component is IListSource)
					component = GetFirstItem(((IListSource)component).GetList(), null);
				else if(component is IList)
					component = GetFirstItem((IList)component, null);
				else if(component is IEnumerable)
					component = GetFirstItem((IEnumerable)component, null);
				if(component == null)
					return null;
			}
			return component;
		}
		public override bool CanResetValue(object component) {
			return false;
		}
		public override void ResetValue(object component) {
		}
		public override void SetValue(object component, object value) {
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
	}
}
