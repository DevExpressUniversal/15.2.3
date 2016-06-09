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
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Templates {
	public enum ActionContainersChangedType {
		Added,
		Removed
	}
	public enum ActionsToolbarVisibility { Default, Show, Hide }
	public class ActionContainersChangedEventArgs : EventArgs {
		private IEnumerable<IActionContainer> actionContainers;
		private ActionContainersChangedType changedType;
		public ActionContainersChangedEventArgs(IEnumerable<IActionContainer> actionContainers, ActionContainersChangedType changedType) {
			this.actionContainers = actionContainers;
			this.changedType = changedType;
		}
		public IEnumerable<IActionContainer> ActionContainers {
			get { return actionContainers; }
		}
		public ActionContainersChangedType ChangedType {
			get { return changedType; }
		}
	}
	public interface IDynamicContainersTemplate : IFrameTemplate {
		void RegisterActionContainers(IEnumerable<IActionContainer> actionContainers);
		void UnregisterActionContainers(IEnumerable<IActionContainer> actionContainers);
		event EventHandler<ActionContainersChangedEventArgs> ActionContainersChanged;
	}
	public interface IFrameTemplate {
		ICollection<IActionContainer> GetContainers(); 
		IActionContainer DefaultContainer { get; }
		void SetView(View view);
	}
	public interface ISupportActionsToolbarVisibility {
		void SetVisible(bool isVisible);		
	}
	public interface IViewSiteTemplate {
		object ViewSiteControl { get;}
	}
	public interface ISupportStoreSettings {
		void SetSettings(IModelTemplate settings);
		void ReloadSettings();
		void SaveSettings();
		event EventHandler SettingsReloaded;
	} 
	public interface IWindowTemplate : IFrameTemplate {
		void SetStatus(ICollection<string> statusMessages);
		void SetCaption(string caption);
		bool IsSizeable { get; set; }
	}
	public interface ISupportViewChanged {
		event EventHandler<TemplateViewChangedEventArgs> ViewChanged;
	}
	public interface IViewHolder : ISupportViewChanged {
		View View { get; }
	}
	public class TemplateViewChangedEventArgs : EventArgs {
		private View view;
		public TemplateViewChangedEventArgs(View view) {
			this.view = view;
		}
		public View View {
			get { return view; }
			set { view = value; }
		}
	}
}
