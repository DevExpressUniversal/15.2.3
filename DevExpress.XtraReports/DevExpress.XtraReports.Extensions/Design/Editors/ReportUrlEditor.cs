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
using System.ComponentModel;
using DevExpress.XtraReports.Native;
using System.Windows.Forms.Design;
using System.Windows.Forms;
namespace DevExpress.XtraReports.Design {
	public class ReportUrlEditor : UITypeEditor {
		public ReportUrlEditor()
			: base() {
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(GetEditStyle(context) == UITypeEditorEditStyle.DropDown)
				return RunPicker(context, provider, value);
			string result = ReportStorageServiceInteractive.GetNewUrl();
			return !string.IsNullOrEmpty(result) ? result : value;
		}
		string RunPicker(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(provider != null) {
				try {
					string[] standardNames = ReportStorageServiceInteractive.GetStandardUrls(context);
					ReportUrlPicker picker = new ReportUrlPicker(standardNames);
					picker.SelectionMode = SelectionMode.One;
					string itemName = (value != null) ? (string)value : string.Empty;
					IWindowsFormsEditorService editServ = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
					picker.Start(context, editServ, itemName);
					editServ.DropDownControl(picker);
					value = Array.IndexOf<string>(standardNames, picker.ItemName) >= 0 ? 
						picker.ItemName :
						string.Empty;
					picker.End();
				} catch { }
			}
			return (string)value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext typeDescriptorContext) {
			return ReportStorageServiceInteractive.GetStandardUrlsSupported(typeDescriptorContext) ? 
				UITypeEditorEditStyle.DropDown :
				UITypeEditorEditStyle.Modal;
		}
	}
	public class ReportUrlPicker : PickerBase {
		string[] standardNames;
		public ReportUrlPicker(string[] standardNames) {
			this.standardNames = standardNames;
		}
		protected override string[] GetItemNames() {
			return standardNames;
		}
	}
}
