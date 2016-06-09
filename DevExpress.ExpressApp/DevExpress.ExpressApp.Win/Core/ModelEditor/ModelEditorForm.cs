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
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core.ModelEditor.NodesTree;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public interface IModelEditorSettings {
		void ModelEditorSaveSettings();
	}
	[ToolboxItem(false)]
	public class ModelEditorForm : DevExpress.ExpressApp.Win.Templates.DetailViewForm, IModelEditorSettings {
		public const string Title = "eXpressApp Framework Model Editor";
		private IModelEditorController controller;
		private SettingsStorage settingsStorage;
		private ModelEditorControl modelEditorControl;
		private bool dropModelDifs = false;
		public ModelEditorForm(ModelEditorViewController controller, SettingsStorage settingsStorage)
			: base() {
			this.settingsStorage = settingsStorage;
			MainMenuBar.Visible = false;
			modelEditorControl = new ModelEditorControl(settingsStorage);
			controller.SetControl(modelEditorControl);
			controller.SetTemplate(this);
			this.controller = controller;
			modelEditorControl.Dock = DockStyle.Fill;
			((Control)ViewSiteControl).Controls.Add(modelEditorControl);
			if(settingsStorage != null) {
				new FormStateAndBoundsManager().Load(this, settingsStorage);
			}
			Image modelEditorImage = ImageLoader.Instance.GetLargeImageInfo("EditModel").Image;
			if(modelEditorImage != null) {
				this.Icon = Icon.FromHandle(new Bitmap(modelEditorImage).GetHicon());
			}
			Text = Title;
			Disposed += new EventHandler(ModelEditorForm_Disposed);
			this.controller.LoadSettings();
			this.Tag = "testdialog=ModelEditor";
		}
		void ModelEditorForm_Disposed(object sender, EventArgs e) {
		}
		public void SetCaption(string text) {
			Text = string.IsNullOrEmpty(text) ? Title : string.Format("{0} - {1}", text, Title);
		}
		protected virtual DialogResult CloseModelEditorDialog() {
			return ModelEditorViewController.CloseModelEditorDialog;
		}
		protected override void OnClosing(CancelEventArgs e) {
			if(controller.IsModified) {
				switch(CloseModelEditorDialog()) {
					case DialogResult.Yes: {
							if(controller.Save()) {
								DialogResult = DialogResult.Yes;
							}
							else {
								e.Cancel = true; 
							}
							break;
						}
					case DialogResult.No: {
							dropModelDifs = true;
							break;
						}
					case DialogResult.Cancel: {
							e.Cancel = true;
							break;
						}
				}
			}
			if(!e.Cancel) {
				ModelEditorSaveSettings(); 
			}
		}
		protected override void OnClosed(EventArgs e) {
			if(dropModelDifs) {
				controller.ReloadModel(false, false);
			}
			modelEditorControl.OnClosed();
			controller.Dispose();
			controller = null;
			base.OnClosed(e);
		}
		protected override void Dispose(bool disposing) {
			lock(ModelEditorViewController.LockDisposeObject) {
				if(controller != null) {
					controller.IsDisposing = true;
				}
				base.Dispose(disposing);
				controller = null;
				settingsStorage = null;
				if(disposing) {
					modelEditorControl.Dispose();
				}
			}
		}
		#region IModelEditorSettingsStorage Members
		public void ModelEditorSaveSettings() {
			if(controller != null) {
				controller.SaveSettings();
			}
			modelEditorControl.CurrentModelTreeListNode = null; 
			new FormStateAndBoundsManager().Save(this, settingsStorage);
		}
		#endregion
	}
}
