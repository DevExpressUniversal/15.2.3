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

namespace DevExpress.Utils.Filtering {
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	public abstract class QueryDataEventArgs<TData> : EventArgs
		where TData : MetricAttributesData {
		readonly TData resultCore;
		protected QueryDataEventArgs(string propertyPath, IDictionary<string, object> memberValues) {
			this.PropertyPath = propertyPath;
			resultCore = CreateData(memberValues);
		}
		protected abstract TData CreateData(IDictionary<string, object> memberValues);
		public string PropertyPath {
			get;
			private set;
		}
		public TData Result {
			get { return resultCore; }
		}
	}
	public delegate void QueryDataEventHandler<TEventArgs, TData>(
		object sender, TEventArgs e)
		where TEventArgs : QueryDataEventArgs<TData>
		where TData : MetricAttributesData;
	public abstract class MetricAttributesData {
		IDictionary<string, object> memberValues;
		protected MetricAttributesData(IDictionary<string, object> memberValues) {
			this.memberValues = memberValues;
		}
		protected TValue GetValue<TValue>(Expression<Func<TValue>> expression) {
			return GetValue<TValue>(Internal.ExpressionHelper.GetPropertyName(expression));
		}
		protected void SetValue<TValue>(Expression<Func<TValue>> expression, object value) {
			SetValue(Internal.ExpressionHelper.GetPropertyName(expression), value);
		}
		TValue GetValue<TValue>(string memberName) {
			object value;
			return memberValues.TryGetValue(memberName, out value) ? (value is TValue ? (TValue)value : default(TValue)) : default(TValue);
		}
		void SetValue<TValue>(string memberName, TValue value) {
			if(!memberValues.ContainsKey(memberName))
				memberValues.Add(memberName, value);
			else
				memberValues[memberName] = value;
		}
	}
	public class RangeData : MetricAttributesData {
		internal RangeData(IDictionary<string, object> memberValues)
			: base(memberValues) {
		}
		public object Minimum {
			get { return GetValue(() => Minimum); }
			set { SetValue(() => Minimum, value); }
		}
		public object Maximum {
			get { return GetValue(() => Maximum); }
			set { SetValue(() => Maximum, value); }
		}
		public object Average {
			get { return GetValue(() => Average); }
			set { SetValue(() => Average, value); }
		}
	}
	public class QueryRangeDataEventArgs : QueryDataEventArgs<RangeData> {
		internal QueryRangeDataEventArgs(string propertyPath, IDictionary<string, object> memberValues)
			: base(propertyPath, memberValues) {
		}
		protected sealed override RangeData CreateData(IDictionary<string, object> memberValues) {
			return new RangeData(memberValues);
		}
	}
	public class LookupData : MetricAttributesData {
		internal LookupData(IDictionary<string, object> memberValues)
			: base(memberValues) {
		}
		public object DataSource {
			get { return GetValue(() => DataSource); }
			set { SetValue(() => DataSource, value); }
		}
		public int Top {
			get { return GetValue(() => Top); }
			set { SetValue(() => Top, value); }
		}
		public int MaxCount {
			get { return GetValue(() => MaxCount); }
			set { SetValue(() => MaxCount, value); }
		}
	}
	public class QueryLookupDataEventArgs : QueryDataEventArgs<LookupData> {
		internal QueryLookupDataEventArgs(string propertyPath, IDictionary<string, object> memberValues)
			: base(propertyPath, memberValues) {
		}
		protected sealed override LookupData CreateData(IDictionary<string, object> memberValues) {
			return new LookupData(memberValues);
		}
	}
	public class BooleanChoiceData : MetricAttributesData {
		internal BooleanChoiceData(IDictionary<string, object> memberValues)
			: base(memberValues) {
		}
		public bool DefaultValue {
			get { return GetValue(() => DefaultValue); }
			set { SetValue(() => DefaultValue, value); }
		}
	}
	public class QueryBooleanChoiceDataEventArgs : QueryDataEventArgs<BooleanChoiceData> {
		internal QueryBooleanChoiceDataEventArgs(string propertyPath, IDictionary<string, object> memberValues)
			: base(propertyPath, memberValues) {
		}
		protected sealed override BooleanChoiceData CreateData(IDictionary<string, object> memberValues) {
			return new BooleanChoiceData(memberValues);
		}
	}
}
