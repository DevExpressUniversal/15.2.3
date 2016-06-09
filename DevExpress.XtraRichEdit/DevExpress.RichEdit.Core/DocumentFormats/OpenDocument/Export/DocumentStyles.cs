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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
namespace DevExpress.XtraRichEdit.Export.OpenDocument {
	public struct StyleKey {
		int propertyIndex;
		int styleIndex;
		public StyleKey(int propertyIndex, int styleIndex) {
			this.propertyIndex = propertyIndex;
			this.styleIndex = styleIndex;
		}
		public int PropertyIndex { get { return propertyIndex; } }
		public int StyleIndex { get { return styleIndex; } }
		public override bool Equals(object obj) {
			StyleKey info = (StyleKey)obj;
			return propertyIndex == info.PropertyIndex && styleIndex == info.StyleIndex;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public abstract class StyleInfoBase {
		internal const int EmptyStyleIndex = -1; 
		bool isStyleUsedInMainDocument;
		readonly string name;
		protected StyleInfoBase(string name) {
			this.name = name;
			this.isStyleUsedInMainDocument = true;
		}
		public string Name { get { return name; } }
		public bool IsStyleUsedInMainDocument { get { return isStyleUsedInMainDocument; } set { isStyleUsedInMainDocument = value; } }
		public override int GetHashCode() {
			int addition = (isStyleUsedInMainDocument)? 0 : 1;
			return name.GetHashCode() + addition;
		}
	}
	public class CharacterStyleInfo : StyleInfoBase {
		CharacterProperties characterProperties;
		public CharacterStyleInfo(CharacterProperties properties, string styleName)
			: base(styleName) {
			this.characterProperties = properties;
			Guard.ArgumentNotNull(properties, "properties");
		}
		public CharacterProperties CharacterProperties { get { return characterProperties; } }
	}
	public class PictureStyleInfo : StyleInfoBase {
		InlinePictureProperties pictureProperties;
		public PictureStyleInfo(InlinePictureProperties properties, string styleName)
			: base(styleName) {
			Guard.ArgumentNotNull(properties, "properties");
			this.pictureProperties = properties;		   
		}
		public InlinePictureProperties PictureProperties { get { return pictureProperties; } }
	}
	public class FloatingObjectStyleInfo : StyleInfoBase {
		#region Fields
		readonly FloatingObjectProperties floatingObjectProperties;
		readonly Shape shape;
		readonly TextBoxProperties textBoxProperties;
		#endregion
		public FloatingObjectStyleInfo(FloatingObjectProperties floatingObjectProperties, Shape shape, TextBoxProperties textBoxProperties, string styleName)
			:base(styleName) {
			Guard.ArgumentNotNull(floatingObjectProperties, "floatingObject");
			Guard.ArgumentNotNull(shape, "shape");
			this.floatingObjectProperties = floatingObjectProperties;
			this.shape = shape;
			this.textBoxProperties = textBoxProperties;
		}
		#region Properties
		public FloatingObjectProperties FloatingObjectProperties { get { return floatingObjectProperties; } }
		public Shape Shape { get { return shape; } }
		public TextBoxProperties TextBoxProperties { get { return textBoxProperties; } }
		#endregion
	}
	public class ParagraphStyleInfo : StyleInfoBase {
		string masterPageName = string.Empty;
		string styleListName = string.Empty;
		ParagraphBreakType breakAfterType = ParagraphBreakType.None;
		public ParagraphStyleInfo(string masterPageName, string styleName, string styleListName, ParagraphBreakType breakAfterType)
			: base(styleName) {
			this.masterPageName = masterPageName;
			this.styleListName = styleListName;
			this.breakAfterType = breakAfterType;
		}
		public string MasterPageName { get { return masterPageName; } }
		public string StyleListName { get { return styleListName; } }
		public ParagraphBreakType BreakAfterType { get { return breakAfterType; } }
	}
	public class CharacterStyleInfoTable : Dictionary<StyleKey, CharacterStyleInfo> {
		public void RegisterStyle(CharacterProperties properties) {
			RegisterStyle(properties, StyleInfoBase.EmptyStyleIndex, false);
		}
		public void RegisterStyle(CharacterProperties properties, int styleIndex, bool isUsedInMainPieceTable) {
			StyleKey key = new StyleKey(properties.Index, styleIndex);
			if (ContainsKey(key))
				return;
			string styleName = NameResolver.CalculateCharacterAutoStyleName(Count + 1);
			CharacterStyleInfo info = new CharacterStyleInfo(properties, styleName);
			info.IsStyleUsedInMainDocument = isUsedInMainPieceTable;
			Add(key, info);
		}
		public string GetStyleName(CharacterProperties properties) {
			return GetStyleName(properties.Index, StyleInfoBase.EmptyStyleIndex);
		}
		public string GetStyleName(int propertyIndex) {
			return GetStyleName(propertyIndex, StyleInfoBase.EmptyStyleIndex);
		}
		public string GetStyleName(CharacterProperties properties, int styleIndex) {
			return GetStyleName(properties.Index, styleIndex);
		}
		public string GetStyleName(int propertyIndex, int styleIndex) {
			StyleKey key = new StyleKey(propertyIndex, styleIndex);
			CharacterStyleInfo info;
			if (TryGetValue(key, out info))
				return info.Name;
			return String.Empty;
		}
	}
	public class ParagraphStyleInfoTable : Dictionary<Paragraph, ParagraphStyleInfo> {
		public void RegisterStyle(Paragraph paragraph, string masterPageName, ParagraphBreakType breakAfterType, bool isUsedInMainPieceTable) {
			if (ContainsKey(paragraph))
				return;
			string listStyleName = NameResolver.CalculateNumberingListReferenceName(paragraph);
			string styleName = NameResolver.CalculateParagraphAutoStyleName(Count + 1);
			ParagraphStyleInfo info = new ParagraphStyleInfo(masterPageName, styleName, listStyleName, breakAfterType);
			info.IsStyleUsedInMainDocument = isUsedInMainPieceTable;
			Add(paragraph, info);
		}
	}
	public class PictureStyleInfoTable : Dictionary<StyleKey, PictureStyleInfo> {
		public void RegisterStyle(InlinePictureProperties properties, bool isUsedInMainPieceTable) {
			RegisterStyle(properties, StyleInfoBase.EmptyStyleIndex, true);
		}
		void RegisterStyle(InlinePictureProperties properties, int styleIndex, bool isUsedInMainPieceTable) {
			StyleKey key = new StyleKey(properties.Index, styleIndex);
			if (ContainsKey(key))
				return;
			string styleName = NameResolver.CalculatePictureAutoStyleName(Count);
			PictureStyleInfo info = new PictureStyleInfo(properties, styleName);
			Add(key, info);
		}
		public string GetStyleName(int propertyIndex) {
			StyleKey key = new StyleKey(propertyIndex, StyleInfoBase.EmptyStyleIndex);
			PictureStyleInfo info;
			if (TryGetValue(key, out info))
				return info.Name;
			return String.Empty;
		}
	}
	public class FloatingObjectStyleInfoTable : Dictionary<StyleKey, FloatingObjectStyleInfo> {
		public void RegisterStyle(FloatingObjectAnchorRun run, bool isUsedInMainPieceTable) {
			TextBoxProperties textBoxProperties;
			TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
			if (textBoxContent != null)
				textBoxProperties = textBoxContent.TextBoxProperties;
			else
				textBoxProperties = null;
			RegisterStyle(run.FloatingObjectProperties, run.Shape, textBoxProperties, StyleInfoBase.EmptyStyleIndex, true);
		}
		void RegisterStyle(FloatingObjectProperties floatingObjectProperties, Shape shape, TextBoxProperties textBoxProperties, int styleIndex, bool isUsedInMainPieceTable) {
			StyleKey key = new StyleKey(floatingObjectProperties.Index, styleIndex);
			if (ContainsKey(key))
				return;
			string styleName = NameResolver.CalculateFloatingObjectAutoStyleName(Count);
			FloatingObjectStyleInfo info = new FloatingObjectStyleInfo(floatingObjectProperties, shape, textBoxProperties, styleName);
			Add(key, info);
		}
		public string GetStyleName(int propertyIndex) {
			StyleKey key = new StyleKey(propertyIndex, StyleInfoBase.EmptyStyleIndex);
			FloatingObjectStyleInfo info;
			if (TryGetValue(key, out info))
				return info.Name;
			return String.Empty;
		}
	}
	#region TableStyleInfo
	public class TableStyleInfo : StyleInfoBase {
		int parentWidth;
		string masterPageName = String.Empty;
		public TableStyleInfo(string name, string masterPageName)
			: base(name) {
				this.masterPageName = masterPageName;
		}
		public string MasterPageName { get { return masterPageName; } }
		public int ParentWidth { get { return parentWidth; } set { parentWidth = value; } }
	}
	#endregion
	#region TableStyleInfoTable
	public class TableStyleInfoTable : StyleNameTableBase<Table, TableStyleInfo> {
		public void RegisterStyle(Table table, int parentWidth, string masterPageName) {
			if (ContainsKey(table))
				return;
			string styleName = NameResolver.CalculateTableStyleName(Count + 1);
			TableStyleInfo info = new TableStyleInfo(styleName, masterPageName);
			info.ParentWidth = parentWidth;
			info.IsStyleUsedInMainDocument = table.PieceTable.IsMain;
			Add(table, info);
		}
	}
	#endregion
	#region TableCellStyleInfo
	public class TableCellStyleInfo : StyleInfoBase {
		public TableCellStyleInfo(string name)
			: base(name) {
		}
	}
	#endregion
	#region TableCellStyleInfoTable
	public class TableCellStyleInfoTable : StyleNameTableBase<TableCell, TableCellStyleInfo> {
		public void RegisterStyle(TableCell cell, string tableRowStyleName) {
			if (ContainsKey(cell))
				return;
			string styleName = NameResolver.CalculateTableCellStyleName(tableRowStyleName, Count + 1);
			TableCellStyleInfo info = new TableCellStyleInfo(styleName);
			info.IsStyleUsedInMainDocument = cell.PieceTable.IsMain;
			Add(cell, info);
		}
	}
	#endregion
	#region TableColumnStyleInfoTable
	public class TableColumnStyleInfoTable : Dictionary<Table, OpenDocumentTableColumnsInfo> {
		public void RegisterStyle(Table table, OpenDocumentTableColumnsInfo info) {
			if (ContainsKey(table))
				return;
			Add(table, info);
		}
	}
	#endregion
	#region TableRowStyleInfo
	public class TableRowStyleInfo : StyleInfoBase {
		public TableRowStyleInfo(string name)
			: base(name) {
		}
	}
	#endregion
	#region TableRowStyleInfoTable
	public class TableRowStyleInfoTable : StyleNameTableBase<TableRow, TableRowStyleInfo> {
		public void RegisterStyle(TableRow row, string tableStyleName) {
			if (ContainsKey(row))
				return;
			string styleName = NameResolver.CalculateTableRowStyleName(tableStyleName, Count + 1);
			TableRowStyleInfo info = new TableRowStyleInfo(styleName);
			info.IsStyleUsedInMainDocument = row.PieceTable.IsMain;
			Add(row, info);
		}
	}
	#endregion
	#region StyleNameTableBase ( abstract class ) 
	public abstract class StyleNameTableBase<T, U> : Dictionary<T, U> where U : StyleInfoBase {
		public string GetName(T key) {
			if (ContainsKey(key))
				return this[key].Name;
			return String.Empty;
		}
	}
	#endregion
}
