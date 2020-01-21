using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Ardalis.GuardClauses;

namespace ApplicationCore.Services
{
    public class TrayDicService:ITrayDicService
    {

        private IAsyncRepository<TrayDic> _trayDicRepository;
        private IAsyncRepository<TrayDicType> _trayDicTypeRepository;

        public TrayDicService(IAsyncRepository<TrayDic> trayDicRepository,
                              IAsyncRepository<TrayDicType> trayDicTypeRepository)
        {
            this._trayDicRepository = trayDicRepository;
            this._trayDicTypeRepository = trayDicTypeRepository;
        }

        public async Task AddTrayDic(TrayDic trayDic)
        {
            Guard.Against.Null(trayDic, nameof(trayDic));
            Guard.Against.NullOrEmpty(trayDic.TrayName,
                                     nameof(trayDic.TrayName));
            if (string.IsNullOrEmpty(trayDic.TrayCode))
                trayDic.TrayCode = NPinyin.Pinyin.GetPinyin(trayDic.TrayName);
            TrayDicSpecification trayDicSpec = new TrayDicSpecification(null, trayDic.TrayCode, null);
            var trays = await this._trayDicRepository.ListAsync(trayDicSpec);
            if (trays.Count > 0) throw new Exception(string.Format("托盘编码[{0}],已经存在！",trayDic.TrayCode));
            await this._trayDicRepository.AddAsync(trayDic);
        }

        public async Task DelTrayDic(List<int> ids)
        {
            Guard.Against.NullOrEmpty(ids, nameof(ids));
            TrayDicSpecification trayDicSpec = new TrayDicSpecification(null,null,null);
            TrayDicTypeSpecification trayDicTypeSpec = new TrayDicTypeSpecification(null, null, null, null);
            var trayDicTypes = await this._trayDicTypeRepository.ListAsync(trayDicTypeSpec);
            var trayDics = await this._trayDicRepository.ListAsync(trayDicSpec);
            Guard.Against.Zero(trayDics.Count, nameof(trayDics));
            List<TrayDicType> delTrayDicTypes=new List<TrayDicType>();
            List<TrayDic> delTrayDics=new List<TrayDic>();
            
            ids.ForEach(id =>
            {
                var trayDic = trayDics.Find(td => td.Id == id);
                delTrayDics.Add(trayDic);
                var trayType = trayDicTypes.Find(td => td.TrayDicId == id);
                delTrayDicTypes.Add(trayType);
            });
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {  
                    this._trayDicTypeRepository.Delete(delTrayDicTypes);
                    this._trayDicRepository.Delete(delTrayDics);
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task UpdateTrayDic(TrayDic trayDic)
        {
            Guard.Against.Null(trayDic, nameof(trayDic));
            Guard.Against.Zero(trayDic.Id, nameof(trayDic.Id));
            TrayDicSpecification trayDicSpec = new TrayDicSpecification(trayDic.Id, null, null);
            var trayDics = await this._trayDicRepository.ListAsync(trayDicSpec);
            Guard.Against.Zero(trayDics.Count, nameof(trayDics.Count));
            var updTrayDic = trayDics[0];
            if (!string.IsNullOrEmpty(trayDic.TrayName))
                updTrayDic.TrayName = trayDic.TrayName;
            await this._trayDicRepository.UpdateAsync(updTrayDic);
        }
    }
}
