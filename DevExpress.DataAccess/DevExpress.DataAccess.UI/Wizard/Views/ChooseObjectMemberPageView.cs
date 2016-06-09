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
using System.Drawing;
using System.Linq;
using DevExpress.DataAccess.Native.ObjectBinding;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	public partial class ChooseObjectMemberPageView : WizardViewBase, IChooseObjectMemberPageView {
		const string HighlightedFieldName = "Highlighted";
		static readonly ColumnFilterInfo showAllFilter = new ColumnFilterInfo(string.Format("[{0}]", HighlightedFieldName));
		public enum MemberType {
			Property = 0,
			StaticProperty = 1,
			Method = 2,
			StaticMethod = 3
		};
		public class Item {
			public Item(ObjectMember data, string description) {
				Data = data;
				Description = description;
			}
			public MemberType MemberType {
				get {
					return Data.IsProperty
						? (Data.IsStatic ? MemberType.StaticProperty : MemberType.Property)
						: (Data.IsStatic ? MemberType.StaticMethod : MemberType.Method);
				}
			}
			public string Description { get; private set; }
			public bool Highlighted { get { return Data.Highlighted; } }
			public ObjectMember Data { get; private set; }
		}
		bool showAll = true;
		bool highlightedMembers;
		bool staticType;
		public ChooseObjectMemberPageView() {
			InitializeComponent();
			LocalizeComponent();
			InitializeIcons();
		}
		public bool ShowAll {
			get { return showAll; }
			protected set {
				checkEditShowOnlyHighlighted.Checked = !value;
				if(value == showAll)
					return;
				showAll = value;
				gridViewMembers.Columns[HighlightedFieldName].FilterInfo = value
					? ColumnFilterInfo.Empty
					: showAllFilter;
				if(!highlightedMembers)
					radioGroupEntireObject.Properties.Items.GetItemByValue(false).Enabled = value;
			}
		}
		#region Overrides of WizardViewBase
		public override string HeaderDescription { get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectMember); } }
		#endregion
		#region Implementation of IChooseObjectMemberPageView
		public ObjectMember Result {
			get {
				if((bool)radioGroupEntireObject.EditValue)
					return null;
				var row = gridViewMembers.GetFocusedRow() as Item;
				if(row == null)
					return null;
				return row.Data;
			}
			set {
				if(value == null) {
					if(!staticType) {
						radioGroupEntireObject.EditValue = true;
						gridControlMembers.Enabled = false;
					}
					else
						radioGroupEntireObject.EditValue = false;
					SelectRow(null);
					radioGroupEntireObject.Focus();
					return;
				}
				radioGroupEntireObject.EditValue = false;
				gridControlMembers.Enabled = true;
				if(!value.Highlighted)
					ShowAll = true;
				SelectRow(value);
				gridViewMembers.Focus();
			}
		}
		public void Initialize(IEnumerable<ObjectMember> items, bool staticType, bool showAll) {
			this.staticType = staticType;
			var data = new List<Item>();
			highlightedMembers = false;
			foreach(ObjectMember item in items) {
				highlightedMembers |= item.Highlighted;
				data.Add(new Item(item,
					string.Format(item.IsProperty ? "{0} : {1}" : "{0}{2} : {1}", item.Name,
						TypeNamesHelper.ShortName(item.ReturnType), item.Parameters)));
			}
			gridControlMembers.DataSource = data;
			radioGroupEntireObject.Properties.Items.GetItemByValue(true).Enabled = !staticType;
			if(!(highlightedMembers)) {
				ShowAll = true;
				checkEditShowOnlyHighlighted.Enabled = false;
			}
			else {
				ShowAll = showAll;
				checkEditShowOnlyHighlighted.Enabled = true;
			}
		}
		public event EventHandler Changed;
		#endregion
		protected virtual void OnGridViewMembersCustomDrawCell(GridView view, RowCellCustomDrawEventArgs e) {
			if(e.RowHandle == view.FocusedRowHandle && e.Column.VisibleIndex == 1) {
				Rectangle r = e.Bounds;
				r.X += gridControlMembers.Margin.Left;
				r.Width -= gridControlMembers.Margin.Right;
				e.Appearance.DrawString(e.Cache, e.DisplayText, r);
				e.Handled = true;
			}
		}
		protected void RaiseChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		void LocalizeComponent() {
			checkEditShowOnlyHighlighted.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectMember_ShowOnlyHighlighted);
			radioGroupEntireObject.Properties.Items[0].Description = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectMember_BindToObject);
			radioGroupEntireObject.Properties.Items[1].Description = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectMember_BindToMember);
		}
		void InitializeIcons() {
			ImageCollection images = new ImageCollection();
			images.AddImage(GetImage("Method.png"));
			images.AddImage(GetImage("Property.png"));
			images.AddImage(GetImage("StaticMethod.png"));
			images.AddImage(GetImage("StaticProperty.png"));
			repositoryItemMemberType.SmallImages = images;
			repositoryItemMemberType.Items.Add(new ImageComboBoxItem("Method", MemberType.Method, 0));
			repositoryItemMemberType.Items.Add(new ImageComboBoxItem("Property", MemberType.Property, 1));
			repositoryItemMemberType.Items.Add(new ImageComboBoxItem("Static method", MemberType.StaticMethod, 2));
			repositoryItemMemberType.Items.Add(new ImageComboBoxItem("Static property", MemberType.StaticProperty, 3));
		}
		void SelectRow(object value) {
			Item rowItem = ((IEnumerable<Item>)gridControlMembers.DataSource).FirstOrDefault(item => item.Data == value);
			int index = gridViewMembers.FindRow(rowItem);
			gridViewMembers.FocusedRowHandle = index;
			gridViewMembers.SelectRow(index);
		}
		Image GetImage(string name) {
			return ResourceImageHelper.CreateImageFromResources("DevExpress.DataAccess.UI.Wizard.Images." + name, GetType().Assembly);
		}
		private void checkEditShowAll_CheckedChanged(object sender, EventArgs e) { ShowAll = !checkEditShowOnlyHighlighted.Checked; }
		private void radioGroupMemberType_SelectedIndexChanged(object sender, EventArgs e) {
			bool value = (bool)radioGroupEntireObject.EditValue;
			gridControlMembers.Enabled = !value;
			if(value)
				radioGroupEntireObject.Focus();
			else
				gridViewMembers.Focus();
			RaiseChanged();
		}
		private void gridViewMembers_DoubleClick(object sender, EventArgs e) {
			GridView view = (GridView)sender;
			GridHitInfo hi = view.CalcHitInfo(view.GridControl.PointToClient(MousePosition));
			if(hi.InRow) {
				this.MoveForward();
			}
		}
		void gridViewMembers_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e) {
			OnGridViewMembersCustomDrawCell((GridView)sender, e);
		}
		private void gridViewMembers_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
			RaiseChanged();
		}
	}
}
