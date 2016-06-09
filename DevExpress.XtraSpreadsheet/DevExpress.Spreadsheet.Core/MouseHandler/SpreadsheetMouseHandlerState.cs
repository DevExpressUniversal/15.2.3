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

using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Mouse {
	#region SpreadsheetMouseHandlerState (abstract class)
	public abstract class SpreadsheetMouseHandlerState : MouseHandlerState {
		protected SpreadsheetMouseHandlerState(SpreadsheetMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		#region Properties
		public new SpreadsheetMouseHandler MouseHandler { get { return (SpreadsheetMouseHandler)base.MouseHandler; } }
		public ISpreadsheetControl Control { get { return MouseHandler.Control; } }
		public DocumentModel DocumentModel { get { return Control.InnerControl.DocumentModel; } }
		public virtual bool UseHover { get { return false; } }
		public virtual bool SuppressDefaultMouseWheelProcessing { get { return false; } }
		#endregion
		public override void OnMouseMove(MouseEventArgs e) {
			Point physicalPoint = new Point(e.X, e.Y);
			SpreadsheetHitTestResult hitTestResult = CalculatePageHitTest(physicalPoint);
			OnMouseMoveCore(e, physicalPoint, hitTestResult);
		}
		protected internal virtual SpreadsheetHitTestResult CalculatePageHitTest(Point physicalPoint) {
			return Control.InnerControl.ActiveView.CalculatePageHitTest(physicalPoint);
		}
		protected virtual void OnMouseMoveCore(MouseEventArgs e, Point physicalPoint, SpreadsheetHitTestResult hitTestResult) {
			MouseHandler.SetActiveObject(CalculateActiveObject(hitTestResult));
		}
		protected internal virtual object CalculateActiveObject(SpreadsheetHitTestResult hitTestResult) {
			if(hitTestResult == null)
				return null;
			return CalculateActiveObjectCore(hitTestResult);
		}
		protected virtual object CalculateActiveObjectCore(SpreadsheetHitTestResult hitTestResult) {
			Worksheet activeSheet = DocumentModel.ActiveSheet;
			CellPosition activeCellPosition = hitTestResult.CellPosition;
			CellKey activeCellKey = new CellKey(activeSheet.SheetId, activeCellPosition.Column, activeCellPosition.Row);
			int hyperlinkIndex = activeSheet.Hyperlinks.GetHyperlink(activeCellKey);
			if (hyperlinkIndex != Int32.MinValue)
				return activeSheet.Hyperlinks[hyperlinkIndex];
			if (hitTestResult.PictureBox == null)
				return null;
			return activeSheet.DrawingObjects[hitTestResult.PictureBox.DrawingIndex];
		}
		protected internal virtual void SetMouseCursor(SpreadsheetCursor cursor) {
			Control.Cursor = cursor.Cursor;
		}
		public Rectangle Normalize(Rectangle value) {
			if (value.Width < 0 || value.Height < 0) {
				int top = Math.Min(value.Top, value.Bottom);
				int bottom = Math.Max(value.Top, value.Bottom);
				int left = Math.Min(value.Left, value.Right);
				int right = Math.Max(value.Left, value.Right);
				return Rectangle.FromLTRB(left, top, right, bottom);
			}
			else
				return value;
		}
	}
	#endregion
}
