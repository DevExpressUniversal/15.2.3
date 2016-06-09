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
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Localization;
using System.ComponentModel;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region InsertTableViewModel
	public class InsertTableViewModel : ReferenceEditViewModel {
		#region Fields
		string reference = String.Empty;
		string style = String.Empty;
		bool hasHeaders;
		#endregion
		public InsertTableViewModel(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public string Reference {
			get { return reference; }
			set {
				if (Reference == value)
					return;
				this.reference = value;
				OnPropertyChanged("Reference");
			}
		}
		public string Style {
			get { return style; }
			set {
				if (Style == value)
					return;
				this.style = value;
				OnPropertyChanged("Style");
				OnPropertyChanged("FormText");
			}
		}
		public bool HasHeaders {
			get { return hasHeaders; }
			set {
				if (HasHeaders == value)
					return;
				this.hasHeaders = value;
				OnPropertyChanged("HasHeaders");
			}
		}
		public string FormText { get { return XtraSpreadsheetLocalizer.GetString(String.IsNullOrEmpty(Style) ? XtraSpreadsheetStringId.Caption_CreateTable : XtraSpreadsheetStringId.MenuCmd_FormatAsTable); } }
		#endregion
		public bool Validate() {
			return CreateCommand().Validate(this);
		}
		public void ApplyChanges() {
			CreateCommand().ApplyChanges(this);
		}
		protected internal virtual InsertTableUICommand CreateCommand() {
			return new InsertTableUICommand(Control);
		}
	}
	#endregion
	#region FormatAsTableViewModel
	public class FormatAsTableViewModel : InsertTableViewModel {
		public FormatAsTableViewModel(ISpreadsheetControl control)
			: base(control) {
		}
		protected internal override InsertTableUICommand CreateCommand() {
			return new FormatAsTableCommand(Control);
		}
	}
	#endregion
}
