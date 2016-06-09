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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Templates.ActionContainers.Items;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers {
	public class ButtonsContainersParametrizedActionItem : ButtonsContainersActionItemBase
#if DebugTest
, DevExpress.ExpressApp.Tests.IParametrizedActionItemUnitTestable
#endif
 {
		private EditorButton goButton;
		private bool controlCreated = false;
		private bool showExecuteButton = true;
		private bool canClear = true;
		private ParametrizedActionItemControlFactory factory;
		private object GetActionValue(object value) {
			object result = value;
			if(value == null && Action.ValueType == typeof(int)) {
				result = 0;
			}
			return result;
		}
		private void action_ValueChanged(object sender, EventArgs e) {
			Control.EditValue = GetActionValue(Action.Value);
		}
		private void UpdateImageLocation() {
			if(goButton != null) {
				goButton.ImageLocation = string.IsNullOrEmpty(goButton.Caption) ? ImageLocation.MiddleCenter : ImageLocation.MiddleLeft;
			}
		}
		private void control_ButtonClick(object sender, ButtonPressedEventArgs e) {
			if(e.Button == goButton) {
				ExecuteWithCurrentValue();
			}
		}
		private void control_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter) {
				ExecuteWithCurrentValue();
			}
		}
		private void result_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
			if(e.KeyData == Keys.Return && e.Modifiers == Keys.None) {
				e.IsInputKey = true;
			}
		}
		private void ExecuteWithCurrentValue() {
			object editValue = Control.EditValue;
			if(Action.ValueType == typeof(int)) {
				editValue = Control.EditValue == null ? 0 : decimal.ToInt32((decimal)Control.EditValue);
			}
			Action.DoExecute(editValue);
		}
		protected override Control CreateControl() {
			ButtonEdit control = factory.CreateControl(ShowExecuteButton, CanClear);
			control.PreviewKeyDown += new PreviewKeyDownEventHandler(result_PreviewKeyDown);
			goButton = ParametrizedActionItemControlFactory.GetButtonById(ParametrizedActionItemControlFactory.GoButtonID, control.Properties);
			control.ButtonClick += new ButtonPressedEventHandler(control_ButtonClick);
			control.KeyDown += new KeyEventHandler(control_KeyDown);
			control.Tag = EasyTestTagHelper.FormatTestAction(Action.Caption);
			control.Name = "Control_" + Guid.NewGuid();
			control.EditValue = GetActionValue(Action.Value);
			controlCreated = true;
			return control;
		}
		protected override DevExpress.XtraLayout.Utils.Padding GetPadding() {
			return new DevExpress.XtraLayout.Utils.Padding(6, 0, 1, 1);
		}
		protected override void SetCaption(string caption) {
			if(GetPaintStyle() == ActionItemPaintStyle.Image) {
				caption = String.Empty;
			}
			if(!String.IsNullOrEmpty(caption)) {
				LayoutItem.Text = caption;
			}
			if(goButton != null) {
				factory.UpdateGoButtonAppearance(this.Control.Properties);
			}
			UpdateImageLocation();
			base.SetCaption(caption);
		}
		protected override void SetToolTip(string toolTip) {
			Control.ToolTip = toolTip;
		}
		protected override void SetImage(ImageInfo imageInfo) {
			if(goButton != null) {
				goButton.Image = !imageInfo.IsEmpty ? imageInfo.Image : null;
			}
			base.SetImage(imageInfo);
		}
		protected override void SetConfirmationMessage(string message) {
		}
		protected override ActionItemPaintStyle GetDefaultPaintStyle() {
			return ActionItemPaintStyle.Image;
		}
		protected override void SetPaintStyle(ActionItemPaintStyle paintStyle) {
			base.SetPaintStyle(paintStyle);
			factory.UpdateGoButtonAppearance(this.Control.Properties);
			UpdateImageLocation();
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
		public new ButtonEdit Control {
			get { return (ButtonEdit)base.Control; }
		}
		public new ParametrizedAction Action {
			get { return (ParametrizedAction)base.Action; }
		}
		public override void ProcessShortcut() {
			Control.Focus();
		}
		public override void Dispose() {
			if(factory != null) {
				factory.Dispose();
				factory = null;
			}
			if(Control != null) {
				Control.ButtonClick -= new ButtonPressedEventHandler(control_ButtonClick);
				Control.PreviewKeyDown -= new PreviewKeyDownEventHandler(result_PreviewKeyDown);
				Control.KeyDown -= new KeyEventHandler(control_KeyDown);
				Control.Dispose();
			}
			Action.ValueChanged -= new EventHandler(action_ValueChanged);
			base.Dispose();
		}
		public ButtonsContainersParametrizedActionItem(ParametrizedAction action, ButtonsContainer owner)
			: base(action, owner) {
			factory = new ParametrizedActionItemControlFactory(action);
			action.ValueChanged += new EventHandler(action_ValueChanged);
		}
#if DebugTest
		#region IParametrizedActionItemUnitTestable Members
		public string GoButtonCaption {
			get { return goButton.Caption; }
		}
		public object Value {
			get { return Control.EditValue; }
		}
		#endregion
		public override string ControlToolTip {
			get { return Control.ToolTip; }
		}
		public override bool ImageVisible {
			get { return goButton.Image != null; }
		}
		public override string ControlCaption {
			get { return LayoutItem.Text; }
		}
		public override bool CaptionVisible {
			get { return LayoutItem.TextVisible; }
		}
		public void DebugTest_ExecuteWithCurrentValue() {
			ExecuteWithCurrentValue();
		}
#endif
	}
}
