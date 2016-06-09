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
using DevExpress.Office.Utils;
using System.Windows.Forms;
using System.Reflection;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Utils {
	#region SpreadsheetCursor
	public class SpreadsheetCursor : OfficeCursor {
		public SpreadsheetCursor(Cursor cursor)
			: base(cursor) {
		}
	}
	#endregion
	#region SpreadsheetCursors
	public static class SpreadsheetCursors {
		const string cursorsPath = "DevExpress.XtraSpreadsheet.Cursors.";
		public static SpreadsheetCursor Default { get { return defaultCursor; } }
		public static SpreadsheetCursor IBeam { get { return iBeam; } }
		public static SpreadsheetCursor IBeamItalic { get { return iBeamItalic; } }
		public static SpreadsheetCursor WaitCursor { get { return waitCursor; } }
		public static SpreadsheetCursor SizeNS { get { return sizeNS; } }
		public static SpreadsheetCursor SizeWE { get { return sizeWE; } }
		public static SpreadsheetCursor SizeNESW { get { return sizeNESW; } }
		public static SpreadsheetCursor SizeNWSE { get { return sizeNWSE; } }
		public static SpreadsheetCursor Cross { get { return cross; } }
		public static SpreadsheetCursor SmallCross { get { return smallCross; } }
		public static SpreadsheetCursor Hand { get { return hand; } }
		public static SpreadsheetCursor ResizeRow { get { return resizeRow; } }
		public static SpreadsheetCursor ResizeColumn { get { return resizeColumn; } }
		public static SpreadsheetCursor SizeAll { get { return sizeAll; } }
		public static SpreadsheetCursor BeginRotate { get { return beginRotate; } }
		public static SpreadsheetCursor Rotate { get { return rotate; } }
		public static SpreadsheetCursor SelectRow { get { return selectRow; } }
		public static SpreadsheetCursor SelectColumn { get { return selectColumn; } }
		public static SpreadsheetCursor DragRange { get { return dragRange; } }
		public static SpreadsheetCursor ResizeNWSE { get { return resizeNWSE; } }
		public static SpreadsheetCursor ResizeNESW { get { return resizeNESW; } }
		public static SpreadsheetCursor DragItemNone { get { return dragItemNone; } }
		public static SpreadsheetCursor DragItemMove { get { return dragItemMove; } }
		public static SpreadsheetCursor DragItemRemove { get { return dragItemRemove; } }
		static readonly SpreadsheetCursor defaultCursor = new SpreadsheetCursor(Cursors.Default);
		static readonly SpreadsheetCursor iBeam = new SpreadsheetCursor(Cursors.IBeam);
		static readonly SpreadsheetCursor iBeamItalic = new SpreadsheetCursor(OfficeCursors.IBeamItalic);
		static readonly SpreadsheetCursor waitCursor = new SpreadsheetCursor(Cursors.WaitCursor);
		static readonly SpreadsheetCursor sizeNS = new SpreadsheetCursor(Cursors.SizeNS);
		static readonly SpreadsheetCursor sizeWE = new SpreadsheetCursor(Cursors.SizeWE);
		static readonly SpreadsheetCursor sizeNESW = new SpreadsheetCursor(Cursors.SizeNESW);
		static readonly SpreadsheetCursor sizeNWSE = new SpreadsheetCursor(Cursors.SizeNWSE);
		static readonly SpreadsheetCursor cross = new SpreadsheetCursor(Cursors.Cross);
		static readonly SpreadsheetCursor smallCross = new SpreadsheetCursor(CreateSmallCrossCursor());
		static readonly SpreadsheetCursor hand = new SpreadsheetCursor(Cursors.Hand);
		static readonly SpreadsheetCursor resizeRow = new SpreadsheetCursor(OfficeCursors.ResizeRow);
		static readonly SpreadsheetCursor resizeColumn = new SpreadsheetCursor(OfficeCursors.ResizeColumn);
		static readonly SpreadsheetCursor sizeAll = new SpreadsheetCursor(Cursors.SizeAll);
		static readonly SpreadsheetCursor beginRotate = new SpreadsheetCursor(OfficeCursors.BeginRotate);
		static readonly SpreadsheetCursor rotate = new SpreadsheetCursor(OfficeCursors.Rotate);
		static readonly SpreadsheetCursor selectRow = new SpreadsheetCursor(CreateSelectRowCursor());
		static readonly SpreadsheetCursor selectColumn = new SpreadsheetCursor(OfficeCursors.SelectColumn);
		static readonly SpreadsheetCursor dragRange = new SpreadsheetCursor(CreateDragRangeCursor());
		static readonly SpreadsheetCursor resizeNWSE = new SpreadsheetCursor(CreateResizeNWSECursor());
		static readonly SpreadsheetCursor resizeNESW = new SpreadsheetCursor(CreateResizeNESWCursor());
		static readonly SpreadsheetCursor dragItemNone = new SpreadsheetCursor(CreateDragItemNoneCursor());
		static readonly SpreadsheetCursor dragItemMove = new SpreadsheetCursor(CreateDragItemMoveCursor());
		static readonly SpreadsheetCursor dragItemRemove = new SpreadsheetCursor(CreateDragItemRemoveCursor());
		static Cursor LoadCursor(string name) {
#if DXPORTABLE
			return new Cursor(name);
#else
			return ResourceImageHelper.CreateCursorFromResources(cursorsPath + name + ".cur", Assembly.GetExecutingAssembly());
#endif
		}
		static Cursor CreateSelectRowCursor() {
			return LoadCursor("SelectRow");
		}
		static Cursor CreateDragRangeCursor() {
			return LoadCursor("DragRange");
		}
		static Cursor CreateResizeNESWCursor() {
			return LoadCursor("ResizeNESW");
		}
		static Cursor CreateResizeNWSECursor() {
			return LoadCursor("ResizeNWSE");
		}
		static Cursor CreateSmallCrossCursor() {
			return LoadCursor("SmallCross");
		}
		static Cursor CreateDragItemNoneCursor() {
			return LoadCursor("DragItemNone");
		}
		static Cursor CreateDragItemMoveCursor() {
			return LoadCursor("DragItemMove");
		}
		static Cursor CreateDragItemRemoveCursor() {
			return LoadCursor("DragItemRemove");
		}
		public static SpreadsheetCursor GetCursor(DragDropEffects effects) {
			return new SpreadsheetCursor(DragAndDropCursors.GetCursor(effects));
		}
	}
	#endregion
}
