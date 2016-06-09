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

using System.Linq;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraVerticalGrid.Utils {
	public class ParamCollection : IEnumerable {
		public class ScaleParams {
			internal int Width, MinWidth, FixedWidth;
			internal ScaleParams() : this(0, 0, 0) {}
			internal ScaleParams(int width) : this(width, 0, 0) {}
			internal ScaleParams(int width, int minWidth) : this(width, minWidth, 0) {}
			internal ScaleParams(int width, int minWidth, int fixedWidth) {
				this.Width = width;
				this.MinWidth = minWidth;
				this.FixedWidth = fixedWidth;
			}
		}
		private ArrayList _list;
		internal ParamCollection() {
			_list = new ArrayList();
		}
		internal void Add(int width, int minWidth, int fixedWidth) {
			_list.Add(new ScaleParams(width, minWidth, fixedWidth));
		}
		internal void Add(int width, int minWidth) {
			Add(width, minWidth, 0);
		}
		internal void Add(int width) {
			Add(width, 0, 0);
		}
		internal void Clear() { _list.Clear(); }
		internal int Count { get { return _list.Count; } }
		internal ScaleParams this[int index] {
			get {
				if(index < 0 || index > Count -	1) return null;
				return (ScaleParams)_list[index];
			}
		}
		IEnumerator IEnumerable.GetEnumerator() { return _list.GetEnumerator(); }
	}
	public enum ScaleMode { Truncate, Add }
	public class RectScaler {
		private ParamCollection items;
		public RectScaler() {
			items = new ParamCollection();
		}
		public ParamCollection Items { get { return items; } }
		protected ParamCollection.ScaleParams SummaryParam {
			get {
				ParamCollection.ScaleParams summaryParam = new ParamCollection.ScaleParams();
				foreach(ParamCollection.ScaleParams pars in Items) {
					summaryParam.Width += pars.Width;
					summaryParam.MinWidth += pars.MinWidth;
					summaryParam.FixedWidth += pars.FixedWidth;
				}
				return summaryParam;
			}
		}
		public Rectangle[] ScaleRect(Rectangle Rect) {
			return ScaleRect(Rect, ScaleMode.Truncate);
		}
		public Rectangle[] ScaleRect(Rectangle Rect, ScaleMode mode) {
			if(Items.Count == 0) return null;
			Rectangle[] rects = new Rectangle[Items.Count];
			ParamCollection.ScaleParams sumParam = SummaryParam;
			int w = Rect.Width - sumParam.FixedWidth;
			double coeff = (double)w / (double)sumParam.Width;
			double error = 0;
			int left = Rect.Left;
			for(int i = 0; i < Items.Count; i++) {
				double rectWidth = coeff * (double)items[i].Width;
				Rectangle r = new Rectangle(left, Rect.Top, Convert.ToInt32(rectWidth), Rect.Height);
				error += (double)r.Width - rectWidth;
				if(r.Width < Items[i].MinWidth) r.Width = Items[i].MinWidth;
				if(i == Items.Count - 1) {
					if(mode == ScaleMode.Truncate)
						r.Width = Rect.Right - left;
					else {
						if(r.Right < Rect.Right)
							r.Width += Rect.Right - r.Right;
						else if(r.Right == Rect.Right + error)
							r.Width -= Convert.ToInt32(error);
					}
				}
				rects[i] = r;
				if(r.Right > Rect.Right && mode == ScaleMode.Truncate) {
					for(int j = i + 1; j < Items.Count; j++)
						rects[j].Location = new Point(r.Right, r.Top);
					break;
				}
				left += r.Width + Items[i].FixedWidth;
			}
			return rects;
		}
	}
	public class RectScaleScroller : RectScaler {
		static RectScaleScroller instance;
		protected RectScaleScroller() { }
		static RectScaleScroller() { instance = new RectScaleScroller(); }
		public void CalcCellRects(RectScaleScrollerArgs args) {
			Items.Clear();
			IList<MultiEditorRowProperties> propertiesList = !args.CalcValues ? args.Row.GetVisibleRowProperties() : args.Row.PropertiesCollection.Cast<MultiEditorRowProperties>().ToList();
			for (int i = 0; i < propertiesList.Count - 1; i++) {
				Items.Add(args.GetWidth(propertiesList[i]), MultiEditorRowProperties.MinWidth, args.SeparatorWidth);
			}
			int lastItemIndex = propertiesList.Count - 1;
			if (0 <= lastItemIndex)
				Items.Add(args.GetWidth(propertiesList[lastItemIndex]), MultiEditorRowProperties.MinWidth, 0);
			Rectangle[] rects = ScaleRect(args.Bounds, ScaleMode.Add);
			if(args.Row.Grid.ViewInfo.IsRightToLeft)
				for(int i = 0; i < rects.Length; i++)
					rects[i] = args.Row.Grid.ViewInfo.ConvertBoundsToRTL(rects[i], args.Bounds);
			args.Rects = rects;
			TransformToVisibleFocusRect(args);
		}
		protected virtual void TransformToVisibleFocusRect(RectScaleScrollerArgs args) {
			if(args.Rects == null) return;
			CalsFirstCellParams(args);
			ArrayList rectangles = new ArrayList();
			for(int i = args.FirstCellIndex; i < args.Rects.Length; i++) {
				Rectangle r = args.Rects[i];
				if(!r.IsEmpty) {
					r.X += args.Offset;
					if(r.Right < args.Bounds.Left) continue;
					if(r.Left > args.Bounds.Right) break;
					rectangles.Add(r);
				}
			}
			args.Rects = (Rectangle[])rectangles.ToArray(typeof(Rectangle));
		}
		protected virtual void CalsFirstCellParams(RectScaleScrollerArgs args) {
			Rectangle focusCell = args.Rects[args.FocusIndex];
			focusCell.X += args.Offset;
			if(args.Bounds.Contains(focusCell) || args.Bounds.Width <= focusCell.Width) return;
			if(focusCell.Left < args.Bounds.Left) {
				args.Offset += args.Bounds.Left - focusCell.Left;
				args.FirstCellIndex = args.FocusIndex;
				return;
			}
			if(focusCell.Right > args.Bounds.Right) {
				args.Offset -= focusCell.Right - args.Bounds.Right;
				args.FirstCellIndex = args.FocusIndex;
				for(int i = args.FocusIndex; i > -1; i--) {
					args.FirstCellIndex = i;
					if(args.Rects[i].Left + args.Offset < args.Bounds.Left) break;
				}
			}
		}
		public static RectScaleScroller Instance { get { return instance; } }
	}
	public class RectScaleScrollerArgs {
		Rectangle[] rects;
		Rectangle bounds;
		MultiEditorRow row;
		int firstCellIndex, offset, separatorWidth;
		bool calcValues;
		public RectScaleScrollerArgs(Rectangle bounds, MultiEditorRow row, int firstCellIndex, int offset, int separatorWidth, bool calcValues) {
			this.rects = null;
			this.bounds = bounds;
			this.row = row;
			this.firstCellIndex = firstCellIndex;
			this.offset = offset;
			this.separatorWidth = separatorWidth;
			this.calcValues = calcValues;
		}
		public int GetWidth(MultiEditorRowProperties prop) {
			return (CalcValues ? prop.CellWidth : prop.Width);
		}
		public bool CalcValues { get { return calcValues; } }
		public Rectangle Bounds { get { return bounds; } }
		public MultiEditorRow Row { get { return row; } }
		public int SeparatorWidth { get { return separatorWidth; } }
		public Rectangle[] Rects { get { return rects; } set { rects = value; } }
		public int FirstCellIndex { get { return firstCellIndex; } set { firstCellIndex = value; } }
		public int FocusIndex {
			get {
				if(!Row.IsConnected || Row != Row.Grid.FocusedRow)
					return 0;
				return Math.Min(Row.Grid.FocusedRecordCellIndex, Rects.Length - 1);
			}
		}
		public int Offset { get { return offset; } set { offset = value; } }
	}
	public class RectUtils {
		static public int GetTopCentralPoint(int height, Rectangle r) {
			return (r.Top + (r.Height - height) / 2);
		}
		static public int GetLeftCentralPoint(int width, Rectangle r) {
			return (r.Left + (r.Width - width) / 2);
		}
		static public Rectangle IncreaseFromLeft(Rectangle r, int size) {
			r.X -= size;
			r.Width += size;
			return r;
		}
		static public Rectangle IncreaseFromRight(Rectangle r, int size) {
			r.X -= size;
			r.Width += 2 * size;
			return r;
		}
		static public Rectangle IncreaseFromTop(Rectangle r, int size) {
			r.Y -= size;
			r.Height += size;
			return r;
		}
	}
	public class MathUtils {
		public static int FitBounds(int value, int min, int max) {
			int trueMin = Math.Min(min, max);
			int trueMax = Math.Max(min, max);
			return Math.Max(trueMin, Math.Min(value, trueMax));
		}
	}
	public class MemoryUtils {
		public static AppearanceObject[] IncreaseFromHead(AppearanceObject[] array, int count) {
			AppearanceObject[] result = new AppearanceObject[array.Length + count];
			Array.Copy(array, 0, result, count, array.Length);
			return result;
		}
	}
	public class DesignUtils {
		public static void FireChanged(Component component, bool loading, bool designMode, IComponentChangeService serv) {
			if(loading || !designMode) return;
			if(serv != null) {
				serv.OnComponentChanged(component, null, null, null);
			}
		}
	}
}
