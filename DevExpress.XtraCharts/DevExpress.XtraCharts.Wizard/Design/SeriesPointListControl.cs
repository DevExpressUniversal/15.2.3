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
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Design {
	public partial class SeriesPointListControl : DevExpress.XtraEditors.XtraUserControl {
		class SeriesPointItem : CheckedListBoxItem {
			SeriesPoint point;
			public SeriesPoint Point { get { return point; } }
			public SeriesPointItem(SeriesPoint point) : base(point.Argument) {
				this.point = point;
			}
			public override string ToString() {
				return point.Argument;
			}
		}
		Series series;
		List<SeriesPoint> points;
		ListBoxControl[] values;
		bool selectedIndexLock;
		public SeriesPointListControl() {
			InitializeComponent();
		}
		public void Initialize(Series series, List<SeriesPoint> points) {
			this.selectedIndexLock = true;
			this.series = series;
			this.points = points;
			CreateControlsForValues();
			FillData();
			UpdateLayout();
			this.selectedIndexLock = false;
		}
		void CreateControlsForValues() {
			values = new ListBoxControl[((IViewArgumentValueOptions)this.series.View).PointDimension];
			values[0] = listBoxValue;
			for(int i = 1; i < values.Length; i++) {
				values[i] = new ListBoxControl();
				Controls.Add(values[i]);
			}
			foreach(ListBoxControl valueControl in values)
				valueControl.SelectedIndexChanged += new System.EventHandler(checkedListBoxArguments_SelectedIndexChanged);
		}
		void FillData() {
			foreach(SeriesPoint point in points) {
				this.checkedListBoxArguments.Items.Add(new SeriesPointItem(point));
				for(int i = 0; i < values.Length; i++)
					values[i].Items.Add(point.GetValueString(i));
			}
		}
		void UpdateLayout() {
			if(values == null)
				return;
			int width = (int)Math.Floor((double)Width / (values.Length + 1));
			checkedListBoxArguments.Bounds = new Rectangle(0, 0, width, Height);
			int x = width;
			for(int i = 0; i < values.Length - 1; i++) {
				values[i].Bounds = new Rectangle(x, 0, width, Height);
				x += width;
			}
			values[values.Length - 1].Bounds = new Rectangle(x, 0, Width - x, Height);
		}
		private void SeriesPointListControl_Resize(object sender, EventArgs e) {
			UpdateLayout();
		}
		private void checkedListBoxArguments_SelectedIndexChanged(object sender, EventArgs e) {
			if(this.selectedIndexLock)
				return;
			this.selectedIndexLock = true;
			SynchronizeSelectedIndex((BaseListBoxControl)sender);
			this.selectedIndexLock = false;
		}
		void SynchronizeSelectedIndex(BaseListBoxControl control) {
			int selectedIndex = control.SelectedIndex;
			foreach(BaseListBoxControl valueControl in values)
				if(valueControl != control)
					valueControl.SelectedIndex = selectedIndex;
			if(this.checkedListBoxArguments != control)
				this.checkedListBoxArguments.SelectedIndex = selectedIndex;
		}
	}
}
