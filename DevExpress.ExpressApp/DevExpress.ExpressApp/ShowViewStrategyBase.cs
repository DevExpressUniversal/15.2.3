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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Localization;
using DevExpress.Utils;
namespace DevExpress.ExpressApp {
	public enum FrameContext { Common, Nested, Lookup }
	public enum TargetWindow {
		Default,
		Current,
		NewWindow,
		NewModalWindow,
	};
	public enum WindowStyle { Explorer, Inspector }
	public enum NewWindowTarget { Default, MdiChild, Separate }
	public enum UIType { MultipleWindowSDI, SingleWindowSDI, StandardMDI, TabbedMDI }
	public class ShowViewParameters {
		private const bool createAllControllersDefault = true;
		private const NewWindowTarget newWindowTargetDefault = NewWindowTarget.Default;
		private List<Controller> controllers = new List<Controller>();
		private View createdView;
		private TargetWindow targetWindow;
		private TemplateContext context = TemplateContext.Undefined;
		private bool createAllControllers = createAllControllersDefault;
		private NewWindowTarget newWindowTarget = newWindowTargetDefault;
		public ShowViewParameters() { }
		public ShowViewParameters(View view) {
			createdView = view;
		}
		public void Assign(ShowViewParameters source) {
			controllers = source.controllers;
			createdView = source.createdView;
			targetWindow = source.targetWindow;
			context = source.context;
			createAllControllers = source.createAllControllers;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ShowViewParametersCreatedView")]
#endif
		public View CreatedView {
			get { return createdView; }
			set { createdView = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ShowViewParametersTargetWindow")]
#endif
		public TargetWindow TargetWindow {
			get { return targetWindow; }
			set { targetWindow = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ShowViewParametersContext")]
#endif
		public TemplateContext Context {
			get { return context; }
			set { context = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ShowViewParametersControllers")]
#endif
		public List<Controller> Controllers {
			get { return controllers; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ShowViewParametersCreateAllControllers")]
#endif
		[DefaultValue(createAllControllersDefault)] 
		public bool CreateAllControllers {
			get { return createAllControllers; }
			set { createAllControllers = value; }
		}
		[DefaultValue(newWindowTargetDefault)] 
		public NewWindowTarget NewWindowTarget {
			get { return newWindowTarget; }
			set { newWindowTarget = value; }
		}
	}
	public class ShowViewSource {
		private View sourceView;
		private Frame sourceFrame;
		private ActionBase sourceAction;
		public ShowViewSource(Frame sourceFrame, ActionBase sourceAction) {
			this.sourceFrame = sourceFrame;
			if(sourceFrame != null) {
				this.sourceView = sourceFrame.View;
			}
			this.sourceAction = sourceAction;
		}
		public View SourceView {
			get {
				return sourceView;
			}
		}
		public Frame SourceFrame {
			get { return sourceFrame; }
		}
		public Controller SourceController {
			get { return (sourceAction != null) ? sourceAction.Controller : null; }
		}
		public ActionBase SourceAction {
			get { return sourceAction; }
		}
	}
	public abstract class ShowViewStrategyBase : IDisposable {
		public const string NewWindowTargetKey = "NewWindowTarget";
		private XafApplication application;
		protected bool IsSameObjectSpace(ShowViewParameters parameters, ShowViewSource showViewSource) {
			return (showViewSource.SourceView != null && parameters.CreatedView.ObjectSpace == showViewSource.SourceView.ObjectSpace);
		}
		protected FrameContext GetContext(ShowViewSource showViewSource) {
			FrameContext result = FrameContext.Common;
			Frame sourceFrame = showViewSource.SourceFrame;
			View sourceView = showViewSource.SourceView;
			if(sourceFrame != null && sourceView != null) {
				if(sourceFrame.Context == TemplateContext.LookupControl || sourceFrame.Context == TemplateContext.LookupWindow) {
					result = FrameContext.Lookup;
				}
				else if(!sourceView.IsRoot) {
					result = FrameContext.Nested;
				}
			}
			return result;
		}
		protected abstract void ShowViewInModalWindow(ShowViewParameters parameters, ShowViewSource sourceFrame);
		protected abstract Window ShowViewInNewWindow(ShowViewParameters parameters, ShowViewSource showViewSource);
		protected abstract void ShowViewInCurrentWindow(ShowViewParameters parameters, ShowViewSource showViewSource);
		protected abstract void ShowViewFromCommonView(ShowViewParameters parameters, ShowViewSource showViewSource);
		protected abstract void ShowViewFromNestedView(ShowViewParameters parameters, ShowViewSource showViewSource);
		protected abstract void ShowViewFromLookupView(ShowViewParameters parameters, ShowViewSource showViewSource);
		protected virtual void ShowViewCore(ShowViewParameters parameters, ShowViewSource showViewSource) {
			switch(parameters.TargetWindow) {
				case TargetWindow.NewModalWindow:
					ShowViewInModalWindow(parameters, showViewSource);
					break;
				case TargetWindow.NewWindow:
					ShowViewInNewWindow(parameters, showViewSource);
					break;
				case TargetWindow.Default:
					FrameContext frameContext = GetContext(showViewSource);
					switch(frameContext) {
						case FrameContext.Common:
							ShowViewFromCommonView(parameters, showViewSource);
							break;
						case FrameContext.Nested:
							ShowViewFromNestedView(parameters, showViewSource);
							break;
						case FrameContext.Lookup:
							ShowViewFromLookupView(parameters, showViewSource);
							break;
						default:
							throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotShowViewUnknownContext, frameContext));
					}
					break;
				case TargetWindow.Current:
					ShowViewInCurrentWindow(parameters, showViewSource);
					break;
				default:
					throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotShowViewUnknownTarget, parameters.TargetWindow));
			}
		}
		protected XafApplication Application { get { return application; } }
		public ShowViewStrategyBase(XafApplication application) {
			Guard.ArgumentNotNull(application, "application");
			this.application = application;
		}
		public virtual void Dispose() { }
		public void ShowView(ShowViewParameters parameters, ShowViewSource showViewSource) {
			if(parameters == null || parameters.CreatedView == null) {
				return;
			}
			if(IsSameObjectSpace(parameters, showViewSource)) {
				parameters.CreatedView.IsRoot = false;
			}
			ShowViewCore(parameters, showViewSource);
		}
		#region Obsolete 15.2
		[Obsolete("In ASP.NET applications, use 'IModelOptionsWeb.CollectionsEditMode' instead. In WinForms applications, this method is not used anymore."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanUseNestedObjectSpace() {
			return true;
		}
		#endregion
		public virtual bool SupportViewNavigationHistory {
			get { return true; }
		}
	}
	public interface ISupportCollectionsEditMode {
		DevExpress.ExpressApp.Editors.ViewEditMode CollectionsEditMode { get; set; }
	}
}
