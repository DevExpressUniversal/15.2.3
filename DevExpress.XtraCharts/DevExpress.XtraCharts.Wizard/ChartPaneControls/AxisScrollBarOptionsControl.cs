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
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	internal partial class AxisScrollBarOptionsControl : ChartUserControl {
		ScrollBarOptionsProperties scrollBarOptions;
		struct ScrollBarAlignmentItem {
			readonly ScrollBarAlignment alignment;
			readonly string text;
			public ScrollBarAlignment Alignment { get { return alignment; } }
			public ScrollBarAlignmentItem(ScrollBarAlignment alignment) {
				this.alignment = alignment;
				switch (alignment) {
					case ScrollBarAlignment.Near:
						text = ChartLocalizer.GetString(ChartStringId.WizScrollBarAlignmentNear);
						break;
					case ScrollBarAlignment.Far:
						text = ChartLocalizer.GetString(ChartStringId.WizScrollBarAlignmentFar);
						break;
					default:
						ChartDebug.Fail("Unknown scroll bar alignment.");
						text = ChartLocalizer.GetString(ChartStringId.WizScrollBarAlignmentNear);
						break;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				if (!(obj is ScrollBarAlignmentItem))
					return false;
				ScrollBarAlignmentItem item = (ScrollBarAlignmentItem)obj;
				return alignment == item.alignment;
			}
			public override int GetHashCode() {
				return alignment.GetHashCode();
			}
		}
		public AxisScrollBarOptionsControl() {
			InitializeComponent();
		}
		public void Initialize(ScrollBarOptionsProperties scrollBarOptions) {
			this.scrollBarOptions = scrollBarOptions;
			ceVisibleScrollBar.Checked = scrollBarOptions.Visible;
			cbPosition.Properties.Items.Clear();
			cbPosition.Properties.Items.Add(new ScrollBarAlignmentItem(ScrollBarAlignment.Near));
			cbPosition.Properties.Items.Add(new ScrollBarAlignmentItem(ScrollBarAlignment.Far));
			cbPosition.SelectedItem = new ScrollBarAlignmentItem(scrollBarOptions.Alignment);
		}
		void ceVisibleScrollBar_CheckedChanged(object sender, EventArgs e) {
			scrollBarOptions.Visible = ceVisibleScrollBar.Checked;
			pnlPosition.Enabled = scrollBarOptions.Visible;
		}
		void cbPosition_SelectedIndexChanged(object sender, EventArgs e) {
			scrollBarOptions.Alignment = ((ScrollBarAlignmentItem)cbPosition.SelectedItem).Alignment;
		}
	}
	internal abstract class ScrollBarOptionsProperties {
		readonly ScrollBarOptions scrollBarOptions;
		protected ScrollBarOptions ScrollBarOptions { get { return scrollBarOptions; } }
		public abstract bool Visible { get; set; }
		public abstract ScrollBarAlignment Alignment { get; set; }
		public ScrollBarOptionsProperties(ScrollBarOptions scrollBarOptions) {
			this.scrollBarOptions = scrollBarOptions;
		}
	}
	internal class AxisXScrollBarOptionsProperties : ScrollBarOptionsProperties {
		public override bool Visible {
			get { return ScrollBarOptions.XAxisScrollBarVisible; }
			set { ScrollBarOptions.XAxisScrollBarVisible = value; }
		}
		public override ScrollBarAlignment Alignment {
			get { return ScrollBarOptions.XAxisScrollBarAlignment; }
			set { ScrollBarOptions.XAxisScrollBarAlignment = value; }
		}
		public AxisXScrollBarOptionsProperties(ScrollBarOptions scrollBarOptions) : base(scrollBarOptions) {
		}
	}
	internal class AxisYScrollBarOptionsProperties : ScrollBarOptionsProperties {
		public override bool Visible {
			get { return ScrollBarOptions.YAxisScrollBarVisible; }
			set { ScrollBarOptions.YAxisScrollBarVisible = value; }
		}
		public override ScrollBarAlignment Alignment {
			get { return ScrollBarOptions.YAxisScrollBarAlignment; }
			set { ScrollBarOptions.YAxisScrollBarAlignment = value; }
		}
		public AxisYScrollBarOptionsProperties(ScrollBarOptions scrollBarOptions) : base(scrollBarOptions) {
		}
	}
}
