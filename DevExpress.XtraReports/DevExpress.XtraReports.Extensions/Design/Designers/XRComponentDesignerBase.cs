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
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using System.Collections;
namespace DevExpress.XtraReports.Design {
	public class XRComponentDesignerBase : ComponentDesigner {
		public static void RaiseFilterComponentProperties(IComponent component, IDictionary properties) {
			if(component.Site != null) {
				IDesignerHost host = component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
				RaiseFilterComponentProperties(host, component, properties);
			}
		}
		static void RaiseFilterComponentProperties(IDesignerHost host, IComponent component, IDictionary properties) {
			if(host != null && DesignToolHelper.IsEndUserDesigner(host))
				XtraReport.RaiseFilterComponentProperties(host.RootComponent as XtraReport, component, properties);
		}
		public bool IsInheritedReadonly {
			get { return InheritanceAttribute == InheritanceAttribute.InheritedReadOnly; }
		}
		public bool IsInherited {
			get { return InheritanceAttribute != InheritanceAttribute.NotInherited; }
		}
		protected override void PreFilterEvents(IDictionary events) {
			base.PreFilterEvents(events);
			IPropertyFilterService filter = GetService(typeof(IPropertyFilterService)) as IPropertyFilterService;
			if(filter != null)
				filter.PreFilterEvents(events, Component);
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			RaiseFilterComponentProperties(GetService(typeof(IDesignerHost)) as IDesignerHost, Component, properties);
			IPropertyFilterService filter = GetService(typeof(IPropertyFilterService)) as IPropertyFilterService;
			if(filter != null) filter.PreFilterProperties(properties, Component);
			string[] names = GetFilteredProperties();
			foreach(string name in names) {
				if(properties.Contains(name))
					properties.Remove(name);
			}
		}
		protected virtual string[] GetFilteredProperties() {
			return new string[] { };
		}
		protected void OnXRDesignerVerbInvoke(object sender, EventArgs e) {
			XRDesignerVerb xrVerb = sender as XRDesignerVerb;
			if(xrVerb == null)
				return;
			IMenuCommandService menuCommandService = Component.Site.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if(menuCommandService != null)
				menuCommandService.GlobalInvoke(xrVerb.CommandID);
		}
	}
}
