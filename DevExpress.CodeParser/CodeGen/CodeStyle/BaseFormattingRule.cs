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
using System.ComponentModel;
using System;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CodeStyle.Formatting
#else
namespace DevExpress.CodeParser.CodeStyle.Formatting
#endif
{
  public abstract class BaseFormattingRule
  {
	object _Value;
	FormattingRuleCollection _ParentCollection;
	protected const ParserLanguageID CSharpAndBasic = ParserLanguageID.Basic | ParserLanguageID.CSharp;
	T GetValue<T>()
	{
	  return GetValue<T>(Value);
	}
	T GetDefaultValue<T>()
	{
	  return GetValue<T>(DefaultValue);
	}
	T GetValue<T>(object obj)
	{
	  if (obj is T)
		return (T)obj;
	  return default(T);
	}
	protected virtual void ValueChanged(object oldValue)
	{
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual object GetDefaultValue(LanguageElement element)
	{
	  return null;
	}
	public bool IsLanguageSupport(ParserLanguageID languageID)
	{
	  return (SupportedLanguage & languageID) != 0;
	}
	public abstract FormattingCategory Category { get; }
	public abstract string Name { get; }
	public abstract string SubCategory { get; }
	public virtual ParserLanguageID SupportedLanguage
	{
	  get { return ParserLanguageID.None; }
	}
	public bool Enabled
	{
	  get { return GetValue<bool>(); }
	}
	public bool BoolValue
	{
	  get { return Enabled; }
	}
	public int IntValue
	{
	  get { return Convert.ToInt32(Value); }
	}
	public string StringValue
	{
	  get { return GetValue<string>(); }
	}
	public object Value
	{
	  get
	  {
		if (_Value == null)
		  return DefaultValue;
		return _Value;
	  }
	  set
	  {
		if (object.Equals(_Value, value))
		  return;
		object oldValue = _Value;
		_Value = value;
		ValueChanged(oldValue);
	  }
	}
	public virtual object DefaultValue
	{
	  get { return null; }
	}
	public int DefaultIntValue
	{
	  get { return Convert.ToInt32(DefaultValue); }
	}
	public virtual FormattingOption EditOption
	{
	  get { return FormattingOption.Boolean; }
	}
	public FormattingRuleCollection ParentCollection
	{
	  get { return _ParentCollection; }
	  internal set { _ParentCollection = value; }
	}
  }
}
