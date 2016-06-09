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
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.SystemModule {
	public class WindowTemplateController : WindowController {
		private IWindowTemplate windowTemplate;
		private void Window_TemplateChanged(object sender, EventArgs e) {
			OnWindowTemplateChanged();
		}
		private void OnWindowTemplateChanged() {
			windowTemplate = Window.Template as IWindowTemplate;
			if(windowTemplate != null) {
				UpdateWindowStatusMessage();
				UpdateWindowCaption();
			}
		}
		private void Window_ViewChanged(object sender, ViewChangedEventArgs e) {
			AddViewListeners(Window.View);
			UpdateWindowCaption();
		}
		private void Window_ViewChanging(object sender, ViewChangingEventArgs e) {
			RemoveViewListeners(Window.View);
		}
		private void view_CaptionChanged(object sender, EventArgs e) {
			UpdateWindowCaption();
		}
		private void detailView_CurrentObjectChanged(object sender, EventArgs e) {
			UpdateWindowCaption();
		}
		private void editor_ValueStored(object sender, EventArgs e) {
			UpdateWindowCaption();
		}
		private void ObjectSpace_Committed(object sender, EventArgs e) {
			UpdateWindowCaption();
		}
		protected string GetCurrentObjectCaption() {
			return Window.View.GetCurrentObjectCaption();
		}
		protected virtual void AddViewListeners(View view) {
			if(view != null) {
				view.CaptionChanged += new EventHandler(view_CaptionChanged);
				DetailView detailView = view as DetailView;
				if(detailView != null) {
					if(detailView.ObjectSpace != null) {
						detailView.ObjectSpace.Committed += new EventHandler(ObjectSpace_Committed);
					}
					detailView.CurrentObjectChanged += new EventHandler(detailView_CurrentObjectChanged);
					foreach(PropertyEditor editor in detailView.GetItems<PropertyEditor>()) {
						editor.ValueStored += new EventHandler(editor_ValueStored);
					}
				}
			}
		}
		protected virtual void RemoveViewListeners(View view) {
			if(view != null) {
				view.CaptionChanged -= new EventHandler(view_CaptionChanged);
				DetailView detailView = view as DetailView;
				if(detailView != null) {
					if(detailView.ObjectSpace != null) {
						detailView.ObjectSpace.Committed -= new EventHandler(ObjectSpace_Committed);
					}
					detailView.CurrentObjectChanged -= new EventHandler(detailView_CurrentObjectChanged);
					foreach(PropertyEditor editor in detailView.GetItems<PropertyEditor>()) {
						editor.ValueStored -= new EventHandler(editor_ValueStored);
					}
				}
			}
		}
		protected virtual SplitString GetWindowCaption() {
			return GetWindowCaption(null);
		}
		protected virtual SplitString GetWindowCaption(string formText) {
			SplitString result = new SplitString();
			if(Window.View == null) {
				result.FirstPart = Application.Title;
			}
			else {
				if(Window.View is DetailView) {
					result.FirstPart = GetCurrentObjectCaption();
				}
				if(Window.IsMain) {
					if(string.IsNullOrEmpty(result.FirstPart)) {
						result.FirstPart = Window.View.Caption;
					}
					result.SecondPart = Application.Title;
				}
				else {
					result.SecondPart = Window.View.Caption;
				}
			}
			return result;
		}
		protected virtual ICollection<string> GetWindowStatusMessages() {
			List<string> messages = new List<string>();
			if(!string.IsNullOrEmpty(SecuritySystem.CurrentUserName)) {
				messages.Add(string.Format(CaptionHelper.GetLocalizedText("Messages", "User"), SecuritySystem.CurrentUserName));
			}
			return messages;
		}
		protected override void OnActivated() {
			base.OnActivated();
			Window.TemplateChanged += new EventHandler(Window_TemplateChanged);
			Window.ViewChanging += new EventHandler<ViewChangingEventArgs>(Window_ViewChanging);
			Window.ViewChanged += new EventHandler<ViewChangedEventArgs>(Window_ViewChanged);
			OnWindowTemplateChanged();
			AddViewListeners(Window.View);
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			RemoveViewListeners(Window.View);
			Window.TemplateChanged -= new EventHandler(Window_TemplateChanged);
			Window.ViewChanging -= new EventHandler<ViewChangingEventArgs>(Window_ViewChanging);
			Window.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Window_ViewChanged);
			windowTemplate = null;
		}
		protected IWindowTemplate WindowTemplate {
			get { return windowTemplate; }
		}
		public void UpdateWindowStatusMessage() {
			if(windowTemplate != null) {
				CustomizeWindowStatusMessagesEventArgs args = new CustomizeWindowStatusMessagesEventArgs(GetWindowStatusMessages());
				if(CustomizeWindowStatusMessages != null) {
					CustomizeWindowStatusMessages(this, args);
				}
				windowTemplate.SetStatus(args.StatusMessages);
				if(CustomizeStatusBar != null) {
					CustomizeStatusBar(this, EventArgs.Empty);
				}
			}
		}
		public void UpdateWindowCaption(string formText) {
			if(windowTemplate != null) {
				CustomizeWindowCaptionEventArgs args = new CustomizeWindowCaptionEventArgs(GetWindowCaption(formText));
				if(CustomizeWindowCaption != null) {
					CustomizeWindowCaption(this, args);
				}
				windowTemplate.SetCaption(args.WindowCaption.Text);
			}
		}
		public void UpdateWindowCaption() {
			UpdateWindowCaption(null);
		}
		public event EventHandler<CustomizeWindowCaptionEventArgs> CustomizeWindowCaption;
		public event EventHandler<CustomizeWindowStatusMessagesEventArgs> CustomizeWindowStatusMessages;
		public event EventHandler<EventArgs> CustomizeStatusBar;
	}
	public class CustomizeWindowCaptionEventArgs : EventArgs {
		private SplitString windowCaption;
		public CustomizeWindowCaptionEventArgs(SplitString windowCaption) {
			this.windowCaption = windowCaption;
		}
		public SplitString WindowCaption {
			get { return windowCaption; }
		}
	}
	public class CustomizeWindowStatusMessagesEventArgs : EventArgs {
		private ICollection<string> statusMessages;
		public CustomizeWindowStatusMessagesEventArgs(ICollection<string> statusMessages) {
			this.statusMessages = statusMessages;
		}
		public ICollection<string> StatusMessages {
			get { return statusMessages; }
		}
	}
}
