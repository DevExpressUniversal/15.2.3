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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.ServiceModel;
namespace DevExpress.DashboardWin.Native {
	public partial class ScatterChartPointLabelOptionsForm : DashboardForm, IScatterChartLabelOptionsView {
		EventHandler OKAction;
		EventHandler CancelAction;
		EventHandler ShowPointLabelsAction;
		EventHandler<ScatterChartLabelOptionsEventArgs> ChangedAction;
		public ScatterChartPointLabelOptionsForm() {
			InitializeComponent();
		}
		void Initialize(ScatterChartPointLabelSettings settings, bool positionEnabled) {
			ceShowPointLabels.Checked = settings.ShowPointLabels;
			cbContentType.SelectedIndex = (int)settings.Content;
			cbOverlappingMode.SelectedIndex = (int)settings.OverlappingMode;
			cbOrientation.SelectedIndex = (int)settings.Orientation;
			cbPosition.SelectedIndex = (int)settings.Position;
			cbPosition.Enabled = positionEnabled;
		}
		ScatterChartPointLabelSettings GetSettings() {
			return new ScatterChartPointLabelSettings {
				ShowPointLabels = ceShowPointLabels.Checked,
				Content = (ScatterPointLabelContentType)cbContentType.SelectedIndex,
				OverlappingMode = (PointLabelOverlappingMode)cbOverlappingMode.SelectedIndex,
				Orientation = (PointLabelOrientation)cbOrientation.SelectedIndex,
				Position = (PointLabelPosition)cbPosition.SelectedIndex
			};
		}
		void UpdateUI() {
			lcOptions.Enabled = ceShowPointLabels.Checked;
		}
		void OnChanged() {
			if(ChangedAction != null) {
				ScatterChartLabelOptionsEventArgs args = new ScatterChartLabelOptionsEventArgs(GetSettings());
				ChangedAction(this, args);
			}
		}
		void btnOK_Click(object sender, EventArgs e) {
			if(OKAction != null)
				OKAction(this, new EventArgs());
		}
		void btnCancel_Click(object sender, EventArgs e) {
			if(CancelAction != null)
				CancelAction(this, new EventArgs());
		}
		void ceShowPointLabels_CheckedChanged(object sender, EventArgs e) {
			OnChanged();
			if(ShowPointLabelsAction != null)
				ShowPointLabelsAction(this, new EventArgs());			
		}
		void cbContentType_SelectedIndexChanged(object sender, EventArgs e) {
			OnChanged();
		}
		void cbOverlappingMode_SelectedIndexChanged(object sender, EventArgs e) {
			OnChanged();
		}
		void cbOrientation_SelectedIndexChanged(object sender, EventArgs e) {
			OnChanged();
		}
		void cbPosition_SelectedIndexChanged(object sender, EventArgs e) {
			OnChanged();
		}
		#region IScatterChartLabelOptionsView Members
		event EventHandler<ScatterChartLabelOptionsEventArgs> IScatterChartLabelOptionsView.Changed {
			add { ChangedAction += value; }
			remove { ChangedAction -= value; }
		}
		event EventHandler IScatterChartLabelOptionsView.OK {
			add { OKAction += value; }
			remove { OKAction -= value; }
		}
		event EventHandler IScatterChartLabelOptionsView.Cancel {
			add { CancelAction += value; }
			remove { CancelAction -= value; }
		}
		event EventHandler IScatterChartLabelOptionsView.ShowPointLabelsChanged {
			add { ShowPointLabelsAction += value; }
			remove { ShowPointLabelsAction -= value; }
		}
		void IScatterChartLabelOptionsView.Initialize(ScatterChartPointLabelSettings settings, bool positionEnabled) {
			Initialize(settings, positionEnabled);
		}
		void IScatterChartLabelOptionsView.UpdateUI() {
			UpdateUI();
		}
		#endregion
	}
	public interface IScatterChartLabelOptionsView {
		event EventHandler<ScatterChartLabelOptionsEventArgs> Changed;
		event EventHandler OK;
		event EventHandler Cancel;
		event EventHandler ShowPointLabelsChanged;
		void Initialize(ScatterChartPointLabelSettings settings, bool positionEnabled);
		void UpdateUI();
	}
	public class ScatterChartLabelOptionsEventArgs : EventArgs {
		public ScatterChartPointLabelSettings Settings { get; private set; }
		public ScatterChartLabelOptionsEventArgs(ScatterChartPointLabelSettings settings) {
			Settings = settings;
		}
	}
	public class ScatterChartLabelOptionsPresenter {
		readonly IDashboardDesignerHistoryService historyService;
		readonly ScatterChartPointLabelHistoryItem historyItem;
		public ScatterChartLabelOptionsPresenter(IDashboardDesignerHistoryService historyService, ScatterChartPointLabelHistoryItem historyItem) {
			this.historyService = historyService;
			this.historyItem = historyItem;
		}
		public void Initialize(IScatterChartLabelOptionsView view) {
			view.Initialize(historyItem.Settings, historyItem.PositionEnabled);
			view.UpdateUI();
			view.Changed += OnViewChanged;
			view.OK += OnViewOK;
			view.Cancel += OnViewCancel;
			view.ShowPointLabelsChanged += (s, e) => { view.UpdateUI(); };
		}
		void OnViewChanged(object sender, ScatterChartLabelOptionsEventArgs e) {
			historyItem.UpdateSettings(e.Settings);
			historyService.Redo(historyItem);
		}
		void OnViewOK(object sender, EventArgs e) {
			historyService.RedoAndAdd(historyItem);
		}
		void OnViewCancel(object sender, EventArgs e) {
			historyService.Undo(historyItem);
		}
	}
}
