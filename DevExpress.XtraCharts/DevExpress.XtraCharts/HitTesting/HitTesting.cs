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

using DevExpress.Charts.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraCharts.Native {
	public class HitTestParams : IDisposable {
		IHitTest obj;
		object additionalObj;
		IHitRegion hitRegion;
		bool useSpecificCursor;
		public IHitTest Object { get { return obj; } }
		public object AdditionalObj { get { return additionalObj; } }
		public IHitRegion HitRegion { get { return hitRegion; } }
		public bool UseSpecificCursor { get { return useSpecificCursor; } }
		public HitTestParams(IHitTest obj, object additionalObj, bool useSpecificCursor, IHitRegion hitRegion) {
			this.obj = obj;
			this.additionalObj = additionalObj;
			this.hitRegion = hitRegion;
			this.useSpecificCursor = useSpecificCursor;
		}
		public HitTestParams(IHitTest obj, object additionalObj, IHitRegion hitRegion)
			: this(obj, additionalObj, false, hitRegion) {
		}
		public void Dispose() {
			if (hitRegion != null) {
				hitRegion.Dispose();
				hitRegion = null;
			}
		}
		public bool HitTest(PointF hitPoint) {
			return hitRegion.IsVisible(hitPoint);
		}
	}
	public class HitTestController {
		Chart chart;
		List<HitTestParams> objs;
		List<HitTestParams> specificCursorObjs;
		public bool Enabled { get { return chart != null && chart.HitTestingEnabled; } }
		public IHitTest Selected {
			get {
				foreach (HitTestParams prms in objs)
					if (prms.Object.State.Select)
						return prms.Object;
				return null;
			}
		}
		public IHitTest Hot {
			get {
				foreach (HitTestParams prms in objs)
					if (prms.Object.State.Hot)
						return prms.Object;
				return null;
			}
		}
		public IList<HitTestParams> Items { get { return GetItems(false); } }
		public HitTestController(Chart chart) {
			this.chart = chart;
			objs = new List<HitTestParams>();
			specificCursorObjs = new List<HitTestParams>();
		}
		static object FindInternalHitTestObject(List<HitTestParams> hitTestObjs, Point pt, Type type) {
			for (int i = hitTestObjs.Count - 1; i >= 0; i--) {
				HitTestParams prms = (HitTestParams)hitTestObjs[i];
				if (prms.HitTest(pt))
					if (prms.AdditionalObj != null && prms.AdditionalObj.GetType().Equals(type))
						return prms.AdditionalObj;
			}
			return null;
		}
		List<HitTestParams> FilterInternalHitTestObjects(IList<HitTestParams> list, Type type) {
			List<HitTestParams> removedHitTestObjects = new List<HitTestParams>();
			for (int i = 0; i < list.Count; )
				if (list[i].AdditionalObj != null && list[i].AdditionalObj.GetType().Equals(type)) {
					removedHitTestObjects.Add(list[i]);
					list.RemoveAt(i);
				}
				else
					i++;
			return removedHitTestObjects;
		}
		List<HitTestParams> FilterInternalHitTestObjects(IList<HitTestParams> list) {
			FilterInternalHitTestObjects(list, typeof(ChartScrollParameters));
			List<HitTestParams> chartFocusedAreaParams = FilterInternalHitTestObjects(list, typeof(ChartFocusedArea));
			return chartFocusedAreaParams;
		}
		public bool SeriesToolTipEnabled(SeriesBase series) {
			return chart != null && chart.SeriesToolTipEnabled && series.IsToolTipsSupported && series.ActualToolTipEnabled;
		}
		public bool PointToolTipEnabled(SeriesBase series) {
			return chart != null && chart.PointToolTipEnabled && series.IsToolTipsSupported && series.ActualToolTipEnabled;
		}
		public void Clear() {
			foreach (HitTestParams prms in objs)
				prms.Dispose();
			objs.Clear();
			specificCursorObjs.Clear();
		}
		public void Register(HitTestParams prms) {
			objs.Add(prms);
			if (prms.UseSpecificCursor)
				specificCursorObjs.Add(prms);
		}
		public ChartScrollParameters FindScrollCommand(Point pt) {
			return (ChartScrollParameters)FindInternalHitTestObject(objs, pt, typeof(ChartScrollParameters));
		}
		public ChartFocusedArea FindFocusedArea(Point pt) {
			return (ChartFocusedArea)FindInternalHitTestObject(objs, pt, typeof(ChartFocusedArea));
		}
		public ChartFocusedArea FindSpecificCursorArea(Point pt) {
			return (ChartFocusedArea)FindInternalHitTestObject(specificCursorObjs, pt, typeof(ChartFocusedArea));
		}
		public IList<HitTestParams> Find(Point pt) {
			List<HitTestParams> list = new List<HitTestParams>();
			for (int i = objs.Count - 1; i >= 0; i--) {
				HitTestParams prms = (HitTestParams)objs[i];
				if (prms.HitTest(pt))
					list.Add(prms);
			}
			FilterInternalHitTestObjects(list);
			return list;
		}
		public IList<HitTestParams> GetItems(bool filterInternalHitTestObjects) {
			List<HitTestParams> hitTestParams = new List<HitTestParams>(objs);
			if (filterInternalHitTestObjects)
				FilterInternalHitTestObjects(hitTestParams);
			return hitTestParams;
		}
		public IList<HitTestParams> GetItems(bool filterInternalHitTestObjects, out List<HitTestParams> filteredItems) {
			List<HitTestParams> hitTestParams = new List<HitTestParams>(objs);
			if (filterInternalHitTestObjects)
				filteredItems = FilterInternalHitTestObjects(hitTestParams);
			else
				filteredItems = null;
			return hitTestParams;
		}
	}
	public class HitTestState {
		bool hot = false;
		bool select = false;
		bool masterSelect = false;
		object hotObject = null;
		bool objectMasterSelect = false;
		protected bool IsHot { get { return hot; } }
		protected object HotObject { get { return hotObject; } }
		protected bool ObjectMasterSelect { get { return objectMasterSelect; } }
		protected bool MasterSelect { get { return masterSelect; } }
		readonly List<object> selectObjects = new List<object>();
		internal List<object> SelectedObjects { get { return selectObjects; } }
		public HatchStyle HatchStyle { get { return HatchStyle.WideUpwardDiagonal; } }
		public bool Hot { get { return hot && !masterSelect; } }
		public virtual bool Select { get { return (!hot && !select && masterSelect) || (!hot && select) || (hot && masterSelect); } }
		public bool Normal { get { return !Hot && !Select; } }
		public Color ActualColor {
			get {
				if (Hot) return HitTestColors.Hot;
				if (Select) return HitTestColors.Selected;
				return Color.Empty;
			}
		}
		public Color HatchColor {
			get {
				if (Hot) return HitTestColors.HotHatch;
				if (Select) return HitTestColors.SelectHatch;
				return Color.Transparent;
			}
		}
		public Color HatchColorLight {
			get {
				if (Hot) return HitTestColors.HotHatchLight;
				if (Select) return HitTestColors.SelectHatchLight;
				return Color.Transparent;
			}
		}
		protected bool IsSelected(object innerObject) {
			for (int i = 0; i < SelectedObjects.Count; i++) {
				if (Object.ReferenceEquals(innerObject, SelectedObjects[i]))
					return true;
			}
			return false;
		}
		public void DoEnter(object innerObject) {
			this.hot = true;
			if (innerObject != null) {
				if (!object.ReferenceEquals(innerObject, this.hotObject))
					this.objectMasterSelect = false;
				this.hotObject = innerObject;
			}
		}
		public void DoSelect(object innerObject) {
			DoSelect(innerObject, true);
		}
		public void DoSelect(object innerObject, bool setMasterSelection) {
			DoSelect(innerObject, setMasterSelection, ElementSelectionMode.Single);
		}
		public void DoSelect(object innerObject, bool setMasterSelection, ElementSelectionMode selectionMode) {
			this.masterSelect = setMasterSelection;
			this.select = true;
			if (innerObject != null) {
				if (!IsSelected(innerObject))
					this.selectObjects.Add(innerObject);
				this.objectMasterSelect = setMasterSelection;
			}
		}
		public void DoLeave() {
			this.hot = false;
			this.hotObject = null;
			this.masterSelect = false;
			this.objectMasterSelect = false;
		}
		public void UndoSelect(bool clearHot) {
			if (clearHot) {
				this.hot = false;
				this.hotObject = null;
			}
			this.select = false;
			this.masterSelect = false;
			this.objectMasterSelect = false;
			this.selectObjects.Clear();
		}
		public void UndoSelect(object innerObject, bool clearHot) {
			if (innerObject != null) {
				if (clearHot && object.ReferenceEquals(innerObject, this.hotObject)) {
					this.hot = false;
					this.hotObject = null;
				}
				if (IsSelected(innerObject)) {
					this.select = false;
					this.masterSelect = false;
					this.objectMasterSelect = false;
					this.selectObjects.Remove(innerObject);
				}
			}
		}
	}
	public class SeriesHitTestState : HitTestState {
		SelectionState legendSelectionState;
		public ISeriesBase Series { get; set; }
		public override bool Select { get { return ((!Hot && !IsSelected(Series) && MasterSelect) || (!Hot && IsSelected(Series)) || (Hot && MasterSelect)); } }
		public SelectionState LegendSelectionState {
			get {
				if (legendSelectionState != SelectionState.Normal)
					return legendSelectionState;
				if (Hot)
					return SelectionState.HotTracked;
				if (SelectedObjects.Count > 0)
					return SelectionState.Selected;
				return SelectionState.Normal;
			}
			set {
				legendSelectionState = value;
			}
		}
		public bool IsSeriesHot(SeriesSelectionMode selectionMode) {
			return selectionMode == SeriesSelectionMode.Series && IsHot && !MasterSelect;
		}
		public bool IsSeriesSelect(SeriesSelectionMode selectionMode) {
			bool isHot = IsSeriesHot(selectionMode);
			bool isSelect = IsSelected(Series);
			return ((!isHot && isSelect) || (isHot && MasterSelect)); 
		}
		public virtual bool IsPointHot(ISeriesPoint seriesPoint, SeriesSelectionMode selectionMode) {
			if (selectionMode == SeriesSelectionMode.Series)
				return Hot;
			return seriesPoint != null && object.ReferenceEquals(seriesPoint, this.HotObject) && !ObjectMasterSelect;
		}
		public virtual bool IsPointSelect(ISeriesPoint seriesPoint, SeriesSelectionMode selectionMode) {
			return
				seriesPoint != null ?
				IsSelected(seriesPoint) && !IsPointHot(seriesPoint, selectionMode) ||
				(selectionMode == SeriesSelectionMode.Series && IsSeriesSelect(selectionMode) && !IsSeriesHot(selectionMode)) :
				Select;
		}
		public Color GetPointHatchColor(SeriesSelectionMode mode, SelectionState state) {
			if (IsSeriesSelect(mode))
				return HatchColor;
			switch (state) {
				case SelectionState.HotTracked:
					return HitTestColors.HotHatch;
				case SelectionState.Selected:
					return HitTestColors.SelectHatch;
				case SelectionState.Normal:
				default:
					return Color.Transparent;
			}
		}
		public Color GetPointHatchColor(SelectionState state) {
			if (Select)
				return HatchColor;
			switch (state) {
				case SelectionState.HotTracked:
					return HitTestColors.HotHatch;
				case SelectionState.Selected:
					return HitTestColors.SelectHatch;
				case SelectionState.Normal:
				default:
					return Color.Transparent;
			}
		}
		public Color GetHatchColorLight(SelectionState selectionState) {
			switch (selectionState) {
				case SelectionState.HotTracked:
					return HitTestColors.HotHatchLight;
				case SelectionState.Selected:
					return HitTestColors.SelectHatchLight;
				case SelectionState.Normal:
				default:
					return Color.Transparent;
			}
		}
	}
	public class PointHitTestState : SeriesHitTestState {
		public override bool IsPointSelect(ISeriesPoint seriesPoint, SeriesSelectionMode selectionMode) {
			bool isSeriesSelected = Select && (SelectedObjects.Count == 0 || IsSelected(Series));
			return
				seriesPoint != null ?
				(isSeriesSelected || IsSelected(seriesPoint)) && !IsPointHot(seriesPoint, selectionMode) :
				Select;
		}
		public override bool IsPointHot(ISeriesPoint seriesPoint, SeriesSelectionMode selectionMode) {
			if (selectionMode == SeriesSelectionMode.Series)
				return Hot;
			return seriesPoint != null && object.ReferenceEquals(seriesPoint, this.HotObject) && !ObjectMasterSelect;
		}
	}
	public class ChartHitTestEnumerator : IEnumerator, IEnumerable {
		#region inner classes
		abstract class HitTestEnumeratorItem {
			static HitTestEnumeratorItem CreateEnumeratorItem(IHitTest hitElement) {
				if (hitElement is Chart)
					return new ChartHitTestEnumeratorItem((Chart)hitElement);
				if (hitElement is XYDiagram2D)
					return new XYDiagram2DHitTestEnumeratorItem((XYDiagram2D)hitElement);
				if (hitElement is RadarDiagram)
					return new RadarDiagramHitTestEnumeratorItem((RadarDiagram)hitElement);
				if (hitElement is Axis2D)
					return new Axis2DHitTestEnumeratorItem((Axis2D)hitElement);
				if (hitElement is Series)
					return new SeriesHitTestEnumeratorItem((Series)hitElement);
				return null;
			}
			IHitTest rootElement;
			ArrayList childrenList = new ArrayList();
			HitTestEnumeratorItem childEnumeratorItem = null;
			int index = -1;
			protected IHitTest RootElement { get { return this.rootElement; } }
			protected ArrayList ChildrenList { get { return this.childrenList; } }
			public HitTestEnumeratorItem(IHitTest rootElement) {
				this.rootElement = rootElement;
				FillChildrenList();
			}
			protected abstract void FillChildrenList();
			public IHitTest NextElement() {
				if (this.index == -1) {
					this.index++;
					return this.rootElement;
				}
				if (this.childEnumeratorItem != null) {
					IHitTest nextElement = this.childEnumeratorItem.NextElement();
					if (nextElement != null)
						return nextElement;
				}
				if (this.index < this.childrenList.Count) {
					IHitTest childElement = (IHitTest)this.childrenList[index++];
					this.childEnumeratorItem = CreateEnumeratorItem(childElement);
					if (this.childEnumeratorItem != null)
						return this.childEnumeratorItem.NextElement();
					else
						return childElement;
				}
				return null;
			}
		}
		class ChartHitTestEnumeratorItem : HitTestEnumeratorItem {
			protected Chart Chart { get { return (Chart)base.RootElement; } }
			protected DataContainer DataContainer { get { return Chart.DataContainer; } }
			public ChartHitTestEnumeratorItem(Chart chart)
				: base(chart) {
			}
			protected override void FillChildrenList() {
				XYDiagram2D xyDiagram = Chart.Diagram as XYDiagram2D;
				if (xyDiagram != null)
					ChildrenList.Add(xyDiagram);
				RadarDiagram radarDiagram = Chart.Diagram as RadarDiagram;
				if (radarDiagram != null)
					ChildrenList.Add(radarDiagram);
				ChildrenList.Add(DataContainer.SeriesTemplate);
				foreach (Series se in Chart.Series)
					ChildrenList.Add(se);
				foreach (ChartTitle title in Chart.Titles)
					ChildrenList.Add(title);
				foreach (Annotation annotation in Chart.AnnotationRepository)
					ChildrenList.Add(annotation);
				ChildrenList.Add(Chart.Legend);
			}
		}
		class XYDiagram2DHitTestEnumeratorItem : HitTestEnumeratorItem {
			protected XYDiagram2D Diagram { get { return (XYDiagram2D)base.RootElement; } }
			public XYDiagram2DHitTestEnumeratorItem(XYDiagram2D diagram)
				: base(diagram) {
			}
			protected override void FillChildrenList() {
				ChildrenList.Add(Diagram.ActualAxisX);
				ChildrenList.Add(Diagram.ActualAxisY);
				foreach (Axis2D axis in Diagram.ActualSecondaryAxesX)
					ChildrenList.Add(axis);
				foreach (Axis2D axis in Diagram.ActualSecondaryAxesY)
					ChildrenList.Add(axis);
				foreach (XYDiagramPane pane in Diagram.Panes)
					ChildrenList.Add(pane);
			}
		}
		class RadarDiagramHitTestEnumeratorItem : HitTestEnumeratorItem {
			protected RadarDiagram Diagram { get { return (RadarDiagram)base.RootElement; } }
			public RadarDiagramHitTestEnumeratorItem(RadarDiagram diagram)
				: base(diagram) {
			}
			protected override void FillChildrenList() {
				ChildrenList.Add(Diagram.AxisX);
				ChildrenList.Add(Diagram.AxisY);
			}
		}
		class Axis2DHitTestEnumeratorItem : HitTestEnumeratorItem {
			protected Axis2D Axis { get { return (Axis2D)base.RootElement; } }
			public Axis2DHitTestEnumeratorItem(Axis2D axis)
				: base(axis) {
			}
			protected override void FillChildrenList() {
				foreach (ConstantLine contLine in Axis.ConstantLines)
					ChildrenList.Add(contLine);
			}
		}
		class SeriesHitTestEnumeratorItem : HitTestEnumeratorItem {
			protected Series Series { get { return (Series)base.RootElement; } }
			public SeriesHitTestEnumeratorItem(Series series)
				: base(series) {
			}
			void FillChildrenListForSimpleDiagram(SimpleDiagramSeriesViewBase view) {
				foreach (SeriesTitle title in view.Titles)
					ChildrenList.Add(title);
			}
			void FillChildrenListForXYDiagram(XYDiagram2DSeriesViewBase view) {
				foreach (Indicator indicator in view.Indicators)
					ChildrenList.Add(indicator);
			}
			protected override void FillChildrenList() {
				if (Series.Label != null)
					ChildrenList.Add(Series.Label);
				if (Series.View is SimpleDiagramSeriesViewBase)
					FillChildrenListForSimpleDiagram((SimpleDiagramSeriesViewBase)Series.View);
				else if (Series.View is XYDiagram2DSeriesViewBase)
					FillChildrenListForXYDiagram((XYDiagram2DSeriesViewBase)Series.View);
			}
		}
		#endregion
		Chart chart;
		ChartHitTestEnumeratorItem chartEnumeratorItem;
		IHitTest currentElement;
		public ChartHitTestEnumerator(Chart chart) {
			this.chart = chart;
			((IEnumerator)this).Reset();
		}
		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator() {
			((IEnumerator)this).Reset();
			return this;
		}
		#endregion
		#region IEnumerator implementation
		object IEnumerator.Current { get { return this.currentElement; } }
		void IEnumerator.Reset() {
			this.currentElement = null;
			this.chartEnumeratorItem = new ChartHitTestEnumeratorItem(this.chart);
		}
		bool IEnumerator.MoveNext() {
			this.currentElement = this.chartEnumeratorItem.NextElement();
			return this.currentElement != null;
		}
		#endregion
		public void DoLeave(IHitTest exceptElement) {
			foreach (IHitTest hitElement in this)
				if (exceptElement == null || hitElement.Object != exceptElement.Object)
					hitElement.State.DoLeave();
		}
		public void UndoSelect(IHitTest exceptElement, bool clearHot) {
			foreach (IHitTest hitElement in this)
				if (exceptElement == null || hitElement.Object != exceptElement.Object)
					hitElement.State.UndoSelect(clearHot);
		}
	}
	public class HitTestColors {
		public static Color Selected { get { return Color.FromArgb(227, 66, 30); } }
		public static Color Hot { get { return Color.FromArgb(23, 135, 238); } }
		public static Color SelectHatch { get { return Color.FromArgb(110, 255, 255, 255); } }
		public static Color HotHatch { get { return Color.FromArgb(50, 255, 255, 255); } }
		public static Color SelectHatchLight { get { return Color.FromArgb(120, 255, 255, 255); } }
		public static Color HotHatchLight { get { return Color.FromArgb(90, 255, 255, 255); } }
		public static Color MixColors(Color c1, Color c2) {
			double a = (double)c1.A / 255.0;
			int r = (int)(a * c1.R + (1 - a) * c2.R);
			int g = (int)(a * c1.G + (1 - a) * c2.G);
			int b = (int)(a * c1.B + (1 - a) * c2.B);
			return Color.FromArgb(r, g, b);
		}
		public static Color MixAlphaChanel(Color color, byte alpha) {
			double alpha1 = (double)color.A / 255.0;
			double alpha2 = (double)alpha / 255.0;
			byte newAlpha = (byte)((alpha1 * alpha2) * 255);
			return Color.FromArgb(newAlpha, color);
		}
	}
	public class HyperlinkSource {
		readonly string hyperlink;
		public string Hyperlink { get { return hyperlink; } }
		public HyperlinkSource(string hyperlink) {
			this.hyperlink = hyperlink;
		}
	}
}
