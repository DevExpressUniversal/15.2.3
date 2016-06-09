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
using System.Linq;
using System.Text;
using DevExpress.XtraCharts.Design;
using DevExpress.Utils.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	public abstract class RadarViewBaseModel : SeriesViewModelBase {
		ShadowModel shadowModel;
		protected new RadarSeriesViewBase SeriesView { get { return (RadarSeriesViewBase)base.SeriesView; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool ColorEach {
			get { return SeriesView.ColorEach; }
			set { SetProperty("ColorEach", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public ShadowModel Shadow { get { return shadowModel; } }
		public RadarViewBaseModel(RadarSeriesViewBase seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if(shadowModel != null)
				Children.Add(shadowModel);
			base.AddChildren();
		}
		public override void Update() {
			this.shadowModel = new ShadowModel(SeriesView.Shadow, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(RadarPointSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(RadarPointSeriesViewTypeConverter))]
	public class RadarPointViewModel : RadarViewBaseModel {
		SimpleMarkerModel pointMarkerOptionsModel;
		protected new RadarPointSeriesView SeriesView { get { return (RadarPointSeriesView)base.SeriesView; } }
		[PropertyForOptions,
		AllocateToGroup("Marker Options")]
		public SimpleMarkerModel PointMarkerOptions { get { return pointMarkerOptionsModel; } }
		public RadarPointViewModel(RadarPointSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if(pointMarkerOptionsModel != null)
				Children.Add(pointMarkerOptionsModel);
			base.AddChildren();
		}
		public override void Update() {
			this.pointMarkerOptionsModel = new SimpleMarkerModel(SeriesView.PointMarkerOptions, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(RadarLineSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(RadarLineSeriesViewTypeConverter))]
	public class RadarLineViewModel : RadarPointViewModel {
		LineStyleModel lineStyleModel;
		MarkerModel markerModel;
		protected new RadarLineSeriesView SeriesView { get { return (RadarLineSeriesView)base.SeriesView; } }
		[Browsable(false)]
		public new SimpleMarkerModel PointMarkerOptions { get; set; }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool Closed {
			get { return SeriesView.Closed; }
			set { SetProperty("Closed", value); }
		}
		[PropertyForOptions("Marker Options"), TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean MarkerVisibility {
			get { return SeriesView.MarkerVisibility; }
			set { SetProperty("MarkerVisibility", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public LineStyleModel LineStyle { get { return lineStyleModel; } }
		[PropertyForOptions,
		AllocateToGroup("Marker Options")]
		public MarkerModel LineMarkerOptions { get { return markerModel; } }
		public RadarLineViewModel(RadarLineSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if(lineStyleModel != null)
				Children.Add(lineStyleModel);
			if(markerModel != null)
				Children.Add(markerModel);
			base.AddChildren();
		}
		public override void Update() {
			this.lineStyleModel = new LineStyleModel(SeriesView.LineStyle, CommandManager);
			this.markerModel = new MarkerModel(SeriesView.LineMarkerOptions, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(ScatterRadarLineSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(ScatterRadarLineSeriesViewTypeConverter))]
	public class ScatterRadarLineViewModel : RadarLineViewModel {
		protected new ScatterRadarLineSeriesView SeriesView { get { return (ScatterRadarLineSeriesView)base.SeriesView; } }
		public ScatterRadarLineViewModel(ScatterRadarLineSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(RadarAreaSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(RadarAreaSeriesViewTypeConverter))]
	public class RadarAreaViewModel : RadarLineViewModel {
		CustomBorderModel borderModel;
		PolygonFillStyleModel fillStyleModel;
		MarkerModel markerOptionsModel;
		protected new RadarAreaSeriesView SeriesView { get { return (RadarAreaSeriesView)base.SeriesView; } }
		[Browsable(false)]
		public new bool Closed { get; set; }
		[Browsable(false)]
		public new LineStyleModel LineStyle { get; set; }
		[Browsable(false)]
		public new MarkerModel LineMarkerOptions { get; set; }
		[PropertyForOptions("Appearance")]
		public byte Transparency {
			get { return SeriesView.Transparency; }
			set { SetProperty("Transparency", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public CustomBorderModel Border { get { return borderModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public PolygonFillStyleModel FillStyle { get { return fillStyleModel; } }
		[PropertyForOptions("Marker Options"), TypeConverter(typeof(DefaultBooleanConverter))]
		public new DefaultBoolean MarkerVisibility {
			get { return SeriesView.MarkerVisibility; }
			set { SetProperty("MarkerVisibility", value); }
		}
		[PropertyForOptions,
		AllocateToGroup("Marker Options")]
		public MarkerModel MarkerOptions { get { return markerOptionsModel; } }
		public RadarAreaViewModel(RadarAreaSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if(borderModel != null)
				Children.Add(borderModel);
			if(fillStyleModel != null)
				Children.Add(fillStyleModel);
			if(markerOptionsModel != null)
				Children.Add(markerOptionsModel);
			base.AddChildren();
		}
		public override void Update() {
			this.borderModel = new CustomBorderModel(SeriesView.Border, CommandManager);
			this.fillStyleModel = new PolygonFillStyleModel(SeriesView.FillStyle, CommandManager);
			this.markerOptionsModel = new MarkerModel(SeriesView.MarkerOptions, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(PolarPointSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(PolarPointSeriesViewTypeConverter))]
	public class PolarPointViewModel : RadarPointViewModel {
		protected new PolarPointSeriesView SeriesView { get { return (PolarPointSeriesView)base.SeriesView; } }
		public PolarPointViewModel(PolarPointSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(PolarLineSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(PolarLineSeriesViewTypeConverter))]
	public class PolarLineViewModel : RadarLineViewModel {
		protected new PolarLineSeriesView SeriesView { get { return (PolarLineSeriesView)base.SeriesView; } }
		public PolarLineViewModel(PolarLineSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(ScatterPolarLineSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(ScatterPolarLineSeriesViewTypeConverter))]
	public class ScatterPolarLineViewModel : PolarLineViewModel {
		protected new ScatterPolarLineSeriesView SeriesView { get { return (ScatterPolarLineSeriesView)base.SeriesView; } }
		public ScatterPolarLineViewModel(ScatterPolarLineSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(PolarAreaSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(PolarAreaSeriesViewTypeConverter))]
	public class PolarAreaViewModel : RadarAreaViewModel {
		protected new PolarAreaSeriesView SeriesView { get { return (PolarAreaSeriesView)base.SeriesView; } }
		public PolarAreaViewModel(PolarAreaSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
}
