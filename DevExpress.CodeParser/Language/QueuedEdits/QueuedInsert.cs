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
	public class QueuedInsert: QueuedEdit
	{
		#region private fields...
		private string _NewCode;
		#endregion
		#region QueuedInsert()
		protected QueuedInsert()
		{			
		}
		#endregion
		#region QueuedInsert(SourcePoint insertionPoint, string newCode)
		public QueuedInsert(SourcePoint insertionPoint, string newCode)
		{
			InternalName = "Insert";
			SetRange(insertionPoint, insertionPoint);
			_NewCode = newCode;
		}
		#endregion
		#region ApplyEdit
		protected override void ApplyEdit(IDocument iDocument, bool format)
		{
	  if (_NewCode != null && _NewCode != "")
	  {
		SourceRange resultRange = iDocument.InsertText(BoundRange.Start, _NewCode);
		if (format)
		  iDocument.Format(resultRange);
	  }
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is QueuedInsert))
				return;
			QueuedInsert lSource = (QueuedInsert)source;
			_NewCode = lSource._NewCode;							
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			QueuedInsert lClone = new QueuedInsert();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region ElementType
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.QueuedInsert;
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
	}
}
