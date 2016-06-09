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

using DevExpress.Utils.Design;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Reflection;
using System.ServiceModel;
using System.Text;
namespace DevExpress.MarkupUtils.Design {
	public static class XamlMarkupHelper {
		const string STR_GetMarkupOptions = "GetMarkupOptions";
		const string STR_XamlLanguageServiceGuid = "{C9164055-039B-4669-832D-F257BD5554D4}";
		const string STR_IXmlFormattingOptions = "IXmlFormattingOptions";
		const string STR_MicrosoftExpressionMarkupStringSourceChanger = "Microsoft.Expression.Markup.StringSourceChanger";
		const string STR_MicrosoftExpressionMarkupStringSourceReader = "Microsoft.Expression.Markup.StringSourceReader";
		const string STR_MicrosoftExpressionMarkupSourceModifier = "Microsoft.Expression.Markup.SourceModifier";
		const string STR_MicrosoftExpressionMarkupISourceChanger = "Microsoft.Expression.Markup.ISourceChanger";
		const string STR_MicrosoftExpressionMarkupXmlFormatter = "Microsoft.Expression.Markup.XmlFormatter";
		const string STR_Format = "Format";
		const string STR_Commit = "Commit";
		const string STR__builder = "_builder";
		public static string FormatXaml(string xaml) {
			try {
				if(string.IsNullOrEmpty(xaml))
					return null;
				Guid xamlLanguageServiceId = new Guid(STR_XamlLanguageServiceGuid);
				IVsLanguageInfo vsInfo = DTEHelper.Query<IVsLanguageInfo>(xamlLanguageServiceId);
				if(vsInfo == null)
					return null;
				Type xamlLanguageServiceType = vsInfo.GetType();
				MethodInfo mGetMarkupOptions = xamlLanguageServiceType.GetMethod(STR_GetMarkupOptions, BindingFlags.Instance | BindingFlags.NonPublic);
				if(mGetMarkupOptions == null)
					return null;
				object xmlFormattingOptions = mGetMarkupOptions.Invoke(vsInfo, new object[0]);
				if(xmlFormattingOptions == null)
					return null;
				Type iIXmlFormattingOptions = xmlFormattingOptions.GetType().GetInterface(STR_IXmlFormattingOptions);
				if(iIXmlFormattingOptions == null)
					return xaml;
				Assembly markupAssembly = iIXmlFormattingOptions.Assembly;
				Type stringSourceChangerType = markupAssembly.GetType(STR_MicrosoftExpressionMarkupStringSourceChanger);
				if(stringSourceChangerType == null)
					return null;
				ConstructorInfo cStringSourceChanger = stringSourceChangerType.GetConstructor(new Type[] { typeof(string) });
				if(cStringSourceChanger == null)
					return null;
				object ssc = cStringSourceChanger.Invoke(new object[] { xaml });
				if(ssc == null)
					return null;
				Type stringSourceReader = markupAssembly.GetType(STR_MicrosoftExpressionMarkupStringSourceReader);
				if(stringSourceReader == null)
					return null;
				ConstructorInfo cStringSourceReader = stringSourceReader.GetConstructor(new Type[] { typeof(string) });
				if(cStringSourceReader == null)
					return null;
				object ssr = cStringSourceReader.Invoke(new object[] { xaml });
				if(ssr == null)
					return null;
				Type sourceModifierType = markupAssembly.GetType(STR_MicrosoftExpressionMarkupSourceModifier);
				if(sourceModifierType == null)
					return null;
				ConstructorInfo cSourceModifier = sourceModifierType.GetConstructor(new Type[] { markupAssembly.GetType(STR_MicrosoftExpressionMarkupISourceChanger), markupAssembly.GetType("Microsoft.Expression.Markup.ISourceReader") });
				if(cSourceModifier == null)
					return null;
				object sourceModifier = cSourceModifier.Invoke(new object[] { ssc, ssr });
				if(sourceModifier == null)
					return null;
				Type xmlFormatter = markupAssembly.GetType(STR_MicrosoftExpressionMarkupXmlFormatter);
				if(xmlFormatter == null)
					return null;
				MethodInfo formatMethod = xmlFormatter.GetMethod(STR_Format, BindingFlags.Public | BindingFlags.Static, null,
					new Type[] { typeof(string), sourceModifierType, iIXmlFormattingOptions }, null);
				if(formatMethod == null)
					return null;
				formatMethod.Invoke(null, new object[] { xaml, sourceModifier, xmlFormattingOptions });
				MethodInfo mCommit = sourceModifierType.GetMethod(STR_Commit, BindingFlags.Instance | BindingFlags.Public);
				if(mCommit == null)
					return null;
				mCommit.Invoke(sourceModifier, new object[] { });
				FieldInfo _builder = stringSourceChangerType.GetField(STR__builder, BindingFlags.NonPublic | BindingFlags.Instance);
				StringBuilder result = _builder.GetValue(ssc) as StringBuilder;
				if(result == null)
					return null;
				return result.ToString();
			} catch {
			}
			return null;
		}
	}
}
