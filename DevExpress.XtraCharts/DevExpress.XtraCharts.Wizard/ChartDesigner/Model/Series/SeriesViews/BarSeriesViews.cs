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
using System.Drawing.Design;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.Utils.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	[GroupPrefix("View: ")]
	public abstract class BarViewModel : ColorEachSupportViewBaseModel {
		InsideRectangularBorderModel borderModel;
		RectangleFillStyleModel fillStyleModel;
		protected new BarSeriesView SeriesView { get { return (BarSeriesView)base.SeriesView; } }
		[PropertyForOptions]
		public double BarWidth {
			get { return SeriesView.BarWidth; }
			set { SetProperty("BarWidth", value); }
		}
		public byte Transparency {
			get { return SeriesView.Transparency; }
			set { SetProperty("Transparency", value); }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("Border")]
		public RectangularBorderModel Border {
			get { return borderModel; }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("Appearance")]
		public RectangleFillStyleModel FillStyle {
			get { return fillStyleModel; }
		}
		public BarViewModel(BarSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if(borderModel != null)
				Children.Add(borderModel);
			if(fillStyleModel != null)
				Children.Add(fillStyleModel);
			base.AddChildren();
		}
		public override void Update() {
			this.borderModel = new InsideRectangularBorderModel((InsideRectangularBorder)SeriesView.Border, CommandManager);
			this.fillStyleModel = new RectangleFillStyleModel(SeriesView.FillStyle, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(SideBySideBarSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(SideBySideBarSeriesViewTypeConverter))]
	public class SideBySideBarViewModel : BarViewModel {
		protected new SideBySideBarSeriesView SeriesView { get { return (SideBySideBarSeriesView)base.SeriesView; } }
		public double BarDistance {
			get { return SeriesView.BarDistance; }
			set { SetProperty("BarDistance", value); }
		}
		public int BarDistanceFixed {
			get { return SeriesView.BarDistanceFixed; }
			set { SetProperty("BarDistanceFixed", value); }
		}
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool EqualBarWidth {
			get { return SeriesView.EqualBarWidth; }
			set { SetProperty("EqualBarWidth", value); }
		}
		public SideBySideBarViewModel(SideBySideBarSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(StackedBarSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(StackedBarSeriesViewTypeConverter))]
	public class StackedBarViewModel : BarViewModel {
		protected new StackedBarSeriesView SeriesView { get { return (StackedBarSeriesView)base.SeriesView; } }
		public StackedBarViewModel(StackedBarSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(FullStackedBarSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(FullStackedBarSeriesViewTypeConverter))]
	public class FullStackedBarViewModel : StackedBarViewModel {
		protected new FullStackedBarSeriesView SeriesView { get { return (FullStackedBarSeriesView)base.SeriesView; } }
		public FullStackedBarViewModel(FullStackedBarSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(SideBySideStackedBarSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(SideBySideStackedBarSeriesViewTypeConverter))]
	public class SideBySideStackedBarViewModel : StackedBarViewModel {
		protected new SideBySideStackedBarSeriesView SeriesView { get { return (SideBySideStackedBarSeriesView)base.SeriesView; } }
		[
		PropertyForOptions,
		UseEditor(typeof(StackedGroupControl), typeof(StackedGroupControlAdapter)),
		Editor(typeof(SeriesGroupModelEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(ObjectTypeConverter))
		]
		public object StackedGroup {
			get { return SeriesView.StackedGroup; }
			set { SetProperty("StackedGroup", value); }
		}
		public double BarDistance {
			get { return SeriesView.BarDistance; }
			set { SetProperty("BarDistance", value); }
		}
		public int BarDistanceFixed {
			get { return SeriesView.BarDistanceFixed; }
			set { SetProperty("BarDistanceFixed", value); }
		}
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool EqualBarWidth {
			get { return SeriesView.EqualBarWidth; }
			set { SetProperty("EqualBarWidth", value); }
		}
		public SideBySideStackedBarViewModel(SideBySideStackedBarSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(SideBySideFullStackedBarSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(SideBySideFullStackedBarSeriesViewTypeConverter))]
	public class SideBySideFullStackedBarViewModel : FullStackedBarViewModel {
		protected new SideBySideFullStackedBarSeriesView SeriesView { get { return (SideBySideFullStackedBarSeriesView)base.SeriesView; } }
		[
		PropertyForOptions,
		UseEditor(typeof(StackedGroupControl), typeof(StackedGroupControlAdapter)),
		Editor(typeof(SeriesGroupModelEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(ObjectTypeConverter))
		]
		public object StackedGroup {
			get { return SeriesView.StackedGroup; }
			set { SetProperty("StackedGroup", value); }
		}
		public double BarDistance {
			get { return SeriesView.BarDistance; }
			set { SetProperty("BarDistance", value); }
		}
		public int BarDistanceFixed {
			get { return SeriesView.BarDistanceFixed; }
			set { SetProperty("BarDistanceFixed", value); }
		}
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool EqualBarWidth {
			get { return SeriesView.EqualBarWidth; }
			set { SetProperty("EqualBarWidth", value); }
		}
		public SideBySideFullStackedBarViewModel(SideBySideFullStackedBarSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[GroupPrefix("View: ")]
	public abstract class Bar3DViewModel : View3DColorEachSupportBaseModel {
		RectangleFillStyle3DModel fillStyleModel;
		protected new Bar3DSeriesView SeriesView { get { return (Bar3DSeriesView)base.SeriesView; } }
		public double BarDepth {
			get { return SeriesView.BarDepth; }
			set { SetProperty("BarDepth", value); }
		}
		[PropertyForOptions]
		public double BarWidth {
			get { return SeriesView.BarWidth; }
			set { SetProperty("BarWidth", value); }
		}
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool BarDepthAuto {
			get { return SeriesView.BarDepthAuto; }
			set { SetProperty("BarDepthAuto", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("Model"),
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowFacet {
			get { return SeriesView.ShowFacet; }
			set { SetProperty("ShowFacet", value); }
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("Appearance")]
		public RectangleFillStyle3DModel FillStyle { get { return fillStyleModel; } }
		[PropertyForOptions]
		public Bar3DModel Model {
			get { return SeriesView.Model; }
			set { SetProperty("Model", value); }
		}
		public Bar3DViewModel(Bar3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if(fillStyleModel != null)
				Children.Add(fillStyleModel);
			base.AddChildren();
		}
		public override void Update() {
			this.fillStyleModel = new RectangleFillStyle3DModel(SeriesView.FillStyle, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(SideBySideBar3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(SideBySideBar3DSeriesViewTypeConverter))]
	public class SideBySideBar3DViewModel : Bar3DViewModel {
		protected new SideBySideBar3DSeriesView SeriesView { get { return (SideBySideBar3DSeriesView)base.SeriesView; } }
		public double BarDistance {
			get { return SeriesView.BarDistance; }
			set { SetProperty("BarDistance", value); }
		}
		public int BarDistanceFixed {
			get { return SeriesView.BarDistanceFixed; }
			set { SetProperty("BarDistanceFixed", value); }
		}
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool EqualBarWidth {
			get { return SeriesView.EqualBarWidth; }
			set { SetProperty("EqualBarWidth", value); }
		}
		public SideBySideBar3DViewModel(SideBySideBar3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(StackedBar3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(StackedBar3DSeriesViewTypeConverter))]
	public class StackedBar3DViewModel : Bar3DViewModel {
		protected new StackedBar3DSeriesView SeriesView { get { return (StackedBar3DSeriesView)base.SeriesView; } }
		public StackedBar3DViewModel(StackedBar3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(FullStackedBar3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(FullStackedBar3DSeriesViewTypeConverter))]
	public class FullStackedBar3DViewModel : StackedBar3DViewModel {
		protected new FullStackedBar3DSeriesView SeriesView { get { return (FullStackedBar3DSeriesView)base.SeriesView; } }
		public FullStackedBar3DViewModel(FullStackedBar3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(ManhattanBarSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(ManhattanBarSeriesViewTypeConverter))]
	public class ManhattanBarViewModel : Bar3DViewModel {
		protected new ManhattanBarSeriesView SeriesView { get { return (ManhattanBarSeriesView)base.SeriesView; } }
		public ManhattanBarViewModel(ManhattanBarSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(SideBySideStackedBar3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(SideBySideStackedBar3DSeriesViewTypeConverter))]
	public class SideBySideStackedBar3DViewModel : StackedBar3DViewModel {
		protected new SideBySideStackedBar3DSeriesView SeriesView { get { return (SideBySideStackedBar3DSeriesView)base.SeriesView; } }
		[
		PropertyForOptions,
		UseEditor(typeof(StackedGroupControl), typeof(StackedGroupControlAdapter)),
		Editor(typeof(SeriesGroupModelEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(ObjectTypeConverter))
		]
		public object StackedGroup {
			get { return SeriesView.StackedGroup; }
			set { SetProperty("StackedGroup", value); }
		}
		public double BarDistance {
			get { return SeriesView.BarDistance; }
			set { SetProperty("BarDistance", value); }
		}
		public int BarDistanceFixed {
			get { return SeriesView.BarDistanceFixed; }
			set { SetProperty("BarDistanceFixed", value); }
		}
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool EqualBarWidth {
			get { return SeriesView.EqualBarWidth; }
			set { SetProperty("EqualBarWidth", value); }
		}
		public SideBySideStackedBar3DViewModel(SideBySideStackedBar3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(SideBySideFullStackedBar3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(SideBySideFullStackedBar3DSeriesViewTypeConverter))]
	public class SideBySideFullStackedBar3DViewModel : FullStackedBar3DViewModel {
		protected new SideBySideFullStackedBar3DSeriesView SeriesView { get { return (SideBySideFullStackedBar3DSeriesView)base.SeriesView; } }
		[
		PropertyForOptions,
		Editor(typeof(SeriesGroupModelEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(ObjectTypeConverter))
		]
		public object StackedGroup {
			get { return SeriesView.StackedGroup; }
			set { SetProperty("StackedGroup", value); }
		}
		public double BarDistance {
			get { return SeriesView.BarDistance; }
			set { SetProperty("BarDistance", value); }
		}
		public int BarDistanceFixed {
			get { return SeriesView.BarDistanceFixed; }
			set { SetProperty("BarDistanceFixed", value); }
		}
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool EqualBarWidth {
			get { return SeriesView.EqualBarWidth; }
			set { SetProperty("EqualBarWidth", value); }
		}
		public SideBySideFullStackedBar3DViewModel(SideBySideFullStackedBar3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[GroupPrefix("View: ")]
	public abstract class RangeBarViewModel : BarViewModel {
		MarkerModel minValueMarkerModel;
		MarkerModel maxValueMarkerModel;
		protected new RangeBarSeriesView SeriesView { get { return (RangeBarSeriesView)base.SeriesView; } }
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean MaxValueMarkerVisibility {
			get { return SeriesView.MaxValueMarkerVisibility; }
			set { SetProperty("MaxValueMarkerVisibility", value); }
		}
		[TypeConverter(typeof(DefaultBooleanConverter))]
		public DefaultBoolean MinValueMarkerVisibility {
			get { return SeriesView.MinValueMarkerVisibility; }
			set { SetProperty("MinValueMarkerVisibility", value); }
		}
		public MarkerModel MinValueMarker { get { return minValueMarkerModel; } }
		public MarkerModel MaxValueMarker { get { return maxValueMarkerModel; } }
		public RangeBarViewModel(RangeBarSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if(minValueMarkerModel != null)
				Children.Add(minValueMarkerModel);
			if(maxValueMarkerModel != null)
				Children.Add(maxValueMarkerModel);
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
			this.minValueMarkerModel = new MarkerModel(SeriesView.MinValueMarker, CommandManager);
			this.maxValueMarkerModel = new MarkerModel(SeriesView.MaxValueMarker, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(OverlappedRangeBarSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(OverlappedRangeBarSeriesViewTypeConverter))]
	public class OverlappedRangeBarViewModel : RangeBarViewModel {
		protected new OverlappedRangeBarSeriesView SeriesView { get { return (OverlappedRangeBarSeriesView)base.SeriesView; } }
		public OverlappedRangeBarViewModel(OverlappedRangeBarSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(SideBySideRangeBarSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(SideBySideRangeBarSeriesViewTypeConverter))]
	public class SideBySideRangeBarViewModel : RangeBarViewModel {
		protected new SideBySideRangeBarSeriesView SeriesView { get { return (SideBySideRangeBarSeriesView)base.SeriesView; } }
		public double BarDistance {
			get { return SeriesView.BarDistance; }
			set { SetProperty("BarDistance", value); }
		}
		public int BarDistanceFixed {
			get { return SeriesView.BarDistanceFixed; }
			set { SetProperty("BarDistanceFixed", value); }
		}
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool EqualBarWidth {
			get { return SeriesView.EqualBarWidth; }
			set { SetProperty("EqualBarWidth", value); }
		}
		public SideBySideRangeBarViewModel(SideBySideRangeBarSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
}
