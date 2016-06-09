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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	#region IInlineCustomObject
	public interface IInlineCustomObject {
		IInlineCustomObject Clone();
		Size CalculateOriginalSize(DocumentModelUnitConverter unitConverter);
		event EventHandler StateChanged;
		byte[] GetState();
		void SetState(byte[] state);
	}
	#endregion
	#region InlineCustomObjectRun
	public class InlineCustomObjectRun : InlineObjectRunBase<InlineCustomObjectInfo> {
		Size originalSize;
		IInlineCustomObject customObject;
		public InlineCustomObjectRun(Paragraph paragraph, IInlineCustomObject customObject)
			: base(paragraph, new Size(1, 1)) {
			Guard.ArgumentNotNull(customObject, "customObject");
			this.customObject = customObject;
			this.originalSize = customObject.CalculateOriginalSize(paragraph.DocumentModel.UnitConverter);
			SubscribeCustomObjectEvents();
			SubscribePropertiesEvents();
		}
		#region Properties
		protected internal InlineCustomObjectProperties CustomObjectProperties { get { return (InlineCustomObjectProperties)Properties; } }
		public override bool CanPlaceCaretBefore { get { return true; } }
		public IInlineCustomObject CustomObject { get { return customObject; } }
		public override Size OriginalSize { get { return originalSize; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (customObject != null) {
					UnsubscribeCustomObjectEvents();
					IDisposable disposable = customObject as IDisposable;
					if (disposable != null)
						disposable.Dispose();
					customObject = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected internal override void SetOriginalSize(Size size) {
			originalSize = size;
		}
		protected internal override void CopyOriginalSize(Size originalSize) {
			this.originalSize = originalSize;
		}
		protected internal override InlineObjectProperties<InlineCustomObjectInfo> CreateProperties(PieceTable pieceTable) {
			return new InlineCustomObjectProperties(pieceTable);
		}
		protected internal virtual void SubscribeCustomObjectEvents() {
			customObject.StateChanged += OnCustomObjectStateChanged;
		}
		protected internal virtual void UnsubscribeCustomObjectEvents() {
			customObject.StateChanged -= OnCustomObjectStateChanged;
		}
		protected internal virtual void SubscribePropertiesEvents() {
			CustomObjectProperties.IndexChanged += OnPropertiesIndexChanged;
		}
		protected internal virtual void UnsubscribePropertiesEvents() {
			CustomObjectProperties.IndexChanged -= OnPropertiesIndexChanged;
		}
		protected internal virtual void OnCustomObjectStateChanged(object sender, EventArgs e) {
			UnsubscribePropertiesEvents();
			try {
				CustomObjectProperties.State = customObject.GetState();
			}
			finally {
				SubscribePropertiesEvents();
			}
		}
		protected internal virtual void OnPropertiesIndexChanged(object sender, EventArgs e) {
			UnsubscribeCustomObjectEvents();
			try {
				customObject.SetState(CustomObjectProperties.State);
			}
			finally {
				SubscribeCustomObjectEvents();
			}
		}
		public override TextRunBase Copy(DocumentModelCopyManager copyManager) {
			PieceTable targetPieceTable = copyManager.TargetPieceTable;
			DocumentModelPosition targetPosition = copyManager.TargetPosition;
			Debug.Assert(this.DocumentModel == copyManager.SourceModel);
			Debug.Assert(targetPosition.PieceTable == targetPieceTable);
			Debug.Assert(targetPosition.RunOffset == 0);
			targetPieceTable.InsertInlineCustomObject(targetPosition.LogPosition, CustomObject.Clone());
			InlineCustomObjectRun run = (InlineCustomObjectRun)targetPieceTable.Runs[targetPosition.RunIndex];
			run.CustomObjectProperties.CopyFrom(this.CustomObjectProperties.Info);
			run.CopyOriginalSize(this.OriginalSize);
			return run;
		}
	}
	#endregion
}
