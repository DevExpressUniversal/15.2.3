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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.ExpressApp.Win.Layout {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class XafLayoutControlHandler : LayoutControlHandler {
		public XafLayoutControlHandler(ILayoutControl owner) : base(owner) { }
		protected override bool IsLimitedFunctionality {
			get { return ForceUnlimitedFunctionality ? false : base.IsLimitedFunctionality; }
		}
		public bool ForceUnlimitedFunctionality { get; set; }
	}
	[ToolboxItem(false)]
	public class XafLayoutControl : LayoutControl, ISupportUpdate, IXtraResizableControl {
		private void OnCustomizationClosed(Object sender, EventArgs e) {
			if(CustomizationClosed != null) {
				CustomizationClosed(sender, e);
			}
		}
		private void customizationForm_VisibleChanged(Object sender, EventArgs e) {
			UserCustomizationForm customizationForm = (UserCustomizationForm)sender;
			if(!customizationForm.Visible) {
				OnCustomizationClosed(sender, e);
			}
		}
		private void customizationForm_FormClosed(Object sender, FormClosedEventArgs e) {
			OnCustomizationClosed(sender, EventArgs.Empty);
		}
		private XafLayoutControlImplementor LayoutControlImplementor {
			get { return (XafLayoutControlImplementor)((ISupportImplementor)this).Implementor; }
		}
		protected override LayoutControlImplementor CreateILayoutControlImplementorCore() {
			return new XafLayoutControlImplementor(this);
		}
		protected override DevExpress.XtraLayout.Handlers.LayoutControlHandler CreateLayoutControlRuntimeHandler() {
			return new XafLayoutControlHandler(this);
		}
		public XafLayoutControl() {
			this.BeginInit();
			OptionsCustomizationForm.ShowLoadButton = false;
			OptionsCustomizationForm.ShowSaveButton = false;
#if DebugTest
			OptionsCustomizationForm.ShowPropertyGrid = true;
#endif
			Dock = DockStyle.Fill;
			Root.GroupBordersVisible = false;
			OptionsItemText.AlignControlsWithHiddenText = true;
			OptionsItemText.TextAlignMode = TextAlignMode.AlignInLayoutControl;
			Root.Name = "Root";
			Root.DefaultLayoutType = LayoutType.Vertical;
			OptionsView.AllowItemSkinning = false;
			OptionsView.EnableIndentsInGroupsWithoutBorders = true;
			OptionsFocus.AllowFocusReadonlyEditors = false;
			AutoScroll = true;
			this.EndInit();
		}
		public override UserCustomizationForm CreateCustomizationForm() {
			UserCustomizationForm customizationForm = base.CreateCustomizationForm();
			customizationForm.FormClosed += new FormClosedEventHandler(customizationForm_FormClosed);
			customizationForm.VisibleChanged += new EventHandler(customizationForm_VisibleChanged);
			return customizationForm;
		}
		public XafLayoutConstants XafLayoutConstants {
			get { return LayoutControlImplementor.XafLayoutConstants; }
			set { LayoutControlImplementor.XafLayoutConstants = value; }
		}
		public event EventHandler CustomizationClosed;
		#region IXtraResizableControl Members
		public Size MaxSize {
			get { return MaximumSize; }
		}
		public Size MinSize {
			get { return Root == null ? MinimumSize : Root.MinSize; }
		}
		#endregion
		void ISupportUpdate.BeginUpdate() {
			BeginUpdate();
		}
		void ISupportUpdate.EndUpdate() {
			EndUpdate();
		}
	}
}
