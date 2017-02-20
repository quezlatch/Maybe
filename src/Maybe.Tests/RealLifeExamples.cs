using System;
using System.Collections.Generic;
using Xunit;

namespace Hazzik.Maybe.Tests
{
	public class RealLifeExamples
	{
		private static readonly string DefaultRobotsTxt = @"User-agent: *"
												  + Environment.NewLine +
												  "Disallow: /sitecore";
		private Item _item;

		[Fact]
		public void NoItem()
		{
			_item = null;
			Assert.Equal(DefaultRobotsTxt, GetResponse());
		}

		[Fact]
		public void NoFieldValue()
		{
			SetFieldValue(null);
			Assert.Equal(DefaultRobotsTxt, GetResponse());
		}


		[Fact]
		public void EmptyFieldValue()
		{
			SetFieldValue("");
			Assert.Equal(DefaultRobotsTxt, GetResponse());
		}

		[Fact]
		public void FieldHasValue()
		{
			SetFieldValue("aaa");
			Assert.Equal("aaa", GetResponse());
		}

		[Fact]
		public void FieldHasDefaultValue()
		{
			SetFieldValue("default");
			Assert.Equal(DefaultRobotsTxt, GetResponse());
		}

		void SetFieldValue(string p)
		{
			_item = new Item { Fields = new Dictionary<string, string> { { "Robots Txt", p } } };
		}

		public string GetResponse()
		{
			return GetRobotsText() ?? DefaultRobotsTxt;
		}

		private string GetRobotsText()
		{
			var item = GetSettingsItem();
			if (item != null)
			{
				var field = item.Fields["Robots Txt"];
				if (field != null)
				{
					var robotsText = field;
					if (!string.IsNullOrWhiteSpace(robotsText))
					{
						if (robotsText != "default")
						{
							return robotsText;
						}
					}
				}
			}
			return null;
		}

		public string GetResponseMaybe()
		{
			var maybe = from item in GetSettingsItem().ToMaybe()
						from field in item.Fields["Robots Txt"].ToNullOrWhiteSpaceMaybe()
						where field != "default"
						select field;
			return maybe.GetValueOrDefault(DefaultRobotsTxt);
		}

		private Item GetSettingsItem() => _item;

		private class Item
		{
			public IDictionary<string, string> Fields { get; set; }
		}
	}
}
