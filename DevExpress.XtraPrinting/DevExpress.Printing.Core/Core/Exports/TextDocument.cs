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
using System.Reflection;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.Export.Text 
{
	public class TextDocument {
		#region inner classes
		class ObjectInfoComparer : IComparer {
			int IComparer.Compare(object x, object y) {
				ObjectInfo xObject = (ObjectInfo)x;
				ObjectInfo yObject = (ObjectInfo)y;
				return Compare(xObject.ColIndex, xObject.RowIndex, yObject.ColIndex, yObject.RowIndex);
			}
			int Compare(int x1, int y1, int x2, int y2) {
				if(y1 > y2)
					return 1;
				else if(y1 < y2)
					return -1;
				else if(x1 > x2)
					return 1;
				else if(x1 < x2)
					return -1;
				else
					return 0;
			}
		}
		#endregion
		public static void CreateDocument(LayoutControlCollection layoutControls, Document document, StreamWriter writer, TextExportOptionsBase options, bool insertSpaces) {
			document.ProgressReflector.RangeValue++;
			isTxt = options is TextExportOptions;
			TextDocument doc = isTxt ? new TextDocument(document, options.Separator, insertSpaces) :
				new TextDocument(document, options.Separator, insertSpaces, ((CsvExportOptions)options).SkipEmptyRows, ((CsvExportOptions)options).SkipEmptyColumns, CsvExportOptions.FollowReportLayout); 
			doc.WriteTo(writer, layoutControls);
		}
		Document document;
		string separator = TextExportOptions.DefaultSeparator;
		bool insertSpaces;
		bool followReportLayout, skipEmptyRows, skipEmptyColumns;
		static bool isTxt;
		public TextDocument(Document document, string separator, bool insertSpaces) {
			this.document = document;
			if(separator != null)
				this.separator = separator;
			this.insertSpaces = insertSpaces;
		}
		public TextDocument(Document document, string separator, bool insertSpaces, bool skipEmptyRows, bool skipEmptyColumns, bool followReportLayout)
			: this(document, separator, insertSpaces) {
			this.skipEmptyColumns = skipEmptyColumns;
			this.followReportLayout = followReportLayout;
			this.skipEmptyRows = skipEmptyRows;
		}
		void WriteRow(StreamWriter writer, TextLayoutItem[] row, int[] columnWidthArray) {
			System.Diagnostics.Debug.Assert(row.Length == columnWidthArray.Length, "Invalid row layout");
			if(row.Length == 0)
				return;
			for(int i = 0; i < row.Length - 1; i++) {
				WriteTextItem(writer, row[i], columnWidthArray[i]);
				writer.Write(this.separator);
			}
			int lastIndex = row.Length - 1;
			WriteTextItem(writer, row[lastIndex], columnWidthArray[lastIndex]);
			writer.WriteLine();
		}
		void WriteTextItemWidthSpaces(StreamWriter writer, TextLayoutItem item, int columnWidth) {
			if(item != null) {
				int width = item.Text.Length;
				writer.Write(item.Text + CreateSpaces(columnWidth - width));
			} else
				writer.Write(CreateSpaces(columnWidth));
		}
		void WriteTextItemWithoutSpaces(StreamWriter writer, TextLayoutItem item) {
			if(item != null)
				writer.Write(item.Text);
		}
		void WriteTextItem(StreamWriter writer, TextLayoutItem item, int columnWidth) {
			if(insertSpaces)
				WriteTextItemWidthSpaces(writer, item, columnWidth);
			else
				WriteTextItemWithoutSpaces(writer, item);
		}
		string CreateSpaces(int count) {
			return new string(' ', count);
		}
		void WriteCsvContent(StreamWriter writer, object[] objects, int[] mapX, int[] mapY) {
			int currentObjectIndex = 0;
			for(int i = 0; i < mapY.Length; i++) {
				int lastItemColIndex = 0;
				int countSeparators = 0;
				if(mapY[i] == 0 && this.skipEmptyRows)
					continue;
				for(int j = 0; j < mapY[i]; j++) {
					ObjectInfo objInfo = (objects[currentObjectIndex] as ObjectInfo);
					TextBrickViewData control = objInfo.Object as TextBrickViewData;
					TextLayoutItem item = control.GetLayoutItem(0);
					countSeparators = objInfo.ColIndex - lastItemColIndex;
					WriteSeparators(writer, countSeparators, lastItemColIndex, objInfo.ColIndex, mapX);
					writer.Write(item.Text); 
					lastItemColIndex = objInfo.ColIndex;
					currentObjectIndex++;
				}
				countSeparators = mapX.Length - lastItemColIndex - 1;
				WriteSeparators(writer, countSeparators, lastItemColIndex, mapX.Length, mapX);
				writer.WriteLine();
			}
		}
		void WriteSeparators(StreamWriter writer, int countSeparators, int startCol, int endCol, int[] mapX) {
			if(this.skipEmptyColumns)
				for(int k = startCol; k < endCol; k++)
					if(mapX[k] == 0) countSeparators -= 1;
			WriteSeparators(writer, countSeparators);
		}
		void WriteSeparators(StreamWriter writer, int count) {
			for(int i = 0; i < count; i++)
				writer.Write(this.separator);
		}
		void WriteTxtContent(StreamWriter writer, TextLayoutMapX mapX, TextLayoutMapY mapY) {
			int columnCount = mapX.Count;
			for(int y = 0; y < mapY.Count; y++) {
				TextLayoutItem[] row = new TextLayoutItem[columnCount];
				for(int x = 0; x < mapY[y].Count; x++) {
					TextLayoutItem item = mapY[y][x];
					int index = mapX.FindAndRemove(item.Owner);
					row[index] = item;
				}
				WriteRow(writer, row, mapX.ColumnWidthArray);
			}
		}
		public void WriteTo(StreamWriter writer, LayoutControlCollection layoutControls) {
			if(isTxt || !followReportLayout) {
				TextLayoutMapX mapX = new TextLayoutMapX(layoutControls);
				if(document != null)
					document.ProgressReflector.RangeValue++;
				TextLayoutMapY mapY = new TextLayoutMapY(layoutControls);
				if(document != null)
					document.ProgressReflector.RangeValue++;
				WriteTxtContent(writer, mapX, mapY);
			} else {
				MegaTable megaTable = new MegaTable(layoutControls, true, false);
				int[] mapX = new int[megaTable.ColumnCount];
				int[] mapY = new int[megaTable.RowCount];
				for(int i = 0; i < megaTable.Objects.Length; i++) {
					mapX[megaTable.Objects[i].ColIndex]++;
					mapY[megaTable.Objects[i].RowIndex]++;
				}
				if(document != null)
					document.ProgressReflector.RangeValue++;
				ObjectInfo[] objects = megaTable.Objects;
				Array.Sort(objects, new ObjectInfoComparer());
				WriteCsvContent(writer, objects, mapX, mapY);
			}
			if(document != null)
				document.ProgressReflector.RangeValue++;
		}
	}
	class TextLayoutGroup {
		ArrayList list = new ArrayList();
		bool decomposed;
		float minCenterX = float.MaxValue;
		float minCenterY = float.MaxValue;
		int startIndex;
		public int StartIndex { get { return startIndex >= 0 ? startIndex : 0; } }
		public int Count { get { return list.Count; } }
		public TextLayoutItem this[int index] { get { return (TextLayoutItem)list[index]; } }
		public bool Decomposed { get { return this.decomposed; } }
		public float MinCenterX { get { return minCenterX; } }
		public float MinCenterY { get { return minCenterY; } }
		public TextLayoutGroup(bool decomposed) {
			this.decomposed = decomposed;
		}
		public void Add(TextLayoutItem item) {
			RecalcMinCenters(item);
			list.Add(item);
		}
		public void RemoveAt(int index) {
			if(index == startIndex && startIndex != -1)
				startIndex++;
			else
				startIndex = -1;
			list[index] = null;
		}
		public void Sort(IComparer comparer) {
			list.Sort(comparer);
		}
		public void Insert(int index, TextLayoutItem item) {
			RecalcMinCenters(item);
			if(index < Count)
				list.Insert(index, item);
			else
				list.Add(item);
		}
		void RecalcMinCenters(TextLayoutItem item) {
			PointF center = RectHelper.CenterOf((RectangleF)item.Owner.Bounds);
			minCenterX = Math.Min(minCenterX, center.X);
			minCenterY = Math.Min(minCenterY, center.Y);
		}
	}
	abstract class TextLayoutMap {
		ArrayList list = new ArrayList();
		protected abstract IComparer PrimaryControlComparer { get; }
		protected abstract IComparer SecondaryItemComparer { get; }
		public int Count { get { return list.Count; } }
		public TextLayoutGroup this[int index] { get { return (TextLayoutGroup)list[index]; } }
		protected TextLayoutMap(LayoutControlCollection layoutControls) {
			if(layoutControls.Count == 0)
				return;
			ArrayList arrayList = new ArrayList(layoutControls);
			arrayList.Sort(PrimaryControlComparer);
			FillLayoutMap(arrayList);
			DecomposeLayoutGroups();
			SortLayoutGroups();
		}
		TextLayoutGroup Add() {
			TextLayoutGroup group = new TextLayoutGroup(false);
			list.Add(group);
			return group;
		}
		protected abstract bool ItemInGroup(TextBrickViewData control, TextLayoutGroup group);
		bool FillLayoutGroup(TextLayoutGroup group, ArrayList layoutControls, ref int baseIndex) {
			TextBrickViewData  control1 = (TextBrickViewData )layoutControls[baseIndex];
			group.Add(control1.GetLayoutItem(0));
			for(int i = baseIndex + 1; i < layoutControls.Count; i++) {
				TextBrickViewData  control2 = (TextBrickViewData )layoutControls[i];
				if(ItemInGroup(control2, group))
					group.Add(control2.GetLayoutItem(0));
				else {
					baseIndex = i;
					return false;
				}
			}
			return true;
		}
		void FillLayoutMap(ArrayList layoutControls) {
			int baseIndex = 0;
			bool finish = false;
			while(!finish) {
				TextLayoutGroup group = Add();
				finish = FillLayoutGroup(group, layoutControls, ref baseIndex);
			}
		}
		void SortLayoutGroups() {
			for(int i = 0; i < Count; i++)
				this[i].Sort(SecondaryItemComparer);
		}
		void DecomposeLayoutGroups() {
			int groupIndex = 0;
			while(groupIndex < Count) {
				TextLayoutGroup group = this[groupIndex];
				if(!group.Decomposed) {
					int index = 0;
					while(index < group.Count) {
						TextBrickViewData control = group[index].Owner;
						if(control.IsCompoundControl)
							DecomposeCompoundControl(groupIndex, ref index, control);
						index++;
					}
				}
				groupIndex++;
			}
		}
		TextLayoutGroup InsertGroup(int groupIndex) {
			TextLayoutGroup group = new TextLayoutGroup(true);
			if(groupIndex < Count)
				list.Insert(groupIndex, group);
			else
				list.Add(group);
			return group;
		}
		protected TextLayoutGroup GetGroup(int groupIndex) {
			if(groupIndex < Count && this[groupIndex].Decomposed)
				return this[groupIndex];
			else
				return InsertGroup(groupIndex);
		}
		protected abstract void DecomposeCompoundControl(int groupIndex, ref int index, TextBrickViewData control);
	}
	class TextLayoutMapX : TextLayoutMap {
		int[] columnWidthArray;
		public int[] ColumnWidthArray { get { return this.columnWidthArray; } }
		IComparer InGroupComparer { get { return TextBrickViewData.YComparer.Instance; } }
		protected override IComparer PrimaryControlComparer { get { return TextBrickViewData.XComparer.Instance; } }
		protected override IComparer SecondaryItemComparer { get { return TextLayoutItem.YComparer.Instance; } }
		public TextLayoutMapX(LayoutControlCollection layoutControls)
			: base(layoutControls) {
			this.columnWidthArray = new int[Count];
			for(int x = 0; x < Count; x++) {
				this.columnWidthArray[x] = 0;
				for(int y = 0; y < this[x].Count; y++)
					this.columnWidthArray[x] = Math.Max(this.columnWidthArray[x], this[x][y].Text.Length);
			}
		}
		protected override void DecomposeCompoundControl(int groupIndex, ref int index, TextBrickViewData control) {
			for(int row = 1; row < control.Texts.Count; row++) {
				this[groupIndex].Insert(++index, control.GetLayoutItem(row));
			}
		}
		protected override bool ItemInGroup(TextBrickViewData control, TextLayoutGroup group) {
			return control.Bounds.X <= group.MinCenterX;
		}
		int FindInGroup(TextLayoutGroup group, TextBrickViewData control) {
			for(int i = group.StartIndex; i < group.Count; i++) {
				if(group[i] == null)
					continue;
				TextBrickViewData groupControl = group[i].Owner;
				if(InGroupComparer.Compare(groupControl, control) > 0)
					break;
				if(groupControl == control)
					return i;
			}
			return -1;
		}
		public int FindAndRemove(TextBrickViewData control) {
			int groupIndex = 0;
			while(groupIndex < Count) {
				if(this[groupIndex].Count > 0) {
					int indexInGroup = FindInGroup(this[groupIndex], control);
					if(indexInGroup != -1) {
						this[groupIndex].RemoveAt(indexInGroup);
						return groupIndex;
					}
				}
				groupIndex++;
			}
			throw new TextException();
		}
	}
	class TextLayoutMapY : TextLayoutMap {
		protected override IComparer PrimaryControlComparer { get { return TextBrickViewData.YComparer.Instance; } }
		protected override IComparer SecondaryItemComparer { get { return TextLayoutItem.XComparer.Instance; } }
		public TextLayoutMapY(LayoutControlCollection layoutControls) : base(layoutControls) {
		}
		protected override void DecomposeCompoundControl(int groupIndex, ref int index, TextBrickViewData control) {
			for(int row = 1; row < control.Texts.Count; row++) {
				TextLayoutGroup group = GetGroup(++groupIndex);
				group.Add(control.GetLayoutItem(row));
			}
		}
		protected override bool ItemInGroup(TextBrickViewData control, TextLayoutGroup group) {
			return control.Bounds.Y <= group.MinCenterY;
		}
	}
	public class TextLayoutItem {
		#region inner classes
		public class XComparer : IComparer {
			public static readonly XComparer Instance = new XComparer();
			private XComparer() {
			}
			public int Compare(object x, object y) {
				TextLayoutItem itemX = (TextLayoutItem)x;
				TextLayoutItem itemY = (TextLayoutItem)y;
				return TextBrickViewData.XComparer.Instance.Compare(itemX.Owner, itemY.Owner);			
			}
		}
		public class YComparer : IComparer {
			public static readonly YComparer Instance = new YComparer();
			private YComparer() {
			}
			public int Compare(object x, object y) {
				TextLayoutItem itemX = (TextLayoutItem)x;
				TextLayoutItem itemY = (TextLayoutItem)y;
				return TextBrickViewData.YComparer.Instance.Compare(itemX.Owner, itemY.Owner);
			}
		}
		#endregion 
		string text;
		TextBrickViewData owner;
		public string Text { get { return this.text; } }
		public TextBrickViewData Owner { get { return this.owner; } }
		public TextLayoutItem(string text, TextBrickViewData owner) {
			this.text = text;
			this.owner = owner;
		}
	}
	public class TextException : Exception {
		public TextException() {
		}
		public TextException(string message) : base(message) {
		}
		public TextException(string message, Exception innerEx) : base(message, innerEx) {
		}
	}
}
