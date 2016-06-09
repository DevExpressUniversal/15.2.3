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
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraPivotGrid.Localization;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class FieldHeadersPanel : Panel, IFieldHeadersPanel {
		#region static stuff
		public static readonly DependencyProperty IndexProperty;
		public static readonly DependencyProperty SkipWidthProperty;
		public static readonly DependencyProperty MeasureModeProperty;
		public static readonly DependencyProperty IsCuttedProperty;
		static readonly DependencyPropertyKey IsCuttedPropertyKey;
		static FieldHeadersPanel() {
			Type ownerType = typeof(FieldHeadersPanel);
			IndexProperty = DependencyPropertyManager.RegisterAttached("Index", typeof(int),
				ownerType, new FrameworkPropertyMetadata(-1));
			SkipWidthProperty = DependencyPropertyManager.Register("SkipWidth", typeof(int),
				ownerType, new FrameworkPropertyMetadata(0, (d, e) => ((FieldHeadersPanel)d).OnMeasureModeChanged()));
			MeasureModeProperty = DependencyPropertyManager.Register("MeasureMode", typeof(FieldHeadersMeasureMode),
				ownerType, new FrameworkPropertyMetadata(FieldHeadersMeasureMode.Default, (d, e) => ((FieldHeadersPanel)d).OnMeasureModeChanged()));
			IsCuttedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsCutted", typeof(bool),
				ownerType, new FrameworkPropertyMetadata(false));
			IsCuttedProperty = IsCuttedPropertyKey.DependencyProperty;
		}
		public static int GetIndex(DependencyObject d) {
			if(d == null)
				throw new ArgumentNullException("d");
			return (int)d.GetValue(IndexProperty);
		}
		public static void SetIndex(DependencyObject d, int value) {
			if(d == null)
				throw new ArgumentNullException("d");
			d.SetValue(IndexProperty, value);
		}
		#endregion
		public FieldHeadersPanel() {
			OnMeasureModeChanged();
		}
		FieldHeadersPanelMeasureStrategyBase measureStrategy;
		void OnMeasureModeChanged() {
			switch(MeasureMode) {
				case FieldHeadersMeasureMode.Default:
				case FieldHeadersMeasureMode.Split:
					measureStrategy = new FieldHeadersPanelSplitMeasureStrategy();
					break;
				case FieldHeadersMeasureMode.None:
					measureStrategy = new FieldHeadersPanelFixedMeasureStrategy();
					break;
				case FieldHeadersMeasureMode.Cut:
					measureStrategy = new FieldHeadersPanelCutMeasureStrategy();
					break;
				case FieldHeadersMeasureMode.MultiLine:
					measureStrategy = new FieldHeadersPanelMultiLineMeasureStrategy();
					break;
				default:
					throw new NotImplementedException();
			}
			IsCutted = false;
			InvalidateArrange();
			InvalidateMeasure();
		}
		public int SkipWidth {
			get { return (int)GetValue(SkipWidthProperty); }
			set { SetValue(SkipWidthProperty, value); }
		}
		public FieldHeadersMeasureMode MeasureMode {
			get { return (FieldHeadersMeasureMode)GetValue(MeasureModeProperty); }
			set { SetValue(MeasureModeProperty, value); }
		}
		public bool IsCutted {
			get { return (bool)GetValue(IsCuttedProperty); }
			private set { this.SetValue(IsCuttedPropertyKey, value); }
		}
		public List<UIElement> SortedChildren {
			get {
				UIElement[] children = GetSortedChildren(Children);
				List<UIElement> res = new List<UIElement>(children.Length);
				for(int i = 0; i < children.Length; i++) {
					if(children[i].Visibility == Visibility.Visible && children[i] as FieldHeaderBase != null)
						res.Add(children[i]);
				}
				return res;
			}
		}
		protected override Size MeasureOverride(Size constraint) {
			return measureStrategy.MeasureOverride(constraint, this);
		}
		protected override Size ArrangeOverride(Size constraint) {
			return measureStrategy.ArrangeOverride(constraint, this);
		}
		protected UIElement[] GetSortedChildren(UIElementCollection collection) {
			UIElement[] res = new UIElement[collection.Count];
			collection.CopyTo(res, 0);
			int indexedElementsCount = GetIndexedElementsCount(res);
			if(indexedElementsCount == 0)
				return res;
			Array.Sort<UIElement>(res, delegate(UIElement x, UIElement y) {
				int i1 = GetIndex(x), i2 = GetIndex(y);
				return Comparer<int>.Default.Compare(i1, i2);
			});
			return res;
		}
		int GetIndexedElementsCount(UIElement[] array) {
			int count = 0;
			for(int i = 0; i < array.Length; i++) {
				if(GetIndex(array[i]) >= 0)
					count++;
			}
			return count;
		}
		class FieldHeadersPanelMultiLineMeasureStrategy : FieldHeadersPanelSplitMeasureStrategy {
			protected override Size GetElementDesiredSize(IFieldHeadersPanel panel, UIElement element, Size availableSize, Size constrait, ref double widthD) {
				Size elSize;
				elSize = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
				element.Measure(elSize);
				elSize = element.DesiredSize;
				return elSize;
			}
			protected override bool GetIsNeedsResize(IFieldHeadersPanel panel, ref Size constraint, ref Size size) {
				return false;
			}
			protected override Size MeasureOverrideCore(Size resizeProportion, IFieldHeadersPanel panel, Size constrait) {
				Size size = new Size();
				IList<UIElement> internalChildren = panel.SortedChildren;
				int count = internalChildren.Count;
				double needClearSize = panel.SkipWidth;
				double widthD = 0;
				double curWidth = 0;
				double curHeight = 0;
				List<Dictionary<UIElement, Size>> lines = new List<Dictionary<UIElement, Size>>();
				int curLine = 0;
				lines.Add(new Dictionary<UIElement, Size>());
				for(int i = 0; i < count; i++) {
					UIElement element = internalChildren[i];
					if(element == null)
						continue;
					Size elSize = GetElementDesiredSize(panel, element, resizeProportion, constrait, ref widthD);
					if(needClearSize > 0) {
						needClearSize -= elSize.Width;
						elSize.Width = 0;
					}
					if(needClearSize < 0) {
						elSize.Width = -needClearSize;
						needClearSize = 0;
					}
					if(elSize.Width < MinElementWidth)
						elSize.Width = 0;
					curWidth = curWidth + elSize.Width; 
					if(curWidth > constrait.Width) {
						if(curWidth != elSize.Width)
							curWidth = elSize.Width;
						else
							curWidth = 0;
						size.Height += curHeight + 2;
						curHeight = 0;
						curLine++;
						lines.Add(new Dictionary<UIElement, Size>()); 
					} 
					  lines[curLine].Add(element, elSize);
					  curHeight = Math.Max(curHeight, elSize.Height);
				}
				size.Height += curHeight;
				return new Size(Double.IsPositiveInfinity(constrait.Width) ? 0 : lines.Count > 1 ? constrait.Width : curWidth, Math.Min(size.Height, constrait.Height));
			}
			public override Size ArrangeOverride(Size constraint, IFieldHeadersPanel panel) {
				IList<UIElement> children = panel.SortedChildren;
				int count = children.Count;
				double maxHeight = 0;
				Rect finalRect = new Rect();
				List<Dictionary<UIElement, Rect>> lines = new List<Dictionary<UIElement, Rect>>();
				int curLine = 0;
				lines.Add(new Dictionary<UIElement, Rect>());
				for(int i = 0; i < count; i++) {
					UIElement element = children[i];
					if(element == null)
						continue;
					Size sz = element.DesiredSize;
					finalRect.Width = sz.Width;
					if(finalRect.X + finalRect.Width > constraint.Width) {
						finalRect.X = 0;
						finalRect.Y += maxHeight + 2;
						maxHeight = 0;
						curLine++;
						lines.Add(new Dictionary<UIElement, Rect>());
					} else {
					}
					Rect arrangeRect = new Rect(finalRect.X, finalRect.Y, sz.Width, sz.Height);
					if(!Double.IsPositiveInfinity(constraint.Width) && arrangeRect.Left > constraint.Width)
						arrangeRect.Width = 0;
					else
						if(!Double.IsPositiveInfinity(constraint.Width) && arrangeRect.Right > constraint.Width)
							arrangeRect.Width = constraint.Width - arrangeRect.Left;
					if(arrangeRect.Width < MinElementWidth)
						arrangeRect.Width = 0;
					lines[curLine].Add(element, arrangeRect);
					maxHeight = Math.Max(maxHeight, arrangeRect.Height);
					finalRect.X += finalRect.Width;
				}
				for(int i = 0; i < lines.Count; i++) {
					double height = 0;
					foreach(KeyValuePair<UIElement, Rect> el in lines[i])
						height = Math.Max(height, el.Value.Height);
					foreach(KeyValuePair<UIElement, Rect> el in lines[i])
						el.Key.Arrange(new Rect(el.Value.Location(), new Size(el.Value.Width, height)));
				}
				return constraint;
			}
		}
		bool IFieldHeadersPanel.IsCutted {
			set { IsCutted = value; }
		}
		class FieldHeadersPanelCutMeasureStrategy : FieldHeadersPanelSplitMeasureStrategy {
			double MinSizeToHeader = 55;
			bool showAll;
			protected override bool GetIsNeedsResize(IFieldHeadersPanel panel, ref Size constraint, ref Size size) {
				bool resize = base.GetIsNeedsResize(panel, ref constraint, ref size);
				if(resize && constraint.Width / panel.SortedChildren.Count < MinSizeToHeader)
					showAll = false;
				else
					showAll = true;
				panel.IsCutted = !showAll;
				return resize;
			}
			protected override Size GetElementDesiredSize(IFieldHeadersPanel panel, UIElement element, Size resizeProportion, Size constrait, ref double widthD) {
				Size size = base.GetElementDesiredSize(panel, element, resizeProportion, constrait, ref widthD);
				if(showAll == false && resizeProportion.Width != 1)
					size.Width = 0;
				return size;
			}
		}
		class FieldHeadersPanelSplitMeasureStrategy : FieldHeadersPanelMeasureStrategyBase {
		}
		class FieldHeadersPanelFixedMeasureStrategy : FieldHeadersPanelMeasureStrategyBase {
			protected override Size GetElementDesiredSize(IFieldHeadersPanel panel, UIElement element, Size availableSize, Size constrait, ref double widthD) {
				Size elSize;
				elSize = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
				element.Measure(elSize);
				elSize = element.DesiredSize;
				return elSize;
			}
			protected override void MeasureNotResizing(IFieldHeadersPanel panel) {
				if(panel.SkipWidth > 0)
					MeasureSkipWidth(panel);
			}
			protected override bool GetIsNeedsResize(IFieldHeadersPanel panel, ref Size constraint, ref Size size) {
				return false;
			}
		}
		abstract class FieldHeadersPanelMeasureStrategyBase {
			protected const double MinElementWidth = 10;
			protected bool doResizing;
			public FieldHeadersPanelMeasureStrategyBase() { }
			public Size MeasureOverride(Size constraint, IFieldHeadersPanel panel) {
				Size size = MeasureOverrideCore(GetFlexResizeProportion(constraint), panel, constraint);
				doResizing = GetIsNeedsResize(panel, ref constraint, ref size);
				if(doResizing) {
					Size propotion = GetFixedResizedProportion(constraint, panel);
					size = MeasureOverrideCore(propotion, panel, constraint);
					size.Width = constraint.Width;
				} else {
					MeasureNotResizing(panel);
				}
				return size;
			}
			protected virtual bool GetIsNeedsResize(IFieldHeadersPanel panel, ref Size constraint, ref Size size) {
				return size.Width > (constraint.Width - panel.SkipWidth) && size.Width > panel.SkipWidth;
			}
			protected virtual void MeasureNotResizing(IFieldHeadersPanel panel) { }
			public virtual Size ArrangeOverride(Size constraint, IFieldHeadersPanel panel) {
				IList<UIElement> children = panel.SortedChildren;
				int count = children.Count;
				Rect finalRect = new Rect();
				for(int i = 0; i < count; i++) {
					UIElement element = children[i];
					if(element == null)
						continue;
					Size sz = element.DesiredSize;
					sz.Height = constraint.Height;
					finalRect.Width = sz.Width;
					finalRect.Height = sz.Height;
					Rect arrangeRect = new Rect(finalRect.X, finalRect.Y, finalRect.Width, finalRect.Height);
					if(!Double.IsPositiveInfinity(constraint.Width) && arrangeRect.Left > constraint.Width)
						arrangeRect.Width = 0;
					else
						if(!Double.IsPositiveInfinity(constraint.Width) && arrangeRect.Right > constraint.Width)
							arrangeRect.Width = constraint.Width - arrangeRect.Left;
					if(arrangeRect.Width < MinElementWidth)
						arrangeRect.Width = 0;
					if(minusLeft != 0 && sz.Width > 0) {
						arrangeRect.Location = new Point(arrangeRect.Left + minusLeft, arrangeRect.Top);
					}
					element.Arrange(arrangeRect);
					finalRect.X += finalRect.Width;
				}
				return constraint;
			}
			double minusLeft = 0;
			protected void MeasureSkipWidth(IFieldHeadersPanel panel) {
				minusLeft = 0;
				Size size = new Size();
				IList<UIElement> internalChildren = panel.SortedChildren;
				int count = internalChildren.Count;
				double needClearSize = panel.SkipWidth;
				for(int i = 0; i < count; i++) {
					UIElement element = internalChildren[i];
					if(element == null || needClearSize <= 0)
						continue;
					size = element.DesiredSize;
					needClearSize -= size.Width;
					if(needClearSize < 0) {
						minusLeft = - needClearSize - size.Width;
						needClearSize = 0;
					} else {
						size.Width = 0;
					}
					if(size.Width < MinElementWidth)
						size.Width = 0;
					element.Measure(size);
				}
			}
			protected virtual Size MeasureOverrideCore(Size resizeProportion, IFieldHeadersPanel panel, Size constrait) {
				Size size = new Size();
				IList<UIElement> internalChildren = panel.SortedChildren;
				int count = internalChildren.Count;
				double needClearSize = panel.SkipWidth;
				double widthD = 0;
				for(int i = 0; i < count; i++) {
					UIElement element = internalChildren[i];
					if(element == null)
						continue;
					Size elSize = GetElementDesiredSize(panel, element, resizeProportion, constrait, ref widthD);
					if(needClearSize > 0 && resizeProportion.Width != 1) {
						needClearSize -= elSize.Width;
						elSize.Width = 0;
					}
					if(needClearSize < 0) {
						elSize.Width = -needClearSize;
						needClearSize = 0;
					}
					if(elSize.Width < MinElementWidth)
						elSize.Width = 0;
					element.Measure(elSize);
					Size desiredSize = element.DesiredSize;
					widthD += elSize.Width - element.DesiredSize.Width;
					size.Width += desiredSize.Width;
					size.Height = Math.Max(size.Height, desiredSize.Height);
				}
				return size;
			}
			protected virtual Size GetElementDesiredSize(IFieldHeadersPanel panel, UIElement element, Size resizeProportion, Size constrait, ref double widthD) {
				element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Size elSize;
				if(resizeProportion.Width != 1)
					elSize = new Size(element.DesiredSize.Width * resizeProportion.Width + widthD, element.DesiredSize.Height * resizeProportion.Height);
				else
					elSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
				widthD = elSize.Width - Math.Floor(elSize.Width);
				elSize.Width = Math.Floor(elSize.Width);
				return elSize;
			}
			Size GetFixedResizedProportion(Size constraint, IFieldHeadersPanel panel) {
				double desiredWidth = 0;
				for(int i = 0; i < panel.SortedChildren.Count; i++)
					desiredWidth += panel.SortedChildren[i].DesiredSize.Width;
				Size availableSize = new Size(1, 1);
				if(doResizing)
					availableSize.Width = desiredWidth != 0 ? (constraint.Width + panel.SkipWidth) / desiredWidth : 1;
				else
					availableSize.Width = 1;
				return availableSize;
			}
			Size GetFlexResizeProportion(Size constraint) {
				return new Size(1, 1);
			}
		}
	}
	public enum FieldHeadersMeasureMode {
		Default,
		None,
		Split,
		MultiLine,
		Cut
	}
	interface IFieldHeadersPanel {
		int SkipWidth { get; }
		List<UIElement> SortedChildren { get; }
		bool IsCutted { set; }
	}
}
