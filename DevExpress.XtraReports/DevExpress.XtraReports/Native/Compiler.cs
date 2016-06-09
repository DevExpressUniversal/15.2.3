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
using DevExpress.XtraReports.Serialization;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;
using System.Collections.Generic;
using System.Security.Policy;
using System.Linq;
namespace DevExpress.XtraReports.Native {
	public class ReferenceCollection : CollectionBase {
		#region static
		static bool IsValidExeFile(string path) {
			if(string.Compare(Path.GetExtension(path), ".exe", true) == 0 && File.Exists(path)) {
				using(BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))) {
					try {
						reader.BaseStream.Position = 0x3c;
						int peHeaderOffset = reader.ReadInt32();
						reader.BaseStream.Position = peHeaderOffset + 24;
						int magic = reader.ReadUInt16();
						if(magic != 0x10b && magic != 0x20b)
							return false;
						bool isPE32 = magic == 0x10b;
						reader.BaseStream.Position = peHeaderOffset + 24 + (isPE32 ? 92 : 108);
						int numberOfDataDirectories = reader.ReadInt32();
						if(numberOfDataDirectories < 15)
							return false;
						reader.BaseStream.Position = peHeaderOffset + 24 + (isPE32 ? 96 : 112) + 14 * 8;
						int rva = reader.ReadInt32();
						int size = reader.ReadInt32();
						return rva != 0 && size != 0;
					} catch {
						return false;
					}
				}
			}
			return true;
		}
		#endregion
		public string this[int index] { get { return (string)InnerList[index]; } }
		public void Add(string reference) {
			Add(reference, true);
		}
		public virtual void Add(string reference, bool checkFileExistance) {
			if(string.IsNullOrEmpty(reference))
				return;
			if(checkFileExistance) {
				reference = Path.GetFullPath(reference);
				if(!File.Exists(reference))
					return;
			}
			if(IndexOf(reference) >= 0)
				return;
			if(CompilerHelper.StringsEqual(Path.GetFileName(reference), "mscorlib.dll"))
				return;
			if(!IsValidExeFile(reference))
				return;
			List.Add(reference);
		}
		public void AddRange(string[] references) {
			foreach(string reference in references) {
				Add(reference);
			}
		}
		public void AddRange(string[] references, bool checkFileExistance) {
			foreach(string reference in references) {
				Add(reference, checkFileExistance);
			}
		}
		public int IndexOf(string reference) {
			string refFileName = Path.GetFileName(reference);
			for(int i = 0; i < Count; i++) {
				if(CompilerHelper.StringsEqual(refFileName, Path.GetFileName(this[i])))
					return i;
			}
			return -1;
		}
		public string[] ToStringArray() {
			string[] result = new string[Count];
			InnerList.CopyTo(result);
			return result;
		}
	}
	internal class DXReferenceCollection : ReferenceCollection {
		public override void Add(string reference, bool checkFileExistance) {
			base.Add(DXVersionPatcher.PatchPath(reference), checkFileExistance);
		}
	}
	public static class CompilerReferences {
		public static string[] GetFrameworkRefs() {
			List<string> refs = new List<string> {
				"System.dll",  
				"System.Data.dll",
				"System.Xml.dll",
				"System.Drawing.dll", 
				"System.Windows.Forms.dll",
				"System.Core.dll",
				"Microsoft.CSharp.dll"						 
			};
			return refs.ToArray();
		}
		static List<string> xtraReportsRefs = new List<string>();
		public static List<string> XtraReportsRefs {
			get {
				return xtraReportsRefs;
			}
		}
		static CompilerReferences() {
			XRSerializer.FillReferencedAssemblyLocations(xtraReportsRefs);
			xtraReportsRefs.Add(XRSerializer.GetXtraReportsAssemblyLocation());
			XRSerializer.FillDXAssemblyLocations(xtraReportsRefs, AssemblyInfo.SRAssemblyDataAccess, AssemblyInfo.SRAssemblyXpo);
		}
	}
	public class Compiler {
		static Hashtable assemblyCache = new Hashtable();
		static string EntryAssemblyLocation {
			get {
				Assembly assembly = Assembly.GetEntryAssembly();
				return assembly != null ? assembly.Location : string.Empty;
			}
		}
		IApplicationPathService applicationPathService = null;
		protected string source;
		public static bool IncludeAppDomainReferences = true;
		CompilerResults results;
		public IApplicationPathService ApplicationPathService {
			get { return applicationPathService; }
		}
		public CompilerResults Results {
			get { return results; }
		}
		public Compiler(string source, IApplicationPathService applicationPathService) {
			this.source = source;
			this.applicationPathService = applicationPathService != null ? applicationPathService : new DefaultApplicationPathService();
		}
		public Assembly GetCompiledAssembly(Evidence evidence) {
			string[] references = GetReferences();
			int sourceHashCode = ComputeSourceHash(source);
			if(assemblyCache.ContainsKey(sourceHashCode))
				return (Assembly)assemblyCache[sourceHashCode];
			results = CompileAssemblyFromSource(CreateCompilerParams(references, evidence), source);
			if(results.Errors.HasErrors)
				return null;
			assemblyCache[sourceHashCode] = results.CompiledAssembly;
			return results.CompiledAssembly;
		}
		protected internal virtual string[] GetReferencesFromSource() {
			return new string[0];
		}
		protected virtual void GetReferences(ReferenceCollection referenceCollection) {
			GetDomainReferences(referenceCollection);
			new CompilerHelper(this).GetLiveReferences(referenceCollection);
			referenceCollection.Add(EntryAssemblyLocation);
			referenceCollection.AddRange(CompilerReferences.XtraReportsRefs.ToArray());
			referenceCollection.AddRange(CompilerReferences.GetFrameworkRefs(), false);
		}
		protected virtual int ComputeSourceHash(string source) {
			return source.GetHashCode();
		}
		protected virtual CompilerParameters CreateCompilerParams(string[] references, Evidence evidence) {
			CompilerParameters compilerParams = new CompilerParameters();
			compilerParams.ReferencedAssemblies.AddRange(references);
			compilerParams.GenerateInMemory = true;
			if(evidence != null)
#pragma warning disable 0618
				compilerParams.Evidence = evidence;
#pragma warning restore 0618
			return compilerParams;
		}
		protected virtual CodeDomProvider GetCodeProvider() {
			return new CSharpCodeProvider();
		}
		CompilerResults CompileAssemblyFromSource(CompilerParameters parameters, string source) {
			lock(this.GetType()) {
				return CompileAssemblyFromSourceCore(parameters, source);
			}
		}
		protected virtual CompilerResults CompileAssemblyFromSourceCore(CompilerParameters parameters, string source) {
			return GetCodeProvider().CompileAssemblyFromSource(parameters, source);
		}
		string[] GetReferences() {
			ReferenceCollection referenceCollection = new DXReferenceCollection();
			GetReferences(referenceCollection);
			return referenceCollection.ToStringArray();
		}
		void GetDomainReferences(ReferenceCollection referencesCollection) {
			if(IncludeAppDomainReferences) {
				Assembly[] domainAssemblies = CompilerHelper.GetDomainAssemblies();
				IDictionary<string, string> referencesFromCurrentAppDomain = CompilerHelper.GetReferences(domainAssemblies);
				referencesCollection.AddRange(referencesFromCurrentAppDomain.Values.ToArray<string>());
			}
		}
	}
	public static class DXVersionPatcher {
		static string gacPath = null;
		static string GacPath {
			get {
				if(gacPath == null) {
					IDictionary envVar = Environment.GetEnvironmentVariables();
					string val = envVar["windir"] as string;
					gacPath = !string.IsNullOrEmpty(val) ? Path.Combine(val, @"Microsoft.Net\assembly\") : string.Empty;
				}
				return gacPath;
			}
		}
		public static string Patch(string s, string newVersion) {
			System.Text.RegularExpressions.Group group = GetVersionGroup(s);
			return group.Success ? s.Replace(group.Value, newVersion) : s;
		}
		public static string PatchPath(string s) {
			System.Text.RegularExpressions.Group group = GetVersionGroup(s);
			if(!group.Success)
				return s;
			if(!string.IsNullOrEmpty(GacPath) && s.StartsWith(GacPath, StringComparison.OrdinalIgnoreCase)) {
				string s2 = s.Replace(group.Value, AssemblyInfo.VSuffixWithoutSeparator);
				s2 = System.Text.RegularExpressions.Regex.Replace(s2, group.Value.Substring(1) + "\\.\\d+\\.\\d+", AssemblyInfo.Version);
				return s2;
			}
			string s3 = s.Remove(group.Index, group.Length);
			return s3.Insert(group.Index, AssemblyInfo.VSuffixWithoutSeparator);
		}
		public static string GetDXVersion(string s) {
			System.Text.RegularExpressions.Group group = GetVersionGroup(s);
			return group.Value;
		}
		static System.Text.RegularExpressions.Group GetVersionGroup(string s) {
			System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(s.ToLower(), @"devexpress[\w\.]+(?<version>v\d+\.\d+)");
			return m.Groups["version"];
		}
	}
	class CompilerHelper {
		static bool IsCompactFrameworkAssembly(Assembly asm) {
			return asm.FullName.IndexOf("969db8053d3322ac") > 0;
		}
		static string PatchDXVersion(string path) {
			string fileName = Path.GetFileName(path);
			string directoryName = Path.GetDirectoryName(path);
			string patchedFileName = DXVersionPatcher.Patch(fileName, AssemblyInfo.VSuffixWithoutSeparator);
			return Path.Combine(directoryName, fileName);
		}
		string[] GetLiveReferences(string[] references, IDictionary<string, string> referenceContainer) {
			List<string> result = new List<string>();
			foreach(string reference in references) {
				if(string.IsNullOrEmpty(reference))
					continue;
				string patchedReference = PatchDXVersion(reference);
				string path;
				if(referenceContainer.TryGetValue(patchedReference, out path)) {
					result.Add(path);
					continue;
				}
				path = ApplicationPathService.GetFullPath(Path.GetFileName(patchedReference));
				if(!String.IsNullOrEmpty(path)) {
					result.Add(path);
					continue;
				}
				if(File.Exists(patchedReference)) {
					result.Add(patchedReference);
					continue;
				}
			}
			return result.ToArray();
		}
		public static bool StringsEqual(string s1, string s2) {
			return String.Compare(s1, s2, true) == 0;
		}
		public static IDictionary<string, string> GetReferences(Assembly[] assemblies) {
			Dictionary<string, string> result = new Dictionary<string, string>(assemblies.Length);
			foreach(Assembly assembly in assemblies) {
				try {
					if(string.IsNullOrEmpty(assembly.Location)) continue;
					string key = assembly.GetName().Name;
					string location;
					if(result.TryGetValue(key, out location)) {
						DateTime dateTime1 = File.GetLastWriteTime(location);
						DateTime dateTime2 = File.GetLastWriteTime(assembly.Location);
						if(dateTime1 > dateTime2) continue;
					}
					result[key] = assembly.Location;
				} catch { }
			}
			return result;
		}
		public static Assembly[] GetDomainAssemblies() {
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			ArrayList result = new ArrayList(assemblies.Length);
			foreach(Assembly asm in assemblies) {
				try {
					if(asm is System.Reflection.Emit.AssemblyBuilder || asm.GetType().FullName == "System.Reflection.Emit.InternalAssemblyBuilder")
						continue;
					string ignore = asm.Location;
					if(!IsCompactFrameworkAssembly(asm))
						result.Add(asm);
				}
				catch {
				}
			}
			return (Assembly[])result.ToArray(typeof(Assembly));
		}
		Compiler compiler;
		IApplicationPathService ApplicationPathService { get { return compiler.ApplicationPathService; } }
		public CompilerHelper(Compiler compiler) {
			this.compiler = compiler;
		}
		public void GetLiveReferences(ReferenceCollection referenceCollection) {
			Assembly[] domainAssemblies = GetDomainAssemblies();
			string[] references = compiler.GetReferencesFromSource();
			string[] liveReferences = GetLiveReferences(references, GetReferences(domainAssemblies));
			referenceCollection.AddRange(liveReferences);
		}
	}
}
