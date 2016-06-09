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
using DevExpress.ExpressApp.Editors;
using System.ComponentModel;
namespace DevExpress.ExpressApp.ConditionalAppearance {
	public class RefreshAppearanceController : ObjectViewController {
		private bool forceRefreshOnObjectChanged = true;
		private bool forceRefreshOnCommitted = true;
		private bool forceRefreshOnSelectionChangedInDetailView = true;
		protected virtual void RefreshAppearance() {
			if(Active) {
				Frame.GetController<AppearanceController>().Refresh();
			}
		}
		private void View_SelectionChanged(object sender, EventArgs e) {
			if(forceRefreshOnSelectionChangedInDetailView) {
				RefreshAppearance();
			}
		}
		internal virtual void RefreshAppearance(object obj) {
			if(View != null
				) {
				RefreshAppearance();
			}
		}
		private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
			if (ForceRefreshOnObjectChanged) {
				RefreshAppearance(e.Object);
			}
		}
		private void ObjectSpace_Committed(object sender, EventArgs e) {
			if (ForceRefreshOnCommitted) {
				RefreshAppearance();
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(View is DetailView) {
				View.SelectionChanged += new EventHandler(View_SelectionChanged);
			}
			View.ObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			View.ObjectSpace.Committed += new EventHandler(ObjectSpace_Committed);
		}
		protected override void OnDeactivated() {
			View.ObjectSpace.Committed -= new EventHandler(ObjectSpace_Committed);
			View.ObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			if(View is DetailView) {
				View.SelectionChanged -= new EventHandler(View_SelectionChanged);
			}
			base.OnDeactivated();
		}
		public RefreshAppearanceController() {
			TypeOfView = typeof(ObjectView);
		}
		public bool ForceRefreshOnObjectChanged {
			get { return forceRefreshOnObjectChanged; }
			set { forceRefreshOnObjectChanged = value; }
		}
		public bool ForceRefreshOnCommitted {
			get { return forceRefreshOnCommitted; }
			set { forceRefreshOnCommitted = value; }
		}
		public bool ForceRefreshOnSelectionChangedInDetailView {
			get { return forceRefreshOnSelectionChangedInDetailView; }
			set { forceRefreshOnSelectionChangedInDetailView = value; }
		}
	}
}
