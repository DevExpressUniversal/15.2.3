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
using DevExpress.ExpressApp.Templates;
namespace DevExpress.ExpressApp.SystemModule {
	public abstract class CustomizeTemplateViewControllerBase<TemplateType> : ViewController where TemplateType : IFrameTemplate {
		private void AddControlsToTemplate() {
			if(Active && Frame.Template is TemplateType) {
				AddControlsToTemplateCore((TemplateType)Frame.Template);
				if(View != null) {
					UpdateControls(View);
					UpdateControls(View.CurrentObject);
				}
			}
		}
		private void RemoveControlsFromTemplate() {
			if(Frame.Template is TemplateType) {
				RemoveControlsFromTemplateCore((TemplateType)Frame.Template);
			}
		}
		private void Frame_TemplateChanged(object sender, EventArgs e) {
			AddControlsToTemplate();
		}
		private void Frame_TemplateChanging(object sender, EventArgs e) {
			RemoveControlsFromTemplate();
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			if(Frame.Template is TemplateType) {
				UpdateControls(View.CurrentObject);
			}
		}
		private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
			if(Frame != null && Frame.Template is TemplateType && e.Object != null && e.Object == View.CurrentObject) {
				UpdateControls(View.CurrentObject);
			}
		}
		protected abstract void UpdateControls(object currentObject);
		protected abstract void AddControlsToTemplateCore(TemplateType template);
		protected abstract void RemoveControlsFromTemplateCore(TemplateType template);
		protected abstract void UpdateControls(View view);
		protected override void OnActivated() {
			base.OnActivated();
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			View.ObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			Frame.TemplateChanging += new EventHandler(Frame_TemplateChanging);
			Frame.TemplateChanged += new EventHandler(Frame_TemplateChanged);
			AddControlsToTemplate();
		}
		protected override void OnDeactivated() {
			RemoveControlsFromTemplate();
			View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			View.ObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			Frame.TemplateChanging -= new EventHandler(Frame_TemplateChanging);
			Frame.TemplateChanged -= new EventHandler(Frame_TemplateChanged);
			base.OnDeactivated();
		}
	}
}
