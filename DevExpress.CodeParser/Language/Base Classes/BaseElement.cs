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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public abstract class BaseElement : ICloneable
	{
		object System.ICloneable.Clone()
		{
			return Clone();
		}
		SourceRange GetRangeFromNodes(NodeList nodes)
		{
			if (nodes == null)
				return SourceRange.Empty;
			SourceRange result = SourceRange.Empty;
			for (int i = 0; i < nodes.Count; i++)
			{
				LanguageElement node = nodes[i] as LanguageElement;
				if (node == null || node.Range.IsEmpty)
					continue;
				if (result.IsEmpty)
					result = node.Range;
				else
					result = SourceRange.Union(result, node.Range);
			}
			return result;
		}
		#region CleanUpOwnedReferencesForNodes
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected void CleanUpOwnedReferencesForNodes(NodeList nodeList)
		{
			if (nodeList == null)
				return;
			for (int i = 0; i < nodeList.Count; i++)
			{
				BaseElement element = nodeList[i] as BaseElement;
				if (element == null)
					continue;
				element.CleanUpOwnedReferences();
			}
		}
		#endregion
		#region CreateNodeList
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual NodeList CreateNodeList()
		{
			return new NodeList();
		}
		#endregion
		#region CloneDataFrom(BaseElement source)
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public SourceRange GetDetailNodeRange()
		{
	  return GetRangeFromNodes(DetailNodes);			
		}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public SourceRange GetNodeRange()
	{
	  return GetRangeFromNodes(Nodes);			
	}
		#region OwnedReferencesTransfered
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void OwnedReferencesTransfered()
		{
		}
		#endregion
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void CleanUpOwnedReferences()
		{
		}
		#endregion
		protected internal abstract NodeList InnerNodes { get; }
		protected internal int InnerNodeCount { get { return InnerNodes == null ? 0 : InnerNodes.Count; } }
		protected internal abstract NodeList InnerDetailNodes { get; }
		protected internal int InnerDetailNodeCount { get { return (InnerDetailNodes == null ? 0 : InnerDetailNodes.Count); } }
		#region Nodes
		[Description("The language elements parented by this element.")]
		[Category("Family")]
		public abstract NodeList Nodes
		{
			get;
		}
		#endregion
		#region NodeCount
		[Description("The number of child language elements parented by this element.")]
		[Category("Family")]
		public int NodeCount
		{
			get
			{
				return Nodes == null ? 0 : Nodes.Count;
			}
		}
		#endregion
		#region DetailNodes
		[Description("The detail language elements associated with this element.")]
		[Category("Family")]
		public abstract NodeList DetailNodes
		{
			get;
		}
		#endregion
		#region DetailNodeCount
		[Description("The number of detail language elements parented by this element.")]
		[Category("Family")]
		public int DetailNodeCount
		{
			get
			{
				return(DetailNodes == null ? 0 : DetailNodes.Count);
			}
		}
		#endregion
		#region Name
		[Description("The name of this language element.")]
		[Category("Description")]
		public abstract string Name
		{
			get;
			set;
		}
		#endregion
		public virtual BaseElement Clone()
		{
			return Clone(ElementCloneOptions.Default);
		}
		public virtual BaseElement Clone(ElementCloneOptions options)
		{
			return null;
		}
	}
}
