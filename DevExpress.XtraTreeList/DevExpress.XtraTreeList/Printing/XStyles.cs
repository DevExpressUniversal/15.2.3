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
using DevExpress.XtraEditors.Repository;
using System.Reflection;
using System.IO;
namespace DevExpress.XtraTreeList.Design {
	public class XStyles {	
		private System.Xml.XmlDocument xmlStyles = new System.Xml.XmlDocument();
		private System.Xml.XmlNode nStyles = null;
		private ArrayList styles = new ArrayList();
		private string DefStyle = "(Default)";
		public object[] Styles {
			get { return styles.ToArray(); }
		}
		private string FindFileName() {
			string styleFileName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.System) + "\\DevExpress.XtraTreeList.Styles.xml";
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
		private void LoadSchemeRow(System.Xml.XmlNode node, TreeList treeList) {
		}
		private void LoadSchemeMain(System.Xml.XmlNode node, string stylename, TreeList treeList) {
			if(node.Name == "StyleName" && node.Attributes["name"].Value == stylename) {
				node = node.FirstChild;
				while (node != null) {
					LoadSchemeRow(node, treeList);
					node = node.NextSibling;
				}
				return;
			}
			node = node.FirstChild;	
			while (node != null) {
				LoadSchemeMain(node, stylename, treeList);
				node = node.NextSibling;
			}
		}
		public void LoadScheme(System.Xml.XmlNode node, string stylename, TreeList treeList) {
			if(stylename == DefStyle) {
				return;
			}
			if(node == null) node = nStyles;
			if(node == null) return;
			treeList.BeginUpdate();
				LoadSchemeMain(node, stylename, treeList);
			treeList.EndUpdate();
		}
	}
	public class XViews {
		private TreeList treeList;
		private ImageCollection imageList1;
		public XViews(TreeList tl) {
			treeList = tl;
			if(treeList != null) {
				InitResources();
				InitData();
				BestFitColumns();
			}
		}
		private void BestFitColumns() {
			if(treeList != null) {
				int w = treeList.Width;
				treeList.Columns[0].Width = w / 2;
				treeList.Columns[1].Width = treeList.Columns[2].Width = w / 4;
			}
		}
		public void Update() {
			treeList.PopulateColumns();
			BestFitColumns();
		}
		protected virtual void InitResources() {
			imageList1 = new ImageCollection();
			imageList1.ImageSize = new System.Drawing.Size(16, 16);
			try {
				System.IO.Stream str = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraTreeList.Images.nodes_images.png");
				imageList1.AddImageStrip(Bitmap.FromStream(str));
			} catch {}
			treeList.SelectImageList = imageList1;
		}
		protected virtual void InitData() {
			DataSet ds = new DataSet();
			System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraTreeList.Printing.XView.xml");
			ds.ReadXml(stream);
			stream.Close(); 
			treeList.DataSource = ds.Tables[0];
			treeList.PopulateColumns();
			treeList.OptionsBehavior.Editable = false;
			treeList.Columns[1].Format.FormatString = "c";
			treeList.ExpandAll();
		}
	}
	public class TreeListAssign {
		public static TreeList CreateTreeListControlAssign(TreeList editingTreeList) {
			TreeList treeList = Assembly.GetAssembly(editingTreeList.GetType()).CreateInstance(editingTreeList.GetType().ToString()) as DevExpress.XtraTreeList.TreeList;
			treeList.Dock = DockStyle.Fill;
			treeList.OptionsBehavior.PopulateServiceColumns = true;
			TreeListControlAssign(editingTreeList, treeList, true, true);	
			return treeList;
		}
		public static void TreeListControlAssign(TreeList editingTreeList, TreeList treeList, bool setRepository, bool setStyles) {
			if(setRepository) {
				AssignEditors(editingTreeList, treeList);
				treeList.ExternalRepository = editingTreeList.ExternalRepository;
			}
			treeList.LookAndFeel.Assign(editingTreeList.LookAndFeel);
			treeList.SelectImageList = editingTreeList.SelectImageList;
			treeList.StateImageList = editingTreeList.StateImageList;
			treeList.Appearance.Assign(editingTreeList.Appearance);
			System.IO.MemoryStream str = new System.IO.MemoryStream();
			editingTreeList.SaveLayoutToStream(str);
			str.Seek(0, System.IO.SeekOrigin.Begin);
			treeList.RestoreLayoutFromStream(str);
			str.Close();
		}
		private static void AssignEditors(TreeList sourceTreeList, TreeList treeList) {
			foreach(DevExpress.XtraEditors.Repository.RepositoryItem item in sourceTreeList.RepositoryItems) 
				treeList.RepositoryItems.Add(item.Clone() as DevExpress.XtraEditors.Repository.RepositoryItem);
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
			return System.Environment.GetFolderPath(System.Environment.SpecialFolder.System) + "\\DevExpress.XtraTreeList.Appearances.xml";
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
				using(Stream xmlStream = AssemblyHelper.GetEmbeddedResourceStream(typeof(TreeList).Assembly, "Blending.DevExpress.XtraTreeList.Appearances.xml", false)) {
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
		public void LoadSchemeFromFile(string fileName, TreeList treeList) {
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
			if(treeList != null) treeList.Appearance.Reset();
			System.Xml.XmlNode node = xmlFFormats.DocumentElement;
			while(node != null) {
				if(node.Name.IndexOf("StyleName") != -1) {
					treeList.BeginUpdate();
					node = node.FirstChild;
					while (node != null) {
						LoadSchemeRow(node, treeList);
						node = node.NextSibling;
					}
					treeList.EndUpdate();
					return;
				}
				node = node.NextSibling;
			}
		}
		public void LoadScheme(string formatName, TreeList treeList) {
			if(treeList == null) return;
			treeList.BeginUpdate();
			try {
				treeList.Appearance.Reset();
				treeList.OptionsView.EnableAppearanceEvenRow = false;
				treeList.OptionsView.EnableAppearanceOddRow = false;
				if(formatName != DefStyle && nodeFormats != null) 
					LoadScheme(nodeFormats, formatName, treeList);
			} finally {
				treeList.EndUpdate();
			}
		}
		private void LoadScheme(System.Xml.XmlNode node, string formatName, TreeList treeList) {
			if(node.Name.IndexOf("StyleName") != -1 && node.Attributes["name"].Value == formatName) {
				node = node.FirstChild;
				while (node != null) {
					LoadSchemeRow(node, treeList);
					node = node.NextSibling;
				}
				return;
			}
			node = node.FirstChild;	
			while (node != null) {
				LoadScheme(node, formatName, treeList);
				node = node.NextSibling;
			}
		}
		private void LoadSchemeRow(System.Xml.XmlNode node, TreeList treeList) {
			AppearanceObject appObject;
			if(node.Name == "Style") {
				System.Xml.XmlNode anode = node.FirstChild;
				appObject = treeList.Appearance.GetAppearance(anode.Value);
				if(appObject == null) return;
				AppearanceLayoutHelper.SetAppearanceFontColors(node, appObject);
				if(appObject.Name == "EvenRow") treeList.OptionsView.EnableAppearanceEvenRow = appObject.BackColor != Color.Empty;
				if(appObject.Name == "OddRow") treeList.OptionsView.EnableAppearanceOddRow = appObject.BackColor != Color.Empty;
			}
		}
	}
}
