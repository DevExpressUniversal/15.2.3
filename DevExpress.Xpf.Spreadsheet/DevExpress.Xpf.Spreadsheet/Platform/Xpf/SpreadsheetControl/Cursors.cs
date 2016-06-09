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
using System.Windows.Input;
using PlatformIndependentCursor = System.Windows.Forms.Cursor;
using PlatformIndependentCursors = System.Windows.Forms.Cursors;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.XtraSpreadsheet.Utils;
using Microsoft.Win32.SafeHandles;
using System.Security.Permissions;
using System.Security;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public static class CursorsProvider {
		static Dictionary<PlatformIndependentCursor, Cursor> cursorsTable;
		static CursorsProvider() {
			PopulateCursorsTable();
		}
		[SecuritySafeCritical]
		public static Cursor GetCursor(PlatformIndependentCursor cursor) {
			if (cursorsTable.ContainsKey(cursor)) return cursorsTable[cursor];
			return Cursors.Arrow;
		}
		public static PlatformIndependentCursor GetPlatformIndependentCursor(Cursor cursor) {
			foreach (PlatformIndependentCursor key in cursorsTable.Keys) {
				if (cursorsTable[key] == cursor) return key;
			}
			return PlatformIndependentCursors.Default;
		}
		private static void PopulateCursorsTable() {
			cursorsTable = new Dictionary<PlatformIndependentCursor, Cursor>();
			cursorsTable.Add(PlatformIndependentCursors.Default, Cursors.Arrow);
			cursorsTable.Add(PlatformIndependentCursors.Hand, Cursors.Hand);
			cursorsTable.Add(PlatformIndependentCursors.SizeNESW, Cursors.SizeNESW);
			cursorsTable.Add(PlatformIndependentCursors.SizeNS, Cursors.SizeNS);
			cursorsTable.Add(PlatformIndependentCursors.SizeNWSE, Cursors.SizeNWSE);
			cursorsTable.Add(PlatformIndependentCursors.SizeWE, Cursors.SizeWE);
			cursorsTable.Add(PlatformIndependentCursors.AppStarting, Cursors.AppStarting);
			cursorsTable.Add(PlatformIndependentCursors.Cross, Cursors.Cross);
			cursorsTable.Add(PlatformIndependentCursors.Help, Cursors.Help);
			cursorsTable.Add(PlatformIndependentCursors.No, Cursors.No);
			cursorsTable.Add(PlatformIndependentCursors.PanEast, Cursors.ScrollE);
			cursorsTable.Add(PlatformIndependentCursors.PanNE, Cursors.ScrollNE);
			cursorsTable.Add(PlatformIndependentCursors.PanNorth, Cursors.ScrollN);
			cursorsTable.Add(PlatformIndependentCursors.PanNW, Cursors.ScrollNW);
			cursorsTable.Add(PlatformIndependentCursors.PanSE, Cursors.ScrollSE);
			cursorsTable.Add(PlatformIndependentCursors.PanSouth, Cursors.ScrollS);
			cursorsTable.Add(PlatformIndependentCursors.PanSW, Cursors.ScrollSW);
			cursorsTable.Add(PlatformIndependentCursors.PanWest, Cursors.ScrollW);
			cursorsTable.Add(PlatformIndependentCursors.SizeAll, Cursors.SizeAll);
			cursorsTable.Add(PlatformIndependentCursors.UpArrow, Cursors.UpArrow);
			cursorsTable.Add(PlatformIndependentCursors.WaitCursor, Cursors.Wait);
			cursorsTable.Add(PlatformIndependentCursors.IBeam, CreateIBeamCursor());
			cursorsTable.Add(SpreadsheetCursors.ResizeRow.Cursor, CreateResizeRowCursor());
			cursorsTable.Add(SpreadsheetCursors.ResizeColumn.Cursor, CreateResizeColumnCursor());
			cursorsTable.Add(SpreadsheetCursors.BeginRotate.Cursor, CreateBeginRotateCursor());
			cursorsTable.Add(SpreadsheetCursors.Rotate.Cursor, CreateRotateCursor());
			cursorsTable.Add(SpreadsheetCursors.SelectColumn.Cursor, CreateSelectColumnCursor());
			cursorsTable.Add(SpreadsheetCursors.SelectRow.Cursor, CreateSelectRowCursor());
			cursorsTable.Add(SpreadsheetCursors.DragRange.Cursor, CreateDragRangeCursor());
			cursorsTable.Add(SpreadsheetCursors.ResizeNWSE.Cursor, CreateResizeNWSECursor());
			cursorsTable.Add(SpreadsheetCursors.ResizeNESW.Cursor, CreateResizeNESWCursor());
			cursorsTable.Add(SpreadsheetCursors.SmallCross.Cursor, CreateSmallCrossCursor());
			cursorsTable.Add(SpreadsheetCursors.DragItemNone.Cursor, CreateDragItemNoneCursor());
			cursorsTable.Add(SpreadsheetCursors.DragItemMove.Cursor, CreateDragItemMoveCursor());
			cursorsTable.Add(SpreadsheetCursors.DragItemRemove.Cursor, CreateDragItemRemoveCursor());
		}
		#region CreateCursors methods
		static Cursor CreateCursorFromResources(string resourcePath, Assembly asm) {
			return new Cursor(asm.GetManifestResourceStream(resourcePath));
		}
		static string officeCursorsPath = "DevExpress.Office.Cursors.";
		private static Cursor CreateIBeamCursor() {
			return CreateCursorFromResources(officeCursorsPath + "IBeamItalic.cur", GetOfficeAssembly());
		}
		private static Cursor CreateResizeRowCursor() {
			return CreateCursorFromResources(officeCursorsPath + "ResizeRow.cur", GetOfficeAssembly());
		}
		private static Cursor CreateResizeColumnCursor() {
			return CreateCursorFromResources(officeCursorsPath + "ResizeColumn.cur", GetOfficeAssembly());
		}
		private static Cursor CreateBeginRotateCursor() {
			return CreateCursorFromResources(officeCursorsPath + "BeginRotate.cur", GetOfficeAssembly());
		}
		private static Cursor CreateRotateCursor() {
			return CreateCursorFromResources(officeCursorsPath + "Rotate.cur", GetOfficeAssembly());
		}
		private static Assembly GetOfficeAssembly() {
			return typeof(DevExpress.Office.Utils.OfficeCursor).Assembly;
		}
		private static Cursor CreateSelectColumnCursor() {
			return CreateCursorFromResources(officeCursorsPath + "SelectColumn.cur", GetOfficeAssembly());
		}
		private static Cursor CreateResizeNESWCursor() {
			return CreateCursorFromResources(corePath + "ResizeNESW.cur", GetCoreAssembly());
		}
		private static Assembly GetCoreAssembly() {
			return typeof(SpreadsheetCursor).Assembly;
		}
		private static Cursor CreateResizeNWSECursor() {
			return CreateCursorFromResources(corePath + "ResizeNWSE.cur", GetCoreAssembly());
		}
		private static Cursor CreateDragRangeCursor() {
			return CreateCursorFromResources(corePath + "DragRange.cur", GetCoreAssembly());
		}
		const string corePath = "DevExpress.XtraSpreadsheet.Cursors.";
		static Cursor CreateSelectRowCursor() {
			return CreateCursorFromResources(corePath + "SelectRow.cur", GetCoreAssembly());
		}
		static Cursor CreateSmallCrossCursor() {
			return CreateCursorFromResources(corePath + "SmallCross.cur", GetCoreAssembly());
		}
		static Cursor CreateDragItemNoneCursor() {
			return CreateCursorFromResources(corePath + "DragItemNone.cur", GetCoreAssembly());
		}
		static Cursor CreateDragItemMoveCursor() {
			return CreateCursorFromResources(corePath + "DragItemMove.cur", GetCoreAssembly());
		}
		static Cursor CreateDragItemRemoveCursor() {
			return CreateCursorFromResources(corePath + "DragItemRemove.cur", GetCoreAssembly());
		}
		#endregion
	}
}
