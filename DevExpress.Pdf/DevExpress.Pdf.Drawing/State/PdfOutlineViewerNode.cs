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

using System.Collections.ObjectModel;
using System.ComponentModel;
namespace DevExpress.Pdf.Drawing {
	public class PdfOutlineViewerNode : INotifyPropertyChanged {
		internal static ObservableCollection<PdfOutlineViewerNode> CreateTree(PdfDocument document) {
			ObservableCollection<PdfOutlineViewerNode> tree = new ObservableCollection<PdfOutlineViewerNode>();
			PdfOutlines outlines = document.Outlines;
			if (outlines != null)
				CreateOutlinesNode(tree, outlines.First, 0);
			return tree;
		}
		static int CreateOutlinesNode(ObservableCollection<PdfOutlineViewerNode> tree, PdfOutline outline, int id) {
			int parentNodeNumber = id;
			while (outline != null) {
				id++;
				tree.Add(new PdfOutlineViewerNode(outline, id, parentNodeNumber));
				if (outline.First != null)
					id = CreateOutlinesNode(tree, outline.First, id);
				outline = outline.Next;
			}
			return id;
		}
		readonly PdfOutline outline;
		readonly int id;
		readonly int parentId;
		readonly string title;
		readonly bool italic;
		readonly bool bold;
		readonly PdfColor foreColor;
		readonly bool hasChildNodes;
		bool useForeColor;
		PdfInteractiveOperation interactiveOperation;
		public int Id { get { return id; } }
		public int ParentId { get { return parentId; } }
		public string Title { get { return title; } }
		public bool Expanded {
			get { return !outline.Closed; }
			set {
				value = !value;
				if (outline.Closed != value) {
					outline.Closed = value;
					RaisePropertyChanged("Expanded");
				}
			}
		}
		public bool Bold { get { return bold; } }
		public bool Italic { get { return italic; } }
		public PdfColor ForeColor { get { return foreColor; } }
		public bool HasChildNodes { get { return hasChildNodes; } }
		public bool UseForeColor {
			get { return useForeColor; }
			set {
				if (useForeColor != value) {
					useForeColor = value;
					RaisePropertyChanged("UseForeColor");
				}
			}
		}
		internal PdfInteractiveOperation InteractiveOperation {
			get {
				if (interactiveOperation == null)
					interactiveOperation = new PdfInteractiveOperation(outline.Action, outline.Destination);
				return interactiveOperation;
			}
		}
		internal PdfOutline Outline { get { return outline; } }
		public event PropertyChangedEventHandler PropertyChanged;
		public PdfOutlineViewerNode(PdfOutline outline, int id, int parentId) {
			if (outline != null) {
				this.outline = outline;
				this.title = outline.Title;
				this.italic = outline.IsItalic;
				this.bold = outline.IsBold;
				this.foreColor = outline.Color;
				this.hasChildNodes = outline.First != null;
			}
			this.id = id;
			this.parentId = parentId;
		}
		void RaisePropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
