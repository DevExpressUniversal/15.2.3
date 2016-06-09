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
using System.Collections;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Design {
	public partial class AxisVisibilityInPanesForm : XtraForm {
		class PaneItem : ListViewItem {
			XYDiagramPaneBase pane;
			public XYDiagramPaneBase Pane { get { return pane; } }
			public bool Visible { get { return base.Checked; } }
			public PaneItem(XYDiagramPaneBase pane, bool visible) {
				this.pane = pane;
				Text = "";
				SubItems.Add(pane.Name);
				Checked = visible;
			}
		}
		readonly IDictionary visibleInPanes;
		readonly Chart chart;
		readonly Locker locker = new Locker();
		AxisVisibilityInPanesForm() {
			InitializeComponent();
		}
		public AxisVisibilityInPanesForm(IDictionary visibleInPanes, Chart chart) : this() {
			this.visibleInPanes = visibleInPanes;
			this.chart = chart;
			locker.Lock();
			try {
				visibleColumn.Text = ChartLocalizer.GetString(ChartStringId.PanesVisibilityDialogVisibleColumn);
				paneColumn.Text = ChartLocalizer.GetString(ChartStringId.PanesVisibilityDialogPanesColumn);
				foreach (DictionaryEntry item in visibleInPanes)
					listViewPanes.Items.Add(new PaneItem((XYDiagramPaneBase)item.Key, (bool)item.Value));
				AlignListView();
			}
			finally {
				locker.Unlock();
			}
		}
		void AlignListView() {
			NativeUtils.SendMessage(listViewPanes.Handle, NativeUtils.LVM_SETCOLUMNWIDTH, 0, NativeUtils.LVSCW_AUTOSIZE_USEHEADER);
			paneColumn.Width = listViewPanes.ClientSize.Width - visibleColumn.Width;
		}
		void AxisVisibilityInPanesForm_Resize(object sender, EventArgs e) {
			listViewPanes.BeginUpdate();
			try {
				AlignListView();
			}
			finally {
				listViewPanes.EndUpdate();
			}
		}
		void listViewPanes_ItemChecked(object sender, ItemCheckedEventArgs e) {
			if (!locker.IsLocked) {
				PaneItem item = (PaneItem)e.Item;
				SetVisibilityInPane(item.Pane, item.Visible);
				((IChartElementWizardAccess)chart).RaiseControlChanged();
			}
		}
		protected virtual void SetVisibilityInPane(XYDiagramPaneBase pane, bool visible) {
			visibleInPanes[pane] = visible;
		}
	}
}
