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
using DevExpress.Compatibility.System.Windows.Forms;
#if !SL
using System.Windows.Forms;
using System.Reflection;
#else
using System.Windows.Input;
using System.Windows;
#endif
namespace DevExpress.XtraRichEdit.Utils {
	#region RichEditCursor
	public class RichEditCursor : OfficeCursor {
		public RichEditCursor(Cursor cursor)
			: base(cursor) {
		}
#if SL
		public RichEditCursor(Cursor cursor, FrameworkElement element)
			: base(cursor, element) {
		}
#endif
	}
	#endregion
	#region RichEditCursors
	public static class RichEditCursors {
		const string cursorsPath = "DevExpress.XtraRichEdit.Cursors.";
		public static RichEditCursor Default { get { return defaultCursor; } }
		public static RichEditCursor IBeam { get { return iBeam; } }
		public static RichEditCursor IBeamItalic { get { return iBeamItalic; } }
		public static RichEditCursor SelectRow { get { return selectRow; } }
		public static RichEditCursor WaitCursor { get { return waitCursor; } }
		public static RichEditCursor SizeNS { get { return sizeNS; } }
		public static RichEditCursor SizeWE { get { return sizeWE; } }
		public static RichEditCursor SizeNESW { get { return sizeNESW; } }
		public static RichEditCursor SizeNWSE { get { return sizeNWSE; } }
		public static RichEditCursor Cross { get { return cross; } }
		public static RichEditCursor Hand { get { return hand; } }
		public static RichEditCursor SelectTableCell { get { return selectTableCell; } }
		public static RichEditCursor SelectTableColumn { get { return selectTableColumn; } }
		public static RichEditCursor ResizeTableRow { get { return resizeTableRow; } }
		public static RichEditCursor ResizeTableColumn { get { return resizeTableColumn; } }
		public static RichEditCursor SizeAll { get { return sizeAll; } }
		public static RichEditCursor BeginRotate { get { return beginRotate; } }
		public static RichEditCursor Rotate { get { return rotate; } }
#if SL
		static readonly RichEditCursor defaultCursor = new RichEditCursor(Cursors.Arrow);
		static readonly RichEditCursor iBeam = new RichEditCursor(Cursors.IBeam);
		static readonly RichEditCursor iBeamItalic = new RichEditCursor(Cursors.IBeam);
		static readonly RichEditCursor selectRow = new RichEditCursor(Cursors.Arrow);
		static readonly RichEditCursor waitCursor = new RichEditCursor(Cursors.Wait);
		static readonly RichEditCursor sizeNS = new RichEditCursor(Cursors.SizeNS);
		static readonly RichEditCursor sizeWE = new RichEditCursor(Cursors.SizeWE);
		static readonly RichEditCursor sizeNESW = new RichEditCursor(Cursors.SizeNESW); 
		static readonly RichEditCursor sizeNWSE = new RichEditCursor(Cursors.SizeNWSE); 
		static readonly RichEditCursor cross = new RichEditCursor(Cursors.Arrow); 
		static readonly RichEditCursor hand = new RichEditCursor(Cursors.Hand);
		static readonly RichEditCursor selectTableCell = new RichEditCursor(Cursors.Arrow);
		static readonly RichEditCursor selectTableColumn = new RichEditCursor(Cursors.Arrow);
		static readonly RichEditCursor resizeTableRow = new RichEditCursor(Cursors.SizeNS);
		static readonly RichEditCursor resizeTableColumn = new RichEditCursor(Cursors.SizeWE);
		static readonly RichEditCursor sizeAll = new RichEditCursor(Cursors.Arrow); 
		static readonly RichEditCursor beginRotate = new RichEditCursor(Cursors.Hand);
		static readonly RichEditCursor rotate = new RichEditCursor(Cursors.Hand);
		public static RichEditCursor GetCursor(DragDropEffects effects) {
			if (effects == DragDropEffects.None)
				return new RichEditCursor(Cursors.Wait);
			else
				return new RichEditCursor(Cursors.Arrow);
		}
#else
		static readonly RichEditCursor defaultCursor = new RichEditCursor(Cursors.Default);
		static readonly RichEditCursor iBeam = new RichEditCursor(Cursors.IBeam);
		static readonly RichEditCursor iBeamItalic = new RichEditCursor(OfficeCursors.IBeamItalic);
		static readonly RichEditCursor selectRow = new RichEditCursor(CreateSelectRowCursor());
		static readonly RichEditCursor waitCursor = new RichEditCursor(Cursors.WaitCursor);
		static readonly RichEditCursor sizeNS = new RichEditCursor(Cursors.SizeNS);
		static readonly RichEditCursor sizeWE = new RichEditCursor(Cursors.SizeWE);
		static readonly RichEditCursor sizeNESW = new RichEditCursor(Cursors.SizeNESW);
		static readonly RichEditCursor sizeNWSE = new RichEditCursor(Cursors.SizeNWSE);
		static readonly RichEditCursor cross = new RichEditCursor(Cursors.Cross);
		static readonly RichEditCursor hand = new RichEditCursor(Cursors.Hand);
		static readonly RichEditCursor selectTableCell = new RichEditCursor(CreateSelectTableCellCursor());
		static readonly RichEditCursor selectTableColumn = new RichEditCursor(OfficeCursors.SelectColumn);
		static readonly RichEditCursor resizeTableRow = new RichEditCursor(OfficeCursors.ResizeRow);
		static readonly RichEditCursor resizeTableColumn = new RichEditCursor(OfficeCursors.ResizeColumn);
		static readonly RichEditCursor sizeAll = new RichEditCursor(Cursors.SizeAll);
		static readonly RichEditCursor beginRotate = new RichEditCursor(OfficeCursors.BeginRotate);
		static readonly RichEditCursor rotate = new RichEditCursor(OfficeCursors.Rotate);
		static Cursor CreateSelectRowCursor() {
#if !DXPORTABLE
			return ResourceImageHelper.CreateCursorFromResources(cursorsPath + "ReverseArrow.cur", Assembly.GetExecutingAssembly());
#else
			return null;
#endif
		}
		static Cursor CreateSelectTableCellCursor() {
#if !DXPORTABLE
			return ResourceImageHelper.CreateCursorFromResources(cursorsPath + "SelectTableCell.cur", Assembly.GetExecutingAssembly());
#else
			return null;
#endif
		}
		public static RichEditCursor GetCursor(DragDropEffects effects) {
			return new RichEditCursor(DragAndDropCursors.GetCursor(effects));
		}
#endif
		}
	#endregion
}
