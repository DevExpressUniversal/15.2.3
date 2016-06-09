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
namespace DevExpress.Spreadsheet {
	#region BuiltInTableStyleId
	public enum BuiltInTableStyleId {
		TableStyleLight1 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight1,
		TableStyleLight2 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight2,
		TableStyleLight3 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight3,
		TableStyleLight4 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight4,
		TableStyleLight5 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight5,
		TableStyleLight6 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight6,
		TableStyleLight7 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight7,
		TableStyleLight8 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight8,
		TableStyleLight9 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight9,
		TableStyleLight10 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight10,
		TableStyleLight11 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight11,
		TableStyleLight12 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight12,
		TableStyleLight13 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight13,
		TableStyleLight14 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight14,
		TableStyleLight15 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight15,
		TableStyleLight16 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight16,
		TableStyleLight17 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight17,
		TableStyleLight18 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight18,
		TableStyleLight19 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight19,
		TableStyleLight20 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight20,
		TableStyleLight21 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleLight21,
		TableStyleMedium1 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium1,
		TableStyleMedium2 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium2,
		TableStyleMedium3 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium3,
		TableStyleMedium4 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium4,
		TableStyleMedium5 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium5,
		TableStyleMedium6 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium6,
		TableStyleMedium7 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium7,
		TableStyleMedium8 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium8,
		TableStyleMedium9 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium9,
		TableStyleMedium10 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium10,
		TableStyleMedium11 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium11,
		TableStyleMedium12 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium12,
		TableStyleMedium13 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium13,
		TableStyleMedium14 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium14,
		TableStyleMedium15 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium15,
		TableStyleMedium16 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium16,
		TableStyleMedium17 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium17,
		TableStyleMedium18 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium18,
		TableStyleMedium19 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium19,
		TableStyleMedium20 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium20,
		TableStyleMedium21 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium21,
		TableStyleMedium22 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium22,
		TableStyleMedium23 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium23,
		TableStyleMedium24 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium24,
		TableStyleMedium25 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium25,
		TableStyleMedium26 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium26,
		TableStyleMedium27 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium27,
		TableStyleMedium28 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleMedium28,
		TableStyleDark1 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleDark1,
		TableStyleDark2 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleDark2,
		TableStyleDark3 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleDark3,
		TableStyleDark4 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleDark4,
		TableStyleDark5 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleDark5,
		TableStyleDark6 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleDark6,
		TableStyleDark7 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleDark7,
		TableStyleDark8 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleDark8,
		TableStyleDark9 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleDark9,
		TableStyleDark10 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleDark10,
		TableStyleDark11 = DevExpress.XtraSpreadsheet.Model.PredefinedTableStyleId.TableStyleDark11,
	} 
	#endregion
	public interface TableStyleCollection { 
		TableStyle Add(string name);
		void Remove(string name);
		int Count { get; }
		TableStyle this[string name] { get; }
		TableStyle this[BuiltInTableStyleId id] { get; }
		TableStyle this[BuiltInPivotStyleId id] { get; }
		TableStyle DefaultStyle { get; set; }
		TableStyle DefaultPivotStyle { get; set; }
		bool Contains(string name);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	#region Usings
	using System.Collections.Generic;
	using DevExpress.Spreadsheet;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.XtraSpreadsheet.Utils;
	#endregion
	partial class NativeTableStyleCollection : TableStyleCollection {
		#region Fields
		readonly NativeWorkbook nativeWorkbook;
		public const int BuiltInTableStylesCount = 60; 
		public const int BuiltInPivotStylesCount = 84;
		readonly Dictionary<BuiltInTableStyleId, NativeTableStyle> cachedBuiltInTableStyles;
		readonly Dictionary<BuiltInPivotStyleId, NativeTableStyle> cachedBuiltInPivotStyles;
		readonly Dictionary<string, NativeTableStyle> cachedCustomStyles;
		#endregion
		public NativeTableStyleCollection(NativeWorkbook nativeWorkbook) {
			this.nativeWorkbook = nativeWorkbook;
			this.cachedBuiltInTableStyles = new Dictionary<BuiltInTableStyleId, NativeTableStyle>();
			this.cachedBuiltInPivotStyles = new Dictionary<BuiltInPivotStyleId, NativeTableStyle>();
			this.cachedCustomStyles = new Dictionary<string, NativeTableStyle>();
		}
		protected internal int CachedBuiltInTableStylesCount { get { return cachedBuiltInTableStyles.Count; } }
		protected internal int CachedBuiltInPivotStylesCount { get { return cachedBuiltInPivotStyles.Count; } }
		protected internal int CachedCustomStylesCount { get { return cachedCustomStyles.Count; } }
		Model.TableStyleCollection ModelTableStyles { get { return nativeWorkbook.DocumentModel.StyleSheet.TableStyles; } }
		public int Count { get { return cachedCustomStyles.Count + BuiltInTableStylesCount + BuiltInPivotStylesCount; } }
		public TableStyle DefaultStyle {
			get {
				if (ModelTableStyles.Count == 0)
					return null;
				return new NativeTableStyle(nativeWorkbook, ModelTableStyles.DefaultTableStyle);
			}
			set {
				NativeTableStyle nativeStyle = value as NativeTableStyle;
				if (!nativeStyle.IsTableStyle)
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorInvalidTableStyleType);
				if (nativeStyle != null && Object.ReferenceEquals(nativeStyle.ModelTableStyle.DocumentModel, nativeWorkbook.DocumentModel))
					ModelTableStyles.SetDefaultTableStyleName(value.Name);
				else
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorUseTableStyleFromAnotherWorkbook);
			}
		}
		public TableStyle DefaultPivotStyle {
			get {
				if (ModelTableStyles.Count == 0)
					return null;
				return new NativeTableStyle(nativeWorkbook, ModelTableStyles.DefaultPivotStyle);
			}
			set {
				NativeTableStyle nativeStyle = value as NativeTableStyle;
				if (!nativeStyle.IsPivotStyle)
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorInvalidTableStyleType);
				if (nativeStyle != null && Object.ReferenceEquals(nativeStyle.ModelTableStyle.DocumentModel, nativeWorkbook.DocumentModel))
					ModelTableStyles.SetDefaultPivotStyleName(value.Name);
				else
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorUseTableStyleFromAnotherWorkbook);
			}
		}
		public TableStyle this[BuiltInTableStyleId id] {
			get {
				NativeTableStyle result;
				if (!cachedBuiltInTableStyles.TryGetValue(id, out result)) {
					Model.PredefinedTableStyleId modelId = (Model.PredefinedTableStyleId)id;
					ModelTableStyles.AddCore(Model.TableStyle.CreateTablePredefinedStyle(nativeWorkbook.DocumentModel, modelId));
					return cachedBuiltInTableStyles[id];
				}
				return result;
			}
		}
		public TableStyle this[BuiltInPivotStyleId id] {
			get {
				NativeTableStyle result;
				if (!cachedBuiltInPivotStyles.TryGetValue(id, out result)) {
					Model.PredefinedPivotStyleId modelId = (Model.PredefinedPivotStyleId)id;
					ModelTableStyles.AddCore(Model.TableStyle.CreatePivotPredefinedStyle(nativeWorkbook.DocumentModel, modelId));
					return cachedBuiltInPivotStyles[id];
				}
				return result;
			}
		}
		public TableStyle this[string name] {
			get {
				if (!ModelTableStyles.ContainsStyleName(name))
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorStyleNotFound);
				return new NativeTableStyle(nativeWorkbook, ModelTableStyles[name]);
			}
		}
		public TableStyle Add(string name) {
			if (ModelTableStyles.ContainsStyleName(name) || Model.TableStyleName.CheckPredefinedName(name) || Model.TableStyleName.CheckDefaultStyleName(name))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorDuplicateStyleName);
			Model.TableStyle customTableStyle = Model.TableStyle.CreateCustomStyle(nativeWorkbook.DocumentModel, name, Model.TableStyleElementIndexTableType.General);
			ModelTableStyles.Add(customTableStyle);
			return this[name];
		}
		public void Remove(string name) {
			if (Model.TableStyleName.CheckPredefinedName(name))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorDeletingBuiltInTableStyle);
			ModelTableStyles[name].Remove();
		}
		public bool Contains(string name) {
			if (Model.TableStyleName.CheckPredefinedName(name))
				return true;
			return ModelTableStyles.ContainsStyleName(name);
		}
		public void PopulateStyles() {
			ClearCore();
			ModelTableStyles.ForEach(RegisterModelStyle);
		}
		public void OnTableStyles_StyleAdded(object sender, DevExpress.XtraSpreadsheet.Model.TableStyleCollectionChangedEventArgs args) {
			RegisterModelStyle(args.NewStyle);
		}
		public void OnTableStyles_StyleRemoved(object sender, DevExpress.XtraSpreadsheet.Model.TableStyleCollectionChangedEventArgs args) {
			UnRegisterModelStyle(args.NewStyle);
		}
		public void OnTableStyles_Cleared(object sender, EventArgs args) {
			ClearCore();
		}
		protected internal void ClearCore() {
			foreach (var item in cachedBuiltInTableStyles.Values)
				item.IsValid = false;
			cachedBuiltInTableStyles.Clear();
			foreach (var item in cachedBuiltInPivotStyles.Values)
				item.IsValid = false;
			cachedBuiltInPivotStyles.Clear();
			foreach (var item in cachedCustomStyles.Values)
				item.IsValid = false;
			cachedCustomStyles.Clear();
		}
		void RegisterModelStyle(Model.TableStyle modelStyle) {
			Model.TableStyleName newStyleName = modelStyle.Name;
			NativeTableStyle newNativeStyle = new NativeTableStyle(this.nativeWorkbook, modelStyle);
			if (newStyleName.IsPredefined) {
				int modelId = newStyleName.Id.Value;
				if (modelStyle.IsTableStyle) {
					BuiltInTableStyleId tableStyleId = (BuiltInTableStyleId)modelId;
					if (!cachedBuiltInTableStyles.ContainsKey(tableStyleId))
						cachedBuiltInTableStyles.Add(tableStyleId, newNativeStyle);
				}
				else {
					BuiltInPivotStyleId pivotStyleId = (BuiltInPivotStyleId)modelId;
					if (!cachedBuiltInPivotStyles.ContainsKey(pivotStyleId))
						cachedBuiltInPivotStyles.Add(pivotStyleId, newNativeStyle);
				}
			}
			else
				cachedCustomStyles.Add(newStyleName.Name, newNativeStyle);
		}
		void UnRegisterModelStyle(Model.TableStyle deletedModelStyle) {
			NativeTableStyle deletedNativeBuiltInStyle;
			if (deletedModelStyle.IsPredefined) {
				int modelId = deletedModelStyle.Name.Id.Value;
				if (deletedModelStyle.IsTableStyle) {
					BuiltInTableStyleId tableStyleId = (BuiltInTableStyleId)modelId;
					deletedNativeBuiltInStyle = cachedBuiltInTableStyles[tableStyleId];
					cachedBuiltInTableStyles.Remove(tableStyleId);
				}
				else {
					BuiltInPivotStyleId pivotStyleId = (BuiltInPivotStyleId)modelId;
					deletedNativeBuiltInStyle = cachedBuiltInPivotStyles[pivotStyleId];
					cachedBuiltInPivotStyles.Remove(pivotStyleId);
				}
			}
			else {
				deletedNativeBuiltInStyle = cachedCustomStyles[deletedModelStyle.Name.Name];
				cachedCustomStyles.Remove(deletedModelStyle.Name.Name);
			}
			deletedNativeBuiltInStyle.IsValid = false;
		}
	}
}
