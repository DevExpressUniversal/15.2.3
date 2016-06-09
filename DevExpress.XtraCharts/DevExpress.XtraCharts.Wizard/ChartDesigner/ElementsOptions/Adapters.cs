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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using System.Drawing;
namespace DevExpress.XtraCharts.Designer.Native {
	public abstract class Adapter {
		INotified notified;
		bool isLock = false;
		public INotified Notified {
			get { return notified; }
			set { notified = value; }
		}
		public abstract void UpdateEditor(object value, Control editor);
		public abstract object GetEditorValue(Control editor);
		public abstract void SubscribeToChangedEvent(Control editor);
		public abstract void UnsubscribeToChangedEvent(Control editor);
		public void Lock() {
			isLock = true;
		}
		public void Unlock() {
			isLock = false;
		}
		protected void Notify() {
			if (!isLock)
				Notified.Notify();
		}
	}
	public abstract class Adapter<V, C> : Adapter
		where C : Control {
		public override void UpdateEditor(object value, Control editor) {
			C _editor = editor as C;
			if (_editor == null)
				return;
			Update(ConvertValue(value), _editor);
		}
		protected virtual V ConvertValue(object value) {
			return (V)value;
		}
		public override object GetEditorValue(Control editor) {
			C _editor = editor as C;
			return GetValue(_editor);
		}
		public override void SubscribeToChangedEvent(Control editor) {
			C _editor = editor as C;
			Subscribe(_editor);
		}
		public override void UnsubscribeToChangedEvent(Control editor) {
			C _editor = editor as C;
			Unsubscribe(_editor);
		}
		public abstract void Update(V value, C editor);
		public abstract object GetValue(C editor);
		public abstract void Subscribe(C editor);
		public abstract void Unsubscribe(C editor);
	}
	public class SizeAdapter : Adapter<Size, SizeControl> {
		void editor_ValueChanged(object sender, EventArgs e) {
			Notify();
		}
		public override void Update(Size value, SizeControl editor) {
			editor.Value = value;
		}
		public override object GetValue(SizeControl editor) {
			return editor.Value;
		}
		public override void Subscribe(SizeControl editor) {
			editor.ValueChanged += editor_ValueChanged;
		}
		public override void Unsubscribe(SizeControl editor) {
			editor.ValueChanged -= editor_ValueChanged;
		}
	}
	public class PatternControlAdapter : Adapter<string, PatternControl> {
		void editor_ValueChanged(object sender, EventArgs e) {
			Notify();
		}
		public override void Update(string value, PatternControl editor) {
			editor.Value = value;
		}
		public override object GetValue(PatternControl editor) {
			return editor.Value;
		}
		public override void Subscribe(PatternControl editor) {
			editor.ValueChanged += editor_ValueChanged;
		}
		public override void Unsubscribe(PatternControl editor) {
			editor.ValueChanged -= editor_ValueChanged;
		}
	}
	public class StackedGroupControlAdapter : Adapter<string, StackedGroupControl> {
		void editor_ValueChanged(object sender, EventArgs e) {
			Notify();
		}
		public override void Update(string value, StackedGroupControl editor) {
			editor.Value = value;
		}
		public override object GetValue(StackedGroupControl editor) {
			return editor.Value;
		}
		public override void Subscribe(StackedGroupControl editor) {
			editor.ValueChanged += editor_ValueChanged;
		}
		public override void Unsubscribe(StackedGroupControl editor) {
			editor.ValueChanged -= editor_ValueChanged;
		}
	}
	public class PaneAdapter : Adapter<XYDiagramPaneBaseModel, PaneControl> {
		void editor_PaneChanged(object sender, EventArgs e) {
			Notify();
		}
		public override void Update(XYDiagramPaneBaseModel value, PaneControl editor) {
			editor.Element = value;
			editor.UpdateComboBox();
		}
		public override object GetValue(PaneControl editor) {
			return editor.Element;
		}
		public override void Subscribe(PaneControl editor) {
			editor.ValueChanged += editor_PaneChanged;
		}
		public override void Unsubscribe(PaneControl editor) {
			editor.ValueChanged -= editor_PaneChanged;
		}
	}
	public class ChartAppearanceAdapter : Adapter<ChartAppearanceModel, AppearanceGalleryControl> {
		void editor_AppearanceChanged(object sender, EventArgs e) {
			Notify();
		}
		public override void Update(ChartAppearanceModel value, AppearanceGalleryControl editor) {
			editor.Appearance = value;
			editor.UpdateText();
		}
		public override object GetValue(AppearanceGalleryControl editor) {
			return editor.Appearance;
		}
		public override void Subscribe(AppearanceGalleryControl editor) {
			editor.AppearanceChanged += editor_AppearanceChanged;
		}
		public override void Unsubscribe(AppearanceGalleryControl editor) {
			editor.AppearanceChanged -= editor_AppearanceChanged;
		}
	}
	public class SwiftPlotAxisAdapter : Adapter<SwiftPlotDiagramAxisModel, SwiftPlotAxisControl> {
		void editor_ValueChanged(object sender, EventArgs e) {
			Notify();
		}
		public override void Update(SwiftPlotDiagramAxisModel value, SwiftPlotAxisControl editor) {
			editor.Axis = value;
		}
		public override object GetValue(SwiftPlotAxisControl editor) {
			return editor.Axis;
		}
		public override void Subscribe(SwiftPlotAxisControl editor) {
			editor.ValueChanged += editor_ValueChanged;
		}
		public override void Unsubscribe(SwiftPlotAxisControl editor) {
			editor.ValueChanged -= editor_ValueChanged;
		}
	}
	public class AxisAdapter : Adapter<AxisModel, AxisControl> {
		void editor_ValueChanged(object sender, EventArgs e) {
			Notify();
		}
		public override void Update(AxisModel value, AxisControl editor) {
			editor.Axis = value;
		}
		public override object GetValue(AxisControl editor) {
			return editor.Axis;
		}
		public override void Subscribe(AxisControl editor) {
			editor.ValueChanged += editor_ValueChanged;
		}
		public override void Unsubscribe(AxisControl editor) {
			editor.ValueChanged -= editor_ValueChanged;
		}
	}
	public class LinesAdapter : Adapter<string[], MemoEdit> {
		void editor_EditValueChanged(object sender, EventArgs e) { Notify(); }
		public override void Update(string[] value, MemoEdit editor) {
			editor.Lines = value;
		}
		public override object GetValue(MemoEdit editor) {
			return editor.Lines;
		}
		public override void Subscribe(MemoEdit editor) {
			editor.EditValueChanged += editor_EditValueChanged;
		}
		public override void Unsubscribe(MemoEdit editor) {
			editor.EditValueChanged -= editor_EditValueChanged;
		}
	}
	public class StrintgToLinesAdapter : Adapter<string, MemoEdit> {
		void editor_EditValueChanged(object sender, EventArgs e) { Notify(); }
		public override void Update(string value, MemoEdit editor) {
			editor.Lines = value.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
		}
		public override object GetValue(MemoEdit editor) {
			string str = "";
			if (editor.Lines.Length == 0)
				return str;
			str = editor.Lines[0];
			for (int i = 1; i < editor.Lines.Length; i++)
				str = str + Environment.NewLine + editor.Lines[i];
			return str;
		}
		public override void Subscribe(MemoEdit editor) {
			editor.EditValueChanged += editor_EditValueChanged;
		}
		public override void Unsubscribe(MemoEdit editor) {
			editor.EditValueChanged -= editor_EditValueChanged;
		}
	}
	public class PaletteAdapter : Adapter<Palette, PaletteGalleryControl> {
		void editor_PaletteChanged(object sender, EventArgs e) {
			Notify();
		}
		public override void Update(Palette value, PaletteGalleryControl editor) {
			editor.Palette = value;
		}
		public override object GetValue(PaletteGalleryControl editor) {
			return editor.Palette;
		}
		public override void Subscribe(PaletteGalleryControl editor) {
			editor.PaletteChanged += editor_PaletteChanged;
		}
		public override void Unsubscribe(PaletteGalleryControl editor) {
			editor.PaletteChanged -= editor_PaletteChanged;
		}
	}
	public class ColorAdapter : Adapter<Color, ColorPickEdit> {
		void editor_ColorChanged(object sender, EventArgs e) {
			Notify();
		}
		protected override Color ConvertValue(object value) {
			return value != null ? base.ConvertValue(value) : Color.Empty;
		}
		public override void Update(Color value, ColorPickEdit editor) {
			editor.Color = value;
		}
		public override object GetValue(ColorPickEdit editor) {
			return editor.Color;
		}
		public override void Subscribe(ColorPickEdit editor) {
			editor.ColorChanged += editor_ColorChanged;
		}
		public override void Unsubscribe(ColorPickEdit editor) {
			editor.ColorChanged -= editor_ColorChanged;
		}
	}
	public class BooleanAdapter : Adapter<Boolean, CheckEdit> {
		void editor_CheckedChanged(object sender, EventArgs e) {
			Notify();
		}
		public override void Update(bool value, CheckEdit editor) {
			editor.Checked = value;
		}
		public override object GetValue(CheckEdit editor) {
			return editor.Checked;
		}
		public override void Subscribe(CheckEdit editor) {
			editor.CheckedChanged += editor_CheckedChanged;
		}
		public override void Unsubscribe(CheckEdit editor) {
			editor.CheckedChanged -= editor_CheckedChanged;
		}
	}
	public class StringAdapter : Adapter<string, TextEdit> {
		void editor_TextChanged(object sender, EventArgs e) {
			Notify();
		}
		protected override string ConvertValue(object value) {
			return value != null ? value.ToString() : null;
		}
		public override void Update(string value, TextEdit editor) {
			editor.Text = value;
		}
		public override object GetValue(TextEdit editor) {
			return editor.Text;
		}
		public override void Subscribe(TextEdit editor) {
			editor.TextChanged += editor_TextChanged;
		}
		public override void Unsubscribe(TextEdit editor) {
			editor.TextChanged -= editor_TextChanged;
		}
	}
	public class DoubleAdapter : Adapter<double, SpinEdit> {
		void editor_ValueChanged(object sender, EventArgs e) {
			Notify();
		}
		public override void Update(double value, SpinEdit editor) {
			editor.Value = Convert.ToDecimal(value);
		}
		public override object GetValue(SpinEdit editor) {
			return (double)editor.Value;
		}
		public override void Subscribe(SpinEdit editor) {
			editor.ValueChanged += editor_ValueChanged;
		}
		public override void Unsubscribe(SpinEdit editor) {
			editor.ValueChanged -= editor_ValueChanged;
		}
	}
	public class IntegerAdapter : Adapter<int, SpinEdit> {
		void editor_ValueChanged(object sender, EventArgs e) {
			Notify();
		}
		public override void Update(int value, SpinEdit editor) {
			editor.Value = value;
		}
		public override object GetValue(SpinEdit editor) {
			return (int)editor.Value;
		}
		public override void Subscribe(SpinEdit editor) {
			editor.ValueChanged += editor_ValueChanged;
		}
		public override void Unsubscribe(SpinEdit editor) {
			editor.ValueChanged -= editor_ValueChanged;
		}
	}
	public class ByteAdapter : Adapter<Byte, SpinEdit> {
		void editor_ValueChanged(object sender, EventArgs e) {
			Notify();
		}
		public override void Update(Byte value, SpinEdit editor) {
			editor.Value = value;
		}
		public override object GetValue(SpinEdit editor) {
			return (Byte)editor.Value;
		}
		public override void Subscribe(SpinEdit editor) {
			editor.ValueChanged += editor_ValueChanged;
		}
		public override void Unsubscribe(SpinEdit editor) {
			editor.ValueChanged -= editor_ValueChanged;
		}
	}
	public class DefaultBooleanAdapter : Adapter<DefaultBoolean, CheckEdit> {
		void editor_CheckStateChanged(object sender, EventArgs e) {
			Notify();
		}
		public override void Update(DefaultBoolean value, CheckEdit editor) {
			switch (value) {
				case DefaultBoolean.True:
					editor.CheckState = CheckState.Checked;
					break;
				case DefaultBoolean.False:
					editor.CheckState = CheckState.Unchecked;
					break;
				case DefaultBoolean.Default:
					editor.CheckState = CheckState.Indeterminate;
					break;
			}
		}
		public override object GetValue(CheckEdit editor) {
			switch (editor.CheckState) {
				case CheckState.Checked:
					return DefaultBoolean.True;
				case CheckState.Unchecked:
					return DefaultBoolean.False;
				case CheckState.Indeterminate:
					return DefaultBoolean.Default;
			}
			return DefaultBoolean.Default;
		}
		public override void Subscribe(CheckEdit editor) {
			editor.CheckStateChanged += editor_CheckStateChanged;
		}
		public override void Unsubscribe(CheckEdit editor) {
			editor.CheckStateChanged -= editor_CheckStateChanged;
		}
	}
	public class EnumAdapter : Adapter<object, ComboBoxEdit> {
		void editor_SelectedIndexChanged(object sender, EventArgs e) {
			Notify();
		}
		public override void Update(object value, ComboBoxEdit editor) {
			foreach (EnumElementPresentation presentation in editor.Properties.Items) {
				if (presentation.Value.Equals(value)) {
					editor.SelectedItem = presentation;
					break;
				}
			}
		}
		public override object GetValue(ComboBoxEdit editor) {
			EnumElementPresentation presentation = editor.SelectedItem as EnumElementPresentation;
			return presentation != null ? presentation.Value : null;
		}
		public override void Subscribe(ComboBoxEdit editor) {
			editor.SelectedIndexChanged += editor_SelectedIndexChanged;
		}
		public override void Unsubscribe(ComboBoxEdit editor) {
			editor.SelectedIndexChanged -= editor_SelectedIndexChanged;
		}
	}
	public class EnumGalleryAdapter : Adapter<object, EnumGalleryControl> {
		void editor_SelectedItemChanged(object sender, EventArgs e) {
			Notify();
		}
		public override void Update(object value, EnumGalleryControl editor) {
			editor.Value = value;
		}
		public override object GetValue(EnumGalleryControl editor) {
			return editor.Value;
		}
		public override void Subscribe(EnumGalleryControl editor) {
			editor.SelectedItemChanged += editor_SelectedItemChanged;
		}
		public override void Unsubscribe(EnumGalleryControl editor) {
			editor.SelectedItemChanged -= editor_SelectedItemChanged;
		}
	}
}
