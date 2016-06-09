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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.Utils.Design;
using System.Drawing;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)
	]
	public enum PointLabelPosition {
		Center = 0,
		Outside = 1
	}
	[
	TypeConverter(typeof(PointSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class PointSeriesLabel : SeriesLabelBase {
		int angleDegree;
		PointLabelPosition position;
		protected virtual int DefaultAngleDegree { get { return 45; } }
		protected virtual PointLabelPosition DefaultPosition { get { return PointLabelPosition.Outside; } }
		protected internal override bool ShadowSupported { get { return true; } }
		protected internal override bool ConnectorSupported { get { return true; } }
		protected internal override bool ConnectorEnabled { get { return position == PointLabelPosition.Outside; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PointSeriesLabelAngle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PointSeriesLabel.Angle"),
		Category(Categories.Behavior),
		Localizable(true),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public int Angle {
			get { return angleDegree; }
			set {
				if (value != angleDegree) {
					if (value < -360 || value > 360)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectLabelAngle));
					SendNotification(new ElementWillChangeNotification(this));
					angleDegree = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PointSeriesLabelPosition"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PointSeriesLabel.Position"),
		Category(Categories.Behavior),
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public PointLabelPosition Position {
			get { return position; }
			set {
				if (value != position) {
					SendNotification(new ElementWillChangeNotification(this));
					position = value;
					SynchronizeOverlappingMode();
					RaiseControlChanged();
				}
			}
		}
		public PointSeriesLabel()
			: base() {
			angleDegree = DefaultAngleDegree;
			position = DefaultPosition;
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Angle")
				return ShouldSerializeAngle();
			if (propertyName == "Position")
				return ShouldSerializePosition();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeAngle() {
			return angleDegree != DefaultAngleDegree;
		}
		void ResetAngle() {
			Angle = DefaultAngleDegree;
		}
		protected bool ShouldSerializePosition() {
			return position != DefaultPosition;
		}
		void ResetPosition() {
			Position = DefaultPosition;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeAngle() ||
				ShouldSerializePosition();
		}
		#endregion
		XYDiagramSeriesLabelLayout CalculateLayoutForCenter(XYDiagramSeriesLabelLayoutList xyLabelLayoutList, TextMeasurer textMeasurer, RefinedPointData pointData, SeriesLabelViewData labelViewData, double pointValue, int markerSize) {
			RefinedPoint refinedPoint = pointData.RefinedPoint;
			XYDiagramMappingBase mapping = xyLabelLayoutList.GetMapping(refinedPoint.Argument, pointValue);
			if (mapping == null)
				return null;
			DiagramPoint anchorPoint = mapping.GetScreenPoint(refinedPoint.Argument, pointValue);
			TextPainter painter = labelViewData.CreateTextPainterForCenterDrawing(this, textMeasurer, anchorPoint);
			RectangleF validRectangle = SeriesLabelHelper.CalculateValidRectangleForCenterPosition(anchorPoint, markerSize);
			return XYDiagramSeriesLabelLayout.CreateWithValidRectangle(pointData, pointData.DrawOptions.Color, painter, null, ResolveOverlappingMode, anchorPoint, validRectangle);
		}
		void SynchronizeOverlappingMode() {
			if (Position == PointLabelPosition.Center) {
				if (ResolveOverlappingMode == ResolveOverlappingMode.JustifyAllAroundPoint || ResolveOverlappingMode == ResolveOverlappingMode.JustifyAroundPoint)
					ResolveOverlappingMode = ResolveOverlappingMode.Default;
			}
			else if (ResolveOverlappingMode == ResolveOverlappingMode.Default)
				ResolveOverlappingMode = ResolveOverlappingMode.JustifyAroundPoint;
		}
		protected internal override void CalculateLayout(SeriesLabelLayoutList labelLayoutList, RefinedPointData pointData, TextMeasurer textMeasurer) {
			XYDiagramSeriesLabelLayoutList xyLabelLayoutList = labelLayoutList as XYDiagramSeriesLabelLayoutList;
			if (xyLabelLayoutList == null) {
				ChartDebug.Fail("XYDiagramSeriesLabelsLayout expected.");
				return;
			}
			PointSeriesView view = Series.View as PointSeriesView;
			if (view == null) {
				ChartDebug.Fail("PointSeriesView expected.");
				return;
			}
			MinMaxValues values = view.GetSeriesPointValues(pointData.RefinedPoint);
			SeriesLabelViewData labelViewData = pointData.LabelViewData[0];
			XYDiagramSeriesLabelLayout layout = Position == PointLabelPosition.Outside ?
				SeriesLabelHelper.CalculateLayoutForPoint(xyLabelLayoutList, textMeasurer, pointData, labelViewData, this, values.Max, angleDegree) :
				CalculateLayoutForCenter(xyLabelLayoutList, textMeasurer, pointData, labelViewData, values.Max, view.PointMarkerOptions.Size);
			if (layout != null)
				xyLabelLayoutList.Add(layout);
		}
		protected override ChartElement CreateObjectForClone() {
			return new PointSeriesLabel();
		}
		protected internal override bool CheckResolveOverlappingMode(ResolveOverlappingMode mode) {
			if (Position == PointLabelPosition.Outside)
				return true;
			return mode == ResolveOverlappingMode.None || mode == ResolveOverlappingMode.Default || mode == ResolveOverlappingMode.HideOverlapped;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			PointSeriesLabel label = obj as PointSeriesLabel;
			if (label == null)
				return;
			this.angleDegree = label.angleDegree;
			this.position = label.position;
		}
	}
}
