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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Data.Utils;
namespace DevExpress.Utils.Filtering {
	public class ElementBindingInfo {
		readonly PropertyDescriptor descriptorCore;
		public ElementBindingInfo(PropertyDescriptor descriptor) {
			this.descriptorCore = descriptor;
			BoundPropertyName = "EditValue";
			Visible = true;
			if(descriptor != null) {
				DataMember = descriptor.Name;
				ColumnOptions = AnnotationAttributes.GetColumnOptions(descriptor, 0, false);
				AnnotationAttributes = ColumnOptions.Attributes;
				GroupName = AnnotationAttributes.GroupName;
				Visible = ColumnOptions.ColumnIndex != -1;
			}
		}
		public string Caption {
			get { return descriptorCore.Name; }
		}
		public string DataMember {
			get;
			internal set;
		}
		public Type PropertyType {
			get { return descriptorCore.PropertyType; }
		}
		public AnnotationAttributes AnnotationAttributes {
			get;
			set;
		}
		public AnnotationAttributes.ColumnOptions ColumnOptions {
			get;
			set;
		}
		public bool Visible {
			get;
			set;
		}
		public Type EditorType {
			get;
			set;
		}
		public string BoundPropertyName {
			get;
			set;
		}
		public DataSourceUpdateMode DataSourceUpdateMode {
			get;
			set;
		}
		public object DataSourceNullValue {
			get;
			set;
		}
		public ElementsBindingInfo InnerLayoutElementsBindingInfo {
			get;
			internal set;
		}
		public string GroupName {
			get;
			set;
		}
		internal Control CustomControl {
			get;
			set;
		}
	}
	public class ElementsBindingInfo {
		internal List<ElementBindingInfo> bindingsInfo;
		public ElementsBindingInfo(List<ElementBindingInfo> bindingsInfo) {
			this.bindingsInfo = bindingsInfo;
			bindingsInfo.Sort(new AnnotationAttributesOrderComparer(bindingsInfo));
		}
		public List<ElementBindingInfo> GetAllBindings() {
			return bindingsInfo;
		}
		int columnCountCore = 1;
		public int ColumnCount {
			get { return columnCountCore; }
			set {
				if(value > 0)
					columnCountCore = value;
			}
		}
	}
	public class AnnotationAttributesOrderComparer : IComparer<ElementBindingInfo> {
		ElementBindingInfo[] bindingInfo;
		public AnnotationAttributesOrderComparer(List<ElementBindingInfo> listLayoutElementBindingInfo) {
			bindingInfo = listLayoutElementBindingInfo.ToArray();
		}
		public int Compare(ElementBindingInfo x, ElementBindingInfo y) {
			if(x != null && y != null && x.ColumnOptions != null && y.ColumnOptions != null) {
				if(x.AnnotationAttributes.Order.HasValue && y.AnnotationAttributes.Order.HasValue) return x.AnnotationAttributes.Order.GetValueOrDefault(int.MaxValue) - y.AnnotationAttributes.Order.GetValueOrDefault(int.MaxValue);
				if(!x.AnnotationAttributes.Order.HasValue && y.AnnotationAttributes.Order.HasValue) return 1;
				if(x.AnnotationAttributes.Order.HasValue && !y.AnnotationAttributes.Order.HasValue) return -1;
			}
			return Array.IndexOf(bindingInfo, x) - Array.IndexOf(bindingInfo, y);
		}
	}
	public class ElementBindingInfoHelper {
		public ElementsBindingInfo CreateDataLayoutElementsBindingInfo(Type type) {
			ICollection dataColumnInfoCollection = GetDataColumnsCollection(type);
			return new ElementsBindingInfo(CreateListElementBindingInfoFromFieldList(dataColumnInfoCollection));
		}
		List<ElementBindingInfo> CreateListElementBindingInfoFromFieldList(ICollection propertyDescriptorCollection,
			string dataMember = null, Hashtable dataTypeHashtable = null, PropertyDescriptor parentInfo = null) {
			List<ElementBindingInfo> result = new List<ElementBindingInfo>();
			foreach(PropertyDescriptor pd in propertyDescriptorCollection) {
				dataTypeHashtable = InitializeHashtableIfNeed(dataTypeHashtable, pd);
				if(CanCreateBindingInfoForProperty(pd)) {
					Type dataType = pd.PropertyType;
					ElementBindingInfo bindingInfo = new ElementBindingInfo(pd);
					bindingInfo.DataMember = GetDataMemberName(dataMember, pd);
					result.Add(bindingInfo);
					if(IsSimpleType(dataTypeHashtable, dataType, parentInfo == null ? null : parentInfo.PropertyType)) continue;
					if(parentInfo != null && !parentInfo.PropertyType.GetProperties().Any(e => e.Name == pd.Name)) continue;
					else {
						Hashtable hashTable = new Hashtable();
						if(dataTypeHashtable != null) hashTable = dataTypeHashtable.Clone() as Hashtable;
						hashTable.Add(dataType, null);
						ICollection childDataColumnInfoCollection = GetDataColumnsCollection(dataType);
						if(CheckDataType(dataType, childDataColumnInfoCollection))
							bindingInfo.InnerLayoutElementsBindingInfo = new ElementsBindingInfo(CreateListElementBindingInfoFromFieldList(childDataColumnInfoCollection, GetDataMemberName(dataMember, pd), hashTable, pd));
					}
				}
			}
			return result;
		}
		protected virtual ICollection GetDataColumnsCollection(Type type) {
			return TypeDescriptor.GetProperties(type);
		}
		bool CheckDataType(Type dataType, ICollection columns) {
			if(columns == null || columns.Count <= 0) return false;
			PropertyInfo[] propertyInfo = dataType.GetProperties();
			foreach(PropertyDescriptor item in columns) {
				if(!propertyInfo.Any(e => e.Name == item.Name)) return false;
			}
			return true;
		}
		protected virtual Hashtable InitializeHashtableIfNeed(Hashtable table, PropertyDescriptor info) {
			if(table == null && info != null && info.ComponentType != null) {
				table = new Hashtable();
				table.Add(info.ComponentType, null);
			}
			return table;
		}
		protected virtual bool IsSimpleType(Hashtable dataTypeHashtable, Type checkDataType, Type parentDataType) {
			if(checkDataType == null) return true;
			if(checkDataType == parentDataType) return true;
			if(!checkDataType.IsClass) return true;
			if(checkDataType.IsEnum) return true;
			if(checkDataType.IsPrimitive) return true;
			if(checkDataType == typeof(string)) return true;
			if(checkDataType == typeof(DateTime)) return true;
			if(checkDataType == typeof(Byte[])) return true;
			if(checkDataType == typeof(Image)) return true;
			if(checkDataType.IsSubclassOf(typeof(Image))) return true;
			bool isWritable = checkDataType.GetProperties().Any(p => p.CanWrite && IsSimpleType(dataTypeHashtable, p.PropertyType, checkDataType));
			if(!isWritable) return true;
			if(dataTypeHashtable != null && dataTypeHashtable.ContainsKey(checkDataType)) return true;
			return false;
		}
		static string GetDataMemberName(string dataMember, PropertyDescriptor info) {
			return string.IsNullOrEmpty(dataMember) ? info.Name : dataMember + "." + info.Name;
		}
		protected virtual bool CanCreateBindingInfoForProperty(PropertyDescriptor propertyDescriptor) {
			if(propertyDescriptor == null || !propertyDescriptor.IsBrowsable)
				return false;
			var annotationAttributes = new AnnotationAttributes(propertyDescriptor);
			if(!AnnotationAttributes.GetAutoGenerateColumnOrFilter(annotationAttributes))
				return false;
			var filterAttributes = new Internal.FilterAttributes(annotationAttributes, propertyDescriptor.PropertyType);
			return filterAttributes.HasFilterPropertyAttribute && filterAttributes.IsFilterProperty;
		}
	}
	public class AnnotationAttributesGroupName {
		const char StartTabbedGroupChar = '{';
		const char EndTabbedGroupChar = '}';
		const char StartGroupBorderVisibleFalse = '<';
		const char EndGroupBorderVisibleFalse = '>';
		const char StartGroupBorderVisibleTrue = '[';
		const char EndGroupBorderVisibleTrue = ']';
		const char HorizontalGroupMark = '-';
		const char VerticalGroupMark = '|';
		const char GroupPathSeparator = '/';
		public const string BeginNameForGroup = "autoGroupFor";
		public AnnotationAttributesGroupName(string GroupName) {
			int counterOfTabbedGroupChars = 0;
			int counterOfGroupBorderVisibleFalse = 0;
			int counterOfGroupBorderVisibleTrue = 0;
			for(int i = 0; i < GroupName.Length; i++) {
				switch(GroupName[i]) {
					case AnnotationAttributesGroupName.GroupPathSeparator:
						childGroupCore = new AnnotationAttributesGroupName(GroupName.Remove(0, i + 1));
						return;
					case AnnotationAttributesGroupName.StartGroupBorderVisibleFalse:
						groupBordersVisible = false;
						counterOfGroupBorderVisibleFalse++;
						if(counterOfGroupBorderVisibleFalse > 1) groupNameCore += GroupName[i];
						break;
					case AnnotationAttributesGroupName.EndGroupBorderVisibleFalse:
						counterOfGroupBorderVisibleFalse--;
						if(counterOfGroupBorderVisibleFalse > 1) groupNameCore += GroupName[i];
						break;
					case AnnotationAttributesGroupName.StartGroupBorderVisibleTrue:
						groupBordersVisible = true;
						counterOfGroupBorderVisibleTrue++;
						if(counterOfGroupBorderVisibleTrue > 1) groupNameCore += GroupName[i];
						break;
					case AnnotationAttributesGroupName.EndGroupBorderVisibleTrue:
						counterOfGroupBorderVisibleTrue--;
						if(counterOfGroupBorderVisibleTrue > 1) groupNameCore += GroupName[i];
						break;
					case AnnotationAttributesGroupName.StartTabbedGroupChar:
						isTab = true;
						counterOfTabbedGroupChars++;
						if(counterOfTabbedGroupChars > 1) groupNameCore += GroupName[i];
						break;
					case AnnotationAttributesGroupName.EndTabbedGroupChar:
						counterOfTabbedGroupChars--;
						if(counterOfTabbedGroupChars > 1) groupNameCore += GroupName[i];
						break;
					case AnnotationAttributesGroupName.HorizontalGroupMark:
						break;
					case AnnotationAttributesGroupName.VerticalGroupMark:
						break;
					default:
						groupNameCore += GroupName[i];
						break;
				}
			}
		}
		string groupNameCore = string.Empty;
		public string GroupName { get { return groupNameCore; } }
		AnnotationAttributesGroupName childGroupCore;
		public AnnotationAttributesGroupName ChildGroup { get { return childGroupCore; } }
		bool isTab = false;
		public bool IsTab { get { return isTab; } }
		bool groupBordersVisible = true;
		public bool GroupBordersVisible { get { return groupBordersVisible; } }
	}
}
