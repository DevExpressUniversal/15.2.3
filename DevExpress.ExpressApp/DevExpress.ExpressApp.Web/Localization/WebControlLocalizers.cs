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
using DevExpress.Web;
using DevExpress.ExpressApp.Utils;
using System.Resources;
using DevExpress.Persistent.Base;
using DevExpress.Web.Localization;
using System.Xml;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Web.Localization {
	public class ASPxGridViewResourceLocalizerProvider : ActiveLocalizerProvider<ASPxGridViewStringId> {
		protected override XtraLocalizer<ASPxGridViewStringId> GetActiveLocalizerCore() {
			return ASPxGridViewResourceLocalizer.Active;
		}
		protected override void SetActiveLocalizerCore(XtraLocalizer<ASPxGridViewStringId> localizer) {
		}
		public ASPxGridViewResourceLocalizerProvider()
			: base(new ASPxGridViewResLocalizer(new ASPxGridViewDefaultLocalizer())) {
		}
	}
	internal class ASPxGridViewDefaultLocalizer : ASPxGridViewLocalizer {
		public override string GetLocalizedString(ASPxGridViewStringId id) {
			if(id == ASPxGridViewStringId.CustomizationWindowCaption) {
				return "Column Chooser";
			}
			return base.GetLocalizedString(id);
		}
	}
	[System.ComponentModel.DisplayName("ASPxGridView Control")]
	public class ASPxGridViewResourceLocalizer : ASPxGridViewLocalizer, IXmlResourceLocalizer {
		private ControlXmlResourcesLocalizerLogic<ASPxGridViewStringId> logic;
		public ASPxGridViewResourceLocalizer() {
			logic = new ControlXmlResourcesLocalizerLogic<ASPxGridViewStringId>(this);
		}
		public override string GetLocalizedString(ASPxGridViewStringId id) {
			return logic.GetString(id, base.GetLocalizedString(id));
		}
		public static new ASPxGridViewResourceLocalizer Active {
			get {
				return SessionLocalizerProvider<ASPxGridViewResourceLocalizer, ASPxGridViewStringId>.XafActiveLocalizer;
			}
			set {
				SessionLocalizerProvider<ASPxGridViewResourceLocalizer, ASPxGridViewStringId>.XafActiveLocalizer = value;
			}
		}
		#region IXmlResourceLocalizer Members
		public void Setup(IModelApplication applicationNode) {
			logic.Setup(applicationNode);
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
						"DevExpress.ASPxGridView",
						"ASPxGridViewStringId.",
						this
						);
				}
				return xafResourceManagerParameters;
			}
		}
		#endregion
	}
	[System.ComponentModel.DisplayName("ASPxGridListEditor Control")]
	public class ASPxGridViewControlLocalizer : XafResourceLocalizer {
		private static ASPxGridViewControlLocalizer activeLocalizer;
		static ASPxGridViewControlLocalizer() {
			activeLocalizer = new ASPxGridViewControlLocalizer();
		}
		protected override IXafResourceManagerParameters GetXafResourceManagerParameters() {
			return new XafResourceManagerParameters(
				"DevExpress.ASPxGridView",
				"DevExpress.ExpressApp.Web.Editors.ASPx.GridListEditor.Localization",
				String.Empty,
				GetType().Assembly
				);
		}
		public static ASPxGridViewControlLocalizer Active {
			get { return activeLocalizer; }
			set { activeLocalizer = value; }
		}
		public override void Activate() {
			Active = this;
		}
	}
	public enum ASPxCriteriaPropertyEditorId {
		EnterValueText
	}
	[System.ComponentModel.DisplayName("ASPxCriteriaPropertyEditor Control")]
	public class ASPxCriteriaPropertyEditorLocalizer : XafResourceLocalizer<ASPxCriteriaPropertyEditorId> {
		private static ASPxCriteriaPropertyEditorLocalizer activeLocalizer;
		static ASPxCriteriaPropertyEditorLocalizer() {
			activeLocalizer = new ASPxCriteriaPropertyEditorLocalizer();
		}
		protected override IXafResourceManagerParameters GetXafResourceManagerParameters() {
			return new XafResourceManagerParameters(
				"ASPxCriteriaEdit",
				"DevExpress.ExpressApp.Web.Resources.ASPxCriteriaPropertyEditorLocalization",
				String.Empty,
				GetType().Assembly
				);
		}
		public static ASPxCriteriaPropertyEditorLocalizer Active {
			get { return activeLocalizer; }
			set { activeLocalizer = value; }
		}
		public override void Activate() {
			Active = this;
		}
	}
	[System.ComponentModel.DisplayName("ASPxImagePropertyEditor Control")]
	public class ASPxImagePropertyEditorLocalizer : XafResourceLocalizer {
		private static ASPxImagePropertyEditorLocalizer activeLocalizer;
		static ASPxImagePropertyEditorLocalizer() {
			activeLocalizer = new ASPxImagePropertyEditorLocalizer();
		}
		protected override IXafResourceManagerParameters GetXafResourceManagerParameters() {
			return new XafResourceManagerParameters(
				null,
				new string[] { XafResourceManager.ControlsLocalizationNodeName, "DropDownImageEdit" },
				"DevExpress.ExpressApp.Web.Editors.ASPx.ImagePropertyEditor.Localization",
				String.Empty,
				GetType().Assembly
				);
		}
		public static ASPxImagePropertyEditorLocalizer Active {
			get { return activeLocalizer; }
			set { activeLocalizer = value; }
		}
		public override void Activate() {
			Active = this;
		}
	}
	[System.ComponentModel.DisplayName("ASPxEditors Control")]
	public class ASPxEditorsResourceLocalizer : ASPxEditorsLocalizer, IXmlResourceLocalizer {
		private ControlXmlResourcesLocalizerLogic<ASPxEditorsStringId> logic;
		public ASPxEditorsResourceLocalizer() {
			logic = new ControlXmlResourcesLocalizerLogic<ASPxEditorsStringId>(this);
		}
		public override string GetLocalizedString(ASPxEditorsStringId id) {
			return logic.GetString(id, base.GetLocalizedString(id));
		}
		#region IXmlResourceLocalizer Members
		public void Setup(IModelApplication applicationNode) {
			logic.Setup(applicationNode);
			SessionLocalizerProvider<ASPxEditorsResourceLocalizer, DevExpress.Web.Localization.ASPxEditorsStringId>.XafActiveLocalizer = this;
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
						"DevExpress.ASPxEditors",
						"ASPxEditorsStringId.",
						this
						);
				}
				return xafResourceManagerParameters;
			}
		}
		#endregion
	}
	public class ASPxperienceResourceLocalizerProvider : ActiveLocalizerProvider<ASPxperienceStringId> {
		protected override XtraLocalizer<ASPxperienceStringId> GetActiveLocalizerCore() {
			return ASPxperienceResourceLocalizer.Active;
		}
		protected override void SetActiveLocalizerCore(XtraLocalizer<ASPxperienceStringId> localizer) {
		}
		public ASPxperienceResourceLocalizerProvider()
			: base(null) {
		}
	}
	[System.ComponentModel.DisplayName("ASPxperience Control")]
	public class ASPxperienceResourceLocalizer : ASPxperienceLocalizer, IXmlResourceLocalizer {
		private ControlXmlResourcesLocalizerLogic<ASPxperienceStringId> logic;
		public ASPxperienceResourceLocalizer() {
			logic = new ControlXmlResourcesLocalizerLogic<ASPxperienceStringId>(this);
		}
		public override string GetLocalizedString(ASPxperienceStringId id) {
			return logic.GetString(id, base.GetLocalizedString(id));
		}
		public static new ASPxperienceResourceLocalizer Active {
			get {
				return SessionLocalizerProvider<ASPxperienceResourceLocalizer, ASPxperienceStringId>.XafActiveLocalizer;
			}
			set {
				SessionLocalizerProvider<ASPxperienceResourceLocalizer, ASPxperienceStringId>.XafActiveLocalizer = value;
			}
		}
		#region IXmlResourceLocalizer Members
		public void Setup(IModelApplication applicationNode) {
			logic.Setup(applicationNode);
			if(ASPxperienceLocalizer.Active != this) {
				ASPxperienceLocalizer.SetActiveLocalizerProvider(new ASPxperienceResourceLocalizerProvider());
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
						"DevExpress.ASPxperience",
						"ASPxperienceStringId.",
						this
						);
				}
				return xafResourceManagerParameters;
			}
		}
		#endregion
	}
}
