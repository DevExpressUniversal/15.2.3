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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public partial class StackedGroupControl : XtraUserControl, IModelBinded {
		DesignerChartElementModelBase model;
		SeriesBase series;
		string value;
		public string Value {
			get { return value; }
			set {
				if(!string.Equals(value, this.value)) {
					this.value = value;
					OnValueChanged();
				}
			}
		}
		public event EventHandler ValueChanged;
		public StackedGroupControl() {
			InitializeComponent();
			comboBoxEdit1.EditValueChanged += comboBoxEdit1_EditValueChanged;
			comboBoxEdit1.QueryPopUp += comboBoxEdit1_Popup;
		}
		#region IModelBinded implementation
		void IModelBinded.SetModel(DesignerChartElementModelBase model) {
			this.model = model;
			this.series = ((IOwnedElement)model.ChartElement).Owner as SeriesBase;
		}
		#endregion
		void OnValueChanged() {
			this.comboBoxEdit1.EditValue = value;
			if(ValueChanged != null)
				ValueChanged(this, new EventArgs());
		}
		void comboBoxEdit1_Popup(object sender, EventArgs e) {
			PopulateGroups();
		}
		void PopulateGroups() {
			comboBoxEdit1.Properties.Items.Clear();
			comboBoxEdit1.Properties.Items.AddRange(SeriesGroupsHelper.CreateSeriesGroupArray(series));
		}
		void comboBoxEdit1_EditValueChanged(object sender, EventArgs e) {
			Value = comboBoxEdit1.EditValue != null ? comboBoxEdit1.EditValue.ToString() : string.Empty;
		}
	}
}
