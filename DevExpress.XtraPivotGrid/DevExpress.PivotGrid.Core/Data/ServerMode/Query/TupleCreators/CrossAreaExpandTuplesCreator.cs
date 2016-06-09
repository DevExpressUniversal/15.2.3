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

using System.Collections;
using System.Collections.Generic;
using DevExpress.PivotGrid.QueryMode;
using System.Linq;
namespace DevExpress.PivotGrid.ServerMode {
	class CrossAreaExpandTuplesCreator : BaseTuplesCreator {
		List<QueryTuple> AreaTuples { get { return Context.GetTuples(IsColumn); } }
		public CrossAreaExpandTuplesCreator(IServerModeHelpersOwner helpersOwner, MultipleLevelsQueryExecutor executor, IServerQueryContext context, bool isColumn)
			: base(helpersOwner, executor, context, isColumn, false) {
		}
		protected override IEnumerable<AreaQueryContext> CreateTuplesCore() {
			List<QueryTuple> postColumnsTopNFilters = GetPostColumnsFilter(0);
			IList<ServerModeColumn> area = Area;
			Dictionary<int, IRawCriteriaConvertible> levelTopNFilter = new Dictionary<int, IRawCriteriaConvertible>();
			for(int i = 0; i < area.Count; i++) {
				List<QueryTuple> levelTuples = new List<QueryTuple>();
				for(int j = i; j < area.Count; j++)
					if(postColumnsTopNFilters[j] != null)
						levelTuples.Add(postColumnsTopNFilters[j]);
				levelTopNFilter[i - 1] = new InversedQueryTupleListToCriteriaOperatorConverter(levelTuples);
			}
			levelTopNFilter[area.Count - 1] = null;
			Dictionary<int, List<QueryTuple>> dic = new Dictionary<int, List<QueryTuple>>();
			foreach(QueryTuple tuple in AreaTuples) {
				if(!dic.ContainsKey(tuple.BaseGroup.Level + 1))
					dic.Add(tuple.BaseGroup.Level + 1, new List<QueryTuple>());
				dic[tuple.BaseGroup.Level + 1].Add(tuple);
			}
			if(dic.Values.Count > 0)
				foreach(KeyValuePair<int, List<QueryTuple>> pair in dic)
					foreach(AreaQueryContext item in EnumerateTuplesList(area, pair.Value, pair.Key - 1, pair.Value.Count > 0, levelTopNFilter[pair.Key - 1]))
						yield return item;
			else
				yield return CreateAreaContext((QueryTuple)null, -1);
		}
	}
}
