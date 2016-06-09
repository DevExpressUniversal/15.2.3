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
using System.Windows.Forms;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.MultiLevelNumberingListForm.lbLevel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.MultiLevelNumberingListForm.lBoxLevel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.MultiLevelNumberingListForm.lblFollowNumberWith")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.MultiLevelNumberingListForm.edtFollowNumber")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.MultiLevelNumberingListForm.lblDisplayFormat")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	#region MultiLevelNumberingListForm
	[DXToolboxItem(false)]
	public partial class MultiLevelNumberingListForm : SimpleNumberingListFormBase {
		public MultiLevelNumberingListForm() { 
			InitializeComponent();
		}
		public MultiLevelNumberingListForm(ListLevelCollection<ListLevel> levels, int levelIndex, RichEditControl control, int startNumber, IFormOwner formOwner)
			: base(levels, levelIndex, control, formOwner) {
			InitializeComponent();
			this.Controller.EditedLevelIndex = levelIndex;
			this.Controller.Start = startNumber;
			UpdateForm();
		}
		#region Properties
		public new MultiLevelNumberingListFormController Controller { get { return (MultiLevelNumberingListFormController)base.Controller; } }
		#endregion
		protected override BulletedListFormController CreateController(ListLevelCollection<ListLevel> sourceLevels) {
			return new MultiLevelNumberingListFormController(sourceLevels);
		}
		protected override void UpdateFormCore() {
			base.UpdateFormCore();
			edtStart.Value = Controller.Start;
			lBoxLevel.SelectedIndex = Controller.EditedLevelIndex;
			UpdateSeparator();
		}
		protected internal override void SubscribeControlsEvents() {
			base.SubscribeControlsEvents();
			lBoxLevel.SelectedIndexChanged += OnLevelSelectedIndexChanged;
			edtFollowNumber.SelectedIndexChanged += OnFollowNumberSelectedIndexChanged;
		}
		protected internal override void UnsubscribeControlsEvents() {
			base.UnsubscribeControlsEvents();
			lBoxLevel.SelectedIndexChanged -= OnLevelSelectedIndexChanged;
			edtFollowNumber.SelectedIndexChanged -= OnFollowNumberSelectedIndexChanged;
		}
		protected internal override void ChangeFocus() {
			edtDisplayFormat.Focus();
		}
		protected internal virtual void UpdateSeparator() {
			char separator = Controller.Separator;
			switch (separator) {
				case Characters.TabMark:
					edtFollowNumber.SelectedIndex = 0;
					break;
				case ' ':
					edtFollowNumber.SelectedIndex = 1;
					break;
				default:
					edtFollowNumber.SelectedIndex = 2;
					break;
			}
		}
		private void OnLevelSelectedIndexChanged(object sender, EventArgs e) {
			int index = this.lBoxLevel.SelectedIndex;
			Controller.ApplyChanges();
			Controller.EditedLevelIndex = index;
			UpdateForm();
		}
		void OnFollowNumberSelectedIndexChanged(object sender, EventArgs e) {
			int index = edtFollowNumber.SelectedIndex;
			switch (index) {
				case 0:
					Controller.Separator = Characters.TabMark;
					break;
				case 1:
					Controller.Separator = ' ';
					break;
				default:
					Controller.Separator = '\u0000';
					break;
			}
		}
	}
	#endregion
}
