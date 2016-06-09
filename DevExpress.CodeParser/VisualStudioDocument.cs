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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class VisualStudioDocument : PathElement
	{
		#region private fields...
	private string _FileNameOnly;
		private IDocument _Document;
		IProjectElement _Project;
		string _Kind;
	#endregion
		#region VisualStudioDocument
		public VisualStudioDocument()
		{
			SetStart(1, 1);
			_Kind = String.Empty;
	}
		#endregion
		#region VisualStudioDocument
		public VisualStudioDocument(string kind, string name)
		{
			SetStart(1, 1);
			_Kind = kind;
			InternalName = name;
		}
		#endregion
		protected void ClearFileName()
		{
			_FileNameOnly = null;
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options) 
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is VisualStudioDocument))
				return;
			VisualStudioDocument lSource = (VisualStudioDocument)source;
			_Document = lSource.Document;
			_FileNameOnly = lSource._FileNameOnly;
		}		
	public void SetProject(IProjectElement project)
	{
	  _Project = project;
	}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.File;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			if (_FileNameOnly == null || _FileNameOnly == String.Empty)
				_FileNameOnly = Path.GetFileName(InternalName);
			return _FileNameOnly;
		}
		#endregion
		#region SetDocument
		public void SetDocument(IDocument aDocument)
		{
			_Document = aDocument;
		}
		#endregion
		#region AssignProject
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AssignProject(IProjectElement project)
		{
			_Project = project;
		}
		#endregion
		#region InvalidateRange
		public virtual void InvalidateRange(SourceRange range)
		{
		}
		#endregion
		#region RangedParseComplete
		public virtual void RangedParseComplete(SourceRange range, LanguageElement context)
		{
		}
		#endregion		
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			VisualStudioDocument lClone = new VisualStudioDocument();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override IDocument Document
		{
			get
			{
				return _Document;
			}
		}
		#region Project
		public override IProjectElement Project
		{
			get
			{
				if (_Project == null)
				{
					if (Document != null)
						_Project = Document.ProjectElement;
				}
				return _Project;
			}
		}
		#endregion
		public string Kind
		{
			get
			{
				return _Kind;
			}
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.VisualStudioDocument;
			}
		}
		public override bool IsNewContext
		{
			get 
			{
				return true;
			}
		}
	}
}
