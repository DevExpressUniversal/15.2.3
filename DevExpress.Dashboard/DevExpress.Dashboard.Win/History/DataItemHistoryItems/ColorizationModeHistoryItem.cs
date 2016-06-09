#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.DashboardWin.Native {
	public class DimensionColoringModeHistoryItem : DataItemHistoryItem {
		readonly ColoringMode? coloringMode;
		ColoringMode previousColoringMode;
		public override string Caption {
			get {
				return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.HistoryItemDimensionColoringMode), DataItem.DisplayName); 
			}
		}
		public DimensionColoringModeHistoryItem(DataDashboardItem dataDashboardItem, Dimension dimension, ColoringMode coloringMode)
			: base(dataDashboardItem, dimension) {
			this.coloringMode = coloringMode;
		}
		protected override void PerformUndo() {
			Dimension dimension = (Dimension)DataItem;
			dimension.ColoringMode = previousColoringMode;
		}
		protected override void PerformRedo() {
			Dimension dimension = (Dimension)DataItem;
			previousColoringMode = dimension.ColoringMode;
			dimension.ColoringMode = coloringMode ?? Dimension.DefaultColoringMode;
		}
	}
}
