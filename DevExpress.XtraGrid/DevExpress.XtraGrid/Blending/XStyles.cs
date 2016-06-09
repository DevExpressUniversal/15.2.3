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
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Views.Layout;
using System.IO;
namespace DevExpress.XtraGrid.Design {
	public class XStyles {	
		private System.Xml.XmlDocument xmlStyles = new System.Xml.XmlDocument();
		private System.Xml.XmlNode nStyles = null;
		private ArrayList styles = new ArrayList();
		private string DefStyle = "(Default)";
		public object[] Styles {
			get { return styles.ToArray(); }
		}
		private string FindFileName() {
			string styleFileName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.System) + "\\DevExpress.XtraGrid2.Styles.xml";
			if(!System.IO.File.Exists(styleFileName)) 
				styleFileName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.System) + "\\DevExpress.XtraGrid.Styles.xml";
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
				XtraMessageBox.Show(string.Format(Localization.GridLocalizer.Active.GetLocalizedString(GridStringId.FileIsNotFoundError), styleFileName), DevExpress.XtraEditors.Controls.Localizer.Active.GetLocalizedString(DevExpress.XtraEditors.Controls.StringId.CaptionError), MessageBoxButtons.OK, MessageBoxIcon.Error); 
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
		private void LoadSchemeRow(System.Xml.XmlNode node, DevExpress.XtraGrid.GridControl grid, bool b) {
		}
		private void SetBorderStyle(DevExpress.XtraGrid.Views.Base.BaseView bv, object bs) {
			try {
				DevExpress.XtraEditors.Controls.BorderStyles gbs = (DevExpress.XtraEditors.Controls.BorderStyles)Enum.Parse(typeof(DevExpress.XtraEditors.Controls.BorderStyles), bs.ToString());
				bv.BorderStyle = gbs;
			} catch {}
		}
		private void LoadSchemeMain(System.Xml.XmlNode node, string stylename, DevExpress.XtraGrid.GridControl grid, string viewtype) {
			if(node.Name == "StyleName" && node.Attributes["name"].Value == stylename) {
				node = node.FirstChild;
				while (node != null) {
					LoadSchemeRow(node, grid, viewtype == "CardView");
					node = node.NextSibling;
				}
				return;
			}
			node = node.FirstChild;	
			while (node != null) {
				LoadSchemeMain(node, stylename, grid, viewtype);
				node = node.NextSibling;
			}
		}
		private string GridViewType(DevExpress.XtraGrid.Views.Base.BaseView bv) {
			return bv.BaseInfo.ViewName;
		}
		private static string EncodingForCardStyle(string s) {
			string res = "";
			switch(s) {
				case "HorzLine":
					res = "CardBorder";
					break;
				case "HeaderPanel":
					res = "CardCaption";
					break;
				case "Empty":
					res = "EmptySpace";
					break;
				case "EvenRow":
					res = "FieldCaption";
					break;
				case "Row":
					res = "FieldValue";
					break;
				case "FocusedRow":
					res = "FocusedCardCaption";
					break;
				case "VertLine":
					res = "SeparatorLine";
					break;
			}
			return res;
		}
		public void LoadScheme(System.Xml.XmlNode node, string stylename, DevExpress.XtraGrid.GridControl grid, DevExpress.XtraGrid.Views.Base.BaseView bv) {
			if(stylename == DefStyle) {
				return;
			}
			if(node == null) node = nStyles;
			if(node == null) return;
			grid.BeginUpdate();
			LoadSchemeMain(node, stylename, grid, GridViewType(bv));
			grid.EndUpdate();
			grid.MainView.LayoutChanged();
		}
	}
	public class GridAssign {
		public static GridControl CreateGridControlAssign(GridControl editingGrid, BaseView editingView) {
			GridControl grid = null;
			try {
				grid = Activator.CreateInstance(editingGrid.GetType()) as GridControl;
			} catch {
				grid = new DevExpress.XtraGrid.GridControl();
			}
			grid.Dock = DockStyle.Fill;
			GridControlAssign(editingGrid, editingView, grid, true, true);	
			return grid;
		}
		public static void RegisterViews(GridControl sourceGrid, GridControl createdGrid) {
			System.Reflection.MethodInfo method = typeof(GridControl).GetMethod("RegisterAvailableViewsCore", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			method.Invoke(sourceGrid, new Object[] { createdGrid.AvailableViews });
		}
		public static void GridControlAssign(GridControl editingGrid, BaseView editingView, GridControl grid, bool setRepository, bool setStyles) {
			string viewName = editingView.BaseInfo.ViewName;
			RegisterViews(editingGrid, grid);
			DevExpress.XtraGrid.Views.Base.BaseView oldView = grid.MainView;
			grid.MainView = editingView.BaseInfo.CreateView(grid);
			PatchViewName(editingView, grid);
			if(grid.MainView == null) {
				grid.MainView = oldView;
				return;
			}
			if(oldView != null) oldView.Dispose();
			if(setRepository) {
				AssignEditors(editingGrid, grid);
				grid.ExternalRepository = editingGrid.ExternalRepository;
			}
			if(setStyles)SetAppearances(editingView, grid.MainView);
			grid.LookAndFeel.ParentLookAndFeel = editingGrid.LookAndFeel;
			grid.MainView.PaintStyleName = editingView.PaintStyleName;
			grid.BackgroundImage = editingView.GridControl.BackgroundImage;
			if(grid.MainView is ColumnView) {
				((ColumnView)grid.MainView).Images = ((ColumnView)editingView).Images;
			}
			System.IO.MemoryStream str = new System.IO.MemoryStream();
			editingView.SaveLayoutToStream(str, null);
			str.Seek(0, System.IO.SeekOrigin.Begin);
			grid.MainView.RestoreLayoutFromStream(str, null);
			str.Close();
		}
		private static void PatchViewName(BaseView editingView, GridControl grid) {
			if(editingView is LayoutView && grid.MainView != null) grid.MainView.Name = editingView.Name;
		}
		public static void AppearanceAssign(BaseView sourceView, BaseView view) {
			view.Appearance.Assign(sourceView.Appearance);
			view.PaintStyleName = sourceView.PaintStyleName;
			view.GridControl.LookAndFeel.ParentLookAndFeel = sourceView.GridControl.LookAndFeel;
			GridView gView = view as GridView;
			if(gView != null && sourceView is GridView) {
				gView.OptionsView.EnableAppearanceEvenRow = ((GridView)sourceView).OptionsView.EnableAppearanceEvenRow;
				gView.OptionsView.EnableAppearanceOddRow = ((GridView)sourceView).OptionsView.EnableAppearanceOddRow;
				gView.OptionsSelection.EnableAppearanceFocusedCell = ((GridView)sourceView).OptionsSelection.EnableAppearanceFocusedCell;
				gView.OptionsSelection.EnableAppearanceFocusedRow = ((GridView)sourceView).OptionsSelection.EnableAppearanceFocusedRow;
			}
			view.GridControl.BackgroundImage = sourceView.GridControl.BackgroundImage;
		}
		private static void AssignEditors(GridControl sourceGrid, GridControl grid) {
			foreach(DevExpress.XtraEditors.Repository.RepositoryItem item in sourceGrid.RepositoryItems) 
				grid.RepositoryItems.Add(item.Clone() as DevExpress.XtraEditors.Repository.RepositoryItem);
		}
		#region Appearances Assign
		public static void SetAppearances(DevExpress.XtraGrid.GridControl sourceGrid, DevExpress.XtraGrid.GridControl grid) {
			SetAppearances(sourceGrid.MainView, grid.MainView);
		}
		public static void SetAppearances(BaseView sourceView, BaseView view) {
			view.Appearance.Assign(sourceView.Appearance);
		}
		#endregion
		#region ViewStyles Assign
		public static void SetStyles(DevExpress.XtraGrid.GridControl sourceGrid, DevExpress.XtraGrid.GridControl grid) {
			SetStyles(sourceGrid, sourceGrid.MainView, grid);
		}
		public static void SetStyles(DevExpress.XtraGrid.GridControl sourceGrid, BaseView view, DevExpress.XtraGrid.GridControl grid) {
		}
		#endregion
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
			return System.Environment.GetFolderPath(System.Environment.SpecialFolder.System) + "\\DevExpress.XtraGrid.Appearances.xml";
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
		public XAppearances(string styleFileName) : this(styleFileName, false){}
		public XAppearances(string styleFileName, bool isNew) {
			if(styleFileName == "System") styleFileName = FindFileName();
			if(System.IO.File.Exists(styleFileName)) {
				xmlFormats.Load(styleFileName);
			} else {
				using(Stream xmlStream = AssemblyHelper.GetEmbeddedResourceStream(typeof(GridControl).Assembly, "Blending.DevExpress.XtraGrid.Appearances.xml", false)) {
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
		private string GridViewType(DevExpress.XtraGrid.Views.Base.BaseView bv) {
			return bv.BaseInfo.ViewName;
		}
		public void LoadSchemeFromFile(string fileName, DevExpress.XtraGrid.Views.Base.BaseView bv) {
			if(!System.IO.File.Exists(fileName)) {
				XtraMessageBox.Show(string.Format(Localization.GridLocalizer.Active.GetLocalizedString(GridStringId.FileIsNotFoundError), fileName), DevExpress.XtraEditors.Controls.Localizer.Active.GetLocalizedString(DevExpress.XtraEditors.Controls.StringId.CaptionError), MessageBoxButtons.OK, MessageBoxIcon.Error); 
				return;
			}
			System.Xml.XmlDocument xmlFFormats = new System.Xml.XmlDocument();
			try {
				xmlFFormats.Load(fileName);
			} catch {
				XtraMessageBox.Show("Invalid File Format.", DevExpress.XtraEditors.Controls.Localizer.Active.GetLocalizedString(DevExpress.XtraEditors.Controls.StringId.CaptionError), MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if(bv != null) {
				bv.Appearance.Reset();
				SetEnableAppearanceEvenRow(bv, false);
				SetEnableAppearanceOddRow(bv, false);
			}
			System.Xml.XmlNode node = xmlFFormats.DocumentElement;
			while(node != null) {
				if(node.Name.IndexOf("StyleName") != -1) {
					bv.BeginUpdate();
					node = node.FirstChild;
					while (node != null) {
						LoadSchemeRow(node, bv);
						node = node.NextSibling;
					}
					bv.EndUpdate();
					return;
				}
				node = node.NextSibling;
			}
		}
		public void LoadScheme(string formatName, DevExpress.XtraGrid.Views.Base.BaseView bv) {
			if(bv == null) return;
			bv.BeginUpdate();
			try {
				bv.Appearance.Reset();
				if(formatName == DefStyle) {
					SetEnableAppearanceEvenRow(bv, false);
					SetEnableAppearanceOddRow(bv, false);
				}
				if(formatName != DefStyle && nodeFormats != null) 
					LoadScheme(nodeFormats, formatName, bv);
			} finally {
				bv.EndUpdate();
			}
		}
		private void LoadScheme(System.Xml.XmlNode node, string formatName, DevExpress.XtraGrid.Views.Base.BaseView bv) {
			if(node.Name.IndexOf("StyleName") != -1 && node.Attributes["name"].Value == formatName) {
				node = node.FirstChild;
				while (node != null) {
					LoadSchemeRow(node, bv);
					node = node.NextSibling;
				}
				return;
			}
			node = node.FirstChild;	
			while (node != null) {
				LoadScheme(node, formatName, bv);
				node = node.NextSibling;
			}
		}
		private void LoadSchemeRow(System.Xml.XmlNode node, DevExpress.XtraGrid.Views.Base.BaseView bv) {
			AppearanceObject appObject;
			bool setHorzGradient = false;
			bool setCardBorder = false;
			if(node.Name == "Style") {
				System.Xml.XmlNode anode = node.FirstChild;
				if(GridViewType(bv) == "CardView") { 
					string name = EncodingForCardStyle(anode.Value);
					appObject = bv.Appearance.GetAppearance(name);
					setHorzGradient = name == "FieldCaption";
					setCardBorder = name == "FocusedCardCaption" || name == "HideSelectionCardCaption" || name == "CardCaption";
				} else if(GridViewType(bv) == "LayoutView") { 
					string name = EncodingForLayoutStyle(anode.Value);
					appObject = bv.Appearance.GetAppearance(name);
					setHorzGradient = name == "FieldCaption";
					setCardBorder = name == "FocusedCardCaption" || name == "HideSelectionCardCaption" || name == "CardCaption";
				}
				else
					appObject = bv.Appearance.GetAppearance(anode.Value);
				if(appObject == null) return;
				AppearanceLayoutHelper.SetAppearanceFontColors(node, appObject);
				if(appObject.Name == "EvenRow") SetEnableAppearanceEvenRow(bv, appObject.BackColor != Color.Empty);
				if(appObject.Name == "OddRow") SetEnableAppearanceOddRow(bv, appObject.BackColor != Color.Empty);
				if(setCardBorder && appObject.BorderColor == Color.Empty) appObject.BorderColor = appObject.BackColor;
				if(setHorzGradient)
						appObject.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
			}
		}
		void SetEnableAppearanceEvenRow(DevExpress.XtraGrid.Views.Base.BaseView bv, bool isSet) {
			GridView gv = bv as GridView;
			if(gv != null) 
				gv.OptionsView.EnableAppearanceEvenRow = isSet;
		}
		void SetEnableAppearanceOddRow(DevExpress.XtraGrid.Views.Base.BaseView bv, bool isSet) {
			GridView gv = bv as GridView;
			if(gv != null) 
				gv.OptionsView.EnableAppearanceOddRow = isSet;
		}
		private static string EncodingForCardStyle(string s) {
			string res = "";
			switch(s) {
				case "HorzLine":
					res = "CardBorder";
					break;
				case "HeaderPanel":
					res = "CardCaption";
					break;
				case "Empty":
					res = "EmptySpace";
					break;
				case "EvenRow":
					res = "FieldCaption";
					break;
				case "Row":
					res = "FieldValue";
					break;
				case "FocusedRow":
					res = "FocusedCardCaption";
					break;
				case "SelectedRow":
					res = "SelectedCardCaption";
					break;
				case "HideSelectionRow":
					res = "HideSelectionCardCaption";
					break;
				case "VertLine":
					res = "SeparatorLine";
					break;
				case "FilterCloseButton":
				case "FilterPanel":
					res = s;
					break;
			}
			return res;
		}
		private static string EncodingForLayoutStyle(string s) {
			string res = "";
			switch(s) {
				case "HeaderPanel":
					res = "CardCaption";
					break;
				case "Empty":
					res = "ViewBackground";
					break;
				case "EvenRow":
					res = "FieldCaption";
					break;
				case "Row":
					res = "FieldValue";
					break;
				case "FocusedRow":
					res = "FocusedCardCaption";
					break;
				case "SelectedRow":
					res = "SelectedCardCaption";
					break;
				case "HideSelectionRow":
					res = "HideSelectionCardCaption";
					break;
				case "VertLine":
					res = "SeparatorLine";
					break;
				case "FilterCloseButton":
				case "FilterPanel":
					res = s;
					break;
			}
			return res;
		}
	}
}
