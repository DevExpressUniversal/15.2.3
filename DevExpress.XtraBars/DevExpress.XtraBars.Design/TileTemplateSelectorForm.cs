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

using DevExpress.Skins;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Design {
	public partial class TileTemplateSelectorForm : XtraForm {
		public TileTemplateSelectorForm(ITileItem targetItem) {
			InitializeComponent();
			SkinManager.EnableFormSkins();
			DevExpress.Utils.Design.WindowsFormsDesignTimeSettings.ApplyDesignSettings(this);
			TargetItem = targetItem;
			PrepareTileControl();
		}
		string[] resources = null;
		public TileItem Item;
		public ITileItem TargetItem;
		List<TemplateGroup> templateGroups = new List<TemplateGroup>() 
		{
			new TemplateGroup(){ Description = "Medium", FolderName = "Medium" },
			new TemplateGroup(){ Description = "Medium with animation", FolderName = "MediumPeek" },
			new TemplateGroup(){ Description = "Wide", FolderName = "Wide" },
			new TemplateGroup(){ Description = "Wide with animation", FolderName = "WidePeek"}
		};
		void PrepareTileControl() {
			resources = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
			tileControl.BeginUpdate();
			foreach(TemplateGroup templateGroup in templateGroups) {
				CreateGroup(templateGroup.Description, templateGroup.FolderName);
			}
			AssignTileControlAppearance();
			tileControl.EndUpdate();
		}
		void AssignTileControlAppearance() {
			if(TargetItem != null && TargetItem.Control != null) {
				tileControl.BackColor = TargetItem.Control.BackColor;
				tileControl.BackgroundImage = TargetItem.Control.BackgroundImage;
				tileControl.BackgroundImageLayout = TargetItem.Control.Properties.BackgroundImageLayout;
				tileControl.AppearanceGroupText.ForeColor = tileControl.BackColor.GetBrightness() < 0.5f ? Color.White : Color.Black;
				tileControl.AppearanceItem.Normal.Assign(TargetItem.Control.AppearanceItem.Normal);
				tileControl.AppearanceItem.Pressed.Assign(TargetItem.Control.AppearanceItem.Pressed);
				foreach(TileGroup group in tileControl.Groups) {
					foreach(TileItem item in group.Items) {
						item.AppearanceItem.Normal.Assign(TargetItem.Appearances.Normal);
						item.AppearanceItem.Pressed.Assign(TargetItem.Appearances.Pressed);
					}
				}
			}
		}
		void CreateGroup(string description, string folderName) {
			TileGroup group = new TileGroup();
			group.Text = description;
			tileControl.Groups.Add(group);
			Proceed(folderName, group);
		}
		const string path = "DevExpress.XtraBars.Design.TileControlItemTemplates.";
		void Proceed(string name, TileGroup group) {
			if(resources == null)
				return;
			string fullPath = path + name + ".";
			foreach(string resName in resources) {
				if(resName.StartsWith(fullPath)) {
					TileItem item = new TileItem();
					group.Items.Add(item);
					SetTemplateToItem(item, resName);
					item.Tag = resName;
				}
			}
		}
		void SetTemplateToItem(ITileItem item, string templateFilePath) {
			TileControlTemplateHelper helper = new TileControlTemplateHelper(item);
			Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(templateFilePath);
			helper.SetTemplate(helper.LoadTemplate(stream));
			helper = null;
		}
		private void tileControl1_ItemClick(object sender, TileItemEventArgs e) {
			e.Item.Checked = true;
			if(e.Item.Checked) Item = e.Item;
		}
		private void tileControl1_ItemCheckedChanged(object sender, TileItemEventArgs e) {
			btnOk.Enabled = e.Item.Checked;
		}
		private void btnOk_Click(object sender, EventArgs e) {
			ApplyTemplate();
		}
		void ApplyTemplate() {
			if(TargetItem == null | Item == null)
				return;
			SetTemplateToItem(TargetItem, Item.Tag.ToString());
		}
	}
	class TemplateGroup {
		public string Description { get; set; }
		public string FolderName { get; set; }
	}
}
