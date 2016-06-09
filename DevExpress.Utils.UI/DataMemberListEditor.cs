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
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using DevExpress.Data.Browsing.Design;
namespace DevExpress.XtraReports.Design {
	public abstract class DataMemberListEditor : UITypeEditor {
		DesignTreeListBindingPicker designTreeListBindingPicker;
		public override bool IsDropDownResizable { get { return true; } }
		protected abstract object GetDataSource(object instance, IServiceProvider provider);
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return (context != null && context.Instance != null) ? UITypeEditorEditStyle.DropDown : base.GetEditStyle(context);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(provider != null) {
				try {
					IWindowsFormsEditorService editServ = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
					if(designTreeListBindingPicker == null || designTreeListBindingPicker.IsDisposed)
						designTreeListBindingPicker = new TreeListPicker();
					object dataSource = GetDataSource(context.Instance, provider);
					string dataMember = (string)value;
					if(dataMember == null)
						dataMember = String.Empty;
					designTreeListBindingPicker.Start(new object[] { dataSource }, provider, new DesignBinding(dataSource, dataMember));
					editServ.DropDownControl(designTreeListBindingPicker);
					value = designTreeListBindingPicker.SelectedBinding.DataMember;
					designTreeListBindingPicker.End();
				}
				catch {
				}
			}
			return value;
		}
	}
}
