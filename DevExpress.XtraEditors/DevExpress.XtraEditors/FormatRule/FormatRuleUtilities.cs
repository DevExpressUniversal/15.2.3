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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraEditors {
	public enum FormatConditionTopBottomType { Top, Bottom }
	public enum FormatConditionUniqueDuplicateType { Unique, Duplicate }
	public enum FormatConditionComparisonType { Greater, GreaterOrEqual }
	public enum FormatConditionValueType { Number, Percent, Automatic }
	public enum FormatConditionAboveBelowType { Above, Below, EqualOrAbove, EqualOrBelow }
	[Flags]
	public enum FormatConditionRuleChangeType { None = 0, UI = 1, Data = 2, All = UI | Data };
}
namespace DevExpress.XtraEditors.Helpers {
	public class RuleDataColumnInfoWrapper : IDataColumnInfo {
		DataControllerBase controller;
		List<IDataColumnInfo> columns;
		public RuleDataColumnInfoWrapper(DataControllerBase controller, List<IDataColumnInfo> columns) {
			this.columns = columns;
			this.controller = controller;
		}
		string IDataColumnInfo.Caption {
			get {
				return ToString();
			}
		}
		DataControllerBase IDataColumnInfo.Controller { get { return controller; } }
		List<IDataColumnInfo> IDataColumnInfo.Columns { get { return columns; } }
		string IDataColumnInfo.FieldName { get { return string.Empty; } }
		Type IDataColumnInfo.FieldType { get { return typeof(object); } }
		string IDataColumnInfo.Name { get { return string.Empty; } }
		string IDataColumnInfo.UnboundExpression { get { return string.Empty; } }
	}
	public interface IFormatConditionRuleOwner {
		void OnModified(FormatConditionRuleChangeType changeType);
		bool IsLoading { get; }
		ExpressionEvaluator CreateExpressionEvaluator(CriteriaOperator criteriaOperator, out bool readyToCreate);
		UserLookAndFeel LookAndFeel { get; }
		bool GetIsRightToLeft();
	}
	public class FormatRuleAppearanceArgs {
		IFormatConditionRuleValueProvider valueProvider;
		object cellValue;
		public FormatRuleAppearanceArgs(IFormatConditionRuleValueProvider valueProvider, object cellValue) {
			this.cellValue = cellValue;
			this.valueProvider = valueProvider;
		}
		public IFormatConditionRuleValueProvider ValueProvider { get { return valueProvider; } }
		public object CellValue { get { return cellValue; } }
	}
	public delegate void DrawAppearanceMethod(GraphicsCache cache, AppearanceObject appearance);
	public class FormatRuleDrawArgs {
		public static bool IsSupportContextImage(object target) {
			var ci = target as IFormatRuleSupportContextImage;
			return ci != null && ci.SupportContextImage;
		}
		GraphicsCache cache;
		Rectangle bounds;
		IFormatConditionRuleValueProvider valueProvider;
		public FormatRuleDrawArgs(GraphicsCache cache, Rectangle bounds, IFormatConditionRuleValueProvider valueProvider) {
			this.cache = cache;
			this.bounds = bounds;
			this.valueProvider = valueProvider;
		}
		public Rectangle Bounds { get { return bounds; } }
		public GraphicsCache Cache { get { return cache; } }
		public IFormatConditionRuleValueProvider ValueProvider { get { return valueProvider; } }
		public DrawAppearanceMethod OriginalContentPainter { get; set; }
		public AppearanceObject OriginalContentAppearance { get; set; }
		internal void SetBounds(Rectangle bounds) { this.bounds = bounds; }
	}
	public interface IFormatRuleSupportContextImage {
		bool SupportContextImage { get; }
		void SetContextImage(Image image);
	}
	public interface IFormatRuleContextImage {
		Image GetContextImage(FormatRuleDrawArgs e);
	}
	public interface IFormatRuleDraw {
		void DrawOverlay(FormatRuleDrawArgs e);
		bool AllowDrawValue { get; }
	}
	public interface IFormatRuleAppearance {
		AppearanceObjectEx QueryAppearance(FormatRuleAppearanceArgs e);
	}
	[Flags]
	public enum FormatRuleValueQueryKind {
		None = 0,
		Minimum = 0x001,
		Maximum = 0x002,
		Average = 0x004,
		Count = 0x010,
		Top = 0x020,
		Bottom = 0x040,
		TopPercent = 0x080,
		BottomPercent = 0x100,
		Duplicate = 0x200,
		Unique = 0x400
	}
	internal class FormatConditionEmptyValueProvider : IFormatConditionRuleValueProvider {
		object IFormatConditionRuleValueProvider.GetValueExpression(FormatConditionRuleBase rule) {
			return null;
		}
		object IFormatConditionRuleValueProvider.GetValue(FormatConditionRuleBase rule) {
			return null;
		}
		object IFormatConditionRuleValueProvider.GetQueryValue(FormatConditionRuleBase rule, FormatRuleValueQueryKind argument) {
			return null;
		}
		FormatConditionRuleState IFormatConditionRuleValueProvider.GetState(FormatConditionRuleBase rule) {
			return new FormatConditionRuleState(rule);
		}
	}
	public class FormatConditionDrawPreviewArgs {
		string text;
		Graphics graphics;
		AppearanceObject appearance;
		Rectangle bounds;
		public FormatConditionDrawPreviewArgs(Graphics graphics, Rectangle bounds, AppearanceObject appearance, string text) {
			this.graphics = graphics;
			this.appearance = appearance;
			this.text = text;
			this.bounds = bounds;
		}
		public Rectangle Bounds { get { return bounds; } }
		public Graphics Graphics { get { return graphics; } }
		public string Text { get { return text; } }
		public AppearanceObject Appearance { get { return appearance; } }
	}
	public interface IFormatConditionDrawPreview {
		void Draw(FormatConditionDrawPreviewArgs e);
	}
	public interface IFormatConditionRuleValueQuery {
		FormatConditionRuleState GetRuleState();
	}
	public interface IFormatConditionRuleValueProvider {
		object GetValueExpression(FormatConditionRuleBase rule);
		object GetValue(FormatConditionRuleBase rule);
		object GetQueryValue(FormatConditionRuleBase rule, FormatRuleValueQueryKind argument = FormatRuleValueQueryKind.None);
		FormatConditionRuleState GetState(FormatConditionRuleBase rule);
	}
	public abstract class FormatConditionRuleValueProviderBase : IFormatConditionRuleValueProvider {
		FormatRuleBase format;
		FormatConditionRuleState ruleState;
		public FormatConditionRuleValueProviderBase(FormatRuleBase format) {
			this.format = format;
		}
		public FormatRuleBase Format { get { return format; } }
		public FormatConditionRuleState RuleState {
			get {
				if(Format.Rule == null) return FormatConditionRuleState.NullState;
				if(ruleState != null && !ruleState.IsActual(Format.Rule)) ruleState = null;
				if(ruleState == null) {
					ruleState = ((IFormatConditionRuleValueQuery)format.Rule).GetRuleState();
				}
				return ruleState;
			}
		}
		#region IFormatConditionRuleValueProvider Members
		object IFormatConditionRuleValueProvider.GetValueExpression(FormatConditionRuleBase rule) {
			return GetValueExpressionCore(rule);
		}
		object IFormatConditionRuleValueProvider.GetValue(FormatConditionRuleBase rule) {
			return GetValueCore(rule);
		}
		protected abstract object GetValueExpressionCore(FormatConditionRuleBase rule);
		protected abstract object GetValueCore(FormatConditionRuleBase rule);
		object IFormatConditionRuleValueProvider.GetQueryValue(FormatConditionRuleBase rule, FormatRuleValueQueryKind argument) {
			return RuleState.GetValue<object>(argument.ToString());
		}
		FormatConditionRuleState IFormatConditionRuleValueProvider.GetState(FormatConditionRuleBase rule) { return RuleState; }
		#endregion
	}
	public class FormatConditionRuleState {
		int stateId = 0;
		decimal countPercent = 0;
		object cachedDefaultValue = null;
		FormatRuleValueQueryKind queryKind;
		Dictionary<string, object> values;
		bool valuesReady = false;
		public FormatConditionRuleState(FormatConditionRuleBase rule) : this(rule, FormatRuleValueQueryKind.None) { }
		public FormatConditionRuleState(FormatConditionRuleBase rule, FormatRuleValueQueryKind queryKind) {
			if(rule != null) this.stateId = rule.stateId;
			this.queryKind = queryKind;
		}
		public FormatRuleValueQueryKind QueryKind { get { return queryKind; } }
		public T GetValue<T>(string name) {
			if(DefaultValueName == name) {
				return GetDefaultValue<T>(name);
			}
			if(values == null) return default(T);
			object v;
			if(!values.TryGetValue(name, out v)) return default(T);
			return (T)v;
		}
		T GetDefaultValue<T>(string name) {
			if(cachedDefaultValue != null) return (T)cachedDefaultValue;
			if(values == null || values.Count == 0) return default(T);
			object[] value = new object[1];
			values.Values.CopyTo(value, values.Count - 1);
			cachedDefaultValue = value[0];
			return (T)value[0];
		}
		public virtual void SetValue(string name, object value) {
			this.cachedDefaultValue = null;
			if(values == null) this.values = new Dictionary<string, object>();
			this.values[name] = value;
		}
		public decimal CountPercent { get { return countPercent; } set { countPercent = value; } }
		public bool IsActual(FormatConditionRuleBase rule) {
			return rule != null && rule.stateId == stateId;
		}
		public virtual bool ValuesReady {
			get { return valuesReady; }
			set { valuesReady = value; }
		}
		public static readonly string DefaultValueName = FormatRuleValueQueryKind.None.ToString();
		static FormatConditionRuleState nullState;
		public static FormatConditionRuleState NullState {
			get {
				if(nullState == null) nullState = new NullFormatConditionRuleState();
				return nullState;
			}
		}
		class NullFormatConditionRuleState : FormatConditionRuleState {
			public NullFormatConditionRuleState() : base(null) { }
			public override void SetValue(string name, object value) {
			}
			public override bool ValuesReady { get { return base.ValuesReady; } set { } }
		}
	}
	public abstract class FormatPredefinedBase<T> : IEnumerable, IEnumerable<T> where T : class {
		Dictionary<string, T> sets;
		protected T this[string key] {
			get {
				EnsureSets();
				T res;
				if(sets.TryGetValue(key, out res)) return res;
				return null;
			}
		}
		protected Color FromInt(int color) {
			if(color == int.MinValue) return Color.Empty;
			Color res = Color.FromArgb(color);
			if(res.A == 0) res = Color.FromArgb(255, res);
			return res;
		}
		protected virtual void EnsureSets() {
			if(sets == null) {
				sets = new Dictionary<string, T>();
				Register(sets);
			}
		}
		public const string DarkSkinSuffix = ".Dark";
		public T Find(FormatConditionRuleBase rule, string key) {
			if(rule == null) return Find(UserLookAndFeel.Default, key);
			return Find(rule.LookAndFeel, key);
		}
		public List<T> Find(FormatConditionRuleBase rule) {
			if(rule == null) return Find(UserLookAndFeel.Default);
			return Find(rule.LookAndFeel);
		}
		public T Find(UserLookAndFeel lookAndFeel, string key) {
			string skinName = lookAndFeel.ActiveSkinName;
			string targetKey = key;
			if(DevExpress.Utils.Frames.FrameHelper.IsDarkSkin(skinName, Utils.Frames.FrameHelper.SkinDefinitionReason.Rules)) targetKey = key + DarkSkinSuffix;
			var res = this[targetKey];
			if(res == null && targetKey != key) return this[key];
			return res;
		}
		public List<T> Find(UserLookAndFeel lookAndFeel) {
			EnsureSets();
			List<T> res = new List<T>();
			string skinName = lookAndFeel == null ? UserLookAndFeel.Default.ActiveSkinName : lookAndFeel.ActiveSkinName;
			bool isDark = false;
			if(DevExpress.Utils.Frames.FrameHelper.IsDarkSkin(skinName, Utils.Frames.FrameHelper.SkinDefinitionReason.Rules)) isDark = true;
			foreach(KeyValuePair<string, T> kp in sets) {
				string key = kp.Key;
				if(key.EndsWith(DarkSkinSuffix)) {
					if(isDark) res.Add(kp.Value);
				}
				else {
					if(!isDark) res.Add(kp.Value);
				}
			}
			return res;
		}
		protected abstract void Register(Dictionary<string, T> sets);
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			EnsureSets();
			return sets.Values.GetEnumerator();
		}
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			EnsureSets();
			return sets.Values.GetEnumerator();
		}
		#endregion
	}
	public class FormatPredefinedColorScale : FormatPredefinedBaseScheme {
		public FormatPredefinedColorScale(string key) : base(key) {
		}
		public Color MinimumColor { get; set; }
		public Color MiddleColor { get; set; }
		public Color MaximumColor { get; set; }
	}
	public class FormatPredefinedColorScales : FormatPredefinedBase<FormatPredefinedColorScale> {
		static FormatPredefinedColorScales _default;
		public static FormatPredefinedColorScales Default {
			get {
				if(_default == null) _default = new FormatPredefinedColorScales();
				return _default;
			}
		}
		protected override void Register(Dictionary<string, FormatPredefinedColorScale> sets) {
			Register(sets, "Green, Yellow, Red", "Green, Yellow, Red", 0x8cc542, 0xfec22c, 0xea4747, Localizer.Active.GetLocalizedString(StringId.ColorScaleGreenYellowRed));
			Register(sets, "Purple, White, Azure", "Purple, White, Azure", 0xb04b8b, 0xffffff, 0x1caedb, Localizer.Active.GetLocalizedString(StringId.ColorScalePurpleWhiteAzure));
			Register(sets, "Purple, White, Azure", "Purple, White, Azure" + DarkSkinSuffix, 0xb04b8b, 0x414141, 0x1caedb, Localizer.Active.GetLocalizedString(StringId.ColorScalePurpleWhiteAzure) + "[Dark]");
			Register(sets, "Yellow, Orange, Coral", "Yellow, Orange, Coral", 0xfec22c, 0xff9500, 0xf76a43, Localizer.Active.GetLocalizedString(StringId.ColorScaleYellowOrangeCoral));
			Register(sets, "Green, White, Red", "Green, White, Red", 0x8cc542, 0xffffff, 0xea4747, Localizer.Active.GetLocalizedString(StringId.ColorScaleGreenWhiteRed));
			Register(sets, "Green, White, Red", "Green, White, Red" + DarkSkinSuffix, 0x8cc542, 0x414141, 0xea4747, Localizer.Active.GetLocalizedString(StringId.ColorScaleGreenWhiteRed) + "[Dark]");
			Register(sets, "Emerald, Azure, Blue", "Emerald, Azure, Blue", 0x4ebf8f, 0x1caedb, 0x377fd0, Localizer.Active.GetLocalizedString(StringId.ColorScaleEmeraldAzureBlue));
			Register(sets, "White, Red", "White, Red", 0xffffff, -1, 0xea4747, Localizer.Active.GetLocalizedString(StringId.ColorScaleWhiteRed));
			Register(sets, "White, Red", "White, Red" + DarkSkinSuffix, 0x414141, -1, 0xea4747, Localizer.Active.GetLocalizedString(StringId.ColorScaleWhiteRed) + "[Dark]");
			Register(sets, "White, Green", "White, Green", 0xffffff, -1, 0x8cc542, Localizer.Active.GetLocalizedString(StringId.ColorScaleWhiteGreen));
			Register(sets, "White, Green", "White, Green" + DarkSkinSuffix, 0x414141, -1, 0x8cc542, Localizer.Active.GetLocalizedString(StringId.ColorScaleWhiteGreen) + "[Dark]");
			Register(sets, "White, Azure", "White, Azure", 0xffffff, -1, 0x1caedb, Localizer.Active.GetLocalizedString(StringId.ColorScaleWhiteAzure));
			Register(sets, "White, Azure", "White, Azure" + DarkSkinSuffix, 0x414141, -1, 0x1caedb, Localizer.Active.GetLocalizedString(StringId.ColorScaleWhiteAzure) + "[Dark]");
			Register(sets, "Yellow, Green", "Yellow, Green", 0xfec22c, -1, 0x8cc542, Localizer.Active.GetLocalizedString(StringId.ColorScaleYellowGreen));
			Register(sets, "Blue, White, Red", "Blue, White, Red", 0x377fd0, 0xffffff, 0xea4747, Localizer.Active.GetLocalizedString(StringId.ColorScaleBlueWhiteRed));
			Register(sets, "Blue, White, Red", "Blue, White, Red" + DarkSkinSuffix, 0x377fd0, 0x414141, 0xea4747, Localizer.Active.GetLocalizedString(StringId.ColorScaleBlueWhiteRed) + "[Dark]");
		}
		void Register(Dictionary<string, FormatPredefinedColorScale> sets, string key, string hashKey, int minimum, int middle, int maximum, string title) {
			Register(sets, key, hashKey, FromInt(minimum), middle == -1 ? Color.Empty : FromInt(middle), FromInt(maximum), title);
		}
		void Register(Dictionary<string, FormatPredefinedColorScale> sets, string key, string hashKey, Color minimum, Color middle, Color maximum, string title) {
			sets.Add(hashKey, new FormatPredefinedColorScale(key) { MinimumColor = minimum, MiddleColor = middle, MaximumColor = maximum, Title = title });
		}
	}
	public class FormatPredefinedBaseScheme {
		string key = "";
		public FormatPredefinedBaseScheme(string key) {
			this.key = key;
		}
		public string Title { get; set; }
		public string Description { get; set; }
		public string Key { get { return key; } }
		public override string ToString() {
			return Title;
		}
	}
	public class FormatPredefinedAppearance : FormatPredefinedBaseScheme {
		public FormatPredefinedAppearance(string key) : base(key) {
			this.Appearance = new AppearanceDefault();
		}
		public AppearanceDefault Appearance { get; set; }
	}
	public class FormatPredefinedDataBarScheme : FormatPredefinedBaseScheme {
		public FormatPredefinedDataBarScheme(string key) : base(key) {
			this.Positive = new AppearanceDefault();
			this.Negative = new AppearanceDefault();
			this.AxisColor = Color.Empty;
		}
		public AppearanceDefault Positive { get; set; }
		public AppearanceDefault Negative { get; set; }
		public Color AxisColor { get; set; }
	}
	public class FormatPredefinedDataBarSchemes : FormatPredefinedBase<FormatPredefinedDataBarScheme> {
		static FormatPredefinedDataBarSchemes _default;
		public static FormatPredefinedDataBarSchemes Default {
			get {
				if(_default == null) _default = new FormatPredefinedDataBarSchemes();
				return _default;
			}
		}
		AppearanceDefault AppearanceDefaultNegativeRedSolid { get { return new AppearanceDefault() { BackColor = FromInt(0xea4747), BorderColor = Color.Empty, ForeColor = Color.White }; } }
		AppearanceDefault AppearanceDefaultNegativeRedGradient { get { return new AppearanceDefault() { BackColor2 = FromInt(0xea4747), BackColor = Color.Transparent, BorderColor = FromInt(0xea4747) }; } }
		AppearanceDefault GetAppearanceDefaultPositiveSolid(int color) { return new AppearanceDefault() { BackColor = FromInt(color), BorderColor = Color.Empty, ForeColor = Color.White }; }
		AppearanceDefault GetAppearanceDefaultPositiveGradient(int color) { return new AppearanceDefault() { BackColor = FromInt(color), BackColor2 = Color.Transparent, BorderColor = FromInt(color) }; }
		protected override void Register(Dictionary<string, FormatPredefinedDataBarScheme> sets) {
			Register(sets, "Blue", "Blue", GetAppearanceDefaultPositiveSolid(0x377fd0), AppearanceDefaultNegativeRedSolid, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarBlue));
			Register(sets, "Light Blue", "Light Blue", GetAppearanceDefaultPositiveSolid(0x1caedb), AppearanceDefaultNegativeRedSolid, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarLightBlue));
			Register(sets, "Green", "Green", GetAppearanceDefaultPositiveSolid(0x8cc542), AppearanceDefaultNegativeRedSolid, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarGreen));
			Register(sets, "Yellow", "Yellow", GetAppearanceDefaultPositiveSolid(0xfec22c), AppearanceDefaultNegativeRedSolid, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarYellow));
			Register(sets, "Orange", "Orange", GetAppearanceDefaultPositiveSolid(0xff9500), AppearanceDefaultNegativeRedSolid, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarOrange));
			Register(sets, "Mint", "Mint", GetAppearanceDefaultPositiveSolid(0x4ebf8f), AppearanceDefaultNegativeRedSolid, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarMint));
			Register(sets, "Violet", "Violet", GetAppearanceDefaultPositiveSolid(0x6462ca), AppearanceDefaultNegativeRedSolid, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarViolet));
			Register(sets, "Raspberry", "Raspberry", GetAppearanceDefaultPositiveSolid(0xb04b8b), AppearanceDefaultNegativeRedSolid, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarRaspberry));
			Register(sets, "Coral", "Coral", GetAppearanceDefaultPositiveSolid(0xf76a43), AppearanceDefaultNegativeRedSolid, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarCoral));
			Register(sets, "Blue Gradient", "Blue Gradient", GetAppearanceDefaultPositiveGradient(0x377fd0), AppearanceDefaultNegativeRedGradient, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarBlueGradient));
			Register(sets, "Light Blue Gradient", "Light Blue Gradient", GetAppearanceDefaultPositiveGradient(0x1caedb), AppearanceDefaultNegativeRedGradient, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarLightBlueGradient));
			Register(sets, "Green Gradient", "Green Gradient", GetAppearanceDefaultPositiveGradient(0x8cc542), AppearanceDefaultNegativeRedGradient, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarGreenGradient));
			Register(sets, "Yellow Gradient", "Yellow Gradient", GetAppearanceDefaultPositiveGradient(0xfec22c), AppearanceDefaultNegativeRedGradient, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarYellowGradient));
			Register(sets, "Orange Gradient", "Orange Gradient", GetAppearanceDefaultPositiveGradient(0xff9500), AppearanceDefaultNegativeRedGradient, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarOrangeGradient));
			Register(sets, "Mint Gradient", "Mint Gradient", GetAppearanceDefaultPositiveGradient(0x4ebf8f), AppearanceDefaultNegativeRedGradient, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarMintGradient));
			Register(sets, "Violet Gradient", "Violet Gradient", GetAppearanceDefaultPositiveGradient(0x6462ca), AppearanceDefaultNegativeRedGradient, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarVioletGradient));
			Register(sets, "Raspberry Gradient", "Raspberry Gradient", GetAppearanceDefaultPositiveGradient(0xb04b8b), AppearanceDefaultNegativeRedGradient, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarRaspberryGradient));
			Register(sets, "Coral Gradient", "Coral Gradient", GetAppearanceDefaultPositiveGradient(0xf76a43), AppearanceDefaultNegativeRedGradient, Color.Black, Localizer.Active.GetLocalizedString(StringId.DataBarCoralGradient));
		}
		void Register(Dictionary<string, FormatPredefinedDataBarScheme> sets, string key, string hashKey, AppearanceDefault positive, AppearanceDefault negative, Color axis, string title) {
			sets.Add(hashKey, new FormatPredefinedDataBarScheme(key) { Positive = positive, Negative = negative, AxisColor = axis, Title = title });
		}
	}
	public class FormatPredefinedAppearances : FormatPredefinedBase<FormatPredefinedAppearance> {
		static FormatPredefinedAppearances _default;
		public static FormatPredefinedAppearances Default {
			get {
				if(_default == null) _default = new FormatPredefinedAppearances();
				return _default;
			}
		}
		protected override void Register(Dictionary<string, FormatPredefinedAppearance> sets) {
			Register(sets, "Red Fill, Red Text", "Red Fill, Red Text",
				new AppearanceDefault() { BackColor = FromInt(0xf8c4c4), ForeColor = FromInt(0xc60f1c) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceRedFillRedText));
			Register(sets, "Red Fill, Red Text", "Red Fill, Red Text" + DarkSkinSuffix,
				new AppearanceDefault() { BackColor = FromInt(0x710000), ForeColor = FromInt(0xea4747) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceRedFillRedText) + "[Dark]");
			Register(sets, "Yellow Fill, Yellow Text", "Yellow Fill, Yellow Text",
				new AppearanceDefault() { BackColor = FromInt(0xfee095), ForeColor = FromInt(0xb55100) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceYellowFillYellowText));
			Register(sets, "Yellow Fill, Yellow Text", "Yellow Fill, Yellow Text" + DarkSkinSuffix,
				new AppearanceDefault() { BackColor = FromInt(0xb36a05), ForeColor = FromInt(0xfec22c) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceYellowFillYellowText) + "[Dark]");
			Register(sets, "Green Fill, Green Text", "Green Fill, Green Text",
				new AppearanceDefault() { BackColor = FromInt(0xc5e2a0), ForeColor = FromInt(0x2a5e00) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceGreenFillGreenText));
			Register(sets, "Green Fill, Green Text", "Green Fill, Green Text" + DarkSkinSuffix,
				new AppearanceDefault() { BackColor = FromInt(0x12752c), ForeColor = FromInt(0x8cc542) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceGreenFillGreenText) + "[Dark]");
			Register(sets, "Red Fill", "Red Fill",
				new AppearanceDefault() { BackColor = FromInt(0xf8c4c4) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceRedFill));
			Register(sets, "Red Fill", "Red Fill" + DarkSkinSuffix,
				new AppearanceDefault() { BackColor = FromInt(0x710000) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceRedFill) + "[Dark]");
			Register(sets, "Red Text", "Red Text",
				new AppearanceDefault() { ForeColor = FromInt(0xc60f1c) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceRedText));
			Register(sets, "Red Text", "Red Text" + DarkSkinSuffix,
				new AppearanceDefault() { ForeColor = FromInt(0xea4747) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceRedText) + "[Dark]");
			Register(sets, "Green Fill", "Green Fill",
				new AppearanceDefault() { BackColor = FromInt(0xc5e2a0) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceGreenFill));
			Register(sets, "Green Fill", "Green Fill" + DarkSkinSuffix,
				new AppearanceDefault() { BackColor = FromInt(0x12752c) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceGreenFill) + "[Dark]");
			Register(sets, "Green Text", "Green Text",
				new AppearanceDefault() { ForeColor = FromInt(0x2a5e00) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceGreenText));
			Register(sets, "Green Text", "Green Text" + DarkSkinSuffix,
				new AppearanceDefault() { ForeColor = FromInt(0x8cc542) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceGreenText) + "[Dark]");
			Register(sets, "Bold Text", "Bold Text",
				new AppearanceDefault() { Font = new Font(AppearanceObject.DefaultFont, FontStyle.Bold) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceBoldText));
			Register(sets, "Italic Text", "Italic Text",
				new AppearanceDefault() { Font = new Font(AppearanceObject.DefaultFont, FontStyle.Italic) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceItalicText));
			Register(sets, "Strikeout Text", "Strikeout Text",
				new AppearanceDefault() { Font = new Font(AppearanceObject.DefaultFont, FontStyle.Strikeout) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceStrikeoutText));
			Register(sets, "Red Bold Text", "Red Bold Text",
				new AppearanceDefault() { ForeColor = FromInt(0xc60f1c), Font = new Font(AppearanceObject.DefaultFont, FontStyle.Bold) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceRedBoldText));
			Register(sets, "Red Bold Text", "Red Bold Text" + DarkSkinSuffix,
				new AppearanceDefault() { ForeColor = FromInt(0xea4747), Font = new Font(AppearanceObject.DefaultFont, FontStyle.Bold) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceRedBoldText) + "[Dark]");
			Register(sets, "Green Bold Text", "Green Bold Text",
				new AppearanceDefault() { ForeColor = FromInt(0x2a5e00), Font = new Font(AppearanceObject.DefaultFont, FontStyle.Bold) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceGreenBoldText));
			Register(sets, "Green Bold Text", "Green Bold Text" + DarkSkinSuffix,
				new AppearanceDefault() { ForeColor = FromInt(0x8cc542), Font = new Font(AppearanceObject.DefaultFont, FontStyle.Bold) }, Localizer.Active.GetLocalizedString(StringId.FormatPredefinedAppearanceGreenBoldText) + "[Dark]");
		}
		void Register(Dictionary<string, FormatPredefinedAppearance> sets, string key, string hashKey, AppearanceDefault appearance, string title) {
			sets.Add(hashKey, new FormatPredefinedAppearance(key) { Appearance = appearance, Title = title });
		}
	}
}
