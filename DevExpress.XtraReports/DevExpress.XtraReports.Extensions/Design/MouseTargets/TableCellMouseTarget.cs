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
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraReports.UI;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Design.MouseTargets {
	class TableCellMouseTarget : TextControlMouseTarget {
		XRTableCell Cell { get { return XRControl as XRTableCell; } }
		XRTableRow Row { get { return Cell.Row; } }
		XRTable Table { get { return Row != null ? Row.Table : null; } }
		public TableCellMouseTarget(XRControl xrControl, IServiceProvider servProvider)
			: base(xrControl, servProvider) {
		}
		public override void HandleMouseDown(object sender, BandMouseEventArgs e) {
			if(XRControlDesignerBase.CursorEquals(XRCursors.LeftArrow, XRCursors.RightArrow))
				Designer.SelectComponents(new object[] { Cell.Parent }, ControlConstants.SelectionTypeAuto);
			else if(XRControlDesignerBase.CursorEquals(XRCursors.UpArrow, XRCursors.DownArrow))
				Designer.SelectComponents(((XRTableCellDesigner)Designer).GetColumnCells(Cell), ControlConstants.SelectionTypeAuto);
			else 
				base.HandleMouseDown(sender, e);
		}
		public override void HandleMouseUp(object sender, BandMouseEventArgs e) {
			if(!XRControlDesignerBase.CursorEquals(XRCursors.LeftArrow, XRCursors.RightArrow, XRCursors.UpArrow, XRCursors.DownArrow))
				base.HandleMouseUp(sender, e);
		}
		protected override Cursor GetCursor(Point pt) {
			int delta = 5;
			RectangleF rect = BandViewSvc.GetControlViewClientBounds(XRControl);
			if(XRControlDesignerBase.IsLastChild(Cell) && pt.X > rect.Right - delta)
				return XRCursors.LeftArrow;
			else if(XRControlDesignerBase.IsFirstChild(Cell) && pt.X < rect.Left + delta)
				return XRCursors.RightArrow;
			else if(XRControlDesignerBase.IsFirstChild(Row) && pt.Y < rect.Top + delta)
				return XRCursors.DownArrow;
			else if(XRControlDesignerBase.IsLastChild(Row) && pt.Y > rect.Bottom - delta)
				return XRCursors.UpArrow;
			return base.GetCursor(pt);
		}
	}
}
