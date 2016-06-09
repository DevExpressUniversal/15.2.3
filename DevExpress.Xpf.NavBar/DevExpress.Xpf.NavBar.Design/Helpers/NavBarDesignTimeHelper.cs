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

extern alias Platform;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using System.Collections.Generic;
using Microsoft.Windows.Design.Interaction;
#if SL
using Platform::DevExpress.Xpf.NavBar;
#endif
namespace DevExpress.Xpf.NavBar.Design {
	public static class NavBarDesignTimeHelper {
		public static ModelItem AddNewNavBarGroup(ModelItem navBar) {
			if(navBar == null) return null;
			ModelItem group = CreateNavBarGroup(navBar.Context);
			using(ModelEditingScope scope = navBar.BeginEdit("Add NavBarGroup")) {
				navBar.Properties["Groups"].Collection.Add(group);
				scope.Complete();
			}
			return group;
		}
		public static ModelItem AddNewNavBarItem(ModelItem group) {
			if(group == null) return null;
			ModelItem item = CreateNavBarItem(group.Context);
			using(ModelEditingScope scope = group.BeginEdit("Add NavBarItem")) {
				group.Properties["Items"].Collection.Add(item);
				scope.Complete();
			}
			return item;
		}
		public static ModelItem FindInParents<T>(ModelItem from) {
			ModelItem item = from;
			while(item != null) {
				if(item.IsItemOfType(typeof(T))) return item;
				item = item.Parent;
			}
			return item;
		}
		public static ModelItem FindNavBarControl(ModelItem from) {
			return FindInParents<NavBarControl>(from);
		}
		static ModelItem CreateNavBarGroup(EditingContext context) {
#if !SL
			return ModelFactory.CreateItem(context, typeof(NavBarGroup), CreateOptions.InitializeDefaults);
#else
			ModelItem newGroup = ModelFactory.CreateItem(context, typeof(NavBarGroup), CreateOptions.InitializeDefaults);
			RenameModelItem(newGroup);
			newGroup.Properties["Header"].SetValue(newGroup.Name);
			return newGroup;
#endif
		}
		static ModelItem CreateNavBarItem(EditingContext context) {
#if !SL
			return ModelFactory.CreateItem(context, typeof(NavBarItem), CreateOptions.InitializeDefaults);
#else
			ModelItem newItem = ModelFactory.CreateItem(context, typeof(NavBarItem), CreateOptions.InitializeDefaults);
			RenameModelItem(newItem);
			newItem.Properties["Content"].SetValue(newItem.Name);
			return newItem;
#endif
		}
		static void RenameModelItem(ModelItem item) {
			ModelService service = item.Context.Services.GetRequiredService<ModelService>();
			List<ModelItem> existingItems = new List<ModelItem>(service.Find(item.Root, item.ItemType));
			if(!IsNameExist(existingItems, item.Name)) return;
			int counter = 1;
			string prefix = (char)(item.ItemType.Name[0] + 32) + item.ItemType.Name.Substring(1);
			string newName = string.Empty;
			do {
				newName = string.Format("{0}{1}", prefix, counter);
				counter++;
			} while(IsNameExist(existingItems, newName));
			item.Name = newName;
		}
		static bool IsNameExist(List<ModelItem> existingItems, string name) {
			if(string.IsNullOrEmpty(name)) return false;
			foreach(ModelItem item in existingItems)
				if(item.Name.Equals(name)) return true;
			return false;
		}
	}
	public static class NavBarViewItemHelper {
		public static List<ViewItem> GetViewItems<T>(ViewItem from) {
			List<ViewItem> result = new List<ViewItem>();
			if(from == null) return result;
			foreach(ViewItem child in from.VisualChildren) {
				if(child.ItemType == typeof(T)) result.Add(child);
				result.AddRange(GetViewItems<T>(child));
			}
			return result;
		}
	}
}
