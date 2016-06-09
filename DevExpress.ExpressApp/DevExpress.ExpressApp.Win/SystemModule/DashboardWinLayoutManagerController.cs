#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Windows.Forms;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class DashboardWinLayoutManagerController : ViewController<DashboardView> {
		private LayoutControl layoutControl;
		private BaseButton addButton;
		private BaseButton removeButton;
		private LayoutManager LayoutManager {
			get { return (View != null) ? View.LayoutManager : null; }
		}
		private CustomizationForm CustomizationForm {
			get { return (layoutControl != null) ? layoutControl.CustomizationForm as CustomizationForm : null; }
		}
		private void DeleteEditorForLayoutControlItem(LayoutControlItem item) {
			((DashboardView)View).RemoveItem(item.Name);
		}
		private void DeleteEditorsForLayoutControlGroup(LayoutControlGroup layoutControlGroup) {
			while(layoutControlGroup.Items.Count > 0) {
				BaseLayoutItem item = layoutControlGroup.Items[0];
				if(item is LayoutControlItem) {
					DeleteEditorForLayoutControlItem((LayoutControlItem)item);
				}
				else if(item is LayoutControlGroup) {
					DeleteEditorsForLayoutControlGroup((LayoutControlGroup)item);
					layoutControlGroup.Items.RemoveAt(0);
				}
			}
		}
		private void DeleteEditorForTabbedControlGroup(TabbedControlGroup tabbedControlGroup) {
			foreach(BaseLayoutItem item in tabbedControlGroup.TabPages) {
				DeleteEditorsForLayoutItem(item);
			}
		}
		private void DeleteEditorsForLayoutItem(BaseLayoutItem item) {
			if(item is LayoutControlItem) {
				DeleteEditorForLayoutControlItem((LayoutControlItem)item);
			}
			else if(item is LayoutControlGroup) {
				DeleteEditorsForLayoutControlGroup((LayoutControlGroup)item);
			}
			else if(item is TabbedControlGroup) {
				DeleteEditorForTabbedControlGroup((TabbedControlGroup)item);
			}
		}
		private void EnableRemoveButtonByCurrentSelection() {
			removeButton.Enabled = ((ListBoxControl)ActiveListBox).SelectedItem is XafLayoutControlItem;
		}
		private void ActiveListBox_MouseClick(object sender, MouseEventArgs e) {
			EnableRemoveButtonByCurrentSelection();
		}
		private void ActiveListBox_SelectedIndexChanged(object sender, EventArgs e) {
			EnableRemoveButtonByCurrentSelection();
		}
		private void LayoutControl_ShowCustomization(object sender, EventArgs e) {
			if(CustomizationForm != null) {
				CustomizationForm.hiddenItemsList1.AllowResetFocusOnMouseLeave = false;
				InsertButtons();
				EnableRemoveButtonByCurrentSelection();
				((ListBoxControl)ActiveListBox).SelectedIndexChanged += new EventHandler(ActiveListBox_SelectedIndexChanged);
				ActiveListBox.MouseClick += new MouseEventHandler(ActiveListBox_MouseClick);
			}
		}
		private void layoutControl_HideCustomization(object sender, EventArgs e) {
			if(CustomizationForm != null) {
				CustomizationForm.hiddenItemsList1.PressedItemIndex = -1;
				DeleteButtons();
			}
			UnsubscribeListBoxEvents();
		}
		private void UnsubscribeListBoxEvents() {
			if(ActiveListBox != null) {
				((ListBoxControl)ActiveListBox).SelectedIndexChanged -= new EventHandler(ActiveListBox_SelectedIndexChanged);
				ActiveListBox.MouseClick -= new MouseEventHandler(ActiveListBox_MouseClick);
			}
		}
		private void layoutControl_ResetLayout(object sender, EventArgs e) {
			Cursor cursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			layoutControl.BeginUpdate();
			((ModelNode)View.Model.Layout).Undo();
			((ModelNode)View.Model.Items).Undo();
			View.BreakLinksToControls();
			View.LoadModel();
			View.CreateControls();
			layoutControl.EndUpdate();
			Cursor.Current = cursor;
		}
		private void view_ControlsCreated(object sender, EventArgs e) {
			layoutControl = (XafLayoutControl)LayoutManager.Container;
			layoutControl.ShowCustomization += new EventHandler(LayoutControl_ShowCustomization);
			layoutControl.HideCustomization += new EventHandler(layoutControl_HideCustomization);
		}
		private void dialogController_Accepting(object sender, DialogControllerAcceptingEventArgs e) {
			layoutControl.BeginUpdate();
			((DashboardOrganizer)((DialogController)sender).Window.View.CurrentObject).SaveDashboardChangesToModel();
			View.LoadModel();
			layoutControl.EndUpdate();
		}
		private void addButton_Click(object sender, EventArgs e) {
			View.SaveModel();
			DashboardOrganizer dashboardOrganizer = new DashboardOrganizer(View.Model);
			DetailView dashboardOrganizerView = Application.CreateDetailView(Application.CreateObjectSpace(dashboardOrganizer.GetType()), dashboardOrganizer, true);
			ShowViewParameters parameters = new ShowViewParameters(dashboardOrganizerView);
			parameters.TargetWindow = TargetWindow.NewModalWindow;
			parameters.Context = TemplateContext.PopupWindow;
			DialogController dialogController = Application.CreateController<DialogController>();
			dialogController.Accepting += new EventHandler<DialogControllerAcceptingEventArgs>(dialogController_Accepting);
			parameters.Controllers.Add(dialogController);
			Application.ShowViewStrategy.ShowView(parameters, new ShowViewSource(Frame, null));
		}
		private void removeButton_Click(object sender, EventArgs e) {
			BaseLayoutItem layoutItem = (BaseLayoutItem)CustomizationForm.hiddenItemsList1.SelectedItem;
			layoutControl.HiddenItems.RemoveAt(layoutControl.HiddenItems.IndexOf(layoutItem));
			DeleteEditorsForLayoutItem(layoutItem);
			CustomizationForm.hiddenItemsList1.PressedItemIndex = -1;
			CustomizationForm.Update();
		}
		private void CreateButtons() {
			addButton = new BaseButton();
			addButton.Name = "AddButton";
			addButton.Text = CaptionHelper.GetLocalizedText("DialogButtons", "Add") + "...";
			addButton.Dock = DockStyle.Bottom;
			addButton.Click += new EventHandler(addButton_Click);
			removeButton = new BaseButton();
			removeButton.Name = "RemoveButton";
			removeButton.Text = CaptionHelper.GetLocalizedText("DialogButtons", "Remove");
			removeButton.Dock = DockStyle.Bottom;
			removeButton.Click += new EventHandler(removeButton_Click);
		}
		private void AddButtonsToCustomizationForm() {
			CustomizationForm.layoutControl1.Controls.Add(this.removeButton);
			CustomizationForm.layoutControl1.Controls.Add(this.addButton);
			LayoutControlItem addButtonLayoutItem = CustomizationForm.hiddenItemsGroup.AddItem();
			addButtonLayoutItem.Control = this.addButton;
			addButtonLayoutItem.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 0, 0);
			addButtonLayoutItem.TextVisible = false;
			LayoutControlItem removeButtonLayoutItem = CustomizationForm.hiddenItemsGroup.AddItem();
			removeButtonLayoutItem.Control = this.removeButton;
			removeButtonLayoutItem.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 0, 5);
			removeButtonLayoutItem.TextVisible = false;
		}
		private void InsertButtons() {
			CreateButtons();
			AddButtonsToCustomizationForm();
		}
		private void DeleteButtons() {
			if(addButton != null) {
				addButton.Click -= new EventHandler(addButton_Click);
				addButton.Dispose();
				addButton = null;
			}
			if(removeButton != null) {
				removeButton.Click -= new EventHandler(removeButton_Click);
				removeButton.Dispose();
				removeButton = null;
			}
		}
		private void UpdateControllerActivity() {
			IModelOptionsDashboards dashboardOptions = Application.Model.Options as IModelOptionsDashboards;
			if(dashboardOptions != null) {
				this.Active.SetItemValue(DashboardCustomizationController.RuntimeDashboardCustomizationEnabledKey, dashboardOptions.Dashboards.EnableCustomization);
			}
		}
		protected Control ActiveListBox {
			get { return CustomizationForm != null ? CustomizationForm.hiddenItemsList1 : null; }
		}
		protected override void OnActivated() {
			base.OnActivated();
			layoutControl = (XafLayoutControl)LayoutManager.Container;
			if(layoutControl != null) {
				layoutControl.ShowCustomization += new EventHandler(LayoutControl_ShowCustomization);
				layoutControl.HideCustomization += new EventHandler(layoutControl_HideCustomization);
				layoutControl.DefaultLayoutLoaded += new EventHandler(layoutControl_ResetLayout);
			}
			else {
				View.ControlsCreated += new EventHandler(view_ControlsCreated);
			}
		}
		protected override void OnDeactivated() {
			UnsubscribeListBoxEvents();
			View.ControlsCreated -= new EventHandler(view_ControlsCreated);
			if(layoutControl != null) {
				layoutControl.ShowCustomization -= new EventHandler(LayoutControl_ShowCustomization);
				layoutControl.HideCustomization -= new EventHandler(layoutControl_HideCustomization);
				layoutControl.DefaultLayoutLoaded -= new EventHandler(layoutControl_ResetLayout);
				if(layoutControl.IsHandleCreated == true) { layoutControl.HideCustomizationForm(); }
				layoutControl = null;
			}
			base.OnDeactivated();
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			UpdateControllerActivity();
		}
		protected override void OnViewChanged() {
			base.OnViewChanged();
			UpdateControllerActivity();
		}
		public DashboardWinLayoutManagerController() {
			TypeOfView = typeof(DashboardView);
		}
	}
	public class WinFocusDashboardControlController : WindowController {
		private void Window_TemplateViewChanged(object sender, EventArgs e) {
			DashboardView dashboardView = Window.View as DashboardView;
			if(dashboardView != null && dashboardView.IsControlCreated) {
				((Control)dashboardView.Control).Focus();
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			Window.TemplateViewChanged += new EventHandler(Window_TemplateViewChanged);
		}
		protected override void OnDeactivated() {
			Window.TemplateViewChanged -= new EventHandler(Window_TemplateViewChanged);
			base.OnDeactivated();
		}
	}
}
