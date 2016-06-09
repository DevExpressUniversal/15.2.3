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
using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl;
namespace DevExpress.XtraEditors {
	public abstract class FormatConditionRuleAppearanceBase : FormatConditionRuleBase, IAppearanceOwner, IFormatRuleAppearance {
		AppearanceObjectEx appearance;
		string predefinedName = "";
		public FormatConditionRuleAppearanceBase() {
			this.appearance = new AppearanceObjectEx(this);
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
		}
		[XtraSerializableProperty]
		[DefaultValue("")]
		[RefreshProperties(System.ComponentModel.RefreshProperties.Repaint)]
		[Editor(typeof(DevExpress.XtraEditors.Design.FormatPredefinedAppearancesUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string PredefinedName { 
			get { return predefinedName; } 
			set {
				if(PredefinedName == value) return;
				if(value == null) value = "";
				predefinedName = value;
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		protected override void AssignCore(FormatConditionRuleBase rule) {
			base.AssignCore(rule);
			var source = rule as FormatConditionRuleAppearanceBase;
			if(source == null) return;
			Appearance.Assign(source.Appearance);
			PredefinedName = source.PredefinedName;
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			OnModified(FormatConditionRuleChangeType.UI);
		}
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		void ResetAppearance() { Appearance.Reset(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("FormatConditionRuleAppearanceBaseAppearance"),
#endif
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.FormatConditionRuleAppearanceBase.Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)
		]
		public AppearanceObjectEx Appearance { get { return appearance; } }
		#region IAppearanceOwner Members
		bool IAppearanceOwner.IsLoading { get { return false; } } 
		#endregion
		AppearanceObjectEx IFormatRuleAppearance.QueryAppearance(FormatRuleAppearanceArgs e) { 
			if(string.IsNullOrEmpty(PredefinedName)) return Appearance;
			FormatPredefinedAppearance predefined = FormatPredefinedAppearances.Default.Find(LookAndFeel, PredefinedName);
			if(predefined == null) return Appearance;
			AppearanceObjectEx res = new AppearanceObjectEx();
			AppearanceHelper.Combine(res, new AppearanceObject[] { Appearance}, predefined.Appearance);
			return res;
		}
		protected override void DrawPreviewCore(Utils.Drawing.GraphicsCache cache, FormatConditionDrawPreviewArgs e) {
			var appearance = ((IFormatRuleAppearance)this).QueryAppearance(new FormatRuleAppearanceArgs(null, null));
			appearance.FillRectangle(cache, e.Bounds);
			appearance.DrawString(cache, e.Text, Rectangle.Inflate(e.Bounds, -3, 0));
		}
	}
	public class FormatConditionRuleAboveBelowAverage : FormatConditionRuleAppearanceBase, IFormatConditionRuleAboveBelowAverage {
		FormatConditionAboveBelowType averageType = FormatConditionAboveBelowType.Above;
		[DefaultValue(FormatConditionAboveBelowType.Above)]
		[XtraSerializableProperty]
		public FormatConditionAboveBelowType AverageType {
			get { return averageType; }
			set {
				if(AverageType == value) return;
				averageType = value;
				OnModified(FormatConditionRuleChangeType.Data);
			}
		}
		protected override void AssignCore(FormatConditionRuleBase rule) {
			base.AssignCore(rule);
			var source = rule as FormatConditionRuleAboveBelowAverage;
			if(source == null) return;
			AverageType = source.AverageType;
		}
		public override FormatConditionRuleBase CreateInstance() {
			return new FormatConditionRuleAboveBelowAverage();
		}
		protected override bool IsFitCore(IFormatConditionRuleValueProvider valueProvider) {
			object val;
			if(!CheckQueryValue(valueProvider, out val)) return false;
			decimal? numValue = ConvertToNumeric(val);
			if(numValue == null) return false;
			decimal? average = ConvertToNumeric(valueProvider.GetQueryValue(this));
			if(average == null) return false;
			int res = decimal.Compare(numValue.Value, average.Value);
			switch(AverageType) {
				case FormatConditionAboveBelowType.Above:
					return res > 0;
				case FormatConditionAboveBelowType.Below:
					return res < 0;
				case FormatConditionAboveBelowType.EqualOrAbove :
					return res >= 0;
				case FormatConditionAboveBelowType.EqualOrBelow:
					return res <= 0;
			}
			return false; 
		}
		protected override FormatConditionRuleState GetQueryKindStateCore() {
			return new FormatConditionRuleState(this, FormatRuleValueQueryKind.Average);
		}
		#region IFormatConditionRuleAboveBelowAverage
		XlCondFmtAverageCondition IFormatConditionRuleAboveBelowAverage.Condition {
			get { return ExportHelper.GetAboveBelowAverageType(AverageType); }
		}
		XlDifferentialFormatting IFormatConditionRuleAboveBelowAverage.Formatting {
			get {
				return ExportHelper.GetAppearanceFormatPredefinedAppearances(Appearance, LookAndFeel, PredefinedName);
			}
		}	  
		#endregion
	}
}
