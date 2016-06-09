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
using System.Text;
using System.Collections;
using DevExpress.XtraReports.UI;
using System.IO;
using System.Reflection;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Serialization;
using System.ComponentModel.Design;
using Microsoft.CSharp;
using System.Collections.Generic;
namespace DevExpress.XtraReports.Native {
	internal class ReportCompiler : Compiler {
		DevExpress.XtraReports.Serialization.XRTypeInfo typeInfo;
		public ReportCompiler(string source) : base(source, null) {
			typeInfo = DevExpress.XtraReports.Serialization.XRTypeInfo.Deserialize(source);
			int codeStart = GetMatchingLineStart(source);
			string modifiedSource = codeStart > 0 ? source.Substring(codeStart) : source;
			if(string.IsNullOrEmpty(typeInfo.Version) || new Version(typeInfo.Version) < new Version(12, 2))
				modifiedSource = ReportConverter_v12_2.ConvertSource(modifiedSource);
			this.source = modifiedSource;
		}
		protected internal override string[] GetReferencesFromSource() {
			return typeInfo.GetReferences();
		}
		Dictionary<string, string> GetResources() {
			Dictionary<string, string> resources = new Dictionary<string, string>();
			foreach(var resource in typeInfo.Resources)
				resources.Add(resource.Name, resource.Content);
			return resources;
		}
		internal static XtraReport Compile(Stream stream) {
			if(stream.CanSeek)
				stream.Seek(0, SeekOrigin.Begin);
			return Compile(new StreamReader(stream).ReadToEnd(), null);
		}
		internal static XtraReport Compile(string streamString, IServiceProvider serviceProvider) {
			LoadAppAssemblyInDomain(serviceProvider);
			ReportCompiler compiler = new ReportCompiler(streamString);
			Assembly compiledAssembly = compiler.GetCompiledAssembly(null);
			if(compiledAssembly == null)
				throw new XRSerializationException(compiler.Results.Errors);
			try {
				XRResourceManager.RegisterResourceStrings(compiler.GetResources());
				return GetSerializedReport(compiledAssembly);
			} finally {
				XRResourceManager.ClearResourceStrings();
			}
		}
		static int GetMatchingLineStart(string streamString) {
			int index = 0;
			int length = streamString.Length;
			char next;
			while(true) {
				index = streamString.IndexOfAny(new char[] { '\r', '\n' }, index);
				if(index < 0) return -1;
				do {
					index++;
					if(index >= length)
						return -1;
					next = streamString[index];
				} while(next == '\n' || next == '\r');
				if(next != '/' || index + 2 >= length || streamString[index + 1] != '/' || streamString[index + 2] != '/')
					return index;
			}
		}
#if DEBUGTEST
		public static int Test_GetMatchingLineStart(string streamString) {
			return GetMatchingLineStart(streamString);
		}
#endif
		static void LoadAppAssemblyInDomain(IServiceProvider serviceProvider) {
			if(serviceProvider == null)
				return;
			IDesignerHost designerHost = serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(designerHost != null)
				designerHost.GetType(designerHost.RootComponentClassName);
		}
		static XtraReport GetSerializedReport(Assembly assembly) {
			Type[] types = assembly.GetTypes();
			foreach(Type type in types) {
				if(typeof(XtraReport).IsAssignableFrom(type) && type.Namespace.Equals(XtraReport.SerializationNamespace)) {
					ConstructorInfo ci = type.GetConstructor(Type.EmptyTypes);
					return ci.Invoke(new object[] { }) as XtraReport;
				}
			}
			return null;
		}
	}
}
