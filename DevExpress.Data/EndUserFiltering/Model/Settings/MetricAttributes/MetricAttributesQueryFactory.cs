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
	public interface IMetricAttributesQuery {
		void QueryValues(IDictionary<string, object> values);
	}
	public interface IMetricAttributesQueryFactory {
		IMetricAttributesQuery CreateQuery(IEndUserFilteringMetric metric, IMetricAttributesQueryOwner owner);
	}
	public interface IMetricAttributesQueryOwner {
		void RaiseMetricAttributesQuery<TEventArgs, TData>(TEventArgs e)
			where TEventArgs : QueryDataEventArgs<TData>
			where TData : MetricAttributesData;
	}
	sealed class DefaultMetricAttributesQueryFactory : IMetricAttributesQueryFactory {
		internal static readonly IMetricAttributesQueryFactory Instance = new DefaultMetricAttributesQueryFactory();
		DefaultMetricAttributesQueryFactory() { }
		IDictionary<Type, Func<IEndUserFilteringMetric, IMetricAttributesQueryOwner, IMetricAttributesQuery>> initializers =
			new Dictionary<Type, Func<IEndUserFilteringMetric, IMetricAttributesQueryOwner, IMetricAttributesQuery>>() { 
			{ typeof(IRangeMetricAttributes<>), (metric, owner) => new RangeMetricAttributesQuery(metric, owner) },
			{ typeof(ILookupMetricAttributes<>), (metric, owner) => new LookupMetricAttributesQuery(metric,owner) },
			{ typeof(IChoiceMetricAttributes<>), (metric, owner) => new BooleanChoiceMetricAttributesQuery(metric,owner) },
		};
		IMetricAttributesQuery IMetricAttributesQueryFactory.CreateQuery(IEndUserFilteringMetric metric, IMetricAttributesQueryOwner owner) {
			Func<IEndUserFilteringMetric, IMetricAttributesQueryOwner, IMetricAttributesQuery> initializer;
			if(initializers.TryGetValue(metric.AttributesTypeDefinition, out initializer))
				return initializer(metric, owner);
			return EmptyMetricAttributesQuery.Instance;
		}
		#region Queries
		class EmptyMetricAttributesQuery : IMetricAttributesQuery {
			internal static readonly IMetricAttributesQuery Instance = new EmptyMetricAttributesQuery();
			EmptyMetricAttributesQuery() { }
			public void QueryValues(IDictionary<string, object> memberValues) { }
		}
		abstract class BaseMetricAttributesQuery : IMetricAttributesQuery {
			readonly IEndUserFilteringMetric metricCore;
			IMetricAttributesQueryOwner ownerCore;
			protected BaseMetricAttributesQuery(IEndUserFilteringMetric metric, IMetricAttributesQueryOwner owner) {
				this.metricCore = metric;
				this.ownerCore = owner;
			}
			protected string Path {
				get { return metricCore.Path; }
			}
			protected IMetricAttributesQueryOwner Owner {
				get { return ownerCore; }
			}
			public abstract void QueryValues(IDictionary<string, object> memberValues);
		}
		class RangeMetricAttributesQuery : BaseMetricAttributesQuery {
			public RangeMetricAttributesQuery(IEndUserFilteringMetric metric, IMetricAttributesQueryOwner owner)
				: base(metric, owner) {
			}
			public override void QueryValues(IDictionary<string, object> memberValues) {
				QueryRangeDataEventArgs args = new QueryRangeDataEventArgs(Path, memberValues);
				Owner.RaiseMetricAttributesQuery<QueryRangeDataEventArgs, RangeData>(args);
			}
		}
		class LookupMetricAttributesQuery : BaseMetricAttributesQuery {
			public LookupMetricAttributesQuery(IEndUserFilteringMetric metric, IMetricAttributesQueryOwner owner)
				: base(metric, owner) {
			}
			public override void QueryValues(IDictionary<string, object> memberValues) {
				QueryLookupDataEventArgs args = new QueryLookupDataEventArgs(Path, memberValues);
				Owner.RaiseMetricAttributesQuery<QueryLookupDataEventArgs, LookupData>(args);
			}
		}
		class BooleanChoiceMetricAttributesQuery : BaseMetricAttributesQuery {
			public BooleanChoiceMetricAttributesQuery(IEndUserFilteringMetric metric, IMetricAttributesQueryOwner owner)
				: base(metric, owner) {
			}
			public override void QueryValues(IDictionary<string, object> memberValues) {
				QueryBooleanChoiceDataEventArgs args = new QueryBooleanChoiceDataEventArgs(Path, memberValues);
				Owner.RaiseMetricAttributesQuery<QueryBooleanChoiceDataEventArgs, BooleanChoiceData>(args);
			}
		}
		#endregion Queries
	}
}
