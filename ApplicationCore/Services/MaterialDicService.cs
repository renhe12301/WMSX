using System;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities;
using ApplicationCore.Entities.BasicInformation;
using System.Threading.Tasks;
using System.Transactions;
using Ardalis.GuardClauses;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services
{
    public class MaterialDicService:IMaterialDicService
    {
        private readonly IAsyncRepository<MaterialDic> _materialDicRepository;
        private readonly IAsyncRepository<MaterialDicType> _materialDicTypeRepository;
        public MaterialDicService(IAsyncRepository<MaterialDic> materialDicRepository)
        {
            this._materialDicRepository = materialDicRepository;
        }

        public async Task AddMaterialDic(MaterialDic materialDic)
        {
            Guard.Against.Null(materialDic,nameof(materialDic));
           
            Guard.Against.NullOrEmpty(materialDic.MaterialName,
                                     nameof(materialDic.MaterialName));

            if (string.IsNullOrEmpty(materialDic.MaterialCode))
                materialDic.MaterialCode = NPinyin.Pinyin.GetPinyin(materialDic.MaterialCode);
            MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(null, materialDic.MaterialCode,
                null,null);
            var materialDics = await this._materialDicRepository.ListAsync(materialDicSpec);
            if (materialDics.Count > 0) throw new Exception(string.Format("物料编码[{0}],已经存在！",materialDic.MaterialCode));

            await this._materialDicRepository.AddAsync(materialDic);
        }

        public async Task DelMaterialDic(int id)
        {
            Guard.Against.Zero(id, nameof(id));
            MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(id,
                                                       null,null,null);
            MaterialDicTypeSpecification materialDicTypeSpec = new MaterialDicTypeSpecification(id, null, 
                null, null, null);
            var materialDics = await this._materialDicRepository.ListAsync(materialDicSpec);
            var materialDicTypes = await this._materialDicTypeRepository.ListAsync(materialDicTypeSpec);
            Guard.Against.Zero(materialDics.Count, nameof(materialDics));
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    this._materialDicTypeRepository.Delete(materialDicTypes);
                    this._materialDicRepository.Delete(materialDics[0]);
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task UpdateMaterialDic(MaterialDic materialDic)
        {
            Guard.Against.Null(materialDic, nameof(materialDic));
            Guard.Against.Zero(materialDic.Id, nameof(materialDic.Id));
            MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(materialDic.Id,
                                                           null,null,null);
            var materialDics = await this._materialDicRepository.ListAsync(materialDicSpec);
            Guard.Against.Zero(materialDics.Count, nameof(materialDics));
            var updMaterialDic = materialDics[0];
            if (!string.IsNullOrEmpty(materialDic.MaterialName))
                updMaterialDic.MaterialName = materialDic.MaterialName;
            if (!string.IsNullOrEmpty(materialDic.Spec))
                updMaterialDic.Spec = materialDic.Spec;
            if (materialDic.MaterialUnitId.HasValue)
                updMaterialDic.MaterialUnitId = materialDic.MaterialUnitId;
            await this._materialDicRepository.UpdateAsync(updMaterialDic);
        }
    }
}
