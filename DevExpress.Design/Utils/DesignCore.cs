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

using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
namespace DevExpress.LookAndFeel.Design {
	class DefaultLookAndFeelDesigner : BaseComponentDesigner {
		public DefaultLookAndFeelDesigner() {
			((UserLookAndFeelDefault)UserLookAndFeel.Default).SetUserLookAndFeelModified();;
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new DefaultLookAndFeelActionList(Component));
			base.RegisterActionLists(list);
		}
	}
	class DefaultLookAndFeelActionList : DesignerActionList {
		public DefaultLookAndFeelActionList(IComponent component)
			: base(component) {
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection items = new DesignerActionItemCollection();
			items.Add(new DesignerActionHeaderItem(CategoryName.Options));
			items.Add(new DesignerActionPropertyItem("SkinName", "Skin Name:", CategoryName.Options));
			if(AllowActionHeader) {
				items.Add(new DesignerActionHeaderItem(CategoryName.Action));
			}
			if(AllowRegisterBonusSkin) {
				items.Add(new DesignerActionMethodItem(this, "RegisterBonusSkins", "Register BonusSkins", CategoryName.Action));
			}
			return items;
		}
		protected virtual bool AllowRegisterBonusSkin {
			get {
				EnvDTE.Project project = ProjectHelper.GetActiveProject(LookAndFeel.Site);
				if(project == null || ProjectHelper.IsReferenceExists(project, AssemblyInfo.SRAssemblyBonusSkins)) return false;
				return true;
			}
		}
		protected bool AllowActionHeader { get { return AllowRegisterBonusSkin; } }
		[TypeConverter(typeof(SkinNameTypeConverter))]
		public string SkinName {
			get { return LookAndFeel.LookAndFeel.SkinName; }
			set { EditorContextHelper.SetPropertyValue(Component.Site, LookAndFeel.LookAndFeel, "SkinName", value); }
		}
		public void RegisterBonusSkins() {
			EnvDTE.Project project = ProjectHelper.GetActiveProject(LookAndFeel.Site);
			if(project == null) return;
			try {
				ProjectHelper.AddReference(project, AssemblyInfo.SRAssemblyBonusSkins);
			}
			catch { }
			EditorContextHelperEx.HideSmartPanel(LookAndFeel);
			DesignTimeTools.EnsureBonusSkins();
			EditorContextHelper.SetPropertyValue(LookAndFeel.Site, LookAndFeel, "EnableBonusSkins", true);
		}
		protected DefaultLookAndFeel LookAndFeel { get { return Component as DefaultLookAndFeel; } }
	}
}
