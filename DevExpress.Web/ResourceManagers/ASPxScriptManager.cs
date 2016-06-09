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

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public enum ControlType {
		None,
		ASPxCallback, ASPxCallbackPanel, ASPxCloudControl, ASPxDataView, ASPxDocking, ASPxFileManager, ASPxFormLayout, ASPxGlobalEvents, ASPxHiddenField, ASPxImageGallery, ASPxImageSlider, ASPxImageZoom, ASPxImageZoomNavigator,
		ASPxLoadingPanel, ASPxMenu, ASPxNavBar, ASPxNewsControl, ASPxPager, ASPxPanel, ASPxPopupControl, ASPxPopupMenu, ASPxRatingControl, ASPxRoundPanel, ASPxSplitter, ASPxTabControl,
		ASPxTimer, ASPxTitleIndex, ASPxTreeView, ASPxUploadControl,
		ASPxBinaryImage, ASPxButton, ASPxButtonEdit, ASPxCalendar, ASPxCaptcha, ASPxCheckBox, ASPxCheckBoxList, ASPxColorEdit, ASPxComboBox, ASPxDateEdit, ASPxDropDownEdit, ASPxHyperLink,
		ASPxImage, ASPxLabel, ASPxListBox, ASPxMemo, ASPxProgressBar, ASPxRadioButton, ASPxRadioButtonList, ASPxRibbon, ASPxSpinEdit, ASPxTextBox, ASPxTimeEdit, ASPxTokenBox, ASPxTrackBar, ASPxValidationSummary
	}
	[ToolboxItem(false)
]
	public class ASPxScriptManager : ASPxResourceManagerBase {
		const string ScriptManagerContextKey = "DXScriptManager";
		static bool active;
		public ASPxScriptManager()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxScriptManagerItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public ScriptCollection Items {
			get { return ResourceItems as ScriptCollection; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxScriptManagerActive")]
#endif
		public static bool Active {
			get {
				return HttpContext.Current != null ? GetActiveValueFromContext(ScriptManagerContextKey) : active;
			}
		}
		protected override ResourceItemCollection CreateItemCollection(ASPxResourceManagerBase manager) {
			return new ScriptCollection(manager as ASPxScriptManager);
		}
		protected override void SetActiveValue(bool value) {
			base.SetActiveValue(value);
			if(Active != value)
				active = value;
		}
		protected override void RenderInternal(HtmlTextWriter writer) {
			if(!DesignMode)
				writer.Write(RenderScriptTags());
		}
		protected internal string RenderScriptTags() {
			foreach(ResourceScript item in Items) {
				IList<ASPxWebControl> controls = null;
				if(item.Control != ControlType.None)
					controls = RegistrationControlsFactory.GetControlsForScriptRegistration(item.Control);
				else {
					if(item.Suite != Suite.None)
						controls = RegistrationControlsFactory.GetControlsForScriptRegistration(item.Suite);
				}
				if(controls != null) {
					foreach(ASPxWebControl control in controls)
						RegisterScripts(control);
				}
			}
			return RenderScriptResources();
		}
		protected string RenderScriptResources() {
			var sb = new StringBuilder();
			var sw = new StringWriter(sb);
			ResourceManager.RenderScriptResources(Page, new HtmlTextWriter(sw));
			ResourceManager.RenderScriptBlocks(Page, new HtmlTextWriter(sw));
			return sb.ToString();
		}
		protected void RegisterScripts(Control control) {
			ASPxWebControl aspxWebControl = control as ASPxWebControl;
			if(aspxWebControl != null) {
				aspxWebControl.RegisterClientIncludeScripts();
				aspxWebControl.RegisterClientScriptBlocks();
			}
			foreach(Control child in control.Controls)
				RegisterScripts(child);
		}
	}
}
