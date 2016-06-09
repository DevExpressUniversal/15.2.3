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

using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Web.ASPxTreeList.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Utils.Localization;
using DevExpress.Persistent.Base;
using System.Xml;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.TreeListEditors.Web {
	public class ASPxTreeListResourceLocalizerProvider : ActiveLocalizerProvider<ASPxTreeListStringId> {
		protected override XtraLocalizer<ASPxTreeListStringId> GetActiveLocalizerCore() {
			return ASPxTreeListResourceLocalizer.Active;
		}
		protected override void SetActiveLocalizerCore(XtraLocalizer<ASPxTreeListStringId> localizer) {
		}
		public ASPxTreeListResourceLocalizerProvider()
			: base(null) {
		}
	}
	[System.ComponentModel.DisplayName("ASPxTreeList Control")]
	public class ASPxTreeListResourceLocalizer : ASPxTreeListLocalizer, IXmlResourceLocalizer {
		private ControlXmlResourcesLocalizerLogic<ASPxTreeListStringId> logic;
		public ASPxTreeListResourceLocalizer() {
			logic = new ControlXmlResourcesLocalizerLogic<ASPxTreeListStringId>(this);
		}
		public override string GetLocalizedString(ASPxTreeListStringId id) {
			return logic.GetString(id, base.GetLocalizedString(id));
		}
		public static new ASPxTreeListResourceLocalizer Active {
			get {
				return ValueManager.GetValueManager<ASPxTreeListResourceLocalizer>("ASPxTreeListResourceLocalizer_ASPxTreeListResourceLocalizer").Value;
			}
			set {
				ValueManager.GetValueManager<ASPxTreeListResourceLocalizer>("ASPxTreeListResourceLocalizer_ASPxTreeListResourceLocalizer").Value = value;
			}
		}
		#region IXmlResourceLocalizer Members
		public void Setup(IModelApplication applicationNode) {
			logic.Setup(applicationNode);
			if(ASPxTreeListLocalizer.Active != this) {
				ASPxTreeListLocalizer.SetActiveLocalizerProvider(new ASPxTreeListResourceLocalizerProvider());
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
						"DevExpress.ASPxTreeList",
						"ASPxTreeListStringId.",
						this
						);
				}
				return xafResourceManagerParameters;
			}
		}
		#endregion
	}
}
