#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.DB;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.DragDrop;
using DevExpress.DashboardWin.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.Skins;
using DevExpress.XtraTreeList.Menu;
using DevExpress.DashboardCommon.Localization;
using DevExpress.Utils.Menu;
namespace DevExpress.DashboardWin.Native {
	public partial class DataFieldsBrowser {
		class ItemComparer : IComparer<DataFieldsBrowserItem> {
			static int CompareNodes(DataNodeType x, DataNodeType y) {
				return GetNodeSortOrder(x).CompareTo(GetNodeSortOrder(y));
			}
			static int GetNodeSortOrder(DataNodeType nodeType) {
				switch(nodeType) {
					case DataNodeType.OlapMeasureFolder:
						return 0;
					case DataNodeType.OlapKpiFolder:
						return 1;
					case DataNodeType.OlapDimensionFolder:
						return 2;
					case DataNodeType.OlapFolder:
						return 3;
					case DataNodeType.OlapMeasure:
						return 4;
					case DataNodeType.OlapKpi:
						return 5;
					case DataNodeType.OlapDimension:
						return 6;
					case DataNodeType.OlapHierarchy:
						return 7;
					case DataNodeType.CalculatedFields:
						return -4;
					case DataNodeType.Unknown:
					case DataNodeType.DataSource:
					case DataNodeType.DataMember:
					case DataNodeType.DataField:
					case DataNodeType.OlapDataSource:
					default:
						return -3;
				}
			}
			static string GetDisplayName(DataFieldsBrowserItem item) {
				return item != null ? item.DisplayName : null;
			}
			readonly int factor;
			public ItemComparer(bool ascending) {
				factor = ascending ? 1 : -1;
			}
			public int Compare(DataFieldsBrowserItem x, DataFieldsBrowserItem y) {
				int result = CompareNodes(x.DataNode.NodeType, y.DataNode.NodeType);
				if(result != 0)
					return result;
				string displayNameX = GetDisplayName(x);
				string displayNameY = GetDisplayName(y);
				if(displayNameX != null)
					return displayNameY != null ? displayNameX.CompareTo(displayNameY) * factor : 1;
				return displayNameY != null ? -1 : 0;
			}
		}
		class ItemGroupContainer {
			readonly OrderedDictionary<DataFieldsBrowserGroupType, List<DataFieldsBrowserItem>> dictionary = new OrderedDictionary<DataFieldsBrowserGroupType, List<DataFieldsBrowserItem>>();
			readonly List<DataFieldsBrowserItem> untypedGroup = new List<DataFieldsBrowserItem>();
			public ReadOnlyCollection<List<DataFieldsBrowserItem>> Groups { get { return dictionary.Values; } }
			public List<DataFieldsBrowserItem> UntypedGroup { get { return untypedGroup; } }
			public ItemGroupContainer() {
				RegisterItemGroup(DataFieldsBrowserGroupType.CalculatedFields);
				RegisterItemGroup(DataFieldsBrowserGroupType.CalculatedFieldText);
				RegisterItemGroup(DataFieldsBrowserGroupType.CalculatedFieldDateTime);
				RegisterItemGroup(DataFieldsBrowserGroupType.CalculatedFieldBool);
				RegisterItemGroup(DataFieldsBrowserGroupType.CalculatedFieldInteger);
				RegisterItemGroup(DataFieldsBrowserGroupType.CalculatedFieldDecimal);
				RegisterItemGroup(DataFieldsBrowserGroupType.CalculatedFieldObject);
				RegisterItemGroup(DataFieldsBrowserGroupType.CalculatedFieldCorrupted);
				RegisterItemGroup(DataFieldsBrowserGroupType.Text);
				RegisterItemGroup(DataFieldsBrowserGroupType.DateTime);
				RegisterItemGroup(DataFieldsBrowserGroupType.Bool);
				RegisterItemGroup(DataFieldsBrowserGroupType.Integer);
				RegisterItemGroup(DataFieldsBrowserGroupType.Float);
				RegisterItemGroup(DataFieldsBrowserGroupType.Double);
				RegisterItemGroup(DataFieldsBrowserGroupType.Decimal);
				RegisterItemGroup(DataFieldsBrowserGroupType.Custom);
				RegisterItemGroup(DataFieldsBrowserGroupType.Complex);
				RegisterItemGroup(DataFieldsBrowserGroupType.List);
			}
			public void AddItem(DataFieldsBrowserItem item) {
				if (dictionary.ContainsKey(item.GroupType))
					dictionary[item.GroupType].Add(item);
				else
					untypedGroup.Add(item);
			}
			void RegisterItemGroup(DataFieldsBrowserGroupType groupType) {
				dictionary.Add(groupType, new List<DataFieldsBrowserItem>());
			}
		}
		class ExpandAndFocusCache {
			static bool CheckItem(DataFieldsBrowserItem item) {
				return !String.IsNullOrEmpty(GetDataMember(item));
			}
			static string GetDataMember(DataFieldsBrowserItem item) {
				if(item.GroupType == DataFieldsBrowserGroupType.CalculatedFields)
					return item.DisplayName;;
				return item.DataMember;
			}
			readonly List<string> dataMembers = new List<string>();
			string focusDataMember;
			public void OnExpand(TreeListNode node) {
				DataFieldsBrowserItem item = GetItem(node);
				if (CheckItem(item)) {
					if (!dataMembers.Contains(GetDataMember(item)))
						dataMembers.Add(GetDataMember(item));
				}
				item.OnExpand(node);
			}
			public void OnCollapse(TreeListNode node) {
				DataFieldsBrowserItem item = GetItem(node);
				if (CheckItem(item)) {
					if (dataMembers.Contains(GetDataMember(item)))
						dataMembers.Remove(GetDataMember(item));
					else
						throw new InvalidOperationException(String.Format("{0} is already collapsed", GetDataMember(item)));
				}
				item.OnCollapse(node);
			}
			public void OnFocus(TreeListNode node) {
				DataFieldsBrowserItem item = GetItem(node);
				focusDataMember = CheckItem(item) ? GetDataMember(item) : null;
			}
			public void OnFocus(string dataMember) {
				focusDataMember = dataMember;
			}
			public bool ShouldExpand(TreeListNode node) {
				DataFieldsBrowserItem item = GetItem(node);
				return CheckItem(item) && (dataMembers.Contains(GetDataMember(item)) || focusDataMember != null && focusDataMember.StartsWith(GetDataMember(item) + "."));
			}
			public bool ShouldFocus(TreeListNode node) {
				DataFieldsBrowserItem item = GetItem(node);
				return CheckItem(item) && GetDataMember(item) == focusDataMember;
			}
			public void Clear() {
				dataMembers.Clear();
				focusDataMember = null;
			}
		}
		class ExpandAndFocusOperation : TreeListOperation {
			readonly ExpandAndFocusCache cache;
			public ExpandAndFocusOperation(ExpandAndFocusCache cache) {
				this.cache = cache;
			}
			public override void Execute(TreeListNode node) {
				if (cache.ShouldExpand(node))
					node.Expanded = true;
				if (cache.ShouldFocus(node))
					node.Selected = true;
			}
		}
	}
}
