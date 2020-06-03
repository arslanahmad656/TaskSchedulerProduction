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
			owners.RemoveAll((IdentityStoreObject o) => (o.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_DistinguishedName()][0].get_Value().Contains("Microsoft Exchange Security Groups") || o.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_DistinguishedName()][0].get_Value().Contains("CN=Microsoft Exchange System Objects") || o.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_DistinguishedName()][0].get_Value().Contains("CN=Builtin") || o.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_DistinguishedName()][0].get_Value().Contains("CN=DnsAdmins") ? true : o.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_DistinguishedName()][0].get_Value().Contains("CN=DnsUpdateProxy")));
			return owners;
		}

		private Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute GetAdditionalOwnerToPromote(List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> additionalOwners, ILookup<string, User> addOwnersLookup, List<string> supportedObjectTypes)
		{
			Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute;
			MembershipType val;
			MembershipType val;
			if (additionalOwners.Count == 0)
			{
				attribute = null;
			}
			else if ((addOwnersLookup == null ? false : supportedObjectTypes.Count != 0))
			{
				foreach (Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute additionalOwner in additionalOwners)
				{
					User user = addOwnersLookup[additionalOwner.get_Value()].FirstOrDefault<User>();
					if ((user == null ? false : supportedObjectTypes.Any<string>((string t) => StringUtility.EqualsIgnoreCase(t, user.get_ObjectType()))))
					{
						if ((additionalOwner.get_AttributeCollection() == null ? false : additionalOwner.get_AttributeCollection().ContainsKey("XOwnershipType")))
						{
							Enum.TryParse<MembershipType>(additionalOwner.get_AttributeCollection()["XOwnershipType"], out val);
							if (val == 1)
							{
								attribute = additionalOwner;
								return attribute;
							}
						}
						else
						{
							attribute = additionalOwner;
							return attribute;
						}
					}
				}
				attribute = null;
			}
			else
			{
				foreach (Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute additionalOwner in additionalOwners)
				{
					if ((additionalOwner.get_AttributeCollection() == null ? false : additionalOwner.get_AttributeCollection().ContainsKey("XOwnershipType")))
					{
						Enum.TryParse<MembershipType>(additionalOwner.get_AttributeCollection()["XOwnershipType"], out val);
						if (val == 1)
						{
							attribute = additionalOwner;
							return attribute;
						}
					}
					else
					{
						attribute = additionalOwner;
						return attribute;
					}
				}
				attribute = null;
			}
			return attribute;
		}

		private List<IdentityStoreObject> GetOrphanGroups(List<string> attributesToLoad)
		{
			int totalFound = 0;
			List<IdentityStoreObject> owners = new List<IdentityStoreObject>();
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
				FilterCriteria filter = filterCriterium;
				List<FilterCriteria> child = filter.get_Child();
				FilterCriteria filterCriterium1 = new FilterCriteria();
				filterCriterium1.set_Attribute(Helper.KnownProviderAttributes.get_Owner());
				filterCriterium1.set_Operator("not present");
				child.Add(filterCriterium1);
				FilterCriteria filterCriterium2 = new FilterCriteria();
				filterCriterium2.set_Operator("and");
				filterCriterium2.set_Child(new List<FilterCriteria>());
				FilterCriteria masterFilter = filterCriterium2;
				masterFilter.get_Child().Add(filter);
				List<FilterCriteria> filterCriterias = masterFilter.get_Child();
				FilterCriteria filterCriterium3 = new FilterCriteria();
				filterCriterium3.set_Attribute(Helper.KnownProviderAttributes.get_GroupType());
				filterCriterium3.set_Operator("is not");
				filterCriterium3.set_Value(Helper.KnownProviderAttributes.get_GroupTypeLocalSecurity_Builtin());
				filterCriterias.Add(filterCriterium3);
				FilterCriteria filterCriterium4 = new FilterCriteria();
				filterCriterium4.set_Operator("or");
				filterCriterium4.set_Child(new List<FilterCriteria>());
				FilterCriteria childCriteria1 = filterCriterium4;
				List<FilterCriteria> child1 = childCriteria1.get_Child();
				FilterCriteria filterCriterium5 = new FilterCriteria();
				filterCriterium5.set_Attribute(Helper.KnownProviderAttributes.get_IsCriticalSystemObject());
				filterCriterium5.set_Operator("is exactly");
				filterCriterium5.set_Value("FALSE");
				filterCriterium5.set_ValueType(5);
				child1.Add(filterCriterium5);
				List<FilterCriteria> filterCriterias1 = childCriteria1.get_Child();
				FilterCriteria filterCriterium6 = new FilterCriteria();
				filterCriterium6.set_Attribute(Helper.KnownProviderAttributes.get_IsCriticalSystemObject());
				filterCriterium6.set_Operator("not present");
				filterCriterium6.set_Value(string.Empty);
				filterCriterium6.set_ValueType(5);
				filterCriterias1.Add(filterCriterium6);
				masterFilter.get_Child().Add(childCriteria1);
				FilterCriteria filterCriterium7 = new FilterCriteria();
				filterCriterium7.set_Operator("and");
				filterCriterium7.set_Child(new List<FilterCriteria>());
				FilterCriteria extDataFilter = filterCriterium7;
				List<FilterCriteria> child2 = extDataFilter.get_Child();
				FilterCriteria filterCriterium8 = new FilterCriteria();
				filterCriterium8.set_Attribute("IMGIsExpired");
				filterCriterium8.set_Operator("is exactly");
				filterCriterium8.set_Value("false");
				filterCriterium8.set_ValueType(5);
				child2.Add(filterCriterium8);
				List<FilterCriteria> filterCriterias2 = extDataFilter.get_Child();
				FilterCriteria filterCriterium9 = new FilterCriteria();
				filterCriterium9.set_Attribute("IMGIsDeleted");
				filterCriterium9.set_Operator("is exactly");
				filterCriterium9.set_Value("false");
				filterCriterium9.set_ValueType(5);
				filterCriterias2.Add(filterCriterium9);
				List<FilterCriteria> child3 = extDataFilter.get_Child();
				FilterCriteria filterCriterium10 = new FilterCriteria();
				filterCriterium10.set_Attribute("XAdditionalOwner");
				filterCriterium10.set_Operator("present");
				child3.Add(filterCriterium10);
				SearchFilter sFilter = new SearchFilter(extDataFilter, masterFilter);
				sFilter.set_MergeCriteriaWithOrOperator(false);
				Dictionary<string, bool> containers = null;
				if ((Helper.CurrentTask.get_Targets() == null ? false : Helper.CurrentTask.get_Targets().Count > 0))
				{
					containers = Helper.CurrentTask.get_Targets().ToDictionary<SchedulingTarget, string, bool>((SchedulingTarget target) => target.get_Target(), (SchedulingTarget target) => false);
				}
				ServicesSearchServiceClient searchServiceClient = new ServicesSearchServiceClient(false);
				owners = searchServiceClient.SearchEx(Helper.CurrentTask.get_IdentityStoreId(), 2, ref totalFound, sFilter, containers, string.Empty, 1, -1, 20000, attributesToLoad, false);
				owners = this.ExcludeSpecialGroups(owners);
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				OrphanGroupProcessor.logger.Error(1022, 10294, ex.Message, ex, new object[0]);
			}
			return owners;
		}

		public void ProcessJob(TaskScheduling task)
		{
			List<User> ownerUsers = new List<User>();
			List<IdentityStoreObject> orphanGrps = this.GetOrphanGroups(null);
			IStoreTypeHelper storeHelper = Helper.GetStoreTypeHelper(Helper.CurrentTask.get_IdentityStoreId());
			ILookup<string, User> addOwnersLookup = null;
			List<string> supportedObjectTypes = new List<string>();
			if (storeHelper != null)
			{
				supportedObjectTypes = storeHelper.GetSupportedObjectTypes(Helper.KnownProviderAttributes.get_Owner());
				if (supportedObjectTypes.Count > 0)
				{
					List<string> strs1 = new List<string>();
					orphanGrps.ForEach((IdentityStoreObject g) => {
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
					ServicesUserServiceClient serviceUser = new ServicesUserServiceClient(false);
					List<User> owners = serviceUser.Get(Helper.CurrentTask.get_IdentityStoreId(), strs1, new List<string>());
					addOwnersLookup = owners.ToLookup<User, string>((User o) => o.get_ObjectIdFromIdentityStore(), StringComparer.OrdinalIgnoreCase);
				}
			}
			List<IdentityStoreObject> orphansList = new List<IdentityStoreObject>();
			foreach (IdentityStoreObject oGrp in orphanGrps)
			{
				if (oGrp.get_AttributesBusinessObject().IsIn(Helper.KnownProviderAttributes.get_Container()))
				{
					oGrp.get_AttributesBusinessObject().Remove(Helper.KnownProviderAttributes.get_Container());
				}
				if (oGrp.get_AttributesBusinessObject().IsIn(Helper.KnownProviderAttributes.get_DisplayName()))
				{
					oGrp.get_AttributesBusinessObject().Remove(Helper.KnownProviderAttributes.get_DisplayName());
				}
				if (oGrp.get_AttributesBusinessObject().IsIn(Helper.KnownProviderAttributes.get_DistinguishedName()))
				{
					oGrp.get_AttributesBusinessObject().Remove(Helper.KnownProviderAttributes.get_DistinguishedName());
				}
				if (oGrp.get_AttributesBusinessObject().IsIn(Helper.KnownProviderAttributes.get_CommonName()))
				{
					oGrp.get_AttributesBusinessObject().Remove(Helper.KnownProviderAttributes.get_CommonName());
				}
				Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute additionalOwner = this.GetAdditionalOwnerToPromote(oGrp.get_AttributesBusinessObject().GetValues("XAdditionalOwner"), addOwnersLookup, supportedObjectTypes);
				if (additionalOwner != null)
				{
					ServicesUserServiceClient serviceUser = new ServicesUserServiceClient(false);
					User additionaOwnerDN = serviceUser.Get(Helper.CurrentTask.get_IdentityStoreId(), additionalOwner.get_Value(), new List<string>()
					{
						Helper.KnownProviderAttributes.get_DistinguishedName(),
						Helper.KnownProviderAttributes.get_EmailAddress(),
						Helper.KnownProviderAttributes.get_DisplayName()
					}, false);
					if ((additionaOwnerDN == null ? false : additionaOwnerDN.get_AttributesBusinessObject().HasValue(Helper.KnownProviderAttributes.get_DistinguishedName())))
					{
						oGrp.set_ObjectName(additionalOwner.get_Value());
						ownerUsers.Add(additionaOwnerDN);
						additionaOwnerDN.set_ObjectIdFromIdentityStore(additionalOwner.get_Value());
						additionalOwner.set_Action(2);
						string dnValue = additionaOwnerDN.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_DistinguishedName()][0].get_Value();
						if (oGrp.get_AttributesBusinessObject().HasValue(Helper.KnownProviderAttributes.get_Owner()))
						{
							oGrp.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_Owner()][0].set_Value(dnValue);
							oGrp.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_Owner()][0].set_Action(1);
						}
						else if (!oGrp.get_AttributesBusinessObject().IsIn(Helper.KnownProviderAttributes.get_Owner()))
						{
							Dictionary<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> attributesCollection = oGrp.get_AttributesBusinessObject().get_AttributesCollection();
							string owner = Helper.KnownProviderAttributes.get_Owner();
							List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
							Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
							attribute.set_Action(1);
							attribute.set_Value(dnValue);
							attributes.Add(attribute);
							attributesCollection.Add(owner, attributes);
						}
						else
						{
							Dictionary<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> attributesCollection1 = oGrp.get_AttributesBusinessObject().get_AttributesCollection();
							string str = Helper.KnownProviderAttributes.get_Owner();
							List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes1 = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
							Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute1 = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
							attribute1.set_Action(1);
							attribute1.set_Value(dnValue);
							attributes1.Add(attribute1);
							attributesCollection1[str] = attributes1;
						}
						orphansList.Add(oGrp);
					}
				}
			}
			if (orphansList.Count > 0)
			{
				ServicesGroupServiceClient groupServiceClient = new ServicesGroupServiceClient(false);
				string cData = DataCompressionHelper.CompressObjects<List<IdentityStoreObject>>(orphansList);
				if (groupServiceClient.UpdateManyWithCompression(Helper.CurrentTask.get_IdentityStoreId(), cData, typeof(IdentityStoreObject).FullName).get_Status() == 0)
				{
					List<IdentityStoreObject> idObjectsList = Helper.PrepareCompressedData(orphansList);
					groupServiceClient.SendOwnerUpdateNotification(Helper.CurrentTask.get_IdentityStoreId(), idObjectsList, ownerUsers);
				}
			}
		}
	}
}