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
using System.Linq;
using System.Text;
using System.Activities.Validation;
using System.Activities;
using System.Activities.Statements;
using DevExpress.Workflow.Activities;
using System.Collections.ObjectModel;
using DevExpress.ExpressApp;
using System.ComponentModel;
namespace DevExpress.Workflow.Utils {
#if DebugTest
	public class DebugTest_TraceActivity : NativeActivity {
		protected override void Execute(NativeActivityContext context) {
			string val = this.Text.Get(context);
			val = string.IsNullOrEmpty(val) ? "Null" : val;
			Trace += val + "\r\n";
		}
		[DefaultValue(null)]
		public InArgument<string> Text { get; set; }
		public static string Trace = "";
	}
#endif
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public static class ConstraintHelper {
		public static Constraint VerifyParent<T>(Activity activity) {
			DelegateInArgument<Activity> element = new DelegateInArgument<Activity>();
			DelegateInArgument<ValidationContext> context = new DelegateInArgument<ValidationContext>();
			DelegateInArgument<Activity> child = new DelegateInArgument<Activity>();
			Variable<bool> result = new Variable<bool>();
			return new Constraint<Activity> {
				Body = new ActivityAction<Activity, ValidationContext> {
					Argument1 = element,
					Argument2 = context,
					Handler = new Sequence {
						Variables = { result },
						Activities = {
							new ForEach<Activity>  {								
								Values = new GetParentChain { ValidationContext = context },
								Body = new ActivityAction<Activity> {									
									Argument = child, 
									Handler = new If {										  
										Condition = new InArgument<bool>((env) => typeof(T).IsAssignableFrom(child.Get(env).GetType())),
										Then = new Assign<bool>  {
											Value = true,
											To = result
										}
									}
								}								
							},
							new AssertValidation {
								Assertion = new InArgument<bool>(env => result.Get(env)),
								Message = new InArgument<string> (string.Format("{0} can only be added inside an {1} activity.", activity.GetType().Name, typeof(T).Name)),
								PropertyName = new InArgument<string>((env) => element.Get(env).DisplayName)
							}
						}
					}
				}
			};
		}
		private static ActivityAction<Variable> CreateCheckVariableActivity(DelegateInArgument<Variable> variable, Variable<bool> result, Variable<string> variableName) {
			return new ActivityAction<Variable> {
				Argument = variable,
				Handler = new If {
					Condition = new InArgument<bool>((env) => !(variable.Get(env).GetType().IsGenericType && variable.Get(env).GetType().GetGenericArguments().Length > 0 && CanSerializeInstanceOfType(variable.Get(env).GetType().GetGenericArguments()[0]))),
					Then = new Sequence {
						Activities = {
							new Assign<bool> {
								Value = true,
								To = result
							},
							new Assign<string> {
								Value = new InArgument<string>((env) => variable.Get(env).Name),
								To = variableName
							},
#if DebugTest
							new DebugTest_TraceActivity { Text = new InArgument<string>((env) => variable.Get(env).Name + "/" + variable.Get(env).Type.ToString() + "/" + variableName.Get(env)) },
#endif
						}
					}
				}
			};
		}
		private static AssertValidation CreateAssertValidation(Variable<Activity> element, Variable<bool> result, Variable<string> variableName) {
			return new AssertValidation {
				Assertion = new InArgument<bool>(env => !result.Get(env)),
				Message = new InArgument<string>((env) => string.Format("Activity '{0} - {1}' contains non-serializable variable '{2}'.", element.Get(env).Id, element.Get(env).DisplayName, variableName.Get(env))),
				PropertyName = new InArgument<string>((env) => variableName.Get(env))
			};
		}
		public static bool CanSerializeInstanceOfTypeCore(Type type, List<Type> checkedTypes) {
			if(checkedTypes.Contains(type)) {
				return true;
			}
			CustomCanSerializeInstanceOfTypeEventArgs args = new CustomCanSerializeInstanceOfTypeEventArgs(type);
			if(CustomCanSerializeInstanceOfType != null) {
				CustomCanSerializeInstanceOfType(null, args);
			}
			if(args.CanSerialize.HasValue) {
				return args.CanSerialize.Value;
			}
			bool result = type.FullName.StartsWith("System.")
				|| type.IsSerializable;
			if(result && type.HasElementType) {
				Type elementType = type.GetElementType();
				if(elementType != null) {
					result = CanSerializeInstanceOfTypeCore(elementType, checkedTypes);
				}
			}
			if(result && type.IsGenericType &&
					(ForceCheckCanSerializeIListElementType && type.GetGenericTypeDefinition() == typeof(IList<>)
					|| ForceCheckCanSerializeIEnumerableElementType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
					|| ForceCheckCanSerializeListElementType && type.GetGenericTypeDefinition() == typeof(List<>))) {
				Type elementType = type.GetGenericArguments()[0];
				result = CanSerializeInstanceOfTypeCore(elementType, checkedTypes);
			}
			return result;
		}
		static ConstraintHelper() {
			ForceCheckCanSerializeIListElementType = true;
			ForceCheckCanSerializeIEnumerableElementType = true;
			ForceCheckCanSerializeListElementType = true;
		}
		[DefaultValue(true)]
		public static bool ForceCheckCanSerializeIListElementType { get; set; }
		[DefaultValue(true)]
		public static bool ForceCheckCanSerializeIEnumerableElementType { get; set; }
		[DefaultValue(true)]
		public static bool ForceCheckCanSerializeListElementType { get; set; }
		public static bool CanSerializeInstanceOfType(Type variableType) {
			List<Type> checkedTypes = new List<Type>();
			return CanSerializeInstanceOfTypeCore(variableType, checkedTypes);
		}
		public static Constraint SequenceHasNoVariablesOfNonSerializableObjectTypes() {
			DelegateInArgument<Sequence> element = new DelegateInArgument<Sequence>();
			Variable<Activity> activity = new Variable<Activity>();
			DelegateInArgument<Variable> variable = new DelegateInArgument<Variable>();
			Variable<string> variableName = new Variable<string>();
			Variable<bool> result = new Variable<bool>();
			return new Constraint<Sequence> {
				Body = new ActivityAction<Sequence, ValidationContext> {
					Argument1 = element,
					Handler = new Sequence {
						Variables = { result, variableName, activity },
						Activities = {
							new ForEach<Variable>  {								
								Values = new InArgument<IEnumerable<Variable>>((env) => element.Get(env).Variables),
								Body = CreateCheckVariableActivity(variable, result, variableName)
							},								
#if DebugTest
							new DebugTest_TraceActivity { Text = new InArgument<string>((env) => "(" + result.Get(env).ToString() + ")") },
#endif
							new Assign<Activity> { To = activity, Value = element },
							CreateAssertValidation(activity, result, variableName)
						}
					}
				}
			};
		}
		public static Constraint FlowchartHasNoVariablesOfNonSerializableObjectTypes() {
			DelegateInArgument<Flowchart> element = new DelegateInArgument<Flowchart>();
			Variable<Activity> activity = new Variable<Activity>();
			DelegateInArgument<Variable> variable = new DelegateInArgument<Variable>();
			Variable<string> variableName = new Variable<string>();
			Variable<bool> result = new Variable<bool>();
			return new Constraint<Flowchart> {
				Body = new ActivityAction<Flowchart, ValidationContext> {
					Argument1 = element,
					Handler = new Sequence {
						Variables = { result, variableName, activity },
						Activities = {
							new ForEach<Variable>  {								
								Values = new InArgument<IEnumerable<Variable>>((env) => element.Get(env).Variables),
								Body = CreateCheckVariableActivity(variable, result, variableName)
							},								
#if DebugTest
							new DebugTest_TraceActivity { Text = new InArgument<string>((env) => "(" + result.Get(env).ToString() + ")") },
#endif
							new Assign<Activity> { To = activity, Value = element },
							CreateAssertValidation(activity, result, variableName)
						}
					}
				}
			};
		}
		public static ValidationResults ValidateActivity(Activity activity) {
			ValidationSettings settings = new ValidationSettings() {
				AdditionalConstraints = {
					{ typeof(System.Activities.Statements.Sequence), new List<Constraint> { ConstraintHelper.SequenceHasNoVariablesOfNonSerializableObjectTypes() } },	 
					{ typeof(System.Activities.Statements.Flowchart), new List<Constraint> { ConstraintHelper.FlowchartHasNoVariablesOfNonSerializableObjectTypes() } },	 
				},
				SingleLevel = false
			};
			return ActivityValidationServices.Validate(activity, settings);
		}
		public static event EventHandler<CustomCanSerializeInstanceOfTypeEventArgs> CustomCanSerializeInstanceOfType;
	}
	public class CustomCanSerializeInstanceOfTypeEventArgs : EventArgs {
		public CustomCanSerializeInstanceOfTypeEventArgs(Type type) {
			this.InstanceOfType = type;
		}
		public Type InstanceOfType {get;private set;}
		public bool? CanSerialize { get; set; }
	}
}
