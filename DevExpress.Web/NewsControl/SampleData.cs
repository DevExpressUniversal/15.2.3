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
using System.Collections.Generic;
namespace DevExpress.Web.Internal {
	public class NewsControlSampleDataItem {
		private string fHeaderText;
		private string fText;
		private DateTime fDate;
		public string HeaderText {
			get { return fHeaderText; }
		}
		public string Text {
			get { return fText; }
		}
		public DateTime Date {
			get { return fDate; }
		}
		public NewsControlSampleDataItem(string headerText, string text, DateTime date) {
			fHeaderText = headerText;
			fText = text;
			fDate = date;
		}
	}
	public class NewsControlSampleData : IEnumerable {
		private readonly string[] SampleItemHeaderText = new string[] {
			"New Years Eve and New Years Day", 
			"Valentine's Day", 
			"Easter", 
			"Memorial Day", 
			"Independence Day", 
			"Labor Day", 
			"Halloween", 
			"Veterans Day", 
			"Thanksgiving", 
			"Christmas"
		};
		private readonly string[] SampleItemText = new string[] {
			"New Years Day is the first day of the year, January 1st.  It is a celebration of the old year and the new one to come.  People make New Years Resolutions each New Years and promise themselves that they will keep this resolution until next year. New Years Eve is a major social event.  Clubs everywhere are packed with party-goers who stay out all night and go nuts at midnight.  At midnight it is a tradition to make lots of noise.  The traditional New Years Ball is dropped every year in Times Square in New York City at 12 o’clock.  This event can be seen all over the world on television.", 
			"Saint Valentine's Day is a day that is set aside to promote the idea of \"love\".  It is celebrated on February 14th.   People send greeting cards or gifts to loved ones and friends to show them that they care.", 
			"Easter is a major Christian holiday that commemorates the resurrection of Jesus Christ.  It is celebrated on a Sunday between March 22 and April 25. The 40 days leading up to Easter are observed as Lent.  Besides the religious aspects of Easter, people also celebrate spring or the signs of new life. Flowers are seen everywhere.  There are often Easter Parades such as the one in New York City where people dress up in their new spring clothes.  Children receive Easter baskets filled with candy Easter eggs, chocolate bunnies and jelly beans!  The dying of eggs with food color is also an Easter tradition in many American families.", 
			"Memorial Day is a legal holiday that takes place every year on the last Monday in May.  Memorial Day is in honor of the nation’s armed forces who were killed defending their country in war.  Memorial Day was originally called Decoration Day.  It is celebrated with parades, memorial speeches and ceremonies, and the decoration of graves with flowers and flags.  Memorial Day is a indication or reminder that summer is on its way. Many small towns in America celebrate memorial day in their own special way.  In our town, we have a small fair with barbequed chicken, rides for the children and a special fireworks display. Memorial Day is also the traditional day for people to open up their swimming pools!", 
			"Independence Day (fourth of July)  is celebrated every year in the U.S. on July 4th.  It commemorates the signing of the Declaration of Independence. The first Independence Day was celebrated in Philadelphia on July 8, 1776. This is when the declaration was read aloud.  Parades, patriotic speeches, fireworks and pageants are all ways of celebrating today.  Many families celebrate the Fourth of July by having picnics and going to the beach.  You will also find fireworks being displayed in many towns and cities across America on the Fourth of July!", 
			"Labor Day is a legal holiday celebrated on the first Monday in September.  The celebration of Labor Day is in honor of the working class.  Parades are held throughout the cities and towns of the United States.  Generally, Labor Day is the last day of summer celebrations.  It is a signal to students across the country that school is ready to begin again!", 
			"Halloween is celebrated on October 31st. \"The observances connected with Halloween are thought to have originated among the ancient Druids, who believed that on that evening, Saman, the lord of the dead, called forth hosts of evil spirits.\" (Encarta 96)  In the United States you will find many children dressed in costumes on Halloween.  They walk from door to door collecting candy.  The chant \"trick or treat\" is heard throughout the neighborhood.  There is really no signifcance for most people in the US  associated with Halloween, other than it is fun to dress in costumes, go to parties, play spooky music, and collect candy!",
			"Veterans Day used to be called Armistice Day.  It is a holiday observed every year in the United States to honor all the men and women who served with the U.S. armed forces during the wars.  It is observed either on November 11th or on the fourth Monday of October. Americans display an American Flag outside their homes  Banks, offices, and schools are ususally closed.", 
			"Thanksgiving Day was first celebrated in colonial times in New England.  When the Pilgrims landed their ships at Plymouth Rock in the year 1621, they needed the help of the neighboring Native Americans to learn how to plant crops and grow food.  After they had completed their first harvest, the Pilgrims had a feast with the Indians (Native Americans) to celebrate their friendships. This was called \"The First Thanksgiving\".  Thanksgiving is still celebrated every year on the fourth Thursday of November, usually with a feast of turkey, stuffing, corn, mashed potatoes and other foods.  A favorite side-dish of many families is cranberry sauce and cranberry relish.   Thanksgiving is a time for each person to think of what and who they are thankful for.", 
			"Christmas  is a Christian holiday that celebrates the birth of Christ. There are many traditions associated with Christmas that individual families brought with them when they came to the United States.  Americans bring evergreen trees trimmed with lights and ornaments into their homes. \"The use of a Christmas tree began early in the 17th century, in Strasbourg, France, spreading from there through Germany and then into northern Europe. In 1841 Albert, prince consort of Queen Victoria, introduced the Christmas tree custom to Great Britain; from there it accompanied immigrants to the United States”.(Encarta '96)  Besides the many religious ceremonies and songs celebrated throughout the United States, many American children wait excitedly for Santa Claus to arrive on Christmas Eve and leave presents under the Christmas tree.  Christmas has become known as a time for friendship, giving, and cheer.  Many Americans wish this goodwill could continue throughout the entire year!"
		};
		private readonly DateTime[] SampleItemDate = new DateTime[] {
			new DateTime(DateTime.Now.Year, 1, 1), 
			new DateTime(DateTime.Now.Year, 2, 14), 
			new DateTime(DateTime.Now.Year, 3, 22), 
			new DateTime(DateTime.Now.Year, 5, 31), 
			new DateTime(DateTime.Now.Year, 7, 4), 
			new DateTime(DateTime.Now.Year, 9, 1), 
			new DateTime(DateTime.Now.Year, 10, 31), 
			new DateTime(DateTime.Now.Year, 11, 11), 
			new DateTime(DateTime.Now.Year, 11, 30), 
			new DateTime(DateTime.Now.Year, 12, 25)
		};
		private List<NewsControlSampleDataItem> fData = null;
		public NewsControlSampleData() {
			fData = CreateData();
		}
		protected List<NewsControlSampleDataItem> CreateData() {
			int count = SampleItemHeaderText.Length;
			List<NewsControlSampleDataItem> list = new List<NewsControlSampleDataItem>(count);
			for(int i = 0; i < count; ++i)
				list.Add(new NewsControlSampleDataItem(SampleItemHeaderText[i], SampleItemText[i], SampleItemDate[i]));
			return list;
		}
		public IEnumerator GetEnumerator() {
			return fData.GetEnumerator();
		}
	}
}
