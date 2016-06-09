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
using System.Drawing;
using System.Globalization;
using System.Text;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Web;
using DevExpress.Utils;
namespace DevExpress.XtraCharts.Native {
	public static class ClientObjectModelHelper {
		public static string ValueLevelToString(ValueLevel valueLevel) {
			switch (valueLevel) {
				case ValueLevel.Value:
					return "Value";
				case ValueLevel.Value_1:
					return "Value_1";
				case ValueLevel.Value_2:
					return "Value_2";
				case ValueLevel.Low:
					return "Low";
				case ValueLevel.High:
					return "High";
				case ValueLevel.Open:
					return "Open";
				case ValueLevel.Close:
					return "Close";
				default:
					ChartDebug.Fail("Incorrect value level");
					goto case ValueLevel.Value;
			}
		}
	}
	public abstract class ClientChartElementBase {
		protected const char Separator = ',';
		public abstract bool ShouldSerialize { get; }
		public abstract void Serialize(StringBuilder builder);
		public abstract void InitializeHitInfo(Dictionary<object, int> objects, Dictionary<object, int> additionalObjects);
	}
	public abstract class ClientChartElementsBase : ClientChartElementBase {
		const string prefix = "[";
		const string postfix = "]";
		readonly IEnumerable<ClientChartElementBase> elements;
		protected IEnumerable<ClientChartElementBase> Elements { get { return elements; } }
		public override bool ShouldSerialize {
			get {
				foreach (ClientChartElementBase element in elements)
					if (element.ShouldSerialize)
						return true;
				return false;
			}
		}
		public ClientChartElementsBase(IEnumerable<ClientChartElementBase> elements) {
			this.elements = elements;
		}
		public override void InitializeHitInfo(Dictionary<object, int> objects, Dictionary<object, int> additionalObjects) {
			foreach (ClientChartElementBase element in elements)
				element.InitializeHitInfo(objects, additionalObjects);
		}
		public override void Serialize(StringBuilder builder) {
			builder.Append(prefix);
			bool isFirstElement = true;
			foreach (ClientChartElementBase element in elements) {
				if (isFirstElement)
					isFirstElement = false;
				else
					builder.Append(Separator);
				element.Serialize(builder);
			}
			builder.Append(postfix);
		}
	}
	public class ClientChartElements : ClientChartElementsBase {
		public new ClientChartElementBase[] Elements { get { return (ClientChartElementBase[])base.Elements; } }
		public ClientChartElements(int count)
			: base(new ClientChartElementBase[count]) {
		}
	}
	public class ClientChartElementList : ClientChartElementsBase {
		public ClientChartElementList()
			: base(new List<ClientChartElementBase>()) {
		}
		public void Add(ClientChartElementBase element) {
			((List<ClientChartElementBase>)Elements).Add(element);
		}
	}
	public abstract class ClientChartElement : ClientChartElementBase {
		class HitInfo {
			public readonly int id;
			public int Id { get { return id; } }
			public HitInfo(int id) {
				this.id = id;
			}
		}
		const string prefix = "{";
		const string postfix = "}";
		const string arrayPostfix = "]";
		const string arrayPrefix = "[";
		const string datePrefix = "new Date(";
		const string datePostfix = ")";
		const string propertySeparator = ":";
		const string error = "Client object model error";
		static string UpdateTimeString(string time, int value) {
			if (!String.IsNullOrEmpty(time))
				return time.Insert(0, value.ToString() + Separator);
			return value == 0 ? time : value.ToString();
		}
		static string GetDateTimeValueString(DateTime value) {
			string date = datePrefix + value.Year.ToString() + Separator + (value.Month - 1).ToString() + Separator + value.Day.ToString();
			string time = UpdateTimeString(String.Empty, value.Millisecond);
			time = UpdateTimeString(time, value.Second);
			time = UpdateTimeString(time, value.Minute);
			time = UpdateTimeString(time, value.Hour);
			if (!String.IsNullOrEmpty(time))
				date += Separator + time;
			return date + datePostfix;
		}
		static string GetArrayValueString(IEnumerable value) {
			bool isFirstElement = true;
			string array = arrayPrefix;
			foreach (object child in value) {
				if (isFirstElement)
					isFirstElement = false;
				else
					array += Separator;
				array += GetValueString(child);
			}
			return array + arrayPostfix;
		}
		static string EscapeChar(char ch) {
			switch (ch) {
				case '\\':
				case '\'':
					return "\\" + ch;
				case '\x0000':
					return "\\0";
				case '\x0008':
					return "\\b";
				case '\x0009':
					return "\\t";
				case '\x000A':
					return "\\n";
				case '\x000B':
					return "\\v";
				case '\x000C':
					return "\\f";
				case '\x000D':
					return "\\r";
				default:
					return Convert.ToString(ch);
			}
		}
		static string EscapeString(string value) {
			string result = String.Empty;
			foreach (char ch in value)
				result += EscapeChar(ch);
			return result;
		}
		static string GetValueString(object value) {
			if (value == null)
				return "''";
			if (value is string)
				return "'" + EscapeString((string)value) + "'";
			if (value is bool)
				return ((bool)value) ? "true" : "false";
			if (value is double)
				return ((double)value).ToString(NumberFormatInfo.InvariantInfo);
			if (value is float)
				return ((float)value).ToString(NumberFormatInfo.InvariantInfo);
			if (value is int)
				return ((int)value).ToString(NumberFormatInfo.InvariantInfo);
			if (value is DateTime)
				return GetDateTimeValueString((DateTime)value);
			if (value is IEnumerable)
				return GetArrayValueString((IEnumerable)value);
			throw new InternalException(error);
		}
		bool firstProperty;
		List<ClientChartElementBase> childElements;
		HitInfo hitInfo;
		HitInfo additionalHitInfo;
		List<ClientChartElementBase> ChildElements {
			get {
				if (childElements == null) {
					childElements = new List<ClientChartElementBase>();
					FillChildElements(childElements);
				}
				return childElements;
			}
		}
		bool ShouldSerializeChildElements {
			get {
				foreach (ClientChartElementBase element in ChildElements)
					if (element.ShouldSerialize)
						return true;
				return false;
			}
		}
		protected abstract object InnerObject { get; }
		protected virtual string TypeName { get { return null; } }
		protected virtual bool ShouldSerializeProperties { get { return TypeName != null || hitInfo != null || additionalHitInfo != null; } }
		public override bool ShouldSerialize { get { return ShouldSerializeChildElements || ShouldSerializeProperties; } }
		void SerializeSeparator(StringBuilder builder) {
			if (firstProperty)
				firstProperty = false;
			else
				builder.Append(Separator);
		}
		void SerializeChartElements(StringBuilder builder) {
			foreach (ClientChartElementBase element in ChildElements)
				if (element.ShouldSerialize) {
					SerializeSeparator(builder);
					builder.Append(GetElementName(element) + propertySeparator);
					element.Serialize(builder);
				}
		}
		void SerializeHitInfo(StringBuilder builder) {
			if (hitInfo != null)
				SerializeProperty(builder, "hi", hitInfo.Id);
			if (additionalHitInfo != null)
				SerializeProperty(builder, "hia", additionalHitInfo.Id);
		}
		protected void SerializeProperty(StringBuilder builder, string name, object value) {
			SerializeSeparator(builder);
			builder.Append(name + propertySeparator + GetValueString(value));
		}
		protected virtual void SerializeProperties(StringBuilder builder) {
			if (TypeName != null)
				SerializeProperty(builder, "t", TypeName);
			if (hitInfo != null || additionalHitInfo != null)
				SerializeHitInfo(builder);
		}
		protected virtual void FillChildElements(List<ClientChartElementBase> childElements) {
		}
		protected virtual string GetElementName(ClientChartElementBase element) {
			throw new InternalException(error);
		}
		protected internal string GetColorString(Color color) {
			return color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
		}
		public override void Serialize(StringBuilder builder) {
			firstProperty = true;
			builder.Append(prefix);
			SerializeChartElements(builder);
			SerializeProperties(builder);
			builder.Append(postfix);
		}
		public override void InitializeHitInfo(Dictionary<object, int> objects, Dictionary<object, int> additionalObjects) {
			if (InnerObject != null) {
				if (objects.ContainsKey(InnerObject))
					hitInfo = new HitInfo(objects[InnerObject]);
				if (additionalObjects.ContainsKey(InnerObject))
					additionalHitInfo = new HitInfo(additionalObjects[InnerObject]);
			}
			foreach (ClientChartElementBase elements in ChildElements)
				elements.InitializeHitInfo(objects, additionalObjects);
		}
	}
	public abstract class ClientChartElementNamed : ClientChartElement {
		readonly string name;
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientChartElementNamed(string name)
			: base() {
			this.name = name;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "n", name);
		}
	}
	public class ClientWebChartControl : ClientChartElement {
		readonly WebChartControl chartControl;
		readonly ClientXYDiagramBase clientXYDiagram;
		readonly ClientLegend clientLegend;
		readonly ClientChartElements clientSeries;
		readonly ClientChartElements clientTitles;
		readonly ClientChartElementList clientAnnotations;
		readonly ClientToolTipPosition clientToolTipPosition;
		readonly ClientToolTipController clientToolTipController;
		readonly ClientCrosshairOptions clientCrosshairOptions;
		protected override object InnerObject { get { return chartControl; } }
		public ClientWebChartControl(WebChartControl chartControl) {
			this.chartControl = chartControl;
			clientXYDiagram = CreateClientXYDiagram();
			clientLegend = new ClientLegend(chartControl.Legend);
			clientSeries = CreateClientSeries();
			clientTitles = CreateClientTitles();
			clientAnnotations = CreateClientAnnotations();
			clientToolTipPosition = CreateClientToolTipPosition();
			clientToolTipController = new ClientToolTipController(chartControl.ToolTipController);
			clientCrosshairOptions = new ClientCrosshairOptions(chartControl, chartControl.CrosshairOptions);
		}
		ClientXYDiagramBase CreateClientXYDiagram() {
			Diagram diagram = chartControl.Diagram;
			if (diagram is XYDiagram)
				return new ClientXYDiagram(chartControl.Chart, (XYDiagram)diagram);
			if (diagram is SwiftPlotDiagram)
				return new ClientSwiftPlotDiagram(chartControl.Chart, (SwiftPlotDiagram)diagram);
			if (diagram is XYDiagram3D)
				return new ClientXYDiagram3D(chartControl.Chart, (XYDiagram3D)diagram);
			if (diagram is RadarDiagram)
				return new ClientRadarDiagram(chartControl.Chart, (RadarDiagram)diagram);
			return null;
		}
		ClientChartElements CreateClientSeries() {
			int count = chartControl.Series.Count;
			ClientChartElements clientSeries = new ClientChartElements(count);
			for (int i = 0; i < count; i++)
				clientSeries.Elements[i] = new ClientSeries(chartControl.Series[i], chartControl);
			return clientSeries;
		}
		ClientChartElements CreateClientTitles() {
			int count = chartControl.Titles.Count;
			ClientChartElements clientTitles = new ClientChartElements(count);
			for (int i = 0; i < count; i++)
				clientTitles.Elements[i] = new ClientTitle(chartControl.Titles[i]);
			return clientTitles;
		}
		ClientChartElementList CreateClientAnnotations() {
			ClientChartElementList clientAnnotations = new ClientChartElementList();
			foreach (Annotation annotation in chartControl.AnnotationRepository) {
				ClientAnnotation clientAnnotation = ClientAnnotation.CreateInstance(annotation);
				if (clientAnnotation != null)
					clientAnnotations.Add(clientAnnotation);
			}
			return clientAnnotations;
		}
		ClientToolTipPosition CreateClientToolTipPosition() {
			ToolTipPosition position = chartControl.ToolTipOptions.ToolTipPosition;
			if (position is ToolTipMousePosition)
				return new ClientToolTipMousePosition((ToolTipMousePosition)position);
			if (position is ToolTipFreePosition)
				return new ClientToolTipFreePosition(chartControl, (ToolTipFreePosition)position);
			if (position is ToolTipRelativePosition)
				return new ClientToolTipRelativePosition((ToolTipRelativePosition)position);
			return null;
		}
		protected override void FillChildElements(List<ClientChartElementBase> childElements) {
			base.FillChildElements(childElements);
			if (clientXYDiagram != null)
				childElements.Add(clientXYDiagram);
			childElements.Add(clientSeries);
			childElements.Add(clientLegend);
			childElements.Add(clientTitles);
			childElements.Add(clientAnnotations);
			if (chartControl.Chart.ActualSeriesToolTipEnabled || chartControl.Chart.ActualPointToolTipEnabled) {
				childElements.Add(clientToolTipPosition);
				childElements.Add(clientToolTipController);
			}
			if (chartControl.Chart.ActualCrosshairEnabled) {
				childElements.Add(clientCrosshairOptions);
			}
		}
		protected override string GetElementName(ClientChartElementBase element) {
			if (element == clientXYDiagram)
				return "d";
			if (element == clientSeries)
				return "s";
			if (element == clientLegend)
				return "l";
			if (element == clientTitles)
				return "ti";
			if (element == clientAnnotations)
				return "a";
			if (element == clientToolTipPosition)
				return "ttp";
			if (element == clientToolTipController)
				return "ttc";
			if (element == clientCrosshairOptions) {
				return "co";
			}
			return base.GetElementName(element);
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "an", chartControl.AppearanceName);
			SerializeProperty(builder, "pn", chartControl.PaletteName);
			if (chartControl.Chart.SeriesToolTipEnabled)
				SerializeProperty(builder, "sst", chartControl.Chart.SeriesToolTipEnabled);
			if (chartControl.Chart.PointToolTipEnabled)
				SerializeProperty(builder, "spt", chartControl.Chart.PointToolTipEnabled);
			if (chartControl.Chart.ActualCrosshairEnabled)
				SerializeProperty(builder, "sc", chartControl.Chart.ActualCrosshairEnabled);
			if (chartControl.Chart.SelectionMode != ElementSelectionMode.None)
				SerializeProperty(builder, "sm", chartControl.Chart.SelectionMode.ToString());
			if (!string.IsNullOrEmpty(chartControl.CssPostfix))
				SerializeProperty(builder, "css", chartControl.CssPostfix);
		}
	}
	public abstract class ClientXYDiagramBase : ClientChartElement {
		readonly Chart chart;
		readonly Diagram diagram;
		readonly ClientAxis clientAxisX;
		readonly ClientAxis clientAxisY;
		protected Chart Chart { get { return chart; } }
		protected Diagram Diagram { get { return diagram; } }
		protected override object InnerObject { get { return diagram; } }
		public ClientXYDiagramBase(Chart chart, Diagram diagram)
			: base() {
			this.chart = chart;
			this.diagram = diagram;
			clientAxisX = CreateClientAxisX();
			clientAxisY = CreateClientAxisY();
		}
		protected override void FillChildElements(List<ClientChartElementBase> childElements) {
			base.FillChildElements(childElements);
			childElements.Add(clientAxisX);
			childElements.Add(clientAxisY);
		}
		protected override string GetElementName(ClientChartElementBase element) {
			if (element == clientAxisX)
				return "x";
			if (element == clientAxisY)
				return "y";
			return base.GetElementName(element);
		}
		protected abstract ClientAxis CreateClientAxisX();
		protected abstract ClientAxis CreateClientAxisY();
	}
	public abstract class ClientXYDiagram2D : ClientXYDiagramBase {
		readonly ClientChartElementList clientSecondaryAxesX;
		readonly ClientChartElementList clientSecondaryAxesY;
		readonly ClientXYDiagramPane clientDefaultPane;
		readonly ClientChartElements clientPanes;
		protected new XYDiagram2D Diagram { get { return (XYDiagram2D)base.Diagram; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientXYDiagram2D(Chart chart, XYDiagram2D diagram)
			: base(chart, diagram) {
			clientSecondaryAxesX = CreateClientSecondaryAxesX();
			clientSecondaryAxesY = CreateClientSecondaryAxesY();
			clientDefaultPane = new ClientXYDiagramPane(chart, diagram.DefaultPane);
			clientPanes = CreateClientPanes();
		}
		ClientChartElements CreateClientPanes() {
			ClientChartElements clientPanes = new ClientChartElements(Diagram.Panes.Count);
			for (int i = 0; i < Diagram.Panes.Count; i++)
				clientPanes.Elements[i] = new ClientXYDiagramPane(Chart, Diagram.Panes[i]);
			return clientPanes;
		}
		protected override void FillChildElements(List<ClientChartElementBase> childElements) {
			base.FillChildElements(childElements);
			childElements.Add(clientSecondaryAxesX);
			childElements.Add(clientSecondaryAxesY);
			childElements.Add(clientDefaultPane);
			childElements.Add(clientPanes);
		}
		protected override string GetElementName(ClientChartElementBase element) {
			if (element == clientSecondaryAxesX)
				return "sx";
			if (element == clientSecondaryAxesY)
				return "sy";
			if (element == clientDefaultPane)
				return "dp";
			if (element == clientPanes)
				return "pa";
			return base.GetElementName(element);
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			if (Chart.ActualCrosshairEnabled) {
				string paneLayoutDirection = Diagram.PaneLayoutDirection == PaneLayoutDirection.Horizontal ? "Horizontal" : "Vertical";
				SerializeProperty(builder, "pld", paneLayoutDirection);
			}
		}
		protected abstract ClientChartElementList CreateClientSecondaryAxesX();
		protected abstract ClientChartElementList CreateClientSecondaryAxesY();
	}
	public class ClientXYDiagram : ClientXYDiagram2D {
		new XYDiagram Diagram { get { return (XYDiagram)base.Diagram; } }
		protected override string TypeName { get { return "XYD"; } }
		public ClientXYDiagram(Chart chart, XYDiagram diagram)
			: base(chart, diagram) {
		}
		ClientChartElementList CreateClientSecondaryAxes(SecondaryAxisCollection collection) {
			ClientChartElementList clientAxes = new ClientChartElementList();
			foreach (Axis axis in collection) {
				ClientAxisXYDiagram2D clientAxis = new ClientAxisXYDiagram2D(Chart, axis);
				clientAxis.SetClientAxisScale(ClientAxisScale.Create(axis));
				clientAxes.Add(clientAxis);
			}
			return clientAxes;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "dr", Diagram.Rotated);
		}
		protected override ClientAxis CreateClientAxisX() {
			ClientAxisXYDiagram2D axis = new ClientAxisXYDiagram2D(Chart, Diagram.AxisX);
			axis.SetClientAxisScale(ClientAxisScale.Create(Diagram.AxisX));
			return axis;
		}
		protected override ClientAxis CreateClientAxisY() {
			ClientAxisXYDiagram2D axis = new ClientAxisXYDiagram2D(Chart, Diagram.AxisY);
			axis.SetClientAxisScale(ClientAxisScale.Create(Diagram.AxisY));
			return axis;
		}
		protected override ClientChartElementList CreateClientSecondaryAxesX() {
			return CreateClientSecondaryAxes(Diagram.SecondaryAxesX);
		}
		protected override ClientChartElementList CreateClientSecondaryAxesY() {
			return CreateClientSecondaryAxes(Diagram.SecondaryAxesY);
		}
	}
	public class ClientSwiftPlotDiagram : ClientXYDiagram2D {
		new SwiftPlotDiagram Diagram { get { return (SwiftPlotDiagram)base.Diagram; } }
		protected override string TypeName { get { return "SPD"; } }
		public ClientSwiftPlotDiagram(Chart chart, SwiftPlotDiagram diagram)
			: base(chart, diagram) {
		}
		ClientChartElementList CreateClientSecondaryAxes(SecondaryAxisCollection collection) {
			ClientChartElementList clientAxes = new ClientChartElementList();
			foreach (SwiftPlotDiagramAxis axis in collection) {
				ClientAxis2D clientAxis = new ClientAxis2D(Chart, axis);
				clientAxes.Add(clientAxis);
				clientAxis.SetClientAxisScale(ClientAxisScale.Create(axis));
			}
			return clientAxes;
		}
		protected override ClientAxis CreateClientAxisX() {
			ClientAxis2D axis = new ClientAxis2D(Chart, Diagram.AxisX);
			axis.SetClientAxisScale(ClientAxisScale.Create(Diagram.AxisX));
			return axis;
		}
		protected override ClientAxis CreateClientAxisY() {
			ClientAxis2D axis = new ClientAxis2D(Chart, Diagram.AxisY);
			axis.SetClientAxisScale(ClientAxisScale.Create(Diagram.AxisY));
			return axis;
		}
		protected override ClientChartElementList CreateClientSecondaryAxesX() {
			return CreateClientSecondaryAxes(Diagram.SecondaryAxesX);
		}
		protected override ClientChartElementList CreateClientSecondaryAxesY() {
			return CreateClientSecondaryAxes(Diagram.SecondaryAxesY);
		}
	}
	public class ClientXYDiagramPane : ClientChartElementNamed {
		readonly Chart chart;
		readonly PaneAxesContainer data;
		readonly XYDiagramPaneBase pane;
		readonly ClientChartElements clientAxisLabelBounds;
		protected override object InnerObject { get { return pane; } }
		public ClientXYDiagramPane(Chart chart, XYDiagramPaneBase pane)
			: base(pane.Name) {
			this.chart = chart;
			this.pane = pane;
			this.data = chart.GetPaneAxesData(pane);
			if (chart.ActualCrosshairEnabled && pane.Visible)
				clientAxisLabelBounds = CreateClientAxisLabelBounds();
		}
		ClientChartElements CreateClientAxisLabelBounds() {
			XYDiagram2D diagram = chart.Diagram as XYDiagram2D;
			List<ClientAxisLabelBounds> bounds = new List<ClientAxisLabelBounds>();
			if (data != null) {
				foreach (IAxisData axis in data.Axes) {
					Axis2D axis2D = axis as Axis2D;
					if (axis2D.GetVisibilityInPane(pane)) {
						Rectangle labelBounds = CommonUtils.GetLabelBounds(pane, axis2D);
						if (!labelBounds.IsEmpty)
							bounds.Add(new ClientAxisLabelBounds(chart.GetActualAxisID(axis2D), axis.IsArgumentAxis, labelBounds));
					}
				}
			}
			ClientChartElements clientBounds = new ClientChartElements(bounds.Count);
			for (int i = 0; i < bounds.Count; i++)
				clientBounds.Elements[i] = bounds[i];
			return clientBounds;
		}
		protected override void FillChildElements(List<ClientChartElementBase> childElements) {
			base.FillChildElements(childElements);
			if (chart.ActualCrosshairEnabled && clientAxisLabelBounds != null)
				childElements.Add(clientAxisLabelBounds);
		}
		protected override string GetElementName(ClientChartElementBase element) {
			if (element == clientAxisLabelBounds)
				return "lb";
			return base.GetElementName(element);
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			int primaryAxisXID;
			int primaryAxisYID;
			if (data == null) {
				primaryAxisXID = -1;
				primaryAxisYID = -1;
			}
			else {
				primaryAxisXID = chart.GetActualAxisID((Axis2D)data.PrimaryAxisX);
				primaryAxisYID = chart.GetActualAxisID((Axis2D)data.PrimaryAxisY);
			}
			SerializeProperty(builder, "paxi", primaryAxisXID);
			SerializeProperty(builder, "payi", primaryAxisYID);
			SerializeProperty(builder, "id", chart.GetActualPaneID(pane));
			if (((WebChartControl)chart.Container).EnableClientSideAPI) {
				Rectangle bounds = chart.GetPaneMappingBounds(pane);
				SerializeProperty(builder, "dx", bounds.X);
				SerializeProperty(builder, "dy", bounds.Y);
				SerializeProperty(builder, "dw", bounds.Width);
				SerializeProperty(builder, "dh", bounds.Height);
				SerializeProperty(builder, "vsb", pane.Visible);
			}
		}
	}
	public class ClientXYDiagram3D : ClientXYDiagramBase {
		protected new XYDiagram3D Diagram { get { return (XYDiagram3D)base.Diagram; } }
		protected override string TypeName { get { return "XYD3"; } }
		public ClientXYDiagram3D(Chart chart, XYDiagram3D diagram)
			: base(chart, diagram) {
		}
		protected override ClientAxis CreateClientAxisX() {
			return new ClientAxis(Chart, Diagram.AxisX);
		}
		protected override ClientAxis CreateClientAxisY() {
			return new ClientAxis(Chart, Diagram.AxisY);
		}
	}
	public class ClientRadarDiagram : ClientXYDiagramBase {
		readonly ClientRadarDiagramMapping clientRadarDiagramMapping;
		new RadarDiagram Diagram { get { return (RadarDiagram)base.Diagram; } }
		protected override string TypeName { get { return "RD"; } }
		public ClientRadarDiagram(Chart chart, RadarDiagram diagram)
			: base(chart, diagram) {
			clientRadarDiagramMapping = new ClientRadarDiagramMapping(diagram, ((IRadarDiagram)diagram).RadarDiagramMapping);
		}
		protected override void FillChildElements(List<ClientChartElementBase> childElements) {
			base.FillChildElements(childElements);
			if (((WebChartControl)Chart.Container).EnableClientSideAPI)
				childElements.Add(clientRadarDiagramMapping);
		}
		protected override string GetElementName(ClientChartElementBase element) {
			return element == clientRadarDiagramMapping ? "m" : base.GetElementName(element);
		}
		protected override ClientAxis CreateClientAxisX() {
			ClientAxis axis = new ClientAxis(Chart, Diagram.AxisX);
			axis.SetClientAxisScale(ClientAxisScale.Create(Diagram.AxisX));
			return axis;
		}
		protected override ClientAxis CreateClientAxisY() {
			ClientAxis axis = new ClientAxis(Chart, Diagram.AxisY);
			axis.SetClientAxisScale(ClientAxisScale.Create(Diagram.AxisY));
			return axis;
		}
	}
	public class ClientRadarDiagramMapping : ClientChartElement {
		readonly RadarDiagram diagram;
		readonly RadarDiagramMapping mapping;
		readonly ClientChartElementList clientVertices = new ClientChartElementList();
		protected override object InnerObject { get { return null; } }
		protected override bool ShouldSerializeProperties { get { return mapping != null; } }
		public ClientRadarDiagramMapping(RadarDiagram diagram, RadarDiagramMapping mapping) {
			this.diagram = diagram;
			this.mapping = mapping;
			if (mapping != null && mapping.Vertices != null)
				foreach (RadarDiagramMapping.Vertex vertex in mapping.Vertices)
					clientVertices.Add(new ClientVertex(vertex));
		}
		protected override void FillChildElements(List<ClientChartElementBase> childElements) {
			base.FillChildElements(childElements);
			childElements.Add(clientVertices);
		}
		protected override string GetElementName(ClientChartElementBase element) {
			return element == clientVertices ? "v" : base.GetElementName(element);
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "r", mapping.Radius);
			SerializeProperty(builder, "cx", mapping.Center.X);
			SerializeProperty(builder, "cy", mapping.Center.Y);
			SerializeProperty(builder, "ma", mapping.MinArgument);
			SerializeProperty(builder, "mv", mapping.MinValue);
			SerializeProperty(builder, "ra", mapping.RevertAngle);
			SerializeProperty(builder, "a", mapping.DiagramStartAngle);
			SerializeProperty(builder, "f", mapping.ValueScaleFactor);
			SerializeProperty(builder, "d", mapping.ArgumentDiapason);
			SerializeProperty(builder, "ad", mapping.ArgumentDelta);
			SerializeProperty(builder, "ci", diagram.DrawingStyle == RadarDiagramDrawingStyle.Circle);
			SerializeProperty(builder, "c", mapping.ClipArgument);
			SerializeProperty(builder, "mxa", mapping.MaxArgument);
			SerializeProperty(builder, "mxv", mapping.MaxValue);
			SerializeProperty(builder, "sa", mapping.StartAngle);
		}
	}
	public class ClientVertex : ClientChartElement {
		readonly RadarDiagramMapping.Vertex vertex;
		protected override object InnerObject { get { return null; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientVertex(RadarDiagramMapping.Vertex vertex) {
			this.vertex = vertex;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "a", vertex.Argument);
			SerializeProperty(builder, "x", vertex.X);
			SerializeProperty(builder, "y", vertex.Y);
		}
	}
	public class ClientAxis : ClientChartElementNamed {
		readonly Chart chart;
		readonly AxisBase axis;
		readonly ClientAxisRange clientRange;
		readonly ClientChartElementList labelItems = new ClientChartElementList();
		ClientAxisScale scale;
		protected Chart Chart { get { return chart; } }
		protected AxisBase Axis { get { return axis; } }
		protected override object InnerObject { get { return axis; } }
		public ClientAxis(Chart chart, AxisBase axis)
			: base(axis.Name) {
			this.chart = chart;
			this.axis = axis;
			clientRange = new ClientAxisRange(axis.VisualRange);
		}
		public void SetClientAxisScale(ClientAxisScale scale) {
			this.scale = scale;
		}
		public override void InitializeHitInfo(Dictionary<object, int> objects, Dictionary<object, int> additionalObjects) {
			foreach (object obj in additionalObjects.Keys) {
				AxisLabelItemBase item = obj as AxisLabelItemBase;
				if (item != null && item.Axis == axis)
					labelItems.Add(new ClientAxisLabelItem(item));
			}
			base.InitializeHitInfo(objects, additionalObjects);
		}
		protected override void FillChildElements(List<ClientChartElementBase> childElements) {
			base.FillChildElements(childElements);
			childElements.Add(clientRange);
			if (scale != null && ((WebChartControl)Chart.Container).EnableClientSideAPI)
				childElements.Add(scale);
			childElements.Add(labelItems);
		}
		protected override string GetElementName(ClientChartElementBase element) {
			if (element == clientRange)
				return "r";
			if (element == scale)
				return "m";
			if (element == labelItems)
				return "l";
			return base.GetElementName(element);
		}
	}
	public class ClientAxis2D : ClientAxis {
		readonly ClientAxisTitle clientAxisTitle;
		readonly ClientIntervalBoundsCache clientIntervalBoundsCache;
		readonly ClientChartElements clientStrips;
		readonly ClientChartElements clientConstantLines;
		readonly ClientChartElements clientIntervals;
		readonly ClientCrosshairAxisLabelOptions crosshairAxisLabelOptions;
		protected new Axis2D Axis { get { return (Axis2D)base.Axis; } }
		public ClientAxis2D(Chart chart, Axis2D axis)
			: base(chart, axis) {
			clientAxisTitle = new ClientAxisTitle(axis.Title);
			clientStrips = CreateClientStrips();
			clientConstantLines = CreateClientConstantLines();
			clientIntervals = CreateClientIntervals();
			AxisIntervalLayoutCache cache = chart.GetIntervalBoundsCache(axis);
			if (cache.Count != 0)
				clientIntervalBoundsCache = new ClientIntervalBoundsCache(cache, chart);
			crosshairAxisLabelOptions = new ClientCrosshairAxisLabelOptions(Axis.CrosshairAxisLabelOptions, Axis);
		}
		ClientChartElements CreateClientStrips() {
			int count = Axis.Strips.Count;
			ClientChartElements clientStrips = new ClientChartElements(count);
			for (int i = 0; i < count; i++)
				clientStrips.Elements[i] = new ClientStrip(Axis.Strips[i]);
			return clientStrips;
		}
		ClientChartElements CreateClientConstantLines() {
			int count = Axis.ConstantLines.Count;
			ClientChartElements clientConstantLines = new ClientChartElements(count);
			for (int i = 0; i < count; i++)
				clientConstantLines.Elements[i] = new ClientConstantLine(Axis.ConstantLines[i]);
			return clientConstantLines;
		}
		ClientChartElements CreateClientIntervals() {
			IList<AxisInterval> intervals = Chart.GetAxisIntervals(Axis);
			ClientChartElements clientIntervals = new ClientChartElements(intervals.Count);
			for (int i = 0; i < intervals.Count; i++)
				clientIntervals.Elements[i] = new ClientAxisInterval(intervals[i]);
			return clientIntervals;
		}
		protected override void FillChildElements(List<ClientChartElementBase> childElements) {
			base.FillChildElements(childElements);
			childElements.Add(crosshairAxisLabelOptions);
			childElements.Add(clientAxisTitle);
			childElements.Add(clientStrips);
			childElements.Add(clientConstantLines);
			childElements.Add(clientIntervals);
			if (clientIntervalBoundsCache != null && ((WebChartControl)Chart.Container).EnableClientSideAPI)
				childElements.Add(clientIntervalBoundsCache);
		}
		protected override string GetElementName(ClientChartElementBase element) {
			if (element == clientAxisTitle)
				return "t";
			if (element == clientStrips)
				return "s";
			if (element == clientConstantLines)
				return "cl";
			if (element == clientIntervals)
				return "i";
			if (element == clientIntervalBoundsCache)
				return "ibc";
			if (element == crosshairAxisLabelOptions)
				return "cao";
			return base.GetElementName(element);
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "x", ((IAxisData)Axis).IsArgumentAxis);
			SerializeProperty(builder, "id", Chart.GetActualAxisID(Axis));
		}
	}
	public class ClientAxisXYDiagram2D : ClientAxis2D {
		protected new Axis Axis { get { return (Axis)base.Axis; } }
		public ClientAxisXYDiagram2D(Chart chart, Axis axis)
			: base(chart, axis) {
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "ar", Axis.Reverse);
		}
	}
	public class ClientAxisTitle : ClientChartElement {
		readonly AxisTitle title;
		protected override object InnerObject { get { return title; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientAxisTitle(AxisTitle title) {
			this.title = title;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "tx", title.Text);
		}
	}
	public class ClientAxisLabelItem : ClientChartElement {
		readonly AxisLabelItemBase item;
		protected override object InnerObject { get { return item; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientAxisLabelItem(AxisLabelItemBase item) {
			this.item = item;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "tx", item.Text);
			SerializeProperty(builder, "av", item.AxisValue);
			SerializeProperty(builder, "iv", item.AxisValueInternal);
		}
	}
	public class ClientAxisRange : ClientChartElement {
		readonly Range range;
		protected override object InnerObject { get { return null; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientAxisRange(Range range)
			: base() {
			this.range = range;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			if (range.MinValue != null)
				SerializeProperty(builder, "mi", range.MinValue);
			if (range.MaxValue != null)
				SerializeProperty(builder, "ma", range.MaxValue);
			SerializeProperty(builder, "ii", range.MinValueInternal);
			SerializeProperty(builder, "ia", range.MaxValueInternal);
		}
	}
	public class ClientAxisInterval : ClientChartElement {
		readonly AxisInterval interval;
		protected override object InnerObject { get { return null; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientAxisInterval(AxisInterval interval)
			: base() {
			this.interval = interval;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "mi", interval.Range.Min);
			SerializeProperty(builder, "ma", interval.Range.Max);
		}
	}
	public class ClientIntervalBoundsCache : ClientChartElement {
		readonly ClientChartElementList clientItems = new ClientChartElementList();
		protected override object InnerObject { get { return null; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientIntervalBoundsCache(AxisIntervalLayoutCache cache, Chart chart)
			: base() {
			foreach (KeyValuePair<XYDiagramPaneBase, List<AxisIntervalLayout>> pair in cache)
				clientItems.Add(new ClientIntervalBoundsCacheItem(chart.GetActualPaneID(pair.Key), pair.Value));
		}
		protected override void FillChildElements(List<ClientChartElementBase> childElements) {
			base.FillChildElements(childElements);
			childElements.Add(clientItems);
		}
		protected override string GetElementName(ClientChartElementBase element) {
			return element == clientItems ? "i" : base.GetElementName(element);
		}
	}
	public class ClientIntervalBoundsCacheItem : ClientChartElement {
		readonly int paneID;
		readonly ClientChartElements clientIntervalBoundsList;
		protected override object InnerObject { get { return null; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientIntervalBoundsCacheItem(int paneID, IList<AxisIntervalLayout> intervalBoundsList)
			: base() {
			this.paneID = paneID;
			clientIntervalBoundsList = new ClientChartElements(intervalBoundsList.Count);
			for (int i = 0; i < intervalBoundsList.Count; i++)
				clientIntervalBoundsList.Elements[i] = new ClientIntervalBounds(intervalBoundsList[i].Bounds);
		}
		protected override void FillChildElements(List<ClientChartElementBase> childElements) {
			base.FillChildElements(childElements);
			childElements.Add(clientIntervalBoundsList);
		}
		protected override string GetElementName(ClientChartElementBase element) {
			return element == clientIntervalBoundsList ? "ibl" : base.GetElementName(element);
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "pid", paneID);
		}
	}
	public class ClientIntervalBounds : ClientChartElement {
		readonly Interval intervalBounds;
		protected override object InnerObject { get { return null; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientIntervalBounds(Interval intervalBounds)
			: base() {
			this.intervalBounds = intervalBounds;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "p", intervalBounds.Start);
			SerializeProperty(builder, "l", intervalBounds.Length);
		}
	}
	public abstract class ClientAxisScale : ClientChartElement {
		public static ClientAxisScale Create(AxisBase axis) {
			AxisScaleTypeMap axisMap = ((IAxisData)axis).AxisScaleTypeMap;
			AxisQualitativeMap qualitativeMap = axisMap as AxisQualitativeMap;
			if (qualitativeMap != null)
				return new ClientQualitativeAxisScale(qualitativeMap);
			AxisDateTimeMap dtMap = axisMap as AxisDateTimeMap;
			if (dtMap != null) {
				ScaleMode mode = CommonUtils.GetDateTimeScaleMode(axis);
				DateTimeMeasureUnit measureUnit = mode == ScaleMode.Continuous ? DateTimeMeasureUnit.Millisecond : axis.DateTimeScaleOptions.MeasureUnit;
				DateTimeGridAlignment gridAlignment = axis.DateTimeScaleOptions.GridAlignment;
				return new ClientDateTimeAxisScale(dtMap, measureUnit, gridAlignment, axis.DateTimeScaleOptions.WorkdaysOnly, axis.DateTimeScaleOptions.WorkdaysOptions);
			}
			AxisNumericalMap nuMap = axisMap as AxisNumericalMap;
			if (nuMap != null)
				return new ClientNumericalAxisScale(nuMap);
			return null;
		}
		protected override bool ShouldSerializeProperties { get { return true; } }
	}
	public class ClientQualitativeAxisScale : ClientAxisScale {
		readonly AxisQualitativeMap map;
		protected override object InnerObject { get { return null; } }
		protected override string TypeName { get { return "Q"; } }
		public ClientQualitativeAxisScale(AxisQualitativeMap map)
			: base() {
			this.map = map;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "vl", map.GetValues());
		}
	}
	public class ClientNumericalAxisScale : ClientAxisScale {
		readonly AxisNumericalMap map;
		protected override object InnerObject { get { return null; } }
		protected override string TypeName { get { return "N"; } }
		public ClientNumericalAxisScale(AxisNumericalMap map)
			: base() {
			this.map = map;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			LogarithmicTransformation logarithmicTransformation = map.Transformation as LogarithmicTransformation;
			if (logarithmicTransformation != null) {
				SerializeProperty(builder, "l", true);
				SerializeProperty(builder, "lb", logarithmicTransformation.LogarithmicBase);
				SerializeProperty(builder, "mlv", logarithmicTransformation.MinLogValue);
			}
			else
				SerializeProperty(builder, "l", false);
		}
	}
	public class ClientDateTimeAxisScale : ClientAxisScale {
		static int WeekdaysToString(WeekdayCore weekdays) {
			int result = 0;
			if ((weekdays & WeekdayCore.Sunday) == WeekdayCore.Sunday)
				result |= 1;
			if ((weekdays & WeekdayCore.Monday) == WeekdayCore.Monday)
				result |= 2;
			if ((weekdays & WeekdayCore.Tuesday) == WeekdayCore.Tuesday)
				result |= 4;
			if ((weekdays & WeekdayCore.Wednesdey) == WeekdayCore.Wednesdey)
				result |= 8;
			if ((weekdays & WeekdayCore.Thursday) == WeekdayCore.Thursday)
				result |= 16;
			if ((weekdays & WeekdayCore.Friday) == WeekdayCore.Friday)
				result |= 32;
			if ((weekdays & WeekdayCore.Saturday) == WeekdayCore.Saturday)
				result |= 64;
			return result;
		}
		readonly AxisDateTimeMap map;
		readonly DateTimeMeasureUnit measureUnit;
		readonly DateTimeGridAlignment gridAlignment;
		readonly bool workdaysOnly;
		readonly WeekdayCore workdays;
		readonly IEnumerable<DateTime> holidays;
		readonly IEnumerable<DateTime> exactWorkdays;
		readonly IWorkdaysOptions workdaysOptions;
		protected override object InnerObject { get { return null; } }
		protected override string TypeName { get { return "D"; } }
		public ClientDateTimeAxisScale(AxisDateTimeMap map, DateTimeMeasureUnit measureUnit, DateTimeGridAlignment gridAlignment, bool workdaysOnly, IWorkdaysOptions workdaysOptions) {
			this.map = map;
			this.measureUnit = measureUnit;
			this.gridAlignment = gridAlignment;
			this.workdaysOnly = workdaysOnly;
			this.workdaysOptions = workdaysOptions;
			if (workdaysOnly) {
				workdays = workdaysOptions.Workdays;
				holidays = workdaysOptions.Holidays;
				exactWorkdays = workdaysOptions.ExactWorkdays;
			}
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "su", measureUnit.ToString());
			SerializeProperty(builder, "sa", gridAlignment.ToString());
			DayOfWeek firstDayOfWeek = DateTimeUtilsExt.GetFirstDayOfWeek(workdaysOptions);
			SerializeProperty(builder, "sv", DateTimeUtilsExt.Floor(DateTimeUtilsExt.MinDateTime, (DateTimeMeasureUnitNative)measureUnit, firstDayOfWeek).ToString("yyyy-MM-dd"));
			if (workdaysOnly) {
				SerializeProperty(builder, "swo", true);
				SerializeProperty(builder, "sw", WeekdaysToString(workdays));
				SerializeProperty(builder, "sh", holidays);
				SerializeProperty(builder, "sew", exactWorkdays);
			}
		}
	}
	public class ClientKnownDate : ClientChartElement {
		readonly DateTime date;
		protected override object InnerObject { get { return null; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientKnownDate(DateTime date) {
			this.date = date;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "d", date);
		}
	}
	public class ClientStrip : ClientChartElementNamed {
		readonly Strip strip;
		protected override object InnerObject { get { return strip; } }
		public ClientStrip(Strip strip)
			: base(strip.Name) {
			this.strip = strip;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			if (strip.MinLimit.Enabled)
				SerializeProperty(builder, "mi", strip.MinLimit.AxisValue);
			if (strip.MaxLimit.Enabled)
				SerializeProperty(builder, "ma", strip.MaxLimit.AxisValue);
		}
	}
	public class ClientConstantLine : ClientChartElementNamed {
		readonly ConstantLine constantLine;
		protected override object InnerObject { get { return constantLine; } }
		public ClientConstantLine(ConstantLine constantLine)
			: base(constantLine.Name) {
			this.constantLine = constantLine;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "v", constantLine.AxisValue);
			if (constantLine.Title.Text != String.Empty)
				SerializeProperty(builder, "ti", constantLine.Title.Text);
		}
	}
	public class ClientSeries : ClientChartElementNamed {
		readonly Series series;
		readonly WebChartControl chartControl;
		readonly ClientChartElements clientPoints;
		readonly ClientChartElements clientTitles;
		readonly ClientChartElements clientCrosshairValueItems;
		readonly ClientSeriesLabel clientLabel;
		readonly ClientChartElementList clientIndicators;
		readonly Color? color;
		protected override object InnerObject { get { return series; } }
		internal Color? Color { get { return color; } }
		internal Series Series { get { return series; } }
		public ClientSeries(Series series, WebChartControl chartControl)
			: base(series.Name) {
			this.series = series;
			this.chartControl = chartControl;
			clientPoints = CreateClientPoints();
			clientTitles = CreateClientTitles();
			clientCrosshairValueItems = CreateClientCrosshairValueItems();
			clientLabel = new ClientSeriesLabel(series.Label);
			clientIndicators = CreateClientIndicators();
			color = CommonUtils.GetCrosshairMarkerColor(series);
		}
		ClientChartElements CreateClientPoints() {
			IList<RefinedPoint> points = CommonUtils.GetDisplayPoints(Series);
			int count = points.Count;
			ClientChartElements clientPoints = new ClientChartElements(count);
			for (int i = 0; i < count; i++) {
				Color? crosshairPointColor = CommonUtils.GetCrosshairMarkerColor(series, i, count);
				clientPoints.Elements[i] = new ClientSeriesPoint(points[i], this, crosshairPointColor, chartControl);
			}
			return clientPoints;
		}
		ClientChartElements CreateClientCrosshairValueItems() {
			ClientChartElements clientItems = null;
			if (CommonUtils.GetSeriesActualCrosshairEnabled(Series)) {
				List<CrosshairValueItem> items = CommonUtils.GetCrosshairSortedData(Series);
				if (items != null) {
					clientItems = new ClientChartElements(items.Count);
					for (int i = 0; i < items.Count; i++)
						clientItems.Elements[i] = new ClientCrosshairValueItem(items[i]);
				}
			}
			return clientItems;
		}
		ClientChartElements CreateClientTitles() {
			SimpleDiagramSeriesViewBase view = series.View as SimpleDiagramSeriesViewBase;
			if (view == null)
				return null;
			int count = view.Titles.Count;
			ClientChartElements clientTitles = new ClientChartElements(count);
			for (int i = 0; i < count; i++)
				clientTitles.Elements[i] = new ClientTitle(view.Titles[i]);
			return clientTitles;
		}
		ClientChartElementList CreateClientIndicators() {
			XYDiagram2DSeriesViewBase view = series.View as XYDiagram2DSeriesViewBase;
			if (view == null)
				return null;
			ClientChartElementList clientIndicators = new ClientChartElementList();
			foreach (Indicator indicator in view.Indicators)
				clientIndicators.Add(ClientIndicator.CreateInstance(indicator));
			return clientIndicators;
		}
		protected override void FillChildElements(List<ClientChartElementBase> childElements) {
			base.FillChildElements(childElements);
			if (chartControl.EnableClientSideAPI)
				childElements.Add(clientPoints);
			if (clientTitles != null)
				childElements.Add(clientTitles);
			childElements.Add(clientLabel);
			if (clientIndicators != null)
				childElements.Add(clientIndicators);
			if (chartControl.Chart.ActualCrosshairEnabled && clientCrosshairValueItems != null)
				childElements.Add(clientCrosshairValueItems);
		}
		protected override string GetElementName(ClientChartElementBase element) {
			if (element == clientPoints)
				return "p";
			if (element == clientTitles)
				return "ti";
			if (element == clientLabel)
				return "l";
			if (element == clientIndicators)
				return "id";
			if (element == clientCrosshairValueItems)
				return "cvi";
			return base.GetElementName(element);
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			ViewType viewType = SeriesViewFactory.GetViewType(series.View);
			if (viewType != ViewType.Bar)
				SerializeProperty(builder, "v", Enum.GetName(viewType.GetType(), viewType));
			ScaleType argumentScaleType = series.ActualArgumentScaleType;
			if (argumentScaleType != ScaleType.Qualitative)
				SerializeProperty(builder, "as", Enum.GetName(argumentScaleType.GetType(), argumentScaleType));
			ScaleType valueScaleType = series.ValueScaleType;
			if (valueScaleType != ScaleType.Numerical)
				SerializeProperty(builder, "vs", Enum.GetName(valueScaleType.GetType(), valueScaleType));
			bool toolTipEnabled = CommonUtils.GetSeriesActualToolTipEnabled(series);
			if (toolTipEnabled) {
				if (chartControl.Chart.ActualSeriesToolTipEnabled) {
					PatternParser patternParser = new PatternParser(series.ToolTipSeriesPattern, series.View);
					patternParser.SetContext(series);
					string toolTipText = patternParser.GetText();
					if (!String.IsNullOrEmpty(toolTipText))
						SerializeProperty(builder, "st", toolTipText);
				}
				if (series.ToolTipImage != null && (chartControl.Chart.ActualSeriesToolTipEnabled || chartControl.Chart.ActualPointToolTipEnabled)) {
					string imageUrl = series.ToolTipImage.ImageUrl;
					if (!String.IsNullOrEmpty(imageUrl))
						SerializeProperty(builder, "tti", chartControl.ResolveUrl(imageUrl));
				}
				SerializeProperty(builder, "tte", true);
			}
			if (chartControl.Chart.ActualCrosshairEnabled) {
				bool crosshairEnabled = CommonUtils.GetSeriesActualCrosshairEnabled(Series);
				SerializeProperty(builder, "ace", crosshairEnabled);
				if (crosshairEnabled) {
					if (color != null)
						SerializeProperty(builder, "scc", GetColorString(color.Value));
					IXYSeriesView view = series.View as IXYSeriesView;
					if (view != null) {
						string actualCrosshairLabelPattern = CrosshairPaneInfoEx.GetActualPattern(view);
						bool snapsToArgument = true;
						SerializeProperty(builder, "clp", actualCrosshairLabelPattern.Replace("\n", "<br/>"));
						string groupedElementsPattern;
						if (!String.IsNullOrEmpty(view.CrosshairLabelPattern))
							groupedElementsPattern = actualCrosshairLabelPattern;
						else
							groupedElementsPattern = view.CrosshairConverter.GetGroupedPointPattern(!snapsToArgument, snapsToArgument);
						SerializeProperty(builder, "gep", groupedElementsPattern.Replace("\n", "<br/>"));
					}
					SerializeProperty(builder, "aclv", CommonUtils.GetSeriesActualCrosshairLabelVisibility(Series));
				}
			}
			IXYSeriesView2D xyView = series.View as IXYSeriesView2D;
			if (xyView != null) {
				if (xyView.AxisX != null) {
					SerializeProperty(builder, "ax", xyView.AxisX.Name);
					SerializeProperty(builder, "axi", chartControl.Chart.GetActualAxisID(xyView.AxisX));
				}
				if (xyView.AxisY != null) {
					SerializeProperty(builder, "ay", xyView.AxisY.Name);
					SerializeProperty(builder, "ayi", chartControl.Chart.GetActualAxisID(xyView.AxisY));
				}
				if (xyView.Pane != null) {
					SerializeProperty(builder, "pa", xyView.Pane.Name);
					SerializeProperty(builder, "pi", chartControl.Chart.GetActualPaneID(xyView.Pane));
				}
			}
			if (!series.Visible)
				SerializeProperty(builder, "nvi", true);
			if (chartControl.Chart.ActualCrosshairEnabled) {
				ISideBySideStackedBarSeriesView view = series.View as ISideBySideStackedBarSeriesView;
				if (view != null) {
					object stackedGroup = view.StackedGroup;
					string sg = string.Empty;
					if (stackedGroup != null) {
						sg = stackedGroup.ToString();
					}
					SerializeProperty(builder, "sg", sg);
				}
			}
			IStepSeriesView stepView = series.View as IStepSeriesView;
			if (stepView != null)
				SerializeProperty(builder, "is", stepView.InvertedStep);
		}
	}
	public class ClientSeriesPoint : ClientChartElement {
		readonly ISeriesPoint point;
		readonly RefinedPoint refinedPoint;
		readonly ClientSeries series;
		readonly WebChartControl chartControl;
		readonly ScaleType argumentScaleType;
		readonly ScaleType valueScaleType;
		readonly Color? crosshairPointColor;
		object Argument { get { return point.UserArgument; } }
		Array Values {
			get {
				switch (valueScaleType) {
					case ScaleType.Numerical:
						return point.UserValues;
					case ScaleType.DateTime:
						return point.DateTimeValues;
					default:
						throw new DefaultSwitchException();
				}
			}
		}
		protected override object InnerObject { get { return point; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientSeriesPoint(RefinedPoint refinedPoint, ClientSeries series, Color? crosshairPointColor, WebChartControl chartControl) : base() {
			this.refinedPoint = refinedPoint;
			this.point = refinedPoint.SeriesPoint;
			this.series = series;
			this.chartControl = chartControl;
			this.crosshairPointColor = crosshairPointColor;
			this.argumentScaleType = series.Series.ActualArgumentScaleType;
			this.valueScaleType = series.Series.ValueScaleType;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "x", Argument);
			SerializeProperty(builder, "y", Values);
			bool toolTipEnabled = CommonUtils.GetSeriesActualToolTipEnabled(series.Series);
			if (toolTipEnabled && chartControl.Chart.ActualPointToolTipEnabled) {
				string pattern = ((IPatternHolder)series.Series.View).PointPattern;
				PatternParser patternParser = new PatternParser(pattern, series.Series.View);
				patternParser.SetContext(series.Series);
				patternParser.SetContext(refinedPoint);
				string toolTipText = patternParser.GetText();
				if (!String.IsNullOrEmpty(toolTipText))
					SerializeProperty(builder, "pt", toolTipText.Replace("\n", "<br/>"));
			}
			if (chartControl.Chart.ActualCrosshairEnabled) {
				if (series.Color == null && crosshairPointColor.HasValue)
					SerializeProperty(builder, "pcc", series.GetColorString(crosshairPointColor.Value));
				IXYSeriesView view = series.Series.View as IXYSeriesView;
				if (view != null) {
					IEnumerable<double> crosshairValues = view.GetCrosshairValues(refinedPoint);
					SerializeProperty(builder, "chv", crosshairValues);
				}
				double percentValue = ToolTipUtils.GetPointPercentValue(refinedPoint, series.Series);
				if (!double.IsNaN(percentValue))
					SerializeProperty(builder, "pv", percentValue);
				SeriesPoint seriesPoint = point as SeriesPoint;
				if (seriesPoint != null && !string.IsNullOrEmpty(seriesPoint.ToolTipHint))
					SerializeProperty(builder, "h", seriesPoint.ToolTipHint);
				ISideBySideBarSeriesView sideBySideView = view as ISideBySideBarSeriesView;
				if (sideBySideView != null) {
					SerializeProperty(builder, "o", ((ISideBySidePoint)refinedPoint).Offset);
					SerializeProperty(builder, "fo", ((ISideBySidePoint)refinedPoint).FixedOffset);
				}
				IBarSeriesView barView = view as IBarSeriesView;
				if (barView != null) {
					if (((ISideBySidePoint)refinedPoint).BarWidth != 0)
						SerializeProperty(builder, "bw", ((ISideBySidePoint)refinedPoint).BarWidth);
					else
						SerializeProperty(builder, "bw", barView.BarWidth);
				}
				SerializeProperty(builder, "ie", point.IsEmpty(point.ArgumentScaleType));
			}
		}
	}
	public class ClientSeriesLabel : ClientChartElement {
		readonly SeriesLabelBase label;
		protected override object InnerObject { get { return label; } }
		public ClientSeriesLabel(SeriesLabelBase label)
			: base() {
			this.label = label;
		}
	}
	public class ClientLegend : ClientChartElement {
		readonly Legend legend;
		protected override object InnerObject { get { return legend; } }
		public ClientLegend(Legend legend) : base() {
			this.legend = legend;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			if (legend.UseCheckBoxes)
				SerializeProperty(builder, "chb", legend.UseCheckBoxes);
		}
	}
	public class ClientTitle : ClientChartElement {
		static string AlignmentToString(StringAlignment alignment) {
			switch (alignment) {
				case StringAlignment.Near:
					return "Near";
				case StringAlignment.Center:
					return "Center";
				case StringAlignment.Far:
					return "Far";
				default:
					throw new DefaultSwitchException();
			}
		}
		static string DockToString(ChartTitleDockStyle dockStyle) {
			switch (dockStyle) {
				case ChartTitleDockStyle.Top:
					return "Top";
				case ChartTitleDockStyle.Bottom:
					return "Bottom";
				case ChartTitleDockStyle.Left:
					return "Left";
				case ChartTitleDockStyle.Right:
					return "Right";
				default:
					throw new DefaultSwitchException();
			}
		}
		readonly DockableTitle title;
		protected override object InnerObject { get { return title; } }
		protected override bool ShouldSerializeProperties { get { return base.ShouldSerializeProperties || title.Lines.Length > 0; } }
		public ClientTitle(DockableTitle title)
			: base() {
			this.title = title;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "l", title.Lines);
			SerializeProperty(builder, "a", AlignmentToString(title.Alignment));
			SerializeProperty(builder, "d", DockToString(title.Dock));
		}
	}
	public abstract class ClientIndicator : ClientChartElementNamed {
		public static ClientIndicator CreateInstance(Indicator indicator) {
			if (indicator is RegressionLine)
				return new ClientRegressionLine((RegressionLine)indicator);
			else if (indicator is TrendLine)
				return new ClientTrendLine((TrendLine)indicator);
			else if (indicator is FibonacciIndicator)
				return new ClientFibonacciIndicator((FibonacciIndicator)indicator);
			else if (indicator is SimpleMovingAverage)
				return new ClientSimpleMovingAverage((SimpleMovingAverage)indicator);
			else if (indicator is ExponentialMovingAverage)
				return new ClientExponentialMovingAverage((ExponentialMovingAverage)indicator);
			else if (indicator is WeightedMovingAverage)
				return new ClientWeightedMovingAverage((WeightedMovingAverage)indicator);
			else if (indicator is TriangularMovingAverage)
				return new ClientTrianglarMovingAverage((TriangularMovingAverage)indicator);
			else if (indicator is TripleExponentialMovingAverageTema)
				return new ClientTripleExponentialMovingAverageTema((TripleExponentialMovingAverageTema)indicator);
			else if (indicator is BollingerBands)
				return new ClientBollingerBands((BollingerBands)indicator);
			else if (indicator is MedianPrice)
				return new ClientMedianPrice((MedianPrice)indicator);
			else if (indicator is TypicalPrice)
				return new ClientTypicalPrice((TypicalPrice)indicator);
			else if (indicator is WeightedClose)
				return new ClientWeightedClose((WeightedClose)indicator);
			else if (indicator is AverageTrueRange)
				return new ClientAverageTrueRange((AverageTrueRange)indicator);
			else if (indicator is ChaikinsVolatility)
				return new ClientChaikinsVolatility((ChaikinsVolatility)indicator);
			else if (indicator is CommodityChannelIndex)
				return new ClientCommodityChannelIndex((CommodityChannelIndex)indicator);
			else if (indicator is DetrendedPriceOscillator)
				return new ClientDetrendedPriceOscillator((DetrendedPriceOscillator)indicator);
			else if (indicator is MassIndex)
				return new ClientMassIndex((MassIndex)indicator);
			else if (indicator is MovingAverageConvergenceDivergence)
				return new ClientMovingAverageConvergenceDivergence((MovingAverageConvergenceDivergence)indicator);
			else if (indicator is RateOfChange)
				return new ClientRateOfChange((RateOfChange)indicator);
			else if (indicator is RelativeStrengthIndex)
				return new ClientRelativeStrengthIndex((RelativeStrengthIndex)indicator);
			else if (indicator is StandardDeviation)
				return new ClientStandardDeviation((StandardDeviation)indicator);
			else if (indicator is TripleExponentialMovingAverageTrix)
				return new ClientTripleExponentialMovingAverageTrix((TripleExponentialMovingAverageTrix)indicator);
			else if (indicator is WilliamsR)
				return new ClientWilliamsR((WilliamsR)indicator);
			ChartDebug.Fail("Unknown indicator type");
			return null;
		}
		readonly Indicator indicator;
		protected Chart Chart {
			get {
				IOwnedElement owned = indicator;
				Chart chart = null;
				while (owned.Owner != null) {
					chart = owned.Owner as Chart;
					if (chart != null)
						break;
					owned = owned.Owner;
				}
				return chart;
			}
		}
		protected override object InnerObject { get { return indicator; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		protected ClientIndicator(Indicator indicator)
			: base(indicator.Name) {
			this.indicator = indicator;
		}
	}
	public class ClientFinancialIndicatorPoint : ClientChartElement {
		readonly FinancialIndicatorPoint point;
		protected override object InnerObject { get { return point; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientFinancialIndicatorPoint(FinancialIndicatorPoint point)
			: base() {
			this.point = point;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "a", point.Argument);
			SerializeProperty(builder, "vl", ClientObjectModelHelper.ValueLevelToString(point.ValueLevel));
		}
	}
	public abstract class ClientFinancialIndicator : ClientIndicator {
		readonly ClientFinancialIndicatorPoint clientPoint1;
		readonly ClientFinancialIndicatorPoint clientPoint2;
		public ClientFinancialIndicator(FinancialIndicator financialIndicator)
			: base(financialIndicator) {
			clientPoint1 = new ClientFinancialIndicatorPoint(financialIndicator.Point1);
			clientPoint2 = new ClientFinancialIndicatorPoint(financialIndicator.Point2);
		}
		protected override void FillChildElements(List<ClientChartElementBase> childElements) {
			base.FillChildElements(childElements);
			childElements.Add(clientPoint1);
			childElements.Add(clientPoint2);
		}
		protected override string GetElementName(ClientChartElementBase element) {
			if (element == clientPoint1)
				return "p1";
			if (element == clientPoint2)
				return "p2";
			return base.GetElementName(element);
		}
	}
	public class ClientTrendLine : ClientFinancialIndicator {
		protected override string TypeName { get { return "TL"; } }
		public ClientTrendLine(TrendLine trendLine)
			: base(trendLine) {
		}
	}
	public class ClientFibonacciIndicator : ClientFinancialIndicator {
		protected override string TypeName { get { return "FI"; } }
		public ClientFibonacciIndicator(FibonacciIndicator fibonacciIndicator)
			: base(fibonacciIndicator) {
		}
	}
	public abstract class ClientSingleLevelIndicator : ClientIndicator {
		public ClientSingleLevelIndicator(SingleLevelIndicator indicator)
			: base(indicator) {
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "vl", ClientObjectModelHelper.ValueLevelToString(((SingleLevelIndicator)InnerObject).ValueLevel));
		}
	}
	public class ClientRegressionLine : ClientSingleLevelIndicator {
		protected override string TypeName { get { return "RL"; } }
		public ClientRegressionLine(RegressionLine regressionLine)
			: base(regressionLine) {
		}
	}
	public abstract class ClientMovingAverage : ClientSingleLevelIndicator {
		static string MovingAverageKindToString(MovingAverageKind kind) {
			switch (kind) {
				case MovingAverageKind.MovingAverage:
					return "MovingAverage";
				case MovingAverageKind.Envelope:
					return "Envelope";
				case MovingAverageKind.MovingAverageAndEnvelope:
					return "MovingAverageAndEnvelope";
				default:
					throw new DefaultSwitchException();
			}
		}
		public ClientMovingAverage(MovingAverage movingAverage)
			: base(movingAverage) {
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			MovingAverage movingAverage = (MovingAverage)InnerObject;
			SerializeProperty(builder, "pc", movingAverage.PointsCount);
			SerializeProperty(builder, "ki", MovingAverageKindToString(movingAverage.Kind));
			if (movingAverage.Kind == MovingAverageKind.Envelope || movingAverage.Kind == MovingAverageKind.MovingAverageAndEnvelope)
				SerializeProperty(builder, "ep", movingAverage.EnvelopePercent);
		}
	}
	public class ClientSimpleMovingAverage : ClientMovingAverage {
		protected override string TypeName { get { return "SMA"; } }
		public ClientSimpleMovingAverage(SimpleMovingAverage movingAverage)
			: base(movingAverage) {
		}
	}
	public class ClientExponentialMovingAverage : ClientMovingAverage {
		protected override string TypeName { get { return "EMA"; } }
		public ClientExponentialMovingAverage(ExponentialMovingAverage movingAverage)
			: base(movingAverage) {
		}
	}
	public class ClientWeightedMovingAverage : ClientMovingAverage {
		protected override string TypeName { get { return "WMA"; } }
		public ClientWeightedMovingAverage(WeightedMovingAverage movingAverage)
			: base(movingAverage) {
		}
	}
	public class ClientTrianglarMovingAverage : ClientMovingAverage {
		protected override string TypeName { get { return "TMA"; } }
		public ClientTrianglarMovingAverage(TriangularMovingAverage movingAverage)
			: base(movingAverage) {
		}
	}
	public class ClientTripleExponentialMovingAverageTema : ClientMovingAverage {
		protected override string TypeName { get { return "TEMA"; } }
		public ClientTripleExponentialMovingAverageTema(TripleExponentialMovingAverageTema movingAverage)
			: base(movingAverage) {
		}
	}
	public abstract class ClientSeparatePaneIndicator : ClientIndicator {
		public ClientSeparatePaneIndicator(SeparatePaneIndicator indicator)
			: base(indicator) { }
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			var separatePaneIndicator = (SeparatePaneIndicator)InnerObject;
			if (separatePaneIndicator.AxisY != null) {
				SerializeProperty(builder, "ay", separatePaneIndicator.AxisY.Name);
				SerializeProperty(builder, "ayi", Chart.GetActualAxisID(separatePaneIndicator.AxisY));
			}
			if (separatePaneIndicator.Pane != null) {
				SerializeProperty(builder, "pa", separatePaneIndicator.Pane.Name);
				SerializeProperty(builder, "pi", Chart.GetActualPaneID(separatePaneIndicator.Pane));
			}
		}
	}
	public class ClientBollingerBands : ClientIndicator {
		protected override string TypeName { get { return "BB"; } }
		public ClientBollingerBands(BollingerBands indicator)
			: base(indicator) {
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			var bollingerBands = (BollingerBands)InnerObject;
			SerializeProperty(builder, "pc", bollingerBands.PointsCount);
			SerializeProperty(builder, "vl", ClientObjectModelHelper.ValueLevelToString(bollingerBands.ValueLevel));
		}
	}
	public class ClientMedianPrice : ClientIndicator {
		protected override string TypeName { get { return "MP"; } }
		public ClientMedianPrice(MedianPrice indicator)
			: base(indicator) {
		}
	}
	public class ClientTypicalPrice : ClientIndicator {
		protected override string TypeName { get { return "TP"; } }
		public ClientTypicalPrice(TypicalPrice indicator)
			: base(indicator) {
		}
	}
	public class ClientWeightedClose : ClientIndicator {
		protected override string TypeName { get { return "WC"; } }
		public ClientWeightedClose(WeightedClose indicator)
			: base(indicator) {
		}
	}
	public class ClientAverageTrueRange : ClientSeparatePaneIndicator {
		protected override string TypeName { get { return "ATR"; } }
		public ClientAverageTrueRange(AverageTrueRange indicator)
			: base(indicator) {
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			var atr = (AverageTrueRange)InnerObject;
			SerializeProperty(builder, "pc", atr.PointsCount);
		}
	}
	public class ClientChaikinsVolatility : ClientSeparatePaneIndicator {
		protected override string TypeName { get { return "ChV"; } }
		public ClientChaikinsVolatility(ChaikinsVolatility indicator)
			: base(indicator) {
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			var indicator = (ChaikinsVolatility)InnerObject;
			SerializeProperty(builder, "pc", indicator.PointsCount);
		}
	}
	public class ClientCommodityChannelIndex : ClientSeparatePaneIndicator {
		protected override string TypeName { get { return "CCI"; } }
		public ClientCommodityChannelIndex(CommodityChannelIndex indicator)
			: base(indicator) {
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			var indicator = (CommodityChannelIndex)InnerObject;
			SerializeProperty(builder, "pc", indicator.PointsCount);
		}
	}
	public class ClientDetrendedPriceOscillator : ClientSeparatePaneIndicator {
		protected override string TypeName { get { return "DPO"; } }
		public ClientDetrendedPriceOscillator(DetrendedPriceOscillator indicator)
			: base(indicator) {
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			var indicator = (DetrendedPriceOscillator)InnerObject;
			SerializeProperty(builder, "pc", indicator.PointsCount);
			SerializeProperty(builder, "vl", ClientObjectModelHelper.ValueLevelToString(indicator.ValueLevel));
		}
	}
	public class ClientMassIndex : ClientSeparatePaneIndicator {
		protected override string TypeName { get { return "MI"; } }
		public ClientMassIndex(MassIndex indicator)
			: base(indicator) {
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			var indicator = (MassIndex)InnerObject;
			SerializeProperty(builder, "mapc", indicator.MovingAveragePointsCount);
			SerializeProperty(builder, "spc", indicator.SumPointsCount);
		}
	}
	public class ClientMovingAverageConvergenceDivergence : ClientSeparatePaneIndicator {
		protected override string TypeName { get { return "MACD"; } }
		public ClientMovingAverageConvergenceDivergence(MovingAverageConvergenceDivergence indicator)
			: base(indicator) {
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			var indicator = (MovingAverageConvergenceDivergence)InnerObject;
			SerializeProperty(builder, "shp", indicator.ShortPeriod);
			SerializeProperty(builder, "lp", indicator.LongPeriod);
			SerializeProperty(builder, "ssp", indicator.SignalSmoothingPeriod);
		}
	}
	public class ClientRateOfChange : ClientSeparatePaneIndicator {
		protected override string TypeName { get { return "RoC"; } }
		public ClientRateOfChange(RateOfChange indicator)
			: base(indicator) {
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			var indicator = (RateOfChange)InnerObject;
			SerializeProperty(builder, "pc", indicator.PointsCount);
			SerializeProperty(builder, "vl", ClientObjectModelHelper.ValueLevelToString(indicator.ValueLevel));
		}
	}
	public class ClientRelativeStrengthIndex : ClientSeparatePaneIndicator {
		protected override string TypeName { get { return "RSI"; } }
		public ClientRelativeStrengthIndex(RelativeStrengthIndex indicator)
			: base(indicator) {
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			var indicator = (RelativeStrengthIndex)InnerObject;
			SerializeProperty(builder, "pc", indicator.PointsCount);
			SerializeProperty(builder, "vl", ClientObjectModelHelper.ValueLevelToString(indicator.ValueLevel));
		}
	}
	public class ClientStandardDeviation : ClientSeparatePaneIndicator {
		protected override string TypeName { get { return "StdDev"; } }
		public ClientStandardDeviation(StandardDeviation indicator)
			: base(indicator) {
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			var indicator = (StandardDeviation)InnerObject;
			SerializeProperty(builder, "pc", indicator.PointsCount);
			SerializeProperty(builder, "vl", ClientObjectModelHelper.ValueLevelToString(indicator.ValueLevel));
		}
	}
	public class ClientTripleExponentialMovingAverageTrix : ClientSeparatePaneIndicator {
		protected override string TypeName { get { return "TRIX"; } }
		public ClientTripleExponentialMovingAverageTrix(TripleExponentialMovingAverageTrix indicator)
			: base(indicator) {
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			var indicator = (TripleExponentialMovingAverageTrix)InnerObject;
			SerializeProperty(builder, "pc", indicator.PointsCount);
			SerializeProperty(builder, "vl", ClientObjectModelHelper.ValueLevelToString(indicator.ValueLevel));
		}
	}
	public class ClientWilliamsR : ClientSeparatePaneIndicator {
		protected override string TypeName { get { return "WR"; } }
		public ClientWilliamsR(WilliamsR indicator)
			: base(indicator) {
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			var indicator = (WilliamsR)InnerObject;
			SerializeProperty(builder, "pc", indicator.PointsCount);
		}
	}
	public abstract class ClientAnnotation : ClientChartElementNamed {
		public static ClientAnnotation CreateInstance(Annotation annotation) {
			if (annotation is TextAnnotation)
				return new ClientTextAnnotation((TextAnnotation)annotation);
			if (annotation is ImageAnnotation)
				return new ClientImageAnnotation((ImageAnnotation)annotation);
			ChartDebug.Fail("Unknown annotation type");
			return null;
		}
		readonly Annotation annotation;
		protected Annotation Annotation { get { return annotation; } }
		protected override object InnerObject { get { return annotation; } }
		public ClientAnnotation(Annotation annotation)
			: base(annotation.Name) {
			this.annotation = annotation;
		}
	}
	public class ClientTextAnnotation : ClientAnnotation {
		TextAnnotation TextAnnotation { get { return (TextAnnotation)base.Annotation; } }
		protected override string TypeName { get { return "TA"; } }
		public ClientTextAnnotation(TextAnnotation textAnnotation)
			: base(textAnnotation) {
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "l", TextAnnotation.Lines);
		}
	}
	public class ClientImageAnnotation : ClientAnnotation {
		protected override string TypeName { get { return "IA"; } }
		public ClientImageAnnotation(ImageAnnotation imageAnnotation)
			: base(imageAnnotation) {
		}
	}
	public abstract class ClientToolTipPosition : ClientChartElement {
		ToolTipPosition position;
		protected override object InnerObject { get { return position; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientToolTipPosition(ToolTipPosition position) {
			this.position = position;
		}
	}
	public class ClientToolTipMousePosition : ClientToolTipPosition {
		protected override string TypeName { get { return "MP"; } }
		public ClientToolTipMousePosition(ToolTipMousePosition position)
			: base(position) {
		}
	}
	public class ClientToolTipFreePosition : ClientToolTipPosition {
		ToolTipFreePosition position;
		WebChartControl chartControl;
		protected override string TypeName { get { return "FP"; } }
		public ClientToolTipFreePosition(WebChartControl chartControl, ToolTipFreePosition position)
			: base(position) {
			this.position = position;
			this.chartControl = chartControl;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "ox", position.OffsetX);
			SerializeProperty(builder, "oy", position.OffsetY);
			if (position.DockTarget != null && position.DockTarget is XYDiagramPaneBase)
				SerializeProperty(builder, "dt", chartControl.Chart.GetActualPaneID((XYDiagramPaneBase)position.DockTarget));
			ToolTipDockCorner dockCorner = position.DockCorner;
			SerializeProperty(builder, "dp", Enum.GetName(dockCorner.GetType(), dockCorner));
		}
	}
	public class ClientToolTipRelativePosition : ClientToolTipPosition {
		ToolTipRelativePosition position;
		protected override string TypeName { get { return "RP"; } }
		public ClientToolTipRelativePosition(ToolTipRelativePosition position)
			: base(position) {
			this.position = position;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "ox", position.OffsetX);
			SerializeProperty(builder, "oy", position.OffsetY);
		}
	}
	public class ClientToolTipController : ClientChartElement {
		ChartToolTipController toolTipController;
		protected override object InnerObject { get { return toolTipController; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientToolTipController(ChartToolTipController toolTipController) {
			this.toolTipController = toolTipController;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "st", toolTipController.ShowText);
			SerializeProperty(builder, "si", toolTipController.ShowImage);
			ChartImagePosition imagePosition = toolTipController.ImagePosition;
			SerializeProperty(builder, "ip", Enum.GetName(imagePosition.GetType(), imagePosition));
			ToolTipOpenMode openMode = toolTipController.OpenMode;
			SerializeProperty(builder, "om", Enum.GetName(openMode.GetType(), openMode));
		}
	}
	public class ClientCrosshairOptions : ClientChartElement {
		readonly ClientCrosshairLabelPosition position;
		readonly ClientLineStyle argumentLineStyle;
		readonly ClientLineStyle valueLineStyle;
		CrosshairOptions crosshairOptions;
		WebChartControl chartControl;
		protected override object InnerObject { get { return crosshairOptions; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientCrosshairOptions(WebChartControl chartControl, CrosshairOptions crosshairOptions) {
			this.crosshairOptions = crosshairOptions;
			this.chartControl = chartControl;
			position = CreateClientCrosshairLabelPosition();
			argumentLineStyle = new ClientLineStyle(crosshairOptions.ArgumentLineStyle);
			valueLineStyle = new ClientLineStyle(crosshairOptions.ValueLineStyle);
		}
		ClientCrosshairLabelPosition CreateClientCrosshairLabelPosition() {
			if (crosshairOptions.CrosshairLabelMode == CrosshairLabelMode.ShowCommonForAllSeries) {
				CrosshairLabelPosition position = crosshairOptions.CommonLabelPosition;
				if (position is CrosshairMousePosition)
					return new ClientCrosshairMousePosition(position);
				if (position is CrosshairFreePosition)
					return new ClientCrosshairFreePosition(chartControl, position);
			}
			return null;
		}
		protected override string GetElementName(ClientChartElementBase element) {
			if (element == position)
				return "clp";
			if (element == argumentLineStyle)
				return "als";
			if (element == valueLineStyle)
				return "vls";
			return base.GetElementName(element);
		}
		protected override void FillChildElements(List<ClientChartElementBase> childElements) {
			base.FillChildElements(childElements);
			if (position != null && chartControl.Chart.ActualCrosshairEnabled) {
				childElements.Add(position);
				if (crosshairOptions.ShowArgumentLine)
					childElements.Add(argumentLineStyle);
				if (crosshairOptions.ShowValueLine)
					childElements.Add(valueLineStyle);
			}
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "sx", crosshairOptions.ShowArgumentLabels);
			SerializeProperty(builder, "sy", crosshairOptions.ShowValueLabels);
			SerializeProperty(builder, "sl", crosshairOptions.ShowCrosshairLabels);
			SerializeProperty(builder, "sxl", crosshairOptions.ShowArgumentLine);
			SerializeProperty(builder, "syl", crosshairOptions.ShowValueLine);
			SerializeProperty(builder, "sfp", crosshairOptions.ShowOnlyInFocusedPane);
			SerializeProperty(builder, "sm", Enum.GetName(crosshairOptions.SnapMode.GetType(), crosshairOptions.SnapMode));
			SerializeProperty(builder, "lm", Enum.GetName(crosshairOptions.CrosshairLabelMode.GetType(), crosshairOptions.CrosshairLabelMode));
			Color? argumentLineColor = crosshairOptions.ArgumentLineColor;
			if (argumentLineColor.HasValue)
				SerializeProperty(builder, "alc", GetColorString(argumentLineColor.Value));
			Color? valueLineColor = crosshairOptions.ValueLineColor;
			if (valueLineColor.HasValue)
				SerializeProperty(builder, "vlc", GetColorString(valueLineColor.Value));
			SerializeProperty(builder, "sgh", crosshairOptions.ShowGroupHeaders);
			string pattern;
			if (String.IsNullOrEmpty(crosshairOptions.GroupHeaderPattern)) {
				bool snapsToArgument = crosshairOptions.SnapMode == CrosshairSnapMode.NearestArgument;
				CrosshairGroupHeaderValueToStringConverter converter = new CrosshairGroupHeaderValueToStringConverter(chartControl.Series[0], snapsToArgument, !snapsToArgument);
				pattern = converter.DefaulPointPattern;
			}
			else
				pattern = crosshairOptions.GroupHeaderPattern;
			SerializeProperty(builder, "ghp", pattern.Replace("\n", "<br/>"));
		}
	}
	public class ClientCrosshairValueItem : ClientChartElement {
		CrosshairValueItem item;
		protected override object InnerObject { get { return item; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientCrosshairValueItem(CrosshairValueItem item) {
			this.item = item;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "v", item.Value);
			SerializeProperty(builder, "pi", item.PointIndex);
		}
	}
	public class ClientAxisLabelBounds : ClientChartElement {
		readonly int axisID;
		readonly bool isAxisX;
		readonly Rectangle labelBounds;
		protected override object InnerObject { get { return null; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientAxisLabelBounds(int axisID, bool isAxisX, Rectangle labelBounds) {
			this.axisID = axisID;
			this.isAxisX = isAxisX;
			this.labelBounds = labelBounds;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "ai", axisID);
			SerializeProperty(builder, "ax", isAxisX);
			SerializeProperty(builder, "lt", labelBounds.Top);
			SerializeProperty(builder, "ll", labelBounds.Left);
			SerializeProperty(builder, "lh", labelBounds.Height);
			SerializeProperty(builder, "lw", labelBounds.Width);
		}
	}
	public abstract class ClientCrosshairLabelPosition : ClientChartElement {
		CrosshairLabelPosition position;
		protected CrosshairLabelPosition Position { get { return position; } }
		protected override object InnerObject { get { return position; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientCrosshairLabelPosition(CrosshairLabelPosition position) {
			this.position = position;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "ox", position.OffsetX);
			SerializeProperty(builder, "oy", position.OffsetY);
		}
	}
	public class ClientCrosshairMousePosition : ClientCrosshairLabelPosition {
		protected override string TypeName { get { return "CMP"; } }
		public ClientCrosshairMousePosition(CrosshairLabelPosition position)
			: base(position) {
		}
	}
	public class ClientCrosshairFreePosition : ClientCrosshairLabelPosition {
		WebChartControl chartControl;
		protected override string TypeName { get { return "CFP"; } }
		public ClientCrosshairFreePosition(WebChartControl chartControl, CrosshairLabelPosition position)
			: base(position) {
			this.chartControl = chartControl;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			CrosshairFreePosition freePosition = (CrosshairFreePosition)Position;
			if (freePosition.DockTarget != null && freePosition.DockTarget is XYDiagramPaneBase)
				SerializeProperty(builder, "dt", chartControl.Chart.GetActualPaneID((XYDiagramPaneBase)freePosition.DockTarget));
			DockCorner dockCorner = freePosition.DockCorner;
			SerializeProperty(builder, "dp", Enum.GetName(dockCorner.GetType(), dockCorner));
		}
	}
	public class ClientLineStyle : ClientChartElement {
		LineStyle lineStyle;
		protected override object InnerObject { get { return lineStyle; } }
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientLineStyle(LineStyle lineStyle) {
			this.lineStyle = lineStyle;
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "th", lineStyle.Thickness);
			SerializeProperty(builder, "ds", Enum.GetName(lineStyle.DashStyle.GetType(), lineStyle.DashStyle));
			SerializeProperty(builder, "lj", Enum.GetName(lineStyle.LineJoin.GetType(), lineStyle.LineJoin));
		}
	}
	public class ClientFont : ClientChartElement {
		Font font;
		public ClientFont(Font font) {
			this.font = font;
		}
		protected override bool ShouldSerializeProperties { get { return true; } }
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "fs", font.Size);
			SerializeProperty(builder, "ff", font.Name);
		}
		protected override object InnerObject {
			get { return font; }
		}
	}
	public class ClientCrosshairAxisLabelOptions : ClientChartElement {
		readonly string textPattern;
		CrosshairAxisLabelOptions crosshairAxisLabelOptions;
		ClientFont crosshairLabelFont;
		protected override bool ShouldSerializeProperties { get { return true; } }
		public ClientCrosshairAxisLabelOptions(CrosshairAxisLabelOptions crosshairAxisLabelOptions, Axis2D axis) {
			this.crosshairAxisLabelOptions = crosshairAxisLabelOptions;
			crosshairLabelFont = new ClientFont(crosshairAxisLabelOptions.ActualFont);
			textPattern = GetActualTextPattern(crosshairAxisLabelOptions.ActualPattern, axis);
		}
		string GetActualTextPattern(string optionsPattern, Axis2D axis) {
			return string.IsNullOrEmpty(optionsPattern) ? ((IAxisLabel)axis.Label).TextPattern : optionsPattern;
		}
		protected override string GetElementName(ClientChartElementBase element) {
			if (element == crosshairLabelFont)
				return "clf";
			return base.GetElementName(element);
		}
		protected override void FillChildElements(List<ClientChartElementBase> childElements) {
			base.FillChildElements(childElements);
			childElements.Add(crosshairLabelFont);
		}
		protected override void SerializeProperties(StringBuilder builder) {
			base.SerializeProperties(builder);
			SerializeProperty(builder, "clp", textPattern.Replace("\n", "<br/>"));
			SerializeProperty(builder, "clv", crosshairAxisLabelOptions.ActualVisibility);
			SerializeColorProperty(builder, "clbc", crosshairAxisLabelOptions.ActualBackColor);
			SerializeColorProperty(builder, "cltc", crosshairAxisLabelOptions.ActualTextColor);
		}
		void SerializeColorProperty(StringBuilder builder, string serializeName, Color color) {
			Color? oColor = color;
			if (oColor.HasValue && !color.IsEmpty)
				SerializeProperty(builder, serializeName, GetColorString(color));
		}
		protected override object InnerObject {
			get { return crosshairAxisLabelOptions; }
		}
	}
}
