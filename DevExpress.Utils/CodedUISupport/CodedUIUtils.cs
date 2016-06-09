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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace DevExpress.Utils.CodedUISupport {
	public class CodedUIUtils {
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsIdentifyControlMessage(ref Message message, object sender) {
			if(message.Msg == SharedMembers.DevExpressCodedUIMessage) {
				message.Result = IntPtr.Zero;
				CodedUICommands commands = (CodedUICommands)message.LParam.ToInt32();
				if(commands.HasFlag(CodedUICommands.DisableControlAfterProcessing)) {
					if(sender is Control && !(sender as Control).Enabled)
						CodedUINativeMethods.DisableWindow((sender as Control).Handle);
				}
				if(commands.HasFlag(CodedUICommands.IdentifyDxControl)) {
					return true;
				}
				else if(commands.HasFlag(CodedUICommands.InitIpcConnection)) {
					NamedPipeClient.Instance.Connect(message.WParam.ToInt32());
					message.Result = new IntPtr(1);
					return false;
				}
				else if(commands.HasFlag(CodedUICommands.ParentHandleRequest)) {
					if(sender is Control && (sender as Control).Parent != null && (sender as Control).Parent.IsHandleCreated)
						message.Result = (sender as Control).Parent.Handle;
					return false;
				}
				else if(commands.HasFlag(CodedUICommands.SaveUtilsAssemblyPath)) {
					string utilsPath = GetAssemblyPath(AssemblyInfo.SRAssemblyUtils);
					string dataPath = GetAssemblyPath(AssemblyInfo.SRAssemblyData);
					string utilsResourcesPath = GetAssemblyPath(AssemblyInfo.SRAssemblyUtils + ".resources");
					string dataResourcesPath = GetAssemblyPath(AssemblyInfo.SRAssemblyData + ".resources");
					FileStream tempFile = File.Open(SharedMembers.UtilsAssemblyPathFilePath, FileMode.Create);
					StreamWriter sw = new StreamWriter(tempFile);
					sw.WriteLine(utilsPath);
					sw.WriteLine(dataPath);
					sw.WriteLine(utilsResourcesPath);
					sw.WriteLine(dataResourcesPath);
					sw.Close();
					tempFile.Close();
					message.Result = new IntPtr(1);
					return false;
				}
			}
			return false;
		}
		static string GetAssemblyPath(string assemblyName) {
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach(Assembly assembly in assemblies)
				if(assembly.GetName().Name == assemblyName)
					return assembly.Location;
			return String.Empty;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static int IdentifyDXControl(IntPtr windowHandle) {
			CodedUICommands command = CodedUICommands.IdentifyDxControl;
			bool wasWindowEnabled = CodedUINativeMethods.IsWindowEnabled(windowHandle);
			if(!wasWindowEnabled) {
				command = command | CodedUICommands.DisableControlAfterProcessing;
				CodedUINativeMethods.EnableWindow(windowHandle);
			}
			int result = CodedUINativeMethods.SendMessage(windowHandle, SharedMembers.DevExpressCodedUIMessage, new IntPtr(AssemblyInfo.VersionId), new IntPtr((int)command)).ToInt32();
			if(!wasWindowEnabled && result == 0)
				CodedUINativeMethods.DisableWindow(windowHandle);
			return result;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string ConvertParametersToStringForHandleViaReflection(object[] parameters, bool includeTypeNames) {
			string[] parametersAsString = new string[parameters.Length];
			string result = String.Empty;
			for(int i = 0; i < parameters.Length; i++) {
				parametersAsString[i] = ConvertToString(parameters[i]);
				if(includeTypeNames) {
					string typeName = parameters[i].GetType().FullName;
					typeName = typeName.Replace(".", ",");
					parametersAsString[i] += "!!" + typeName;
				}
				result += parametersAsString[i];
				if(i < parameters.Length - 1)
					result += "|";
			}
			return result;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetMethodStringForHandleViaReflection(string methodName, object[] parameters, bool includeTypeNames) {
			string parametersAsString = ConvertParametersToStringForHandleViaReflection(parameters, includeTypeNames);
			return methodName + "(" + parametersAsString + ")";
		}
		public static Type GetTypeFromName(string stringType) {
			if(stringType != null) {
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach(Assembly assembly in assemblies) {
					Type typeFromAssembly = assembly.GetType(stringType);
					if(typeFromAssembly != null) {
						return typeFromAssembly;
					}
				}
			}
			return typeof(object);
		}
		public static T ConvertFromString<T>(string valueAsString) {
			if(valueAsString != null) {
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
				if(converter != null && converter.CanConvertFrom(typeof(string)))
					try {
						return (T)converter.ConvertFrom(null, CultureInfo.InvariantCulture, valueAsString);
					}
					catch {
						try {
							return (T)converter.ConvertFrom(valueAsString);
						}
						catch {
							return default(T);
						}
					}
			}
			return default(T);
		}
		public static object ConvertFromString(string valueAsString, string typeName) {
			if(valueAsString != null) {
				Type valueType = GetTypeFromName(typeName);
				return ConvertFromString(valueAsString, valueType);
			}
			else
				return null;
		}
		public static object ConvertFromString(string valueAsString, Type valueType) {
			if(valueAsString != null && valueType != typeof(DBNull)) {
				if(valueType != null) {
					TypeConverter converter = TypeDescriptor.GetConverter(valueType);
					if(converter != null && converter.CanConvertFrom(typeof(string)))
						return converter.ConvertFrom(null, CultureInfo.InvariantCulture, valueAsString);
				}
				return valueAsString;
			}
			else
				return null;
		}
		public static string ConvertToString(object value) {
			if(value != null && value.GetType() != typeof(DBNull)) {
				Type valueType = value.GetType();
				TypeConverter converter = TypeDescriptor.GetConverter(valueType);
				if(converter != null && converter.CanConvertTo(typeof(string)))
					return (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(string));
				else
					return value.ToString();
			}
			else
				return null;
		}
		public static bool TryConvertToString(object value, out string convertedValue) {
			if(value == null || value.GetType() == typeof(DBNull)) {
				convertedValue = null;
				return true;
			}
			else {
				TypeConverter converter = TypeDescriptor.GetConverter(value.GetType());
				if(converter != null && converter.CanConvertTo(typeof(string))) {
					convertedValue = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(string));
					return true;
				}
				else { convertedValue = null; return false; }
			}
		}
		[Obsolete]
		public static ValueStruct GetValueStruct(object value) {
			return new ValueStruct(value);
		}
		public static object GetValue(ValueStruct value) {
			return ConvertFromString(value.ValueAsString, value.ValueTypeName);
		}
		public static T GetValue<T>(ValueStruct value) {
			return CastToType<T>(ConvertFromString(value.ValueAsString, value.ValueTypeName));
		}
		public static T CastToType<T>(object value) {
			if(value is T)
				return (T)value;
			else
				return default(T);
		}
	}
	[Serializable]
	public struct ValueStruct {
		public ValueStruct(object value) {
			ValueAsString = ValueTypeName = ErrorText = DisplayText = null;
			if(value == null)
				this.WasValueRead = true;
			else {
				this.ValueTypeName = value.GetType().FullName;
				this.WasValueRead = CodedUIUtils.TryConvertToString(value, out this.ValueAsString);
			}
		}
		public ValueStruct(string valueAsString, string valueTypeName) {
			ValueAsString = ValueTypeName = ErrorText = DisplayText = null;
			this.WasValueRead = true;
			this.ValueAsString = valueAsString;
			if(valueTypeName == null && valueAsString != null)
				this.ValueTypeName = typeof(string).FullName;
			else
				this.ValueTypeName = valueTypeName;
		}
		public bool WasValueRead;
		public string ValueAsString;
		public string ValueTypeName;
		public string ErrorText;
		public string DisplayText;
	}
	public enum Side {
		Unknown,
		Left,
		Right
	}
	[System.Security.SecuritySafeCritical]
	class CodedUINativeMethods {
		internal static IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam) {
			return UnsafeCodedUINativeMethods.SendMessage(hWnd, wMsg, wParam, lParam);
		}
		internal static bool IsWindowEnabled(IntPtr hWnd) {
			return UnsafeCodedUINativeMethods.IsWindowEnabled(hWnd);
		}
		internal static bool EnableWindow(IntPtr hWnd) {
			return UnsafeCodedUINativeMethods.EnableWindow(hWnd, true);
		}
		internal static bool DisableWindow(IntPtr hWnd) {
			return UnsafeCodedUINativeMethods.EnableWindow(hWnd, false);
		}
		internal static bool EnumChildWindows(IntPtr hwndParent, UnsafeCodedUINativeMethods.EnumWindowsDelegate lpEnumFunc, IntPtr lParam) {
			return UnsafeCodedUINativeMethods.EnumChildWindows(hwndParent, lpEnumFunc, lParam);
		}
	}
	class UnsafeCodedUINativeMethods {
		[DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
		internal static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		internal static extern bool EnableWindow(IntPtr hWnd, bool enable);
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		internal static extern bool IsWindowEnabled(IntPtr hWnd);
		internal delegate bool EnumWindowsDelegate(IntPtr windowHandle, IntPtr lParam);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsDelegate lpEnumFunc, IntPtr lParam);
	}
}
