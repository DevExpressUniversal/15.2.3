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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.DashboardWin.Localization;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class FormatRuleControlBase : DashboardUserControl, IFormatRuleControlView {
		protected static void ConstraintControl(Control control) {
			control.MinimumSize = control.Size;
		}
		static object stateUpdated = new object();
		static object itemApplyToChanged = new object();
		ComboBoxEdit cbApplyTo;
		CheckEdit ceApplyToRow;
		CheckEdit ceApplyToColumn;
		IntersectionLevelPanel intersectionLevelPanel;
		protected virtual bool IsValid { get { return true; } }
		protected LayoutControlGroup RootGroup { get { return layoutControl.Root; } }
		protected override XtraBars.BarManager BarManager { get { return barManager; } }
		protected FormatRuleControlBase()
			: base() {
			InitializeComponent();
#if DEBUGTEST
			layoutControl.AllowCustomization = true;
#endif
		}
		protected override IXtraResizableControl GetInnerIXtraResizableControl() {
			return layoutControl;
		}
		protected virtual void Initialize(IFormatRuleControlViewInitializationContext initializationContext) {
			this.lblDescription.Text = initializationContext.Description;
			this.cbApplyTo = EditorsInitializer.CreateCombo("cbApplyTo", OnItemApplyToChanged);
			if(initializationContext.IsApplyToReadOnly) {
				this.cbApplyTo.Enabled = false;
				this.ceApplyToRow = null;
			} else {
				this.ceApplyToRow = new CheckEdit() {
					Name = "ceApplyToRow",
					Text = DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleApplyToRow)
				};
				this.ceApplyToRow.EditValueChanged += OnStateChanged;
			}
			if(initializationContext.IsIntersectionLevel) {
				if(initializationContext.IsApplyToColumnSupported) {
					this.ceApplyToColumn = new CheckEdit() {
						Name = "ceApplyToColumn",
						Text = DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleApplyToColumn)
					};
					this.ceApplyToColumn.EditValueChanged += OnStateChanged;
				}
				this.intersectionLevelPanel = new IntersectionLevelPanel(OnStateChanged);
			}
		}
		protected void OnStateChanged(object sender, EventArgs e) {
			RaiseStateUpdated();
		}
		protected void RaiseStateUpdated() {
			ViewStateChangedEventHandler handler = (ViewStateChangedEventHandler)Events[stateUpdated];
			if(handler != null) {
				handler(this, new ViewStateChangedEventArgs());
			}
		}
		protected void AddGroupApplyTo(BaseLayoutItem baseItem, InsertType insertType) {
			AddGroupApplyTo(baseItem, insertType, null);
		}
		protected void AddGroupApplyTo(BaseLayoutItem baseItem, InsertType insertType, LayoutControlGroup groupOwner) {
			if(groupOwner == null) groupOwner = RootGroup;
			LayoutControlGroup lcgApplyTo = groupOwner.AddGroup(baseItem, insertType);
			lcgApplyTo.Name = "lcgApplyTo";
			lcgApplyTo.EnableIndentsWithoutBorders = Utils.DefaultBoolean.True;
			lcgApplyTo.GroupBordersVisible = false;
			lcgApplyTo.Padding = new XtraLayout.Utils.Padding(0, 0, 10, 0);
			lcgApplyTo.Spacing = new XtraLayout.Utils.Padding(0);
			lcgApplyTo.TextVisible = false;
			LayoutControlItem lciApplyTo = lcgApplyTo.AddItem(DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleApplyTo), this.cbApplyTo);
			lciApplyTo.Name = "lciApplyTo";
			lciApplyTo.Padding = new XtraLayout.Utils.Padding(2, 2, 2, 8);
			lciApplyTo.TextAlignMode = TextAlignModeItem.CustomSize;
			lciApplyTo.TextLocation = Utils.Locations.Top;
			lciApplyTo.TextToControlDistance = 0;
			LayoutControlItem lciApplyToRow = lcgApplyTo.AddItem(string.Empty, this.ceApplyToRow);
			lciApplyToRow.Name = "lciApplyToRow";
			lciApplyToRow.TextVisible = false;
			if(this.ceApplyToRow == null) lciApplyToRow.Visibility = LayoutVisibility.Never;
			if(this.ceApplyToColumn != null) {
				LayoutControlItem lciApplyToColumn = lcgApplyTo.AddItem(string.Empty, this.ceApplyToColumn, lciApplyToRow, InsertType.Right);
				lciApplyToColumn.Name = "lciApplyToColumn";
				lciApplyToColumn.TextVisible = false;
			}
			if(this.intersectionLevelPanel != null) {
				this.intersectionLevelPanel.AddGroup(lcgApplyTo, InsertType.Top, groupOwner);
			}
		}
		void OnItemApplyToChanged(object sender, EventArgs e) {
			RaiseItemApplyToChanged();
			OnStateChanged(sender, e);
		}
		void RaiseItemApplyToChanged() {
			EventHandler handler = (EventHandler)Events[itemApplyToChanged];
			if(handler != null) {
				handler(this, EventArgs.Empty);
			}
		}
		#region IFormatRuleBaseControlView Members
		event ViewStateChangedEventHandler IFormatRuleBaseControlView.StateUpdated {
			add { Events.AddHandler(stateUpdated, value); }
			remove { Events.RemoveHandler(stateUpdated, value); }
		}
		bool IFormatRuleBaseControlView.IsValid { get { return IsValid; } }
		void IFormatRuleBaseControlView.Initialize(IFormatRuleBaseViewInitializationContext initializationContext) {
			layoutControl.BeginUpdate();
			try {
				Initialize((IFormatRuleControlViewInitializationContext)initializationContext);
			} finally {
				layoutControl.EndUpdate();
			}
		}
		#endregion
		#region IFormatRuleControlView Members
		event EventHandler IFormatRuleControlView.ItemApplyToChanged {
			add { Events.AddHandler(itemApplyToChanged, value); }
			remove { Events.RemoveHandler(itemApplyToChanged, value); }
		}
		void IFormatRuleControlView.SetItemsApplyTo(IEnumerable items) {
			cbApplyTo.Properties.BeginUpdate();
			cbApplyTo.Properties.Items.Clear();
			foreach(object item in items) {
				cbApplyTo.Properties.Items.Add(item);
			}
			cbApplyTo.Properties.EndUpdate();
		}
		bool? IFormatRuleControlView.ApplyToRow {
			get { return (ceApplyToRow == null || !ceApplyToRow.Enabled) ? null : (bool?)ceApplyToRow.Checked; }
			set {
				if(ceApplyToRow != null) {
					ceApplyToRow.Enabled = value.HasValue;
					ceApplyToRow.Checked = value.HasValue ? value.Value : false;
				}
			}
		}
		bool? IFormatRuleControlView.ApplyToColumn {
			get { return (ceApplyToColumn == null || !ceApplyToColumn.Enabled) ? null : (bool?)ceApplyToColumn.Checked; }
			set {
				if(ceApplyToColumn != null) {
					ceApplyToColumn.Enabled = value.HasValue;
					ceApplyToColumn.Checked = value.HasValue ? value.Value : false;
				}
			}
		}
		int IFormatRuleControlView.SelectedItemApplyToIndex {
			get { return cbApplyTo.SelectedIndex; }
			set { cbApplyTo.SelectedIndex = value; }
		}
		IFormatRuleIntersectionLevel IFormatRuleControlView.IntersectionLevel {
			get { return this.intersectionLevelPanel; }
		}
		#endregion
	}
	class IntersectionLevelPanel : IFormatRuleIntersectionLevel {
		readonly ComboBoxEdit cbLevelMode;
		readonly ComboBoxEdit cbLevelRow;
		readonly ComboBoxEdit cbLevelColumn;
		event EventHandler modeChanged;
		public IntersectionLevelPanel(EventHandler handler) {
			this.cbLevelMode = EditorsInitializer.CreateCombo("cbLevelMode", handler);
			this.cbLevelMode.SelectedIndexChanged += OnModeSelectedIndexChanged;
			this.cbLevelRow = EditorsInitializer.CreateCombo("cbLevelRow", handler);
			this.cbLevelColumn = EditorsInitializer.CreateCombo("cbLevelColumn", handler);
		}
		public void AddGroup(BaseLayoutItem baseItem, InsertType insertType, LayoutControlGroup groupOwner) {
			LayoutControlGroup lcgLevel = groupOwner.AddGroup(baseItem, insertType);
			lcgLevel.Name = "lcgLevel";
			lcgLevel.EnableIndentsWithoutBorders = Utils.DefaultBoolean.True;
			lcgLevel.GroupBordersVisible = false;
			lcgLevel.Padding = new XtraLayout.Utils.Padding(0, 0, 10, 0);
			lcgLevel.Spacing = new XtraLayout.Utils.Padding(0, 0, 0, 0);
			lcgLevel.TextVisible = false;
			LayoutControlItem lciLevelMode = lcgLevel.AddItem(DashboardWinLocalizer.GetString(DashboardWinStringId.IntersectionLevelModeCaption), this.cbLevelMode);
			lciLevelMode.Name = "lciLevelMode";
			lciLevelMode.Padding = new XtraLayout.Utils.Padding(2, 2, 2, 2);
			lciLevelMode.TextAlignMode = TextAlignModeItem.CustomSize;
			lciLevelMode.TextLocation = Utils.Locations.Top;
			lciLevelMode.TextToControlDistance = 0;
			LayoutControlItem lciLevelRow = lcgLevel.AddItem(DashboardWinLocalizer.GetString(DashboardWinStringId.IntersectionLevelRowCaption), this.cbLevelRow);
			lciLevelRow.Name = "lciLevelRow";
			lciLevelRow.Padding = new XtraLayout.Utils.Padding(2, 4, 2, 8);
			lciLevelRow.TextAlignMode = TextAlignModeItem.CustomSize;
			lciLevelRow.TextLocation = Utils.Locations.Top;
			lciLevelRow.TextToControlDistance = 0;
			LayoutControlItem lciLevelColumn = lcgLevel.AddItem(DashboardWinLocalizer.GetString(DashboardWinStringId.IntersectionLevelColumnCaption), this.cbLevelColumn, lciLevelRow, InsertType.Right);
			lciLevelColumn.Name = "lciLevelColumn";
			lciLevelColumn.Padding = new XtraLayout.Utils.Padding(4, 2, 2, 8);
			lciLevelColumn.TextAlignMode = TextAlignModeItem.CustomSize;
			lciLevelColumn.TextLocation = Utils.Locations.Top;
			lciLevelColumn.TextToControlDistance = 0;
		}
		void OnModeSelectedIndexChanged(object sender, EventArgs e) {
			if(modeChanged != null)
				modeChanged(this, EventArgs.Empty);
		}
		#region IFormatRuleIntersectionLevel Members
		event EventHandler IFormatRuleIntersectionLevel.ModeChanged {
			add { modeChanged += value; }
			remove { modeChanged -= value; }
		}
		int IFormatRuleIntersectionLevel.SelectedModeIndex {
			get { return this.cbLevelMode.SelectedIndex; }
			set { this.cbLevelMode.SelectedIndex = value; }
		}
		int IFormatRuleIntersectionLevel.SelectedAxis1ItemIndex {
			get { return cbLevelColumn.SelectedIndex; }
			set { cbLevelColumn.SelectedIndex = value; }
		}
		int IFormatRuleIntersectionLevel.SelectedAxis2ItemIndex {
			get { return cbLevelRow.SelectedIndex; }
			set { cbLevelRow.SelectedIndex = value; }
		}
		void IFormatRuleIntersectionLevel.SetModes(IEnumerable modes) {
			EditorsInitializer.FillComboItems(cbLevelMode, modes);
		}
		void IFormatRuleIntersectionLevel.SetAxis1Items(IEnumerable items) {
			EditorsInitializer.FillComboItems(cbLevelColumn, items);
		}
		void IFormatRuleIntersectionLevel.SetAxis2Items(IEnumerable items) {
			EditorsInitializer.FillComboItems(cbLevelRow, items);
		}
		void IFormatRuleIntersectionLevel.Enable(bool enabled) {
			cbLevelRow.Enabled = enabled;
			cbLevelColumn.Enabled = enabled;
		}
		#endregion
	}
	static class EditorsInitializer {
		public static ComboBoxEdit CreateCombo(string name, EventHandler handler) {
			ComboBoxEdit combo = new ComboBoxEdit() { Name = name };
			combo.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			combo.EditValueChanged += handler;
			return combo;
		}
		public static void FillComboItems(ComboBoxEdit combo, IEnumerable items) {
			combo.Properties.BeginUpdate();
			combo.Properties.Items.Clear();
			var selectedItem = combo.SelectedItem;
			foreach(object item in items) {
				combo.Properties.Items.Add(item);
			}
			int index = combo.Properties.Items.IndexOf(selectedItem);
			combo.SelectedIndex = index > 0 ? index : 0;
			combo.Properties.EndUpdate();
		}
	}
}
