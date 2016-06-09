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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.XtraDashboardLayout;
using DevExpress.XtraLayout.Resizing;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraLayout.Helpers {
	internal static class TableLayoutHelper {
		static LayoutControlGroup GetLayoutGroup(OptionsTableLayoutGroup OptionsTableLayout, LayoutType ltype) {
			int size = 10;
			LayoutControlGroup group = new LayoutControlGroup();
			group.Padding = group.Spacing = Padding.Empty;
			group.GroupBordersVisible = false;
			BaseItemCollection collection = new BaseItemCollection();
			if(ltype == LayoutType.Horizontal) {
				for(int j = 0; j < OptionsTableLayout.ColumnCount; j++) {
					EmptySpaceItem emptySpace = new EmptySpaceItem();
					emptySpace.SizeConstraintsType = SizeConstraintsType.Custom;
					emptySpace.SetBounds(new Rectangle(j * size, 0, size, size));
					collection.Add(emptySpace);
				}
			} else {
				for(int i = 0; i < OptionsTableLayout.RowCount; i++) {		 
					EmptySpaceItem emptySpace = new EmptySpaceItem();	 
					emptySpace.SizeConstraintsType = SizeConstraintsType.Custom;
					emptySpace.SetBounds(new Rectangle(0, i * size, size, size));
					collection.Add(emptySpace);
				}
			}
			group.Items.AddRange(collection.ToArray());
			return group;
		}
		internal static LayoutControlItem GetItemFromColumn(Resizer tableLayoutManager, ColumnDefinition columnDefinition, OptionsTableLayoutGroup OptionsTableLayout) {
			int index = OptionsTableLayout.ColumnDefinitions.IndexOf(columnDefinition);
			return tableLayoutManager.GroupForTable.GetItem(index) as LayoutControlItem;
		}
		internal static LayoutControlItem GetItemFromRow(Resizer tableLayoutManager, RowDefinition rowDefinition, OptionsTableLayoutGroup OptionsTableLayout) {
			int index = OptionsTableLayout.RowDefinitions.IndexOf(rowDefinition);
			return tableLayoutManager.GroupForTable.GetItem(index) as LayoutControlItem;
		}
		internal static Resizer CreateFakeTableResizer(OptionsTableLayoutGroup OptionsTableLayout, Size groupSize,LayoutType lType) {
			Resizer resizer = new Resizer(GetLayoutGroup(OptionsTableLayout, lType));
			resizer.GroupForTable.Size = resizer.GroupForTable.ViewInfo.AddLabelIndentions(groupSize);
			return resizer;
		}
	}
}
