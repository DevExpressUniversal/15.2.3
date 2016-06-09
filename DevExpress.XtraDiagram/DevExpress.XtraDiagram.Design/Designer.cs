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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraDiagram.Design.Properties;
namespace DevExpress.XtraDiagram.Design {
	public class DiagramControlDesigner : ControlDesigner {
		public DiagramControlDesigner() {
		}
		DesignerActionListCollection actionLists = null;
		public override DesignerActionListCollection ActionLists {
			get {
				if(this.actionLists == null) {
					this.actionLists = new DesignerActionListCollection();
					this.actionLists.Add(new DiagramControlActionList(this));
				}
				return this.actionLists;
			}
		}
		public void RunDesigner() {
			Diagram.RunDesigner(GetOwnerForm());
		}
		public override ICollection AssociatedComponents {
			get {
				ICollection baseCol = base.AssociatedComponents;
				ArrayList col = new ArrayList();
				if(baseCol != null)
					col.AddRange(baseCol);
				col.Add(Diagram.Page);
				return col;
			}
		}
		protected Form GetOwnerForm() {
			return Diagram.FindForm();
		}
		public DiagramControl Diagram { get { return Component as DiagramControl; } }
	}
	public class DiagramControlActionList : DesignerActionList {
		DiagramControlDesigner designer;
		public DiagramControlActionList(DiagramControlDesigner designer) : base(designer.Diagram) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection col = new DesignerActionItemCollection();
			col.Add(new DesignerActionMethodItem(this, "RunDesigner", Resources.RunDesignerAction, true));
			col.Add(new DesignerActionHeaderItem("Commands", "Commands"));
			col.Add(new DesignerActionMethodItem(this, "CreateRibbon", DiagramControlLocalizer.Active.GetLocalizedString(DiagramControlStringId.CreateRibbonDesignerActionListCommand), "Commands"));
			col.Add(new DesignerActionHeaderItem("Layout", "Layout"));
			col.Add(new DesignerActionPropertyItem("Dock", "Choose Dock Style", "Layout"));
			col.Add(new DesignerActionHeaderItem("View", "View"));
			col.Add(new DesignerActionPropertyItem("TransparentRulerBackground", "Transparent Rulers", "View"));
			return col;
		}
		public void RunDesigner() {
			this.designer.RunDesigner();
		}
		public DockStyle Dock {
			get { return Diagram.Dock; }
			set { EditorContextHelper.SetPropertyValue(Component.Site, Diagram, "Dock", value); }
		}
		public void CreateRibbon() {
			DiagramDesignUtils.CreateRibbon(Diagram, true);
		}
		public bool TransparentRulerBackground {
			get { return Diagram.OptionsView.TransparentRulerBackground != DefaultBoolean.False; }
			set { Diagram.OptionsView.TransparentRulerBackground = value ? DefaultBoolean.Default : DefaultBoolean.False; }
		}
		public DiagramControl Diagram { get { return Component as DiagramControl; } }
	}
}
