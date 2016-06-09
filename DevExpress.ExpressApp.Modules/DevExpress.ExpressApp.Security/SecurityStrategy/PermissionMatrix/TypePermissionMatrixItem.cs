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

using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.ExpressApp.Security.Strategy.PermissionMatrix {
	[DevExpress.ExpressApp.DC.DomainComponent]
	[DefaultListViewOptions(true, NewItemRowPosition.None)]
	[ImageName("BO_Security_Permission_Type")]
	[System.ComponentModel.DisplayName("Type Permission Matrix Item")]
#pragma warning disable 0618
	[NonPersistentEditable]
#pragma warning restore 0618
	public class TypePermissionMatrixItem : ITypePermissionOperations, INotifyPropertyChanged {
		private Type targetType;
		private BindingListEx<SecuritySystemMemberPermissionsObject> memberPermissions;
		private BindingListEx<SecuritySystemObjectPermissionsObject> objectPermissions;
		private void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		internal SecuritySystemTypePermissionObject CreateSourceInSpecificObjectSpace(IObjectSpace targetObjectSpace) {
			if(Source == null) {
				return targetObjectSpace.GetObject<IPermissionMatrixTypePermissionsOwner>(Owner).CreateItem(targetType);
			}
			return targetObjectSpace.GetObject(Source);
		}
		private SecuritySystemTypePermissionObject EnsureSource() {
			if(Source == null) {
				Source = Owner.CreateItem(targetType);
			}
			return Source;
		}
		private void Initialize() {
			memberPermissions = new BindingListEx<SecuritySystemMemberPermissionsObject>();
			memberPermissions.AllowNew = true;
			memberPermissions.AllowEdit = true;
			memberPermissions.AllowRemove = true;
			memberPermissions.ItemRemoving += delegate(object sender, ItemRemovingEventArgs e) {
				Source.MemberPermissions.Remove((SecuritySystemMemberPermissionsObject)e.Item);
				Source.Session.Delete(e.Item);
			};
			memberPermissions.AddingNew += delegate(object sender, AddingNewEventArgs e) {
				EnsureSource();
				e.NewObject = ((IBindingList)Source.MemberPermissions).AddNew();
			};
			objectPermissions = new BindingListEx<SecuritySystemObjectPermissionsObject>();
			objectPermissions.AllowNew = true;
			objectPermissions.AllowEdit = true;
			objectPermissions.AllowRemove = true;
			objectPermissions.ItemRemoving += delegate(object sender, ItemRemovingEventArgs e) {
				Source.ObjectPermissions.Remove((SecuritySystemObjectPermissionsObject)e.Item);
				Source.Session.Delete(e.Item);
			};
			objectPermissions.AddingNew += delegate(object sender, AddingNewEventArgs e) {
				EnsureSource();
				e.NewObject = ((IBindingList)Source.ObjectPermissions).AddNew();
			};
		}
		public TypePermissionMatrixItem(SecuritySystemTypePermissionObject source) {
			Guard.ArgumentNotNull(source, "source");
			this.targetType = source.TargetType;
			this.Source = source;
			Initialize();
			foreach(SecuritySystemMemberPermissionsObject memberPermissionsObject in source.MemberPermissions) {
				memberPermissions.Add(memberPermissionsObject);
			}
			foreach(SecuritySystemObjectPermissionsObject objectPermissionsObject in source.ObjectPermissions) {
				objectPermissions.Add(objectPermissionsObject);
			}
		}
		public TypePermissionMatrixItem(Type targetType, IPermissionMatrixTypePermissionsOwner owner) {
			Guard.ArgumentNotNull(owner, "owner");
			Guard.ArgumentNotNull(targetType, "targetType");
			this.targetType = targetType;
			this.Owner = owner;
			Initialize();
		}
		[Browsable(false)]
		public IPermissionMatrixTypePermissionsOwner Owner { get; private set; }
		[Browsable(false)]
		public SecuritySystemTypePermissionObject Source { get; internal set; }
		private static int currentId = 0;
		private int id = 0;
		[Browsable(false)]
		public int Id {
			get {
				if(id == 0) {
					id = ++currentId;
				}
				return id;
			}
			set { }
		}
		public string Object {
			get {
				string classCaption = CaptionHelper.GetClassCaption(TargetType.FullName);
				return string.IsNullOrEmpty(classCaption) ? TargetType.Name : classCaption;
			}
		}
		[VisibleInDetailView(false)]
		public string Category { get { return CaptionHelper.GetLocalizedText("Namespaces", TargetType.Namespace, TargetType.Namespace); } }
		[VisibleInListView(false), VisibleInDetailView(false)]
		public Type TargetType {
			get { return targetType; }
		}
		[System.ComponentModel.DisplayName("Read")]
		public bool AllowRead {
			get { return Source != null ? Source.AllowRead : false; }
			set {
				EnsureSource().AllowRead = value;
				RaisePropertyChanged("Read");
			}
		}
		[System.ComponentModel.DisplayName("Write")]
		public bool AllowWrite {
			get { return Source != null ? Source.AllowWrite : false; }
			set {
				EnsureSource().AllowWrite = value;
				RaisePropertyChanged("Write");
			}
		}
		[System.ComponentModel.DisplayName("Create")]
		public bool AllowCreate {
			get { return Source != null ? Source.AllowCreate : false; }
			set {
				EnsureSource().AllowCreate = value;
				RaisePropertyChanged("Create");
			}
		}
		[System.ComponentModel.DisplayName("Delete")]
		public bool AllowDelete {
			get { return Source != null ? Source.AllowDelete : false; }
			set {
				EnsureSource().AllowDelete = value;
				RaisePropertyChanged("Delete");
			}
		}
		[System.ComponentModel.DisplayName("Navigate")]
		public bool AllowNavigate {
			get { return Source != null ? Source.AllowNavigate : false; }
			set {
				EnsureSource().AllowNavigate = value;
				RaisePropertyChanged("Navigate");
			}
		}
		public BindingList<SecuritySystemMemberPermissionsObject> MemberPermissions {
			get { return memberPermissions; }
		}
		public BindingList<SecuritySystemObjectPermissionsObject> ObjectPermissions {
			get { return objectPermissions; }
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
