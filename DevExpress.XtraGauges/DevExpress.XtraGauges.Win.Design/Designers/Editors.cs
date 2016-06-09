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
using System.Text;
using System.ComponentModel.Design;
using System.Collections;
using DevExpress.XtraGauges.Core.Model;
using System.Drawing.Design;
using DevExpress.XtraGauges.Core.Drawing;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design.Serialization;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Win.Base;
using DevExpress.XtraGauges.Win.Gauges.Circular;
using DevExpress.XtraGauges.Win.Gauges.Linear;
using DevExpress.XtraGauges.Win.Gauges.Digital;
using DevExpress.XtraGauges.Win.Gauges.State;
using DevExpress.XtraEditors;
namespace DevExpress.XtraGauges.Design {
	public static class DesignerMessages {
		public static string CollectionEditorWarning =
			"Editing this Collection via the Collection Editor is strongly not recommended.\r\n" +
			"Please use the Visual Designer instead. Do you still want to edit the collection via the Collection Editor?";
	}
	public class ComponentCollectionEditor : CollectionEditor {
		public ComponentCollectionEditor(Type type)
			: base(type) {
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			bool fUserEdit = MessageBox.Show(DesignerMessages.CollectionEditorWarning, "Components", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
			return fUserEdit ? base.EditValue(context, provider, value) : null;
		}
		protected override object SetItems(object editValue, object[] value) {
			if(editValue != null || !(editValue is IList)) {
				IList list = editValue as IList;
				ClearList(list, value);
				for(int i = 0; i < value.Length; i++) {
					if(!list.Contains(value[i])) list.Add(value[i]);
				}
			}
			return editValue;
		}
		protected override void DestroyInstance(object instance) {
			IGauge gauge = (Context != null) ? Context.Instance as IGauge : null;
			CheckRemoveGaugeElement(gauge, instance);
			base.DestroyInstance(instance);
		}
		protected void ClearList(IList list, object[] newObjects) {
			ArrayList existingItems = new ArrayList(list);
			ArrayList newItems = new ArrayList(newObjects);
			for(int i = 0; i < existingItems.Count; i++) {
				object item = existingItems[i];
				if(!newItems.Contains(item)) 
					list.Remove(item);
			}
		}
		void CheckRemoveGaugeElement(IGauge gauge, object item) {
			Core.Base.BaseElement<Core.Primitive.IRenderableElement> element =
				item as Core.Base.BaseElement<Core.Primitive.IRenderableElement>;
			if(gauge != null && element != null)
				gauge.RemoveGaugeElement(element);
		}
	}
	public class GaugeCollectionEditor : CollectionEditor {
		public GaugeCollectionEditor(Type type)
			: base(type) {
		}
		protected override object CreateInstance(Type type) {
			using(GaugeTypeSelectionForm dropDown = new GaugeTypeSelectionForm()) {
				if(dropDown.ShowDialog() == DialogResult.OK)
					return dropDown.GaugeKind.CreateInstance();
			}
			return null;
		}
		protected override Type CreateCollectionItemType() {
			return typeof(IGauge);
		}
		protected override CollectionForm CreateCollectionForm() {
			CollectionForm form = base.CreateCollectionForm();
			form.Text = "Gauges";
			return form;
		}
		protected override IList GetObjectsFromInstance(object instance) {
			if(instance != null) return base.GetObjectsFromInstance(instance);
			return new ArrayList();
		}
	}
	public class IndicatorStateCollectionEditor : CollectionEditor {
		public IndicatorStateCollectionEditor(Type type)
			: base(type) {
		}
		protected override object CreateInstance(Type type) {
			if(Context != null && (
					Context.Instance is IScaleComponent ||
					Context.Instance is ArcScaleStateIndicatorComponentWrapper ||
					Context.Instance is LinearScaleStateIndicatorComponentWrapper )
				)
				return new ScaleIndicatorState();
			else return new IndicatorState();
		}
		protected override Type CreateCollectionItemType() {
			return typeof(IIndicatorState);
		}
		protected override CollectionForm CreateCollectionForm() {
			CollectionForm form = base.CreateCollectionForm();
			form.Text = "Indicator States";
			return form;
		}
	}
	public class ImageIndicatorStateCollectionEditor : CollectionEditor {
		public ImageIndicatorStateCollectionEditor(Type type)
			: base(type) {
		}
		protected override object CreateInstance(Type type) {
			if(Context != null && (
					Context.Instance is IScaleComponent ||
					Context.Instance is StateImageIndicatorComponentWrapper)
				)
				return new ImageIndicatorState();
			else return new IndicatorState();
		}
		protected override Type CreateCollectionItemType() {
			return typeof(IIndicatorState);
		}
		protected override CollectionForm CreateCollectionForm() {
			CollectionForm form = base.CreateCollectionForm();
			form.Text = "Image Indicator States";
			return form;
		}
	}
	public class LabelCollectionEditor : CollectionEditor {
		public LabelCollectionEditor(Type type)
			: base(type) {
		}
		protected override object CreateInstance(Type type) {
			return new ScaleLabel();
		}
		protected override Type CreateCollectionItemType() {
			return typeof(ILabel);
		}
		protected override CollectionForm CreateCollectionForm() {
			CollectionForm form = base.CreateCollectionForm();
			form.Text = "Scale Labels";
			return form;
		}
	}
	public class RangeCollectionEditor : CollectionEditor {
		public RangeCollectionEditor(Type type)
			: base(type) {
		}
		protected override object CreateInstance(Type type) {
			if(Context == null) return null;
			return (Context.Instance is ArcScaleComponent || Context.Instance is ArcScaleComponentWrapper)
				? (IRange)new ArcScaleRange() :
				(IRange)new LinearScaleRange();
		}
		protected override Type CreateCollectionItemType() {
			return typeof(IRange);
		}
		protected override CollectionForm CreateCollectionForm() {
			CollectionForm form = base.CreateCollectionForm();
			form.Text = "Scale Ranges";
			return form;
		}
	}
	public class ShaderObjectTypeEditor : UITypeEditor {
		readonly static BaseColorShader emptyShader;
		readonly static BaseColorShader grayShader;
		readonly static BaseColorShader opacityShader;
		readonly static BaseColorShader styleShader;
		readonly static BaseColorShader complexShader;
		static ShaderObjectTypeEditor() {
			emptyShader = BaseColorShader.Empty;
			grayShader = new GrayShader();
			opacityShader = new OpacityShader();
			styleShader = new StyleShader();
			complexShader = new ComplexShader();
		}
		IWindowsFormsEditorService editorService = null;
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if((context != null) && (context.Instance != null)) {
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if(editorService == null) return value;
			object newValue = value;
			using(EditorDropDownForm dropDown = new EditorDropDownForm(this, editorService, GetShaderObjectType(value as BaseColorShader), Enum.GetValues(typeof(ShaderObjectType)))) {
				editorService.DropDownControl(dropDown);
				newValue = GetShaderObject(value as BaseColorShader, (ShaderObjectType)dropDown.EditValue);
			}
			return newValue;
		}
		ShaderObjectType GetShaderObjectType(BaseColorShader shader) {
			if(shader.IsEmpty) return ShaderObjectType.EmptyShader;
			if(shader is GrayShader) return ShaderObjectType.GrayShader;
			if(shader is OpacityShader) return ShaderObjectType.OpacityShader;
			if(shader is ComplexShader) return ShaderObjectType.ComplexShader;
			if(shader is StyleShader) return ShaderObjectType.StyleShader;
			return ShaderObjectType.EmptyShader;
		}
		object GetShaderObject(BaseColorShader value, ShaderObjectType newValueType) {
			ShaderObjectType valueType = GetShaderObjectType(value);
			if(valueType == newValueType) return value;
			switch(newValueType) {
				case ShaderObjectType.GrayShader: return grayShader.Clone();
				case ShaderObjectType.OpacityShader: return opacityShader.Clone();
				case ShaderObjectType.StyleShader: return styleShader.Clone();
				case ShaderObjectType.ComplexShader: return complexShader.Clone();
				case ShaderObjectType.EmptyShader:
				default: return emptyShader.Clone();
			}
		}
		void listBox_SelectedIndexChanged(object sender, EventArgs e) {
			if(editorService != null) editorService.CloseDropDown();
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	public class BrushObjectTypeEditor : UITypeEditor {
		readonly static BrushObject emptyBrushObject;
		readonly static BrushObject solidBrushObject;
		readonly static BrushObject linearBrushObject;
		readonly static BrushObject ellipseBrushObject;
		readonly static LinearGradientBrush linearBrush;
		readonly static PathGradientBrush pathBrush;
		static BrushObjectTypeEditor() {
			emptyBrushObject = BrushObject.Empty;
			solidBrushObject = new SolidBrushObject();
			linearBrushObject = new LinearGradientBrushObject();
			ellipseBrushObject = new EllipticalGradientBrushObject();
			linearBrush = new LinearGradientBrush(new Rectangle(0, 0, 20, 14), Color.White, Color.Black, LinearGradientMode.Horizontal);
			using(GraphicsPath gPath = new GraphicsPath()) {
				gPath.AddEllipse(new RectangleF(-5, -5, 30, 24));
				pathBrush = new PathGradientBrush(gPath);
				pathBrush.CenterColor = Color.White;
				pathBrush.CenterPoint = new PointF(10, 7);
				pathBrush.SurroundColors = new Color[] { Color.Black };
			}
		}
		IWindowsFormsEditorService editorService = null;
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if((context != null) && (context.Instance != null)) {
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if(editorService == null) return value;
			object newValue = value;
			using(EditorDropDownForm dropDown = new EditorDropDownForm(this, editorService, GetBrushObjectType(value as BrushObject), Enum.GetValues(typeof(BrushObjectType)))) {
				editorService.DropDownControl(dropDown);
				newValue = GetBrushObject(value as BrushObject, (BrushObjectType)dropDown.EditValue);
			}
			return newValue;
		}
		BrushObjectType GetBrushObjectType(BrushObject brush) {
			if(brush is SolidBrushObject) return BrushObjectType.Solid;
			if(brush is LinearGradientBrushObject) return BrushObjectType.LinearGradient;
			if(brush is EllipticalGradientBrushObject) return BrushObjectType.EllipseGradient;
			return BrushObjectType.Empty;
		}
		object GetBrushObject(BrushObject value, BrushObjectType newValueType) {
			BrushObjectType valueType = GetBrushObjectType(value);
			if(valueType == newValueType) return value;
			switch(newValueType) {
				case BrushObjectType.Solid: return solidBrushObject.Clone();
				case BrushObjectType.LinearGradient: return linearBrushObject.Clone();
				case BrushObjectType.EllipseGradient: return ellipseBrushObject.Clone();
				case BrushObjectType.Empty:
				default:
					return emptyBrushObject.Clone();
			}
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e) {
			BrushObject brush = e.Value as BrushObject;
			SmoothingMode mode = e.Graphics.SmoothingMode;
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			if(brush == null) return;
			if(brush.IsEmpty) e.Graphics.FillRectangle(Brushes.White, e.Bounds);
			if(brush is SolidBrushObject) e.Graphics.FillRectangle(Brushes.Gray, e.Bounds);
			if(brush is LinearGradientBrushObject) e.Graphics.FillRectangle(linearBrush, e.Bounds);
			if(brush is EllipticalGradientBrushObject) e.Graphics.FillRectangle(pathBrush, e.Bounds);
			e.Graphics.SmoothingMode = mode;
		}
	}
	[ToolboxItem(false)]
	public class EditorDropDownForm : Panel {
		UITypeEditor editorCore;
		IWindowsFormsEditorService editorService;
		object editValueCore;
		ListBox listBoxCore;
		object originalValueCore;
		public EditorDropDownForm(UITypeEditor editor, IWindowsFormsEditorService edSvc, object editValue, Array values) {
			this.listBoxCore = new ListBox();
			this.editValueCore = this.originalValueCore = editValue;
			this.editorCore = editor;
			base.BorderStyle = BorderStyle.None;
			this.editorService = edSvc;
			List.BorderStyle = BorderStyle.None;
			List.Dock = DockStyle.Fill;
			foreach(object obj in values)
				List.Items.Add(obj);
			if(List.Items.Contains(EditValue))
				List.SelectedItem = EditValue;
			else
				List.SelectedIndex = 1;
			List.SelectedValueChanged += OnSelectedValueChanged;
			List.Click += OnClick;
			Controls.Add(List);
			List.CreateControl();
			Size = new Size(0, List.ItemHeight * Math.Min(List.Items.Count, 10));
		}
		void OnClick(object sender, EventArgs e) {
			this.editorService.CloseDropDown();
		}
		void OnSelectedValueChanged(object sender, EventArgs e) {
			this.EditValue = List.SelectedItem;
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			List.Focus();
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Return) {
				this.editorService.CloseDropDown();
				return true;
			}
			if(keyData == Keys.Escape) {
				this.editValueCore = this.originalValueCore;
				this.editorService.CloseDropDown();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		protected UITypeEditor Editor {
			get { return editorCore; }
		}
		protected ListBox List {
			get { return listBoxCore; }
		}
		public object EditValue {
			get { return editValueCore; }
			set {
				if(EditValue == value) return;
				this.editValueCore = value;
			}
		}
	}
}
