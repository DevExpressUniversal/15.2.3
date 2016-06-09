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
using System.Data;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class DataViewItemTemplateContainer : ItemTemplateContainerBase {
		private DataViewItem fDataViewItem = null;
#if !SL
	[DevExpressWebLocalizedDescription("DataViewItemTemplateContainerItem")]
#endif
		public DataViewItem Item {
			get { return fDataViewItem; }
		}
		public DataViewItemTemplateContainer(DataViewItem item)
			: base(item.Index, item.DataItem) {
			fDataViewItem = item;
		}
		protected override bool OnBubbleEvent(object source, EventArgs e) {
			if (e is CommandEventArgs) {
				RaiseBubbleEvent(this, new DataViewItemCommandEventArgs(Item, source, e as CommandEventArgs));
				return true;
			}
			return false;
		}
		protected override object GetDataItem() {
			return Item.DataItem;
		}
		public override void DataBind() {
			if(DataItem == null) return;
			base.DataBind();
		}
	}
	public class DataViewTemplateContainer : TemplateContainerBase {
#if !SL
	[DevExpressWebLocalizedDescription("DataViewTemplateContainerDataView")]
#endif
		public ASPxDataViewBase DataView {
			get { return DataItem as ASPxDataViewBase; }
		}
		public DataViewTemplateContainer(ASPxDataViewBase dataView)
			: base(0, dataView) {
		}
	}
	public class DataViewPagerPanelTemplateContainer: TemplateContainerBase {
		private PagerPanelPosition fPosition = PagerPanelPosition.Top;
		private PagerPanelTemplatePosition fTemplatePosition = PagerPanelTemplatePosition.Left;
#if !SL
	[DevExpressWebLocalizedDescription("DataViewPagerPanelTemplateContainerDataView")]
#endif
		public ASPxDataViewBase DataView {
			get { return DataItem as ASPxDataViewBase; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("DataViewPagerPanelTemplateContainerPosition")]
#endif
		public PagerPanelPosition Position {
			get { return fPosition; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("DataViewPagerPanelTemplateContainerTemplatePosition")]
#endif
		public PagerPanelTemplatePosition TemplatePosition {
			get { return fTemplatePosition; }
		}
		public DataViewPagerPanelTemplateContainer(ASPxDataViewBase dataView,
			PagerPanelPosition position, PagerPanelTemplatePosition templatePosition)
			: base(0, dataView) {
			fPosition = position;
			fTemplatePosition = templatePosition;
		}
		protected override bool OnBubbleEvent(object source, EventArgs e) {
			if (e is CommandEventArgs) {
				RaiseBubbleEvent(this, new DataViewPagerPanelCommandEventArgs(Position, TemplatePosition, source, e as CommandEventArgs));
				return true;
			}
			return false;
		}
	}
}
