using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services
{
    public class TrayTypeService:ITrayTypeService
    {
        private readonly IAsyncRepository<TrayType> _trayTypeRepository;
        private readonly IAsyncRepository<TrayDicType> _trayDicTypeRepository;
        private readonly IAsyncRepository<TrayDicTypeArea> _trayDicTypeAreaRepository;
        public TrayTypeService(IAsyncRepository<TrayType> trayTypeRepository,
                               IAsyncRepository<TrayDicType> trayDicTypeRepository,
                               IAsyncRepository<TrayDicTypeArea> trayDicTypeAreaRepository)
        {
            this._trayTypeRepository = trayTypeRepository;
            this._trayDicTypeRepository = trayDicTypeRepository;
            this._trayDicTypeAreaRepository = trayDicTypeAreaRepository;
        }

        public async Task AddTrayType(TrayType trayType)
        {
            Guard.Against.Null(trayType, nameof(trayType));
            await this._trayTypeRepository.AddAsync(trayType);
        }

        public async Task AssignTrayDic(int typeId, List<int> trayDicIds)
        {
            Guard.Against.Zero(typeId, nameof(typeId));
            Guard.Against.Null(trayDicIds, nameof(trayDicIds));
            Guard.Against.Zero(trayDicIds.Count, nameof(trayDicIds.Count));
            List<TrayDicType> trayDicTypes=new List<TrayDicType>();
            trayDicIds.ForEach(async (mId) =>
            {
                TrayDicType trayDicType = new TrayDicType
                {
                    TrayDicId = mId,
                    TrayTypeId = typeId
                };
                trayDicTypes.Add(trayDicType);
            });
            await this._trayDicTypeRepository.AddAsync(trayDicTypes);
        }

        public async Task DelTrayType(int id)
        {
            Guard.Against.Zero(id, nameof(id));
            TrayTypeSpecification tTypeSpec = new TrayTypeSpecification(null, null, null);
            var trayTypes = await this._trayTypeRepository.ListAsync(tTypeSpec);
            TrayDicTypeSpecification trayDicTypeSpec =
                new TrayDicTypeSpecification(null, null, null, null);
            var dicTypes = await this._trayDicTypeRepository.ListAsync(trayDicTypeSpec);
            TrayDicTypeAreaSpecification trayDicTypeAreaSpec =
                new TrayDicTypeAreaSpecification(null, null, null);
            var dicAreaTypes = await this._trayDicTypeAreaRepository.ListAsync(trayDicTypeAreaSpec);
            Guard.Against.Zero(trayTypes.Count, nameof(trayTypes.Count));
            List<int> delIds = new List<int> {id};
            await this._Del(id, delIds,trayTypes);
            List<TrayType> delTrayTypes=new List<TrayType>();
            List<TrayDicType> delTrayDicTypes=new List<TrayDicType>();
            List<TrayDicTypeArea> delTrayDicAreas=new List<TrayDicTypeArea>();
            delIds.ForEach(async (delId) =>
            {
                var trayType = trayTypes.Find(t => t.Id == delId);
                delTrayTypes.Add(trayType);
                delTrayDicTypes.AddRange(dicTypes.FindAll(t=>t.TrayTypeId==delId));
                delTrayDicAreas.AddRange(dicAreaTypes.FindAll(t=>t.TrayTypeId==delId));
            });
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                     this._trayTypeRepository.Delete(delTrayTypes);
                       
                     this._trayDicTypeRepository.Delete(delTrayDicTypes);
                      
                     this._trayDicTypeAreaRepository.Delete(dicAreaTypes);
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        private async Task _Del(int cur, List<int> delIds,List<TrayType> allTypes)
        {
            var trayTypes = allTypes.FindAll(t=>t.ParentId==cur);
            trayTypes.ForEach(async (c) =>
            {
                await _Del(c.Id, delIds,allTypes);
            });

        }

        public async Task UpdateTrayType(TrayType trayType)
        {
            Guard.Against.Null(trayType, nameof(trayType));
            Guard.Against.Zero(trayType.Id, nameof(trayType.Id));
            TrayTypeSpecification tTypeSpec = new TrayTypeSpecification(trayType.Id, null, null);
            var trayTypes = await this._trayTypeRepository.ListAsync(tTypeSpec);
            Guard.Against.Zero(trayTypes.Count, nameof(trayTypes.Count));
            var updTrayType = trayTypes[0];
            if (updTrayType.ParentId > 0)
                updTrayType.ParentId = trayType.ParentId;
            updTrayType.TypeName = trayType.TypeName;
            await this._trayTypeRepository.UpdateAsync(updTrayType);
        }
    }
}
