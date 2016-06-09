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
using System.Collections.Generic;
using System.Drawing.Design;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts {
	public abstract class Axis : Axis2D, IAutoScaleBreaksContainer {
		readonly ScaleBreakCollection scaleBreaks;
		readonly ScaleBreakOptions scaleBreakOptions;
		readonly AutoScaleBreaks autoScaleBreaks;
		bool reverse;														
		protected virtual bool DefaultReverse { get { return false; } }		
		protected override IEnumerable<IScaleBreak> ScaleBreaksEnumeration { get { return scaleBreaks; } }
		protected internal override bool ActualReverse { get { return reverse; } }
		protected internal override int IntervalsDistance { get { return scaleBreakOptions.SizeInPixels + 1; } }
		protected internal override ScaleBreakOptions ActualScaleBreakOptions { get { return scaleBreakOptions; } }
		internal XYDiagram XYDiagram { get { return (XYDiagram)Owner; } }
		internal IList<IScaleDiapason> ScaleDiapasons {
			get {
				List<IScaleDiapason> scaleDiapasons = new List<IScaleDiapason>();
				foreach (ScaleBreak scaleBreak in scaleBreaks)
					if (IsCompatibleWith(scaleBreak.ScaleBreakEdge1) && IsCompatibleWith(scaleBreak.ScaleBreakEdge2))
						scaleDiapasons.Add(scaleBreak);
				foreach (AutomaticScaleBreak scaleBreak in autoScaleBreaks.ScaleBreaks)
					scaleDiapasons.Add(scaleBreak);
				return scaleDiapasons;
			}
		}	 
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisReverse"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis.Reverse"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Reverse {
			get { return reverse; }
			set {
				if (value != reverse) {
					SendNotification(new ElementWillChangeNotification(this));
					reverse = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisScaleBreaks"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis.ScaleBreaks"),
		TypeConverter(typeof(CollectionTypeConverter)),
		Category(Categories.Elements),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraCharts.Design.ScaleBreakCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public ScaleBreakCollection ScaleBreaks { get { return scaleBreaks; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisScaleBreakOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis.ScaleBreakOptions"),
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public ScaleBreakOptions ScaleBreakOptions { get { return scaleBreakOptions; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AxisAutoScaleBreaks"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.Axis.AutoScaleBreaks"),
		Category(Categories.Behavior),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public AutoScaleBreaks AutoScaleBreaks { get { return autoScaleBreaks; } }
		protected Axis(string name, XYDiagram diagram) : base(name, diagram) {
			reverse = DefaultReverse;						
			scaleBreaks = new ScaleBreakCollection(this);
			scaleBreakOptions = new ScaleBreakOptions(this);
			autoScaleBreaks = new AutoScaleBreaks(this);
		}
		#region IAutoScaleBreaksContainer
		bool IAutoScaleBreaksContainer.Enabled { get { return autoScaleBreaks.Enabled; } }
		void IAutoScaleBreaksContainer.UpdateScaleBreaks(IList<IRefinedSeries> refinedSeries) {
			List<double> initialValues = GetInitialValuesForAutomaticScaleBreakCalculation(refinedSeries);
			autoScaleBreaks.Calculate(initialValues, ((IAxisData)this).AxisScaleTypeMap.Transformation);
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch(propertyName) {
				case "Reverse":
					return ShouldSerializeReverse();
				case "ScaleBreakOptions":
					return ShouldSerializeScaleBreakOptions();
				case "AutoScaleBreaks":
					return ShouldSerializeAutoScaleBreaks();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		protected override void XtraSetIndexCollectionItem(string propertyName, object item) {
			if(propertyName == "ScaleBreaks")
				scaleBreaks.Add((ScaleBreak)item);
			else
				base.XtraSetIndexCollectionItem(propertyName, item);
		}
		protected override object XtraCreateCollectionItem(string propertyName) {
			if(propertyName == "ScaleBreaks")
				return new ScaleBreak();
			return base.XtraCreateCollectionItem(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeReverse() {
			return reverse != DefaultReverse;
		}
		void ResetReverse() {
			Reverse = DefaultReverse;
		}
		bool ShouldSerializeScaleBreakOptions() {
			return scaleBreakOptions.ShouldSerialize();
		}
		bool ShouldSerializeAutoScaleBreaks() {
			return autoScaleBreaks.ShouldSerialize();
		}
		#endregion
		protected abstract IEnumerable<double> GetInitialValuesForAutomaticScaleBreakCalculation(ISeries series, RefinedPoint refinedPoint);
		protected override IList<AxisInterval> CreateIntervals(double min, double max) {
			return new AxisIntervalsGenerator(this, min, max).GenerateIntervals();
		}
		protected override IList<IMinMaxValues> CreateIntervalLimits(double min, double max) {
			return new AxisIntervalsGenerator(this, min, max).GenerateIntervalLimits();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				scaleBreaks.Dispose();
				scaleBreakOptions.Dispose();
				autoScaleBreaks.Dispose();
			}
			base.Dispose(disposing);
		}
		internal List<double> GetInitialValuesForAutomaticScaleBreakCalculation(IList<IRefinedSeries> refinedSeries) {
			List<double> initialValues = new List<double>();
			foreach (IRefinedSeries series in refinedSeries) 
				foreach(RefinedPoint point in series.Points)
					initialValues.AddRange(GetInitialValuesForAutomaticScaleBreakCalculation(series.Series, point));
			if (((IAxisData)this).AxisScaleTypeMap.ScaleType == ActualScaleType.DateTime) {
				initialValues.Add(((IAxisData)this).VisualRange.Min);
			} else
				initialValues.Add(0);
			return initialValues;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Axis axis = obj as Axis;
			if (axis != null) {
				reverse = axis.reverse;																
				scaleBreaks.Assign(axis.scaleBreaks);
				scaleBreakOptions.Assign(axis.scaleBreakOptions);
				autoScaleBreaks.Assign(axis.autoScaleBreaks);
			}
		}
	}
}
