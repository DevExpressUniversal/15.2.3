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
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using DevExpress.Data.Filtering;
	public class EnumValueBox<T> : SimpleValueBox<T>, IEnumValueViewModel<T>, IFilterValueViewModel
		where T : struct {
		readonly static IEnumerable<T> UnsetValues = new T[] { };
		readonly static object valuesKey = new object();
		public virtual IEnumerable<T> Values {
			get { return GetValue<IEnumerable<T>>(valuesKey, UnsetValues); }
			set {
				if(TrySetValue(valuesKey, value, () => Values))
					OnValuesChanged();
			}
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			EnumProcessingHelper.RegisterEnum(EnumType);
		}
		protected void OnValuesChanged() {
			SetIsModified();
		}
		protected sealed override void ResetCore() {
			base.ResetCore();
			ResetValue(valuesKey, () => Values);
		}
		protected sealed override bool CanResetCore() {
			return base.CanResetCore() || HasValue(valuesKey);
		}
		protected IEnumChoiceMetricAttributes<T> MetricAttributes {
			get { return ((IEnumChoiceMetricAttributes<T>)MetricViewModel.Metric.Attributes); }
		}
		[Browsable(false)]
		public Type EnumType {
			get { return MetricAttributes.EnumType; }
		}
		[Browsable(false)]
		public bool UseFlags {
			get { return MetricAttributes.UseFlags; }
		}
		[Browsable(false)]
		public bool UseSelectAll {
			get { return MetricAttributes.UseSelectAll; }
		}
		[Browsable(false)]
		public string SelectAllName {
			get { return MetricAttributes.SelectAllName; }
		}
		[Browsable(false)]
		public string NullName {
			get { return MetricAttributes.NullName; }
		}
		CriteriaOperator IFilterValueViewModel.CreateFilterCriteria() {
			if(!IsModified)
				return null;
			var prop = new OperandProperty(MetricViewModel.Metric.Path);
			var isNull = new UnaryOperator(UnaryOperatorType.IsNull, prop);
			if(UseFlags && !HasValue(valuesKey)) {
				if(!Value.HasValue)
					return AllowNull ? isNull : null;
				else
					return GetBitwiseCriteria(prop, isNull, Value.Value);
			}
			if(!Values.Any())
				return null;
			if(Values.Count() == Enum.GetValues(EnumType).Length)
				return null;
			if(UseFlags) {
				T xorRes = (T)(object)Values
					.Cast<int>()
					.Aggregate(0, (s, f) => s | f);
				return GetBitwiseCriteria(prop, isNull, xorRes);
			}
			if(Values.Count() == 1) {
				var value = new OperandValue(Values.First());
				var equal = new BinaryOperator(prop, value, BinaryOperatorType.Equal);
				return AllowNull ? CriteriaOperator.Or(isNull, equal) : equal;
			}
			var inOp = new InOperator(prop, Values.Select(v => new OperandValue(v)));
			return AllowNull ? CriteriaOperator.Or(isNull, inOp) : inOp;
		}
		CriteriaOperator GetBitwiseCriteria(OperandProperty prop, UnaryOperator isNull, T xorRes) {
			var value = new OperandValue(xorRes);
			var bitwiseAnd = new BinaryOperator(prop, value, BinaryOperatorType.BitwiseAnd);
			var equal = new BinaryOperator(bitwiseAnd, value, BinaryOperatorType.Equal);
			return AllowNull ? CriteriaOperator.Or(isNull, equal) : equal;
		}
	}
}
