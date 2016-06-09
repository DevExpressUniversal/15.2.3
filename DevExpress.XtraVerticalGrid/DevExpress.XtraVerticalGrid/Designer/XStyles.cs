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
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Persistent;
using DevExpress.XtraVerticalGrid;
using System.IO;
namespace DevExpress.XtraVerticalGrid.Design {
	public class XViews {
		VGridControlBase vGrid;
		[Obsolete("Use the ConfigureDemoView(VGridControlBase verticalGrid) method instead")]
		public XViews(VGridControlBase vg) {
			ConfigureDemoView(vg);
		}
		protected XViews() { }
		protected VGridControlBase VGrid {
			get { return vGrid; }
			set {
				if(vGrid != null)
					return;
				vGrid = value;
				OnVGridChanged();
			}
		}
		public static XViews ConfigureDemoView(VGridControlBase verticalGrid) {
			XViews xv = new XViews();
			xv.VGrid = verticalGrid;
			return xv;
		}
		protected void OnVGridChanged() {
			if(VGrid != null) {
				InitLayout();
				AdjustmentEditors();
				InitData();
				InitRowImages();
			}
		}
		void InitLayout() {
			System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraVerticalGrid.Designer.layout.xml");
			vGrid.RestoreLayoutFromStream(stream);
		}
		void InitData() {
			DataSet ds = new DataSet();
			System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraVerticalGrid.Designer.Employees.xml");
			ds.ReadXml(stream);
			stream.Close();
			if(vGrid is PropertyGridControl)
				((PropertyGridControl)vGrid).SelectedObject = ds.Tables[0].DefaultView[0];
			else
				((VGridControl)vGrid).DataSource = ds.Tables[0];
		}
		void InitRowImages() {
			vGrid.ImageList = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraVerticalGrid.Designer.RowImages.png", typeof(XViews).Assembly, new Size(16, 16));
			vGrid.Rows[0].Properties.ImageIndex = 0;
			vGrid.Rows[1].ChildRows[0].Properties.ImageIndex = 1;
			vGrid.Rows[2].ChildRows[1].Properties.ImageIndex = 2;
		}
		void AdjustmentEditors() {
			RepositoryItemDateEdit de = new RepositoryItemDateEdit();
			vGrid.RepositoryItems.Add(de);
			RepositoryItemPictureEdit pe = new RepositoryItemPictureEdit();
			vGrid.RepositoryItems.Add(pe);
			pe.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
			RepositoryItemImageComboBox pi = new RepositoryItemImageComboBox();
			vGrid.RepositoryItems.Add(pi);
			pi.SmallImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraVerticalGrid.Designer.TitleOfCourtesy.png", typeof(XViews).Assembly, new Size(16, 16));
			string[] s = new string[]{"Dr.", "Mr.", "Ms.", "Mrs."};
			for(int i = 0; i < s.Length; i++)
				pi.Items.Add(new ImageComboBoxItem(s[i], s[i], i));
			RepositoryItemComboBox cb1 = new RepositoryItemComboBox();
			vGrid.RepositoryItems.Add(cb1);
			s = new string[]{"AK", "BC", "CA", "DF", "ID", "MT", "NM", "OR", "RJ", "SP", "WA", "WY"};
			for(int i = 0; i < s.Length; i++)
				cb1.Items.Add(s[i]);
			RepositoryItemComboBox cb2 = new RepositoryItemComboBox();
			vGrid.RepositoryItems.Add(cb2);
			s = new string[]{"USA", "UK"};
			for(int i = 0; i < s.Length; i++)
				cb2.Items.Add(s[i]);
			RepositoryItemTextEdit te = new RepositoryItemTextEdit();
			vGrid.RepositoryItems.Add(te);
			te.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Simple;
			te.Mask.EditMask = "(999) 000-0000";
			RepositoryItemMemoExEdit me = new RepositoryItemMemoExEdit();
			vGrid.RepositoryItems.Add(me);
			vGrid.Rows[0].ChildRows[3].Properties.RowEdit = pi;
			vGrid.Rows[0].ChildRows[4].Properties.RowEdit = de;
			vGrid.Rows[0].ChildRows[5].Properties.RowEdit = de;
			vGrid.Rows[1].ChildRows[1].GetRowProperties(1).RowEdit = cb1;
			vGrid.Rows[1].ChildRows[2].GetRowProperties(1).RowEdit = cb2;
			vGrid.Rows[1].ChildRows[3].Properties.RowEdit = te;
			vGrid.Rows[2].ChildRows[0].Properties.RowEdit = pe;
			vGrid.Rows[2].ChildRows[1].Properties.RowEdit = me;
			vGrid.Rows[2].ChildRows[0].Height += 12;
		}
	}
	public class XStyles {	
		private System.Xml.XmlDocument xmlStyles = new System.Xml.XmlDocument();
		private System.Xml.XmlNode nStyles = null;
		private ArrayList styles = new ArrayList();
		private string DefStyle = "(Default)";
		public object[] Styles {
			get { return styles.ToArray(); }
		}
		private string FindFileName() {
			string styleFileName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.System) + "\\DevExpress.XtraVerticalGrid.Styles.xml";
			return styleFileName;
		} 
		public XStyles(string styleFileName) {
			if(styleFileName == "System") 
				styleFileName = FindFileName();
			styles.Add(DefStyle);
			if(System.IO.File.Exists(styleFileName)) {
				xmlStyles.Load(styleFileName);
				nStyles = xmlStyles.DocumentElement;
				CreateStyleNames(nStyles, styles);
			}
			else
				MessageBox.Show("File " + styleFileName + " is not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
		}
		private void CreateStyleNames(System.Xml.XmlNode node, ArrayList styles) {
			if(node.Name == "StyleName") {
				styles.Add(node.Attributes["name"].Value);
			}
			node = node.FirstChild;	
			while (node != null) {
				CreateStyleNames(node, styles);
				node = node.NextSibling;
			}
		}
		private void LoadSchemeRow(System.Xml.XmlNode node, VGridControlBase vGrid) {
			if(node.Name == "Style") {
				System.Xml.XmlNode anode = node.FirstChild;
				AppearanceObject appearance = vGrid.Appearance.GetAppearance(anode.Value.ToString());
				if(appearance == null) return;
				AppearanceDefault def = new AppearanceDefault();
				FontStyle fstyle = FontStyle.Regular;
				if(Convert.ToBoolean(node.Attributes["b"].Value))
					fstyle |= FontStyle.Bold;
				if(Convert.ToBoolean(node.Attributes["i"].Value))
					fstyle |= FontStyle.Italic;
				if(Convert.ToBoolean(node.Attributes["u"].Value))
					fstyle |= FontStyle.Underline;
				if(Convert.ToBoolean(node.Attributes["s"].Value))
					fstyle |= FontStyle.Strikeout;
				string FontName = node.Attributes["fontname"].Value;
				int FontSize = Convert.ToInt32(node.Attributes["fontsize"].Value);
				def.BackColor = Color.FromArgb((int)Convert.ToInt64("0x" + node.Attributes["backcolor"].Value, 16));
				def.ForeColor = Color.FromArgb((int)Convert.ToInt64("0x" + node.Attributes["forecolor"].Value, 16));
				if(CanSetEmptyBackColor2(appearance.Name))
					def.BackColor2 = Color.Empty;
				try {
					def.Font = new Font(FontName, FontSize, fstyle);
				} catch {
					def.Font = new Font("Arial", FontSize, fstyle);
				}
				appearance.Assign(def);
			}
			if(node.Name == "FocusedCellEnabled") 
				SetEnabledFocusedCell(vGrid, Convert.ToBoolean(node.FirstChild.Value));
		}
		private bool CanSetEmptyBackColor2(string appearanceName) {
			if(appearanceName == null) return false;
			if(appearanceName == "HorzLine" ||
				appearanceName == "VertLine" ||
				appearanceName == "Separator" ||
				appearanceName == "BandBorder")
				return false;
			return true;
		}
		private void SetEnabledFocusedCell(VGridControlBase vGrid, bool enabled) {
			AppearanceObject appearance = vGrid.Appearance.FocusedCell;
			appearance.Options.UseBackColor = enabled;
		}
		private void LoadSchemeMain(System.Xml.XmlNode node, string stylename, VGridControlBase vGrid) {
			if(node.Name == "StyleName" && node.Attributes["name"].Value == stylename) {
				node = node.FirstChild;
				while (node != null) {
					LoadSchemeRow(node, vGrid);
					node = node.NextSibling;
				}
				return;
			}
			node = node.FirstChild;	
			while (node != null) {
				LoadSchemeMain(node, stylename, vGrid);
				node = node.NextSibling;
			}
		}
		public void LoadScheme(System.Xml.XmlNode node, string stylename, VGridControlBase vGrid) {
			if(stylename == DefStyle) {
				vGrid.RestoreDefaultStyles();
				return;
			}
			if(node == null) node = nStyles;
			if(node == null) return;
			vGrid.BeginUpdate();
			LoadSchemeMain(node, stylename, vGrid);
			vGrid.EndUpdate();
		}
	}
	public class XAppearances {	
		private System.Xml.XmlDocument xmlFormats = new System.Xml.XmlDocument();
		private System.Xml.XmlNode nodeFormats = null;
		private ArrayList formatNames = new ArrayList();
		private string DefStyle = "(Default)";
		private bool isNew;
		public object[] FormatNames {
			get { return formatNames.ToArray(); }
		}
		private string FindFileName() {
			return System.Environment.GetFolderPath(System.Environment.SpecialFolder.System) + "\\DevExpress.XtraVerticalGrid.Appearances.xml";
		} 
		public bool ShowNewStylesOnly {
			get { return isNew;}
			set { 
				isNew = value;
				if(this.nodeFormats != null) {
					formatNames.Clear();
					formatNames.Add(DefStyle);
					CreateFormatNames(nodeFormats, formatNames, isNew);
				}
			}
		}
		void ShowError(string styleFileName) {
			MessageBox.Show(string.Format("File {0} is not found", styleFileName), DevExpress.XtraEditors.Controls.Localizer.Active.GetLocalizedString(DevExpress.XtraEditors.Controls.StringId.CaptionError), MessageBoxButtons.OK, MessageBoxIcon.Error); 
		}
		public XAppearances(string styleFileName) : this(styleFileName, false){}
		public XAppearances(string styleFileName, bool isNew) {
			if(styleFileName == "System") styleFileName = FindFileName();
			if(System.IO.File.Exists(styleFileName)) {
				xmlFormats.Load(styleFileName);
			} else {
				using(Stream xmlStream = AssemblyHelper.GetEmbeddedResourceStream(typeof(VGridControl).Assembly, "Blending.DevExpress.XtraVerticalGrid.Appearances.xml", false)) {
					xmlFormats.Load(xmlStream);
				}
			}
			nodeFormats = xmlFormats.DocumentElement;
			ShowNewStylesOnly = isNew;
		}
		private void CreateFormatNames(System.Xml.XmlNode node, ArrayList names, bool isNew) {
			string newName = isNew ? "New" : "";
			if(node.Name.IndexOf("StyleName" + newName) != -1) {
				names.Add(node.Attributes["name"].Value);
			}
			node = node.FirstChild;	
			while (node != null) {
				CreateFormatNames(node, names, isNew);
				node = node.NextSibling;
			}
		}
		public void LoadSchemeFromFile(string fileName, VGridControlBase vgrid) {
			if(!System.IO.File.Exists(fileName)) {
				ShowError(fileName);
				return;
			}
			System.Xml.XmlDocument xmlFFormats = new System.Xml.XmlDocument();
			try {
				xmlFFormats.Load(fileName);
			} catch {
				MessageBox.Show("Invalid File Format.", DevExpress.XtraEditors.Controls.Localizer.Active.GetLocalizedString(DevExpress.XtraEditors.Controls.StringId.CaptionError), MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if(vgrid != null) vgrid.Appearance.Reset();
			System.Xml.XmlNode node = xmlFFormats.DocumentElement;
			while(node != null) {
				if(node.Name.IndexOf("StyleName") != -1) {
					vgrid.BeginUpdate();
					node = node.FirstChild;
					while (node != null) {
						LoadSchemeRow(node, vgrid);
						node = node.NextSibling;
					}
					vgrid.EndUpdate();
					return;
				}
				node = node.NextSibling;
			}
		}
		public void LoadScheme(string formatName, VGridControlBase vgrid) {
			if(vgrid == null) return;
			vgrid.BeginUpdate();
			try {
				vgrid.Appearance.Reset();
				if(formatName != DefStyle && nodeFormats != null) 
					LoadScheme(nodeFormats, formatName, vgrid);
			} finally {
				vgrid.EndUpdate();
			}
		}
		private void LoadScheme(System.Xml.XmlNode node, string formatName, VGridControlBase vgrid) {
			if(node.Name.IndexOf("StyleName") != -1 && node.Attributes["name"].Value == formatName) {
				node = node.FirstChild;
				while (node != null) {
					LoadSchemeRow(node, vgrid);
					node = node.NextSibling;
				}
				return;
			}
			node = node.FirstChild;	
			while (node != null) {
				LoadScheme(node, formatName, vgrid);
				node = node.NextSibling;
			}
		}
		private void LoadSchemeRow(System.Xml.XmlNode node, VGridControlBase vgrid) {
			AppearanceObject appObject;
			if(node.Name == "Style") {
				System.Xml.XmlNode anode = node.FirstChild;
				appObject = vgrid.Appearance.GetAppearance(anode.Value);
				if(appObject == null) return;
				AppearanceLayoutHelper.SetAppearanceFontColors(node, appObject);
				if(appObject.Name == "HorzLine") 
					vgrid.Appearance.GetAppearance("Category").BorderColor = appObject.BackColor;
			}
		}
	}
}
