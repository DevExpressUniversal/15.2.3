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
	public abstract class FinancialViewBaseModel : XYDiagramViewBaseModel {
		ReductionStockOptionsModel reductionOptionsModel;
		protected new FinancialSeriesViewBase SeriesView { get { return (FinancialSeriesViewBase)base.SeriesView; } }
		[PropertyForOptions]
		public double LevelLineLength {
			get { return SeriesView.LevelLineLength; }
			set { SetProperty("LevelLineLength", value); }
		}
		[PropertyForOptions]
		public int LineThickness {
			get { return SeriesView.LineThickness; }
			set { SetProperty("LineThickness", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public ReductionStockOptionsModel ReductionOptions { get { return reductionOptionsModel; } }
		public FinancialViewBaseModel(FinancialSeriesViewBase seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if(reductionOptionsModel != null)
				Children.Add(reductionOptionsModel);
			base.AddChildren();
		}
		public override List<DataMemberInfo> GetDataMembersInfo() {
			DesignerSeriesModelBase seriesModel = Parent as DesignerSeriesModelBase;
			List<DataMemberInfo> dataMembersInfo = new List<DataMemberInfo>();
			if (seriesModel != null) {
				dataMembersInfo.Add(new DataMemberInfo("LowValueDataMember", "Low", seriesModel.ValueDataMembers[0], ValueScaleTypes));
				dataMembersInfo.Add(new DataMemberInfo("HighValueDataMember", "High", seriesModel.ValueDataMembers[1], ValueScaleTypes));
				dataMembersInfo.Add(new DataMemberInfo("OpenValueDataMember", "Open", seriesModel.ValueDataMembers[2], ValueScaleTypes));
				dataMembersInfo.Add(new DataMemberInfo("CloseValueDataMember", "Close", seriesModel.ValueDataMembers[3], ValueScaleTypes));
			}
			return dataMembersInfo;
		}
		public override void Update() {
			this.reductionOptionsModel = new ReductionStockOptionsModel(SeriesView.ReductionOptions, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(StockSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(StockSeriesViewTypeConverter))]
	public class StockViewModel : FinancialViewBaseModel {
		protected new StockSeriesView SeriesView { get { return (StockSeriesView)base.SeriesView; } }
		[PropertyForOptions]
		public StockType ShowOpenClose {
			get { return SeriesView.ShowOpenClose; }
			set { SetProperty("ShowOpenClose", value); }
		}
		public StockViewModel(StockSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(CandleStickSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(CandleStickSeriesViewTypeConverter))]
	public class CandleStickViewModel : FinancialViewBaseModel {
		protected new CandleStickSeriesView SeriesView { get { return (CandleStickSeriesView)base.SeriesView; } }
		public CandleStickViewModel(CandleStickSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[GroupPrefix("View: ")]	
	public abstract class GanttViewModel : RangeBarViewModel {
		TaskLinkOptionsModel linkOptionsModel;
		protected new GanttSeriesView SeriesView { get { return (GanttSeriesView)base.SeriesView; } }
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		PropertyForOptions,
		AllocateToGroup("LinkOptions")]
		public TaskLinkOptionsModel LinkOptions { get { return linkOptionsModel; } }
		public GanttViewModel(GanttSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if(linkOptionsModel != null)
				Children.Add(linkOptionsModel);
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
			this.linkOptionsModel = new TaskLinkOptionsModel(SeriesView.LinkOptions, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(OverlappedGanttSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(OverlappedGanttSeriesViewTypeConverter))]
	public class OverlappedGanttViewModel : GanttViewModel {
		protected new OverlappedGanttSeriesView SeriesView { get { return (OverlappedGanttSeriesView)base.SeriesView; } }	
		public OverlappedGanttViewModel(OverlappedGanttSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(SideBySideGanttSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(SideBySideGanttSeriesViewTypeConverter))]
	public class SideBySideGanttViewModel : GanttViewModel {
		protected new SideBySideGanttSeriesView SeriesView { get { return (SideBySideGanttSeriesView)base.SeriesView; } }
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
		public SideBySideGanttViewModel(SideBySideGanttSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
}
