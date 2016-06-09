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

using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	public abstract class SimpleDiagramViewBaseModel : SeriesViewModelBase {
		SeriesTitleCollectionModel titlesCollectionModel;
		protected new SimpleDiagramSeriesViewBase SeriesView { get { return (SimpleDiagramSeriesViewBase)base.SeriesView; } }
		[Browsable(false)]
		public SeriesTitleCollectionModel Titles { get { return titlesCollectionModel; } }
		public SimpleDiagramViewBaseModel(SimpleDiagramSeriesViewBase seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if(titlesCollectionModel != null)
				Children.Add(titlesCollectionModel);
			base.AddChildren();
		}
		public override void Update() {
			this.titlesCollectionModel = new SeriesTitleCollectionModel(SeriesView.Titles, CommandManager, null);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	public abstract class PieViewBaseModel : SimpleDiagramViewBaseModel {
		SeriesPointFilterCollectionModel explodedPointsFiltersModel;
		ExplodedSeriesPointCollectionModel explodedPointsModel;
		protected new PieSeriesViewBase SeriesView { get { return (PieSeriesViewBase)base.SeriesView; } }
		public double ExplodedDistancePercentage {
			get { return SeriesView.ExplodedDistancePercentage; }
			set { SetProperty("ExplodedDistancePercentage", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(EnumTypeConverter))]
		public PieSweepDirection SweepDirection {
			get { return SeriesView.SweepDirection; }
			set { SetProperty("SweepDirection", value); }
		}
		[TypeConverter(typeof(ExplodeModeTypeConverter))]
		public PieExplodeMode ExplodeMode {
			get { return SeriesView.ExplodeMode; }
			set { SetProperty("ExplodeMode", value); }
		}
		[TypeConverter(typeof(CollectionTypeConverter)), Editor(typeof(SeriesPointFilterModelCollectionEditor), typeof(UITypeEditor))]
		public SeriesPointFilterCollectionModel ExplodedPointsFilters { get { return explodedPointsFiltersModel; } }
		[TypeConverter(typeof(CollectionTypeConverter)), Editor(typeof(ExplodedPointsModelEditor), typeof(UITypeEditor))]
		public ExplodedSeriesPointCollectionModel ExplodedPoints { get { return explodedPointsModel; } }
		public PieViewBaseModel(PieSeriesViewBase seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if(explodedPointsFiltersModel != null)
				Children.Add(explodedPointsFiltersModel);
			if (explodedPointsModel != null)
				Children.Add(explodedPointsModel);
			base.AddChildren();
		}
		public override void Update() {
			if (this.explodedPointsFiltersModel == null || !this.explodedPointsFiltersModel.ChartCollection.Equals(SeriesView.ExplodedPointsFilters))
				this.explodedPointsFiltersModel = new SeriesPointFilterCollectionModel(SeriesView.ExplodedPointsFilters, CommandManager);
			if (this.explodedPointsModel == null || !this.explodedPointsModel.ChartCollection.Equals(SeriesView.ExplodedPoints))
				this.explodedPointsModel = new ExplodedSeriesPointCollectionModel(SeriesView.ExplodedPoints, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(PieSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(PieSeriesViewTypeConverter))]
	public class PieViewModel : PieViewBaseModel {
		CustomBorderModel customBorderModel;
		PolygonFillStyleModel polygonFillStyleModel;
		protected new PieSeriesView SeriesView { get { return (PieSeriesView)base.SeriesView; } }
		[Browsable(false)]
		public new Color Color { get; set; }
		[PropertyForOptions]
		public double HeightToWidthRatio {
			get { return SeriesView.HeightToWidthRatio; }
			set { SetProperty("HeightToWidthRatio", value); }
		}
		[PropertyForOptions]
		public double MinAllowedSizePercentage {
			get { return SeriesView.MinAllowedSizePercentage; }
			set { SetProperty("MinAllowedSizePercentage", value); }
		}
		[PropertyForOptions]
		public int Rotation {
			get { return SeriesView.Rotation; }
			set { SetProperty("Rotation", value); }
		}
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool RuntimeExploding {
			get { return SeriesView.RuntimeExploding; }
			set { SetProperty("RuntimeExploding", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public CustomBorderModel Border { get { return customBorderModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public PolygonFillStyleModel FillStyle { get { return polygonFillStyleModel; } }
		public PieViewModel(PieSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if(customBorderModel != null)
				Children.Add(customBorderModel);
			if(polygonFillStyleModel != null)
				Children.Add(polygonFillStyleModel);
			base.AddChildren();
		}
		public override void Update() {
			this.customBorderModel = new CustomBorderModel(SeriesView.Border, CommandManager);
			this.polygonFillStyleModel = new PolygonFillStyleModel(SeriesView.FillStyle, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(DoughnutSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(DoughnutSeriesViewTypeConverter))]
	public class DoughnutViewModel : PieViewModel {
		protected new DoughnutSeriesView SeriesView { get { return (DoughnutSeriesView)base.SeriesView; } }
		[PropertyForOptions]
		public int HoleRadiusPercent {
			get { return SeriesView.HoleRadiusPercent; }
			set { SetProperty("HoleRadiusPercent", value); }
		}
		public DoughnutViewModel(DoughnutSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(NestedDoughnutSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(NestedDoughnutSeriesViewTypeConverter))]
	public class NestedDoughnutViewModel : PieViewModel {
		protected new NestedDoughnutSeriesView SeriesView { get { return (NestedDoughnutSeriesView)base.SeriesView; } }
		[PropertyForOptions]
		public double InnerIndent {
			get { return SeriesView.InnerIndent; }
			set { SetProperty("InnerIndent", value); }
		}
		[PropertyForOptions]
		public double Weight {
			get { return SeriesView.Weight; }
			set { SetProperty("Weight", value); }
		}
		[PropertyForOptions]
		public int HoleRadiusPercent {
			get { return SeriesView.HoleRadiusPercent; }
			set { SetProperty("HoleRadiusPercent", value); }
		}
		[
		PropertyForOptions,
		UseEditor(typeof(StackedGroupControl), typeof(StackedGroupControlAdapter)),
		Editor(typeof(SeriesGroupModelEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(ObjectTypeConverter))
		]
		public object Group {
			get { return SeriesView.Group; }
			set { SetProperty("Group", value); }
		}
		public NestedDoughnutViewModel(NestedDoughnutSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(Pie3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(Pie3DSeriesViewTypeConverter))]
	public class Pie3DViewModel : PieViewBaseModel {
		PolygonFillStyle3DModel fillStyleModel;
		protected new Pie3DSeriesView SeriesView { get { return (Pie3DSeriesView)base.SeriesView; } }
		[Browsable(false)]
		public new Color Color { get; set; }
		[PropertyForOptions]
		public int Depth {
			get { return SeriesView.Depth; }
			set { SetProperty("Depth", value); }
		}
		public double SizeAsPercentage {
			get { return SeriesView.SizeAsPercentage; }
			set { SetProperty("SizeAsPercentage", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public PolygonFillStyle3DModel PieFillStyle { get { return fillStyleModel; } }
		public Pie3DViewModel(Pie3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if(fillStyleModel != null)
				Children.Add(fillStyleModel);
			base.AddChildren();
		}
		public override void Update() {
			this.fillStyleModel = new PolygonFillStyle3DModel(SeriesView.PieFillStyle, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(Doughnut3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(Doughnut3DSeriesViewTypeConverter))]
	public class Doughnut3DViewModel : Pie3DViewModel {
		protected new Doughnut3DSeriesView SeriesView { get { return (Doughnut3DSeriesView)base.SeriesView; } }
		[PropertyForOptions]
		public int HoleRadiusPercent {
			get { return SeriesView.HoleRadiusPercent; }
			set { SetProperty("HoleRadiusPercent", value); }
		}
		public Doughnut3DViewModel(Doughnut3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	public abstract class FunnelViewBaseModel : SimpleDiagramViewBaseModel {
		protected new FunnelSeriesViewBase SeriesView { get { return (FunnelSeriesViewBase)base.SeriesView; } }
		[PropertyForOptions]
		public double HeightToWidthRatio {
			get { return SeriesView.HeightToWidthRatio; }
			set { SetProperty("HeightToWidthRatio", value); }
		}
		[PropertyForOptions]
		public int PointDistance {
			get { return SeriesView.PointDistance; }
			set { SetProperty("PointDistance", value); }
		}
		public FunnelViewBaseModel(FunnelSeriesViewBase seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(FunnelSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(FunnelSeriesViewTypeConverter))]
	public class FunnelViewModel : FunnelViewBaseModel {
		CustomBorderModel borderModel;
		PolygonFillStyleModel fillStyleModel;
		protected new FunnelSeriesView SeriesView { get { return (FunnelSeriesView)base.SeriesView; } }
		[Browsable(false)]
		public new Color Color { get; set; }
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool AlignToCenter {
			get { return SeriesView.AlignToCenter; }
			set { SetProperty("AlignToCenter", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(BooleanTypeConverter))]
		public bool HeightToWidthRatioAuto {
			get { return SeriesView.HeightToWidthRatioAuto; }
			set { SetProperty("HeightToWidthRatioAuto", value); }
		}
		[
		PropertyForOptions,
		DependentUpon("HeightToWidthRatioAuto")]
		public new double HeightToWidthRatio {
			get { return base.HeightToWidthRatio; }
			set { base.HeightToWidthRatio = value; }
		}
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public CustomBorderModel Border { get { return borderModel; } }
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public PolygonFillStyleModel FillStyle { get { return fillStyleModel; } }
		public FunnelViewModel(FunnelSeriesView seriesView, CommandManager commandManager)
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
			this.borderModel = new CustomBorderModel(SeriesView.Border, CommandManager);
			this.fillStyleModel = new PolygonFillStyleModel(SeriesView.FillStyle, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(Funnel3DSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(Funnel3DSeriesViewTypeConverter))]
	public class Funnel3DViewModel : FunnelViewBaseModel {
		protected new Funnel3DSeriesView SeriesView { get { return (Funnel3DSeriesView)base.SeriesView; } }
		[Browsable(false)]
		public new Color Color { get; set; }
		[PropertyForOptions]
		public int HoleRadiusPercent {
			get { return SeriesView.HoleRadiusPercent; }
			set { SetProperty("HoleRadiusPercent", value); }
		}
		public Funnel3DViewModel(Funnel3DSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
}
