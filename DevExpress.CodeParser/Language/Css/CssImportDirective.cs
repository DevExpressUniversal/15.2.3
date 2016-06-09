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
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public class CssImportDirective : BaseCssElement, ICssImportDirective
  {
	string _Source = string.Empty;
	StringCollection _SupportedMediaTypes;
	CssMediaQueryCollection _Queries;
	#region Clone
	public override BaseElement Clone()
	{
	  return Clone(ElementCloneOptions.Default);
	}
	#endregion
	#region Clone(ElementCloneOptions options)
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  CssImportDirective clone = new CssImportDirective();
	  clone.CloneDataFrom(this, options);
	  return clone;
	}
	#endregion
	public void AddQuery(CssMediaQuery query)
	{
	  if (query == null)
		return;
	  if (_Queries == null)
		_Queries = new CssMediaQueryCollection();
	  _Queries.Add(query);
	  AddDetailNode(query);
	  SupportedMediaTypes.Add(query.Name);
	}
	internal void SetQueries(CssMediaQueryCollection queries)
	{
	  if (queries == null)
		return;
	  foreach (CssMediaQuery query in queries)
		AddQuery(query);
	}
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  CssImportDirective cssImportDirective = source as CssImportDirective;
	  if (cssImportDirective == null)
		return;
	  SupportedMediaTypes.Clear();
	  for (int i = 0; i < cssImportDirective.SupportedMediaTypes.Count; i++)
		SupportedMediaTypes.Add(cssImportDirective.SupportedMediaTypes[i]);
	  _Source = cssImportDirective._Source;
	}
	#endregion
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.CssImportDirective;
	  }
	}
	public string Source
	{
	  get
	  {
		return _Source;
	  }
	  set
	  {
		_Source = value;
	  }
	}
	public StringCollection SupportedMediaTypes
	{
	  get
	  {
		if (_SupportedMediaTypes == null)
		  _SupportedMediaTypes = new StringCollection();
		return _SupportedMediaTypes;
	  }
	}
	public int SupportedMediaTypesCount
	{
	  get
	  {
		if (_SupportedMediaTypes == null)
		  return 0;
		return _SupportedMediaTypes.Count;
	  }
	}
  }
}
