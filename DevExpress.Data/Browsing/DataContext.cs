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
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Data;
using DevExpress.Data.Native;
using DevExpress.Utils;
using DevExpress.Data.Helpers;
using System.Linq;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Data.Browsing {
	public struct DataContextOptions {
		bool showInnerLists;
		bool useCalculatedFields;
		public bool ShowInnerLists {
			get { return showInnerLists; }
		}
		public bool UseCalculatedFields {
			get { return useCalculatedFields; }
		}
		public DataContextOptions(bool showInnerLists, bool useCalculatedFields) {
			this.showInnerLists = showInnerLists;
			this.useCalculatedFields = useCalculatedFields;
		}
		public override bool Equals(object obj) {
			if(!(obj is DataContextOptions))
				return false;
			DataContextOptions options = (DataContextOptions)obj;
			return ShowInnerLists == options.ShowInnerLists && UseCalculatedFields == options.UseCalculatedFields;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public class DataBrowserSR {
		public const string UntypedDataSource = "Untyped data source";
	}
	public class DataContext : DataContextBase {
		static Type[] standardTypes = new Type[] { 
			typeof(bool), 
			typeof(byte), 
			typeof(byte[]), 
			typeof(char), 
			typeof(DateTime), 
			typeof(decimal), 
			typeof(double), 
			typeof(Guid), 
			typeof(short), 
			typeof(int), 
			typeof(long), 
			typeof(object), 
			typeof(sbyte), 
			typeof(float), 
			typeof(string), 
			typeof(TimeSpan), 
			typeof(ushort), 
			typeof(uint), 
			typeof(ulong)
		};
		static string[] standardTypeNames = new string[] { "System.DateTimeOffset" }; 
		static bool IsStandardTypeCore(Type type) {
			foreach (Type standardType in standardTypes)
				if (type.Equals(standardType))
					return true;
			return false;
		}
		static bool IsStandardTypeName(string typeName) {
			foreach (string standardTypeName in standardTypeNames)
				if (typeName.Equals(standardTypeName))
					return true;
			return false;
		}
		public static bool IsStandardType(Type type) {
			if (Nullable.GetUnderlyingType(type) != null)
				return IsStandardType(Nullable.GetUnderlyingType(type));
			return IsStandardTypeCore(type) || IsStandardTypeName(type.FullName) || 
				typeof(Image).IsAssignableFrom(type) || typeof(Icon).IsAssignableFrom(type) || type.Equals(typeof(object)) || type.IsEnum(); 
		}
		public static bool IsComplexType(Type type) {
			return !IsStandardType(type) && !type.IsArray;
		}
		public DataContext(bool suppressListFilling) : base(suppressListFilling) {
		}
		public DataContext() {
		}
		public PropertyDescriptor[] GetItemProperties(object dataSource, string dataMember, bool forceList) {
			Func<PropertyDescriptor, bool> predicate = delegate(PropertyDescriptor property) { return forceList || !BindingHelper.IsList(property); };
			return new DataContextUtils(this).GetDisplayedProperties(dataSource, dataMember, predicate);
		}
		public string GetCustomDataSourceDisplayName(object dataSource) {
			IDisplayNameProvider displayNameProvider = dataSource as IDisplayNameProvider;
			if (displayNameProvider != null) {
				string name = displayNameProvider.GetDataSourceDisplayName();
				return string.IsNullOrEmpty(name) ? DataBrowserSR.UntypedDataSource : name;
			}
			return string.Empty;
		}
		public virtual string GetDataSourceDisplayName(object dataSource, string dataMember) {
			string name = GetCustomDataSourceDisplayName(dataSource);
			if(!string.IsNullOrEmpty(name))
				return name;
			name = GetListName(dataSource as ITypedList, string.Empty);
			if(!string.IsNullOrEmpty(name))
				return name;
			name = GetListName(dataSource as ITypedList, dataMember);
			if(!string.IsNullOrEmpty(name))
				return name;
			if(SuppressListFilling && dataSource is IListSource && dataSource is IQueryable)
				name = GetNameFromTypedList(dataSource as ITypedList);
			else
				name = GetNameFromTypedList(ForceList(dataSource) as ITypedList);
			if(!string.IsNullOrEmpty(name))
				return name;
			if(dataSource is IComponent)
				return BindingHelper.GetDataSourceName((IComponent)dataSource);
			Type type = GetDataSourceType(dataSource);
			return type != null ? GetDisplayDataSourceTypeName(type) : string.Empty;
		}
		Type GetDataSourceType(object dataSource) {
			DataBrowser dataBrowser = GetDataBrowser(dataSource, string.Empty, true);
			return dataBrowser != null ? dataBrowser.DataSourceType : null;
		}
		static string GetDisplayDataSourceTypeName(Type dataSourceType) {
			DisplayNameAttribute displayNameAttribute = TypeDescriptor.GetAttributes(dataSourceType)[typeof(DisplayNameAttribute)] as DisplayNameAttribute;
			string result = displayNameAttribute != null ? displayNameAttribute.DisplayName : DataBrowserSR.UntypedDataSource;
			return string.IsNullOrEmpty(result) ? DataBrowserSR.UntypedDataSource : result;
		}
		public string GetDataMemberDisplayName(object dataSource, string dataMemberPrefix, string dataMember) {
			string dataMemberName;
			bool dataMemberHasName = TryGetDataMemberDisplayName(dataSource, dataMember, out dataMemberName);
			if(string.IsNullOrEmpty(dataMemberName) || string.IsNullOrEmpty(dataMemberPrefix))
				return dataMemberName;
			string prefixName = dataMemberPrefix;
			if(dataMemberHasName)
				TryGetDataMemberDisplayName(dataSource, dataMemberPrefix, out prefixName);
			if(!String.IsNullOrEmpty(prefixName)) {
				prefixName += '.';
				if(dataMemberName.StartsWith(prefixName))
					dataMemberName = dataMemberName.Remove(0, prefixName.Length);
			}
			return dataMemberName;
		}
		public bool TryGetDataMemberDisplayName(object dataSource, string dataMember, out string result) {
			if(dataSource == null) {
				result = String.IsNullOrEmpty(dataMember) ? string.Empty : dataMember;
				return false;
			}
			result = String.Empty;
			DataBrowser startBrowser = GetDataBrowserInternal(dataSource, dataMember, true);
			IRelatedDataBrowser[] browsers = GetDataBrowserAccessors(startBrowser as IRelatedDataBrowser);
			for(int i = browsers.Length - 1; i >= 0; i--) {
				string displayDataMember = GetDisplayDataMemberCore(dataSource as IDisplayNameProvider, browsers[i]);
				if(string.IsNullOrEmpty(displayDataMember)) {
					result = String.Empty;
					break;
				}
				result = String.IsNullOrEmpty(result) ? displayDataMember : displayDataMember + '.' + result;
			}
			if(String.IsNullOrEmpty(result)) {
				result = dataSource is IDisplayNameProvider ? string.Empty : dataMember;
				return false;
			}
			return true;
		}
		public string GetDataMemberDisplayName(object dataSource, string dataMember) {
			string fullDisplayPath;
			TryGetDataMemberDisplayName(dataSource, dataMember, out fullDisplayPath);
			return fullDisplayPath;
		}
		IRelatedDataBrowser[] GetDataBrowserAccessors(IRelatedDataBrowser startBrowser) {
			List<IRelatedDataBrowser> list = new List<IRelatedDataBrowser>();
			IRelatedDataBrowser currentDataBrowser = startBrowser;
			while (currentDataBrowser != null) {
				list.Insert(0, currentDataBrowser);
				currentDataBrowser = currentDataBrowser.Parent;
			}
			return list.ToArray();
		}
		string[] GetFieldAccessors(IRelatedDataBrowser startBrowser) {
			IRelatedDataBrowser[] browsers = GetDataBrowserAccessors(startBrowser);
			string[] names = new string[browsers.Length];
			for (int i = 0; i < browsers.Length; i++)
				names[i] = browsers[i].RelatedProperty.Name;
			return names;
		}
		string GetDisplayDataMemberCore(IDisplayNameProvider dataDictionary, IRelatedDataBrowser dataBrowser) {
			Guard.ArgumentNotNull(dataBrowser, "dataBrowser");
			if (dataDictionary != null && IsIDisplayNameProviderSupported(dataBrowser.RelatedProperty)) {
				string[] fieldAccessors = GetFieldAccessors(dataBrowser);
				if (fieldAccessors == null || fieldAccessors.Length == 0)
					return string.Empty;
				return dataDictionary.GetFieldDisplayName(fieldAccessors);
			}
			return MasterDetailHelper.GetDisplayNameCore(dataBrowser.RelatedProperty);
		}
		protected virtual bool IsIDisplayNameProviderSupported(PropertyDescriptor property) {
			return true;
		}
		protected override bool ShouldExpand(DataBrowser dataBrowser) {
			return dataBrowser != null && (base.ShouldExpand(dataBrowser) || IsComplexType(dataBrowser.DataSourceType));
		}
	}
}
