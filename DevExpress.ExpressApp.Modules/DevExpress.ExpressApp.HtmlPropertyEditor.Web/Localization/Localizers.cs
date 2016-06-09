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
using DevExpress.ExpressApp.Localization;
using DevExpress.Persistent.Base;
using DevExpress.Web.ASPxHtmlEditor.Localization;
using System.Xml;
using DevExpress.Utils.Localization;
using DevExpress.Web.ASPxSpellChecker.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.HtmlPropertyEditor.Web.Localization {
	public class ASPxHtmlEditorResourceLocalizerProvider : ActiveLocalizerProvider<ASPxHtmlEditorStringId> {
		protected override XtraLocalizer<ASPxHtmlEditorStringId> GetActiveLocalizerCore() {
			return ASPxHtmlEditorResourceLocalizer.Active;
		}
		protected override void SetActiveLocalizerCore(XtraLocalizer<ASPxHtmlEditorStringId> localizer) {
		}
		public ASPxHtmlEditorResourceLocalizerProvider()
			: base(null) {
		}
	}
	[System.ComponentModel.DisplayName("ASPxHtmlEditor Control")]
	public class ASPxHtmlEditorResourceLocalizer : ASPxHtmlEditorLocalizer, IXmlResourceLocalizer {
		private ControlXmlResourcesLocalizerLogic<ASPxHtmlEditorStringId> logic;
		public ASPxHtmlEditorResourceLocalizer() {
			logic = new ControlXmlResourcesLocalizerLogic<ASPxHtmlEditorStringId>(this);
		}
		public override string GetLocalizedString(ASPxHtmlEditorStringId id) {
			return logic.GetString(id, base.GetLocalizedString(id));
		}
		public static new ASPxHtmlEditorResourceLocalizer Active {
			get {
				return ValueManager.GetValueManager<ASPxHtmlEditorResourceLocalizer>("ASPxHtmlEditorResourceLocalizer_ASPxHtmlEditorResourceLocalizer").Value;
			}
			set {
				ValueManager.GetValueManager<ASPxHtmlEditorResourceLocalizer>("ASPxHtmlEditorResourceLocalizer_ASPxHtmlEditorResourceLocalizer").Value = value;
			}
		}
		#region IXmlResourceLocalizer Members
		public void Setup(IModelApplication applicationNode) {
			logic.Setup(applicationNode);
			if(ASPxHtmlEditorLocalizer.Active != this) {
				ASPxHtmlEditorLocalizer.SetActiveLocalizerProvider(new ASPxHtmlEditorResourceLocalizerProvider());
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
						"DevExpress.ASPxHtmlEditor",
						"ASPxHtmlEditorStringId.",
						this
						);
				}
				return xafResourceManagerParameters;
			}
		}
		#endregion
	}
	public class ASPxSpellCheckerResourceLocalizerProvider : ActiveLocalizerProvider<ASPxSpellCheckerStringId> {
		protected override XtraLocalizer<ASPxSpellCheckerStringId> GetActiveLocalizerCore() {
			return ASPxSpellCheckerResourceLocalizer.Active;
		}
		protected override void SetActiveLocalizerCore(XtraLocalizer<ASPxSpellCheckerStringId> localizer) {
		}
		public ASPxSpellCheckerResourceLocalizerProvider()
			: base(null) {
		}
	}
	[System.ComponentModel.DisplayName("ASPxSpellChecker Control")]
	public class ASPxSpellCheckerResourceLocalizer : ASPxSpellCheckerLocalizer, IXmlResourceLocalizer {
		private ControlXmlResourcesLocalizerLogic<ASPxSpellCheckerStringId> logic;
		public ASPxSpellCheckerResourceLocalizer() {
			logic = new ControlXmlResourcesLocalizerLogic<ASPxSpellCheckerStringId>(this);
		}
		public override string GetLocalizedString(ASPxSpellCheckerStringId id) {
			return logic.GetString(id, base.GetLocalizedString(id));
		}
		public static new ASPxSpellCheckerResourceLocalizer Active {
			get {
				return ValueManager.GetValueManager<ASPxSpellCheckerResourceLocalizer>("ASPxSpellCheckerResourceLocalizer_ASPxSpellCheckerResourceLocalizer").Value;
			}
			set {
				ValueManager.GetValueManager<ASPxSpellCheckerResourceLocalizer>("ASPxSpellCheckerResourceLocalizer_ASPxSpellCheckerResourceLocalizer").Value = value;
			}
		}
		#region IXmlResourceLocalizer Members
		public void Setup(IModelApplication applicationNode) {
			logic.Setup(applicationNode);
			if(ASPxSpellCheckerLocalizer.Active != this) {
				ASPxSpellCheckerLocalizer.SetActiveLocalizerProvider(new ASPxSpellCheckerResourceLocalizerProvider());
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
						"DevExpress.ASPxSpellChecker",
						"ASPxSpellCheckerStringId.",
						this
						);
				}
				return xafResourceManagerParameters;
			}
		}
		#endregion
	}
}
