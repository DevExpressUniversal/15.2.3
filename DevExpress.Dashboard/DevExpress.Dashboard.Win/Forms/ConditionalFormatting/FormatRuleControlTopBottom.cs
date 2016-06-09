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
using System.ComponentModel;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraLayout;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class FormatRuleControlTopBottom : FormatRuleControlStyleBase, IFormatRuleControlTopBottomView {
		CheckEdit ceUsePercents;
		SpinEdit seRank;
		public FormatRuleControlTopBottom() : base() {
			InitializeComponent();
		}
		protected override void Initialize(IFormatRuleControlViewInitializationContext initializationContext) {
			base.Initialize(initializationContext);
			AddRankItem();
		}
		void AddRankItem() {
			this.ceUsePercents = new CheckEdit();
			this.ceUsePercents.Name = "ceUsePercents";
			this.ceUsePercents.Properties.Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRulePercentOf);
			this.ceUsePercents.CheckedChanged += OnUsePercentsChanged;
			this.seRank = new SpinEdit();
			this.seRank.Name = "seRank";
			this.seRank.Properties.EditValueChangedFiringMode = EditValueChangedFiringMode.Default;
			this.seRank.Properties.MinValue = 1;
			this.seRank.Properties.MaxValue = 100;
			this.seRank.Properties.Mask.MaskType = MaskType.Numeric;
			this.seRank.Properties.Mask.UseMaskAsDisplayFormat = true;
			ApplyRankEditorMask(false);
			this.seRank.EditValue = 10;
			this.seRank.EditValueChanged += OnRankChanged;
			LayoutControlGroup lcgTopBottomN = ValuePanelGroup.AddGroup();
			lcgTopBottomN.GroupBordersVisible = false;
			lcgTopBottomN.Name = "lcgTopBottomN";
			lcgTopBottomN.Padding = new XtraLayout.Utils.Padding(0);
			lcgTopBottomN.TextVisible = false;
			LayoutControlItem lciRank = lcgTopBottomN.AddItem(DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionTopBottomNCaption), this.seRank);
			lciRank.Name = "lciRank";
			lciRank.Padding = new XtraLayout.Utils.Padding(0);
			lciRank.TextAlignMode = XtraLayout.TextAlignModeItem.AutoSize;
			lciRank.TextToControlDistance = 10;
			LayoutControlItem lciUsePercents = lcgTopBottomN.AddItem(string.Empty, this.ceUsePercents, lciRank, XtraLayout.Utils.InsertType.Right);
			lciUsePercents.Name = "lciUsePercents";
			lciUsePercents.Padding = new XtraLayout.Utils.Padding(12, 0, 0, 0);
			lciUsePercents.TextVisible = false;
		}
		void OnUsePercentsChanged(object sender, EventArgs e) {
			ApplyRankEditorMask(ceUsePercents.Checked);
			RaiseStateUpdated();
		}
		void OnRankChanged(object sender, EventArgs e) {
			RaiseStateUpdated();
		}
		void ApplyRankEditorMask(bool usePercent) {
			this.seRank.Properties.Mask.EditMask = usePercent ? "P" : "d";
		}
		#region IFormatRuleControlTopBottomView Members
		bool IFormatRuleControlTopBottomView.IsPercent {
			get { return ceUsePercents.Checked; }
			set { ceUsePercents.Checked = value; }
		}
		decimal IFormatRuleControlTopBottomView.Rank {
			get { return Convert.ToDecimal(seRank.EditValue); }
			set { seRank.EditValue = value; }
		}
		#endregion
	}
}
