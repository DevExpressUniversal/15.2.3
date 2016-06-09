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

using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Native;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(StepArea3DSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class StepArea3DSeriesView : Area3DSeriesView, IStepSeriesView {
		const bool DefaultInvertedStep = false;
		bool invertedStep = DefaultInvertedStep;
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnStepArea3D); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("StepArea3DSeriesViewInvertedStep"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.StepAreaSeriesView.InvertedStep"),
		TypeConverter(typeof(BooleanTypeConverter)),
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
		public StepArea3DSeriesView() : base() { 
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
			return new StepAreaGeometryStripCreator(InvertedStep);
		}
		protected override IGeometryStrip CreateStripInternal() {
			return new RangeStrip();
		}
		protected override void RenderLegendMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			Line3DDrawOptions lineDrawOptions = seriesPointDrawOptions as Line3DDrawOptions;
			if (lineDrawOptions == null)
				return;
			RangeStrip markerStrip = StepAreaSeriesViewPainter.CreateStepAreaMarkerStrip(this, bounds, invertedStep);
			renderer.EnablePolygonAntialiasing(true);
			StripsUtils.Render(renderer, markerStrip, LegendFillStyle.Options, lineDrawOptions.Color, lineDrawOptions.ActualColor2, new SeriesHitTestState(), null, selectionState);
			renderer.RestorePolygonAntialiasing();
		}
		protected override ChartElement CreateObjectForClone() {
			return new StepArea3DSeriesView();
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
			return invertedStep == ((StepArea3DSeriesView)obj).invertedStep;
		}
	}
}
