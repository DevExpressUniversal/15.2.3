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
using System.Linq;
using System.Text;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Windows.Forms;
using System.Drawing;
#else
using System.Windows.Media;
using DevExpress.Xpf.Windows.Forms;
#endif
namespace DevExpress.XtraSpreadsheet.Forms {
	#region ConditionalFormattingViewModelBase (abstract class)
	public abstract class ConditionalFormattingViewModelBase : ReferenceEditViewModel {
		#region Fields
		string formText = String.Empty;
		string labelHeaderText = String.Empty;
		string labelWithText = String.Empty;
		ConditionalFormattingStyle style;
		int rank;
		#endregion
		protected ConditionalFormattingViewModelBase(ISpreadsheetControl control)
			: base(control) {
			this.Style = Styles[0];
		}
		#region Properties
		public string FormText {
			get { return formText; }
			set {
				if (formText == value)
					return;
				formText = value;
				OnPropertyChanged("FormText");
			}
		}
		public string LabelHeaderText {
			get { return labelHeaderText; }
			set {
				if (labelHeaderText == value)
					return;
				labelHeaderText = value;
				OnPropertyChanged("LabelHeaderText");
			}
		}
		public int Rank {
			get { return rank; }
			set {
				if (Rank == value)
					return;
				this.rank = value;
				OnPropertyChanged("Rank");
			}
		}
		public string LabelWithText {
			get { return labelWithText; }
			set {
				if (labelWithText == value)
					return;
				labelWithText = value;
				OnPropertyChanged("LabelWithText");
			}
		}
		public ConditionalFormattingStyle Style {
			get { return style; }
			set {
				if (Style == value)
					return;
				this.style = value;
				OnPropertyChanged("Style");
			}
		}
		public List<ConditionalFormattingStyle> Styles { get { return DefaultConditionalFormattingStyleCollection.Styles; } }
		internal CellRangeBase Range { get; set; }
		public IConditionalFormattingCommand Command { get; set; }
		#endregion
		public bool ApplyChanges() {
			if (Command != null)
				return Command.ApplyChanges(this);
			else
				return true;
		}
	}
	#endregion
	#region ConditionalFormattingTopBottomRuleViewModel
	public class ConditionalFormattingTopBottomRuleViewModel : ConditionalFormattingViewModelBase {
		public ConditionalFormattingTopBottomRuleViewModel(ISpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region ConditionalFormattingAverageRuleViewModel
	public class ConditionalFormattingAverageRuleViewModel : ConditionalFormattingViewModelBase {
		public ConditionalFormattingAverageRuleViewModel(ISpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region ConditionalFormattingStyle
	public class ConditionalFormattingStyle {
		public Color FontColor { get; set; }
		public Color FillBackColor { get; set; }
		public Color FillForeColor { get; set; }
		public Color BorderColor { get; set; }
		public XlBorderLineStyle BorderLineStyle { get; set; }
		public XlPatternType FillPatternType { get; set; }
		public string DisplayName { get; set; }
		public override string ToString() {
			return DisplayName;
		}
	}
	#endregion   
}
