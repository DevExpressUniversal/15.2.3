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
using System.Collections;
#if SL
using DevExpress.Utils;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  using Diagnostics;
	public enum BracesOnNewLineOptions
	{
		LeaveOnPrevious,
		PlaceOnNewLine
	}
	public enum BraceHolder
	{
		Unknown,
		Type,
		ArrayInitializer,
		AnonymousMethod,
		Method,
		ControlFlow
	}
  public interface IBraceSettings
  {
	BracesOnNewLineOptions this[LanguageElement element] {get;}
  }
  public class DefaultBraceSettings : IBraceSettings
  {
	Hashtable _BraceSettings;
	public DefaultBraceSettings()
	{
	  _BraceSettings = new Hashtable();
	  try
	  {
		LoadDefaultSettings();
	  }
	  catch (Exception e)
	  {
		Log.SendException("Exception while loading brace settings", e);
		LoadDefaultSettings();
	  }
	}
	void LoadDefaultSettings()
	{
	  AddBraceSetting(BracesOnNewLineOptions.PlaceOnNewLine, BraceHolder.ControlFlow);
	  AddBraceSetting(BracesOnNewLineOptions.PlaceOnNewLine, BraceHolder.AnonymousMethod);
	  AddBraceSetting(BracesOnNewLineOptions.PlaceOnNewLine, BraceHolder.Type);
	  AddBraceSetting(BracesOnNewLineOptions.PlaceOnNewLine, BraceHolder.Method);
	  AddBraceSetting(BracesOnNewLineOptions.LeaveOnPrevious, BraceHolder.ArrayInitializer);
	}
	void AddBraceSetting(BracesOnNewLineOptions setting, BraceHolder holder)
	{
	  if (_BraceSettings == null)
		return;
	  _BraceSettings[holder] = setting;
	}
	BraceHolder TranslateLanguageElement(LanguageElement element)
	{
	  if (element == null)
		return BraceHolder.Unknown;
	  if (element is TypeDeclaration)
		return BraceHolder.Type;
	  if (element is Method)
		return BraceHolder.Method;
	  if (element is AnonymousMethodExpression)
		return BraceHolder.AnonymousMethod;
	  if (element is DelimiterCapableBlock)
		return BraceHolder.ControlFlow;
	  return BraceHolder.Unknown;
	}
	BracesOnNewLineOptions GetSettingForLanguageElement(LanguageElement element)
	{
	  if (_BraceSettings == null)
		_BraceSettings = new Hashtable();
	  BraceHolder holder = TranslateLanguageElement(element);
	  if (_BraceSettings.Contains(holder))
		return (BracesOnNewLineOptions)_BraceSettings[holder];
	  else
		return BracesOnNewLineOptions.PlaceOnNewLine;
	}
	protected void SetBraceOptionForElement(BraceHolder holder, BracesOnNewLineOptions option)
	{
	  if (_BraceSettings == null)
		_BraceSettings = new Hashtable();
	  if (_BraceSettings.Contains(holder))
		_BraceSettings[holder] = option;
	  else
		_BraceSettings.Add(holder, option);
	}
	public BracesOnNewLineOptions this[LanguageElement element]
	{
	  get
	  {
		return GetSettingForLanguageElement(element);
	  }
	}
  }
}
