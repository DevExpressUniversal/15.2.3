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
using System.Drawing;
using System.Reflection;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(RelativePositionTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RelativePosition : AnnotationShapePosition {
		const double DefaultAngle = 45;
		const double DefaultConnectorLength = 40;
		double angle;
		double connectorLength;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RelativePositionConnectorLength"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RelativePosition.ConnectorLength"),
		XtraSerializableProperty
		]
		public double ConnectorLength {
			get { return connectorLength; }
			set {
				if (value < 0 || value >= 1000)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectRelativePositionConnectorLength));
				if (value != connectorLength) {
					SendNotification(new ElementWillChangeNotification(this));
					connectorLength = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RelativePositionAngle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RelativePosition.Angle"),
		XtraSerializableProperty
		]
		public double Angle {
			get { return angle; }
			set {
				if (value != angle) {
					if (value < -360 || value > 360)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectRelativePositionAngle));
					SendNotification(new ElementWillChangeNotification(this));
					angle = value;
					RaiseControlChanged();
				}
			}
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Angle")
				return ShouldSerializeAngle();
			if (propertyName == "ConnectorLength")
				return ShouldSerializeConnectorLength();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeAngle() {
			return angle != DefaultAngle;
		}
		void ResetAngle() {
			Angle = DefaultAngle;
		}
		bool ShouldSerializeConnectorLength() {
			return connectorLength != DefaultConnectorLength;
		}
		void ResetConnectorLength() {
			ConnectorLength = DefaultConnectorLength;
		}
		protected internal override bool ShouldSerialize() {
			return true;
		}
		#endregion
		public RelativePosition() : this(DefaultAngle, DefaultConnectorLength) {			
		}
		public RelativePosition(double angle, double connectorLength) {
			this.angle = angle;
			this.connectorLength = connectorLength;
		}
		double GetCorrectedAngleInRadian() {
			return MathUtils.Degree2Radian(MathUtils.NormalizeDegree(-Angle));
		}
		protected override ChartElement CreateObjectForClone() {
			return new RelativePosition();
		}
		protected internal override void OnEndLoading() {			
		}
		internal DiagramPoint GetShapeLocation(DiagramPoint anchorPoint) {
			double currentAngle = GetCorrectedAngleInRadian();
			Annotation.UpdateSize();
			DiagramPoint location = new DiagramPoint(anchorPoint.X + connectorLength * Math.Cos(currentAngle) - Annotation.Width / 2.0,
				anchorPoint.Y + connectorLength * Math.Sin(currentAngle) - Annotation.Height / 2.0);
			return MathUtils.StrongRoundXY(location);
		}
		internal void SetAngle(double value) {
			angle = value;
		}
		internal void SetConnectorLength(double value) {
			connectorLength = value;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			RelativePosition position = obj as RelativePosition;
			if (position == null)
				return;
			angle = position.angle;
			connectorLength = position.connectorLength;
		}
	}
}
