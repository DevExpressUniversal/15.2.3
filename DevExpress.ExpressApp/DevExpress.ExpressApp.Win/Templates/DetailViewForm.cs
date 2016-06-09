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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Templates {
	public partial class DetailViewForm : DevExpress.ExpressApp.Win.Templates.XtraFormTemplateBase, ISupportClassicToRibbonTransform {
		protected override void OnLoad(EventArgs e) {
			CheckTransformToRibbon(false);
			base.OnLoad(e);
		}
		protected override void SetFormIcon(View view) {
			if(view != null && view.Model != null) {
				IModelOptionsWin options = ((ModelNode)view.Model).Application.Options as IModelOptionsWin;
				if(options != null && options.UIType == UIType.StandardMDI) { 
					NativeMethods.SetFormIcon(this,
						ImageLoader.Instance.GetImageInfo(ViewImageNameHelper.GetImageName(view)).Image,
						ImageLoader.Instance.GetImageInfo(ViewImageNameHelper.GetImageName(view)).Image);
				}
				else {
					base.SetFormIcon(view);
				}
			}
		}
		protected override IModelFormState GetFormStateNode() {
			if(View != null) {
				return TemplatesHelper.GetFormStateNode(View.Id);
			}
			else {
				return base.GetFormStateNode();
			}
		}
		public override void SetSettings(IModelTemplate modelTemplate) {
			base.SetSettings(modelTemplate);
			formStateModelSynchronizerComponent.Model = GetFormStateNode();
		}
		public DetailViewForm() {
			InitializeComponent();
			BarManager.ForceLinkCreate();
		}
		public Bar MainMenuBar {
			get { return _mainMenuBar; }
		}
		public Bar ToolBar {
			get { return standardToolBar; }
		}
		public Bar ClassicStatusBar {
			get { return _statusBar; }
		}
	}
}
