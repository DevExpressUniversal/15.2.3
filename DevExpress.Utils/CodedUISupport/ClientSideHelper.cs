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
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Internal;
namespace DevExpress.Utils.CodedUISupport {
	[ToolboxItem(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ClientSideHelper : ClientSideHelperBase {
		public ClientSideHelper(RemoteObject remoteObject) : base(remoteObject) { }
		#region GetHandles methods
		class PropertyNames {
			public const string Name = "Name";
			public const string ClassName = "ClassName";
		}
		const string WindowsFormsHostTypeName = "System.Windows.Forms.Integration.WindowsFormsHost";
		const string PresentationFrameworkAssemblyName = "PresentationFramework";
		const string WpfApplicationTypeName = "System.Windows.Application";
		public int[] GetChildrenHandlesFromName(IntPtr parentHandle, string childName, bool contains, int searchDepth) {
			return CrossThreadRunMethod(parentHandle, (Func<int[]>)delegate() {
				return GetChildrenHandles(parentHandle, new SearchProperty(PropertyNames.Name, childName, contains), searchDepth);
			});
		}
		public int[] GetChildrenHandlesFromClassName(IntPtr parentHandle, string className, bool contains, int searchDepth) {
			return CrossThreadRunMethod(parentHandle, (Func<int[]>)delegate() {
				return GetChildrenHandles(parentHandle, new SearchProperty(PropertyNames.ClassName, className, contains), searchDepth);
			});
		}
		public int[] GetAllChildrenHandles(IntPtr parentHandle, int searchDepth) {
			return CrossThreadRunMethod(parentHandle, (Func<int[]>)delegate() {
				return GetChildrenHandles(parentHandle, new SearchProperty(), searchDepth);
			});
		}
		public int[] GetClosestDXChildrenHandles(IntPtr parentHandle) {
			return CrossThreadRunMethod(parentHandle, (Func<int[]>)delegate() {
				Control control = Control.FromHandle(parentHandle);
				if(control != null)
					return GetClosestDXChildrenHandles(control).ToArray();
				else
					return new int[] { };
			});
		}
		List<int> GetClosestDXChildrenHandles(Control control) {
			List<int> children = new List<int>();
			foreach(Control child in control.Controls)
				if(child.IsHandleCreated) {
					DXControlsSupported controlType = (DXControlsSupported)CodedUIUtils.IdentifyDXControl(child.Handle);
					if(controlType != DXControlsSupported.MaskBox) {
						if(controlType == DXControlsSupported.NotSupported)
							children.AddRange(GetClosestDXChildrenHandles(child));
						else
							children.Add(child.Handle.ToInt32());
					}
				}
			return children;
		}
		public int[] GetHandlesOfSiblingsWithSameClassName(IntPtr windowHandle, string className, int searchDepth) {
			return CrossThreadRunMethod(windowHandle, (Func<int[]>)delegate() {
				Control control = Control.FromHandle(windowHandle);
				if(control != null) {
					for(int i = 0; i < searchDepth; i++) {
						if(control.Parent != null)
							control = control.Parent;
						else
							break;
						if(i == searchDepth - 1)
							return GetChildrenHandles(control.Handle, new SearchProperty(PropertyNames.ClassName, className, false), searchDepth);
					}
				}
				return new int[] { };
			});
		}
		int[] GetChildrenHandles(IntPtr parentHandle, SearchProperty searchProperty, int searchDepth) {
			if(searchDepth <= 0)
				return null;
			List<Control> parentControls = new List<Control>();
			List<int> children = new List<int>();
			Control controlFromHandle = Control.FromHandle(parentHandle);
			if(controlFromHandle != null)
				parentControls.Add(controlFromHandle);
			else {
				HwndSource hwndSource = HwndSource.FromHwnd(parentHandle);
				if(hwndSource != null && hwndSource.RootVisual != null)
					parentControls.AddRange(GetAllContainedWinFormsAdapters(hwndSource.RootVisual));
				else {
					CodedUINativeMethods.EnumChildWindows(parentHandle, delegate(IntPtr childHandle, IntPtr lParam) {
						if(CodedUIUtils.IdentifyDXControl(childHandle) > 0) {
							Control child = Control.FromHandle(childHandle);
							if(IsControlMatchSearchProperty(child, searchProperty)) {
								IntPtr suitableHandle = GetSuitableControlHandle(child);
								if(suitableHandle != IntPtr.Zero)
									children.Add(suitableHandle.ToInt32());
							}
						}
						return true;
					}, IntPtr.Zero);
				}
			}
			foreach(Control parent in parentControls) {
				int currentDepth = 0;
				children.AddRange(GetChildrenHandlesRecursive(parent, searchProperty, searchDepth, ref currentDepth));
			}
			return children.ToArray();
		}
		List<Control> GetAllContainedWinFormsAdapters(DependencyObject parent) {
			List<Control> winFormsAdapters = new List<Control>();
			int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
			if(childrenCount > 0)
				for(int i = 0; i < childrenCount; i++) {
					DependencyObject child = VisualTreeHelper.GetChild(parent, i);
					if(child.GetType().FullName == WindowsFormsHostTypeName) {
						PropertyInfo childProperty = child.GetType().GetProperty("Child", BindingFlags.Public | BindingFlags.Instance);
						if(childProperty != null) {
							Control windowsFormsHostChild = childProperty.GetValue(child, new object[] { }) as Control;
							if(windowsFormsHostChild != null && windowsFormsHostChild.Parent != null)
								winFormsAdapters.Add(windowsFormsHostChild.Parent);
						}
					}
					else
						winFormsAdapters.AddRange(GetAllContainedWinFormsAdapters(child));
				}
			return winFormsAdapters;
		}
		List<int> GetChildrenHandlesRecursive(Control control, SearchProperty searchProperty, int searchDepth, ref int currentDepth) {
			List<int> handles = new List<int>();
			currentDepth++;
			foreach(Control childControl in control.Controls) {
				if(IsControlMatchSearchProperty(childControl, searchProperty)) {
					IntPtr handle = GetSuitableControlHandle(childControl);
					if(handle != IntPtr.Zero)
						handles.Add(handle.ToInt32());
				}
				if(currentDepth < searchDepth)
					handles.AddRange(GetChildrenHandlesRecursive(childControl, searchProperty, searchDepth, ref currentDepth));
			}
			currentDepth--;
			return handles;
		}
		bool IsControlMatchSearchProperty(Control control, SearchProperty searchProperty) {
			if(searchProperty.Name == PropertyNames.ClassName) {
				if(searchProperty.Compare(control.GetType().Name))
					return true;
			}
			else if(searchProperty.Name == PropertyNames.Name) {
				if(searchProperty.Compare(control.Name))
					return true;
			}
			else if(searchProperty.Name == null)
				return true;
			return false;
		}
		IntPtr GetSuitableControlHandle(Control control) {
			if(control.IsHandleCreated) {
				if(CodedUIUtils.IdentifyDXControl(control.Handle) != (int)DXControlsSupported.MaskBox)
					return control.Handle;
			}
			else if(control is ScrollTouchBase) {
				ScrollTouchBase scrollBar = control as ScrollTouchBase;
				if(scrollBar.TouchMode) {
					PropertyInfo FloatingScrollbarProperty = typeof(ScrollTouchBase).GetProperty("FloatingScrollbar", BindingFlags.NonPublic | BindingFlags.Instance);
					if(FloatingScrollbarProperty != null) {
						FloatingScrollbar floatingScrollbar = FloatingScrollbarProperty.GetValue(scrollBar, new object[] { }) as FloatingScrollbar;
						if(floatingScrollbar != null && floatingScrollbar.IsCreated)
							return floatingScrollbar.Handle;
					}
				}
			}
			return IntPtr.Zero;
		}
		public int GetPopupHandleOrOpenPopup(IntPtr ownerHandle) {
			return CrossThreadRunMethod(ownerHandle, (Func<int>)delegate() {
				Control control = Control.FromHandle(ownerHandle);
				if(control is Win.IPopupControl) {
					PropertyInfo isPopupOpenProperty = control.GetType().GetProperty("IsPopupOpen");
					if(isPopupOpenProperty != null) {
						if(!(bool)isPopupOpenProperty.GetValue(control, new object[] { })) {
							MethodInfo showPopupMethod = control.GetType().GetMethod("ShowPopup");
							if(showPopupMethod != null)
								control.BeginInvoke(new MethodInvoker(delegate() { showPopupMethod.Invoke(control, new object[] { }); }));
							return 0;
						}
					}
					if((control as Win.IPopupControl).PopupWindow != null)
						if((control as Win.IPopupControl).PopupWindow.IsHandleCreated)
							return (control as Win.IPopupControl).PopupWindow.Handle.ToInt32();
				}
				return 0;
			});
		}
		public int GetParentHandle(IntPtr windowHandle, out int depth) {
			int _depth = 0;
			int result = CrossThreadRunMethod(windowHandle, (Func<int>)delegate() {
				Control control = Control.FromHandle(windowHandle);
				if(control == null)
					return 0;
				else
					return FindParentRecursive(control, ref _depth);
			});
			depth = _depth;
			return result;
		}
		protected int FindParentRecursive(Control control, ref int depth) {
			if(control.Parent != null) {
				depth++;
				if(CodedUIUtils.IdentifyDXControl(control.Parent.Handle) > 0)
					return control.Parent.Handle.ToInt32();
				else
					return FindParentRecursive(control.Parent, ref depth);
			}
			else
				return 0;
		}
		public int GetChildHandleFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return CrossThreadRunMethod(windowHandle, (Func<int>)delegate() {
				Control control = Control.FromHandle(windowHandle);
				if(control != null) {
					System.Drawing.Point pointToClient = control.PointToClient(new System.Drawing.Point(pointX, pointY));
					Control child = control.GetChildAtPoint(pointToClient);
					if(child != null && child.IsHandleCreated && child.Visible)
						return child.Handle.ToInt32();
				}
				return 0;
			});
		}
		#endregion
		#region HandleViaReflectionMethods
		public string GetPropertyFast(IntPtr windowHandle, string propertyName, string targetTypeName) {
			return CrossThreadRunMethod(windowHandle, (Func<string>)delegate() {
				object target = Control.FromHandle(windowHandle);
				if(target == null)
					target = NativeWindow.FromHandle(windowHandle);
				if(target != null) {
					object result;
					if(CheckIfPropertyIsPassword(target, propertyName, null, false, out result))
						return (string)result;
					PropertyInfo propertyInfo;
					if(targetTypeName == null)
						propertyInfo = GetPropertyInfo(target, propertyName, BindingFlags.Public | BindingFlags.Instance);
					else {
						Type targetType = target.GetType();
						while(!(targetType == typeof(object) || targetType.FullName == targetTypeName))
							targetType = targetType.BaseType;
						propertyInfo = targetType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
					}
					if(propertyInfo != null) {
						result = propertyInfo.GetValue(target, new object[] { });
						return CodedUIUtils.ConvertToString(result);
					}
				}
				return null;
			});
		}
		public string HandleViaReflection(IntPtr windowHandle, string membersAsString, string memberValue, string memberValueType, int[] bindingFlags, bool isSet) {
			return CrossThreadRunMethod(windowHandle, (Func<string>)delegate() {
				object reflectionTarget = Control.FromHandle(windowHandle);
				if(reflectionTarget == null)
					reflectionTarget = NativeWindow.FromHandle(windowHandle);
				return HandleViaReflection(reflectionTarget, membersAsString, memberValue, memberValueType, bindingFlags, isSet);
			});
		}
		public string HandleViaReflection(object target, string membersAsString, string memberValue, string memberValueType, int[] bindingFlags, bool isSet) {
			try {
				if(target == null)
					return null;
				string[] members = membersAsString.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
				CheckBindingFlags(members.Length, ref bindingFlags);
				for(int i = 0; i < members.Length - 1; i++) {
					target = ProcessMember(target, members[i], null, null, bindingFlags[i], false);
				}
				object result = ProcessMember(target, members[members.Length - 1], memberValue, memberValueType, bindingFlags[members.Length - 1], isSet);
				return CodedUIUtils.ConvertToString(result);
			}
			catch(BreakOperationException) {
				return null;
			}
		}
		protected void CheckBindingFlags(int membersLength, ref int[] bindingFlags) {
			if(bindingFlags == null) {
				bindingFlags = new int[membersLength];
				for(int index = 0; index < membersLength; index++)
					bindingFlags[index] = (int)(BindingFlags.Public | BindingFlags.Instance);
			}
			if(bindingFlags.Length != membersLength)
				throw new BreakOperationException();
		}
		protected object ProcessMember(object target, string member, string memberValue, string memberValueType, int bindingFlags, bool isSet) {
			if(member.Contains("(") && member.Contains(")")) {
				return RunMethod(target, member, bindingFlags, isSet);
			}
			else if(member.Contains("[") && member.Contains("]")) {
				return GetSetCollectionElement(target, member, memberValue, memberValueType, bindingFlags, isSet);
			}
			else {
				return GetSetProperty(target, member, memberValue, memberValueType, bindingFlags, isSet);
			}
		}
		protected object GetSetProperty(object target, string propertyName, string newValue, string valueTypeName, int bindingFlags, bool isSet) {
			object result;
			if(CheckIfPropertyIsPassword(target, propertyName, newValue, isSet, out result))
				return result;
			PropertyInfo property = GetPropertyInfo(target, propertyName, (BindingFlags)bindingFlags);
			if(property == null)
				return GetSetField(target, propertyName, null, bindingFlags, isSet);
			if(!isSet)
				return GetPropertyValue(target, property);
			else {
				SetPropertyValue(target, property, newValue, valueTypeName);
				return null;
			}
		}
		protected object GetPropertyValue(object target, PropertyInfo propertyInfo) {
			object result = propertyInfo.GetValue(target, new object[] { });
			if(result == null)
				throw new BreakOperationException();
			else
				return result;
		}
		protected void SetPropertyValue(object target, PropertyInfo property, string newValue, string valueTypeName) {
			object convertedPropertyValue = newValue;
			if(convertedPropertyValue != null) {
				Type propertyType = property.PropertyType;
				if(!String.IsNullOrEmpty(valueTypeName)) {
					Type typeFromName = CodedUIUtils.GetTypeFromName(valueTypeName);
					if(typeFromName != typeof(object))
						propertyType = typeFromName;
				}
				if(propertyType == typeof(object))
					propertyType = typeof(string);
				convertedPropertyValue = CodedUIUtils.ConvertFromString(newValue, propertyType);
			}
			BeginInvoke(new Action(delegate() {
				property.SetValue(target, convertedPropertyValue, new object[] { });
			}), target as Control);
		}
		protected bool CheckIfPropertyIsPassword(object target, string member, string newValue, bool isSet, out object result) {
			result = null;
			if(target is Control) {
				List<string> membersForCheck = new List<string>(new string[] { "EditValue", "fEditValue", "Text" });
				if(membersForCheck.Contains(member)) {
					if((target as Control).IsHandleCreated && CodedUIUtils.IdentifyDXControl((target as Control).Handle) == (int)DXControlsSupported.MaskBox)
						throw new BreakOperationException();
					Control control = target as Control;
					PropertyInfo propertiesProperty = GetPropertyInfo(target, "Properties", BindingFlags.Public | BindingFlags.Instance);
					if(propertiesProperty != null)
						target = propertiesProperty.GetValue(target, new object[] { });
					PropertyInfo passwordCharProperty = target.GetType().GetProperty("PasswordChar");
					PropertyInfo useSystemPasswordCharProperty = target.GetType().GetProperty("UseSystemPasswordChar");
					if(passwordCharProperty != null && useSystemPasswordCharProperty != null) {
						char passwordChar = (char)passwordCharProperty.GetValue(target, new object[] { });
						bool useSystemPasswordChar = (bool)useSystemPasswordCharProperty.GetValue(target, new object[] { });
						if(useSystemPasswordChar || passwordChar != 0) {
							result = GetOrSetPassword(control, newValue, passwordChar, useSystemPasswordChar, isSet);
							return true;
						}
					}
				}
			}
			return false;
		}
		protected object GetOrSetPassword(Control target, string newValue, char passwordChar, bool useSystemPasswordChar, bool isSet) {
			PropertyInfo textProperty = GetPropertyInfo(target, "Text", BindingFlags.Public | BindingFlags.Instance);
			if(textProperty != null) {
				if(!isSet) {
					if(useSystemPasswordChar)
						passwordChar = '*';
					object result = textProperty.GetValue(target, new object[] { });
					StringBuilder sb = new StringBuilder(((string)result).Length);
					for(int i = 0; i < ((string)result).Length; i++)
						sb.Append(passwordChar);
					result = sb.ToString();
					return result;
				}
				else
					target.BeginInvoke(new MethodInvoker(delegate() { textProperty.SetValue(target, newValue, new object[] { }); }));
			}
			return null;
		}
		protected object GetSetField(object target, string fieldName, string fieldValue, int bindingFlags, bool isSet) {
			if(!isSet) {
				FieldInfo field = GetFieldInfo(target, fieldName, (BindingFlags)bindingFlags);
				if(field != null) {
					object result = field.GetValue(target);
					if(result != null)
						return result;
				}
			}
			throw new BreakOperationException();
		}
		protected object GetSetCollectionElement(object target, string propertyName, string newValue, string valueTypeName, int bindingFlags, bool isSet) {
			string collectionName = propertyName.Substring(0, propertyName.IndexOf("["));
			string indexString = propertyName.Substring(propertyName.IndexOf("[") + 1, propertyName.IndexOf("]") - propertyName.IndexOf("[") - 1);
			int index;
			if(Int32.TryParse(indexString, out index)) {
				PropertyInfo collectionProperty = GetPropertyInfo(target, collectionName, (BindingFlags)bindingFlags);
				if(collectionProperty != null) {
					if(!isSet)
						return GetCollectionElement(target, collectionProperty, index);
					else {
						SetCollectionElement(target, collectionProperty, index, newValue, valueTypeName);
						return null;
					}
				}
			}
			throw new BreakOperationException();
		}
		protected object GetCollectionElement(object target, PropertyInfo collectionProperty, int index) {
			IEnumerable collection = collectionProperty.GetValue(target, new object[] { }) as IEnumerable;
			object result = null;
			if(collection != null) {
				int collectionIndex = 0;
				foreach(object collectionElement in collection)
					if(index == collectionIndex) {
						result = collectionElement;
						break;
					}
					else
						collectionIndex++;
			}
			if(result == null)
				throw new BreakOperationException();
			else
				return result;
		}
		protected void SetCollectionElement(object target, PropertyInfo collectionProperty, int index, string newValue, string valueTypeName) {
			IList collection = collectionProperty.GetValue(target, new object[] { }) as IList;
			if(collection != null && index < collection.Count) {
				object convertedPropertyValue = newValue;
				if(convertedPropertyValue != null) {
					Type newElementType = GetTypeForSetCollectionElement(collection, index, valueTypeName);
					convertedPropertyValue = CodedUIUtils.ConvertFromString(newValue, newElementType);
				}
				BeginInvoke(new Action(delegate() {
					collection[index] = convertedPropertyValue;
				}), target as Control);
			}
		}
		protected object RunMethod(object target, string memberName, int bindingFlags, bool useBeginInvoke) {
			string[] parameters;
			MethodInfo method = GetMethodInfo(target, memberName, (BindingFlags)bindingFlags, out parameters);
			object result = null;
			if(method != null) {
				object[] convertedParameters = ConvertParametersForRunMethod(method, parameters);
				if(!useBeginInvoke) {
					try {
						result = method.Invoke(target, convertedParameters);
					}
					catch {
						useBeginInvoke = true;
					}
				}
				if(useBeginInvoke)
					BeginInvoke(new Action(delegate() {
						try {
							method.Invoke(target, convertedParameters);
						}
						catch { }
					}), target as Control);
			}
			if(result == null)
				throw new BreakOperationException();
			else
				return result;
		}
		protected PropertyInfo GetPropertyInfo(object target, string propertyName, BindingFlags bindingFlags) {
			PropertyInfo property = null;
			if(target != null) {
				try {
					property = target.GetType().GetProperty(propertyName, bindingFlags);
				}
				catch(System.Reflection.AmbiguousMatchException) {
					Type targetType = target.GetType();
					while(true) {
						property = targetType.GetProperty(propertyName, bindingFlags | BindingFlags.DeclaredOnly);
						if(property != null || targetType == typeof(object))
							break;
						targetType = targetType.BaseType;
					}
				}
			}
			return property;
		}
		protected FieldInfo GetFieldInfo(object target, string fieldName, BindingFlags bindingFlags) {
			FieldInfo field = null;
			try {
				field = target.GetType().GetField(fieldName, bindingFlags);
				if(field == null)
					throw new AmbiguousMatchException();
			}
			catch(System.Reflection.AmbiguousMatchException) {
				Type targetType = target.GetType();
				while(true) {
					field = targetType.GetField(fieldName, bindingFlags | BindingFlags.DeclaredOnly);
					if(field != null || targetType == typeof(object))
						break;
					targetType = targetType.BaseType;
				}
			}
			return field;
		}
		protected MethodInfo GetMethodInfo(object target, string memberName, BindingFlags bindingFlags, out string[] parameters) {
			string methodName = memberName.Substring(0, memberName.IndexOf("("));
			string parametersString = memberName.Substring(memberName.IndexOf("(") + 1, memberName.IndexOf(")") - memberName.IndexOf("(") - 1);
			parameters = parametersString.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
			MethodInfo[] methods = target.GetType().GetMethods(bindingFlags);
			foreach(MethodInfo mi in methods)
				if(mi.Name == methodName && mi.GetParameters().Length == parameters.Length) {
					return mi;
				}
			return null;
		}
		protected Type GetTypeForSetCollectionElement(IList collection, int index, string valueTypeName) {
			Type newElementType = typeof(object);
			if(valueTypeName != null)
				newElementType = CodedUIUtils.GetTypeFromName(valueTypeName);
			if(newElementType == typeof(object)) {
				object previousElement = null;
				previousElement = collection[index];
				if(previousElement != null)
					newElementType = previousElement.GetType();
			}
			return newElementType;
		}
		protected object[] ConvertParametersForRunMethod(MethodInfo method, string[] parameters) {
			object[] convertedParameters = new object[parameters.Length];
			ParameterInfo[] parametersInfo = method.GetParameters();
			for(int index = 0; index < parameters.Length; index++) {
				Type parameterType = parametersInfo[index].ParameterType;
				string parameter = parameters[index];
				if(parameters[index].Contains("!!")) {
					parameter = parameters[index].Substring(0, parameters[index].LastIndexOf("!!"));
					string parameterTypeString = parameters[index].Substring(parameters[index].LastIndexOf("!!") + 2);
					parameterTypeString = parameterTypeString.Replace(",", ".");
					parameterType = CodedUIUtils.GetTypeFromName(parameterTypeString);
				}
				convertedParameters[index] = CodedUIUtils.ConvertFromString(parameter, parameterType);
			}
			return convertedParameters;
		}
		protected Form FindForm() {
			foreach(Form form in Application.OpenForms)
				if(form.IsHandleCreated)
					return form;
			return null;
		}
		protected Dispatcher FindDispatcher() {
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach(Assembly assembly in assemblies)
				if(assembly.GetName().Name == PresentationFrameworkAssemblyName) {
					Type applicationType = assembly.GetType(WpfApplicationTypeName);
					if(applicationType != null) {
						PropertyInfo currentProperty = applicationType.GetProperty("Current", BindingFlags.Static | BindingFlags.Public);
						if(currentProperty != null) {
							object currentApplication = currentProperty.GetValue(null, null);
							if(currentApplication != null) {
								PropertyInfo mainWindowProperty = currentApplication.GetType().GetProperty("MainWindow", BindingFlags.Public | BindingFlags.Instance);
								if(mainWindowProperty != null) {
									DependencyObject mainWindow = mainWindowProperty.GetValue(currentApplication, null) as DependencyObject;
									if(mainWindow != null)
										return mainWindow.Dispatcher;
								}
							}
						}
					}
				}
			return null;
		}
		protected void BeginInvoke(Delegate method, Control control) {
			if(control != null)
				control.BeginInvoke(method);
			else {
				Form form = FindForm();
				if(form != null)
					form.BeginInvoke(method);
				else {
					Dispatcher dispatcher = FindDispatcher();
					if(dispatcher != null)
						dispatcher.BeginInvoke(method, null);
				}
			}
		}
		class BreakOperationException : System.Exception { }
		#endregion
		public string GetClassName(IntPtr windowHandle) {
			return CrossThreadRunMethod(windowHandle, (Func<string>)delegate() {
				Control control = Control.FromHandle(windowHandle);
				if(control != null)
					return control.GetType().Name;
				return null;
			});
		}
		public AppearanceObjectSerializable GetAppearanceObject(IntPtr windowHandle) {
			return CrossThreadRunMethod(windowHandle, delegate() {
				Control control = Control.FromHandle(windowHandle);
				if(control != null) {
					object appearance = null;
					PropertyInfo appearancePropertyInfo = GetPropertyInfo(control, "Appearance", BindingFlags.Public | BindingFlags.Instance);
					if(appearancePropertyInfo != null)
						appearance = appearancePropertyInfo.GetValue(control, new object[] { });
					if(!(appearance is AppearanceObject)) {
						PropertyInfo propertiesPropertyInfo = GetPropertyInfo(control, "Properties", BindingFlags.Public | BindingFlags.Instance);
						if(propertiesPropertyInfo != null) {
							object properties = propertiesPropertyInfo.GetValue(control, new object[] { });
							if(properties != null) {
								appearancePropertyInfo = GetPropertyInfo(properties, "Appearance", BindingFlags.Public | BindingFlags.Instance);
								if(appearancePropertyInfo != null)
									appearance = appearancePropertyInfo.GetValue(properties, new object[] { });
							}
						}
					}
					if(appearance is AppearanceObject)
						return new AppearanceObjectSerializable(appearance as AppearanceObject);
				}
				return null;
			});
		}
		protected ClientSideXtraGridMethods gridMethodsInternal;
		public ClientSideXtraGridMethods GridMethods {
			get {
				if(gridMethodsInternal == null)
					gridMethodsInternal = new ClientSideXtraGridMethods(this.remoteObject, this);
				return gridMethodsInternal;
			}
		}
		protected ClientSideXtraEditorsMethods editorsMethodsInternal;
		public ClientSideXtraEditorsMethods EditorsMethods {
			get {
				if(editorsMethodsInternal == null)
					editorsMethodsInternal = new ClientSideXtraEditorsMethods(this.remoteObject);
				return editorsMethodsInternal;
			}
		}
		protected ClientSideXtraBarsMethods barsMethodsInternal;
		public ClientSideXtraBarsMethods BarsMethods {
			get {
				if(barsMethodsInternal == null)
					barsMethodsInternal = new ClientSideXtraBarsMethods(this.remoteObject);
				return barsMethodsInternal;
			}
		}
		protected ClientSideXtraNavBarMethods navBarMethodsInternal;
		public ClientSideXtraNavBarMethods NavBarMethods {
			get {
				if(navBarMethodsInternal == null)
					navBarMethodsInternal = new ClientSideXtraNavBarMethods(this.remoteObject);
				return navBarMethodsInternal;
			}
		}
		protected ClientSideXtraPrintingMethods printingMethodsInternal;
		public ClientSideXtraPrintingMethods PrintingMethods {
			get {
				if(printingMethodsInternal == null)
					printingMethodsInternal = new ClientSideXtraPrintingMethods(this.remoteObject);
				return printingMethodsInternal;
			}
		}
		ClientSidePivotGridMethods pivotGridMethods;
		public ClientSidePivotGridMethods PivotGridMethods {
			get {
				if(pivotGridMethods == null)
					pivotGridMethods = new ClientSidePivotGridMethods(this.remoteObject);
				return pivotGridMethods;
			}
		}
		ClientSideUtilsMethods utilsMethods;
		public ClientSideUtilsMethods UtilsMethods {
			get {
				if(utilsMethods == null)
					utilsMethods = new ClientSideUtilsMethods(this.remoteObject);
				return utilsMethods;
			}
		}
		ClientSideXtraTreeListMethods treeListMethods;
		public ClientSideXtraTreeListMethods TreeListMethods {
			get {
				if(treeListMethods == null)
					treeListMethods = new ClientSideXtraTreeListMethods(this.remoteObject);
				return treeListMethods;
			}
		}
		ClientSideXtraVerticalGridMethods verticalGridMethods;
		public ClientSideXtraVerticalGridMethods VerticalGridMethods {
			get {
				if(verticalGridMethods == null)
					verticalGridMethods = new ClientSideXtraVerticalGridMethods(this.remoteObject);
				return verticalGridMethods;
			}
		}
		ClientSideXtraLayoutMethods xtraLayoutMethods;
		public ClientSideXtraLayoutMethods XtraLayoutMethods {
			get {
				if(xtraLayoutMethods == null)
					xtraLayoutMethods = new ClientSideXtraLayoutMethods(this.remoteObject);
				return xtraLayoutMethods;
			}
		}
	}
}
