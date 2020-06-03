using Imanami.Foundation;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services;
using Imanami.GroupID.DataTransferObjects.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Imanami.GroupID.TaskScheduler
{
	public class AttributesHelper
	{
		public AttributesHelper()
		{
		}

		public static Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute GetAttribute(string attributeName, AttributeCollection attributes)
		{
			Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute;
			if ((attributes == null ? true : attributes.get_AttributesCollection() == null))
			{
				attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
			}
			else if (attributes.IsIn(attributeName))
			{
				List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> values = attributes.get_AttributesCollection()[attributeName];
				attribute = (values.Count == 0 ? new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute() : values[0]);
			}
			else
			{
				attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
			}
			return attribute;
		}

		public static List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> GetAttributes(string attributeName, AttributeCollection attributes)
		{
			List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes1;
			if ((attributes == null ? false : attributes.get_AttributesCollection() != null))
			{
				List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> values = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
				if (attributes.IsIn(attributeName))
				{
					values = attributes.get_AttributesCollection()[attributeName];
					attributes1 = values;
				}
				else
				{
					attributes1 = values;
				}
			}
			else
			{
				attributes1 = null;
			}
			return attributes1;
		}

		public static List<string> GetAttributeValues(string attributeName, AttributeCollection attributes)
		{
			List<string> strs;
			List<string> values = new List<string>();
			List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributesList = AttributesHelper.GetAttributes(attributeName, attributes);
			if (attributesList != null)
			{
				foreach (Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute in attributesList)
				{
					if (!StringUtility.IsBlank(attribute.get_Value()))
					{
						values.Add(attribute.get_Value());
					}
				}
				strs = values;
			}
			else
			{
				strs = values;
			}
			return strs;
		}

		public static DateTime GetValueAsDatetime(List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> values)
		{
			DateTime date;
			DateTime minValue;
			if (values.Count != 0)
			{
				minValue = (!DateTime.TryParse(values[0].get_Value(), out date) ? DateTime.MinValue : date);
			}
			else
			{
				minValue = DateTime.MinValue;
			}
			return minValue;
		}

		public static string GetValueAsFormattedDatetime(string attributeName, AttributeCollection attributes)
		{
			return AttributesHelper.GetValueAsFormattedDatetime(AttributesHelper.GetAttributes(attributeName, attributes));
		}

		public static string GetValueAsFormattedDatetime(List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> values)
		{
			DateTime date;
			string empty;
			if (values.Count == 0)
			{
				empty = string.Empty;
			}
			else if (!DateTime.TryParse(values[0].get_Value(), out date))
			{
				empty = (!DateTime.TryParseExact(Regex.Replace(values[0].get_Value(), "[A-Za-z ]", ""), "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out date) ? string.Empty : date.ToString("yyyy MMMM dd HH:mm:ss"));
			}
			else
			{
				empty = date.ToString("yyyy MMMM dd HH:mm:ss");
			}
			return empty;
		}

		public static string GetValueAsString(List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> values)
		{
			string str;
			str = (values.Count != 0 ? values[0].get_Value() ?? string.Empty : string.Empty);
			return str;
		}

		public static string GetValueAsString(string attributeName, AttributeCollection collection, string defaultValue = "")
		{
			string retValue = defaultValue;
			string key = collection.get_AttributesCollection().Keys.FirstOrDefault<string>((string f) => f.Equals(attributeName, StringComparison.OrdinalIgnoreCase));
			if (!string.IsNullOrEmpty(key))
			{
				if (collection.get_AttributesCollection()[key].Count > 0)
				{
					retValue = collection.get_AttributesCollection()[key][0].get_Value();
				}
			}
			return retValue;
		}

		public static bool SetAttributeValue(string attributeName, string value, AttributeCollection attributes, AttributeActionType action = 3)
		{
			bool flag;
			if (attributes != null)
			{
				if (!attributes.HasValue(attributeName))
				{
					Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute1 = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
					attribute1.set_Value(value);
					attribute1.set_Action(action);
					Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = attribute1;
					if (!attributes.IsIn(attributeName))
					{
						attributes.Add(attributeName, new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>()
						{
							attribute
						});
					}
					else
					{
						attributes.get_AttributesCollection()[attributeName] = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>()
						{
							attribute
						};
					}
				}
				else
				{
					AttributesHelper.GetAttribute(attributeName, attributes).set_Value(value);
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public static bool SetAttributeValue(string attributeName, List<string> values, AttributeCollection attributes)
		{
			bool flag;
			if (attributes != null)
			{
				List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributesToUpdate = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
				foreach (string value in values)
				{
					Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
					attribute.set_Value(value);
					attributesToUpdate.Add(attribute);
				}
				if (!attributes.IsIn(attributeName))
				{
					attributes.Add(attributeName, attributesToUpdate);
				}
				else
				{
					attributes.get_AttributesCollection()[attributeName] = attributesToUpdate;
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public static bool SetAttributeValue(string attributeName, IList values, AttributeCollection attributes)
		{
			bool flag;
			if (attributes != null)
			{
				List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributesToUpdate = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
				if (!(values is List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>))
				{
					foreach (object obj in values)
					{
						object empty = obj;
						if (empty == null)
						{
							empty = string.Empty;
						}
						string value = empty.ToString();
						Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
						attribute.set_Value(value);
						attributesToUpdate.Add(attribute);
					}
				}
				else
				{
					attributesToUpdate = values as List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>;
				}
				if (!attributes.IsIn(attributeName))
				{
					attributes.Add(attributeName, attributesToUpdate);
				}
				else
				{
					attributes.get_AttributesCollection()[attributeName] = attributesToUpdate;
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}
	}
}