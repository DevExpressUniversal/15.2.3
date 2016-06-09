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
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers.Items {
	public class ParametrizedActionItem : BarActionBaseItem
#if DebugTest
, DevExpress.ExpressApp.Tests.IParametrizedActionItemUnitTestable
#endif
 {
		private const int ControlWidth = 150;
		private bool isValueChangedEventLocked;
		private bool isValueChanged;
		private bool allowExecuteOnLoseFocus;
		private bool showExecuteButton = true;
		private bool canClear = true;
		private RepositoryItemButtonEdit repositoryItemButtonEdit;
		private ParametrizedActionItemControlFactory factory;
		private BarEditItem editItem;
		private bool controlCreated;
		private bool forceExecute;
		private void Action_ValueChanged(object sender, EventArgs e) {
			if(Control != null) {
				isValueChangedEventLocked = true;
				try {
					Control.EditValue = Action.Value;
				}
				finally {
					isValueChangedEventLocked = false;
				}
			}
		}
		private void edit_ShownEditor(object sender, ItemClickEventArgs e) {
			RefreshClearActionVisible();
			isValueChanged = false;
		}
		private void edit_EditValueChanged(object sender, EventArgs e) {
			RefreshClearActionVisible();
			isValueChanged = true;
		}
		private void RefreshClearActionVisible() {
			RepositoryItemButtonEditWithClearButton myButtonEdit = editItem.Edit as RepositoryItemButtonEditWithClearButton;
			if(myButtonEdit != null) {
				myButtonEdit.SetClearButtonVisibility(!string.IsNullOrEmpty(editItem.EditValue as string));
			}
		}
		private void edit_HiddenEditor(object sender, ItemClickEventArgs e) {
			if(forceExecute || (isValueChanged && !isValueChangedEventLocked && (allowExecuteOnLoseFocus || !showExecuteButton))) {
				DoExecute(ExtractTypedValue(Control.EditValue));
			}
			RefreshClearActionVisible();
		}
		private object ExtractTypedValue(object value) {
			if(value is decimal) {
				value = decimal.ToInt32((decimal)value);
			}
			return value;
		}
		private void ForceExecute() {
			forceExecute = true;
			try {
				GetActiveEditItemLink().CloseEditor();
			}
			finally {
				forceExecute = false;
			}
		}
		private BarEditItemLink GetActiveEditItemLink() {
			BarEditItemLink result = Manager.ActiveEditItemLink;
			if(result == null) {
				BarManager mergedOwner = Manager.MergedOwner;
				if(mergedOwner != null) {
					result = mergedOwner.ActiveEditItemLink;
				}
				else if(Manager is RibbonBarManager) {
					RibbonBarManager ribbonBarManager = (RibbonBarManager)Manager;
					RibbonControl mergeOwner = ribbonBarManager.Ribbon.MergeOwner;
					if(mergeOwner != null) {
						result = mergeOwner.Manager.ActiveEditItemLink;
					}
				}
			}
			if(result == null) {
				throw new InvalidOperationException("Cannot obtain ActiveEditItemLink.");
			}
			return result;
		}
		private void repositoryItemButtonEdit_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter) {
				ForceExecute();
				e.Handled = true;
			}
		}
		private void repositoryItem_ButtonClick(object sender, ButtonPressedEventArgs e) {
			if(ParametrizedActionItemControlFactory.GoButtonID.Equals(e.Button.Tag)) {
				ForceExecute();
			}
		}
		private void editItem_ShowingEditor(object sender, ItemCancelEventArgs e) {
			BindingHelper.EndCurrentEdit(Form.ActiveForm);
		}
		private void UpdateItemAppearance() {
			ActionItemPaintStyle paintStyle = Action.PaintStyle;
			switch(paintStyle) {
				case ActionItemPaintStyle.Caption:
					Control.PaintStyle = BarItemPaintStyle.Caption;
					Control.Glyph = null;
					Control.LargeGlyph = null;
					break;
				case ActionItemPaintStyle.CaptionAndImage:
					Control.PaintStyle = BarItemPaintStyle.CaptionGlyph;
					break;
				case ActionItemPaintStyle.Default:
					Control.PaintStyle = BarItemPaintStyle.Caption;
					Control.Caption = "";
					Control.Glyph = null;
					Control.LargeGlyph = null;
					break;
				case ActionItemPaintStyle.Image:
					Control.PaintStyle = BarItemPaintStyle.Standard;
					Control.Caption = "";
					break;
			}
			factory.UpdateGoButtonAppearance((RepositoryItemButtonEdit)this.editItem.Edit);
		}
		protected void DoExecute(object value) {
			if(IsConfirmed()) {
				Action.DoExecute(value);
			}
		}
		protected override BarItem CreateControlCore() {
			Action.ValueChanged += new EventHandler(Action_ValueChanged);
			editItem = new BarEditItem();
			repositoryItemButtonEdit = factory.CreateRepositoryItem(ShowExecuteButton, CanClear);
			editItem.Width = ControlWidth;
			if(ShowExecuteButton) {
				repositoryItemButtonEdit.ButtonClick += new ButtonPressedEventHandler(repositoryItem_ButtonClick);
			}
			repositoryItemButtonEdit.KeyDown += new KeyEventHandler(repositoryItemButtonEdit_KeyDown);
			editItem.Edit = repositoryItemButtonEdit;
			editItem.EditValue = Action.Value;
			RefreshClearActionVisible();
			editItem.EditValueChanged += new EventHandler(edit_EditValueChanged);
			editItem.ShowingEditor += new ItemCancelEventHandler(editItem_ShowingEditor);
			editItem.ShownEditor += new ItemClickEventHandler(edit_ShownEditor);
			editItem.HiddenEditor += new ItemClickEventHandler(edit_HiddenEditor);
			controlCreated = true;
			return editItem;
		}
		protected override void SetCaption(string caption) {
			base.SetCaption(caption);
			UpdateItemAppearance();
		}
		protected override void SetImage(ImageInfo imageInfo) {
			UpdateItemAppearance();
		}
		protected override void SetPaintStyle(ActionItemPaintStyle paintStyle) {
			base.SetPaintStyle(paintStyle);
			UpdateItemAppearance();
		}
		public override void Dispose() {
			try {
				if(Action != null) {
					Action.ValueChanged -= new EventHandler(Action_ValueChanged);
				}
				if(factory != null) {
					factory.Dispose();
					factory = null;
				}
				BarEditItem editItem = (BarEditItem)control;
				if(editItem != null) {
					editItem.ShowingEditor -= new ItemCancelEventHandler(editItem_ShowingEditor);
					editItem.EditValueChanged -= new EventHandler(edit_EditValueChanged);
					editItem.ShownEditor -= new ItemClickEventHandler(edit_ShownEditor);
					editItem.HiddenEditor -= new ItemClickEventHandler(edit_HiddenEditor);
					if(editItem.Edit != null) {
						repositoryItemButtonEdit.ButtonClick -= new ButtonPressedEventHandler(repositoryItem_ButtonClick);
						repositoryItemButtonEdit.KeyDown -= new KeyEventHandler(repositoryItemButtonEdit_KeyDown);
						editItem.Edit.Dispose();
						editItem.Edit = null;
					}
					editItem = null;
				}
			}
			finally {
				base.Dispose();
			}
		}
		public new ParametrizedAction Action {
			get { return (ParametrizedAction)base.Action; }
		}
		public new BarEditItem Control {
			get { return (BarEditItem)base.Control; }
		}
		public bool CanClear {
			get { return canClear; }
			set {
				if(controlCreated) {
					throw new InvalidOperationException("Unable to set the CanClear property value. The Control has already been created.");
				}
				canClear = value;
			}
		}
		public bool ShowExecuteButton {
			get { return showExecuteButton; }
			set {
				if(controlCreated) {
					throw new InvalidOperationException("Unable to set the ShowExecuteButton property value. The Control has already been created.");
				}
				showExecuteButton = value;
			}
		}
		public bool AllowExecuteOnLoseFocus {
			get { return allowExecuteOnLoseFocus; }
			set { allowExecuteOnLoseFocus = value; }
		}
		public ParametrizedActionItem(ParametrizedAction action, BarManager manager)
			: base(action, manager) {
			factory = new ParametrizedActionItemControlFactory(action);
		}
#if DebugTest
		#region IActionBaseItemUnitTestable Members
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.CaptionVisible {
			get { return !string.IsNullOrEmpty(Control.Caption); }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ImageVisible {
			get {
				EditorButton goButton = ParametrizedActionItemControlFactory.GetGoButton((RepositoryItemButtonEdit)editItem.Edit);
				return goButton != null && goButton.Image != null;
			}
		}
		#endregion
		#region IParametrizedActionItemUnitTestable Members
		string DevExpress.ExpressApp.Tests.IParametrizedActionItemUnitTestable.GoButtonCaption {
			get {
				RepositoryItemButtonEdit buttonEdit = ((BarEditItem)Control).Edit as RepositoryItemButtonEdit;
				EditorButton button = ParametrizedActionItemControlFactory.GetGoButton(buttonEdit);
				if(button != null) {
					return button.Caption;
				}
				return "";
			}
		}
		object DevExpress.ExpressApp.Tests.IParametrizedActionItemUnitTestable.Value {
			get { return ((BarEditItem)Control).EditValue; }
		}
		#endregion
#endif
		#region Obsolete 14.2
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected object lastExecutedValue;
		[Obsolete("Use the DoExecute(object) method instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected void DoExecute(object value, out bool executed) {
			executed = true;
			DoExecute(value);
		}
		#endregion
	}
}
