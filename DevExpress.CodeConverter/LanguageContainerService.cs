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

using DevExpress.CodeParser;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
namespace DevExpress.CodeConverter {
	public class LanguageContainerService {
	  const string CsToVbTransformsAssemblyName = @"DevExpress.CsToVbTransforms, Version=13.2.0.0, Culture=neutral, PublicKeyToken=79868b8147b5eae4";
		Dictionary<string, CodeConverter> converters;
		CompositionContainer container;
		[ImportMany]
		IEnumerable<Lazy<ConvertRule, IConvert>> allRules;
		static LanguageContainerService instance;
		static LanguageContainerService Instance {
		  get {
			if (instance != null)
			  return instance;
			instance = new LanguageContainerService(null, GetCsToVbAssembly());
			return instance;
		  }
		}
		static Assembly GetCsToVbAssembly() {
			return typeof(LanguageContainerService).Assembly;
		}
		public static string ConvertCsToVb(string code, ConvertArguments arguments = null) {
		  return Convert(code, "CSharp", "Basic", arguments);
		}
		public static string Convert(string code, string from, string to, ConvertArguments arguments = null) {
		  CodeConverter converter = Instance.GetConverter(from, to);
		  if (converter == null)
			return null;
		  return converter.Convert(code, arguments);
		}
		private LanguageContainerService(string path, params Assembly[] assemblies) {
		  Bind(path, assemblies);
		}
		void Bind(string path, params Assembly[] assemblies) {
		  AggregateCatalog aggregateCatalog = new AggregateCatalog();
		  if (!string.IsNullOrEmpty(path))
			aggregateCatalog.Catalogs.Add(new DirectoryCatalog(path));
		  if (assemblies != null)
			foreach (Assembly asm in assemblies)
			  if (asm != null)
				aggregateCatalog.Catalogs.Add(new AssemblyCatalog(asm));
		  container = new CompositionContainer(aggregateCatalog);
		  container.ComposeParts(this);
		}
		CodeConverter GetConverter(string from, string to) {
			string converterDescription = CodeConverter.GetConverterDescritption(from, to);
			if (converters == null)
				converters = new Dictionary<string, CodeConverter>();
			if (converters.ContainsKey(converterDescription))
				return converters[converterDescription];
			CodeConverter converter = new CodeConverter(from, to, GetRules(from, to));
			converters.Add(converter.ToString(), converter);
			return converter;
		}
		RulesByMode GetRules(string from, string to) {
			RulesByMode result = new RulesByMode();
			foreach (Lazy<ConvertRule, IConvert> convert in allRules)
				if (convert.Metadata.From == from && convert.Metadata.To == to)
					result.Add(convert.Metadata.Mode, convert.Value);
			return result;
		}
	}
}
