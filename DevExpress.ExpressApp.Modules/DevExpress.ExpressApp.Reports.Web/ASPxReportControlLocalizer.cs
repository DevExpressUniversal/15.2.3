#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using System.Resources;
using System.Xml;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Utils.Localization;
using DevExpress.Web.Localization;
using DevExpress.XtraReports.Web.Localization;
namespace DevExpress.ExpressApp.Reports.Web {
	public class ASPxReportControlLocalizerProvider : ASPxActiveLocalizerProvider<ASPxReportsStringId> {
		protected override XtraLocalizer<ASPxReportsStringId> GetActiveLocalizerCore() {
			return ASPxReportControlLocalizer.Active;
		}
		protected override void SetActiveLocalizerCore(XtraLocalizer<ASPxReportsStringId> localizer) {
		}
		public ASPxReportControlLocalizerProvider()
			: base(null) {
		}
	}
	[System.ComponentModel.DisplayName("ASPxReport Control")]
	public class ASPxReportControlLocalizer : ASPxReportsResLocalizer, IXmlResourceLocalizer {
		private ControlXmlResourcesLocalizerLogic<ASPxReportsStringId> logic;
		public ASPxReportControlLocalizer() {
			logic = new ControlXmlResourcesLocalizerLogic<ASPxReportsStringId>(this);
		}
		private string GetStringName(ASPxReportsStringId id) {
			return "ASPxReportsStringId." + id.ToString();
		}
		private ResourceManager Manager {
			get {
				return logic.Manager;
			}
		}
		public override string GetLocalizedString(ASPxReportsStringId id) {
			return Manager.GetString(GetStringName(id));
		}
		public static new ASPxReportControlLocalizer Active {
			get {
				return ValueManager.GetValueManager<ASPxReportControlLocalizer>("ASPxReportControlLocalizer_ASPxReportControlLocalizer").Value;
			}
			set {
				ValueManager.GetValueManager<ASPxReportControlLocalizer>("ASPxReportControlLocalizer_ASPxReportControlLocalizer").Value = value;
			}
		}
		#region IXmlResourceLocalizer Members
		public void Setup(IModelApplication applicationNode) {
			logic.Setup(applicationNode);
			if(ASPxReportsResLocalizer.Active != this) {
				ASPxReportsResLocalizer.SetActiveLocalizerProvider(new ASPxReportControlLocalizerProvider());
			}
			Active = this;
		}
		public void Reset() {
			logic.Reset();
		}
		public XmlDocument GetXmlResources() {
			return logic.GetXmlResources();
		}
		#endregion
		#region IXafResourceManagerParametersProvider Members
		private IXafResourceManagerParameters xafResourceManagerParameters;
		public IXafResourceManagerParameters XafResourceManagerParameters {
			get {
				if(xafResourceManagerParameters == null) {
					xafResourceManagerParameters = new XafResourceManagerParameters(
							"DevExpress.XtraReports",
							"ASPxReportsStringId.",
							this
							);
				}
				return xafResourceManagerParameters;
			}
		}
		#endregion
	}
}
