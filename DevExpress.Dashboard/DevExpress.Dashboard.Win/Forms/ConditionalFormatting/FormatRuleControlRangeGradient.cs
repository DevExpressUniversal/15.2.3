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
using System.Drawing;
using DevExpress.DashboardWin.Localization;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class FormatRuleControlRangeGradient : FormatRuleControlBase, IFormatRuleControlRangeGradientView {
		const int MaxRangeCount = 50;
		event FormatRuleRangeGradientViewGeneratingEventHandler rangeViewGenerating;
		event FormatRuleRangeViewChangingEventHandler rangeViewChanging;
		event EventHandler usePercentChanging;
		FormatRuleRangeEditorControl rangeEditorControl;
		CheckEdit ceUsePercents;
		SpinEdit seSegmentNumber;
		protected virtual int RangeControlMinHeight { get { return 244; } }
		public FormatRuleControlRangeGradient()
			: base() {
			InitializeComponent();
		}
		protected override void Initialize(IFormatRuleControlViewInitializationContext initializationContext) {
			base.Initialize(initializationContext);
			IFormatRuleViewRangeGradientContext rangeSetContext = (IFormatRuleViewRangeGradientContext)initializationContext;
			AddLayoutItems(rangeSetContext.IsPercentsSupported);
			rangeEditorControl.Initialize(rangeSetContext.DataType, rangeSetContext.DateTimeGroupInterval);
		}
		void AddLayoutItems(bool isAggregationSupported) {
			LayoutControlGroup lcgRangeSegments = RootGroup.AddGroup();
			lcgRangeSegments.Name = "lcgRangeSegments";
			lcgRangeSegments.GroupBordersVisible = false;
			lcgRangeSegments.EnableIndentsWithoutBorders = Utils.DefaultBoolean.True;
			lcgRangeSegments.Padding = new XtraLayout.Utils.Padding(0, 0, 0, 8);
			lcgRangeSegments.Spacing = new XtraLayout.Utils.Padding(0);
			lcgRangeSegments.TextVisible = false;
			SimpleLabelItem lciSegmentNumberLabel = new SimpleLabelItem() { 
				Name = "lciSegmentNumberLabel",
				Text = DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleRangeSegmentNumber),
				Padding = new XtraLayout.Utils.Padding(2, 2, 4, 2),
				AllowHotTrack = false
			};
			lciSegmentNumberLabel.AppearanceItemCaption.Options.UseTextOptions = true;
			lciSegmentNumberLabel.AppearanceItemCaption.TextOptions.VAlignment = VertAlignment.Top;
			lcgRangeSegments.AddItem(lciSegmentNumberLabel);
			this.seSegmentNumber = new SpinEdit() {
				Name = "seSegmentNumber",
				EditValue = 2,
				MinimumSize = new Size(50, 20)
			};
			seSegmentNumber.Properties.EditValueChangedFiringMode = EditValueChangedFiringMode.Default;
			seSegmentNumber.Properties.IsFloatValue = false;
			seSegmentNumber.Properties.Mask.EditMask = "N00";
			seSegmentNumber.Properties.MinValue = 2;
			seSegmentNumber.Properties.MaxValue = MaxRangeCount;
			LayoutControlItem lciSegmentNumber = lcgRangeSegments.AddItem(string.Empty, this.seSegmentNumber, lciSegmentNumberLabel, InsertType.Right);			
			lciSegmentNumber.Name = "lciSegmentNumber";
			lciSegmentNumber.TextVisible = false;			
			SimpleButton sbGenerateRanges = new SimpleButton() {
				Name = "sbGenerateRanges",
				MinimumSize = new Size(120, 22),
				Text = DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleRangeGenerateRanges)
			};
			sbGenerateRanges.Click += OnGenerateRanges;
			LayoutControlItem lciGenerateRanges = lcgRangeSegments.AddItem(string.Empty, sbGenerateRanges, lciSegmentNumber, InsertType.Bottom);
			lciGenerateRanges.Name = "lciGenerateRanges";
			lciGenerateRanges.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 4, 2);
			lciGenerateRanges.TextVisible = false;
			this.ceUsePercents = new CheckEdit() { Name = "ceUsePercents" };
			this.ceUsePercents.Properties.Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleRangeUsePercent);
			this.ceUsePercents.CheckedChanged += OnUsePercentsCheckedChanged;
			this.ceUsePercents.Enabled = isAggregationSupported;
			LayoutControlItem lciUsePercents = RootGroup.AddItem(string.Empty, this.ceUsePercents);
			lciUsePercents.Name = "lciUsePercents";
			lciUsePercents.TextVisible = false;
			this.rangeEditorControl = new FormatRuleRangeEditorControl() {
				Name = "rangeEditorControl",
				MinimumSize = new Size(250, RangeControlMinHeight)
			};
			this.rangeEditorControl.RangeViewChanging += OnChanging;
			this.rangeEditorControl.RangeViewChanged += OnStateChanged;
			LayoutControlItem lciRangesEditor = RootGroup.AddItem(string.Empty, this.rangeEditorControl);
			lciRangesEditor.Name = "lciRangesEditor";
			lciRangesEditor.TextVisible = false;
			AddGroupApplyTo(lciRangesEditor, InsertType.Bottom);			
		}
		void OnChanging(object sender, FormatRuleRangeViewChangingEventArgs e) {
			if(rangeViewChanging != null)
				rangeViewChanging(this, e);
		}
		void OnGenerateRanges(object sender, EventArgs e) {
			if(rangeViewGenerating != null) {
				FormatRuleRangeGradientViewGeneratingEventArgs args = new FormatRuleRangeGradientViewGeneratingEventArgs(Convert.ToInt32(seSegmentNumber.EditValue));
				rangeViewGenerating(this, args);
			}
			RaiseStateUpdated();
		}
		void OnUsePercentsCheckedChanged(object sender, EventArgs e) {
			rangeEditorControl.IsPercent = ceUsePercents.Checked;
			if(usePercentChanging != null)
				usePercentChanging(this, new EventArgs());
			RaiseStateUpdated();
		}
		#region IFormatRuleControlRangeBaseView Members
		bool IFormatRuleControlRangeBaseView.IsPercent {
			get { return ceUsePercents.Checked; }
			set { ceUsePercents.Checked = value; }
		}
		IList<IFormatRuleRangeView> IFormatRuleControlRangeBaseView.Ranges {
			get { return rangeEditorControl.Ranges; }
			set {
				rangeEditorControl.Ranges = value;
				seSegmentNumber.EditValue = value.Count;
			}
		}
		event FormatRuleRangeViewChangingEventHandler IFormatRuleControlRangeBaseView.RangeViewChanged {
			add { rangeViewChanging += value; }
			remove { rangeViewChanging -= value; }
		}
		event EventHandler IFormatRuleControlRangeBaseView.UsePercentChanged {
			add { usePercentChanging += value; }
			remove { usePercentChanging -= value; }
		}
		#endregion
		#region IFormatRuleControlRangeGradientView Members
		event FormatRuleRangeGradientViewGeneratingEventHandler IFormatRuleControlRangeGradientView.RangeGradientViewGenerating {
			add { rangeViewGenerating += value; }
			remove { rangeViewGenerating -= value; }
		}
		#endregion
	}
}
