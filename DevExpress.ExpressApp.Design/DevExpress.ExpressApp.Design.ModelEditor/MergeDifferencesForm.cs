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
using System.ComponentModel;
using System.Drawing;
using System.Security;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Design.Core;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
namespace DevExpress.ExpressApp.Design.ModelEditor {
	[ToolboxItem(false)]
	public class MergeDifferencesForm : PopupForm, DevExpress.ExpressApp.Win.Core.ModelEditor.IModelEditorSettings {
		public const string Title = "Model Merge Tool";
		private IModelEditorController controller;
		private IProjectWrapper projectWrapper;
		private ModelEditorControl modelEditorControl;
		private SimpleAction saveAction;
		private SimpleAction cancelAction;
		private ButtonsContainer leftButtonsContainer;
		private LayoutControlItem leftButtonsContainerLayoutItem;
		public SimpleAction SaveAction {
			get { return saveAction; }
		}
		public SimpleAction CancelAction {
			get { return cancelAction; }
		}
		public SingleChoiceAction ChooseMergeModuleAction {
			get { return controller.ChooseMergeModuleAction; }
		}
		public SimpleAction MergeDifferencesAction {
			get { return controller.MergeDifferencesAction; }
		}
		public ModelEditorControl ModelEditorControl {
			get { return modelEditorControl; }
		}
		[SecuritySafeCritical]
		public MergeDifferencesForm(IModelEditorController controller, IProjectWrapper projectWrapper) {
			this.controller = controller;
			this.projectWrapper = projectWrapper;
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.InitialMinimumSize = new Size(550, 600);
			this.leftButtonsContainer = new ButtonsContainer();
			this.leftButtonsContainer.AllowCustomization = false;
			this.leftButtonsContainer.ContainerId = "PopupActions";
			this.leftButtonsContainer.Name = "leftButtonsContainer";
			this.leftButtonsContainer.Orientation = ActionContainerOrientation.Horizontal;
			this.leftButtonsContainer.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
			this.leftButtonsContainer.OptionsView.UseSkinIndents = false;
			this.leftButtonsContainer.OptionsView.EnableIndentsInGroupsWithoutBorders = true;
			this.leftButtonsContainer.OptionsView.AllowItemSkinning = false;
			this.leftButtonsContainer.BackColor = this.ButtonsContainer.BackColor;
			this.leftButtonsContainer.Margin = this.ButtonsContainer.Margin;
			this.leftButtonsContainer.Root.Padding = this.ButtonsContainer.Root.Padding;
			this.leftButtonsContainerLayoutItem = new LayoutControlItem();
			this.leftButtonsContainerLayoutItem.Control = this.leftButtonsContainer;
			this.leftButtonsContainerLayoutItem.Name = "leftButtonsContainerLayoutItem";
			this.leftButtonsContainerLayoutItem.Padding = this.BottomPanel.GetItemByControl(this.ButtonsContainer).Padding;
			this.leftButtonsContainerLayoutItem.TextSize = new Size(0, 0);
			this.leftButtonsContainerLayoutItem.TextToControlDistance = 0;
			this.leftButtonsContainerLayoutItem.TextVisible = false;
			BaseLayoutItem emptySpaceItem = this.BottomPanel.Root[1];
			this.BottomPanel.AddItem(this.leftButtonsContainerLayoutItem, emptySpaceItem, DevExpress.XtraLayout.Utils.InsertType.Left);
			modelEditorControl = new ModelEditorControl(new NullSettingsStorage());
			modelEditorControl.Dock = DockStyle.Fill;
			modelEditorControl.BorderStyle = BorderStyles.NoBorder;
			((Control)ViewSiteControl).Controls.Add(modelEditorControl);
			((ModelEditorViewController)controller).SetControl(modelEditorControl);
			((ModelEditorViewController)controller).SetTemplate(this);
			Image modelEditorImage = ImageLoader.Instance.GetImageInfo("ModelEditor_ModelMerge").Image;
			if(modelEditorImage != null) {
				this.Icon = Icon.FromHandle(new Bitmap(modelEditorImage).GetHicon());
			}
			Text = Title;
			CreateActions();
			UpdateActionStates();
		}
		private void CreateActions() {
			this.leftButtonsContainer.ActionItemAdded += new EventHandler<ActionItemEventArgs>(leftButtonsContainer_ActionItemAdded);
			controller.MergeDifferencesAction.Caption = "Merge";
			controller.MergeDifferencesAction.PaintStyle = ActionItemPaintStyle.Caption;
			controller.MergeDifferencesAction.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(MergeDifferencesAction_ExecuteCompleted);
			this.leftButtonsContainer.Register(controller.MergeDifferencesAction);
			this.leftButtonsContainer.Register(controller.ChooseMergeModuleAction);
			saveAction = new SimpleAction();
			saveAction.Caption = "Save";
			saveAction.Execute += new SimpleActionExecuteEventHandler(saveAction_Execute);
			this.ButtonsContainer.Register(saveAction);
			ButtonsContainer.AllowCustomization = true;
			cancelAction = new SimpleAction();
			cancelAction.Caption = "Cancel";
			cancelAction.Execute += new SimpleActionExecuteEventHandler(cancelAction_Execute);
			this.ButtonsContainer.Register(cancelAction);
		}
		private void MergeDifferencesAction_ExecuteCompleted(object sender, ActionBaseEventArgs e) {
			UpdateActionStates();
		}
		private void leftButtonsContainer_ActionItemAdded(object sender, ActionItemEventArgs e) {
			if(e.Item.Action == ChooseMergeModuleAction) {
				e.Item.LayoutItem.Control.MinimumSize = new Size(250, e.Item.LayoutItem.ControlMinSize.Height);
			}
			e.Item.LayoutItem.ControlAlignment = ContentAlignment.MiddleLeft;
		}
		private void cancelAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
		private ChoiceActionItem currentDifferenceActionItem;
		private void chooseDifferencesAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			currentDifferenceActionItem = e.SelectedChoiceActionItem;
		}
		private void UpdateActionStates() {
			saveAction.Enabled["IsModified"] = controller.IsModified;
		}
		[SecuritySafeCritical]
		private void saveAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			ModelSaver saver = new ModelSaver(null, projectWrapper, controller);
			try {
				if(saver.Save()) {
					UpdateActionStates();
					Close();
				}
			}
			catch(Exception ex) {
				HandleException(ex);
			}
		}
		public override void SetCaption(string text) {
			Text = string.Format("{0} - {1}", Title, text);
		}
		protected override void OnClosed(EventArgs e) {
			modelEditorControl.OnClosed();
			modelEditorControl.Dispose();
			base.OnClosed(e);
		}
		public static void HandleException(Exception e) {
			Tracing.Tracer.LogError(e);
			Messaging.GetMessaging(null).Show(MergeDifferencesForm.Title, e);
		}
		#region IModelEditorSettingsStorage Members
		public void ModelEditorSaveSettings() {
		}
		#endregion
	}
}
