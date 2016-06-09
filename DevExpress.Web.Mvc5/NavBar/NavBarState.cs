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

using DevExpress.Data.IO;
using DevExpress.Web.Internal;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace DevExpress.Web.Mvc {
	public class NavBarState {
		const char PropertySeparator = ';';
		public NavBarState() {
			Groups = new NavBarGroupState[0];
		}
		public IEnumerable<NavBarGroupState> Groups { get; private set; }
		public NavBarItemState SelectedItem { get; private set; }
		internal static string SaveGroupsItemsGeneralInfo(NavBarGroupCollection groups) {
			using(var stream = new MemoryStream())
			using(var writer = new TypedBinaryWriter(stream)) {
				writer.WriteObject(groups.Count);
				for(int i = 0; i < groups.Count; i++) {
					writer.WriteObject(groups[i].Index);
					writer.WriteObject(groups[i].Name);
					writer.WriteObject(groups[i].Text);
					SaveGroupItemsCore(writer, groups[i].Items);
				}
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		static void SaveGroupItemsCore(TypedBinaryWriter writer, NavBarItemCollection items) {
			writer.WriteObject(items.Count);
			for(int i = 0; i < items.Count; i++) {
				writer.WriteObject(items[i].Index);
				writer.WriteObject(items[i].Name);
				writer.WriteObject(items[i].Text);
			}
		}
		internal static NavBarState Load(string serializedGroupsInfo, string groupsExpandedState, string itemsSelectionState) {
			if(string.IsNullOrEmpty(serializedGroupsInfo))
				return null;
			var navBarState = new NavBarState();
			navBarState.LoadGroupsItemsGeneralInfo(serializedGroupsInfo);
			navBarState.LoadGroupsExpandedState(groupsExpandedState);
			navBarState.LoadItemsSelectionState(itemsSelectionState);
			return navBarState;
		}
		protected void LoadGroupsItemsGeneralInfo(string serializedGroupsInfo) {
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(serializedGroupsInfo)))
			using(TypedBinaryReader reader = new TypedBinaryReader(stream)) {
				int groupsCount = reader.ReadObject<int>();
				var groupsSate = new NavBarGroupState[groupsCount];
				for(int i = 0; i < groupsCount; i++) {
					groupsSate[i] = new NavBarGroupState();
					groupsSate[i].Index = reader.ReadObject<int>();
					groupsSate[i].Name = reader.ReadObject<string>();
					groupsSate[i].Text = reader.ReadObject<string>();
					LoadGroupItems(reader, groupsSate[i]);
				}
				Groups = groupsSate;
			}
		}
		void LoadGroupItems(TypedBinaryReader reader, NavBarGroupState group) {
			int itemsCount = reader.ReadObject<int>();
			var itemsState = new NavBarItemState[itemsCount];
			for(int i = 0; i < itemsCount; i++) {
				itemsState[i] = new NavBarItemState(group);
				itemsState[i].Index = reader.ReadObject<int>();
				itemsState[i].Name = reader.ReadObject<string>();
				itemsState[i].Text = reader.ReadObject<string>();
			}
			group.Items = itemsState;
		}
		protected void LoadGroupsExpandedState(string groupsExpandedState) {
			if(string.IsNullOrEmpty(groupsExpandedState))
				return;
			string[] expandInfo = groupsExpandedState.Split(new char[] { ';' });
			for(int i = 0; i < Groups.Count(); i++) {
				if(i < expandInfo.Length)
					Groups.ElementAt(i).Expanded = expandInfo[i] == "1";
			}
		}
		protected void LoadItemsSelectionState(string itemsSelectionState) {
			if(string.IsNullOrEmpty(itemsSelectionState))
				return;
			string[] indexes = itemsSelectionState.Split(RenderUtils.IndexSeparator);
			int groupIndex = int.Parse(indexes[0]);
			if(groupIndex < 0 || groupIndex >= Groups.Count())
				return;
			NavBarGroupState group = Groups.ElementAt(groupIndex);
			int itemIndex = int.Parse(indexes[1]);
			if(0 <= itemIndex && itemIndex < group.Items.Count())
				SelectedItem = group.Items.ElementAt(itemIndex);
		}
	}
	public class NavBarGroupState: NavBarItemStateBase {
		public NavBarGroupState() {
			Items = new NavBarItemState[0];
		}
		public IEnumerable<NavBarItemState> Items { get; protected internal set; }
		public bool Expanded { get; protected internal set; }
	}
	public class NavBarItemState: NavBarItemStateBase {
		public NavBarItemState() {
		}
		internal NavBarItemState(NavBarGroupState group) {
			Group = group;
		}
		public NavBarGroupState Group { get; private set; }
	}
	public class NavBarItemStateBase {
		public int Index { get; protected internal set; }
		public string Name { get; protected internal set; }
		public string Text { get; protected internal set; }
	}
}
