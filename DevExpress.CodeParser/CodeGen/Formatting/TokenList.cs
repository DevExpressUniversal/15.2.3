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
using System.Globalization;
using System.Collections.Generic;
using System.Xml;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public class TokenList: List<FormattingTokenType>
  {
	public TokenList()
	{
	}
	public TokenList(params FormattingTokenType[] types)
	{
	  if (types == null)
		return;
	  foreach (FormattingTokenType type in types)
		Add(type);
	}
	public TokenList Clone()
	{
	  TokenList clone = new TokenList();
	  foreach (FormattingTokenType type in this)
		clone.Add(type);
	  return clone;
	}
	public void AddUnique(TokenList tokenList)
	{
	  if (tokenList == null)
		return;
	  foreach (FormattingTokenType tokenType in tokenList)
	  {
		if (Contains(tokenType))
		  continue;
		Add(tokenType);
	  }
	}
	public void AddUnique(params FormattingTokenType[] tokens)
	{
	  if (tokens == null)
		return;
	  foreach (FormattingTokenType tokenType in tokens)
	  {
		if (Contains(tokenType))
		  continue;
		Add(tokenType);
	  }
	}
	public bool ContainsAtLeastOne(TokenList tokens)
	{
	  if (tokens == null || tokens.Count == 0)
		return false;
	  foreach (FormattingTokenType tokenType in tokens)
		if (Contains(tokenType))
		  return true;
	  return false;
	}
	public TokenList And(TokenList tokens)
	{
	  TokenList result = Clone();
	  if (tokens == null || tokens.Count == 0)
		return result;
	  foreach (FormattingTokenType ft in tokens)
	  {
		if (result.Contains(ft))
		  continue;
		result.Add(ft);
	  }
	  return result;
	}
	public static TokenList Merge(TokenList fTokens, TokenList sTokens)
	{
	  if (fTokens == null || fTokens.Count == 0 || sTokens == null || sTokens.Count == 0)
		return new TokenList();
	  TokenList result = new TokenList();
	  foreach (FormattingTokenType ft in fTokens)
	  {
		if (sTokens.Contains(ft))
		  result.Add(ft);
	  }
	  return result;
	}
	public static TokenList Load(System.Xml.XmlNode ruleNode)
	{
#if SL
	  return null;
#else
	  if (ruleNode == null)
		return null;
	  TokenList result = new TokenList();
	  foreach (System.Xml.XmlNode tokenNode in ruleNode.SelectNodes("Token"))
	  {
		System.Xml.XmlAttribute attrName = tokenNode.Attributes["Name"];
		if (attrName == null)
		  continue;
		try
		{
		  FormattingTokenType token = (FormattingTokenType)Enum.Parse(typeof(FormattingTokenType), attrName.Value);
		  result.Add(token);
		}
		catch { }
	  }
	  return result;
#endif
	}
	public void Save(System.Xml.XmlNode ruleNode)
	{
#if SL
	  return;
#else
	  if (ruleNode == null)
		return;
	  foreach (FormattingTokenType type in this)
	  {
		XmlDocument doc = ruleNode.OwnerDocument;
		if (doc == null)
		  return;
		System.Xml.XmlNode tokenNode = doc.CreateNode(System.Xml.XmlNodeType.Element, "Token", String.Empty);
		ruleNode.AppendChild(tokenNode);
		System.Xml.XmlAttribute attrType = doc.CreateAttribute("Name");
		attrType.Value = type.ToString();
		tokenNode.Attributes.Append(attrType);
	  }
#endif
	}
  }
}
