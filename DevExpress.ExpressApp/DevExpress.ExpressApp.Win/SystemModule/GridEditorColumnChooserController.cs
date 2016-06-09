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
using System.Windows.Forms;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Win.Editors;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class GridEditorColumnChooserController : ObjectViewController {
		private GridListEditorColumnChooserExtender columnChooserExtender;
		public GridEditorColumnChooserController()
			: base() {
			TypeOfView = typeof(ListView);
		}
		protected void CreateColumnChooserExtender() {
			columnChooserExtender = CreateColumnChooserExtenderCore();
		}
		protected virtual GridListEditorColumnChooserExtender CreateColumnChooserExtenderCore() {
			GridListEditor gridEditor = ((ListView)View).Editor as GridListEditor;
			if(gridEditor != null && gridEditor.GridView != null) {
				return new GridListEditorColumnChooserExtender(((ListView)View).Model, gridEditor, ((ListView)View).ObjectTypeInfo, gridEditor.GridView);
			}
			return null;
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(!this.View.IsControlCreated) {
				this.View.ControlsCreated += View_ControlsCreated;
			}
			else {
				CreateColumnChooserExtender();
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			this.View.ControlsCreated -= View_ControlsCreated;
			if(columnChooserExtender != null) {
				columnChooserExtender.Dispose();
				columnChooserExtender = null;
			}
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			CreateColumnChooserExtender();
		}
	}
}
