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
using System.Linq;
using System.Text;
using System.Windows.Markup;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.MetaData;
using DevExpress.Design.Mvvm;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm {
	public interface IXamlNamespaceDeclaration {
		string GetXaml();
		string Prefix { get; }
		string Definition { get; }
		string ClrNamespace { get; }
	}
	public class XamlNamespaces : IEnumerable<IXamlNamespaceDeclaration> {
		public const string xmlns_presentation06 = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
		public const string xmlns_presentation09 = "http://schemas.microsoft.com/netfx/2009/xaml/presentation";
		public const string xmlns_xaml = "http://schemas.microsoft.com/winfx/2006/xaml";
		public const string xmlns_markup_compatibility = "http://schemas.openxmlformats.org/markup-compatibility/2006";
		public const string xmlns_blend = "http://schemas.microsoft.com/expression/blend/2008";
		Dictionary<string, XamlNamespaceDeclarationBase> dictionary;
		List<XamlNamespaceDeclaration> devexpressXamlNamespaces;
		public XamlNamespaces() {
			InitDevexpressXamlNamespaces();
			dictionary = new Dictionary<string, XamlNamespaceDeclarationBase>();
			Add(xmlns_xaml, "x");
			Add(xmlns_markup_compatibility, "mc");
			Add(xmlns_blend, "d");
		}
		void InitDevexpressXamlNamespaces() {
			devexpressXamlNamespaces = new List<XamlNamespaceDeclaration>();
			devexpressXamlNamespaces.AddRange(new XamlNamespaceDeclaration[] {
				new XamlNamespaceDeclaration(XmlNamespaceConstants.UtilsNamespaceDefinition, XmlNamespaceConstants.UtilsPrefix),
				new XamlNamespaceDeclaration(XmlNamespaceConstants.RibbonNamespaceDefinition, XmlNamespaceConstants.RibbonPrefix),
				new XamlNamespaceDeclaration(XmlNamespaceConstants.LayoutControlNamespaceDefinition, XmlNamespaceConstants.LayoutControlPrefix),
				new XamlNamespaceDeclaration(XmlNamespaceConstants.BarsNamespaceDefinition, XmlNamespaceConstants.BarsPrefix),
				new XamlNamespaceDeclaration(XmlNamespaceConstants.DockingNamespaceDefinition, XmlNamespaceConstants.DockingPrefix),
				new XamlNamespaceDeclaration(XmlNamespaceConstants.NavBarNamespaceDefinition, XmlNamespaceConstants.NavBarPrefix),
				new XamlNamespaceDeclaration(XmlNamespaceConstants.GridNamespaceDefinition, XmlNamespaceConstants.GridPrefix),
				new XamlNamespaceDeclaration(XmlNamespaceConstants.EditorsNamespaceDefinition, XmlNamespaceConstants.EditorsPrefix),
				new XamlNamespaceDeclaration(XmlNamespaceConstants.MvvmNamespaceDefinition, XmlNamespaceConstants.MvvmPrefix),
				new XamlNamespaceDeclaration(XmlNamespaceConstants.WindowsUINamespaceDefinition, XmlNamespaceConstants.WindowsUIPrefix),
				new XamlNamespaceDeclaration(XmlNamespaceConstants.WindowsUINavigationNamespaceDefinition, XmlNamespaceConstants.WindowsUINavigationPrefix),
				new XamlNamespaceDeclaration(XmlNamespaceConstants.WindowsUIInternalNamespaceDefinition, XmlNamespaceConstants.WindowsUIPrefix + "i"),
				new XamlNamespaceDeclaration(XmlNamespaceConstants.NavigationNamespaceDefinition, XmlNamespaceConstants.NavigationPrefix),
				new XamlNamespaceDeclaration(XmlNamespaceConstants.NavigationInternalNamespaceDefinition, XmlNamespaceConstants.NavigationInternalPrefix),
				new XamlNamespaceDeclaration(XmlNamespaceConstants.ReportDesignerExtensionsNamespaceDefinition, XmlNamespaceConstants.ReportDesignerExtensionsPrefix),
			});
		}
		internal XamlNamespaceDeclarationBase Add(string xamlNamespace, string prefix) {
			if(string.IsNullOrEmpty(xamlNamespace))
				return null;
			XamlNamespaceDeclaration declaration = new XamlNamespaceDeclaration(xamlNamespace, prefix);
			return Register(declaration);
		}
		public XamlNamespaceDeclarationBase Register(XamlNamespaceDeclarationBase declaration) {
			XamlNamespaceDeclarationBase existing = GetExisting(declaration.Definition);
			if(existing != null)
				return existing;
			if(declaration.Definition == xmlns_presentation06 && this.Contains(xmlns_presentation09))
				return GetExisting(xmlns_presentation09);
			if(declaration.Definition == xmlns_presentation09 && this.Contains(xmlns_presentation06))
				return GetExisting(xmlns_presentation06);
			XamlNamespaceDeclarationBase devexpressXamlNamespace = devexpressXamlNamespaces.FirstOrDefault(d => d.Definition == declaration.Definition);
			if(devexpressXamlNamespace != null && declaration != devexpressXamlNamespace)
				declaration = devexpressXamlNamespace;
			dictionary[declaration.Definition] = declaration;
			SetUniquePrefix(declaration, declaration.Prefix);
			return declaration;
		}
		XamlNamespaceDeclarationBase GetExisting(string definition) {
			XamlNamespaceDeclarationBase existing = null;
			if(dictionary.TryGetValue(definition, out existing))
				return existing;
			return null;
		}
		bool Contains(string definition) {
			return GetExisting(definition) != null;
		}
		public string SetUniquePrefix(XamlNamespaceDeclarationBase declaration, string basePrefix) {
			if(declaration == null)
				return string.Empty;
			if(devexpressXamlNamespaces.Contains(declaration) || declaration.Definition == xmlns_presentation06 || declaration.Definition == xmlns_presentation09)
				return declaration.Prefix;
			if(basePrefix == null)
				basePrefix = "ns";
			int index = 0;
			string prefix = basePrefix;
			while(dictionary.Values.Any(d => d != declaration && d.Prefix == prefix) || devexpressXamlNamespaces.Any(d => d.Prefix == prefix)) {
				index++;
				prefix = basePrefix + index;
			}
			declaration.Prefix = prefix;
			return declaration.Prefix;
		}
		public string GetPrefix(string definition) {
			XamlNamespaceDeclarationBase declaration = GetExisting(definition);
			return declaration != null ? declaration.Prefix : string.Empty;
		}
		public XamlNamespaceDeclarationBase GetNamespaceDeclaration(Type type, bool isLocal) {
			XamlNamespaceDeclarationBase declaration = null;
			if(!isLocal)
				declaration = XamlNamespaceDeclaration.Create(type);
			if(declaration == null)
				declaration = new XamlClrNamespaceDeclaration(type.Namespace, type.Assembly.GetName().Name) { IsLocal = isLocal };
			return this.Register(declaration);
		}
		string GetPrefix(Type type, bool isLocal) {
			XamlNamespaceDeclarationBase declaration = GetNamespaceDeclaration(type, isLocal);
			return declaration.Prefix;
		}
		public string GetPrefix(Type type) {
			bool isLocal = false;
			if(MetaDataServices.IsInitialized) {
				IDXTypeInfo typeInfo = MetaDataServices.SolutionTypesProvider.ActiveProjectTypes.GetExistingOrCreateNew(type);
				isLocal = typeInfo != null && typeInfo.Assembly.IsProjectAssembly;
			}
			return GetPrefix(type, isLocal);
		}
		public string GetXaml() {
			if(!this.Contains(xmlns_presentation06) && !this.Contains(xmlns_presentation09))
				this.Add(xmlns_presentation09, "");
			StringBuilder stringBuilder = new StringBuilder();
			List<XamlNamespaceDeclarationBase> list = new List<XamlNamespaceDeclarationBase>(this.dictionary.Values);
			list.Sort((x, y) => {
				XamlNamespaceDeclaration xdX = x as XamlNamespaceDeclaration;
				XamlNamespaceDeclaration xdY = y as XamlNamespaceDeclaration;
				if(xdX != null && xdY != null) {
					if(xdX.Prefix != null && xdY.Prefix != null && xdX.Prefix.Length != xdY.Prefix.Length)
						return (xdX.Prefix.Length > xdY.Prefix.Length).CompareTo(true);
					return (string.Compare(xdX.Prefix, xdY.Prefix, true));
				}
				if(x is XamlClrNamespaceDeclaration && y is XamlClrNamespaceDeclaration)
					return 0;
				if(x is XamlNamespaceDeclaration)
					return -1;
				return 1;
			});
			foreach(XamlNamespaceDeclarationBase declaration in list)
				stringBuilder.AppendLine(declaration.GetXaml());
			return stringBuilder.ToString();
		}
		public IEnumerable<IXamlNamespaceDeclaration> ClrDeclarations {
			get {
				return dictionary.Values.Where(el => el is XamlClrNamespaceDeclaration).Where(el => el != null);
			}
		}
		public IEnumerable<IXamlNamespaceDeclaration> XamlDeclarations {
			get {
				return dictionary.Values.Where(el => el is XamlNamespaceDeclaration).Where(el => el != null);
			}
		}
		public void AddDevExpressXamlNamespaces(string[] definitions) {
			if(definitions == null || definitions.Length == 0)
				return;
			foreach(string definition in definitions) {
				XamlNamespaceDeclaration declaration = this.devexpressXamlNamespaces.FirstOrDefault(ns => ns.XamlNamespace == definition);
				if(declaration == null) {
#if DEBUGTEST
					throw new InvalidXamlNamespaceDeclarationException(definition);
#else
					continue;
#endif
				}
				this.Register(declaration);
			}
		}
		public IEnumerator<IXamlNamespaceDeclaration> GetEnumerator() {
			return dictionary.Values.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return dictionary.Values.GetEnumerator();
		}
	}
	public class InvalidXamlNamespaceDeclarationException : Exception {
		public InvalidXamlNamespaceDeclarationException(string message, Exception innerException = null) : base(message, innerException) { }
	}
	public abstract class XamlNamespaceDeclarationBase : IXamlNamespaceDeclaration {
		protected const string STR_xmlns = "xmlns";
		public abstract string Definition { get; }
		public abstract string GetXaml();
		public virtual string Prefix { get; set; }
		public virtual string ClrNamespace { get; set; }
		public override string ToString() {
			return Definition;
		}
	}
	public class XamlNamespaceDeclaration : XamlNamespaceDeclarationBase {
		public XamlNamespaceDeclaration(string xamlNamespace, string prefix) {
			this.XamlNamespace = xamlNamespace;
			this.Prefix = prefix;
		}
		public override string Definition {
			get { return XamlNamespace; }
		}
		public string XamlNamespace { get; private set; }
		public override string GetXaml() {
			if(string.IsNullOrEmpty(Prefix))
				return STR_xmlns + "=" + "\"" + Definition + "\"";
			else
				return STR_xmlns + ":" + Prefix + "=" + "\"" + Definition + "\"";
		}
		public static XamlNamespaceDeclaration Create(Type type) {
			try {
				XmlnsDefinitionAttribute definition = type.Assembly.GetCustomAttributes(typeof(XmlnsDefinitionAttribute), false).Cast<XmlnsDefinitionAttribute>().FirstOrDefault(x => x.ClrNamespace == type.Namespace);
				if(definition == null)
					return null;
				if(definition.XmlNamespace == XamlNamespaces.xmlns_presentation06 || definition.XmlNamespace == XamlNamespaces.xmlns_presentation09)
					return new XamlNamespaceDeclaration(definition.XmlNamespace, string.Empty);
				XmlnsPrefixAttribute prefix = type.Assembly.GetCustomAttributes(typeof(XmlnsPrefixAttribute), false).Cast<XmlnsPrefixAttribute>().FirstOrDefault(x => x.XmlNamespace == definition.XmlNamespace);
				if(prefix != null)
					return new XamlNamespaceDeclaration(definition.XmlNamespace, prefix.Prefix) { ClrNamespace = definition.ClrNamespace };
			}
			catch { }
			return null;
		}
	}
	public class XamlClrNamespaceDeclaration : XamlNamespaceDeclarationBase {
		string prefix;
		public XamlClrNamespaceDeclaration(string clrNamespace, string assemblyName) :
			this(clrNamespace, assemblyName, false) {
		}
		public XamlClrNamespaceDeclaration(string clrNamespace, string assemblyName, bool isLocal) {
			this.ClrNamespace = clrNamespace;
			this.AssemblyName = assemblyName;
			this.IsLocal = isLocal;
		}
		public string AssemblyName { get; set; }
		public bool IsLocal { get; internal set; }
		public override string Definition {
			get {
				return string.Format("\"clr-namespace:{0};assembly={1}\"", ClrNamespace, AssemblyName);
			}
		}
		public override string GetXaml() {
			if(IsLocal)
				return string.Format("{0}:{1}=\"clr-namespace:{2}\"", STR_xmlns, Prefix, ClrNamespace);
			return string.Format("{0}:{1}=\"clr-namespace:{2};assembly={3}\"", STR_xmlns, Prefix, ClrNamespace, AssemblyName);
		}
		public string GetPrefixFromNamespace() {
			string[] components = ClrNamespace.ToLowerInvariant().Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
			return components[components.Length - 1];
		}
		public override string Prefix {
			get {
				if(string.IsNullOrEmpty(prefix))
					prefix = GetPrefixFromNamespace();
				return prefix;
			}
			set {
				prefix = value;
			}
		}
	}
}
