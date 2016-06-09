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

using System.Drawing;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class EmfPlusGraphicsStateStack : PdfDisposableObject {
		class EmfPlusGraphicsState : PdfDisposableObject {
			int? stateId;
			EmfPlusRegion clip;
			PdfTransformationMatrix transform;
			public EmfPlusRegion Clip {
				get { return clip; }
				set { clip = value; }
			}
			public int? StateId { get { return stateId; } }
			public PdfTransformationMatrix Transform {
				get { return transform; }
				set { transform = value; }
			}
			public EmfPlusGraphicsState() {
				transform = new PdfTransformationMatrix();
			}
			public EmfPlusGraphicsState Clone(int newId) {
				EmfPlusGraphicsState state = new EmfPlusGraphicsState();
				state.clip = clip;
				state.transform = transform;
				state.stateId = newId;
				return state;
			}
			protected override void Dispose(bool disposing) {
				if (disposing)
					clip.Dispose();
			}
		}
		readonly Stack<EmfPlusGraphicsState> graphicsStateStack;
		EmfPlusGraphicsState current;
		public EmfPlusRegion Clip {
			get { return current.Clip; }
			set { current.Clip = value; }
		}
		public int? CurrentId { get { return current.StateId; } }
		public PdfTransformationMatrix Transform {
			get { return current.Transform; }
			set { current.Transform = value; }
		}
		public EmfPlusGraphicsStateStack() {
			graphicsStateStack = new Stack<EmfPlusGraphicsState>();
			current = new EmfPlusGraphicsState();
		}
		public void Push(int id) {
			graphicsStateStack.Push(current.Clone(id));
		}
		public void Pop() {
			current = graphicsStateStack.Pop();
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				foreach (EmfPlusGraphicsState state in graphicsStateStack)
					state.Dispose();
			}
		}
	}
}
