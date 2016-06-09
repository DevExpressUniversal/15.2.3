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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class TitleIndexColumnSeparatorTemplateContainer : TemplateContainerBase {
		private TitleIndexColumn fColumn = null;
		public TitleIndexColumn Column {
			get { return fColumn; }
		}
		public TitleIndexColumnSeparatorTemplateContainer(int cellIndex, TitleIndexColumn column)
			: base(cellIndex, null) {
			fColumn = column;
		}
	}
	public class TitleIndexItemTemplateContainer : ItemTemplateContainerBase {
		private TitleIndexItem fItem = null;
#if !SL
	[DevExpressWebLocalizedDescription("TitleIndexItemTemplateContainerItem")]
#endif
		public TitleIndexItem Item {
			get { return fItem; }
		}
		public TitleIndexItemTemplateContainer(TitleIndexItem item)
			: base(item.Index, item) {
			fItem = item;
		}
		protected override bool OnBubbleEvent(object source, EventArgs e) {
			if (e is CommandEventArgs) {
				RaiseBubbleEvent(this, new TitleIndexItemCommandEventArgs(Item, source, e as CommandEventArgs));
				return true;
			}
			return false;
		}
		protected override object GetDataItem() {
			return Item.DataItem;
		}
	}
	public class GroupHeaderTemplateContainer : TemplateContainerBase {
		private object fGroupValue = null;
		private int fGroupItemCount = 0;
#if !SL
	[DevExpressWebLocalizedDescription("GroupHeaderTemplateContainerGroupItemCount")]
#endif
		public int GroupItemCount {
			get { return fGroupItemCount; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("GroupHeaderTemplateContainerGroupValue")]
#endif
		public object GroupValue {
			get { return fGroupValue; }
		}
		public GroupHeaderTemplateContainer(int index, object groupValue, int groupItemCount)
			: base(index, null) {
			fGroupItemCount = groupItemCount;
			fGroupValue = groupValue;
		}
		protected override bool OnBubbleEvent(object source, EventArgs e) {
			if (e is CommandEventArgs) {
				RaiseBubbleEvent(this, new GroupHeaderCommandEventArgs(GroupValue, source, e as CommandEventArgs));
				return true;
			}
			return false;
		}
	}
	public class IndexPanelItemTemplateContainer : TemplateContainerBase {
		private object fGroupValue = null;
		private int fGroupItemCount = 0;
		private string fNavigateUrl = "";
#if !SL
	[DevExpressWebLocalizedDescription("IndexPanelItemTemplateContainerGroupValue")]
#endif
		public object GroupValue {
			get { return fGroupValue; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("IndexPanelItemTemplateContainerGroupItemCount")]
#endif
public int GroupItemCount {
			get { return fGroupItemCount; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("IndexPanelItemTemplateContainerNavigateUrl")]
#endif
public string NavigateUrl {
			get { return fNavigateUrl; }
		}
		public IndexPanelItemTemplateContainer(int index, object groupValue, int groupItemCount, string navigateUrl)
			: base(index, null) {
			fGroupValue = groupValue;
			fGroupItemCount = groupItemCount;
			fNavigateUrl = navigateUrl;
		}
		protected override bool OnBubbleEvent(object source, EventArgs e) {
			if (e is CommandEventArgs) {
				RaiseBubbleEvent(this, new IndexPanelItemCommandEventArgs(GroupValue, source, e as CommandEventArgs));
				return true;
			}
			return false;
		}
	}
}
