#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.XtraPivotGrid;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.DataProcessing {
	public abstract class DataModelBase<T> : IEquatable<T> where T : DataModelBase<T> {
		protected abstract int ModelHashCode();
		protected abstract bool ModelEquals(T other);
		public override bool Equals(object other) {
			return (other != null) && ModelEquals((T)other);
		}
		public override int GetHashCode() {
			return ModelHashCode();
		}
		public bool Equals(T other) {
			return (other != null) && ModelEquals(other);
		}
		public static bool operator ==(DataModelBase<T> model1, DataModelBase<T> model2) {
			bool bothNull = object.ReferenceEquals(model1, null) && object.ReferenceEquals(model2, null);
			if(bothNull)
				return true;
			else
				return !object.ReferenceEquals(model1, null) && model1.Equals((T)model2);
		}
		public static bool operator !=(DataModelBase<T> model1, DataModelBase<T> model2) {
			return !(model1 == model2);
		}
	}
	public class DimensionModel : DataItemModel<DimensionModel> {
		public override ExpressionMode UnboundMode {
			get { return ExpressionMode.DataSourceLevel; }
			set { throw new InvalidOperationException(); }
		}
		public PivotGroupInterval GroupInterval { get; set; }
		public DimensionModel(string dataMember, string name) : base(dataMember, name) {
		}
		protected override int ModelHashCode() {
			return HashcodeHelper.GetCompositeHashCode<object>(Name, DataMember, UnboundExpression, UnboundType, UnboundMode, GroupInterval, DrillDownName);
		}
		protected override bool ModelEquals(DimensionModel other) {
			return base.ModelEquals(other)
				&& Name == other.Name
				&& DataMember == other.DataMember
				&& UnboundExpression == other.UnboundExpression
				&& UnboundType == other.UnboundType
				&& UnboundMode == other.UnboundMode
				&& GroupInterval == other.GroupInterval;
		}
		public override string ToString() {
			return Name;
		}
	}
	public class DimensionSortByMeasureModel : DataModelBase<DimensionSortByMeasureModel> {
		internal static bool Equal(DimensionSortByMeasureModel a, DimensionSortByMeasureModel b) {
			if((a == null) != (b == null))
				return false;
			if(a == null)
				return true;
			if(a.Measure != b.Measure)
				return false;
			if((a.Dimensions == null) != (b.Dimensions == null))
				return false;
			if(a.Dimensions == null)
				return true;
			return a.Dimensions.Zip(b.Dimensions, (x, y) => x.Equals(y)).All(z => z);
		}
		IList<DimensionModel> dimensions;
		readonly MeasureModel measure;
		public IList<DimensionModel> Dimensions {
			get { return dimensions; }
			set { dimensions = value ?? new List<DimensionModel>(); }
		}
		public MeasureModel Measure {
			get { return measure; }
		}
		public DimensionSortByMeasureModel(MeasureModel measure)
			: this(measure, null) {
		}
		public DimensionSortByMeasureModel(MeasureModel measure, IEnumerable<DimensionModel> dimensions) {
			Guard.ArgumentNotNull(measure, "measure");
			this.measure = measure;
			this.Dimensions = dimensions == null ? new List<DimensionModel>() : dimensions.ToList();
		}
		protected override bool ModelEquals(DimensionSortByMeasureModel other) {
			return Equal(this, other);
		}
		protected override int ModelHashCode() {
			throw new NotSupportedException();
		}
	}
	public class DimensionSortModel : DataModelBase<DimensionSortModel> {
		public DimensionModel SortedDimension { get; private set; }
		public DimensionSortByMeasureModel SortByMeasure { get; set; }
		public PivotSortMode SortMode { get; set; }
		public PivotSortOrder SortOrder { get; set; }
		public DimensionSortModel(DimensionModel dimensionModel) {
			DXContract.Requires(dimensionModel != null);
			this.SortedDimension  = dimensionModel;
		}
		protected override int ModelHashCode() {
			return HashcodeHelper.GetCompositeHashCode<object>(SortByMeasure == null ? null : SortByMeasure.Measure, SortMode, SortOrder);
		}
		protected override bool ModelEquals(DimensionSortModel other) {
			return SortedDimension == other.SortedDimension
				&& DimensionSortByMeasureModel.Equal(SortByMeasure, other.SortByMeasure)
				&& SortMode == other.SortMode
				&& SortOrder == other.SortOrder;
		}
	}
	public class DimensionTopNModel : DataModelBase<DimensionTopNModel> {
		public DimensionModel DimensionModel { get; private set; }
		public bool TopNEnabled { get; set; }
		public int TopNCount { get; set; }
		public bool TopNShowOthers { get; set; }
		public MeasureModel TopNMeasure { get; set; }
		public PivotSortOrder TopNDirection { get; set; }
		public IEnumerable<DimensionModel> TopNSubgroup { get; set; }
		public DimensionTopNModel(DimensionModel dimensionModel) {
			DXContract.Requires(dimensionModel != null);
			this.DimensionModel = dimensionModel;
			this.TopNSubgroup = new DimensionModel[0];
		}
		protected override int ModelHashCode() {
			int hash = HashcodeHelper.GetCompositeHashCode<object>(TopNEnabled, TopNCount, TopNShowOthers, TopNMeasure, TopNDirection);
			return hash ^ HashcodeHelper.GetCompositeHashCode(TopNSubgroup);
		}
		protected override bool ModelEquals(DimensionTopNModel other) {
			return DimensionModel == other.DimensionModel
				&& TopNEnabled == other.TopNEnabled
				&& TopNCount == other.TopNCount
				&& TopNShowOthers == other.TopNShowOthers
				&& TopNMeasure == other.TopNMeasure
				&& TopNDirection == other.TopNDirection
				&& TopNSubgroup.SequenceEqual(other.TopNSubgroup);
		}
	}
}
