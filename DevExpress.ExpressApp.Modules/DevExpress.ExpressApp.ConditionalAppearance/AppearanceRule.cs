#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.ConditionalAppearance {
	public enum AppearanceItemType { ViewItem, Action, LayoutItem }
	public enum AppearanceContext { ListView, DetailView, Any }
#if !SL
	[DevExpressExpressAppConditionalAppearanceLocalizedDescription("ConditionalAppearanceIAppearance")]
#endif
	public interface IAppearance {
#if !SL
	[DevExpressExpressAppConditionalAppearanceLocalizedDescription("IAppearancePriority")]
#endif
		[Category("Behavior")]
		int Priority { get; set; }
#if !SL
	[DevExpressExpressAppConditionalAppearanceLocalizedDescription("IAppearanceFontStyle")]
#endif
		[Category("Appearance")]
		FontStyle? FontStyle { get; set; }
#if !SL
	[DevExpressExpressAppConditionalAppearanceLocalizedDescription("IAppearanceFontColor")]
#endif
		[Category("Appearance")]
		Color? FontColor { get; set; }
#if !SL
	[DevExpressExpressAppConditionalAppearanceLocalizedDescription("IAppearanceBackColor")]
#endif
		[Category("Appearance")]
		Color? BackColor { get; set; }
#if !SL
	[DevExpressExpressAppConditionalAppearanceLocalizedDescription("IAppearanceVisibility")]
#endif
		[Category("Appearance")]
		ViewItemVisibility? Visibility { get; set; }
#if !SL
	[DevExpressExpressAppConditionalAppearanceLocalizedDescription("IAppearanceEnabled")]
#endif
		[Category("Appearance")]
		bool? Enabled { get; set; }
	}
#if !SL
	[DevExpressExpressAppConditionalAppearanceLocalizedDescription("ConditionalAppearanceIAppearanceRuleProperties")]
#endif
	public interface IAppearanceRuleProperties : IAppearance {
#if !SL
	[DevExpressExpressAppConditionalAppearanceLocalizedDescription("IAppearanceRulePropertiesTargetItems")]
#endif
		[Required]
		[Category("Data")]
		string TargetItems { get; set; }
#if !SL
	[DevExpressExpressAppConditionalAppearanceLocalizedDescription("IAppearanceRulePropertiesAppearanceItemType")]
#endif
		[Category("Behavior")]
		[TypeConverter(typeof(AppearanceItemTypeConverter))]
		string AppearanceItemType { get; set; }
#if !SL
	[DevExpressExpressAppConditionalAppearanceLocalizedDescription("IAppearanceRulePropertiesCriteria")]
#endif
		[Category("Behavior")]
		[CriteriaOptions("DeclaringType")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string Criteria { get; set; }
#if !SL
	[DevExpressExpressAppConditionalAppearanceLocalizedDescription("IAppearanceRulePropertiesMethod")]
#endif
		[DataSourceProperty("MethodNames")]
		[Category("Behavior")]
		string Method { get; set; }
#if !SL
	[DevExpressExpressAppConditionalAppearanceLocalizedDescription("IAppearanceRulePropertiesContext")]
#endif
		[Category("Behavior")]
		[TypeConverter(typeof(AppearanceContextTypeConverter))]
		string Context { get; set; }
		[Browsable(false)]
		Type DeclaringType { get; }
	}
	public class AppearanceContextTypeConverter : StringConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(Enum.GetNames(typeof(AppearanceContext)));
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
	}
	public class AppearanceItemTypeConverter : StringConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(Enum.GetNames(typeof(AppearanceItemType)));
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
	}
	public class AppearanceRule {
		public static char[] Delimiters = new char[] { ',', ';' };
		public const string SelectAllString = "*";
		private IObjectSpace objectSpace;
		private LocalizedCriteriaWrapper wrapper = null;
		private LocalizedCriteriaWrapper CriteriaWrapper {
			get {
				if(wrapper == null) {
					CriteriaOperator criteria = null;
					if(objectSpace != null) {
						criteria = objectSpace.ParseCriteria(Properties.Criteria);
						wrapper = new LocalizedCriteriaWrapper(Properties.DeclaringType, criteria);
					}
					else {
						wrapper = new LocalizedCriteriaWrapper(Properties.DeclaringType, Properties.Criteria);
					}
				}
				return wrapper;
			}
		}
		private EvaluatorContextDescriptor defaultDescriptor = null;
		private EvaluatorContextDescriptor DefaultDescriptor {
			get {
				if(defaultDescriptor == null) {
					defaultDescriptor = objectSpace != null ? objectSpace.GetEvaluatorContextDescriptor(Properties.DeclaringType) : new EvaluatorContextDescriptorDefault(Properties.DeclaringType);
				}
				return defaultDescriptor;
			}
		}
		private static List<string> Parse(string items) {
			List<string> result = new List<string>();
			if(!string.IsNullOrEmpty(items)) {
				foreach(string item in items.Split(AppearanceRule.Delimiters, StringSplitOptions.RemoveEmptyEntries)) {
					result.Add(item.Trim());
				}
			}
			return result;
		}
		private bool ValidateByCriteria(object contextObject, EvaluatorContextDescriptor evaluatorContextDescriptor) {
			if(contextObject != null && evaluatorContextDescriptor == null && !Properties.DeclaringType.IsAssignableFrom(contextObject.GetType())) {
				return false;
			}
			LocalizedCriteriaWrapper wrapper = CriteriaWrapper;
			wrapper.UpdateParametersValues(contextObject); 
			EvaluatorContextDescriptor descriptor = evaluatorContextDescriptor ?? DefaultDescriptor;
			ExpressionEvaluator evaluator = objectSpace.GetExpressionEvaluator(descriptor, wrapper.CriteriaOperator);
			return evaluator.Fit(contextObject);
		}
		private bool ValidateByMethod(object contextObject) {
			if(contextObject != null && !contextObject.GetType().IsClass) {
				Tracing.Tracer.LogError("Conditional appeararance does not support 'ValidateByMethod' with AsyncServerMode.");
				return false;
			}
			Type contextObjectType = contextObject != null ? contextObject.GetType() : Properties.DeclaringType;
			MethodInfo parametrizedMethodInfo = contextObjectType.GetMethod(Properties.Method, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if(parametrizedMethodInfo != null && parametrizedMethodInfo.GetParameters().Length > 0) {
				if(parametrizedMethodInfo.ReturnType != typeof(bool)) {
					throw new InvalidOperationException(string.Format("The 'System.Boolean' result type is expected but the '{0}' method result type is '{1}'.",
						contextObjectType.Name + "." + parametrizedMethodInfo.Name, parametrizedMethodInfo.ReturnType.FullName));
				}
				try {
					return (bool)parametrizedMethodInfo.Invoke(null, new object[] { contextObject });
				}
				catch(Exception e) {
					throw new Exception("An error occurs while calling the '" + contextObjectType.Name + "." + parametrizedMethodInfo.Name + "' method: " + e.Message, e);
				}
			}
			MethodInfo methodInfo = contextObjectType.GetMethod(Properties.Method, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if(methodInfo == null) {
				throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotFindTheMemberWithinTheClass, Properties.Method, contextObjectType.FullName));
			}
			if(contextObject == null && !methodInfo.IsStatic) {
				Tracing.Tracer.LogWarning("Cannot call a non-static method '{0}' because there is no context object. Make the '{0}' method static to allow calling it in this scenario.", contextObjectType.Name + "." + methodInfo.Name);
				return false;
			}
			return (bool)methodInfo.Invoke(contextObject, null);
		}
#if DebugTest
		public IAppearanceRuleProperties DebugTest_Properties { get { return Properties; } }
#endif
		protected internal IAppearanceRuleProperties Properties { get; private set; }
		public AppearanceRule(IAppearanceRuleProperties properties, IObjectSpace objectSpace) {
			Guard.ArgumentNotNull(properties, "properties");
			this.Properties = properties;
			this.objectSpace = objectSpace;
		}
		private List<string> targetContexts;
		internal List<string> TargetContexts {
			get {
				if(targetContexts == null) {
					targetContexts = Parse(Properties.Context);
				}
				return targetContexts;
			}
		}
		private List<string> targetItems;
		internal List<string> TargetItems {
			get {
				if(targetItems == null) {
					targetItems = Parse(Properties.TargetItems);
				}
				return targetItems;
			}
		}
		public IList<IConditionalAppearanceItem> Validate(object contextObject, EvaluatorContextDescriptor evaluatorContextDescriptor) {
			return Validate(new object[] { contextObject }, evaluatorContextDescriptor);
		}
		internal IList<IConditionalAppearanceItem> Validate(object[] contextObjects, EvaluatorContextDescriptor evaluatorContextDescriptor) {
			bool ruleValid = GetRuleValid(contextObjects, evaluatorContextDescriptor);
			AppearanceState state = ruleValid ? AppearanceState.CustomValue : AppearanceState.ResetValue;
			return CreateAppearanceItems(state);
		}
		protected virtual IList<IConditionalAppearanceItem> CreateAppearanceItems(AppearanceState state) {
			List<IConditionalAppearanceItem> result = new List<IConditionalAppearanceItem>();
			if(Properties.Enabled.HasValue) {
				result.Add(new AppearanceItemEnabled(state, Properties.Priority, Properties.Enabled.Value));
			}
			if(Properties.FontStyle.HasValue) {
				result.Add(new AppearanceItemFontStyle(state, Properties.Priority, Properties.FontStyle.Value));
			}
			if(Properties.Visibility.HasValue) {
				result.Add(new AppearanceItemVisibility(state, Properties.Priority, Properties.Visibility.Value));
			}
			if(Properties.FontColor.HasValue) {
				result.Add(new AppearanceItemFontColor(state, Properties.Priority, Properties.FontColor.Value));
			}
			if(Properties.BackColor.HasValue) {
				result.Add(new AppearanceItemBackColor(state, Properties.Priority, Properties.BackColor.Value));
			}
			return result;
		}
		public bool GetRuleValid(object contextObject, EvaluatorContextDescriptor evaluatorContextDescriptor) {
			return GetRuleValid(new object[] { contextObject }, evaluatorContextDescriptor);
		}
		public bool GetRuleValid(object[] contextObjects, EvaluatorContextDescriptor evaluatorContextDescriptor) {
			object[] internalContextObjects = contextObjects;
			if(internalContextObjects == null || internalContextObjects.Length == 0) {
				internalContextObjects = new object[] { null };
			}
			bool ruleValid = false;
			bool isValidateByCriteria = string.IsNullOrEmpty(Properties.Method);
			foreach(object contextObject in internalContextObjects) {
				if(isValidateByCriteria) {
					ruleValid = ruleValid | ValidateByCriteria(contextObject, evaluatorContextDescriptor);
				}
				else {
					ruleValid = ruleValid | ValidateByMethod(contextObject);
				}
			}
			return ruleValid;
		}
	}
	public enum AppearanceState { ResetValue, CustomValue, None }
	public interface IConditionalAppearanceItem {
		bool IsCombineValue { get; }
		void CombineValue(IConditionalAppearanceItem item);
		AppearanceState State { get; }
		int Priority { get; }
	}
	public abstract class AppearanceItemBase : IConditionalAppearanceItem {
		private AppearanceState state;
		int priority;
		public AppearanceItemBase(AppearanceState state, int priority) {
			this.state = state;
			this.priority = priority;
		}
		public void Apply(object targetItem) {
			if(State != AppearanceState.None) {
				ApplyCore(targetItem);
			}
		}
		public AppearanceState State {
			get { return state; }
			set { state = value; }
		}
		public int Priority {
			get { return priority; }
			set { priority = value; }
		}
		public virtual bool IsCombineValue {
			get {
				return false;
			}
		}
		public virtual void CombineValue(IConditionalAppearanceItem item) { }
		protected abstract void ApplyCore(object targetItem);
	}
	public class AppearanceItemEnabled : AppearanceItemBase {
		private bool? enabled;
		public AppearanceItemEnabled(AppearanceState state, int priority, bool? enabled)
			: base(state, priority) {
			if(state == AppearanceState.CustomValue) {
				this.enabled = enabled;
			}
		}
		public bool? Enabled {
			get {
				return enabled;
			}
			set {
				State = value.HasValue ? AppearanceState.CustomValue : AppearanceState.ResetValue;
				enabled = value;
			}
		}
		public override bool IsCombineValue {
			get {
				return true;
			}
		}
		public override void CombineValue(IConditionalAppearanceItem item) {
			AppearanceItemEnabled _item = (AppearanceItemEnabled)item;
			if(_item.Enabled.HasValue) {
				Enabled = Enabled.HasValue ? Enabled.Value && _item.Enabled.Value : _item.Enabled.Value;
			}
		}
		protected override void ApplyCore(object targetItem) {
			if(targetItem is IAppearanceEnabled) {
				if(State != AppearanceState.None) {
					if(State == AppearanceState.CustomValue) {
						((IAppearanceEnabled)targetItem).Enabled = Enabled.Value;
					}
					else {
						((IAppearanceEnabled)targetItem).ResetEnabled();
					}
				}
			}
		}
	}
	public class AppearanceItemFontStyle : AppearanceItemBase {
		private FontStyle? fontStyle;
		public AppearanceItemFontStyle(AppearanceState state, int priority, FontStyle? fontStyle)
			: base(state, priority) {
			if(state == AppearanceState.CustomValue) {
				this.fontStyle = fontStyle;
			}
		}
		public FontStyle? FontStyle {
			get { return fontStyle; }
			set {
				State = value.HasValue ? AppearanceState.CustomValue : AppearanceState.ResetValue;
				fontStyle = value;
			}
		}
		public override bool IsCombineValue {
			get {
				return true;
			}
		}
		public override void CombineValue(IConditionalAppearanceItem item) {
			AppearanceItemFontStyle _item = (AppearanceItemFontStyle)item;
			if(_item.FontStyle.HasValue) {
				FontStyle = (!FontStyle.HasValue || FontStyle == System.Drawing.FontStyle.Regular) ?
						_item.FontStyle.Value : FontStyle | _item.FontStyle.Value;
			}
		}
		protected override void ApplyCore(object targetItem) {
			if(targetItem is IAppearanceFormat) {
				if(State != AppearanceState.None) {
					if(State == AppearanceState.CustomValue) {
						((IAppearanceFormat)targetItem).FontStyle = FontStyle.Value;
					}
					else {
						((IAppearanceFormat)targetItem).ResetFontStyle();
					}
				}
			}
		}
	}
	public class AppearanceItemVisibility : AppearanceItemBase {
		private ViewItemVisibility? visibility;
		public AppearanceItemVisibility(AppearanceState state, int priority, ViewItemVisibility? visibility)
			: base(state, priority) {
			if(state == AppearanceState.CustomValue) {
				this.visibility = visibility;
			}
		}
		public ViewItemVisibility? Visibility {
			get { return visibility; }
			set {
				State = value.HasValue ? AppearanceState.CustomValue : AppearanceState.ResetValue;
				visibility = value;
			}
		}
		public override bool IsCombineValue {
			get {
				return true;
			}
		}
		public override void CombineValue(IConditionalAppearanceItem item) {
			AppearanceItemVisibility _item = (AppearanceItemVisibility)item;
			if(_item.Visibility.HasValue) {
				if(!Visibility.HasValue ||
					_item.Visibility.Value == ViewItemVisibility.Hide ||
					(Visibility.HasValue && Visibility.Value == ViewItemVisibility.Show)) {
					Visibility = _item.Visibility.Value;
				}
			}
		}
		protected override void ApplyCore(object targetItem) {
			if(targetItem is IAppearanceVisibility) {
				if(State != AppearanceState.None) {
					if(State == AppearanceState.CustomValue) {
						((IAppearanceVisibility)targetItem).Visibility = Visibility.Value;
					}
					else {
						((IAppearanceVisibility)targetItem).ResetVisibility();
					}
				}
			}
		}
	}
	public class AppearanceItemFontColor : AppearanceItemBase {
		private Color? fontColor;
		public AppearanceItemFontColor(AppearanceState state, int priority, Color? fontColor)
			: base(state, priority) {
			if(state == AppearanceState.CustomValue) {
				this.fontColor = fontColor;
			}
		}
		public Color? FontColor {
			get { return fontColor; }
			set {
				State = value.HasValue ? AppearanceState.CustomValue : AppearanceState.ResetValue;
				fontColor = value;
			}
		}
		protected override void ApplyCore(object targetItem) {
			if(targetItem is IAppearanceFormat) {
				if(State != AppearanceState.None) {
					if(State == AppearanceState.CustomValue) {
						((IAppearanceFormat)targetItem).FontColor = FontColor.Value;
					}
					else {
						((IAppearanceFormat)targetItem).ResetFontColor();
					}
				}
			}
		}
	}
	public class AppearanceItemBackColor : AppearanceItemBase {
		private Color? backColor;
		public AppearanceItemBackColor(AppearanceState state, int priority, Color? backColor)
			: base(state, priority) {
			if(state == AppearanceState.CustomValue) {
				this.backColor = backColor;
			}
		}
		public Color? BackColor {
			get { return backColor; }
			set {
				State = value.HasValue ? AppearanceState.CustomValue : AppearanceState.ResetValue;
				backColor = value;
			}
		}
		protected override void ApplyCore(object targetItem) {
			if(targetItem is IAppearanceFormat) {
				if(State != AppearanceState.None) {
					if(State == AppearanceState.CustomValue && BackColor.HasValue) {
						((IAppearanceFormat)targetItem).BackColor = BackColor.Value;
					}
					else {
						((IAppearanceFormat)targetItem).ResetBackColor();
					}
				}
			}
		}
	}
	public class AppearanceObject {
		List<IConditionalAppearanceItem> items;
		public AppearanceObject()
			: this(new List<IConditionalAppearanceItem>()) {
		}
		public AppearanceObject(List<IConditionalAppearanceItem> items) {
			this.items = items;
		}
		public List<IConditionalAppearanceItem> Items {
			get {
				return items;
			}
		}
		public bool? Enabled {
			get {
				return GetAppearanceItemEnabled.Enabled;
			}
			set {
				GetAppearanceItemEnabled.Enabled = value;
			}
		}
		public ViewItemVisibility? Visibility {
			get {
				return GetAppearanceItemVisibility.Visibility;
			}
			set {
				GetAppearanceItemVisibility.Visibility = value;
			}
		}
		public FontStyle? FontStyle {
			get {
				return GetAppearanceItemFontStyle.FontStyle;
			}
			set {
				GetAppearanceItemFontStyle.FontStyle = value;
			}
		}
		public Color? FontColor {
			get {
				return GetAppearanceItemFontColor.FontColor;
			}
			set {
				GetAppearanceItemFontColor.FontColor = value;
			}
		}
		public Color? BackColor {
			get {
				return GetBackColorAppearanceItem.BackColor;
			}
			set {
				GetBackColorAppearanceItem.BackColor = value;
			}
		}
		private AppearanceItemEnabled GetAppearanceItemEnabled {
			get {
				AppearanceItemEnabled item = GetItemByType<AppearanceItemEnabled>();
				if(item == null) {
					item = new AppearanceItemEnabled(AppearanceState.None, 0, null);
					items.Add(item);
				}
				return item;
			}
		}
		private AppearanceItemFontStyle GetAppearanceItemFontStyle {
			get {
				AppearanceItemFontStyle item = GetItemByType<AppearanceItemFontStyle>();
				if(item == null) {
					item = new AppearanceItemFontStyle(AppearanceState.None, 0, null);
					items.Add(item);
				}
				return item;
			}
		}
		private AppearanceItemFontColor GetAppearanceItemFontColor {
			get {
				AppearanceItemFontColor item = GetItemByType<AppearanceItemFontColor>();
				if(item == null) {
					item = new AppearanceItemFontColor(AppearanceState.None, 0, null);
					items.Add(item);
				}
				return item;
			}
		}
		private AppearanceItemVisibility GetAppearanceItemVisibility {
			get {
				AppearanceItemVisibility item = GetItemByType<AppearanceItemVisibility>();
				if(item == null) {
					item = new AppearanceItemVisibility(AppearanceState.None, 0, null);
					items.Add(item);
				}
				return item;
			}
		}
		private AppearanceItemBackColor GetBackColorAppearanceItem {
			get {
				AppearanceItemBackColor item = GetItemByType<AppearanceItemBackColor>();
				if(item == null) {
					item = new AppearanceItemBackColor(AppearanceState.None, 0, null);
					items.Add(item);
				}
				return item;
			}
		}
		private T GetItemByType<T>() where T : IConditionalAppearanceItem {
			foreach(IConditionalAppearanceItem item in items) {
				if(typeof(T).IsAssignableFrom(item.GetType())) {
					return (T)item;
				}
			}
			return default(T);
		}
	}
}
