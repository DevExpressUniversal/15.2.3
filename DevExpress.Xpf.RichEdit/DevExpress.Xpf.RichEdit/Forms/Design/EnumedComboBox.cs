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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Native;
namespace DevExpress.Xpf.RichEdit.UI {
	[DXToolboxBrowsableAttribute(false)]
	public class EnumedComboBox<T> : ComboBoxEdit, IDisposable where T : struct {
		protected internal T? TypedEditValue {
			get {
				if (EditValue == null)
					return null;
				if (EditValue is T)
					return (T)EditValue;
				else
					return default(T);
			}
			set {
				EditValue = value;
			}
		}
		public EnumedComboBox() {
			PopulateCombo();
			IsTextEditable = false;
			TypedEditValue = null;
			DisplayMember = "Text";
			ValueMember = "Value";
		}
		protected virtual void PopulateCombo() {
			foreach (T item in DevExpress.Utils.EnumExtensions.GetValues(typeof(T))) {
				LookUpEditEnumItem<T> customItem = new LookUpEditEnumItem<T>();
				customItem.Value = item;
				string keyString = String.Format("Caption_{0}_{1}", typeof(T).Name, item.ToString());
				XtraRichEditStringId stringId;
				string localizedString;
				if (Enum.TryParse<XtraRichEditStringId>(keyString, out stringId))
					localizedString = XtraRichEditLocalizer.GetString(stringId);
				else
					localizedString = String.Empty;
				if (String.IsNullOrEmpty(localizedString))
					customItem.Text = item.ToString();
				else
					customItem.Text = localizedString;
				Items.Add(customItem);
			}
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
	[DXToolboxBrowsableAttribute(false)]
	public class AlignmentEdit : EnumedComboBox<ParagraphAlignment> {
		public ParagraphAlignment? Alignment { get { return TypedEditValue; } set { TypedEditValue = value; } }
	}
	[DXToolboxBrowsableAttribute(false)]
	public class FirstLineIndentTypeEdit : EnumedComboBox<ParagraphFirstLineIndent> {
		public ParagraphFirstLineIndent? Value { get { return TypedEditValue; } set { TypedEditValue = value; } }
	}
	[DXToolboxBrowsableAttribute(false)]
	public class LineSpacingEdit : EnumedComboBox<ParagraphLineSpacing> {
		public ParagraphLineSpacing? LineSpacing { get { return TypedEditValue; } set { TypedEditValue = value; } }
	}
}
