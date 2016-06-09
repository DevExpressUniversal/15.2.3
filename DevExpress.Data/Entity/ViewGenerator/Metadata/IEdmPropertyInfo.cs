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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.Entity.Model {
	public interface IEdmPropertyInfo {
		string Name { get; }
		string DisplayName { get; }
		Type PropertyType { get; }
		bool IsForeignKey { get; }
		bool IsReadOnly { get; }
		DataColumnAttributes Attributes { get; }
		object ContextObject { get; } 
		bool IsNavigationProperty { get; }
		IEdmPropertyInfo AddAttributes(IEnumerable<Attribute> newAttributes);
	}
	public class EdmPropertyInfo : IEdmPropertyInfo {
		readonly PropertyDescriptor property;
		readonly bool isForeignKey;
		readonly bool isNavigationProperty;
		readonly DataColumnAttributes attributes;
		public EdmPropertyInfo(PropertyDescriptor property, DataColumnAttributes attributes, bool isNavigationProperty, bool isForeignKey = false) {
			Guard.ArgumentNotNull(property, "property");
			this.property = property;
			this.isForeignKey = isForeignKey;
			this.isNavigationProperty = isNavigationProperty;
			this.attributes = attributes;
		}
		Type IEdmPropertyInfo.PropertyType { get { return property.PropertyType; } }
		string IEdmPropertyInfo.Name { get { return property.Name; } }
		string IEdmPropertyInfo.DisplayName { get { return property.DisplayName; } }
		bool IEdmPropertyInfo.IsReadOnly { get { return property.IsReadOnly; } }
		bool IEdmPropertyInfo.IsForeignKey { get { return isForeignKey; } }
		DataColumnAttributes IEdmPropertyInfo.Attributes { get { return attributes; } }
		object IEdmPropertyInfo.ContextObject { get { return property; } }
		bool IEdmPropertyInfo.IsNavigationProperty { get { return isNavigationProperty; } }
		IEdmPropertyInfo IEdmPropertyInfo.AddAttributes(IEnumerable<Attribute> newAttributes) {
			if(newAttributes == null)
				return this;
			return new EdmPropertyInfo(property, attributes.AddAttributes(newAttributes), isNavigationProperty, isForeignKey);
		}
	}
	public sealed class EmptyEdmPropertyInfo : IEdmPropertyInfo {
		readonly Type componentType;
		readonly DataColumnAttributes attributes;
		public EmptyEdmPropertyInfo(Type componentType) {
			Guard.ArgumentNotNull(componentType, "componentType");
			this.componentType = componentType;
			this.attributes = new DataColumnAttributes(new AttributeCollection(new Attribute[] { new ReadOnlyAttribute(true) }));
		}
		Type IEdmPropertyInfo.PropertyType { get { return componentType; } }
		string IEdmPropertyInfo.Name { get { return null; } }
		string IEdmPropertyInfo.DisplayName { get { return null; } }
		bool IEdmPropertyInfo.IsReadOnly { get { return true; } }
		bool IEdmPropertyInfo.IsForeignKey { get { return false; } }
		DataColumnAttributes IEdmPropertyInfo.Attributes { get { return attributes; } }
		object IEdmPropertyInfo.ContextObject { get { return null; } }
		bool IEdmPropertyInfo.IsNavigationProperty { get { return false; } }
		IEdmPropertyInfo IEdmPropertyInfo.AddAttributes(IEnumerable<Attribute> newAttributes) { return this; }
	}
}
