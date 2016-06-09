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
using System.Text;
using DevExpress.Persistent.Base.General;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Utils;
using System.Windows.Forms;
namespace DevExpress.ExpressApp.Scheduler.Win {
	public class SchedulerResourceDeletingController : ObjectViewController {
		private void ObjectSpace_ObjectDeleted(object sender, ObjectsManipulatingEventArgs e) {
			foreach(object obj in e.Objects) {
				if(obj == View.CurrentObject) {
					ShowWarning();
					break;
				}
			}
		}
		protected virtual void ShowWarning() {
			Messaging.DefaultMessaging.Show(CaptionHelper.GetLocalizedText("Messages", "SchedulerResourceDeletingWarning"),
				CaptionHelper.GetLocalizedText("Captions", "SchedulerResourceDeletingWarning"), MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		protected override void OnActivated() {
			base.OnActivated();
			ObjectSpace.ObjectDeleted += new EventHandler<ObjectsManipulatingEventArgs>(ObjectSpace_ObjectDeleted);
		}
		protected override void OnDeactivated() {
			ObjectSpace.ObjectDeleted -= new EventHandler<ObjectsManipulatingEventArgs>(ObjectSpace_ObjectDeleted);
			base.OnDeactivated();
		}
		public SchedulerResourceDeletingController() {
			TargetObjectType = typeof(IResource);
			TargetViewType = ViewType.DetailView;
		}
	}
}
