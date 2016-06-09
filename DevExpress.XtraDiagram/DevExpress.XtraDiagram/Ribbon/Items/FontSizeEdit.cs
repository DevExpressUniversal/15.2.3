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
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Internal;
using System.Globalization;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars;
namespace DevExpress.XtraDiagram.Bars {
	[ToolboxItem(false)]
	public class DiagramFontSizeEdit : ComboBoxEdit {
		static DiagramFontSizeEdit() {
			RepositoryItemDiagramFontSizeEdit.RegisterDiagramFontSizeEdit();
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return RepositoryItemDiagramFontSizeEdit.InternalEditorTypeName; } }
		[Category(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemDiagramFontSizeEdit Properties { get { return base.Properties as RepositoryItemDiagramFontSizeEdit; } }
		public DiagramControl Control {
			get { return Properties != null ? Properties.Diagram : null; }
			set { if(Properties != null) Properties.Diagram = value; }
		}
		public DiagramFontSizeEdit() {
		}
	}
	[ToolboxItem(false)]
	public class RepositoryItemDiagramFontSizeEdit : RepositoryItemComboBox {
		FontSizeCollection fontSizes;
		static RepositoryItemDiagramFontSizeEdit() {
			RegisterDiagramFontSizeEdit();
		}
		public static void RegisterDiagramFontSizeEdit() {
			EditorClassInfo editorInfo = new EditorClassInfo(InternalEditorTypeName, typeof(DiagramFontSizeEdit), typeof(RepositoryItemDiagramFontSizeEdit), typeof(ComboBoxViewInfo), new ButtonEditPainter(), true);
			editorInfo.AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars;
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		internal static string InternalEditorTypeName { get { return typeof(DiagramFontSizeEdit).Name; } }
		DiagramControl control;
		public RepositoryItemDiagramFontSizeEdit() {
			this.fontSizes = new FontSizeCollection();
		}
		#region Properties
		[Localizable(false)]
		public override ComboBoxItemCollection Items { get { return base.Items; } }
		public override string EditorTypeName { get { return InternalEditorTypeName; } }
		public DiagramControl Diagram {
			get { return control; }
			set {
				if(control == value)
					return;
				UnsubscribeEvents();
				control = value;
				SubscribeEvents();
				OnControlChanged();
			}
		}
		#endregion
		protected override bool ShouldSerializeItems() {
			if(Diagram == null) return base.ShouldSerializeItems();
			FontSizeCollection other = FontSizeCollection.Instance;
			if(other.Count != Items.Count)
				return true;
			for(int i = 0; i < other.Count; i++) {
				if(!object.Equals(Items[i], other[i])) return true;
			}
			return false;
		}
		protected internal virtual void PopulateItems() {
			Items.Clear();
			if(Diagram != null) {
				BeginUpdate();
				try {
					int count = FontSizes.Count;
					for(int i = 0; i < count; i++) {
						Items.Add(FontSizes[i]);
					}
				}
				finally {
					EndUpdate();
				}
			}
		}
		protected FontSizeCollection FontSizes { get { return fontSizes; } }
		protected virtual void OnControlChanged() {
			PopulateItems();
		}
		public override void BeginInit() {
			base.BeginInit();
			Items.Clear();
		}
		public override void EndInit() {
			if(Items.Count <= 0)
				PopulateItems();
			base.EndInit();
		}
		protected internal virtual void SubscribeEvents() { }
		protected internal virtual void UnsubscribeEvents() { }
		[Browsable(false)]
		public new DiagramFontSizeEdit OwnerEdit { get { return base.OwnerEdit as DiagramFontSizeEdit; } }
	}
	public class FontSizeEditItemUIState : BarEditItemUIState<int?> {
		public FontSizeEditItemUIState(BarEditItem item) : base(item) {
		}
		public override int? Value {
			get {
				if(Item.EditValue == null) return null;
				return (int)((float)Item.EditValue * 2);
			}
			set {
				if(value != null)
					Item.EditValue = value / 2f;
				else
					Item.EditValue = null;
			}
		}
	}
	static class FontSizeEditUtils {
		public static bool IsValidFontSize(int fontSize) {
			return fontSize >= FontSizeCollection.MinFontSize && fontSize <= FontSizeCollection.MaxFontSize * 2;
		}
		public static bool IsFontSizeValid(object edtValue, out string text, out int value) {
			text = string.Empty;
			bool isIntValue = TryGetHalfSizeValue(edtValue, out value);
			if(isIntValue) {
				if(!IsValidFontSize(value)) {
					text = "Msg_InvalidFontSize";
					return false;
				}
			}
			else {
				text = "Msg_InvalidNumber";
				return false;
			}
			return true;
		}
		public static bool IsFontSizeValid(object edtValue, out string text) {
			int value;
			return IsFontSizeValid(edtValue, out text, out value);
		}
		public static bool TryGetHalfSizeValue(object editValue, out int value) {
			value = 0;
			if(editValue is int) {
				value = (int)editValue * 2;
				return true;
			}
			float editfloat = 0;
			string editText = String.Empty;
			if(editValue is float)
				editfloat = (float)editValue;
			else
				editText = editValue as string;
			if((!string.IsNullOrEmpty(editText) && float.TryParse(editText, NumberStyles.Float, CultureInfo.CurrentCulture, out editfloat)) || editfloat > 0) {
				if(editfloat * 10 % 5 == 0) {
					value = (int)(editfloat * 2);
					return true;
				}
				return false;
			}
			return false;
		}
	}
	public class FontSizeCollection : List<int> {
		public const int MinFontSize = 1;
		public const int MaxFontSize = 1500;
		public static readonly FontSizeCollection Instance = new FontSizeCollection();
		public FontSizeCollection() {
			CreateDefaultContent();
		}
		public static int ValidateFontSize(int value) {
			return Math.Max(Math.Min(value, MaxFontSize), MinFontSize);
		}
		protected internal virtual void CreateDefaultContent() {
			foreach(int fontSize in FontManager.GetPredefinedFontSizes()) {
				Add(fontSize);
			}
		}
		public int CalculateNextFontSize(int fontSize) {
			return ValidateFontSize(CalculateNextFontSizeCore(fontSize));
		}
		protected internal virtual int CalculateNextFontSizeCore(int fontSize) {
			if(Count == 0) return fontSize + 1;
			if(fontSize < this[0]) return fontSize + 1;
			int fontSizeIndex = BinarySearch(fontSize);
			if(fontSizeIndex < 0)
				fontSizeIndex = ~fontSizeIndex;
			else
				fontSizeIndex++;
			if(fontSizeIndex < Count)
				return this[fontSizeIndex];
			return CalcNextTen(fontSize);
		}
		public int CalculatePreviousFontSize(int fontSize) {
			return ValidateFontSize(CalculatePreviousFontSizeCore(fontSize));
		}
		protected internal virtual int CalculatePreviousFontSizeCore(int fontSize) {
			if(this.Count == 0) return fontSize - 1;
			if(fontSize <= this[0]) return fontSize - 1;
			int fontSizeIndex = BinarySearch(fontSize);
			if(fontSizeIndex >= 0) return this[fontSizeIndex - 1];
			if(fontSizeIndex != ~Count)
				return this[(~fontSizeIndex) - 1];
			if(fontSize > CalcNextTen(this[Count - 1]))
				return CalcPrevTen(fontSize);
			return this[(~fontSizeIndex) - 1];
		}
		protected internal virtual int CalcNextTen(int value) {
			return value + (10 - value % 10);
		}
		protected internal virtual int CalcPrevTen(int value) {
			if(value % 10 != 0) {
				return value - (value % 10);
			}
			return value - 10;
		}
	}
}
