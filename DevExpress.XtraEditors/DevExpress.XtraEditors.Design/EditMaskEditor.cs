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
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraEditors.Design {
	public class MaskDesigner : UITypeEditor {
		protected object GetInstance(ITypeDescriptorContext context) {
			if(context == null) return null;
			if(context.Instance is RepositoryItemTextEdit) return context.Instance;
			TextEdit edit = context.Instance as TextEdit;
			if(edit != null) return edit.Properties;
			return null;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (context != null &&
				GetInstance(context) is RepositoryItemTextEdit &&
				provider != null) {
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) {
					RepositoryItemTextEdit repositoryItem = (RepositoryItemTextEdit)GetInstance(context);
					using(EditMaskEditorForm form = new EditMaskEditorForm()) {
						IComponentChangeService service = (IComponentChangeService)provider.GetService(typeof(IComponentChangeService));
						EditMaskEditorComponentChangedHandler handler = new EditMaskEditorComponentChangedHandler(delegate(object sender, EditMaskEditorComponentChangedEventArgs e) {
							if(service != null){
								service.OnComponentChanged(e.Component, null, null, null);
							}
						});
						form.RepositoryItem = repositoryItem;
						form.ComponentChanged += handler;
						try {
							edSvc.ShowDialog(form);
						} finally {
							form.ComponentChanged -= handler;
						}
						return repositoryItem.Mask;
					}
				}
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(GetInstance(context) is RepositoryItemTextEdit) {
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
}
