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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[ToolboxItem(false)]
	public class TemplateContainerBase: Control, IDataItemContainer, INamingContainer {
		private object fDataItem;
		private int fItemIndex;
#if !SL
	[DevExpressWebLocalizedDescription("TemplateContainerBaseDataItem")]
#endif
		public object DataItem {
			get { return fDataItem; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("TemplateContainerBaseItemIndex")]
#endif
		public int ItemIndex {
			get { return fItemIndex; }
		}
		public TemplateContainerBase(int itemIndex, object dataItem)
			: base() {
			fItemIndex = itemIndex;
			fDataItem = dataItem;
		}
#if !SL
	[DevExpressWebLocalizedDescription("TemplateContainerBaseClientID")]
#endif
		public override string ClientID {
			get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
		}
		object IDataItemContainer.DataItem {
			get {
				return fDataItem;
			}
		}
		int IDataItemContainer.DataItemIndex {
			get {
				return fItemIndex;
			}
		}
		int IDataItemContainer.DisplayIndex {
			get {
				return fItemIndex;
			}
		}
		public void AddToHierarchy(Control parent, string containerID) {
			ClientIDHelper.AddTemplateContainerToHierarchy(parent, this, containerID);
		}
		public static Control FindTemplateControl(Control control, string templateContainerID, string id) {
			Control container = control.FindControl(templateContainerID);
			return (container != null) ? container.FindControl(id) : null;
		}
	}
	[ToolboxItem(false)]
	public class ItemTemplateContainerBase : TemplateContainerBase {
		public ItemTemplateContainerBase(int itemIndex, object dataItem)
			: base(itemIndex, dataItem) {
		}
		public object EvalDataItem(string expression) {
			return DataBinder.Eval(GetDataItem(), expression);
		}
		protected virtual object GetDataItem() {
			return DataItem;
		}
	}
}
