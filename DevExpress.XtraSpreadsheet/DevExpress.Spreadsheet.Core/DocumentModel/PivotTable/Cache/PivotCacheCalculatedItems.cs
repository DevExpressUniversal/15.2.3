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
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotCacheCalculatedItemCollection
	public class PivotCacheCalculatedItemCollection : List<PivotCacheCalculatedItem> {
	}
	#endregion
	#region PivotCacheCalculatedItem
	public class PivotCacheCalculatedItem {
		int field = -1;
		string formula; 
		PivotArea pivotArea;
		public PivotArea PivotArea { get { return pivotArea; } set { pivotArea = value; } }
		public int Field { get { return field; } set { field = value; } }
		public string Formula { get { return formula; } set { formula = value; } }
		public void CopyFrom(PivotCacheCalculatedItem source, DocumentModel anotherModel, Worksheet anotherSheet, CellPositionOffset rangeOffset) {
			this.field = source.field;
			this.formula = source.formula; 
			this.pivotArea.CopyFrom(source.pivotArea, anotherModel, anotherSheet, rangeOffset);
		}
	}
	#endregion
	#region PivotCacheCalculatedMemberCollection
	public class PivotCacheCalculatedMemberCollection : List<PivotCacheCalculatedMember> {
	}
	#endregion
	#region PivotCacheCalculatedMember
	public class PivotCacheCalculatedMember {
		string name;
		string mdx;
		string memberName;
		string hierarchy;
		string parent;
		int solveOrder;
		bool set;
		public string Name { get { return name; } set { name = value; } }
		public string Mdx { get { return mdx; } set { mdx = value; } }
		public string MemberName { get { return memberName; } set { memberName = value; } }
		public string Hierarchy { get { return hierarchy; } set { hierarchy = value; } }
		public string Parent { get { return parent; } set { parent = value; } }
		public int SolveOrder { get { return solveOrder; } set { solveOrder = value; } }
		public bool Set { get { return set; } set { set = value; } }
		public void CopyFrom(PivotCacheCalculatedMember source) {
			this.name = source.name;
			this.mdx = source.mdx;
			this.memberName = source.memberName;
			this.hierarchy = source.hierarchy;
			this.parent = source.parent;
			this.solveOrder = source.solveOrder;
			this.set = source.set;
		}
	}
	#endregion
}
