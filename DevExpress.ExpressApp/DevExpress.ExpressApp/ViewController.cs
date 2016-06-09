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
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp {
	public enum Nesting { Any, Root, Nested }
	public enum ViewType { Any, DetailView, ListView, DashboardView }
	public class ViewControllerViewChangingEventArgs : EventArgs {
		private bool cancel;
		private View view;
		public ViewControllerViewChangingEventArgs(View view) {
			this.view = view;
		}
		public bool Cancel {
			get { return cancel; }
			set { cancel = value; }
		}
		public View View {
			get { return view; }
		}
	}
	[ToolboxItem(false)]
	public abstract class ViewController<ViewType> : ViewController where ViewType : View {
		public ViewController()
			: base() {
			this.TypeOfView = typeof(ViewType);
		}
		public new ViewType View {
			get { return base.View as ViewType; }
		}
	}
	[ToolboxItem(false)]
	public abstract class ObjectViewController<ViewType, ObjectType> : ViewController<ViewType> where ViewType : ObjectView {
		public ObjectViewController()
			: base() {
			this.TargetObjectType = typeof(ObjectType);
		}
		public ObjectType ViewCurrentObject {
			get { return (ObjectType)View.CurrentObject; }
		}
		public new ViewType View {
			get { return base.View as ViewType; }
		}
	}
	[ToolboxItem(false)]
	public class ViewController : Controller {
		public const string ViewIsAssignedReason = "View is assigned";
		private Nesting nesting;
		private Type objectType;
		private Type typeOfView;
		private View view;
		private string targetViewID;
		private static Boolean IsFitToNesting(View view, Nesting nesting, BoolList list) {
			bool result = true;
			if(nesting != Nesting.Any) {
				result =
						((nesting == Nesting.Nested) && !view.IsRoot)
						|| ((nesting == Nesting.Root) && view.IsRoot);
				list["View nesting is " + nesting] = result;
			}
			return result;
		}
		private static Boolean IsFitToObjectType(View view, Type objectType, BoolList list) {
			Boolean result = false;
			ObjectView objectView = view as ObjectView;
			if(objectType != null) {
				if(objectView != null) {
					if(objectView.ObjectTypeInfo != null) {
						if(objectType != null) {
							result = objectType.IsAssignableFrom(objectView.ObjectTypeInfo.Type);
							list["Object type is " + objectType.FullName] = result;
						}
					}
					else {
						if(objectType != null) {
							result = false;
							list["Object type is " + objectType.FullName] = result;
						}
					}
				}
				else {
					if(objectType != null) {
						result = false;
						list["Object type is " + objectType.FullName] = result;
					}
				}
			}
			else {
				result = true;
			}
			return result;
		}
		private static Boolean IsFitToViewType(View view, Type typeOfView, BoolList list) {
			bool result = true;
			if (typeOfView != null) {
				result = typeOfView.IsAssignableFrom(view.GetType());
				list["View type is " + typeOfView.Name] = result;
			}
			return result;
		}
		private static Boolean IsFitToViewID(View view, String viewID, BoolList list) {
			bool result = true;
			if(!String.IsNullOrEmpty(viewID) && (viewID != ActionBase.AnyCaption)) {
				result = (viewID == view.Id);
				if(!result && viewID.Contains(";")) {
					IList<String> viewIDs = viewID.Split(';');
					result = viewIDs.Contains(view.Id);
				}
				list["ViewID is " + viewID] = result;
			}
			return result;
		}
		private static Boolean IsFitToView(View view, Nesting targetViewNesting, Type targetObjectType, Type typeOfView, String targetViewId, BoolList list) {
			try {
				list.BeginUpdate();
				Boolean isFitToNesting = IsFitToNesting(view, targetViewNesting, list);
				Boolean isFitToObjectType = IsFitToObjectType(view, targetObjectType, list);
				Boolean isFitToViewType = IsFitToViewType(view, typeOfView, list);
				Boolean isFitToViewID = IsFitToViewID(view, targetViewId, list);
				return isFitToNesting && isFitToViewType && isFitToObjectType && isFitToViewID;
			}
			finally {
				list.EndUpdate();
			}
		}
		private void view_ControlsCreated(Object sender, EventArgs e) {
			OnViewControlsCreated();
		}
		private void view_ControlsDestroying(Object sender, EventArgs e) {
			OnViewControlsDestroying();
		}
		protected override void UpdateActionActivity(ActionBase action) {
			base.UpdateActionActivity(action);
			if(Active && IsFitToView(view, action.TargetViewNesting, action.TargetObjectType, action.TypeOfView, action.TargetViewId, action.Active)) {
				action.SelectionContext = view as ISelectionContext;
			}
			else {
				action.SelectionContext = null;
			}
		}
		protected virtual void OnViewChanging(View view) {
			IsFitToView(view, TargetViewNesting, TargetObjectType, TypeOfView, TargetViewId, Active);
		}
		protected override void OnActivated() {
			if(view != null) {
				view.ControlsCreated -= new EventHandler(view_ControlsCreated);
				view.ControlsCreated += new EventHandler(view_ControlsCreated);
				view.ControlsDestroying -= new EventHandler(view_ControlsDestroying);
				view.ControlsDestroying += new EventHandler(view_ControlsDestroying);
			}
			foreach(ActionBase action in Actions) {
				if(IsFitToView(view, action.TargetViewNesting, action.TargetObjectType, action.TypeOfView, action.TargetViewId, action.Active)) {
					action.SelectionContext = view as ISelectionContext;
				}
			}
			base.OnActivated();
		}
		protected override void OnDeactivated() {
			if(view != null) {
				view.ControlsCreated -= new EventHandler(view_ControlsCreated);
				view.ControlsDestroying -= new EventHandler(view_ControlsDestroying);
			}
			foreach(ActionBase action in Actions) {
				action.SelectionContext = null;
			}
			base.OnDeactivated();
		}
		protected virtual void OnViewChanged() {
		}
		protected virtual void OnViewControlsCreated() {
			if(ViewControlsCreated != null) {
				ViewControlsCreated(this, EventArgs.Empty);
			}
		}
		protected virtual void OnViewControlsDestroying() {
			if(ViewControlsDestroying != null) {
				ViewControlsDestroying(this, EventArgs.Empty);
			}
		}
		protected internal virtual void OnViewControllersActivated() {
		}
		protected IObjectSpace ObjectSpace {
			get { return (View == null) ? null : View.ObjectSpace; }
		}
		protected override void Dispose(bool disposing) {
			ViewControlsCreated = null;
			if(disposing) {
				if(view != null) {
					view.ControlsCreated -= new EventHandler(view_ControlsCreated);
					view.ControlsDestroying -= new EventHandler(view_ControlsDestroying);
					view = null;
				}
			}
			base.Dispose(disposing);
		}
		public ViewController() {
			Active.SetItemValue(ViewIsAssignedReason, false);
		}
		public void SetView(View newView) {
			if(newView == null) {
				Active.SetItemValue(ViewIsAssignedReason, false);
				view = null;
			}
			else {
				Active.BeginUpdate();
				try {
					if(Active) {
						throw new InvalidOperationException(String.Format("The '{0}' controller is active", GetType()));
					}
					OnViewChanging(newView);
					view = newView;
					OnViewChanged();
					Active.SetItemValue(ViewIsAssignedReason, true);
				}
				finally {
					Active.EndUpdate();
				}
			}
		}
		[Browsable(false)]
		public View View {
			get { return view; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ViewControllerTargetViewNesting"),
#endif
 Category("Target View"), DefaultValue(Nesting.Any)]
		public Nesting TargetViewNesting {
			get {
				return nesting;
			}
			set {
				nesting = value;
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ViewControllerTargetObjectType"),
#endif
 Category("Target View")]
		[TypeConverter(typeof(BusinessClassTypeConverter<object>)), DefaultValue(null)]
		public Type TargetObjectType {
			get {
				return objectType;
			}
			set {
				objectType = value;
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ViewControllerTargetViewType"),
#endif
 Category("Target View"), DefaultValue(ViewType.Any)]
		public ViewType TargetViewType {
			get {
				if(typeof(ListView).IsAssignableFrom(typeOfView)) {
					return ViewType.ListView;
				}
				if(typeof(DetailView).IsAssignableFrom(typeOfView)) {
					return ViewType.DetailView;
				}
				if(typeof(DashboardView).IsAssignableFrom(typeOfView)) {
					return ViewType.DashboardView;
				}
				return ViewType.Any;
			}
			set {
				if(value == ViewType.ListView) {
					typeOfView = typeof(ListView);
				}
				else if(value == ViewType.DetailView) {
					typeOfView = typeof(DetailView);
				}
				else if(value == ViewType.DashboardView) {
					typeOfView = typeof(DashboardView);
				}
				else {
					typeOfView = typeof(View);
				}
			}
		}
		[Browsable(false)]
		public Type TypeOfView {
			get { return typeOfView; }
			set { typeOfView = value; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ViewControllerTargetViewId"),
#endif
 Category("Target View"), DefaultValue(ActionBase.AnyCaption)]
		public string TargetViewId {
			get {
				return String.IsNullOrEmpty(targetViewID) ? ActionBase.AnyCaption : targetViewID;
			}
			set {
				targetViewID = value;
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ViewControllerViewControlsCreated"),
#endif
 Category("Events")]
		public event EventHandler ViewControlsCreated;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler ViewControlsDestroying;
	}
}
