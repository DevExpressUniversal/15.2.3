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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security;
namespace DevExpress.ExpressApp {
	[Serializable]
	public class InfiniteRecursionException : InvalidOperationException {
		protected InfiniteRecursionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public InfiniteRecursionException(string message) : base(message) { }
	}
	public class DetailView : ObjectView {
		public const string ViewEditModeKeyName = "mode";
		public const string DetailViewReadOnlyKey = "DetailView.AllowEdit";
		private ViewEditMode viewEditMode = ViewEditMode.View;
		private Object currentObject;
		private Boolean raiseObjectChangedOnControlValueChanged;
		private Dictionary<ViewEditMode, Point> scrollPositions = new Dictionary<ViewEditMode, Point>();
		private void UnsubscribeFromObject(Object currentObject) {
			INotifyPropertyChanged notifyPropertyChanged = currentObject as INotifyPropertyChanged;
			if(notifyPropertyChanged != null) {
				notifyPropertyChanged.PropertyChanged -= new PropertyChangedEventHandler(notifyPropertyChanged_PropertyChanged);
			}
		}
		private void SubscribeToObject(Object currentObject) {
			INotifyPropertyChanged notifyPropertyChanged = currentObject as INotifyPropertyChanged;
			if(notifyPropertyChanged != null) {
				notifyPropertyChanged.PropertyChanged += new PropertyChangedEventHandler(notifyPropertyChanged_PropertyChanged);
			}
		}
		private void notifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			RefreshViewItemByPropertyName(e.PropertyName);
		}
		private Object GetLastMemberOwner(Object masterObject, IMemberInfo memberInfo) {
			return memberInfo.GetOwnerInstance(masterObject);
		}
		private void editor_ControlValueChanged(Object sender, EventArgs e) {
			DisableEditorsRefreshing();
			try {
				if(raiseObjectChangedOnControlValueChanged) {
					ObjectSpace.SetModified(GetLastMemberOwner(currentObject, ((PropertyEditor)sender).MemberInfo), ((PropertyEditor)sender).MemberInfo);
				}
				else {
					ObjectSpace.SetModified(null, ((PropertyEditor)sender).MemberInfo);
				}
			}
			finally {
				EnableEditorsRefreshing();
			}
		}
		private void UpdateLayoutManagerViewEditMode() {
			if(LayoutManager is ISupportViewEditMode) {
				((ISupportViewEditMode)LayoutManager).ViewEditMode = viewEditMode;
			}
		}
		protected internal override String GetObjectCaptionFormat() {
			if(Model != null) {
				return Model.ObjectCaptionFormat;
			}
			else {
				return base.GetObjectCaptionFormat();
			}
		}
		protected override void OnAllowEditChanged() {
			base.OnAllowEditChanged();
			foreach(ViewItem detailViewItem in GetItems<PropertyEditor>()) {
				if(detailViewItem is PropertyEditor) {
					((PropertyEditor)detailViewItem).AllowEdit.SetItemValue(DetailViewReadOnlyKey, AllowEdit);
					detailViewItem.Refresh();
				}
			}
		}
		protected override void OnSelectionChanged() {
			base.OnSelectionChanged();
			OnCaptionChanged();
		}
		protected virtual void OnViewEditModeChanging(CancelEventArgs args) {
			if(ViewEditModeChanging != null)
				ViewEditModeChanging(this, args);
		}
		protected virtual void OnViewEditModeChanged() {
			if(ViewEditModeChanged != null)
				ViewEditModeChanged(this, EventArgs.Empty);
		}
		protected override void InitializeItem(ViewItem item) {
			if(item is ISupportViewEditMode) {
				ISupportViewEditMode supportViewEditMode = (ISupportViewEditMode)item;
				supportViewEditMode.ViewEditMode = ViewEditMode;
			}
			base.InitializeItem(item);
		}
		protected override void OnObjectSpaceReloaded() {
			CurrentObject = ObjectSpace.GetObject(currentObject);
		}
		protected override void DisposeCore() {
			UnsubscribeFromObject(currentObject);
			ViewEditModeChanging = null;
			ViewEditModeChanged = null;
			base.DisposeCore();
		}
		protected override void ItemChangedInternal(ViewItem item, ViewItemsChangedType changeType) {
			base.ItemChangedInternal(item, changeType);
			PropertyEditor editor = item as PropertyEditor;
			if (editor != null) {
				if (changeType == ViewItemsChangedType.Added) {
					editor.ControlValueChanged += new EventHandler(editor_ControlValueChanged);
					editor.AllowEdit.SetItemValue(DetailViewReadOnlyKey, AllowEdit);
				} else {
					editor.ControlValueChanged -= new EventHandler(editor_ControlValueChanged);
				}
			}
		}
		protected override void UpdateSecurityModifiers() {
			if(CurrentObject != null && !(SecuritySystem.Instance is DevExpress.ExpressApp.Security.IRequestSecurity)) {
				AllowEdit.SetItemValue(SecurityReadOnlyItemName, DataManipulationRight.CanEdit(CurrentObject.GetType(), "", CurrentObject, null, ObjectSpace)); 
			}
			else {
				base.UpdateSecurityModifiers();
			}
		}
		protected override ViewShortcut CreateShortcutCore() {
			ViewShortcut result = ViewShortcut.Empty;
			if(String.IsNullOrEmpty(keyMemberValueForPendingLoading)
				&&
				(
					(CurrentObject == null)
					||
					(ObjectSpace != null) && ObjectSpace.IsNewObject(CurrentObject)
				)
			) {
				result = new ViewShortcut(null, null, null);
				result[ViewEditModeKeyName] = "";
				result[ViewShortcut.IsNewObject] = "true";
				result[ViewShortcut.TemporaryObjectKeyParamName] = CurrentObject != null ? CurrentObject.GetHashCode().ToString() : "";
			}
			else {
				result = base.CreateShortcutCore();
				result[ViewEditModeKeyName] = viewEditMode.ToString();
			}
			return result;
		}
		static DetailView() {
			ViewShortcut.EqualsDefaultIgnoredParameters.Add(ViewEditModeKeyName);
		}
		public DetailView(IModelDetailView info, IObjectSpace objectSpace, Object obj, XafApplication application, Boolean isRoot)
			: base(application, objectSpace, isRoot) {
			DelayedItemsInitialization = true;
			raiseObjectChangedOnControlValueChanged = true;
			CurrentObject = obj;
			if((ObjectTypeInfo == null) && (obj != null)) {
				ObjectTypeInfo = XafTypesInfo.Instance.FindTypeInfo(obj.GetType());
			}
			if(application != null) {
				UpdateLayoutManagerViewEditMode();
			}
			if(info != null) {
				SetModel(info);
			}
		}
		public DetailView(IObjectSpace objectSpace, Object obj, XafApplication application, Boolean isRoot)
			: this(null, objectSpace, obj, application, isRoot) {
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DetailViewCurrentObject")]
#endif
		public override object CurrentObject {
			get { return currentObject; }
			set {
				if(currentObject != value) {
					if(IsRoot) {
						Tracing.Tracer.LogVerboseValue("CurrentObject", value);
					}
					if(IsDisposed) {
						throw new ObjectDisposedException(GetType().FullName);
					}
					Guard.CheckObjectFromObjectSpace(ObjectSpace, value);
					if(OnQueryCanChangeCurrentObject()) {
						UnsubscribeFromObject(currentObject);
						currentObject = ObjectSpace.GetObject(value);
						SubscribeToObject(currentObject);
						foreach(ViewItem item in Items) {
							item.CurrentObject = currentObject;
						}
						OnSelectionChanged();
						OnCurrentObjectChanged();
						UpdateSecurityModifiers();
					}
				}
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DetailViewSelectedObjects")]
#endif
		public override IList SelectedObjects {
			get {
				if(CurrentObject != null) {
					return new Object[] { CurrentObject };
				}
				else {
					return new Object[] { };
				}
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DetailViewSelectionType")]
#endif
		public override SelectionType SelectionType {
			get { return SelectionType.Full; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DetailViewScrollPosition")]
#endif
		public override Point ScrollPosition {
			get {
				Point scrollPosition;
				if(!scrollPositions.TryGetValue(viewEditMode, out scrollPosition)) {
					scrollPosition = new Point(0, 0);
				}
				return scrollPosition;
			}
			set {
				scrollPositions[viewEditMode] = value;
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DetailViewViewEditMode")]
#endif
		public ViewEditMode ViewEditMode {
			get { return viewEditMode; }
			set {
				if(viewEditMode != value) {
					CancelEventArgs args = new CancelEventArgs();
					OnViewEditModeChanging(args);
					if(args.Cancel) {
						return;
					}
					viewEditMode = value;
					UpdateLayoutManagerViewEditMode();
					foreach(ViewItem detailViewItem in Items) {
						if(detailViewItem is ISupportViewEditMode) {
							((ISupportViewEditMode)detailViewItem).ViewEditMode = viewEditMode;
						}
					}
					OnViewEditModeChanged();
				}
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("DetailViewModel")]
#endif
		public new IModelDetailView Model {
			get { return (IModelDetailView)base.Model; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Boolean RaiseObjectChangedOnControlValueChanged {
			get { return raiseObjectChangedOnControlValueChanged; }
			set { raiseObjectChangedOnControlValueChanged = value; }
		}
		public override void RefreshDataSource() {
			base.RefreshDataSource();
			if((ObjectSpace != null) && (currentObject != null)) {
				ObjectSpace.ReloadObject(currentObject);
			}
		}
		public event EventHandler<CancelEventArgs> ViewEditModeChanging;
		public event EventHandler<EventArgs> ViewEditModeChanged;
	}
	public enum ViewItemsChangedType { Added, Removed };
	public class ViewItemsChangedEventArgs : EventArgs {
		private ViewItemsChangedType changedType;
		private ViewItem item;
		public ViewItemsChangedEventArgs(ViewItemsChangedType changedType, ViewItem item) {
			this.changedType = changedType;
			this.item = item;
		}
		public ViewItemsChangedType ChangedType {
			get { return changedType; }
		}
		public ViewItem Item {
			get { return item; }
		}
	}
}
