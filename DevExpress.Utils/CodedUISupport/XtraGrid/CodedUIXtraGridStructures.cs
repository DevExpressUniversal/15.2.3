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
namespace DevExpress.Utils.CodedUISupport {
	[Serializable]
	public struct GridElementInfo {
		public GridControlElements ElementType;
		public GridControlViews ViewType;
		public int RowHandle;
		public string ColumnName;
		public string ViewName;
	}
	public enum GridElementProperties {
		Default,
		Appearance,
		Value,
		Text,
		ErrorText
	}
	public enum GridControlViews : int {
		Undefined,
		GridView,
		BandedGridView,
		AdvBandedGridView,
		CardView,
		LayoutView
	}
	public enum GridControlElements : int {
		Unknown = 0,
		Cell,
		GroupPanel,
		ColumnHeader,
		BandedColumnHeader,
		Row,
		RowIndicator,
		RowPreview,
		RowGroupButton,
		GroupPanelColumn,
		GroupPanelBandedColumn,
		ColumnEdge,
		FilterPanelActiveButton,
		FilterPanelCustomizeButton,
		FilterPanel,
		FilterPanelCloseButton,
		FilterPanelMRUButton,
		FilterPanelText,
		GroupRow,
		Band,
		BandEdge,
		FixedLeftDiv,
		FixedRightDiv,
		ColumnFilterButton,
		RowFooter,
		RowFooterCell,
		Footer,
		FooterCell,
		GroupPanelColumnFilterButton,
		CellButton,
		MasterTabPageHeader
	}
}
