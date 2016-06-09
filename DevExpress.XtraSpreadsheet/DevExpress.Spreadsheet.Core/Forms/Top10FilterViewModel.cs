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
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region Top10FilterViewModel
	public class Top10FilterViewModel : ViewModelBase {
		readonly ISpreadsheetControl control;
		readonly List<Top10OrderItem> orderDataSource;
		readonly List<Top10TypeItem> typeDataSource;
		int value;
		bool isTop;
		bool isPercent;
		public Top10FilterViewModel(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.orderDataSource = CreateOrderDataSource();
			this.typeDataSource = CreateTypeDataSource();
			this.IsTop = true;
			this.Value = 10;
		}
		#region Properties
		public ISpreadsheetControl Control { get { return control; } }
		public int Value {
			get { return value; }
			set {
				if (Value == value)
					return;
				this.value = value;
				OnPropertyChanged("Value");
			}
		}
		public IList<Top10OrderItem> OrderDataSource { get { return orderDataSource; } }
		public IList<Top10TypeItem> TypeDataSource { get { return typeDataSource; } }
		public bool IsTop {
			get { return isTop; }
			set {
				if (IsTop == value)
					return;
				this.isTop = value;
				OnPropertyChanged("IsTop");
			}
		}
		public bool IsPercent {
			get { return isPercent; }
			set {
				if (IsPercent == value)
					return;
				this.isPercent = value;
				OnPropertyChanged("IsPercent");
			}
		}
		#endregion
		List<Top10OrderItem> CreateOrderDataSource() {
			List<Top10OrderItem> result = new List<Top10OrderItem>();
			result.Add(new Top10OrderItem() { Value = true, Text = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_Top10FilterOrderTop) });
			result.Add(new Top10OrderItem() { Value = false, Text = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_Top10FilterOrderBottom) });
			return result;
		}
		List<Top10TypeItem> CreateTypeDataSource() {
			List<Top10TypeItem> result = new List<Top10TypeItem>();
			result.Add(new Top10TypeItem() { Value = false, Text = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_Top10FilterTypeItems) });
			result.Add(new Top10TypeItem() { Value = true, Text = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_Top10FilterTypePercent) });
			return result;
		}
		public bool Validate() {
			FilterTop10Command command = new FilterTop10Command(Control);
			return command.Validate(this);
		}
		public void ApplyChanges() {
			FilterTop10Command command = new FilterTop10Command(Control);
			command.ApplyChanges(this);
		}
	}
	#endregion
	public class Top10OrderItem {
		public string Text { get; set; }
		public bool Value { get; set; }
		public override string ToString() {
			return Text;
		}
	}
	public class Top10TypeItem {
		public string Text { get; set; }
		public bool Value { get; set; }
		public override string ToString() {
			return Text;
		}
	}
}
