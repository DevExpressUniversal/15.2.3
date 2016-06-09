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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class XmlSignatureBuilder
	{
	const string STR_OpenTag = "<";
	const string STR_CloseTag = ">";
	const string STR_Asp = "asp";	
	const string STR_Comma = ",";
	const string STR_Space = " ";
	const string STR_Equal = "=";
		private XmlSignatureBuilder()
		{
		}
	static void AppendArtibutes(StringBuilder builder, HtmlAttributeCollection attributes)
	{
	  if (attributes == null || attributes.Count == 0)
		return;
	  int count = attributes.Count;
	  for (int i = 0; i < count; i++)
	  {
		string signature = GetXmlAttributeSignature(attributes[i]);
		builder.Append(signature);
		if (i < count - 1)
		  builder.Append(STR_Comma + STR_Space);		
	  }
	}
	static string GetXmlNodeSignature(BaseXmlNode element)
	{
	  return String.Format("{0}{1} {2}{3}", STR_OpenTag, element.ElementType, element.Name, STR_CloseTag);	  
	}
	static string GetHtmlElementSignature(HtmlElement element)
	{
	  StringBuilder builder = new StringBuilder();
	  builder.Append(STR_OpenTag);
	  builder.Append(element.ElementType);
	  builder.Append(STR_Space);
	  builder.Append(element.Name + STR_Space);
	  AppendArtibutes(builder, element.Attributes);
	  builder.Append(STR_CloseTag);
	  return builder.ToString();
	}
	static string GetXmlAttributeSignature(XmlAttribute element)
	{
	  return String.Format("{0}{1}{2}", element.Name.ToLower(), STR_Equal, element.Value);
	}
	static string GetXmlTextSignature(XmlText element)
	{
	  return String.Format("{0}{1}{2}", STR_OpenTag, element.ElementType, STR_CloseTag);
	}
	static string DefaultXmlSignature(XmlNode element)
	{
	  return String.Format("{0}{1} {2}{3}", STR_OpenTag, element.ElementType, element.Name, STR_CloseTag);
	}
	public static string GetSignature(XmlNode element)
	{
	  if (element == null)
		return String.Empty;
	  if (element is BaseXmlNode)
		return GetXmlNodeSignature((BaseXmlNode)element);	  
	  if (element is HtmlElement)
		return GetHtmlElementSignature((HtmlElement)element);
	  if (element is XmlAttribute)
		return GetXmlAttributeSignature((XmlAttribute)element);
	  if (element is XmlText)
		return GetXmlTextSignature((XmlText)element);
	  return DefaultXmlSignature(element);
	}
	public static string GetSignature(BaseCssElement element)
	{
	  if (element == null)
		return String.Empty;
	  return String.Format("{0} {1}{2}", element.ElementType, element.Name, element.Index); 
	}
	}
}
