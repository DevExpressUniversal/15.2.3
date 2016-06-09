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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)
	]
	public enum BubbleLabelValueToDisplay {
		Weight = 0,
		Value = 1,
		ValueAndWeight = 2
	}
	[
	TypeConverter(typeof(BubbleSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class BubbleSeriesLabel : PointSeriesLabel {
		const double DefaultIndentFromMarker = 0;
		const BubbleLabelValueToDisplay DefaultValueToDisplay = BubbleLabelValueToDisplay.Weight;
		double indentFromMarker = DefaultIndentFromMarker;
		BubbleLabelValueToDisplay valueToDisplay = DefaultValueToDisplay;
		protected override int DefaultAngleDegree { get { return 90; } }
		protected override PointLabelPosition DefaultPosition { get { return PointLabelPosition.Center; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BubbleSeriesLabelIndentFromMarker"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BubbleSeriesLabel.IndentFromMarker"),
		Category(Categories.Appearance),
		XtraSerializableProperty
		]
		public double IndentFromMarker {
			get { return indentFromMarker; }
			set {
				if (value != indentFromMarker) {
					SendNotification(new ElementWillChangeNotification(this));
					indentFromMarker = value;
					RaiseControlChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new int LineLength { get { return 0; } }
		#region Obsolete Properties
		[
		Obsolete("This property is obsolete now. Use the TextPattern property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public BubbleLabelValueToDisplay ValueToDisplay {
			get { return valueToDisplay; }
			set {
				if (value != valueToDisplay) {
					SendNotification(new ElementWillChangeNotification(this));
					valueToDisplay = value;
					UpdateTextPattern(PointOptions);
					RaiseControlChanged();
				}
			}
		}
		#endregion
		public BubbleSeriesLabel()
			: base() {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "IndentFromMarker")
				return ShouldSerializeIndentFromMarker();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeIndentFromMarker() {
			return indentFromMarker != DefaultIndentFromMarker;
		}
		void ResetIndentFromMarker() {
			IndentFromMarker = DefaultIndentFromMarker;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeIndentFromMarker();
		}
		#endregion
		XYDiagramSeriesLabelLayout CalculateLayoutForCenter(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer, IXYWPoint refinedPoint, RefinedPointData pointData, Color color) {
			DiagramPoint anchorPoint = diagramMapping.GetScreenPointNoRound(refinedPoint.Argument, refinedPoint.Value);
			double size = refinedPoint.Size *
				MathUtils.CalcLength2D(diagramMapping.GetInterimPoint(1, 0, false, false), diagramMapping.GetInterimPoint(0, 0, false, false));
			SeriesLabelViewData labelViewData = pointData.LabelViewData[0];
			TextPainter textPainter = labelViewData.CreateTextPainterForCenterDrawing(this, textMeasurer, anchorPoint);
			return XYDiagramSeriesLabelLayout.CreateWithValidRectangle(pointData, color, textPainter, null,
				ResolveOverlappingMode, anchorPoint, SeriesLabelHelper.CalculateValidRectangleForCenterPosition(anchorPoint, size));
		}
		XYDiagramSeriesLabelLayout CalculateLayoutForOutside(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer, IXYWPoint refinedPoint, RefinedPointData pointData, Color color) {
			double angle = XYDiagramMappingHelper.CorrectAngle(diagramMapping, MathUtils.Degree2Radian(MathUtils.NormalizeDegree(Angle)));
			DiagramPoint startPoint = diagramMapping.GetScreenPointNoRound(refinedPoint.Argument, refinedPoint.Value);
			double lineLength = indentFromMarker + refinedPoint.Size / 2 *
				MathUtils.CalcLength2D(diagramMapping.GetInterimPoint(1, 0, false, false), diagramMapping.GetInterimPoint(0, 0, false, false));
			DiagramPoint finishPoint = SeriesLabelHelper.CalculateFinishPoint(angle, startPoint, lineLength);
			SeriesLabelViewData labelViewData = pointData.LabelViewData[0];
			TextPainter painter = labelViewData.CreateTextPainterForFlankDrawing(this, textMeasurer, finishPoint, angle);
			if (!diagramMapping.Bounds.IntersectsWith(Rectangle.Round(painter.BoundsWithBorder)))
				return null;
			return XYDiagramSeriesLabelLayout.CreateWithExcludedRectangle(pointData, color, painter,
				ActualLineVisible ? new LineConnectorPainter(startPoint, finishPoint, angle, (ZPlaneRectangle)painter.BoundsWithBorder, true) : null,
				ResolveOverlappingMode, startPoint, SeriesLabelHelper.CalcAnchorHoleForPoint(startPoint, LineLength));
		}
		internal override void UpdateTextPattern(ChartElement sender) {
			base.UpdateTextPattern(sender);
			if (PointOptionsInternal != null && (sender == PointOptionsInternal || sender.Owner == PointOptionsInternal) && TextPattern != null) {
				switch (valueToDisplay) {
					case BubbleLabelValueToDisplay.Value:
						break;
					case BubbleLabelValueToDisplay.Weight:
						TextPattern = PatternUtils.ReplacePlaceholder(TextPattern, PatternUtils.ValuePlaceholder, PatternUtils.WeightPlaceholder);
						break;
					case BubbleLabelValueToDisplay.ValueAndWeight:
						TextPattern = PatternUtils.ReplacePlaceholder(TextPattern, PatternUtils.ValuePlaceholder, PatternUtils.ValuePlaceholder, PatternUtils.WeightPlaceholder);
						break;
					default:
						throw new DefaultSwitchException();
				}
			}
		}
		protected internal override void CalculateLayout(SeriesLabelLayoutList labelLayoutList, RefinedPointData pointData, TextMeasurer textMeasurer) {
			XYDiagramSeriesLabelLayoutList xyLabelLayoutList = labelLayoutList as XYDiagramSeriesLabelLayoutList;
			IXYWPoint refinedPoint = pointData.RefinedPoint;
			if (xyLabelLayoutList == null)
				return;
			Color color = pointData.DrawOptions.Color;
			XYDiagramMappingBase mapping = xyLabelLayoutList.GetMapping(refinedPoint.Argument, refinedPoint.Value);
			if (mapping == null)
				return;
			XYDiagramSeriesLabelLayout layout = Position == PointLabelPosition.Outside ?
				CalculateLayoutForOutside(mapping, textMeasurer, refinedPoint, pointData, color) :
				CalculateLayoutForCenter(mapping, textMeasurer, refinedPoint, pointData, color);
			if (layout != null)
				xyLabelLayoutList.Add(layout);
		}
		protected override bool GetDefaultLineVisible(SeriesLabelAppearance labelAppearance) {
			SeriesLabel2DAppearance label2DAppearance = labelAppearance as SeriesLabel2DAppearance;
			return label2DAppearance != null ? label2DAppearance.ShowBubbleConnector : false;
		}
		protected override ChartElement CreateObjectForClone() {
			return new BubbleSeriesLabel();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			BubbleSeriesLabel label = obj as BubbleSeriesLabel;
			if (label != null) {
				indentFromMarker = label.indentFromMarker;
				valueToDisplay = label.valueToDisplay;
			}
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class BubbleValueToStringConveter : ValueToStringConverter {
		const string separator = ", ";
		BubbleLabelValueToDisplay valueToDisplay;
		public BubbleValueToStringConveter(INumericOptions numericOptions, IDateTimeOptions dateTimeOptions, BubbleLabelValueToDisplay valueToDisplay)
			: base(numericOptions, dateTimeOptions) {
			this.valueToDisplay = valueToDisplay;
		}
		public override string ConvertTo(object[] values) {
			switch (valueToDisplay) {
				case BubbleLabelValueToDisplay.Value:
					return GetValueText(values[0]);
				case BubbleLabelValueToDisplay.Weight:
					return GetValueText(values[1]);
				case BubbleLabelValueToDisplay.ValueAndWeight:
					return GetValueText(values[0]) + separator + GetValueText(values[1]);
				default:
					throw new DefaultSwitchException();
			}
		}
	}
}
