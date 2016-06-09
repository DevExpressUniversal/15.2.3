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
using System.ComponentModel.Design;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Data;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPivotGrid.Design {
	class OLAPDataSourceObject : OLAPDataSourceNode, IDisplayNameProvider {
		public OLAPDataSourceObject(string cubeName, string[] fieldList) 
			: base(cubeName) {
			CreateChilds(fieldList);
		}
		protected void CreateChilds(string[] fieldList) {
			OLAPDataSourceNode[] lastNodes = new OLAPDataSourceNode[3];
			for(int i = 0; i < fieldList.Length; i++) {
				string[] parts = fieldList[i].Split('.');
				int partsCount = Math.Min(parts.Length, 3);
				for(int j = 0; j < partsCount; j++) {
					if(lastNodes[j] == null || parts[j] != lastNodes[j].Name) {
						OLAPDataSourceNode node = new OLAPDataSourceNode(parts[j]);
						if(j == 0)
							ChildNodes.Add(node);
						else
							lastNodes[j - 1].ChildNodes.Add(node);
						lastNodes[j] = node;
						for(int k = j + 1; k < 3; k++)
							lastNodes[k] = null;
					}
				}				
			}
		}
		#region IDisplayNameProvider Members
		string IDisplayNameProvider.GetDataSourceDisplayName() {
			return Name;
		}
		string IDisplayNameProvider.GetFieldDisplayName(string[] fieldAccessors) {
			return fieldAccessors[fieldAccessors.Length - 1];
		}
		#endregion
	}
	class OLAPDataSourceNode : ICustomTypeDescriptor {
		readonly string name;
		readonly List<OLAPDataSourceNode> childNodes;
		OLAPDataSourceNodeProperty propertyDescriptor;
		public OLAPDataSourceNode(string name) {
			this.name = name;
			this.childNodes = new List<OLAPDataSourceNode>();
		}
		internal string Name { get { return this.name; } }
		internal List<OLAPDataSourceNode> ChildNodes { get { return childNodes; } }
		internal PropertyDescriptor PropertyDescriptor {
			get {
				if(this.propertyDescriptor == null)
					this.propertyDescriptor = new OLAPDataSourceNodeProperty(this);
				return this.propertyDescriptor;
			}
		}
		#region ICustomTypeDescriptor Members
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return new AttributeCollection();
		}
		string ICustomTypeDescriptor.GetClassName() {
			return Name;
		}
		string ICustomTypeDescriptor.GetComponentName() {
			return Name;
		}
		TypeConverter ICustomTypeDescriptor.GetConverter() {
			return null;
		}
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return null;
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return null;
		}
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return null;
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return new EventDescriptorCollection(null);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return new EventDescriptorCollection(null);
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return GetProperties();
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return GetProperties();
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			OLAPDataSourceNodeProperty prop = pd as OLAPDataSourceNodeProperty;
			return prop == null || ChildNodes.IndexOf(prop.Node) < 0 ? null : this;
		}
		#endregion
		protected PropertyDescriptorCollection GetProperties() {
			PropertyDescriptor[] props = new PropertyDescriptor[ChildNodes.Count];
			for(int i = 0; i < ChildNodes.Count; i++) 
				props[i] = ChildNodes[i].PropertyDescriptor;
			return new PropertyDescriptorCollection(props);
		}
	}
	class OLAPDataSourceNodeProperty : PropertyDescriptor {
		readonly OLAPDataSourceNode node;
		public OLAPDataSourceNodeProperty(OLAPDataSourceNode node) : base(node.Name, null) {
			this.node = node;
		}
		internal OLAPDataSourceNode Node { get { return this.node; } }
		public override bool CanResetValue(object component) { return false; }
		public override Type ComponentType { get { return typeof(OLAPDataSourceNode); } }
		public override object GetValue(object component) { return Node; }
		public override bool IsReadOnly { get { return true; } }
		public override Type PropertyType {	get { return typeof(OLAPDataSourceNode); } }
		public override void ResetValue(object component) { }
		public override void SetValue(object component, object value) { }
		public override bool ShouldSerializeValue(object component) { return false; }
	}
}
