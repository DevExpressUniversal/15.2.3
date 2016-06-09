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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.ComponentModel;
#if !DXPORTABLE
using System.Drawing.Design;
#endif
namespace DevExpress.XtraGauges.Core.Drawing {
	[Flags]
	public enum ShadingFlags {
		Normal = 0x00,
		NoShading = 0x01
	}
	public enum ShaderObjectType { EmptyShader, GrayShader, OpacityShader, StyleShader, ComplexShader }
#if !DXPORTABLE
	[Editor("DevExpress.XtraGauges.Design.ShaderObjectTypeEditor, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(UITypeEditor))]
#endif
	[TypeConverter(typeof(ShaderObjectTypeConverter)), RefreshProperties(RefreshProperties.All)]
	public abstract class BaseColorShader : BaseObjectEx, IColorShader, ISupportAssign<BaseColorShader>, IFormattable {
		[ThreadStatic]
		static BaseColorShader emptyShaderCore;
		public static BaseColorShader Empty {
			get {
				if(emptyShaderCore == null) emptyShaderCore = new EmptyShader();
				return emptyShaderCore;
			}
		}
		public BaseColorShader()
			: base() {
		}
		protected BaseColorShader(string shaderData)
			: this() {
			BeginUpdate();
			Assign(shaderData);
			EndUpdate();
		}
		public bool IsEmpty {
			get { return this is EmptyShader; }
		}
		public void Process(ref Color sourceColor) {
			ProcessCore(ref sourceColor);
		}
		public void Process(ref Color[] colors) {
			for(int i = 0; i < colors.Length; i++) ProcessCore(ref colors[i]);
		}
		protected abstract void ProcessCore(ref Color sourceColor);
		protected sealed override void CopyToCore(BaseObject clone) {
			BaseColorShader clonedShader = clone as BaseColorShader;
			if(clonedShader != null) {
				ShaderCopyToCore(clonedShader);
			}
		}
		protected virtual void ShaderCopyToCore(BaseColorShader clone) { }
		public void Assign(BaseColorShader source) {
			BeginUpdate();
			AssignCore(source);
			EndUpdate();
		}
		public bool IsDifferFrom(BaseColorShader source) {
			return IsDifferFromCore(source);
		}
		protected virtual bool IsDifferFromCore(BaseColorShader source) {
			return (source == null) ? true : (this.GetType() != source.GetType());
		}
		protected virtual void AssignCore(BaseColorShader source) { }
		class EmptyShader : BaseColorShader {
			protected override void OnCreate() { }
			protected override void ProcessCore(ref Color sourceColor) { }
			protected override BaseObject CloneCore() {
				return new EmptyShader();
			}
			protected override string GetShaderTypeTag() {
				return "Empty";
			}
			protected internal override string GetShaderDataTag() {
				return string.Empty;
			}
			protected internal override void Assign(string shaderData) { }
		}
		string IFormattable.ToString(string format, IFormatProvider provider) {
			string dataTag = GetShaderDataTag();
			return (string.IsNullOrEmpty(dataTag)) ? String.Format("<ShaderObject Type=\"{0}\"/>", GetShaderTypeTag()) :
				String.Format("<ShaderObject Type=\"{0}\" Data=\"{1}\"/>", GetShaderTypeTag(), dataTag);
		}
		protected abstract string GetShaderTypeTag();
		protected internal abstract string GetShaderDataTag();
		protected internal abstract void Assign(string shaderData);
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[XtraSerializableProperty, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string TypeTag {
			get { return GetShaderTypeTag(); }
		}
	}
	public class OpacityShader : BaseColorShader, IOpacityShader {
		float opacityCore;
		public OpacityShader() : base() { }
		public OpacityShader(string shaderData) : base(shaderData) { }
		protected override void OnCreate() {
			this.opacityCore = 1f;
		}
		[XtraSerializableProperty]
		[DefaultValue(1f)]
		public float Opacity {
			get { return opacityCore; }
			set {
				if(value > 1f) value = 1f;
				if(value < 0f) value = 0f;
				if(Opacity == value) return;
				opacityCore = value;
				OnObjectChanged("Opacity");
			}
		}
		protected override BaseObject CloneCore() {
			return new OpacityShader();
		}
		protected override void ShaderCopyToCore(BaseColorShader clone) {
			OpacityShader clonedShader = clone as OpacityShader;
			if(clonedShader != null) {
				clonedShader.opacityCore = this.opacityCore;
			}
		}
		protected override void ProcessCore(ref Color sourceColor) {
			sourceColor = ShadingHelper.Combine(sourceColor, Opacity);
		}
		protected override bool IsDifferFromCore(BaseColorShader source) {
			OpacityShader shader = source as OpacityShader;
			return base.IsDifferFromCore(source) || (shader != null && (shader.Opacity != Opacity));
		}
		protected override void AssignCore(BaseColorShader source) {
			OpacityShader shader = source as OpacityShader;
			if(shader != null) {
				Opacity = shader.opacityCore;
			}
		}
		protected override string GetShaderTypeTag() {
			return "Opacity";
		}
		protected internal override string GetShaderDataTag() {
			return Opacity != 1f ? String.Format(CultureInfo.InvariantCulture, "Opacity[{0}]", Opacity) : string.Empty;
		}
		protected internal override void Assign(string shaderData) {
			if(string.IsNullOrEmpty(shaderData)) return;
			MatchCollection matches = Regex.Matches(shaderData, @"Opacity\[(?<opacity>.*?)\]\z");
			if(matches.Count == 1) Opacity = float.Parse(matches[0].Groups["opacity"].Value, CultureInfo.InvariantCulture);
		}
	}
	public class GrayShader : BaseColorShader {
		public GrayShader() : base() { }
		public GrayShader(string shaderData) : base(shaderData) { }
		protected override void OnCreate() { }
		protected override BaseObject CloneCore() {
			return new GrayShader();
		}
		protected override void ProcessCore(ref Color sourceColor) {
			sourceColor = ShadingHelper.Combine(sourceColor, Color.White, Color.Black);
		}
		protected override string GetShaderTypeTag() { return "Gray"; }
		protected internal override string GetShaderDataTag() { return string.Empty; }
		protected internal override void Assign(string shaderData) { }
	}
	public class DisabledShader : BaseColorShader {
		static DisabledShader defaultInstanceCore = new DisabledShader();
		public DisabledShader() : base() { }
		public DisabledShader(string shaderData) : base(shaderData) { }
		protected override void OnCreate() { }
		protected override BaseObject CloneCore() {
			return new DisabledShader();
		}
		protected override void ProcessCore(ref Color sourceColor) {
			Color styleColor = ShadingHelper.Combine(sourceColor, Color.Black, Color.WhiteSmoke);
			sourceColor = ShadingHelper.Combine(styleColor, 0.9f);
		}
		public static DisabledShader Default { get { return defaultInstanceCore; } }
		protected override string GetShaderTypeTag() { return "Disabled"; }
		protected internal override string GetShaderDataTag() { return string.Empty; }
		protected internal override void Assign(string shaderData) { }
	}
	public class StyleShader : BaseColorShader, IStyleShader {
		Color styleColor1Core;
		Color styleColor2Core;
		public StyleShader() : base() { }
		public StyleShader(string shaderData) : base(shaderData) { }
		protected override void OnCreate() {
			this.styleColor1Core = Color.Empty;
			this.styleColor2Core = Color.Empty;
		}
		[XtraSerializableProperty]
		public Color StyleColor1 {
			get { return styleColor1Core; }
			set {
				if(StyleColor1 == value) return;
				styleColor1Core = value;
				OnObjectChanged("StyleColor1");
			}
		}
		[XtraSerializableProperty]
		public Color StyleColor2 {
			get { return styleColor2Core; }
			set {
				if(StyleColor2 == value) return;
				styleColor2Core = value;
				OnObjectChanged("StyleColor2");
			}
		}
		protected override BaseObject CloneCore() {
			return new StyleShader();
		}
		protected override void ShaderCopyToCore(BaseColorShader clone) {
			StyleShader clonedShader = clone as StyleShader;
			if(clonedShader != null) {
				clonedShader.styleColor1Core = this.styleColor1Core;
				clonedShader.styleColor2Core = this.styleColor2Core;
			}
		}
		protected override void ProcessCore(ref Color sourceColor) {
			sourceColor = ShadingHelper.Combine(sourceColor, StyleColor1, StyleColor2);
		}
		protected override bool IsDifferFromCore(BaseColorShader source) {
			StyleShader shader = source as StyleShader;
			return base.IsDifferFromCore(source) || (shader != null && (shader.StyleColor1 != StyleColor1 || shader.StyleColor2 != StyleColor2));
		}
		protected override void AssignCore(BaseColorShader source) {
			base.AssignCore(source);
			StyleShader shader = source as StyleShader;
			if(shader != null) {
				StyleColor1 = shader.StyleColor1;
				StyleColor2 = shader.StyleColor2;
			}
		}
		protected override string GetShaderTypeTag() {
			return "Style";
		}
		protected internal override string GetShaderDataTag() {
			return ColorsToString();
		}
		protected internal override void Assign(string brushData) {
			if(string.IsNullOrEmpty(brushData)) return;
			MatchCollection colorMatches = Regex.Matches(brushData, @"Colors\[(?<colors>.*?)\]");
			if(colorMatches.Count == 1) ParseColors(colorMatches[0].Groups["colors"].Value);
		}
		protected string ColorsToString() {
			return "Colors[" +
				String.Format("Style1:{0}", ARGBColorTranslator.ToHtml(StyleColor1)) + ";" +
				String.Format("Style2:{0}", ARGBColorTranslator.ToHtml(StyleColor2)) + "]";
		}
		protected void ParseColors(string str) {
			MatchCollection m = Regex.Matches(str, @"Style1:(?<color1>.*?);Style2:(?<color2>.*?)$");
			if(m.Count == 1) {
				StyleColor1 = ARGBColorTranslator.FromHtml(m[0].Groups["color1"].Value);
				StyleColor2 = ARGBColorTranslator.FromHtml(m[0].Groups["color2"].Value);
			}
		}
	}
	public class ComplexShader : StyleShader, IOpacityShader {
		float opacityCore;
		public ComplexShader() : base() { }
		public ComplexShader(string shaderData) : base(shaderData) { }
		protected override void OnCreate() {
			base.OnCreate();
			this.opacityCore = 1f;
		}
		protected override void ShaderCopyToCore(BaseColorShader clone) {
			base.ShaderCopyToCore(clone);
			ComplexShader clonedShader = clone as ComplexShader;
			if(clonedShader != null) {
				clonedShader.opacityCore = this.Opacity;
			}
		}
		protected override BaseObject CloneCore() {
			return new ComplexShader();
		}
		[XtraSerializableProperty]
		[DefaultValue(1f)]
		public float Opacity {
			get { return opacityCore; }
			set {
				if(value > 1f) value = 1f;
				if(value < 0f) value = 0f;
				if(Opacity == value) return;
				opacityCore = value;
				OnObjectChanged("Opacity");
			}
		}
		protected override void ProcessCore(ref Color sourceColor) {
			Color styleColor = ShadingHelper.Combine(sourceColor, StyleColor1, StyleColor2);
			sourceColor = ShadingHelper.Combine(styleColor, Opacity);
		}
		protected override bool IsDifferFromCore(BaseColorShader source) {
			ComplexShader shader = source as ComplexShader;
			return base.IsDifferFromCore(source) || (shader != null && (shader.Opacity != Opacity));
		}
		protected override void AssignCore(BaseColorShader source) {
			base.AssignCore(source);
			ComplexShader shader = source as ComplexShader;
			if(shader != null) {
				Opacity = shader.Opacity;
			}
		}
		protected override string GetShaderTypeTag() {
			return "Complex";
		}
		protected internal override string GetShaderDataTag() {
			return String.Format("Opacity[{0}] ", Opacity) + ColorsToString();
		}
		protected internal override void Assign(string brushData) {
			if(string.IsNullOrEmpty(brushData)) return;
			MatchCollection opacityMatches = Regex.Matches(brushData, @"Opacity\[(?<opacity>.*?)\]");
			if(opacityMatches.Count == 1) Opacity = float.Parse(opacityMatches[0].Groups["opacity"].Value, CultureInfo.InvariantCulture);
			MatchCollection colorMatches = Regex.Matches(brushData, @"Colors\[(?<colors>.*?)\]");
			if(colorMatches.Count == 1) ParseColors(colorMatches[0].Groups["colors"].Value);
		}
	}
	public class ShadingHelper {
		public static Color Combine(Color source, float opacity) {
			return Color.FromArgb((int)((float)source.A * opacity), source.R, source.G, source.B);
		}
		public static Color Combine(Color source, Color styleColor1, Color styleColor2) {
			float grayLevel = ((float)source.R * 0.3f + (float)source.G * 0.59f + (float)source.B * 0.11f) / 255f;
			int styleColorR = styleColor1.R + (int)((float)(styleColor2.R - styleColor1.R) * grayLevel);
			int styleColorG = styleColor1.G + (int)((float)(styleColor2.G - styleColor1.G) * grayLevel);
			int styleColorB = styleColor1.B + (int)((float)(styleColor2.B - styleColor1.B) * grayLevel);
			return Color.FromArgb(source.A, styleColorR, styleColorG, styleColorB);
		}
		public static void ProcessShape(BaseShape shape, BaseColorShader shader, bool enabled) {
			if(shape.IsEmpty) return;
			shape.Appearance.BeginUpdate();
			shape.AppearanceText.BeginUpdate();
			if(!shader.IsEmpty) shape.Accept(shader);
			if(!enabled) shape.Accept(DisabledShader.Default);
			shape.AppearanceText.CancelUpdate();
			shape.Appearance.CancelUpdate();
		}
	}
	public class ShaderObjectTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return FilterPropertiesHelper.FilterProperties(base.GetProperties(context, value, attributes), GetExpectedProperties(value));
		}
		static string[] GetExpectedProperties(object value) {
			IColorShader baseShader = value as IColorShader;
			if(baseShader == null || baseShader.IsEmpty) return new string[0];
			ArrayList filteredProperties = new ArrayList();
			if(baseShader is IOpacityShader) {
				filteredProperties.AddRange(new string[] { "Opacity" });
			}
			if(baseShader is IStyleShader) {
				filteredProperties.AddRange(new string[] { "StyleColor1", "StyleColor2" });
			}
			return (string[])filteredProperties.ToArray(typeof(string));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
			return (destType == typeof(string)) || (destType == typeof(InstanceDescriptor));
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) {
			if(value is BaseColorShader) {
				if(destType == typeof(string)) {
					return ((IFormattable)value).ToString(null, culture);
				}
				if(destType == typeof(InstanceDescriptor)) {
					ConstructorInfo constructor = value.GetType().GetConstructor(new Type[] { typeof(string) });
					if(constructor != null) return new InstanceDescriptor(constructor, new object[] { ((BaseColorShader)value).GetShaderDataTag() });
				}
			}
			return base.ConvertTo(context, culture, value, destType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string str = value as string;
			if(str == null) return base.ConvertFrom(context, culture, value);
			str = str.Trim();
			if(str.Length == 0) return null;
			string pattern = "<ShaderObject\\s+Type\\s*=\\s*\"(?<shaderType>.*?)\"(\\s+(Data\\s*=\\s*\"(?<shaderData>.*?)\"\\s?)?)?/>";
			MatchCollection matches = Regex.Matches(str, pattern);
			BaseColorShader shader = null;
			if(matches.Count == 1) {
				shader = CreateShader(matches[0].Groups["shaderType"].Value);
				if(shader != null) {
					shader.BeginUpdate();
					shader.Assign(matches[0].Groups["shaderData"].Value);
					shader.EndUpdate();
				}
			}
			return shader;
		}
		protected BaseColorShader CreateShader(string typeTag) {
			switch(typeTag) {
				case "Empty": return BaseColorShader.Empty;
				case "Opacity": return new OpacityShader();
				case "Style": return new StyleShader();
				case "Complex": return new ComplexShader();
				case "Gray": return new GrayShader();
				case "Disabled": return new DisabledShader();
				default: return null;
			}
		}
	}
}
