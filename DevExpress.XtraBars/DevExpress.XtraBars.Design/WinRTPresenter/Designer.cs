#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using System.ComponentModel.Design;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraBars.WinRTLiveTiles;
namespace DevExpress.XtraBars.WinRTLiveTiles.DesignTime {
	public class WinRTLiveTileManagerDesigner : BaseComponentDesigner {
		IComponentChangeService componentChangeService;
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			if (AllowDesigner)
				list.Add(new WinRTLiveTileManagerDesignerActionList(this));
			base.RegisterActionLists(list);
		}
		protected IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null) {
					componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
				}
				return componentChangeService;
			}
		}
		protected WinRTLiveTileManager Presenter {
			get {
				return Component as WinRTLiveTileManager;
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			((IWinRTLiveTileManagerDesignerMethods)Component).Changed += OnChanged;
			((IWinRTLiveTileManagerDesignerMethods)Component).Changing += OnChanging;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				((IWinRTLiveTileManagerDesignerMethods)Component).Changed -= OnChanged;
				((IWinRTLiveTileManagerDesignerMethods)Component).Changing -= OnChanging;
			}
			base.Dispose(disposing);
		}
		public virtual void OnChanged(Object sender, EventArgs e) {
			ComponentChangeService.OnComponentChanged(Component, null, null, null);
		}
		public virtual void OnChanging(object sender, CancelEventArgs e) {
			ComponentChangeService.OnComponentChanging(Component, null);
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			Presenter.BeginInit(); 
			Presenter.Id = Guid.NewGuid().ToString();
			Presenter.EndInit(); 
		}
		protected void ShowDesignerFormCore(Form designerForm) {
			if (Presenter== null) return;
			try {
				designerForm.ShowInTaskbar = false;
				designerForm.ShowDialog();
			}
			finally {
			}
		}
		public virtual void OnShowTileTemplateHelper(object sender, EventArgs ea) {
			TileTemplateHelperForm form = new TileTemplateHelperForm(Component.Site.Name, System.Reflection.Assembly.GetExecutingAssembly());
			StartChange();
			ShowDesignerFormCore(form);
			EndChange();
			form = null;
		}
		public virtual void OnConvertToStandartLayout(object sender, EventArgs ea) {
			Form form = new Form();
			StartChange();
			ShowDesignerFormCore(form);
			EndChange();
			form = null;
		}
		DesignerTransaction temporaryTransaction = null;
		protected void StartChange() {
			temporaryTransaction = DesignerHost.CreateTransaction("layout conversion");
		}
		protected void EndChange() {
			if (temporaryTransaction != null) {
				temporaryTransaction.Commit();
			}
			temporaryTransaction = null;
		}
		IDesignerHost designerHost;
		public IDesignerHost DesignerHost {
			get {
				if (designerHost == null) designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
				return designerHost;
			}
		}
	}
	public class WinRTLiveTileManagerDesignerActionList : DesignerActionList {
		WinRTLiveTileManagerDesigner designer;
		public WinRTLiveTileManagerDesignerActionList(WinRTLiveTileManagerDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public void ShowTileTemplateHelper() {
			if(designer != null) designer.OnShowTileTemplateHelper(this, null);
		}
		public void ToRegularLayout() {
			if (designer != null) designer.OnConvertToStandartLayout(this, null);
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			if (designer != null && designer.Component != null) {
			}
			return res;
		}
	}
}
