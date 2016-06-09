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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	#region DefineBase
	public class DefineBase
	{
		#region fields...
		string _ConstantDefinition = null;
		NodeList _TokenList;
		string _Path = String.Empty;
		SourcePoint _DefinitionStartPoint = SourcePoint.Empty;
		SourceRange _NameRange = SourceRange.Empty;
		#endregion
		#region constructors...
		public DefineBase(string definition)
		{
			ConstantDefinition = definition;
		}
		#endregion
		#region public properties...
		public SourcePoint DefinitionStartPoint
		{
			get
			{
				return _DefinitionStartPoint;
			}
			set
			{
				_DefinitionStartPoint = value;
			}
		}
		public SourceRange NameRange
		{
			get
			{
				return _NameRange;
			}
			set
			{
				_NameRange = value;
			}
		}
		public string Path
		{
			get
			{
				return _Path;
			}
			set
			{
				_Path = value;
			}
		}
		public NodeList TokenList 
		{
			get
			{
				if (_TokenList == null)
					_TokenList = new NodeList();
				return _TokenList;
			}
			set
			{
				_TokenList = value;
			}
		}		
		public string ConstantDefinition
		{
			get
			{
				return _ConstantDefinition;
			}
			set
			{
				_ConstantDefinition = value;
			}
		}
		#endregion
	}
	#endregion
}
