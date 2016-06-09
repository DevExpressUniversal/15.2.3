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

namespace DevExpress.Utils.MVVM.Services {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Windows.Forms;
	public enum DefaultMessageBoxServiceType {
		Default,
		MessageBox,
		XtraMessageBox,
		FlyoutMessageBox
	}
	public static class MessageBoxService {
		public static object CreateMessageBoxService() {
			return Create(DefaultMessageBoxServiceType.MessageBox);
		}
		public static object CreateXtraMessageBoxService() {
			return Create(DefaultMessageBoxServiceType.XtraMessageBox);
		}
		public static object CreateFlyoutMessageBoxService() {
			return Create(DefaultMessageBoxServiceType.FlyoutMessageBox);
		}
		public static object Create(DefaultMessageBoxServiceType type) {
			IMessageBoxServiceFactory messageBoxServiceFactory = MessageBoxServiceProxy.CreateMessageBoxServiceFactory();
			switch(type) {
				case DefaultMessageBoxServiceType.MessageBox:
					return messageBoxServiceFactory.GetMessageBoxService();
				case DefaultMessageBoxServiceType.XtraMessageBox:
					return messageBoxServiceFactory.GetXtraMessageBoxService();
				case DefaultMessageBoxServiceType.FlyoutMessageBox:
					return messageBoxServiceFactory.GetFlyoutMessageBoxService();
				default:
					return messageBoxServiceFactory.GetXtraMessageBoxService();
			}
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IMessageBox {
		DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton);
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IMessageBoxProxy {
		int Show(string text, string caption, int buttons, int icon, int defaultButton);
	}
	interface IMessageBoxServiceFactory {
		object GetMessageBoxService();
		object GetXtraMessageBoxService();
		object GetFlyoutMessageBoxService();
	}
	sealed class MessageBoxServiceProxy {
		public static void RegisterMessageBoxService() {
			RegisterMessageBoxService(f => f.GetMessageBoxService());
		}
		public static void RegisterXtraMessageBoxService() {
			RegisterMessageBoxService(f => f.GetXtraMessageBoxService());
		}
		public static void RegisterFlyoutMessageBoxService() {
			RegisterMessageBoxService(f => f.GetFlyoutMessageBoxService());
		}
		static void RegisterMessageBoxService(Func<IMessageBoxServiceFactory, object> getService) {
			IPOCOInterfaces pocoInterfaces = POCOInterfacesProxy.Instance;
			object serviceContainer = pocoInterfaces.GetDefaultServiceContainer();
			IMessageBoxServiceFactory messageBoxServiceFactory = CreateMessageBoxServiceFactory();
			pocoInterfaces.RegisterService(serviceContainer, getService(messageBoxServiceFactory));
		}
		internal static IMessageBoxServiceFactory CreateMessageBoxServiceFactory() {
			IMVVMServiceTypesResolver typesResolver = MVVMTypesResolver.Instance as IMVVMServiceTypesResolver;
			return new MessageBoxServiceFactory(typesResolver.GetIMessageBoxServiceType());
		}
	}
	sealed class MessageBoxServiceFactory : IMessageBoxServiceFactory {
		Type messageBoxServiceType;
		MethodInfo methodInfo;
		ParameterInfo[] parameters;
		internal MessageBoxServiceFactory(Type messageBoxServiceType) {
			this.messageBoxServiceType = messageBoxServiceType;
			methodInfo = messageBoxServiceType.GetMethod("Show");
			parameters = methodInfo.GetParameters();
		}
		object IMessageBoxServiceFactory.GetMessageBoxService() {
			return GetMessageBoxService(typeof(System.Windows.Forms.MessageBox));
		}
		object IMessageBoxServiceFactory.GetXtraMessageBoxService() {
			Type messageBoxType = typeof(System.Windows.Forms.MessageBox);
			var editorsAssembly = AssemblyHelper.GetLoadedAssembly(AssemblyInfo.SRAssemblyEditors)
				?? AssemblyHelper.LoadDXAssembly(AssemblyInfo.SRAssemblyEditors);
			if(editorsAssembly != null)
				messageBoxType = editorsAssembly.GetType("DevExpress.XtraEditors.XtraMessageBox");
			return GetMessageBoxService(messageBoxType);
		}
		object IMessageBoxServiceFactory.GetFlyoutMessageBoxService() {
			Type messageBoxType = typeof(System.Windows.Forms.MessageBox);
			var barsAssembly = AssemblyHelper.GetLoadedAssembly(AssemblyInfo.SRAssemblyBars)
				?? AssemblyHelper.LoadDXAssembly(AssemblyInfo.SRAssemblyBars);
			if(barsAssembly != null)
				messageBoxType = barsAssembly.GetType("DevExpress.XtraBars.FlyoutMessageBox");
			return GetMessageBoxService(messageBoxType);
		}
		static IDictionary<Type, Func<IMessageBoxProxy, object>> cache = new Dictionary<Type, Func<IMessageBoxProxy, object>>();
		object GetMessageBoxService(Type messageBoxType) {
			Func<IMessageBoxProxy, object> create;
			if(!cache.TryGetValue(messageBoxType, out create)) {
				Type serviceType = GetMessageBoxServiceType(messageBoxType);
				var ctorInfo = serviceType.GetConstructor(new Type[] { typeof(IMessageBoxProxy) });
				var p = Expression.Parameter(typeof(IMessageBoxProxy));
				create = Expression.Lambda<Func<IMessageBoxProxy, object>>(
						Expression.New(ctorInfo, p), p
					).Compile();
				cache.Add(messageBoxType, create);
			}
			return create(new MessageBoxProxy(messageBoxType));
		}
		Type GetMessageBoxServiceType(Type messageBoxType) {
			var typeBuilder = DynamicTypesHelper.GetTypeBuilder(messageBoxType);
			var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(IMessageBoxProxy) });
			var proxyField = typeBuilder.DefineField("proxy", typeof(IMessageBoxProxy), FieldAttributes.InitOnly);
			var ctorGenerator = ctorBuilder.GetILGenerator();
			ctorGenerator.Emit(OpCodes.Ldarg_0);
			ctorGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
			ctorGenerator.Emit(OpCodes.Ldarg_0);
			ctorGenerator.Emit(OpCodes.Ldarg_1);
			ctorGenerator.Emit(OpCodes.Stfld, proxyField);
			ctorGenerator.Emit(OpCodes.Ret);
			var methodBuilder = typeBuilder.DefineMethod("Show", MethodAttributes.Public | MethodAttributes.Virtual, methodInfo.ReturnType, parameters.Select(p => p.ParameterType).ToArray());
			var methodGenerator = methodBuilder.GetILGenerator();
			methodGenerator.Emit(OpCodes.Ldarg_0);
			methodGenerator.Emit(OpCodes.Ldfld, proxyField);
			methodGenerator.Emit(OpCodes.Ldarg_1);
			methodGenerator.Emit(OpCodes.Ldarg_2);
			methodGenerator.Emit(OpCodes.Ldarg_3);
			methodGenerator.Emit(OpCodes.Ldarg_S, 4);
			methodGenerator.Emit(OpCodes.Ldarg_S, 5);
			methodGenerator.Emit(OpCodes.Callvirt, typeof(IMessageBoxProxy).GetMethod("Show"));
			methodGenerator.Emit(OpCodes.Ret);
			typeBuilder.AddInterfaceImplementation(messageBoxServiceType);
			typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
			return typeBuilder.CreateType();
		}
	}
	sealed class MessageBoxProxy : IMessageBoxProxy {
		readonly IMessageBox messageBox;
		static IDictionary<Type, IMessageBox> cache = new Dictionary<Type, IMessageBox>();
		public MessageBoxProxy(Type messageBoxType) {
			if(messageBoxType != null) {
				if(!cache.TryGetValue(messageBoxType, out messageBox)) {
					var showMethod = messageBoxType.GetMethod("Show", new Type[] { typeof(string), typeof(string), typeof(MessageBoxButtons), typeof(MessageBoxIcon), typeof(MessageBoxDefaultButton) });
					if(showMethod != null) {
						ParameterExpression text = Expression.Parameter(typeof(string), "text");
						ParameterExpression caption = Expression.Parameter(typeof(string), "caption");
						ParameterExpression buttons = Expression.Parameter(typeof(MessageBoxButtons), "buttons");
						ParameterExpression icon = Expression.Parameter(typeof(MessageBoxIcon), "icon");
						ParameterExpression defaultButton = Expression.Parameter(typeof(MessageBoxDefaultButton), "defaultButton");
						var call = Expression.Call(null, showMethod, text, caption, buttons, icon, defaultButton);
						var show = Expression.Lambda<Func<string, string, MessageBoxButtons, MessageBoxIcon, MessageBoxDefaultButton, DialogResult>>(
							call, text, caption, buttons, icon, defaultButton).Compile();
						this.messageBox = new Default(show);
					}
					cache.Add(messageBoxType, messageBox);
				}
			}
			messageBox = messageBox ?? Empty.Instance;
		}
		int IMessageBoxProxy.Show(string messageBoxText, string caption, int button, int icon, int defaultResult) {
			return messageBox.Show(messageBoxText, caption,
					button.ToMessageBoxButtons(),
					icon.ToMessageBoxIcon(),
					defaultResult.ToMessageBoxDefaultButton(button)
				).ToMessageResult();
		}
		class Default : IMessageBox {
			Func<string, string, MessageBoxButtons, MessageBoxIcon, MessageBoxDefaultButton, DialogResult> show;
			public Default(Func<string, string, MessageBoxButtons, MessageBoxIcon, MessageBoxDefaultButton, DialogResult> show) {
				this.show = show;
			}
			DialogResult IMessageBox.Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) {
				return show(text, caption, buttons, icon, defaultButton);
			}
		}
		class Empty : IMessageBox {
			internal static IMessageBox Instance = new Empty();
			Empty() { }
			DialogResult IMessageBox.Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) {
				return DialogResult.No;
			}
		}
	}
	static class MessageBoxEnumsConverter {
		internal static MessageBoxDefaultButton ToMessageBoxDefaultButton(this int result, int buttons) {
			switch(result) {
				case 4:
					return MessageBoxDefaultButton.Button2;
				case 2:
					return (buttons == 1) ?
						MessageBoxDefaultButton.Button2 : MessageBoxDefaultButton.Button3;
				default:
					return MessageBoxDefaultButton.Button1;
			}
		}
		internal static int ToMessageResult(this DialogResult result) {
			switch(result) {
				case DialogResult.Cancel: return 2;
				case DialogResult.No: return 4;
				case DialogResult.Yes: return 3;
				case DialogResult.OK: return 1;
				default: return 0;
			}
		}
		internal static MessageBoxButtons ToMessageBoxButtons(this int button) {
			switch(button) {
				case 1: return MessageBoxButtons.OKCancel;
				case 3: return MessageBoxButtons.YesNo;
				case 2: return MessageBoxButtons.YesNoCancel;
				default: return MessageBoxButtons.OK;
			}
		}
		internal static int ToMessageButton(this ConfirmationButtons button) {
			switch(button) {
				case ConfirmationButtons.OKCancel: return 1;
				case ConfirmationButtons.YesNo: return 3;
				case ConfirmationButtons.YesNoCancel: return 2;
				default: return 0;
			}
		}
		internal static int ToMessageButton(this MessageBoxButtons button) {
			switch(button) {
				case MessageBoxButtons.OKCancel: return 1;
				case MessageBoxButtons.YesNo: return 3;
				case MessageBoxButtons.YesNoCancel: return 2;
				default: return 0;
			}
		}
		internal static MessageBoxIcon ToMessageBoxIcon(this int icon) {
			switch(icon) {
				case 1: return MessageBoxIcon.Error;
				case 4: return MessageBoxIcon.Information;
				case 2: return MessageBoxIcon.Question;
				case 3: return MessageBoxIcon.Warning;
				default: return MessageBoxIcon.None;
			}
		}
		internal static int ToMessageIcon(this MessageBoxIcon icon) {
			switch(icon) {
				case MessageBoxIcon.Error: return 1;
				case MessageBoxIcon.Information: return 4;
				case MessageBoxIcon.Question: return 2;
				case MessageBoxIcon.Warning: return 3;
				default: return 0;
			}
		}
	}
}
#if DEBUGTEST
namespace DevExpress.Utils.MVVM.Tests {
	using DevExpress.Utils.MVVM.Services;
	using NUnit.Framework;
	[TestFixture]
	public class MessageBoxProxyTest {
		[Test, Explicit]
		public void Test00_Smoke() {
			((IMessageBoxProxy)new MessageBoxProxy(typeof(System.Windows.Forms.MessageBox))).Show("text", "caption", 3, 2, 1);
		}
	}
	[TestFixture]
	public class MessageBoxServiceFactoryTest {
		[Test, Explicit]
		public void Test00_Smoke() {
			IMVVMServiceTypesResolver typesResolver = MVVMTypesResolver.Instance as IMVVMServiceTypesResolver;
			IMessageBoxServiceFactory f = new MessageBoxServiceFactory(typesResolver.GetIMessageBoxServiceType());
			var service = f.GetMessageBoxService();
			Assert.IsNotNull(service);
		}
	}
}
#endif
