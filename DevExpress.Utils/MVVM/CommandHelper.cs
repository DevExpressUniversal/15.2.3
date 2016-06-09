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
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq.Expressions;
	using System.Reflection;
	public interface ISupportCommandBinding {
		IDisposable BindCommand(object command, Func<object> queryCommandParameter = null);
		IDisposable BindCommand<T>(Expression<Action<T>> commandSelector, object source, Func<T> queryCommandParameter = null);
		IDisposable BindCommand(Expression<Action> commandSelector, object source, Func<object> queryCommandParameter = null);
	}
	public static class CommandHelper {
		public static IDisposable Bind<T>(T target, Action<T, Action> subscribe, Action<T, Func<bool>> updateState,
			Expression<Action> commandSelector, object source, Func<object> queryCommandParameter = null) {
			Type commandType;
			string path;
			object command = GetCommand(commandSelector, source, out commandType, out path);
			return new SourceChangeTracker(path, source, MemberInfoHelper.GetMethod(commandSelector),
				BindCore<T>(target, subscribe, updateState, command, commandType, queryCommandParameter));
		}
		public static IDisposable Bind<T, TParameter>(T target, Action<T, Action> subscribe, Action<T, Func<bool>> updateState,
			Expression<Action<TParameter>> commandSelector, object source, Func<object> queryCommandParameter = null) {
			Type commandType;
			string path;
			object command = GetCommand(commandSelector, source, out commandType, out path);
			return new SourceChangeTracker(path, source, MemberInfoHelper.GetMethod(commandSelector),
				BindCore<T>(target, subscribe, updateState, command, commandType, queryCommandParameter));
		}
		public static IDisposable Bind<T>(T target, Action<T, Action> subscribe, Action<T, Func<bool>> updateState,
			object command, Func<object> queryCommandParameter = null) {
			return BindCore<T>(target, subscribe, updateState, command, command.GetType(), queryCommandParameter);
		}
		static IDisposable BindCore<T>(T target, Action<T, Action> subscribe, Action<T, Func<bool>> updateState, object command, Type commandType, Func<object> queryCommandParameter) {
			if(queryCommandParameter == null)
				queryCommandParameter = () => null;
			CommandExpressionBuilder builder = (commandType != null) ? GetCommandExpressionBuilder(commandType) : null;
			var eventInfo = EventInfoHelper.GetEventInfo<T>(subscribe);
			var commandHandler = new CommandHandler<T>(target, eventInfo, builder, command, queryCommandParameter, updateState);
			if(eventInfo == null && builder != null) 
				subscribe(target, builder.GetExecute(command, queryCommandParameter));
			return commandHandler;
		}
		static CommandExpressionBuilder GetCommandExpressionBuilder(Type commandType) {
			CommandExpressionBuilder builder;
			if(!cache.TryGetValue(commandType, out builder)) {
				builder = new CommandExpressionBuilder(commandType);
				cache.Add(commandType, builder);
			}
			return builder;
		}
		internal static PropertyInfo GetCommandProperty(Expression<Action> commandSelector, object source) {
			string methodPath;
			return GetCommandPropertyCore(MVVMTypesResolver.Instance, commandSelector, ref source, out methodPath);
		}
		internal static PropertyInfo GetCommandProperty<T>(Expression<Action<T>> commandSelector, object source) {
			string methodPath;
			return GetCommandPropertyCore<T>(MVVMTypesResolver.Instance, commandSelector, ref source, out methodPath);
		}
		static PropertyInfo GetCommandPropertyCore(IMVVMTypesResolver typesResolver, Expression<Action> commandSelector, ref object source, out string methodPath) {
			MethodCallExpression callExpression = GetMethodCallExpression(commandSelector, ref source, out methodPath);
			return (source != null) ? MemberInfoHelper.GetCommandProperty(source, typesResolver, callExpression.Method) : null;
		}
		static PropertyInfo GetCommandPropertyCore<T>(IMVVMTypesResolver typesResolver, Expression<Action<T>> commandSelector, ref object source, out string methodPath) {
			MethodCallExpression callExpression = GetMethodCallExpression(commandSelector, ref source, out methodPath);
			return (source != null) ? MemberInfoHelper.GetCommandProperty(source, typesResolver, callExpression.Method) : null;
		}
		static MethodCallExpression GetMethodCallExpression(LambdaExpression commandSelector, ref object source, out string methodPath) {
			methodPath = null;
			MethodCallExpression callExpression = commandSelector.Body as MethodCallExpression;
			var member = callExpression.Object as MemberExpression;
			if(member != null) {
				methodPath = ExpressionHelper.GetPath(member) + "." + callExpression.Method.Name;
				source = NestedPropertiesHelper.GetSource(methodPath + ".", source, source.GetType());
			}
			return callExpression;
		}
		#region CommandExpressionBuilder
		#region EventToCommand
		internal static object GetCancelCommandCore(IMVVMTypesResolver typesResover, object command) {
			return InterfacesProxy.GetCancelCommand(typesResover.GetAsyncCommandType(), command);
		}
		internal static object GetCommand(Expression<Action> commandSelector, object source, out Type commandType) {
			string memberPath;
			return GetCommand(commandSelector, source, out commandType, out memberPath);
		}
		internal static object GetCommand(Expression<Action> commandSelector, object source, out Type commandType, out string methodPath) {
			return GetCommandCore(MVVMTypesResolver.Instance, commandSelector, ref source, out commandType, out methodPath);
		}
		internal static object GetCommand<T>(Expression<Action<T>> commandSelector, object source, out Type commandType) {
			string memberPath;
			return GetCommand(commandSelector, source, out commandType, out memberPath);
		}
		internal static object GetCommand<T>(Expression<Action<T>> commandSelector, object source, out Type commandType, out string methodPath) {
			return GetCommandCore<T>(MVVMTypesResolver.Instance, commandSelector, ref source, out commandType, out methodPath);
		}
		internal static object GetCommandCore(IMVVMTypesResolver typesResolver, Expression<Action> commandSelector, ref object source, out Type commandType, out string methodPath) {
			var commandProperty = GetCommandPropertyCore(typesResolver, commandSelector, ref source, out methodPath);
			commandType = (commandProperty != null) ? commandProperty.PropertyType : null;
			return (commandProperty != null) ? commandProperty.GetValue(source, null) : null;
		}
		internal static object GetCommandCore<T>(IMVVMTypesResolver typesResolver, Expression<Action<T>> commandSelector, ref object source, out Type commandType, out string methodPath) {
			var commandProperty = GetCommandPropertyCore<T>(typesResolver, commandSelector, ref source, out methodPath);
			commandType = (commandProperty != null) ? commandProperty.PropertyType : null;
			return (commandProperty != null) ? commandProperty.GetValue(source, null) : null;
		}
		internal static Func<object> GetQueryCommandParameter<T, TValue>(Expression<Func<T, TValue>> parameterSelector, T source) {
			return ExpressionHelper.ReduceBoxAndCompile(parameterSelector, source);
		}
		internal static Func<bool> GetCanExecute(object command, Type commandType, Func<object> queryParameters) {
			return GetCommandExpressionBuilder(commandType).GetCanExecute(command, queryParameters);
		}
		internal static Action GetExecute(object command, Type commandType, Func<object> queryParameters) {
			return GetCommandExpressionBuilder(commandType).GetExecute(command, queryParameters);
		}
		#endregion
		static IDictionary<Type, CommandExpressionBuilder> cache = new Dictionary<Type, CommandExpressionBuilder>();
		sealed class CommandExpressionBuilder {
			internal readonly Type commandType;
			readonly Func<object, object, bool> canExecute = (c, p) => true;
			readonly Action<object, object> execute;
			readonly EventInfo canExecuteChangedEvent;
			ParameterExpression commandObject;
			ParameterExpression parameter;
			public CommandExpressionBuilder(Type commandType) {
				this.commandType = commandType;
				var executeMethod = MemberInfoHelper.GetMethodInfo(commandType, "Execute", MemberInfoHelper.SingleObjectParameterTypes);
				if(executeMethod == null)
					throw new NotSupportedException(commandType.ToString() + ": Missing Execute() method");
				commandObject = Expression.Parameter(typeof(object), "command");
				parameter = Expression.Parameter(typeof(object), "parameter");
				var command = Expression.TypeAs(commandObject, commandType);
				this.execute = Expression.Lambda<Action<object, object>>(
							Expression.Call(command, executeMethod, parameter),
							commandObject, parameter
						).Compile();
				var canExecuteMethod = MemberInfoHelper.GetMethodInfo(commandType, "CanExecute", MemberInfoHelper.SingleObjectParameterTypes);
				if(canExecuteMethod != null) {
					this.canExecute = Expression.Lambda<Func<object, object, bool>>(
							Expression.Call(command, canExecuteMethod, parameter),
							commandObject, parameter
						).Compile();
					this.canExecuteChangedEvent = MemberInfoHelper.GetEventInfo(commandType, "CanExecuteChanged");
				}
			}
			public Func<bool> GetCanExecute(object command, Func<object> queryParameter) {
				return () => canExecute(command, queryParameter());
			}
			public Action GetExecute(object command, Func<object> queryParameter) {
				return () => execute(command, queryParameter());
			}
			static IDictionary<HandlerKey, HandlerExpressionBuilder> handlersCache = new Dictionary<HandlerKey, HandlerExpressionBuilder>();
			public IDisposable SubscribeCommandState<T>(object command, T target, Func<bool> canExecute, Action<T, Func<bool>> updateState) {
				if(canExecuteChangedEvent == null) return null;
				HandlerKey key = new HandlerKey(typeof(T), canExecuteChangedEvent.DeclaringType);
				HandlerExpressionBuilder builder;
				if(!handlersCache.TryGetValue(key, out builder)) {
					builder = new HandlerExpressionBuilder(canExecuteChangedEvent);
					handlersCache.Add(key, builder);
				}
				Action updateStateFunc = () => updateState(target, canExecute);
				var handlerDelegate = builder.GetHandler(Expression.Call(
						Expression.Constant(updateStateFunc.Target), updateStateFunc.Method
					));
				builder.Subscribe(command, handlerDelegate);
				return new DisposableToken(() => builder.Unsubscribe(command, handlerDelegate));
			}
		}
		#endregion CommandExpressionBuilder
		#region CommandHandler
		sealed class CommandHandler<T> : ICommandChangeAware, IDisposable {
			EventInfo eventInfo;
			T target;
			CommandExpressionBuilder commandBuilder;
			object command;
			Func<object> queryParameters;
			Action<T, Func<bool>> updateState;
			public CommandHandler(T target, EventInfo eventInfo, CommandExpressionBuilder commandBuilder, object command, Func<object> queryParameters, Action<T, Func<bool>> updateState) {
				this.target = target;
				this.eventInfo = eventInfo;
				this.command = command;
				this.commandBuilder = commandBuilder;
				this.queryParameters = queryParameters;
				this.updateState = updateState;
				Subscribe(eventInfo);
			}
			public void Dispose() {
				Ref.Dispose(ref updateCommandStateToken);
				Unsubscribe(eventInfo);
				this.command = null;
				this.queryParameters = null;
				this.updateState = null;
				this.eventInfo = null;
				this.target = default(T);
				GC.SuppressFinalize(this);
			}
			Delegate handlerDelegate;
			IDisposable updateCommandStateToken;
			void Subscribe(EventInfo eventInfo) {
				if(command == null) return;
				if((handlerDelegate == null) && (eventInfo != null)) {
					HandlerExpressionBuilder builder = EventInfoHelper.GetBuilder(GetType(), eventInfo);
					var execute = commandBuilder.GetExecute(command, queryParameters);
					handlerDelegate = builder.GetHandler(Expression.Call(
						Expression.Constant(execute.Target), execute.Method));
					builder.Subscribe(target, handlerDelegate);
				}
				var canExecute = commandBuilder.GetCanExecute(command, queryParameters);
				updateState(target, canExecute);
				this.updateCommandStateToken = commandBuilder.SubscribeCommandState(command, target, canExecute, updateState);
			}
			void Unsubscribe(EventInfo eventInfo) {
				Ref.Dispose(ref updateCommandStateToken);
				HandlerExpressionBuilder builder;
				if((eventInfo != null) && EventInfoHelper.TryGetBuilder(GetType(), eventInfo, out builder)) {
					if(handlerDelegate != null)
						builder.Unsubscribe(target, handlerDelegate);
					this.handlerDelegate = null;
				}
			}
			void ICommandChangeAware.CommandChanged(object command, Type commandType) {
				if(object.ReferenceEquals(this.command, command)) return;
				Unsubscribe(eventInfo);
				this.command = command;
				UpdateCommandBuilder(commandType);
				Subscribe(eventInfo);
			}
			void UpdateCommandBuilder(Type commandType) {
				Type actualCommandType = (commandBuilder != null) ? commandBuilder.commandType : null;
				if(actualCommandType != commandType)
					commandBuilder = (commandType != null) ? GetCommandExpressionBuilder(commandType) : null;
			}
		}
		#endregion CommandHandler
		interface ICommandChangeAware {
			void CommandChanged(object command, Type commandType);
		}
		sealed class SourceChangeTracker : IDisposable {
			IDisposable handlerCore;
			DisposableObjectsContainer container;
			public SourceChangeTracker(string path, object source, MethodInfo commandMethod, IDisposable handler) {
				this.container = new DisposableObjectsContainer();
				this.handlerCore = handler;
				TrackSourceChanges(handler as ICommandChangeAware, path, source, source.GetType(), commandMethod);
			}
			void IDisposable.Dispose() {
				Ref.Dispose(ref handlerCore);
				Ref.Dispose(ref container);
				GC.SuppressFinalize(this);
			}
			void TrackSourceChanges(ICommandChangeAware handler, string path, object source, Type sourceType, MethodInfo commandMethod) {
				if(handler == null) return;
				do {
					string rootPath = NestedPropertiesHelper.GetRootPath(ref path);
					if(string.IsNullOrEmpty(rootPath))
						break;
					PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(sourceType);
					PropertyDescriptor rootDescriptor = properties[rootPath];
					if(rootDescriptor != null) {
						var propertySelector = ExpressionHelper.Accessor<object>(sourceType, rootDescriptor.Name);
						var sourceChangeAction = new CommandChangeAction(path, rootDescriptor.PropertyType, handler, commandMethod);
						var triggerRef = SetTrigger(source, sourceType, propertySelector, sourceChangeAction);
						if(triggerRef != null)
							container.Register(triggerRef);
						source = rootDescriptor.GetValue(source);
						sourceType = rootDescriptor.PropertyType;
					}
				}
				while(true);
			}
			IDisposable SetTrigger(object source, Type sourceType, Expression<Func<object, object>> propertySelector, CommandChangeAction triggerAction) {
				if(source is INotifyPropertyChanged)
					return BindingHelper.SetNPCTriggerCore(source, propertySelector, triggerAction);
				if(MemberInfoHelper.HasChangedEvent(sourceType, ExpressionHelper.GetPropertyName(propertySelector)))
					return BindingHelper.SetCLRTriggerCore(source, propertySelector, triggerAction);
				return null;
			}
			sealed class CommandChangeAction : ITriggerAction {
				string path;
				Type sourceType;
				ICommandChangeAware handler;
				MethodInfo commandMethod;
				public CommandChangeAction(string path, Type sourceType, ICommandChangeAware handler, MethodInfo commandMethod) {
					this.path = path;
					this.sourceType = sourceType;
					this.handler = handler;
					this.commandMethod = commandMethod;
				}
				bool ITriggerAction.CanExecute(object value) {
					return handler != null;
				}
				void ITriggerAction.Execute(object value) {
					executing++;
					if(value != null) {
						var source = NestedPropertiesHelper.GetSource(path, value, sourceType);
						var commandProp = MemberInfoHelper.GetCommandProperty(source, MVVMTypesResolver.Instance, commandMethod);
						handler.CommandChanged(commandProp.GetValue(source, new object[] { }), commandProp.PropertyType);
					}
					else handler.CommandChanged(null, null);
					executing--;
				}
				int executing;
				bool ITriggerAction.IsExecuting {
					get { return executing > 0; }
				}
			}
		}
	}
}
#if DEBUGTEST
namespace DevExpress.Utils.MVVM.Tests {
	using System;
	using NUnit.Framework;
	#region TestClasses
	class TestViewModel {
		public TestViewModel() {
			C1Command = new CommandImplicitImpl();
			C2PrivateCommand = new CommandImplicitImplPrivate();
			C3Command = new CommandExplicitImpl();
			C4PrivateCommand = new CommandExplicitImpl();
			PropertyForC5 = new CommandImplicitImpl();
			PropertyForC6 = new CommandImplicitImpl();
		}
		public CommandImplicitImpl C1Command { get; set; }
		public CommandImplicitImplPrivate C2PrivateCommand { get; set; }
		public ICommandTest C3Command { get; set; }
		public CommandExplicitImpl C4PrivateCommand { get; set; }
		public void C1() { }
		public void C2Private() { }
		public void C3() { }
		public void C4Private() { }
		[DataAnnotations.Command(Name = "PropertyForC5")]
		public void C5() { }
		public CommandImplicitImpl PropertyForC5 { get; set; }
		public void C6() { }
		public CommandImplicitImpl PropertyForC6 { get; set; }
	}
	class CommandImplicitImpl {
		public event EventHandler CanExecuteChanged;
		public bool CanExecute(object parameter) {
			return true;
		}
		public void Execute(object parameter) { executed = true; }
		public void Raise() {
			if(CanExecuteChanged != null)
				CanExecuteChanged(this, EventArgs.Empty);
		}
		internal bool executed;
	}
	class CommandImplicitImplPrivate {
		event EventHandler CanExecuteChanged;
		bool CanExecute(object parameter) {
			return true;
		}
		void Execute(object parameter) { executed = true; }
		public void Raise() {
			if(CanExecuteChanged != null)
				CanExecuteChanged(this, EventArgs.Empty);
		}
		internal bool executed;
	}
	class CommandExplicitImpl : ICommandTest {
		EventHandler CanExecuteChangedCore;
		event EventHandler ICommandTest.CanExecuteChanged {
			add { CanExecuteChangedCore += value; }
			remove { CanExecuteChangedCore -= value; }
		}
		bool ICommandTest.CanExecute(object parameter) {
			return true;
		}
		void ICommandTest.Execute(object parameter) { executed = true; }
		void ICommandTest.Raise() {
			RaiseCore();
		}
		internal void RaiseCore() {
			if(CanExecuteChangedCore != null)
				CanExecuteChangedCore(this, EventArgs.Empty);
		}
		internal bool executed;
		bool ICommandTest.executed { get { return executed; } }
	}
	interface ICommandTest {
		bool executed { get; }
		event EventHandler CanExecuteChanged;
		bool CanExecute(object parameter);
		void Execute(object parameter);
		void Raise();
	}
	class RootViewModel {
		public RootViewModel() {
			Child = new TestViewModel();
		}
		public RootViewModel Root { get; set; }
		public TestViewModel Child { get; private set; }
	}
	class NPCRootViewModel : NPCBase {
		TestViewModel childCore;
		public TestViewModel Child {
			get { return childCore; }
			set {
				if(childCore == value) return;
				childCore = value;
				this.RaisePropertyChanged("Child");
			}
		}
	}
	#endregion TestClasses
	[TestFixture]
	public class CommandHelperTests : MVVM.Tests.MVVMDependentTest {
		public override void FixtureTearDown() {
			MVVMTypesResolver.Reset();
			base.FixtureTearDown();
		}
		[Test]
		public void Bind_C1_Command() {
			TestViewModel viewModel = new TestViewModel();
			using(var btn = new System.Windows.Forms.Button()) {
				btn.Enabled = false;
				CommandHelper.Bind(btn,
					(button, execute) => button.Click += (s, e) => execute(),
					(button, canExecute) => button.Enabled = canExecute(),
					viewModel.C1Command);
				Assert.IsTrue(btn.Enabled);
				Assert.IsFalse(viewModel.C1Command.executed);
				btn.PerformClick();
				Assert.IsTrue(viewModel.C1Command.executed);
				btn.Enabled = false;
				viewModel.C1Command.Raise();
				Assert.IsTrue(btn.Enabled);
			}
		}
		[Test]
		public void Bind_C1_CommandSelector() {
			TestViewModel viewModel = new TestViewModel();
			using(var btn = new System.Windows.Forms.Button()) {
				btn.Enabled = false;
				CommandHelper.Bind(btn,
					(button, execute) => button.Click += (s, e) => execute(),
					(button, canExecute) => button.Enabled = canExecute(),
					() => viewModel.C1(), viewModel);
				Assert.IsTrue(btn.Enabled);
				Assert.IsFalse(viewModel.C1Command.executed);
				btn.PerformClick();
				Assert.IsTrue(viewModel.C1Command.executed);
				btn.Enabled = false;
				viewModel.C1Command.Raise();
				Assert.IsTrue(btn.Enabled);
			}
		}
		[Test]
		public void Bind_C2Private_Command() {
			TestViewModel viewModel = new TestViewModel();
			using(var btn = new System.Windows.Forms.Button()) {
				btn.Enabled = false;
				CommandHelper.Bind(btn,
					(button, execute) => button.Click += (s, e) => execute(),
					(button, canExecute) => button.Enabled = canExecute(),
					viewModel.C2PrivateCommand);
				Assert.IsTrue(btn.Enabled);
				Assert.IsFalse(viewModel.C2PrivateCommand.executed);
				btn.PerformClick();
				Assert.IsTrue(viewModel.C2PrivateCommand.executed);
				btn.Enabled = false;
				viewModel.C2PrivateCommand.Raise();
				Assert.IsTrue(btn.Enabled);
			}
		}
		[Test]
		public void Bind_C2Private_CommandSelector() {
			TestViewModel viewModel = new TestViewModel();
			using(var btn = new System.Windows.Forms.Button()) {
				btn.Enabled = false;
				CommandHelper.Bind(btn,
					(button, execute) => button.Click += (s, e) => execute(),
					(button, canExecute) => button.Enabled = canExecute(),
					() => viewModel.C2Private(), viewModel);
				Assert.IsTrue(btn.Enabled);
				Assert.IsFalse(viewModel.C2PrivateCommand.executed);
				btn.PerformClick();
				Assert.IsTrue(viewModel.C2PrivateCommand.executed);
				btn.Enabled = false;
				viewModel.C2PrivateCommand.Raise();
				Assert.IsTrue(btn.Enabled);
			}
		}
		[Test]
		public void Bind_C3_Command() {
			TestViewModel viewModel = new TestViewModel();
			using(var btn = new System.Windows.Forms.Button()) {
				btn.Enabled = false;
				CommandHelper.Bind(btn,
					(button, execute) => button.Click += (s, e) => execute(),
					(button, canExecute) => button.Enabled = canExecute(),
					viewModel.C3Command);
				Assert.IsTrue(btn.Enabled);
				Assert.IsFalse(viewModel.C3Command.executed);
				btn.PerformClick();
				Assert.IsTrue(viewModel.C3Command.executed);
				btn.Enabled = false;
				viewModel.C3Command.Raise();
				Assert.IsTrue(btn.Enabled);
			}
		}
		[Test]
		public void Bind_C3_CommandSelector() {
			TestViewModel viewModel = new TestViewModel();
			using(var btn = new System.Windows.Forms.Button()) {
				btn.Enabled = false;
				CommandHelper.Bind(btn,
					(button, execute) => button.Click += (s, e) => execute(),
					(button, canExecute) => button.Enabled = canExecute(),
					() => viewModel.C3(), viewModel);
				Assert.IsTrue(btn.Enabled);
				Assert.IsFalse(viewModel.C3Command.executed);
				btn.PerformClick();
				Assert.IsTrue(viewModel.C3Command.executed);
				btn.Enabled = false;
				viewModel.C3Command.Raise();
				Assert.IsTrue(btn.Enabled);
			}
		}
		[Test]
		public void Bind_C4Private_Command() {
			TestViewModel viewModel = new TestViewModel();
			using(var btn = new System.Windows.Forms.Button()) {
				btn.Enabled = false;
				CommandHelper.Bind(btn,
					(button, execute) => button.Click += (s, e) => execute(),
					(button, canExecute) => button.Enabled = canExecute(),
					viewModel.C4PrivateCommand);
				Assert.IsTrue(btn.Enabled);
				Assert.IsFalse(viewModel.C4PrivateCommand.executed);
				btn.PerformClick();
				Assert.IsTrue(viewModel.C4PrivateCommand.executed);
				btn.Enabled = false;
				viewModel.C4PrivateCommand.RaiseCore();
				Assert.IsTrue(btn.Enabled);
			}
		}
		[Test]
		public void Bind_C4Private_CommandSelector() {
			TestViewModel viewModel = new TestViewModel();
			using(var btn = new System.Windows.Forms.Button()) {
				btn.Enabled = false;
				CommandHelper.Bind(btn,
					(button, execute) => button.Click += (s, e) => execute(),
					(button, canExecute) => button.Enabled = canExecute(),
					() => viewModel.C4Private(), viewModel);
				Assert.IsTrue(btn.Enabled);
				Assert.IsFalse(viewModel.C4PrivateCommand.executed);
				btn.PerformClick();
				Assert.IsTrue(viewModel.C4PrivateCommand.executed);
				btn.Enabled = false;
				viewModel.C4PrivateCommand.RaiseCore();
				Assert.IsTrue(btn.Enabled);
			}
		}
		[Test]
		public void Bind_C5_CommandSelector_Attribute() {
			TestViewModel viewModel = new TestViewModel();
			using(var btn = new System.Windows.Forms.Button()) {
				btn.Enabled = false;
				CommandHelper.Bind(btn,
					(button, execute) => button.Click += (s, e) => execute(),
					(button, canExecute) => button.Enabled = canExecute(),
					() => viewModel.C5(), viewModel);
				Assert.IsTrue(btn.Enabled);
				Assert.IsFalse(viewModel.PropertyForC5.executed);
				btn.PerformClick();
				Assert.IsTrue(viewModel.PropertyForC5.executed);
				btn.Enabled = false;
				viewModel.PropertyForC5.Raise();
				Assert.IsTrue(btn.Enabled);
			}
		}
		[Test]
		public void Bind_C6_CommandSelector_Fluent() {
			MVVMAssemblyProxy.SetUpMetadataHelperAttributes(new Attribute[] {
				new DataAnnotations.CommandAttribute() { Name = "PropertyForC6" }
			});
			TestViewModel viewModel = new TestViewModel();
			using(var btn = new System.Windows.Forms.Button()) {
				btn.Enabled = false;
				CommandHelper.Bind(btn,
					(button, execute) => button.Click += (s, e) => execute(),
					(button, canExecute) => button.Enabled = canExecute(),
					() => viewModel.C6(), viewModel);
				Assert.IsTrue(btn.Enabled);
				Assert.IsFalse(viewModel.PropertyForC6.executed);
				btn.PerformClick();
				Assert.IsTrue(viewModel.PropertyForC6.executed);
				btn.Enabled = false;
				viewModel.PropertyForC6.Raise();
				Assert.IsTrue(btn.Enabled);
			}
			MVVMAssemblyProxy.ResetMetadataHelperAttributes();
		}
		[Test]
		public void Bind_C1_CommandSelector_Unsubscribe() {
			TestViewModel viewModel = new TestViewModel();
			using(var btn = new System.Windows.Forms.Button()) {
				btn.Enabled = false;
				var token = CommandHelper.Bind(btn,
					(button, execute) => button.Click += (s, e) => execute(),
					(button, canExecute) => button.Enabled = canExecute(),
					() => viewModel.C1(), viewModel);
				Assert.IsTrue(btn.Enabled);
				token.Dispose();
				Assert.IsFalse(viewModel.C1Command.executed);
				btn.PerformClick();
				Assert.IsFalse(viewModel.C1Command.executed);
				btn.Enabled = false;
				viewModel.C1Command.Raise();
				Assert.IsFalse(btn.Enabled);
			}
		}
	}
	[TestFixture]
	public class CommandHelperTests_NestedCommands : MVVM.Tests.MVVMDependentTest {
		public override void FixtureTearDown() {
			MVVMTypesResolver.Reset();
			base.FixtureTearDown();
		}
		[Test]
		public void Bind_C1_CommandSelector() {
			var viewModel = new RootViewModel();
			using(var btn = new System.Windows.Forms.Button()) {
				btn.Enabled = false;
				CommandHelper.Bind(btn,
					(button, execute) => button.Click += (s, e) => execute(),
					(button, canExecute) => button.Enabled = canExecute(),
					() => viewModel.Child.C1(), viewModel);
				Assert.IsTrue(btn.Enabled);
				Assert.IsFalse(viewModel.Child.C1Command.executed);
				btn.PerformClick();
				Assert.IsTrue(viewModel.Child.C1Command.executed);
				btn.Enabled = false;
				viewModel.Child.C1Command.Raise();
				Assert.IsTrue(btn.Enabled);
			}
		}
		[Test]
		public void Bind_C1_CommandSelector_MultiLevel() {
			var viewModel = new RootViewModel()
			{
				Root = new RootViewModel() { Root = new RootViewModel() }
			};
			using(var btn = new System.Windows.Forms.Button()) {
				btn.Enabled = false;
				CommandHelper.Bind(btn,
					(button, execute) => button.Click += (s, e) => execute(),
					(button, canExecute) => button.Enabled = canExecute(),
					() => viewModel.Root.Root.Child.C1(), viewModel);
				Assert.IsTrue(btn.Enabled);
				Assert.IsFalse(viewModel.Root.Root.Child.C1Command.executed);
				btn.PerformClick();
				Assert.IsTrue(viewModel.Root.Root.Child.C1Command.executed);
				btn.Enabled = false;
				viewModel.Root.Root.Child.C1Command.Raise();
				Assert.IsTrue(btn.Enabled);
			}
		}
		[Test]
		public void Bind_C1_SourceChanged() {
			var child = new TestViewModel();
			var viewModel = new NPCRootViewModel() { Child = child };
			using(var btn = new System.Windows.Forms.Button()) {
				btn.Enabled = false;
				CommandHelper.Bind(btn,
					(button, execute) => button.Click += (s, e) => execute(),
					(button, canExecute) => button.Enabled = canExecute(),
					() => viewModel.Child.C1(), viewModel);
				Assert.IsTrue(btn.Enabled);
				Assert.IsFalse(child.C1Command.executed);
				btn.PerformClick();
				Assert.IsTrue(child.C1Command.executed);
				btn.Enabled = false;
				child.C1Command.Raise();
				Assert.IsTrue(btn.Enabled);
				btn.Enabled = false;
				child.C1Command.executed = false;
				var newChild = new TestViewModel();
				viewModel.Child = newChild;
				Assert.IsTrue(btn.Enabled);
				Assert.IsFalse(newChild.C1Command.executed);
				btn.PerformClick();
				Assert.IsFalse(child.C1Command.executed);
				Assert.IsTrue(newChild.C1Command.executed);
				btn.Enabled = false;
				newChild.C1Command.Raise();
				Assert.IsTrue(btn.Enabled);
			}
		}
		[Test]
		public void Bind_C1_SourceChanged_Lazy() {
			var child = new TestViewModel();
			var viewModel = new NPCRootViewModel();
			using(var btn = new System.Windows.Forms.Button()) {
				btn.Enabled = false;
				CommandHelper.Bind(btn,
					(button, execute) => button.Click += (s, e) => execute(),
					(button, canExecute) => button.Enabled = canExecute(),
					() => viewModel.Child.C1(), viewModel);
				Assert.IsFalse(btn.Enabled);
				viewModel.Child = child;
				Assert.IsTrue(btn.Enabled);
				Assert.IsFalse(child.C1Command.executed);
				btn.PerformClick();
				Assert.IsTrue(child.C1Command.executed);
				btn.Enabled = false;
				child.C1Command.Raise();
				Assert.IsTrue(btn.Enabled);
				viewModel.Child = null;
				child.C1Command.executed = false;
				btn.PerformClick();
				Assert.IsFalse(child.C1Command.executed);
				btn.Enabled = false;
				child.C1Command.Raise();
				Assert.IsFalse(btn.Enabled);
			}
		}
	}
}
#endif
