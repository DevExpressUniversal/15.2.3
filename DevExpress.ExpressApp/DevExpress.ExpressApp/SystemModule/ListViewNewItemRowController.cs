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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
namespace DevExpress.ExpressApp.SystemModule {
	public class CustomCalculateNewItemRowPositionEventArgs : EventArgs {
		private NewItemRowPosition newItemRowPosition;
		public CustomCalculateNewItemRowPositionEventArgs(NewItemRowPosition newItemRowPosition) {
			this.newItemRowPosition = newItemRowPosition;
		}
		public NewItemRowPosition NewItemRowPosition {
			get { return newItemRowPosition; }
			set { newItemRowPosition = value; }
		}
	}
	public class NewItemRowListViewController : ListViewControllerBase, IModelExtender {
		private NewObjectViewController newObjectController;
		private bool CanCreate() {
			bool canCreate = false;
			if(newObjectController != null) {
				canCreate = newObjectController.NewObjectAction.Active && newObjectController.NewObjectAction.Enabled;
			}
			return canCreate;
		}
		protected override void SubscribeToListEditorEvent() {
			base.SubscribeToListEditorEvent();
			if(View.Editor != null) {
				View.Editor.AllowEditChanged += new EventHandler(Editor_AllowEditChanged);
			}
			UpdateNewItemRowPosition();
		}
		protected override void UnsubscribeToListEditorEvent() {
			base.UnsubscribeToListEditorEvent();
			if(View.Editor != null) {
				View.Editor.AllowEditChanged -= new EventHandler(Editor_AllowEditChanged);
			}
		}
		private void Editor_AllowEditChanged(object sender, EventArgs e) {
			UpdateNewItemRowPosition();
		}
		private void NewAction_Changed(object sender, ActionChangedEventArgs e) {
			if(e.ChangedPropertyType == ActionChangedType.Active || e.ChangedPropertyType == ActionChangedType.Enabled) {
				UpdateNewItemRowPosition();
			}
		}
		private void ListView_ModelChanged(object sender, EventArgs e) {
			UpdateNewItemRowPosition();
		}
		private void View_AllowEditChanged(object sender, EventArgs e) {
			UpdateNewItemRowPosition();
		}
		protected override void OnActivated() {
			base.OnActivated();
			newObjectController = Frame.GetController<NewObjectViewController>();
			if(newObjectController != null) {
				newObjectController.NewObjectAction.Changed += new EventHandler<ActionChangedEventArgs>(NewAction_Changed);
			}
			View.AllowEditChanged += new EventHandler(View_AllowEditChanged);
			View.ModelChanged += new EventHandler(ListView_ModelChanged);
			UpdateNewItemRowPosition();
		}
		protected override void OnDeactivated() {
			if(newObjectController != null) {
				newObjectController.NewObjectAction.Changed -= new EventHandler<ActionChangedEventArgs>(NewAction_Changed);
			}
			View.AllowEditChanged -= new EventHandler(View_AllowEditChanged);
			View.ModelChanged -= new EventHandler(ListView_ModelChanged);
			base.OnDeactivated();
		}
		protected virtual NewItemRowPosition CalculateNewItemRowPosition() {
			NewItemRowPosition result = NewItemRowPosition.None;
			if(CanCreate() && View.AllowEdit && View.Model != null) {
				result = ((IModelListViewNewItemRow)View.Model).NewItemRowPosition;
			}
			return result;
		}
		public NewItemRowListViewController() {
			this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
		}
		public void UpdateNewItemRowPosition() {
			if(View != null && View.Editor != null && View.Editor is ISupportNewItemRowPosition) {
				CustomCalculateNewItemRowPositionEventArgs args = new CustomCalculateNewItemRowPositionEventArgs(CalculateNewItemRowPosition());
				if(CustomCalculateNewItemRowPosition != null) {
					CustomCalculateNewItemRowPosition(this, args);
				}
				((ISupportNewItemRowPosition)View.Editor).NewItemRowPosition = args.NewItemRowPosition;
			}
		}
		public event EventHandler<CustomCalculateNewItemRowPositionEventArgs> CustomCalculateNewItemRowPosition;
		#region IExtendModel Members
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelClass, IModelClassNewItemRow>();
			extenders.Add<IModelListView, IModelListViewNewItemRow>();
		}
		#endregion
	}
	[DomainLogic(typeof(IModelListViewNewItemRow))]
	public static class NewItemRowPositionDomainLogic {
		public static NewItemRowPosition Get_NewItemRowPosition(IModelListViewNewItemRow modelListView) {
			if(((IModelListView)modelListView).ModelClass != null) {
				return ((IModelClassNewItemRow)((IModelListView)modelListView).ModelClass).DefaultListViewNewItemRowPosition;
			}
			return NewItemRowPosition.None;
		}
	}
	[DomainLogic(typeof(IModelClassNewItemRow))]
	public static class DefaultListViewNewItemRowPositionDomainLogic {
		public static NewItemRowPosition Get_DefaultListViewNewItemRowPosition(IModelClass modelClass) {
			NewItemRowPosition result = DefaultListViewOptionsAttribute.Default.NewItemRowPosition;
			DefaultListViewOptionsAttribute defaultListViewOptionsAttribute = modelClass.TypeInfo.FindAttribute<DefaultListViewOptionsAttribute>();
			if(defaultListViewOptionsAttribute != null) {
				result = defaultListViewOptionsAttribute.NewItemRowPosition;
			}
			return result;
		}
	}
	public interface IModelClassNewItemRow {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassNewItemRowDefaultListViewNewItemRowPosition"),
#endif
 Category("Behavior")]
		NewItemRowPosition DefaultListViewNewItemRowPosition { get; set; }
	}
	public interface IModelListViewNewItemRow {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewNewItemRowNewItemRowPosition"),
#endif
 Category("Behavior")]
		NewItemRowPosition NewItemRowPosition { get; set; }
	}
}
