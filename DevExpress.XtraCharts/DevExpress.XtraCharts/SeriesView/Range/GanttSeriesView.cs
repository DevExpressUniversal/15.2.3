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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	public abstract class GanttSeriesView : RangeBarSeriesView {
		static SeriesPointLayout FindSeriesPointLayout(SeriesPoint point, SeriesLayout seriesLayout) {
			foreach (SeriesPointLayout pointLayout in seriesLayout)
				if (pointLayout.PointData.SeriesPoint == point)
					return pointLayout;
			return null;
		}
		#region fields and properties
		TaskLinkOptions linkOptions;
		protected internal override bool IsSupportedRelations { get { return true; } }
		protected override int PixelsPerArgument { get { return 40; } }
		protected override CompatibleViewType CompatibleViewType { get { return CompatibleViewType.GanttView; } }
		protected internal override string DefaultLabelTextPattern {
			get {
				if (SeriesBase != null)
					return SeriesBase.ValueScaleType == ScaleType.DateTime ? "{" + PatternUtils.ValuePlaceholder + ":d}" : "{" + PatternUtils.ValuePlaceholder + "}";
				else
					return "{" + PatternUtils.ValuePlaceholder + "}";
			}
		}
		protected internal override bool NeedFilterVisiblePoints {
			get {
				return false;
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Type DiagramType { get { return typeof(GanttDiagram); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("GanttSeriesViewLinkOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.GanttSeriesView.LinkOptions"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public TaskLinkOptions LinkOptions { get { return linkOptions; } }
		#endregion
		public GanttSeriesView() : base() {
			linkOptions = new TaskLinkOptions(this);
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "LinkOptions")
				return ShouldSerializeLinkOptions();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeLinkOptions() {
			return LinkOptions.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeLinkOptions();
		}
		#endregion
		void RenderTaskLinkShadow(IRenderer renderer, TaskLinkLayout layout, Shadow shadow) {
			if (layout.LineRects != null) {
				foreach (ZPlaneRectangle rect in layout.LineRects)
					shadow.Render(renderer, (Rectangle)rect);
			}
			if (layout.ArrowPoints != null) {
				VariousPolygon polygon = new VariousPolygon(layout.ArrowPoints, RectangleF.Empty);
				polygon.RenderShadow(renderer, shadow, -1);
			}
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new GanttDrawOptions(this);
		}
		protected internal override WholeSeriesLayout CalculateWholeSeriesLayout(XYDiagramMappingBase diagramMapping, SeriesLayout seriesLayout) {
			if (!this.linkOptions.Visible)
				return null;
			GanttWholeSeriesLayout ganttLayout = new GanttWholeSeriesLayout(seriesLayout);
			foreach (SeriesPointLayout parentLayout in seriesLayout) {
				ArrayList list = new ArrayList();
				foreach (TaskLink link in SeriesPoint.GetSeriesPoint(parentLayout.PointData.SeriesPoint).Relations) {
					SeriesPointLayout childLayout = FindSeriesPointLayout(link.ChildPoint, seriesLayout);
					if (childLayout == null)
						throw new InternalException("TaskLink error");
					TaskLinkLayout linkLayout = TaskLinkLayout.CreateLayout(
						diagramMapping,
						((GanttDrawOptions)parentLayout.PointData.DrawOptions).LinkOptions,
						link,
						(BarSeriesPointLayout)parentLayout,
						(BarSeriesPointLayout)childLayout);
					list.Add(linkLayout);
				}
				ganttLayout.Add(parentLayout, (TaskLinkLayout[])list.ToArray(typeof(TaskLinkLayout)));
			}
			return ganttLayout;
		}
		protected internal override void RenderWholeSeries(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) {
			GanttWholeSeriesLayout ganttLayout = (GanttWholeSeriesLayout)layout;
			IDictionaryEnumerator enumerator = ganttLayout.GetEnumerator();
			while (enumerator.MoveNext()) {
				SeriesPointLayout parentLayout = (SeriesPointLayout)enumerator.Key;
				TaskLinkLayout[] linkLayouts = (TaskLinkLayout[])enumerator.Value;
				this.linkOptions.Render(renderer, parentLayout, linkLayouts);
			}
		}
		protected internal override GraphicsCommand CreateWholeSeriesShadowGraphicsCommand(Rectangle mappingBounds, WholeSeriesLayout layout) {
			return null;
		}
		protected internal override void RenderWholeSeriesShadow(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) {
			GanttWholeSeriesLayout ganttLayout = (GanttWholeSeriesLayout)layout;
			IDictionaryEnumerator enumerator = ganttLayout.GetEnumerator();
			while (enumerator.MoveNext()) {
				TaskLinkLayout[] linkLayouts = (TaskLinkLayout[])enumerator.Value;
				foreach (TaskLinkLayout linkLayout in linkLayouts)
					RenderTaskLinkShadow(renderer, linkLayout, ((GanttDrawOptions)layout.SeriesLayout.SeriesData.DrawOptions).Shadow);
			}
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			GanttSeriesView view = (GanttSeriesView)obj;
			return linkOptions.Equals(view.linkOptions);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			GanttSeriesView view = obj as GanttSeriesView;
			if (view == null)
				return;
			linkOptions.Assign(view.linkOptions);
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class GanttWholeSeriesLayout : WholeSeriesLayout {
		Hashtable links = new Hashtable();
		public GanttWholeSeriesLayout(SeriesLayout seriesLayout) : base(seriesLayout) {
		}
		static HitRegionContainer CalculateHitRegionForTaskLinks(TaskLinkLayout[] linkLayouts) {
			if (linkLayouts.Length == 0)
				return null;
			HitRegionContainer hitRegion = new HitRegionContainer();
			foreach (TaskLinkLayout linkLayout in linkLayouts) {
				if (linkLayout.LineRects != null) {
					foreach (ZPlaneRectangle rect in linkLayout.LineRects) {
						Rectangle hitRect = GraphicUtils.InflateRect((Rectangle)rect, 1, 1);
						hitRegion.Union(new HitRegion(hitRect));
					}
				}
				if (linkLayout.ArrowPoints != null) {
					GraphicsPath hitPath = new GraphicsPath();
					hitPath.AddPolygon(StripsUtils.Convert(linkLayout.ArrowPoints));
					hitRegion.Union(new HitRegion(hitPath));
				}
			}
			return hitRegion;
		}
		public void Add(SeriesPointLayout parentLayout, TaskLinkLayout[] linkLayouts) {
			if (this.links.Contains(parentLayout))
				throw new InternalException("TaskLink error");
			this.links.Add(parentLayout, linkLayouts);
		}
		public IDictionaryEnumerator GetEnumerator() {
			return this.links.GetEnumerator();
		}
		public override HitRegionContainer CalculateHitRegion(DrawOptions drawOptions) {
			HitRegionContainer hitRegion = base.CalculateHitRegion(drawOptions);
			IDictionaryEnumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext()) {
				TaskLinkLayout[] linkLayouts = (TaskLinkLayout[])enumerator.Value;
				HitRegionContainer taskLinksRegion = CalculateHitRegionForTaskLinks(linkLayouts);
				if (taskLinksRegion != null)
					using (taskLinksRegion)
						hitRegion.Union(taskLinksRegion);
			}
			return hitRegion;
		}
	}
}
