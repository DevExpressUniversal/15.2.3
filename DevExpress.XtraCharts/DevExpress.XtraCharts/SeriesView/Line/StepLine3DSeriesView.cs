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
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(StepLine3DSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class StepLine3DSeriesView : Line3DSeriesView, IStepSeriesView {
		const bool DefaultInvertedStep = false;
		bool invertedStep = DefaultInvertedStep;
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnStepLine3D); } }
		protected internal override bool InterlacedPoints { get { return true; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("StepLine3DSeriesViewInvertedStep"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.StepLine3DSeriesView.InvertedStep"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool InvertedStep {
			get { return invertedStep; }
			set {
				if (value != invertedStep) {
					SendNotification(new ElementWillChangeNotification(this));
					invertedStep = value;
					RaiseControlChanged();
				}
			}
		}
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
		protected override GeometryStripCreator CreateStripCreator() {
			return new StepLineGeometryStripCreator(InvertedStep);
		}
		protected override void RenderLegendMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			Line3DDrawOptions drawOptions = seriesPointDrawOptions as Line3DDrawOptions;
			if (drawOptions == null)
				return;
			LineStrip points = StripsUtils.CalcLegendStepLinePoints(bounds, invertedStep);
			LineStyle lineStyle = LegendLineStyle;
			renderer.EnableAntialiasing(lineStyle.AntiAlias);
			renderer.DrawLines(points, drawOptions.Color, 1, lineStyle, LineCap.Round);
			renderer.RestoreAntialiasing();
		}
		protected override ChartElement CreateObjectForClone() {
			return new StepLine3DSeriesView();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			IStepSeriesView view = obj as IStepSeriesView;
			if (view != null)
				invertedStep = view.InvertedStep;
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			return invertedStep == ((StepLine3DSeriesView)obj).invertedStep;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
