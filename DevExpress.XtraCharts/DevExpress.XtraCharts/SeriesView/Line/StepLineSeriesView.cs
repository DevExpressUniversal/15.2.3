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

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(StepLineSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class StepLineSeriesView : LineSeriesView, ILineSeriesView, IStepSeriesView {
		const bool DefaultInvertedStep = false;
		bool invertedStep = DefaultInvertedStep;
		protected internal override bool DefaultLineAntialiasing { get { return false; } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnStepLine); } }		
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("StepLineSeriesViewInvertedStep"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.StepLineSeriesView.InvertedStep"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool InvertedStep {
			get { return invertedStep; }
			set {
				if (invertedStep != value) {
					SendNotification(new ElementWillChangeNotification(this));
					invertedStep = value;
					RaiseControlChanged();
				}
			}
		}
		public StepLineSeriesView() : base() {
		}
		#region ILineSeriesView Members
		LineStyle ILineSeriesView.LineStyle {
			get { return LineStyle; }
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "InvertedStep" ? ShouldSerializeInvertedStep() : base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeInvertedStep() {
			return invertedStep != DefaultInvertedStep;
		}
		void ResetInvertedStep() {
			InvertedStep = DefaultInvertedStep;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeInvertedStep();
		}
		#endregion
		protected override void AddStripElementInternal(IGeometryStrip strip, RefinedPoint pointInfo) {
			((LineStrip)strip).AddStepToPoint(new GRealPoint2D(pointInfo.Argument, ((IValuePoint)pointInfo).Value), InvertedStep);
		}
		protected override GeometryStripCreator CreateStripCreator() {
			return new StepLineGeometryStripCreator(InvertedStep);
		}
		protected override PointSeriesViewPainter CreatePainter() {
			return new StepLineSeriesViewPainter(this);
		}
		protected override ChartElement CreateObjectForClone() {
			return new StepLineSeriesView();
		}
		protected internal override bool GetActualAntialiasing(int pointsCount) {
			return DefaultBooleanUtils.ToBoolean(EnableAntialiasing, false);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			IStepSeriesView view = obj as IStepSeriesView;
			if (view != null)
				invertedStep = view.InvertedStep;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			return invertedStep == ((StepLineSeriesView)obj).invertedStep;
		}
	}
}
