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
using DevExpress.Office.Utils;
using DevExpress.Utils;
using System.Text;
namespace DevExpress.Snap.Core.Native.Data.Implementations {   
	public class ListParameters {
		static readonly ListParameters empty = new ListParameters(null, null);
		public static ListParameters Empty { get { return empty; } }
		readonly List<GroupProperties> groups;
		readonly FilterProperties filters;
		GroupFieldInfo[] groupFieldInfos;
		public ListParameters(List<GroupProperties> groups, FilterProperties filters) {
			this.groups = groups;
			this.filters = filters;
		}
		public List<GroupProperties> Groups { get { return groups; } }
		public FilterProperties Filters { get { return filters; } }
		public GroupFieldInfo[] GetGroupFieldInfos() {
			if(groupFieldInfos == null)
				groupFieldInfos = GetGroupFieldInfosCore();
			if (groupFieldInfos.Length == 0)
				return null;
			else
				return groupFieldInfos;
		}
		GroupFieldInfo[] GetGroupFieldInfosCore() {
			List<GroupFieldInfo> result = new List<GroupFieldInfo>();
			if (groups == null)
				return result.ToArray();
			foreach (GroupProperties groupProperties in groups) {
				if (groupProperties.GroupFieldInfos.Count > 0)
					result.AddRange(groupProperties.GroupFieldInfos);
			}
			return result.ToArray();
		}
		public override bool Equals(object obj) {
			if (Object.ReferenceEquals(this, obj))
				return true;
			ListParameters other = obj as ListParameters;
			if (Object.ReferenceEquals(other, null))
				return false;
			if (!IsFiltersEquals(filters, other.Filters))
				return false;
			return ListUtils.AreEquals(groups, other.Groups);
		}
		bool IsFiltersEquals(FilterProperties firstFilters, FilterProperties secondFilters) {
			bool firstFiltersEmpty = firstFilters == null || firstFilters.Filters == null || firstFilters.Filters.Count == 0;
			bool secondFiltersEmpty = secondFilters == null || secondFilters.Filters == null || secondFilters.Filters.Count == 0;
			if (firstFiltersEmpty != secondFiltersEmpty)
				return false;
			if (firstFiltersEmpty)
				return true;
			return Object.Equals(firstFilters, secondFilters);
		}
		public override int GetHashCode() {
			return ListUtils.CalcHashCode(groups) ^ (filters != null ? filters.GetHashCode() : 0);
		}
	}
	public interface IDataControllerFieldContext : IFieldContext {		
		RootFieldContext Root { get; }
	}
	public interface ISingleObjectFieldContext : IDataControllerFieldContext {
		IDataControllerListFieldContext ListContext { get; }
		IDataControllerFieldContext Parent { get; }
		int VisibleIndex { get; }
		int RowHandle { get; }
		int CurrentRecordIndex { get; }
		int CurrentRecordIndexInGroup { get; }
	}	
	public interface IDataControllerListFieldContext : IDataControllerFieldContext {
		ISingleObjectFieldContext Parent { get; }
		ListParameters ListParameters { get; }
	}
	public class PathHelper {
		public static string Join(string path1, string path2) {
			if (String.IsNullOrEmpty(path1))
				return path2;
			if (String.IsNullOrEmpty(path2))
				return path1;
			return path1 + "." + path2;
		}
		public static void Join(StringBuilder path, string path2) {
			if (String.IsNullOrEmpty(path2))
				return;
			if (path.Length > 0)
				path.Append('.');
			path.Append(path2);
		}
		public static string GetPath(FieldPathDataMemberInfo fieldPathDataMemberInfo) {
			if (fieldPathDataMemberInfo.IsEmpty)
				return String.Empty;
			string path = String.Empty;
			foreach(FieldDataMemberInfoItem item in fieldPathDataMemberInfo.Items) {
				if (item.HasFilters || item.HasGroups)
					Exceptions.ThrowInternalException();
				path = PathHelper.Join(path, item.FieldName);				
			}
			return path;			
		}
	}	
}
