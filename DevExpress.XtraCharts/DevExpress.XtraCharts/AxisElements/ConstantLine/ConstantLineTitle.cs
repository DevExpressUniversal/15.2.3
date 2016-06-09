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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
				   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")]
	public class ConstantLineTitle : Title, ITextPropertiesProvider {
		const ConstantLineTitleAlignment DefaultAlignment = ConstantLineTitleAlignment.Near;
		const bool DefaultShowBelowLine = false;
		ConstantLineTitleAlignment alignment = DefaultAlignment;
		bool showBelowLine = DefaultShowBelowLine;
		protected override bool DefaultVisible { get { return true; } }
		protected override string DefaultText { get { return String.Empty; } }
		protected override Font DefaultFont { get { return DefaultFonts.Tahoma8; } }
		protected override bool DefaultAntialiasing { get { return false; } }
		protected internal override ChartElement BackElement { get { return null; } }
		internal ConstantLine ConstantLine { get { return (ConstantLine)base.Owner; } }
		internal string ActualText { get { return String.IsNullOrEmpty(Text) ? ConstantLine.Name : Text; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ConstantLineTitleAlignment"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ConstantLineTitle.Alignment"),
		Localizable(true),
		XtraSerializableProperty]
		public ConstantLineTitleAlignment Alignment {
			get { return alignment; }
			set {
				if (value != alignment) {
					SendNotification(new ElementWillChangeNotification(this));
					alignment = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ConstantLineTitleShowBelowLine"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ConstantLineTitle.ShowBelowLine"),
		XtraSerializableProperty,
		TypeConverter(typeof(BooleanTypeConverter))]
		public bool ShowBelowLine {
			get { return showBelowLine; }
			set {
				if (value != showBelowLine) {
					SendNotification(new ElementWillChangeNotification(this));
					showBelowLine = value;
					RaiseControlChanged();
				}
			}
		}
		internal ConstantLineTitle(ConstantLine constantLine)
			: base(constantLine) {
		}
		#region IHitTest implementation
		object IHitTest.Object { get { return ((IHitTest)ConstantLine).Object; } }
		HitTestState IHitTest.State { get { return ((IHitTest)ConstantLine).State; } }
		#endregion
		#region ITextPropertiesProvider implementation
		RectangleFillStyle ITextPropertiesProvider.FillStyle { get { return RectangleFillStyle.Empty; } }
		RectangularBorder ITextPropertiesProvider.Border { get { return null; } }
		Shadow ITextPropertiesProvider.Shadow { get { return null; } }
		StringAlignment ITextPropertiesProvider.Alignment { get { return StringFormat.GenericDefault.Alignment; } }
		bool ITextPropertiesProvider.ChangeSelectedBorder { get { return true; } }
		bool ITextPropertiesProvider.CorrectBoundsByBorder { get { return false; } }
		Color ITextPropertiesProvider.BackColor { get { return Color.Empty; } }
		Color ITextPropertiesProvider.GetTextColor(Color color) { return ActualTextColor; }
		Color ITextPropertiesProvider.GetBorderColor(Color color) { return Color.Empty; }
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Alignment":
					return ShouldSerializeAlignment();
				case "ShowBelowLine":
					return ShouldSerializeShowBelowLine();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeAlignment() {
			return alignment != DefaultAlignment;
		}
		void ResetAlignment() {
			Alignment = DefaultAlignment;
		}
		bool ShouldSerializeShowBelowLine() {
			return showBelowLine != DefaultShowBelowLine;
		}
		void ResetShowBelowLine() {
			ShowBelowLine = DefaultShowBelowLine;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeAlignment() || ShouldSerializeShowBelowLine();
		}
		#endregion
		protected override Color GetTextColor(IChartAppearance actualAppearance) {
			return actualAppearance.ConstantLineAppearance.TitleColor;
		}
		protected override ChartElement CreateObjectForClone() {
			return new ConstantLineTitle(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ConstantLineTitle title = obj as ConstantLineTitle;
			if (title != null) {
				alignment = title.alignment;
				showBelowLine = title.showBelowLine;
			}
		}
	}
}
