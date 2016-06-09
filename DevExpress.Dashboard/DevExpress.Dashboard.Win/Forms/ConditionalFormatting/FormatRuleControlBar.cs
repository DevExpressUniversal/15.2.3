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
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class FormatRuleControlBar : FormatRuleControlValue, IFormatRuleControlBarView {
		BarOptionsPanel barOptionsPanel;
		ComboBoxEdit cbMinType;
		ComboBoxEdit cbMaxType;
		ValueTypeWrapperList valueTypes;
		protected override string StyleControl1Caption { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleStyle); } }
		protected override string StyleControl2Caption { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleNegativeStyle); } }
		protected override bool IsValid {
			get {
				if(!base.IsValid)
					return false;
				if(MinimumType == DashboardFormatConditionValueType.Percent) {
					if(Minimum < 0 || Minimum > 100)
						return false;
				}
				if(MaximumType == DashboardFormatConditionValueType.Percent) {
					if(Maximum < 0 || Maximum > 100)
						return false;
				}
				return true;
			}
		}
		IFormatRuleControlBarOptions IFormatRuleControlBarOptionsView.BarOptions { get { return barOptionsPanel; } }
		ValueTypeWrapperList ValueTypes {
			get {
				if(valueTypes == null) {
					valueTypes = new ValueTypeWrapperList();
					EnumManager.Iterate<DashboardFormatConditionValueType>((type) => {
						valueTypes.Add(type);
					});
				}
				return valueTypes;
			}
		}
		public FormatRuleControlBar() {
			InitializeComponent();
			barOptionsPanel = new BarOptionsPanel(RootGroup);
			barOptionsPanel.OptionsChanged += OnBarOptionsPanelOptionsChanged;
		}
		protected override FormatRuleStyleSettingsControl CreateStyleControl1() {
			return new FormatRuleStyleSettingsControl(StyleMode.Bar, false, LookAndFeel) { Name = "styleControl" };
		}
		protected override FormatRuleStyleSettingsControl CreateStyleControl2() {
			return new FormatRuleStyleSettingsControl(StyleMode.Bar, false, LookAndFeel) { Name = "negativeStyleControl" };
		}
		protected override void OnStyleChanged(object sender, StyleSettingsContainerItemChangedEventArgs e) {
			RaiseStateUpdated();
		}
		protected override void Initialize(IFormatRuleControlViewInitializationContext initializationContext) {
			base.Initialize(initializationContext);
			barOptionsPanel.AddLayoutItems(((IFormatRuleViewBarContext)initializationContext).BarOptions);
		}
		void OnBarOptionsPanelOptionsChanged(object sender, EventArgs e) {
			RaiseStateUpdated();
		}
		protected override void FillValuePanel(IFormatRuleControlViewInitializationContext initializationContext) {
			FormatRuleControlViewTypedContext context = (FormatRuleControlViewTypedContext)initializationContext;
			DataType = context.DataType;
			LayoutControlItem lciMin = InsertBottomLayoutItem(DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionMinimumCaption), "lciMin");
			ValueEditor = CreateValueEditor("minValueEditor", DataType, context.DateTimeGroupInterval);
			ValueEditor.MinimumSize = new Size(96, 0);
			ValueEditor.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
			LayoutControlItem lciMinValue = InsertRightLayoutItem("lciMinValue", ValueEditor, lciMin, new XtraLayout.Utils.Padding(2, 4, 2, 2));
			cbMinType = CreateMinMaxTypeComboBox("cbMinType");
			InsertRightLayoutItem("lciMinType", cbMinType, lciMinValue, new XtraLayout.Utils.Padding(4, 2, 2, 2));
			LayoutControlItem lciMax = InsertBottomLayoutItem(DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionMaximumCaption), "lciMax");
			Value2Editor = CreateValueEditor("maxValueEditor", DataType, context.DateTimeGroupInterval);
			Value2Editor.MinimumSize = new Size(96, 0);
			Value2Editor.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
			LayoutControlItem lciMaxValue = InsertRightLayoutItem("lciMaxValue", Value2Editor, lciMax, new XtraLayout.Utils.Padding(2, 4, 2, 2));
			cbMaxType = CreateMinMaxTypeComboBox("cbMaxType");
			InsertRightLayoutItem("lciMaxType", cbMaxType, lciMaxValue, new XtraLayout.Utils.Padding(4, 2, 2, 2));
		}
		ComboBoxEdit CreateMinMaxTypeComboBox(string name) {
			ComboBoxEdit comboBox = EditorsInitializer.CreateCombo(name, OnMinMaxTypeChanged);
			comboBox.Properties.BeginUpdate();
			comboBox.Properties.Items.Clear();
			foreach(object item in ValueTypes) {
				comboBox.Properties.Items.Add(item);
			}
			comboBox.Properties.EndUpdate();
			comboBox.MinimumSize = new Size(90, 0);
			return comboBox;
		}
		void OnMinMaxTypeChanged(object sender, EventArgs e) {
			RaiseStateUpdated();
			ValueEditor.Enabled = MinimumType != DashboardFormatConditionValueType.Automatic;
			Value2Editor.Enabled = MaximumType != DashboardFormatConditionValueType.Automatic;
		}
		LayoutControlItem InsertRightLayoutItem(string lciName, Control control, LayoutControlItem layoutControlItem, XtraLayout.Utils.Padding padding) {
			LayoutControlItem lci = ValuePanelGroup.AddItem(String.Empty, control, layoutControlItem, InsertType.Right);
			lci.Name = lciName;
			lci.Padding = padding;
			lci.TextVisible = false;
			return lci;
		}
		LayoutControlItem InsertBottomLayoutItem(string text, string lciName) {
			LayoutControlItem lci = ValuePanelGroup.AddItem(String.Empty);
			lci.Name = lciName;
			LabelControl label = new LabelControl();
			label.Name = "lbl" + lciName;
			label.Text = text;
			lci.Control = label;
			lci.TextVisible = false;
			lci.TrimClientAreaToControl = false;
			lci.ControlAlignment = ContentAlignment.MiddleLeft;
			return lci;
		}
		#region IFormatRuleControlBarView Members
		StyleSettingsContainer IFormatRuleControlStyleView.Style {
			get {
				return ((IStyleContainerProvider)StyleControl1).Style;
			}
			set {
				this.LciTab.SelectedTabPage = this.LciTab.TabPages[0];
				((IStyleContainerProvider)StyleControl1).Style = value;
			}
		}
		StyleSettingsContainer IFormatRuleControlBarView.NegativeStyle {
			get {
				return ((IStyleContainerProvider)StyleControl2).Style;
			}
			set {
				this.LciTab.SelectedTabPage = this.LciTab.TabPages[1];
				((IStyleContainerProvider)StyleControl2).Style = value;
			}
		}
		public DashboardFormatConditionValueType MinimumType {
			get { return (DashboardFormatConditionValueType)cbMinType.SelectedIndex; }
			set { cbMinType.SelectedIndex = valueTypes.IndexOf(value); }
		}
		public DashboardFormatConditionValueType MaximumType {
			get { return (DashboardFormatConditionValueType)cbMaxType.SelectedIndex; }
			set { cbMaxType.SelectedIndex = valueTypes.IndexOf(value); }
		}
		public decimal Minimum {
			get { return Convert.ToDecimal(ValueEditor.EditValue); }
			set { ValueEditor.EditValue = value; }
		}
		public decimal Maximum {
			get { return Convert.ToDecimal(Value2Editor.EditValue); }
			set { Value2Editor.EditValue = value; }
		}
		#endregion
	}
	class BarOptionsPanel : IFormatRuleControlBarOptions {
		readonly LayoutControlGroup rootGroup;
		event EventHandler optionsChanged;
		CheckEdit ceAllowNegativeAxis;
		CheckEdit ceDrawAxis;
		CheckEdit ceShowBarOnly;
		public BarOptionsPanel(LayoutControlGroup rootGroup) {
			this.rootGroup = rootGroup;
		}
		public void AddLayoutItems(IFormatRuleBarOptionsInitializationContext barOptions) {
			LayoutControlItem lcItem;
			ceAllowNegativeAxis = CreateCheckEdit(DashboardWinStringId.FormatRuleAllowNegativeAxis, "AllowNegativeAxis", barOptions.AllowNegativeAxis, OnCheckedChanged, out lcItem);
			lcItem.Spacing = new XtraLayout.Utils.Padding(0, 0, 8, 0);
			ceDrawAxis = CreateCheckEdit(DashboardWinStringId.FormatRuleDrawAxis, "DrawAxis", barOptions.DrawAxis, OnCheckedChanged, out lcItem);
			ceShowBarOnly = CreateCheckEdit(DashboardWinStringId.FormatRuleShowBarOnly, "ShowBarOnly", barOptions.ShowBarOnly, OnCheckedChanged, out lcItem);
		}
		protected CheckEdit CreateCheckEdit(DashboardWinStringId id, string name, object editValue, EventHandler checkedChanged, out LayoutControlItem lcItem) {
			CheckEdit checkEdit = new DevExpress.XtraEditors.CheckEdit() { Name = String.Format("ce{0}", name) };
			checkEdit.Properties.Caption = DashboardWinLocalizer.GetString(id);
			checkEdit.EditValue = editValue;
			checkEdit.CheckedChanged += checkedChanged;
			lcItem = rootGroup.AddItem(string.Empty, checkEdit);
			lcItem.Name = String.Format("lci{0}", name);
			lcItem.TextVisible = false;
			return checkEdit;
		}
		public bool DrawAxis { get { return ceDrawAxis.Checked; } }
		public bool AllowNegativeAxis { get { return ceAllowNegativeAxis.Checked; } }
		public bool ShowBarOnly { get { return ceShowBarOnly.Checked; } }
		public event EventHandler OptionsChanged {
			add { optionsChanged += value; }
			remove { optionsChanged -= value; }
		}
		void OnCheckedChanged(object sender, EventArgs e) {
			if(optionsChanged != null)
				optionsChanged(this, e);
		}
	}
}
