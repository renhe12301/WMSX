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
        public MaterialDicService(IAsyncRepository<MaterialDic> materialDicRepository)
        {
            this._materialDicRepository = materialDicRepository;
        }

        public async Task AddMaterialDic(MaterialDic materialDic)
        {
            Guard.Against.Null(materialDic, nameof(materialDic));
            Guard.Against.NullOrEmpty(materialDic.MaterialName,
                nameof(materialDic.MaterialName));
            if (string.IsNullOrEmpty(materialDic.MaterialCode))
                materialDic.MaterialCode = NPinyin.Pinyin.GetPinyin(materialDic.MaterialCode);
            MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(null, materialDic.MaterialCode,
                null,null, null);
            var materialDics = await this._materialDicRepository.ListAsync(materialDicSpec);
            if (materialDics.Count == 0)
                await this._materialDicRepository.AddAsync(materialDic);
        }

        public async Task DelMaterialDic(int id)
        {
            Guard.Against.Zero(id, nameof(id));
            MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(id,
                null, null, null,null);
            var materialDics = await this._materialDicRepository.ListAsync(materialDicSpec);
            if (materialDics.Count > 0)
                this._materialDicRepository.Delete(materialDics[0]);
        }

        public async Task UpdateMaterialDic(MaterialDic materialDic)
        {
            Guard.Against.Null(materialDic, nameof(materialDic));
            Guard.Against.Zero(materialDic.Id, nameof(materialDic.Id));
            MaterialDicSpecification materialDicSpec = new MaterialDicSpecification(materialDic.Id,
                                                           null,null,null,null);
            var materialDics = await this._materialDicRepository.ListAsync(materialDicSpec);
            if (materialDics.Count > 0)
            {
                var updMaterialDic = materialDics[0];
                updMaterialDic.MaterialName = materialDic.MaterialName;
                updMaterialDic.Spec = materialDic.Spec;
                updMaterialDic.Brand = materialDic.Brand;
                updMaterialDic.Img = materialDic.Img;
                updMaterialDic.Memo = materialDic.Memo;
                updMaterialDic.Status = materialDic.Status;
                updMaterialDic.Unit = materialDic.Unit;
                updMaterialDic.CreateTime = materialDic.CreateTime;
                updMaterialDic.EndTime = materialDic.EndTime;
                updMaterialDic.IsStock = materialDic.IsStock;
                updMaterialDic.MaterialCode = materialDic.MaterialCode;
                updMaterialDic.MaterialTypeId = materialDic.MaterialTypeId;
                updMaterialDic.WarehouseId = materialDic.WarehouseId;
                await this._materialDicRepository.UpdateAsync(updMaterialDic);
            }
        }
    }
}
