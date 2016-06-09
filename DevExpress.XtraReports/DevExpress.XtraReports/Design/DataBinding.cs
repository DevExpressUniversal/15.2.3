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
using DevExpress.Data.Browsing.Design;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.UI;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting.Native;
using System.Collections;
namespace DevExpress.XtraReports.Design {
	public class DataBinding {
		#region static
		public static bool IsThereBinding(object instance) {
			foreach(DataBinding item in AllDataBindings(instance)) {
				if(item.Binding != null && !item.Binding.IsNull)
					return true;
			}
			return false;
		}
		static IEnumerable<DataBinding> AllDataBindings(object instance) {
			if(instance is DataBinding) {
				yield return (DataBinding)instance;
			} else if(instance is IEnumerable) {
				foreach(object obj in (IEnumerable)instance)
					if(obj is DataBinding)
						yield return (DataBinding)obj;
			}
		}
		#endregion
		XRControl xrControl;
		DesignBinding binding;
		string formatString = string.Empty;
		string dataBindingName;
		[
		Editor("DevExpress.XtraReports.Design.DataBindingFormatStringEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.Design.DataBinding.FormatString"),
		]
		public string FormatString { get { return formatString; } set { formatString = value; UpdateDataBindings(); } }
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.Design.DataBinding.Binding"),
		RefreshPropertiesAttribute(System.ComponentModel.RefreshProperties.All),
		]
		public DesignBinding Binding { get { return binding; } set { binding = value; UpdateDataBindings(); } }
		public DataBinding(XRControl xrControl, DesignBinding binding, string formatString, string dataBindingName) {
			this.xrControl = xrControl;
			this.binding = binding;
			this.formatString = formatString;
			this.dataBindingName = dataBindingName;
		}
		public DataBinding(XRControl xrControl, object dataSource, string dataMember, string formatString, string dataBindingName) {
			this.xrControl = xrControl;
			this.binding = new DesignBinding(dataSource, dataMember);
			this.formatString = formatString;
			this.dataBindingName = dataBindingName;
		}
		void UpdateDataBindings() {
			IComponentChangeService changeSvc = (IComponentChangeService)xrControl.Site.GetService(typeof(IComponentChangeService));
			PropertyDescriptor property = TypeDescriptor.GetProperties(xrControl)["DataBindings"];
			StringHelper.ValidateFormatString(FormatString);
			if(changeSvc != null && property != null)
				changeSvc.OnComponentChanging(xrControl, property);
			SetIntoDataBindings();
			if(changeSvc != null && property != null)
				changeSvc.OnComponentChanged(xrControl, property, null, null);
		}
		internal void SetIntoDataBindings() {
			XRBinding xrBinding = xrControl.DataBindings[dataBindingName];
			if(xrBinding != null)
				xrControl.DataBindings.Remove(xrBinding);
			if(!Binding.IsNull)
				xrControl.DataBindings.Add(XRBinding.Create(dataBindingName, Binding.DataSource, Binding.DataMember, FormatString));
		}
	}
}
