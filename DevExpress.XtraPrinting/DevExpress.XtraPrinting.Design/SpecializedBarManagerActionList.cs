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

using DevExpress.XtraPrinting.Preview;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraBars.Design;
namespace DevExpress.XtraPrinting.Design {
	public abstract class SpecializedBarManagerActionList : BarManagerActionList {
		public override bool AutoShow {
			get {
				return AutoShowCore;
			}
			set {
				AutoShowCore = value;
			}
		}		protected abstract string HeaderText { get; }
		SpecializedBarManagerDesigner SpecializedBarManagerDesigner {
			get { return (SpecializedBarManagerDesigner)Designer; }
		}
		protected SpecializedBarManagerActionList(SpecializedBarManagerDesigner designer)
			: base(designer) {
				AutoShowCore = false;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = base.GetSortedActionItems();
			if(SpecializedBarManagerDesigner.UpdateNeeded) {
				res.Add(new DesignerActionHeaderItem(HeaderText));
				res.Add(new DesignerActionMethodItem(this, "Update", "Update", HeaderText));
			}
			return res;
		}
		protected override void CreateConvertToRibbonItems(DesignerActionItemCollection actionItems) {
		}
		public void Update() {
			SpecializedBarManagerDesigner.Update();
		}
	}
}
