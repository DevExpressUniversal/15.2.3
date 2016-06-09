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

using System.Collections.Generic;
using System.Reflection;
using System;
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CodeStyle.Formatting
#else
namespace DevExpress.CodeParser.CodeStyle.Formatting
#endif
{
  public class FormattingRuleCollection : IEnumerable<BaseFormattingRule>, IComparer<BaseFormattingRule>
  {
	Dictionary<string, BaseFormattingRule> _Rules = new Dictionary<string, BaseFormattingRule>();
	void Sort()
	{
	  List<BaseFormattingRule> list = new List<BaseFormattingRule>(_Rules.Values);
	  list.Sort(this);
	  _Rules.Clear();
	  foreach (BaseFormattingRule rule in list)
		Add(rule);
	}
	public void Add(BaseFormattingRule rule)
	{
	  if (rule.Name == null)
		return;
	  rule.ParentCollection = this;
	  _Rules[rule.Name] = rule;
	}
	public void Add(BaseFormattingRule rule, object value)
	{
	  if (rule == null)
		return;
	  rule.Value = value;
	  Add(rule);
	}
	public bool Enabled(string name)
	{
	  BaseFormattingRule rule = Find(name);
	  return rule != null ? rule.Enabled : false;
	}
	public object GetDefaultValue(string name)
	{
	  BaseFormattingRule rule = Find(name);
	  return rule != null ? rule.DefaultValue : null;
	}
	#region Find(string name)
	public BaseFormattingRule Find(string name)
	{
	  BaseFormattingRule rule = null;
	  if (_Rules.TryGetValue(name, out rule))
		return rule;
	  return null;
	}
	#endregion
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static FormattingRuleCollection GetRulesByLanguage(ParserLanguageID languageId)
	{
	  FormattingRuleCollection result = new FormattingRuleCollection();
	  Type baseType = typeof(BaseFormattingRule);
	  Assembly current = Assembly.GetExecutingAssembly();
	  foreach (Type type in current.GetTypes())
	  {
		if (type.IsAbstract || !baseType.IsAssignableFrom(type))
		  continue;
		try
		{
		  BaseFormattingRule rule = (BaseFormattingRule)Activator.CreateInstance(type);
		  if (rule.IsLanguageSupport(languageId))
			result.Add(rule);
		}
		catch { }
	  }
	  result.Sort();
	  return result;
	}
	#region this[string name]
	public BaseFormattingRule this[string name]
	{
	  get
	  {
		return Find(name);
	  }
	}
	#endregion
	public int Count
	{
	  get { return _Rules.Count; }
	}
	public bool IsReadOnly
	{
	  get { return false; }
	}
	IEnumerator<BaseFormattingRule> IEnumerable<BaseFormattingRule>.GetEnumerator()
	{
	  return _Rules.Values.GetEnumerator();
	}
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
	  return _Rules.Values.GetEnumerator();
	}
	public int Compare(BaseFormattingRule x, BaseFormattingRule y)
	{
	  int result = string.Compare(x.Category.ToString(), y.Category.ToString());
	  if (result != 0)
		return result;
	  string xFirst = x.SubCategory.Split('\\')[0];
	  string yFirst = y.SubCategory.Split('\\')[0];
	  if (xFirst == "General" && xFirst != yFirst)
		return -1;
	  if (yFirst == "General" && xFirst != yFirst)
		return 1;
	  if (xFirst == "Other" && xFirst != yFirst)
		return 1;
	  if (yFirst == "Other" && xFirst != yFirst)
		return -1;
	  result = string.Compare(x.SubCategory, y.SubCategory);
	  if (result != 0)
		return result;
	  result = string.Compare(x.Name, y.Name);
	  return result;
	}
  }
}
