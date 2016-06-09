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
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Globalization;
using DevExpress.Utils.Design;
using DevExpress.Utils.About;
namespace DevExpress.XtraEditors.Design {
	public class DXValidationProviderDesigner : ComponentDesigner {
		DesignerVerbCollection verbs;
		public DXValidationProviderDesigner()
			: base() {
		}
		public DXValidationProvider ValidationProvider { get { return this.Component as DXValidationProvider; } }
		void OnSetupValidationRules(object sender, EventArgs e) {
			new ValidationRulesEditorForm((IDesignerHost)GetService(typeof(IDesignerHost)), ValidationProvider, null).ShowDialog();
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
		}
		public override DesignerVerbCollection Verbs { 
			get {
				if(verbs == null) {
					verbs = new DesignerVerbCollection();
					verbs.Add(new DesignerVerb(Properties.Resources.CustomizeValidationRulesCaption, OnSetupValidationRules));
					DXSmartTagsHelper.CreateDefaultVerbs(this, verbs);
				}
				return verbs; 
			} 
		}
	}
	public class ValidationRulesUITypeEditor : UITypeEditor {
		public ValidationRulesUITypeEditor() { }
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if((context != null) && (provider != null)) {
				DXValidationProvider validationProvider = null;
				string validationProviderName = context.PropertyDescriptor.DisplayName.Split(new char[] { ' ' })[2];
				ComponentCollection components = context.Container.Components;
				for(int i = 0; i < components.Count; i++) {
					validationProvider = components[i] as DXValidationProvider;
					if((validationProvider != null) && (validationProvider.Site.Name == validationProviderName)) break;
				}
				object[] selectedComponents = context.Instance as object[];
				if(selectedComponents == null) selectedComponents = new object[] { context.Instance };
				new ValidationRulesEditorForm((IDesignerHost)provider.GetService(typeof(IDesignerHost)), validationProvider, selectedComponents).ShowDialog();
				((ISelectionService)provider.GetService(typeof(ISelectionService))).SetSelectedComponents(selectedComponents);
			}
			return base.EditValue(context, provider, value);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null) {
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
	public class ValidatingControlListUITypeEditor : UITypeEditor {
		public ValidatingControlListUITypeEditor() : base() { }
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null) {
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			IWindowsFormsEditorService editorService = null;
			IValidatingControlCollection controlList = null;
			Hashtable controlNames = new Hashtable();
			if(provider != null) editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if(context != null) controlList = context.Instance as IValidatingControlCollection;
			if(editorService != null && controlList != null) {
				ControlsListBox listBox = new ControlsListBox(editorService);
				if(controlList.Controls != null) {
					foreach(Control control in controlList.Controls) {
						string name = TypeDescriptor.GetComponentName(control);
						if(name == null) continue;
						controlNames[name] = control;
					}
					ArrayList names = new ArrayList(controlNames.Keys);
					names.Insert(0, "(none)");
					listBox.Items.AddRange(names.ToArray());
				}
				editorService.DropDownControl(listBox);
				if(string.Equals(listBox.SelectedItem, "(none)")) value = null;
				else {
					if(listBox.SelectedItem != null) {
						Control control = controlNames[(string)listBox.SelectedItem] as Control;
						value = control as Control;
					}
				}
				listBox.Items.Clear();
				controlNames.Clear();
			}
			return base.EditValue(context, provider, value);
		}
		public class ControlsListBox : ListBox {
			IWindowsFormsEditorService editorService;
			public ControlsListBox(IWindowsFormsEditorService editorService) {
				this.editorService = editorService;
				this.Sorted = true;
			}
			protected override void OnMouseClick(MouseEventArgs e) {
				base.OnMouseClick(e);
				editorService.CloseDropDown();
			}
		}
	}
}
