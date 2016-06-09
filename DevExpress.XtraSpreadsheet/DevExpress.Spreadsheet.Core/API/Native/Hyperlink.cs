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
using DevExpress.Office;
using DevExpress.Utils;
namespace DevExpress.Spreadsheet {
	#region HyperlinkBase
	public interface HyperlinkBase {
		string TooltipText { get; set; }
		string Uri { get; }
		bool IsExternal { get; }
		void SetUri(string uri, bool isExternal);
	}
	#endregion
	#region Hyperlink
	public interface Hyperlink : HyperlinkBase {
		Range Range { get; }
		string DisplayText { get; set; }
		void SetUri(string uri, bool isExternal, string displayText);
	}
	#endregion
	public interface HyperlinkCollection : ISimpleCollection<Hyperlink> {
		Hyperlink Add(Range range, string uri, bool isExternal, string displayText);
		Hyperlink Add(Range range, string uri, bool isExternal);
		int IndexOf(Hyperlink hyperlink);
		void Remove(Hyperlink hyperlink);
		void RemoveAt(int index);
		void Clear();
		bool Contains(Hyperlink hyperlink);
		IList<Hyperlink> GetHyperlinks(Range range);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Office.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.XtraSpreadsheet.Utils;
	using ModelHyperlink = DevExpress.XtraSpreadsheet.Model.ModelHyperlink;
	using DevExpress.Spreadsheet;
	using System.Collections;
	#region NativeHyperlink
	partial class NativeHyperlink : Hyperlink {
		readonly NativeWorksheet worksheet;
		readonly ModelHyperlink modelHyperlink;
		bool isValid;
		public NativeHyperlink(NativeWorksheet worksheet, ModelHyperlink modelHyperlink) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			Guard.ArgumentNotNull(modelHyperlink, "modelHyperlink");
			this.worksheet = worksheet;
			this.modelHyperlink = modelHyperlink;
			this.isValid = true;
		}
		#region Properties
		NativeWorksheet Worksheet { get { return this.worksheet; } }
		protected internal Model.Worksheet ModelWorksheet { get { return Worksheet.ModelWorksheet; } }
		protected internal Model.DocumentModel ModelWorkbook { get { return ModelWorksheet.Workbook; } }
		protected internal Model.ModelHyperlink ModelHyperlink { get { CheckValid(); return modelHyperlink; } }
		protected internal bool IsValid { get { return isValid; } }
		public Range Range { get { return new NativeRange(ModelHyperlink.Range, worksheet); } }
		public string DisplayText { get { return ModelHyperlink.DisplayText; } set { ModelHyperlink.DisplayText = value; } }
		public string TooltipText { get { return ModelHyperlink.TooltipText; } set { ModelHyperlink.TooltipText = value; } }
		public bool IsExternal { get { return ModelHyperlink.IsExternal; } }
		public string Uri { get { return ModelHyperlink.TargetUri; } }
		#endregion
		#region CheckValid
		void CheckValid() {
			if (!IsValid)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorUseDeletedHyperlink);
		}
		#endregion
		#region Invalidate
		protected internal void Invalidate() {
			CheckValid();
			isValid = false;
		}
		#endregion
		#region SetUri
		public void SetUri(string uri, bool isExternal) {
			SetUri(uri, isExternal, uri);
		}
		#endregion
		#region SetUri
		public void SetUri(string uri, bool isExternal, string displayText) {
			ModelHyperlink.IsExternal = isExternal;
			ModelHyperlink.SetTargetUriWithoutHistory(uri);
			ModelHyperlink.DisplayText = displayText;
		}
		#endregion
	}
	#endregion
	#region NativeHyperlinkCollection
	partial class NativeHyperlinkCollection : NativeCollectionForUndoableCollectionBase<Hyperlink, NativeHyperlink, ModelHyperlink>, HyperlinkCollection {
		public NativeHyperlinkCollection(NativeWorksheet worksheet)
			: base(worksheet) {
		}
		public override IEnumerable<ModelHyperlink> GetModelItemEnumerable() {
			return ModelWorksheet.Hyperlinks.InnerList;
		}
		public override int ModelCollectionCount {
			get { return ModelWorksheet.Hyperlinks.Count; }
		}
		protected override NativeHyperlink CreateNativeObject(ModelHyperlink modelHyperlink) {
			return new NativeHyperlink(Worksheet, modelHyperlink);
		}
		protected override void RemoveModelObjectAt(int index) {
			ModelWorksheet.RemoveHyperlinkAt(index);
		}
		protected override void ClearModelObjects() {
			ModelWorksheet.ClearHyperlinks();
		}
		protected override void InvalidateItem(NativeHyperlink item) {
			item.Invalidate();
		}
		protected override UndoableCollection<ModelHyperlink> GetModelCollection() {
			return ModelWorksheet.Hyperlinks;
		}
		#region Add
		public Hyperlink Add(Range range, string uri, bool isExternal, string displayText) {
			Worksheet.ValidateRange(range);
			Model.CellRange modelRange = Worksheet.GetModelSingleRange(range);
			Model.InsertHyperlinkCommand command = new Model.InsertHyperlinkCommand(ApiErrorHandler.Instance, ModelWorksheet, modelRange, uri, isExternal, displayText, false);
			command.SetDisplayTextToTopLeftCell = true;
			command.SetHyperlinkStyleToRange = true;
			command.Execute();
			return InnerList[Count - 1];
		}
		#endregion
		#region Add
		public Hyperlink Add(Range range, string uri, bool isExternal) {
			return Add(range, uri, isExternal, uri);
		}
		#endregion
		#region GetHyperlink
		protected internal Hyperlink GetHyperlink(NativeCell nativeCell) {
			int index = ModelWorksheet.Hyperlinks.GetHyperlink(nativeCell.ReadOnlyModelCell);
			if (index >= 0)
				return InnerList[index];
			return null;
		}
		#endregion
		#region GetHyperlinks
		public IList<Hyperlink> GetHyperlinks(Range range) {
			List<Hyperlink> result = new List<Hyperlink>();
			for (int i = Count - 1; i >= 0; i--) {
				NativeHyperlink hyperlink = InnerList[i] as NativeHyperlink;
				Model.CellRange hyperlinkRange = hyperlink.ModelHyperlink.Range;
				if (Worksheet.GetModelRange(range).Intersects(hyperlinkRange))
					result.Add(hyperlink);
			}
			return result;
		}
		#endregion
	}
	#endregion
}
