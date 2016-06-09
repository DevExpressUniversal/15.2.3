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
	[GroupPrefix("View: ")]
	public abstract class AreaViewBaseModel : LineViewModel {
		CustomBorderModel borderModel;
		PolygonFillStyleModel fillStyleModel;
		MarkerModel markerOptionsModel;
		protected new AreaSeriesViewBase SeriesView { get { return (AreaSeriesViewBase)base.SeriesView; } }
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
		public AreaViewBaseModel(AreaSeriesViewBase seriesView, CommandManager commandManager)
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
	[ModelOf(typeof(AreaSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(AreaSeriesViewTypeConverter))]
	public class AreaViewModel : AreaViewBaseModel {
		protected new AreaSeriesView SeriesView { get { return (AreaSeriesView)base.SeriesView; } }
		public AreaViewModel(AreaSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(StackedAreaSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(StackedAreaSeriesViewTypeConverter))]
	public class StackedAreaViewModel : AreaViewModel {
		protected new StackedAreaSeriesView SeriesView { get { return (StackedAreaSeriesView)base.SeriesView; } }
		[Browsable(false)]
		public new MarkerModel MarkerOptions { get; set; }
		[Browsable(false)]
		protected new DefaultBoolean MarkerVisibility { get; set; }
		[Browsable(false)]
		public new bool ColorEach { get; set; }
		public StackedAreaViewModel(StackedAreaSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(FullStackedAreaSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(FullStackedAreaSeriesViewTypeConverter))]
	public class FullStackedAreaViewModel : StackedAreaViewModel {
		protected new FullStackedAreaSeriesView SeriesView { get { return (FullStackedAreaSeriesView)base.SeriesView; } }
		[Browsable(false)]
		public new CustomBorderModel Border { get; set; }
		public FullStackedAreaViewModel(FullStackedAreaSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(StepAreaSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(StepAreaSeriesViewTypeConverter))]
	public class StepAreaViewModel : AreaViewModel {
		protected new StepAreaSeriesView SeriesView { get { return (StepAreaSeriesView)base.SeriesView; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool InvertedStep {
			get { return SeriesView.InvertedStep; }
			set { SetProperty("InvertedStep", value); }
		}
		public StepAreaViewModel(StepAreaSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(SplineAreaSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(SplineAreaSeriesViewTypeConverter))]
	public class SplineAreaViewModel : AreaViewModel {
		protected new SplineAreaSeriesView SeriesView { get { return (SplineAreaSeriesView)base.SeriesView; } }
		[PropertyForOptions]
		public int LineTensionPercent {
			get { return SeriesView.LineTensionPercent; }
			set { SetProperty("LineTensionPercent", value); }
		}
		public SplineAreaViewModel(SplineAreaSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(StackedSplineAreaSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(SplineStackedAreaSeriesViewTypeConverter))]
	public class StackedSplineAreaViewModel : StackedAreaViewModel {
		protected new StackedSplineAreaSeriesView SeriesView { get { return (StackedSplineAreaSeriesView)base.SeriesView; } }
		[PropertyForOptions]
		public int LineTensionPercent {
			get { return SeriesView.LineTensionPercent; }
			set { SetProperty("LineTensionPercent", value); }
		}
		public StackedSplineAreaViewModel(StackedSplineAreaSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(FullStackedSplineAreaSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(SplineFullStackedAreaSeriesViewTypeConverter))]
	public class FullStackedSplineAreaViewModel : FullStackedAreaViewModel {
		protected new FullStackedSplineAreaSeriesView SeriesView { get { return (FullStackedSplineAreaSeriesView)base.SeriesView; } }
		[PropertyForOptions]
		public int LineTensionPercent {
			get { return SeriesView.LineTensionPercent; }
			set { SetProperty("LineTensionPercent", value); }
		}
		public FullStackedSplineAreaViewModel(FullStackedSplineAreaSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(Area3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(Area3DSeriesViewTypeConverter))]
	public class Area3DViewModel : Line3DViewModel {
		protected new Area3DSeriesView SeriesView { get { return (Area3DSeriesView)base.SeriesView; } }
		[Browsable(false)]
		public new int LineThickness { get; set; }
		[Browsable(false)]
		public new double LineWidth { get; set; }
		[PropertyForOptions]
		public double AreaWidth {
			get { return SeriesView.AreaWidth; }
			set { SetProperty("AreaWidth", value); }
		}
		public Area3DViewModel(Area3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(StackedArea3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(StackedArea3DSeriesViewTypeConverter))]
	public class StackedArea3DViewModel : Area3DViewModel {
		protected new StackedArea3DSeriesView SeriesView { get { return (StackedArea3DSeriesView)base.SeriesView; } }
		public StackedArea3DViewModel(StackedArea3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(FullStackedArea3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(FullStackedArea3DSeriesViewTypeConverter))]
	public class FullStackedArea3DViewModel : StackedArea3DViewModel {
		protected new FullStackedArea3DSeriesView SeriesView { get { return (FullStackedArea3DSeriesView)base.SeriesView; } }
		public FullStackedArea3DViewModel(FullStackedArea3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(StepArea3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(StepArea3DSeriesViewTypeConverter))]
	public class StepArea3DViewModel : Area3DViewModel {
		protected new StepArea3DSeriesView SeriesView { get { return (StepArea3DSeriesView)base.SeriesView; } }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool InvertedStep {
			get { return SeriesView.InvertedStep; }
			set { SetProperty("InvertedStep", value); }
		}
		public StepArea3DViewModel(StepArea3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(SplineArea3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(SplineArea3DSeriesViewTypeConverter))]
	public class SplineArea3DViewModel : Area3DViewModel {
		protected new SplineArea3DSeriesView SeriesView { get { return (SplineArea3DSeriesView)base.SeriesView; } }
		[PropertyForOptions]
		public int LineTensionPercent {
			get { return SeriesView.LineTensionPercent; }
			set { SetProperty("LineTensionPercent", value); }
		}
		public SplineArea3DViewModel(SplineArea3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(StackedSplineArea3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(SplineAreaStacked3DSeriesViewTypeConverter))]
	public class StackedSplineArea3DViewModel : StackedArea3DViewModel {
		protected new StackedSplineArea3DSeriesView SeriesView { get { return (StackedSplineArea3DSeriesView)base.SeriesView; } }
		[PropertyForOptions]
		public int LineTensionPercent {
			get { return SeriesView.LineTensionPercent; }
			set { SetProperty("LineTensionPercent", value); }
		}
		public StackedSplineArea3DViewModel(StackedSplineArea3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(FullStackedSplineArea3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(SplineAreaFullStacked3DSeriesViewTypeConverter))]
	public class FullStackedSplineArea3DViewModel : FullStackedArea3DViewModel {
		protected new FullStackedSplineArea3DSeriesView SeriesView { get { return (FullStackedSplineArea3DSeriesView)base.SeriesView; } }
		[PropertyForOptions]
		public int LineTensionPercent {
			get { return SeriesView.LineTensionPercent; }
			set { SetProperty("LineTensionPercent", value); }
		}
		public FullStackedSplineArea3DViewModel(FullStackedSplineArea3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(RangeAreaSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(RangeAreaSeriesViewTypeConverter))]
	public class RangeAreaViewModel : AreaViewModel {
		MarkerModel marker1Model;
		MarkerModel marker2Model;
		CustomBorderModel border1Model;
		CustomBorderModel border2Model;
		protected new RangeAreaSeriesView SeriesView { get { return (RangeAreaSeriesView)base.SeriesView; } }
		[Browsable(false)]
		public new CustomBorderModel Border { get; set; }
		[Browsable(false)]
		public new MarkerModel MarkerOptions { get; set; }
		[Browsable(false)]
		public new DefaultBoolean MarkerVisibility { get; set; }
		[PropertyForOptions("Marker1 Options", 0), TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean Marker1Visibility {
			get { return SeriesView.Marker1Visibility; }
			set { SetProperty("Marker1Visibility", value); }
		}
		[PropertyForOptions("Marker2 Options", 1), TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean Marker2Visibility {
			get { return SeriesView.Marker2Visibility; }
			set { SetProperty("Marker2Visibility", value); }
		}
		[PropertyForOptions,
		AllocateToGroup("Marker1 Options", 0)]
		public MarkerModel Marker1 { get { return marker1Model; } }
		[PropertyForOptions,
		AllocateToGroup("Marker2 Options", 1)]
		public MarkerModel Marker2 { get { return marker2Model; } }
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public CustomBorderModel Border1 { get { return border1Model; } }
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public CustomBorderModel Border2 { get { return border2Model; } }
		public RangeAreaViewModel(RangeAreaSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if(marker1Model != null)
				Children.Add(marker1Model);
			if(marker2Model != null)
				Children.Add(marker2Model);
			if(border1Model != null)
				Children.Add(border1Model);
			if(border2Model != null)
				Children.Add(border2Model);
			base.AddChildren();
		}
		public override List<DataMemberInfo> GetDataMembersInfo() {
			DesignerSeriesModelBase seriesModel = Parent as DesignerSeriesModelBase;
			List<DataMemberInfo> dataMembersInfo = new List<DataMemberInfo>();
			if (seriesModel != null) {
				dataMembersInfo.Add(new DataMemberInfo("Value1DataMember", "Value 1", seriesModel.ValueDataMembers[0], ValueScaleTypes));
				dataMembersInfo.Add(new DataMemberInfo("Value2DataMember", "Value 2", seriesModel.ValueDataMembers[1], ValueScaleTypes));
			}
			return dataMembersInfo;
		}
		public override void Update() {
			this.marker1Model = new MarkerModel(SeriesView.Marker1, CommandManager);
			this.marker2Model = new MarkerModel(SeriesView.Marker2, CommandManager);
			this.border1Model = new CustomBorderModel(SeriesView.Border1, CommandManager);
			this.border2Model = new CustomBorderModel(SeriesView.Border2, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(RangeArea3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(RangeArea3DSeriesViewTypeConverter))]
	public class RangeArea3DViewModel : Area3DViewModel {
		protected new RangeArea3DSeriesView SeriesView { get { return (RangeArea3DSeriesView)base.SeriesView; } }
		public RangeArea3DViewModel(RangeArea3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
}
