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
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts.Native {
	public interface ILineOwner {
		Brush Brush { get; }
		int Thickness { get; }
	}
	public interface ITickmarksOwner {
		bool TickmarksVisible { get; }
		bool TickmarksMinorVisible { get; }
		int TickmarksThickness { get; }
		int TickmarksMinorThickness { get; }
		int TickmarksLength { get; }
		int TickmarksMinorLength { get; }
		bool TickmarksCrossAxis { get; }
	}
	public interface ITransformable {
		Transform GeometryTransform { get; }
	}
}
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class Axis2DItem : NotifyPropertyChangedObject {
		static bool CheckLabelItem(AxisLabelItem labelItem, AxisTextItem textItem, bool staggered) {
			if (labelItem.Value != textItem.Value)
				return false;
			labelItem.Visible = textItem.Visible;
			labelItem.IsOutOfRange = !textItem.Visible;
			object labelContent = labelItem.Content;
			string labelString = labelContent as string;
			object itemContent = textItem.Content;
			string itemText = itemContent as string;
			return (labelString != null && itemText != null) ? labelString == itemText : Object.ReferenceEquals(labelContent, itemContent);
		}
		readonly AxisBase axis;
		readonly AxisTitleItem titleItem;
		List<AxisLabelItem> labelItems;
		Axis2DElementLayout layout;
		Axis2DElementLayout scrollBarLayout;
		double titleIndent;
		Rect labelRect;
		Rect axisGeometry;
		List<Rect> majorTickmarksGeometry;
		List<Rect> minorTickmarksGeometry;
		Transform geometryTransform;
		SelectionGeometryItem selectionGeometryItem;
		GRealRect2D boundsCache;
		ITickmarksOwner TickmarksOwner { get { return axis as ITickmarksOwner; } }
		ILineOwner LineOwner { get { return axis as ILineOwner; } }
		public SelectionGeometryItem SelectionGeometryItem { get { return selectionGeometryItem; } }
		public Rect AxisGeometry {
			get { return axisGeometry; }
			set {
				axisGeometry = value;
				OnPropertyChanged("AxisGeometry");
			}
		}
		public List<Rect> MajorTickmarksGeometry {
			get { return majorTickmarksGeometry; }
			set {
				majorTickmarksGeometry = value;
				OnPropertyChanged("MajorTickmarksGeometry");
			}
		}
		public List<Rect> MinorTickmarksGeometry {
			get { return minorTickmarksGeometry; }
			set {
				minorTickmarksGeometry = value;
				OnPropertyChanged("MinorTickmarksGeometry");
			}
		}
		public Transform GeometryTransform {
			get { return geometryTransform; }
			set {
				geometryTransform = value;
				OnPropertyChanged("GeometryTransform");
			}
		}
		public AxisBase Axis { get { return axis; } }
		internal AxisTitleItem TitleItem { get { return titleItem; } }
		internal IList<AxisLabelItem> LabelItems { get { return labelItems; } }
		internal Axis2DElementLayout Layout {
			get {
				Axis2D axis = this.Axis as Axis2D;
				if (axis != null && !axis.ActualVisible)
					return null;
				return layout;
			}
			set { layout = value; }
		}
		Axis2DElementLayout InternalLayout {
			get {
				return layout;
			}
			set { layout = value; }
		}
		internal Rect LabelRect {
			get {
				Axis2D axis = this.Axis as Axis2D;
				if (axis != null && !axis.ActualVisible)
					return Rect.Empty;
				return labelRect;
			}
			set { labelRect = value; }
		}
		Rect InternalLabelRect {
			get {
				return labelRect;
			}
			set { labelRect = value; }
		}
		internal double TitleIndent {
			get { return titleIndent; }
			set { titleIndent = value; }
		}
		internal double TotalThickness { get { return Thickness + ScrollBarThickness + ActualTickmarksCrossLength + MaxTickmarksLength; } }
		internal int TickmarksMajorLength { get { return TickmarksOwner != null && TickmarksOwner.TickmarksVisible ? TickmarksOwner.TickmarksLength : 0; } }
		internal int TickmarksMinorLength { get { return TickmarksOwner != null && TickmarksOwner.TickmarksMinorVisible ? TickmarksOwner.TickmarksMinorLength : 0; } }
		internal int MaxTickmarksLength { get { return Math.Max(TickmarksMajorLength, TickmarksMinorLength); } }
		internal int ActualTickmarksCrossLength { get { return TickmarksOwner != null && TickmarksOwner.TickmarksCrossAxis ? MaxTickmarksLength : 0; } }
		internal int Thickness { get { return LineOwner == null ? 0 : LineOwner.Thickness; } }
		double ScrollBarThickness {
			get {
				if (scrollBarLayout != null && scrollBarLayout.Visible) {
					double thickness = axis.IsVertical ? scrollBarLayout.Bounds.Width : scrollBarLayout.Bounds.Height;
					if (thickness >= 1)
						return thickness - 1;
				}
				return 0.0;
			}
		}
		internal Axis2DItem(AxisBase axis, AxisTitleItem titleItem) {
			this.axis = axis;
			this.titleItem = titleItem;
			this.selectionGeometryItem = new SelectionGeometryItem(axis.SelectionInfo);
			Axis2D axis2D = axis as Axis2D;
			if (axis2D != null) {
				axis2D.GetBoundsDelegate = () => { return GetBoundsDelegate(); };
				axis2D.DecreaseSizeDelegate = (GRealSize2D x) => { return DecreaseSize(x); };
			}
		}
		GRealRect2D GetBoundsDelegate() {
			if (Axis is Axis2D) {
				Axis2D axis2D = Axis as Axis2D;
				if (axis2D.ActualVisible) {
					List<Rect> selectionGeometry = CalculateSelectionGeometry();
					if (selectionGeometry == null || selectionGeometry.Count < 1)
						return GRealRect2D.Empty;
					Rect rect = selectionGeometry[0];
					boundsCache = new GRealRect2D(rect.Left, rect.Top, rect.Width, rect.Height);
					return boundsCache;
				}
				else {
					return boundsCache;
				}
			}
			return GRealRect2D.Empty;
		}
		GRealSize2D DecreaseSize(GRealSize2D layoutSize) {
			List<Rect> selectionGeometry = CalculateSelectionGeometry();
			if (selectionGeometry == null || selectionGeometry.Count < 1)
				return layoutSize;
			Rect rect = selectionGeometry[0];
			if (axis.IsVertical)
				return new GRealSize2D(layoutSize.Width - rect.Width, layoutSize.Height);
			return new GRealSize2D(layoutSize.Width, layoutSize.Height - rect.Height);
		}		
		Rect CreateGeometryRect(double x, double y, double width, double height) {
			return axis.IsVertical ? new Rect(y, x, height, width) : new Rect(x, y, width, height);
		}
		List<Rect> CreateTickmarksGeometry(IAxisMapping axisMapping, List<double> values, int length, int thickness, double offset) {
			int leftPart = thickness / 2;
			int rightPart = leftPart + thickness % 2;
			List<Rect> tickmarksGeometry = new List<Rect>();
			foreach (double value in values) {
				double center = axisMapping.GetRoundedClampedAxisValue(value);
				double startOffset = axisMapping.Clamp(center - leftPart);
				tickmarksGeometry.Add(CreateGeometryRect(startOffset, offset, axisMapping.Clamp(center + rightPart) - startOffset, length));
			}
			return tickmarksGeometry;
		}
		List<Rect> CreateMajorTickmarksGeometry(IAxisMapping axisMapping, GridAndTextDataEx gridAndTextData, double offset) {
			return CreateTickmarksGeometry(axisMapping, gridAndTextData.GridData.Items.VisibleValues, TickmarksOwner.TickmarksLength, TickmarksOwner.TickmarksThickness, offset);
		}
		List<Rect> CreateMinorTickmarksGeometry(IAxisMapping axisMapping, GridAndTextDataEx gridAndTextData, double offset) {
			return CreateTickmarksGeometry(axisMapping, gridAndTextData.GridData.MinorValues, TickmarksOwner.TickmarksMinorLength, TickmarksOwner.TickmarksMinorThickness, offset);
		}
		Rect CreateAxisGeometry(IAxisMapping axisMapping) {
			double offset = 0;
			if (TickmarksOwner != null)
				if (TickmarksOwner.TickmarksCrossAxis)
					offset = MaxTickmarksLength;
			offset += ScrollBarThickness;
			return CreateGeometryRect(0, offset, axisMapping.Lenght + 1, Thickness);
		}
		List<Rect> CreateMajorTickmarksGeometry(IAxisMapping axisMapping, GridAndTextDataEx gridAndTextData) {
			List<Rect> majorTickmarksGeometry = new List<Rect>();
			if (TickmarksOwner != null) {
				double offset = 0;
				if (TickmarksOwner.TickmarksCrossAxis) {
					int tickmarksLength = MaxTickmarksLength;
					if (TickmarksOwner.TickmarksVisible)
						majorTickmarksGeometry.AddRange(CreateMajorTickmarksGeometry(axisMapping, gridAndTextData, tickmarksLength - TickmarksOwner.TickmarksLength));
					offset = tickmarksLength;
				}
				offset += ScrollBarThickness + Thickness;
				if (TickmarksOwner.TickmarksVisible)
					majorTickmarksGeometry.AddRange(CreateMajorTickmarksGeometry(axisMapping, gridAndTextData, offset));
			}
			return majorTickmarksGeometry;
		}
		List<Rect> CreateMinorTickmarksGeometry(IAxisMapping axisMapping, GridAndTextDataEx gridAndTextData) {
			List<Rect> minorTickmarksGeometry = new List<Rect>();
			if (TickmarksOwner != null) {
				double offset = 0;
				if (TickmarksOwner.TickmarksCrossAxis) {
					int tickmarksLength = MaxTickmarksLength;
					if (TickmarksOwner.TickmarksMinorVisible)
						minorTickmarksGeometry.AddRange(CreateMinorTickmarksGeometry(axisMapping, gridAndTextData, tickmarksLength - TickmarksOwner.TickmarksMinorLength));
					offset = tickmarksLength;
				}
				offset += ScrollBarThickness + Thickness;
				if (TickmarksOwner.TickmarksMinorVisible)
					minorTickmarksGeometry.AddRange(CreateMinorTickmarksGeometry(axisMapping, gridAndTextData, offset));
			}
			return minorTickmarksGeometry;
		}
		Transform CreateGeometryTransform() {
			ITransformable transformableAxis = Axis as ITransformable;
			if (transformableAxis != null)
				return transformableAxis.GeometryTransform;
			else
				return new MatrixTransform() { Matrix = Matrix.Identity };
		}
		bool ShouldCreateLabelItems(AxisTextDataEx textData) {
			if (labelItems == null)
				return textData != null;
			if (textData != null) {
				int count = textData.Count;
				if (textData.Count != labelItems.Count)
					return true;
				int i;
				IList<AxisTextItem> items = textData.PrimaryItems;
				count = items.Count;
				for (i = 0; i < count; i++)
					if (!CheckLabelItem(labelItems[i], items[i], false))
						return true;
				items = textData.StaggeredItems;
				count = items.Count;
				for (int j = 0; j < count; i++, j++)
					if (!CheckLabelItem(labelItems[i], items[j], true))
						return true;
			}
			else
				labelItems.Clear();
			return false;
		}
		internal bool UpdateLabelItems(AxisTextDataEx textData) {
			if (!ShouldCreateLabelItems(textData))
				return false;
			AxisLabel label = axis.ActualLabel;
			List<AxisLabelItem> newItems = new List<AxisLabelItem>();
			foreach (AxisTextItem textItem in textData.PrimaryItems)
				newItems.Add(new AxisLabelItem(label, textItem, false));
			foreach (AxisTextItem textItem in textData.StaggeredItems)
				newItems.Add(new AxisLabelItem(label, textItem, true));
			labelItems = newItems;
			return true;
		}
		internal void SetScrollBarLayout(Axis2DElementLayout scrollBarLayout) {
			this.scrollBarLayout = scrollBarLayout;
		}
		internal void UpdateGeometry(GridAndTextDataEx gridAndTextData, Rect axisBounds) {
			if (gridAndTextData == null) {
				AxisGeometry = new Rect(0, 0, 0, 0);
				MajorTickmarksGeometry = new List<Rect>();
				MinorTickmarksGeometry = new List<Rect>();
				GeometryTransform = null;
				selectionGeometryItem.Geometry = new List<Rect>();
			}
			else {
				IAxisMapping axisMapping = axis.CreateMapping(axisBounds);
				AxisGeometry = CreateAxisGeometry(axisMapping);
				MajorTickmarksGeometry = CreateMajorTickmarksGeometry(axisMapping, gridAndTextData);
				MinorTickmarksGeometry = CreateMinorTickmarksGeometry(axisMapping, gridAndTextData);
				GeometryTransform = CreateGeometryTransform();
				selectionGeometryItem.Geometry = CalculateSelectionGeometry();
			}
		}
		public bool ShouldSerializeMajorTickmarksGeometry(XamlDesignerSerializationManager manager) {
			return false;
		}
		public bool ShouldSerializeMinorTickmarksGeometry(XamlDesignerSerializationManager manager) {
			return false;
		}
		public List<Rect> CalculateSelectionGeometry() {
			List<Rect> rectList = new List<Rect>();
			if (InternalLayout != null) {
				if (Axis is CircularAxisX2D) {
					double labelSize = 1;
					if (LabelItems != null) {
						foreach (AxisLabelItem item in LabelItems) {
							if (item.Layout != null) {
								Rect itemBounds = item.Layout.Bounds;
								labelSize = Math.Max(labelSize, MathUtils.CalcDistance(new Point(itemBounds.Left, itemBounds.Top), new Point(itemBounds.Right, itemBounds.Bottom)));
							}
						}
					}
					Rect outerRect = InternalLayout.Bounds.IsEmpty ? Rect.Empty : new Rect(InternalLayout.Bounds.Left - labelSize, InternalLayout.Bounds.Top - labelSize, InternalLayout.Bounds.Width + labelSize * 2, InternalLayout.Bounds.Height + labelSize * 2);
					Rect innerRect = InternalLayout.Bounds.IsEmpty ? Rect.Empty : new Rect(InternalLayout.Bounds.Left, InternalLayout.Bounds.Top, InternalLayout.Bounds.Width, InternalLayout.Bounds.Height);
					rectList.Add(outerRect);
					rectList.Add(innerRect);
				}
				else {
					Rect selectionRect = (InternalLabelRect != RectExtensions.Zero) ? InternalLabelRect : Rect.Empty;
					selectionRect.Union(InternalLayout.Bounds);
					selectionRect.Inflate(VisualSelectionHelper.SelectionRectInflate, VisualSelectionHelper.SelectionRectInflate);
					rectList.Add(selectionRect);
				}
			}
			return rectList;
		}
	}
}
