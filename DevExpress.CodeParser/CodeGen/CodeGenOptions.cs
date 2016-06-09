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

using System.ComponentModel;
#if DXCORE
using DevExpress.CodeRush.StructuralParser.CodeStyle.Formatting;
namespace DevExpress.CodeRush.StructuralParser
#else
using DevExpress.CodeParser.CodeStyle.Formatting;
namespace DevExpress.CodeParser
#endif
{
	public class CodeGenOptions
	{
		#region private fields...
		string _IndentString;
		IBraceSettings _BraceSettings;
	IndentionFormattingOptions _Indention;
	BlankLinesFormattingOptions _BlankLines;
	GeneralFormattingOptions _General;
	LineBreaksFormattingOptions _LineBreaks;
	OtherLanguageSpecificFormattingOptions _OtherLanguageSpecific;
	SortingOrderingFormattingOptions _SortingOrdering;
	SpacingFormattingOptions _Spacing;
	WrappingAlignmentFormattingOptions _WrappingAlignment;
	ParserLanguageID _Language;
	bool _SaveFormat;
	bool _InsertSpaces;
	int _TabSize;
		#endregion
		#region CodeGenOptions
	public CodeGenOptions(ParserLanguageID language)
	{
	  _IndentString = "\t\t";
	  InitializeLanguageOptions(language);
	}
		#endregion
		#region CodeGenOptions
		public CodeGenOptions(IBraceSettings settings, ParserLanguageID language) : this(language)
		{
			_BraceSettings = settings;
		}
		#endregion
	void InitializeLanguageOptions(ParserLanguageID language)
	{
	  _Language = language;
	  _BraceSettings = StructuralParserServicesHolder.LoadBraceSettings();
	  if (_BraceSettings == null)
		_BraceSettings = new DefaultBraceSettings();
	  _Indention = new IndentionFormattingOptions();
	  _BlankLines = new BlankLinesFormattingOptions();
	  _General = new GeneralFormattingOptions();
	  _LineBreaks = new LineBreaksFormattingOptions();
	  _OtherLanguageSpecific = new OtherLanguageSpecificFormattingOptions();
	  _SortingOrdering = new SortingOrderingFormattingOptions();
	  _Spacing = new SpacingFormattingOptions();
	  _WrappingAlignment = new WrappingAlignmentFormattingOptions();
	  IFormattingService fs = StructuralParserServicesHolder.StructuralParserServices.FormattingService;
	  if (fs == null)
	  {
		LoadDefaultFormattingSettings(language);
		return;
	  }
	  FormattingRuleCollection rules = fs.GetRules(language);
	  LoadFormattingSettings(rules);
	  SetIndentString(StructuralParserServicesHolder.GetTabSettings(language));
	}
	void SetIndentString(ITabSettings settings)
	{
	  if (settings == null)
		return;
	  _IndentString = new string(settings.InsertSpaces ? ' ' : '\t', settings.TabSize);
	  _TabSize = settings.TabSize;
	  _InsertSpaces = settings.InsertSpaces;
	}
	#region LoadDefaultFormattingSettings
	void LoadDefaultFormattingSettings(ParserLanguageID language)
	{
	  _Indention.LoadDefault(language);
	  _BlankLines.LoadDefault(language);
	  _General.LoadDefault(language);
	  _LineBreaks.LoadDefault(language);
	  _OtherLanguageSpecific.LoadDefault(language);
	  _SortingOrdering.LoadDefault(language);
	  _Spacing.LoadDefault(language);
	  _WrappingAlignment.LoadDefault(language);
	  SetIndentString(new DefaultTabSettings());
	}
	#endregion
	#region LoadFormattingSettings
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void LoadFormattingSettings(FormattingRuleCollection rules)
	{
	  if (rules == null || rules.Count == 0)
	  {
		LoadDefaultFormattingSettings(_Language);
		return;
	  }
	  _Indention.Load(rules);
	  _BlankLines.Load(rules);
	  _General.Load(rules);
	  _LineBreaks.Load(rules);
	  _OtherLanguageSpecific.Load(rules);
	  _SortingOrdering.Load(rules);
	  _Spacing.Load(rules);
	  _WrappingAlignment.Load(rules);
	}
	#endregion
		#region Default
		public static CodeGenOptions Default
		{
			get
			{
		return new CodeGenOptions(ParserLanguageID.None);
			}
		}
		#endregion
		#region IndentString
		public string IndentString
		{
			get
			{
				return _IndentString;
			}
			set
			{
				if (_IndentString == value)
					return;
				_IndentString = value;
			}
		}
		#endregion
		public IBraceSettings BraceSettings
		{
			get
			{
				return _BraceSettings;
			}
		}
	public IndentionFormattingOptions Indention
	{
	  get
	  {
		return _Indention;
	  }
	}
	public BlankLinesFormattingOptions BlankLines
	{
	  get
	  {
		return _BlankLines;
	  }
	}
	public GeneralFormattingOptions General
	{
	  get
	  {
		return _General;
	  }
	}
	public LineBreaksFormattingOptions LineBreaks
	{
	  get
	  {
		return _LineBreaks;
	  }
	}
	public OtherLanguageSpecificFormattingOptions OtherLanguageSpecific
	{
	  get
	  {
		return _OtherLanguageSpecific;
	  }
	}
	public SortingOrderingFormattingOptions SortingOrdering
	{
	  get
	  {
		return _SortingOrdering;
	  }
	}
	public SpacingFormattingOptions Spacing
	{
	  get
	  {
		return _Spacing;
	  }
	}
	public WrappingAlignmentFormattingOptions WrappingAlignment
	{
	  get
	  {
		return _WrappingAlignment;
	  }
	}
	public bool SaveFormat
	{
	  get { return _SaveFormat; }
	  set { _SaveFormat = value; }
	}
	public bool InsertSpaces
	{
	  get
	  {
		return _InsertSpaces;
	  }
	  internal set
	  {
		_InsertSpaces = value;
	  }
	}
	public int TabSize
	{
	  get
	  {
		return _TabSize;
	  }
	}
	}
}
