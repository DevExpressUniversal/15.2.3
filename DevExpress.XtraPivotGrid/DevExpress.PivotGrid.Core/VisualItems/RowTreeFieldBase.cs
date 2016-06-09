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
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotTreeRowFieldBase : PivotGridFieldBase {
		public static int GetAreaIndex(PivotVisualItemsBase visualItems) {
			return GetAreaIndex(visualItems, true);
		}
		public static int GetAreaIndex(PivotVisualItemsBase visualItems, bool useFieldCollections) {
			int res = visualItems.GetMaxExpandedLevel(false);
			res = Math.Min(res, visualItems.Data.GetFieldCountByArea(PivotArea.RowArea, useFieldCollections) - 1);
			res = Math.Max(res, 0);
			return res;
		}
		public static int GetWidth(PivotVisualItemsBase visualItems) {
			return visualItems.RowTreeLevelWidth;
		}
		public static void SetWidth(PivotVisualItemsBase visualItems, int value) {
			visualItems.RowTreeLevelWidth = value;
		}
		PivotVisualItemsBase visualItems;
		public PivotTreeRowFieldBase(PivotVisualItemsBase visualItems) {
			this.visualItems = visualItems;
			Area = PivotArea.RowArea;
		}
		protected PivotVisualItemsBase VisualItems { get { return visualItems; } }
		public override int AreaIndex {
			get {
				return GetAreaIndex(VisualItems);
			}
			set { }
		}
		public override int Width {
			get { return GetWidth(VisualItems); }
			set { SetWidth(VisualItems, value); }
		}
	}
}
