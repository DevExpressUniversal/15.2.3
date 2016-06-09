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
using System.Drawing;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.PivotGrid.Internal;
using System.Collections.Generic;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.XtraPivotGrid {
	public class PivotChartDescriptorBase : PropertyDescriptor {
		public const string ValuesColumn = "Values",
			ArgumentsColumn = "Arguments", SeriesColumn = "Series";
		static string GetName(PivotChartDescriptorType type) {
			switch(type) {
				case PivotChartDescriptorType.Value:
					return ValuesColumn;
				case PivotChartDescriptorType.Argument:
					return ArgumentsColumn;
				case PivotChartDescriptorType.Series:
					return SeriesColumn;
			}
			throw new ArgumentException("Unknown type!");
		}
		public PivotChartDescriptorBase(PivotChartDescriptorType type)
			: base(GetName(type), new Attribute[0] { }) {
			this.type = type;
		}
		PivotChartDescriptorType type;
		protected PivotChartDescriptorType Type { get { return type; } }
		public override bool CanResetValue(object component) { return false; }
		public override Type ComponentType { get { return typeof(object); } }
		public override bool IsReadOnly { get { return true; } }
		public override Type PropertyType {
			get {
				switch(type) {
					case PivotChartDescriptorType.Argument:
					case PivotChartDescriptorType.Series:
						return typeof(string);
					case PivotChartDescriptorType.Value:
						return typeof(decimal);
				}
				return null;
			}
		}
		public override void ResetValue(object component) { }
		public override object GetValue(object component) { return null; }
		public override void SetValue(object component, object value) { }
		public override bool ShouldSerializeValue(object component) { return false; }
	}
	public class PivotChartDescriptor : PivotChartDescriptorBase {
		readonly PivotChartDataSourceBase dataSource;
		protected PivotChartDataSourceBase DataSource { get { return dataSource; } }
		public PivotChartDescriptor(PivotChartDataSourceBase dataSource, PivotChartDescriptorType type)
			: base(type) {
			this.dataSource = dataSource;
		}
		public override object GetValue(object component) {
			PivotChartDataSourceRowItem row = (PivotChartDataSourceRowItem)component;
			if(row == null)
				return null;
			switch(Type) {
				case PivotChartDescriptorType.Value:
					return row.Value;
				case PivotChartDescriptorType.Argument:
					return row.Argument;
				case PivotChartDescriptorType.Series:
					return row.Series;
			}
			return null;
		}
		public override Type PropertyType {
			get {
				switch(Type) {
					case PivotChartDescriptorType.Argument:
						return dataSource.ArgumentValueType;
					case PivotChartDescriptorType.Series:
						return dataSource.SeriesValueType;
					case PivotChartDescriptorType.Value:
						return dataSource.ValuesValueType;
				}
				return base.PropertyType;
			}
		}
	}
	public class PivotChartDataSourceRowItem
#if !SL && !DXPORTABLE
 : ICustomTypeDescriptor
#endif
 {
		readonly int rowIndex;
		readonly PivotChartDataSourceBase dataSource;
		PivotChartDataSourceRowItem(int rowIndex, PivotChartDataSourceBase dataSource) {
			this.rowIndex = rowIndex;
			this.dataSource = dataSource;
		}
		public static PivotChartDataSourceRowItem CreateRow(int rowIndex, PivotChartDataSourceBase dataSource) {
			return new PivotChartDataSourceRowItem(rowIndex, dataSource);
		}
		public int CellX { get { return RowItem.Cell.X; } }
		public int CellY { get { return RowItem.Cell.Y; } }
		PivotChartDataSourceBase DataSource { get { return dataSource; } }
		PivotChartDataSourceRowBase RowItem { get { return DataSource.GetRowItem(rowIndex); } }
		public object Value { get { return RowItem.Value; } }
		public object Series { get { return RowItem.Series; } }
		public object Argument { get { return RowItem.Argument; } }
		public int[] Indexes { get { return new int[] { CellX, CellY }; } }
#if !SL && !DXPORTABLE
		#region ICustomTypeDescriptor Members
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return TypeDescriptor.GetAttributes(this, true);
		}
		TypeConverter ICustomTypeDescriptor.GetConverter() {
			return TypeDescriptor.GetConverter(this, true);
		}
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		string ICustomTypeDescriptor.GetClassName() {
			return GetType().Name;
		}
		string ICustomTypeDescriptor.GetComponentName() {
			return GetType().Name;
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return TypeDescriptor.GetEvents(this, true);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return DataSource.ChartProps;
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return DataSource.ChartProps;
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return this;
		}
		#endregion
#endif
	}
	public class PivotChartDataSourceRowBase {
		static readonly Point UnknownPoint = new Point(-1, -1);
		readonly PivotChartDataSourceBase source;
		readonly Point cell;
		readonly bool isRowCustom;
		bool isArgumentCustom;
		bool isSeriesCustom;
		bool isValueCustom;
		protected internal PivotChartDataSourceRowBase(PivotChartDataSourceBase source, Point cell) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			this.cell = cell;
		}
		protected internal PivotChartDataSourceRowBase(PivotChartDataSourceBase source)
			: this(source, UnknownPoint) {
			isRowCustom = true;
		}
		object argument;
		object series;
		object value;
		public object Argument {
			get {
				if(argument == null && !isArgumentCustom && !isRowCustom)
					argument = source.GetArgumentValue(Cell);
				return argument;
			}
			set {
				argument = value;
				isArgumentCustom = true;
			}
		}
		public object Series {
			get {
				if(series == null && !isSeriesCustom && !isRowCustom)
					series = source.GetSeriesValue(Cell);
				return series;
			}
			set {
				series = value;
				isSeriesCustom = true;
			}
		}
		public object Value {
			get {
				if(value == null && !isValueCustom && !isRowCustom)
					value = source.GetChartCellValue(Cell.X, Cell.Y);
				return value;
			}
			set {
				this.value = value;
				isValueCustom = true;
			}
		}
		protected internal PivotFieldValueItem ColumnItem {
			get {
				if(!isRowCustom)
					return source.GetLastLevelItem(true, cell.X);
				return null;
			}
		}
		protected internal PivotFieldValueItem RowItem {
			get {
				if(!isRowCustom)
					return source.GetLastLevelItem(false, cell.Y);
				return null;
			}
		}
		protected internal PivotGridCellItem CellItem {
			get {
				if(!isRowCustom)
					return source.CreateCellItem(cell.X, cell.Y);
				return null;
			}
		}
		internal Point Cell { get { return cell; } }
	}
	public class PivotChartDataSourceRowBaseListWrapper<TDerived> : UnsafeIListWrapper<TDerived, PivotChartDataSourceRowBase, PivotChartDataSourceRowBase> where TDerived : PivotChartDataSourceRowBase {
		public PivotChartDataSourceRowBaseListWrapper(IList<PivotChartDataSourceRowBase> sourceList)
			: base(sourceList) { }
	}
}
