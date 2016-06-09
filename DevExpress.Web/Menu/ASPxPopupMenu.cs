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
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[DXWebToolboxItem(true), ToolboxData("<{0}:ASPxPopupMenu runat=\"server\"></{0}:ASPxPopupMenu>"),
	Designer("DevExpress.Web.Design.ASPxPopupMenuDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxPopupMenu.bmp")
	]
	public class ASPxPopupMenu: ASPxMenuBase {
		protected internal const string PopupMenuScriptResourceName = WebScriptsResourcePath + "PopupMenu.js";
		private static readonly object EventPopupElementResolve = new object();
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupMenuLeft"),
#endif
		DefaultValue(0), AutoFormatDisable]
		public int Left {
			get { return GetIntProperty("Left", 0); }
			set { SetIntProperty("Left", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupMenuPopupElementID"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string PopupElementID {
			get { return GetStringProperty("PopupElementID", ""); }
			set { SetStringProperty("PopupElementID", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupMenuPopupAction"),
#endif
		DefaultValue(PopupAction.RightMouseClick), AutoFormatDisable]
		public PopupAction PopupAction {
			get { return (PopupAction)GetEnumProperty("PopupAction", PopupAction.RightMouseClick); }
			set { SetEnumProperty("PopupAction", PopupAction.RightMouseClick, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupMenuCloseAction"),
#endif
		DefaultValue(PopupMenuCloseAction.OuterMouseClick), AutoFormatDisable]
		public PopupMenuCloseAction CloseAction {
			get { return (PopupMenuCloseAction)GetEnumProperty("CloseAction", PopupMenuCloseAction.OuterMouseClick); }
			set { SetEnumProperty("CloseAction", PopupMenuCloseAction.OuterMouseClick, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupMenuEnableScrolling"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnableScrolling {
			get { return EnableScrollingInternal; }
			set { EnableScrollingInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupMenuPopupHorizontalAlign"),
#endif
		DefaultValue(PopupHorizontalAlign.NotSet), AutoFormatDisable]
		public PopupHorizontalAlign PopupHorizontalAlign {
			get { return (PopupHorizontalAlign)GetEnumProperty("PopupHorizontalAlign", PopupHorizontalAlign.NotSet); }
			set { SetEnumProperty("PopupHorizontalAlign", PopupHorizontalAlign.NotSet, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupMenuPopupAlignCorrection"),
#endif
		DefaultValue(PopupAlignCorrection.Auto), AutoFormatDisable]
		public PopupAlignCorrection PopupAlignCorrection {
			get { return (PopupAlignCorrection)GetEnumProperty("PopupAlignCorrection", PopupAlignCorrection.Auto); }
			set { SetEnumProperty("PopupAlignCorrection", PopupAlignCorrection.Auto, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupMenuPopupHorizontalOffset"),
#endif
		DefaultValue(0), AutoFormatDisable]
		public int PopupHorizontalOffset {
			get { return (int)GetIntProperty("PopupHorizontalOffset", 0); }
			set { SetIntProperty("PopupHorizontalOffset", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupMenuPopupVerticalAlign"),
#endif
		DefaultValue(PopupVerticalAlign.NotSet), AutoFormatDisable]
		public PopupVerticalAlign PopupVerticalAlign {
			get { return (PopupVerticalAlign)GetEnumProperty("PopupVerticalAlign", PopupVerticalAlign.NotSet); }
			set { SetEnumProperty("PopupVerticalAlign", PopupVerticalAlign.NotSet, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupMenuPopupVerticalOffset"),
#endif
		DefaultValue(0), AutoFormatDisable]
		public int PopupVerticalOffset {
			get { return (int)GetIntProperty("PopupVerticalOffset", 0); }
			set { SetEnumProperty("PopupVerticalOffset", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupMenuTop"),
#endif
		DefaultValue(0), AutoFormatDisable]
		public int Top {
			get { return GetIntProperty("Top", 0); }
			set { SetIntProperty("Top", 0, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("The client-side API is always available for this control.")]
		public new bool EnableClientSideAPI {
			get { return base.EnableClientSideAPIInternal; }
			set { base.EnableClientSideAPIInternal = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		protected bool ShouldSerializeHeight() { return false; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		protected bool ShouldSerializeWidth() { return false; }
		public ASPxPopupMenu()
			: base() {
		}
		protected ASPxPopupMenu(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		protected void OnPopupElementResolve(ControlResolveEventArgs e) {
			EventHandler<ControlResolveEventArgs> handler = (EventHandler<ControlResolveEventArgs>)Events[EventPopupElementResolve];
			if(handler != null)
				handler(this, e);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPopupMenuPopupElementResolve"),
#endif
		Category("Events")]
		public event EventHandler<ControlResolveEventArgs> PopupElementResolve
		{
			add { Events.AddHandler(EventPopupElementResolve, value); }
			remove { Events.RemoveHandler(EventPopupElementResolve, value); }
		}
		protected override bool IsOneLevelMenu() {
			return false;
		}
		protected internal override bool IsMainMenu(MenuItem item) {
			return false;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxPopupMenu), PopupMenuScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (PopupElementID != "")
				stb.Append(localVarName + ".popupElementIDList=" + GetPopupElementIDList(PopupElementID) + ";\n");
			if(PopupAction != PopupAction.RightMouseClick)
				stb.Append(localVarName + ".popupAction='" + PopupAction.ToString() + "';\n");
			if(CloseAction != PopupMenuCloseAction.OuterMouseClick)
				stb.Append(localVarName + ".closeAction='" + CloseAction.ToString() + "';\n");
			if(PopupHorizontalAlign != PopupHorizontalAlign.NotSet)
				stb.Append(localVarName + ".popupHorizontalAlign='" + PopupHorizontalAlign.ToString() + "'\n");
			if(PopupVerticalAlign != PopupVerticalAlign.NotSet)
				stb.Append(localVarName + ".popupVerticalAlign='" + PopupVerticalAlign.ToString() + "';\n");
			if(PopupHorizontalOffset != 0)
				stb.Append(localVarName + ".popupHorizontalOffset=" + PopupHorizontalOffset.ToString() + "\n");
			if(PopupVerticalOffset != 0)
				stb.Append(localVarName + ".popupVerticalOffset=" + PopupVerticalOffset.ToString() + ";\n");
			if(Left != 0)
				stb.Append(localVarName + ".left=" + Left.ToString() + ";\n");
			if(Top != 0)
				stb.Append(localVarName + ".top=" + Top.ToString() + ";\n");
			if(PopupAlignCorrection != PopupAlignCorrection.Auto)
				stb.Append(localVarName + ".isPopupFullCorrectionOn=false;\n");
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientPopupMenu";
		}
		protected string GetPopupElementIDList(string ids) {
			ids = ids.Trim();
			if(string.IsNullOrEmpty(ids))
				return "[]";
			List<string> result = new List<string>();
			foreach(string id in ids.Split(';'))
				result.Add(RenderUtils.GetReferentControlClientID(this, id.Trim(), OnPopupElementResolve));
			return HtmlConvertor.ToJSON(result, true);
		}
	}
}
namespace DevExpress.Web.Internal {
	[ToolboxItem(false)]
	public class ASPxPopupMenuExt : ASPxPopupMenu {
		protected override string GetClientObjectClassName() {
			return "ASPxClientPopupMenuExt";
		}
		protected internal override MenuItem CreateSampleItems() {
			return CreateSampleItemsCore(new MenuItem(this));
		}
		protected internal override bool NeedCreateItemsOnClientSide() {
			return true;
		}
	}
}
