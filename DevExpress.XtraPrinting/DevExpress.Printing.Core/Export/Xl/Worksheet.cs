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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Compatibility.System.Drawing.Printing;
#if !SL
using System.Drawing.Printing;
#else
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.Export.Xl {
	#region XlHeaderFooter
	public class XlHeaderFooter {
		public const string Left = "&L";
		public const string Center = "&C";
		public const string Right = "&R";
		public const string PageNumber = "&P";
		public const string PageTotal = "&N";
		public const string BookName = "&F";
		public const string BookPath = "&Z";
		public const string SheetName = "&A";
		public const string Date = "&D";
		public const string Time = "&T";
		public const string Bold = "&B";
		public const string Italic = "&I";
		public const string Strikethrough = "&S";
		public const string Superscript = "&X";
		public const string Subscript = "&Y";
		public const string Underline = "&U";
		public const string DoubleUnderline = "&E";
		public static string FromLCR(string leftPart, string centerPart, string rightPart) {
			StringBuilder sb = new StringBuilder();
			if (!string.IsNullOrEmpty(leftPart)) {
				sb.Append(Left);
				sb.Append(leftPart);
			}
			if (!string.IsNullOrEmpty(centerPart)) {
				sb.Append(Center);
				sb.Append(centerPart);
			}
			if (!string.IsNullOrEmpty(rightPart)) {
				sb.Append(Right);
				sb.Append(rightPart);
			}
			return sb.ToString();
		}
		public XlHeaderFooter() {
			Clear();
		}
		public bool AlignWithMargins { get; set; }
		public bool DifferentFirst { get; set; }
		public bool DifferentOddEven { get; set; }
		public bool ScaleWithDoc { get; set; }
		public string FirstHeader { get; set; }
		public string FirstFooter { get; set; }
		public string EvenHeader { get; set; }
		public string EvenFooter { get; set; }
		public string OddHeader { get; set; }
		public string OddFooter { get; set; }
		public void Clear() {
			AlignWithMargins = true;
			ScaleWithDoc = true;
			DifferentFirst = false;
			DifferentOddEven = false;
			FirstHeader = string.Empty;
			FirstFooter = string.Empty;
			EvenHeader = string.Empty;
			EvenFooter = string.Empty;
			OddHeader = string.Empty;
			OddFooter = string.Empty;
		}
		internal bool IsDefault() {
			return AlignWithMargins && ScaleWithDoc && !DifferentFirst && !DifferentOddEven &&
				string.IsNullOrEmpty(FirstHeader) && string.IsNullOrEmpty(FirstFooter) &&
				string.IsNullOrEmpty(EvenHeader) && string.IsNullOrEmpty(EvenFooter) &&
				string.IsNullOrEmpty(OddHeader) && string.IsNullOrEmpty(OddFooter);
		}
	}
	#endregion
	#region XlPrintTitles
	public class XlPrintTitles {
		readonly IXlSheet sheet;
		public XlPrintTitles(IXlSheet sheet) {
			this.sheet = sheet;
		}
		public XlCellRange Rows { get; set; }
		public XlCellRange Columns { get; set; }
		internal bool IsValid() {
			return (Rows != null && Rows.TopLeft.IsRow && Rows.BottomRight.IsRow) ||
				(Columns != null && Columns.TopLeft.IsColumn && Columns.BottomRight.IsColumn);
		}
		public void SetRows(int startIndex, int endIndex) {
			Rows = new XlCellRange(
				new XlCellPosition(-1, startIndex, XlPositionType.Absolute, XlPositionType.Absolute),
				new XlCellPosition(-1, endIndex, XlPositionType.Absolute, XlPositionType.Absolute));
		}
		public void SetColumns(int startIndex, int endIndex) {
			Columns = new XlCellRange(
				new XlCellPosition(startIndex, -1, XlPositionType.Absolute, XlPositionType.Absolute),
				new XlCellPosition(endIndex, -1, XlPositionType.Absolute, XlPositionType.Absolute));
		}
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			AppendRange(sb, Columns);
			AppendRange(sb, Rows);
			return sb.ToString();
		}
		void AppendRange(StringBuilder sb, XlCellRange intervalRange) {
			if (intervalRange == null)
				return;
			XlCellRange range = new XlCellRange(intervalRange.TopLeft, intervalRange.BottomRight);
			range.SheetName = sheet.Name;
			if (sb.Length > 0)
				sb.Append(",");
			sb.Append(range.ToString(true));
		}
	}
	#endregion
	#region XlSheetVisibleState
	public enum XlSheetVisibleState {
		Visible = 0,
		Hidden = 1,
		VeryHidden = 2
	}
	#endregion
	#region XlIgnoreErrors
	[Flags]
	public enum XlIgnoreErrors {
		None = 0x0000,
		CalculatedColumn = 0x0001,
		EmptyCellReference = 0x0002,
		EvaluationError = 0x0004,
		Formula = 0x0008,
		FormulaRange = 0x0010,
		ListDataValidation = 0x0020,
		NumberStoredAsText = 0x0040,
		TwoDigitTextYear = 0x0080,
		UnlockedFormula = 0x0100,
		Any = 0x01ff
	}
	#endregion
	#region XlPageUnits
	public enum XlPageUnits {
		Inches,
		Centimeters
	}
	#endregion
	#region XlPageMargins
	public class XlPageMargins {
		const double k = 2.54;
		double[] margins = new double[] { 0.7, 0.7, 0.75, 0.75, 0.3, 0.3 };
		public XlPageUnits PageUnits { get; set; }
		public double Left {
			get { return GetValue(0); }
			set { SetValue(0, value, "Left"); }
		}
		public double Right {
			get { return GetValue(1); }
			set { SetValue(1, value, "Right"); }
		}
		public double Top {
			get { return GetValue(2); }
			set { SetValue(2, value, "Top"); }
		}
		public double Bottom {
			get { return GetValue(3); }
			set { SetValue(3, value, "Bottom"); }
		}
		public double Header {
			get { return GetValue(4); }
			set { SetValue(4, value, "Header"); }
		}
		public double Footer {
			get { return GetValue(5); }
			set { SetValue(5, value, "Footer"); }
		}
		internal double LeftInches { get { return margins[0]; } }
		internal double RightInches { get { return margins[1]; } }
		internal double TopInches { get { return margins[2]; } }
		internal double BottomInches { get { return margins[3]; } }
		internal double HeaderInches { get { return margins[4]; } }
		internal double FooterInches { get { return margins[5]; } }
		double GetValue(int index) {
			if (PageUnits == XlPageUnits.Centimeters)
				return margins[index] * k;
			return margins[index];
		}
		void SetValue(int index, double value, string valueName) {
			if(PageUnits == XlPageUnits.Centimeters) {
				if(value < 0 || value > (49 * k))
					throw new ArgumentOutOfRangeException(string.Format("{0} margin out of range 0..124.46 centimeters", valueName));
				margins[index] = value / k;
			}
			else {
				if(value < 0 || value > 49)
					throw new ArgumentOutOfRangeException(string.Format("{0} margin out of range 0..49 inches", valueName));
				margins[index] = value;
			}
		}
	}
	#endregion
	#region XlCommentsPrintMode
	public enum XlCommentsPrintMode {
		AsDisplayed = 0,
		AtEnd = 1,
		None = 2,
	}
	#endregion
	#region XlErrorsPrintMode
	public enum XlErrorsPrintMode {
		Displayed = 0,
		Blank = 1,
		Dash = 2,
		NA = 3,
	}
	#endregion
	#region XlPagePrintOrder
	public enum XlPagePrintOrder {
		DownThenOver = 0,
		OverThenDown = 1,
	}
	#endregion
	#region XlPageOrientation
	public enum XlPageOrientation {
		Default,
		Portrait,
		Landscape,
	}
	#endregion
	#region XlPageSetup
	public class XlPageSetup {
		#region Fields
		int scale = 100;
		int copies = 1;
		int firstPageNumber = 1;
		int fitToWidth = 1;
		int fitToHeight = 1;
		int horizontalDpi = 600;
		int verticalDpi = 600;
		#endregion
		public XlPageSetup() {
			PaperKind = PaperKind.Letter;
			CommentsPrintMode = XlCommentsPrintMode.None;
			AutomaticFirstPageNumber = true;
			UsePrinterDefaults = true;
		}
		#region Properties
		public PaperKind PaperKind { get; set; }
		public XlCommentsPrintMode CommentsPrintMode { get; set; }
		public XlErrorsPrintMode ErrorsPrintMode { get; set; }
		public XlPagePrintOrder PagePrintOrder { get; set; }
		public XlPageOrientation PageOrientation { get; set; }
		public int Scale {
			get { return scale; }
			set {
				if(value < 10 || value > 400)
					throw new ArgumentOutOfRangeException("Scale out of range 10%...400%");
				scale = value;
			}
		}
		public bool BlackAndWhite { get; set; }
		public bool Draft { get; set; }
		public bool AutomaticFirstPageNumber { get; set; }
		public bool UsePrinterDefaults { get; set; }
		public bool FitToPage { get; set; }
		public int Copies { 
			get { return copies; } 
			set {
				if(value < 1 || value > short.MaxValue)
					throw new ArgumentOutOfRangeException("Copies out of range 1...32767");
				copies = value; 
			} 
		}
		public int FirstPageNumber { 
			get { return firstPageNumber; } 
			set {
				if(value < short.MinValue || value > short.MaxValue)
					throw new ArgumentOutOfRangeException(string.Format("FirstPageNumber out of range {0}...{1}", short.MinValue, short.MaxValue));
				firstPageNumber = value; 
			} 
		}
		public int FitToWidth { 
			get { return fitToWidth; } 
			set {
				if(value < 0 || value > short.MaxValue)
					throw new ArgumentOutOfRangeException("FitToWidth out of range 0...32767");
				fitToWidth = value; 
			} 
		}
		public int FitToHeight { 
			get { return fitToHeight; } 
			set {
				if(value < 0 || value > short.MaxValue)
					throw new ArgumentOutOfRangeException("FitToHeight out of range 0...32767");
				fitToHeight = value; 
			} 
		}
		public int HorizontalDpi {
			get { return horizontalDpi; }
			set {
				if(value < 1 || value > ushort.MaxValue)
					throw new ArgumentOutOfRangeException("HorizontalDpi out of range 1...65535");
				horizontalDpi = value;
			}
		}
		public int VerticalDpi {
			get { return verticalDpi; }
			set {
				if(value < 1 || value > ushort.MaxValue)
					throw new ArgumentOutOfRangeException("VerticalDpi out of range 1...65535");
				verticalDpi = value;
			}
		}
		#endregion
		internal bool IsDefault() {
			return !((PaperKind != PaperKind.Letter) ||
				(CommentsPrintMode != XlCommentsPrintMode.None) ||
				(ErrorsPrintMode != XlErrorsPrintMode.Displayed) ||
				(PagePrintOrder != XlPagePrintOrder.DownThenOver) ||
				(PageOrientation != XlPageOrientation.Default) ||
				(Scale != 100) || BlackAndWhite || Draft ||
				(!AutomaticFirstPageNumber) || (!UsePrinterDefaults) ||
				(Copies != 1) || (FirstPageNumber != 1) ||
				(FitToWidth != 1) || (FitToHeight != 1) ||
				(HorizontalDpi != 600) || (VerticalDpi != 600));
		}
	}
	#endregion
	#region XlPrintOptions
	public class XlPrintOptions {
		public bool GridLines { get; set; }
		public bool Headings { get; set; }
		public bool HorizontalCentered { get; set; }
		public bool VerticalCentered { get; set; }
		internal bool IsDefault() {
			return !GridLines && !Headings && !HorizontalCentered && !VerticalCentered;
		}
	}
	#endregion
	#region IXlPageBreaks
	public interface IXlPageBreaks : IEnumerable<int>, ICollection, IEnumerable {
		int this[int index] { get; }
		void Add(int position);
		int IndexOf(int position);
		bool Contains(int position);
		void Remove(int position);
		void RemoveAt(int index);
		void Clear();
	}
	#endregion
	#region IXlMergedCells
	public interface IXlMergedCells : IEnumerable<XlCellRange>, ICollection, IEnumerable {
		XlCellRange this[int index] { get; }
		void Add(XlCellRange range);
		void Add(XlCellRange range, bool checkOverlap);
		int IndexOf(XlCellRange range);
		bool Contains(XlCellRange range);
		void Remove(XlCellRange range);
		void RemoveAt(int index);
		void Clear();
	}
	#endregion
	#region IXlSheetViewOptions
	public interface IXlSheetViewOptions {
		bool ShowFormulas { get; set; }
		bool ShowGridLines { get; set; }
		bool ShowRowColumnHeaders { get; set; }
		bool ShowZeroValues { get; set; }
		bool ShowOutlineSymbols { get; set; }
		bool RightToLeft { get; set; }
	}
	#endregion
}
namespace DevExpress.XtraExport.Implementation {
	using DevExpress.Export.Xl;
	#region XlPageBreaksCollection
	public class XlPageBreaksCollection : IXlPageBreaks {
		readonly List<int> innerList = new List<int>();
		readonly int maxPosition;
		public XlPageBreaksCollection(int maxPosition) {
			this.maxPosition = maxPosition;
		}
		public int this[int index] { get { return innerList[index]; } }
		public int Count { get { return innerList.Count; } }
		public void Add(int position) {
			if(position <= 0 || position > maxPosition)
				throw new ArgumentOutOfRangeException(string.Format("Position out of range 0...{0}", maxPosition));
			int index = innerList.BinarySearch(position);
			if(index < 0) {
				index = ~index;
				this.innerList.Insert(index, position);
			}
		}
		public int IndexOf(int position) {
			int index = innerList.BinarySearch(position);
			return index >= 0 ? index : -1;
		}
		public bool Contains(int position) {
			int index = innerList.BinarySearch(position);
			return index >= 0;
		}
		public void Remove(int position) {
			int index = innerList.BinarySearch(position);
			if(index >= 0)
				innerList.RemoveAt(index);
		}
		public void RemoveAt(int index) {
			innerList.RemoveAt(index);
		}
		public void Clear() {
			innerList.Clear();
		}
		#region IEnumerable<int> Members
		public IEnumerator<int> GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
		bool ICollection.IsSynchronized {
			get { return ((ICollection)innerList).IsSynchronized; }
		}
		object ICollection.SyncRoot {
			get { return ((ICollection)innerList).SyncRoot; }
		}
		#endregion
	}
	#endregion
	#region XlRangeOverlapChecker
	public class XlRangeOverlapChecker {
		const int rowDistance = 16;
		const int columnDistance = 64;
		const int lastClusterIndex = -1;
		abstract class RangeCluster {
			public abstract void Add(XlCellRange range);
			public abstract bool IsNotOverlapped(XlCellRange range);
			public abstract void Remove(XlCellRange range);
		}
		class ColumnsCluster : RangeCluster {
			List<XlCellRange> innerList = new List<XlCellRange>();
			public override void Add(XlCellRange range) {
				this.innerList.Add(range);
			}
			public override bool IsNotOverlapped(XlCellRange range) {
				int count = this.innerList.Count;
				for(int i = 0; i < count; i++) {
					if(range.HasCommonCells(this.innerList[i]))
						return false;
				}
				return true;
			}
			public override void Remove(XlCellRange range) {
				this.innerList.Remove(range);
			}
		}
		class RowsCluster : RangeCluster {
			Dictionary<int, RangeCluster> clusters = new Dictionary<int, RangeCluster>();
			public override void Add(XlCellRange range) {
				int index = GetIndexOfCluster(range);
				RangeCluster cluster = GetCluster(index);
				cluster.Add(range);
			}
			public override bool IsNotOverlapped(XlCellRange range) {
				int index = GetIndexOfCluster(range);
				RangeCluster cluster = GetCluster(index);
				bool result;
				if(index != lastClusterIndex) {
					result = cluster.IsNotOverlapped(range);
					if(result) {
						if(!GetCluster(lastClusterIndex).IsNotOverlapped(range))
							result = false;
					}
				}
				else {
					result = cluster.IsNotOverlapped(range);
					if(result) {
						int firstIndex = range.FirstColumn / columnDistance;
						int lastIndex = range.LastColumn / columnDistance;
						for(int i = firstIndex; i <= lastIndex; i++) {
							RangeCluster clusterItem;
							if(this.clusters.TryGetValue(i, out clusterItem) && !clusterItem.IsNotOverlapped(range)) {
								result = false;
								break;
							}
						}
					}
				}
				return result;
			}
			public override void Remove(XlCellRange range) {
				int index = GetIndexOfCluster(range);
				RangeCluster cluster = FindCluster(index);
				if(cluster != null)
					cluster.Remove(range);
			}
			int GetIndexOfCluster(XlCellRange range) {
				int firstIndex = range.FirstColumn / columnDistance;
				int lastIndex = range.LastColumn / columnDistance;
				if(firstIndex != lastIndex)
					return lastClusterIndex;
				return firstIndex;
			}
			RangeCluster GetCluster(int index) {
				if(this.clusters.ContainsKey(index))
					return this.clusters[index];
				RangeCluster cluster = new ColumnsCluster();
				this.clusters.Add(index, cluster);
				return cluster;
			}
			RangeCluster FindCluster(int index) {
				if(this.clusters.ContainsKey(index))
					return this.clusters[index];
				return null;
			}
		}
		readonly Dictionary<int, RangeCluster> clusters;
		public XlRangeOverlapChecker() {
			this.clusters = new Dictionary<int, RangeCluster>();
		}
		public void Clear() {
			this.clusters.Clear();
		}
		public bool IsNotOverlapped(XlCellRange range) {
			int index = GetIndexOfCluster(range);
			RangeCluster cluster = GetCluster(index);
			bool result;
			if(index != lastClusterIndex) {
				result = cluster.IsNotOverlapped(range);
				if(result) {
					if(GetCluster(lastClusterIndex).IsNotOverlapped(range))
						cluster.Add(range);
					else
						result = false;
				}
			}
			else {
				result = cluster.IsNotOverlapped(range);
				if(result) {
					int firstIndex = range.FirstRow / rowDistance;
					int lastIndex = range.LastRow / rowDistance;
					for(int i = firstIndex; i <= lastIndex; i++) {
						RangeCluster clusterItem;
						if(this.clusters.TryGetValue(i, out clusterItem) && !clusterItem.IsNotOverlapped(range)) {
							result = false;
							break;
						}
					}
					if(result)
						cluster.Add(range);
				}
			}
			return result;
		}
		public void Remove(XlCellRange range) {
			int index = GetIndexOfCluster(range);
			RangeCluster cluster = FindCluster(index);
			if(cluster != null)
				cluster.Remove(range);
		}
		int GetIndexOfCluster(XlCellRange range) {
			int firstIndex = range.FirstRow / rowDistance;
			int lastIndex = range.LastRow / rowDistance;
			if(firstIndex != lastIndex)
				return lastClusterIndex;
			return firstIndex;
		}
		RangeCluster GetCluster(int index) {
			if(this.clusters.ContainsKey(index))
				return this.clusters[index];
			RangeCluster cluster = new RowsCluster();
			this.clusters.Add(index, cluster);
			return cluster;
		}
		RangeCluster FindCluster(int index) {
			if(this.clusters.ContainsKey(index))
				return this.clusters[index];
			return null;
		}
	}
	#endregion
	#region XlMergedCellsCollection
	public class XlMergedCellsCollection : IXlMergedCells {
		readonly List<XlCellRange> innerList = new List<XlCellRange>();
		readonly XlRangeOverlapChecker overlapChecker = new XlRangeOverlapChecker();
		public XlMergedCellsCollection() {
		}
		public XlCellRange this[int index] { get { return innerList[index]; } }
		public int Count { get { return innerList.Count; } }
		public void Add(XlCellRange range) {
			if(!overlapChecker.IsNotOverlapped(range))
				throw new ArgumentException("Cell range shouldn't overlap other ranges of merged cells.");
			innerList.Add(range);
		}
		public void Add(XlCellRange range, bool checkOverlap) {
			if(checkOverlap && !overlapChecker.IsNotOverlapped(range))
				throw new ArgumentException("Cell range shouldn't overlap other ranges of merged cells.");
			innerList.Add(range);
		}
		public int IndexOf(XlCellRange range) {
			return innerList.IndexOf(range);
		}
		public bool Contains(XlCellRange range) {
			return innerList.Contains(range);
		}
		public void Remove(XlCellRange range) {
			overlapChecker.Remove(range);
			innerList.Remove(range);
		}
		public void RemoveAt(int index) {
			XlCellRange range = innerList[index];
			overlapChecker.Remove(range);
			innerList.RemoveAt(index);
		}
		public void Clear() {
			overlapChecker.Clear();
			innerList.Clear();
		}
		#region IEnumerable<XlCellRange> Members
		public IEnumerator<XlCellRange> GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
		bool ICollection.IsSynchronized {
			get { return ((ICollection)innerList).IsSynchronized; }
		}
		object ICollection.SyncRoot {
			get { return ((ICollection)innerList).SyncRoot; }
		}
		#endregion
	}
	#endregion
}
