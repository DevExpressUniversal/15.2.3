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
using System.Collections;
namespace DevExpress.Web.Design {
	public class GridLookupDesignerEditForm : LayoutViewDesignerEditorForm {
		public GridLookupDesignerEditForm(ASPxGridLookup gridLookup)
			: base(new GridLookupCommonFormDesigner(gridLookup, gridLookup.Site)) {
		}
	}
	public class GridLookupCommonFormDesigner : GridViewCommonFormDesigner {
		ItemsEditorOwner gridLookupItemsOwner;
		public GridLookupCommonFormDesigner(object gridLookup, IServiceProvider provider)
			: base(gridLookup, provider) {
			GridLookup = (ASPxGridLookup)gridLookup;
		}
		ASPxGridLookup GridLookup { get; set; }
		protected override ASPxGridView GridView { get { return ((ASPxGridLookup)Control).Properties.GridView; } }
		public override ItemsEditorOwner ItemsOwner {
			get {
				if(gridLookupItemsOwner == null)
					gridLookupItemsOwner = new GridLookupColumnsOwner(GridLookup, Provider, GridLookup.Columns);
				return gridLookupItemsOwner;
			}
		}
		protected override void CreateMainGroupItems() {
			CreateItemsItem();
			AddButtonsItem();
			AddClientSideEventsItems();
		}
		protected void AddButtonsItem() {
			var buttonsOwner = new ButtonEditButtonsOwner(GridLookup, Provider);
			var insertBefore = MainGroup.IndexOf(MainGroup.GetItemByCaption(ItemsOwner.GetNavBarItemsGroupName()));
			MainGroup.Insert(insertBefore, CreateDesignerItem("Buttons", "Buttons", typeof(ItemsEditorFrame), GridLookup, ButtonsItemImageIndex, buttonsOwner));
		}
		protected void AddClientSideEventsItems() {
			var gridView = GridLookup.GridView;
			var eventsOwner = new ClientSideEventsOwner(gridView, Provider, ItemsOwner.Designer);
			MainGroup.Add(CreateDesignerItem("GridViewClientSideEvents", "GridView Client-Side Events", typeof(ClientSideEventsFrame), gridView, ClientSideEventsItemImageIndex, eventsOwner));
			eventsOwner = new ClientSideEventsOwner(GridLookup, Provider, gridLookupItemsOwner.Designer);
			MainGroup.Add(CreateDesignerItem("ClientSideEvents", "GridLookup Client-Side Events", typeof(ClientSideEventsFrame), GridLookup, ClientSideEventsItemImageIndex, eventsOwner));
		}
	}
	public class GridLookupColumnsOwner : GridViewColumnsOwner {
		public GridLookupColumnsOwner(object gridLookup, IServiceProvider provider, IList columns)
			: base(gridLookup, provider, columns) {
			GridLookup = gridLookup as ASPxGridLookup;
		}
		ASPxGridLookup GridLookup { get; set; }
		protected override ASPxGridBase GetGrid() {
			return GridLookup.Properties.GridView;
		}
	}
}
