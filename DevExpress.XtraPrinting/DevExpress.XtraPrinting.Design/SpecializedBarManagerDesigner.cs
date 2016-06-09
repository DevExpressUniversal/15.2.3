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
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraPrinting.Native;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraBars.Design;
namespace DevExpress.XtraPrinting.Design {
	public abstract class SpecializedBarManagerDesigner : BarManagerDesigner {
		static bool CanCreateDesignerActionList = true;
		BarManagerConfigurator[] updaters;
		DesignerVerb updateVerb;
		public bool UpdateNeeded {
			get {
				foreach(BarManagerConfigurator updater in updaters) {
					if(updater.UpdateNeeded)
						return true;
				}
				return false; 
			}
		}
		protected SpecializedBarManagerDesigner() {
		}
		public void Update() {
			BarManagerConfigurator.Configure(updaters, false);
			ValidateUpdateVerb();
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			updaters = CreateUpdaters();
			updateVerb = new DesignerVerb("Update", new EventHandler(OnUpdateClick));
		}
		protected override void OnNewComponent() {
			base.OnNewComponent();
			InitSpecializedBarManager();
			ValidateUpdateVerb();
			BarsReferencesHelper.AddBarsReferences(this);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			if(CanCreateDesignerActionList) {
				base.RegisterActionLists(list);
			}
		}
		protected abstract void InitSpecializedBarManager();
		protected abstract BarManagerConfigurator[] CreateUpdaters();
		protected override void CreateVerbs() {
			base.CreateVerbs();
			DXVerbs.Add(updateVerb);
			ValidateUpdateVerb();
		}
		protected override void CreateDefaultBars() {
		}
		protected override void CreateConvertToRibbonVerb() {
		}
		void ValidateUpdateVerb() {
			updateVerb.Visible = UpdateNeeded;
		}
		void OnUpdateClick(object sender, EventArgs e) {
			Update();
		}
	}
}
