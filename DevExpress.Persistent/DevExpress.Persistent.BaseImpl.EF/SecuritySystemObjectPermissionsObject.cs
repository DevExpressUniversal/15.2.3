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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.Persistent.BaseImpl.EF {
	[System.ComponentModel.DisplayName("Object Operation Permissions")]
	[ImageName("BO_Security_Permission_Object")]
	[DefaultListViewOptions(true, NewItemRowPosition.Top)]
	public class SecuritySystemObjectPermissionsObject : IOwnerInitializer {
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		[FieldSize(FieldSizeAttribute.Unlimited)]
		[CriteriaOptions("Owner.TargetType")]
		[VisibleInListView(true)]
		[EditorAlias(EditorAliases.PopupCriteriaPropertyEditor)]
		public string Criteria { get; set; }
		[VisibleInListView(false), VisibleInDetailView(false)]
		public bool AllowRead { get; set; }
		[VisibleInListView(false), VisibleInDetailView(false)]
		public bool AllowWrite { get; set; }
		[VisibleInListView(false), VisibleInDetailView(false)]
		public bool AllowDelete { get; set; }
		[VisibleInListView(false), VisibleInDetailView(false)]
		public bool AllowNavigate { get; set; }
		[VisibleInListView(false), VisibleInDetailView(false)]
		public virtual TypePermissionObject Owner { get; set; }
		public IList<IOperationPermission> GetPermissions() {
			IList<IOperationPermission> result = new List<IOperationPermission>();
			if(Owner == null) {
				Tracing.Tracer.LogWarning("Cannot create OperationPermission objects: Owner property returns null. {0} class, {1} Id", GetType(), ID);
			}
			else if(Owner.TargetType == null) {
				Tracing.Tracer.LogWarning("Cannot create OperationPermission objects: Owner.TargetType property returns null. {0} class, {1} Id", GetType(), ID);
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
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DisplayName("Read")]
		public bool? EffectiveRead {
			get { return AllowRead ? true : (Owner != null && Owner.AllowRead) ? (bool?)null : false; }
			set { AllowRead = value.HasValue ? value.Value : false; }
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DisplayName("Write")]
		public bool? EffectiveWrite {
			get { return AllowWrite ? true : (Owner != null && Owner.AllowWrite) ? (bool?)null : false; }
			set { AllowWrite = value.HasValue ? value.Value : false; }
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DisplayName("Delete")]
		public bool? EffectiveDelete {
			get { return AllowDelete ? true : (Owner != null && Owner.AllowDelete) ? (bool?)null : false; }
			set { AllowDelete = value.HasValue ? value.Value : false; }
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DisplayName("Navigate")]
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
		#region IMasterObjectInitializer Members
		void IOwnerInitializer.SetMasterObject(object masterObject) {
			TypePermissionObject typePermission = masterObject as TypePermissionObject;
			if(typePermission != null) {
				Owner = typePermission;
			}
		}
		#endregion
	}
}
