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
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap.Design {
	public abstract class GenericTypePickerEditor<T> : UITypeEditor where T : class {
		IWindowsFormsEditorService edSvc;
		protected abstract GenericTypePickerControl<T> CreateObjectPickerControl(ITypeDescriptorContext context, object value);
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (!CanEditValue(context, provider))
				return value;
			using (GenericTypePickerControl<T> objectPickerControl = CreateObjectPickerControl(context, value)) {
				objectPickerControl.Initialize(context);
				edSvc.DropDownControl(objectPickerControl);
				object newValue = objectPickerControl.EditValue;
				if (newValue == null)
					value = newValue;
				else {
					Type type = newValue as Type;
					if (type != null && (value == null || type != value.GetType()))
						value = CreateInstanceByType(type);
				}
			}
			return value;
		}
		protected virtual bool CanEditValue(ITypeDescriptorContext context, IServiceProvider provider) {
			if (context == null || context.Instance == null || provider == null)
				return false;
			edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if (edSvc == null)
				return false;
			return true;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null) {
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
		internal void CloseDropDown() {
			edSvc.CloseDropDown();
		}
		public virtual object CreateInstanceByType(Type type) {
			return Activator.CreateInstance(type);
		}
	}
	[ToolboxItem(false)]
	public abstract class GenericTypePickerControl<T> : UserControl where T : class {
		readonly GenericTypePickerEditor<T> editor;
		DevExpress.XtraEditors.ListBoxControl listBox;
		object editValue;
		object initialValue;
		Type[] supportedTypes;
		bool supportNoneItem = true;
		protected internal bool SupportNoneItem { get { return supportNoneItem; } set { supportNoneItem = value; } }
		public object EditValue { get { return editValue; } }
		public GenericTypePickerEditor<T> Editor { get { return editor; } }
		protected GenericTypePickerControl(GenericTypePickerEditor<T> editor, object editValue) {
			this.editor = editor;
			this.initialValue = editValue;
			this.editValue = editValue;
			BorderStyle = BorderStyle.None;
			listBox = new DevExpress.XtraEditors.ListBoxControl();
			listBox.Dock = DockStyle.Fill;
			listBox.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			listBox.MouseUp += new MouseEventHandler(lbMouseUp);
			listBox.SelectedIndexChanged += new EventHandler(lbSelectedIndexChanged);
			Controls.Add(listBox);
		}
		protected virtual Type[] GetSupportedTypes(ITypeDescriptorContext context) {
			return MapUtils.GetTypeDescendants(MapUtils.XtraMapAssembly, typeof(T), DesignHelper.IgnoredTypes);
		}
		public virtual void Initialize(ITypeDescriptorContext context) {
			listBox.BeginUpdate();
			if (SupportNoneItem)
				listBox.Items.Add(DesignSR.NoneString);
			supportedTypes = GetSupportedTypes(context);
			for (int i = 0; i < supportedTypes.Length; i++) {
				listBox.Items.Add(supportedTypes[i].Name);
			}
			listBox.EndUpdate();
			if (initialValue != null) {
				listBox.SelectedValue = initialValue.GetType().Name;
			} else {
				if (SupportNoneItem) listBox.SelectedIndex = 0;
			}
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			this.listBox.Focus();
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if (keyData == Keys.Enter) {
				editor.CloseDropDown();
				return true;
			}
			if (keyData == Keys.Escape) {
				editValue = initialValue;
				editor.CloseDropDown();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		void lbSelectedIndexChanged(object sender, EventArgs e) {
			int index = SupportNoneItem ? listBox.SelectedIndex - 1 : listBox.SelectedIndex;
			editValue = index >= 0 ? supportedTypes[index] : null;
		}
		void lbMouseUp(object sender, MouseEventArgs e) {
			editor.CloseDropDown();
		}
	}
}
