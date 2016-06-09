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
using DevExpress.ExpressApp.Win.Templates.ActionContainers.Ribbon;
using DevExpress.ExpressApp.Win.Templates.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking2010;
namespace DevExpress.ExpressApp.Win.Templates {
	public partial class MainFormTemplateBase : XtraFormTemplateBase, IXafDocumentsHostWindow {
		private const bool CanManageDefaultSelectedPageDefaultValue = true;
		private UIType uiType = UIType.MultipleWindowSDI;
		private bool GetIsMdi() {
			return UIType == UIType.StandardMDI || UIType == UIType.TabbedMDI;
		}
		protected virtual void UpdateMdiModeDependentProperties() {
			bool isMdi = GetIsMdi();
			IsMdiContainer = isMdi;
			if(isMdi) {
				documentManager.ForceInitialize();
				if(UIType == UIType.TabbedMDI) {
					documentManager.View = tabbedView;
				}
				else {
					documentManager.View = nativeMdiView;
				}
				if(UIType == UIType.StandardMDI) {
					BarManager.MdiMenuMergeStyle = BarMdiMenuMergeStyle.OnlyWhenChildMaximized;
				}
				else {
					BarManager.MdiMenuMergeStyle = BarMdiMenuMergeStyle.Always;
				}
			}
		}
		protected override void OnLoad(EventArgs e) {
			CheckTransformToRibbon(true);
			if((RibbonTransformer != null) && (Ribbon != null) && (DocumentManager != null) && CanManageDefaultSelectedPage) {
				new RibbonDefaultSelectedPageHelper(RibbonTransformer.DefaultHomePageCaption).Attach(
				   this.DocumentManager.View, this.Ribbon);
			}
			base.OnLoad(e);
		}
		protected override void SetFormIcon(View view) { }
		#region IXafDocumentsHostWindow Members
		public UIType UIType {
			get { return uiType; }
			set {
				if(uiType != value) {
					uiType = value;
					UpdateMdiModeDependentProperties();
				}
			}
		}
		#endregion
		#region IDocumentsHostWindow Members
		public bool DestroyOnRemovingChildren {
			get { return true; }
		}
		public DocumentManager DocumentManager {
			get { return documentManager; }
		}
		#endregion
		public MainFormTemplateBase() {
			InitializeComponent();
			this.IsMdiContainer = false;
			this.CanManageDefaultSelectedPage = CanManageDefaultSelectedPageDefaultValue;
			new LoadingIndicatorHelper().Attach(documentManager);
			new DocumentManagerViewPopupMenuHelper().Attach(documentManager);
		}
		[DefaultValue(CanManageDefaultSelectedPageDefaultValue)]
		public bool CanManageDefaultSelectedPage { get; set; }
	}
}
