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
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Internal;
	[ToolboxItem(false)]
	public class MVCxDockManager : ASPxDockManager {
		public MVCxDockManager()
			: base() {
		}
		public ImagesBase Images {
			get { return base.ImagesInternal; }
		}
		public StylesBase Styles {
			get { return base.StylesInternal; }
		}
		public object CallbackRouteValues { get; set; }
		public override bool IsCallback {
			get { return MvcUtils.CallbackName == ID; }
		}
		protected internal override bool IsCallBacksEnabled() {
			return CallbackRouteValues != null;
		}
		public new IEnumerable<MVCxDockZone> Zones {
			get {
				foreach(ASPxDockZone zone in base.Zones)
					yield return (MVCxDockZone)zone;
			}
		}
		public new IEnumerable<MVCxDockPanel> Panels {
			get {
				foreach(ASPxDockPanel panel in base.Panels)
					yield return (MVCxDockPanel)panel;
			}
		}
		public new MVCxDockPanel FindPanelByUID(string panelUID) {
			return (MVCxDockPanel)base.FindPanelByUID(panelUID);
		}
		public new MVCxDockZone FindZoneByUID(string zoneUID) {
			return (MVCxDockZone)base.FindZoneByUID(zoneUID);
		}
		public new void ResetLayoutToInitial() {
			base.ResetLayoutToInitial();
			ClientLayoutState = base.SaveClientState();
		}
		protected internal override string SaveClientState() {
			return IsCallback ? Request.Params["ClientLayoutState"] : base.SaveClientState();
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientDockManager";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxDockManager), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxDockManager), Utils.DockManagerScriptResourceName);
		}
		protected internal new void EnsureClientStateLoaded() {
			base.EnsureClientStateLoaded();
		}
		protected override bool NeedLoadClientState() {
			return !IsCallback;
		}
	}
}
