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
using DevExpress.Web.ASPxScheduler;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Design;
using DevExpress.XtraScheduler.Design.Wizards;
namespace DevExpress.Web.ASPxScheduler.Design.Wizards {
	#region AddFormTemplatesWizard
	public class AddFormTemplatesWizard {
		IDesignerHost host;
		public AddFormTemplatesWizard(IDesignerHost host) {
			this.host = host;
		}
		public IDesignerHost Host { get { return host; } }
		public virtual void Execute() {
		}
	}
	#endregion
	#region ASPxSetupAppointmentCustomFieldMappingsWizard
	public class ASPxSetupAppointmentCustomFieldMappingsWizard : SetupCustomFieldMappingsWizard<Appointment> {
		public ASPxSetupAppointmentCustomFieldMappingsWizard(IDesignerHost host, IPersistentObjectStorage<Appointment> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
		}
		protected internal override CustomFieldMappingBase<Appointment> CreateCustomFieldMapping() {
			return new ASPxAppointmentCustomFieldMapping();
		}
	}
	#endregion
	#region ASPxSetupResourceCustomFieldMappingsWizard
	public class ASPxSetupResourceCustomFieldMappingsWizard : SetupCustomFieldMappingsWizard<Resource> {
		public ASPxSetupResourceCustomFieldMappingsWizard(IDesignerHost host, IPersistentObjectStorage<Resource> objectStorage, IDataFieldsProvider dataFieldsProvider)
			: base(host, objectStorage, dataFieldsProvider) {
		}
		protected internal override CustomFieldMappingBase<Resource> CreateCustomFieldMapping() {
			return new ASPxResourceCustomFieldMapping();
		}
	}
	#endregion
}
