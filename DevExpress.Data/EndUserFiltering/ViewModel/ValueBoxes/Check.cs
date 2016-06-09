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
	public class BooleanValueBox<T> : SimpleValueBox<bool>, IBooleanValueViewModel,
		IFilterValueViewModel {
		protected IBooleanChoiceMetricAttributes MetricAttributes {
			get { return ((IBooleanChoiceMetricAttributes)MetricViewModel.Metric.Attributes); }
		}
		public override bool? Value {
			get {
				if(!DefaultValue.HasValue)
					return base.Value;
				return base.Value.GetValueOrDefault(DefaultValue.Value);
			}
			set {
				if(DefaultValue.HasValue && Equals(value, DefaultValue))
					value = null;
				base.Value = value;
			}
		}
		[Browsable(false)]
		public string TrueName {
			get { return MetricAttributes.TrueName; }
		}
		[Browsable(false)]
		public string FalseName {
			get { return MetricAttributes.FalseName; }
		}
		[Browsable(false)]
		public string DefaultName {
			get { return MetricAttributes.DefaultName; }
		}
		[Browsable(false)]
		public bool? DefaultValue {
			get { return MetricAttributes.DefaultValue; }
		}
		CriteriaOperator IFilterValueViewModel.CreateFilterCriteria() {
			if(!IsModified)
				return null;
			if(!AllowNull && !Value.HasValue)
				return null;
			string path = MetricViewModel.Metric.Path;
			if(Value.HasValue) {
				if(DefaultValue.HasValue && Equals(DefaultValue, Value))
					return null;
				return new BinaryOperator(path, Value);
			}
			else return new UnaryOperator(UnaryOperatorType.IsNull, path);
		}
	}
}
