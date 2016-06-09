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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Utils;
namespace DevExpress.Utils.UI {
	public class TypeDescriptorContext : NativeDesignerHost, IWindowsFormsEditorService, ITypeDescriptorContext {
		readonly object obj;
		readonly PropertyDescriptor propertyDescriptor;
		readonly IServiceProvider parentServiceProvider;
		readonly IComponentChangeService changeService;
		public TypeDescriptorContext(object obj, PropertyDescriptor propertyDescriptor, IServiceProvider parentServiceProvider) {
			this.obj = obj;
			this.propertyDescriptor = propertyDescriptor;
			this.parentServiceProvider = parentServiceProvider;
			this.changeService = (parentServiceProvider != null) ? parentServiceProvider.GetService<IComponentChangeService>() : null;
		}
		#region ITypeDescriptorContext Members
		public object Instance {
			get { return obj; }
		}
		public void OnComponentChanged() {
			if(this.changeService != null) {
				this.changeService.OnComponentChanged(obj, propertyDescriptor, null, null);
			}
		}
		public bool OnComponentChanging() {
			if(this.changeService != null) {
				try {
					this.changeService.OnComponentChanging(obj, propertyDescriptor);
				} catch(CheckoutException exception) {
					if(exception != CheckoutException.Canceled) {
						throw exception;
					}
					return false;
				}
			}
			return true;
		}
		public PropertyDescriptor PropertyDescriptor {
			get { return propertyDescriptor; }
		}
		#endregion
		#region IServiceProvider Members
		public override object GetService(Type serviceType) {
			if (parentServiceProvider != null) {
				object service = parentServiceProvider.GetService(serviceType);
				if (service != null)
					return service;
			}
			if (serviceType == typeof(ITypeDescriptorContext) || serviceType == typeof(IWindowsFormsEditorService) || serviceType == typeof(IDesignerHost)) {
				return this;
			}
			return base.GetService(serviceType);
		}
		#endregion
		#region IWindowsFormsEditorService Members
		public void CloseDropDown() {
		}
		public void DropDownControl(System.Windows.Forms.Control control) {
		}
		public System.Windows.Forms.DialogResult ShowDialog(System.Windows.Forms.Form dialog) {
			IWin32Window parent = parentServiceProvider.GetService(typeof(IWin32Window)) as IWin32Window;
			if(parent == null)
				parent = obj as IWin32Window;
			return dialog.ShowDialog(parent);
		}
		#endregion
	}
}
