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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization.Controls;
using DevExpress.XtraLayout.Localization;
namespace DevExpress.XtraLayout.Customization {
	public enum ResizingState { Begin, ShowStatePlus, ShowStateMinus, AgreeWithSize } ;
	public class SizeInfo {
		public int Height { get; set; }
		public Size AutoSize { get; set; }
		public int Index { get; set; }
	}
	public static class ControlHierarchyHelper {
		public static List<Control> getControlsRecursive(Control control, List<Control> list) {
			if(control.Controls.Count > 0) {
				for(int i = 0; i < control.Controls.Count; i++) {
					getControlsRecursive(control.Controls[i], list);
				}
			} else list.Add(control);
			return list;
		}
	}
	public class InteractionsAndControls {
		public BaseInteraction interaction { get; set; }
		public Control control { get; set; }
		public InteractionsAndControls(BaseInteraction inter, Control cnrl) {
			interaction = inter;
			control = cnrl;
		}
	}
	public class UserInterectionVisualHelper : IDisposable {
		Form Owner { get; set; }
		Size visualizerFormLayoutSize { get { return new Size(204, 900); } }
		Control oldVisualizerForm;
		Label label;
		int rows;
		bool fullCustomizationPrevState = false;
		int widghtFullCustomization = 300;
		FormWindowState cfWindowState;
		List<InteractionsAndControls> interactionControls;
		bool groupLabel;
		int itemsCount;
		bool emptyGroup;
		EventHandler executeInteraction;
		EventHandler buttonChangeCheckedState;
		ToolTip tt;
		bool fullCustomizationState;
		bool leftButtonChangedCheckedState = false;
		const int elementHeight = 40;
		public FlowLayoutPanel visualizerFormLayout { get; set; }
		public ILayoutControl Layout { get; set; }
		public UserInterectionVisualHelper(Form owner, ILayoutControl layout) {
			Owner = owner;
			Layout = layout;
			itemsCount = 1;
			tt = new ToolTip();
			interactionControls = new List<InteractionsAndControls>();
			if(Layout.CustomizationForm != null) {
				if(!(Layout.CustomizationForm is CustomizationForm)) {
					throw new ArgumentException("This mode is incompatible with custom customization forms");
				}
				cfWindowState = layout.CustomizationForm.WindowState;
				Layout.CustomizationForm.LocationChanged += CustomizationForm_LocationChanged;
				Layout.CustomizationForm.ClientSizeChanged += CustomizationForm_ClientSizeChanged;
				((CustomizationForm)layout.CustomizationForm).MinimumSize = new Size(0, 500);
				Utils.LayoutGroupItemCollection selItems = ((CustomizationForm)layout.CustomizationForm).buttonsPanelItem.Parent.Items;
				for(int i = 0; i < selItems.ItemCount; i++) {
					if(selItems[i] is EmptySpaceItem) {
						if(!(selItems[i] is SplitterItem)) {
							selItems[i].Dispose();
						}
					}
				}
			}
		}
		protected bool AllowSaveWidth {
			get { return (((CustomizationForm)Layout.CustomizationForm).Flags & CustomizationFormStates.AllowSaveWidth) != 0; }
			set {
				if(value) ((CustomizationForm)Layout.CustomizationForm).Flags |= CustomizationFormStates.AllowSaveWidth;
				else ((CustomizationForm)Layout.CustomizationForm).Flags &= ~CustomizationFormStates.AllowSaveWidth;
			}
		}
		void CustomizationForm_ClientSizeChanged(object sender, EventArgs e) {
			if(visualizerFormLayout != null && fullCustomizationState && AllowSaveWidth) {
				if(Layout.CustomizationForm.ClientSize.Width - visualizerFormLayout.ClientSize.Width < 300) {
					widghtFullCustomization = 300;
				} else widghtFullCustomization = Layout.CustomizationForm.ClientSize.Width - visualizerFormLayout.ClientSize.Width;
			}
			AllowSaveWidth = true;
		}
		void CustomizationForm_LocationChanged(object sender, EventArgs e) {
			if(Layout.CustomizationForm.WindowState != cfWindowState) {
				cfWindowState = Layout.CustomizationForm.WindowState;
				OnWindowStateChanged();
			}
		}
		void OnWindowStateChanged() {
			if(fullCustomizationState != true) {
				Layout.CustomizationForm.WindowState = FormWindowState.Normal;
			} else AllowSaveWidth = false;
		}
		public void HideCustomization(object sender, EventArgs e) {
			Layout.EnableCustomizationMode = false;
			Layout.LongPressControl.Enabled = true;
			sizeInfo.Clear();
			Dispose();
		}
		public void UpdateButtonEnable(List<BaseInteraction> interactions, bool fullCustomization) {
			for(int i = 0; i < interactionControls.Count; i++) {
				for(int j = 0; j < interactions.Count; j++) {
					if(interactionControls[i].interaction.Text == interactions[j].Text) {
						interactionControls[i].interaction = interactions[j];
						continue;
					}
				}
			}
			for(int i = 0; i < interactionControls.Count; i++) {
				if(interactionControls[i].control is CheckButton) {
					(interactionControls[i].control as CheckButton).Enabled = interactionControls[i].interaction.Enabled;
					(interactionControls[i].control as CheckButton).Checked = interactionControls[i].interaction.Checked;
				}
				if(interactionControls[i].control is SimpleButton) {
					(interactionControls[i].control as SimpleButton).Enabled = interactionControls[i].interaction.Enabled;
				}
			}
		}
		public void Show(List<BaseInteraction> interactions, bool fullCustomization) {
			fullCustomizationState = fullCustomization;
			executeInteraction = null;
			tt.RemoveAll();
			InitializeVisualizerFormLayout();
			interactionControls.Clear();
			if(Layout.CustomizationForm != null) {
				((CustomizationForm)Layout.CustomizationForm).FormClosing += HideCustomization;
				Layout.CustomizationForm.ResetActiveControl();
			}
			UpdateInterectionsForShow(interactions);
			SetSizeInfoToVisualizerFormLayoutTag();
			if(Layout.CustomizationForm != null) UpdateCustomizationFrom();
			if(interactions.Count > 0) {
				if(Layout.CustomizationForm != null) {
					Layout.CustomizationForm.Visible = true;
					SetCustomizationFormSize(fullCustomization);
					if(!fullCustomization) {
						ToQuickCustomization(Layout);
					} else ToFullCustomization(Layout);
				}
			}
			oldVisualizerForm = visualizerFormLayout;
		}
		void InitializeVisualizerFormLayout() {
			visualizerFormLayout = new DXFlowLayoutPanel();
			visualizerFormLayout.FlowDirection = FlowDirection.TopDown;
			visualizerFormLayout.BindingContext = Owner.BindingContext;
			visualizerFormLayout.MinimumSize = new Size(0, 460);
			visualizerFormLayout.AutoSize = true;
			visualizerFormLayout.Dock = WindowsFormsSettings.GetIsRightToLeft(Layout.Control) ? DockStyle.Right : DockStyle.Left;
		}
		void UpdateInterectionsForShow(List<BaseInteraction> interactions) {
			bool group = false;
			bool group2Level = false;
			DXFlowLayoutPanel buttonGroupHelper = null;
			DXFlowLayoutPanel buttonGroupHelper2Level = null;
			visualizerFormLayout.Padding = new Padding(0, 7, 0, 7);
			foreach(BaseInteraction interacation in interactions) {
				LayoutControlItem item = new LayoutControlItem();
				Control control = null;
				switch(interacation.State) {
					case GroupState.StartGroup:
						if(!group) {
							emptyGroup = true;
							buttonGroupHelper = CreateButtonGroupHelper();
							if(interacation.Text != null) {
								groupLabel = true;
								label = new Label();
								label.Text = interacation.Text;
								label.Margin = new Padding(0);
								label.MaximumSize = new Size(visualizerFormLayoutSize.Width, 15);
								buttonGroupHelper.Controls.Add(label);
								buttonGroupHelper.MinimumSize = buttonGroupHelper.MaximumSize = new Size(visualizerFormLayoutSize.Width, elementHeight + visualizerFormLayout.Margin.Top + label.Height);
							} else groupLabel = false;
						} else {
							rows++;
							group2Level = true;
							buttonGroupHelper2Level = CreateButtonGroupHelper2(buttonGroupHelper);
						}
						group = true;
						itemsCount = interacation.ItemsInRow;
						continue;
					case GroupState.EndGroup:
						if(group2Level) {
							group2Level = false;
							if(emptyGroup) rows--;
							continue;
						}
						group = false;
						rows = 0;
						if(emptyGroup && label != null && group) { label.Dispose(); }
						itemsCount = 1;
						continue;
					default:
						break;
				}
				AddControlToForm(group, group2Level, buttonGroupHelper, buttonGroupHelper2Level, interacation, control);
			}
		}
		void AddControlToForm(bool group, bool group2Level, DXFlowLayoutPanel buttonGroupHelper, DXFlowLayoutPanel buttonGroupHelper2Level, BaseInteraction interacation, Control control) {
			emptyGroup = false;
			executeInteraction = (s, e) => { leftButtonChangedCheckedState = true; interacation.Execute(); leftButtonChangedCheckedState = false; };
			buttonChangeCheckedState = (s, e) => { ChangeCheckedStateEvent(s); };
			control = CreateControlForIneraction(interacation, control, executeInteraction, tt, buttonChangeCheckedState);
			InteractionsAndControls pair = new InteractionsAndControls(interacation, control);
			interactionControls.Add(pair);
			control.Margin = new Padding(3, 3, 0, 0);
			if(!group) {
				control.MaximumSize = new Size((visualizerFormLayoutSize.Width - visualizerFormLayout.Margin.Horizontal) / itemsCount, elementHeight);
				control.MinimumSize = new Size((visualizerFormLayoutSize.Width - visualizerFormLayout.Margin.Horizontal) / itemsCount, elementHeight);
				visualizerFormLayout.Controls.Add(control);
			}
			if(group && !group2Level) {
				control.MaximumSize = new Size((visualizerFormLayoutSize.Width - visualizerFormLayout.Margin.Horizontal - ((itemsCount - 1) * (visualizerFormLayout.Margin.Right))) / itemsCount, elementHeight);
				control.MinimumSize = new Size((visualizerFormLayoutSize.Width - visualizerFormLayout.Margin.Horizontal - ((itemsCount - 1) * (visualizerFormLayout.Margin.Right))) / itemsCount, elementHeight);
				buttonGroupHelper.Controls.Add(control);
				visualizerFormLayout.Controls.Add(buttonGroupHelper);
			}
			if(group && group2Level) {
				control.MaximumSize = new Size((visualizerFormLayoutSize.Width - visualizerFormLayout.Margin.Horizontal - ((itemsCount - 1) * (visualizerFormLayout.Margin.Right))) / itemsCount, elementHeight);
				control.MinimumSize = new Size((visualizerFormLayoutSize.Width - visualizerFormLayout.Margin.Horizontal - ((itemsCount - 1) * (visualizerFormLayout.Margin.Right))) / itemsCount, elementHeight);
				buttonGroupHelper2Level.Controls.Add(control);
				visualizerFormLayout.Controls.Add(buttonGroupHelper);
				int labelHeight = 0;
				if(groupLabel) labelHeight = label.Height;
				buttonGroupHelper.MinimumSize = buttonGroupHelper.MaximumSize = new Size(visualizerFormLayoutSize.Width, (elementHeight * rows + visualizerFormLayout.Margin.Top * rows) + 10 + labelHeight);
				buttonGroupHelper.Controls.Add(buttonGroupHelper2Level);
			}
		}
		void UpdateCustomizationFrom() {
			if(!(Layout.CustomizationForm is CustomizationForm)) {
				throw new ArgumentException("This mode is incompatible with custom customization forms");
			}
			Layout.CustomizationForm.SuspendLayout();
			Layout.CustomizationForm.Controls.Add(visualizerFormLayout);
			if(oldVisualizerForm != null) {
				visualizerFormLayout.Bounds = oldVisualizerForm.Bounds;
				oldVisualizerForm.Parent = null;
				List<Control> lc = new List<Control>();
				ControlHierarchyHelper.getControlsRecursive(oldVisualizerForm, lc);
				foreach(Control control in lc) {
					control.Click -= executeInteraction;
					if(control is CheckButton) {
						(control as CheckButton).CheckedChanged -= buttonChangeCheckedState;
					}
				}
				oldVisualizerForm.Controls.Clear();
				oldVisualizerForm.Dispose();
				oldVisualizerForm = null;
			}
			Layout.CustomizationForm.ResumeLayout();
		}
		void ChangeCheckedStateEvent(object s) {
			if(!leftButtonChangedCheckedState) {
				leftButtonChangedCheckedState = true;
				if((s as CheckButton).Checked) {
					(s as CheckButton).Checked = false;
				} else (s as CheckButton).Checked = true;
				return;
			}
			leftButtonChangedCheckedState = false;
		}
		DXFlowLayoutPanel CreateButtonGroupHelper2(DXFlowLayoutPanel buttonGroupHelper) {
			DXFlowLayoutPanel buttonGroupHelper2Level = new DXFlowLayoutPanel();
			buttonGroupHelper2Level.Margin = new Padding(0);
			buttonGroupHelper.Padding = new Padding(0);
			buttonGroupHelper2Level.MinimumSize = buttonGroupHelper2Level.MaximumSize = new Size(visualizerFormLayoutSize.Width, elementHeight + visualizerFormLayout.Margin.Top);
			return buttonGroupHelper2Level;
		}
		DXFlowLayoutPanel CreateButtonGroupHelper() {
			DXFlowLayoutPanel buttonGroupHelper = new DXFlowLayoutPanel();
			buttonGroupHelper.Margin = new Padding(7, 0, 7, 0);
			buttonGroupHelper.Padding = new Padding(0);
			buttonGroupHelper.MinimumSize = buttonGroupHelper.MaximumSize = new Size(visualizerFormLayoutSize.Width, elementHeight + visualizerFormLayout.Margin.Top);
			return buttonGroupHelper;
		}
	   internal void SetCustomizationFormSize(bool fullCustomization) {
			if(fullCustomization) {
				((CustomizationForm)Layout.CustomizationForm).fullCustomization = true;
				((CustomizationForm)Layout.CustomizationForm).MaximumSize = new Size(3000, 3000);
				((CustomizationForm)Layout.CustomizationForm).MinimumSize = new Size(0, 500);
				if(!fullCustomizationPrevState) {
					((CustomizationForm)Layout.CustomizationForm).Size = new Size(((CustomizationForm)Layout.CustomizationForm).Size.Width + widghtFullCustomization, ((CustomizationForm)Layout.CustomizationForm).Height);
				}
				fullCustomizationPrevState = fullCustomization;
			}
			if(fullCustomizationPrevState && !fullCustomization) {
				((CustomizationForm)Layout.CustomizationForm).fullCustomization = false;
				((CustomizationForm)Layout.CustomizationForm).Size = new Size(((CustomizationForm)Layout.CustomizationForm).Size.Width - widghtFullCustomization, ((CustomizationForm)Layout.CustomizationForm).Size.Height);
				((CustomizationForm)Layout.CustomizationForm).MinimumSize = ((CustomizationForm)Layout.CustomizationForm).MaximumSize = new Size(((CustomizationForm)Layout.CustomizationForm).Width, ((CustomizationForm)Layout.CustomizationForm).Height);
				fullCustomizationPrevState = fullCustomization;
			}
		}
		void SetSizeInfoToVisualizerFormLayoutTag() {
			Form tempResizingStates = new Form();
			sizeInfo.Clear();
			tempResizingStates.Controls.Add(visualizerFormLayout);
			foreach(Control c in visualizerFormLayout.Controls) c.LocationChanged += c_LocationChanged;
			for(int i = 0; i < 1500; i++) tempResizingStates.Height = i;
			foreach(Control c in visualizerFormLayout.Controls) c.LocationChanged -= c_LocationChanged;
			if(sizeInfo.Count == 0) {
				int height = visualizerFormLayout.Controls[visualizerFormLayout.Controls.Count - 1].Bounds.Bottom - visualizerFormLayout.Controls[0].Bounds.Top + 14 + 40;
				sizeInfo.Add(new SizeInfo() { Height = height, AutoSize = new Size(visualizerFormLayout.Width + 14, height), Index = 2 });
			}
			visualizerFormLayout.Tag = SizeInfoFilter(sizeInfo);
		}
		List<SizeInfo> SizeInfoFilter(List<SizeInfo> sizeInfo) {
			if(!fullCustomizationState) {
				if(sizeInfo.Count > 0 && Layout.CustomizationForm != null) {
					List<int> templst = new List<int>();
					for(int i = 0; i < sizeInfo.Count; i++) {
						templst.Add(Math.Abs(Layout.CustomizationForm.Height - sizeInfo[i].AutoSize.Height));
					}
					int min = templst[0];
					int mini = 0;
					for(int i = 1; i < templst.Count; i++)
						if(templst[i] < min) {
							min = templst[i];
							mini = i;
						}
					((CustomizationForm)Layout.CustomizationForm).Flags = CustomizationFormStates.AllowNothing;
					((CustomizationForm)Layout.CustomizationForm).MinimumSize = ((CustomizationForm)Layout.CustomizationForm).MaximumSize = sizeInfo[mini].AutoSize;
					((CustomizationForm)Layout.CustomizationForm).Flags = CustomizationFormStates.AllowEverything;
				}
			}
			return sizeInfo;
		}
		Size lastSize = Size.Empty;
		List<SizeInfo> sizeInfo = new List<SizeInfo>();
		int marginHeight = 5;
		void c_LocationChanged(object sender, EventArgs e) {
			Form temp = (sender as Control).FindForm();
			Size currentSize = visualizerFormLayout.Size;
			if(currentSize != lastSize) {
				sizeInfo.Add(new SizeInfo() { Height = temp.Height, AutoSize = new Size(visualizerFormLayout.Width + 14, temp.Size.Height), Index = sizeInfo.Count });
				if(sender is Control && marginHeight > (sender as Control).Height) marginHeight = (sender as Control).Height;
				lastSize = currentSize;
			}
		}
		Control CreateControlForIneraction(BaseInteraction interaction, Control control, EventHandler myEvent, ToolTip tt, EventHandler buttonChangeCheckedState) {
			switch(interaction.EditorType) {
				case EditorTypes.TextPositionEditor:
					control = new TextPositionEditor();
					control.Dock = DockStyle.Fill;
					control.DataBindings.Add(new Binding("Align", interaction, "CurrentValue", false, DataSourceUpdateMode.OnPropertyChanged));
					break;
				case EditorTypes.ContentAlignmentEditor:
					control = new ControlPositionEditor();
					control.Dock = DockStyle.Fill;
					control.DataBindings.Add(new Binding("Align", interaction, "CurrentValue", false, DataSourceUpdateMode.OnPropertyChanged));
					break;
				case EditorTypes.Text:
					control = new TextEdit();
					control.DataBindings.Add(new Binding("Text", interaction, "CurrentValue", false, DataSourceUpdateMode.OnPropertyChanged));
					break;
				case EditorTypes.CheckBox:
					control = new CheckBox();
					control.DataBindings.Add(new Binding("Checked", interaction, "CurrentValue", false, DataSourceUpdateMode.OnPropertyChanged));
					break;
				case EditorTypes.Label:
					DottedLabel label = new DottedLabel();
					label.Appearance.TextOptions.HAlignment = interaction.Halignment;
					label.Appearance.TextOptions.VAlignment = interaction.Valignment;
					label.Font = interaction.Font;
					label.Text = interaction.Text;
					control = label;
					break;
				case EditorTypes.RadioButton:
					control = CreateCheckButton(interaction, control, myEvent, tt, buttonChangeCheckedState);
					break;
				case EditorTypes.Button:
					control = CreateButton(interaction, control, myEvent, tt);
					break;
				case EditorTypes.ComboBox:
					System.Windows.Forms.ComboBox tempcontrol = new System.Windows.Forms.ComboBox();
					tempcontrol.DisplayMember = "Name";
					tempcontrol.ValueMember = "Value";
					control = tempcontrol;
					control.DataBindings.Add(new Binding("SelectedValue", interaction, "CurrentValue", false, DataSourceUpdateMode.OnPropertyChanged));
					break;
			}
			return control;
		}
		static Control CreateButton(BaseInteraction interacation, Control control, EventHandler myEvent, ToolTip tt) {
			control = new SimpleButton();
			control.Text = interacation.Text;
			(control as SimpleButton).ImageLocation = ImageLocation.MiddleCenter;
			if(interacation.Image != null) {
				(control as SimpleButton).Image = interacation.Image;
			}
			tt.SetToolTip(control, interacation.Text);
			control.Enabled = interacation.Enabled;
			control.Click += myEvent;
			return control;
		}
		static Control CreateCheckButton(BaseInteraction interacation, Control control, EventHandler myEvent, ToolTip tt, EventHandler buttonChangeCheckedState) {
			control = new CheckButton();
			(control as CheckButton).Text = interacation.Text;
			(control as CheckButton).ImageLocation = ImageLocation.MiddleCenter;
			(control as CheckButton).Checked = interacation.Checked;
			if(interacation.Image != null) (control as CheckButton).Image = interacation.Image;
			tt.SetToolTip(control, interacation.Text);
			control.Enabled = interacation.Enabled;
			control.Click += myEvent;
			(control as CheckButton).CheckedChanged += buttonChangeCheckedState;
			return control;
		}
	  internal void ToQuickCustomization(ILayoutControl layout) {
			CustomizationForm customizationForm = ((CustomizationForm)Layout.CustomizationForm);
			customizationForm.tabbedControlGroup1.Visibility = Utils.LayoutVisibility.Never;
			customizationForm.layoutTreeViewGroup.Visibility = Utils.LayoutVisibility.Never;
			customizationForm.splitterItem1.Visibility = Utils.LayoutVisibility.Never;
			customizationForm.propertyGridItem.Visibility = Utils.LayoutVisibility.Never;
			customizationForm.hiddenItemsListItem.Visibility = Utils.LayoutVisibility.Never;
			customizationForm.buttonsPanelItem.Visibility = Utils.LayoutVisibility.Never;
			layout.CustomizationForm.WindowState = FormWindowState.Normal;
		}
	  internal void ToFullCustomization(ILayoutControl layout) {
			CustomizationForm customizationForm = ((CustomizationForm)Layout.CustomizationForm);
			customizationForm.templateListGroup.Visibility = Utils.LayoutVisibility.Never;
			customizationForm.tabbedControlGroup1.Visibility = Utils.LayoutVisibility.Always;
			if(layout.OptionsCustomizationForm.ShowLayoutTreeView) customizationForm.layoutTreeViewGroup.Visibility = Utils.LayoutVisibility.Always;
			else { customizationForm.layoutTreeViewGroup.Visibility = Utils.LayoutVisibility.Never; }
			if(layout.OptionsCustomizationForm.ShowPropertyGrid) {
				customizationForm.propertyGridItem.Visibility = Utils.LayoutVisibility.Always;
				customizationForm.splitterItem1.Visibility = Utils.LayoutVisibility.Always;
			}
		   customizationForm.hiddenItemsListItem.Visibility = Utils.LayoutVisibility.Always;
		}
		public void Dispose() {
			if(Layout.CustomizationForm != null) {
				((CustomizationForm)Layout.CustomizationForm).FormClosing -= HideCustomization;
				Layout.CustomizationForm.LocationChanged -= CustomizationForm_LocationChanged;
				Layout.CustomizationForm.ClientSizeChanged -= CustomizationForm_ClientSizeChanged;
			}
		}
	}
	internal class DXFlowLayoutPanel : FlowLayoutPanel {
		protected override void OnCreateControl() {
			base.OnCreateControl();
			this.OnBindingContextChanged(EventArgs.Empty);
		}
	}
}
internal class DottedLabel : LabelControl {
	protected override void OnPaint(PaintEventArgs e) {
		base.OnPaint(e);
		Rectangle rect = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
		rect.Inflate(-1, -1);
		Pen pen = new Pen(Brushes.Black);
		pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
		pen.DashPattern = new float[] { 6f, 6f };
		e.Graphics.DrawRectangle(pen, rect);
	}
}
