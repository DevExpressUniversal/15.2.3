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
using System.Drawing.Design;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Exceptions;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.UI {
	[ToolboxItem(false)]
	[DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraReports.Design.Designers.CalculatedFieldDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull)]
	[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.CalculatedField")]
	public class CalculatedField : DataContainerComponent, IScriptable, IDisplayNamePropertyContainer, ICalculatedField {
		#region Inner Classes
		internal static class EventNames {
			public const string GetValue = "GetValue";
		}
		#endregion
		#region Fields & Properties
		static readonly object GetValueEvent = new object();
		CalculatedFieldCollection owner;
		CalculatedFieldScripts scripts;
		FieldType fieldType;
		string name = string.Empty;
		string displayName = string.Empty;
		string expression = string.Empty;
		[DefaultValue("")]
		[Browsable(false)]
		[XtraSerializableProperty]
		public string Name {
			get { return Site != null ? Site.Name : name; }
			set { name = value; }
		}
		[SRCategory(ReportStringId.CatBehavior)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraReportsLocalizedDescription("CalculatedFieldScripts")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.CalculatedField.Scripts")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public CalculatedFieldScripts Scripts {
			get { return scripts ?? (scripts = new CalculatedFieldScripts(this)); }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("CalculatedFieldFieldType")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.CalculatedField.FieldType")]
		[SRCategory(ReportStringId.CatData)]
		[DefaultValue(FieldType.None)]
		[XtraSerializableProperty]
		public FieldType FieldType {
			get { return fieldType; }
			set { fieldType = value; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("CalculatedFieldDisplayName")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.CalculatedField.DisplayName")]
		[SRCategory(ReportStringId.CatData)]
		[Localizable(true)]
		[XtraSerializableProperty]
		public string DisplayName {
			get { return !string.IsNullOrEmpty(displayName) ? displayName : Name; }
			set {
				if(value == displayName)
					return;
				if(DesignMode)
					ValidateDisplayNameUniqueness(value);
				displayName = value;
			}
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("CalculatedFieldExpression")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.CalculatedField.Expression")]
		[SRCategory(ReportStringId.CatData)]
		[DefaultValue("")]
		[Editor("DevExpress.XtraReports.Design.CalculatedFieldExpressionEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor))]
		[XtraSerializableProperty]
		[TypeConverter(typeof(DevExpress.XtraReports.Design.TextPropertyTypeConverter))]
		public string Expression {
			get { return expression; }
			set {
				if(expression != value) {
					if(DesignMode)
						ValidateExpression(value);
					expression = value;
				}
			}
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("CalculatedFieldDataSource")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.CalculatedField.DataSource")]
		[RefreshProperties(RefreshProperties.Repaint)]
		public override object DataSource {
			get { return base.DataSource; }
			set { base.DataSource = value; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("CalculatedFieldDataMember")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.CalculatedField.DataMember")]
		public override string DataMember {
			get { return base.DataMember; }
			set { base.DataMember = value ?? String.Empty; }
		}
		[Browsable(false)]
		public bool IsDisposed {
			get;
			private set;
		}
		internal CalculatedFieldCollection Owner {
			get { return owner; }
			set { owner = value; }
		}
		internal XtraReport Report {
			get { return owner != null ? owner.Report : null; }
		}
		protected override IDataContainer Parent {
			get { return Report; }
		}
		#endregion
		#region Constructors
		public CalculatedField() {
		}
		public CalculatedField(object dataSource, string dataMember) {
			Assign(dataSource, dataMember);
		}
		#endregion
		#region Events
		public event GetValueEventHandler GetValue {
			add { Events.AddHandler(GetValueEvent, value); }
			remove { Events.RemoveHandler(GetValueEvent, value); }
		}
		#endregion
		#region Methods
		public override bool Equals(object obj) {
			CalculatedField other = obj as CalculatedField;
			if(!object.ReferenceEquals(other, null)) {
				return this.Name == other.Name &&
					this.expression == other.expression &&
					this.displayName == other.displayName &&
					this.fieldType == other.fieldType &&
					EqualsCore(other);
			}
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		protected override void Dispose(bool disposing) {
			if(!IsDisposed) {
				if(disposing) {
					if(owner != null)
						owner.Remove(this);
					if(scripts != null)
						scripts.Dispose();
				}
				IsDisposed = true;
			}
			base.Dispose(disposing);
		}
		internal void ValidateExpression() {
			try {
				ValidateExpression(expression);
			} catch(CriteriaParserException) {
				throw new Exception(string.Format("Parameter {0} contains invalid expression.", Name));
			}
		}
		internal Hashtable GetEventHandlers() {
			Hashtable handlers = new Hashtable();
			Type type = GetType();
			EventInfo[] events = type.GetEvents();
			foreach(EventInfo eventInfo in events) {
				FieldInfo fieldInfo = eventInfo.DeclaringType.GetField(eventInfo.Name + "Event", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic);
				if(fieldInfo != null) {
					Delegate handler = Events[fieldInfo.GetValue(this)];
					if(handler != null)
						handlers[eventInfo] = handler;
				}
			}
			return handlers;
		}
		internal void OnGetValue(GetValueEventArgs e) {
			RunEventScript(GetValueEvent, EventNames.GetValue, e);
			GetValueEventHandler handler = (GetValueEventHandler)Events[GetValueEvent];
			if(handler != null) handler(this, e);
		}
		internal void InitializeScripts() {
			Report.EventsScriptManager.AddVariable(Name, this);
			Scripts.Initialize();
		}
		static void ValidateExpression(string value) {
			try {
				CriteriaOperator criteria = CriteriaOperator.Parse(value, null);
				FunctionNameValidator.Validate(criteria);
			} catch(CriteriaParserException exception) {
				throw (new CriteriaParserException(ConditionBooleanTypeValidator.GetExceptionString(exception), exception.Line, exception.Column));
			}
		}
		bool RunEventScript<E>(object eventId, string eventName, E e) where E : EventArgs {
			return !DesignMode && Report != null && Report.EventsScriptManager.RunEventScript<E>(eventId, eventName, this, e);
		}
		#endregion
		#region Serialization
		bool ShouldSerializeDataSource() {
			return ShouldSerializeDataSourceCore();
		}
		bool ShouldSerializeXmlDataSource() {
			return ShouldSerializeXmlDataSourceCore();
		}
		bool ShouldSerializeScripts() {
			return scripts != null && !scripts.IsDefault();
		}
		bool ShouldSerializeDisplayName() {
			return !string.IsNullOrEmpty(displayName);
		}
		#endregion
		#region ICalculatedField
		object ICalculatedField.DataSource {
			get { return GetEffectiveDataSource(); }
		}
		#endregion
		#region IScriptable Members
		XRScriptsBase IScriptable.Scripts {
			get { return Scripts; }
		}
		#endregion
		#region IDisplayNamePropertyContainer Members
		string IDisplayNamePropertyContainer.GetDisplayPropertyValue(string source) {
			return CriteriaFieldNameConverter.ToDisplayNames(Report, GetEffectiveDataSource(), DataMember, source);
		}
		string IDisplayNamePropertyContainer.GetRealPropertyValue(string source) {
			return CriteriaFieldNameConverter.ToRealNames(Report, GetEffectiveDataSource(), DataMember, source);
		}
		string IDisplayNamePropertyContainer.GetDisplayName(string source) {
			return CriteriaFieldNameConverter.ToDisplayName(Report, GetEffectiveDataSource(), DataMember, source);
		}
		#endregion
		#region DisplayName Uniqueness Validation
		internal void ValidateNameUniqueness(string name) {
			if(!string.IsNullOrEmpty(displayName))
				return;
			ValidateDisplayNameUniquenessCore(name, "An incorrect Name value. Name cannot be identical to the display name of any item in the Field List.");
			ValidateUniquenessCore(name, pd => pd.Name, "An incorrect Name value. Name cannot be identical to the name of any item in the Field List.");
		}
		void ValidateDisplayNameUniqueness(string displayName) {
			if(!string.IsNullOrEmpty(displayName))
				ValidateDisplayNameUniquenessCore(displayName, "An incorrect DisplayName value. DisplayName cannot be identical to the name of any item in the Field List.");
			else
				ValidateDisplayNameUniquenessCore(Name, "An incorrect Name value. When the DisplayName is empty, it is taken from the Name property value. In this case, the Name property value cannot be identical to the name of any other item in the Field List.");
		}
		void ValidateDisplayNameUniquenessCore(string value, string message) {
			ValidateUniquenessCore(value, pd => pd.DisplayName, message);
		}
		void ValidateUniquenessCore(string value, Func<PropertyDescriptor, string> nameFunc, string message) {
			if(Report == null)
				return;
			using(DataContextProvider provider = new DataContextProvider(Report)) {
				PropertyDescriptorCollection pdCollection = provider.DataContext.GetItemProperties(GetEffectiveDataSource(), GetEffectiveDataMember());
				foreach(PropertyDescriptor pd in pdCollection) {
					if(string.Equals(nameFunc(pd), value, StringComparison.CurrentCulture))
						throw new ArgumentException(message);
				}
			}
		}
		#endregion
	}
}
