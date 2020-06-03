using Imanami.Foundation;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services;
using Imanami.GroupID.DataTransferObjects.Enums;
using log4net.Attributes;
using PostSharp.Aspects;
using PostSharp.Aspects.Internals;
using PostSharp.ImplementationDetails_bda91a0d;
using PostSharp.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
			Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = null;
			Arguments<string, AttributeCollection> argument = new Arguments<string, AttributeCollection>()
			{
				Arg0 = attributeName,
				Arg1 = attributes
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886340108287L),
				Method = <>z__a_1._3
			};
			<>z__a_1.a1.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (attributes == null || attributes.get_AttributesCollection() == null)
						{
							attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
						}
						else if (attributes.IsIn(attributeName))
						{
							List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> item = attributes.get_AttributesCollection()[attributeName];
							attribute = (item.Count == 0 ? new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute() : item[0]);
						}
						else
						{
							attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_1.a1.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								attribute = (Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute)methodExecutionArg.ReturnValue;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					methodExecutionArg.ReturnValue = attribute;
					<>z__a_1.a1.OnExit(methodExecutionArg);
					attribute = (Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				attribute = (Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute)methodExecutionArg.ReturnValue;
			}
			return attribute;
		}

		public static List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> GetAttributes(string attributeName, AttributeCollection attributes)
		{
			List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> returnValue = null;
			Arguments<string, AttributeCollection> argument = new Arguments<string, AttributeCollection>()
			{
				Arg0 = attributeName,
				Arg1 = attributes
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886340108288L),
				Method = <>z__a_1._1
			};
			<>z__a_1.a0.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (attributes == null || attributes.get_AttributesCollection() == null)
						{
							returnValue = null;
						}
						else
						{
							List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> item = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
							if (attributes.IsIn(attributeName))
							{
								item = attributes.get_AttributesCollection()[attributeName];
								returnValue = item;
							}
							else
							{
								returnValue = item;
							}
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_1.a0.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								returnValue = (List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>)methodExecutionArg.ReturnValue;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					methodExecutionArg.ReturnValue = returnValue;
					<>z__a_1.a0.OnExit(methodExecutionArg);
					returnValue = (List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public static List<string> GetAttributeValues(string attributeName, AttributeCollection attributes)
		{
			List<string> returnValue = null;
			Arguments<string, AttributeCollection> argument = new Arguments<string, AttributeCollection>()
			{
				Arg0 = attributeName,
				Arg1 = attributes
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886340108286L),
				Method = <>z__a_1._5
			};
			<>z__a_1.a2.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						List<string> strs = new List<string>();
						List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes1 = AttributesHelper.GetAttributes(attributeName, attributes);
						if (attributes1 != null)
						{
							foreach (Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute in attributes1)
							{
								if (StringUtility.IsBlank(attribute.get_Value()))
								{
									continue;
								}
								strs.Add(attribute.get_Value());
							}
							returnValue = strs;
						}
						else
						{
							returnValue = strs;
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_1.a2.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								returnValue = (List<string>)methodExecutionArg.ReturnValue;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					methodExecutionArg.ReturnValue = returnValue;
					<>z__a_1.a2.OnExit(methodExecutionArg);
					returnValue = (List<string>)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (List<string>)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public static DateTime GetValueAsDatetime(List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> values)
		{
			DateTime dateTime;
			DateTime minValue = new DateTime();
			Arguments<List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> argument = new Arguments<List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>>()
			{
				Arg0 = values
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886340108283L),
				Method = <>z__a_1._b
			};
			<>z__a_1.a5.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (values.Count != 0)
						{
							minValue = (!DateTime.TryParse(values[0].get_Value(), out dateTime) ? DateTime.MinValue : dateTime);
						}
						else
						{
							minValue = DateTime.MinValue;
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_1.a5.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								minValue = (DateTime)methodExecutionArg.ReturnValue;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					methodExecutionArg.ReturnValue = minValue;
					<>z__a_1.a5.OnExit(methodExecutionArg);
					minValue = (DateTime)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				minValue = (DateTime)methodExecutionArg.ReturnValue;
			}
			return minValue;
		}

		public static string GetValueAsFormattedDatetime(string attributeName, AttributeCollection attributes)
		{
			string valueAsFormattedDatetime = null;
			Arguments<string, AttributeCollection> argument = new Arguments<string, AttributeCollection>()
			{
				Arg0 = attributeName,
				Arg1 = attributes
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886340108282L),
				Method = <>z__a_1._d
			};
			<>z__a_1.a6.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						valueAsFormattedDatetime = AttributesHelper.GetValueAsFormattedDatetime(AttributesHelper.GetAttributes(attributeName, attributes));
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_1.a6.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								valueAsFormattedDatetime = (string)methodExecutionArg.ReturnValue;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					methodExecutionArg.ReturnValue = valueAsFormattedDatetime;
					<>z__a_1.a6.OnExit(methodExecutionArg);
					valueAsFormattedDatetime = (string)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				valueAsFormattedDatetime = (string)methodExecutionArg.ReturnValue;
			}
			return valueAsFormattedDatetime;
		}

		public static string GetValueAsFormattedDatetime(List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> values)
		{
			DateTime dateTime;
			string empty = null;
			Arguments<List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> argument = new Arguments<List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>>()
			{
				Arg0 = values
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886340108281L),
				Method = <>z__a_1._f
			};
			<>z__a_1.a7.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (values.Count == 0)
						{
							empty = string.Empty;
						}
						else if (!DateTime.TryParse(values[0].get_Value(), out dateTime))
						{
							empty = (!DateTime.TryParseExact(Regex.Replace(values[0].get_Value(), "[A-Za-z ]", ""), "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime) ? string.Empty : dateTime.ToString("yyyy MMMM dd HH:mm:ss"));
						}
						else
						{
							empty = dateTime.ToString("yyyy MMMM dd HH:mm:ss");
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_1.a7.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								empty = (string)methodExecutionArg.ReturnValue;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					methodExecutionArg.ReturnValue = empty;
					<>z__a_1.a7.OnExit(methodExecutionArg);
					empty = (string)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				empty = (string)methodExecutionArg.ReturnValue;
			}
			return empty;
		}

		public static string GetValueAsString(List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> values)
		{
			string returnValue = null;
			Arguments<List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> argument = new Arguments<List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>>()
			{
				Arg0 = values
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886340108285L),
				Method = <>z__a_1._7
			};
			<>z__a_1.a3.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						returnValue = (values.Count != 0 ? values[0].get_Value() ?? string.Empty : string.Empty);
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_1.a3.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								returnValue = (string)methodExecutionArg.ReturnValue;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					methodExecutionArg.ReturnValue = returnValue;
					<>z__a_1.a3.OnExit(methodExecutionArg);
					returnValue = (string)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (string)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public static string GetValueAsString(string attributeName, AttributeCollection collection, string defaultValue = "")
		{
			string returnValue = null;
			Arguments<string, AttributeCollection, string> argument = new Arguments<string, AttributeCollection, string>()
			{
				Arg0 = attributeName,
				Arg1 = collection,
				Arg2 = defaultValue
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886340108284L),
				Method = <>z__a_1._9
			};
			<>z__a_1.a4.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						string value = defaultValue;
						string str = collection.get_AttributesCollection().Keys.FirstOrDefault<string>((string f) => f.Equals(attributeName, StringComparison.OrdinalIgnoreCase));
						if (!string.IsNullOrEmpty(str) && collection.get_AttributesCollection()[str].Count > 0)
						{
							value = collection.get_AttributesCollection()[str][0].get_Value();
						}
						returnValue = value;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_1.a4.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								returnValue = (string)methodExecutionArg.ReturnValue;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					methodExecutionArg.ReturnValue = returnValue;
					<>z__a_1.a4.OnExit(methodExecutionArg);
					returnValue = (string)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (string)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public static bool SetAttributeValue(string attributeName, string value, AttributeCollection attributes, AttributeActionType action = 3)
		{
			bool returnValue = false;
			Arguments<string, string, AttributeCollection, AttributeActionType> argument = new Arguments<string, string, AttributeCollection, AttributeActionType>()
			{
				Arg0 = attributeName,
				Arg1 = value,
				Arg2 = attributes,
				Arg3 = action
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886340108280L),
				Method = <>z__a_1._11
			};
			<>z__a_1.a8.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (attributes != null)
						{
							if (!attributes.HasValue(attributeName))
							{
								Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
								attribute.set_Value(value);
								attribute.set_Action(action);
								Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute1 = attribute;
								if (!attributes.IsIn(attributeName))
								{
									attributes.Add(attributeName, new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>()
									{
										attribute1
									});
								}
								else
								{
									attributes.get_AttributesCollection()[attributeName] = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>()
									{
										attribute1
									};
								}
							}
							else
							{
								AttributesHelper.GetAttribute(attributeName, attributes).set_Value(value);
							}
							returnValue = true;
						}
						else
						{
							returnValue = false;
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_1.a8.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								returnValue = (bool)methodExecutionArg.ReturnValue;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					methodExecutionArg.ReturnValue = returnValue;
					<>z__a_1.a8.OnExit(methodExecutionArg);
					returnValue = (bool)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (bool)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public static bool SetAttributeValue(string attributeName, List<string> values, AttributeCollection attributes)
		{
			bool returnValue = false;
			Arguments<string, List<string>, AttributeCollection> argument = new Arguments<string, List<string>, AttributeCollection>()
			{
				Arg0 = attributeName,
				Arg1 = values,
				Arg2 = attributes
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886340108279L),
				Method = <>z__a_1._13
			};
			<>z__a_1.a9.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (attributes != null)
						{
							List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes1 = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
							foreach (string value in values)
							{
								Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
								attribute.set_Value(value);
								attributes1.Add(attribute);
							}
							if (!attributes.IsIn(attributeName))
							{
								attributes.Add(attributeName, attributes1);
							}
							else
							{
								attributes.get_AttributesCollection()[attributeName] = attributes1;
							}
							returnValue = true;
						}
						else
						{
							returnValue = false;
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_1.a9.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								returnValue = (bool)methodExecutionArg.ReturnValue;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					methodExecutionArg.ReturnValue = returnValue;
					<>z__a_1.a9.OnExit(methodExecutionArg);
					returnValue = (bool)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (bool)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public static bool SetAttributeValue(string attributeName, IList values, AttributeCollection attributes)
		{
			bool returnValue = false;
			Arguments<string, IList, AttributeCollection> argument = new Arguments<string, IList, AttributeCollection>()
			{
				Arg0 = attributeName,
				Arg1 = values,
				Arg2 = attributes
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886340108278L),
				Method = <>z__a_1._15
			};
			<>z__a_1.a10.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (attributes != null)
						{
							List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes1 = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
							if (!(values is List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>))
							{
								foreach (object value in values)
								{
									if (value == null)
									{
										value = string.Empty;
									}
									string str = value.ToString();
									Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
									attribute.set_Value(str);
									attributes1.Add(attribute);
								}
							}
							else
							{
								attributes1 = values as List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>;
							}
							if (!attributes.IsIn(attributeName))
							{
								attributes.Add(attributeName, attributes1);
							}
							else
							{
								attributes.get_AttributesCollection()[attributeName] = attributes1;
							}
							returnValue = true;
						}
						else
						{
							returnValue = false;
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_1.a10.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								returnValue = (bool)methodExecutionArg.ReturnValue;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					methodExecutionArg.ReturnValue = returnValue;
					<>z__a_1.a10.OnExit(methodExecutionArg);
					returnValue = (bool)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (bool)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}
	}
}