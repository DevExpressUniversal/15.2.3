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
using System.Xml;
using System.Collections;
#if SL
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  using System.Reflection;
  public class XmlNodeComparer
  {
	SourceRange[] _Ranges;
	class XmlNodeComparerException : Exception
	{
	  public XmlNodeComparerException(string message)
		: base(message)
	  { }
	}
	public XmlNodeComparer()
	  : this(null)
	{
	}
	public XmlNodeComparer(SourceRange[] ranges)
	{
	  _Ranges = ranges;
	}
	#region FindNode
	System.Xml.XmlNode FindNode(XmlNodeList list, string name)
	{
	  int lCount = list.Count;
	  for (int i = 0; i < lCount; i++)
	  {
		System.Xml.XmlNode lNode = list[i];
		if (lNode.Name == name)
		  return lNode;
	  }
	  return null;
	}
	#endregion
	#region GetNodes
	ArrayList GetNodes(System.Xml.XmlNodeList list, string name)
	{
	  if (list == null)
		return new ArrayList();
	  ArrayList lResult = new ArrayList();
	  XmlNodeList lNodes = list;
	  int lCount = lNodes.Count;
	  for (int i = 0; i < lCount; i++)
	  {
		System.Xml.XmlNode lNode = lNodes[i];
		if (lNode.Name == name)
		  lResult.Add(lNode);
	  }
	  return lResult;
	}
	#endregion
	#region CheckProperties
	void CheckProperties(System.Xml.XmlNode node, LanguageElement element)
	{
	  XmlAttributeCollection lAttributes = node.Attributes;
	  int lCount = lAttributes.Count;
	  for (int i = 0; i < lCount; i++)
	  {
		CheckProperty(lAttributes[i].Name, lAttributes[i].Value, element);
	  }
	}
	#endregion
	#region CheckProperty
	void CheckProperty(string propertyName, string expectedValue, LanguageElement element)
	{
	  expectedValue = GetSpecialPropertyValue(propertyName, expectedValue);
	  PropertyInfo info = element.GetType().GetProperty(propertyName);
	  object propertyVal = info.GetValue(element, null);
	  string actualValue = propertyVal.ToString();
	  if (String.Compare(expectedValue, actualValue) != 0)
		throw new XmlNodeComparerException(String.Format("{0} values are not equal. Expected: {1} but was: {2}", propertyName, expectedValue, actualValue));
	}
	#endregion
	#region CheckNodeList
	void CheckNodeList(XmlNodeList xmlNodes, NodeList nodes)
	{
	  int lCount = xmlNodes.Count;
	  if (lCount != nodes.Count)
		throw new XmlNodeComparerException(String.Format("Node count is not equal. Expected: {0} but was: {1}", lCount, nodes.Count));
	  for (int i = 0; i < lCount; i++)
	  {
		System.Xml.XmlNode lNode = xmlNodes[i] as System.Xml.XmlNode;
		LanguageElement lElementNode = nodes[i] as LanguageElement;
		if (lNode == null && lElementNode == null)
		  continue;
		if (lNode == null || lElementNode == null)
		  throw new XmlNodeComparerException("Can't compare nodes, because one of them is null!");
		CheckInternal(lNode, lElementNode);
	  }
	}
	#endregion
	#region CheckDetailNodes
	bool CheckDetailNodes(System.Xml.XmlNode node, LanguageElement element)
	{
	  System.Xml.XmlNode lNode = FindNode(node.ChildNodes, "details");
	  if (lNode == null)
		return false;
	  CheckNodeList(lNode.ChildNodes, element.DetailNodes);
	  return true;
	}
	#endregion
	#region CheckNodes
	bool CheckNodes(System.Xml.XmlNode node, LanguageElement element)
	{
	  System.Xml.XmlNode lNode = FindNode(node.ChildNodes, "nodes");
	  if (lNode == null)
		return false;
	  CheckNodeList(lNode.ChildNodes, element.Nodes);
	  return true;
	}
	#endregion
	#region CheckInternal(System.Xml.XmlNode node, LanguageElement element)
	void CheckInternal(System.Xml.XmlNode node, LanguageElement element)
	{
	  CheckProperties(node, element);
	  if (node.ChildNodes != null && node.ChildNodes.Count > 0)
	  {
		bool detailsChecked = CheckDetailNodes(node, element);
		bool nodesChecked = CheckNodes(node, element);
		if(!detailsChecked && !nodesChecked)
		  throw new XmlNodeComparerException("Can't find <nodes> or <details>!");
	  }
	}
	#endregion
	#region CheckInternal(XmlDocument doc, LanguageElement element)
	void CheckInternal(XmlDocument doc, LanguageElement element)
	{
	  System.Xml.XmlNode lRoot = FindNode(doc.ChildNodes, "root");
	  CheckInternal(lRoot, element);
	}
	#endregion
	#region IsSpecialProperty
	protected virtual bool IsSpecialProperty(string name)
	{
	  if (name == null || name.Length == 0)
		return false;
	  return name.EndsWith("Range");
	}
	#endregion
	#region GetSpecialPropertyValue
	protected virtual string GetSpecialPropertyValue(string propertyName, string value)
	{
	  if (!IsSpecialProperty(propertyName))
		return value;
	  if (value == null || value.Length == 0)
		return String.Empty;
	  int index = value.IndexOf("#");
	  if (index < 0 || _Ranges == null)
		return value;
	  int rangeIndex = int.Parse(value.Substring(index + 1));
	  SourceRange range = _Ranges[rangeIndex];
	  return range.ToString();
	}
	#endregion
	#region Check(string xml, LanguageElement element, out string msg)
	public bool Check(string xml, LanguageElement element, out string msg)
	{
	  msg = String.Empty;
	  try
	  {
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(xml);
		CheckInternal(doc, element);
		return true;
	  }
	  catch (Exception ex)
	  {
		msg = ex.Message;
		return false;
	  }
	}
	#endregion
	#region Check(System.Xml.XmlNode node, LanguageElement element, out string msg)
	public bool Check(System.Xml.XmlNode node, LanguageElement element, out string msg)
	{
	  msg = String.Empty;
	  try
	  {
		CheckInternal(node, element);
		return true;
	  }
	  catch (Exception ex)
	  {
		msg = ex.Message;
		return false;
	  }
	}
	#endregion
	#region Check(XmlDocument doc, LanguageElement element, out string msg)
	public bool Check(XmlDocument doc, LanguageElement element, out string msg)
	{
	  msg = String.Empty;
	  try
	  {
		System.Xml.XmlNode lRoot = FindNode(doc.ChildNodes, "root");
		CheckInternal(lRoot, element);
		return true;
	  }
	  catch (Exception ex)
	  {
		msg = ex.Message;
		return false;
	  }
	}
	#endregion
  }
}
