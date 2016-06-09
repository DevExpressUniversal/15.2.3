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
using System.ComponentModel.Design;
using System.Windows.Forms;
using EnvDTE;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraReports.UserDesigner;
namespace DevExpress.XtraReports.Design {
	public abstract class ComponentBootstrap : ComponentDesigner {
		protected abstract void CreateRealComponent(IDesignerHost designerHost);
		public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			IDesignerHost designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
			designerHost.DestroyComponent(Component);
			ProjectItem projectItem = designerHost.GetService(typeof(ProjectItem)) as ProjectItem;
			if(projectItem != null) {
				Property prop = projectItem.ContainingProject.Properties.Item("TargetFrameworkMoniker");
				if(prop != null) {
					string mokiker = prop.Value as String;
					if(mokiker.EndsWith("Profile=Client", StringComparison.OrdinalIgnoreCase))
						throw new InvalidOperationException("The .NET Framework Client Profile is not supported by this component. Correct the target framework in the project properties.");
				}
			}
			CreateRealComponent(designerHost);
		}
		protected void SetComonentName(IComponent component, IDesignerHost designerHost, Type type) {
			INameCreationService serv = designerHost.GetService(typeof(INameCreationService)) as INameCreationService;
			string name = serv.CreateName(designerHost.Container, type);
			PropertyDescriptor prop = TypeDescriptor.GetProperties(component)["Name"];
			prop.SetValue(component, name);
		}
	}
	public class StandardReportDesignerBootstrap : ComponentBootstrap {
		protected override void CreateRealComponent(IDesignerHost designerHost) {
			if(DevExpress.XtraPrinting.Native.DesignHelpers.FindSameOrInheritedComponent(designerHost.Container, typeof(XRDesignRibbonController)) != null)
				throw new InvalidOperationException("You have RibbonReportDesigner installed. Remove it before adding StandardReportDesigner.");			
			if(DevExpress.XtraPrinting.Native.DesignHelpers.FindComponent(designerHost.Container, typeof(XRDesignMdiController)) == null) {
				IComponent comp =  DevExpress.XtraPrinting.Native.DesignHelpers.CreateComponent(designerHost, typeof(XRDesignMdiController));
				SetComonentName(comp, designerHost, typeof(DevExpress.XtraReports.Fake.ReportDesigner));
			}
			if(DevExpress.XtraPrinting.Native.DesignHelpers.FindSameOrInheritedComponent(designerHost.Container, typeof(XRDesignBarManager)) == null)
				DevExpress.XtraPrinting.Native.DesignHelpers.CreateComponent(designerHost, typeof(DevExpress.XtraReports.UserDesigner.XRDesignBarManager));
			if(DevExpress.XtraPrinting.Native.DesignHelpers.FindSameOrInheritedComponent(designerHost.Container, typeof(XRDesignDockManager)) == null)
				DevExpress.XtraPrinting.Native.DesignHelpers.CreateComponent(designerHost, typeof(XRDesignDockManager));
		}
		public StandardReportDesignerBootstrap() { 
		}
	}
	public class RibbonReportDesignerBootstrap : ComponentBootstrap {
		protected override void CreateRealComponent(IDesignerHost designerHost) {
			if(DevExpress.XtraPrinting.Native.DesignHelpers.FindSameOrInheritedComponent(designerHost.Container, typeof(XRDesignBarManager)) != null)
				throw new InvalidOperationException("You have StandardReportDesigner installed. Remove it before adding RibbonReportDesigner.");
			if(DevExpress.XtraPrinting.Native.DesignHelpers.FindComponent(designerHost.Container, typeof(XRDesignMdiController)) == null) {
				IComponent comp = DevExpress.XtraPrinting.Native.DesignHelpers.CreateComponent(designerHost, typeof(XRDesignMdiController));
				SetComonentName(comp, designerHost, typeof(DevExpress.XtraReports.Fake.ReportDesigner));
			}
			if(DevExpress.XtraPrinting.Native.DesignHelpers.FindSameOrInheritedComponent(designerHost.Container, typeof(XRDesignRibbonController)) == null)
				DevExpress.XtraPrinting.Native.DesignHelpers.CreateComponent(designerHost, typeof(DevExpress.XtraReports.UserDesigner.XRDesignRibbonController));
			if(DevExpress.XtraPrinting.Native.DesignHelpers.FindSameOrInheritedComponent(designerHost.Container, typeof(XRDesignDockManager)) == null)
				DevExpress.XtraPrinting.Native.DesignHelpers.CreateComponent(designerHost, typeof(XRDesignDockManager));
		}
		public RibbonReportDesignerBootstrap() {
		}
	}
}
namespace DevExpress.XtraReports.Fake {
	class ReportDesigner { 
	}
}
