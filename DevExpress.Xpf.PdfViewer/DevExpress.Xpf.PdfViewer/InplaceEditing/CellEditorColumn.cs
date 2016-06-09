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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
namespace DevExpress.Xpf.PdfViewer {
	public class CellEditorColumn : IInplaceEditorColumn {
		readonly PdfEditorSettings editorSettings;
		readonly PdfBehaviorProvider provider;
		readonly PdfPageViewModel page;
		bool isReadOnly;
		public CellEditorColumn(PdfBehaviorProvider provider, PdfPageViewModel page, PdfEditorSettings editorSettings) {
			this.page = page;
			this.editorSettings = editorSettings;
			this.provider = provider;
			EditSettings = CreateEditSettings();
		}
		protected virtual BaseEditSettings CreateEditSettings() {
			switch (editorSettings.EditorType) {
				case PdfEditorType.TextEdit:
					return CreateTextEditSettings((PdfTextEditSettings)editorSettings);
				case PdfEditorType.ComboBox:
					return CreateComboBoxEditSettings((PdfComboBoxSettings)editorSettings);
				case PdfEditorType.ListBox:
					return CreateListBoxEditSettings((PdfListBoxSettings)editorSettings);
				case PdfEditorType.StickyNote:
					return CreateStickyNoteSettings((PdfStickyNoteEditSettings)editorSettings);
			}
			throw new NotImplementedException();
		}
		BaseEditSettings CreateStickyNoteSettings(PdfStickyNoteEditSettings st) {
			return new StickyNotesEditSettings() { Title = st.Title };
		}
		BaseEditSettings CreateListBoxEditSettings(PdfListBoxSettings lbs) {
			return new ListBoxEditSettings() {
				VerticalContentAlignment = VerticalAlignment.Center,
				ItemsSource = lbs.Values,
				ValueMember = "Text",
				DisplayMember = "ExportText",
				SelectionMode = lbs.Multiselect ? SelectionMode.Extended : SelectionMode.Single,
			};
		}
		BaseEditSettings CreateComboBoxEditSettings(PdfComboBoxSettings ces) {
			isReadOnly = ces.ReadOnly;
			return new ComboBoxEditSettings() {
				VerticalContentAlignment = VerticalAlignment.Center,
				ItemsSource = ces.Editable ? (IEnumerable)ces.Values.Select(x => x.Text).ToList() : ces.Values,
				ValueMember = ces.Editable ? string.Empty : "Text", DisplayMember = ces.Editable ? string.Empty : "ExportText",
				ValidateOnTextInput = !ces.Editable,
				AutoComplete = true,
			};
		}
		BaseEditSettings CreateTextEditSettings(PdfTextEditSettings tes) {
			isReadOnly = tes.ReadOnly;
			if (editorSettings.UsePasswordChar)
				return new PasswordBoxEditSettings() {
					VerticalContentAlignment = VerticalAlignment.Center,
					HorizontalContentAlignment = GetHorizontalAlignment(tes.TextJustification),
				};
			return new TextEditSettings() {
				VerticalContentAlignment = VerticalAlignment.Center,
				AcceptsReturn = tes.Multiline,
				HorizontalContentAlignment = GetHorizontalAlignment(tes.TextJustification),
				MaxLength = tes.MaxLen,
			};
		}
		EditSettingsHorizontalAlignment GetHorizontalAlignment(PdfTextJustification textJustification) {
			if (textJustification == PdfTextJustification.Centered)
				return EditSettingsHorizontalAlignment.Center;
			if (textJustification == PdfTextJustification.LeftJustified)
				return EditSettingsHorizontalAlignment.Left;
			if (textJustification == PdfTextJustification.RightJustified)
				return EditSettingsHorizontalAlignment.Right;
			return EditSettingsHorizontalAlignment.Stretch;
		}
		public bool IsReadOnly { get { return isReadOnly; } }
		public HorizontalAlignment DefaultHorizontalAlignment { get; private set; }
		bool IDefaultEditorViewInfo.HasTextDecorations { get { return false; } }
		public BaseEditSettings EditSettings { get; private set; }
		public DataTemplateSelector EditorTemplateSelector { get; private set; }
		public ControlTemplate EditTemplate { get; private set; }
		public ControlTemplate DisplayTemplate { get; private set; }
		public PdfEditorType EditorType { get { return editorSettings.EditorType; } }
		public Brush Background { get { return new SolidColorBrush(ToWpfColor(editorSettings.BackgroundColor)); } }
		public Brush Foreground { get { return new SolidColorBrush(ToWpfColor(editorSettings.FontColor)); } }
		public double FontSize { get { return provider.ZoomFactor * page.DpiMultiplier * editorSettings.FontSize; } }
		public FontFamily FontFamily { get { return editorSettings.FontData != null ? new FontFamily(editorSettings.FontData.FontFamily) : null; } }
		public Brush BorderBrush { get { return CreateBorderBrush(); } }
		public double RotateAngle { get { return provider.RotateAngle; } }
		Brush CreateBorderBrush() {
			Color color = ToWpfColor(editorSettings.Border.Color);
			Brush brush = new SolidColorBrush(color);
			if (editorSettings.Border.BorderStyle == PdfEditorBorderStyle.Dashed)
				return new VisualBrush(new Rectangle() { Stroke = brush, StrokeThickness = editorSettings.Border.BorderWidth, StrokeDashArray = new DoubleCollection(editorSettings.Border.DashPattern), Width = editorSettings.DocumentArea.Area.Width, Height = editorSettings.DocumentArea.Area.Height });
			return brush;
		}
		public Thickness BorderThickness { get { return CreateBorderThickness(); } }
		Thickness CreateBorderThickness() {
			return editorSettings.Border.BorderStyle == PdfEditorBorderStyle.Underline ? new Thickness(0, 0, 0, editorSettings.Border.BorderWidth) : new Thickness(editorSettings.Border.BorderWidth);
		}
		public CornerRadius CornerRadius { get { return new CornerRadius(editorSettings.Border.HorizontalRadius, editorSettings.Border.VerticalRadius, editorSettings.Border.HorizontalRadius, editorSettings.Border.VerticalRadius); } }
		public object InitialValue { get { return editorSettings.EditValue; } }
		public event ColumnContentChangedEventHandler ContentChanged;
		void RaiseContentChanged() {
			ContentChanged.Do(x => x(this, new ColumnContentChangedEventArgs(null)));
		}
		Color ToWpfColor(System.Drawing.Color color) {
			return Color.FromArgb(color.A, color.R, color.G, color.B);
		}
	}
}
