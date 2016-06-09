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
using DevExpress.Office;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.Spreadsheet {
	public interface StyleCollection : ISimpleCollection<Style> {
		Style Add(string name);
		Style this[string name] { get; }
		Style this[BuiltInStyleId id] { get; }
		Style DefaultStyle { get; }
		void Hide(Style style);
		void Unhide(Style style);
		[Obsolete("Use StyleCollection.Hide(Style style) method instead.", true)]
		bool Remove(string name);
		bool Contains(string name);
		[Obsolete("All built-in styles are added to the collection by default.", true)]
		bool Contains(BuiltInStyleId id);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	#region usings
	using DevExpress.Utils;
	using System.Collections.Generic;
	using DevExpress.XtraSpreadsheet.Utils;
	using DevExpress.XtraSpreadsheet.Internal;
	using DevExpress.XtraSpreadsheet.Localization;
	using ModelDefinedName = DevExpress.XtraSpreadsheet.Model.DefinedName;
	using ModelDefinedNameCollection = DevExpress.XtraSpreadsheet.Model.DefinedNameCollection;
	using ModelDefinedNameDase = DevExpress.XtraSpreadsheet.Model.DefinedNameBase;
	using ModelStyleSheet = DevExpress.XtraSpreadsheet.Model.StyleSheet;
	using ModelWorkbook = DevExpress.XtraSpreadsheet.Model.DocumentModel;
	using ModelWorkbookDataContext = DevExpress.XtraSpreadsheet.Model.WorkbookDataContext;
	using ModelWorksheet = DevExpress.XtraSpreadsheet.Model.Worksheet;
	using DevExpress.Spreadsheet;
	using DevExpress.Office.Utils;
	#endregion
	#region NativeStyleCollection
	partial class NativeStyleCollection : List<NativeCellStyle>, StyleCollection {
		readonly Model.DocumentModel modelWorkbook;
		Dictionary<BuiltInStyleId, int> BuiltInStyleIdTable;
		public NativeStyleCollection(Model.DocumentModel model) {
			BuiltInStyleIdTable = CreateBuiltInStyleIdTable();
			this.modelWorkbook = model;
		}
		public Model.StyleSheet ModelStyleSheet { get { return modelWorkbook.StyleSheet; } }
		public Model.CellStyleCollection CellStyles { get { return modelWorkbook.StyleSheet.CellStyles; } }
		public void OnCellStyles_StyleAdded(object sender, DevExpress.XtraSpreadsheet.Model.StyleCollectionChangedEventArgs args) {
			RegisterModelStyle(args.NewStyle);
		}
		public void OnCellStyles_StyleRemoved(object sender, DevExpress.XtraSpreadsheet.Model.StyleCollectionChangedEventArgs args) {
			UnRegisterModelStyle(args.NewStyle);
		}
		public void OnCellStyles_Cleared(object sender, EventArgs args) {
			ForEach(x => x.IsValid = false);
			this.Clear();
		}
		#region CreateBuiltInStyleIdTable
		Dictionary<BuiltInStyleId, int> CreateBuiltInStyleIdTable() {
			Dictionary<BuiltInStyleId, int> result = new Dictionary<BuiltInStyleId, int>();
			result.Add(BuiltInStyleId.Normal, 0);
			result.Add(BuiltInStyleId.Comma, 3);
			result.Add(BuiltInStyleId.Currency, 4);
			result.Add(BuiltInStyleId.Percent, 5);
			result.Add(BuiltInStyleId.Comma0, 6);
			result.Add(BuiltInStyleId.Currency0, 7);
			result.Add(BuiltInStyleId.Hyperlink, 8);
			result.Add(BuiltInStyleId.FollowedHyperlink, 9);
			result.Add(BuiltInStyleId.Note, 10);
			result.Add(BuiltInStyleId.WarningText, 11);
			result.Add(BuiltInStyleId.Emphasis1, 12);
			result.Add(BuiltInStyleId.Emphasis2, 13);
			result.Add(BuiltInStyleId.Emphasis3, 14);
			result.Add(BuiltInStyleId.Title, 15);
			result.Add(BuiltInStyleId.Heading1, 16);
			result.Add(BuiltInStyleId.Heading2, 17);
			result.Add(BuiltInStyleId.Heading3, 18);
			result.Add(BuiltInStyleId.Heading4, 19);
			result.Add(BuiltInStyleId.Input, 20);
			result.Add(BuiltInStyleId.Output, 21);
			result.Add(BuiltInStyleId.Calculation, 22);
			result.Add(BuiltInStyleId.CheckCell, 23);
			result.Add(BuiltInStyleId.LinkedCell, 24);
			result.Add(BuiltInStyleId.Total, 25);
			result.Add(BuiltInStyleId.Good, 26);
			result.Add(BuiltInStyleId.Bad, 27);
			result.Add(BuiltInStyleId.Neutral, 28);
			result.Add(BuiltInStyleId.Accent1, 29);
			result.Add(BuiltInStyleId.Accent1_20percent, 30);
			result.Add(BuiltInStyleId.Accent1_40percent, 31);
			result.Add(BuiltInStyleId.Accent1_60percent, 32);
			result.Add(BuiltInStyleId.Accent2, 33);
			result.Add(BuiltInStyleId.Accent2_20percent, 34);
			result.Add(BuiltInStyleId.Accent2_40percent, 35);
			result.Add(BuiltInStyleId.Accent2_60percent, 36);
			result.Add(BuiltInStyleId.Accent3, 37);
			result.Add(BuiltInStyleId.Accent3_20percent, 38);
			result.Add(BuiltInStyleId.Accent3_40percent, 39);
			result.Add(BuiltInStyleId.Accent3_60percent, 40);
			result.Add(BuiltInStyleId.Accent4, 41);
			result.Add(BuiltInStyleId.Accent4_20percent, 42);
			result.Add(BuiltInStyleId.Accent4_40percent, 43);
			result.Add(BuiltInStyleId.Accent4_60percent, 44);
			result.Add(BuiltInStyleId.Accent5, 45);
			result.Add(BuiltInStyleId.Accent5_20percent, 46);
			result.Add(BuiltInStyleId.Accent5_40percent, 47);
			result.Add(BuiltInStyleId.Accent5_60percent, 48);
			result.Add(BuiltInStyleId.Accent6, 49);
			result.Add(BuiltInStyleId.Accent6_20percent, 50);
			result.Add(BuiltInStyleId.Accent6_40percent, 51);
			result.Add(BuiltInStyleId.Accent6_60percent, 52);
			result.Add(BuiltInStyleId.Explanatory, 53);
			result.Add(BuiltInStyleId.TableStyleLight1, 54);
			return result;
		}
		#endregion
		public void PopulateStyles() {
			this.Clear();
			Model.CellStyleCollection cellStyles = ModelStyleSheet.CellStyles;
			cellStyles.ForEach(RegisterModelStyle);
		}
		void RegisterModelStyle(Model.CellStyleBase modelStyle) {
			this.Add(new NativeCellStyle(modelStyle));
		}
		void UnRegisterModelStyle(Model.CellStyleBase deletedModelStyle) {
		}
		#region Add
		public Style Add(string name) {
			Model.CellStyleBase BuiltInStyle = CellStyles.GetCellStyleByName(name);
			if ((BuiltInStyle != null && !CellStyles.GetCellStyleByName(name).IsHidden) || (BuiltInStyle == null && Model.BuiltInCellStyleCalculator.IsBuiltInStyle(name)))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorDuplicateStyleName);
			Model.CustomCellStyle customCellStyle = new Model.CustomCellStyle(modelWorkbook, name);
			CellStyles.Add(customCellStyle);
			return this[name];
		}
		#endregion
		#region Add
		public Style Add(BuiltInStyleId idStyle) {
			if (ContainsCore(idStyle))
				return this[idStyle];
			Model.BuiltInCellStyle style = new Model.BuiltInCellStyle(modelWorkbook, BuiltInStyleIdTable[idStyle]);
			CellStyles.Add(style);
			return this[style.Name];
		}
		#endregion
		#region  this[string name]
		public Style this[string name] {
			get {
				try {
					int index = CellStyles.GetCellStyleIndexByName(name);
					if(index != -1) {
						Style result = this[index];
						return IsValid(result) ? result : null;
					}
					return null;
				}
				catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		#region IsValid
		bool IsValid(Style result) {
			return !((NativeCellStyle)result).Hidden;
		}
		#endregion
		#region DefaultStyle
		public Style DefaultStyle {
			get { return this["Normal"]; }
		}
		#endregion
		#region Remove
		public bool Remove(String name) {
			Hide(this[name]);
			return true;
		}
		public void Hide(Style style) {
			NativeCellStyle nativeStyle = (NativeCellStyle)style;
			if (nativeStyle != null) {
				((NativeCellStyle)nativeStyle).Hidden = true;
			}
		}
		public void Unhide(Style style) {
			NativeCellStyle nativeStyle = (NativeCellStyle)style;
			if (nativeStyle != null) {
				((NativeCellStyle)nativeStyle).Hidden = false;
			}
		}
		#endregion
		public bool Contains(string name) {
			return this[name] != null;
		}
		[Obsolete("All built-in styles are added to the collection by default.", true)]
		public bool Contains(BuiltInStyleId id) {
			return ContainsCore(id);
		}
		bool ContainsCore(BuiltInStyleId id) {
			string styleName = DevExpress.XtraSpreadsheet.Model.BuiltInCellStyleCalculator.GetBuiltInCellStyleInfo((int)id).CellStyleName;
			return ModelStyleSheet.CellStyles.ContainsCellStyleName(styleName);
		}
		IEnumerator<Style> IEnumerable<Style>.GetEnumerator() {
			return new EnumeratorAdapter<Style, NativeCellStyle>(this.GetEnumerator());
		}
		#region ISimpleCollection<Style>.Item
		Style ISimpleCollection<Style>.this[int index] {
			get { return base[index]; }
		}
		#endregion
		#region Count
		public new int Count {
			get {
				int count = 0;
				for (int i = 0; i < base.Count; i++)
					if (IsValid(this[i]))
						count++;
				return count;
			}
		}
		#endregion
		#region this[BuiltInStyleId id]
		public Style this[BuiltInStyleId id] {
			get {
				try {
					Model.BuiltInCellStyleInfo info = DevExpress.XtraSpreadsheet.Model.BuiltInCellStyleCalculator.GetBuiltInCellStyleInfo((int)id);
					string styleName = info.CellStyleName;
					int index = CellStyles.GetCellStyleIndexByName(styleName);
					if (index == -1)
						return Add(id);
					Style result = this[index];
					return IsValid(result) ? result : null;
				}
				catch (Exception) {
					return null;
				}
			}
		}
		#endregion
	}
	#endregion
}
