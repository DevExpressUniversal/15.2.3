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
using System.ComponentModel;
using System.Text;
using System.Web;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Web {
	public interface ISupportModelSaving {
		event EventHandler<EventArgs> ModelSaving;
	}	
	public class CookiesModelDifferenceStore : ModelDifferenceStore {
		private TimeSpan aliveInterval = new TimeSpan(30, 0, 0, 0);
		private void LoadCookie(ModelApplicationBase model, string aspect) {
			HttpCookie cookie = HttpContext.Current.Request.Cookies[GetCookieName(aspect)];
			if(cookie != null) {
				string data = HttpUtility.UrlDecode(cookie.Value);
				if(!String.IsNullOrEmpty(data)) {
					try {
						new ModelXmlReader().ReadFromString(model, aspect, data);
					}
					catch {
						Tracing.Tracer.LogWarning("Can't load the user model differences from the cookies. The actual value is '{0}'", data);
					}
				}
			}
		}
		private void SaveDifference(ModelApplicationBase model, string aspect) {
			string cookieName = GetCookieName(aspect);
			HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
			if(cookie == null) {
				cookie = new HttpCookie(cookieName);
			}
			cookie.Expires = DateTime.Now.Add(aliveInterval);
			string value = SerializeModelForCookies(model, aspect);
			cookie.Value = HttpUtility.UrlEncode(value);
			HttpContext.Current.Response.Cookies.Set(cookie);
		}
		private static string GetCookieName(string aspect) {
			string cookieName = UserDiffDefaultName;
			if(!String.IsNullOrEmpty(aspect)) {
				cookieName += "_" + aspect;
			}
			return cookieName;
		}
		public override void Load(ModelApplicationBase model) {
			Guard.ArgumentNotNull(model, "model");
			if(HttpContext.Current != null) {
				LoadCookie(model, String.Empty);
				foreach(string aspect in model.GetAspectNames()) {
					LoadCookie(model, aspect);
				}
			}
		}
		private string SerializeModelForCookies(ModelApplicationBase model, string aspect) {
			StringBuilder buffer = new StringBuilder();
			ModelXmlWriter writer = new ModelXmlWriter();
			ModelNode views = model.GetNodeInThisLayer("Views");
			if(views != null) {
				for(int i = 0; i < views.NodeCountInThisLayer; ++i) {
					ModelNode view = views.GetNodeInThisLayer(i);
					IModelListViewStateStore modelListView = view as IModelListViewStateStore;
					if(modelListView != null && modelListView.SaveStateInCookies) {
						string listViewXml = writer.WriteToString((ModelNode)modelListView, model.GetAspectIndex(aspect));
						if(!string.IsNullOrEmpty(listViewXml)) {
							buffer.AppendLine(listViewXml);
						}
					}
				}
			}
			if(buffer.Length == 0) {
				buffer.Append("<Application />");
			}
			else {
				buffer.Insert(0, Environment.NewLine).Insert(0, "<Application><Views>").Append("</Views></Application>");
			}
			return buffer.ToString();
		}
		public override void SaveDifference(ModelApplicationBase model) {
			Guard.ArgumentNotNull(model, "model");
			bool saveListViewStateInCookies = ((IModelOptionsStateStore)WebApplication.Instance.Model.Options).SaveListViewStateInCookies;
			if(saveListViewStateInCookies) {
				SaveDifference(model, String.Empty);
				foreach(string aspect in model.GetAspectNames()) {
					SaveDifference(model, aspect);
				}
			}
		}
		public override string Name { get { return GetType().Name; } }
		public override bool ReadOnly { get { return false; } }
		public TimeSpan AliveInterval {
			get { return aliveInterval; }
			set { aliveInterval = value; }
		}
	}
	public class SessionModelDifferenceStore : ModelDifferenceStore {
		private const string UserDiffKey = ModelDifferenceStore.UserDiffDefaultName;
		private ModelDifferenceStore underlyingDifferenceStore;
		public SessionModelDifferenceStore(ModelDifferenceStore underlyingDifferenceStore) {
			Guard.ArgumentNotNull(underlyingDifferenceStore, "underlyingDifferenceStore");
			this.underlyingDifferenceStore = underlyingDifferenceStore;
		}
		public SessionModelDifferenceStore() : this(new CookiesModelDifferenceStore()) { }
		public override string Name { get { return GetType().Name; } }
		public override void Load(ModelApplicationBase model) {
			Guard.ArgumentNotNull(model, "model");
			if(HttpContext.Current != null && HttpContext.Current.Session[UserDiffKey] != null) {
				string data = HttpContext.Current.Session[UserDiffKey] as string;
				if(!string.IsNullOrEmpty(data)) {
					try {
						new ModelXmlReader().ReadFromString(model, model.CurrentAspect, data);
					}
					catch {
					}
				}
			}
			else {
				underlyingDifferenceStore.Load(model);
			}
		}
		public override void SaveDifference(ModelApplicationBase model) {
			Guard.ArgumentNotNull(model, "model");
			string data = new ModelXmlWriter().WriteToString(model, model.GetCurrentAspectIndex());
			if(HttpContext.Current != null) {
				HttpContext.Current.Session[UserDiffKey] = data;
			}
		}
		public void SaveDifferenceToUnderlyingStore(ModelApplicationBase model) {
			Guard.ArgumentNotNull(model, "model");
			if(HttpContext.Current != null) {
				underlyingDifferenceStore.SaveDifference(model);
			}
		}
	}
	public class SessionDictionaryDifferenceStoreWindowController : WindowController {
		SessionModelDifferenceStore diffsStore;
		private void window_PagePreRender(object sender, EventArgs e) {
			diffsStore.SaveDifferenceToUnderlyingStore(((ModelApplicationBase)Application.Model).LastLayer);
		}
		protected override void OnActivated() {
			base.OnActivated();
			diffsStore = ((WebApplication)Application).UserDiffsStore as SessionModelDifferenceStore;
			Active["UserDiffsStore is SessionModelDifferenceStore"] = diffsStore != null;
			if(Active) {
				WebWindow window = Frame as WebWindow;
				if(window != null) {
					window.PagePreRender += new EventHandler(window_PagePreRender);
				}
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			WebWindow window = Frame as WebWindow;
			if(window != null) {
				window.PagePreRender -= new EventHandler(window_PagePreRender);
			}
		}
	}
	public class WebListEditorSettingsStoreViewController : ViewController, IModelExtender {
		SessionModelDifferenceStore diffsStore;
		private void SaveModelChanges() {
			View.SaveModel();
			Application.SaveModelChanges();			
		}
		private void Frame_ViewChanged(object sender, ViewChangedEventArgs e) {
			SaveModelChanges();
		}		
		private void supportModelSaving_ModelSaving(object sender, EventArgs e) {
			SaveModelChanges();
			if(diffsStore != null) {
				diffsStore.SaveDifferenceToUnderlyingStore(((ModelApplicationBase)Application.Model).LastLayer);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			diffsStore = ((WebApplication)Application).UserDiffsStore as SessionModelDifferenceStore;
			if(!ASPxGridListEditor.UseASPxGridViewDataSpecificColumns) { 
				Frame.ViewChanged += new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
			}
			ISupportModelSaving supportModelSaving = ((ListView)View).Editor as ISupportModelSaving;
			if(supportModelSaving != null) {
				supportModelSaving.ModelSaving += new EventHandler<EventArgs>(supportModelSaving_ModelSaving);
			}			
		}		
		protected override void OnDeactivated() {
			Frame.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
			ISupportModelSaving supportModelSaving = ((ListView)View).Editor as ISupportModelSaving;
			if(supportModelSaving != null) {
				supportModelSaving.ModelSaving -= new EventHandler<EventArgs>(supportModelSaving_ModelSaving);
			}
			diffsStore = null;
			base.OnDeactivated();
		}
		public WebListEditorSettingsStoreViewController() {
			TargetViewType = ViewType.ListView;
		}
		#region IExtendModel Members
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelOptions, IModelOptionsStateStore>();
			extenders.Add<IModelListView, IModelListViewStateStore>();
		}
		#endregion
	}
	public interface IModelOptionsStateStore {
		[
#if !SL
	DevExpressExpressAppWebLocalizedDescription("IModelOptionsStateStoreSaveListViewStateInCookies"),
#endif
 DefaultValue(false)]
		bool SaveListViewStateInCookies { get; set; }
	}
	[DomainLogic(typeof(IModelListViewStateStore))]
	public static class ModelListViewStateStoreDomainLogic {
		public static bool Get_SaveStateInCookies(IModelListView modelListView) {
			if(modelListView.Application != null) {
				return ((IModelOptionsStateStore)modelListView.Application.Options).SaveListViewStateInCookies;
			}
			return true;
		}
	}
	public interface IModelListViewStateStore {
#if !SL
	[DevExpressExpressAppWebLocalizedDescription("IModelListViewStateStoreSaveStateInCookies")]
#endif
		bool SaveStateInCookies { get; set; }
	}
}
