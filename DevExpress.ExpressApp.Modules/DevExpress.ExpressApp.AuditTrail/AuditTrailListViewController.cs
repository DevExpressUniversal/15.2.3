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
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.AuditTrail;
namespace DevExpress.ExpressApp.AuditTrail {
	public class AuditTrailListViewController : ViewController {
		public AuditTrailListViewController() {
			TargetViewNesting = Nesting.Any;
			TargetViewType = ViewType.ListView;
		}
		private void CollectionSource_CriteriaApplied(object sender, EventArgs e) {
			XPObjectSpace objectSpace = View.ObjectSpace as XPObjectSpace;
			if(objectSpace != null) {
				((ListView)View).CollectionSource.CriteriaApplied -= new EventHandler(CollectionSource_CriteriaApplied);
				AuditTrailService.Instance.BeginSessionAudit(objectSpace.Session, AuditTrailStrategy.OnObjectLoaded);
			}
		}
		private void CollectionSource_CriteriaApplying(object sender, EventArgs e) {
			XPObjectSpace objectSpace = View.ObjectSpace as XPObjectSpace;
			if(objectSpace != null) {
				if(AuditTrailService.Instance.IsSessionAudited(objectSpace.Session) && AuditTrailService.Instance.GetStrategy(objectSpace.Session) == AuditTrailStrategy.OnObjectLoaded) {
					AuditTrailService.Instance.BeginSessionAudit(objectSpace.Session, AuditTrailStrategy.OnObjectChanged);
					((ListView)View).CollectionSource.CriteriaApplied += new EventHandler(CollectionSource_CriteriaApplied);
				}
			}
		}
		protected override void OnActivated() {
			((ListView)View).CollectionSource.CriteriaApplying += new EventHandler(CollectionSource_CriteriaApplying);
			base.OnActivated();
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			((ListView)View).CollectionSource.CriteriaApplying -= new EventHandler(CollectionSource_CriteriaApplying);
		}
	}
}
