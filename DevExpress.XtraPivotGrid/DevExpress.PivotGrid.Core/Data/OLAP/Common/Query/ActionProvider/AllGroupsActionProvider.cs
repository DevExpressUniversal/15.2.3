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
using DevExpress.PivotGrid.QueryMode;
using DevExpress.PivotGrid.QueryMode.TuplesTree;
namespace DevExpress.PivotGrid.OLAP {
	public class AllGroupsActionProvider : AllGroupsActionProvider<OLAPCubeColumn> {
		protected override List<QueryTuple> GetTuples(List<OLAPCubeColumn> area, AreaFieldValues fieldValues, GroupInfo[] groups) {
			int maxLevel = fieldValues.MaxLevel;
			if(area.Count <= maxLevel)
				throw new ArgumentException("invalid area list item count");
			if(maxLevel == -1)
				return new List<QueryTuple>();
			List<QueryTuple> result = new List<QueryTuple>();
			List<QueryMember> lastMembers = new List<QueryMember>(maxLevel + 1);
			List<int> levelMap = new List<int>(maxLevel + 1);
			int tupleIndex = -1;
			for(int i = 0; i < maxLevel + 1; i++) {
				lastMembers.Add(null);
				if(i == 0 || !area[i - 1].IsParent(area[i]))
					tupleIndex++;
				levelMap.Add(tupleIndex);
			}
			QueryMember[] members = new QueryMember[tupleIndex + 1];
			for(int i = maxLevel; i >= 0; i--)
				members[levelMap[i]] = area[i].TotalMember;
			QueryTuple gt = new QueryTuple(GroupInfo.GrandTotalGroup, members);
			result.Add(gt);
			for(int i = 0; i < fieldValues.Count; i++) {
				if(lastMembers[fieldValues[i].Level] != fieldValues[i].Member) {
					lastMembers[fieldValues[i].Level] = fieldValues[i].Member;
					for(int j = fieldValues[i].Level + 1; j < lastMembers.Count; j++)
						lastMembers[j] = area[j].TotalMember;
				}
				for(int j = 0; j < members.Length; j++)
					members[j] = null;
				for(int j = 0; j < lastMembers.Count; j++) {
					if(members[levelMap[j]] == null || !lastMembers[j].IsTotal)
						members[levelMap[j]] = lastMembers[j];
				}
				result.Add(new QueryTuple(fieldValues[i], members));
			}
			return result;
		}
	}
}
