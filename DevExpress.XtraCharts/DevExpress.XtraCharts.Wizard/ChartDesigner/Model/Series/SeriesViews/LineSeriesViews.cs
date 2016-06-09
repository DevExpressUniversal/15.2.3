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

using DevExpress.Utils;
using DevExpress.XtraCharts.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.XtraCharts.Design;
using DevExpress.Utils.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	[ModelOf(typeof(LineSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(LineSeriesViewTypeConverter))]
	public class LineViewModel : PointViewBaseModel {
		LineStyleModel lineStyleModel;
		MarkerModel markerOptionsModel;
		protected new LineSeriesView SeriesView { get { return (LineSeriesView)base.SeriesView; } }
		[PropertyForOptions("Marker Options"), TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean MarkerVisibility {
			get { return SeriesView.MarkerVisibility; }
			set { SetProperty("MarkerVisibility", value); }
		}
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean EnableAntialiasing {
			get { return SeriesView.EnableAntialiasing; }
			set { SetProperty("EnableAntialiasing", value); }
		}
		[PropertyForOptions(-1, "Appearance")]
		public new Color Color {
			get { return base.Color; }
			set { base.Color = value; }
		}
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public LineStyleModel LineStyle { get { return lineStyleModel; } }
		[PropertyForOptions,
		AllocateToGroup("Marker Options")]
		public MarkerModel LineMarkerOptions { get { return markerOptionsModel; } }
		public LineViewModel(LineSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if(lineStyleModel != null)
				Children.Add(lineStyleModel);
			if(markerOptionsModel != null)
				Children.Add(markerOptionsModel);
			base.AddChildren();
		}
		public override void Update() {
			this.lineStyleModel = new LineStyleModel(SeriesView.LineStyle, CommandManager);
			this.markerOptionsModel = new MarkerModel(SeriesView.LineMarkerOptions, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(StackedLineSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(StackedLineSeriesViewTypeConverter))]
	public class StackedLineViewModel : LineViewModel {
		protected new StackedLineSeriesView SeriesView { get { return (StackedLineSeriesView)base.SeriesView; } }
		public StackedLineViewModel(LineSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(FullStackedLineSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(FullStackedLineSeriesViewTypeConverter))]
	public class FullStackedLineViewModel : StackedLineViewModel {
		protected new FullStackedLineSeriesView SeriesView { get { return (FullStackedLineSeriesView)base.SeriesView; } }
		public FullStackedLineViewModel(FullStackedLineSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(StepLineSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(StepLineSeriesViewTypeConverter))]
	public class StepLineViewModel : LineViewModel {
		protected new StepLineSeriesView SeriesView { get { return (StepLineSeriesView)base.SeriesView; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool InvertedStep {
			get { return SeriesView.InvertedStep; }
			set { SetProperty("InvertedStep", value); }
		}
		public StepLineViewModel(StepLineSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(SplineSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(SplineSeriesViewTypeConverter))]
	public class SplineViewModel : LineViewModel {
		protected new SplineSeriesView SeriesView { get { return (SplineSeriesView)base.SeriesView; } }
		[PropertyForOptions]
		public int LineTensionPercent {
			get { return SeriesView.LineTensionPercent; }
			set { SetProperty("LineTensionPercent", value); }
		}
		public SplineViewModel(SplineSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(ScatterLineSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(ScatterLineSeriesViewTypeConverter))]
	public class ScatterLineViewModel : LineViewModel {
		protected new ScatterLineSeriesView SeriesView { get { return (ScatterLineSeriesView)base.SeriesView; } }
		public ScatterLineViewModel(ScatterLineSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(SwiftPlotSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(SwiftPlotSeriesViewTypeConverter))]
	public class SwiftPlotViewModel : SwiftPlotViewBaseModel {
		LineStyleModel lineStyleModel;
		protected new SwiftPlotSeriesView SeriesView { get { return (SwiftPlotSeriesView)base.SeriesView; } }
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool Antialiasing {
			get { return SeriesView.Antialiasing; }
			set { SetProperty("Antialiasing", value); }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("Line Style")]
		public LineStyleModel LineStyle { get { return lineStyleModel; } }
		public SwiftPlotViewModel(SwiftPlotSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if(lineStyleModel != null)
				Children.Add(lineStyleModel);
			base.AddChildren();
		}
		public override void Update() {
			this.lineStyleModel = new LineStyleModel(SeriesView.LineStyle, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(Line3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(Line3DSeriesViewTypeConverter))]
	public class Line3DViewModel : XYDiagram3DViewBaseModel {
		protected new Line3DSeriesView SeriesView { get { return (Line3DSeriesView)base.SeriesView; } }
		[PropertyForOptions]
		public int LineThickness {
			get { return SeriesView.LineThickness; }
			set { SetProperty("LineThickness", value); }
		}
		public double LineWidth {
			get { return SeriesView.LineWidth; }
			set { SetProperty("LineWidth", value); }
		}
		public Line3DViewModel(Line3DSeriesView seriesView, CommandManager commandManager) : base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(StackedLine3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(StackedLine3DSeriesViewTypeConverter))]
	public class StackedLine3DViewModel : Line3DViewModel {
		protected new StackedLine3DSeriesView SeriesView { get { return (StackedLine3DSeriesView)base.SeriesView; } }
		public StackedLine3DViewModel(StackedLine3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(FullStackedLine3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(FullStackedLine3DSeriesViewTypeConverter))]
	public class FullStackedLine3DViewModel : StackedLine3DViewModel {
		protected new FullStackedLine3DSeriesView SeriesView { get { return (FullStackedLine3DSeriesView)base.SeriesView; } }
		public FullStackedLine3DViewModel(FullStackedLine3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(StepLine3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(StepLine3DSeriesViewTypeConverter))]
	public class StepLine3DViewModel : Line3DViewModel {
		protected new StepLine3DSeriesView SeriesView { get { return (StepLine3DSeriesView)base.SeriesView; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool InvertedStep {
			get { return SeriesView.InvertedStep; }
			set { SetProperty("InvertedStep", value); }
		}
		public StepLine3DViewModel(StepLine3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(Spline3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(Spline3DSeriesViewTypeConverter))]
	public class Spline3DViewModel : Line3DViewModel {
		protected new Spline3DSeriesView SeriesView { get { return (Spline3DSeriesView)base.SeriesView; } }
		[PropertyForOptions]
		public int LineTensionPercent {
			get { return SeriesView.LineTensionPercent; }
			set { SetProperty("LineTensionPercent", value); }
		}
		public Spline3DViewModel(Spline3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
}
