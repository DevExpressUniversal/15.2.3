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
using DevExpress.Utils.Commands;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Office.UI;
namespace DevExpress.Office.Internal {
	#region BarItemCommandUIState
	public class BarItemCommandUIState : ICommandUIState {
		readonly BarItem item;
		public BarItemCommandUIState(BarItem item) {
			Guard.ArgumentNotNull(item, "item");
			this.item = item;
		}
		protected BarItem Item { get { return item; } }
		public virtual object EditValue {
			get {
				BarEditItem editItem = item as BarEditItem;
				if (editItem != null)
					return editItem.EditValue;
				IEditValueBarItem editValueBarItem = item as IEditValueBarItem;
				if (editValueBarItem != null)
					return editValueBarItem.EditValue;
				return null;
			}
			set {
				BarEditItem editItem = item as BarEditItem;
				if (editItem != null && !Object.Equals(editItem.EditValue, value))
					editItem.EditValue = value;
				IEditValueBarItem editValueBarItem = item as IEditValueBarItem;
				if (editValueBarItem != null && !(item is BarSplitButtonColorEditItem))
					editValueBarItem.EditValue = value;
			}
		}
		#region ICommandUIState Members
		public bool Checked {
			get {
				BarCheckItem checkItem = item as BarCheckItem;
				if (checkItem != null) {
					Nullable<bool> val = checkItem.IsChecked;
					return val.HasValue ? val.Value : false;
				}
				else
					return false;
			}
			set {
				BarCheckItem checkItem = item as BarCheckItem;
				if (checkItem != null)
					checkItem.IsChecked = value;
			}
		}
		public bool Enabled { get { return item.IsEnabled; } set { item.IsEnabled = value; } }
		public bool Visible { get { return item.IsVisible; } set { item.IsVisible = value; } }
		#endregion
	}
	#endregion
}
