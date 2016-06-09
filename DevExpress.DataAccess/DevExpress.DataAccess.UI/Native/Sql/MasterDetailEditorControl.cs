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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.DataAccess.UI.Native.Sql {
	[ToolboxItem(false)]
	public partial class MasterDetailEditorControl : ConditionEditorControl {
		#region inner classes
		class RelationGroup : List<Relation> {
			readonly string name;
			readonly MasterDetailEditorControl parentControl;
			readonly HoverImageButton addRelationButton;
			readonly LabelControl labelGroupName;
			public LabelControl LabelGroupName {
				get {
					return this.labelGroupName;
				}
			}
			public HoverImageButton AddRelationButton {
				get {
					return this.addRelationButton;
				}
			}
			public string Name {
				get {
					return this.name;
				}
			}
			public bool IsValid {
				get {
					return this.parentControl.ObjectNames.Any(n => n.Key == this.name);
				}
			}
			public bool CanAddRelations {
				get {
					if(IsValid)
						return AvailableDetailTables.Count() > 0;
					return false;
				}
			}
			IEnumerable<string> AvailableDetailTables {
				get {
					return this.parentControl.ObjectNames.Select(n => n.Key).Where(p => p != Name);
				}
			}
			public RelationGroup(MasterDetailEditorControl parentControl, string name) {
				this.name = name;
				this.parentControl = parentControl;
				this.labelGroupName = parentControl.CreateLabelControl(name);
				this.addRelationButton = parentControl.CreateAddButton();
				this.addRelationButton.ToolTip = DataAccessUILocalizer.GetString(DataAccessUIStringId.MasterDetailEditorAddRelationMessage);
				this.addRelationButton.MouseClick += addRelationButton_MouseClick;
				this.labelGroupName.ForeColor = CommonSkins.GetSkin(parentControl.LookAndFeel).Colors.GetColor(IsValid ? "WindowText" : "Critical");
			}
			public void RemoveFromList(List<RelationGroup> groups) {
				groups.Remove(this);
				this.parentControl.RemoveControl(this.addRelationButton);
				this.parentControl.RemoveControl(this.labelGroupName);
			}
			void RefreshDetailQueriesMenu(PopupMenu menu) {
				List<BarItem> deletedItems = new List<BarItem>(menu.ItemLinks.Count);
				foreach(BarItemLink link in menu.ItemLinks) {
					link.Item.ItemClick -= PopupMenuDetailQueries_ItemClick;
					deletedItems.Add(link.Item);
				}
				deletedItems.ForEach(item => this.parentControl.CurrentBarManager.Items.Remove(item));
				menu.ClearLinks();
				foreach(string queryName in AvailableDetailTables) {
					BarCheckItem newItem = new BarCheckItem(this.parentControl.CurrentBarManager, false);
					newItem.Name = queryName;
					newItem.ItemClick += PopupMenuDetailQueries_ItemClick;
					List<MasterDetailInfo> details = MasterDetailEditorHelper.GetRelationsBetweenQueries(this.parentControl.DataSource, Name, queryName);
					if(details.Count > 0) {
						newItem.Tag = details;
						newItem.AllowHtmlText = DefaultBoolean.True;
						newItem.Caption = string.Format("<B>{0}</B>", queryName);
					} else {
						newItem.Caption = queryName;
					}
					menu.AddItem(newItem);
				}
			}
			void addRelationButton_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) {
				RefreshDetailQueriesMenu(this.parentControl.PopupMenuDetailQueries);
				this.parentControl.PopupMenuDetailQueries.ShowPopup(this.addRelationButton.PointToScreen(e.Location));
			}
			void PopupMenuDetailQueries_ItemClick(object sender, ItemClickEventArgs e) {
				List<MasterDetailInfo> masterDetails = e.Item.Tag as List<MasterDetailInfo> ?? new List<MasterDetailInfo>();
				if(masterDetails.Count == 0)
					masterDetails.Add(new MasterDetailInfo(Name, e.Item.Name, new RelationColumnInfo()));
				this.parentControl.CreateRelationsByMasterDetailInfo(masterDetails);
				this.parentControl.AlignItems();
			}
		}
		class Relation : List<ConditionControl> {
			readonly string masterTableName, detailTableName;
			readonly MasterDetailEditorControl parentControl;
			readonly HoverImageButton addConditionButton;
			readonly HoverImageButton removeRelationButton;
			readonly LabelControl labelRelationName;
			string relationName;
			public event EventHandler RemoveButtonClick;
			public string MasterTableName {
				get {
					return this.masterTableName;
				}
			}
			public string DetailTableName {
				get {
					return this.detailTableName;
				}
			}
			public LabelControl LabelRelationName {
				get {
					return this.labelRelationName;
				}
			}
			public HoverImageButton AddConditionButton {
				get {
					return this.addConditionButton;
				}
			}
			public HoverImageButton RemoveRelationButton {
				get {
					return this.removeRelationButton;
				}
			}
			public string Name {
				get {
					if(this.relationName != null)
						return this.relationName;
					return string.Format("{0}{1}", this.masterTableName, this.detailTableName);
				}
				set {
					if(this.relationName == value)
						return;
					this.relationName = value;
					this.labelRelationName.Text = Name;
				}
			}
			public bool HasCustomName {
				get {
					return this.relationName != null;
				}
			}
			public bool IsValid {
				get {
					return this.parentControl.ObjectNames.Any(n => n.Key == this.masterTableName) && this.parentControl.ObjectNames.Any(n => n.Key == this.detailTableName);
				}
			}
			public Relation(MasterDetailEditorControl parentControl, string masterTableName, string detailTableName) {
				this.masterTableName = masterTableName;
				this.detailTableName = detailTableName;
				this.parentControl = parentControl;
				this.labelRelationName = parentControl.CreateLabelControl(Name);
				this.labelRelationName.Click += labelRelationName_Click;
				if(IsValid) {
					this.addConditionButton = parentControl.CreateAddButton();
					this.addConditionButton.ToolTip = DataAccessUILocalizer.GetString(DataAccessUIStringId.MasterDetailEditorAddConditionMessage);
					this.addConditionButton.Click += addItemButton_Click;
				}
				this.labelRelationName.ForeColor = CommonSkins.GetSkin(parentControl.LookAndFeel).Colors.GetColor(IsValid ? "WindowText" : "Critical");
				this.removeRelationButton = parentControl.CreateRemoveButton();
				this.removeRelationButton.ToolTip = DataAccessUILocalizer.GetString(DataAccessUIStringId.MasterDetailEditorRemoveRelationMessage);
				this.removeRelationButton.Click += removeRelationButton_Click;
			}
			public Relation(MasterDetailEditorControl parentControl, string masterTableName, string detailTableName, string relationName)
				: this(parentControl, masterTableName, detailTableName) {
				Name = relationName;
			}
			public void RemoveFromGroup(RelationGroup group) {
				group.Remove(this);
				if(this.addConditionButton != null)
					this.parentControl.RemoveControl(this.addConditionButton);
				this.parentControl.RemoveControl(this.removeRelationButton);
				this.parentControl.RemoveControl(this.labelRelationName);
			}
			void addItemButton_Click(object sender, EventArgs e) {
				ConditionControl control = this.parentControl.CreateEmptyCondition();
				control.LeftTableName = MasterTableName;
				control.RightTableName = DetailTableName;
				Add(control);
				this.parentControl.AlignItems();
			}
			void removeRelationButton_Click(object sender, EventArgs e) {
				if(RemoveButtonClick != null)
					RemoveButtonClick(this, EventArgs.Empty);
			}
			void labelRelationName_Click(object sender, EventArgs e) {
				this.parentControl.ShowInplaceEditor(this);
			}
		}
		#endregion
		const int leftGroupMargin = 25;
		const int verticalGroupSpacing = 3;
		readonly List<RelationGroup> groups = new List<RelationGroup>(); 
		readonly Dictionary<string, List<string>> objectNames = new Dictionary<string, List<string>>();
		SqlDataSource dataSource;
		IEnumerable<MasterDetailInfo> relations;
		PopupMenu popupMenuDetailQueries;
		BaseEdit relationNameInplaceEditor;
		Relation relationRenaming;
		public SqlDataSource DataSource {
			get {
				return this.dataSource;
			}
			set {
				if(this.dataSource == value)
					return;
				this.dataSource = value;
				CreateObjectNames();
				CreateItems();
				AlignItems();
			}
		}
		public IEnumerable<MasterDetailInfo> Relations {
			get {
				return this.relations;
			}
		}
		internal Dictionary<string, List<string>> ObjectNames {
			get {
				return this.objectNames;
			}
		}
		internal PopupMenu PopupMenuDetailQueries {
			get {
				return this.popupMenuDetailQueries;
			}
		}
		internal BarManager CurrentBarManager {
			get {
				return this.barManager;
			}
		}
		internal bool RelationNameInplaceEditorIsShown {
			get {
				return this.relationRenaming != null;
			}
		}
		public MasterDetailEditorControl() : base() {
			this.panelControls.MouseClick += scrollableControl_MouseDown;
			this.panelControls.ControlAdded += scrollableControl_ControlAdded;
			this.panelControls.ControlRemoved += scrollableControl_ControlRemoved;
			this.popupMenuDetailQueries = CreatePopupMenu();
			RepositoryItemTextEdit item = new RepositoryItemTextEdit();
			this.relationNameInplaceEditor = item.CreateEditor();
			item.AllowFocused = true;
			item.ReadOnly = false;
			item.AllowNullInput = DefaultBoolean.False;
			item.ValidateOnEnterKey = false;
			this.relationNameInplaceEditor.Properties.Assign(item);
			item.Tag = this.relationNameInplaceEditor;
			this.relationNameInplaceEditor.Visible = false;
			this.panelControls.Controls.Add(this.relationNameInplaceEditor);
			this.relationNameInplaceEditor.Properties.Appearance.ForeColor = CommonSkins.GetSkin(LookAndFeel).Colors.GetColor("WindowText");
			this.relationNameInplaceEditor.CausesValidation = false;
			this.relationNameInplaceEditor.KeyDown += relationNameInplaceEditor_KeyDown;
			this.relationNameInplaceEditor.Leave += relationNameInplaceEditor_Leave;
		}
		public void CreateRelations() {
			List<MasterDetailInfo> masterDetails = new List<MasterDetailInfo>();
			foreach(RelationGroup group in this.groups) {
				foreach(Relation relation in group) {
					MasterDetailInfo masterDetail = new MasterDetailInfo(relation.MasterTableName, relation.DetailTableName);
					foreach(ConditionControl condition in relation) {
						if(string.IsNullOrWhiteSpace(condition.RightTableName) || string.IsNullOrWhiteSpace(condition.RightColumnName) || string.IsNullOrWhiteSpace(condition.LeftTableName) || string.IsNullOrWhiteSpace(condition.LeftColumnName))
							throw new IncompleteConditionException(DataAccessUILocalizer.GetString(DataAccessUIStringId.JoinEditorFillAllFieldsException));
						masterDetail.KeyColumns.Add(new RelationColumnInfo(condition.LeftColumnName, condition.RightColumnName));
					}
					if(relation.HasCustomName)
						masterDetail.Name = relation.Name;
					masterDetails.Add(masterDetail);
				}
			}
			this.relations = masterDetails;
			try {
				MasterDetailEditorHelper.ValidateRelations(DataSource, this.relations);
			} catch(RelationException e) {
				string confirmation = string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.MasterDetailEditorColumnsHasDifferentTypesConfirmation), e.MasterTable, e.MasterColumn, e.MasterType, e.DetailTable, e.DetailColumn, e.DetailType);
				if(XtraMessageBox.Show(LookAndFeel, this, confirmation, DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxWarningTitle), MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
					throw;
			}
		}
		protected internal override void AlignItems() {
			RemoveEmptyGroups();
			int posY = topMargin;
			int buttonAlignCorrection = 0;
			foreach(RelationGroup group in this.groups) {
				const int posX = leftMargin;
				group.LabelGroupName.Left = posX;
				group.LabelGroupName.Top = posY;
				posY += group.LabelGroupName.Height + verticalGroupSpacing;
				foreach(Relation relation in group) {
					buttonAlignCorrection = (int)Math.Floor(Convert.ToDecimal(relation.LabelRelationName.Height - relation.RemoveRelationButton.Height) / 2);
					relation.RemoveRelationButton.Left = posX;
					relation.RemoveRelationButton.Top = posY + buttonAlignCorrection;
					int relationPosX = posX + relation.RemoveRelationButton.Width + horizontalSpacing;
					relation.LabelRelationName.Left = relationPosX;
					relation.LabelRelationName.Top = posY;
					posY += relation.LabelRelationName.Height + verticalSpacing;
					foreach(ConditionControl control in relation) {
						control.Top = posY;
						control.Left = relationPosX;
						posY += control.Height + verticalSpacing;
					}
					if(relation.AddConditionButton != null) {
						relation.AddConditionButton.Left = relationPosX;
						relation.AddConditionButton.Top = posY + buttonAlignCorrection;
						posY += relation.AddConditionButton.Height + verticalSpacing;
					}
				}
				bool canAddRelations = group.CanAddRelations;
				if(canAddRelations) {
					group.AddRelationButton.Left = posX;
					group.AddRelationButton.Top = posY;
					posY += group.AddRelationButton.Height + verticalGroupSpacing;
				}
				group.AddRelationButton.Visible = canAddRelations;
			}
		}
		internal void CloseInplaceEditor() {
			this.relationRenaming = null;
			this.relationNameInplaceEditor.Hide();
		}
		protected override void CreateBitmaps(Color operatorColor, Color foreColor) {
			foreach(Image[] images in this.conditionOperationsImages.Values) {
				DisposeImageNotNull(ref images[0]);
				DisposeImageNotNull(ref images[1]);
			}
			this.conditionOperationsImages.Clear();
			this.conditionOperationsImages.Add(BinaryOperatorType.Equal, new Image[2]);
			this.conditionOperationsImages[BinaryOperatorType.Equal][0] = ImageHelper.ColorBitmap(ImageHelper.GetImage("Equals"), foreColor);
			foreach(Image[] images in this.conditionOperationsImages.Values)
				images[1] = ImageHelper.ColorBitmap(images[0], operatorColor);
		}
		protected override void CreateObjectNames() {
			this.objectNames.Clear();
			foreach(ResultTable table in DataSource.ResultSet.Tables)
				this.objectNames.Add(table.TableName, table.Columns.Select(column => column.Name).ToList());
		}
		protected override void CreateItems() {
			foreach(string queryName in this.objectNames.Keys) {
				RelationGroup currentGroup = new RelationGroup(this, queryName);
				this.groups.Add(currentGroup);
			}
			CreateRelationsByMasterDetailInfo(DataSource.Relations);
		}
		protected override void UpdateSkinColors() {
			base.UpdateSkinColors();
			foreach(RelationGroup group in this.groups)
				foreach(Relation relation in group)
					foreach(ConditionControl condition in relation) {
						condition.LookAndFeel.ParentLookAndFeel = LookAndFeel;
						condition.Images = this.conditionOperationsImages;
					}
			if(this.relationNameInplaceEditor != null)
				this.relationNameInplaceEditor.Properties.Appearance.ForeColor = CommonSkins.GetSkin(LookAndFeel).Colors.GetColor("WindowText");
		}
		HoverImageButton CreateHoverImageButton(Image hover, Image normal) {
			HoverImageButton button = new HoverImageButton(hover, normal);
			this.panelControls.Controls.Add(button);
			return button;
		}
		HoverImageButton CreateAddButton() {
			return CreateHoverImageButton(this.addHover, this.addNormal);
		}
		HoverImageButton CreateRemoveButton() {
			return CreateHoverImageButton(this.removeHover, this.removeNormal);
		}
		LabelControl CreateLabelControl(string text) {
			LabelControl labelControl = new LabelControl();
			labelControl.Text = text;
			labelControl.ForeColor = CommonSkins.GetSkin(LookAndFeel).Colors.GetColor("WindowText");
			this.panelControls.Controls.Add(labelControl);
			return labelControl;
		}
		ConditionControl CreateEmptyCondition() {
			ConditionControl condition = new ConditionControl(this.barManager, this.conditionOperationsImages, this.removeNormal, this.removeHover);
			condition.LeftObjectNames = this.objectNames;
			condition.RightObjectNames = this.objectNames;
			condition.AllowChangeOperatorType = false;
			condition.AllowChangeMasterTable = true;
			condition.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			condition.RemoveButtonClick += condition_RemoveButtonClick;
			condition.LeftTableNameChanged += condition_LeftTableNameChanged;
			condition.RightTableNameChanged += condition_RightTableNameChanged;
			this.panelControls.Controls.Add(condition);
			return condition;
		}
		void RemoveControl(Control control) {
			this.panelControls.Controls.Remove(control);
		}
		void ShowInplaceEditor(Relation relation) {
			if(this.relationRenaming == null) {
				Rectangle rectangle = relation.LabelRelationName.Bounds;
				rectangle.Y -= (this.relationNameInplaceEditor.Height - relation.LabelRelationName.Height) / 2;
				rectangle.Width = Math.Max(rectangle.Width + 20, Width - rectangle.X - 25);
				this.relationNameInplaceEditor.Bounds = rectangle;
				this.relationNameInplaceEditor.EditValue = relation.Name;
				this.relationNameInplaceEditor.Focus();
				this.relationNameInplaceEditor.BringToFront();
				this.relationNameInplaceEditor.Show();
				this.relationRenaming = relation;
			}
		}
		void RemoveEmptyGroups() {
			List<RelationGroup> groupsToRemove = this.groups.Where(g => g.Count == 0 && !this.objectNames.Any(n => n.Key == g.Name)).ToList();
			if(groupsToRemove.Count > 0)
				foreach(RelationGroup group in groupsToRemove)
					group.RemoveFromList(this.groups);
		}
		void CreateRelationsByMasterDetailInfo(IEnumerable<MasterDetailInfo> masterDetails) {
			foreach(MasterDetailInfo masterDetail in masterDetails) {
				RelationGroup currentGroup = this.groups.FirstOrDefault(g => g.Name == masterDetail.MasterQueryName);
				if(currentGroup == null) {
					currentGroup = new RelationGroup(this, masterDetail.MasterQueryName);
					this.groups.Add(currentGroup);
				}
				Relation newRelation = null;
				if(masterDetail.HasCustomName || currentGroup.Any(r => r.MasterTableName == masterDetail.MasterQueryName && r.DetailTableName == masterDetail.DetailQueryName)) {
					string name = masterDetail.Name;
					if(currentGroup.Any(r => r.Name == name)) {
						PrefixNameGenerator prefixNameGenerator = new PrefixNameGenerator(name, "_", 1);
						name = prefixNameGenerator.GenerateName(n => currentGroup.Any(r => r.Name == n));
					}
					newRelation = new Relation(this, masterDetail.MasterQueryName, masterDetail.DetailQueryName, name);
				} else {
					newRelation = new Relation(this, masterDetail.MasterQueryName, masterDetail.DetailQueryName);
				}
				newRelation.RemoveButtonClick += relation_RemoveButtonClick;
				currentGroup.Add(newRelation);
				foreach(RelationColumnInfo columns in masterDetail.KeyColumns) {
					ConditionControl condition = CreateEmptyCondition();
					condition.LeftTableName = masterDetail.MasterQueryName;
					condition.LeftColumnName = columns.ParentKeyColumn;
					condition.RightTableName = masterDetail.DetailQueryName;
					condition.RightColumnName = columns.NestedKeyColumn;
					newRelation.Add(condition);
				}
			}
		}
		void condition_LeftTableNameChanged(object sender, TableNameChangedEventArgs e) {
			ConditionControl control = (ConditionControl)sender;
			RelationGroup newGroup = this.groups.Find(g => g.Name == control.LeftTableName);
			RelationGroup oldGroup = this.groups.Find(g => g.Name == e.OldName);
			Relation oldRelation = oldGroup.First(r => r.MasterTableName == e.OldName && r.DetailTableName == control.RightTableName);
			oldRelation.Remove(control);
			if(oldRelation.Count == 0)
				oldRelation.RemoveFromGroup(oldGroup);
			Relation newRelation = newGroup.FirstOrDefault(r => r.MasterTableName == control.LeftTableName && r.DetailTableName == control.RightTableName);
			if(newRelation == null) {
				newRelation = new Relation(this, control.LeftTableName, control.RightTableName);
				newRelation.RemoveButtonClick += relation_RemoveButtonClick;
				newGroup.Add(newRelation);
			}
			newRelation.Add(control);
			AlignItems();
		}
		void condition_RightTableNameChanged(object sender, TableNameChangedEventArgs e) {
			ConditionControl control = (ConditionControl)sender;
			RelationGroup group = this.groups.Find(g => g.Name == control.LeftTableName);
			Relation oldRelation = group.First(r => r.MasterTableName == control.LeftTableName && r.DetailTableName == e.OldName);
			oldRelation.Remove(control);
			if(oldRelation.Count == 0)
				oldRelation.RemoveFromGroup(group);
			Relation newRelation = group.FirstOrDefault(r => r.MasterTableName == control.LeftTableName && r.DetailTableName == control.RightTableName);
			if(newRelation == null) {
				newRelation = new Relation(this, control.LeftTableName, control.RightTableName);
				newRelation.RemoveButtonClick += relation_RemoveButtonClick;
				group.Add(newRelation);
			}
			newRelation.Add(control);
			AlignItems();
		}
		void condition_RemoveButtonClick(object sender, EventArgs e) {
			ConditionControl condition = sender as ConditionControl;
			if(condition != null) {
				RelationGroup group = this.groups.Find(g => g.Any(r => r.Contains(condition)));
				Relation relation = group.Find(r => r.Contains(condition));
				relation.Remove(condition);
				if(relation.Count == 0)
					relation.RemoveFromGroup(group);
				this.panelControls.Controls.Remove(condition);
				condition.Dispose();
				AlignItems();
			}
		}
		void relation_RemoveButtonClick(object sender, EventArgs e) {
			Relation relation = sender as Relation;
			if(relation != null) {
				RelationGroup group = this.groups.Find(g => g.Contains(relation));
				List<ConditionControl> conditions = relation.ToList();
				foreach(ConditionControl condition in conditions) {
					relation.Remove(condition);
					condition.Dispose();
				}
				relation.RemoveFromGroup(group);
				AlignItems();
			}
		}
		void scrollableControl_ControlRemoved(object sender, ControlEventArgs e) {
			e.Control.MouseDown -= scrollableControl_MouseDown;
			ConditionControl condition = e.Control as ConditionControl;
			if(condition != null)
				condition.UnderlineControlMouseDown -= scrollableControl_MouseDown;
		}
		void scrollableControl_ControlAdded(object sender, ControlEventArgs e) {
			e.Control.MouseDown += scrollableControl_MouseDown;
			ConditionControl condition = e.Control as ConditionControl;
			if(condition != null)
				condition.UnderlineControlMouseDown += scrollableControl_MouseDown;
		}
		void scrollableControl_MouseDown(object sender, MouseEventArgs e) {
			if(this.relationRenaming != null)
				CloseInplaceEditor();
		}
		void relationNameInplaceEditor_Leave(object sender, EventArgs e) {
			TryApplyRenaming();
		}
		void relationNameInplaceEditor_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter) {
				TryApplyRenaming();
			}
		}
		void TryApplyRenaming() {
			if(this.relationRenaming == null)
				return;
			string newRelationName = (string)this.relationNameInplaceEditor.EditValue;
			RelationGroup group = this.groups.Find(g => g.Name == this.relationRenaming.MasterTableName);
			if(group.Where(r => r != this.relationRenaming).Any(r => r.Name == newRelationName)) {
				string message = DataAccessUILocalizer.GetString(DataAccessUIStringId.MasterDetailEditorInvalidRelationNameMessage);
				this.relationNameInplaceEditor.Focus();
				XtraMessageBox.Show(LookAndFeel, this, message, DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxWarningTitle), MessageBoxButtons.OK, MessageBoxIcon.Information);
			} else {
				this.relationRenaming.Name = newRelationName;
				CloseInplaceEditor();
				this.Focus();
			}
		}
	}
}
