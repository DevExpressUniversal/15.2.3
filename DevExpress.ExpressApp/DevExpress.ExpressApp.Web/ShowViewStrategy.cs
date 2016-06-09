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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Configuration;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.SystemModule;
namespace DevExpress.ExpressApp.Web {
	public class ObjectKeyHelperWeb : ObjectKeyHelper {
		protected override string Encode(string str) { return HttpUtility.UrlEncode(str); }
		protected override string Decode(string str) { return HttpUtility.UrlDecode(str); }
	}
	public class UrlHelper {
		private static GlobalizationSection globalizationSection;
		static UrlHelper() {
			try {
				Configuration config = WebConfigurationManager.OpenWebConfiguration("~/");
				if(config != null) {
					globalizationSection = (GlobalizationSection)config.GetSection("system.web/globalization");
				}
			}
			catch { }
		}
		private static Encoding GetRequestEncoding() {
			Encoding result = Encoding.UTF8;
			if(globalizationSection != null) {
				result = globalizationSection.RequestEncoding;
			}
			return result;
		}
		private static Encoding GetResponseEncoding() {
			Encoding result = Encoding.UTF8;
			if(globalizationSection != null) {
				result = globalizationSection.ResponseEncoding;
			}
			return result;
		}
		public static string BuildQueryString(NameValueCollection parameters) {
			Encoding encoding = GetResponseEncoding();
			string result = "";
			foreach(string key in parameters.AllKeys) {
				result += string.Format("{0}={1}&", HttpUtility.UrlEncode(key, encoding), HttpUtility.UrlEncode(parameters[key], encoding));
			}
			return result.TrimEnd('&');
		}
		public static NameValueCollection ParseQueryString(string query) {
			return HttpUtility.ParseQueryString(query, GetRequestEncoding());
		}
		#region Obsolete 15.1
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public static string Decode(string queryParams) {
			return HttpUtility.UrlDecode(queryParams, GetRequestEncoding());
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public static ListDictionary Decode(NameValueCollection queryParams) {
			ListDictionary result = new ListDictionary();
			foreach(string key in queryParams.Keys) {
				result.Add(key, HttpUtility.UrlDecode(queryParams[key], GetRequestEncoding()));
			}
			return result;
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public static string BuildUrl(NameValueCollection parameters) {
			return BuildUrl(HttpContext.Current.Request.Path, parameters);
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public static string BuildUrl(string pageName, NameValueCollection parameters) {
			string queryString = BuildQueryString(parameters);
			if(string.IsNullOrEmpty(queryString)) {
				return pageName;
			}
			string separator = "#";
			if(queryString.Contains(separator)) {
				separator = "?";
			}
			return pageName + separator + queryString;
		}
		#endregion
	}
	public class ShowViewStrategy : ShowViewStrategyBase, ISupportCollectionsEditMode {
		private ViewEditMode collectionsEditMode;
		private bool IsDialogController(Controller controller) {
			return controller is DialogController;
		}
		private void ShowViewByDefault(ShowViewParameters parameters, ShowViewSource showViewSource) {
			if(showViewSource.SourceFrame is PopupWindow) {
				if((showViewSource.SourceView is DetailView) && (parameters.CreatedView is DetailView)
					&&
					(((DetailView)showViewSource.SourceView).ObjectTypeInfo.IsAssignableFrom(((DetailView)parameters.CreatedView).ObjectTypeInfo))) {
					showViewSource.SourceFrame.SetView(parameters.CreatedView);
				}
				else {
					ShowDialog(parameters, showViewSource);
				}
			}
			else {
				Application.MainWindow.SetView(parameters.CreatedView, showViewSource.SourceFrame);
			}
		}
		protected virtual Window ShowDialog(ShowViewParameters parameters, ShowViewSource showViewSource) {
			if(!parameters.Controllers.Exists(IsDialogController)) {
				DialogController dialogController = Application.CreateController<DialogController>();
				dialogController.SaveOnAccept = !IsSameObjectSpace(parameters, showViewSource);
				parameters.Controllers.Add(dialogController);
			}
			Window window = Application.CreatePopupWindow(TemplateContext.PopupWindow, parameters.CreatedView.Id, parameters.CreateAllControllers, parameters.Controllers.ToArray());
			window.SetView(parameters.CreatedView, showViewSource.SourceFrame);
			WebWindow currentWindow = GetCurrentWindow(showViewSource);
			if(currentWindow != null) {
#pragma warning disable 0618
				RegisterStartupWindowOpeningScript(parameters, window, currentWindow);
#pragma warning restore 0618
			}
			else {
				if(PopupWindowManager.ForceShowPopupWindowScriptValidation) {
					throw new InvalidOperationException();
				}
			}
			return window;
		}
		protected virtual WebWindow GetCurrentWindow(ShowViewSource showViewSource) {
			WebWindow currentWindow = showViewSource.SourceFrame as WebWindow;
			if(currentWindow == null) {
				currentWindow = WebWindow.CurrentRequestWindow;
			}
			return currentWindow;
		}
		protected override void ShowViewFromNestedView(ShowViewParameters parameters, ShowViewSource showViewSource) {
			ViewEditMode? collectionsEditMode = (parameters.CreatedView is DetailView) ? GetCollectionsEditMode((DetailView)parameters.CreatedView) : ((IModelOptionsWeb)Application.Model.Options).CollectionsEditMode;
			if(GetCurrentWindow(showViewSource) is PopupWindow || (collectionsEditMode == ViewEditMode.Edit)) {
				ShowDialog(parameters, showViewSource);
			}
			else {
				Application.MainWindow.SetView(parameters.CreatedView, showViewSource.SourceFrame);
			}
		}
		protected override void ShowViewInModalWindow(ShowViewParameters parameters, ShowViewSource showViewSource) {
			ShowDialog(parameters, showViewSource);
		}
		protected override Window ShowViewInNewWindow(ShowViewParameters parameters, ShowViewSource showViewSource) {
			return ShowDialog(parameters, showViewSource);
		}
		protected override void ShowViewInCurrentWindow(ShowViewParameters parameters, ShowViewSource showViewSource) {
			showViewSource.SourceFrame.SetView(parameters.CreatedView, showViewSource.SourceFrame);
		}
		protected override void ShowViewFromCommonView(ShowViewParameters parameters, ShowViewSource showViewSource) {
			ShowViewByDefault(parameters, showViewSource);
		}
		protected override void ShowViewFromLookupView(ShowViewParameters parameters, ShowViewSource showViewSource) {
			ShowDialog(parameters, showViewSource);
		}
		[Obsolete("In ASP.NET applications, use 'IModelOptionsWeb.CollectionsEditMode' instead. In WinForms applications, this method is not used anymore.")]
		public override bool CanUseNestedObjectSpace() {
			return base.CanUseNestedObjectSpace() && (collectionsEditMode == ViewEditMode.Edit);
		}
		protected new WebApplication Application {
			get { return (WebApplication)base.Application; }
		}
		public ShowViewStrategy(XafApplication application)
			: base(application) {
			collectionsEditMode = ViewEditMode.View;
		}
		[Obsolete("In ASP.NET applications, use 'IModelOptionsWeb.CollectionsEditMode' instead. In WinForms applications, this property is not used anymore.")]
		public ViewEditMode CollectionsEditMode {
			get { return collectionsEditMode; }
			set { collectionsEditMode = value; }
		}
		internal static ViewEditMode? GetCollectionsEditMode(DetailView detailView, XafApplication application = null) {
			if(detailView != null && detailView.Model != null && ((IModelDetailViewWeb)detailView.Model).CollectionsEditMode.HasValue) {
				return ((IModelDetailViewWeb)detailView.Model).CollectionsEditMode.Value;
			}
			application = application ?? WebApplication.Instance;
			IModelOptionsWeb modelOptionsWeb = (application != null && application.Model != null) ? application.Model.Options as IModelOptionsWeb : null;
			if(modelOptionsWeb != null) {
				return modelOptionsWeb.CollectionsEditMode;
			}
			return null;
		}
		internal static ViewEditMode? GetCollectionsEditMode(Frame frame, XafApplication application = null) {
			DetailView detailView = frame != null ? frame.View as DetailView : null;
			if(detailView == null && frame is NestedFrame && ((NestedFrame)frame).ViewItem != null) {
				detailView = ((NestedFrame)frame).ViewItem.View as DetailView;
			}
			return GetCollectionsEditMode(detailView, application);
		}
		#region Obsolete 15.1
		[Obsolete("Use the PopupWindwoManager.ShowPopup method instead"), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void RegisterStartupWindowOpeningScript(ShowViewParameters parameters, Window window, WebWindow currentWindow) {
			Application.PopupWindowManager.ShowPopup((WebWindow)window, currentWindow);
		}
		#endregion
	}
	#region Obsolete 15.1
	[Obsolete(ObsoleteMessages.TypeIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
	public interface ISupportStringSerialization {
		void SetValuesFromString(string str);
		string GetValuesAsString();
	}
	[Obsolete(ObsoleteMessages.TypeIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
	public class LogonViewCreatingEventArgs : EventArgs {
		public LogonViewCreatingEventArgs(DetailView detailView) {
			DetailView = detailView;
		}
		public DetailView DetailView { get; set; }
	}
	#endregion
}
