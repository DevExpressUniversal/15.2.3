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
using System.ComponentModel;
using System.Web.UI;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Web.Templates {
	public class TemplateContentFactory {
		public static TemplateContentFactory Instance { get; set; }
		static TemplateContentFactory() {
			Instance = new TemplateContentFactory();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		internal bool NewStyle {
			get {
				IValueManager<bool?> manager = ValueManager.GetValueManager<bool?>("TemplateContentFactoryNewStyle");
				return manager.Value.HasValue ? manager.Value.Value : false;
			}
			set {
				IValueManager<bool?> manager = ValueManager.GetValueManager<bool?>("TemplateContentFactoryNewStyle");
				manager.Value = value;
			}
		}
		private TemplateContent CreateDefaultNestedFrameControl() {
			TemplateContent nestedFrameTemplateControl;
			if(NewStyle) {
				nestedFrameTemplateControl = new NestedFrameControlNew();
			}
			else {
				nestedFrameTemplateControl = new NestedFrameControl();
			}
			return nestedFrameTemplateControl;
		}
		protected virtual TemplateContent CreateDefaultLogonTemplateContent() {
			if(NewStyle) {
				return new LogonTemplateContentNew();
			}
			else {
				return new LogonTemplateContent();
			}
		}
		protected virtual TemplateContent CreateDefaultFindDialogTemplateContent() {
			if(NewStyle) {
				return new FindDialogTemplateContentNew();
			}
			else {
				return new DialogTemplateContent();
			}
		}
		protected virtual TemplateContent CreateDefaultDialogTemplateContent() {
			if(NewStyle) {
				return new DialogTemplateContentNew();
			}
			else {
				return new DialogTemplateContent();
			}
		}
		protected virtual TemplateContent CreateDefaultHorizontalTemplateContent() {
			return new DefaultTemplateContent();
		}
		protected virtual TemplateContent CreateDefaultVerticalTemplateContent() {
			if(NewStyle) {
				return new DefaultVerticalTemplateContentNew();
			}
			else {
				return new DefaultVerticalTemplateContent();
			}
		}
		protected virtual Control LoadControlFromVirtualPath(Page page, string path) {
			return page.LoadControl(path);
		}
		public TemplateContent CreateTemplateContent(Page page, TemplateContentSettings settings, TemplateType templateType) {
			Control templateContent = null;
			switch(templateType) {
				case TemplateType.Logon: {
						bool useStandardTemplateContent = string.IsNullOrEmpty(settings.LogonTemplateContentPath);
						if(useStandardTemplateContent) {
							templateContent = CreateDefaultLogonTemplateContent();
						}
						else {
							templateContent = LoadControlFromVirtualPath(page, settings.LogonTemplateContentPath);
						}
						break;
					}
				case TemplateType.Dialog: {
						bool useStandardTemplateContent = string.IsNullOrEmpty(settings.DialogTemplateContentPath);
						if(useStandardTemplateContent) {
							templateContent = CreateDefaultDialogTemplateContent();
						}
						else {
							templateContent = LoadControlFromVirtualPath(page, settings.DialogTemplateContentPath);
						}
						break;
					}
				case TemplateType.FindDialog: {
						bool useStandardTemplateContent = string.IsNullOrEmpty(settings.FindDialogTemplateContentPath);
						if(useStandardTemplateContent) {
							templateContent = CreateDefaultFindDialogTemplateContent();
						}
						else {
							templateContent = LoadControlFromVirtualPath(page, settings.FindDialogTemplateContentPath);
						}
						break;
					}
				case TemplateType.Horizontal: {
						bool useStandardTemplateContent = string.IsNullOrEmpty(settings.DefaultTemplateContentPath);
						if(useStandardTemplateContent) {
							templateContent = CreateDefaultHorizontalTemplateContent();
						}
						else {
							templateContent = LoadControlFromVirtualPath(page, settings.DefaultTemplateContentPath);
						}
						break;
					}
				case TemplateType.Vertical: {
						bool useStandardTemplateContent = string.IsNullOrEmpty(settings.DefaultVerticalTemplateContentPath);
						if(useStandardTemplateContent) {
							templateContent = CreateDefaultVerticalTemplateContent();
						}
						else {
							templateContent = LoadControlFromVirtualPath(page, settings.DefaultVerticalTemplateContentPath);
						}
						break;
					}
				case TemplateType.NestedFrameControl: {
						bool useStandardTemplate = string.IsNullOrEmpty(settings.NestedFrameControlPath);
						if(useStandardTemplate) {
							templateContent = CreateDefaultNestedFrameControl();
						}
						else {
							templateContent = LoadControlFromVirtualPath(page, settings.NestedFrameControlPath);
						}
						break;
					}
			}
			if(!typeof(TemplateContent).IsAssignableFrom(templateContent.GetType())) {
				throw new WarningException(string.Format("The {0} type should be descendant of the {1} type.", templateContent.GetType().FullName, typeof(TemplateContent).FullName));
			}
			return (TemplateContent)templateContent;
		}
		#region Obsolete 15.1
		[Obsolete("Use the CreateTemplateContent method instead")]
		public virtual TemplateContent CreateDefaultNestedTemplateContent() {
			if(WebWindow.CurrentRequestPage as BaseXafPage == null) {
				throw new Exception(string.Format(
					"Cannot load the '{0}' control because the CurrentRequestPage is null or not an instance of the BaseXafPageClass",
					WebApplication.Instance.Settings.NestedFrameControlPath));
			}
			return CreateTemplateContent(WebWindow.CurrentRequestPage, WebApplication.Instance.Settings, TemplateType.NestedFrameControl);
		}
		#endregion
#if DebugTest
		public bool DebugTest_NewStyle {
			get {
				return NewStyle;
			}
			set {
				NewStyle = value;
			}
		}
#endif
	}
	public abstract class TemplateContent : UserControl, IViewSiteTemplate, ISupportUpdate {
		public abstract void SetStatus(ICollection<string> statusMessages);
		public abstract IActionContainer DefaultContainer { get; }
		public abstract object ViewSiteControl { get; }
		public virtual void BeginUpdate() { }
		public virtual void EndUpdate() { }
	}
	public enum TemplateType { Horizontal, Vertical, Logon, Dialog, FindDialog, NestedFrameControl }
}
