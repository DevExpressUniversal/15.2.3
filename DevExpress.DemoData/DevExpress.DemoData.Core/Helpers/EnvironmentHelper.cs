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
using System.Collections.Generic;
using System.Text;
using System.Windows.Interop;
using System.Deployment.Application;
using DevExpress.Internal;
namespace DevExpress.DemoData.Helpers {
	public class EnvironmentParameter {
		public EnvironmentParameter(string key) {
			Key = key.ToLower();
			Values = new List<string>();
		}
		public string Key { get; private set; }
		public List<string> Values { get; private set; }
		public string Value { get { return Values.Count == 0 ? null : Values[0]; } }
	}
	public static class EnvironmentHelper {
		class ArgEqualityComparer : IEqualityComparer<string> {
			public bool Equals(string x, string y) {
				return string.Equals(x, y, StringComparison.CurrentCultureIgnoreCase);
			}
			public int GetHashCode(string obj) {
				return obj.ToLower().GetHashCode();
			}
		}
		const char OptionChar = '/';
		static IDictionary<string, EnvironmentParameter> args;
		public static bool IsSL {
			get {
				return false;
			}
		}
		public static bool IsXBAP {
			get {
				return BrowserInteropHelper.IsBrowserHosted;
			}
		}
		public static bool IsClickOnce {
			get {
				return DataDirectoryHelper.IsClickOnce;
			}
		}
		public static IDictionary<string, EnvironmentParameter> Args {
			get {
				if(args == null) {
					if(IsClickOnce)
						args = Parse(GetClickOnceParameters());
					else
						args = Parse(Environment.GetCommandLineArgs(), false);
				}
				return args;
			}
		}
		public static EnvironmentParameter GetParameter(string key) {
			if(key == null)
				key = string.Empty;
			EnvironmentParameter parameter;
			return Args.TryGetValue(key, out parameter) ? parameter : new EnvironmentParameter(key);
		}
		static IDictionary<string, EnvironmentParameter> CreateArgsContainer() {
			return new Dictionary<string, EnvironmentParameter>(new ArgEqualityComparer());
		}
		static EnvironmentParameter GetParameter(string key, IDictionary<string, EnvironmentParameter> parsedArgs) {
			EnvironmentParameter parameter;
			if(!parsedArgs.TryGetValue(key, out parameter)) {
				parameter = new EnvironmentParameter(key);
				parsedArgs.Add(key, parameter);
			}
			return parameter;
		}
		static IDictionary<string, EnvironmentParameter> Parse(List<string> pairs) {
			IDictionary<string, EnvironmentParameter> parsedArgs = CreateArgsContainer();
			foreach(string pair in pairs) {
				string[] parts = pair.Split('=');
				string key;
				string v;
				if(parts.Length == 1) {
					key = string.Empty;
					v = parts[0];
				} else {
					key = parts[0];
					v = parts[1];
				}
				EnvironmentParameter parameter = GetParameter(key, parsedArgs);
				parameter.Values.Add(v);
			}
			return parsedArgs;
		}
		static IDictionary<string, EnvironmentParameter> Parse(string[] args, bool includeFirst) {
			IDictionary<string, EnvironmentParameter> parsedArgs = CreateArgsContainer();
			bool onlyTargets = false;
			bool first = true;
			foreach(string arg in args) {
				if(first) {
					first = false;
					if(!includeFirst) continue;
				}
				ParseArg(arg, ref onlyTargets, parsedArgs);
			}
			return parsedArgs;
		}
		static void ParseArg(string arg, ref bool onlyTargets, IDictionary<string, EnvironmentParameter> parsedArgs) {
			arg = PrepareArg(arg);
			if(onlyTargets || !IsKey(arg))
				ParseTarget(arg, ref onlyTargets, parsedArgs);
			else if(IsOnlyTarget(arg))
				ParseOnlyTargets(arg, ref onlyTargets, parsedArgs);
			else
				ParseKey(arg, ref onlyTargets, parsedArgs);
		}
		static void ParseTarget(string arg, ref bool onlyTargets, IDictionary<string, EnvironmentParameter> parsedArgs) {
			EnvironmentParameter parameter = GetParameter(string.Empty, parsedArgs);
			parameter.Values.Add(arg);
		}
		static void ParseOnlyTargets(string arg, ref bool onlyTargets, IDictionary<string, EnvironmentParameter> parsedArgs) {
			onlyTargets = true;
		}
		static void ParseKey(string arg, ref bool onlyTargets, IDictionary<string, EnvironmentParameter> parsedArgs) {
			arg = arg.Substring(1);
			int i = arg.IndexOfAny(new char[] { '=', ':' });
			string v = i < 0 ? null : arg.SafeSubstring(i + 1);
			string key = i < 0 ? arg : arg.SafeRemove(i);
			EnvironmentParameter parameter = GetParameter(key, parsedArgs);
			parameter.Values.Add(v);
		}
		static bool IsKey(string arg) {
			return arg.Length > 1 && arg[0] == OptionChar;
		}
		static bool IsOnlyTarget(string arg) {
			return arg.Length == 2 && arg[0] == OptionChar && arg[1] == OptionChar;
		}
		static string PrepareArg(string arg) {
			StringBuilder ret = new StringBuilder();
			bool skip = false;
			bool insideQuotes = false;
			for(int i = 0; i < arg.Length; ++i) {
				if(skip || arg[i] != '\"') {
					skip = false;
					ret.Append(arg[i]);
					continue;
				}
				if(insideQuotes) {
					if(i + 1 < arg.Length && arg[i + 1] == '\"') {
						skip = true;
					} else {
						insideQuotes = false;
					}
				} else {
					insideQuotes = true;
				}
			}
			return ret.ToString();
		}
		static List<string> ParseClickOnceQueryString(string queryString) {
			return ParseClickOnceParameters(queryString.TrimStart('?'));
		}
		static List<string> ParseClickOnceParameters(string parametersString) {
			List<string> parameters = new List<string>();
			string[] parametersPairs = parametersString.Split('&');
			foreach(string parametersPair in parametersPairs) {
				parameters.Add(parametersPair);
			}
			return parameters;
		}
		static List<string> GetClickOnceParameters() {
			string[] args = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
			if(args != null && args.Length > 0) {
				string preparedString = args[0].Replace("&amp;", "&");
				int afterQSIndex = preparedString.IndexOf('?') + 1;
				if(afterQSIndex >= 1) {
					return ParseClickOnceQueryString(preparedString.Substring(afterQSIndex, preparedString.Length - afterQSIndex));
				}
			}
			return ParseClickOnceQueryString(GetClickOnceQueryString());
		}
		static string GetClickOnceQueryString() {
			string query;
			if(ApplicationDeployment.IsNetworkDeployed) {
				string activationUri = SafeClickOnceActivationUri;
				if(activationUri == null) {
					activationUri = ApplicationDeployment.CurrentDeployment.UpdateLocation.AbsoluteUri;
					string updateQuery = ApplicationDeployment.CurrentDeployment.UpdateLocation.Query;
					if(!string.IsNullOrEmpty(updateQuery))
					activationUri = activationUri.Replace(updateQuery, string.Empty);
				}
				query = activationUri == null ? string.Empty : new Uri(activationUri).Query;
			} else {
				query = string.Empty;
			}
			return query.Replace("&amp;", "&");
		}
		static string SafeClickOnceActivationUri {
			get {
				try {
					return ApplicationDeployment.CurrentDeployment.ActivationUri.ToString();
				} catch(Exception) {
					return null;
				}
			}
		}
	}
}
