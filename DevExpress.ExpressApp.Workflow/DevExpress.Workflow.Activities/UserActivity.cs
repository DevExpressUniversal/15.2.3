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

using System.Activities;
using System.Reflection;
using System;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Security.Policy;
using System.IO;
using System.Activities.XamlIntegration;
using System.ComponentModel;
using System.Activities.Expressions;
using System.Threading;
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.Workflow.Activities {
	[ToolboxBitmap(typeof(ObjectSpaceTransactionScope), "Images.UserActivity.bmp")]
	public abstract class UserActivity : NativeActivity {
		private string xaml = null;
		private Activity activityBody;
		protected override void Execute(NativeActivityContext context) {
			foreach(var innerActivityProperty in ((DynamicActivity)activityBody).Properties) {
				if(typeof(Argument).IsAssignableFrom(innerActivityProperty.Type)) {
					Argument outerActivityArgument = (Argument)GetType().GetProperty(innerActivityProperty.Name).GetValue(this, null);
					if(typeof(OutArgument).IsAssignableFrom(innerActivityProperty.Type) || typeof(InOutArgument).IsAssignableFrom(innerActivityProperty.Type)) {
						if(outerActivityArgument.Expression != null) {
							((Argument)(innerActivityProperty.Value)).Expression = outerActivityArgument.Expression;
						}
					}
					if(typeof(InArgument).IsAssignableFrom(innerActivityProperty.Type) || typeof(InOutArgument).IsAssignableFrom(innerActivityProperty.Type)) {
						object argValue = outerActivityArgument.Get(context);
						if(argValue != null) {
							Type expressionType = typeof(Literal<>).MakeGenericType(outerActivityArgument.ArgumentType);
							object expression = Activator.CreateInstance(expressionType);
							expression.GetType().GetProperty("Value").SetValue(expression, argValue, null);
							((Argument)(innerActivityProperty.Value)).Expression = expression as ActivityWithResult;
						}
					}
				}
			}
			context.ScheduleActivity(activityBody);
		}
		protected override void CacheMetadata(NativeActivityMetadata metadata) {
			activityBody = ActivityXamlServices.Load(new StringReader(xaml));
			metadata.AddImplementationChild(activityBody);
			base.CacheMetadata(metadata);
		}
		public void SetActivityXaml(string xaml) {
			this.xaml = xaml;
		}
		public UserActivity() {
			this.xaml = null;
		}
	}
	public class UserActivityDescription {
		public UserActivityDescription() { }
		public UserActivityDescription(string typeName, string xaml, string displayName) {
			TypeName = typeName;
			Xaml = xaml;
			DisplayName = displayName;
		}
		public string TypeName { get; private set; }
		public string Xaml { get; private set; }
		public string DisplayName { get; private set; }
	}
	public class UserActivityBuilderHelper {
		private const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
		private const string UserActivitiesAssemblyName = "UserActivities";
		private static int revision = 0;
		public static string GetCurrentAssemblyName() {
			return UserActivitiesAssemblyName + "_" + revision.ToString();
		}
		public static IDictionary<string, Type> CreateUserActivityTypes(IList<UserActivityDescription> activities) {
			return CreateUserActivityTypes(activities, UserActivitiesAssemblyName);
		}
		public static Type CreateUserActivityType(string assemblyName, string displayName, string typeName, string xaml, ModuleBuilder moduleBuilder) {
			Guard.ArgumentNotNull(displayName, "displayName");
			DynamicActivity activity = (DynamicActivity)ActivityXamlServices.Load(new StringReader(xaml));
			TypeBuilder typeBuilder = moduleBuilder.DefineType(UserActivitiesAssemblyName + "." + typeName, TypeAttributes.Public | TypeAttributes.Class, typeof(UserActivity));
			ConstructorBuilder ctor0 = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
			ILGenerator ctor0IL = ctor0.GetILGenerator();
			ctor0IL.Emit(OpCodes.Ldarg_0);
			ctor0IL.Emit(OpCodes.Call, typeof(UserActivity).GetConstructor(Type.EmptyTypes));
			ctor0IL.Emit(OpCodes.Ldarg_0);
			ctor0IL.Emit(OpCodes.Ldstr, displayName);
			ctor0IL.Emit(OpCodes.Call, typeof(UserActivity).GetMethod("set_DisplayName"));
			ctor0IL.Emit(OpCodes.Ldarg_0);
			ctor0IL.Emit(OpCodes.Ldstr, xaml);
			ctor0IL.Emit(OpCodes.Call, typeof(UserActivity).GetMethod("SetActivityXaml"));
			ctor0IL.Emit(OpCodes.Ret);
			foreach(var property in activity.Properties) {
				if(typeof(Argument).IsAssignableFrom(property.Type)) {
					FieldBuilder fbField = typeBuilder.DefineField("_" + property.Name, property.Type, FieldAttributes.Private);
					PropertyBuilder pbProperty = typeBuilder.DefineProperty(property.Name, PropertyAttributes.HasDefault, property.Type, null);
					MethodBuilder mbPropertyGetAccessor = typeBuilder.DefineMethod("get_" + property.Name, getSetAttr, property.Type, Type.EmptyTypes);
					ILGenerator getIL = mbPropertyGetAccessor.GetILGenerator();
					getIL.Emit(OpCodes.Ldarg_0);
					getIL.Emit(OpCodes.Ldfld, fbField);
					getIL.Emit(OpCodes.Ret);
					pbProperty.SetGetMethod(mbPropertyGetAccessor);
					MethodBuilder mbPropertySetAccessor = typeBuilder.DefineMethod("set_" + property.Name, getSetAttr, null, new Type[] { property.Type });
					ILGenerator setIL = mbPropertySetAccessor.GetILGenerator();
					setIL.Emit(OpCodes.Ldarg_0);
					setIL.Emit(OpCodes.Ldarg_1);
					setIL.Emit(OpCodes.Stfld, fbField);
					setIL.Emit(OpCodes.Ret);
					pbProperty.SetSetMethod(mbPropertySetAccessor);
				}
			}
			return typeBuilder.CreateType();
		}
		private static string GetHash(string assemblyName, string displayName, string typeName, string xaml) {
			return assemblyName + typeName + displayName + xaml.GetHashCode();
		}
		public static IDictionary<string, Type> CreateUserActivityTypes(IList<UserActivityDescription> activities, string userActivitiesAssemblyName) {
			CustomCreateUserActivityTypesEventArgs args = new CustomCreateUserActivityTypesEventArgs(activities, userActivitiesAssemblyName);
			if(CustomCreateUserActivityTypes != null) {
				CustomCreateUserActivityTypes(null, args);
				if(args.Handled) {
					return args.UserActivityTypes;
				}
			}
			Dictionary<string, Type> result = new Dictionary<string, Type>();
			if(activities.Count > 0) {
				AssemblyName assemblyName = new AssemblyName(userActivitiesAssemblyName);
				assemblyName.Version = new Version(1, 0, 0, revision++);
				AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
				ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(userActivitiesAssemblyName, userActivitiesAssemblyName + ".dll");
				foreach(UserActivityDescription activityDescription in activities) {
					if(!string.IsNullOrEmpty(activityDescription.DisplayName)) {
						Type type = CreateUserActivityType(assemblyName.Name, activityDescription.DisplayName, activityDescription.TypeName, activityDescription.Xaml, moduleBuilder);
						result.Add(activityDescription.DisplayName, type);
					}
				}
			}
			return result;
		}
		public static event EventHandler<CustomCreateUserActivityTypesEventArgs> CustomCreateUserActivityTypes;
	}
	public class CustomCreateUserActivityTypesEventArgs : HandledEventArgs {
		public CustomCreateUserActivityTypesEventArgs(IList<UserActivityDescription> activities, string userActivitiesAssemblyName) {
			this.UserActivityTypes = new Dictionary<string, Type>();
			this.Activities = activities;
			this.UserActivitiesAssemblyName = userActivitiesAssemblyName;
		}
		public IList<UserActivityDescription> Activities  { get; private set; }
		public string UserActivitiesAssemblyName  { get; private set; }
		public Dictionary<string, Type> UserActivityTypes { get; private set; }
	}
}
