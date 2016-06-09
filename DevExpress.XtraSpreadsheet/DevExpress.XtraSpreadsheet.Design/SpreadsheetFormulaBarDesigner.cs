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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Design;
namespace DevExpress.XtraSpreadsheet.Design {
	#region SpreadsheetFormulaBarDesigner
	public class SpreadsheetFormulaBarDesigner : BaseControlDesigner, IServiceProvider {
		IDesignerHost host;
		IComponentChangeService changeService;
		public SpreadsheetFormulaBarDesigner() {
		}
		public SpreadsheetFormulaBarControl FormulaBarControl { get { return Control as SpreadsheetFormulaBarControl; } }
		public IComponentChangeService ChangeService { get { return changeService; } }
		public IDesignerHost DesignerHost {
			get {
				if(host == null) {
					host = (IDesignerHost)GetService(typeof(IDesignerHost));
				}
				return host;
			}
		}
		object IServiceProvider.GetService(Type type) {
			return base.GetService(type);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(CreateSpreadsheetActionList());
			base.RegisterActionLists(list);
		}
		protected virtual DesignerActionList CreateSpreadsheetActionList() {
			return new SpreadsheetFormulaBarActionList(this);
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if(changeService != null) {
				changeService.ComponentAdded += OnComponentAdded;
				changeService.ComponentRemoved += OnComponentRemoved;
			}
		}
		protected virtual void OnComponentRemoved(object sender, ComponentEventArgs e) {
		}
		protected virtual void OnComponentAdded(object sender, ComponentEventArgs e) {
		}
	}
	#endregion
	#region SpreadsheetFormulaBarActionList
	public class SpreadsheetFormulaBarActionList : DesignerActionList {
		readonly SpreadsheetFormulaBarDesigner designer;
		public SpreadsheetFormulaBarActionList(SpreadsheetFormulaBarDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public SpreadsheetFormulaBarDesigner Designer { get { return designer; } }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			PopulateCreateBarsItems(result);
			return result;
		}
		protected internal virtual void PopulateCreateBarsItems(DesignerActionItemCollection items) {
			if (Designer.FormulaBarControl != null)
				items.Add(new DesignerActionPropertyItem("SpreadsheetControl", "SpreadsheetControl", "Toolbar"));
		}
		[RefreshProperties(RefreshProperties.All)]
		public ISpreadsheetControl SpreadsheetControl {
			get {
				return Designer.FormulaBarControl.SpreadsheetControl;
			}
			set {
				Designer.FormulaBarControl.SpreadsheetControl = value;
			}
		}
	}
	#endregion
}
