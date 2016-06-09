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
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class FormatRuleControlRangeSet : FormatRuleControlBase, IFormatRuleControlRangeSetView {
		const int MaxRangeCount = 50;
		event FormatRuleRangeViewCreatingEventHandler rangeViewCreating;
		event FormatRuleRangeViewChangingEventHandler rangeViewChanging;
		event FormatRuleRangeSetPredefinedStyleChangedEventHandler predefinedStyleChanged;
		event EventHandler usePercentChanging;
		RangeStyleEdit formatStyleEdit;
		FormatRuleRangeEditorControl rangeEditorControl;
		CheckEdit ceUsePercents;
		SimpleButton btnAdd;
		SimpleButton btnDelete;
		SimpleButton btnReverseStyles;
		public FormatRuleControlRangeSet() {
			InitializeComponent();
		}
		protected override void Initialize(IFormatRuleControlViewInitializationContext initializationContext) {
			base.Initialize(initializationContext);
			IFormatRuleViewRangeSetContext rangeSetContext = (IFormatRuleViewRangeSetContext)initializationContext;
			AddLayoutItems(rangeSetContext.StyleMode, rangeSetContext.IsPercentsSupported);
			this.rangeEditorControl.Initialize(rangeSetContext.DataType, rangeSetContext.DateTimeGroupInterval);
			this.rangeEditorControl.RangeViewChanging += OnChanging;
			this.rangeEditorControl.RangeViewChanged += OnStateChanged;
		}
		void AddLayoutItems(StyleMode styleMode, bool isAggregationSupported) {
			this.formatStyleEdit = new RangeStyleEdit(BarManager, styleMode) {
				Name = "formatStyleEdit"
			};
			this.formatStyleEdit.PredefinedStyleChanged += OnPredefinedStyleChanged;
			LayoutControlItem lciFormatStyle = RootGroup.AddItem(DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleFormatStyle), this.formatStyleEdit);
			lciFormatStyle.Name = "lciFormatStyle";
			lciFormatStyle.Padding = new XtraLayout.Utils.Padding(2, 2, 2, 16);
			lciFormatStyle.TextLocation = DevExpress.Utils.Locations.Top;
			this.ceUsePercents = new DevExpress.XtraEditors.CheckEdit() { Name = "ceUsePercents" };
			this.ceUsePercents.Properties.Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleRangeUsePercent);
			this.ceUsePercents.CheckedChanged += OnUsePercentsCheckedChanged;
			this.ceUsePercents.Enabled = isAggregationSupported;
			LayoutControlItem lciUsePercents = RootGroup.AddItem(string.Empty, this.ceUsePercents);
			lciUsePercents.Name = "lciUsePercents";
			lciUsePercents.TextVisible = false;
			this.rangeEditorControl = new FormatRuleRangeEditorControl() {
				Name = "rangeEditorControl",
				MinimumSize = new Size(250, 124)
			};
			LayoutControlItem lciRangeEditor = RootGroup.AddItem(string.Empty, this.rangeEditorControl);
			lciRangeEditor.Name = "lciRangeEditor";
			lciRangeEditor.TextVisible = false;
			this.btnAdd = new SimpleButton() {
				Name = "btnAdd",
				Text = DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleRangeAdd),
				MinimumSize = new Size(58, 0),
				MaximumSize = new Size(58, 0)
			};
			btnAdd.Click += OnAdd;
			this.btnDelete = new SimpleButton() {
				Name = "btnDelete",
				Text = DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleRangeDelete),
				MinimumSize = btnAdd.MinimumSize,
				MaximumSize = btnAdd.MaximumSize
			};
			btnDelete.Click += OnDelete;
			this.btnReverseStyles = new SimpleButton() {
				Name = "btnReverseStyles",
				Text = DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleRangeReverseStyles),
				MinimumSize = btnAdd.MinimumSize,
				MaximumSize = btnAdd.MaximumSize
			};
			btnReverseStyles.Click += OnReverseStyles;
			LayoutControlGroup lcgAddDeleteButtons = RootGroup.AddGroup();
			lcgAddDeleteButtons.GroupBordersVisible = false;
			lcgAddDeleteButtons.Name = "lcgAddDeleteButtons";
			lcgAddDeleteButtons.Padding = new XtraLayout.Utils.Padding(10, 10, 10, 30);
			lcgAddDeleteButtons.Spacing = new XtraLayout.Utils.Padding(0);
			lcgAddDeleteButtons.TextVisible = false;
			LayoutControlItem lciAdd = lcgAddDeleteButtons.AddItem(string.Empty, btnAdd);
			lciAdd.Name = "lciAdd";
			lciAdd.TextVisible = false;
			LayoutControlItem lciDelete = lcgAddDeleteButtons.AddItem(string.Empty, btnDelete, lciAdd, XtraLayout.Utils.InsertType.Right);
			lciDelete.Name = "lciDelete";
			lciDelete.TextVisible = false;
			EmptySpaceItem emptySpaceItemAddDeleteButtons = new EmptySpaceItem();
			lcgAddDeleteButtons.AddItem(emptySpaceItemAddDeleteButtons, lciDelete, XtraLayout.Utils.InsertType.Right);
			emptySpaceItemAddDeleteButtons.Name = "emptySpaceItemAddDeleteButtons";
			emptySpaceItemAddDeleteButtons.AllowHotTrack = false;
			emptySpaceItemAddDeleteButtons.TextVisible = false;
			LayoutControlItem lciReverseStyles = lcgAddDeleteButtons.AddItem(string.Empty, btnReverseStyles, emptySpaceItemAddDeleteButtons, XtraLayout.Utils.InsertType.Right);
			lciReverseStyles.Name = "lciReverseStyles";
			lciReverseStyles.TextVisible = false;
			AddGroupApplyTo(lcgAddDeleteButtons, XtraLayout.Utils.InsertType.Bottom);
		}
		IFormatRuleRangeView RaiseRangeViewCreating() {
			if (rangeViewCreating != null)
				return rangeViewCreating(this, new EventArgs());
			throw new ArgumentException("Cannot create range");
		}
		void OnUsePercentsCheckedChanged(object sender, EventArgs e) {
			rangeEditorControl.IsPercent = ceUsePercents.Checked;
			if(usePercentChanging != null)
				usePercentChanging(this, new EventArgs());
			RaiseStateUpdated();
		}
		void OnAdd(object sender, EventArgs e) {
			IFormatRuleRangeView rangeView = RaiseRangeViewCreating();
			rangeEditorControl.Insert(rangeView);
			EnableButtons();
			RaiseStateUpdated();
		}
		void OnDelete(object sender, EventArgs e) {
			rangeEditorControl.Delete();
			EnableButtons();
			RaiseStateUpdated();
		}
		void OnReverseStyles(object sender, EventArgs e) {
			rangeEditorControl.ReverseStyles();
			RaiseStateUpdated();
		}
		void OnPredefinedStyleChanged(object sender, FormatRuleRangeSetPredefinedStyleChangedEventArgs e) {
			if(predefinedStyleChanged != null)
				predefinedStyleChanged(this, e);
			EnableButtons();
			RaiseStateUpdated();
		}
		void OnChanging(object sender, FormatRuleRangeViewChangingEventArgs e) {
			if(rangeViewChanging != null)
				rangeViewChanging(this, e);
		}
		void EnableButtons() {
			int rangeCount = rangeEditorControl.Ranges.Count;
			btnDelete.Enabled = rangeCount > 2;
			btnAdd.Enabled = rangeCount < MaxRangeCount;
			btnReverseStyles.Enabled = rangeCount > 1;
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
				EnableButtons();
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
		#region IFormatRuleControlRangeSetView Members
		FormatConditionRangeSetPredefinedType IFormatRuleControlRangeSetView.PredefinedType {
			get { return formatStyleEdit.PredefinedStyleType; }
			set { formatStyleEdit.PredefinedStyleType = value; }
		}
		event FormatRuleRangeViewCreatingEventHandler IFormatRuleControlRangeSetView.RangeViewCreating {
			add { rangeViewCreating += value; }
			remove { rangeViewCreating -= value; }
		}
		event FormatRuleRangeSetPredefinedStyleChangedEventHandler IFormatRuleControlRangeSetView.PredefinedStyleChanged {
			add { predefinedStyleChanged += value; }
			remove { predefinedStyleChanged -= value; }
		}
		#endregion
	}
}
