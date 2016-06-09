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
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
#if SL
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  internal class PathPart
  {
	string _Name;
	LanguageElement _Element;
	public PathPart(string name, LanguageElement element)
	{
	  _Name = name;
	  _Element = element;
	}
	public string Name
	{
	  get
	  {
		return _Name;
	  }
	}
	public LanguageElement Element
	{
	  get
	  {
		return _Element;
	  }
	}
  }
  internal class ElementLocation
  {
	protected ElementLocation()
	{
	}
	static bool IsEmptyDocText(LanguageElement element)
	{
	  XmlText xmlText = element as XmlText;
	  if (xmlText == null)
		return false;
	  string text = xmlText.Text.Trim();
	  return String.IsNullOrEmpty(text);
	}
	static int GetClearElementIndex(LanguageElement element)
	{
	  LanguageElement parent = element.Parent;
	  if (!(parent is XmlNode))
		return element.Index;
	  int emptyElementsCount = 0;
	  int count = parent.NodeCount;
	  for (int i = 0; i < count; i++)
	  {
		LanguageElement current = parent.Nodes[i] as LanguageElement;
		if (current == element)
		  break;
		if (IsEmptyDocText(current))
		  emptyElementsCount++;
	  }
	  return element.Index - emptyElementsCount;
	}
	static System.Xml.XmlNode CreateXmlNode(System.Xml.XmlDocument doc, string name, bool isDetailNode, int index, string signature)
	{
	  System.Xml.XmlNode result = doc.CreateNode(System.Xml.XmlNodeType.Element, name, string.Empty);
	  System.Xml.XmlAttribute attr = doc.CreateAttribute("isDetail");
	  attr.Value = isDetailNode.ToString();
	  result.Attributes.Append(attr);
	  attr = doc.CreateAttribute("index");
	  attr.Value = index.ToString();
	  result.Attributes.Append(attr);
	  attr = doc.CreateAttribute("signature");
	  attr.Value = signature;
	  result.Attributes.Append(attr);
	  return result;
	}
	static System.Xml.XmlNode CreateXmlNodeForFile(System.Xml.XmlDocument doc, SourceFile file)
	{
	  if (file == null)
		return null;
	  System.Xml.XmlNode result = doc.CreateNode(System.Xml.XmlNodeType.Element, "file", string.Empty);
	  System.Xml.XmlAttribute attr = doc.CreateAttribute("path");
	  attr.Value = file.FilePath;
	  result.Attributes.Append(attr);
	  return result;
	}
	static bool CanBeMoved(LanguageElement element)
	{
	  if ((element is IMethodElement) || 
		(element is IPropertyElement) || 
		(element is IDelegateElement))
		return true;
	  return false;
	}
	static System.Xml.XmlNode GetXmlNodeForElement(System.Xml.XmlDocument doc, string nodeName, LanguageElement element)
	{
	  if (String.IsNullOrEmpty(nodeName) || element == null)
		return null;
	  System.Xml.XmlNode currentXmlNode;
	  bool isDetailNode = element.IsDetailNode;
	  int index = -1;
	  if (!CanBeMoved(element))
		index = GetClearElementIndex(element);
	  string signature = SignatureBuilder.GetSignature(element);
	  currentXmlNode = CreateXmlNode(doc, nodeName, isDetailNode, index, signature);
	  return currentXmlNode;
	}
	static System.Xml.XmlNode CreateXmlNodeForCurrent(System.Xml.XmlDocument doc, PathPart part)
	{
	  if (part.Name == "file")
		return CreateXmlNodeForFile(doc, part.Element as SourceFile);
	  return GetXmlNodeForElement(doc, part.Name, part.Element);	  
	}
	static void CollectParents(ArrayList parents, LanguageElement element)
	{
	  LanguageElement current = element;
	  while (current != null)
	  {
		string name = "scope";
		if (current == element)
		  name = "element";
		else if (current is SourceFile)
		  name = "file";
		parents.Add(new PathPart(name, current));
		if (current is SupportElement)
		{
		  SupportElement supportElement = (SupportElement)current;
		  LanguageElement target = supportElement.TargetNode;
		  if (target != null && CanBeMoved(target))
		  {
			parents.Add(new PathPart("target", target));
			current = target;
		  }
		}
		if (current is SourceFile)
		  break;
		current = current.Parent;
	  }
	}
	static void BuildLocation(System.Xml.XmlDocument doc, LanguageElement element)
	{
	  ArrayList parents = new ArrayList();
	  CollectParents(parents, element);
	  if (parents == null || parents.Count == 0)
		return;
	  System.Xml.XmlNode xmlNode = null;
	  int count = parents.Count;
	  for (int i = 0; i < parents.Count; i++)
	  {
		System.Xml.XmlNode currentXmlNode = CreateXmlNodeForCurrent(doc, parents[i] as PathPart);
		if (currentXmlNode == null)
		  return;
		if (xmlNode != null)
		  currentXmlNode.AppendChild(xmlNode);	  
		xmlNode = currentXmlNode;
	  }
	  if (xmlNode != null)
		doc.AppendChild(xmlNode);
	}
	static bool IsTarget(System.Xml.XmlNode xmlNode)
	{
	  return xmlNode != null && String.Compare(xmlNode.Name, "target", StringComparison.CurrentCulture) == 0;
	}
	static bool IsFile(System.Xml.XmlNode xmlNode)
	{
	  return xmlNode != null && String.Compare(xmlNode.Name, "file", StringComparison.CurrentCulture) == 0;
	}
	static SourceFile LocateFile(IProjectElement proj, System.Xml.XmlNode xmlNode)
	{
	  if (xmlNode == null || !IsFile(xmlNode))
		return null;
	  string path = xmlNode.Attributes["path"].Value;
	  if (proj != null)
		return proj.FindDiskFile(path) as SourceFile;
	  ISolutionElement activeSolution = StructuralParserServicesHolder.ActiveSolution;
	  if (activeSolution == null)
		return null;
	  foreach (IProjectElement project in activeSolution.Projects)
	  {
		ISourceFile sourceFile = project.FindDiskFile(path);
		if (sourceFile != null)
		  return sourceFile as SourceFile;
	  }
	  return null;
	}
	static LanguageElement FindElementByClearIndex(NodeList nodes, int index)
	{
	  if (index < 0)
		return null;
	  int cnt = -1;
	  int count = nodes.Count;
	  for (int i = 0; i < count; i++)
	  {
		LanguageElement current = nodes[i] as LanguageElement;
		if (IsEmptyDocText(current))
		  continue;
		cnt++;
		if (cnt >= index)
		  return current;
	  }
	  return null;
	}
	static bool CompareSignature(LanguageElement element, string signature)
	{
	  if (element == null || signature == null)
		return false;
	  string  elSignature = SignatureBuilder.GetSignature(element);
	  if (elSignature == null)
		return false;
	  return String.Compare(elSignature, signature, StringComparison.CurrentCulture) == 0;
	}
	static LanguageElement LocateElementBySignature(NodeList nodes, string signature)
	{
	  if (nodes == null || signature == null)
		return null;
	  int count = nodes.Count;
	  for (int i = 0; i < count; i++)
	  {
		LanguageElement element = nodes[i] as LanguageElement;
		if (element != null && CompareSignature(element, signature))
		  return element;
	  }
	  return null;
	}
	static LanguageElement LocateElement(NodeList nodes, System.Xml.XmlNode xmlNode)
	{
	  LanguageElement result = null;
	  System.Xml.XmlAttribute indexAttr = xmlNode.Attributes["index"];
	  if (indexAttr != null)
	  {
		int index = int.Parse(indexAttr.Value);
		if (index >= 0)
		  result = FindElementByClearIndex(nodes, index);
	  }
	  System.Xml.XmlAttribute sigAttr = xmlNode.Attributes["signature"];
	  if (sigAttr != null && !String.IsNullOrEmpty(sigAttr.Value))
	  {
		if (result != null)
		{
		  if (CompareSignature(result, sigAttr.Value))
			return result;
		  return null;
		}
		return LocateElementBySignature(nodes, sigAttr.Value);
	  }
	  else 
		return result;
	}
	static bool IsElementNode(System.Xml.XmlNode xmlNode)
	{
	  return xmlNode != null &&
		(String.Compare(xmlNode.Name, "scope", StringComparison.CurrentCulture) == 0 ||
		String.Compare(xmlNode.Name, "element", StringComparison.CurrentCulture) == 0 ||
		String.Compare(xmlNode.Name, "target", StringComparison.CurrentCulture) == 0);
	}
	static LanguageElement  LocateElement(LanguageElement context, System.Xml.XmlNode xmlNode)
	{
	  if (context == null || !IsElementNode(xmlNode))
		return null;
	  System.Xml.XmlAttribute isDetailAttr = xmlNode.Attributes["isDetail"];
	  if (isDetailAttr != null)
	  {
		bool isDetail = bool.Parse(isDetailAttr.Value);
		if (isDetail)
		  return LocateElement(context.DetailNodes, xmlNode);
		return LocateElement(context.Nodes, xmlNode);
	  }
	  LanguageElement result = LocateElement(context.Nodes, xmlNode);
	  if (result == null)
		result = LocateElement(context.DetailNodes, xmlNode);
	  return result;
	}
	static LanguageElement  LocateElementForTarget(LanguageElement context, System.Xml.XmlNode xmlNode)
	{
	  CodeElement codeElement = context as CodeElement;
	  if (codeElement == null || xmlNode == null)
		return null;
	   System.Xml.XmlAttribute sigAttr = xmlNode.Attributes["signature"];
	  if (sigAttr == null)
		return null;
	  string signature = sigAttr.Value;
	  if (CompareSignature(codeElement.DocComment, signature))
		return codeElement.DocComment;
	  for (int i = 0; i < codeElement.CommentCount; i++)
		if (CompareSignature(codeElement.Comments[i], signature))
		  return codeElement.Comments[i];
	  for (int i = 0; i < codeElement.AttributeCount; i++)
	  {
		LanguageElement attr = codeElement.Attributes[i] as LanguageElement;
		if (CompareSignature(attr, signature))
		  return attr;
	  }
	  return null;
	}
	static LanguageElement RestoreElementRecursively(LanguageElement context, System.Xml.XmlNode xmlContext)
	{
	  if (context == null)
		return null;
	  LanguageElement current = context;
	  LanguageElement target = null;
	  while(xmlContext != null)
	  {		
		if (target != null)
		{
		  current = LocateElementForTarget(current, xmlContext);
		  target = null;
		}
		else
		  current = LocateElement(current, xmlContext);
		if (IsTarget(xmlContext))
		  target = current;
		if (current == null)
		  return null;
		xmlContext = xmlContext.ChildNodes.Count > 0 ? xmlContext.ChildNodes[0] : null;
	  }
	  return current;
	}
	static LanguageElement RestoreElement(IProjectElement proj, System.Xml.XmlNode xmlContext)
	{
	  if (xmlContext == null)
		return null;
	  LanguageElement current = LocateFile(proj, xmlContext);
	  xmlContext = xmlContext.ChildNodes.Count > 0 ? xmlContext.ChildNodes[0] : null;
	  return RestoreElementRecursively(current, xmlContext);	  
	}
	static LanguageElement RestoreElement(SourceFile file, System.Xml.XmlNode xmlContext)
	{
	  if (file == null || xmlContext == null)
		return null;
	  if (IsFile(xmlContext))
		xmlContext = xmlContext.ChildNodes.Count > 0 ? xmlContext.ChildNodes[0] : null;
	  return RestoreElementRecursively(file, xmlContext);
	}
	public static string GetFileLocation(LanguageElement element)
	{
	  System.Xml.XmlDocument doc = new System.Xml.XmlDocument();	  
	  BuildLocation(doc, element);
	  StringBuilder sBuilder = new StringBuilder();
	  StringWriter sWriter = new StringWriter(sBuilder);
	  System.Xml.XmlTextWriter xWriter = new System.Xml.XmlTextWriter(sWriter);
	  try
	  {
		doc.WriteTo(xWriter);
		return sBuilder.ToString();
	  }
	  finally
	  {
		sWriter.Close();
		xWriter.Close();
	  }
	}
	public static LanguageElement FindElement(IProjectElement proj, string fileLocation)
	{
	  if (String.IsNullOrEmpty(fileLocation))
		return null;
	  StringReader sReader = new StringReader(fileLocation);
	  System.Xml.XmlTextReader xReader = new System.Xml.XmlTextReader(sReader);	  
	  System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
	  try
	  {
		System.Xml.XmlNode node = doc.ReadNode(xReader);
		return RestoreElement(proj, node);
	  }
	  finally
	  {
		sReader.Close();
		xReader.Close();
	  }
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static LanguageElement FindElementInFile(SourceFile file, string location)
	{
	  if (file == null || String.IsNullOrEmpty(location))
		return null;
	  StringReader sReader = new StringReader(location);
	  System.Xml.XmlTextReader xReader = new System.Xml.XmlTextReader(sReader);	  
	  System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
	  try
	  {
		System.Xml.XmlNode node = doc.ReadNode(xReader);
		return RestoreElement(file, node);
	  }
	  finally
	  {
		sReader.Close();
		xReader.Close();
	  }
	}
	}
}
