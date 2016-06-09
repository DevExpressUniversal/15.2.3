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

using System.Drawing;
using DevExpress.Utils.Base;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils.Design;
namespace DevExpress.Utils.VisualEffects {
	public class BadgeProperties : BaseProperties {
		public BadgeProperties() {
			SetDefaultValueCore<Image>("Image", null);
			SetDefaultValueCore("Text", string.Empty);
			SetDefaultValueCore("Location", ContentAlignment.TopLeft);
			SetDefaultValueCore("Offset", Point.Empty);
			SetDefaultValueCore("AllowImage", true);
			SetDefaultValueCore("ImageStretchMargins", Padding.Empty);
			SetDefaultValueCore("TextMargin", new Padding(2));
			SetDefaultValueCore("StretchImage", true);
			SetDefaultValueCore("AllowGlyphSkinning", true);
		}
		[DefaultValue(true)]
		public bool StretchImage {
			get { return GetValue<bool>("StretchImage"); }
			set { SetValueCore("StretchImage", value); }
		}
		public Padding TextMargin {
			get { return GetValue<Padding>("TextMargin"); }
			set { SetValueCore("TextMargin", value); }
		}
		bool ShouldSerializeTextMargin() { return TextMargin.All != 2; }
		void ResetTextMargin() { SetValueCore("TextMargin", new Padding(2)); }
		public Padding ImageStretchMargins {
			get { return GetValue<Padding>("SizingMargins"); }
			set { SetValueCore("SizingMargins", value); }
		}
		bool ShouldSerializeImageStretchMargins() { return ImageStretchMargins != Padding.Empty; }
		void ResetImageStretchMargins() { SetValueCore("ImageStretchMargins", Padding.Empty); }
		[DefaultValue(null)]
		public Image Image {
			get { return GetValueCore<Image>("Image"); }
			set { SetValueCore<Image>("Image", value); }
		}
		[DefaultValue(true)]
		public bool AllowGlyphSkinning {
			get { return GetValueCore<bool>("AllowGlyphSkinning"); }
			set { SetValueCore("AllowGlyphSkinning", value); }
		}
		[Localizable(true)]
		public string Text {
			get { return GetValueCore<string>("Text"); }
			set { SetValueCore("Text", value); }
		}
		bool ShouldSerializeText() { return !string.IsNullOrEmpty(Text); }
		void ResetText() { SetValueCore("Text", string.Empty); }
		[DefaultValue(ContentAlignment.TopLeft)]
		public ContentAlignment Location {
			get { return GetValueCore<ContentAlignment>("Location"); }
			set { SetValueCore("Location", value); }
		}
		public Point Offset {
			get { return GetValueCore<Point>("Offset"); }
			set { SetValueCore("Offset", value); }
		}
		bool ShouldSerializeOffset() { return !Offset.IsEmpty; }
		void ResetOffset() { SetValueCore("Offset", Point.Empty); }
		[DefaultValue(true)]
		public bool AllowImage {
			get { return GetValueCore<bool>("AllowImage"); }
			set { SetValueCore("AllowImage", value); }
		}
		protected override IBaseProperties CloneCore() { return new BadgeProperties(); }
		protected override void OnDispose() {
			propertyBag.Clear();
			defaultValueBag.Clear();
			propertyBag = defaultValueBag = null;
			base.OnDispose();
		}
	}
	public class BadgeDefaultProperties : BaseDefaultProperties {
		public BadgeDefaultProperties(BadgeProperties parentProperties)
			: base(parentProperties) {
			SetDefaultValueCore<Image>("Image", null);
			SetDefaultValueCore("Text", string.Empty);
			SetDefaultValueCore("AllowImage", DefaultBoolean.Default);
			SetDefaultValueCore("StretchImage", DefaultBoolean.Default);
			SetDefaultValueCore("AllowGlyphSkinning", DefaultBoolean.Default);
			SetConverter("Location", GetNullableValueConverter(ContentAlignment.TopLeft));
			SetConverter("Offset", GetNullableValueConverter(Point.Empty));
			SetConverter("ImageStretchMargins", GetNullableValueConverter(Padding.Empty));
			SetConverter("TextMargin", GetNullableValueConverter(new Padding(2)));
			SetConverter<DefaultBoolean, bool>("StretchImage", BaseDefaultProperties.GetDefaultBooleanConverter(true));
			SetConverter<DefaultBoolean, bool>("AllowImage", BaseDefaultProperties.GetDefaultBooleanConverter(true));
			SetConverter<DefaultBoolean, bool>("AllowGlyphSkinning", BaseDefaultProperties.GetDefaultBooleanConverter(true));
		}
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean StretchImage {
			get { return GetValue<DefaultBoolean>("StretchImage"); }
			set { SetValueCore("StretchImage", value); }
		}
		[DefaultValue(null), TypeConverter(typeof(NullableTypeConverter))]
		public Padding? TextMargin {
			get { return GetValue<Padding?>("TextMargin"); }
			set { SetValueCore("TextMargin", value); }
		}
		[DefaultValue(null), TypeConverter(typeof(NullableTypeConverter))]
		public Padding? ImageStretchMargins {
			get { return GetValue<Padding?>("ImageStretchMargins"); }
			set { SetValueCore("ImageStretchMargins", value); }
		}
		[DefaultValue(null)]
		public Image Image {
			get { return GetValueCore<Image>("Image"); }
			set { SetValueCore<Image>("Image", value); }
		}
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowGlyphSkinning {
			get { return GetValueCore<DefaultBoolean>("AllowGlyphSkinning"); }
			set { SetValueCore("AllowGlyphSkinning", value); }
		}
		[Localizable(true)]
		public string Text {
			get { return GetValueCore<string>("Text"); }
			set { SetValueCore("Text", value); }
		}
		bool ShouldSerializeText() { return !string.IsNullOrEmpty(Text); }
		void ResetText() { SetValueCore("Text", string.Empty); }
		[DefaultValue(null), TypeConverter(typeof(NullableTypeConverter))]
		public ContentAlignment? Location {
			get { return GetValueCore<ContentAlignment?>("Location"); }
			set { SetValueCore("Location", value); }
		}
		[DefaultValue(null), TypeConverter(typeof(NullableTypeConverter))]
		public Point? Offset {
			get { return GetValueCore<Point?>("Offset"); }
			set { SetValueCore("Offset", value); }
		}
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowImage {
			get { return GetValueCore<DefaultBoolean>("AllowImage"); }
			set { SetValueCore("AllowImage", value); }
		}
		protected override IBaseProperties CloneCore() { return new BadgeDefaultProperties(ParentProperties as BadgeProperties); }
		protected override void OnDispose() {
			propertyBag.Clear();
			defaultValueBag.Clear();
			EnsureParentProperties(null);
			propertyBag = defaultValueBag = null;
			base.OnDispose();
		}
		[Browsable(false)]
		public Image ActualImage { get { return GetActualValue<Image, Image>("Image"); } }
		[Browsable(false)]
		public string ActualText { get { return GetActualValue<string, string>("Text"); } }
		[Browsable(false)]
		public ContentAlignment ActualLocation { get { return GetActualValueFromNullable<ContentAlignment>("Location"); } }
		[Browsable(false)]
		public Point ActualOffset { get { return GetActualValueFromNullable<Point>("Offset"); } }
		[Browsable(false)]
		public bool CanUseImage { get { return GetActualValue<DefaultBoolean, bool>("AllowImage"); } }
		[Browsable(false)]
		public bool IsGlyphSkinningEnabled { get { return GetActualValue<DefaultBoolean, bool>("AllowGlyphSkinning"); } }
		[Browsable(false)]
		public Padding ActualImageStretchMargins { get { return GetActualValueFromNullable<Padding>("ImageStretchMargins"); } }
		[Browsable(false)]
		public Padding ActualTextMargin { get { return GetActualValueFromNullable<Padding>("TextMargin"); } }
		[Browsable(false)]
		public bool CanStretchImage { get { return GetActualValue<DefaultBoolean, bool>("StretchImage"); } }
	}
}
