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
using System.Text;
using System.Collections;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data;
using DevExpress.Data.Native;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.WinControls;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UI.BarCode;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Shape.Native;
using System.Collections.Generic;
namespace DevExpress.XtraReports.Design {
	public class XRControlStyleEditor : ObjectPickerEditor {
		#region static
		static bool CanEditValue(ITypeDescriptorContext context) {
			return
				context != null &&
				(context.Instance is XRControl.XRControlStyles ||
				context.Instance is Formatting ||
				context.Instance is System.Array);
		}
		#endregion
		protected override ObjectPickerControl CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			IDesignerHost designerHost = context.GetService(typeof(IDesignerHost)) as IDesignerHost;
			XtraReport report = (XtraReport)designerHost.RootComponent;
			string[] names = report == null ? new string[0] : report.StyleContainer.GetNames();
			List<String> values = new List<string>();
			values.AddRange(names);
			using(var comparer = new DevExpress.Utils.NaturalStringComparer(values.Count))
				values.Sort(comparer);
			values.Insert(0, DesignSR.DataGridNoneString);
			values.Insert(1, DesignSR.DataGridNewString);
			return new PickerFromValuesControl(this, value, values, false);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(!CanEditValue(context))
			   return value;
			XRControlStyle oldStyle = value as XRControlStyle;
			string oldStyleName = oldStyle != null ? oldStyle.Name : DesignSR.DataGridNoneString;
			string styleName = (string)base.EditValue(context, provider, oldStyleName);
			if(styleName == oldStyleName)
				return value;
			IDesignerHost designerHost = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			XtraReport report = (XtraReport)designerHost.RootComponent;
			DesignerTransaction transaction = designerHost.CreateTransaction(string.Format("Change Styles.{0}", context.PropertyDescriptor.Name));
			IComponentChangeService changeService = provider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			try {
				XRControlStyle newStyle = null;
				if(report.StyleContainer[styleName] != null)
					newStyle = report.StyleContainer[styleName];
				if(styleName == DesignSR.DataGridNewString) {
					newStyle = new XRControlStyle(report.Dpi);
					designerHost.Container.Add(newStyle);
				}
				XRControl.XRControlStyles[] stylesArray = XRControlStylesConverter.GetStyles(context);
				foreach(XRControl.XRControlStyles styles in stylesArray) {
					XRControl ownerControl = styles.Owner;
					changeService.OnComponentChanging(ownerControl, XRAccessor.GetPropertyDescriptor(ownerControl, GetSerializableStylePropertyName(context)));
					TypeDescriptor.GetProperties(styles)[context.PropertyDescriptor.Name].SetValue(styles, newStyle);
					changeService.OnComponentChanged(ownerControl, XRAccessor.GetPropertyDescriptor(ownerControl, GetSerializableStylePropertyName(context)), null, null);
				}
				return newStyle;
			} catch {
				transaction.Cancel();
			} finally {
				transaction.Commit();
			}
			return value;
		}
		static string GetSerializableStylePropertyName(ITypeDescriptorContext context) {
			return string.Format("{0}Name", context.PropertyDescriptor.Name);
		}
	}
}
