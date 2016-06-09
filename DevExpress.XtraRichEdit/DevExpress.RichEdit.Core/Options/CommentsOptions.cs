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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
#if SL
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit {
	#region RichEditCommentVisibility
	[ComVisible(true)]
	public enum RichEditCommentVisibility {
		Auto,
		Visible,
		Hidden
	}
	#endregion
	#region CommentOptions
	[ComVisible(false)]
	public class CommentOptions : RichEditNotificationOptions {
		[Flags]
		protected internal enum CommentOption {
			None = 0,
			VisibleAuthors = 1,
			Visibility = 2,
			Color = 4,
			ShowAllAuthors = 16,
			Author = 32,
			HighlightCommentedRange = 64
		}
		protected internal class CommentOptionsChangedEventArgs {
			readonly CommentOption options;
			public CommentOptionsChangedEventArgs(CommentOption options) {
				this.options = options;
			}
			public CommentOption Options { get { return options; } }
			public bool VisibleAuthorsChanged { get { return IsOptionChanged(CommentOption.VisibleAuthors); } }
			public bool VisibilityChanged { get { return IsOptionChanged(CommentOption.Visibility); } }
			public bool ColorChanged { get { return IsOptionChanged(CommentOption.Color); } }
			public bool ShowAllAuthorsChanged { get { return IsOptionChanged(CommentOption.ShowAllAuthors); } }
			public bool AuthorChanged { get { return IsOptionChanged(CommentOption.Author); } }
			public bool HighlightCommentedRangeChanged { get { return IsOptionChanged(CommentOption.HighlightCommentedRange); } }
			bool IsOptionChanged(CommentOption option) {
				return (this.options & option) != 0;
			}
		}
		protected internal delegate void CommentOptionsChangedEventHandler(object sender, CommentOptionsChangedEventArgs e);
		#region Fields
		const RichEditCommentVisibility defaultVisibility = RichEditCommentVisibility.Auto;
		static readonly Color defaultColor = DXColor.Empty;
		static readonly Color[] defaultColors = new Color[] { DXColor.FromArgb(0xD5, 0xD5, 0xFF),
																DXColor.FromArgb(0xFF, 0xD5, 0xFF),
																DXColor.FromArgb(0xFF, 0xE6, 0xD5),
																DXColor.FromArgb(0xD5, 0xFF, 0xFE),
																DXColor.FromArgb(0xFF, 0xFE, 0xD5),
																DXColor.FromArgb(0xE9, 0xE9, 0xE9),
																DXColor.FromArgb(0xFF, 0xD5, 0xD5),
																DXColor.FromArgb(0xD5, 0xFF, 0xD5)};
		RichEditCommentVisibility visibility;
		Color color;
		bool showAllAuthors = true;
		System.Collections.ObjectModel.ObservableCollection<string> visibleAuthors;
		string author = String.Empty;
		bool highlightCommentedRange = false;
		CommentOption defferedChanges;
		bool lockCommentOptionsChangedEvent;
		#endregion
		public CommentOptions() {
			visibleAuthors = new ObservableCollection<string>();
			visibleAuthors.CollectionChanged += visibleAuthors_CollectionChanged;
			defferedChanges = CommentOption.None;
		}
		private void visibleAuthors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			OnChanged("VisibleAuthors", visibleAuthors, visibleAuthors);
			OnCommentOptionsChanged(CommentOption.VisibleAuthors);
		}
		#region Properties
		#region Visibility
		[NotifyParentProperty(true), DefaultValue(defaultVisibility)]
		public RichEditCommentVisibility Visibility {
			get { return visibility; }
			set {
				if (Visibility == value)
					return;
				RichEditCommentVisibility oldValue = Visibility;
				visibility = value;
				OnChanged("Visibility", oldValue, value);
				OnCommentOptionsChanged(CommentOption.Visibility);
			}
		}
		#endregion
		#region Color
		[NotifyParentProperty(true)]
		public Color Color {
			get { return color; }
			set {
				if (Color == value)
					return;
				Color oldValue = Color;
				color = value;
				OnChanged("CommentColor", oldValue, value);
				OnCommentOptionsChanged(CommentOption.Color);
			}
		}
		protected internal virtual bool ShouldSerializeColor() {
			return Color != defaultColor;
		}
		public virtual void ResetColor() {
			Color = defaultColor;
		}
		#endregion
		#region ShowAllAuthors
		[NotifyParentProperty(true), DefaultValue(true)]
		public bool ShowAllAuthors {
			get { return showAllAuthors; } 
			set {
				if (ShowAllAuthors == value)
					return;
				bool oldValue = ShowAllAuthors;
				showAllAuthors = value;
				OnChanged("ShowAllAuthors", oldValue, value);
				OnCommentOptionsChanged(CommentOption.ShowAllAuthors);
			}
		}
		#endregion
		public ObservableCollection<string> VisibleAuthors { get { return visibleAuthors; } }
		#region Author 
		[NotifyParentProperty(true), DefaultValue("")]
		public string Author {
			get { return author; }
			set {
				if (Author == value)
					return;
				string oldValue = Author;
				author = value;
				OnChanged("Author", oldValue, value);
				OnCommentOptionsChanged(CommentOption.Author);
			}
		}
		#endregion
		#region HighlightCommentedRange
		[NotifyParentProperty(true), DefaultValue(false)]
		public bool HighlightCommentedRange {
			get { return highlightCommentedRange; }
			set {
				if (HighlightCommentedRange == value)
					return;
				bool oldValue = HighlightCommentedRange;
				highlightCommentedRange = value;
				OnChanged("HighlightCommentedRange", oldValue, value);
				OnCommentOptionsChanged(CommentOption.HighlightCommentedRange);
			}
		}
		#endregion
		#endregion
		protected internal event CommentOptionsChangedEventHandler CommentOptionsChanged;
		protected internal void OnCommentOptionsChanged(CommentOption option) {
			if (IsLockUpdate)
				defferedChanges |= option;
			else
				RaisCommentOptionsChanged(new CommentOptionsChangedEventArgs(option));
		}
		protected internal virtual void RaisCommentOptionsChanged(CommentOptionsChangedEventArgs e) {
			if (CommentOptionsChanged != null) {
				lockCommentOptionsChangedEvent = true;
				try {
					CommentOptionsChanged(this, e);
				}
				finally {
					lockCommentOptionsChangedEvent = false;
				}
			}
		}
		public override void EndUpdate() {
			base.EndUpdate();
			if (!IsLockUpdate && !lockCommentOptionsChangedEvent) {
				if (this.defferedChanges != CommentOption.None)
					RaisCommentOptionsChanged(new CommentOptionsChangedEventArgs(this.defferedChanges));
				this.defferedChanges = CommentOption.None;
			}
		}
		public override void CancelUpdate() {
			base.CancelUpdate();
			if (!IsLockUpdate)
				this.defferedChanges = CommentOption.None;
		}
		protected internal override void ResetCore() {
			Visibility = defaultVisibility;
			Color = defaultColor;
			ShowAllAuthors = true;
			Author = String.Empty;
			HighlightCommentedRange = false;
		}
		protected internal void CopyFrom(CommentOptions options) {
			this.visibility = options.visibility;
			this.color = options.color;
			this.showAllAuthors = options.showAllAuthors;
			this.visibleAuthors = options.visibleAuthors;
			this.HighlightCommentedRange = options.highlightCommentedRange;
		}
		internal static int GetDefaultColorsLength() {
			return defaultColors.Length;
		}
		internal static Color GetColor(int index){
			return defaultColors[index];
		}
		[ComVisible(false)]
		protected internal static Color SetBorderColor(Color color) {
			Color res = color;
			res = Color.FromArgb(GetDarkValue(res.A), GetDarkValue(res.R), GetDarkValue(res.G), GetDarkValue(res.B));
			return res;
		}
		protected internal static byte GetDarkValue(byte val) {
			byte result = Convert.ToByte((val * 8) / 10);
			return result;
		}
		protected internal static int GetLightValue(byte val) {
			return val + ((255 - val) * 16) / 100;
		}
	}
#endregion
}
