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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class FormatRuleControlStyleBase : FormatRuleControlBase, IFormatRuleControlStyleView {
		readonly bool useTabbedFormatTypeSelector = true;
		readonly FormatRuleStyleSettingsControl appearanceControl;
		readonly FormatRuleStyleSettingsControl iconsControl;
		TabbedControlGroup lciTab;
		LayoutControlGroup lcgValuePanel;
		LayoutControlGroup lcgMainPanel;
		LayoutControlItem lciFormatStyleEditor;
		ComboBoxEdit cbFormatStyleType;
		protected LayoutControlGroup ValuePanelGroup { get { return lcgValuePanel; } }
		protected LayoutControlGroup MainPanelGroup { get { return lcgMainPanel; } }
		protected FormatRuleStyleSettingsControl StyleControl1 { get { return appearanceControl; } }
		protected FormatRuleStyleSettingsControl StyleControl2 { get { return iconsControl; } }
		protected TabbedControlGroup LciTab { get { return lciTab; } }
		protected virtual string StyleControl1Caption { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleStyleAppearance); } }
		protected virtual string StyleControl2Caption { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleStyleIcons); } }
		bool IsAppearanceSelected {
			get {
				int selectedIndex = useTabbedFormatTypeSelector ? lciTab.SelectedTabPageIndex : cbFormatStyleType.SelectedIndex;
				return selectedIndex == 0;
			}
		}
		public FormatRuleControlStyleBase() {
			InitializeComponent();
			this.appearanceControl = CreateStyleControl1();
			this.iconsControl = CreateStyleControl2();
		}
		protected virtual FormatRuleStyleSettingsControl CreateStyleControl1() {
			return new FormatRuleStyleSettingsControl(StyleMode.Appearance, false, LookAndFeel) { Name = "appearanceControl" };
		}
		protected virtual FormatRuleStyleSettingsControl CreateStyleControl2() {
			return new FormatRuleStyleSettingsControl(StyleMode.Icon, false, LookAndFeel) { Name = "iconsControl" };
		}
		protected override void Initialize(IFormatRuleControlViewInitializationContext initializationContext) {
			base.Initialize(initializationContext);
			LayoutControlGroup groupOwner = RootGroup;
			this.lcgValuePanel = groupOwner.AddGroup();
			this.lcgValuePanel.Name = "lcgValuePanel";
			this.lcgValuePanel.GroupBordersVisible = false;
			this.lcgValuePanel.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgValuePanel.Padding = new XtraLayout.Utils.Padding(0, 0, 0, 16);
			this.lcgValuePanel.TextVisible = false;
			this.lcgMainPanel = groupOwner.AddGroup();
			this.lcgMainPanel.GroupBordersVisible = false;
			this.lcgMainPanel.Name = "lcgMainPanel";
			this.lcgMainPanel.Padding = new XtraLayout.Utils.Padding(0);
			this.lcgMainPanel.TextVisible = false;
			((IStyleContainerManager)appearanceControl).StyleChanged += OnStyleChanged;
			((IStyleContainerManager)iconsControl).StyleChanged += OnStyleChanged;
			this.iconsControl.MinimumSize = this.appearanceControl.MinimumSize;
			if(useTabbedFormatTypeSelector) {
				appearanceControl.BorderStyle = iconsControl.BorderStyle = BorderStyles.NoBorder;
				this.lciTab = lcgMainPanel.AddTabbedGroup();
				this.lciTab.Name = "lciTab";
				this.lciTab.Padding = new XtraLayout.Utils.Padding(0);
				LayoutControlGroup lcgTabPageAppearance = lciTab.AddTabPage(StyleControl1Caption) as LayoutControlGroup;
				lcgTabPageAppearance.Padding = new XtraLayout.Utils.Padding(0);
				lcgTabPageAppearance.Name = "lcgTabPageAppearance";				
				LayoutControlItem lciAppearance = lcgTabPageAppearance.AddItem(string.Empty, this.appearanceControl);
				lciAppearance.Name = "lciAppearance";
				lciAppearance.Padding = new XtraLayout.Utils.Padding(0);
				lciAppearance.TextVisible = false;
				LayoutControlGroup lcgTabPageIcons = lciTab.AddTabPage(StyleControl2Caption) as LayoutControlGroup;
				lcgTabPageIcons.Name = "lcgTabPageIcons";
				lcgTabPageIcons.Padding = new XtraLayout.Utils.Padding(0);
				LayoutControlItem lciIcons = lcgTabPageIcons.AddItem(string.Empty, this.iconsControl);
				lciIcons.Name = "lciIcons";
				lciIcons.Padding = new XtraLayout.Utils.Padding(0);
				lciIcons.TextVisible = false;
				AddGroupApplyTo(this.lciTab, XtraLayout.Utils.InsertType.Bottom, lcgMainPanel);
			} else {
				this.lciFormatStyleEditor = lcgMainPanel.AddItem();
				this.lciFormatStyleEditor.Name = "lciFormatStyleEditor";			
				this.cbFormatStyleType = new ComboBoxEdit();
				this.cbFormatStyleType.Name = "cbFormatStyleType";
				this.cbFormatStyleType.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;				
				this.cbFormatStyleType.Properties.Items.AddRange(new object[] { StyleControl1Caption, StyleControl2Caption });
				this.cbFormatStyleType.SelectedIndexChanged += OnFormatStyleTypeChanged;
				this.cbFormatStyleType.SelectedIndex = 0;
				LayoutControlItem lciFormatStyleType = lcgMainPanel.AddItem(DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleFormatStyle), this.cbFormatStyleType, this.lciFormatStyleEditor, XtraLayout.Utils.InsertType.Top);
				lciFormatStyleType.Name = "lciFormatStyleType";
				lciFormatStyleType.Padding = new XtraLayout.Utils.Padding(2, 2, 2, 8);
				lciFormatStyleType.TextLocation = Utils.Locations.Top;
				lciFormatStyleType.TextAlignMode = TextAlignModeItem.CustomSize;
				lciFormatStyleType.TextSize = new Size(70, 20);
				lciFormatStyleType.TextToControlDistance = 0;
				UpdateControls();
				AddGroupApplyTo(this.lciFormatStyleEditor, XtraLayout.Utils.InsertType.Bottom, lcgMainPanel);
			}
		}
		protected virtual void OnStyleChanged(object sender, StyleSettingsContainerItemChangedEventArgs e) {		   
			IStyleContainerManager styleContainerManager = !IsAppearanceSelected ? appearanceControl : iconsControl;
			styleContainerManager.Style = null;
			RaiseStateUpdated();
		}
		void OnFormatStyleTypeChanged(object sender, EventArgs e) {
			UpdateControls();
			RaiseStateUpdated();
		}
		void UpdateControls() {
			if(!useTabbedFormatTypeSelector) {
				bool isAppearanceSelected = IsAppearanceSelected;
				RootGroup.BeginUpdate();
				SetFormatStyleEditorControlVisible(appearanceControl, isAppearanceSelected);
				SetFormatStyleEditorControlVisible(iconsControl, !isAppearanceSelected);
				RootGroup.EndUpdate();
			}
		}
		void SetFormatStyleEditorControlVisible(Control control, bool visible) {
			if(visible) {
				ConstraintControl(control);
				lciFormatStyleEditor.Control = control;
				lciFormatStyleEditor.Update();
			} else {
				if(lciFormatStyleEditor.Control == control) {
					lciFormatStyleEditor.Control = null;
				}
			}
			lciFormatStyleEditor.TextVisible = false;
			control.Visible = visible;
		}
		int CalcStyleTypeIndex(StyleSettingsContainer style) {
			return style == null || style.IsEmpty || style.Mode == StyleMode.Appearance ? 0 : 1;
		}
		#region IFormatRuleControlStyleView Members
		StyleSettingsContainer IFormatRuleControlStyleView.Style {
			get {
				IStyleContainerManager styleContainerManager = IsAppearanceSelected ? appearanceControl : iconsControl;
				if(styleContainerManager.Style == null)
					styleContainerManager = !IsAppearanceSelected ? appearanceControl : iconsControl;
				return styleContainerManager.Style; 
			}
			set {
				int selectedFormatType = CalcStyleTypeIndex(value);
				if(useTabbedFormatTypeSelector) {
					this.lciTab.SelectedTabPage = this.lciTab.TabPages[selectedFormatType];
				} else {
					cbFormatStyleType.SelectedIndex = selectedFormatType;
					UpdateControls();
				}
				IStyleContainerManager styleContainerManager = IsAppearanceSelected ? appearanceControl : iconsControl;
				styleContainerManager.Style = value; 
			}
		}
		#endregion
	}
}
