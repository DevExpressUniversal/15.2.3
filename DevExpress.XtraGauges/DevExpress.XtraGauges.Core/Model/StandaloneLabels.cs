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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraGauges.Core.Model {
	public class Label : ScaleIndependentLayerComponent<BaseLabelProvider>, 
		ILabelEx, ISupportAssign<Label> {
		public Label() : base() { }
		public Label(string name) : base(name) { }
		protected override BaseLabelProvider CreateProvider() {
			return new BaseLabelProvider(OnScaleIndependentComponentChanged);
		}
		protected override bool AllowCacheRenderOperation {
			get { return false; }
		}
		protected override void CalculateScaleIndependentComponent() {
			string key = Provider.TextShape.Name;
			if(!string.IsNullOrEmpty(key)) {
				TextShape shape = Shapes[key] as TextShape;
				if(shape != null) {
					Provider.CalculateTextShape(shape);
				}
			}
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LabelPosition"),
#endif
XtraSerializableProperty]
		public PointF2D Position {
			get { return Provider.Position; }
			set { Provider.Position = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LabelSize"),
#endif
XtraSerializableProperty]
		public SizeF Size {
			get { return Provider.Size; }
			set { Provider.Size = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LabelAllowHTMLString"),
#endif
XtraSerializableProperty]
		[DefaultValue(false)]
		public bool AllowHTMLString {
			get { return Provider.AllowHTMLString; }
			set { Provider.AllowHTMLString = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LabelText"),
#endif
 Localizable(true),
XtraSerializableProperty]
		[DefaultValue("Text")]
		public string Text {
			get { return Provider.Text; }
			set { Provider.Text = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LabelTextOrientation"),
#endif
XtraSerializableProperty]
		[DefaultValue(LabelOrientation.Default)]
		public LabelOrientation TextOrientation {
			get { return Provider.TextOrientation; }
			set { Provider.TextOrientation = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LabelFormatString"),
#endif
 Localizable(true),
XtraSerializableProperty]
		[DefaultValue("{0}")]
		public string FormatString {
			get { return Provider.FormatString; }
			set { Provider.FormatString = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LabelAppearanceBackground"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseShapeAppearance AppearanceBackground {
			get { return Provider.AppearanceBackground; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LabelAppearanceText"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseTextAppearance AppearanceText {
			get { return Provider.AppearanceText; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TextShape TextShape {
			get { return Provider.TextShape; }
		}
		public void Assign(Label source) {
			BeginUpdate();
			if(source != null) {
				AssignPrimitiveProperties(source);
				Provider.Assign(source);
			}
			EndUpdate();
		}
		public bool IsDifferFrom(Label source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
					Provider.IsDifferFrom(source);
		}
		public void Assign(ILabel source) {
			Provider.Assign(source);
		}
		public bool IsDifferFrom(ILabel source) {
			return Provider.IsDifferFrom(source);
		}
		[Browsable(false), Bindable(false)]
		[XtraSerializableProperty]
		public override string Name {
			get { return base.Name; }
			set {
				if(base.Name == value) return;
				base.Name = value;
				Provider.Name = Name;
			}
		}
		internal bool ShouldSerializeAppearanceBackground() {
			return Provider.ShouldSerializeAppearanceBackground();
		}
		internal bool ShouldSerializeAppearanceText() {
			return Provider.ShouldSerializeAppearanceText();
		}
		internal bool ShouldSerializeSize() {
			return Size != new SizeF(125, 25);
		}
		internal void ResetSize() {
			Size = new SizeF(125, 25);
		}
		internal bool ShouldSerializePosition() {
			return Position != new PointF2D(125, 125);
		}
		internal void ResetPosition() {
			Position = new PointF2D(125, 125);
		}
	}
	public class BaseLabelProvider : BaseLabel {
		internal BaseLabelProvider(OwnerChangedAction ownerChanged)
			: base(ownerChanged) {
			BeginUpdate();
			this.Position = new PointF2D(125, 125);
			this.Size = new PointF2D(125, 25);
			this.FormatString = "{0}";
			this.Text = "Text";
			CancelUpdate();
		}
		protected override BaseObject CloneCore() {
			return new BaseLabelProvider(null);
		}
	}
}
