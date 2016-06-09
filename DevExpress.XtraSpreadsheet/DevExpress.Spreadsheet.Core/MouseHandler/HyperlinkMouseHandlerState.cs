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
using System.Drawing;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Compatibility.System.Drawing;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Mouse {
	public class HyperlinkMouseHandlerState : ContinueSelectionByCellsMouseHandlerState {
		const int timeAfterMouseDown = 1000;
		delegate void SelectRangeDelegate(CellPosition cellPosition);
		Timer timer;
		bool isTimerTick;
		public HyperlinkMouseHandlerState(SpreadsheetMouseHandler mouseHandler)
			: base(mouseHandler) {
			InitializeTimer();
		}
		#region Properties
		public override bool StopClickTimerOnStart { get { return true; } }
		#endregion
		void InitializeTimer() {
			timer = new Timer();
#if !SL
			timer.Interval = timeAfterMouseDown;
#else
#endif
			timer.Tick += OnClickTimerTick;
			timer.Start();
		}
		void OnClickTimerTick(object sender, EventArgs e) {
			isTimerTick = true;
			timer.Stop();
			HandleClickTimerTick();
		}
		protected internal virtual void HandleClickTimerTick() {
			SetMouseCursor(SpreadsheetCursors.Default);
		}
		public override void OnMouseUp(MouseEventArgs e) {
			timer.Stop();
			if((e.Button & MouseButtons.Left) == 0)
				return;
			Point physicalPoint = new Point(e.X, e.Y);
			HandleMouseUp(physicalPoint);
			MouseHandler.SwitchToDefaultState();
		}
		protected internal virtual void HandleMouseUp(Point physicalPoint) {
			SpreadsheetHitTestResult hitTestResult = CalculateHitTest(physicalPoint);
			if(hitTestResult != null)
				HandleMouseUp(hitTestResult);
		}
		protected internal virtual void HandleMouseUp(SpreadsheetHitTestResult hitTestResult) {
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(DocumentModel.ActiveSheet, Control.InnerControl);
			SheetViewSelection selection = Control.InnerControl.DocumentModel.ActiveSheet.Selection;
			if(selectionManager.IsHyperlinkActive(hitTestResult) && !isTimerTick && (selection.IsSingleCell || selection.ActiveRange.IsMerged))
				TryProcessHyperlinkClick();
		}
		protected internal override SpreadsheetHitTestResult CalculateHitTest(Point point) {
			return Control.InnerControl.ActiveView.CalculatePageHitTest(point);
		}
		protected internal void TryProcessHyperlinkClick() {
			HyperlinkMouseClickHandler handler = new HyperlinkMouseClickHandler(Control);
			handler.TryProcessHyperlinkClick();
		}
	}
}
