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
using System.Drawing;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Native {
	public class SeriesViewFactory {
		#region inner classes
		class SeriesViewEntry {
			ViewType viewType;
			Type type;
			string stringId;
			public ViewType ViewType { get { return this.viewType; } }
			public Type Type { get { return type; } }
			public string StringID { get { return this.stringId; } }
			public SeriesViewEntry(ViewType viewType, Type type, string stringId) {
				this.viewType = viewType;
				this.type = type;
				this.stringId = stringId;
			}
		}
		class SeriesViewList : ArrayList {
			public Type this[ViewType viewType] {
				get {
					int index = GetIndex(viewType);
					return index == -1 ? null : ((SeriesViewEntry)this[index]).Type;
				}
			}
			public bool ContainsViewType(ViewType viewType) {
				return GetIndex(viewType) != -1;
			}
			public void Add(SeriesViewEntry entry) {
				base.Add(entry);
			}
			public void Remove(ViewType viewType) {
				int index = GetIndex(viewType);
				if (index != -1)
					RemoveAt(index);
			}
			int GetIndex(ViewType viewType) {
				for (int i = 0; i < Count; i++)
					if (((SeriesViewEntry)this[i]).ViewType == viewType)
						return i;
				return -1;
			}
		}
		#endregion
		static ViewType defaultViewType;
		static SeriesViewFactory factory;
		static SeriesViewFactory Factory {
			get {
				if (factory == null)
					factory = new SeriesViewFactory();
				return factory;
			}
		}
		public static ViewType DefaultViewType { get { return defaultViewType; } }
		public static ViewType[] ViewTypes {
			get {
				ViewType[] viewTypes = new ViewType[Factory.list.Count];
				int index = 0;
				foreach (SeriesViewEntry entry in Factory.list) {
					viewTypes[index] = entry.ViewType;
					index++;
				}
				return viewTypes;
			}
		}
		public static string[] StringIDs {
			get {
				string[] ids = new String[Factory.list.Count];
				IEnumerator enumerator = Factory.list.GetEnumerator();
				int i = 0;
				while (enumerator.MoveNext() && i < ids.Length)
					ids[i++] = ((SeriesViewEntry)enumerator.Current).StringID;
				return ids;
			}
		}
		public static Image[] SeriesViewImages {
			get {
				Image[] images = new Image[Factory.list.Count];
				IEnumerator enumerator = Factory.list.GetEnumerator();
				int i = 0;
				while (enumerator.MoveNext() && i < images.Length) {
					SeriesViewBase view = ((SeriesViewBase)Activator.CreateInstance(((SeriesViewEntry)enumerator.Current).Type));
					images[i++] = ImageResourcesUtils.GetImageFromResources(view, SeriesViewImageType.Image);
				}
				return images;
			}
		}
		static SeriesViewFactory() {
			RegisterSeriesView(ViewType.Bar, new SideBySideBarSeriesView(), true);
			RegisterSeriesView(ViewType.StackedBar, new StackedBarSeriesView());
			RegisterSeriesView(ViewType.FullStackedBar, new FullStackedBarSeriesView());
			RegisterSeriesView(ViewType.SideBySideStackedBar, new SideBySideStackedBarSeriesView());
			RegisterSeriesView(ViewType.SideBySideFullStackedBar, new SideBySideFullStackedBarSeriesView());
			RegisterSeriesView(ViewType.Bar3D, new SideBySideBar3DSeriesView());
			RegisterSeriesView(ViewType.StackedBar3D, new StackedBar3DSeriesView());
			RegisterSeriesView(ViewType.FullStackedBar3D, new FullStackedBar3DSeriesView());
			RegisterSeriesView(ViewType.SideBySideStackedBar3D, new SideBySideStackedBar3DSeriesView());
			RegisterSeriesView(ViewType.SideBySideFullStackedBar3D, new SideBySideFullStackedBar3DSeriesView());
			RegisterSeriesView(ViewType.ManhattanBar, new ManhattanBarSeriesView());
			RegisterSeriesView(ViewType.Point, new PointSeriesView());
			RegisterSeriesView(ViewType.Bubble, new BubbleSeriesView());
			RegisterSeriesView(ViewType.Line, new LineSeriesView());
			RegisterSeriesView(ViewType.StackedLine, new StackedLineSeriesView());
			RegisterSeriesView(ViewType.FullStackedLine, new FullStackedLineSeriesView());
			RegisterSeriesView(ViewType.StepLine, new StepLineSeriesView());
			RegisterSeriesView(ViewType.Spline, new SplineSeriesView());
			RegisterSeriesView(ViewType.ScatterLine, new ScatterLineSeriesView());
			RegisterSeriesView(ViewType.SwiftPlot, new SwiftPlotSeriesView());
			RegisterSeriesView(ViewType.Line3D, new Line3DSeriesView());
			RegisterSeriesView(ViewType.StackedLine3D, new StackedLine3DSeriesView());
			RegisterSeriesView(ViewType.FullStackedLine3D, new FullStackedLine3DSeriesView());
			RegisterSeriesView(ViewType.StepLine3D, new StepLine3DSeriesView());
			RegisterSeriesView(ViewType.Spline3D, new Spline3DSeriesView());
			RegisterSeriesView(ViewType.Pie, new PieSeriesView());
			RegisterSeriesView(ViewType.Doughnut, new DoughnutSeriesView());
			RegisterSeriesView(ViewType.NestedDoughnut, new NestedDoughnutSeriesView());
			RegisterSeriesView(ViewType.Pie3D, new Pie3DSeriesView());
			RegisterSeriesView(ViewType.Doughnut3D, new Doughnut3DSeriesView());
			RegisterSeriesView(ViewType.Funnel, new FunnelSeriesView());
			RegisterSeriesView(ViewType.Funnel3D, new Funnel3DSeriesView());
			RegisterSeriesView(ViewType.Area, new AreaSeriesView());
			RegisterSeriesView(ViewType.StackedArea, new StackedAreaSeriesView());
			RegisterSeriesView(ViewType.FullStackedArea, new FullStackedAreaSeriesView());
			RegisterSeriesView(ViewType.StepArea, new StepAreaSeriesView());
			RegisterSeriesView(ViewType.SplineArea, new SplineAreaSeriesView());
			RegisterSeriesView(ViewType.StackedSplineArea, new StackedSplineAreaSeriesView());
			RegisterSeriesView(ViewType.FullStackedSplineArea, new FullStackedSplineAreaSeriesView());
			RegisterSeriesView(ViewType.Area3D, new Area3DSeriesView());
			RegisterSeriesView(ViewType.StackedArea3D, new StackedArea3DSeriesView());
			RegisterSeriesView(ViewType.FullStackedArea3D, new FullStackedArea3DSeriesView());
			RegisterSeriesView(ViewType.StepArea3D, new StepArea3DSeriesView());
			RegisterSeriesView(ViewType.SplineArea3D, new SplineArea3DSeriesView());
			RegisterSeriesView(ViewType.StackedSplineArea3D, new StackedSplineArea3DSeriesView());
			RegisterSeriesView(ViewType.FullStackedSplineArea3D, new FullStackedSplineArea3DSeriesView());
			RegisterSeriesView(ViewType.RangeBar, new OverlappedRangeBarSeriesView());
			RegisterSeriesView(ViewType.SideBySideRangeBar, new SideBySideRangeBarSeriesView());
			RegisterSeriesView(ViewType.RangeArea, new RangeAreaSeriesView());
			RegisterSeriesView(ViewType.RangeArea3D, new RangeArea3DSeriesView());
			RegisterSeriesView(ViewType.RadarPoint, new RadarPointSeriesView());
			RegisterSeriesView(ViewType.RadarLine, new RadarLineSeriesView());
			RegisterSeriesView(ViewType.ScatterRadarLine, new ScatterRadarLineSeriesView());
			RegisterSeriesView(ViewType.RadarArea, new RadarAreaSeriesView());
			RegisterSeriesView(ViewType.PolarPoint, new PolarPointSeriesView());
			RegisterSeriesView(ViewType.PolarLine, new PolarLineSeriesView());
			RegisterSeriesView(ViewType.ScatterPolarLine, new ScatterPolarLineSeriesView());
			RegisterSeriesView(ViewType.PolarArea, new PolarAreaSeriesView());
			RegisterSeriesView(ViewType.Stock, new StockSeriesView());
			RegisterSeriesView(ViewType.CandleStick, new CandleStickSeriesView());
			RegisterSeriesView(ViewType.Gantt, new OverlappedGanttSeriesView());
			RegisterSeriesView(ViewType.SideBySideGantt, new SideBySideGanttSeriesView());
		}
		public static void RegisterSeriesView(ViewType viewType, SeriesViewBase seriesView) {
			RegisterSeriesView(viewType, seriesView, false);
		}
		public static void RegisterSeriesView(ViewType viewType, SeriesViewBase seriesView, bool setAsDefault) {
			if (seriesView != null) {
				Factory.Add(viewType, seriesView.GetType(), seriesView.StringId);
				if (setAsDefault)
					defaultViewType = viewType;
			}
		}
		public static void UnregisterSeriesView(ViewType viewType) {
			Factory.Remove(viewType);
		}
		public static SeriesViewBase CreateInstance(ViewType viewType) {
			Type type = Factory[viewType];
			if (type == null)
				throw new SeriesViewFactoryException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgSeriesViewDoesNotExist), viewType.ToString()));
			SeriesViewBase seriesView = (SeriesViewBase)Activator.CreateInstance(type);
			return seriesView;
		}
		public static SeriesViewBase CreateInstance(SeriesViewBase templateView) {
			ViewType viewType = GetViewType(templateView);
			return CreateInstance(viewType);
		}
		public static SeriesViewBase CreateInstance(string stringId) {
			ViewType viewType = GetViewType(stringId);
			return CreateInstance(viewType);
		}
		public static ViewType GetViewType(SeriesViewBase view) {
			Type type = view.GetType();
			IEnumerator enumerator = Factory.list.GetEnumerator();
			while (enumerator.MoveNext()) {
				SeriesViewEntry entry = (SeriesViewEntry)enumerator.Current;
				if (type.Equals(entry.Type))
					return entry.ViewType;
			}
			throw new SeriesViewFactoryException();
		}
		public static ViewType GetViewType(string stringId) {
			IEnumerator enumerator = Factory.list.GetEnumerator();
			while (enumerator.MoveNext()) {
				SeriesViewEntry entry = (SeriesViewEntry)enumerator.Current;
				if (stringId == entry.StringID)
					return entry.ViewType;
			}
			throw new SeriesViewFactoryException();
		}
		public static string GetStringID(ViewType viewType) {
			IEnumerator enumerator = Factory.list.GetEnumerator();
			while (enumerator.MoveNext()) {
				SeriesViewEntry entry = (SeriesViewEntry)enumerator.Current;
				if (viewType == entry.ViewType)
					return entry.StringID;
			}
			throw new SeriesViewFactoryException();
		}
		public static Type GetType(ViewType viewType) {
			return Factory[viewType];
		}
		SeriesViewList list = new SeriesViewList();
		Type this[ViewType viewType] { get { return (Type)list[viewType]; } }
		SeriesViewFactory() {
		}
		void Add(ViewType viewType, Type type, string stringId) {
			if (type != null && type.IsClass && !list.ContainsViewType(viewType))
				list.Add(new SeriesViewEntry(viewType, type, stringId));
		}
		void Remove(string name) {
			list.Remove(name);
		}
		void Remove(ViewType viewType) {
			list.Remove(viewType);
		}
	}
}
