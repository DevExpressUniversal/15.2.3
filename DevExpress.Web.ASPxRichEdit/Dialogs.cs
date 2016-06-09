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
using System.Web.UI;
using DevExpress.Web.Internal;
using DevExpress.Web;
using System.IO;
using DevExpress.Office.Localization;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	[ToolboxItem(false)]
	public abstract class RichEditDialogUserControl : UserControl, IDialogFormElementRequiresLoad {
		private ASPxRichEdit richEdit = null;
		private string unitFormatString = null;
		protected ASPxRichEdit RichEdit {
			get {
				if(richEdit == null)
					richEdit = FindParentRichEdit();
				return richEdit;
			}
		}
		protected string UnitFormatString {
			get {
				if(unitFormatString == null)
					unitFormatString = GetUnitFormatString();
				return unitFormatString;
			}
		}
		public override string ClientID {
			get { return ClientIDHelper.GenerateClientID(this, base.ClientID); }
		}
		protected override void OnInit(EventArgs e) {
			ClientIDHelper.UpdateClientIDMode(this);
			base.OnInit(e);
		}
		protected override void OnLoad(System.EventArgs e) {
			base.OnLoad(e);
			PrepareChildControls();
		}
		protected virtual ASPxButton[] GetChildDxButtons() {
			return new ASPxButton[] { };
		}
		protected virtual ASPxEditBase[] GetChildDxEdits() {
			return new ASPxEditBase[] { };
		}
		protected virtual DevExpress.Web.ASPxRoundPanel GetChildDxRichEditPanel() {
			return null;
		}
		protected virtual DevExpress.Web.ASPxRoundPanel[] GetChildDxRichEditPanels() {
			List<ASPxRoundPanel> roundPanels = new List<ASPxRoundPanel>();
			ASPxRoundPanel firstPanel = GetChildDxRichEditPanel();
			if(firstPanel != null)
				roundPanels.Add(firstPanel);
			return roundPanels.ToArray();
		}
		private ASPxRichEdit FindParentRichEdit() {
			Control curControl = Parent;
			while(curControl != null) {
				if(curControl is ASPxRichEdit)
					return curControl as ASPxRichEdit;
				curControl = curControl.Parent;
			}
			return null;
		}
		protected virtual string GetUnitFormatString() {
			string unitAbbr = this.RichEdit.Settings.Unit == RichEditUnit.Inch ? OfficeLocalizer.GetString(OfficeStringId.UnitAbbreviation_Inch) : OfficeLocalizer.GetString(OfficeStringId.UnitAbbreviation_Centimeter);
			return "{0}" + unitAbbr;
		}
		protected virtual void PrepareChildControls() {
			EnsureChildControls();
		}
		protected void AddTemplateToControl(Control destinationContainer, ITemplate template) {
			if(template == null || destinationContainer == null)
				return;
			template.InstantiateIn(destinationContainer);
		}
		void IDialogFormElementRequiresLoad.ForceInit() {
			FrameworkInitialize();
		}
		void IDialogFormElementRequiresLoad.ForceLoad() {
			OnLoad(EventArgs.Empty);
		}
	}
}
namespace DevExpress.Web.ASPxRichEdit {
	[ToolboxItem(false)]
	public class RichEditFileManager : ASPxFileManager {
		protected internal new const string ScriptName = ASPxRichEdit.FileManagerScriptResourceName;
		protected internal bool IsRichEditCallback { get; set; }
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(RichEditFileManager), ScriptName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientRichEditFileManager";
		}
		protected internal object GetCallbackResult(string callbackArgs) {
			RaisePostBackEventCore(callbackArgs);
			return GetCallbackResult();
		}
		protected override bool IsNeedResetToInitalFolder() {
			return false;
		}
		protected override bool IsNeedToAddCallbackCommandResult() {
			return base.IsNeedToAddCallbackCommandResult() || IsRichEditCallback;
		}
	}
	[ToolboxItem(false)]
	public class RichEditFolderManager : ASPxFileManager {
		protected internal new const string ScriptName = ASPxRichEdit.FolderManagerScriptResourceName;
		protected internal bool IsRichEditCallback { get; set; }
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(RichEditFolderManager), ScriptName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientRichEditFolderManager";
		}
		protected internal object GetCallbackResult(string callbackArgs) {
			RaisePostBackEventCore(callbackArgs);
			return GetCallbackResult();
		}
		protected override bool IsNeedResetToInitalFolder() {
			return false;
		}
		protected override bool IsNeedToAddCallbackCommandResult() {
			return base.IsNeedToAddCallbackCommandResult() || IsRichEditCallback;
		}
	}
	[ToolboxItem(false)]
	public sealed class RichEditUploadControl : DevExpress.Web.ASPxUploadControl {
		public RichEditUploadControl()
			: base() {
				this.OwnerControl = FindParentRichEditControl();
		}
		protected override void OnInit(System.EventArgs e) {
			ASPxRichEdit editor = FindParentRichEditControl();
			if(editor != null)
				ValidationSettings.AllowedFileExtensions = new string[] { ".doc", ".docx", ".epub", "html", "htm", ".mht", ".mhtml", ".odt", ".txt", ".rtf", "xml" };
			base.OnInit(e);
		}
		private ASPxRichEdit FindParentRichEditControl() {
			Control curControl = Parent;
			while(curControl != null) {
				if(curControl is ASPxRichEdit)
					return curControl as ASPxRichEdit;
				curControl = curControl.Parent;
			}
			return null;
		}
	}
}
