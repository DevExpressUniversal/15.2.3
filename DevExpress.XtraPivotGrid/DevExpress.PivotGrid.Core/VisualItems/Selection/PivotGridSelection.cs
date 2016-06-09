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

using DevExpress.Compatibility.System.Drawing;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.XtraPivotGrid.Selection {
	public class PivotGridSelection {
		readonly SelectionStorage innerCells;
		readonly ReadOnlyCells readOnlyCells;
		ReadOnlyCells fLastState = ReadOnlyCells.Empty;
		bool isLastSelectionIsMultiSelect;
		bool isLastSelectionIsShiftDown;
		bool isChanged = false;
		protected SelectionStorage InnerCells { get { return innerCells; } }
		public bool IsEmpty { get { return Cells.IsEmpty; } }
		public ReadOnlyCells Cells { get { return readOnlyCells; } }
		public Rectangle Rectangle {
			get { return Cells.Rectangle; }
			set {
				Clear();
				AddCore(value);
			}
		}
		public PivotGridSelection() {
			this.innerCells = new SelectionStorage();
			this.readOnlyCells = new ReadOnlyCells(innerCells);
		}
		public void Add(Rectangle selection) {
			LoadLastState();
			AddCore(selection);
		}
		public void AddFieldValueSelection(Rectangle selection) {
			StartSelection(true, true, false, true);
			Add(selection);
		}
		public bool IsChanged { get { return isChanged; } set { isChanged = value; } }
		internal void AddCore(Rectangle selection) {
			if(selection.IsEmpty)
				return;
			bool changed = false;
			List<Point> newPoints = new List<Point>();
			for(int i = selection.Left; i < selection.Right; i++)
				for(int j = selection.Top; j < selection.Bottom; j++) {
					Point newPoint = new Point(i, j);
					if(!InnerCells.Contains(newPoint)) {
						newPoints.Add(newPoint);
						changed = true;
					}
				}
			AddRange(newPoints, selection);
			IsChanged = changed;
		}
		void AddRange(IEnumerable<Point> points) {
			IsChanged = true;
			foreach(Point point in points)
				InnerCells.Add(point);
		}
		void AddRange(IEnumerable<SelectionStorage.RegionSelection> points) {
			IsChanged = true;
			InnerCells.AddRange(points);
		}
		void AddRange(IEnumerable<Point> points, Rectangle rect) {
			IsChanged = true;
			InnerCells.Add(rect);
		}
		public void Subtract(Rectangle selection) {
			LoadLastState();
			SubtractCore(selection);
		}
		void SubtractCore(Rectangle selection) {
			if(IsEmpty)
				return;
			IsChanged = InnerCells.Remove(SelectionStorage.RegionSelection.Create(selection));
		}
		public void Clear() {
			if(InnerCells.Count == 0)
				return;
			IsChanged = true;
			InnerCells.Clear();
		}
		public void SetSelection(params Point[] points) {
			StartSelection(false);
			AddRange(points);
		}
		public void StartSelection(bool multiSelect) {
			StartSelection(multiSelect, multiSelect, false, false);
		}
		public void StartSelection(bool multiSelect, bool isControlDown, bool isShiftDown, bool isFieldValueSelection) {
			isLastSelectionIsMultiSelect = multiSelect;
			if(multiSelect && (isControlDown || isShiftDown)) {
				fLastState = Save();
			} else {
				if(!isFieldValueSelection)
					Clear();
				fLastState = ReadOnlyCells.Empty;
			}
			isLastSelectionIsShiftDown = isShiftDown;
		}
		public void LoadLastState() {
			Load(fLastState);
			IsChanged = false;
		}
		public ReadOnlyCells LastSelection {
			get {
				return Cells - fLastState;
			}
		}
		public bool IsLastSelectionIsMultiSelect { get { return isLastSelectionIsMultiSelect; } }
		public bool IsLastSelectionIsShiftDown { get { return isLastSelectionIsShiftDown; } }
		ReadOnlyCells Save() {
			return new ReadOnlyCells(InnerCells.Clone());
		}
		void Load(ReadOnlyCells cells) {
			Clear();
			AddRange(cells.GetRectEnumerable());
		}
		public void CorrectSelection(int columnCount, int rowCount, int maxWidth, int maxHeight, Rectangle lastSelection, bool? isColumnSelection) {
			Rectangle r = new Rectangle(0, 0, columnCount, rowCount);
			r = Rectangle.Intersect(r, Rectangle);
			r = CorrectSelectionConstraints(lastSelection, r, maxWidth, maxHeight, isColumnSelection);
			bool changed = IsChanged;
			if(innerCells.Correct(r))
				changed = true;
			IsChanged = changed;
		}
		Rectangle CorrectSelectionConstraints(Rectangle curSelection, Rectangle newSelecion, int maxWidth, int maxHeight, bool? isColumnSelection) {
			Rectangle result = newSelecion;
			if(maxWidth >= 0 && newSelecion.Width > maxWidth) {
				if(newSelecion.Left < curSelection.Left && isColumnSelection != false) 
					result.X = newSelecion.Right - maxWidth;
				result.Width = maxWidth;
			}
			if(maxHeight >= 0 && newSelecion.Height > maxHeight) {
				if(newSelecion.Top < curSelection.Top && isColumnSelection != true) 
					result.Y = newSelecion.Bottom - maxHeight;
				result.Height = maxHeight;
			}
			return result;
		}
	}
}
