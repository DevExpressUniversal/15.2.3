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
using System.Diagnostics;
using System.Text;
using DevExpress.Data;
using DevExpress.Snap.Core.API;
namespace DevExpress.Snap.Core.Native.Data.Implementations {
	public class FieldPathService : IFieldPathService {
		internal static readonly string AscendingSortString = "asc";
		internal static readonly string DescendingSortString = "desc";
		public static GroupInterval ParseGroupInterval(string value) {
			try {
				return (GroupInterval)Enum.Parse(typeof(GroupInterval), value, true);
			} catch {
				return GroupInterval.Default;
			}
		}
		public static ColumnSortOrder ParseSortOrder(string value) {
			value = value.ToLowerInvariant();
			if (value == AscendingSortString)
				return ColumnSortOrder.Ascending;
			if (value == DescendingSortString)
				return ColumnSortOrder.Descending;
			return ColumnSortOrder.None;
		}
		protected static readonly string RelativePrefix = "../";
		protected static readonly string AbsolutePrefix = "/";
		public virtual string GetStringPath(FieldPathInfo fieldPathInfo) {
			string dataSourcePath = GetDataSourceStringPath(fieldPathInfo.DataSourceInfo);
			string dataMemberPath = GetDataMemberStringPath(fieldPathInfo.DataMemberInfo);
			if(ShouldSeparateDataSourceAndDataDataMemberPath(fieldPathInfo))
				return String.Format("{0}.{1}", dataSourcePath, dataMemberPath);
			return String.Format("{0}{1}", dataSourcePath, dataMemberPath);
		}
		protected virtual string GetDataSourceStringPath(FieldDataSourceInfo dataSourceInfo) {
			if (dataSourceInfo != null && dataSourceInfo.FieldDataSourceType == FieldDataSourceType.Root) {
				RootFieldDataSourceInfo rootDataSourceInfo = (RootFieldDataSourceInfo)dataSourceInfo;
				return String.Format("{0}{1}", AbsolutePrefix, EncodePath(rootDataSourceInfo.Name));
			}
			string result = String.Empty;
			if (dataSourceInfo != null) {
				Debug.Assert(dataSourceInfo.FieldDataSourceType == FieldDataSourceType.Relative);
				RelativeFieldDataSourceInfo relativeDataSourceInfo = (RelativeFieldDataSourceInfo)dataSourceInfo;
				for (int i = relativeDataSourceInfo.RelativeLevel - 1; i >= 0; i--)
					result += RelativePrefix;
			}
			return result;
		}
		protected internal virtual string GetDataMemberStringPath(FieldPathDataMemberInfo dataMemberInfo) {
			List<FieldDataMemberInfoItem> items = dataMemberInfo.Items;			
			List<string> stringItems = new List<string>(items.Count);
			foreach (FieldDataMemberInfoItem item in dataMemberInfo.Items) {
				stringItems.Add(GetDataMemberItemString(item));
			}
			return String.Join(".", stringItems.ToArray());
		}
		protected virtual bool ShouldSeparateDataSourceAndDataDataMemberPath(FieldPathInfo fieldPathInfo) {
			if (fieldPathInfo.DataMemberInfo.IsEmpty || fieldPathInfo.DataSourceInfo.FieldDataSourceType == FieldDataSourceType.Relative)
				return false;
			return !String.IsNullOrEmpty(fieldPathInfo.DataMemberInfo.Items[0].FieldName);
		}
		protected virtual string GetDataMemberItemString(FieldDataMemberInfoItem item) {
			string groupProperiesString = String.Empty;
			if (item.HasGroups) {
				foreach (GroupProperties groupProperties in item.Groups)
					groupProperiesString += GetGroupPropertiesString(groupProperties);
			}
			string filterPropertiesString = String.Empty;
			if (item.HasFilters) {
				foreach (string filter in item.FilterProperties.Filters) {
					string escapedFilterString = EscapeFilterString(GetFilterString(filter));
					if (!String.IsNullOrEmpty(escapedFilterString))
						filterPropertiesString += String.Format("[{0}]", escapedFilterString);
				}
			}
			string result = EncodePath(item.FieldName);
			if (!String.IsNullOrEmpty(groupProperiesString))
				result += String.Format("[{0}]", groupProperiesString);
			result += filterPropertiesString;
			return result;
		}
		protected virtual string EscapeFilterString(string filter) {
			return !string.IsNullOrEmpty(filter) ? EncodePath(filter).Replace("[", "\\[").Replace("]", "\\]") : null;
		}
		public static string EncodePath(string path) {
			return String.IsNullOrEmpty(path) ? String.Empty : path.Replace(@"\", @"\\").Replace(@".", @"\.");
		}
		public static string DecodePath(string path) {
			StringBuilder result = new StringBuilder(path.Length);
			for(int i = 0; i < path.Length; i++) {
				char c = path[i];
				if(c == '\\' && i != path.Length - 1)
					c = path[++i];
				result.Append(c);
			}
			return result.ToString();
		}
		public static string[] SplitPath(string path) {
			StringBuilder sb = new StringBuilder(path.Length);
			List<string> buffer = new List<string>();
			for(int i = 0; i < path.Length; i++) {
				char ch = path[i];
				if(ch == '\\' && i < path.Length - 1) {
					sb.Append(path[++i]);
					continue;
				}
				if(ch == '.') {
					buffer.Add(sb.ToString());
					sb.Length = 0;
					continue;
				}
				sb.Append(ch);
			}
			buffer.Add(sb.ToString());
			return buffer.ToArray();
		}
		public virtual FieldPathInfo FromString(string fieldPath) {
			fieldPath = fieldPath.Trim();
			if (String.IsNullOrEmpty(fieldPath)) {
				FieldPathInfo result = new FieldPathInfo();
				result.DataSourceInfo = new RelativeFieldDataSourceInfo(0);
				result.DataMemberInfo = new FieldPathDataMemberInfo();
				return result;
			}
			PathParser parser = new PathParser(new StringScanner(fieldPath));
			parser.Parse();
			FieldPathInfo parseResult = parser.GetResult();
			if (parseResult.DataMemberInfo == null)
				parseResult.DataMemberInfo = new FieldPathDataMemberInfo();
			if (parseResult.DataSourceInfo == null)
				parseResult.DataSourceInfo = new RelativeFieldDataSourceInfo(0);
			CalculateGroupIndexes(parseResult);
			return parseResult;
		}
		void CalculateGroupIndexes(FieldPathInfo parseResult) {
			List<FieldDataMemberInfoItem> items = parseResult.DataMemberInfo.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				CalculateCurrentGroupIndex(items[i]);
			}
		}
		void CalculateCurrentGroupIndex(FieldDataMemberInfoItem item) {
			if (!item.HasGroups)
				return;
			int count = item.Groups.Count;
			for (int i = 0; i < count; i++) {
				if (item.Groups[i].HasGroupTemplates)
					item.CurrentGroupIndex++;
			}
		}
		protected virtual string GetGroupPropertiesString(GroupProperties groupProperties) {
			List<GroupFieldInfo> groupFieldInfos = groupProperties.GroupFieldInfos;
			int count = groupFieldInfos.Count;
			string[] groupFieldInfoStrings = new string[count];
			for (int i = 0; i < count; i++)
				groupFieldInfoStrings[i] = GetGroupFieldInfoString(groupFieldInfos[i]);
			return String.Format("({0},{1},{2},{3})", groupProperties.TemplateHeaderSwitch, groupProperties.TemplateFooterSwitch, groupProperties.TemplateSeparatorSwitch, String.Join(",", groupFieldInfoStrings));
		}
		protected virtual string GetGroupFieldInfoString(GroupFieldInfo groupFieldInfo) {
			List<object> args = new List<object>(new[] { groupFieldInfo.FieldName });
			if (groupFieldInfo.SortOrder != ColumnSortOrder.None)
				args.Add(groupFieldInfo.SortOrder == ColumnSortOrder.Ascending ? AscendingSortString : DescendingSortString);
			if (groupFieldInfo.GroupInterval != GroupInterval.Default)
				args.Add(groupFieldInfo.GroupInterval);
			string format = "{0}";
			for (int i = 1; i < args.Count; i++) 
				format += ",{" + i + "}";
			format = args.Count > 1 ? string.Format("({0})", format) : format;
			return string.Format(format, args.ToArray());
		}
		protected virtual string GetFilterString(string filter) {
			if (filter != null && (filter.StartsWith("(") || filter.StartsWith("/")))
				return String.Format("/{0}", filter);
			return filter;
		}
	}
}
