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
namespace DevExpress.Snap.Core.Native.Data {
	public class FieldPathDataMemberInfo {
		static readonly FieldPathDataMemberInfo empty = new FieldPathDataMemberInfo();
		public static FieldPathDataMemberInfo Empty { get { return empty; } }
		readonly List<FieldDataMemberInfoItem> items;
		public bool IsEmpty { get { return items.Count == 0; } }
		public FieldDataMemberInfoItem LastItem { get { return IsEmpty ? null : items[items.Count - 1]; } }
		public FieldPathDataMemberInfo() {
			items = new List<FieldDataMemberInfoItem>();
		}
		public List<FieldDataMemberInfoItem> Items { get { return items; } }
		public void AddFieldName(string fieldName) {
			Items.Add(new FieldDataMemberInfoItem(fieldName));
		}
		public void AddGroup(GroupProperties groupProperties) {
			EnsureItemsNotEmpty();
			FieldDataMemberInfoItem item = Items[Items.Count - 1];
			item.AddGroup(groupProperties);
		}
		public void AddFilter(string filter) {
			EnsureItemsNotEmpty();
			FieldDataMemberInfoItem item = Items[Items.Count - 1];
			item.AddFilter(filter);
		}
		public void AddFilter(string filter, bool unescapeString) {
			if (unescapeString) {
				StringBuilder sb = new StringBuilder(filter.Length);
				int index = 0;
				while (index < filter.Length) {
					char ch = filter[index];
					if (ch == '\\') {
						index++;
						if (index >= filter.Length)
							break;
						ch = filter[index];
					}
					sb.Append(ch);
					index++;
				}
				filter = sb.ToString();				
			}
			AddFilter(filter);
		}
		internal void EnsureItemsNotEmpty() {
			if (Items.Count == 0)
				Items.Add(new FieldDataMemberInfoItem(String.Empty));
		}
		internal void AddEmptyFilterIfNeeded() {
			EnsureItemsNotEmpty();
			if (!Items[Items.Count - 1].HasFilters && !Items[Items.Count - 1].HasGroups)
				AddFilter(String.Empty);
		}
	}
	public class FieldDataMemberInfoItem {
		readonly string fieldName;
		int currentGroupIndex = 0;
		public FieldDataMemberInfoItem(string fieldName)  {
			this.fieldName = fieldName;
		}
		public FieldDataMemberInfoItem(string fieldName, List<GroupProperties> groups, FilterProperties filterProperties) {
			this.fieldName = fieldName;
			this.Groups = groups;
			this.FilterProperties = filterProperties;
		}
		public string FieldName { get { return fieldName; } }
		protected internal int CurrentGroupIndex { get { return currentGroupIndex; } set { currentGroupIndex = value; } }
		public List<GroupProperties> Groups { get; protected set; }
		public FilterProperties FilterProperties { get; protected set; }
		public bool HasFilters { get { return FilterProperties != null && !FilterProperties.IsEmpty; } }
		public bool HasGroups { get { return Groups != null && Groups.Count > 0; } }
		public override string ToString() {
			return FieldName;
		}
		public virtual void AddGroup(GroupProperties groupProperties) {
			if (Groups == null)
				Groups = new List<GroupProperties>();
			Groups.Add(groupProperties);
		}
		public virtual void InsertGroup(GroupProperties groupProperties) {
			if (Groups == null)
				Groups = new List<GroupProperties>();
			Groups.Insert(currentGroupIndex, groupProperties);
			currentGroupIndex++;
		}
		public virtual void RemoveGroup(GroupProperties groupProperties) {
			if (Groups.Remove(groupProperties))
				currentGroupIndex--;
		}
		public virtual void AddFilter(string filter) {
			if (FilterProperties == null)
				FilterProperties = new FilterProperties();
			FilterProperties.AddFilter(filter);			
		}
		public virtual void MoveToGrouped(int index) {
			if (index <= currentGroupIndex)
				return;
			GroupProperties properties = Groups[index];
			Groups.RemoveAt(index);
			Groups.Insert(currentGroupIndex, properties);
			currentGroupIndex++;
		}
	}
}
