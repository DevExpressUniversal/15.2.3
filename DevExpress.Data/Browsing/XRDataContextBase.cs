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
using System.Data;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Compatibility.System.Data;
namespace DevExpress.XtraReports.Native.Data {
	public class XRDataContextBase : DataContext {
		bool isCalculatedFieldsApplied;
		bool isCalculatedFieldsApplying;
		protected IEnumerable<IParameter> parameters;
		protected IEnumerable<ICalculatedField> calculatedFields;
		DataBrowser this[object dataSource, string dataMember, bool suppressException] {
			get { return GetDataBrowserInternal(dataSource, dataMember, suppressException); }
		}
		public XRDataContextBase() {
		}
		public XRDataContextBase(IEnumerable<ICalculatedField> calculatedFields)
			: this(calculatedFields, false) {
		}
		public XRDataContextBase(IEnumerable<ICalculatedField> calculatedFields, bool suppressListFilling)
			: this(calculatedFields, null, suppressListFilling) {
		}
		public XRDataContextBase(IEnumerable<ICalculatedField> calculatedFields, IEnumerable<IParameter> parameters, bool suppressListFilling)
			: base(suppressListFilling) {
			this.parameters = parameters;
			ApplyCalculatedFields(calculatedFields);
		}
		public void ApplyCalculatedFields(IEnumerable<ICalculatedField> calculatedFields) {
			if(isCalculatedFieldsApplying || isCalculatedFieldsApplied || calculatedFields == null)
				return;
			try {
				isCalculatedFieldsApplying = true;
				this.calculatedFields = calculatedFields;
				Dictionary<DataBrowser, List<PropertyDescriptor>> customProperties = new Dictionary<DataBrowser, List<PropertyDescriptor>>();
				foreach(ICalculatedField calculatedField in calculatedFields) {
					if(calculatedField.DataSource == null)
						continue;
					DataBrowser key = GetDataBrowserInternal(calculatedField.DataSource, calculatedField.DataMember, true);
					if(!(key is IPropertiesContainer))
						continue;
					List<PropertyDescriptor> value;
					if(!customProperties.TryGetValue(key, out value)) {
						value = new List<PropertyDescriptor>();
						customProperties.Add(key, value);
					}
					value.Add(CreateCalculatedPropertyDescriptor(calculatedField));
				}
				foreach(KeyValuePair<DataBrowser, List<PropertyDescriptor>> pair in customProperties) {
					((IPropertiesContainer)pair.Key).SetCustomProperties(pair.Value.ToArray());
				}
				isCalculatedFieldsApplied = true;
			} finally {
				isCalculatedFieldsApplying = false;
			}
		}
		public override void Clear() {
			base.Clear();
			isCalculatedFieldsApplied = false;
		}
		protected override DataBrowser GetDataBrowserInternal(object dataSource, string dataMember, bool suppressException) {
			ApplyCalculatedFields(calculatedFields);
			return base.GetDataBrowserInternal(dataSource, dataMember, suppressException);
		}
		protected override RelatedListBrowser CreateRelatedListBrowser(DataBrowser parent, PropertyDescriptor prop, ListControllerBase listController) {
			return new CustomRelatedListBrowser(parent, prop, listController, SuppressListFilling);
		}
		protected override ListBrowser CreateListBrowser(object dataSource, ListControllerBase listController) {
			return new CustomListBrowser(dataSource, listController, SuppressListFilling);
		}
		protected virtual CalculatedPropertyDescriptorBase CreateCalculatedPropertyDescriptor(ICalculatedField calculatedField) {
			return new CalculatedPropertyDescriptorBase(calculatedField, this);
		}
		public bool IsDataMemberValid(object dataSource, string dataMember) {
			return GetDataBrowserInternal(dataSource, dataMember, true) != null;
		}
		public string GetDataMemberFromDisplayName(object dataSource, string dataMember, string displayName) {
			string[] members = String.IsNullOrEmpty(displayName) ? new string[] { displayName } : displayName.Split('.');
			DataTable dataTable = dataSource as DataTable;
			dataSource = dataTable != null && dataTable.DataSet != null ? dataTable.DataSet : dataSource;
			foreach(string member in members) {
				ObjectNameCollection objectNames = GetItemDisplayNames(dataSource, dataMember, true);
				int index = objectNames.IndexOf(member);
				if(index < 0)
					return null;
				dataMember = ConcatStrings(dataMember, objectNames[index].Name);
			}
			return dataMember;
		}
		static string ConcatStrings(string s1, string s2) {
			return s1.Length > 0 ? s1 + "." + s2 : s2;
		}
		public ObjectNameCollection GetItemDisplayNames(object dataSource, string dataMember) {
			return GetItemDisplayNames(dataSource, dataMember, false);
		}
		public ObjectNameCollection GetItemDisplayNames(object dataSource, string dataMember, bool forceList) {
			ObjectNameCollection items = new ObjectNameCollection();
			PropertyDescriptor[] properties = GetItemProperties(dataSource, dataMember, forceList);
			if(properties != null) {
				foreach(PropertyDescriptor property in properties)
					items.Add(new ObjectName(property.Name, property.DisplayName, dataMember));
			}
			return items;
		}
		string GetActualDataMember(object dataSource, string parentDataMember, string dataMember) {
			if (String.IsNullOrEmpty(dataMember))
				return parentDataMember;
			ObjectNameCollection names = GetItemDisplayNames(dataSource, parentDataMember, true);
			string[] members = dataMember.Split('.');
			string currentDataMember = members[0];
			int i = 1;
			for (;;) {
				foreach (ObjectName name in names)
					if (name.FullName == currentDataMember || name.DisplayName == currentDataMember)
						return GetActualDataMember(dataSource, name.FullName, String.Join(".", members, i, members.Length - 1));
				if (i == members.Length)
					return null;
				currentDataMember += "." + members[i++];
			}
		}
		public string GetActualDataMember(object dataSource, string dataMember) {
			return GetActualDataMember(dataSource, String.Empty, dataMember);
		}
		protected override bool IsIDisplayNameProviderSupported(PropertyDescriptor property) {
			return !(property is CalculatedPropertyDescriptorBase);
		}
	}
	public interface IPropertiesContainer {
		void SetCustomProperties(PropertyDescriptor[] customProperties);
	}
	public class CustomPropertiesContainer {
		PropertyDescriptor[] customProperties;
		public CustomPropertiesContainer(PropertyDescriptor[] customProperties) {
			this.customProperties = customProperties;
		}
		public PropertyDescriptorCollection MergeProperties(PropertyDescriptorCollection properties) {
			if(customProperties != null && customProperties.Length > 0)
				return new CustomPropertyDescriptorCollection(properties, customProperties);
			return properties;
		}
	}
	class CustomPropertyDescriptorCollection : PropertyDescriptorCollection {
		PropertyDescriptorCollection source;
		public CustomPropertyDescriptorCollection(PropertyDescriptorCollection source, PropertyDescriptor[] properties) : base(properties) {
			this.source = source;
			foreach(PropertyDescriptor property in source)
				this.Add(property);
		}
		public override PropertyDescriptor Find(string name, bool ignoreCase) {
			PropertyDescriptor result = base.Find(name, ignoreCase);
			return result != null ? result : source.Find(name, ignoreCase);
		}
	}
	public class CustomListBrowser : ListBrowser, IPropertiesContainer {
		CustomPropertiesContainer customPropertiesContainer;
		public CustomListBrowser(object dataSource, ListControllerBase listController, bool suppressListFilling)
			: base(dataSource, listController, suppressListFilling) {
		}
		public override PropertyDescriptorCollection GetItemProperties() {
			PropertyDescriptorCollection properties = base.GetItemProperties();
			return customPropertiesContainer != null ? customPropertiesContainer.MergeProperties(properties) : properties;
		}
		protected override bool IsStandartType(Type propType) {
			return DataContext.IsStandardType(propType);
		}
		void IPropertiesContainer.SetCustomProperties(PropertyDescriptor[] customProperties) {
			customPropertiesContainer = new CustomPropertiesContainer(customProperties);
		}
	}
	public class CustomRelatedListBrowser : RelatedListBrowser, IPropertiesContainer {
		CustomPropertiesContainer customPropertiesContainer;
		public CustomRelatedListBrowser(DataBrowser parent, PropertyDescriptor listAccessor, ListControllerBase listController, bool suppressListFilling)
			: base(parent, listAccessor, listController, suppressListFilling) {
		}
		public override PropertyDescriptorCollection GetItemProperties() {
			PropertyDescriptorCollection properties = base.GetItemProperties();
			return customPropertiesContainer != null ? customPropertiesContainer.MergeProperties(properties) : properties;
		}
		void IPropertiesContainer.SetCustomProperties(PropertyDescriptor[] customProperties) {
			customPropertiesContainer = new CustomPropertiesContainer(customProperties);
		}
	}
}
