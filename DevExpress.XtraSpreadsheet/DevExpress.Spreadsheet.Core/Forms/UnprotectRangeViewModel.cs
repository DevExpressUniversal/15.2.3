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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region UnprotectRangeViewModel
	public abstract class UnprotectRangeViewModel : ViewModelBase {
		readonly ISpreadsheetControl control;
		readonly ModelProtectedRangeCollection protectedRanges;
		string password = String.Empty;
		protected UnprotectRangeViewModel(ISpreadsheetControl control, ModelProtectedRangeCollection protectedRanges) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(control, "protectedRanges");
			this.control = control;
			this.protectedRanges = protectedRanges;
		}
		#region Properties
		#region Password
		public string Password {
			get { return password; }
			set {
				if (Password == value)
					return;
				this.password = value;
				OnPropertyChanged("Password");
			}
		}
		#endregion
		protected ModelProtectedRangeCollection ProtectedRanges { get { return protectedRanges; } }
		public ISpreadsheetControl Control { get { return control; } }
		#endregion
		public void ApplyChanges() {
			IList<ModelProtectedRange> activeRanges = ObtainProtectedRanges();
			bool isAccessGranted = false;
			foreach (ModelProtectedRange range in activeRanges) {
				if (range.Credentials.CheckPassword(Password)) {
					isAccessGranted = true;
					break;
				}
			}
			if (!isAccessGranted) {
				this.Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_IncorrectPassword));
				return;
			}
			foreach (ModelProtectedRange range in ProtectedRanges) {
				if (range.Credentials.CheckPassword(Password))
					range.IsAccessGranted = true;
			}
		}
		protected abstract IList<ModelProtectedRange> ObtainProtectedRanges();
	}
	#endregion
	#region UnprotectRangeViewModelActiveCell
	public class UnprotectRangeViewModelActiveCell : UnprotectRangeViewModel {
		readonly CellPosition activeCell;
		public UnprotectRangeViewModelActiveCell(ISpreadsheetControl control, ModelProtectedRangeCollection protectedRanges, CellPosition activeCell) 
			:base(control, protectedRanges) {
			this.activeCell = activeCell;
		}
		#region Properties
		CellPosition ActiveCell { get { return activeCell; } }
		#endregion
		protected override IList<ModelProtectedRange> ObtainProtectedRanges() {
			return ProtectedRanges.LookupProtectedRanges(ActiveCell);
		}
	}
	#endregion
	#region UnprotectRangeViewModelTargetRange
	public class UnprotectRangeViewModelTargetRange : UnprotectRangeViewModel {
		readonly CellRange targetRange;
		public UnprotectRangeViewModelTargetRange(ISpreadsheetControl control, ModelProtectedRangeCollection protectedRanges, CellRange targetRange)
			: base(control, protectedRanges) {
			this.targetRange = targetRange;
		}
		#region Properties
		CellRange TargetRange { get { return targetRange; } }
		#endregion
		protected override IList<ModelProtectedRange> ObtainProtectedRanges() {
			return ProtectedRanges.LookupProtectedRangesContainingEntireRange(TargetRange);
		}
	}
	#endregion
}
