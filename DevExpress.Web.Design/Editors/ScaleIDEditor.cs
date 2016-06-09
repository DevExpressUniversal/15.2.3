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
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.ComponentModel.Design;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Base;
using System.Drawing;
namespace DevExpress.Web.ASPxGauges.Design {
	public class ScaleIDTypeEditor : UITypeEditor {
		IWindowsFormsEditorService editorService = null;
		readonly string NoneElement = "(none)";
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if((context != null) && (context.Instance != null)) return UITypeEditorEditStyle.DropDown;
			return base.GetEditStyle(context);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if(editorService == null) return value;
			string id = value as string;
			if(string.IsNullOrEmpty(id)) value = NoneElement;
			using(EditorDropDownForm dropDown = new EditorDropDownForm(this, editorService, value, GetExistingScales(context, provider))) {
				editorService.DropDownControl(dropDown);
				id = (dropDown.EditValue as string == NoneElement) ? null : dropDown.EditValue as string;
			}
			return id;
		}
		string[] GetExistingScales(ITypeDescriptorContext context, IServiceProvider provider) {
			IDesignerHost host = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			bool fNeedScale = (context.Instance is IScaleComponent);
			bool fNeedArcScale = (context.Instance is IArcScaleComponent);
			bool fNeedLinearScale = (context.Instance is ILinearScaleComponent);
			List<string> names = new List<string>();
			names.Add(NoneElement);
			if(host != null) {
				foreach(IComponent component in host.Container.Components) {
					if(component is IScale && component is INamed) {
						if(fNeedArcScale && component is IArcScale) {
							names.Add(((INamed)component).Name);
							continue;
						}
						if(fNeedLinearScale && component is ILinearScale) {
							names.Add(((INamed)component).Name);
							continue;
						}
						if(fNeedScale) {
							names.Add(((INamed)component).Name);
							continue;
						}
					}
				}
			}
			return names.ToArray();
		}
	}
	[ToolboxItem(false)]
	public class EditorDropDownForm : Panel {
		UITypeEditor editorCore;
		IWindowsFormsEditorService editorService;
		object editValueCore;
		ListBox listBoxCore;
		object originalValueCore;
		public EditorDropDownForm(UITypeEditor editor, IWindowsFormsEditorService edSvc, object editValue, Array values) {
			this.listBoxCore = new ListBox();
			this.editValueCore = this.originalValueCore = editValue;
			this.editorCore = editor;
			base.BorderStyle = BorderStyle.None;
			this.editorService = edSvc;
			List.BorderStyle = BorderStyle.None;
			List.Dock = DockStyle.Fill;
			foreach(object obj in values)
				List.Items.Add(obj);
			if(List.Items.Contains(EditValue))
				List.SelectedItem = EditValue;
			else
				List.SelectedIndex = 1;
			List.SelectedValueChanged += OnSelectedValueChanged;
			List.Click += OnClick;
			Controls.Add(List);
			List.CreateControl();
			Size = new Size(0, List.ItemHeight * Math.Min(List.Items.Count, 10));
		}
		void OnClick(object sender, EventArgs e) {
			this.editorService.CloseDropDown();
		}
		void OnSelectedValueChanged(object sender, EventArgs e) {
			this.EditValue = List.SelectedItem;
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			List.Focus();
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Return) {
				this.editorService.CloseDropDown();
				return true;
			}
			if(keyData == Keys.Escape) {
				this.editValueCore = this.originalValueCore;
				this.editorService.CloseDropDown();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		protected UITypeEditor Editor {
			get { return editorCore; }
		}
		protected ListBox List {
			get { return listBoxCore; }
		}
		public object EditValue {
			get { return editValueCore; }
			set {
				if(EditValue == value) return;
				this.editValueCore = value;
			}
		}
	}
}
