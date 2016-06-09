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
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Web;
namespace DevExpress.XtraCharts.Native {
	public class ClientHitInfoSerializer : IHitRegionSerializer {
		static bool IsStartPoint(byte pointType) {
			return pointType == 0x00;
		}
		static bool IsBezierPoint(byte pointType) {
			return (pointType & 0x03) == 0x03;
		}
		static int CalculateID(Dictionary<object, int> dictionary, object obj) {
			if(!dictionary.ContainsKey(obj))
				dictionary.Add(obj, dictionary.Count);
			return dictionary[obj];
		}
		JavaScriptWriter writer;
		Dictionary<object, int> objects = new Dictionary<object, int>();
		Dictionary<object, int> additionalObjects = new Dictionary<object, int>();
		Dictionary<ISeriesPoint, DiagramPoint> seriesPointRelativePositions = new Dictionary<ISeriesPoint, DiagramPoint>();
		Dictionary<IHitRegion, string> legendCheckboxRegions = new Dictionary<IHitRegion, string>();
		WebChartControl chartControl;
		public Dictionary<object, int> Objects { get { return objects; } }
		public Dictionary<object, int> AdditionalObjects { get { return additionalObjects; } }
		public ClientHitInfoSerializer(StringBuilder builder, WebChartControl chartControl) {
			this.chartControl = chartControl;
			this.writer = new JavaScriptWriter(builder);
		}
		#region IHitRegionSerializer implementation
		void IHitRegionSerializer.SerializeRectangle(RectangleF rect) {
			SerializeRegionType("R");
			writer.WriteSerapator();
			writer.WriteProperty("r", new float[] { rect.Left, rect.Top, rect.Right, rect.Bottom });
		}
		void IHitRegionSerializer.SerializePath(GraphicsPath path) {
			if(path.PointCount == 0) {
				SerializeEmptyRegion();
				return;
			}
			SerializeRegionType("P");
			writer.WriteSerapator();
			writer.BeginWriteProperty("p");
			SerializePoints(path.PathPoints);
			List<int> startIndexes = new List<int>();
			List<int> bezierIndexes = new List<int>();
			for(int i = 0; i < path.PointCount; i++) {
				if(IsStartPoint(path.PathTypes[i]))
					startIndexes.Add(i);
				if(IsBezierPoint(path.PathTypes[i]))
					bezierIndexes.Add(i);
			}
			writer.WriteSerapator();
			writer.WriteProperty("s", startIndexes.ToArray());
			writer.WriteSerapator();
			writer.WriteProperty("b", bezierIndexes.ToArray());
			writer.WriteSerapator();
			writer.WriteProperty("a", path.FillMode == System.Drawing.Drawing2D.FillMode.Alternate);
		}
		void IHitRegionSerializer.SerializeEmptyRegion() {
			SerializeEmptyRegion();
		}
		void IHitRegionSerializer.SerializeUnionExpression(IHitRegion leftOperand, IHitRegion rightOperand) {
			SerializeRegionExpression("U", leftOperand, rightOperand);
		}
		void IHitRegionSerializer.SerializeIntersectExpression(IHitRegion leftOperand, IHitRegion rightOperand) {
			SerializeRegionExpression("I", leftOperand, rightOperand);
		}
		void IHitRegionSerializer.SerializeExcludeExpression(IHitRegion leftOperand, IHitRegion rightOperand) {
			SerializeRegionExpression("E", leftOperand, rightOperand);
		}
		void IHitRegionSerializer.SerializeXorExpression(IHitRegion leftOperand, IHitRegion rightOperand) {
			SerializeRegionExpression("X", leftOperand, rightOperand);
		}
		#endregion
		void SerializePoint(PointF point) {
			writer.Write(new float[] { point.X, point.Y });
		}
		void SerializePoints(PointF[] points) {
			writer.BeginWriteArray();
			if(points.Length > 0) {
				SerializePoint(points[0]);
				for(int i = 1; i < points.Length; i++) {
					writer.WriteSerapator();
					SerializePoint(points[i]);
				}
			}
			writer.EndWriteArray();
		}
		void SerializeRegionType(string type) {
			writer.WriteProperty("t", type);
		}
		void SerializeRegionExpression(string type, IHitRegion leftOperand, IHitRegion rightOperand) {
			SerializeRegionType(type);
			writer.WriteSerapator();
			writer.BeginWriteProperty("l");
			SerializeHitRegion(leftOperand);
			writer.WriteSerapator();
			writer.BeginWriteProperty("r");
			SerializeHitRegion(rightOperand);
		}
		void SerializeEmptyRegion() {
			SerializeRegionType("O");
		}
		void SerializeObjectID(object obj) {
			int objectID = CalculateID(objects, obj);
			writer.WriteProperty("hi", objectID);
		}
		void SerializeAdditionalObjectID(object additionalObj) {
			RefinedPoint refinedPoint = additionalObj as RefinedPoint;
			int additionalObjectID = refinedPoint != null ? CalculateID(additionalObjects, refinedPoint.SeriesPoint) :
														   CalculateID(additionalObjects, additionalObj);
			writer.WriteProperty("hia", additionalObjectID);
		}
		void SerializeToolTipRelativePosition(object additionalObj) {
			RefinedPoint refinedPoint = additionalObj as RefinedPoint;
			if (refinedPoint != null && seriesPointRelativePositions.Count != 0 && seriesPointRelativePositions.ContainsKey(refinedPoint.SeriesPoint)) {
				writer.WriteSerapator();
				writer.BeginWriteProperty("ttp");
				SerializePoint((PointF)(seriesPointRelativePositions[refinedPoint.SeriesPoint]));
			}
		}
		void SerializeHyperlink(object additionalObj) {
			HyperlinkSource linkSource = additionalObj as HyperlinkSource;
			if (linkSource != null) {
				writer.WriteSerapator();
				writer.BeginWriteProperty("l");
				writer.Write(linkSource.Hyperlink);
			}
		}
		void SerializeHitRegion(IHitRegion hitRegion) {
			writer.BeginWriteObject();
			hitRegion.Serialize(this);
			writer.EndWriteObject();
		}
		void SerializeHitParams(HitTestParams hitParams) {
			writer.BeginWriteObject();
			if(hitParams.Object == null || hitParams.Object.Object == null)
				throw new InternalException("ClientHitInfoSerializer error");
			SerializeObjectID(hitParams.Object.Object);
			object additionalObject = hitParams.AdditionalObj;
			if (additionalObject != null) {
				writer.WriteSerapator();
				SerializeAdditionalObjectID(additionalObject);
				SerializeToolTipRelativePosition(additionalObject);
				SerializeHyperlink(additionalObject);
			}
			if (hitParams.Object is Legend && legendCheckboxRegions.Count > 0)
				SerializeCheckBoxesHitRegions();
			writer.WriteSerapator();
			writer.BeginWriteProperty("r");
			SerializeHitRegion(hitParams.HitRegion);
			writer.EndWriteObject();
		}
		void SerializeCheckBoxesHitRegions() {
			writer.WriteSerapator();
			writer.BeginWriteProperty("legChbRegns");
			writer.BeginWriteArray();
			int counter = 0;
			foreach (IHitRegion region in this.legendCheckboxRegions.Keys) {
				if (counter != 0)
					writer.WriteSerapator();
				writer.BeginWriteObject();
				writer.WriteProperty("legItmId", legendCheckboxRegions[region]);
				writer.WriteSerapator();
				writer.BeginWriteProperty("r");
				SerializeHitRegion(region);
				writer.EndWriteObject();
				counter++;
			}
			writer.EndWriteArray();
		}
		public void Serialize(HitTestController hitController) {
			writer.BeginWriteArray();
			List<HitTestParams> chartFocusedAreaParams;
			IList<HitTestParams> hitTestParams = hitController.GetItems(true, out chartFocusedAreaParams);
			if (chartFocusedAreaParams != null) {
				for (int i = 0; i < chartFocusedAreaParams.Count; i++) {
					ChartFocusedArea area = chartFocusedAreaParams[i].AdditionalObj as ChartFocusedArea;
					if (area != null && area.RelativePosition.HasValue) {
						ISeriesPoint seriesPoint = area.Element as ISeriesPoint;
						if (seriesPoint != null && !seriesPointRelativePositions.ContainsKey(seriesPoint))
							seriesPointRelativePositions.Add(seriesPoint, area.RelativePosition.Value);
					}
					else if (area != null && area.Element is LegendItemViewData) {
						LegendItemViewData legendItemVD = (LegendItemViewData)area.Element;
						ILegendItem representedByLegItemVDChartObject = ((ICheckableLegendItemData)legendItemVD.Item).RepresentedObject as ILegendItem;
						string chartElementPath = chartControl.CalculatePathForRepresentedChartElement(representedByLegItemVDChartObject);
						legendCheckboxRegions.Add(chartFocusedAreaParams[i].HitRegion, chartElementPath);
					}
				}
			}
			if (hitTestParams.Count > 0) {
				SerializeHitParams(hitTestParams[0]);				
				for (int i = 1; i < hitTestParams.Count; i++) {
					writer.WriteSerapator();
					SerializeHitParams(hitTestParams[i]);
				}
			}
			writer.EndWriteArray();
		}
		public override string ToString() {
			return writer.ToString();
		}
	}
	public interface IJavaScriptObjectConverter {
		string ToString(object value);
	}
	public class JSDefaultConverter : IJavaScriptObjectConverter {
		public string ToString(object value) {
			return value.ToString();
		}
	}
	public class JSStringConverter : IJavaScriptObjectConverter {
		public string ToString(object value) {
			return "'" + (string)value + "'";
		}
	}
	public class JSBooleanConverter : IJavaScriptObjectConverter {
		public string ToString(object value) {
			return (bool)value ? "true" : "false";
		}
	}
	public class JSFloatConverter : IJavaScriptObjectConverter {
		public string ToString(object value) {
			float roundValue = (float)Math.Round((float)value, 1);
			float ceilingValue = (float)Math.Ceiling(roundValue);
			int precision = ceilingValue != roundValue ? 1 : 0;
			return roundValue.ToString("f" + precision.ToString(), CultureInfo.InvariantCulture.NumberFormat);
		}
	}
	public class JavaScriptWriter {
		static IJavaScriptObjectConverter defaultConverter = new JSDefaultConverter();
		static IJavaScriptObjectConverter stringConverter = new JSStringConverter();
		static IJavaScriptObjectConverter booleanConverter = new JSBooleanConverter();
		static IJavaScriptObjectConverter floatConverter = new JSFloatConverter();
		StringBuilder builder;
		public JavaScriptWriter(StringBuilder builder) {
			this.builder = builder;
		}
		void WriteArray(Array array, IJavaScriptObjectConverter jsConverter) {
			BeginWriteArray();
			if(array.Length > 0) {
				builder.Append(jsConverter.ToString(array.GetValue(0)));
				for(int i = 1; i < array.Length; i++) {
					WriteSerapator();
					builder.Append(jsConverter.ToString(array.GetValue(i)));
				}
			}
			EndWriteArray();
		}
		public void Write(string value) {
			builder.Append(stringConverter.ToString(value));
		}
		public void Write(bool value) {
			builder.Append(booleanConverter.ToString(value));
		}
		public void Write(float value) {
			builder.Append(floatConverter.ToString(value));
		}
		public void Write(int value) {
			builder.Append(defaultConverter.ToString(value));
		}
		public void Write(string[] values) {
			WriteArray(values, stringConverter);
		}
		public void Write(bool[] values) {
			WriteArray(values, booleanConverter);
		}
		public void Write(float[] values) {
			WriteArray(values, floatConverter);
		}
		public void Write(int[] values) {
			WriteArray(values, defaultConverter);
		}
		public void WriteProperty(string name, string value) {
			BeginWriteProperty(name);
			Write(value);
		}
		public void WriteProperty(string name, bool value) {
			BeginWriteProperty(name);
			Write(value);
		}
		public void WriteProperty(string name, float value) {
			BeginWriteProperty(name);
			Write(value);
		}
		public void WriteProperty(string name, int value) {
			BeginWriteProperty(name);
			Write(value);
		}
		public void WriteProperty(string name, float[] values) {
			BeginWriteProperty(name);
			Write(values);
		}
		public void WriteProperty(string name, int[] values) {
			BeginWriteProperty(name);
			Write(values);
		}
		public void BeginWriteProperty(string name) {
			builder.Append(name + ":");
		}
		public void WriteSerapator() {
			builder.Append(",");
		}
		public void BeginWriteObject() {
			builder.Append("{");
		}
		public void EndWriteObject() {
			builder.Append("}");
		}
		public void BeginWriteArray() {
			builder.Append("[");
		}
		public void EndWriteArray() {
			builder.Append("]");
		}
		public override string ToString() {
			return builder.ToString();
		}
	}
}
