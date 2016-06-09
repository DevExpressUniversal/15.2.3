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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers {
	public class ButtonsContainersSimpleActionItem : ButtonsContainersActionItemBase
#if DebugTest
, DevExpress.ExpressApp.Tests.ISimpleActionItemUnitTestable
#endif
 {
		private void button_Click(object sender, EventArgs e) {
			ProcessClick();
		}
		private void UpdateImageLocation() {
			Control.ImageLocation = string.IsNullOrEmpty(Control.Text) ? ImageLocation.MiddleCenter : ImageLocation.MiddleLeft;
		}
		protected override Control CreateControl() {
			SimpleButton button = new SimpleButton();
			SetupControl(button);
			button.Click += new EventHandler(button_Click);
			return button;
		}
		protected virtual void SetupControl(Control control) {
			SimpleButton button = (SimpleButton)control;
			button.MinimumSize = new Size(75, 20);
			if(Action.ActionMeaning != ActionMeaning.Unknown && Action.Controller == null) {
				button.DialogResult = Action.ActionMeaning == ActionMeaning.Accept ? DialogResult.OK : DialogResult.Cancel;
			}
			button.ImageLocation = ImageLocation.TopCenter;
			button.Tag = EasyTestTagHelper.FormatTestAction(button.Text);
		}
		protected virtual void ProcessClick() {
			if(IsConfirmed()) {
				((SimpleAction)Action).DoExecute();
			}
		}
		protected override void SetCaption(string caption) {
			Control.Text = caption;
			UpdateImageLocation();
			base.SetCaption(String.Empty);
		}
		protected override void SetToolTip(string toolTip) {
			Control.ToolTip = toolTip;
		}
		protected override void SetImage(DevExpress.ExpressApp.Utils.ImageInfo imageInfo) {
			Control.Image = !imageInfo.IsEmpty ? imageInfo.Image : null;
			base.SetImage(imageInfo);
		}
		protected override void SetPaintStyle(ActionItemPaintStyle paintStyle) {
			base.SetPaintStyle(paintStyle);
			UpdateImageLocation();
		}
		public new SimpleButton Control {
			get { return (SimpleButton)base.Control; }
		}
		public ButtonsContainersSimpleActionItem(ActionBase action, ButtonsContainer owner) : base(action, owner) { }
		public override void ProcessShortcut() {
			if(Action.Active && Action.Enabled) {
				ProcessClick();
			}
		}
#if DebugTest
		public override string ControlToolTip {
			get { return Control.ToolTip; }
		}
		public override bool ImageVisible {
			get { return Control.Image != null; }
		}
		public override string ControlCaption {
			get { return Control.Text; }
		}
#endif
	}
}
