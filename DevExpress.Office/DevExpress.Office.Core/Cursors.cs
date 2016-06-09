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
using System.Windows.Forms;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.Office.Utils {
	#region OfficeCursor
	public abstract class OfficeCursor {
		readonly Cursor cursor;
		protected OfficeCursor(Cursor cursor) {
			this.cursor = cursor;
		}
		public Cursor Cursor { get { return cursor; } }
	}
	#endregion
	public static class OfficeCursors {
		#region Fields
		const string cursorsPath = "DevExpress.Office.Cursors.";
		static readonly Cursor iBeamItalic = CreateIBeamItalicCursor();
		static readonly Cursor beginRotate = CreateBeginRotateCursor();
		static readonly Cursor rotate = CreateRotateCursor();
		static readonly Cursor selectColumn = CreateSelectColumnCursor();
		static readonly Cursor resizeColumn = CreateResizeColumnCursor();
		static readonly Cursor resizeRow = CreateResizeRowCursor();
		#endregion
		#region Properites
		public static Cursor IBeamItalic { get { return iBeamItalic; } }
		public static Cursor BeginRotate { get { return beginRotate; } }
		public static Cursor Rotate { get { return rotate; } }
		public static Cursor SelectColumn { get { return selectColumn; } }
		public static Cursor ResizeColumn { get { return resizeColumn; } }
		public static Cursor ResizeRow { get { return resizeRow; } }
		#endregion
		static Cursor CreateIBeamItalicCursor() {
			return LoadCursor("IBeamItalic.cur");
		}
		static Cursor CreateBeginRotateCursor() {
			return LoadCursor("BeginRotate.cur");
		}
		static Cursor CreateRotateCursor() {
			return LoadCursor("Rotate.cur");
		}
		static Cursor CreateSelectColumnCursor() {
			return LoadCursor("SelectColumn.cur");
		}
		static Cursor CreateResizeColumnCursor() {
			return LoadCursor("ResizeColumn.cur");
		}
		static Cursor CreateResizeRowCursor() {
			return LoadCursor("ResizeRow.cur");
		}
		static Cursor LoadCursor(string resourceName) {
#if DXPORTABLE
			return new Cursor(resourceName);
#else
			return ResourceImageHelper.CreateCursorFromResources(cursorsPath + resourceName, Assembly.GetExecutingAssembly());
#endif
		}
	}
}
