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
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public partial class AboutInfoController : WindowController {
		protected virtual void OnAboutInfoCustomizePopupWindowParams(Object sender, CustomizePopupWindowParamsEventArgs e) {
			AboutInfo.Instance.Init(Application);
			e.View = Application.CreateDetailView(Application.CreateObjectSpace(AboutInfo.Instance.GetType()), AboutInfo.Instance, true);
			e.View.ModelSaving += new EventHandler<CancelEventArgs>(OnAboutInfoViewInfoSynchronizing);
			FillAboutInfoDetailView((DetailView)e.View);
			e.DialogController.CancelAction.Active.SetItemValue("It's unnecessary", false);
			e.IsSizeable = false;
		}
		protected virtual void OnAboutInfoViewInfoSynchronizing(Object sender, CancelEventArgs e) {
			e.Cancel = true;
		}
		private void FillAboutInfoDetailView(DetailView detailView) {
			StaticText aboutTextItem = detailView.FindItem("AboutText") as StaticText;
			if(aboutTextItem != null) {
				aboutTextItem.Text = AboutInfo.Instance.AboutInfoString;
			}
			StaticImage aboutImageItem = detailView.FindItem("Logo") as StaticImage;
			if(aboutImageItem != null) {
				aboutImageItem.ImageName = AboutInfo.Instance.LogoImageName;
			}
		}
		public AboutInfoController() {
			InitializeComponent();
			RegisterActions(components);
		}
		public PopupWindowShowAction AboutInfoAction {
			get { return aboutInfoAction; }
		}
	}
}
