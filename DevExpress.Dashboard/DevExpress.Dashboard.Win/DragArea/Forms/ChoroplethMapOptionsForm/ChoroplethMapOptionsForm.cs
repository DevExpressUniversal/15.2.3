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
using System.Linq;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Native {
	public partial class ChoroplethMapOptionsForm : DashboardForm {
		DashboardDesigner designer;
		ChoroplethMapDashboardItem mapDashboardItem;
		ChoroplethMap initialMap;
		ChoroplethMapDragSection section;
		IHistoryItem historyItem;
		int groupIndex;
		HolderCollectionDragGroup<ChoroplethMap> Group { get { return groupIndex >= 0 ? (HolderCollectionDragGroup<ChoroplethMap>)section.Groups[groupIndex] : (HolderCollectionDragGroup<ChoroplethMap>)section.NewGroup; } }
		public ChoroplethMapOptionsForm() {
			InitializeComponent();
		}
		public ChoroplethMapOptionsForm(HolderCollectionDragGroup<ChoroplethMap> group, DashboardDesigner designer, ChoroplethMapDashboardItem mapDashboardItem)
			: this() {
			this.designer = designer;
			this.mapDashboardItem = mapDashboardItem;
			section =  (ChoroplethMapDragSection)group.Section;
			groupIndex = group.IsNewGroup ? -1 : group.GroupIndex;
			valueMapControl.OnChanged += (sender, e) => { OnChanged(); };
			deltaMapControl.MapOptionsForm = this;
			InitializeControls(group.Holder);
			btnApply.Enabled = false;
			UpdateUI();
		}
		public void OnChanged() {
			btnApply.Enabled = true;
		}
		void InitializeControls(ChoroplethMap choroplethMap) {
			ValueMap valueMap = choroplethMap as ValueMap;
			if (valueMap != null)
				valueMapControl.InitializeFrom(valueMap.Palette, valueMap.Scale);
			valueMapCheckEdit.Checked = valueMap != null;
			DeltaMap deltaMap  = choroplethMap as DeltaMap;
			if(deltaMap != null)
				deltaMapControl.InitializeFrom(deltaMap);
			deltaCheckEdit.Checked = deltaMap != null;
			initialMap = choroplethMap;
		}
		void colorizerCheckEdit_CheckedChanged(object sender, EventArgs e) {
			MapTypeChanged();
		}
		void deltaCheckEdit_CheckedChanged(object sender, EventArgs e) {
			MapTypeChanged();
		}
		void MapTypeChanged() {
			btnApply.Enabled = true;
			UpdateUI();
		}
		void UpdateUI() {
			int padding = 35;
			this.Width = valueMapCheckEdit.Checked ? valueMapControl.Right + padding : deltaMapControl.Right + padding;
			buttonsPanel.Left = Width - buttonsPanel.Width;
			valueMapControl.Visible = valueMapCheckEdit.Checked;
			deltaMapControl.Visible = deltaCheckEdit.Checked;
		}
		void ApplyChanges() {
			historyItem = GetHistoryItem();
			historyItem.Redo(designer);
		}
		void AddHistoryItem() {
			if(historyItem != null)
				designer.History.Add(historyItem);
		}
		void btnApply_Click(object sender, EventArgs e) {
			ApplyChanges();
			InitializeControls(Group.Holder);
			btnApply.Enabled = false;
		}
		void btnOK_Click(object sender, EventArgs e) {
			if(btnApply.Enabled)
				ApplyChanges();
			AddHistoryItem();
			DialogResult = DialogResult.OK;
		}
		void btnCancel_Click(object sender, EventArgs e) {
			AddHistoryItem();
			DialogResult = DialogResult.Cancel;
		}
		IHistoryItem GetHistoryItem() {
			Measure measure = initialMap.DataItems.FirstOrDefault() as Measure;
			if(valueMapCheckEdit.Checked) {
				ValueMap valueMap = initialMap as ValueMap;
				if(valueMap != null)
					return new ChangeValueMapHistoryItem(mapDashboardItem, valueMap, valueMapControl.GetPalette(), valueMapControl.GetScale());
				valueMap = new ValueMap();
				valueMap.Value = measure;
				valueMap.Scale = valueMapControl.GetScale();
				valueMap.Palette = valueMapControl.GetPalette();
				return new CreateChoroplethMapHistoryItem(section, groupIndex, mapDashboardItem, initialMap, valueMap);
			}
			else {
				DeltaMap deltaMap = initialMap as DeltaMap;
				if(deltaMap != null)
					return new ChangeDeltaMapHistoryItem(mapDashboardItem, deltaMap, deltaMapControl.GetDeltaOptions());
				deltaMap = new DeltaMap();
				deltaMap.ActualValue = measure;
				deltaMapControl.Apply(deltaMap);
				return new CreateChoroplethMapHistoryItem(section, groupIndex, mapDashboardItem, initialMap, deltaMap);
			}
		}
	}
}
