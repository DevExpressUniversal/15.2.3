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

using DevExpress.Skins.XtraForm;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraBars {
	public class TabForm : XtraForm {
		TabFormControl tabFormControl;
		public TabForm()
			: base() {
			this.tabFormControl = null;
		}
		protected override FormPainter CreateFormBorderPainter() {
			return new TabFormPainter(this, LookAndFeel);
		}
		internal TabFormPainter GetPainter() {
			return (TabFormPainter)FormPainter;
		}
		internal Rectangle GetCaptionClientBounds() {
			if(GetPainter() == null) return Rectangle.Empty;
			return GetPainter().GetCaptionClientBounds();
		}
		protected override bool GetAllowSkin() {
			return true;
		}
		[Browsable(false), DefaultValue(null)]
		public TabFormControl TabFormControl {
			get { return tabFormControl; }
			set {
				if(tabFormControl == value)
					return;
				tabFormControl = value;
				if(tabFormControl != null) {
					tabFormControl.TabForm = this;
					tabFormControl.Init();
				}
			}
		}
		internal bool IsMaximized { get { return WindowState == FormWindowState.Maximized; } }
		protected internal void CreateTabFormControl(bool shouldCreatePage) {
			TabFormControl = new TabFormControl();
			TabFormControl.TabForm = this;
			if(Site != null)
				Site.Container.Add(TabFormControl);
			Controls.Add(TabFormControl);
			if(shouldCreatePage)
				TabFormControl.AddNewPage();
			TabFormControl.BeginInit();
			TabFormControl.EndInit();
			FormPainter.InvalidateNC(this);
		}
		protected override void OnShown(System.EventArgs e) {
			base.OnShown(e);
			if(Site != null && TabFormControl == null) {
				CreateTabFormControl(true);
			}
			if(!TabFormControl.IsDesignMode && TabFormControl.IsRightToLeft) {
				TabFormControl.LayoutChanged();
			}
		}
	}
}
