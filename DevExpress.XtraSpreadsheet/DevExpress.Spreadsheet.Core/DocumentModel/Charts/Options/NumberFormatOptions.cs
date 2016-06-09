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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet.Model {
	public class NumberFormatOptions : ICloneable<NumberFormatOptions>, ISupportsCopyFrom<NumberFormatOptions> {
		#region Fields
		readonly IChart parent;
		string numberFormatCode;
		bool sourceLinked;
		NumberFormat formatter;
		#endregion
		public NumberFormatOptions(IChart parent) {
			this.parent = parent;
			numberFormatCode = string.Empty;
			sourceLinked = true;
			formatter = NumberFormat.Generic;
		}
		protected internal IChart Parent { get { return parent; } }
		protected internal DocumentModel DocumentModel { get { return parent.DocumentModel; } }
		public int NumberFormatId {
			get { return DocumentModel.Cache.NumberFormatCache.GetItemIndex(formatter); }
		}
		#region NumberFormatCode
		public string NumberFormatCode {
			get { return numberFormatCode; }
			set {
				if (string.IsNullOrEmpty(value))
					value = string.Empty;
				if (numberFormatCode == value)
					return;
				SetNumberFormatCode(value);
			}
		}
		void SetNumberFormatCode(string value) {
			NumberFormatCodePropertyChangedHistoryItem historyItem = new NumberFormatCodePropertyChangedHistoryItem(DocumentModel, this, this.numberFormatCode, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetNumberFormatCodeCore(string value) {
			numberFormatCode = value;
			if (string.IsNullOrEmpty(value) || StringExtensions.CompareInvariantCultureIgnoreCase("General", value) == 0)
				formatter = NumberFormat.Generic;
			else {
				NumberFormat format = NumberFormatParser.Parse(value);
				int index = DocumentModel.Cache.NumberFormatCache.AddItem(format);
				formatter = DocumentModel.Cache.NumberFormatCache[index];
			}
			Parent.Invalidate();
		}
		#endregion
		#region SourceLinked
		public bool SourceLinked {
			get { return sourceLinked; }
			set {
				if (SourceLinked == value)
					return;
				SetSourceLinked(value);
			}
		}
		void SetSourceLinked(bool value) {
			NumberFormatSourceLinkedPropertyChangedHistoryItem historyItem = new NumberFormatSourceLinkedPropertyChangedHistoryItem(DocumentModel, this, this.sourceLinked, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetSourceLinkedCore(bool value) {
			sourceLinked = value;
			Parent.Invalidate();
		}
		#endregion
		#region ICloneable<NumberFormatOptions> Members
		public NumberFormatOptions Clone() {
			NumberFormatOptions result = new NumberFormatOptions(this.parent);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		protected internal NumberFormat Formatter { get { return formatter; } }
		#region Equals
		public override bool Equals(object obj) {
			NumberFormatOptions other = obj as NumberFormatOptions;
			if (other == null)
				return false;
			return sourceLinked == other.sourceLinked && numberFormatCode == other.numberFormatCode;
		}
		public override int GetHashCode() {
			return sourceLinked.GetHashCode() ^ numberFormatCode.GetHashCode() ^ formatter.GetHashCode();
		}
		#endregion
		#region ISupportsCopyFrom<NumberFormatOptions> Members
		public void CopyFrom(NumberFormatOptions value) {
			Guard.ArgumentNotNull(value, "value");
			SourceLinked = value.SourceLinked;
			NumberFormatCode = value.NumberFormatCode;
		}
		#endregion
	}
	public class ChartNumberFormat {
		public string FormatCode { get; set; }
		public bool SourceLinked { get; set; }
		public static ChartNumberFormat Clone(ChartNumberFormat other) {
			if (other == null)
				return null;
			ChartNumberFormat result = new ChartNumberFormat();
			result.SourceLinked = other.SourceLinked;
			result.FormatCode = other.FormatCode;
			return result;
		}
	}
}
