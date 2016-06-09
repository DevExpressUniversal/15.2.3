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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.Utils.Serializing;
namespace DevExpress.DataAccess.Native.Data {
	static class FilterInfoPropertyNames {
		public const string XmlFilter = "Filter";
		public const string XmlTableName = "TableName";
		public const string XmlFilterString = "FilterString";
	}
	public class FilterInfo {
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FilterInfoCollection Owner { get; set; }
		[XtraSerializableProperty]
		[Editor("DevExpress.DataAccess.UI.Native.Data.FilterInfoTableNameEditor," + AssemblyInfo.SRAssemblyDataAccessUI, typeof(UITypeEditor))]
		public string TableName {
			get { return tableName; }
			set {
				if(string.CompareOrdinal(tableName, value) == 0)
					return;
				if(!ValidateName(value))
					throw new ArgumentException();
				tableName = value;
			}
		}
		bool ValidateName(string name) {
			if(Owner != null)
				foreach(var filter in Owner)
					if(string.CompareOrdinal(name, filter.TableName) == 0)
						return false;
			return true;
		}
		[XtraSerializableProperty]
		[Editor("DevExpress.DataAccess.UI.Native.Sql.FilterStringEditor," + AssemblyInfo.SRAssemblyDataAccessUI, typeof(UITypeEditor))]
		public string FilterString {
			get;
			set;
		}
		string tableName;
		public FilterInfo() {
		}
		public FilterInfo(string tableName, string filterString) {
			TableName = tableName;
			FilterString = filterString;
		}
		public override bool Equals(object obj) {
			FilterInfo another = obj as FilterInfo;
			return !ReferenceEquals(another, null)
				&& TableName == another.TableName
				&& FilterString == another.FilterString;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public XElement SaveToXml() {
			XElement element = new XElement(FilterInfoPropertyNames.XmlFilter);
			element.Add(new XAttribute(FilterInfoPropertyNames.XmlTableName, this.TableName ?? string.Empty));
			element.Add(new XAttribute(FilterInfoPropertyNames.XmlFilterString, this.FilterString ?? string.Empty));
			return element;
		}
		public FilterInfo Clone() {
			return new FilterInfo(TableName, FilterString);
		}
	}
	[Editor("DevExpress.DataAccess.UI.Native.Data.FilterInfosCollectionEditor, " + AssemblyInfo.SRAssemblyDataAccessUI, typeof(UITypeEditor))]
	public class FilterInfoCollection : ObservableCollection<FilterInfo> {
		public FilterInfoCollection(IParametersOwner parametersOwner) {
			ParametersOwner = parametersOwner;
		}
		public FilterInfo this[string tableName] {
			get {
				foreach(FilterInfo filter in this) {
					if(filter.TableName == tableName)
						return filter;
				}
				return null;
			}
		}
		public IParametersOwner ParametersOwner { get; set; }
		public void AddRange(FilterInfo[] filterInfos) {
			foreach(FilterInfo item in filterInfos) {
				Add(item);
			}
		}
		public void Add(string tableName, string filterString) {
			FilterInfo filter = new FilterInfo();
			filter.TableName = tableName;
			filter.FilterString = filterString;
			filter.Owner = this;
			base.Add(filter);
		}
		public bool Contains(string tableName) {
			foreach(FilterInfo item in this) {
				if(item.TableName == tableName)
					return true;
			}
			return false;
		}
		protected override void InsertItem(int index, FilterInfo item) {
			item.Owner = this;
			base.InsertItem(index, item);
		}
		protected override void RemoveItem(int index) {
			this[index].Owner = null;
			base.RemoveItem(index);
		}
	}
}
