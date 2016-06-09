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
using System.ComponentModel.Design;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.Design.LightSwitch;
namespace DevExpress.XtraReports.Design {
	public class LightSwitchDataSourceDesigner : ComponentDesigner {
		LightSwitchDataSource LightSwitchDataSource {
			get {
				return (LightSwitchDataSource)Component;
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			Verbs.AddRange(new DesignerVerb[] { new DesignerVerb("Update QueryParameters", UpdateQueryParameters) });
			IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			designerHost.LoadComplete += new EventHandler(designerHost_LoadComplete);
			IComponentChangeService componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentChanged += new ComponentChangedEventHandler(componentChangeService_ComponentChanged);
		}
		void designerHost_LoadComplete(object sender, EventArgs e) {
			Verbs[0].Visible = LightSwitchDataSource != null && LightSwitchDataSource.IsQuery;
		}
		void componentChangeService_ComponentChanged(object sender, ComponentChangedEventArgs e) {
			if (e.Component is LightSwitchDataSource && e.Member != null && e.Member.Name == "IsQuery") {
				Verbs[0].Visible = LightSwitchDataSource.IsQuery;
			}
		}
		void UpdateQueryParameters(object sender, EventArgs eventArgs) {
			IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			DesignerTransaction transaction = designerHost.CreateTransaction();
			try {
				QueryParameter[] parameters = DevExpress.XtraReports.Design.LightSwitch.QueryParametersHelper.GetParameters(designerHost, LightSwitchDataSource.DataSourceName, LightSwitchDataSource.CollectionName);
				IComponentChangeService componentChangeService = (IComponentChangeService)designerHost.GetService(typeof(IComponentChangeService));
				componentChangeService.OnComponentChanging(LightSwitchDataSource, null);
				QueryParametersHelper.RemoveQueryParameters(LightSwitchDataSource, designerHost);
				LightSwitchDataSource.QueryParameters.AddRange(parameters);
				foreach (Parameter parameter in LightSwitchDataSource.QueryParameters) {
					DesignToolHelper.AddToContainer(designerHost, parameter, parameter.Name);
				} 
				componentChangeService.OnComponentChanged(LightSwitchDataSource, null, null, null);
				transaction.Commit();
			}
			catch(Exception e) {
				transaction.Cancel();
				if(e is InvalidOperationException)
					throw e;
			}
		}
		protected override void PreFilterProperties(IDictionary properties) {
			if (!LightSwitchDataSource.IsQuery && properties["QueryParameters"] != null) {
				Attribute[] attributes = new Attribute[] { BrowsableAttribute.No };
				properties["QueryParameters"] = TypeDescriptor.CreateProperty(typeof(LightSwitchDataSource), (PropertyDescriptor)properties["QueryParameters"], attributes);
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				QueryParametersHelper.RemoveQueryParameters(LightSwitchDataSource, (IDesignerHost)GetService(typeof(IDesignerHost)));
			}
			base.Dispose(disposing);
		}
		public override ICollection AssociatedComponents {
			get {
				return LightSwitchDataSource.QueryParameters;
			}
		}
	}
}
