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
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Forms {
	public interface IConditionalFormattingHighlightCellsRuleViewModel : IReferenceEditViewModel {
		string Value { get; set; }
		string FormText { get; set; }
		string LabelHeaderText { get; set; }
		ConditionalFormattingStyle Style { get; set; }
		List<ConditionalFormattingStyle> Styles { get; }
	}
	#region ConditionalFormattingHighlightCellsRuleViewModel
	public class ConditionalFormattingHighlightCellsRuleViewModel : ConditionalFormattingViewModelBase, IConditionalFormattingHighlightCellsRuleViewModel {
		string expression = String.Empty;
		public ConditionalFormattingHighlightCellsRuleViewModel(ISpreadsheetControl control)
			: base(control) {
		}
		public string Value {
			get { return expression; }
			set {
				if (Value == value)
					return;
				this.expression = value;
				OnPropertyChanged("Value");
			}
		}
	}
	#endregion
	#region ConditionalFormattingTextRuleViewModel
	public class ConditionalFormattingTextRuleViewModel : ConditionalFormattingViewModelBase, IConditionalFormattingHighlightCellsRuleViewModel {
		string text = String.Empty;
		public ConditionalFormattingTextRuleViewModel(ISpreadsheetControl control)
			: base(control) {
		}
		public string Value {
			get { return text; }
			set {
				if (Value == value)
					return;
				this.text = value;
				OnPropertyChanged("Value");
			}
		}
	}
	#endregion
}
