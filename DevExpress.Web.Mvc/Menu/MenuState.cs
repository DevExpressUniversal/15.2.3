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
	public class MenuState {
		public MenuState() {
			RootItem = new MenuItemState();
		}
		public IEnumerable<MenuItemState> Items { get { return RootItem.Items; } }
		public MenuItemState SelectedItem { get; private set; }
		MenuItemState RootItem { get; set; }
		internal static string SaveItemsInfo(MenuItemCollection items) {
			using(var stream = new MemoryStream())
			using(var writer = new TypedBinaryWriter(stream)) {
				SaveItemsCore(writer, items);
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		static void SaveItemsCore(TypedBinaryWriter writer, MenuItemCollection items) {
			writer.WriteObject(items.Count);
			for(int i = 0; i < items.Count; i++) {
				writer.WriteObject(items[i].Index);
				writer.WriteObject(items[i].Name);
				writer.WriteObject(items[i].Text);
				writer.WriteObject(items[i].Checked);
				SaveItemsCore(writer, items[i].Items);
			}
		}
		internal static MenuState Load(string serializedItemsInfo, string selectedItemState, string itemsCheckedState) {
			if(string.IsNullOrEmpty(serializedItemsInfo))
				return null;
			var menuState = new MenuState();
			menuState.LoadItems(serializedItemsInfo);
			menuState.LoadSelectedItemState(selectedItemState);
			menuState.LoadItemsCheckedState(itemsCheckedState);
			return menuState;
		}
		protected void LoadItems(string serializedItemsInfo) {
			using(var stream = new MemoryStream(Convert.FromBase64String(serializedItemsInfo)))
			using(var reader = new TypedBinaryReader(stream)) {
				LoadItemsCore(reader, RootItem);
			}
		}
		static void LoadItemsCore(TypedBinaryReader reader, MenuItemState rootItem){
			int itemsCount = reader.ReadObject<int>();
			var items = new MenuItemState[itemsCount];
			for(int i = 0; i < itemsCount; i++) {
				items[i] = new MenuItemState();
				items[i].Index = reader.ReadObject<int>();
				items[i].Name = reader.ReadObject<string>();
				items[i].Text = reader.ReadObject<string>();
				items[i].Checked = reader.ReadObject<bool>();
				LoadItemsCore(reader, items[i]);
			}
			rootItem.Items = items;
		}
		protected void LoadSelectedItemState(string selectedItemState) {
			if(string.IsNullOrEmpty(selectedItemState))
				return;
			SelectedItem = GetItemByIndexPath(selectedItemState);
		}
		protected void LoadItemsCheckedState(string itemsCheckedState) {
			if(string.IsNullOrEmpty(itemsCheckedState))
				return;
			string[] indexPathes = itemsCheckedState.Split(';');
			for(int i = 0; i < indexPathes.Length; i++) {
				MenuItemState item = GetItemByIndexPath(indexPathes[i]);
				if(item != null)
					item.Checked = true;
			}
		}
		MenuItemState GetItemByIndexPath(string path) {
			if(string.IsNullOrEmpty(path))
				return null;
			string[] indexes = path.Split(RenderUtils.IndexSeparator);
			MenuItemState item = RootItem;
			for(int i = 0; i < indexes.Length; i++) {
				int index = int.Parse(indexes[i]);
				if(index < 0 || index >= item.Items.Count()) {
					item = null;
					break;
				}
				item = item.Items.ElementAt(index);
			}
			return item;
		}
	}
	public class MenuItemState {
		public MenuItemState() {
			Items = new MenuItemState[0];
		}
		public int Index { get; internal set; }
		public string Name { get; internal set; }
		public string Text { get; internal set; }
		public bool Checked { get; internal set; }
		public IEnumerable<MenuItemState> Items { get; internal set; }
	}
}
