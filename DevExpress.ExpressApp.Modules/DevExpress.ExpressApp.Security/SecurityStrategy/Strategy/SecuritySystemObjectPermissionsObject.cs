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
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.ExpressApp.Security.Strategy {
	[System.ComponentModel.DisplayName("Object Operation Permissions")]
	[ImageName("BO_Security_Permission_Object")]
	[DefaultListViewOptions(true, NewItemRowPosition.Top)]
	public class SecuritySystemObjectPermissionsObject : XPCustomObject {
#if MediumTrust
		private Guid oid = Guid.Empty;
		[Browsable(false), Key(true), NonCloneable]
		public Guid Oid {
			get { return oid; }
			set { oid = value; }
		}
#else
		[Persistent("Oid"), Key(true), Browsable(false), MemberDesignTimeVisibility(false)]
		private Guid oid = Guid.Empty;
		[PersistentAlias("oid"), Browsable(false)]
		public Guid Oid {
			get { return oid; }
		}
#endif
		public SecuritySystemObjectPermissionsObject(Session session) : base(session) { }
		public IList<IOperationPermission> GetPermissions() {
			IList<IOperationPermission> result = new List<IOperationPermission>();
			if(Owner == null) {
				Tracing.Tracer.LogWarning("Cannot create OperationPermission objects: Owner property returns null. {0} class, {1} Oid", GetType(), Oid);
			}
			else if(Owner.TargetType == null) {
				Tracing.Tracer.LogWarning("Cannot create OperationPermission objects: Owner.TargetType property returns null. {0} class, {1} Oid", GetType(), Oid);
			}
			else {
				if(AllowRead) {
					result.Add(new ObjectOperationPermission(Owner.TargetType, Criteria, SecurityOperations.Read));
				}
				if(AllowWrite) {
					result.Add(new ObjectOperationPermission(Owner.TargetType, Criteria, SecurityOperations.Write));
				}
				if(AllowDelete) {
					result.Add(new ObjectOperationPermission(Owner.TargetType, Criteria, SecurityOperations.Delete));
				}
				if(AllowNavigate) {
					result.Add(new ObjectOperationPermission(Owner.TargetType, Criteria, SecurityOperations.Navigate));
				}
			}
			return result;
		}
		[Size(SizeAttribute.Unlimited)]
		[CriteriaOptions("Owner.TargetType")]
		[VisibleInListView(true)]
		[EditorAlias(EditorAliases.PopupCriteriaPropertyEditor)]
		public string Criteria {
			get { return GetPropertyValue<string>("Criteria"); }
			set { SetPropertyValue<string>("Criteria", value); }
		}
		[VisibleInListView(false), VisibleInDetailView(false)]
		public bool AllowRead {
			get { return GetPropertyValue<bool>("AllowRead"); }
			set { SetPropertyValue<bool>("AllowRead", value); }
		}
		[VisibleInListView(false), VisibleInDetailView(false)]
		public bool AllowWrite {
			get { return GetPropertyValue<bool>("AllowWrite"); }
			set { SetPropertyValue<bool>("AllowWrite", value); }
		}
		[VisibleInListView(false), VisibleInDetailView(false)]
		public bool AllowDelete {
			get { return GetPropertyValue<bool>("AllowDelete"); }
			set { SetPropertyValue<bool>("AllowDelete", value); }
		}
		[VisibleInListView(false), VisibleInDetailView(false)]
		public bool AllowNavigate {
			get { return GetPropertyValue<bool>("AllowNavigate"); }
			set { SetPropertyValue<bool>("AllowNavigate", value); }
		}
		[Association]
		[VisibleInListView(false), VisibleInDetailView(false)]
		public SecuritySystemTypePermissionsObjectBase Owner {
			get { return GetPropertyValue<SecuritySystemTypePermissionsObjectBase>("Owner"); }
			set { SetPropertyValue<SecuritySystemTypePermissionsObjectBase>("Owner", value);			 }
		}
		[NonPersistent]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DevExpress.Xpo.DisplayName("Read")]
		public bool? EffectiveRead {
			get { return AllowRead ? true : (Owner != null && Owner.AllowRead) ? (bool?)null : false; }
			set { AllowRead = value.HasValue ? value.Value : false; }
		}
		[NonPersistent]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DevExpress.Xpo.DisplayName("Write")]
		public bool? EffectiveWrite {
			get { return AllowWrite ? true : (Owner != null && Owner.AllowWrite) ? (bool?)null : false; }
			set { AllowWrite = value.HasValue ? value.Value : false; }
		}
		[NonPersistent]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DevExpress.Xpo.DisplayName("Delete")]
		public bool? EffectiveDelete {
			get { return AllowDelete ? true : (Owner != null && Owner.AllowDelete) ? (bool?)null : false; }
			set { AllowDelete = value.HasValue ? value.Value : false; }
		}
		[NonPersistent]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DevExpress.Xpo.DisplayName("Navigate")]
		public bool? EffectiveNavigate {
			get { return AllowNavigate ? true : (Owner != null && Owner.AllowNavigate) ? (bool?)null : false; }
			set { AllowNavigate = value.HasValue ? value.Value : false; }
		}
		public string InheritedFrom {
			get {
				string result = "";
				if(Owner != null) {
					if(Owner.AllowRead) {
						result = string.Concat(result, string.Format(CaptionHelper.GetLocalizedText("Messages", "Read") + CaptionHelper.GetLocalizedText("Messages", "IsInheritedFrom"), CaptionHelper.GetClassCaption(Owner.TargetType.FullName)));
					}
					if(Owner.AllowWrite) {
						result = string.Concat(result, string.Format(CaptionHelper.GetLocalizedText("Messages", "Write") + CaptionHelper.GetLocalizedText("Messages", "IsInheritedFrom"), CaptionHelper.GetClassCaption(Owner.TargetType.FullName)));
					}
					if(Owner.AllowDelete) {
						result = string.Concat(result, string.Format(CaptionHelper.GetLocalizedText("Messages", "Delete") + CaptionHelper.GetLocalizedText("Messages", "IsInheritedFrom"), CaptionHelper.GetClassCaption(Owner.TargetType.FullName)));
					}
					if(Owner.AllowNavigate) {
						result = string.Concat(result, string.Format(CaptionHelper.GetLocalizedText("Messages", "Navigate") + CaptionHelper.GetLocalizedText("Messages", "IsInheritedFrom"), CaptionHelper.GetClassCaption(Owner.TargetType.FullName)));
					}
				}
				return result;
			}
		}
	}
}
