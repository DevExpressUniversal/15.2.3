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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class SeriesGroupsControl : ChartUserControl {
		static readonly object seriesGroupChanged = new object();
		SeriesViewBase view;
		SeriesBase series;
		SeriesGroupWrappers wrappers;
		bool locked;
		object SeriesGroup {
			get {
				ISupportSeriesGroups groupsView = view as ISupportSeriesGroups;
				return groupsView != null ? groupsView.SeriesGroup : null;
			}
			set {
				ISupportSeriesGroups groupsView = view as ISupportSeriesGroups;
				if (groupsView != null)
					groupsView.SeriesGroup = value;
			}
		}
		public event EventHandler SeriesGroupChanged {
			add { Events.AddHandler(seriesGroupChanged, value); }
			remove { Events.RemoveHandler(seriesGroupChanged, value); }
		}
		public SeriesGroupsControl() {
			InitializeComponent();
		}
		public void Initialize(SeriesViewBase view, SeriesBase series) {
			this.view = view;
			this.series = series;
			UpdateWrappers();
		}
		void UpdateWrappers() {
			SeriesGroupWrappers newWrappers = SeriesGroupsHelper.CreateSeriesGroupWrappers(series);
			if (wrappers != null) {
				foreach (SeriesGroupWrapper wrapper in wrappers) {
					foreach (SeriesGroupWrapper newWrapper in newWrappers) {
						if (object.Equals(newWrapper.Group, wrapper.Group)) {
							newWrapper.Name = wrapper.Name;
							break;
						}
					}
				}
			}
			wrappers = newWrappers;
			locked = true;
			try {
				cbSeriesGroup.Properties.Items.Clear();
				foreach (SeriesGroupWrapper wrapper in wrappers)
					cbSeriesGroup.Properties.Items.Add(wrapper);
				cbSeriesGroup.SelectedItem = wrappers.GetWrapperByGroup(SeriesGroup);
			}
			finally {
				locked = false;
			}
		}
		void RaiseSeriesGroupChanged() {
			EventHandler handler = (EventHandler)this.Events[seriesGroupChanged];
			if (handler != null)
				handler(this, new EventArgs());
		}
		void cbSeriesGroup_EditValueChanged(object sender, EventArgs e) {
			if (locked)
				return;
			SeriesGroupWrapper wrapper = cbSeriesGroup.EditValue as SeriesGroupWrapper;
			if (wrapper != null)
				SeriesGroup = wrapper.Group;
			else {
				string name = cbSeriesGroup.EditValue != null ? cbSeriesGroup.EditValue.ToString() : null;
				if (string.IsNullOrEmpty(name))
					SeriesGroup = null;
				else {
					SeriesGroupWrapper wrapperByName = wrappers.GetWrapperByName(name);
					SeriesGroup = wrapperByName != null ? wrapperByName.Group : name;
				}
			}
			UpdateWrappers();
			RaiseSeriesGroupChanged();
		}
	}
}
