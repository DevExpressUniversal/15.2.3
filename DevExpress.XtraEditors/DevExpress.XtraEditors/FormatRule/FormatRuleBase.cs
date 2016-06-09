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
using DevExpress.XtraEditors.Helpers;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.Export.Xl;
namespace DevExpress.XtraEditors {
	[TypeConverter(typeof(UniversalTypeConverterEx))]
	public abstract class FormatConditionRuleBase : IFormatConditionRuleValueQuery, IFormatConditionDrawPreview {
		IFormatConditionRuleOwner owner;
		internal int stateId = 0;
		internal static bool UseTryCatch = true;
		protected internal virtual bool IsLoading { get { return lockUpdate != 0 || Owner == null || Owner.IsLoading; } } 
		[Browsable(false)]
		public virtual bool IsValid { get { return true; } }
		public override string ToString() {
			return GetType().Name;
		}
		internal void SetOwner(IFormatConditionRuleOwner owner) {
			this.owner = owner;
		}
		protected internal virtual void ResetDataSourceProperties() {
		}
		public virtual bool IsFit(IFormatConditionRuleValueProvider valueProvider) {
			if(!IsValid) return false;
			if(!UseTryCatch) return IsFitCore(valueProvider);
			try {
				return IsFitCore(valueProvider);				  
			}
			catch {
				return false;
			}
		}
		protected internal virtual bool GetIsRightToLeft() { return Owner == null ? false : Owner.GetIsRightToLeft();  }
		int lockUpdate = 0;
		FormatConditionRuleChangeType updateModified = FormatConditionRuleChangeType.None;
		public void BeginUpdate() {
			if(lockUpdate++ == 0) {
				updateModified = FormatConditionRuleChangeType.None;
			}
		}
		public void EndUpdate() {
			if(--lockUpdate == 0) {
				if(updateModified != FormatConditionRuleChangeType.None) OnModified(updateModified);
			}
		}
		protected internal FormatConditionRuleBase Clone() {
			var res = CreateInstance();
			res.Assign(this);
			return res;
		}
		public abstract FormatConditionRuleBase CreateInstance();
		public void Assign(FormatConditionRuleBase rule) {
			BeginUpdate();
			try {
				AssignCore(rule);
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual void AssignCore(FormatConditionRuleBase rule) {
		}
		internal void InvalidateState() {
			this.stateId++;
		}
		protected internal virtual void OnModified(FormatConditionRuleChangeType modifiedType) {
			if(modifiedType != FormatConditionRuleChangeType.UI) {
				InvalidateState();
			}
			if(lockUpdate != 0) updateModified |= modifiedType;
			if(IsLoading) return;
			if(Owner != null) Owner.OnModified(modifiedType);
		}
		protected internal IFormatConditionRuleOwner Owner { get { return owner; } }
		protected internal DevExpress.LookAndFeel.UserLookAndFeel LookAndFeel { get { return Owner == null || Owner.LookAndFeel == null ? UserLookAndFeel.Default : Owner.LookAndFeel; } } 
		protected abstract bool IsFitCore(IFormatConditionRuleValueProvider valueProvider);
		protected object CheckNullValue(object val) {
			if(val == DBNull.Value) return null;
			return val;
		}
		protected decimal? CheckQueryNumericValue(IFormatConditionRuleValueProvider valueProvider) {
			return CheckQueryNumericValue(valueProvider.GetValue(this));
		}
		protected decimal? CheckQueryNumericValue(object value) {
			object actualValue = CheckNullValue(value);
			return ConvertToNumeric(actualValue);
		}
		protected bool CheckQueryValue(IFormatConditionRuleValueProvider value, out object actualValue) {
			return CheckQueryValue(value.GetValue(this), out actualValue);
		}
		protected bool CheckQueryValue(object value, out object actualValue) {
			actualValue = CheckNullValue(value);
			if(value == null) return false;
			if(BaseEdit.IsNotLoadedValue(actualValue)) return false;
			return true;
		}
		protected internal static decimal GetPercentValue(decimal min, decimal max, decimal percent) {
			if(min >= max) return max;
			if(percent <= 0) return min;
			if(percent >= 100) return max;
			decimal delta = max - min;
			return min + (delta * (percent / 100));
		}
		protected internal static decimal FindPercentValue(decimal min, decimal max, decimal value) {
			if(min >= max) return 100;
			if(value < min) return 0;
			if(value >= max) return 100;
			decimal delta = max - min;
			return ((value  - min)/ delta) * 100;
		}
		#region IFormatConditionRuleValueQuery Members
		FormatConditionRuleState IFormatConditionRuleValueQuery.GetRuleState() {
			return GetQueryKindStateCore();
		}
		protected virtual FormatConditionRuleState GetQueryKindStateCore() {
			return new FormatConditionRuleState(this, FormatRuleValueQueryKind.None);
		}
		#endregion
		protected virtual int CompareValues(object val1, object val2) {
			return Comparer.Default.Compare(val1, val2);
		}
		protected virtual decimal? ConvertToNumeric(object val) {
			return ConvertToDecimal(val);
		}
		public static decimal? ConvertToDecimal(object value) {
			if(value == null) return null;
			var tc = Type.GetTypeCode(value.GetType());
			if(tc == TypeCode.DateTime)
				return ((DateTime)value).Ticks;
			if(tc == TypeCode.Decimal)
				return (decimal)value;
			if(IsNumericTypeCode(tc))
				return Convert.ToDecimal(value);
			return null;
		}
		internal static bool IsNumericTypeCode(TypeCode typeCode) {
			return typeCode >= TypeCode.SByte && typeCode <= TypeCode.Decimal;
		}
		internal static bool IsNumericOrDateTimeTypeCode(TypeCode typeCode) {
			return IsNumericTypeCode(typeCode) || typeCode == TypeCode.DateTime;
		}
		internal string GetSerializableTypeName() {
			if(GetType().Assembly.Equals(typeof(FormatConditionRuleBase).Assembly)) return "#" + GetType().Name;
			return GetType().AssemblyQualifiedName;
		}
		internal static FormatConditionRuleBase CreateInstance(string ruleTypeName) {
			try {
				if(ruleTypeName.StartsWith("#")) {
					string fullName = typeof(FormatConditionRuleBase).Namespace + "." + ruleTypeName.Substring(1);
					var res = typeof(FormatConditionRuleBase).Assembly.GetType(fullName, false);
					if(res != null) return (FormatConditionRuleBase)Activator.CreateInstance(res);
					return new FormatConditionRuleValue();
				}
				var resType = Type.GetType(ruleTypeName, false);
				if(resType != null) return (FormatConditionRuleBase)Activator.CreateInstance(resType);
			}
			catch {
			}
			return new FormatConditionRuleValue();
		}
		protected internal virtual void ResetVisualCache() { }
		void IFormatConditionDrawPreview.Draw(FormatConditionDrawPreviewArgs e) {
			using(var cache = new GraphicsCache(e.Graphics)) {
				DrawPreviewCore(cache, e);
			}
		}
		protected abstract void DrawPreviewCore(GraphicsCache cache, FormatConditionDrawPreviewArgs e);
	}
	public partial class ExportHelper {
		public static XlDifferentialFormatting GetAppearanceFormatPredefinedAppearances(AppearanceObjectEx appearance, DevExpress.LookAndFeel.UserLookAndFeel lookAndFeel, string predefinedName) {
			if(!string.IsNullOrEmpty(predefinedName))
				return ConvertConditionAppearance(FormatPredefinedAppearances.Default.Find(lookAndFeel, predefinedName));
			return XlDifferentialFormatting.CopyObject(ConvertConditionAppearance(appearance));
		}
		public static XlDifferentialFormatting ConvertConditionAppearance(FormatPredefinedAppearance appearance) {
			AppearanceDefault ad = appearance.Appearance;
			return CreateConditionAppearance(ad.BackColor, ad.ForeColor, ad.Font);
		}
		public static XlDifferentialFormatting ConvertConditionAppearance(AppearanceObject appearance) {
			return CreateConditionAppearance(appearance.GetBackColor(), appearance.GetForeColor(), appearance.GetFont());
		}
		private static XlDifferentialFormatting CreateConditionAppearance(Color backColor, Color foreColor, Font font) {
			XlDifferentialFormatting result = new XlDifferentialFormatting();
			result.Font = Convert(font, foreColor);
			result.Fill = new XlFill() {
				BackColor = backColor,
				ForeColor = foreColor,
				PatternType = XlPatternType.Solid,
			};
			return result;
		}
		public static XlFont Convert(Font font, Color foreColor) {
			XlFont result = new XlFont();
			result.Color = foreColor;
			if(font == null) return result;
			if(font.Underline) result.Underline = XlUnderlineType.Single;
			result.Bold = font.Bold;
			result.StrikeThrough = font.Strikeout;
			result.Italic = font.Italic;
			return result;
		}
		#region IconSets
		static Dictionary<string, XlCondFmtIconSetType> iconSetTypeTable = CreateIconSetTypeTable();
		static Dictionary<string, XlCondFmtIconSetType> CreateIconSetTypeTable() {
			Dictionary<string, XlCondFmtIconSetType> result = new Dictionary<string, XlCondFmtIconSetType>();
			result.Add("Arrows3Colored", XlCondFmtIconSetType.Arrows3);
			result.Add("PositiveNegativeArrows", XlCondFmtIconSetType.Arrows3);
			result.Add("Arrows3Gray", XlCondFmtIconSetType.ArrowsGray3);
			result.Add("PositiveNegativeArrowsGray", XlCondFmtIconSetType.ArrowsGray3);
			result.Add("Arrows4Colored", XlCondFmtIconSetType.Arrows4);
			result.Add("Arrows4Gray", XlCondFmtIconSetType.ArrowsGray4);
			result.Add("Arrows5Colored", XlCondFmtIconSetType.Arrows5);
			result.Add("Arrows5Gray", XlCondFmtIconSetType.ArrowsGray5);
			result.Add("Boxes5", XlCondFmtIconSetType.Boxes5);
			result.Add("Flags3", XlCondFmtIconSetType.Flags3);
			result.Add("Quarters5", XlCondFmtIconSetType.Quarters5);
			result.Add("Ratings4", XlCondFmtIconSetType.Rating4);
			result.Add("Ratings5", XlCondFmtIconSetType.Rating5);
			result.Add("RedToBlack", XlCondFmtIconSetType.RedToBlack4);
			result.Add("Signs3", XlCondFmtIconSetType.Signs3);
			result.Add("Stars3", XlCondFmtIconSetType.Stars3);
			result.Add("Symbols3Uncircled", XlCondFmtIconSetType.Symbols3);
			result.Add("Symbols3Circled", XlCondFmtIconSetType.Symbols3Circled);
			result.Add("TrafficLights3Unrimmed", XlCondFmtIconSetType.TrafficLights3);
			result.Add("TrafficLights3Rimmed", XlCondFmtIconSetType.TrafficLights3Black);
			result.Add("TrafficLights4", XlCondFmtIconSetType.TrafficLights4);
			result.Add("PositiveNegativeTriangles", XlCondFmtIconSetType.Triangles3);
			result.Add("Triangles3", XlCondFmtIconSetType.Triangles3);
			return result;
		}
		public static XlCondFmtIconSetType GetIconSetType(string iconSetTypeName) {
			XlCondFmtIconSetType value;
			if(iconSetTypeTable.TryGetValue(iconSetTypeName, out value)) return value;
			return 0;
		}
		#endregion IconSets
		#region DateOccuring
		static Dictionary<FilterDateType, XlCondFmtTimePeriod> DateOccuringTypeTable = CreateDateOccuringTypeTable();
		static Dictionary<FilterDateType, XlCondFmtTimePeriod> CreateDateOccuringTypeTable() {
			Dictionary<FilterDateType, XlCondFmtTimePeriod> result = new Dictionary<FilterDateType, XlCondFmtTimePeriod>();
			result.Add(FilterDateType.Today, XlCondFmtTimePeriod.Today);
			result.Add(FilterDateType.Tomorrow, XlCondFmtTimePeriod.Tomorrow);
			result.Add(FilterDateType.Yesterday, XlCondFmtTimePeriod.Yesterday);
			result.Add(FilterDateType.EarlierThisWeek, XlCondFmtTimePeriod.Last7Days); 
			result.Add(FilterDateType.MonthAgo1, XlCondFmtTimePeriod.LastMonth); 
			result.Add(FilterDateType.MonthAfter1, XlCondFmtTimePeriod.NextMonth); 
			result.Add(FilterDateType.ThisWeek, XlCondFmtTimePeriod.ThisWeek);
			result.Add(FilterDateType.NextWeek, XlCondFmtTimePeriod.NextWeek);
			result.Add(FilterDateType.LastWeek, XlCondFmtTimePeriod.LastWeek);
			result.Add(FilterDateType.ThisMonth, XlCondFmtTimePeriod.ThisMonth);
			return result;
		}
		public static XlCondFmtTimePeriod GetDateOccuringType(FilterDateType DateOccuringType) {
			XlCondFmtTimePeriod value;
			if(DateOccuringTypeTable.TryGetValue(DateOccuringType, out value)) return value;
			return 0;
		}
		#endregion       
		#region AboveBelowAverage
		static Dictionary<FormatConditionAboveBelowType, XlCondFmtAverageCondition> AboveBelowAverageTypeTable = CreateAboveBelowAverageTypeTable();
		static Dictionary<FormatConditionAboveBelowType, XlCondFmtAverageCondition> CreateAboveBelowAverageTypeTable() {
			Dictionary<FormatConditionAboveBelowType, XlCondFmtAverageCondition> result = new Dictionary<FormatConditionAboveBelowType, XlCondFmtAverageCondition>();
			result.Add(FormatConditionAboveBelowType.Above, XlCondFmtAverageCondition.Above);
			result.Add(FormatConditionAboveBelowType.Below, XlCondFmtAverageCondition.Below);
			result.Add(FormatConditionAboveBelowType.EqualOrAbove, XlCondFmtAverageCondition.AboveOrEqual);
			result.Add(FormatConditionAboveBelowType.EqualOrBelow, XlCondFmtAverageCondition.BelowOrEqual);
			return result;
		}
		public static XlCondFmtAverageCondition GetAboveBelowAverageType(FormatConditionAboveBelowType AboveBelowAverageType) {
			XlCondFmtAverageCondition value;
			if(AboveBelowAverageTypeTable.TryGetValue(AboveBelowAverageType, out value)) return value;
			return 0;
		}
		#endregion
	}
}
