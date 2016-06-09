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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.Data.XtraReports.DataProviders;
namespace DevExpress.XtraReports.Wizards3.Views {
	[System.ComponentModel.ToolboxItem(false)]
	public partial class SummaryOptionsItem : UserControl{
		readonly ColumnInfoSummaryOptions summaryOptions;
		public ColumnInfo ColumnInfo {
			get { return summaryOptions.ColumnInfo; }
		}
		public SummaryOptionsItem() {
			InitializeComponent();
		}
		public SummaryOptionsItem(ColumnInfoSummaryOptions summaryOptions) {
			InitializeComponent();
			this.summaryOptions = summaryOptions;
			columnInfoName.Text = summaryOptions.ColumnInfo.Name;
			InitializeEditors(summaryOptions.Options);
		}
		void InitializeEditors(SummaryOptions options) {
			avgEdit.EditValue = summaryOptions.Options.Avg;
			avgEdit.Tag = SummaryOptionFlags.Avg;
			countEdit.EditValue = summaryOptions.Options.Count;
			countEdit.Tag = SummaryOptionFlags.Count;
			maxEdit.EditValue = summaryOptions.Options.Max;
			maxEdit.Tag = SummaryOptionFlags.Max;
			minEdit.EditValue = summaryOptions.Options.Min;
			minEdit.Tag = SummaryOptionFlags.Min;
			sumEdit.EditValue = summaryOptions.Options.Sum;
			sumEdit.Tag = SummaryOptionFlags.Sum;
			avgEdit.CheckedChanged += OnOptionChanged;
			countEdit.CheckedChanged += OnOptionChanged;
			minEdit.CheckedChanged += OnOptionChanged;
			maxEdit.CheckedChanged += OnOptionChanged;
			sumEdit.CheckedChanged += OnOptionChanged;
		}
		void OnOptionChanged(object sender, EventArgs e) {
			summaryOptions.Options.Avg = avgEdit.Checked;
			summaryOptions.Options.Count = countEdit.Checked;
			summaryOptions.Options.Max = maxEdit.Checked;
			summaryOptions.Options.Min = minEdit.Checked;
			summaryOptions.Options.Sum = sumEdit.Checked;
		}
		private void panel1_Paint(object sender, PaintEventArgs e) {
			var rectangle = new Rectangle(e.ClipRectangle.Left, 0 - (this.Top + this.Parent.Top), 1, this.Parent.Bounds.Height);
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(UserLookAndFeel.Default)[CommonSkins.SkinLabelLineVert], rectangle);
			ObjectPainter.DrawObject(new GraphicsCache(e), SkinElementPainter.Default, info);
		}
	}
}
