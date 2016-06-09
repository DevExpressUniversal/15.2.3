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
	public class QueuedReplace: QueuedDelete
	{
		const string STR_Replace = "Replace";
		#region private fields...
		private string _NewCode;
		bool _UseTextExpansion;
		#endregion
		#region QueuedReplace()
		protected QueuedReplace()
		{			
		}
		#endregion
		#region QueuedReplace(SourceRange sourceRange, string newCode)
		public QueuedReplace(SourceRange sourceRange, string newCode): base(sourceRange)
		{
			_NewCode = newCode;
			InternalName = STR_Replace;
		}
		#endregion
		#region QueuedReplace(LanguageElement element, string newCode)
		public QueuedReplace(LanguageElement element, string newCode): base(element)
		{
			_NewCode = newCode;
			InternalName = STR_Replace;
		}
		#endregion
		#region QueuedReplace(LanguageElement firstSibling, LanguageElement lastSibling, string newCode)
		public QueuedReplace(LanguageElement firstSibling, LanguageElement lastSibling, string newCode): base(firstSibling, lastSibling)
		{
			_NewCode = newCode;
			InternalName = STR_Replace;
		}
		#endregion
		void ReplaceText(IDocument iDocument, bool format)
		{
	  SourceRange resultRange;
			if (_UseTextExpansion)
		resultRange = iDocument.ExpandText(BoundRange.Start, _NewCode);
			else
		resultRange = iDocument.InsertText(BoundRange.Start, _NewCode);
	  if (format)
		iDocument.Format(resultRange);
		}
		#region ApplyEdit
		protected override void ApplyEdit(IDocument iDocument, bool format)
		{
			base.ApplyEdit(iDocument, format);		
			if (_NewCode != null && _NewCode != "")
		ReplaceText(iDocument, format);
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is QueuedReplace))
				return;
			QueuedReplace lSource = (QueuedReplace)source;
			_NewCode = lSource._NewCode;							
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			QueuedReplace lClone = new QueuedReplace();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region ElementType
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.QueuedReplace;
			}
		}
		#endregion
		#region NewCode
		public string NewCode
		{
			get
			{
				return _NewCode;
			}
			set
			{
				_NewCode = value;
			}
		}
		#endregion
		public bool UseTextExpansion
		{
			get
			{
				return _UseTextExpansion;
			}
			set
			{
				_UseTextExpansion = value;
			}
		}
	}
}
