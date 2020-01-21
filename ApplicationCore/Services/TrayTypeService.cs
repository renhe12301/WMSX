using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly ITransactionRepository _transactionRepository;
        public TrayTypeService(IAsyncRepository<TrayType> trayTypeRepository,
                               IAsyncRepository<TrayDicType> trayDicTypeRepository,
                               IAsyncRepository<TrayDicTypeArea> trayDicTypeAreaRepository,
                               ITransactionRepository transactionRepository)
        {
            this._trayTypeRepository = trayTypeRepository;
            this._trayDicTypeRepository = trayDicTypeRepository;
            this._trayDicTypeAreaRepository = trayDicTypeAreaRepository;
            this._transactionRepository = transactionRepository;
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
            this._transactionRepository.Transaction(() =>
            {
                trayDicIds.ForEach(async (mId) =>
                {
                    TrayDicType trayDicType = new TrayDicType
                    {
                        TrayDicId = mId,
                        TrayTypeId = typeId
                    };
                    await this._trayDicTypeRepository.AddAsync(trayDicType);
                });
            });
        }

        public async Task DelTrayType(int id)
        {
            Guard.Against.Zero(id, nameof(id));
            TrayTypeSpecification tTypeSpec = new TrayTypeSpecification(id, null, null);
            var trayTypes = await this._trayTypeRepository.ListAsync(tTypeSpec);
            Guard.Against.Zero(trayTypes.Count, nameof(trayTypes.Count));
            List<int> delIds = new List<int> { id };
            await this._Del(id, delIds);
            this._transactionRepository.Transaction(() =>
            {
                delIds.ForEach(async (delId) =>
                {
                    TrayTypeSpecification delTypeSpec = new TrayTypeSpecification(delId, null, null);
                    var delTrayTypes = await this._trayTypeRepository.ListAsync(delTypeSpec);
                    if (delTrayTypes.Count > 0)
                    {
                        await this._trayTypeRepository.DeleteAsync(delTrayTypes[0]);
                        TrayDicTypeSpecification trayDicTypeSpec = new TrayDicTypeSpecification(null, id, null, null);
                        var dicTypes = await this._trayDicTypeRepository.ListAsync(trayDicTypeSpec);
                        await this._trayDicTypeRepository.DeleteAsync(dicTypes);
                        TrayDicTypeAreaSpecification trayDicTypeAreaSpec = new TrayDicTypeAreaSpecification(id, null, null);
                        var dicAreaTypes = await this._trayDicTypeAreaRepository.ListAsync(trayDicTypeAreaSpec);
                        await this._trayDicTypeAreaRepository.DeleteAsync(dicAreaTypes);
                    }
                });
            });
        }

        private async Task _Del(int cur, List<int> delIds)
        {
            TrayTypeSpecification mTypeSpec = new TrayTypeSpecification(null, cur, null);
            var trayTypes = await this._trayTypeRepository.ListAsync(mTypeSpec);
            trayTypes.ForEach(async (c) =>
            {
                await _Del(c.Id, delIds);
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
