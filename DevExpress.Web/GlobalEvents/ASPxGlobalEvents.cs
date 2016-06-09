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
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[DXWebToolboxItem(DXToolboxItemKind.Free), NonVisualControl, DefaultProperty("ClientSideEvents"),
	Designer("DevExpress.Web.Design.ASPxGlobalEventsDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxGlobalEvents"),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabComponents),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxGlobalEvents.bmp")
	]
	public class ASPxGlobalEvents : ASPxWebComponent {
		protected internal const string GlobalEventsScriptResourceName = WebScriptsResourcePath + "GlobalEvents.js";
		private static readonly object ValidationCompletedEventKey = new object();
		public ASPxGlobalEvents()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGlobalEventsClientSideEvents"),
#endif
		Category("Client-Side"),
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public GlobalEventsClientSideEvents ClientSideEvents {
			get { return (GlobalEventsClientSideEvents)ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGlobalEventsValidationCompleted"),
#endif
		Category("Global")]
		public event EventHandler<ValidationCompletedEventArgs> ValidationCompleted
		{
			add { Events.AddHandler(ValidationCompletedEventKey, value); }
			remove { Events.RemoveHandler(ValidationCompletedEventKey, value); }
		}
		public static List<ASPxGlobalEvents> GetInstances(Page page) {
			return page != null ? (page.Items[typeof(ASPxGlobalEvents)] as List<ASPxGlobalEvents>) : null;
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			Register(Page);
		}
		protected void Register(Page page) {
			if(Page == null || DesignMode)
				return;
			if(page.Items[typeof(ASPxGlobalEvents)] == null)
				page.Items[typeof(ASPxGlobalEvents)] = new List<ASPxGlobalEvents>();
			List<ASPxGlobalEvents> globalEvents = GetInstances(page);
			if(!globalEvents.Contains(this))
				globalEvents.Add(this);
		}
		protected override bool NeedVerifyRenderingInServerForm() {
			return false;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxGlobalEvents), GlobalEventsScriptResourceName);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new GlobalEventsClientSideEvents();
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientGlobalEvents";
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void GetFinalizeClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			stb.Length = 0;
			stb.Append("var dxo = ASPx.GetGlobalEvents();\n");
			stb.Append(ClientSideEventsInternal.GetStartupScript("dxo"));
		}
		internal virtual void OnValidationCompleted(object sender, ValidationCompletedEventArgs e) {
			EventHandler<ValidationCompletedEventArgs> handler =
				(EventHandler<ValidationCompletedEventArgs>)Events[ValidationCompletedEventKey];
			if(handler != null)
				handler(null, e);
		}
	}
}
