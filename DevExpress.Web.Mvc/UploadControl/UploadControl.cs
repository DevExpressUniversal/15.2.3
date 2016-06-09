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

using System.ComponentModel;
using System.Reflection;
using System.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc {
	using System.Collections.Specialized;
	using System.Web.Mvc;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	[ToolboxItem(false)]
	public class MVCxUploadControl: ASPxUploadControl {
		NameValueCollection postDataCollection;
		IValueProvider defaultValueProvider;
		public MVCxUploadControl()
			: this((ViewContext)null) {
		}
		protected internal MVCxUploadControl(ViewContext viewContext)
			: base() {
			ViewContext = viewContext ?? HtmlHelperExtension.ViewContext;
		}
		protected internal MVCxUploadControl(ASPxWebControl owner)
			: base(owner) {
		}
		public object CallbackRouteValues { get; set; }
		public new UploadControlStyles Styles {
			get { return base.Styles; }
		}
		protected internal new string[] ErrorTexts {
			get { return base.ErrorTexts; }
		}
		protected internal new bool ShowUI {
			get { return base.ShowUI; }
			set { base.ShowUI = value; }
		}
		protected internal IValueProvider DefaultValueProvider {
			get {
				if(defaultValueProvider == null)
					defaultValueProvider = ExtensionsHelper.CreateDefaultValueProvider();
				return defaultValueProvider;
			}
		}
		protected internal ViewContext ViewContext { get; private set; }
		protected internal ControllerBase Controller {
			get { return (ViewContext != null) ? ViewContext.Controller : null; }
		}
		protected internal IValueProvider ValueProvider {
			get { return Controller != null && Controller.ValueProvider != null ? Controller.ValueProvider : DefaultValueProvider; }
		}
		protected override NameValueCollection PostDataCollection {
			get {
				if(postDataCollection == null)
					postDataCollection = new MvcPostDataCollection(ValueProvider);
				return postDataCollection;
			}
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientUploadControl";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxUploadControl), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxUploadControl), Utils.UploadControlScriptResourceName);
		}
		public override bool IsLoading() {
			return false;
		}
		protected internal new void EnsureUploaded() {
			base.EnsureUploaded();
		}
		protected override bool IsUploadAllow() {
			if(IsHelperFileUploadingOnCallback()) {
				string key = Request[RenderUtils.ProgressHandlerKeyQueryParamName];
				return HelperUploadManager.FindUploadHelper(key) != null;
			}
			else
				return (Request.Files.Count > 0) && (Request.Files[GetUploadInputName(0)] != null);
		}
		static int assemblyMajorVersion = 0;
		protected internal override bool IsUploadProcessingEnabled() {
			if(base.IsUploadProcessingEnabled()) {
				if(assemblyMajorVersion == 0) {
					try {
						assemblyMajorVersion = GetAssemblyMajorVersion();
					}
					catch(System.Security.SecurityException) {
						assemblyMajorVersion = -1;
					}
				}
				return assemblyMajorVersion >= 4; 
			}
			return false;
		}
		int GetAssemblyMajorVersion() {
			return Assembly.GetAssembly(typeof(HttpWorkerRequest)).GetName().Version.Major;
		}
	}
}
