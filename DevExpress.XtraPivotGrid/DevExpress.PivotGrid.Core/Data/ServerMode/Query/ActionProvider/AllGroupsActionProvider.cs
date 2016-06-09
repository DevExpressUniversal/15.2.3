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
using DevExpress.PivotGrid.ServerMode;
namespace DevExpress.PivotGrid.QueryMode {
	public class AllGroupsActionProvider : AllGroupsActionProvider<ServerModeColumn> {
		protected override List<QueryTuple> GetTuples(List<ServerModeColumn> area, AreaFieldValues fieldValues, GroupInfo[] groups) {
			int maxLevel = fieldValues.MaxLevel;
			if(area.Count <= maxLevel)
				throw new ArgumentException("invalid area list item count");
			if(maxLevel == -1)
				return new List<QueryTuple>();
			List<QueryTuple> result = new List<QueryTuple>();
			int levelCount = maxLevel + 1;
			List<QueryMember> lastMembers = new List<QueryMember>(levelCount);
			for(int i = 0; i < levelCount; i++)
				lastMembers.Add(null);
			QueryMember[] members = new QueryMember[levelCount];
			for(int i = maxLevel; i >= 0; i--)
				members[i] = area[i].TotalMember;
			result.Add(new QueryTuple(GroupInfo.GrandTotalGroup, members));
			members = new QueryMember[levelCount];
			for(int i = 0; i < fieldValues.Count; i++) {
				GroupInfo info = fieldValues[i];
				lastMembers[info.Level] = info.Member;
				for(int j = 0; j <= info.Level; j++)
					members[j] = lastMembers[j];
				for(int j = info.Level + 1; j < levelCount; j++)
					members[j] = area[j].TotalMember;
				result.Add(new QueryTuple(fieldValues[i], members));
			}
			return result;
		}
	}
}
