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
using System.Windows;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Data;
#if !SPREADSHEET
using DevExpress.XtraRichEdit.Utils;
#endif
#if SL
using PlatformIndependentColor = System.Windows.Media.Color;
using PlatformColor = System.Windows.Media.Color;
using PlatformIndependentIDataObject = System.Windows.IDataObject;
using PlatformIDataObject = System.Windows.IDataObject;
using PlatformIndependentCursor = System.Windows.Input.Cursor;
using PlatformCursor = System.Windows.Input.Cursor;
using PlatformIndependentCursors = System.Windows.Input.Cursors;
using PlatformCursors = System.Windows.Input.Cursors;
using Padding = DevExpress.Xpf.Core.Native.Padding;
using PlatformIndependentBrush = System.Windows.Media.Brush;
using PlatformBrush = System.Windows.Media.Brush;
#else
using PlatformIndependentColor = System.Drawing.Color;
using PlatformColor = System.Windows.Media.Color;
using PlatformIndependentIDataObject = System.Windows.Forms.IDataObject;
using PlatformIDataObject = System.Windows.IDataObject;
using PlatformIndependentCursor = System.Windows.Forms.Cursor;
using PlatformCursor = System.Windows.Input.Cursor;
using PlatformIndependentCursors = System.Windows.Forms.Cursors;
using PlatformCursors = System.Windows.Input.Cursors;
using Padding = System.Windows.Forms.Padding;
using PlatformIndependentBrush = System.Drawing.Brush;
using PlatformBrush = System.Windows.Media.Brush;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using System.Windows.Media;
using System.Windows.Input;
#endif
namespace DevExpress.Office.Internal {
	public static class XpfTypeConverter {
#if SL
		static readonly PlatformIndependentCursor Default = PlatformIndependentCursors.Arrow;
#else
		static readonly PlatformIndependentCursor Default = PlatformIndependentCursors.Default;
#endif
		static readonly Dictionary<PlatformCursor, PlatformIndependentCursor> cursorTable = CreateCursorTable();
		static Dictionary<PlatformCursor, PlatformIndependentCursor> CreateCursorTable() {
			Dictionary<PlatformCursor, PlatformIndependentCursor> result = new Dictionary<PlatformCursor, PlatformIndependentCursor>();
			result.Add(PlatformCursors.Arrow, PlatformIndependentCursors.Arrow);
			result.Add(PlatformCursors.Hand, PlatformIndependentCursors.Hand);
			result.Add(PlatformCursors.IBeam, PlatformIndependentCursors.IBeam);
			result.Add(PlatformCursors.SizeNESW, PlatformIndependentCursors.SizeNESW);
			result.Add(PlatformCursors.SizeNS, PlatformIndependentCursors.SizeNS);
			result.Add(PlatformCursors.SizeNWSE, PlatformIndependentCursors.SizeNWSE);
			result.Add(PlatformCursors.SizeWE, PlatformIndependentCursors.SizeWE);
#if !SL
			result.Add(PlatformCursors.AppStarting, PlatformIndependentCursors.AppStarting);
			result.Add(PlatformCursors.ArrowCD, PlatformIndependentCursors.Arrow);
			result.Add(PlatformCursors.Cross, PlatformIndependentCursors.Cross);
			result.Add(PlatformCursors.Help, PlatformIndependentCursors.Help);
			result.Add(PlatformCursors.No, PlatformIndependentCursors.No);
			result.Add(PlatformCursors.ScrollE, PlatformIndependentCursors.PanEast);
			result.Add(PlatformCursors.ScrollNE, PlatformIndependentCursors.PanNE);
			result.Add(PlatformCursors.ScrollN, PlatformIndependentCursors.PanNorth);
			result.Add(PlatformCursors.ScrollNW, PlatformIndependentCursors.PanNW);
			result.Add(PlatformCursors.ScrollSE, PlatformIndependentCursors.PanSE);
			result.Add(PlatformCursors.ScrollS, PlatformIndependentCursors.PanSouth);
			result.Add(PlatformCursors.ScrollSW, PlatformIndependentCursors.PanSW);
			result.Add(PlatformCursors.ScrollW, PlatformIndependentCursors.PanWest);
			result.Add(PlatformCursors.SizeAll, PlatformIndependentCursors.SizeAll);
			result.Add(PlatformCursors.UpArrow, PlatformIndependentCursors.UpArrow);
			result.Add(PlatformCursors.Wait, PlatformIndependentCursors.WaitCursor);
#if !SPREADSHEET
			result.Add(CreateBeginRotateCursor(), RichEditCursors.BeginRotate.Cursor);
			result.Add(CreateRotateCursor(), RichEditCursors.Rotate.Cursor);
			result.Add(CreateIBeamItalicCursor(), RichEditCursors.IBeamItalic.Cursor);
			result.Add(CreateSelectColumnCursor(), RichEditCursors.SelectTableColumn.Cursor);
			result.Add(CreateResizeColumnCursor(), RichEditCursors.ResizeTableColumn.Cursor);
			result.Add(CreateResizeRowCursor(), RichEditCursors.ResizeTableRow.Cursor);
#endif
#else
			result.Add(PlatformCursors.Wait, PlatformIndependentCursors.Wait);
#endif
			return result;
		}
#if !SL
		const string cursorsPath = "DevExpress.Office.Cursors.";
		static PlatformCursor CreateCursorFromResources(string resourcePath, Assembly asm) {
			return new PlatformCursor(asm.GetManifestResourceStream(resourcePath));
		}
		static PlatformCursor CreateBeginRotateCursor() {
			return CreateCursorFromResources(cursorsPath + "BeginRotate.cur", typeof(DevExpress.Office.Utils.OfficeCursor).Assembly);
		}
		static PlatformCursor CreateRotateCursor() {
			return CreateCursorFromResources(cursorsPath + "Rotate.cur", typeof(DevExpress.Office.Utils.OfficeCursor).Assembly);
		}
		static PlatformCursor CreateIBeamItalicCursor() {
			return CreateCursorFromResources(cursorsPath + "IBeamItalic.cur", typeof(DevExpress.Office.Utils.OfficeCursor).Assembly);
		}
		static PlatformCursor CreateSelectColumnCursor() {
			return CreateCursorFromResources(cursorsPath + "SelectColumn.cur", typeof(DevExpress.Office.Utils.OfficeCursor).Assembly);
		}
		static PlatformCursor CreateResizeColumnCursor() {
			return CreateCursorFromResources(cursorsPath + "ResizeColumn.cur", typeof(DevExpress.Office.Utils.OfficeCursor).Assembly);
		}
		static PlatformCursor CreateResizeRowCursor() {
			return CreateCursorFromResources(cursorsPath + "ResizeRow.cur", typeof(DevExpress.Office.Utils.OfficeCursor).Assembly);
		}
#endif
		public static PlatformColor ToPlatformColor(PlatformIndependentColor color) {
#if SL
			return color;
#else
			return PlatformColor.FromArgb(color.A, color.R, color.G, color.B);
#endif
		}
		public static PlatformIndependentColor ToPlatformIndependentColor(PlatformColor color) {
#if SL
			return color;
#else
			return PlatformIndependentColor.FromArgb(color.A, color.R, color.G, color.B);
#endif
		}
		public static PlatformIndependentCursor ToPlatformIndependentCursor(PlatformCursor cursor) {
			if (cursor == null)
				return Default;
			PlatformIndependentCursor result;
			if (cursorTable.TryGetValue(cursor, out result))
				return result;
			else
				return Default;
		}
		public static PlatformCursor ToPlatformCursor(PlatformIndependentCursor cursor) {
			if (cursor == null)
				return PlatformCursors.Arrow;
			foreach (PlatformCursor key in cursorTable.Keys) {
				if (cursorTable[key] == cursor)
					return key;
			}
			return PlatformCursors.Arrow;
		}
		public static PlatformBrush ToPlatformBrush(PlatformIndependentBrush brush) {
#if SL
			return brush;
#else
			System.Drawing.SolidBrush solidBrush = brush as System.Drawing.SolidBrush;
			if (solidBrush == null)
				return System.Windows.Media.Brushes.Black;
			else
				return new System.Windows.Media.SolidColorBrush(ToPlatformColor(solidBrush.Color));
#endif
		}
		public static System.Windows.Point ToPlatformPoint(System.Drawing.Point point) {
			return new System.Windows.Point(point.X, point.Y);
		}
		public static System.Windows.Point ToPlatformPoint(System.Windows.Point point) {
			return point;
		}
	}
	public class XpfDataObject : PlatformIndependentIDataObject {
		readonly PlatformIDataObject dataObject;
		public XpfDataObject(PlatformIDataObject dataObject) {
			Guard.ArgumentNotNull(dataObject, "dataObject");
			this.dataObject = dataObject;
		}
		#region PlatformIndependentIDataObject Members
		object PlatformIndependentIDataObject.GetData(string format, bool autoConvert) {
			return dataObject.GetData(format, autoConvert);
		}
		object PlatformIndependentIDataObject.GetData(Type format) {
			return dataObject.GetData(format);
		}
		object PlatformIndependentIDataObject.GetData(string format) {
			return dataObject.GetData(format);
		}
		bool PlatformIndependentIDataObject.GetDataPresent(string format, bool autoConvert) {
			return dataObject.GetDataPresent(format, autoConvert);
		}
		bool PlatformIndependentIDataObject.GetDataPresent(Type format) {
			return dataObject.GetDataPresent(format);
		}
		bool PlatformIndependentIDataObject.GetDataPresent(string format) {
			return dataObject.GetDataPresent(format);
		}
		string[] PlatformIndependentIDataObject.GetFormats(bool autoConvert) {
			return dataObject.GetFormats(autoConvert);
		}
		string[] PlatformIndependentIDataObject.GetFormats() {
			return dataObject.GetFormats();
		}
#if SL
		void PlatformIndependentIDataObject.SetData(string format, object data, bool autoConvert) {
			dataObject.SetData(format, data, autoConvert);
		}
#else
		void PlatformIndependentIDataObject.SetData(string format, bool autoConvert, object data) {
			dataObject.SetData(format, data, autoConvert);
		}
#endif
		void PlatformIndependentIDataObject.SetData(Type format, object data) {
			dataObject.SetData(format, data);
		}
		void PlatformIndependentIDataObject.SetData(string format, object data) {
			dataObject.SetData(format, data);
		}
		void PlatformIndependentIDataObject.SetData(object data) {
			dataObject.SetData(data);
		}
		#endregion
	}
	public static class FrameworkElementExtensionsSafe {
		public static Point GetPositionSafe(this FrameworkElement element) {
			return element.GetPositionSafe(element.GetParent() as FrameworkElement);
		}
		public static Point GetPositionSafe(this FrameworkElement element, FrameworkElement relativeTo) {
			try {
				return element.MapPoint(new Point(0, 0), relativeTo);
			}
			catch {
				return new Point(0, 0);
			}
		}
	}
	public static class PaddingExtensions {
		public static Thickness ToThickness(this Padding padding) {
			return new Thickness(padding.Left, padding.Top, padding.Right, padding.Bottom);
		}
	}
}
