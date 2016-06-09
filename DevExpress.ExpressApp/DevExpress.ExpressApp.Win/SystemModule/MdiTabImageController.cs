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
using System.Windows.Forms;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class MdiTabImageController : WindowController {
		private bool IsTabbedMdiStrategy {
			get {
				MdiShowViewStrategy strategy = Application.ShowViewStrategy as MdiShowViewStrategy;
				return strategy != null && strategy.MdiMode == MdiMode.Tabbed;
			}
		}
		private Form FormTemplate {
			get { return Window.Template as Form; }
		}
		private void ChildWindow_ViewChanged(object sender, ViewChangedEventArgs e) {
			if(FormTemplate != null) {
				if(FormTemplate.Visible) {
					SetTabImage(Window.View);
				}
				else {
					FormTemplate.Shown += Form_Shown;
				}
			}
		}
		private void Form_Shown(object sender, EventArgs e) {
			FormTemplate.Shown -= Form_Shown;
			SetTabImage(Window.View);
		}
		protected virtual void SetTabImage(View view) {
			BaseDocument activeDocument = GetActiveDocument();
			if(activeDocument != null) {
				using(BatchUpdate.Enter(activeDocument.Manager)) {
					activeDocument.Image = ImageLoader.Instance.GetImageInfo(ViewImageNameHelper.GetImageName(view)).Image;
				}
			}
		}
		protected BaseDocument GetActiveDocument() {
			IXafDocumentsHostWindow documentsHostWindow = FormTemplate != null ? FormTemplate.MdiParent as IXafDocumentsHostWindow : null;
			if(documentsHostWindow != null) {
				return documentsHostWindow.DocumentManager.GetDocument(FormTemplate);
			}
			return null;
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(IsTabbedMdiStrategy) {
				Window.ViewChanged += ChildWindow_ViewChanged;
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(IsTabbedMdiStrategy) {
				Window.ViewChanged -= ChildWindow_ViewChanged;
			}
		}
		public MdiTabImageController() {
			TargetWindowType = WindowType.Child;
		}
	}
}
