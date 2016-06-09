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
using System.Drawing;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using System.Drawing.Printing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Printing;
namespace DevExpress.XtraExport {
	public interface IExportProvider : IDisposable {
		void Commit();
		int RegisterStyle(ExportCacheCellStyle style);
		void SetDefaultStyle(ExportCacheCellStyle style);
		void SetStyle(ExportCacheCellStyle style);
		void SetStyle(int styleIndex);
		void SetCellStyle(int col, int row, int styleIndex);
		void SetCellStyle(int col, int row, ExportCacheCellStyle style);
		void SetCellStyle(int col, int row, int exampleCol, int exampleRow);
		void SetCellUnion(int col, int row, int width, int height);
		void SetCellStyleAndUnion(int col, int row, int width, int height, int styleIndex);
		void SetCellStyleAndUnion(int col, int row, int width, int height, ExportCacheCellStyle style);
		void SetRange(int width, int height, bool isVisible);
		void SetColumnWidth(int col, int width);
		void SetRowHeight(int row, int height);
		void SetCellData(int col, int row, object data);
		void SetCellString(int col, int row, string str);
		void SetPageSettings(MarginsF margins, PaperKind paperKind, bool landscape);
		ExportCacheCellStyle GetStyle(int styleIndex);
		ExportCacheCellStyle GetCellStyle(int col, int row);
		ExportCacheCellStyle GetDefaultStyle();
		int GetColumnWidth(int col);
		int GetRowHeight(int row);
		IExportProvider Clone(string fileName, Stream stream);
		bool IsStreamMode { get; }
		Stream Stream { get; }
		event ProviderProgressEventHandler ProviderProgress;
	}
	public interface IExportInternalProvider {
		void CommitCache(StreamWriter writer);
		void SetCacheToCell(int col, int row, IExportInternalProvider cache);
		void DeleteCacheFromCell(int col, int row);
	}
	public enum ExportCacheDataType {
		Boolean,
		Integer,
		Double,
		Decimal,
		String,
		Object,
		Single
	}
	public enum BrushStyle {
		Clear,
		Solid
	}
	public struct ExportCacheItem {
		public IExportInternalProvider InternalCache;
		public object Data;
		public ExportCacheDataType DataType;
		public int StyleIndex;
		public bool IsUnion;
		public bool IsHidden;
		public int UnionWidth;
		public int UnionHeight;
	}
	public struct ExportCacheCellStyle {
		Dictionary<ushort, int> types;
		Dictionary<ushort, int> Types {
			get {
				if(types == null) types = new Dictionary<ushort, int>();
				return types;
			}
		}
		public Color TextColor;
		public Font TextFont;
		public StringAlignment TextAlignment;
		public StringAlignment LineAlignment;
		public String FormatString;
		public String XlsxFormatString;
		public Color BkColor;
		public Color FgColor;
		public BrushStyle BrushStyle_;
		public ExportCacheCellBorderStyle LeftBorder;
		public ExportCacheCellBorderStyle TopBorder;
		public ExportCacheCellBorderStyle RightBorder;
		public ExportCacheCellBorderStyle BottomBorder;
		public short PreparedCellType;
		public bool WrapText;
		public bool RightToLeft;
		public bool IsEqual(ExportCacheCellStyle style) {
			return style.BkColor == BkColor
				&& style.FgColor == FgColor
				&& style.TextColor == TextColor
				&& TextFont.Equals(style.TextFont)
				&& style.BrushStyle_ == BrushStyle_
				&& LeftBorder.IsEqual(style.LeftBorder)
				&& TopBorder.IsEqual(style.TopBorder)
				&& RightBorder.IsEqual(style.RightBorder)
				&& BottomBorder.IsEqual(style.BottomBorder)
				&& style.TextAlignment == TextAlignment
				&& style.LineAlignment == LineAlignment
				&& style.FormatString == FormatString
				&& style.XlsxFormatString == XlsxFormatString
				&& style.PreparedCellType == PreparedCellType
				&& style.WrapText == WrapText
				&& style.RightToLeft == RightToLeft;
		}
		public override bool Equals(object obj) {
			if(obj is ExportCacheCellStyle) {
				return IsEqual((ExportCacheCellStyle)obj);
			}
			return false;
		}
		public override int GetHashCode() {
			return BkColor.GetHashCode()
				^ FgColor.GetHashCode()
				^ TextColor.GetHashCode()
				^ TextFont.GetHashCode()
				^ (int)BrushStyle_
				^ LeftBorder.GetHashCode()
				^ TopBorder.GetHashCode()
				^ RightBorder.GetHashCode()
				^ BottomBorder.GetHashCode()
				^ (int)TextAlignment
				^ (int)LineAlignment
				^ (FormatString ?? String.Empty).GetHashCode()
				^ (XlsxFormatString ?? String.Empty).GetHashCode()
				^ PreparedCellType.GetHashCode()
				^ WrapText.GetHashCode()
				^ RightToLeft.GetHashCode();
		}
		internal bool WasExportedWithType(ushort type) {
			return Types.ContainsKey(type);
		}
		internal void AddExportedType(ushort type, int result) {
			if(!WasExportedWithType(type))
				Types[type] = result;
		}
		internal int GetExportResult(ushort type) {
			return Types[type];
		}
	}
	public struct ExportCacheCellBorderStyle {
		public bool IsDefault;
		public Color Color_;
		public int Width;
		public DevExpress.XtraPrinting.BorderDashStyle BorderDashStyle;
		public bool IsEqual(ExportCacheCellBorderStyle borderStyle) {
			return borderStyle.Width == 0 && Width == 0 ? true :
				borderStyle.IsDefault == IsDefault && borderStyle.Color_ == Color_ && borderStyle.Width == Width && borderStyle.BorderDashStyle == BorderDashStyle;
		}
		public override bool Equals(object obj) {
			if(obj is ExportCacheCellBorderStyle) {
				return IsEqual((ExportCacheCellBorderStyle)obj);
			}
			return false;
		}
		public override int GetHashCode() {
			return Width == 0 ? 0 :
				Width.GetHashCode()
				^ IsDefault.GetHashCode()
				^ Color_.GetHashCode()
				^ (int)BorderDashStyle;
		}
	}
	public class ExportCustomProvider : IDisposable {
		private string fileName;
		private Stream stream;
		private bool isStreamMode;
		public const string StreamModeName = "Stream";
		public ExportCustomProvider(string fileName) {
			this.fileName = fileName;
			isStreamMode = false;
		}
		public ExportCustomProvider(Stream stream) {
			this.stream = stream;
			isStreamMode = true;
		}
		protected void OnProviderProgress(int position) {
			if(ProviderProgress != null)
				ProviderProgress(this, new ProviderProgressEventArgs(position));
		}
		public static bool IsValidFileName(string fileName) {
			return !String.IsNullOrEmpty(fileName);
		}
		public static bool IsValidStream(Stream stream) {
			return stream != null;
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				ExportStyleManager.DisposeInstance(fileName, stream);
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ExportCustomProvider() {
			Dispose(false);
		}
		#endregion
		public string FileName { get { return fileName; } }
		public Stream Stream { get { return stream; } }
		public bool IsStreamMode { get { return isStreamMode; } }
		public event ProviderProgressEventHandler ProviderProgress;
	}
	public interface IExportStyleManagerCreator {
		ExportStyleManagerBase CreateInstance(string fileName, Stream stream);
	}
	public abstract class ExportStyleManagerBase {
		static readonly Dictionary<string, ExportStyleManagerBase> instances = new Dictionary<string, ExportStyleManagerBase>();
		static readonly Dictionary<Stream, ExportStyleManagerBase> instances2 = new Dictionary<Stream, ExportStyleManagerBase>();
		static ExportStyleManagerBase GetFileInstance(string fileName, IExportStyleManagerCreator creator) {
			ExportStyleManagerBase result;
			lock(instances) {
				if(!instances.TryGetValue(fileName, out result)) {
					result = creator.CreateInstance(fileName, null);
					instances.Add(fileName, result);
				}
			}
			return result;
		}
		static ExportStyleManagerBase GetStreamInstance(Stream stream, IExportStyleManagerCreator creator) {
			ExportStyleManagerBase result;
			lock(instances2) {
				if(!instances2.TryGetValue(stream, out result)) {
					result = creator.CreateInstance(string.Empty, stream);
					instances2.Add(stream, result);
				}
			}
			return result;
		}
		public static void DisposeInstance(string fileName, Stream stream) {
			if(ExportCustomProvider.IsValidFileName(fileName))
				lock(instances) instances.Remove(fileName);
			else {
				if(ExportCustomProvider.IsValidStream(stream))
					lock(instances2) instances2.Remove(stream);
				else
					throw new ExportCacheException("Can't dispose the instance of ExportStyleManager class: Ivalid parameter values.");
			}
		}
		public static ExportStyleManagerBase GetInstance(string fileName, Stream stream, IExportStyleManagerCreator creator) {
			if(ExportCustomProvider.IsValidFileName(fileName))
				return GetFileInstance(fileName, creator);
			else {
				if(ExportCustomProvider.IsValidStream(stream))
					return GetStreamInstance(stream, creator);
				else
					throw new ExportCacheException("Can't create the instance of ExportStyleManager class: Ivalid parameter values.");
			}
		}
		readonly IndexedDictionary<ExportCacheCellStyle> styles = new IndexedDictionary<ExportCacheCellStyle>();
		readonly string fileName;
		readonly Stream stream;
		protected ExportStyleManagerBase(string fileName, Stream stream) {
			this.fileName = fileName;
			this.stream = stream;
			RegisterStyle(CreateDefaultStyle());
		}
		public int Count {
			get {
				return styles.Count;
			}
		}
		public ExportCacheCellStyle this[int index] {
			get {
				return styles[index];
			}
		}
		public ExportCacheCellStyle DefaultStyle {
			get {
				return styles[0];
			}
			set {
				if(!styles[0].IsEqual(value))
					styles[0] = value;
			}
		}
		protected abstract ExportCacheCellStyle CreateDefaultStyle();
		internal void MarkStyleAsExported(int styleIndex, int result, ushort type) {
			ExportCacheCellStyle style = this[styleIndex];
			style.AddExportedType(type, result);
			styles[styleIndex] = style;
		}
		public int RegisterStyle(ExportCacheCellStyle style) {
			int index = styles.IndexOf(style);
			if(index >= 0)
				return index;
			styles.Add(style);
			return styles.Count - 1;
		}
		public void Clear() {
			styles.Clear();
			RegisterStyle(CreateDefaultStyle());
		}
		internal void DisposeInstance() {
			ExportStyleManagerBase.DisposeInstance(fileName, stream);
		}
	}
	public sealed class ExportStyleManager : ExportStyleManagerBase {
		public ExportStyleManager(string fileName, Stream stream)
			: base(fileName, stream) {
		}
		protected override ExportCacheCellStyle CreateDefaultStyle() {
			ExportCacheCellStyle result = new ExportCacheCellStyle();
			result.TextColor = SystemColors.WindowText;
			result.TextAlignment = StringAlignment.Near;
			result.LineAlignment = StringAlignment.Near;
			result.TextFont = new Font("Tahoma", 8);
			result.BkColor = SystemColors.Window;
			result.FgColor = Color.Black;
			result.BrushStyle_ = BrushStyle.Clear;
			result.LeftBorder.Color_ = SystemColors.ActiveBorder;
			result.TopBorder.Color_ = SystemColors.ActiveBorder;
			result.RightBorder.Color_ = SystemColors.ActiveBorder;
			result.BottomBorder.Color_ = SystemColors.ActiveBorder;
			result.LeftBorder.Width = 1;
			result.TopBorder.Width = 1;
			result.RightBorder.Width = 1;
			result.BottomBorder.Width = 1;
			result.LeftBorder.IsDefault = true;
			result.TopBorder.IsDefault = true;
			result.RightBorder.IsDefault = true;
			result.BottomBorder.IsDefault = true;
			return result;
		}
	}
	public sealed class ExportXlsxStyleManager : ExportStyleManagerBase {
		public ExportXlsxStyleManager(string fileName, Stream stream)
			: base(fileName, stream) {
		}
		protected override ExportCacheCellStyle CreateDefaultStyle() {
			ExportCacheCellStyle result = new ExportCacheCellStyle();
			result.TextColor = SystemColors.WindowText;
			result.TextAlignment = StringAlignment.Near;
			result.LineAlignment = StringAlignment.Near;
			result.TextFont = new Font("Calibri", 11);
			result.BkColor = Color.Transparent;
			result.FgColor = Color.Black;
			result.BrushStyle_ = BrushStyle.Clear;
			result.LeftBorder.Color_ = SystemColors.ActiveBorder;
			result.TopBorder.Color_ = SystemColors.ActiveBorder;
			result.RightBorder.Color_ = SystemColors.ActiveBorder;
			result.BottomBorder.Color_ = SystemColors.ActiveBorder;
			result.LeftBorder.Width = 1;
			result.TopBorder.Width = 1;
			result.RightBorder.Width = 1;
			result.BottomBorder.Width = 1;
			return result;
		}
	}
	public class ExportCacheException :
#if DXRESTRICTED
		Exception
#else
		ApplicationException
#endif
		{
		public ExportCacheException(string message)
			: base(message) {
		}
	}
	public delegate void ProviderProgressEventHandler(object sender, ProviderProgressEventArgs e);
	public class ProviderProgressEventArgs : EventArgs {
		int position;
		public ProviderProgressEventArgs(int position)
			: base() {
			this.position = position;
		}
		public int Position { get { return position; } }
	}
}
