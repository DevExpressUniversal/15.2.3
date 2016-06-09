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
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Security.ClientServer {
	[NonPersistent]
	public class MemberMatrixItem : INotifyPropertyChanged {
		private SecuritySystemMemberPermissionsObject owner;
		private bool isAllowed;
		public MemberMatrixItem(string memberName, SecuritySystemMemberPermissionsObject owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.MemberName = memberName;
			this.owner = owner;
		}
		public string MemberName { get; private set; }
		public bool IsAllowed {
			get { return isAllowed; }
			set {
				if(isAllowed != value) {
					isAllowed = value;
					OnPropertyChanged("IsAllowed");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName) {
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	[NonPersistent]
	public class MemberListDetailViewObject {
		private List<MemberMatrixItem> memberItems = new List<MemberMatrixItem>();
		public MemberListDetailViewObject(SecuritySystemMemberPermissionsObject memberPermission) {
			this.MemberPermission = memberPermission;
			Initialize();
		}
		public void Initialize() {
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(MemberPermission.Owner.TargetType);
			foreach(IMemberInfo memberInfo in typeInfo.Members) {
				if(memberInfo.IsVisible) {
					ReadAllowed = MemberPermission.AllowRead;
					WriteAllowed = MemberPermission.AllowWrite;
					Criteria = MemberPermission.Criteria;
					MemberMatrixItem item = new MemberMatrixItem(memberInfo.Name, MemberPermission);
					memberItems.Add(item);
					item.IsAllowed = MemberPermission.Members.Contains(item.MemberName);
				}
			}
		}
		public void Synchronize() {
			string membersString = "";
			foreach(MemberMatrixItem item in memberItems) {
				if(item.IsAllowed) {
					membersString += item.MemberName + ";";
				}
			}
			membersString.Remove(membersString.Length - 1);
			MemberPermission.Criteria = Criteria;
			MemberPermission.Members = membersString;
			MemberPermission.AllowRead = ReadAllowed;
			MemberPermission.AllowWrite = WriteAllowed;
			MemberPermission.Save();
		}
		public List<MemberMatrixItem> AllItems {
			get { return memberItems; }
			set { memberItems = value; }
		}
		public bool ReadAllowed {
			get;
			set;
		}
		public bool WriteAllowed {
			get;
			set;
		}
		[Size(SizeAttribute.Unlimited)]
		[CriteriaOptions("Owner.TargetType")]
		[VisibleInListView(true)]
		[EditorAlias(EditorAliases.PopupCriteriaPropertyEditor)]
		public string Criteria {
			get;
			set;
		}
		public SecuritySystemMemberPermissionsObject MemberPermission {
			get;
			set;
		}
	}
}
