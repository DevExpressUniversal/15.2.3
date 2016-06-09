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
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public abstract class QueuedEdit: LanguageElement
	{
		#region private fields...
		private bool _AppliedEdit;
		SourceRange _BoundRange;
 		#endregion
		protected abstract void ApplyEdit(IDocument iDocument, bool format);
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is QueuedEdit))
				return;
			QueuedEdit lSource = (QueuedEdit)source;
			_AppliedEdit = lSource._AppliedEdit;
	  _BoundRange = lSource.BoundRange;
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _BoundRange = BoundRange;
	}
		void SetPushableOptions(bool start, bool end)
		{
			if (_BoundRange.Start.HasBindingPoint)
				_BoundRange.Start.BindingPoint.IsPushable = start;
			if (_BoundRange.End.HasBindingPoint)
				_BoundRange.End.BindingPoint.IsPushable = end;
		}
		void GetPushableOptions(out bool start, out bool end)
		{
			start = false;
			end = false;
			if (_BoundRange.Start.HasBindingPoint)
				start = _BoundRange.Start.BindingPoint.IsPushable;
			if (_BoundRange.End.HasBindingPoint)
				end = _BoundRange.End.BindingPoint.IsPushable;
		}
	#region Apply(IDocument iDocument)
		public void Apply(IDocument iDocument)
		{
	  Apply(iDocument, false );
		}
		#endregion
	#region Apply(IDocument iDocument, bool format)
	public void Apply(IDocument iDocument, bool format)
	{
	  if (_AppliedEdit)
		return;
	  _AppliedEdit = true;
	  bool firstIsPushable = false;
	  bool secondIsPushable = false;
	  GetPushableOptions(out firstIsPushable, out secondIsPushable);
	  try
	  {
		SetPushableOptions(false, true);
		ApplyEdit(iDocument, format);
	  }
	  finally
	  {
		SetPushableOptions(firstIsPushable, secondIsPushable);
	  }
	}
	#endregion
		public void BindToCode(IDisposableEditPointFactory document)
		{
	  ClearHistory();
			_BoundRange = Range;
			_BoundRange.BindToCode(document);
		}
		public void RemoveBinding()
		{
			_BoundRange.RemoveAllBindings();
		}
		public SourceRange BoundRange
		{
			get
			{
				if (!_BoundRange.IsBoundToCode)
					return Range;
				return GetTransformedRange(_BoundRange);
			}
		}
	}
	public class QueuedEditCollection : CollectionBase
	{
		public QueuedEditCollection()
		{
		}
		public int Add(QueuedEdit edit)
		{
			if (edit == null)
				return -1;
			return InnerList.Add(edit);
		}
		public void AddRange(QueuedEditCollection edits)
		{
			if (edits == null)
				return;
			InnerList.AddRange(edits);
		}
		public void Remove(QueuedEdit edit)
		{
			if (edit == null)
				return;
			InnerList.Remove(edit);
		}
		public void BindToCode(IDisposableEditPointFactory factory)
		{
			if (factory == null)
				return;
			int count = Count;
			for (int i = 0; i < count; i++)
			{
				QueuedEdit edit = this[i];
				if (edit != null)
					edit.BindToCode(factory);
			}
		}
		public void RemoveBinding()
		{
			int count = Count;
			for (int i = 0; i < count; i++)
			{
				QueuedEdit edit = this[i];
				if (edit != null)
					edit.RemoveBinding();
			}
		}
		public QueuedEdit this[int index]
		{
			get
			{
				return (QueuedEdit)InnerList[index];
			}
			set
			{
				InnerList[index] = value;
			}
		}
	}
}
