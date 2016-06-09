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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	[ModelOf(typeof(SeriesPointFilter))]
	public class SeriesPointFilterModel : DesignerChartElementModelBase, IObjectValueTypeProvider {
		readonly SeriesPointFilter seriesPointFilter;
		protected SeriesPointFilter SeriesPointFilter { get { return seriesPointFilter; } }
		protected internal override ChartElement ChartElement { get { return seriesPointFilter; } }
		[PropertyForOptions, TypeConverter(typeof(SeriesPointKeyConverter))]
		public SeriesPointKey Key {
			get { return seriesPointFilter.Key; }
			set { SetProperty("Key", value); }
		}
		[PropertyForOptions]
		public DataFilterCondition Condition {
			get { return seriesPointFilter.Condition; }
			set { SetProperty("Condition", value); }
		}
		[TypeConverter(typeof(ObjectValueTypeConverter))]
		public object Value {
			get { return seriesPointFilter.Value; }
			set { SetProperty("Value", value); }
		}
		public SeriesPointFilterModel(SeriesPointFilter seriesPointFilter, CommandManager commandManager)
			: base(commandManager) {
			this.seriesPointFilter = seriesPointFilter;
		}
		#region IObjectValueTypeProvider Members
		Type IObjectValueTypeProvider.DataType { get { return ((IObjectValueTypeProvider)seriesPointFilter).DataType; } }
		#endregion
	}
	[ModelOf(typeof(DataFilter))]
	public class DataFilterModel : DesignerChartElementModelBase, IObjectValueTypeProvider {
		readonly DataFilter dataFilter;
		protected DataFilter DataFilter { get { return dataFilter; } }
		protected internal override ChartElement ChartElement { get { return dataFilter; } }
		[
		PropertyForOptions,
		Editor(typeof(DataMemberModelEditor), typeof(UITypeEditor))
		]
		public string ColumnName {
			get { return DataFilter.ColumnName; }
			set { SetProperty("ColumnName", value); }
		}
		[PropertyForOptions, TypeConverter(typeof(DataFilterConditionTypeConverter))]
		public DataFilterCondition Condition {
			get { return DataFilter.Condition; }
			set { SetProperty("Condition", value); }
		}
		[TypeConverter(typeof(DataTypeConverter))]
		public Type DataType {
			get { return DataFilter.DataType; }
			set { SetProperty("DataType", value); }
		}
		[TypeConverter(typeof(ObjectValueTypeConverter))]
		public object Value {
			get { return DataFilter.Value; }
			set { SetProperty("Value", value); }
		}
		public DataFilterModel(DataFilter dataFilter, CommandManager commandManager)
			: base(commandManager) {
			this.dataFilter = dataFilter;
		}
	}
	[ModelOf(typeof(SeriesPoint))]
	public class SeriesPointModel : DesignerChartElementModelBase {
		readonly SeriesPoint point;
		readonly SetPointValuesCommand setValuesCommand;
		readonly SetPointDateTimeValuesCommand setDateTimeValuesCommand;
		readonly SetPointIsEmptyCommand setIsEmptyCommand;
		SeriesPointRelationCollectionModel relationsModel;
		protected SeriesPoint Point { get { return point; } }
		protected internal override ChartElement ChartElement { get { return point; } }
		[PropertyForOptions, Browsable(false)]
		public SeriesPointRelationCollectionModel Relations { get { return relationsModel; } }
		[PropertyForOptions, Browsable(false)]
		public Color Color {
			get { return Point.Color; }
			set { SetProperty("Color", value); }
		}
		[PropertyForOptions("Data"), Browsable(false)]
		public string ToolTipHint {
			get { return Point.ToolTipHint; }
			set { SetProperty("ToolTipHint", value); }
		}
		[PropertyForOptions, Browsable(false)]
		public bool IsEmpty {
			get { return Point.IsEmpty; }
			set { setIsEmptyCommand.Execute(value); }
		}
		[Browsable(false)]
		public string Argument {
			get { return Point.Argument; }
			set { SetProperty("Argument", value); }
		}
		[Browsable(false)]
		public double[] Values {
			get { return Point.Values; }
			set { setValuesCommand.Execute(value); }
		}
		[Browsable(false)]
		public DateTime DateTimeArgument {
			get { return Point.DateTimeArgument; }
			set { SetProperty("DateTimeArgument", value); }
		}
		[Browsable(false)]
		public DateTime[] DateTimeValues {
			get { return Point.DateTimeValues; }
			set { setDateTimeValuesCommand.Execute(value); }
		}
		public SeriesPointModel(SeriesPoint point, CommandManager commandManager)
			: base(commandManager) {
			this.point = point;
			this.relationsModel = Point != null ? new SeriesPointRelationCollectionModel(Point.Relations, this, CommandManager, null) : null;
			this.setDateTimeValuesCommand = new SetPointDateTimeValuesCommand(commandManager, point);
			this.setValuesCommand = new SetPointValuesCommand(commandManager, point);
			this.setIsEmptyCommand = new SetPointIsEmptyCommand(commandManager, point);
			Update();
		}
		protected override void AddChildren() {
			if (relationsModel != null)
				Children.Add(relationsModel);
			base.AddChildren();
		}
		bool NeedUpdateRelations() {
			if(relationsModel == null)
				return Point != null;
			return Point == null || relationsModel.ChartCollection != Point.Relations;
		}
		public override void Update() {
			if(NeedUpdateRelations())
				this.relationsModel = Point != null ? new SeriesPointRelationCollectionModel(Point.Relations, this, CommandManager, null) : null;
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(Relation))]
	public class RelationModel : DesignerChartElementModelBase {
		readonly Relation relation;
		readonly RelationSeriesPointModel parentPointModel;
		RelationSeriesPointModel childPointModel;
		protected Relation Relation { get { return relation; } }
		protected internal override ChartElement ChartElement { get { return relation; } }
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public RelationSeriesPointModel ChildPoint {
			get { return childPointModel; }
			set { SetProperty("ChildPoint", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public RelationSeriesPointModel ParentPoint { get { return parentPointModel; } }
		public RelationModel(Relation relation, SeriesPoint point, CommandManager commandManager)
			: base(commandManager) {
			this.relation = relation;
			this.parentPointModel = new RelationSeriesPointModel(point);
			Update();
		}
		public override void Update() {
			this.childPointModel = new RelationSeriesPointModel(Relation.ChildPoint);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(TaskLink))]
	public class TaskLinkModel : RelationModel {
		protected TaskLink TaskLink { get { return (TaskLink)base.Relation; } }
		public TaskLinkType LinkType {
			get { return TaskLink.LinkType; }
			set { SetProperty("LinkType", value); }
		}
		public TaskLinkModel(TaskLink taskLink, SeriesPoint point, CommandManager commandManager)
			: base(taskLink, point, commandManager) {
		}
	}
	public class RelationSeriesPointModel {
		readonly SeriesPoint innerPoint;
		[Browsable(false)]
		public SeriesPoint SeriesPointModel { get { return innerPoint; } }
		public string Argument { get { return innerPoint.Argument; } }
		public string Value1 { get { return innerPoint.GetValueString(0); } }
		public string Value2 { get { return innerPoint.GetValueString(1); } }
		public RelationSeriesPointModel(SeriesPoint seriesPoint) {
			this.innerPoint = seriesPoint;
		}
		public override string ToString() {
			return innerPoint.ToString();
		}
	}
}
