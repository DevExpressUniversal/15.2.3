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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Web.Internal;
using DevExpress.XtraEditors;
using DevExpress.Utils.Design;
namespace DevExpress.Web.Design {
	public class IconUITypeEditor : UITypeEditor {
		IWindowsFormsEditorService smartTagEditorService = null;
		static IconEditorForm iconEditorForm;
		public static IconEditorForm IconEditorForm {
			get {
				if(iconEditorForm == null)
					iconEditorForm = new IconEditorForm();
				return iconEditorForm;
			}
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(!ValidateProperties(context.Instance))
				return value;
			this.smartTagEditorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if(this.smartTagEditorService != null) {
				IconEditorForm.InitServices(provider);
				DialogResult result = this.smartTagEditorService.ShowDialog(IconEditorForm);
				if(result == DialogResult.OK)
					return IconEditorForm.EditValue;
				else if(result == DialogResult.Abort)
					return string.Empty;
			}
			return value;
		}
		bool ValidateProperties(object instance) {
			var properties = instance as object[];
			if(properties == null)
				return GetImagePropertiesRecursive(instance) != null;
			foreach(var property in properties) {
				if(GetImagePropertiesRecursive(property) == null)
					return false;
			}
			return true;
		}
		ImagePropertiesBase GetImagePropertiesRecursive(object instance) {
			var result = instance as ImagePropertiesBase;
			if(result != null)
				return result;
			var objectWrapper = instance as IDXObjectWrapper;
			if(objectWrapper == null)
				return null;
			result = objectWrapper.SourceObject as ImagePropertiesBase;
			return result != null ? result : GetImagePropertiesRecursive(objectWrapper.SourceObject);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
}
