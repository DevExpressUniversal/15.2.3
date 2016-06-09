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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using DevExpress.Serialization.Services;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Serialization;
namespace DevExpress.XtraReports.UserDesigner.Native {
	public class XRDesignerHost : XRDesignerHostBase, IDisposable {
		STypeResolutionService typeResolutionService;
		IExtenderProvider nameExtender;
		protected bool disposing;
		public override bool Loading {
			get { return disposing || base.Loading; }
		}
		public XRDesignerHost(IServiceProvider parentServiceProvider)
			: base(parentServiceProvider) {
			AddServices();
		}
		protected override void Dispose(bool disp) {
			if(disp) {
				disposing = true;
				try {
					base.Dispose(disp);
					RemoveServices();
				} finally {
					disposing = false;
				}
			} else
				base.Dispose(disp);
		}
		void AddServices() {
			AddService(typeof(IEnvironmentService), new EnvironmentService());
			AddService(typeof(System.ComponentModel.Design.IComponentChangeService), new XRComponentChangeService());
			AddService(typeof(System.ComponentModel.Design.ITypeDescriptorFilterService), new XRTypeDescriptorFilterService());
			AddService(typeof(System.Drawing.Design.IToolboxService), new XRToolboxService());
			XRExtenderService extenderService = new XRExtenderService();
			AddService(typeof(System.ComponentModel.Design.IExtenderListService), extenderService);
			AddService(typeof(System.ComponentModel.Design.IExtenderProviderService), extenderService);
			AddService(typeof(System.ComponentModel.Design.IDesignerHost), this);
			AddService(typeof(System.ComponentModel.IContainer), Container);
			AddService(typeof(System.ComponentModel.Design.IDictionaryService), new SDictionaryService());
			AddService(typeof(System.ComponentModel.Design.ISelectionService), new XRSelectionService(this));
			AddService(typeof(System.ComponentModel.Design.Serialization.INameCreationService), new XRNameCreationService(this));
			AddService(typeof(System.ComponentModel.Design.IDesignerEventService), new XRDesignerEventService());
			typeResolutionService = new STypeResolutionService(this, typeof(DevExpress.XtraReports.UI.XtraReport).Assembly);
			AddService(typeof(ITypeResolutionService), typeResolutionService);
			AddService(typeof(System.ComponentModel.Design.IReferenceService), new SReferenceService(this));
			AddService(typeof(IDesignerSerializationService), new XRDesignerSerializationService(this));
			AddService(typeof(ComponentSerializationService), new CodeDomComponentSerializationService(this));
		}
		void RemoveServices() {
			RemoveService(typeof(IEnvironmentService));
			RemoveService(typeof(System.ComponentModel.Design.IComponentChangeService));
			RemoveService(typeof(System.ComponentModel.Design.ITypeDescriptorFilterService));
			RemoveService(typeof(System.Drawing.Design.IToolboxService));
			RemoveService(typeof(System.ComponentModel.Design.IExtenderListService));
			if(nameExtender != null) {
				IExtenderProviderService extenderProviderService = (IExtenderProviderService)GetService(typeof(IExtenderProviderService));
				if(extenderProviderService != null)
					extenderProviderService.RemoveExtenderProvider(nameExtender);
				nameExtender = null;
			}
			RemoveService(typeof(System.ComponentModel.Design.IExtenderProviderService));
			RemoveService(typeof(System.ComponentModel.Design.IDesignerHost));
			RemoveService(typeof(System.ComponentModel.IContainer));
			RemoveService(typeof(System.ComponentModel.Design.IDictionaryService));
			RemoveService(typeof(System.ComponentModel.Design.ISelectionService));
			RemoveService(typeof(System.ComponentModel.Design.Serialization.INameCreationService));
			RemoveService(typeof(System.ComponentModel.Design.IDesignerEventService));
			typeResolutionService.Dispose();
			RemoveService(typeof(ITypeResolutionService));
			RemoveService(typeof(System.ComponentModel.Design.IReferenceService));
			RemoveService(typeof(IDesignerSerializationService));
			RemoveService(typeof(ComponentSerializationService));
			try {
				TypeDescriptor.Refresh(GetType().Assembly);
			} catch {
			}
		}
	}
}
