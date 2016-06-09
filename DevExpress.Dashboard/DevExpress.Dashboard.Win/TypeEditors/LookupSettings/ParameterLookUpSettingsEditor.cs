#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.Design;
using DevExpress.DashboardCommon;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.DashboardWin.Native {
	public class ParameterLookUpSettingsEditor : ListSelectorEditor {
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123")]
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			PropertyDescriptor descriptor = context.PropertyDescriptor;
			if(descriptor != null && descriptor.Converter != null) {
				IWindowsFormsEditorService editServ = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
				ICollection collection = descriptor.Converter.GetStandardValues(context);
				Type type = value != null ? value.GetType() : null;
				using(PopupListBox listBox = new PopupListBox(collection, type, false, editServ)) {
					LookAndFeelProviderHelper.SetParentLookAndFeel(listBox, provider);
					editServ.DropDownControl(listBox);
					type = listBox.SelectedItemValue as Type;
					if(type != null)
						value = Activator.CreateInstance(type);
					else
						value = null;
				}
			}
			return value;
		}
	}
}
