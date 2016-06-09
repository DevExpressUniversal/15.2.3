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
using System.Drawing.Design;
using DevExpress.Data.Browsing;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Exceptions;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.UI {
	[ToolboxItem(false)]
	[DesignTimeVisibleAttribute(false)]
	[System.Drawing.ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "FormattingRule.bmp")]
	[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.FormattingRule")]
	[Designer("DevExpress.XtraReports.Design.Designers.FormatingRuleDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull)]
	public class FormattingRule : DataContainerComponent, IDisplayNamePropertyContainer {
		#region Fields & Properties
		CriteriaOperator criteriaOperator;
		Formatting formatting = new Formatting();
		FormattingRuleSheet owner;
		string name = String.Empty;
		string condition = String.Empty;
		bool disposed;
		[DefaultValue("")]
		[Browsable(false)]
		[NotifyParentProperty(true)]
		[XtraSerializableProperty]
		public string Name {
			get { return Site != null ? Site.Name : name; }
			set { name = value; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("FormattingRuleDataSource")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.FormattingRule.DataSource")]
		public override object DataSource {
			get { return base.DataSource; }
			set { base.DataSource = value; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("FormattingRuleDataMember")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.FormattingRule.DataMember")]
		public override string DataMember {
			get { return base.DataMember; }
			set { base.DataMember = value; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("FormattingRuleCondition")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.FormattingRule.Condition")]
		[SRCategory(ReportStringId.CatBehavior)]
		[DefaultValue("")]
		[Editor("DevExpress.XtraReports.Design.FormattingRuleExpressionEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor))]
		[NotifyParentProperty(true)]
		[XtraSerializableProperty]
		[TypeConverter(typeof(DevExpress.XtraReports.Design.TextPropertyTypeConverter))]
		public string Condition {
			get { return GetCondition(); }
			set { SetCondition(value); }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("FormattingRuleFormatting")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.FormattingRule.Formatting")]
		[SRCategory(ReportStringId.CatBehavior)]
		[TypeConverter(typeof(DevExpress.XtraReports.Design.XtraReportConditionalFormattingTypeConverter))]
		[NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public Formatting Formatting {
			get { return formatting; }
		}
		internal FormattingRuleSheet Owner {
			get { return owner; }
			set { owner = value; }
		}
		protected override IDataContainer Parent {
			get { return owner != null ? owner.Report : null; }
		}
		bool ReportIsLoading {
			get { return owner == null || owner.Report.Loading; }
		}
		#endregion
		#region Methods
		public void ApplyStyle(XRControlStyle style) {
			formatting = new Formatting(formatting.Name, formatting.Visible);
			FormattingRuleCollection.OverlayStyle(formatting, style);
		}
		public override string ToString() {
			return this.Name;
		}
		internal bool EvaluateCondition(DataContext dataContext) {
			if(DesignMode || Owner.Report.IsSerializing || string.IsNullOrEmpty(Condition))
				return false;
			try {
				DataBrowser browser = dataContext[GetEffectiveDataSource(), GetEffectiveDataMember()];
				if(browser == null)
					return false;
				XREvaluatorContextDescriptor descriptor = new XREvaluatorContextDescriptor(Owner.Report.Parameters, dataContext, GetEffectiveDataSource(), GetEffectiveDataMember());
				CriteriaOperator evaluatingCriteriaOperator = (!object.ReferenceEquals(criteriaOperator, null)) ? criteriaOperator : CriteriaOperator.Parse(condition, null);
				ExpressionEvaluator evaluator = new ExpressionEvaluator(descriptor, evaluatingCriteriaOperator);
				return (bool)evaluator.Evaluate(browser.Current);
			} catch {
				return false;
			}
		}
		string GetCondition() {
			if(condition == string.Empty || owner == null)
				return string.Empty;
			try {
				if(ValidateCondition(condition, criteriaOperator))
					return condition;
			} catch {
			}
			return string.Empty;
		}
		bool ValidateCondition(string value, CriteriaOperator criteriaOperator) {
			ConditionBooleanTypeValidator validator = new ConditionBooleanTypeValidator(GetEffectiveDataSource(), GetEffectiveDataMember());
			if(object.ReferenceEquals(criteriaOperator, null)) {
				bool result = validator.ValidateCondition(value);
				this.criteriaOperator = validator.CriteriaOperator;
				FunctionNameValidator.Validate(this.criteriaOperator);
				return result;
			}
			FunctionNameValidator.Validate(criteriaOperator);
			return validator.ValidateCondition(criteriaOperator);
		}
		void SetCondition(string value) {
			if(value == this.condition || value == string.Empty || this.ReportIsLoading) {
				this.condition = value;
				return;
			}
			try {
				this.condition = ValidateCondition(value, null) ? value : string.Empty;
			} catch {
				this.condition = string.Empty;
				throw;
			}
		}
		#region Disposing
		protected override void Dispose(bool disposing) {
			if(!disposed) {
				if(disposing) {
					formatting.Dispose();
				}
				disposed = true;
			}
			base.Dispose(disposing);
		}
		#endregion
		#region Serialization
		bool ShouldSerializeDataSource() {
			return ShouldSerializeDataSourceCore();
		}
		bool ShouldSerializeXmlDataSource() {
			return ShouldSerializeXmlDataSourceCore();
		}
		#endregion
		#endregion
		#region IDisplayNamePropertyContainer Members
		string IDisplayNamePropertyContainer.GetDisplayPropertyValue(string source) {
			return CriteriaFieldNameConverter.ToDisplayNames(Owner.Report, GetEffectiveDataSource(), GetEffectiveDataMember(), source);
		}
		string IDisplayNamePropertyContainer.GetRealPropertyValue(string source) {
			return CriteriaFieldNameConverter.ToRealNames(Owner.Report, GetEffectiveDataSource(), GetEffectiveDataMember(), source);
		}
		string IDisplayNamePropertyContainer.GetDisplayName(string source) {
			return CriteriaFieldNameConverter.ToDisplayName(Owner.Report, GetEffectiveDataSource(), GetEffectiveDataMember(), source);
		}
		#endregion
	}
	public class Formatting : XRControlStyle {
		#region Fields & Properties
		DefaultBoolean visible = DefaultBoolean.Default;
#if !SL
	[DevExpressXtraReportsLocalizedDescription("FormattingVisible")]
#endif
		[SRCategory(ReportStringId.CatBehavior)]
		[DefaultValue(DefaultBoolean.Default)]
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.ConditionFormatting.Visible")]
		[TypeConverter(typeof(DevExpress.XtraReports.Design.XRFormattingVisibleConverter))]
		[NotifyParentProperty(true)]
		[XtraSerializableProperty]
		public DefaultBoolean Visible {
			get { return visible; }
			set { visible = value; }
		}
		#endregion
		#region Constructors
		public Formatting() {
		}
		public Formatting(string name, DefaultBoolean visible) {
			this.Name = name;
			this.visible = visible;
		}
		#endregion
		#region Methods
		void ResetVisible() {
			visible = DefaultBoolean.Default;
		}
		#endregion
	}
}
