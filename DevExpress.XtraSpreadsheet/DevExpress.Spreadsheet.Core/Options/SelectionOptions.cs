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
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet {
	#region ShowSelectionMode
	public enum ShowSelectionMode {
		Always,
		Focused,
	}
	#endregion
	#region MoveActiveCellModeOnEnterPress
	public enum MoveActiveCellModeOnEnterPress {
		None,
		Down,
		Right,
		Up,
		Left
	}
	#endregion
	#region SpreadsheetSelectionOptions
	[ComVisible(true)]
	public class SpreadsheetSelectionOptions : SpreadsheetNotificationOptions {
		#region Fields
		ShowSelectionMode showSelectionMode;
		MoveActiveCellModeOnEnterPress moveActiveCellMode;
		bool allowMultiSelection;
		bool allowExtendSelection;
		bool hideSelection;
		#endregion
		#region Properties
		#region ShowSelectionMode
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetSelectionOptionsShowSelectionMode"),
#endif
		DefaultValue(ShowSelectionMode.Always), NotifyParentProperty(true)]
		public ShowSelectionMode ShowSelectionMode {
			get { return showSelectionMode; }
			set {
				if (this.ShowSelectionMode == value)
					return;
				ShowSelectionMode oldValue = this.ShowSelectionMode;
				this.showSelectionMode = value;
				OnChanged("ShowSelectionMode", oldValue, value);
			}
		}
		#endregion
		#region AllowMultiSelection
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetSelectionOptionsAllowMultiSelection"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowMultiSelection {
			get { return allowMultiSelection; }
			set {
				if (this.AllowMultiSelection == value)
					return;
				this.allowMultiSelection = value;
				OnChanged("AllowMultiSelection", !value, value);
			}
		}
		#endregion
		#region AllowExtendSelection
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetSelectionOptionsAllowExtendSelection"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowExtendSelection {
			get { return allowExtendSelection; }
			set {
				if (this.AllowExtendSelection == value)
					return;
				this.allowExtendSelection = value;
				OnChanged("AllowExtendSelection", !value, value);
			}
		}
		#endregion
		#region MoveActiveCellMode
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetSelectionOptionsMoveActiveCellMode"),
#endif
		DefaultValue(MoveActiveCellModeOnEnterPress.Down), NotifyParentProperty(true)]
		public MoveActiveCellModeOnEnterPress MoveActiveCellMode {
			get { return moveActiveCellMode; }
			set {
				if (this.MoveActiveCellMode == value)
					return;
				MoveActiveCellModeOnEnterPress oldValue = this.MoveActiveCellMode;
				this.moveActiveCellMode = value;
				OnChanged("MoveActiveCellMode", oldValue, value);
			}
		}
		#endregion
		#region HideSelection
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetSelectionOptionsHideSelection"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public bool HideSelection {
			get { return hideSelection; }
			set {
				if (this.hideSelection == value)
					return;
				bool oldValue = this.hideSelection;
				this.hideSelection = value;
				OnChanged("HideSelection", oldValue, value);
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			this.ShowSelectionMode = ShowSelectionMode.Always;
			this.MoveActiveCellMode = MoveActiveCellModeOnEnterPress.Down;
			this.AllowMultiSelection = true;
			this.AllowExtendSelection = true;
			this.HideSelection = false;
		}
		protected internal void CopyFrom(SpreadsheetSelectionOptions value) {
			this.showSelectionMode = value.ShowSelectionMode;
			this.moveActiveCellMode = value.MoveActiveCellMode;
			this.allowMultiSelection = value.AllowMultiSelection;
			this.allowExtendSelection = value.AllowExtendSelection;
			this.hideSelection = value.HideSelection;
		}
	}
	#endregion
}
