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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.EditForm.Helpers.Controls;
namespace DevExpress.XtraGrid.Views.Grid {
	[ProvideProperty("BoundFieldName", typeof(Control))]
	[ProvideProperty("BoundPropertyName", typeof(Control))]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData), DXToolboxItem(DXToolboxItemKind.Regular)]
	[Description("A custom Edit Form for the GridControl.")]
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "EditFormUserControl")]
	public partial class EditFormUserControl : XtraUserControl, IExtenderProvider, IEditorFormTagProvider {
		Dictionary<Control, string> fields = new Dictionary<Control, string>();
		Dictionary<Control, string> properties = new Dictionary<Control, string>();
		public EditFormUserControl() {
			InitializeComponent();
		}
		#region IExtenderProvider Members
		bool IExtenderProvider.CanExtend(object extendee) {
			Control c = extendee as Control;
			return c != null && c != this;
		}
		[DefaultValue(""), RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public string GetBoundFieldName(Control c) {
			if(fields.ContainsKey(c)) return fields[c];
			return string.Empty;
		}
		bool HasEditValue(Control c) {
			return c is BaseEdit;
		}
		string GetDefaultProperty(Control c) { return HasEditValue(c) ? "EditValue" : "Text"; }
		public void SetBoundFieldName(Control c, string value) {
			if(value != null) value = value.Trim();
			if(string.IsNullOrEmpty(value)) {
				if(fields.ContainsKey(c)) fields.Remove(c);
				SetBoundPropertyName(c, "");
			}
			else {
				fields[c] = value;
				if(!properties.ContainsKey(c)) properties[c] = GetDefaultProperty(c);
			}
		}
		[DefaultValue("Text"), TypeConverter(typeof(BindPropertyConverter))]
		public string GetBoundPropertyName(Control c) {
			if(properties.ContainsKey(c)) return properties[c];
			if(fields.ContainsKey(c)) return GetDefaultProperty(c);
			return string.Empty;
		}
		public void SetBoundPropertyName(Control c, string value) {
			if(value != null) {
				value = value.Trim();
				if(!string.IsNullOrEmpty(value) && TypeDescriptor.GetProperties(c)[value] == null)
					value = null;
			}
			if(string.IsNullOrEmpty(value)) {
				if(properties.ContainsKey(c)) properties.Remove(c);
			}
			else {
				properties[c] = value;
			}
		}
		#endregion
		#region IEditorFormTagProvider Members
		string IEditorFormTagProvider.GetFieldName(Control control) {
			return GetBoundFieldName(control);
		}
		string IEditorFormTagProvider.GetPropertyName(Control control) {
			return GetBoundPropertyName(control);
		}
		#endregion
		protected internal virtual void SetMenuManager(IDXMenuManager menuManager) { }
	}
	internal class BindPropertyConverter : TypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) {
				var list = TypeDescriptor.GetProperties(context.Instance).Cast<PropertyDescriptor>().Select(q => q.Name).OrderBy(q => q).ToList();
				list.Insert(0, "");
				if(list.Contains("Text")) {
					list.Remove("Text");
					list.Insert(0, "Text");
				}
				if(list.Contains("EditValue")) {
					list.Remove("EditValue");
					list.Insert(0, "EditValue");
				}
				return new StandardValuesCollection(list);
			}
			return null;
		}
	}
}
