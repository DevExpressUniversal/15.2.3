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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.UI.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
namespace DevExpress.Web.Design {
	public abstract class TypeEditorBase: System.Drawing.Design.UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			ISelectionService selectionService = (ISelectionService)provider.GetService(typeof(ISelectionService));
			object component = (selectionService != null) ? selectionService.PrimarySelection : null;
			if(component == null && context.Instance != null) {
				object instance = context.Instance;
				IDesignTimePropertiesOwner componentOwner = instance as IDesignTimePropertiesOwner;
				while(componentOwner != null) {
					instance = componentOwner.Owner;
					componentOwner = instance as IDesignTimePropertiesOwner;
				}
				if(component == null && instance is IComponent) {
					component = instance;
				}
			}
			Form form = CreateEditorForm(component, context, provider, value);
			if(component != null)
				DesignUtils.ShowDialogOnInvokeTransactedChange(form, (IComponent)component);
			else
				form.ShowDialog();
			return value;
		}
		public abstract Form CreateEditorForm(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue);
	}
	public class CollectionEditor: TypeEditorBase {
		public override Form CreateEditorForm(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue) {
			return new CollectionEditorForm(component, context, provider, propertyValue);
		}
	}
	public class TypeDescriptorContext: ITypeDescriptorContext {
		private IContainer fContainer;
		private object fInstance;
		PropertyDescriptor fPropertyDescriptor;
		IContainer ITypeDescriptorContext.Container { get { return fContainer; } }
		object ITypeDescriptorContext.Instance { get { return fInstance; } }
		PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor { get { return fPropertyDescriptor; } }
		public TypeDescriptorContext(IContainer container, object instance, PropertyDescriptor propertyDescriptor) {
			fContainer = container;
			fInstance = instance;
			fPropertyDescriptor = propertyDescriptor;
		}
		bool ITypeDescriptorContext.OnComponentChanging() {
			return false;
		}
		void ITypeDescriptorContext.OnComponentChanged() {
		}
		object IServiceProvider.GetService(Type serviceType) {
			return null;
		}
	}
	public class CssFileNameEditor : UrlEditor {
		protected override string Filter {
			get { return "Web Documents(*.css)|*.css"; }
		}
	}
	public abstract class DropDownUITypeEditorBase : UITypeEditor {
		public sealed override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public sealed override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(provider != null) {
				IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if(editorService != null) {
					ListBox listBox = CreateValueListBox(editorService, context);
					SetInitiallySelectedValue(listBox, context);
					editorService.DropDownControl(listBox);
					if(listBox.SelectedIndex != -1) {
						IComponentChangeService componentChangeService = (IComponentChangeService)provider.GetService(typeof(IComponentChangeService));
						if(componentChangeService != null) {
							object component = GetComponent(context);
							PropertyDescriptor changedPropertyDescriptor = GetChangedPropertyDescriptor(component);
							object oldValue = null;
							object newValue = null;
							if(changedPropertyDescriptor != null)
								oldValue = changedPropertyDescriptor.GetValue(component);
							ApplySelectedValue(listBox, context);
							if(changedPropertyDescriptor != null)
								newValue = changedPropertyDescriptor.GetValue(component);
							if(changedPropertyDescriptor == null || !object.Equals(oldValue, newValue)) {
								var descriptor = context != null ? context.PropertyDescriptor : null;
								componentChangeService.OnComponentChanged(component, descriptor, oldValue, newValue);
							}
						}
					}
				}
			}
			return value;
		}
		protected abstract void ApplySelectedValue(ListBox valueList, ITypeDescriptorContext context);
		protected abstract object GetComponent(ITypeDescriptorContext context);
		protected abstract PropertyDescriptor GetChangedPropertyDescriptor(object component);
		protected abstract void FillValueList(ListBox valueList, ITypeDescriptorContext context);
		protected abstract void SetInitiallySelectedValue(ListBox valueList, ITypeDescriptorContext context);
		private ListBox CreateValueListBox(IWindowsFormsEditorService editorService, ITypeDescriptorContext context) {
			UIListBox listBox = new UIListBox(editorService);
			listBox.BeginUpdate();
			FillValueList(listBox, context);
			listBox.EndUpdate();
			return listBox;
		}
	}
}
