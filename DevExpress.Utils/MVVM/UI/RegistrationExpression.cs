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

namespace DevExpress.Utils.MVVM {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Windows.Forms;
	using DevExpress.Utils.MVVM.Services;
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IRegistrationSourceProvider {
		object Source { get; }
		event EventHandler SourceChanged;
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface ICommandMethodsProvider {
		MethodInfo[] GetCommandMethods();
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class RegistrationExpression : ICloneable {
		[Browsable(false)]
		public string Name {
			get { return GetName(); }
		}
		[Browsable(false)]
		public string TypeName {
			get { return GetTypeName(); }
		}
		protected virtual string GetName() {
			return GetType().Name.Replace("RegistrationExpression", string.Empty);
		}
		protected virtual string GetTypeName() { return null; }
		protected internal abstract void Register(object source);
		protected internal abstract void Unregister();
		protected internal IRegistrationSourceProvider Provider;
		object ICloneable.Clone() {
			return Clone();
		}
		protected abstract RegistrationExpression Clone();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public abstract object[] GetSerializerParameters();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static DispatcherServiceRegistrationExpression RegisterDispatcherService(string key, bool defaultService) {
			return new DispatcherServiceRegistrationExpression(key, defaultService);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static MessageBoxServiceRegistrationExpression RegisterMessageBoxService(string key, bool defaultService,
			DefaultMessageBoxServiceType type = DefaultMessageBoxServiceType.Default) {
			return new MessageBoxServiceRegistrationExpression(key, defaultService) { ServiceType = type };
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static DialogServiceRegistrationExpression RegisterDialogService(string key, bool defaultService,
			IWin32Window owner = null, DefaultDialogServiceType type = DefaultDialogServiceType.Default, string title = null) {
			return new DialogServiceRegistrationExpression(key, defaultService) { Owner = owner, ServiceType = type, Title = title };
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static DocumentManagerServiceRegistrationExpression RegisterDocumentManagerService(string key, bool defaultService, IDocumentAdapterFactory target) {
			return new DocumentManagerServiceRegistrationExpression(key, defaultService) { Target = target };
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static WindowedDocumentManagerServiceRegistrationExpression RegisterWindowedDocumentManagerService(string key, bool defaultService,
			IWin32Window owner = null, DefaultWindowedDocumentManagerServiceType type = DefaultWindowedDocumentManagerServiceType.Default, IWindowedDocumentAdapterFactory target = null) {
			return new WindowedDocumentManagerServiceRegistrationExpression(key, defaultService) { Owner = owner, ServiceType = type, Target = target };
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static ConfirmationBehaviorRegistrationExpression RegisterConfirmation(string eventName, IComponent target,
			string caption = null, string text = null, ConfirmationButtons buttons = ConfirmationButtons.YesNo, bool showQuestionIcon = true) {
			return new ConfirmationBehaviorRegistrationExpression() { EventName = eventName, Target = target, Caption = caption, Text = text, Buttons = buttons, ShowQuestionIcon = showQuestionIcon };
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static EventToCommandBehaviorRegistrationExpression RegisterEventToCommand(Type sourceType, string eventName, IComponent target, string commandName) {
			return new EventToCommandBehaviorRegistrationExpression(sourceType) { EventName = eventName, Target = target, CommandName = commandName };
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static EventToCommandBehaviorParameterizedRegistrationExpression RegisterEventToCommandParameterized(Type sourceType, string eventName, IComponent target, string commandName, string commandParameterName) {
			return new EventToCommandBehaviorParameterizedRegistrationExpression(sourceType) { EventName = eventName, Target = target, CommandName = commandName, CommandParameterName = commandParameterName };
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static NotificationServiceRegistrationExpression RegisterNotificationService(string key, bool defaultService, INotificationProvider target = null) {
			return new NotificationServiceRegistrationExpression(key, defaultService) { Target = target };
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static SplashScreenServiceRegistrationExpression RegisterSplashScreenService(string key, bool defaultService, ISplashScreenServiceProvider target = null) {
			return new SplashScreenServiceRegistrationExpression(key, defaultService) { Target = target };
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static LayoutSerializationServiceRegistrationExpression RegisterLayoutSerializationService(string key, bool defaultService, DefaultBoolean acceptNestedObject, IComponent target) {
			return new LayoutSerializationServiceRegistrationExpression(key, defaultService, acceptNestedObject, target);
		}
		public override string ToString() {
			string serviceName = GetName();
			string typeName = GetTypeName();
			if(string.IsNullOrEmpty(typeName))
				return serviceName;
			return serviceName + "(" + typeName + ")";
		}
		public static bool AreEqual(RegistrationExpression e1, RegistrationExpression e2) {
			if(object.ReferenceEquals(e1, e2)) return true;
			return (e1 != null && e2 != null) && (e1.Name == e2.Name);
		}
		protected string GetComponentName(object target) {
			IComponent component = target as IComponent;
			return (component != null && component.Site != null) ? component.Site.Name : null;
		}
		protected string GetNameFromProperty(object target) {
			Type targetType = target.GetType();
			var pName = targetType.GetProperty("Name");
			return (pName != null) ? pName.GetValue(target, null) as string : null;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class ServiceRegistrationExpression : RegistrationExpression {
		protected ServiceRegistrationExpression(string key, bool defaultService) {
			this.Key = key;
			this.DefaultService = defaultService;
		}
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Behavior), RefreshProperties(RefreshProperties.All)]
		public string Key { get; set; }
		[Browsable(false)]
		public bool DefaultService { get; private set; }
		protected internal override void Register(object source) {
			if(DefaultService) {
				if(string.IsNullOrEmpty(Key))
					((MVVMContext)Provider).RegisterDefaultService(CreateService());
				else
					((MVVMContext)Provider).RegisterDefaultService(Key, CreateService());
			}
			else {
				if(string.IsNullOrEmpty(Key))
					((MVVMContext)Provider).RegisterService(CreateService());
				else
					((MVVMContext)Provider).RegisterService(Key, CreateService());
			}
		}
		protected abstract object CreateService();
		protected internal override void Unregister() { }
		protected override string GetName() {
			string serviceName = base.GetName();
			if(DefaultService)
				serviceName = "Default" + serviceName;
			if(!string.IsNullOrEmpty(Key))
				serviceName = serviceName + "[" + Key + "]";
			return serviceName;
		}
	}
	[TypeConverter("DevExpress.Utils.MVVM.Design.DispatcherServiceRegistrationExpressionConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
	public class DispatcherServiceRegistrationExpression : ServiceRegistrationExpression {
		internal DispatcherServiceRegistrationExpression(string key, bool defaultService)
			: base(key, defaultService) {
		}
		protected override object CreateService() {
			return DispatcherService.Create();
		}
		public override object[] GetSerializerParameters() {
			return new object[] { Key, DefaultService };
		}
		protected override RegistrationExpression Clone() {
			return new DispatcherServiceRegistrationExpression(Key, DefaultService);
		}
	}
	[TypeConverter("DevExpress.Utils.MVVM.Design.MessageBoxServiceRegistrationExpressionConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
	public class MessageBoxServiceRegistrationExpression : ServiceRegistrationExpression {
		internal MessageBoxServiceRegistrationExpression(string key, bool defaultService)
			: base(key, defaultService) {
		}
		[DefaultValue(DefaultMessageBoxServiceType.Default), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public DefaultMessageBoxServiceType ServiceType { get; set; }
		protected override object CreateService() {
			return MessageBoxService.Create(ServiceType);
		}
		public override object[] GetSerializerParameters() {
			return new object[] { Key, DefaultService, ServiceType };
		}
		protected override RegistrationExpression Clone() {
			return new MessageBoxServiceRegistrationExpression(Key, DefaultService);
		}
		protected override string GetTypeName() {
			return (ServiceType != DefaultMessageBoxServiceType.Default) ? ServiceType.ToString() : null;
		}
	}
	[TypeConverter("DevExpress.Utils.MVVM.Design.DialogServiceRegistrationExpressionConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
	public class DialogServiceRegistrationExpression : ServiceRegistrationExpression {
		internal DialogServiceRegistrationExpression(string key, bool defaultService)
			: base(key, defaultService) {
		}
		[DefaultValue(DefaultDialogServiceType.Default), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public DefaultDialogServiceType ServiceType { get; set; }
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public IWin32Window Owner { get; set; }
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Appearance)]
		public string Title { get; set; }
		protected override object CreateService() {
			return DialogService.Create(Owner, ServiceType, Title);
		}
		public override object[] GetSerializerParameters() {
			return new object[] { Key, DefaultService, Owner, ServiceType, Title };
		}
		protected override RegistrationExpression Clone() {
			return new DialogServiceRegistrationExpression(Key, DefaultService);
		}
		protected override string GetTypeName() {
			return (ServiceType != DefaultDialogServiceType.Default) ? ServiceType.ToString() : null;
		}
	}
	[TypeConverter("DevExpress.Utils.MVVM.Design.DocumentManagerServiceRegistrationExpressionConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
	public class DocumentManagerServiceRegistrationExpression : ServiceRegistrationExpression {
		internal DocumentManagerServiceRegistrationExpression(string key, bool defaultService)
			: base(key, defaultService) {
		}
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public IDocumentAdapterFactory Target { get; set; }
		protected override object CreateService() {
			return DocumentManagerService.Create(Target);
		}
		public override object[] GetSerializerParameters() {
			return new object[] { Key, DefaultService, Target };
		}
		protected override RegistrationExpression Clone() {
			return new DocumentManagerServiceRegistrationExpression(Key, DefaultService);
		}
		protected override string GetTypeName() {
			if(Target == null) return null;
			return (GetComponentName(Target) ?? GetNameFromProperty(Target));
		}
	}
	[TypeConverter("DevExpress.Utils.MVVM.Design.WindowedDocumentManagerServiceRegistrationExpressionConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
	public class WindowedDocumentManagerServiceRegistrationExpression : ServiceRegistrationExpression {
		internal WindowedDocumentManagerServiceRegistrationExpression(string key, bool defaultService)
			: base(key, defaultService) {
		}
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public IWindowedDocumentAdapterFactory Target { get; set; }
		[DefaultValue(DefaultWindowedDocumentManagerServiceType.Default), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public DefaultWindowedDocumentManagerServiceType ServiceType { get; set; }
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public IWin32Window Owner { get; set; }
		protected override object CreateService() {
			if(Target != null)
				return WindowedDocumentManagerService.Create(Target, Owner);
			else
				return WindowedDocumentManagerService.Create(ServiceType, Owner);
		}
		public override object[] GetSerializerParameters() {
			return new object[] { Key, DefaultService, Owner, ServiceType, Target };
		}
		protected override RegistrationExpression Clone() {
			return new WindowedDocumentManagerServiceRegistrationExpression(Key, DefaultService);
		}
		protected override string GetTypeName() {
			if(Target != null)
				return (GetComponentName(Target) ?? GetNameFromProperty(Target));
			return (ServiceType != DefaultWindowedDocumentManagerServiceType.Default) ?
				ServiceType.ToString() : null;
		}
	}
	[TypeConverter("DevExpress.Utils.MVVM.Design.NotificationServiceRegistrationExpressionConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
	public class NotificationServiceRegistrationExpression : ServiceRegistrationExpression {
		internal NotificationServiceRegistrationExpression(string key, bool defaultService)
			: base(key, defaultService) {
		}
		protected override object CreateService() {
			return NotificationService.Create(Target);
		}
		public override object[] GetSerializerParameters() {
			return new object[] { Key, DefaultService, Target };
		}
		protected override RegistrationExpression Clone() {
			return new NotificationServiceRegistrationExpression(Key, DefaultService);
		}
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public INotificationProvider Target { get; set; }
	}
	[TypeConverter("DevExpress.Utils.MVVM.Design.SplashScreenServiceRegistrationExpressionConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
	public class SplashScreenServiceRegistrationExpression : ServiceRegistrationExpression {
		internal SplashScreenServiceRegistrationExpression(string key, bool defaultService)
			: base(key, defaultService) {
		}
		protected override object CreateService() {
			return SplashScreenService.Create(Target);
		}
		public override object[] GetSerializerParameters() {
			return new object[] { Key, DefaultService, Target };
		}
		protected override RegistrationExpression Clone() {
			return new SplashScreenServiceRegistrationExpression(Key, DefaultService);
		}
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public ISplashScreenServiceProvider Target { get; set; }
	}
	[TypeConverter("DevExpress.Utils.MVVM.Design.LayoutSerializationServiceRegistrationExpressionConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
	public class LayoutSerializationServiceRegistrationExpression : ServiceRegistrationExpression {
		internal LayoutSerializationServiceRegistrationExpression(string key, bool defaultService, DefaultBoolean acceptNestedObjects, IComponent target)
			: base(key, defaultService) {
				this.AcceptNestedObjects = acceptNestedObjects;
				this.Target = target;
		}
		protected override object CreateService() {
			return LayoutSerializationService.Create(Target, AcceptNestedObjects != DefaultBoolean.False);
		}
		public override object[] GetSerializerParameters() {
			return new object[] { Key, DefaultService, AcceptNestedObjects, Target };
		}
		protected override RegistrationExpression Clone() {
			return new LayoutSerializationServiceRegistrationExpression(Key, DefaultService, this.AcceptNestedObjects, this.Target);
		}
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public IComponent Target { get; set; }
		[DefaultValue(DefaultBoolean.Default), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		public DefaultBoolean AcceptNestedObjects { get; set; }
	}
	public abstract class BehaviorRegistrationExpression : RegistrationExpression {
		Type baseEventArgsType;
		protected BehaviorRegistrationExpression(Type argsType) {
			this.baseEventArgsType = argsType;
		}
		[Browsable(false)]
		public Type EventArgsType {
			get { return baseEventArgsType; }
		}
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Behavior)]
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public IComponent Target { get; set; }
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Behavior), RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		[TypeConverter("DevExpress.Utils.MVVM.Design.EventNameConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
		public string EventName { get; set; }
		IDisposable behaviorCore;
		protected internal override void Register(object source) {
			Ref.Dispose(ref behaviorCore);
			if(Target != null && !string.IsNullOrEmpty(EventName)) {
				var behaviorType = GetBehaviorType();
				if(behaviorType != null) {
					var attach = InterfacesProxy.GetAttachBehaviorMethod(behaviorType);
					behaviorCore = attach((MVVMContext)Provider, Target, GetBehaviorSettings(), GetCreateParams());
				}
			}
		}
		protected internal override void Unregister() {
			Ref.Dispose(ref behaviorCore);
		}
		protected abstract Type GetBehaviorType();
		protected virtual object GetBehaviorSettings() {
			return null;
		}
		protected virtual object[] GetCreateParams() {
			return new object[] { EventName };
		}
		protected override string GetTypeName() {
			if(Target == null) return null;
			string targetName = (GetComponentName(Target) ?? GetNameFromProperty(Target));
			return (!string.IsNullOrEmpty(EventName)) ? string.Format("{0}.{1}", targetName, EventName) : targetName;
		}
	}
	[TypeConverter("DevExpress.Utils.MVVM.Design.ConfirmationBehaviorRegistrationExpressionConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
	public class ConfirmationBehaviorRegistrationExpression : BehaviorRegistrationExpression {
		internal ConfirmationBehaviorRegistrationExpression()
			: base(typeof(CancelEventArgs)) {
		}
		protected override Type GetBehaviorType() {
			return typeof(ConfirmationBehavior<CancelEventArgs>);
		}
		protected override object GetBehaviorSettings() {
			return new Action<ConfirmationBehavior<CancelEventArgs>>(behavior =>
			{
				UpdateIfSet(() => Caption, caption => behavior.Caption = caption);
				UpdateIfSet(() => Text, text => behavior.Text = text);
				UpdateIfSet(() => Buttons, buttons => behavior.Buttons = buttons);
				UpdateIfSet(() => ShowQuestionIcon, show => behavior.ShowQuestionIcon = show);
			});
		}
		#region Properties
		IDictionary<string, object> propertyBag = new Dictionary<string, object>();
		T GetCore<T>(string propertyName, T defaultValue) {
			object valueObj;
			if(propertyBag.TryGetValue(propertyName, out valueObj))
				return (T)valueObj;
			return defaultValue;
		}
		protected T Get<T>(Expression<Func<T>> e, T defaultValue = default(T)) {
			return GetCore<T>(ExpressionHelper.GetPropertyName((LambdaExpression)e), defaultValue);
		}
		protected void Set<T>(Expression<Func<T>> e, T value, T defaultValue = default(T)) {
			string propertyName = ExpressionHelper.GetPropertyName((LambdaExpression)e);
			if(!object.Equals(GetCore<T>(propertyName, defaultValue), value))
				propertyBag[propertyName] = value;
		}
		protected void UpdateIfSet<T>(Expression<Func<T>> e, Action<T> update) {
			object valueObj;
			if(propertyBag.TryGetValue(ExpressionHelper.GetPropertyName((LambdaExpression)e), out valueObj))
				update((T)valueObj);
		}
		#endregion Properties
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Appearance), Localizable(true)]
		public string Caption {
			get { return Get(() => Caption); }
			set { Set(() => Caption, value); }
		}
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Appearance), Localizable(true)]
		public string Text {
			get { return Get(() => Text); }
			set { Set(() => Text, value); }
		}
		[DefaultValue(ConfirmationButtons.YesNo), Category(DevExpress.XtraEditors.CategoryName.Appearance)]
		public ConfirmationButtons Buttons {
			get { return Get(() => Buttons, ConfirmationButtons.YesNo); }
			set { Set(() => Buttons, value, ConfirmationButtons.YesNo); }
		}
		[DefaultValue(true), Category(DevExpress.XtraEditors.CategoryName.Appearance)]
		public bool ShowQuestionIcon {
			get { return Get(() => ShowQuestionIcon, true); }
			set { Set(() => ShowQuestionIcon, value, true); }
		}
		public override object[] GetSerializerParameters() {
			return new object[] { EventName, Target, Caption, Text, Buttons, ShowQuestionIcon };
		}
		protected override RegistrationExpression Clone() {
			return new ConfirmationBehaviorRegistrationExpression();
		}
		protected override string GetTypeName() {
			string typeName = base.GetTypeName();
			if(string.IsNullOrEmpty(typeName)) return null;
			return string.Format("{0}[{1}]", typeName, Buttons.ToString());
		}
	}
	[TypeConverter("DevExpress.Utils.MVVM.Design.EventToCommandBehaviorRegistrationExpressionConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
	public class EventToCommandBehaviorRegistrationExpression : BehaviorRegistrationExpression, ICommandMethodsProvider {
		Type sourceTypeCore;
		internal EventToCommandBehaviorRegistrationExpression(Type sourceType)
			: base(typeof(EventArgs)) {
			this.sourceTypeCore = sourceType;
		}
		protected Type SourceType {
			get { return sourceTypeCore; }
		}
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Behavior), RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		[TypeConverter("DevExpress.Utils.MVVM.Design.CommandNameConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
		public string CommandName { get; set; }
		protected override Type GetBehaviorType() {
			if(SourceType == null)
				return null;
			return typeof(EventToCommandBehavior<,>).MakeGenericType(new Type[] { SourceType, typeof(EventArgs) });
		}
		protected override object[] GetCreateParams() {
			var selectorExpression = GetCommandSelectorExpression(SourceType, CommandName);
			return new object[] { EventName, selectorExpression };
		}
		protected override RegistrationExpression Clone() {
			return new EventToCommandBehaviorRegistrationExpression(SourceType);
		}
		public override object[] GetSerializerParameters() {
			return new object[] { SourceType, EventName, Target, CommandName };
		}
		MethodInfo[] ICommandMethodsProvider.GetCommandMethods() {
			if(SourceType == null) return new MethodInfo[0];
			return GetCommandMethodsCore();
		}
		protected virtual MethodInfo[] GetCommandMethodsCore() {
			return MemberInfoHelper.GetCommandMethods(MVVMTypesResolver.Instance, SourceType, 0);
		}
		protected static Expression GetCommandSelectorExpression(Type sourceType, string methodName) {
			var mInfo = sourceType.GetMethod(methodName);
			var mInfoParameters = mInfo.GetParameters();
			var instance = Expression.Parameter(sourceType, "x");
			Expression[] arguments = new Expression[mInfoParameters.Length];
			for(int i = 0; i < arguments.Length; i++)
				arguments[i] = Expression.Default(mInfoParameters[i].ParameterType);
			return Expression.Lambda(Expression.Call(instance, mInfo, arguments), instance);
		}
		protected static Expression GetCommandParameterSelectorExpression(Type sourceType, string parameterName) {
			var pInfo = sourceType.GetProperty(parameterName);
			var instance = Expression.Parameter(sourceType, "x");
			return Expression.Lambda(Expression.MakeMemberAccess(instance, pInfo), instance);
		}
		protected override string GetTypeName() {
			string targetName = base.GetTypeName();
			return !string.IsNullOrEmpty(CommandName) ? string.Format("{0} => {1}", targetName, CommandName) : targetName;
		}
	}
	[TypeConverter("DevExpress.Utils.MVVM.Design.EventToCommandBehaviorParameterizedRegistrationExpressionConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
	public class EventToCommandBehaviorParameterizedRegistrationExpression : EventToCommandBehaviorRegistrationExpression {
		internal EventToCommandBehaviorParameterizedRegistrationExpression(Type sourceType)
			: base(sourceType) {
		}
		[DefaultValue(null), Category(DevExpress.XtraEditors.CategoryName.Behavior), RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		[TypeConverter("DevExpress.Utils.MVVM.Design.CommandParameterNameConverter, " + AssemblyInfo.SRAssemblyDesignFull)]
		public string CommandParameterName { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Type GetCommandParameterType() {
			if(string.IsNullOrEmpty(CommandName))
				return null;
			var mInfo = SourceType.GetMethod(CommandName);
			if(mInfo == null)
				return null;
			var parameters = mInfo.GetParameters();
			if(parameters.Length == 0)
				return null;
			return parameters[0].ParameterType;
		}
		protected override Type GetBehaviorType() {
			if(SourceType == null)
				return null;
			Type commandParameterType = GetCommandParameterType();
			if(commandParameterType == null)
				return null;
			return typeof(EventToCommandBehavior<,,>).MakeGenericType(new Type[] { SourceType, commandParameterType, typeof(EventArgs) });
		}
		protected override object[] GetCreateParams() {
			var selectorExpression = GetCommandSelectorExpression(SourceType, CommandName);
			var commandSelectorExpression = GetCommandParameterSelectorExpression(SourceType, CommandParameterName);
			return new object[] { EventName, selectorExpression, commandSelectorExpression };
		}
		protected override MethodInfo[] GetCommandMethodsCore() {
			return MemberInfoHelper.GetCommandMethods(MVVMTypesResolver.Instance, SourceType, 1);
		}
		protected override RegistrationExpression Clone() {
			return new EventToCommandBehaviorParameterizedRegistrationExpression(SourceType);
		}
		public override object[] GetSerializerParameters() {
			return new object[] { SourceType, EventName, Target, CommandName, CommandParameterName };
		}
	}
	[Editor("DevExpress.XtraEditors.MVVM.Design.RegistrationExpressionsEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class RegistrationExpressionCollection : CollectionBase, IEnumerable<RegistrationExpression>, IDisposable {
		IRegistrationSourceProvider provider;
		public RegistrationExpressionCollection(IRegistrationSourceProvider provider) {
			this.provider = provider;
			if(provider != null)
				provider.SourceChanged += provider_SourceChanged;
		}
		void IDisposable.Dispose() {
			if(provider != null)
				provider.SourceChanged -= provider_SourceChanged;
			RemoveRegistrations();
		}
		public void CreateRegistrations() {
			if(provider != null) {
				foreach(RegistrationExpression expression in InnerList)
					expression.Register(provider.Source);
			}
		}
		public void UpdateRegistrations() {
			foreach(RegistrationExpression expression in InnerList) {
				expression.Unregister();
				if(provider != null)
					expression.Register(provider.Source);
			}
		}
		public void RemoveRegistrations() {
			foreach(RegistrationExpression expression in InnerList)
				expression.Unregister();
		}
		void provider_SourceChanged(object sender, EventArgs e) {
			UpdateRegistrations();
		}
		public void Add(RegistrationExpression expression) {
			((IList)this).Add(expression);
		}
		public void Remove(RegistrationExpression expression) {
			((IList)this).Remove(expression);
		}
		public void AddRange(RegistrationExpression[] expressions) {
			for(int i = 0; i < expressions.Length; i++)
				((IList)this).Add(expressions[i]);
		}
		public int IndexOf(RegistrationExpression expression) {
			return InnerList.IndexOf(expression);
		}
		public bool Contains(RegistrationExpression expression) {
			return InnerList.Contains(expression);
		}
		protected override void OnInsert(int index, object value) {
			RegistrationExpression expression = value as RegistrationExpression;
			if(expression != null) {
				expression.Provider = provider;
				if(provider != null && provider.Source != null)
					expression.Register(provider.Source);
			}
		}
		protected override void OnRemove(int index, object value) {
			RegistrationExpression expression = value as RegistrationExpression;
			if(expression != null) {
				expression.Unregister();
				expression.Provider = null;
			}
		}
		protected override void OnClear() {
			RemoveRegistrations();
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			RaiseCollectionChanged(CollectionChangeAction.Remove, oldValue as RegistrationExpression);
			RaiseCollectionChanged(CollectionChangeAction.Add, newValue as RegistrationExpression);
		}
		protected override void OnInsertComplete(int index, object value) {
			RaiseCollectionChanged(CollectionChangeAction.Remove, value as RegistrationExpression);
		}
		protected override void OnRemoveComplete(int index, object value) {
			RaiseCollectionChanged(CollectionChangeAction.Remove, value as RegistrationExpression);
		}
		protected override void OnClearComplete() {
			RaiseCollectionChanged(CollectionChangeAction.Refresh, null);
		}
		public RegistrationExpression this[int index] {
			get { return (RegistrationExpression)InnerList[index]; }
		}
		public RegistrationExpression[] ToArray() {
			RegistrationExpression[] array = new RegistrationExpression[Count];
			InnerList.CopyTo(array, 0);
			return array;
		}
		public event CollectionChangeEventHandler CollectionChanged;
		protected void RaiseCollectionChanged(CollectionChangeAction action, RegistrationExpression expression) {
			var handler = CollectionChanged;
			if(handler != null) handler(this, new CollectionChangeEventArgs(action, expression));
		}
		#region IEnumerable<RegistrationExpression> Members
		IEnumerator<RegistrationExpression> IEnumerable<RegistrationExpression>.GetEnumerator() {
			foreach(RegistrationExpression expression in InnerList)
				yield return expression;
		}
		#endregion
	}
}
