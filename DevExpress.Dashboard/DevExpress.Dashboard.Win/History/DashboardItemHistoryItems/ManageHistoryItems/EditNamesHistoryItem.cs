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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace DevExpress.DashboardWin.Native {
	public class EditNamesHistoryItem : DashboardItemHistoryItem<DashboardItem> {
		readonly List<EditNameCollection> innerCollections = new List<EditNameCollection>();
		readonly ReadOnlyCollection<EditNameCollection> collections;
		public ReadOnlyCollection<EditNameCollection> Collections { get { return collections; } }
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemEditNames; } }
		public EditNamesHistoryItem(DashboardItem dashboardItem, bool allowEditComponentName)
			: base(dashboardItem) {
				collections = new ReadOnlyCollection<EditNameCollection>(innerCollections);
			IEnumerable<EditNameDescription> descriptions = dashboardItem.GetEditNameDescriptions();
			foreach(EditNameDescription description in descriptions) {
				if(description != null && AllowEditName(allowEditComponentName, description)) {
					innerCollections.Add(new EditNameCollection(description.Caption, description.Providers));
				}
			}
		}
		protected override void PerformUndo() {
			innerCollections.ForEach(collection => collection.PerformUndo());
		}
		protected override void PerformRedo() {
			innerCollections.ForEach(collection => collection.PerformRedo());
		}
		bool AllowEditName(bool allowEditComponentName, EditNameDescription decription) {
			foreach(IEditNameProvider provider in decription.Providers) {
				if(provider as DashboardItemComponentNameProvider != null)
					return allowEditComponentName;
			}
			return true;
		}
	}
	public class EditNameCollection {
		readonly string caption;
		readonly List<EditNameItem> innerItems = new List<EditNameItem>();
		readonly ReadOnlyCollection<EditNameItem> items;
		public string Caption { get { return caption; } }
		public ReadOnlyCollection<EditNameItem> Items { get { return items; } }
		public EditNameCollection(string caption, IEnumerable<IEditNameProvider> providers) {
			items = new ReadOnlyCollection<EditNameItem>(innerItems);
			this.caption = caption;
			foreach (IEditNameProvider provider in providers)
				innerItems.Add(new EditNameItem(provider));
		}
		public void PerformUndo() {
			innerItems.ForEach(item => item.PerformUndo());
		}
		public void PerformRedo() {
			innerItems.ForEach(item => item.PerformRedo());
		}
	}
	public class EditNameItem {
		readonly IEditNameProvider provider;
		readonly string oldEditName;
		public string EditName { get; set; }
		public string DisplayName { get { return provider.DisplayName; } }
		public EditNameItem(IEditNameProvider provider) {
			this.provider = provider;
			EditName = oldEditName = provider.EditName;
		}
		public void PerformUndo() {
			provider.EditName = oldEditName;
		}
		public void PerformRedo() {
			provider.EditName = EditName;
		}
	}
}
