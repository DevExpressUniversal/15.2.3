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
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.OLAP.SchemaEntities {
	sealed class OlapLevel : OlapUniqueEntity, IOLAPLevel {
		readonly OlapHierarchy hierarchy;
		readonly string drillDownColumn;
		readonly int levelNumber;
		readonly long cardinality;
		readonly OLAPLevelType levelType;
		Dictionary<string, OLAPDataType> props;
		int keyCount;
		string sortProperty;
		List<CalculatedMemberSource> calculated;
		public OlapLevel(OlapHierarchy hierarchy, string name, string uniqueName, string caption, string description,
					string drillDownColumn, int number, long cardinality, int typeName, Dictionary<string, OLAPDataType> props, int keyCount, string sortProperty, List<CalculatedMemberSource> calculated)
			: base(name, uniqueName, caption, description) {
			this.hierarchy = hierarchy;
			this.drillDownColumn = drillDownColumn;
			this.levelNumber = number;
			this.cardinality = cardinality;
			this.props = props;
			this.levelType = (OLAPLevelType)typeName;
			this.keyCount = keyCount;
			this.sortProperty = sortProperty;
			this.calculated = calculated;
		}
		public OlapHierarchy ParentHierarchy { get { return hierarchy; } }
		#region IOLAPLevel Members
		string IOLAPLevel.DrillDownColumn {
			get { return this.drillDownColumn; }
		}
		int IOLAPLevel.LevelNumber {
			get { return this.levelNumber; }
		}
		OLAPLevelType IOLAPLevel.LevelType {
			get { return this.levelType; }
		}
		long IOLAPLevel.Cardinality {
			get { return this.cardinality; }
		}
		int IOLAPLevel.KeyCount {
			get { return keyCount; }
		}
		string IOLAPLevel.DefaultSortProperty {
			get { return sortProperty; }
		}
		Dictionary<string, OLAPDataType> IOLAPLevel.Properties {
			get { return props; }
		}
		List<CalculatedMemberSource> IOLAPLevel.CalculatedMembers {
			get { return calculated; }
		}
		#endregion
	}
}
