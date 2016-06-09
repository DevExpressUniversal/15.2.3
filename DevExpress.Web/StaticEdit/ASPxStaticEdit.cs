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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	public abstract class StaticEditProperties : EditPropertiesBase {
		public StaticEditProperties()
			: base() {
		}
		public StaticEditProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		protected internal EditorCaptionSettingsBase CaptionSettings {
			get { return base.CaptionSettingsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("StaticEditPropertiesClientSideEvents"),
#endif
		Category("Client-Side"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public new StaticEditClientSideEvents ClientSideEvents {
			get { return (StaticEditClientSideEvents)base.ClientSideEvents; }
		}
		protected internal bool AllowEllipsisInText {
			get { return GetBoolProperty("AllowEllipsisInText", false); }
			set {
				if(AllowEllipsisInText == value)
					return;
				SetBoolProperty("AllowEllipsisInText", false, value);
				Changed();
			}
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new StaticEditClientSideEvents();
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				var src = source as StaticEditProperties;
				if(src != null)
					AllowEllipsisInText = src.AllowEllipsisInText;
			} finally {
				EndUpdate();
			}
		}
	}
	public abstract class ASPxStaticEdit : ASPxEditBase {
		protected internal const string StaticEditScriptResourceName = EditScriptsResourcePath + "StaticEdit.js";
		protected internal const string ClickHandlerName = "return ASPx.SEClick('{0}', event)";
		public ASPxStaticEdit()
			: base() {
		}
		protected ASPxStaticEdit(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxStaticEditCaptionSettings"),
#endif
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorCaptionSettingsBase CaptionSettings {
			get { return Properties.CaptionSettingsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxStaticEditClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public StaticEditClientSideEvents ClientSideEvents {
			get { return (StaticEditClientSideEvents)ClientSideEventsInternal; }
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(ASPxStaticEdit), StaticEditScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientStaticEdit";
		}
		protected override bool NeedVerifyRenderingInServerForm() {
			return false;
		}
		protected internal string GetOnClick() {
			if(ClientSideEvents.Click != "" && Enabled)
				return string.Format(ClickHandlerName, ClientID);
			return "";
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			return false;
		}
	}
}
