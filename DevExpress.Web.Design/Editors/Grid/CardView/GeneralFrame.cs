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
using System.Text;
using System.Windows.Forms;
using DevExpress.Web.ASPxHtmlEditor.Design;
namespace DevExpress.Web.Design {
	public class CardViewGeneralFrame : EditFrameBase {
		Size MinimumFrameSize = new Size(650, 520);
		VisualElementsCheckPropertiesModel visualElementsModelProperties;
		CommandButtonsCheckPropertiesModel commandButtonsModelProperties;
		DataSecurityCheckPropertiesModel dataSecurityModelProperties;
		ASPxCardView cardView;
		public CardViewGeneralFrame()
			: base() {
			MinimumSize = MinimumFrameSize;
			Load += (s, e) => {
				Dock = DockStyle.Fill;
				CreateLayout();
			};
		}
		protected override string FrameName { get { return "CardViewGeneralFrame"; } }
		CheckPropertiesDescriptor VisualElementsCheckProperties { get; set; }
		CheckPropertiesDescriptor CommandButtonsCheckProperties { get; set; }
		CheckPropertiesDescriptor DataSecurityCheckProperties { get; set; }
		ASPxCardView CardView {
			get {
				if(cardView == null)
					cardView = (ASPxCardView)EditingObject;
				return cardView;
			}
		}
		VisualElementsCheckPropertiesModel VisualElementsModelProperties {
			get {
				if(visualElementsModelProperties == null)
					visualElementsModelProperties = new VisualElementsCheckPropertiesModel(CardView);
				return visualElementsModelProperties;
			}
		}
		CommandButtonsCheckPropertiesModel CommandButtonsModelProperties {
			get {
				if(commandButtonsModelProperties == null)
					commandButtonsModelProperties = new CommandButtonsCheckPropertiesModel(CardView);
				return commandButtonsModelProperties;
			}
		}
		DataSecurityCheckPropertiesModel DataSecurityModelProperties {
			get {
				if(dataSecurityModelProperties == null)
					dataSecurityModelProperties = new DataSecurityCheckPropertiesModel(CardView);
				return dataSecurityModelProperties;
			}
		}
		void CreateLayout() {
			SuspendLayout();
			VisualElementsCheckProperties = new CheckPropertiesDescriptor(VisualElementsModelProperties) { CheckGeneratorType = CheckTableGeneratorType.FillByColumnIndex };
			VisualElementsCheckProperties.Location = new Point(0, 0);
			VisualElementsCheckProperties.Create(MainPanel, true);
			CommandButtonsCheckProperties = new CheckPropertiesDescriptor(CommandButtonsModelProperties) { RowCount = 1 };
			CommandButtonsCheckProperties.Parent = MainPanel;
			CommandButtonsCheckProperties.Location = new Point(0, VisualElementsCheckProperties.Bottom + 10);
			CommandButtonsCheckProperties.Create(MainPanel, true);
			DataSecurityCheckProperties = new CheckPropertiesDescriptor(DataSecurityModelProperties) { RowCount = 1 };
			DataSecurityCheckProperties.Parent = MainPanel;
			DataSecurityCheckProperties.Location = new Point(0, CommandButtonsCheckProperties.Bottom + 10);
			DataSecurityCheckProperties.Create(MainPanel, true);
			ResumeLayout();
		}
	}
	public class VisualElementsCheckPropertiesModel : CardViewCheckPropertiesModel {
		public VisualElementsCheckPropertiesModel(ASPxCardView cardView)
			: base(cardView, "Visual") {
		}
		protected override void FillTabsCheckBoxGroup() {
			SettingsCheckGroup.Add(CardView, "Pager", "SettingsPager.AlwaysShowPager", 0);
			SettingsCheckGroup.Add(CardView, "Search Panel", "SettingsSearchPanel.Visible", 0);
			SettingsCheckGroup.Add(CardView, "Select Check Box", GetShowSelectedCheckBox, SetShowSelectedCheckBox, 0);
			SettingsCheckGroup.Add(CardView, "Header Panel", "Settings.ShowHeaderPanel", 1);
			SettingsCheckGroup.Add(CardView, "Summary Panel", "Settings.ShowSummaryPanel", 1);
		}
		object GetShowSelectedCheckBox() { return ActionList != null ? ActionList.ShowSelectCheckBox : false; }
		void SetShowSelectedCheckBox(object value) { ActionList.ShowSelectCheckBox = Convert.ToBoolean(value); }
	}
	public class CommandButtonsCheckPropertiesModel : CardViewCheckPropertiesModel {
		public CommandButtonsCheckPropertiesModel(ASPxCardView cardView)
			: base(cardView, "Command Buttons") {
		}
		protected override void FillTabsCheckBoxGroup() {
			SettingsCheckGroup.Add(CardView, "Edit Button", GetShowEditButton, SetShowEditButton, 1);
			SettingsCheckGroup.Add(CardView, "New Button", GetShowNewButton, SetShowNewButton, 1);
			SettingsCheckGroup.Add(CardView, "Delete Button", GetShowDeleteButton, SetShowDeleteButton, 1);
		}
		object GetShowEditButton() { return ActionList != null ? ActionList.ShowEditButton : false; }
		void SetShowEditButton(object value) { ActionList.ShowEditButton = Convert.ToBoolean(value); }
		object GetShowNewButton() { return ActionList != null ? ActionList.ShowNewButton : false; }
		void SetShowNewButton(object value) { ActionList.ShowNewButton = Convert.ToBoolean(value); }
		object GetShowDeleteButton() { return ActionList != null ? ActionList.ShowDeleteButton : false; }
		void SetShowDeleteButton(object value) { ActionList.ShowDeleteButton = Convert.ToBoolean(value); }
	}
	public class DataSecurityCheckPropertiesModel : CardViewCheckPropertiesModel {
		public DataSecurityCheckPropertiesModel(ASPxCardView cardView)
			: base(cardView, "Data Security") {
		}
		protected override void FillTabsCheckBoxGroup() {
			SettingsCheckGroup.Add(CardView, "Allow Edit", "SettingsDataSecurity.AllowEdit", 2);
			SettingsCheckGroup.Add(CardView, "Allow Insert", "SettingsDataSecurity.AllowInsert", 2);
			SettingsCheckGroup.Add(CardView, "Allow Delete", "SettingsDataSecurity.AllowDelete", 2);
		}
	}
	public abstract class CardViewCheckPropertiesModel : CheckPropertiesModel {
		ASPxCardView cardView;
		CardViewDesignerActionList actionList;
		public CardViewCheckPropertiesModel(ASPxCardView cardView, string caption)
			: base(cardView, caption) {
		}
		protected CardViewDesignerActionList ActionList {
			get {
				if(actionList == null) {
					var designer = CommonDesignerServiceRegisterHelper.GetWebControlDesigner(CardView.Site, CardView.ID) as CardViewDesigner;
					if(designer != null)
						actionList = designer.ActionList as CardViewDesignerActionList;
				}
				return actionList;
			}
		}
		protected ASPxCardView CardView {
			get {
				if(cardView == null)
					cardView = (ASPxCardView)WebControl;
				return cardView;
			}
		}
	}
}
