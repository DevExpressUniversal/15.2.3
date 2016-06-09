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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraLayout;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
namespace DevExpress.XtraLayout.Customization {
	public enum Category { Another, SizeConstrains, Group, URSLPanel, GroupElements }
	public class BaseInteraction {
		public BaseInteraction(UserInteractionHelper owner) { Owner = owner; Enabled = true; Checked = false; }
		public string Text { get; set; }
		public Font Font { get; set; }
		public VertAlignment Valignment { get; set; }
		public HorzAlignment Halignment { get; set; }
		public Image Image { get; set; }
		public bool Enabled { get; set; }
		public bool Checked { get; set; }
		public EditorTypes EditorType { get; set; }
		public GroupState State { get; set; }
		public int ItemsInRow { get; set; }
		public int RowsInGroup { get; set; }
		public Category Category { get; set; }
		protected UserInteractionHelper Owner { get; private set; }
		public virtual void Execute() {
			List<BaseLayoutItem> selItems = Owner.GetSelection();
			foreach(BaseLayoutItem item in selItems) ExecuteCore(item);
		}
		public virtual void ExecuteCore(BaseLayoutItem item) {
		}
		public virtual bool CanExecute() {
			List<BaseLayoutItem> selItems = Owner.GetSelection();
			return selItems.Count > 0;
		}
	}
}
