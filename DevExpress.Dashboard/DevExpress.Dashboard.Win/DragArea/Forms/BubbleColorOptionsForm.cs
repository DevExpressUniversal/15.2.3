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
using System.Linq;
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Native {
	public partial class BubbleColorOptionsForm : DashboardForm {
		readonly DashboardDesigner designer;
		readonly BubbleMapDashboardItem mapDashboardItem;
		IHistoryItem historyItem;
		public BubbleColorOptionsForm() {
			InitializeComponent();
			valueMapControl.OnChanged += OnValueMapControlChanged;
		}
		public BubbleColorOptionsForm(DashboardDesigner designer, BubbleMapDashboardItem mapDashboardItem)
			: this() {
			this.designer = designer;
			this.mapDashboardItem = mapDashboardItem;
			valueMapControl.InitializeFrom(mapDashboardItem.ColorPalette, mapDashboardItem.ColorScale);
		}
		void OnValueMapControlChanged(object sender, EventArgs e) {
			btnApply.Enabled = true;
		}
		void OnButtonOKClick(object sender, EventArgs e) {
			if(btnApply.Enabled)
				ApplyChanges();
			AddHistoryItem();
			DialogResult =  System.Windows.Forms.DialogResult.OK;
		}
		void OnButtonApplyClick(object sender, EventArgs e) {
			ApplyChanges();
			valueMapControl.InitializeFrom(mapDashboardItem.ColorPalette, mapDashboardItem.ColorScale);
			btnApply.Enabled = false;
		}
		void ApplyChanges() {
			historyItem = new BubbleMapOptionsHistoryItem(mapDashboardItem, valueMapControl.GetPalette(), valueMapControl.GetScale());
			historyItem.Redo(designer);
		}
		void AddHistoryItem() {
			if(historyItem != null)
				designer.History.Add(historyItem);
		}
	}
}
