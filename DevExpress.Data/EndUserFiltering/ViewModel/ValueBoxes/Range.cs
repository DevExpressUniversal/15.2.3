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

namespace DevExpress.Utils.Filtering.Internal {
	using System.ComponentModel;
	using DevExpress.Data.Filtering;
	public class RangeValueBox<T> : ValueViewModel, IRangeValueViewModel<T>,
		IFilterValueViewModel
		where T : struct {
		public virtual T? FromValue {
			get;
			set;
		}
		public virtual T? ToValue {
			get;
			set;
		}
		protected virtual void OnFromValueChanged() {
			SetIsModified();
		}
		protected virtual void OnToValueChanged() {
			SetIsModified();
		}
		protected IRangeMetricAttributes<T> MetricAttributes {
			get { return ((IRangeMetricAttributes<T>)MetricViewModel.Metric.Attributes); }
		}
		protected override void OnMetricAttributesChanged(string propertyName) {
			base.OnMetricAttributesChanged(propertyName);
			if(propertyName == "Minimum") 
				FromValue = Minimum;
			if(propertyName == "Maximum")
				ToValue = Maximum;
		}
		[Browsable(false)]
		public bool AllowNull {
			get { return TypeHelper.IsNullable(MetricViewModel.Metric.Type); }
		}
		[Browsable(false)]
		public T? Minimum {
			get { return MetricAttributes.Minimum; }
		}
		[Browsable(false)]
		public T? Maximum {
			get { return MetricAttributes.Maximum; }
		}
		[Browsable(false)]
		public virtual T? Average {
			get { return MetricAttributes.Average; }
		}
		[Browsable(false)]
		public string FromName {
			get { return MetricAttributes.FromName; }
		}
		[Browsable(false)]
		public string ToName {
			get { return MetricAttributes.ToName; }
		}
		protected sealed override void ResetCore() {
			FromValue = Minimum;
			ToValue = Maximum;
		}
		protected sealed override bool CanResetCore() {
			return
				!Equals(FromValue, Minimum) ||
				!Equals(ToValue, Maximum);
		}
		CriteriaOperator IFilterValueViewModel.CreateFilterCriteria() {
			if(!IsModified)
				return null;
			var fromCriteria = GetCriteria(FromValue, BinaryOperatorType.GreaterOrEqual);
			var toCriteria = GetCriteria(ToValue, BinaryOperatorType.LessOrEqual);
			if(object.ReferenceEquals(fromCriteria, null))
				return toCriteria;
			if(object.ReferenceEquals(toCriteria, null))
				return fromCriteria;
			if(toCriteria is UnaryOperator && fromCriteria is UnaryOperator)
				return new UnaryOperator(UnaryOperatorType.IsNull, MetricViewModel.Metric.Path);
			if(toCriteria is UnaryOperator || fromCriteria is UnaryOperator)
				return CriteriaOperator.Or(fromCriteria, toCriteria);
			if(Equals(FromValue, Minimum) && Equals(ToValue, Maximum))
				return null;
			return CriteriaOperator.And(fromCriteria, toCriteria);
		}
		CriteriaOperator GetCriteria(T? value, BinaryOperatorType operatorType) {
			if(!AllowNull && !value.HasValue)
				return null;
			string path = MetricViewModel.Metric.Path;
			if(value.HasValue)
				return new BinaryOperator(path, value, operatorType);
			else
				return new UnaryOperator(UnaryOperatorType.IsNull, path);
		}
	}
}
