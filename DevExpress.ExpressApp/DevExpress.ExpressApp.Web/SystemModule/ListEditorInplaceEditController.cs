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
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public interface ISupportInplaceEdit {
		event EventHandler<EventArgs> CommitChanges;
	}
	public class ListEditorInplaceEditController : ViewController<ListView> {
		private const String autoCommitKey = "AutoCommit";
		private ISupportInplaceEdit editor;
		protected virtual void CommitChangesIfNeed() {
			if(AutoCommitChanges) {
				ObjectSpace.CommitChanges();
			}
		}
		private void editor_CommitChanges(object sender, EventArgs e) {
			CommitChangesIfNeed();
		}
		protected virtual bool AutoCommitChanges {
			get {
				return View.IsRoot || ShowViewStrategy.GetCollectionsEditMode(Frame, Application) == ViewEditMode.View;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			editor = View.Editor as ISupportInplaceEdit;
			if(editor != null) {
				editor.CommitChanges += new EventHandler<EventArgs>(editor_CommitChanges);
			}
		}
		protected override void OnDeactivated() {
			if(editor != null) {
				editor.CommitChanges -= new EventHandler<EventArgs>(editor_CommitChanges);
				editor = null;
			}
			base.OnDeactivated();
		}
	}
}
