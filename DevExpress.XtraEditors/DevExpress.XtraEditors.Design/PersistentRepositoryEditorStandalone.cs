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
using System.Drawing;
using System.Drawing.Design;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils.Design;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.XtraEditors.Design {
	public class PersistentRepositoryEditorStandalone : XtraDesignForm {
		public const string settings = "Software\\Developer Express\\Designer\\XtraEditors\\";
		private PersistentRepositoryEditor editor = null;
		PropertyStore store = null;
		private object ev;
		public PersistentRepositoryEditorStandalone(object editValue) {
			ev = editValue;
			InitializeComponent();
			this.AutoScaleMode = AutoScaleMode.None;
			IServiceProvider host = null;
			ILookAndFeelService serv = null;
			if(editValue is Component)
				host = (editValue as Component).Site;
			if(host != null)
				serv = host.GetService(typeof(ILookAndFeelService)) as ILookAndFeelService;
			if(serv != null && !RegistryDesignerSkinHelper.CanUseDefaultControlDesignersSkin)
				serv.InitializeRootLookAndFeel(this.LookAndFeel);
			else
				LookAndFeel.SetSkinStyle("DevExpress Design");
			editor.InitFrame(ev, "", null);
			ProductInfo = new DevExpress.Utils.About.ProductInfo("XtraGrid", typeof(DevExpress.XtraEditors.Repository.PersistentRepository),
				DevExpress.Utils.About.ProductKind.DXperienceWin, DevExpress.Utils.About.ProductInfoStage.Registered);
			this.store = new PropertyStore(settings);
			Store.Restore();
			RestoreProperties();
		}
		protected PropertyStore Store { get { return store; } }
		protected override void OnClosed(EventArgs e) {
			if(Store != null) {
				StoreProperties();
				Store.Store();
			}
			base.OnClosed(e);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.editor = new DevExpress.XtraEditors.Design.CustomPersistentRepositoryEditor();
			this.SuspendLayout();
			this.editor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.editor.HavePG = true;
			this.editor.ID = 6;
			this.editor.Location = new System.Drawing.Point(8, 10);
			this.editor.Name = "editor";
			this.editor.Size = new System.Drawing.Size(608, 462);
			this.editor.TabIndex = 0;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(670, 500);
			this.Controls.Add(this.editor);
			this.LookAndFeel.SkinName = "DevExpress Design";
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.Name = "PersistentRepositoryEditorStandalone";
			this.Padding = new System.Windows.Forms.Padding(8, 10, 8, 14);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Persistent Repository Editor";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected virtual void RestoreProperties() {
			if(Store == null) return;
			Store.RestoreForm(this);
			editor.pnlMain.Width = Store.RestoreIntProperty("MainPanelWidth", 190);
			if(editor.pnlMain.Width >= this.Width)
				editor.pnlMain.Width = this.Width / 2;
		}
		protected virtual void StoreProperties() {
			if(Store == null) return;
			Store.AddProperty("MainPanelWidth", editor.pnlMain.Width);
			Store.AddForm(this);
		}
	}
	[ToolboxItem(false)]
	public class CustomPersistentRepositoryEditor : PersistentRepositoryEditor {
		public CustomPersistentRepositoryEditor() {
			btAdd.Location = new Point(btAdd.Location.X - 4, btAdd.Location.Y + 4);
			btAdd.Size = new System.Drawing.Size(184, 30);
			btRemove.Location = new Point(btRemove.Location.X + 8, btRemove.Location.Y + 4);
			btRemove.Size = new System.Drawing.Size(100, 30);
			btnSearch.Location = new Point(btRemove.Location.X + btRemove.Width + (btRemove.Location.X - btAdd.Location.X - btAdd.Width), btRemove.Location.Y);
			btnSearch.Size = new System.Drawing.Size(30, 30);
			pnlControl.Size = new Size(pnlControl.Size.Width, pnlControl.Size.Height - 8);
			pnlMain.Width = 290;
		}
	}
	public sealed class RepositoryItemsEditor : UITypeEditor {
		internal IWindowsFormsEditorService edSvc = null;
		public override object EditValue(ITypeDescriptorContext context,
			IServiceProvider provider, object value) {
			if(context == null || context.Instance == null || provider == null)
				return value;
			edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if(edSvc == null) return value;
			DevExpress.XtraEditors.Design.PersistentRepositoryEditorStandalone form = new DevExpress.XtraEditors.Design.PersistentRepositoryEditorStandalone(context.Instance);
			edSvc.ShowDialog(form);
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null)
				return UITypeEditorEditStyle.Modal;
			return base.GetEditStyle(context);
		}
	}
}
