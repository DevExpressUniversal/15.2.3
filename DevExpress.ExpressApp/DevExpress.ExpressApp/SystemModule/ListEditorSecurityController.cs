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
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.SystemModule {
	public class ListEditorSecurityController : ListViewControllerBase {
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			Active.SetItemValue("IRequestSecurity", SecuritySystem.Instance is DevExpress.ExpressApp.Security.IRequestSecurity);
		}
		protected override void SubscribeToListEditorEvent() {
			if(View.Editor is ISupportEnabledCustomization) {
				((ISupportEnabledCustomization)(View.Editor)).CustomizeEnabled += new EventHandler<CustomizeEnabledEventArgs>(EnsureMemberLevelSecurityCanWrite);
			}
		}
		protected override void UnsubscribeToListEditorEvent() {
			if(View.Editor is ISupportEnabledCustomization) {
				((ISupportEnabledCustomization)(View.Editor)).CustomizeEnabled -= new EventHandler<CustomizeEnabledEventArgs>(EnsureMemberLevelSecurityCanWrite);
			}
		}
		private void EnsureMemberLevelSecurityCanWrite(object sender, CustomizeEnabledEventArgs e) {
			IAppearanceEnabled appearanceItem = e.Item as IAppearanceEnabled;
			if(appearanceItem != null && e.ContextObject != null) {
				appearanceItem.Enabled = DataManipulationRight.CanEdit(e.ContextObject.GetType(), e.Name, e.ContextObject, e.CollectionSource, e.ObjectSpace);
			}
		}
	}
	public interface ISupportEnabledCustomization {
		event EventHandler<CustomizeEnabledEventArgs> CustomizeEnabled;
	}
	public class CustomizeEnabledEventArgs : CustomizeAppearanceEventArgs {
		CollectionSourceBase collectionSource;
		IObjectSpace objectSpace;
		public CustomizeEnabledEventArgs(string name, IAppearanceBase item, object contextObject, CollectionSourceBase collectionSource, IObjectSpace objectSpace)
			: base(name, item, contextObject) {
			this.collectionSource = collectionSource;
			this.objectSpace = objectSpace;
		}
		public CollectionSourceBase CollectionSource {
			get { return collectionSource; }
			set { collectionSource = value; }
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
			set { objectSpace = value; }
		}
	}
}
