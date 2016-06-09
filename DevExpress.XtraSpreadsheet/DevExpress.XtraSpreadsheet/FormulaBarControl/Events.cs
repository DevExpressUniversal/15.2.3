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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet {
	public partial class SpreadsheetFormulaBarControl {
		#region KeyDown
		KeyEventHandler onKeyDown;
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetFormulaBarControlKeyDown")]
#endif
		public new event KeyEventHandler KeyDown { add { onKeyDown += value; } remove { onKeyDown -= value; } }
		protected internal virtual void RaiseKeyDown(Keys keyData) {
			if (onKeyDown != null) {
				KeyEventArgs args = new KeyEventArgs(keyData);
				onKeyDown(this, args);
			}
		}
		#endregion
		#region CancelButtonClick
		EventHandler onCancelButtonClick;
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetFormulaBarControlCancelButtonClick")]
#endif
		public event EventHandler CancelButtonClick { add { onCancelButtonClick += value; } remove { onCancelButtonClick -= value; } }
		protected internal virtual void RaiseCancelButtonClick() {
			if (onCancelButtonClick != null)
				onCancelButtonClick(this, EventArgs.Empty);
		}
		#endregion
		#region ButtonOkClick
		EventHandler onOkButtonClick;
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetFormulaBarControlOkButtonClick")]
#endif
		public event EventHandler OkButtonClick { add { onOkButtonClick += value; } remove { onOkButtonClick -= value; } }
		protected internal virtual void RaiseOkButtonClick() {
			if (onOkButtonClick != null)
				onOkButtonClick(this, EventArgs.Empty);
		}
		#endregion
		#region InsertFunctionButtonClick
		EventHandler onInsertFunctionButtonClick;
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetFormulaBarControlInsertFunctionButtonClick")]
#endif
		public event EventHandler InsertFunctionButtonClick { add { onInsertFunctionButtonClick += value; } remove { onInsertFunctionButtonClick -= value; } }
		protected internal virtual void RaiseInsertFunctionButtonClick() {
			if (onInsertFunctionButtonClick != null)
				onInsertFunctionButtonClick(this, EventArgs.Empty);
		}
		#endregion
		#region Rollback
		EventHandler onRollback;
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetFormulaBarControlRollback")]
#endif
		public event EventHandler Rollback { add { onRollback += value; } remove { onRollback -= value; } }
		protected internal virtual void RaiseRollback() {
			if (onRollback != null)
				onRollback(this, EventArgs.Empty);
		}
		#endregion
		#region BeforeEnter
		EventHandler onBeforeEnter;
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetFormulaBarControlBeforeEnter")]
#endif
		public event EventHandler BeforeEnter { add { onBeforeEnter += value; } remove { onBeforeEnter -= value; } }
		void RaiseBeforeEnter(EventArgs args) {
			if (onBeforeEnter != null)
				onBeforeEnter(this, args);
		}
		#endregion
	}
}
