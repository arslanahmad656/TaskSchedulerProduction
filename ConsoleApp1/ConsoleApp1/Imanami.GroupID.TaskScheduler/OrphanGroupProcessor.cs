using Imanami.Common;
using Imanami.Data.ServiceClient;
using Imanami.Foundation;
using Imanami.GroupID.DataTransferObjects.DataContracts;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Filter;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Scheduling;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.User;
using Imanami.GroupID.DataTransferObjects.Enums;
using Imanami.PublicInterfaces.Providers;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Imanami.GroupID.TaskScheduler
{
	internal class OrphanGroupProcessor
	{
		private static ILog logger;

		static OrphanGroupProcessor()
		{
			OrphanGroupProcessor.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public OrphanGroupProcessor()
		{
		}

		private List<IdentityStoreObject> ExcludeSpecialGroups(List<IdentityStoreObject> owners)
		{
			owners.RemoveAll((IdentityStoreObject o) => {
				if (o.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_DistinguishedName()][0].get_Value().Contains("Microsoft Exchange Security Groups") || o.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_DistinguishedName()][0].get_Value().Contains("CN=Microsoft Exchange System Objects") || o.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_DistinguishedName()][0].get_Value().Contains("CN=Builtin") || o.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_DistinguishedName()][0].get_Value().Contains("CN=DnsAdmins"))
				{
					return true;
				}
				return o.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_DistinguishedName()][0].get_Value().Contains("CN=DnsUpdateProxy");
			});
			return owners;
		}

		private Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute GetAdditionalOwnerToPromote(List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> additionalOwners, ILookup<string, User> addOwnersLookup, List<string> supportedObjectTypes)
		{
			MembershipType membershipType;
			Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute;
			MembershipType membershipType1;
			if (additionalOwners.Count == 0)
			{
				return null;
			}
			if (addOwnersLookup == null || supportedObjectTypes.Count == 0)
			{
				foreach (Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute additionalOwner in additionalOwners)
				{
					if (additionalOwner.get_AttributeCollection() == null || !additionalOwner.get_AttributeCollection().ContainsKey("XOwnershipType"))
					{
						attribute = additionalOwner;
						return attribute;
					}
					else
					{
						Enum.TryParse<MembershipType>(additionalOwner.get_AttributeCollection()["XOwnershipType"], out membershipType);
						if (membershipType != 1)
						{
							continue;
						}
						attribute = additionalOwner;
						return attribute;
					}
				}
				return null;
			}
			else
			{
				foreach (Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute additionalOwner1 in additionalOwners)
				{
					User user = addOwnersLookup[additionalOwner1.get_Value()].FirstOrDefault<User>();
					if (user == null || !supportedObjectTypes.Any<string>((string t) => StringUtility.EqualsIgnoreCase(t, user.get_ObjectType())))
					{
						continue;
					}
					if (additionalOwner1.get_AttributeCollection() == null || !additionalOwner1.get_AttributeCollection().ContainsKey("XOwnershipType"))
					{
						attribute = additionalOwner1;
						return attribute;
					}
					else
					{
						Enum.TryParse<MembershipType>(additionalOwner1.get_AttributeCollection()["XOwnershipType"], out membershipType1);
						if (membershipType1 != 1)
						{
							continue;
						}
						attribute = additionalOwner1;
						return attribute;
					}
				}
				return null;
			}
			return attribute;
		}

		private List<IdentityStoreObject> GetOrphanGroups(List<string> attributesToLoad)
		{
			int num = 0;
			List<IdentityStoreObject> identityStoreObjects = new List<IdentityStoreObject>();
			try
			{
				if (attributesToLoad == null)
				{
					attributesToLoad = new List<string>();
				}
				if (!attributesToLoad.Contains("XAdditionalOwner"))
				{
					attributesToLoad.Add("XAdditionalOwner");
				}
				if (!attributesToLoad.Contains(Helper.KnownProviderAttributes.get_DistinguishedName()))
				{
					attributesToLoad.Add(Helper.KnownProviderAttributes.get_DistinguishedName());
				}
				if (!attributesToLoad.Contains(Helper.KnownProviderAttributes.get_Owner()))
				{
					attributesToLoad.Add(Helper.KnownProviderAttributes.get_Owner());
				}
				FilterCriteria filterCriterium = new FilterCriteria();
				filterCriterium.set_Operator("and");
				filterCriterium.set_Child(new List<FilterCriteria>());
				FilterCriteria filterCriterium1 = filterCriterium;
				List<FilterCriteria> child = filterCriterium1.get_Child();
				FilterCriteria filterCriterium2 = new FilterCriteria();
				filterCriterium2.set_Attribute(Helper.KnownProviderAttributes.get_Owner());
				filterCriterium2.set_Operator("not present");
				child.Add(filterCriterium2);
				FilterCriteria filterCriterium3 = new FilterCriteria();
				filterCriterium3.set_Operator("and");
				filterCriterium3.set_Child(new List<FilterCriteria>());
				FilterCriteria filterCriterium4 = filterCriterium3;
				filterCriterium4.get_Child().Add(filterCriterium1);
				List<FilterCriteria> filterCriterias = filterCriterium4.get_Child();
				FilterCriteria filterCriterium5 = new FilterCriteria();
				filterCriterium5.set_Attribute(Helper.KnownProviderAttributes.get_GroupType());
				filterCriterium5.set_Operator("is not");
				filterCriterium5.set_Value(Helper.KnownProviderAttributes.get_GroupTypeLocalSecurity_Builtin());
				filterCriterias.Add(filterCriterium5);
				FilterCriteria filterCriterium6 = new FilterCriteria();
				filterCriterium6.set_Operator("or");
				filterCriterium6.set_Child(new List<FilterCriteria>());
				FilterCriteria filterCriterium7 = filterCriterium6;
				List<FilterCriteria> child1 = filterCriterium7.get_Child();
				FilterCriteria filterCriterium8 = new FilterCriteria();
				filterCriterium8.set_Attribute(Helper.KnownProviderAttributes.get_IsCriticalSystemObject());
				filterCriterium8.set_Operator("is exactly");
				filterCriterium8.set_Value("FALSE");
				filterCriterium8.set_ValueType(5);
				child1.Add(filterCriterium8);
				List<FilterCriteria> filterCriterias1 = filterCriterium7.get_Child();
				FilterCriteria filterCriterium9 = new FilterCriteria();
				filterCriterium9.set_Attribute(Helper.KnownProviderAttributes.get_IsCriticalSystemObject());
				filterCriterium9.set_Operator("not present");
				filterCriterium9.set_Value(string.Empty);
				filterCriterium9.set_ValueType(5);
				filterCriterias1.Add(filterCriterium9);
				filterCriterium4.get_Child().Add(filterCriterium7);
				FilterCriteria filterCriterium10 = new FilterCriteria();
				filterCriterium10.set_Operator("and");
				filterCriterium10.set_Child(new List<FilterCriteria>());
				List<FilterCriteria> child2 = filterCriterium10.get_Child();
				FilterCriteria filterCriterium11 = new FilterCriteria();
				filterCriterium11.set_Attribute("IMGIsExpired");
				filterCriterium11.set_Operator("is exactly");
				filterCriterium11.set_Value("false");
				filterCriterium11.set_ValueType(5);
				child2.Add(filterCriterium11);
				List<FilterCriteria> filterCriterias2 = filterCriterium10.get_Child();
				FilterCriteria filterCriterium12 = new FilterCriteria();
				filterCriterium12.set_Attribute("IMGIsDeleted");
				filterCriterium12.set_Operator("is exactly");
				filterCriterium12.set_Value("false");
				filterCriterium12.set_ValueType(5);
				filterCriterias2.Add(filterCriterium12);
				List<FilterCriteria> child3 = filterCriterium10.get_Child();
				FilterCriteria filterCriterium13 = new FilterCriteria();
				filterCriterium13.set_Attribute("XAdditionalOwner");
				filterCriterium13.set_Operator("present");
				child3.Add(filterCriterium13);
				SearchFilter searchFilter = new SearchFilter(filterCriterium10, filterCriterium4);
				searchFilter.set_MergeCriteriaWithOrOperator(false);
				Dictionary<string, bool> dictionary = null;
				if (Helper.CurrentTask.get_Targets() != null && Helper.CurrentTask.get_Targets().Count > 0)
				{
					dictionary = Helper.CurrentTask.get_Targets().ToDictionary<SchedulingTarget, string, bool>((SchedulingTarget target) => target.get_Target(), (SchedulingTarget target) => false);
				}
				identityStoreObjects = (new ServicesSearchServiceClient(false)).SearchEx(Helper.CurrentTask.get_IdentityStoreId(), 2, ref num, searchFilter, dictionary, string.Empty, 1, -1, 20000, attributesToLoad, false);
				identityStoreObjects = this.ExcludeSpecialGroups(identityStoreObjects);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				OrphanGroupProcessor.logger.Error(1022, 10294, exception.Message, exception, new object[0]);
			}
			return identityStoreObjects;
		}

		public void ProcessJob(TaskScheduling task)
		{
			List<User> users = new List<User>();
			List<IdentityStoreObject> orphanGroups = this.GetOrphanGroups(null);
			IStoreTypeHelper storeTypeHelper = Helper.GetStoreTypeHelper(Helper.CurrentTask.get_IdentityStoreId());
			ILookup<string, User> lookup = null;
			List<string> supportedObjectTypes = new List<string>();
			if (storeTypeHelper != null)
			{
				supportedObjectTypes = storeTypeHelper.GetSupportedObjectTypes(Helper.KnownProviderAttributes.get_Owner());
				if (supportedObjectTypes.Count > 0)
				{
					List<string> strs1 = new List<string>();
					orphanGroups.ForEach((IdentityStoreObject g) => {
						List<string> strs = strs1;
						List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> values = g.get_AttributesBusinessObject().GetValues("XAdditionalOwner");
						Func<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute, string> u003cu003e9_12 = OrphanGroupProcessor.<>c.<>9__1_2;
						if (u003cu003e9_12 == null)
						{
							u003cu003e9_12 = (Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute o) => o.get_Value();
							OrphanGroupProcessor.<>c.<>9__1_2 = u003cu003e9_12;
						}
						strs.AddRange(values.Select<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute, string>(u003cu003e9_12));
					});
					lookup = (new ServicesUserServiceClient(false)).Get(Helper.CurrentTask.get_IdentityStoreId(), strs1, new List<string>()).ToLookup<User, string>((User o) => o.get_ObjectIdFromIdentityStore(), StringComparer.OrdinalIgnoreCase);
				}
			}
			List<IdentityStoreObject> identityStoreObjects = new List<IdentityStoreObject>();
			foreach (IdentityStoreObject orphanGroup in orphanGroups)
			{
				if (orphanGroup.get_AttributesBusinessObject().IsIn(Helper.KnownProviderAttributes.get_Container()))
				{
					orphanGroup.get_AttributesBusinessObject().Remove(Helper.KnownProviderAttributes.get_Container());
				}
				if (orphanGroup.get_AttributesBusinessObject().IsIn(Helper.KnownProviderAttributes.get_DisplayName()))
				{
					orphanGroup.get_AttributesBusinessObject().Remove(Helper.KnownProviderAttributes.get_DisplayName());
				}
				if (orphanGroup.get_AttributesBusinessObject().IsIn(Helper.KnownProviderAttributes.get_DistinguishedName()))
				{
					orphanGroup.get_AttributesBusinessObject().Remove(Helper.KnownProviderAttributes.get_DistinguishedName());
				}
				if (orphanGroup.get_AttributesBusinessObject().IsIn(Helper.KnownProviderAttributes.get_CommonName()))
				{
					orphanGroup.get_AttributesBusinessObject().Remove(Helper.KnownProviderAttributes.get_CommonName());
				}
				Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute additionalOwnerToPromote = this.GetAdditionalOwnerToPromote(orphanGroup.get_AttributesBusinessObject().GetValues("XAdditionalOwner"), lookup, supportedObjectTypes);
				if (additionalOwnerToPromote == null)
				{
					continue;
				}
				User user = (new ServicesUserServiceClient(false)).Get(Helper.CurrentTask.get_IdentityStoreId(), additionalOwnerToPromote.get_Value(), new List<string>()
				{
					Helper.KnownProviderAttributes.get_DistinguishedName(),
					Helper.KnownProviderAttributes.get_EmailAddress(),
					Helper.KnownProviderAttributes.get_DisplayName()
				}, false);
				if (user == null || !user.get_AttributesBusinessObject().HasValue(Helper.KnownProviderAttributes.get_DistinguishedName()))
				{
					continue;
				}
				orphanGroup.set_ObjectName(additionalOwnerToPromote.get_Value());
				users.Add(user);
				user.set_ObjectIdFromIdentityStore(additionalOwnerToPromote.get_Value());
				additionalOwnerToPromote.set_Action(2);
				string value = user.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_DistinguishedName()][0].get_Value();
				if (orphanGroup.get_AttributesBusinessObject().HasValue(Helper.KnownProviderAttributes.get_Owner()))
				{
					orphanGroup.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_Owner()][0].set_Value(value);
					orphanGroup.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_Owner()][0].set_Action(1);
				}
				else if (!orphanGroup.get_AttributesBusinessObject().IsIn(Helper.KnownProviderAttributes.get_Owner()))
				{
					Dictionary<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> attributesCollection = orphanGroup.get_AttributesBusinessObject().get_AttributesCollection();
					string owner = Helper.KnownProviderAttributes.get_Owner();
					List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
					Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
					attribute.set_Action(1);
					attribute.set_Value(value);
					attributes.Add(attribute);
					attributesCollection.Add(owner, attributes);
				}
				else
				{
					Dictionary<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> attributesCollection1 = orphanGroup.get_AttributesBusinessObject().get_AttributesCollection();
					string str = Helper.KnownProviderAttributes.get_Owner();
					List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes1 = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
					Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute1 = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
					attribute1.set_Action(1);
					attribute1.set_Value(value);
					attributes1.Add(attribute1);
					attributesCollection1[str] = attributes1;
				}
				identityStoreObjects.Add(orphanGroup);
			}
			if (identityStoreObjects.Count <= 0)
			{
				return;
			}
			ServicesGroupServiceClient servicesGroupServiceClient = new ServicesGroupServiceClient(false);
			string str1 = DataCompressionHelper.CompressObjects<List<IdentityStoreObject>>(identityStoreObjects);
			if (servicesGroupServiceClient.UpdateManyWithCompression(Helper.CurrentTask.get_IdentityStoreId(), str1, typeof(IdentityStoreObject).FullName).get_Status() == null)
			{
				List<IdentityStoreObject> identityStoreObjects1 = Helper.PrepareCompressedData(identityStoreObjects);
				servicesGroupServiceClient.SendOwnerUpdateNotification(Helper.CurrentTask.get_IdentityStoreId(), identityStoreObjects1, users);
			}
		}
	}
}