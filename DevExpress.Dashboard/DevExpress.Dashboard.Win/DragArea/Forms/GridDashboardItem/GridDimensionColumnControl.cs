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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class GridDimensionColumnControl : DashboardUserControl, IHolderOptionsControl {
		GridColumnDragSection section;
		int groupIndex;
		GridDashboardItem grid;
		GridColumnBase initialColumn;
		double fixedWidth;
		double weight;
		GridColumnFixedWidthType widthType;
		DataItem dataItem;
		GridDimensionColumn column;
		GridDimensionColumnDisplayMode displayMode = GridDimensionColumnDisplayMode.Text;
		public bool Active { get { return column != null; } }
		public bool AutoMode { get; set; }
		public event EventHandler OptionsChanged;
		public GridDimensionColumnControl() {
			InitializeComponent();
		}
		IHistoryItem IHolderOptionsControl.CreateHistoryItem() {
			GridDimensionColumn newColumn = AutoMode ? null : new GridDimensionColumn() {
				Dimension = DashboardWinHelper.CloneToDimension(dataItem),
				DisplayMode = displayMode,
				FixedWidth = fixedWidth,
				WidthType = widthType,
				Weight = weight
			};
			return new CreateGridColumnHistoryItem(section, groupIndex, grid, initialColumn, newColumn);
		}
		void RaiseOptionsChanged() {
			if(OptionsChanged != null)
				OptionsChanged(this, EventArgs.Empty);
		}
		void TextCheckEditCheckedChanged(object sender, EventArgs e) {
			if(textCheckEdit.Checked)
				displayMode = GridDimensionColumnDisplayMode.Text;
			RaiseOptionsChanged();
		}
		void ImageCheckEditCheckedChanged(object sender, EventArgs e) {
			if(imageCheckEdit.Checked)
				displayMode = GridDimensionColumnDisplayMode.Image;
			RaiseOptionsChanged();
		}
		public void Initialize(GridColumnDragSection section, int groupIndex, GridDashboardItem grid, GridColumnBase initialColumn, bool allowImageMode) {
			this.section = section;
			this.groupIndex = groupIndex;
			this.grid = grid;
			this.initialColumn = initialColumn;
			this.fixedWidth = initialColumn.FixedWidth;
			this.widthType = initialColumn.WidthType;
			this.weight = initialColumn.Weight;
			dataItem = initialColumn.DataItems.FirstOrDefault();
			showLabel.Visible = textCheckEdit.Visible = imageCheckEdit.Visible = allowImageMode;
			noOptionsLabel.Visible = !allowImageMode;
			column = initialColumn as GridDimensionColumn;
			if(column != null)
				imageCheckEdit.Checked = column.DisplayMode == GridDimensionColumnDisplayMode.Image;
		}
	}
}
