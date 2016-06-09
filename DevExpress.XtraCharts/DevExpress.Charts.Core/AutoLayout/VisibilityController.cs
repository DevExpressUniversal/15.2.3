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
namespace DevExpress.Charts.Native {
	public class VisibilityCalculator {
		struct VisibilityLayerKey {
			int priority;
			VisibilityElementOrientation orientation;
			public int Priority { get { return priority; } set { priority = value; } }
			public VisibilityElementOrientation Orientation { get { return orientation; } set { orientation = value; } }
			public VisibilityLayerKey(ISupportVisibilityControlElement element) {
				priority = element.Priority;
				orientation = element.Orientation;
			}
		}
		class VisibilityLayerKeyComparer : IComparer<VisibilityLayerKey> {
			public int Compare(VisibilityLayerKey x, VisibilityLayerKey y) {
				return x.Priority == y.Priority ? ((int)x.Orientation).CompareTo((int)y.Orientation) : x.Priority.CompareTo(y.Priority);
			}
		}
		static bool IsValidSize(double newSize, double size, double minimumSize) {
			return newSize == size || newSize >= minimumSize;
		}
		internal static GRealSize2D DecreaseSizeByWidth(GRealSize2D initialSize, GRealSize2D size) {
			if (size.IsEmpty || double.IsInfinity(size.Width))
				return initialSize;
			double width = initialSize.Width - size.Width;
			return width > 0 ? new GRealSize2D(width, initialSize.Height) : GRealSize2D.Empty;
		}
		internal static GRealSize2D DecreaseSizeByHeight(GRealSize2D initialSize, GRealSize2D size) {
			if (size.IsEmpty || double.IsInfinity(size.Height))
				return initialSize;
			double height = initialSize.Height - size.Height;
			return height > 0 ? new GRealSize2D(initialSize.Width, height) : GRealSize2D.Empty;
		}
		internal static GRealSize2D DecreaseSizeByWidthAndHeight(GRealSize2D initialSize, GRealSize2D size) {
			if (size.IsEmpty || double.IsInfinity(size.Height) || double.IsInfinity(size.Width))
				return initialSize;
			double width = initialSize.Width - size.Width;
			double height = initialSize.Height - size.Height;
			return width > 0 && height > 0 ? new GRealSize2D(width, height) : GRealSize2D.Empty;
		}
		internal static GRealSize2D DecreaseSizeProportional(GRealSize2D initialSize, GRealSize2D size) {
			if (size.IsEmpty || double.IsInfinity(size.Height) || double.IsInfinity(size.Width))
				return initialSize;
			double initialSquare = initialSize.Width * initialSize.Height;
			double square = size.Width * size.Height;
			if (square == 0)
				return initialSize;
			if (initialSquare <= square || initialSize.Width <= size.Width || initialSize.Height <= size.Height)
				return GRealSize2D.Empty;
			double heightToWidthRatio = initialSize.Height / initialSize.Width;
			double newHeight = Math.Sqrt((initialSquare - square) * heightToWidthRatio);
			return new GRealSize2D(newHeight / heightToWidthRatio, newHeight);
		}
		readonly GRealSize2D minimumSize;
		readonly VisibilityLayerKeyComparer visibilityLayerKeyComparer = new VisibilityLayerKeyComparer();
		public VisibilityCalculator(GRealSize2D minimumSize) {
			this.minimumSize = minimumSize;
		}
		void ApplyVisibility(bool[] visibility, List<ISupportVisibilityControlElement> elements) {
			for (int i = 0; i < elements.Count; i++)
				elements[i].Visible = visibility[i];
		}
		void SetVisibility(List<bool[]> visibility, int index, bool value) {
			int i = index;
			foreach (bool[] element in visibility) {
				if (i >= element.Length)
					i -= element.Length;
				else {
					element[i] &= value;
					return;
				}
			}
		}
		bool VisibilityIsEqual(bool[] visibility, List<ISupportVisibilityControlElement> elements) {
			for (int i = 0; i < elements.Count; i++)
				if (visibility[i] != elements[i].Visible)
					return false;
			return true;
		}
		bool IsValidSize(GRealSize2D initialLayoutSize, GRealSize2D layoutSize, GRealSize2D newLayoutSize) {
			double minWidth = Math.Max(0.5 * initialLayoutSize.Width, minimumSize.Width);
			double minHeight = Math.Max(0.5 * initialLayoutSize.Height, minimumSize.Height);
			return !IsValidSize(newLayoutSize.Width, layoutSize.Width, minWidth) ||
					!IsValidSize(newLayoutSize.Height, layoutSize.Height, minHeight);
		}
		bool[] CreateVisibilityMask(int count) {
			bool[] visibility = new bool[count];
			for (int i = 0; i < visibility.Length; i++)
				visibility[i] = true;
			return visibility;
		}
		List<ISupportVisibilityControlElement> GetChangedElements(bool[] visibility, List<ISupportVisibilityControlElement> elements) {
			List<ISupportVisibilityControlElement> changedElements = new List<ISupportVisibilityControlElement>();
			for (int i = 0; i < elements.Count; i++)
				if (visibility[i] != elements[i].Visible)
					changedElements.Add(elements[i]);
			return changedElements;
		}
		SortedList<VisibilityLayerKey, List<ISupportVisibilityControlElement>> GetLayers(List<ISupportVisibilityControlElement> elements) {
			SortedList<VisibilityLayerKey, List<ISupportVisibilityControlElement>> layers = new SortedList<VisibilityLayerKey, List<ISupportVisibilityControlElement>>(visibilityLayerKeyComparer);
			foreach (ISupportVisibilityControlElement element in elements) {
				if (element == null)
					continue;
				List<ISupportVisibilityControlElement> layer;
				VisibilityLayerKey key = new VisibilityLayerKey(element);
				if (layers.ContainsKey(key))
					layer = layers[key];
				else {
					layer = new List<ISupportVisibilityControlElement>();
					layers.Add(key, layer);
				}
				layer.Add(element);
			}
			return layers;
		}
		GRealSize2D CalculateVisibility(GRealSize2D layoutSize, List<ISupportVisibilityControlElement> elements, List<bool[]> visibility) {
			SortedList<VisibilityLayerKey, List<ISupportVisibilityControlElement>> layers = GetLayers(elements);
			List<VisibilityElementOrientation> invisibleOrientations = new List<VisibilityElementOrientation>();
			GRealSize2D initialLayoutSize = layoutSize;
			if (!layoutSize.IsEmpty) {
				foreach (KeyValuePair<VisibilityLayerKey, List<ISupportVisibilityControlElement>> layer in layers) {
					bool isLayerVisible = true;
					if (!invisibleOrientations.Contains(layer.Key.Orientation)) {
						foreach (ISupportVisibilityControlElement element in layer.Value) {
							GRealSize2D newLayoutSize = DecreaseLayoutSize(layoutSize, element.Bounds, element.Orientation);
							if (newLayoutSize.IsEmpty || IsValidSize(initialLayoutSize, layoutSize, newLayoutSize)) {
								isLayerVisible = false;
								invisibleOrientations.Add(layer.Key.Orientation);
								break;
							}
							else
								layoutSize = newLayoutSize;
						}
					}
					else
						isLayerVisible = false;
					foreach (ISupportVisibilityControlElement element in layer.Value) {
						int index = elements.IndexOf(element);
						SetVisibility(visibility, index, isLayerVisible);
					}
				}
			}
			return layoutSize;
		}
		GRealSize2D DecreaseLayoutSize(GRealSize2D layoutSize, GRealRect2D elementBounds, VisibilityElementOrientation elementOrientation) {
			switch (elementOrientation) {
				case VisibilityElementOrientation.Horizontal:
					return DecreaseSizeByHeight(layoutSize, elementBounds.Size);
				case VisibilityElementOrientation.Vertical:
					return DecreaseSizeByWidth(layoutSize, elementBounds.Size);
				case VisibilityElementOrientation.Corner:
					return DecreaseSizeByWidthAndHeight(layoutSize, elementBounds.Size);
				case VisibilityElementOrientation.Inside:
					return DecreaseSizeProportional(layoutSize, elementBounds.Size);
				default:
					throw new ArgumentException("An unknown element orientation.");
			}
		}
		public List<ISupportVisibilityControlElement> CalculateLayout(List<ISupportVisibilityControlElement> chartElements, List<VisibilityLayoutRegion> regions) {
			bool hasRegions = false;
			Dictionary<object, bool> oldVisibility = new Dictionary<object, bool>();
			List<ISupportVisibilityControlElement> changed = new List<ISupportVisibilityControlElement>();
			bool[] chartElementsVisibility = CreateVisibilityMask(chartElements.Count);
			StoreVisibility(oldVisibility, chartElements);
			foreach (VisibilityLayoutRegion region in regions) {
				if (region.Size.IsEmpty)
					continue;
				hasRegions = true;
				GRealSize2D size = region.Size;
				if (region.ElementsToRemove != null && region.ElementsToRemove.Count > 0)
					foreach (ISupportVisibilityControlElement element in region.ElementsToRemove)
						size = DecreaseLayoutSize(size, element.Bounds, element.Orientation);
				bool[] modelVisibility = CreateVisibilityMask(region.Elements.Count);
				List<ISupportVisibilityControlElement> elements = new List<ISupportVisibilityControlElement>();
				elements.AddRange(chartElements);
				elements.AddRange(region.Elements);
				StoreVisibility(oldVisibility, region.Elements);			   
				List<bool[]> visibility = new List<bool[]>();
				visibility.Add(chartElementsVisibility);
				visibility.Add(modelVisibility);
				CalculateVisibility(size, elements, visibility);
				changed.AddRange(GetChangedElements(modelVisibility, region.Elements));
				ApplyVisibility(modelVisibility, region.Elements);
			}
			if (hasRegions) {
				changed.AddRange(GetChangedElements(chartElementsVisibility, chartElements));
				ApplyVisibility(chartElementsVisibility, chartElements);
			}
			return changed.FindAll((x) => { return oldVisibility[x] != x.Visible; });
		}
		void StoreVisibility(Dictionary<object, bool> startVisibility, List<ISupportVisibilityControlElement> list) {
			foreach(ISupportVisibilityControlElement element in list){
				if(!startVisibility.ContainsKey(element))
					startVisibility.Add(element,element.Visible);
			}
		}
	}
	public class VisibilityLayoutRegion {
		readonly List<ISupportVisibilityControlElement> elements;
		readonly List<ISupportVisibilityControlElement> elementsToRemove;
		readonly GRealSize2D size;
		public List<ISupportVisibilityControlElement> Elements { get { return elements; } }
		public List<ISupportVisibilityControlElement> ElementsToRemove { get { return elementsToRemove; } }
		public GRealSize2D Size { get { return size; } }
		public VisibilityLayoutRegion(GRealSize2D size, List<ISupportVisibilityControlElement> elements) : this(size, elements, new List<ISupportVisibilityControlElement>()) { }
		public VisibilityLayoutRegion(GRealSize2D size, List<ISupportVisibilityControlElement> elements, List<ISupportVisibilityControlElement> elementsToRemove) {
			this.size = size;
			this.elements = elements;
			this.elementsToRemove = elementsToRemove;
		}
	}
	public enum ChartElementVisibilityPriority {
		ChartTitle = 0,
		AxisX = 1,
		AxisY = 2,
		SeriesTitle = 3,
		AxisXTitle = 4,
		AxisYTitle = 5,
		Legend = 6,
	}
	public enum VisibilityElementOrientation {
		Horizontal,
		Vertical,
		Corner,
		Inside
	}
	public interface ISupportVisibilityControlElement {
		bool Visible { get; set; }
		int Priority { get; }
		GRealRect2D Bounds { get; }
		VisibilityElementOrientation Orientation { get; }
	}
}
