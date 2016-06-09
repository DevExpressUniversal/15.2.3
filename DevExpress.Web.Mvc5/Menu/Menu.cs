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

using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using DevExpress.Web;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Mvc.UI;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Internal;
	[ToolboxItem(false)]
	public class MVCxMenu : ASPxMenu {
		protected const string ItemsInfoKey = "itemsInfo";
		public MVCxMenu()
			: base() {
		}
		public new MenuImages Images {
			get { return base.Images; }
		}
		public new MenuStyles Styles {
			get { return base.Styles; }
		}
		public override bool IsLoading() {
			return false;
		}
		protected internal void ValidateProperties() {
			ValidateSelectedItem();
		}
		protected internal new string GetItemIndexPath(MenuItem item) {
			return base.GetItemIndexPath(item);
		}
		protected internal new string GetCheckedState() {
			return base.GetCheckedState();
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			result.Add(ItemsInfoKey, MenuState.SaveItemsInfo(Items));
			return result;
		}
		internal static MenuState GetState(string name) {
			Hashtable clientObjectState = LoadClientObjectState(HttpContext.Current.Request.Params, name);
			if(clientObjectState == null) return null;
			string serializedItemsInfo = GetClientObjectStateValue<string>(clientObjectState, ItemsInfoKey);
			string selectedItemState = GetClientObjectStateValue<string>(clientObjectState, SelectedItemIndexPathKey);
			string itemsCheckedState = GetClientObjectStateValue<string>(clientObjectState, CheckedStateKey);
			return MenuState.Load(serializedItemsInfo, selectedItemState, itemsCheckedState);
		}
	}
	[ToolboxItem(false)]
	public class MVCxPopupMenu : ASPxPopupMenu {
		public MVCxPopupMenu()
			: base() {
		}
		public new MenuImages Images {
			get { return base.Images; }
		}
		public new MenuStyles Styles {
			get { return base.Styles; }
		}
		public override bool IsLoading() {
			return false;
		}
		protected internal void ValidateProperties() {
			ValidateSelectedItem();
		}
	}
}
